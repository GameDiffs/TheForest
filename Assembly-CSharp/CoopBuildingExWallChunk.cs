using Bolt;
using System;
using TheForest.Buildings.Creation;
using TheForest.Buildings.Interfaces;
using UniLinq;
using UnityEngine;

public class CoopBuildingExWallChunk : EntityBehaviour<IWallChunkBuildingState>
{
	public override void Attached()
	{
		CoopWallChunkToken coopWallChunkToken = (CoopWallChunkToken)this.entity.attachToken;
		WallChunkArchitect component = base.GetComponent<WallChunkArchitect>();
		component.transform.position = coopWallChunkToken.P1;
		component.transform.LookAt(coopWallChunkToken.P2);
		component.P1 = coopWallChunkToken.P1;
		component.P2 = coopWallChunkToken.P2;
		component.Addition = coopWallChunkToken.Additions;
		component.MultipointPositions = ((coopWallChunkToken.PointsPositions == null) ? null : coopWallChunkToken.PointsPositions.ToList<Vector3>());
		component.WasBuilt = true;
		if (coopWallChunkToken.Support != null)
		{
			component.CurrentSupport = (coopWallChunkToken.Support.GetComponentInChildren(typeof(IStructureSupport)) as IStructureSupport);
		}
		if (!this.entity.isOwner)
		{
			this.entity.SendMessage("OnDeserialized");
		}
	}

	public override void Detached()
	{
		if (BoltNetwork.isClient)
		{
			BoltEntity[] componentsInChildren = base.GetComponentsInChildren<BoltEntity>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				BoltEntity boltEntity = componentsInChildren[i];
				if (!object.ReferenceEquals(boltEntity, this.entity))
				{
					boltEntity.transform.parent = null;
				}
			}
		}
	}
}
