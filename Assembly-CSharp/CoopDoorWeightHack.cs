using System;
using UnityEngine;

public class CoopDoorWeightHack : MonoBehaviour
{
	[SerializeField]
	private GameObject[] destroy;

	private void Update()
	{
		if (BoltNetwork.isRunning)
		{
			for (int i = 0; i < this.destroy.Length; i++)
			{
				if (this.destroy[i])
				{
					UnityEngine.Object.Destroy(this.destroy[i]);
				}
			}
		}
	}
}
