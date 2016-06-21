using Bolt;
using System;
using TheForest.Buildings.Creation;
using UniLinq;
using UnityEngine;

public class CoopConstructionExWallChunk : EntityBehaviour<IWallChunkConstructionState>
{
	public override void Attached()
	{
		CoopWallChunkToken coopWallChunkToken = (CoopWallChunkToken)this.entity.attachToken;
		WallChunkArchitect component = base.GetComponent<WallChunkArchitect>();
		component.transform.position = coopWallChunkToken.P1;
		component.transform.LookAt(coopWallChunkToken.P2);
		component.P1 = coopWallChunkToken.P1;
		component.P2 = coopWallChunkToken.P2;
		if (BoltNetwork.isServer)
		{
			base.state.Addition = (int)component.Addition;
			coopWallChunkToken.Additions = component.Addition;
		}
		else
		{
			component.Addition = coopWallChunkToken.Additions;
		}
		component.WasPlaced = true;
		component.MultipointPositions = ((coopWallChunkToken.PointsPositions == null) ? null : coopWallChunkToken.PointsPositions.ToList<Vector3>());
		base.state.AddCallback("Addition", new PropertyCallbackSimple(this.AdditionChanged));
		this.entity.SendMessage("OnDeserialized");
	}

	private void AdditionChanged()
	{
		base.GetComponent<WallChunkArchitect>().UpdateAddition((WallChunkArchitect.Additions)base.state.Addition);
	}
}
