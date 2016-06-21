using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Rock Path Architect")]
	public class RockPathArchitect : WallArchitect
	{
		public int _width = 3;

		public int _minRockCount = 8;

		protected override float SegmentPointTestOffset
		{
			get
			{
				return this._logLength * 2f;
			}
		}

		protected override float MinLockLength
		{
			get
			{
				return (float)this._minRockCount * this._logWidth * ((float)this._minRockCount * this._logWidth);
			}
		}

		private void Start()
		{
			Vector3 size = this._logRenderer.bounds.size;
			this._logLength = size.x;
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
			Transform transform = new GameObject("RockPathEdge").transform;
			transform.transform.position = edge._p1;
			transform.LookAt(edge._p2);
			for (int i = 0; i < edge._segments.Length; i++)
			{
				WallArchitect.HorizontalSegment horizontalSegment = edge._segments[i];
				Vector3 a = horizontalSegment._p2 - horizontalSegment._p1;
				Vector3 normalized = Vector3.Scale(a, new Vector3(1f, 0f, 1f)).normalized;
				Transform transform2 = new GameObject("S" + i).transform;
				transform2.parent = transform;
				transform2.position = horizontalSegment._p1;
				transform2.LookAt(horizontalSegment._p2);
				horizontalSegment._root = transform2;
				Vector3 vector = horizontalSegment._p1 + transform2.right * (float)this._width;
				Vector3 vector2 = horizontalSegment._p1 - transform2.right * (float)this._width;
				float num = Vector3.Distance(horizontalSegment._p1, horizontalSegment._p2);
				int num2 = Mathf.RoundToInt(num / this._logWidth) + 1;
				if (num2 < 1)
				{
					num2 = 1;
				}
				Vector3 vector3 = normalized * (num / (float)num2);
				vector += vector3 / 2f;
				vector2 += vector3 / 2f;
				Terrain activeTerrain = Terrain.activeTerrain;
				for (int j = 0; j < num2; j++)
				{
					vector.y = activeTerrain.SampleHeight(vector);
					Transform transform3 = base.NewLog(vector, transform2.rotation);
					transform3.localScale = Vector3.one;
					transform3.parent = transform2;
					this._newPool.Enqueue(transform3);
					vector += vector3;
					vector2.y = activeTerrain.SampleHeight(vector2);
					Transform transform4 = base.NewLog(vector2, transform2.rotation);
					transform4.localScale = Vector3.one;
					transform4.parent = transform2;
					this._newPool.Enqueue(transform4);
					vector2 += vector3;
				}
			}
			return transform;
		}
	}
}
