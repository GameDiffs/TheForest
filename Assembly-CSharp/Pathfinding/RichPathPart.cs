using Pathfinding.Util;
using System;

namespace Pathfinding
{
	public abstract class RichPathPart : IAstarPooledObject
	{
		public abstract void OnEnterPool();
	}
}
