using System;

namespace Pathfinding
{
	public class PathEndingCondition
	{
		protected Path path;

		protected PathEndingCondition()
		{
		}

		public PathEndingCondition(Path p)
		{
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			this.path = p;
		}

		public virtual bool TargetFound(PathNode node)
		{
			return true;
		}
	}
}
