using System;
using UnityEngine;

public class mutantHitReceiver : MonoBehaviour
{
	private EnemyHealth health;

	public bool isHead;

	private bool doused;

	private bool burning;

	private bool poisonned;

	public bool inNooseTrap;

	private void Awake()
	{
		this.health = base.transform.root.GetComponentInChildren<EnemyHealth>();
	}

	public void Douse()
	{
		if (!this.doused)
		{
			this.doused = true;
			if (this.health)
			{
				this.health.setFireDouse();
			}
			base.Invoke("resetDoused", 0.5f);
		}
	}

	public void Burn()
	{
		if (this.isHead)
		{
			return;
		}
		if (!this.burning)
		{
			if (this.health)
			{
				this.health.Burn();
			}
			this.burning = true;
			base.Invoke("resetBurning", 5f);
		}
	}

	public void Poison()
	{
		if (!this.poisonned)
		{
			if (this.health)
			{
				this.health.Poison();
			}
			this.poisonned = true;
			base.Invoke("resetPoisonned", 10f);
		}
	}

	private void resetDoused()
	{
		this.doused = false;
	}

	private void resetBurning()
	{
		this.burning = false;
	}

	private void resetPoisonned()
	{
		this.poisonned = false;
	}

	private void getSkinHitPosition(Transform hitTr)
	{
		if (!this.health)
		{
			return;
		}
		Vector3 vector = base.transform.InverseTransformPoint(hitTr.position);
		if (vector.y > 1.1f)
		{
			this.health.setSkinDamage(0);
		}
		else if (vector.x > 0f)
		{
			this.health.setSkinDamage(1);
		}
		else
		{
			this.health.setSkinDamage(2);
		}
	}

	public void getAttackDirection(int dir)
	{
		if (this.health)
		{
			this.health.getAttackDirection(dir);
		}
	}

	public void getStealthAttack()
	{
		if (this.health)
		{
			this.health.getStealthAttack();
		}
	}

	public void takeDamage(int dir)
	{
		if (this.health)
		{
			this.health.takeDamage(dir);
		}
	}

	public void getCombo(int combo)
	{
		if (this.health)
		{
			this.health.getCombo(combo);
		}
	}

	public void hitRelay(int damage)
	{
		if (this.health)
		{
			this.health.Hit(damage);
		}
	}
}
