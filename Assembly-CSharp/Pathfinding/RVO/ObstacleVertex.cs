using System;
using UnityEngine;

namespace Pathfinding.RVO
{
	public class ObstacleVertex
	{
		public bool ignore;

		public Vector3 position;

		public Vector2 dir;

		public float height;

		public RVOLayer layer = RVOLayer.DefaultObstacle;

		public bool convex;

		public bool split;

		public ObstacleVertex next;

		public ObstacleVertex prev;
	}
}
