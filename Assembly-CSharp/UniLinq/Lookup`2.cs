using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UniLinq
{
	public class Lookup<TKey, TElement> : IEnumerable, IEnumerable<IGrouping<TKey, TElement>>, ILookup<TKey, TElement>
	{
		private IGrouping<TKey, TElement> nullGrouping;

		private Dictionary<TKey, IGrouping<TKey, TElement>> groups;

		public int Count
		{
			get
			{
				return (this.nullGrouping != null) ? (this.groups.Count + 1) : this.groups.Count;
			}
		}

		public IEnumerable<TElement> this[TKey key]
		{
			get
			{
				if (key == null && this.nullGrouping != null)
				{
					return this.nullGrouping;
				}
				IGrouping<TKey, TElement> result;
				if (key != null && this.groups.TryGetValue(key, out result))
				{
					return result;
				}
				return new TElement[0];
			}
		}

		internal Lookup(Dictionary<TKey, List<TElement>> lookup, IEnumerable<TElement> nullKeyElements)
		{
			this.groups = new Dictionary<TKey, IGrouping<TKey, TElement>>(lookup.Comparer);
			foreach (KeyValuePair<TKey, List<TElement>> current in lookup)
			{
				this.groups.Add(current.Key, new Grouping<TKey, TElement>(current.Key, current.Value));
			}
			if (nullKeyElements != null)
			{
				this.nullGrouping = new Grouping<TKey, TElement>(default(TKey), nullKeyElements);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		[DebuggerHidden]
		public IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			Lookup<TKey, TElement>.<ApplyResultSelector>c__Iterator207<TResult> <ApplyResultSelector>c__Iterator = new Lookup<TKey, TElement>.<ApplyResultSelector>c__Iterator207<TResult>();
			<ApplyResultSelector>c__Iterator.resultSelector = resultSelector;
			<ApplyResultSelector>c__Iterator.<$>resultSelector = resultSelector;
			<ApplyResultSelector>c__Iterator.<>f__this = this;
			Lookup<TKey, TElement>.<ApplyResultSelector>c__Iterator207<TResult> expr_1C = <ApplyResultSelector>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}

		public bool Contains(TKey key)
		{
			return (key == null) ? (this.nullGrouping != null) : this.groups.ContainsKey(key);
		}

		[DebuggerHidden]
		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
		{
			Lookup<TKey, TElement>.<GetEnumerator>c__Iterator208 <GetEnumerator>c__Iterator = new Lookup<TKey, TElement>.<GetEnumerator>c__Iterator208();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}
	}
}
