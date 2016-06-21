using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Wall Defensive Architect")]
	public class StickFenceArchitect : WallArchitect
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
			Quaternion rotation = Quaternion.LookRotation(Vector3.forward);
			Quaternion rhs = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
			for (int i = 0; i < edge._segments.Length; i++)
			{
				WallArchitect.HorizontalSegment horizontalSegment = edge._segments[i];
				Vector3 vector = horizontalSegment._p1;
				Vector3 vector2 = horizontalSegment._p2 - horizontalSegment._p1;
				Vector3 normalized = Vector3.Scale(vector2, new Vector3(1f, 0f, 1f)).normalized;
				float y = Mathf.Tan(Vector3.Angle(vector2, normalized) * 0.0174532924f) * this._logWidth * (float)this._spread;
				Transform transform2 = new GameObject("S" + i).transform;
				transform2.parent = transform;
				transform2.position = horizontalSegment._p1;
				transform2.LookAt(horizontalSegment._p2);
				horizontalSegment._root = transform2;
				bool flag = horizontalSegment._p1.y < horizontalSegment._p2.y;
				float num = Vector3.Distance(horizontalSegment._p1, horizontalSegment._p2);
				int num2 = Mathf.RoundToInt(num / (this._logWidth * (float)this._spread));
				if (num2 > 1)
				{
					num2--;
				}
				else
				{
					num2 = 1;
				}
				Vector3 vector3 = normalized * (num / (float)num2);
				vector3.y = y;
				if (!flag)
				{
					vector3.y *= -1f;
				}
				vector += vector3 / 2f;
				for (int j = 0; j < num2; j++)
				{
					Transform transform3 = base.NewLog(vector, rotation);
					transform3.localScale = Vector3.one;
					transform3.parent = transform2;
					this._newPool.Enqueue(transform3);
					vector += vector3;
				}
				Vector3 vector4 = new Vector3(0f, this._logLength * 0.45f, this._logWidth / 2f);
				Vector3 vector5 = new Vector3(0f, this._logLength * 0.8f, this._logWidth / 2f);
				vector4 += vector3 / 2f;
				vector5 += vector3 / 2f;
				Vector3 localScale = new Vector3(1f, num * 0.335f, 1f);
				Quaternion rotation2 = Quaternion.LookRotation(transform2.forward) * rhs;
				Debug.DrawRay(horizontalSegment._p1 + vector4, -transform2.up, Color.red);
				Transform transform4 = base.NewLog(horizontalSegment._p1 + vector4, rotation2);
				transform4.parent = transform2;
				transform4.localScale = localScale;
				this._newPool.Enqueue(transform4);
				transform4 = base.NewLog(horizontalSegment._p1 + vector5, rotation2);
				transform4.parent = transform2;
				transform4.localScale = localScale;
				this._newPool.Enqueue(transform4);
			}
			return transform;
		}
	}
}
