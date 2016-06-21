using System;
using System.Collections.Generic;

public class Lookup<TK, TR> : Dictionary<TK, TR> where TR : class
{
	public new virtual TR this[TK index]
	{
		get
		{
			if (this.ContainsKey(index))
			{
				return base[index];
			}
			return (TR)((object)null);
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
