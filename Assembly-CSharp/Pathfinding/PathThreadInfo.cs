using System;

namespace Pathfinding
{
	public struct PathThreadInfo
	{
		public int threadIndex;

		public AstarPath astar;

		public PathHandler runData;

		public readonly object lockObject;

		public PathThreadInfo(int index, AstarPath astar, PathHandler runData)
		{
			this.threadIndex = index;
			this.astar = astar;
			this.runData = runData;
			this.lockObject = new object();
		}
	}
}
