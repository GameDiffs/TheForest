using Bolt;
using System;
using TheForest.Buildings.Creation;

public class CoopConstructionRope : EntityBehaviour<IBuildingState>
{
	public bool wasBuilt;

	public override void Attached()
	{
		SingleAnchorStructure componentInChildren = base.GetComponentInChildren<SingleAnchorStructure>();
		CoopSingleAnchorToken coopSingleAnchorToken = (CoopSingleAnchorToken)this.entity.attachToken;
		ICoopAnchorStructure coopAnchorStructure = coopSingleAnchorToken.Anchor.GetComponent(typeof(ICoopAnchorStructure)) as ICoopAnchorStructure;
		componentInChildren._wasPlaced = true;
		componentInChildren._wasBuilt = this.wasBuilt;
		componentInChildren.Anchor1 = coopAnchorStructure.GetAnchor(coopSingleAnchorToken.Index);
	}
}
