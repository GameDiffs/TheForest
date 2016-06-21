using System;
using TheForest.Buildings.Creation;
using TheForest.Buildings.Interfaces;
using UniLinq;
using UnityEngine;

public class CoopConstructionEx : CoopConstruction
{
	public MonoBehaviour[] Architects;

	public override void Attached()
	{
		CoopConstructionExToken coopConstructionExToken = (CoopConstructionExToken)this.entity.attachToken;
		for (int i = 0; i < coopConstructionExToken.Architects.Length; i++)
		{
			ICoopStructure coopStructure = (ICoopStructure)this.Architects[i];
			coopStructure.CustomToken = coopConstructionExToken.Architects[i].CustomToken;
			coopStructure.MultiPointsCount = coopConstructionExToken.Architects[i].PointsCount;
			coopStructure.MultiPointsPositions = coopConstructionExToken.Architects[i].PointsPositions.ToList<Vector3>();
			coopStructure.WasPlaced = true;
			if (coopStructure is FoundationArchitect && coopConstructionExToken.Architects[i].AboveGround)
			{
				((FoundationArchitect)coopStructure)._aboveGround = true;
			}
			if (coopConstructionExToken.Architects[i].Support)
			{
				IStructureSupport structureSupport = coopConstructionExToken.Architects[i].Support.GetComponentInChildren(typeof(IStructureSupport)) as IStructureSupport;
				if (structureSupport != null)
				{
					if (coopStructure is RoofArchitect)
					{
						(coopStructure as RoofArchitect).CurrentSupport = structureSupport;
					}
					if (coopStructure is FloorArchitect)
					{
						(coopStructure as FloorArchitect).CurrentSupport = structureSupport;
					}
				}
			}
		}
		this.entity.SendMessage("OnDeserialized");
		base.Attached();
	}
}
