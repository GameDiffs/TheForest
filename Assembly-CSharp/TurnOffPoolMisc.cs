using PathologicalGames;
using System;
using UnityEngine;

public class TurnOffPoolMisc : MonoBehaviour
{
	public float Wait;

	private void Start()
	{
		base.Invoke("TurnOff", this.Wait);
	}

	private void TurnOff()
	{
		PoolManager.Pools["Misc"].Despawn(base.transform);
	}
}
