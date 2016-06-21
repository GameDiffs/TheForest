using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public interface IRaycastableGraph
	{
		bool Linecast(Vector3 start, Vector3 end);

		bool Linecast(Vector3 start, Vector3 end, GraphNode hint);

		bool Linecast(Vector3 start, Vector3 end, GraphNode hint, out GraphHitInfo hit);

		bool Linecast(Vector3 start, Vector3 end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace);
	}
}
