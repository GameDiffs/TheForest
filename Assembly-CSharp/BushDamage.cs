using Bolt;
using System;
using UnityEngine;

public class BushDamage : EntityBehaviour
{
	public int Health;

	public Transform Burst;

	public Transform MyCut;

	public Transform MyBurstPos;

	public GameObject DestroyTarget;

	private void OnEnable()
	{
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
		if (this.entity.IsAttached() && this.entity.StateIs<IGardenDirtPileState>())
		{
			DestroyPickUp destroyPickUp = DestroyPickUp.Create(GlobalTargets.OnlyServer);
			destroyPickUp.PickUpEntity = this.entity;
			destroyPickUp.Send();
		}
		else
		{
			this.CutDownReal();
		}
	}

	public void CutDownReal()
	{
		Transform transform = (Transform)UnityEngine.Object.Instantiate(this.MyCut, base.transform.position, base.transform.rotation);
		transform.localScale = base.transform.localScale;
		UnityEngine.Object.Destroy((!this.DestroyTarget) ? base.gameObject : this.DestroyTarget);
	}

	private void Explosion(float dist)
	{
		base.Invoke("CutDown", 1f);
	}
}
