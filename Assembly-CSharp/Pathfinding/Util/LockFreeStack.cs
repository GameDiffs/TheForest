using System;
using System.Threading;

namespace Pathfinding.Util
{
	public class LockFreeStack
	{
		public Path head;

		public void Push(Path p)
		{
			Path path;
			do
			{
				p.next = this.head;
				path = Interlocked.CompareExchange<Path>(ref this.head, p, p.next);
			}
			while (path != p.next);
		}

		public Path PopAll()
		{
			return Interlocked.Exchange<Path>(ref this.head, null);
		}
	}
}
