using System;
using System.Collections.Generic;

namespace UniLinq
{
	internal abstract class SortContext<TElement> : IComparer<int>
	{
		protected SortDirection direction;

		protected SortContext<TElement> child_context;

		protected SortContext(SortDirection direction, SortContext<TElement> child_context)
		{
			this.direction = direction;
			this.child_context = child_context;
		}

		public abstract void Initialize(TElement[] elements);

		public abstract int Compare(int first_index, int second_index);
	}
}
