using PathologicalGames;
using System;
using UnityEngine;

public class TurnOffPoolPickUp : MonoBehaviour
{
	public int Wait;

	private void OnEnable()
	{
		base.CancelInvoke("TurnOff");
		base.Invoke("TurnOff", (float)this.Wait);
	}

	private void TurnOff()
	{
		if (!BoltNetwork.isClient)
		{
			if (PoolManager.Pools["PickUps"].IsSpawned(base.transform))
			{
				if (BoltNetwork.isServer)
				{
					BoltEntity component = base.GetComponent<BoltEntity>();
					if (component && component.isAttached)
					{
						BoltNetwork.Detach(base.gameObject);
					}
				}
				PoolManager.Pools["PickUps"].Despawn(base.transform);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
