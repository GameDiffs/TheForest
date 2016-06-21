using System;

namespace Pathfinding
{
	public abstract class GridNodeBase : GraphNode
	{
		protected int nodeInGridIndex;

		public int NodeInGridIndex
		{
			get
			{
				return this.nodeInGridIndex;
			}
			set
			{
				this.nodeInGridIndex = value;
			}
		}

		protected GridNodeBase(AstarPath astar) : base(astar)
		{
		}
	}
}
