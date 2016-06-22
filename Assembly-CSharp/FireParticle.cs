using Bolt;
using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class FireParticle : EntityBehaviour<IFireParticle>
{
	public float MyFuel;

	[HideInInspector]
	public FireDamage Parent;

	private ParticleScaler Ps;

	private bool Burning;

	public override void Attached()
	{
		base.state.Transform.SetTransforms(base.transform);
	}

	private void Awake()
	{
		this.Ps = base.gameObject.GetComponent<ParticleScaler>();
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.DelayedEnable(BoltNetwork.isRunning && this.entity && !this.entity.isAttached));
	}

	private void OnDisable()
	{
		this.Parent = null;
	}

	[DebuggerHidden]
	private IEnumerator DelayedEnable(bool delay)
	{
		FireParticle.<DelayedEnable>c__Iterator15F <DelayedEnable>c__Iterator15F = new FireParticle.<DelayedEnable>c__Iterator15F();
		<DelayedEnable>c__Iterator15F.delay = delay;
		<DelayedEnable>c__Iterator15F.<$>delay = delay;
		<DelayedEnable>c__Iterator15F.<>f__this = this;
		return <DelayedEnable>c__Iterator15F;
	}

	private void Update()
	{
		if (!BoltNetwork.isRunning || (this.entity && this.entity.isAttached && this.entity.isOwner))
		{
			if (this.MyFuel > 0f)
			{
				this.Burning = true;
				this.MyFuel -= 1f * Time.deltaTime;
				this.Ps.particleScale = this.MyFuel / 100f;
			}
			else if (this.Burning)
			{
				this.EndFire();
			}
		}
	}

	private void EndFire()
	{
		if (this.Parent)
		{
			this.Parent.ExtinguishPoint(this);
		}
		if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
		{
			BoltNetwork.Detach(base.gameObject);
		}
		SpawnPool spawnPool = PoolManager.Pools["Particles"];
		if (spawnPool.IsSpawned(base.transform))
		{
			spawnPool.Despawn(base.transform);
		}
		else if (base.transform.parent)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
