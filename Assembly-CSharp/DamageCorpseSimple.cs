using Bolt;
using System;
using UnityEngine;

public class DamageCorpseSimple : EntityBehaviour
{
	public int Health = 20;

	public GameObject BloodSplat;

	public GameObject MyCut;

	public GameObject MyGore;

	private bool ignoreHit;

	public void DoLocalCut(int health)
	{
		if (health >= 20)
		{
			return;
		}
		UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate(this.BloodSplat, base.transform.position, Quaternion.identity), 0.5f);
		this.MyGore.SetActive(true);
		this.Health = health;
		if (health <= 0)
		{
			this.CutDown();
		}
	}

	private void ignoreCutting()
	{
		this.ignoreHit = true;
	}

	private void Hit(int damage)
	{
		if (this.ignoreHit)
		{
			this.ignoreHit = false;
			return;
		}
		this.DoLocalCut(this.Health - damage);
	}

	private void CutDown()
	{
		this.MyCut.SetActive(true);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Explosion(float dist)
	{
		base.Invoke("CutDown", 1f);
	}
}
