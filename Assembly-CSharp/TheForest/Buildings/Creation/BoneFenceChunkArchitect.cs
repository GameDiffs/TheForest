using System;
using System.Collections.Generic;
using TheForest.Buildings.World;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Bone Fence Chunk Architect")]
	public class BoneFenceChunkArchitect : WallChunkArchitect
	{
		public int _spread = 3;

		public override Transform BuiltLogPrefab
		{
			get
			{
				return Prefabs.Instance.BoneFenceExBuiltPrefab;
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
				boxCollider.size = new Vector3(this._logWidth * 2f, 0.85f * this._logLength, Mathf.Abs(vector.z));
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
			Quaternion rhs = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
			Vector3 position = this._p1;
			Vector3 vector = Vector3.Lerp(this._p1, this._p2, 0.5f);
			Vector3 vector2 = new Vector3(0f, this._logLength, 0f);
			Vector3 vector3 = this._p2 - this._p1;
			Vector3 normalized = Vector3.Scale(vector3, new Vector3(1f, 0f, 1f)).normalized;
			float y = Mathf.Tan(Vector3.Angle(vector3, normalized) * 0.0174532924f) * this._logWidth * (float)this._spread;
			transform.position = this._p1;
			transform.LookAt(this._p2);
			bool flag = this._p1.y < this._p2.y;
			float num = Vector3.Distance(this._p1, this._p2);
			int num2 = 4;
			Vector3 vector4 = normalized * (num / (float)num2);
			vector4.y = y;
			if (!flag)
			{
				vector4.y *= -1f;
			}
			for (int i = 0; i < num2; i++)
			{
				Transform transform2;
				switch (i)
				{
				case 0:
					transform2 = base.NewLog(position, Quaternion.LookRotation(Vector3.Lerp(this._p1, this._p2, 0.1f) + vector2 - this._p1) * rhs);
					Debug.DrawLine(Vector3.Lerp(this._p1, this._p2, 0.1f) + vector2, this._p1, Color.red);
					break;
				case 1:
					position = vector;
					transform2 = base.NewLog(position, Quaternion.LookRotation(Vector3.Lerp(this._p1, this._p2, 0.35f) + vector2 - vector) * rhs);
					Debug.DrawLine(Vector3.Lerp(this._p1, this._p2, 0.35f) + vector2, vector, Color.blue);
					break;
				case 2:
					position = vector;
					transform2 = base.NewLog(position, Quaternion.LookRotation(Vector3.Lerp(this._p1, this._p2, 0.7f) + vector2 - vector) * rhs);
					Debug.DrawLine(Vector3.Lerp(this._p1, this._p2, 0.7f) + vector2, vector, Color.green);
					break;
				default:
					position = this._p2;
					transform2 = base.NewLog(position, Quaternion.LookRotation(Vector3.Lerp(this._p1, this._p2, 0.9f) + vector2 - this._p2) * rhs);
					Debug.DrawLine(Vector3.Lerp(this._p1, this._p2, 0.9f) + vector2, this._p2, Color.yellow);
					break;
				}
				transform2.name = "v" + i;
				transform2.parent = transform;
			}
			Vector3 vector5 = new Vector3(0f, this._logLength * 0.4f, this._logWidth / 2f);
			Vector3 b = vector5 + (this._p2 - this._p1) / 2f;
			Quaternion rotation = Quaternion.LookRotation(vector + vector2 * 0.6f - (this._p1 + vector5)) * rhs;
			Transform transform3 = base.NewLog(this._p1 + vector5, rotation);
			transform3.name = "h1";
			transform3.parent = transform;
			transform3 = base.NewLog(this._p1 + b, rotation);
			transform3.name = "h2";
			transform3.parent = transform;
			Quaternion rotation2 = Quaternion.LookRotation(this._p1 + vector2 * 0.8f - vector) * rhs;
			Transform transform4 = base.NewLog(vector, rotation2);
			transform4.name = "d1";
			transform4.parent = transform;
			transform4 = base.NewLog(this._p2, rotation2);
			transform4.name = "d2";
			transform4.parent = transform;
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

		public override float GetLevel()
		{
			return this._p1.y + 0.8f * this._logLength;
		}

		public override List<Vector3> GetMultiPointsPositions()
		{
			return new List<Vector3>();
		}
	}
}
