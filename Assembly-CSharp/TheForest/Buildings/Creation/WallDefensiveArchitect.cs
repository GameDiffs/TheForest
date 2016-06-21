using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Wall Defensive Architect")]
	public class WallDefensiveArchitect : WallArchitect
	{
		private void Start()
		{
			Vector3 size = this._logRenderer.bounds.size;
			this._logLength = size.y;
			this._logWidth = size.z;
			this._maxSegmentHorizontalLength = this._logWidth * 5.49f;
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
				vector2 = base.transform.position - LocalPlayer.MainCamTr.right * base.LogWidth;
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
			Transform transform = new GameObject("WallDefensiveEdge").transform;
			transform.transform.position = edge._p1;
			Vector3 vector = edge._p2 - edge._p1;
			Vector3 normalized = Vector3.Scale(vector, new Vector3(1f, 0f, 1f)).normalized;
			float y = Mathf.Tan(Vector3.Angle(vector, normalized) * 0.0174532924f) * this._logWidth;
			Quaternion rotation = Quaternion.LookRotation(Vector3.forward);
			for (int i = 0; i < edge._segments.Length; i++)
			{
				WallArchitect.HorizontalSegment horizontalSegment = edge._segments[i];
				Vector3 vector2 = horizontalSegment._p1;
				Transform transform2 = new GameObject("Segment" + i).transform;
				transform2.parent = transform;
				horizontalSegment._root = transform2;
				transform2.position = horizontalSegment._p1;
				float num = Vector3.Distance(horizontalSegment._p1, horizontalSegment._p2);
				int num2 = Mathf.RoundToInt(num / this._logWidth);
				Vector3 b = normalized * num / (float)num2;
				b.y = y;
				if (vector.y < 0f)
				{
					b.y *= -1f;
				}
				for (int j = 0; j < num2; j++)
				{
					Transform transform3 = base.NewLog(vector2, rotation);
					transform3.parent = transform2;
					this._newPool.Enqueue(transform3);
					vector2 += b;
				}
			}
			return transform;
		}
	}
}
