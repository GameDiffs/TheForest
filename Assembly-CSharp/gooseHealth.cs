using System;
using TheForest.Utils;
using UnityEngine;

public class gooseHealth : MonoBehaviour
{
	public int health;

	public gooseRagdollify ragdoll;

	private newGooseAi ai;

	private Rigidbody rb;

	private void Start()
	{
		this.ai = base.transform.parent.GetComponent<newGooseAi>();
		this.rb = base.GetComponentInChildren<Rigidbody>();
		this.ragdoll = base.GetComponent<gooseRagdollify>();
	}

	private void Update()
	{
		if (this.health < 1)
		{
			this.die();
		}
	}

	private void Hit(int damage)
	{
		this.health -= damage;
		if (this.health < 1)
		{
			this.die();
		}
	}

	private void die()
	{
		GameStats.BirdKilled.Invoke();
		if (this.ai.leader)
		{
			this.ai.controller.StartCoroutine("resetLeader");
		}
		this.ragdoll.getVelocity = this.rb.velocity;
		if (this.ai.flying)
		{
			this.ragdoll.spinRagdoll = true;
		}
		this.ragdoll.metgoragdoll(default(Vector3));
		UnityEngine.Object.Destroy(base.transform.root.gameObject);
	}
}
