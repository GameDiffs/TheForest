using PathologicalGames;
using System;
using UnityEngine;

public class batHealth : MonoBehaviour
{
	public int health = 2;

	public GameObject fire;

	private clsragdollify cls;

	private void Start()
	{
		this.cls = base.transform.GetComponentInChildren<clsragdollify>();
	}

	private void OnEnable()
	{
		this.health = 2;
		if (this.fire)
		{
			this.fire.SetActive(false);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("hitFire");
	}

	private void Burn()
	{
		if (this.fire)
		{
			this.fire.SetActive(true);
		}
		base.InvokeRepeating("hitFIre", 3f, 3f);
	}

	private void hitFire()
	{
		this.Hit(1);
	}

	private void Hit(int damage)
	{
		this.health -= damage;
		if (this.health <= 0)
		{
			this.die();
		}
	}

	private void die()
	{
		base.CancelInvoke("hitFire");
		this.cls.metgoragdoll(default(Vector3));
		if (PoolManager.Pools["creatures"].IsSpawned(base.transform))
		{
			PoolManager.Pools["creatures"].Despawn(base.transform);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
