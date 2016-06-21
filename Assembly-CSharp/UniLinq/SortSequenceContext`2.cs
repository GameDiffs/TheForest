using System;
using System.Collections.Generic;

namespace UniLinq
{
	internal class SortSequenceContext<TElement, TKey> : SortContext<TElement>
	{
		private Func<TElement, TKey> selector;

		private IComparer<TKey> comparer;

		private TKey[] keys;

		public SortSequenceContext(Func<TElement, TKey> selector, IComparer<TKey> comparer, SortDirection direction, SortContext<TElement> child_context) : base(direction, child_context)
		{
			this.selector = selector;
			this.comparer = comparer;
		}

		public override void Initialize(TElement[] elements)
		{
			if (this.child_context != null)
			{
				this.child_context.Initialize(elements);
			}
			this.keys = new TKey[elements.Length];
			for (int i = 0; i < this.keys.Length; i++)
			{
				this.keys[i] = this.selector(elements[i]);
			}
		}

		public override int Compare(int first_index, int second_index)
		{
			int num = this.comparer.Compare(this.keys[first_index], this.keys[second_index]);
			if (num == 0)
			{
				if (this.child_context != null)
				{
					return this.child_context.Compare(first_index, second_index);
				}
				num = ((this.direction != SortDirection.Descending) ? (first_index - second_index) : (second_index - first_index));
			}
			return (this.direction != SortDirection.Descending) ? num : (-num);
		}
	}
}
