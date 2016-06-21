using Pathfinding.Util;
using System;
using System.Collections.Generic;

namespace Pathfinding
{
	public static class GraphUpdateUtilities
	{
		public static bool UpdateGraphsNoBlock(GraphUpdateObject guo, GraphNode node1, GraphNode node2, bool alwaysRevert = false)
		{
			List<GraphNode> list = ListPool<GraphNode>.Claim();
			list.Add(node1);
			list.Add(node2);
			bool result = GraphUpdateUtilities.UpdateGraphsNoBlock(guo, list, alwaysRevert);
			ListPool<GraphNode>.Release(list);
			return result;
		}

		public static bool UpdateGraphsNoBlock(GraphUpdateObject guo, List<GraphNode> nodes, bool alwaysRevert = false)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (!nodes[i].Walkable)
				{
					return false;
				}
			}
			guo.trackChangedNodes = true;
			bool worked = true;
			AstarPath.RegisterSafeUpdate(delegate
			{
				AstarPath.active.UpdateGraphs(guo);
				AstarPath.active.FlushGraphUpdates();
				worked = (worked && PathUtilities.IsPathPossible(nodes));
				if (!worked || alwaysRevert)
				{
					guo.RevertFromBackup();
					AstarPath.active.FloodFill();
				}
			});
			AstarPath.active.FlushThreadSafeCallbacks();
			guo.trackChangedNodes = false;
			return worked;
		}
	}
}
