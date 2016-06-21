using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	public interface IGrouping<TKey, TElement> : IEnumerable, IEnumerable<TElement>
	{
		TKey Key
		{
			get;
		}
	}
}
