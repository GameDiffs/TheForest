using System;
using System.Collections.Generic;
using TheForest.Buildings.World;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Wall Defensive Chunk Architect")]
	public class WallDefensiveChunkArchitect : WallChunkArchitect
	{
		public override Transform BuiltLogPrefab
		{
			get
			{
				return Prefabs.Instance.LogWallDefensiveExBuiltPrefab;
			}
		}

		public virtual WallDefensiveChunkReinforcement Reinforcement
		{
			get;
			set;
		}

		protected override void OnDeserialized()
		{
			if (!this._initialized)
			{
				if (this._p1 == Vector3.zero)
				{
					this._p1 = base.transform.position;
				}
				if (this._p2 == Vector3.zero)
				{
					this._p2 = base.transform.position + base.transform.forward * (this._logWidth * 5f);
				}
				base.OnDeserialized();
			}
		}

		protected override void CreateStructure(bool isRepair = false)
		{
			if (isRepair)
			{
				base.Clear();
				base.StartCoroutine(base.DelayedAwake(true));
			}
			Vector3 size = this._logRenderer.bounds.size;
			this._logLength = size.y;
			this._logWidth = size.z;
			int layer = LayerMask.NameToLayer("Prop");
			this._wallRoot = this.SpawnEdge();
			this._wallRoot.parent = base.transform;
			if (this._wasBuilt)
			{
				GameObject gameObject = this._wallRoot.gameObject;
				gameObject.tag = "structure";
				gameObject.layer = layer;
				BoxCollider boxCollider = this._wallRoot.gameObject.AddComponent<BoxCollider>();
				Vector3 vector = base.transform.InverseTransformVector(this._p2 - this._p1);
				boxCollider.size = new Vector3(this._logWidth, this._logLength, Mathf.Abs(vector.z));
				Vector3 center = boxCollider.size / 2f;
				center.x = 0f;
				center.y -= 1f;
				center.z -= this._logWidth / 2f;
				boxCollider.center = center;
				this._wallRoot.gameObject.AddComponent<BuildingHealthHitRelay>();
				getStructureStrength getStructureStrength = this._wallRoot.gameObject.AddComponent<getStructureStrength>();
				getStructureStrength._type = getStructureStrength.structureType.wall;
				getStructureStrength._strength = getStructureStrength.strength.veryStrong;
				this._wallRoot.gameObject.AddComponent<gridObjectBlocker>();
			}
		}

		protected override Transform SpawnEdge()
		{
			Transform transform = new GameObject("WallChunk").transform;
			transform.transform.position = this._p1;
			Vector3 vector = this._p2 - this._p1;
			Vector3 normalized = Vector3.Scale(vector, new Vector3(1f, 0f, 1f)).normalized;
			float y = Mathf.Tan(Vector3.Angle(vector, normalized) * 0.0174532924f) * this._logWidth;
			Quaternion rotation = Quaternion.LookRotation(Vector3.forward);
			float num = Vector3.Distance(this._p1, this._p2);
			int num2 = Mathf.RoundToInt(num / this._logWidth);
			Vector3 b = normalized * num / (float)num2;
			b.y = y;
			if (vector.y < 0f)
			{
				b.y *= -1f;
			}
			Vector3 vector2 = this._p1;
			transform.position = this._p1;
			transform.LookAt(this._p2);
			for (int i = 0; i < num2; i++)
			{
				Transform transform2 = base.NewLog(vector2, rotation);
				transform2.parent = transform;
				vector2 += b;
			}
			return transform;
		}

		protected override Quaternion RandomizeLogRotation(Quaternion logRot)
		{
			return logRot * Quaternion.Euler(UnityEngine.Random.Range(-1.5f, 1.5f), (float)UnityEngine.Random.Range(0, 359), UnityEngine.Random.Range(-1.5f, 1.5f));
		}

		private bool CanTurnIntoGate()
		{
			return Mathf.Abs(Vector3.Dot((this._p2 - this._p1).normalized, Vector3.up)) < this._doorAdditionMaxSlope;
		}

		protected bool IsDoor(WallChunkArchitect.Additions addition)
		{
			return addition >= WallChunkArchitect.Additions.Door1;
		}

		public override void ShowToggleAdditionIcon()
		{
			WallChunkArchitect.Additions additions = this.SegmentNextAddition(this._addition);
			switch (additions + 1)
			{
			case WallChunkArchitect.Additions.Window:
				Scene.HudGui.ToggleWallIcon.SetActive(true);
				break;
			case WallChunkArchitect.Additions.Door2:
				Scene.HudGui.ToggleGate1Icon.SetActive(true);
				break;
			}
		}

		protected override WallChunkArchitect.Additions SegmentNextAddition(WallChunkArchitect.Additions addition)
		{
			if (addition == WallChunkArchitect.Additions.Wall && this.CanTurnIntoGate())
			{
				return WallChunkArchitect.Additions.Door1;
			}
			return WallChunkArchitect.Additions.Wall;
		}

		protected override void InitAdditionTrigger()
		{
			if (this.CanTurnIntoGate())
			{
				base.GetComponentInChildren<Craft_Structure>().gameObject.AddComponent<WallAdditionTrigger>();
			}
		}

		public override void UpdateAddition(WallChunkArchitect.Additions addition)
		{
			if (!this._wasBuilt && this.IsDoor(this._addition) && !this.IsDoor(addition))
			{
				UnityEngine.Object.Destroy(base.gameObject.GetComponent<WallDefensiveGateAddition>());
			}
			this._addition = addition;
			if (!this._wasBuilt && this.IsDoor(this._addition) && !BoltNetwork.isClient)
			{
				if (!base.gameObject.GetComponent<WallDefensiveGateAddition>())
				{
					base.gameObject.AddComponent<WallDefensiveGateAddition>();
				}
				else
				{
					base.gameObject.GetComponent<WallDefensiveGateAddition>().Start();
				}
			}
		}

		protected override int GetLogCost()
		{
			return this._wallRoot.childCount;
		}

		public override List<Vector3> GetMultiPointsPositions()
		{
			return null;
		}
	}
}
