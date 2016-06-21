using System;
using UnityEngine;

public class CoopOnDestroyCallback : MonoBehaviour
{
	public Action Callback;

	private void OnDestroy()
	{
		if (this.Callback != null)
		{
			this.Callback();
			this.Callback = null;
		}
	}
}
