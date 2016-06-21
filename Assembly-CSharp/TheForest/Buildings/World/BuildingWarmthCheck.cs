using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.Buildings.Interfaces;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class BuildingWarmthCheck : MonoBehaviour
	{
		private bool _hasActiveWarmth;

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("structure") && !this._hasActiveWarmth)
			{
				FloorArchitect componentInParent = other.GetComponentInParent<FloorArchitect>();
				if (componentInParent)
				{
					IStructureSupport structureSupport = this.SearchValidCeiling(componentInParent);
					if (structureSupport != null && this.FloorHasLitFire(componentInParent.transform))
					{
						base.StartCoroutine(this.BuildingWarmthCheckRoutine(componentInParent, structureSupport));
						return;
					}
				}
			}
		}

		private IStructureSupport SearchValidCeiling(FloorArchitect floor)
		{
			IStructureSupport structureSupport = this.SearchValidCeiling<FloorArchitect>(floor);
			if (structureSupport == null)
			{
				structureSupport = this.SearchValidCeiling<RoofArchitect>(floor);
			}
			return structureSupport;
		}

		private IStructureSupport SearchValidCeiling<T>(FloorArchitect floor) where T : IStructureSupport
		{
			T[] componentsInChildren = floor.GetComponentsInChildren<T>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				T t = componentsInChildren[i];
				if (t != null && t.GetLevel() > floor.GetLevel() + 4f && this.IsPlayerPositionValid(LocalPlayer.Transform.position, floor, t))
				{
					return t;
				}
			}
			return null;
		}

		[DebuggerHidden]
		private IEnumerator BuildingWarmthCheckRoutine(FloorArchitect floor, IStructureSupport ceiling)
		{
			BuildingWarmthCheck.<BuildingWarmthCheckRoutine>c__Iterator145 <BuildingWarmthCheckRoutine>c__Iterator = new BuildingWarmthCheck.<BuildingWarmthCheckRoutine>c__Iterator145();
			<BuildingWarmthCheckRoutine>c__Iterator.floor = floor;
			<BuildingWarmthCheckRoutine>c__Iterator.ceiling = ceiling;
			<BuildingWarmthCheckRoutine>c__Iterator.<$>floor = floor;
			<BuildingWarmthCheckRoutine>c__Iterator.<$>ceiling = ceiling;
			<BuildingWarmthCheckRoutine>c__Iterator.<>f__this = this;
			return <BuildingWarmthCheckRoutine>c__Iterator;
		}

		private bool IsPlayerPositionValid(Vector3 testPos, IStructureSupport floor, IStructureSupport ceiling)
		{
			return testPos.y > floor.GetLevel() && testPos.y < ceiling.GetLevel() && MathEx.IsPointInPolygon(testPos, floor.GetMultiPointsPositions()) && MathEx.IsPointInPolygon(testPos, ceiling.GetMultiPointsPositions());
		}

		private bool FloorHasLitFire(Transform floor)
		{
			bool result = false;
			int childCount = floor.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = floor.transform.GetChild(i);
				if (child.CompareTag("fire"))
				{
					Fire2 componentInChildren = child.GetComponentInChildren<Fire2>();
					if (componentInChildren.Lit)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}
}
