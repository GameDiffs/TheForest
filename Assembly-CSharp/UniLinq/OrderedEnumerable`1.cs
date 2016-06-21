using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	internal abstract class OrderedEnumerable<TElement> : IEnumerable, IEnumerable<TElement>, IOrderedEnumerable<TElement>
	{
		private IEnumerable<TElement> source;

		protected OrderedEnumerable(IEnumerable<TElement> source)
		{
			this.source = source;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual IEnumerator<TElement> GetEnumerator()
		{
			return this.Sort(this.source).GetEnumerator();
		}

		public abstract SortContext<TElement> CreateContext(SortContext<TElement> current);

		protected abstract IEnumerable<TElement> Sort(IEnumerable<TElement> source);

		public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> selector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedSequence<TElement, TKey>(this, this.source, selector, comparer, (!descending) ? SortDirection.Ascending : SortDirection.Descending);
		}
	}
}
