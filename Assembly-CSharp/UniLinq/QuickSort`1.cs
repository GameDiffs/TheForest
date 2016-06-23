using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UniLinq
{
	internal class QuickSort<TElement>
	{
		private TElement[] elements;

		private int[] indexes;

		private SortContext<TElement> context;

		private QuickSort(IEnumerable<TElement> source, SortContext<TElement> context)
		{
			List<TElement> list = new List<TElement>();
			foreach (TElement current in source)
			{
				list.Add(current);
			}
			this.elements = list.ToArray();
			this.indexes = QuickSort<TElement>.CreateIndexes(this.elements.Length);
			this.context = context;
		}

		private static int[] CreateIndexes(int length)
		{
			int[] array = new int[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = i;
			}
			return array;
		}

		private void PerformSort()
		{
			if (this.elements.Length <= 1)
			{
				return;
			}
			this.context.Initialize(this.elements);
			Array.Sort<int>(this.indexes, this.context);
		}

		[DebuggerHidden]
		public static IEnumerable<TElement> Sort(IEnumerable<TElement> source, SortContext<TElement> context)
		{
			QuickSort<TElement>.<Sort>c__Iterator209 <Sort>c__Iterator = new QuickSort<TElement>.<Sort>c__Iterator209();
			<Sort>c__Iterator.source = source;
			<Sort>c__Iterator.context = context;
			<Sort>c__Iterator.<$>source = source;
			<Sort>c__Iterator.<$>context = context;
			QuickSort<TElement>.<Sort>c__Iterator209 expr_23 = <Sort>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}
	}
}
