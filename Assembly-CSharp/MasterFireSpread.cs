using Bolt;
using PathologicalGames;
using System;
using UnityEngine;

public class MasterFireSpread : EntityBehaviour
{
	public bool LegacyFuel = true;

	public float Fuel;

	public float FireSpread = 3f;

	public Transform FireParticle;

	public bool parentFireParticle;

	public bool ignoreSameHierarchy;

	public bool destroyWhenOutOfFuel;

	public GameObject DestroyTarget;

	public Transform owner;

	private bool FireStarted;

	private Transform burnP;

	public override void Attached()
	{
		if (!this.entity.isOwner)
		{
			base.enabled = true;
		}
	}

	private void Start()
	{
		if (this.ignoreSameHierarchy && !this.owner)
		{
			this.owner = base.transform.root;
		}
		this.StartFire();
	}

	private void Update()
	{
		if (this.Fuel > 0f)
		{
			this.Fuel -= 2f * Time.deltaTime;
			this.SpreadFire(base.transform.position, this.FireSpread);
		}
		else if (this.destroyWhenOutOfFuel)
		{
			if (!BoltNetwork.isRunning || !this.entity || !this.entity.isAttached)
			{
				UnityEngine.Object.Destroy((!this.DestroyTarget) ? base.gameObject : this.DestroyTarget);
			}
			else if (this.entity.isOwner)
			{
				base.transform.parent = null;
				BoltNetwork.Destroy((!this.DestroyTarget) ? base.gameObject : this.DestroyTarget);
			}
		}
	}

	private void OnDestroy()
	{
		if (this.burnP)
		{
			if (this.parentFireParticle)
			{
				this.burnP.parent = null;
			}
			PoolManager.Pools["Particles"].Despawn(this.burnP);
		}
	}

	private void StartFire()
	{
		if (!this.FireStarted)
		{
			if (this.LegacyFuel)
			{
				this.Fuel = 50f;
			}
			this.FireStarted = true;
			if (this.FireParticle)
			{
				if (this.parentFireParticle)
				{
					this.burnP = PoolManager.Pools["Particles"].Spawn(this.FireParticle, base.transform.position, Quaternion.identity, base.transform).transform;
				}
				else
				{
					this.burnP = PoolManager.Pools["Particles"].Spawn(this.FireParticle, base.transform.position, Quaternion.identity).transform;
				}
			}
		}
	}

	private void SpreadFire(Vector3 center, float FireSpead)
	{
		Collider[] array = Physics.OverlapSphere(center, FireSpead);
		for (int i = 0; i < array.Length; i++)
		{
			Collider collider = array[i];
			if (!this.ignoreSameHierarchy || this.owner != collider.transform.root)
			{
				collider.SendMessage("Douse", SendMessageOptions.DontRequireReceiver);
				collider.SendMessage("Burn", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
