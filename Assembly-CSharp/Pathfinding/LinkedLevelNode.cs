using System;
using UnityEngine;

namespace Pathfinding
{
	public class LinkedLevelNode
	{
		public Vector3 position;

		public bool walkable;

		public RaycastHit hit;

		public float height;

		public LinkedLevelNode next;
	}
}
