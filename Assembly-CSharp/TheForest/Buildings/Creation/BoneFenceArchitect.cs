using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Bone Fence Architect")]
	public class BoneFenceArchitect : WallArchitect
	{
		public int _spread = 3;

		protected override float SegmentPointTestOffset
		{
			get
			{
				return this._logLength * 2f;
			}
		}

		public override float MaxSegmentHorizontalLength
		{
			get
			{
				return this._logRenderer.bounds.size.z * 5f * (float)this._spread;
			}
		}

		private void Start()
		{
			Vector3 size = this._logRenderer.bounds.size;
			this._logLength = size.y;
			this._logWidth = size.z;
			this._maxSegmentHorizontalLength = this.MaxSegmentHorizontalLength;
		}

		protected override void ShowTempWall()
		{
			Vector3 p = (this._multiPointsPositions.Count <= 0) ? (base.transform.position - LocalPlayer.MainCamTr.right * base.LogWidth / 2f) : this._multiPointsPositions[this._multiPointsPositions.Count - 1];
			Vector3 p2;
			if (this._multiPointsPositions.Count > 2 && Vector3.Distance(base.transform.position, this._multiPointsPositions[0]) < this._closureSnappingDistance)
			{
				p2 = this._multiPointsPositions[0];
			}
			else if (this._multiPointsPositions.Count == 0)
			{
				p2 = base.transform.position + LocalPlayer.MainCamTr.right * base.LogWidth / 2f;
			}
			else
			{
				p2 = base.transform.position;
			}
			GameObject gameObject = null;
			if (this._tmpEdge != null && this._tmpEdge._root)
			{
				gameObject = this._tmpEdge._root.gameObject;
			}
			this._newPool = new Queue<Transform>();
			this._tmpEdge = base.CreateEdge(p, p2);
			this._tmpEdge._root.name = "TempWall";
			this._logPool = this._newPool;
			if (gameObject)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		protected override Transform SpawnEdge(WallArchitect.Edge edge)
		{
			Transform transform = new GameObject("StickFenceEdge").transform;
			transform.transform.position = edge._p1;
			transform.LookAt(edge._p2);
			Quaternion quaternion = Quaternion.LookRotation(Vector3.forward);
			Quaternion rhs = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
			for (int i = 0; i < edge._segments.Length; i++)
			{
				WallArchitect.HorizontalSegment horizontalSegment = edge._segments[i];
				Vector3 position = horizontalSegment._p1;
				Vector3 vector = Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.5f);
				Vector3 vector2 = new Vector3(0f, this._logLength, 0f);
				Vector3 vector3 = horizontalSegment._p2 - horizontalSegment._p1;
				Vector3 normalized = Vector3.Scale(vector3, new Vector3(1f, 0f, 1f)).normalized;
				float y = Mathf.Tan(Vector3.Angle(vector3, normalized) * 0.0174532924f) * this._logWidth * (float)this._spread;
				Transform transform2 = new GameObject("S" + i).transform;
				transform2.parent = transform;
				transform2.position = horizontalSegment._p1;
				transform2.LookAt(horizontalSegment._p2);
				horizontalSegment._root = transform2;
				bool flag = horizontalSegment._p1.y < horizontalSegment._p2.y;
				float num = Vector3.Distance(horizontalSegment._p1, horizontalSegment._p2);
				int num2 = 4;
				Vector3 vector4 = normalized * (num / (float)num2);
				vector4.y = y;
				if (!flag)
				{
					vector4.y *= -1f;
				}
				for (int j = 0; j < num2; j++)
				{
					Transform transform3;
					switch (j)
					{
					case 0:
						transform3 = base.NewLog(position, Quaternion.LookRotation(Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.1f) + vector2 - horizontalSegment._p1) * rhs);
						Debug.DrawLine(Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.1f) + vector2, horizontalSegment._p1, Color.red);
						break;
					case 1:
						position = vector;
						transform3 = base.NewLog(position, Quaternion.LookRotation(Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.35f) + vector2 - vector) * rhs);
						Debug.DrawLine(Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.35f) + vector2, vector, Color.blue);
						break;
					case 2:
						position = vector;
						transform3 = base.NewLog(position, Quaternion.LookRotation(Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.7f) + vector2 - vector) * rhs);
						Debug.DrawLine(Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.7f) + vector2, vector, Color.green);
						break;
					default:
						position = horizontalSegment._p2;
						transform3 = base.NewLog(position, Quaternion.LookRotation(Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.9f) + vector2 - horizontalSegment._p2) * rhs);
						Debug.DrawLine(Vector3.Lerp(horizontalSegment._p1, horizontalSegment._p2, 0.9f) + vector2, horizontalSegment._p2, Color.yellow);
						break;
					}
					transform3.name = "v" + j;
					transform3.parent = transform2;
					this._newPool.Enqueue(transform3);
				}
				Vector3 vector5 = new Vector3(0f, this._logLength * 0.4f, this._logWidth / 2f);
				Vector3 b = vector5 + (horizontalSegment._p2 - horizontalSegment._p1) / 2f;
				Quaternion rotation = Quaternion.LookRotation(vector + vector2 * 0.6f - (horizontalSegment._p1 + vector5)) * rhs;
				Transform transform4 = base.NewLog(horizontalSegment._p1 + vector5, rotation);
				transform4.name = "h1";
				transform4.parent = transform2;
				this._newPool.Enqueue(transform4);
				transform4 = base.NewLog(horizontalSegment._p1 + b, rotation);
				transform4.name = "h2";
				transform4.parent = transform2;
				this._newPool.Enqueue(transform4);
				Quaternion rotation2 = Quaternion.LookRotation(horizontalSegment._p1 + vector2 * 0.8f - vector) * rhs;
				Transform transform5 = base.NewLog(vector, rotation2);
				transform5.name = "d1";
				transform5.parent = transform2;
				this._newPool.Enqueue(transform5);
				transform5 = base.NewLog(horizontalSegment._p2, rotation2);
				transform5.name = "d2";
				transform5.parent = transform2;
				this._newPool.Enqueue(transform5);
			}
			return transform;
		}
	}
}
