using System;
using System.Collections.Generic;
using TheForest.Buildings.World;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Wall Defensive Chunk Architect")]
	public class StickFenceChunkArchitect : WallChunkArchitect
	{
		public int _spread = 3;

		public override Transform BuiltLogPrefab
		{
			get
			{
				return Prefabs.Instance.StickFenceExBuiltPrefab;
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
			this._logLength = size.y;
			this._logWidth = size.z;
			int layer = LayerMask.NameToLayer("Prop");
			this._wallRoot = this.SpawnEdge();
			this._wallRoot.parent = base.transform;
			if (this._wasBuilt)
			{
				GameObject gameObject = this._wallRoot.gameObject;
				gameObject.tag = "jumpObject";
				gameObject.layer = layer;
				BoxCollider boxCollider = this._wallRoot.gameObject.AddComponent<BoxCollider>();
				Vector3 vector = base.transform.InverseTransformVector(this._p2 - this._p1);
				boxCollider.size = new Vector3(this._logWidth * 2f, this._logLength, Mathf.Abs(vector.z));
				Vector3 center = boxCollider.size / 2f;
				center.x = 0f;
				boxCollider.center = center;
				this._wallRoot.gameObject.AddComponent<gridObjectBlocker>();
				this._wallRoot.gameObject.AddComponent<BuildingHealthHitRelay>();
			}
		}

		protected override Transform SpawnEdge()
		{
			Transform transform = new GameObject("FenceChunk").transform;
			transform.position = this._p1;
			transform.LookAt(this._p2);
			Vector3 vector = this._p2 - this._p1;
			Vector3 normalized = Vector3.Scale(vector, new Vector3(1f, 0f, 1f)).normalized;
			float y = Mathf.Tan(Vector3.Angle(vector, normalized) * 0.0174532924f) * this._logWidth * (float)this._spread;
			Quaternion rotation = Quaternion.LookRotation(Vector3.forward);
			Quaternion rhs = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
			Quaternion rotation2 = Quaternion.LookRotation(transform.forward) * rhs;
			bool flag = this._p1.y < this._p2.y;
			float num = Vector3.Distance(this._p1, this._p2);
			int num2 = Mathf.RoundToInt(num / (this._logWidth * (float)this._spread));
			if (num2 > 1)
			{
				num2--;
			}
			Vector3 vector2 = normalized * (num / (float)num2);
			vector2.y = y;
			if (!flag)
			{
				vector2.y *= -1f;
			}
			Vector3 vector3 = this._p1;
			vector3 += vector2 / 2f;
			for (int i = 0; i < num2; i++)
			{
				Transform transform2 = base.NewLog(vector3, rotation);
				transform2.localScale = Vector3.one;
				transform2.parent = transform;
				vector3 += vector2;
			}
			Vector3 vector4 = new Vector3(0f, this._logLength * 0.45f, this._logWidth / 2f);
			Vector3 vector5 = new Vector3(0f, this._logLength * 0.8f, this._logWidth / 2f);
			vector4 += vector2 / 2f;
			vector5 += vector2 / 2f;
			Vector3 localScale = new Vector3(1f, num * 0.335f, 1f);
			Transform transform3 = base.NewLog(this._p1 + vector4, rotation2);
			transform3.parent = transform;
			transform3.localScale = localScale;
			transform3 = base.NewLog(this._p1 + vector5, rotation2);
			transform3.parent = transform;
			transform3.localScale = localScale;
			return transform;
		}

		protected override Quaternion RandomizeLogRotation(Quaternion logRot)
		{
			return logRot * Quaternion.Euler(UnityEngine.Random.Range(-1.5f, 1.5f), (float)UnityEngine.Random.Range(0, 359), UnityEngine.Random.Range(-1.5f, 1.5f));
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
