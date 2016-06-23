using PathologicalGames;
using System;
using UnityEngine;

[RequireComponent(typeof(SmoothLookAtConstraint)), RequireComponent(typeof(Projectile))]
public class DemoSeeker : MonoBehaviour
{
	public float maxVelocity = 500f;

	public float acceleration = 75f;

	private Transform xform;

	private Rigidbody rbd;

	private SmoothLookAtConstraint lookConstraint;

	private Projectile projectile;

	private float minDrag = 10f;

	private float drag = 40f;

	private void Awake()
	{
		this.xform = base.transform;
		this.rbd = base.GetComponent<Rigidbody>();
		this.lookConstraint = base.GetComponent<SmoothLookAtConstraint>();
		this.projectile = base.GetComponent<Projectile>();
		this.projectile.AddOnLaunchedDelegate(new Projectile.OnLaunched(this.OnLaunched));
		this.projectile.AddOnLaunchedUpdateDelegate(new Projectile.OnLaunchedUpdate(this.OnLaunchedUpdate));
		this.projectile.AddOnDetonationDelegate(new Projectile.OnDetonation(this.OnDetonateProjectile));
	}

	private void OnLaunched()
	{
		this.rbd.drag = this.drag;
	}

	private void OnLaunchedUpdate()
	{
		if (!this.projectile.target.isSpawned)
		{
			this.lookConstraint.target = null;
			this.projectile.detonationMode = Projectile.DETONATION_MODES.HitLayers;
		}
		else
		{
			this.lookConstraint.target = this.projectile.target.transform;
			this.projectile.detonationMode = Projectile.DETONATION_MODES.TargetOnly;
		}
		if (this.rbd.drag > this.minDrag)
		{
			this.rbd.drag -= this.acceleration * 0.01f;
		}
		this.rbd.AddForce(this.xform.forward * this.maxVelocity);
	}

	private void OnDetonateProjectile(TargetList targets)
	{
	}
}
