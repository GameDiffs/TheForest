using PathologicalGames;
using System;
using UnityEngine;

public class TurnOffPool : MonoBehaviour
{
	public int Wait;

	private void Start()
	{
		base.Invoke("TurnOff", (float)this.Wait);
	}

	private void TurnOff()
	{
		PoolManager.Pools["Bushes"].Despawn(base.transform);
	}
}
