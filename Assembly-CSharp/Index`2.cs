using System;
using System.Runtime.CompilerServices;

public class Index<TK, TR> : Lookup<TK, TR>, IChanged where TR : class, new()
{
	public event Action<TK, TR, TR> Setting
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Setting = (Action<TK, TR, TR>)Delegate.Combine(this.Setting, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Setting = (Action<TK, TR, TR>)Delegate.Remove(this.Setting, value);
		}
	}

	public event Action<TK, TR> Getting
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Getting = (Action<TK, TR>)Delegate.Combine(this.Getting, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Getting = (Action<TK, TR>)Delegate.Remove(this.Getting, value);
		}
	}

	public override TR this[TK index]
	{
		get
		{
			if (this.ContainsKey(index))
			{
				return base[index];
			}
			TR tR = Activator.CreateInstance<TR>();
			if (tR is INeedParent)
			{
				(tR as INeedParent).SetParent(this, index);
			}
			base[index] = tR;
			this.Getting(index, tR);
			return tR;
		}
		set
		{
			if (this.Setting != null)
			{
				TR arg = (TR)((object)null);
				if (base.ContainsKey(index))
				{
					arg = base[index];
				}
				this.Setting(index, arg, value);
			}
			base[index] = value;
		}
	}

	public Index()
	{
		this.Getting = delegate
		{
		};
		base..ctor();
	}

	public void Changed(object index)
	{
		if (this.Setting != null)
		{
			TR tR = (TR)((object)null);
			if (base.ContainsKey((TK)((object)index)))
			{
				tR = base[(TK)((object)index)];
			}
			this.Setting((TK)((object)index), tR, tR);
		}
	}
}
