using Bolt;
using System;
using UnityEngine;

public class CoopMutantDummy : EntityBehaviour<IMutantState>
{
	public bool Creepy;

	public GameObject PickupTrigger;

	public GameObject RegularParts;

	public GameObject SkinnyParts;

	public CoopMecanimReplicator Replicator;

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
				}
				CoopMutantMaterialSync component2 = base.GetComponent<CoopMutantMaterialSync>();
				if (component2 && coopMutantDummyToken.MaterialIndex >= 0)
				{
					component2.ApplyMaterial(coopMutantDummyToken.MaterialIndex);
					component2.Disabled = true;
				}
				if (!this.Creepy)
				{
					CoopMutantPropSync component3 = base.GetComponent<CoopMutantPropSync>();
					if (component3)
					{
						component3.ApplyPropMask(coopMutantDummyToken.Props);
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
