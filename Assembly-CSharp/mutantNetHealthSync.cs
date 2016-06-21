using System;
using UnityEngine;

public class mutantNetHealthSync : MonoBehaviour
{
	private Animator animator;

	public int health;

	private int damageMult;

	private bool hitBlock;

	private void Start()
	{
		this.animator = base.transform.GetComponent<Animator>();
	}

	private void Update()
	{
	}

	private void Hit(int damage)
	{
		if (this.hitBlock)
		{
			return;
		}
		this.health = this.animator.GetInteger("ClientHealth");
		if (this.animator.GetBool("deathBOOL"))
		{
			this.damageMult = 2;
		}
		else
		{
			this.damageMult = 1;
		}
		if (this.animator.GetBool("sleepBOOL"))
		{
			this.health -= 100;
		}
		this.health -= damage * this.damageMult;
		if (this.health <= 80 && this.health >= 35)
		{
			this.animator.SetIntegerReflected("ClientHurtLevelInt", 1);
		}
		if (this.health < 35 && this.health >= 25)
		{
			this.animator.SetIntegerReflected("ClientHurtLevelInt", 2);
		}
		if (this.health < 25 && this.health >= 1 && this.animator)
		{
			this.animator.SetIntegerReflected("ClientHurtLevelInt", 3);
		}
		if (this.health < 1 && this.animator)
		{
			this.animator.SetIntegerReflected("ClientHurtLevelInt", 4);
			this.animator.SetBoolReflected("deathfinalBOOL", true);
		}
	}

	private void hitBlockReset()
	{
		this.hitBlock = false;
	}
}
