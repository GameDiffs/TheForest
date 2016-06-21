using PathologicalGames;
using System;
using UnityEngine;

public class TurnOffPoolPS : MonoBehaviour
{
	public int Wait;

	private void Start()
	{
		base.Invoke("TurnOff", (float)this.Wait);
	}

	private void TurnOff()
	{
		PoolManager.Pools["Particles"].Despawn(base.transform);
	}
}
