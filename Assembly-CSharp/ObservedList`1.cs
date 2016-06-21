using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[Serializable]
public class ObservedList<T> : List<T>
{
	public event Action<int> Changed
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Changed = (Action<int>)Delegate.Combine(this.Changed, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Changed = (Action<int>)Delegate.Remove(this.Changed, value);
		}
	}

	public new T this[int index]
	{
		get
		{
			return base[index];
		}
		set
		{
			base[index] = value;
			this.Changed(index);
		}
	}

	public ObservedList()
	{
		this.Changed = delegate
		{
		};
		base..ctor();
	}
}
