using PathologicalGames;
using System;
using TheForest.Utils;
using UnityEngine;

public class creepyAnimEvents : MonoBehaviour
{
	private Animator animator;

	private mutantScriptSetup setup;

	private clsragdollify ragDollSetup;

	private GameObject weaponLeft;

	private GameObject weaponLeft1;

	private GameObject weaponRight;

	public GameObject weaponMain;

	public Collider weaponMainCollider;

	public bool netPrefab;

	public bool parry;

	[Header("FMOD Events")]
	public string playerSightedSound;

	private void OnDeserialized()
	{
		this.doStart();
	}

	private void Start()
	{
		this.doStart();
	}

	private void doStart()
	{
		this.ragDollSetup = base.transform.GetComponent<clsragdollify>();
		this.animator = base.gameObject.GetComponent<Animator>();
		this.setup = base.transform.GetComponent<mutantScriptSetup>();
		Transform[] componentsInChildren = base.transform.root.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			if (transform.name == "weaponLeftGO")
			{
				this.weaponLeft = transform.gameObject;
			}
			if (transform.name == "weaponLeftGO1")
			{
				this.weaponLeft1 = transform.gameObject;
			}
			if (transform.name == "weaponRightGO")
			{
				this.weaponRight = transform.gameObject;
			}
			if (transform.name == "weaponGO")
			{
				this.weaponMain = transform.gameObject;
				this.weaponMainCollider = transform.GetComponent<Collider>();
			}
		}
	}

	private void disableDamageBool(bool setDamage)
	{
		if (this.netPrefab)
		{
			return;
		}
		this.animator.SetBoolReflected("damageBOOL", false);
		if (UnityEngine.Random.Range(0, 5) < 2)
		{
		}
	}

	private void enableWeapon()
	{
		if (this.weaponLeft)
		{
			this.weaponLeft.GetComponent<Collider>().enabled = true;
		}
		if (this.weaponLeft1)
		{
			this.weaponLeft1.GetComponent<Collider>().enabled = true;
		}
		if (this.weaponRight)
		{
			this.weaponRight.GetComponent<Collider>().enabled = true;
		}
	}

	public void disableWeapon()
	{
		if (this.weaponLeft)
		{
			this.weaponLeft.GetComponent<Collider>().enabled = false;
		}
		if (this.weaponLeft1)
		{
			this.weaponLeft1.GetComponent<Collider>().enabled = false;
		}
		if (this.weaponRight)
		{
			this.weaponRight.GetComponent<Collider>().enabled = false;
		}
	}

	private void enableLeftWeapon()
	{
		if (this.weaponLeft)
		{
			this.weaponLeft.GetComponent<Collider>().enabled = true;
		}
		if (this.weaponLeft1)
		{
			this.weaponLeft1.GetComponent<Collider>().enabled = true;
		}
	}

	private void disableLeftWeapon()
	{
		if (this.weaponLeft)
		{
			this.weaponLeft.GetComponent<Collider>().enabled = false;
		}
		if (this.weaponLeft1)
		{
			this.weaponLeft1.GetComponent<Collider>().enabled = false;
		}
	}

	private void enableRightWeapon()
	{
		if (this.weaponRight)
		{
			this.weaponRight.GetComponent<Collider>().enabled = true;
		}
	}

	private void disableRightWeapon()
	{
		if (this.weaponRight)
		{
			this.weaponRight.GetComponent<Collider>().enabled = false;
		}
	}

	private void enableMainWeapon()
	{
		if (this.weaponMainCollider)
		{
			this.weaponMainCollider.enabled = true;
		}
		base.Invoke("disableMainWeapon", 2f);
	}

	private void disableMainWeapon()
	{
		if (this.weaponMainCollider)
		{
			this.weaponMainCollider.enabled = false;
		}
	}

	private void disableAllWeapons()
	{
		if (this.weaponMainCollider)
		{
			this.weaponMainCollider.enabled = false;
		}
		if (this.weaponLeft)
		{
			this.weaponLeft.GetComponent<Collider>().enabled = false;
		}
		if (this.weaponLeft1)
		{
			this.weaponLeft1.GetComponent<Collider>().enabled = false;
		}
		if (this.weaponRight)
		{
			this.weaponRight.GetComponent<Collider>().enabled = false;
		}
	}

	private void footStomp()
	{
		if (!LocalPlayer.Transform)
		{
			return;
		}
		if (this.setup.ai.playerDist < 30f)
		{
			LocalPlayer.HitReactions.enableFootShake(this.setup.ai.playerDist, 0.3f);
		}
	}

	private void footStompFast()
	{
		if (!LocalPlayer.Transform)
		{
			return;
		}
		if (this.setup.ai.playerDist < 30f)
		{
			LocalPlayer.HitReactions.enableFootShake(this.setup.ai.playerDist, 0.15f);
		}
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

	private void playerSighted()
	{
		FMODCommon.PlayOneshot(this.playerSightedSound, base.transform);
	}
}
