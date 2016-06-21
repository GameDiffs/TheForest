using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public interface IFunnelGraph
	{
		void BuildFunnelCorridor(List<GraphNode> path, int sIndex, int eIndex, List<Vector3> left, List<Vector3> right);

		void AddPortal(GraphNode n1, GraphNode n2, List<Vector3> left, List<Vector3> right);
	}
}
