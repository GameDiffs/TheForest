using System;
using System.Collections.Generic;
using UnityEngine;

public class mutantRagdollFollow : MonoBehaviour
{
	private float startDelay;

	private float actualDelay;

	private float actualSmoothBlendTime;

	private float dropTime;

	public bool dummyDropped;

	public bool dummy;

	public bool disableForMp;

	public Transform[] ragdollJoints;

	public Transform[] targetJoints;

	public bool doMatch;

	private int length;

	private float blendSpeed;

	private List<Quaternion> storeRot = new List<Quaternion>();

	private List<Quaternion> finalRot = new List<Quaternion>();

	private List<Quaternion> initRotations = new List<Quaternion>();

	private mutantRagdollSetup mrs;

	private bool doneStoreJoints;

	private void Start()
	{
		if (BoltNetwork.isClient)
		{
			this.mrs = base.transform.GetComponentInParent<mutantRagdollSetup>();
			this.startDelay = 1f;
			this.blendSpeed = 7f;
		}
		else
		{
			this.startDelay = 0.6f;
			this.blendSpeed = 6f;
		}
		for (int i = 0; i < this.ragdollJoints.Length; i++)
		{
			this.initRotations.Add(this.ragdollJoints[i].localRotation);
		}
	}

	private void OnEnable()
	{
		if (this.initRotations.Count > 0)
		{
			for (int i = 0; i < this.ragdollJoints.Length; i++)
			{
				this.ragdollJoints[i].localRotation = this.initRotations[i];
			}
		}
		this.disableForMp = false;
		this.actualDelay = Time.time + this.startDelay;
	}

	private void OnDisable()
	{
		this.resetRagDollParams();
	}

	public void resetRagDollParams()
	{
		this.disableForMp = true;
		this.doMatch = false;
		this.dummyDropped = false;
		this.storeRot.Clear();
		if (this.initRotations.Count > 0)
		{
			for (int i = 0; i < this.ragdollJoints.Length; i++)
			{
				this.ragdollJoints[i].localRotation = this.initRotations[i];
			}
		}
	}

	public void setDropped()
	{
		if (this.initRotations.Count > 0)
		{
			for (int i = 0; i < this.ragdollJoints.Length; i++)
			{
				this.ragdollJoints[i].localRotation = this.initRotations[i];
			}
		}
		this.actualDelay = Time.time + this.startDelay;
		this.dropTime = Time.time + 6f;
		this.dummyDropped = true;
		this.disableForMp = false;
	}

	private void LateUpdate()
	{
		if (this.disableForMp)
		{
			return;
		}
		if (this.dummy)
		{
			if (this.dummyDropped && Time.time < this.dropTime)
			{
				if (!this.doMatch)
				{
					if (Time.time < this.actualDelay)
					{
						return;
					}
					for (int i = 0; i < this.ragdollJoints.Length; i++)
					{
						this.ragdollJoints[i].rotation = this.targetJoints[i].rotation;
						this.storeRot.Add(this.targetJoints[i].rotation);
					}
					this.doMatch = true;
				}
				for (int j = 0; j < this.ragdollJoints.Length; j++)
				{
					this.storeRot[j] = Quaternion.Lerp(this.storeRot[j], this.ragdollJoints[j].rotation, Time.deltaTime * this.blendSpeed);
					this.targetJoints[j].rotation = this.storeRot[j];
				}
			}
			else
			{
				this.dummyDropped = false;
			}
			return;
		}
		if (Time.time < this.actualDelay)
		{
			return;
		}
		if (!this.doMatch)
		{
			for (int k = 0; k < this.ragdollJoints.Length; k++)
			{
				this.ragdollJoints[k].rotation = this.targetJoints[k].rotation;
				this.storeRot.Add(this.targetJoints[k].rotation);
				this.actualSmoothBlendTime = Time.time + 2.3f;
				this.finalRot.Add(this.targetJoints[k].rotation);
			}
			this.doMatch = true;
		}
		if (Time.time < this.actualSmoothBlendTime)
		{
			for (int l = 0; l < this.ragdollJoints.Length; l++)
			{
				this.storeRot[l] = Quaternion.Lerp(this.storeRot[l], this.ragdollJoints[l].rotation, Time.deltaTime * this.blendSpeed);
				this.targetJoints[l].rotation = this.storeRot[l];
				this.finalRot[l] = this.targetJoints[l].rotation;
			}
		}
		else
		{
			for (int m = 0; m < this.ragdollJoints.Length; m++)
			{
				if (BoltNetwork.isClient && !this.doneStoreJoints)
				{
					this.mrs.StartCoroutine("generateStoredJointList");
					this.doneStoreJoints = true;
				}
				this.storeRot[m] = Quaternion.Lerp(this.storeRot[m], this.finalRot[m], Time.deltaTime * this.blendSpeed);
				this.targetJoints[m].rotation = this.storeRot[m];
			}
		}
	}
}
