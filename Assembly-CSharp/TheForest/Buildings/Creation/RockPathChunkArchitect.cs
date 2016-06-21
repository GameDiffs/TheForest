using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Rock Path Chunk Architect")]
	public class RockPathChunkArchitect : WallChunkArchitect
	{
		public int _width = 3;

		public override Transform BuiltLogPrefab
		{
			get
			{
				return Prefabs.Instance.RockPathExBuiltPrefab;
			}
		}

		protected override void OnDeserialized()
		{
			if (!this._initialized)
			{
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
			this._logLength = size.x;
			this._logWidth = size.z;
			int num = LayerMask.NameToLayer("Prop");
			this._wallRoot = this.SpawnEdge();
			this._wallRoot.parent = base.transform;
			if (this._wasBuilt)
			{
				int num2 = Mathf.RoundToInt(Vector3.Distance(this._p1, this._p2) / (float)this._width);
				for (int i = 0; i < num2; i++)
				{
					NeoGrassCutter.Cut(Vector3.Lerp(this._p1, this._p2, (float)i / (float)num2), (float)this._width + 0.5f, false);
				}
			}
		}

		protected override Transform SpawnEdge()
		{
			Transform transform = new GameObject("RockPathChunk").transform;
			transform.position = this._p1;
			transform.LookAt(this._p2);
			Vector3 a = this._p2 - this._p1;
			Vector3 normalized = Vector3.Scale(a, new Vector3(1f, 0f, 1f)).normalized;
			Vector3 vector = this._p1 + transform.right * (float)this._width;
			Vector3 vector2 = this._p1 - transform.right * (float)this._width;
			float num = Vector3.Distance(this._p1, this._p2);
			int num2 = Mathf.RoundToInt(num / this._logWidth) + 1;
			if (num2 < 1)
			{
				num2 = 1;
			}
			Vector3 vector3 = normalized * (num / (float)num2);
			vector += vector3 / 2f;
			vector2 += vector3 / 2f;
			Terrain activeTerrain = Terrain.activeTerrain;
			for (int i = 0; i < num2; i++)
			{
				vector.y = activeTerrain.SampleHeight(vector);
				Transform transform2 = base.NewLog(vector, transform.rotation);
				transform2.localScale = Vector3.one;
				transform2.parent = transform;
				vector += vector3;
				vector2.y = activeTerrain.SampleHeight(vector2);
				Transform transform3 = base.NewLog(vector2, transform.rotation);
				transform3.localScale = Vector3.one;
				transform3.parent = transform;
				vector2 += vector3;
			}
			return transform;
		}

		protected override Quaternion RandomizeLogRotation(Quaternion logRot)
		{
			return logRot * Quaternion.Euler(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(25f, 25f), UnityEngine.Random.Range(-3f, 3f));
		}

		protected override void InitAdditionTrigger()
		{
		}

		protected override int GetLogCost()
		{
			return this._wallRoot.childCount;
		}

		public override List<Vector3> GetMultiPointsPositions()
		{
			return new List<Vector3>();
		}
	}
}
