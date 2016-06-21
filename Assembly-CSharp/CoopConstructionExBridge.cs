using Bolt;
using System;
using TheForest.Buildings.Creation;

public class CoopConstructionExBridge : EntityBehaviour
{
	private BridgeArchitect arch;

	public override void Attached()
	{
		this.arch = base.GetComponent<BridgeArchitect>();
		this.arch.WasPlaced = true;
		this.arch.CustomToken = this.entity.attachToken;
		this.entity.SendMessage("OnDeserialized");
	}
}
