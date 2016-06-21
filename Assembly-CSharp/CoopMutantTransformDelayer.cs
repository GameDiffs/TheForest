using Bolt;
using System;
using UnityEngine;

public class CoopMutantTransformDelayer : EntityBehaviour<IMutantState>
{
	private float timer;

	private GameObject realPosition;

	private GameObject realRotation;

	[SerializeField]
	public Transform RotationTransform;

	[SerializeField]
	public CoopMecanimReplicator MecanimReplicator;

	[SerializeField]
	public bool Creepy;

	[Header("Interpolation Delay (Ignored On Host)"), Range(0f, 1f), SerializeField]
	public float InterpolationDelay = 0.15f;

	public bool ignoreRotation;

	private void Awake()
	{
		if (!BoltNetwork.isRunning)
		{
			base.enabled = false;
		}
	}

	public void SetTemporaryRotation(Quaternion rotation, bool ignore)
	{
		base.state.RotationTransform.SetTransforms(null);
		if (ignore)
		{
			this.ignoreRotation = ignore;
		}
		else
		{
			this.ignoreRotation = false;
		}
		if (this.ignoreRotation)
		{
			return;
		}
		this.realRotation.transform.localRotation = base.transform.localRotation;
	}

	public void RestoreRotationReplication()
	{
		this.ignoreRotation = false;
		base.state.RotationTransform.SetTransforms(this.realRotation.transform);
	}

	public override void Attached()
	{
		if (this.entity.isOwner)
		{
			if (this.Creepy)
			{
				base.state.RotationTransform.SetTransforms(base.transform);
			}
			else
			{
				base.state.Transform.SetTransforms(base.transform);
				base.state.RotationTransform.SetTransforms(this.RotationTransform);
			}
		}
		else
		{
			this.timer = 0.55f;
			this.MecanimReplicator.TargetAnimator.applyRootMotion = false;
			if (this.Creepy)
			{
				this.realRotation = new GameObject(this.entity.networkId + "_REAL_ROTATION");
				this.realRotation.transform.localPosition = base.transform.localPosition;
				this.realRotation.transform.localRotation = base.transform.localRotation;
				base.state.RotationTransform.SetTransforms(this.realRotation.transform);
			}
			else
			{
				this.realPosition = new GameObject(this.entity.networkId + "_REAL_POSITION");
				this.realPosition.transform.position = base.transform.position;
				base.state.Transform.SetTransforms(this.realPosition.transform);
				this.realRotation = new GameObject(this.entity.networkId + "_REAL_ROTATION");
				this.realRotation.transform.localPosition = this.RotationTransform.localPosition;
				this.realRotation.transform.localRotation = this.RotationTransform.localRotation;
				base.state.RotationTransform.SetTransforms(this.realRotation.transform);
			}
		}
	}

	private void Update()
	{
		if (this.entity.IsAttached() && !this.entity.IsOwner())
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				if (this.Creepy)
				{
					base.transform.position = this.realRotation.transform.position;
					base.transform.rotation = this.realRotation.transform.rotation;
				}
				else
				{
					base.transform.position = this.realPosition.transform.position;
					this.RotationTransform.localPosition = this.realRotation.transform.localPosition;
					if (!this.ignoreRotation)
					{
						this.RotationTransform.localRotation = this.realRotation.transform.localRotation;
					}
				}
			}
			else
			{
				float num = 1f / this.InterpolationDelay;
				if (this.Creepy)
				{
					base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, this.realRotation.transform.localPosition, Mathf.Clamp01(Time.deltaTime * num));
					base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, this.realRotation.transform.localRotation, Mathf.Clamp01(Time.deltaTime * num));
				}
				else
				{
					base.transform.position = Vector3.Lerp(base.transform.position, this.realPosition.transform.position, Mathf.Clamp01(Time.deltaTime * num));
					this.RotationTransform.localPosition = Vector3.Lerp(this.RotationTransform.localPosition, this.realRotation.transform.localPosition, Mathf.Clamp01(Time.deltaTime * num));
					if (!this.ignoreRotation)
					{
						this.RotationTransform.localRotation = Quaternion.Lerp(this.RotationTransform.localRotation, this.realRotation.transform.localRotation, Mathf.Clamp01(Time.deltaTime * num));
					}
				}
			}
		}
	}
}
