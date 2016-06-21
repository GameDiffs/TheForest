using PathologicalGames;
using System;
using UnityEngine;

public class babyAnimEvents : MonoBehaviour
{
	private mutantAI ai;

	private clsragdollify ragDollSetup;

	public CapsuleCollider weaponCollider;

	private pushRigidBody pushBody;

	public bool netPrefab;

	private void Start()
	{
		this.ragDollSetup = base.transform.GetComponent<clsragdollify>();
		this.ai = base.GetComponent<mutantAI>();
		this.weaponCollider = base.GetComponentInChildren<enemyWeaponMelee>().gameObject.GetComponent<CapsuleCollider>();
		this.pushBody = base.transform.parent.GetComponent<pushRigidBody>();
	}

	private void enableTurning()
	{
		if (this.netPrefab)
		{
			return;
		}
		this.ai.fsmRotateSpeed.Value = 2f;
	}

	public void disableTurning()
	{
		if (this.netPrefab)
		{
			return;
		}
		this.ai.fsmRotateSpeed.Value = 0f;
	}

	private void enableWeapon()
	{
		this.weaponCollider.enabled = true;
		base.Invoke("disableWeapon", 1.5f);
	}

	private void disableWeapon()
	{
		this.weaponCollider.enabled = false;
	}

	private void enableBreakWood()
	{
		if (this.netPrefab)
		{
			return;
		}
		this.pushBody.dontBreakCrates = false;
	}

	private void disableBreakWood()
	{
		if (this.netPrefab)
		{
			return;
		}
		this.pushBody.dontBreakCrates = true;
	}

	private void enableRagDoll()
	{
		if (this.netPrefab)
		{
			return;
		}
		this.ragDollSetup.metgoragdoll(default(Vector3));
		if (PoolManager.Pools["enemies"].IsSpawned(base.transform.parent))
		{
			PoolManager.Pools["enemies"].Despawn(base.transform.parent);
		}
		else
		{
			UnityEngine.Object.Destroy(base.transform.root.gameObject);
		}
	}
}
