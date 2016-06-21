using Bolt;
using System;
using TheForest.Buildings.Creation;

public class CoopBuildingExBridge : EntityBehaviour
{
	private BridgeArchitect arch;

	public override void Attached()
	{
		this.arch = base.GetComponent<BridgeArchitect>();
		this.arch.WasBuilt = true;
		this.arch.CustomToken = this.entity.attachToken;
		if (!this.entity.isOwner)
		{
			this.entity.SendMessage("OnDeserialized");
		}
	}
}
