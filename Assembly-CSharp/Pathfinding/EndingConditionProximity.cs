using System;
using UnityEngine;

namespace Pathfinding
{
	public class EndingConditionProximity : ABPathEndingCondition
	{
		public float maxDistance = 10f;

		public EndingConditionProximity(ABPath p, float maxDistance) : base(p)
		{
			this.maxDistance = maxDistance;
		}

		public override bool TargetFound(PathNode node)
		{
			return ((Vector3)node.node.position - this.abPath.originalEndPoint).sqrMagnitude <= this.maxDistance * this.maxDistance;
		}
	}
}
