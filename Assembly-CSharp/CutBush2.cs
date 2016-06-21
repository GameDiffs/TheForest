using PathologicalGames;
using System;
using UnityEngine;

public class CutBush2 : MonoBehaviour
{
	public int Health = 4;

	public GameObject Burst;

	public GameObject MyCut;

	public Transform MyBurstPos;

	private int startHealth;

	private void Awake()
	{
		this.startHealth = this.Health;
	}

	private void OnEnable()
	{
		this.Health = this.startHealth;
	}

	private void Hit(int damage)
	{
		UnityEngine.Object.Instantiate(this.Burst, this.MyBurstPos.position, this.MyBurstPos.rotation);
		this.Health -= damage;
		if (this.Health <= 0)
		{
			this.CutDown();
		}
	}

	private void CutDown()
	{
		UnityEngine.Object.Instantiate(this.MyCut, base.transform.position, base.transform.rotation);
		if (PoolManager.Pools["Bushes"].IsSpawned(base.transform))
		{
			PoolManager.Pools["Bushes"].Despawn(base.transform);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Explosion()
	{
		base.Invoke("CutDown", 0.35f);
	}
}
