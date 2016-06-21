using Bolt;
using System;
using TheForest.Buildings.Creation;

public class CoopConstructionDWR : EntityBehaviour
{
	public override void Attached()
	{
		WallDefensiveChunkReinforcement component = base.GetComponent<WallDefensiveChunkReinforcement>();
		component._wasPlaced = true;
		this.entity.SendMessage("OnDeserialized");
	}
}
