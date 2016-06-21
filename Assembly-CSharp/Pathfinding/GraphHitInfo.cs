using System;
using UnityEngine;

namespace Pathfinding
{
	public struct GraphHitInfo
	{
		public Vector3 origin;

		public Vector3 point;

		public GraphNode node;

		public Vector3 tangentOrigin;

		public Vector3 tangent;

		public float distance
		{
			get
			{
				return (this.point - this.origin).magnitude;
			}
		}

		public GraphHitInfo(Vector3 point)
		{
			this.tangentOrigin = Vector3.zero;
			this.origin = Vector3.zero;
			this.point = point;
			this.node = null;
			this.tangent = Vector3.zero;
		}
	}
}
