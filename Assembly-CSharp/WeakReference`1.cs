using System;

public class WeakReference<T> where T : class
{
	private WeakReference r;

	public T Target
	{
		get
		{
			return (!this.r.IsAlive) ? ((T)((object)null)) : ((T)((object)this.r.Target));
		}
		set
		{
			this.r = new WeakReference(value);
		}
	}

	public bool IsAlive
	{
		get
		{
			return this.r.IsAlive;
		}
	}

	public static implicit operator T(WeakReference<T> re)
	{
		return re.Target;
	}

	public static implicit operator WeakReference<T>(T value)
	{
		return new WeakReference<T>
		{
			Target = value
		};
	}
}
