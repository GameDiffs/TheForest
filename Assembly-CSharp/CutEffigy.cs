using System;
using UnityEngine;

public class CutEffigy : MonoBehaviour
{
	private int Health = 1;

	public GameObject Loot;

	[Header("FMOD")]
	public string breakEvent = "event:/physics/wood/wood_small_break";

	private bool breakEventPlayed;

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
		if (!this.breakEventPlayed)
		{
			FMODCommon.PlayOneshot(this.breakEvent, base.transform);
			this.breakEventPlayed = true;
			this.Loot.SetActive(true);
			this.Loot.transform.parent = null;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
