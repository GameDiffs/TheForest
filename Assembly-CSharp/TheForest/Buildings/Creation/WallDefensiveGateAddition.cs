using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class WallDefensiveGateAddition : MonoBehaviour
	{
		public void Start()
		{
			WallDefensiveChunkArchitect component = base.GetComponent<WallDefensiveChunkArchitect>();
			WallDefensiveGateArchitect component2 = base.GetComponent<WallDefensiveGateArchitect>();
			if (component && !component2)
			{
				if (!this.CheckCombineGate())
				{
					this.SpawnGate(component.P1, component.P2, component.Addition);
					base.GetComponentInChildren<Craft_Structure>().CancelBlueprintSafe();
				}
			}
			else if (component2 && component2.Addition == WallChunkArchitect.Additions.Wall)
			{
				this.ToggleBackToWall();
			}
		}

		private bool CheckCombineGate()
		{
			WallDefensiveGateAddition[] array = UnityEngine.Object.FindObjectsOfType<WallDefensiveGateAddition>();
			for (int i = 0; i < array.Length; i++)
			{
				WallDefensiveGateAddition wallDefensiveGateAddition = array[i];
				if (wallDefensiveGateAddition != this && !wallDefensiveGateAddition.GetComponent<WallDefensiveGateArchitect>().Is2Sided && Mathf.Abs(Vector3.Dot(base.transform.forward, wallDefensiveGateAddition.transform.forward)) > 0.95f)
				{
					WallDefensiveChunkArchitect component = wallDefensiveGateAddition.GetComponent<WallDefensiveChunkArchitect>();
					WallDefensiveChunkArchitect component2 = base.GetComponent<WallDefensiveChunkArchitect>();
					WallChunkArchitect.Additions addition = component.Addition;
					if (Vector3.Distance(component.P2, component2.P1) < 0.2f && Vector3.Distance(component.P1, component2.P2) < 22f)
					{
						this.SpawnGate(component.P1, component2.P2, addition);
						base.GetComponentInChildren<Craft_Structure>().CancelBlueprintSafe();
						wallDefensiveGateAddition.GetComponentInChildren<Craft_Structure>().CancelBlueprintSafe();
						return true;
					}
					if (Vector3.Distance(component.P1, component2.P2) < 0.2f && Vector3.Distance(component.P2, component2.P1) < 22f)
					{
						this.SpawnGate(component2.P1, component.P2, addition);
						base.GetComponentInChildren<Craft_Structure>().CancelBlueprintSafe();
						wallDefensiveGateAddition.GetComponentInChildren<Craft_Structure>().CancelBlueprintSafe();
						return true;
					}
				}
			}
			return false;
		}

		private void ToggleBackToWall()
		{
			WallDefensiveGateArchitect component = base.GetComponent<WallDefensiveGateArchitect>();
			if (component.WasBuilt)
			{
				UnityEngine.Object.Destroy(this);
			}
			else
			{
				bool is2Sided = component.Is2Sided;
				Vector3 vector = (!is2Sided) ? component.P2 : Vector3.Lerp(component.P1, component.P2, 0.5f);
				if (BoltNetwork.isRunning)
				{
					PrefabId prefabId = Prefabs.Instance.WallDefensiveChunkGhostPrefab.GetComponent<BoltEntity>().prefabId;
					PlaceWallChunk placeWallChunk = PlaceWallChunk.Create(GlobalTargets.OnlyServer);
					CoopWallChunkToken coopWallChunkToken = new CoopWallChunkToken();
					coopWallChunkToken.P1 = component.P1;
					coopWallChunkToken.P2 = vector;
					coopWallChunkToken.PointsPositions = null;
					coopWallChunkToken.Additions = WallChunkArchitect.Additions.Wall;
					placeWallChunk.parent = ((!base.transform.parent) ? null : base.transform.parent.GetComponentInParent<BoltEntity>());
					placeWallChunk.token = coopWallChunkToken;
					placeWallChunk.prefab = prefabId;
					placeWallChunk.support = null;
					placeWallChunk.Send();
					if (is2Sided)
					{
						PlaceWallChunk placeWallChunk2 = PlaceWallChunk.Create(GlobalTargets.OnlyServer);
						CoopWallChunkToken coopWallChunkToken2 = new CoopWallChunkToken();
						coopWallChunkToken2.P1 = vector;
						coopWallChunkToken2.P2 = component.P2;
						coopWallChunkToken2.PointsPositions = null;
						coopWallChunkToken2.Additions = WallChunkArchitect.Additions.Wall;
						placeWallChunk2.parent = ((!base.transform.parent) ? null : base.transform.parent.GetComponentInParent<BoltEntity>());
						placeWallChunk2.token = coopWallChunkToken2;
						placeWallChunk2.prefab = prefabId;
						placeWallChunk2.support = null;
						placeWallChunk2.Send();
					}
				}
				else
				{
					WallChunkArchitect wallChunkArchitect = UnityEngine.Object.Instantiate<WallDefensiveChunkArchitect>(Prefabs.Instance.WallDefensiveChunkGhostPrefab);
					if (base.transform.parent)
					{
						wallChunkArchitect.transform.parent = base.transform.parent;
					}
					wallChunkArchitect.transform.position = component.P1;
					wallChunkArchitect.transform.LookAt(vector);
					wallChunkArchitect.MultipointPositions = null;
					wallChunkArchitect.P1 = component.P1;
					wallChunkArchitect.P2 = vector;
					wallChunkArchitect.CurrentSupport = component.CurrentSupport;
					if (is2Sided)
					{
						WallChunkArchitect wallChunkArchitect2 = UnityEngine.Object.Instantiate<WallDefensiveChunkArchitect>(Prefabs.Instance.WallDefensiveChunkGhostPrefab);
						if (base.transform.parent)
						{
							wallChunkArchitect2.transform.parent = base.transform.parent;
						}
						wallChunkArchitect2.transform.position = vector;
						wallChunkArchitect2.transform.LookAt(component.P2);
						wallChunkArchitect2.MultipointPositions = null;
						wallChunkArchitect2.P1 = vector;
						wallChunkArchitect2.P2 = component.P2;
						wallChunkArchitect2.CurrentSupport = component.CurrentSupport;
					}
				}
				base.StartCoroutine(this.DelayedCancelGhost());
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedCancelGhost()
		{
			WallDefensiveGateAddition.<DelayedCancelGhost>c__Iterator142 <DelayedCancelGhost>c__Iterator = new WallDefensiveGateAddition.<DelayedCancelGhost>c__Iterator142();
			<DelayedCancelGhost>c__Iterator.<>f__this = this;
			return <DelayedCancelGhost>c__Iterator;
		}

		private void SpawnGate(Vector3 p1, Vector3 p2, WallChunkArchitect.Additions addition)
		{
			if (BoltNetwork.isRunning)
			{
				PlaceWallChunk placeWallChunk = PlaceWallChunk.Create(GlobalTargets.OnlyServer);
				CoopWallChunkToken coopWallChunkToken = new CoopWallChunkToken();
				coopWallChunkToken.P1 = p1;
				coopWallChunkToken.P2 = p2;
				coopWallChunkToken.PointsPositions = null;
				coopWallChunkToken.Additions = addition;
				placeWallChunk.parent = base.transform.GetComponentInParent<BoltEntity>();
				placeWallChunk.token = coopWallChunkToken;
				placeWallChunk.prefab = Prefabs.Instance.WallDefensiveGateGhostPrefab.GetComponent<BoltEntity>().prefabId;
				placeWallChunk.support = null;
				placeWallChunk.Send();
			}
			else
			{
				WallDefensiveGateArchitect wallDefensiveGateArchitect = (WallDefensiveGateArchitect)UnityEngine.Object.Instantiate(Prefabs.Instance.WallDefensiveGateGhostPrefab, p1, Quaternion.LookRotation(p2 - p1));
				wallDefensiveGateArchitect.P1 = p1;
				wallDefensiveGateArchitect.P2 = p2;
				wallDefensiveGateArchitect.Addition = addition;
			}
		}
	}
}
