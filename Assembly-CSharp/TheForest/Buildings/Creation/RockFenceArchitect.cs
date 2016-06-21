using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Rock Fence Architect")]
	public class RockFenceArchitect : WallArchitect
	{
		public float _offset = 1f;

		protected override float SegmentPointTestOffset
		{
			get
			{
				return this._offset * 3f;
			}
		}

		public override float MaxSegmentHorizontalLength
		{
			get
			{
				return this._offset * 10f;
			}
		}

		private void Start()
		{
			this._logLength = this._offset;
			this._logWidth = this._offset;
			this._maxSegmentHorizontalLength = this.MaxSegmentHorizontalLength;
		}

		protected override void ShowTempWall()
		{
			Vector3 vector = (this._multiPointsPositions.Count <= 0) ? base.transform.position : this._multiPointsPositions[this._multiPointsPositions.Count - 1];
			Vector3 vector2;
			if (this._multiPointsPositions.Count > 2 && Vector3.Distance(base.transform.position, this._multiPointsPositions[0]) < this._closureSnappingDistance)
			{
				vector2 = this._multiPointsPositions[0];
			}
			else if (this._multiPointsPositions.Count == 0)
			{
				vector2 = base.transform.position + LocalPlayer.MainCamTr.right * base.LogWidth;
			}
			else
			{
				vector2 = base.transform.position;
			}
			GameObject gameObject = null;
			if (this._tmpEdge != null && this._tmpEdge._root)
			{
				gameObject = this._tmpEdge._root.gameObject;
			}
			Debug.DrawLine(vector, vector2);
			this._newPool = new Queue<Transform>();
			this._tmpEdge = base.CreateEdge(vector, vector2);
			this._tmpEdge._root.name = "TempWall";
			this._logPool = this._newPool;
			if (gameObject)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		protected override Transform SpawnEdge(WallArchitect.Edge edge)
		{
			Transform transform = new GameObject("RockFenceEdge").transform;
			transform.transform.position = edge._p1;
			transform.LookAt(edge._p2);
			for (int i = 0; i < edge._segments.Length; i++)
			{
				WallArchitect.HorizontalSegment horizontalSegment = edge._segments[i];
				Vector3 vector = horizontalSegment._p1;
				Vector3 vector2 = horizontalSegment._p2 - horizontalSegment._p1;
				Vector3 normalized = Vector3.Scale(vector2, new Vector3(1f, 0f, 1f)).normalized;
				float y = Mathf.Tan(Vector3.Angle(vector2, normalized) * 0.0174532924f) * this._offset;
				Quaternion rotation = Quaternion.LookRotation(-vector2);
				Transform transform2 = new GameObject("S" + i).transform;
				transform2.parent = transform;
				transform2.position = horizontalSegment._p1;
				transform2.LookAt(horizontalSegment._p2);
				horizontalSegment._root = transform2;
				bool flag = horizontalSegment._p1.y < horizontalSegment._p2.y;
				float f = Vector3.Distance(horizontalSegment._p1, horizontalSegment._p2);
				int j = Mathf.RoundToInt(f);
				if (j < 1)
				{
					j = 1;
				}
				Vector3 a = normalized * this._offset;
				a.y = y;
				if (!flag)
				{
					a.y *= -1f;
				}
				vector.y -= 0.15f;
				while (j > 0)
				{
					int num = Mathf.Min(j, 5);
					j -= num;
					Transform transform3 = (Transform)UnityEngine.Object.Instantiate(Prefabs.Instance.RockFenceChunksGhostPrefabs[num - 1], vector, rotation);
					transform3.parent = transform2;
					vector += a * (float)num;
				}
			}
			return transform;
		}
	}
}
