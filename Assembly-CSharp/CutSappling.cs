using PathologicalGames;
using System;
using UnityEngine;

public class CutSappling : MonoBehaviour
{
	public GameObject Sticks;

	private int Health = 2;

	[Header("FMOD")]
	private string breakEvent;

	private int startHealth;

	private void Awake()
	{
		this.startHealth = this.Health;
	}

	private void OnEnable()
	{
		this.Health = this.startHealth;
	}

	private void Hit()
	{
		if (this.Health > 0)
		{
			this.Health--;
			if (this.Health <= 0)
			{
				this.CutDown();
			}
		}
	}

	private void Explosion(float dist)
	{
		if (this.Health > 0)
		{
			this.Health = 0;
			this.CutDown();
		}
	}

	private void CutDown()
	{
		if (this.breakEvent != null)
		{
			FMOD_StudioSystem.instance.PlayOneShot(this.breakEvent, base.transform.position, null);
		}
		if (PoolManager.Pools["Bushes"].IsSpawned(base.transform))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.Sticks);
			gameObject.SetActive(true);
			gameObject.transform.parent = null;
			PoolManager.Pools["Bushes"].Despawn(base.transform);
		}
		else
		{
			this.Sticks.SetActive(true);
			this.Sticks.transform.parent = null;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
