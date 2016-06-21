using System;
using UnityEngine;

public class tempDummyFixer : MonoBehaviour
{
	public Component[] DestroyIfNotBolt;

	private void Awake()
	{
		if (!BoltNetwork.isRunning)
		{
			Component[] destroyIfNotBolt = this.DestroyIfNotBolt;
			for (int i = 0; i < destroyIfNotBolt.Length; i++)
			{
				Component component = destroyIfNotBolt[i];
				if (component)
				{
					UnityEngine.Object.Destroy(component);
				}
			}
		}
	}
}
