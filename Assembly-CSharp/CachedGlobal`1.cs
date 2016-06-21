using System;
using UnityEngine;

internal struct CachedGlobal<T> where T : Component
{
	private T component;

	private float lastCheck;

	public T Component
	{
		get
		{
			if (this.component)
			{
				return this.component;
			}
			if (this.lastCheck + 0.5f < Time.realtimeSinceStartup)
			{
				this.lastCheck = Time.realtimeSinceStartup;
				return this.component = UnityEngine.Object.FindObjectOfType<T>();
			}
			return (T)((object)null);
		}
	}
}
