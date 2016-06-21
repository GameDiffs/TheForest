using System;
using System.Collections.Generic;

public class ValueLookup<TK, TR> : Dictionary<TK, TR> where TR : struct
{
	public new virtual TR this[TK index]
	{
		get
		{
			if (this.ContainsKey(index))
			{
				return base[index];
			}
			return default(TR);
		}
		set
		{
			base[index] = value;
		}
	}

	public T Get<T>(TK index) where T : class
	{
		return this[index] as T;
	}
}
