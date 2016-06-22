using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class CoopMutantDummy : EntityBehaviour<IMutantState>
{
	public bool Creepy;

	public GameObject PickupTrigger;

	public GameObject RegularParts;

	public GameObject SkinnyParts;

	public Transform rootTr;

	public CoopMecanimReplicator Replicator;

	private mutantRagdollFollow[] mrf;

	public Transform[] ragDollJoints;

	public List<Quaternion> jointRotations = new List<Quaternion>();

	public bool doSync;

	private void Start()
	{
		this.mrf = base.transform.GetComponentsInChildren<mutantRagdollFollow>();
	}

	private void OnDisable()
	{
		this.doSync = false;
		this.jointRotations.Clear();
	}

	public void updateJointRotations()
	{
		this.jointRotations.Clear();
		for (int i = 0; i < this.ragDollJoints.Length; i++)
		{
			this.jointRotations.Add(this.ragDollJoints[i].localRotation);
			this.doSync = true;
		}
	}

	public void syncRagDollJoints(List<Quaternion> joints)
	{
		for (int i = 0; i < joints.Count; i++)
		{
			this.jointRotations.Add(joints[i]);
			this.doSync = true;
		}
	}

	private void setRagDollDrop()
	{
		mutantRagdollFollow[] array = this.mrf;
		for (int i = 0; i < array.Length; i++)
		{
			mutantRagdollFollow mutantRagdollFollow = array[i];
			mutantRagdollFollow.setDropped();
		}
	}

	public void sendResetRagDoll()
	{
		this.doSync = false;
		this.jointRotations.Clear();
		mutantRagdollFollow[] array = this.mrf;
		for (int i = 0; i < array.Length; i++)
		{
			mutantRagdollFollow mutantRagdollFollow = array[i];
			mutantRagdollFollow.resetRagDollParams();
		}
	}

	[DebuggerHidden]
	private IEnumerator syncRagDollPositions(int getToken)
	{
		CoopMutantDummy.<syncRagDollPositions>c__Iterator1E <syncRagDollPositions>c__Iterator1E = new CoopMutantDummy.<syncRagDollPositions>c__Iterator1E();
		<syncRagDollPositions>c__Iterator1E.getToken = getToken;
		<syncRagDollPositions>c__Iterator1E.<$>getToken = getToken;
		<syncRagDollPositions>c__Iterator1E.<>f__this = this;
		return <syncRagDollPositions>c__Iterator1E;
	}

	private void LateUpdate()
	{
		if (this.doSync)
		{
			for (int i = 0; i < this.ragDollJoints.Length; i++)
			{
				this.ragDollJoints[i].localRotation = this.jointRotations[i];
			}
		}
	}

	public override void Attached()
	{
		if (!this.Creepy)
		{
			base.state.Transform.SetTransforms(base.transform);
		}
		if (!this.entity.isOwner)
		{
			CoopMutantDummyToken coopMutantDummyToken = this.entity.attachToken as CoopMutantDummyToken;
			if (coopMutantDummyToken != null)
			{
				base.transform.localScale = coopMutantDummyToken.Scale;
				if (!this.Creepy)
				{
					if (coopMutantDummyToken.OriginalMutant)
					{
						Animator componentInChildren = coopMutantDummyToken.OriginalMutant.GetComponentInChildren<Animator>();
						AnimatorStateInfo currentAnimatorStateInfo = componentInChildren.GetCurrentAnimatorStateInfo(0);
						if (this.Replicator)
						{
							this.Replicator.ApplyHashToRemote(0, currentAnimatorStateInfo.fullPathHash, 0f, currentAnimatorStateInfo.normalizedTime);
						}
					}
					dummyAnimatorControl component = base.GetComponent<dummyAnimatorControl>();
					if (component)
					{
						component.hips.position = coopMutantDummyToken.HipPosition;
						component.hips.rotation = coopMutantDummyToken.HipRotation;
					}
					float num = float.PositiveInfinity;
					GameObject gameObject = null;
					for (int i = 0; i < Scene.SceneTracker.storedRagDollPrefabs.Count; i++)
					{
						if (Scene.SceneTracker.storedRagDollPrefabs[i] != null)
						{
							float num2 = Vector3.Distance(base.transform.position, Scene.SceneTracker.storedRagDollPrefabs[i].transform.position);
							if (num2 < num)
							{
								num = num2;
								gameObject = Scene.SceneTracker.storedRagDollPrefabs[i];
							}
						}
					}
					if (gameObject)
					{
						storeLocalMutantInfo2 component2 = gameObject.transform.GetComponent<storeLocalMutantInfo2>();
						this.jointRotations.Clear();
						for (int j = 0; j < component2.jointAngles.Count; j++)
						{
							this.ragDollJoints[j].localRotation = component2.jointAngles[j];
							this.jointRotations.Add(component2.jointAngles[j]);
						}
						this.doSync = true;
						UnityEngine.Object.Destroy(gameObject);
						Scene.SceneTracker.storedRagDollPrefabs.RemoveAll((GameObject o) => o == null);
						Scene.SceneTracker.storedRagDollPrefabs.TrimExcess();
					}
				}
				CoopMutantMaterialSync component3 = base.GetComponent<CoopMutantMaterialSync>();
				if (component3 && coopMutantDummyToken.MaterialIndex >= 0)
				{
					component3.ApplyMaterial(coopMutantDummyToken.MaterialIndex);
					component3.Disabled = true;
				}
				if (this.Creepy)
				{
					base.StartCoroutine(this.syncRagDollPositions(coopMutantDummyToken.storedRagDollName));
				}
				if (!this.Creepy)
				{
					CoopMutantPropSync component4 = base.GetComponent<CoopMutantPropSync>();
					if (component4)
					{
						component4.ApplyPropMask(coopMutantDummyToken.Props);
					}
					if (this.RegularParts && this.SkinnyParts)
					{
						if (coopMutantDummyToken.Skinny)
						{
							this.RegularParts.SetActive(false);
							this.SkinnyParts.SetActive(true);
						}
						else
						{
							this.RegularParts.SetActive(true);
							this.SkinnyParts.SetActive(false);
						}
					}
				}
			}
		}
	}
}
