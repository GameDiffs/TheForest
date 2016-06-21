using System;
using System.Collections.Generic;

namespace UniLinq
{
	internal class OrderedSequence<TElement, TKey> : OrderedEnumerable<TElement>
	{
		private OrderedEnumerable<TElement> parent;

		private Func<TElement, TKey> selector;

		private IComparer<TKey> comparer;

		private SortDirection direction;

		internal OrderedSequence(IEnumerable<TElement> source, Func<TElement, TKey> key_selector, IComparer<TKey> comparer, SortDirection direction) : base(source)
		{
			this.selector = key_selector;
			this.comparer = (comparer ?? Comparer<TKey>.Default);
			this.direction = direction;
		}

		internal OrderedSequence(OrderedEnumerable<TElement> parent, IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, SortDirection direction) : this(source, keySelector, comparer, direction)
		{
			this.parent = parent;
		}

		public override IEnumerator<TElement> GetEnumerator()
		{
			return base.GetEnumerator();
		}

		public override SortContext<TElement> CreateContext(SortContext<TElement> current)
		{
			SortContext<TElement> sortContext = new SortSequenceContext<TElement, TKey>(this.selector, this.comparer, this.direction, current);
			if (this.parent != null)
			{
				return this.parent.CreateContext(sortContext);
			}
			return sortContext;
		}

		protected override IEnumerable<TElement> Sort(IEnumerable<TElement> source)
		{
			return QuickSort<TElement>.Sort(source, this.CreateContext(null));
		}
	}
}
