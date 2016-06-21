using FMOD.Studio;
using System;
using TheForest.Utils;
using UnityEngine;

public class animalHealth : MonoBehaviour
{
	public enum HitResult
	{
		Alive,
		Dead
	}

	private PlayMakerFSM pmBase;

	public GameObject ClubTrigger;

	private animalAI ai;

	private animalSpawnFunctions spawnFunctions;

	private sceneTracker scene;

	private Animator animator;

	public int Health;

	public GameObject Fire;

	private float distance;

	public string HitEvent;

	public string DieEvent;

	public string onFireEvent;

	public GameObject RagDoll;

	public GameObject RagDollFire;

	public GameObject RagDollTrap;

	private int RandomSplurt;

	private GameObject AiControl;

	public GameObject MyBody;

	public GameObject BloodSplat1;

	public GameObject BloodSplat2;

	public GameObject BloodSplat3;

	public GameObject BloodSplat4;

	public GameObject BloodSplat5;

	public Material Blood1;

	public Material Blood2;

	public Material Blood3;

	public Material Blood4;

	private mutantPropManager MP;

	private int startHealth;

	private EventInstance onFireEventInstance;

	private bool hitSoundEnabled = true;

	private void Awake()
	{
		this.ai = base.GetComponent<animalAI>();
		this.spawnFunctions = base.GetComponent<animalSpawnFunctions>();
		this.scene = Scene.SceneTracker;
		this.MP = base.gameObject.GetComponent<mutantPropManager>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.startHealth = this.Health;
	}

	private void Start()
	{
		PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
		PlayMakerFSM[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			if (playMakerFSM.FsmName == "aiBaseFSM")
			{
				this.pmBase = playMakerFSM;
			}
		}
	}

	private void Update()
	{
		this.UpdateOnFireEvent();
	}

	private void OnDisabled()
	{
		base.CancelInvoke("HitPoison");
		base.CancelInvoke("disablePoison");
		base.CancelInvoke("HitFire");
		base.StopAllCoroutines();
		if (this.Fire)
		{
			this.Fire.SetActive(false);
		}
	}

	public void Poison()
	{
		base.CancelInvoke("HitPoison");
		base.CancelInvoke("disablePoison");
		if (this.HitWithoutSound(2) == animalHealth.HitResult.Alive)
		{
			base.InvokeRepeating("HitPoison", 0.5f, UnityEngine.Random.Range(5f, 6f));
			base.Invoke("disablePoison", UnityEngine.Random.Range(40f, 60f));
			this.PlayEvent(this.HitEvent);
		}
	}

	private void disablePoison()
	{
		base.CancelInvoke("HitPoison");
	}

	private void HitPoison()
	{
		int min = 2;
		int max = 4;
		int damage = UnityEngine.Random.Range(min, max);
		this.Hit(damage);
	}

	public void FireDamage()
	{
		if (this.Fire)
		{
			this.Fire.SetActive(true);
		}
		this.Hit(10);
		base.SendMessageUpwards("getAttackDirection", 3);
		base.InvokeRepeating("HitFire", 1f, 1f);
	}

	private void PlayEvent(string path)
	{
		FMODCommon.PlayOneshotNetworked(path, base.transform, FMODCommon.NetworkRole.Server);
	}

	public void Burn()
	{
		if (this.Fire)
		{
			this.Fire.SetActive(true);
		}
		animalHealth.HitResult hitResult = this.HitWithoutSound(3);
		if (this.spawnFunctions.deer)
		{
			this.StartOnFireEvent();
			this.PlayEvent(this.DieEvent);
		}
		if (hitResult == animalHealth.HitResult.Alive)
		{
		}
		base.InvokeRepeating("HitFire", 3f, 3f);
		base.Invoke("cancelFire", 15f);
	}

	private void TrapDamage()
	{
		this.DieTrap();
	}

	private void HitFire()
	{
		this.Health -= 5;
		this.HitWithoutSound(1);
	}

	private void cancelFire()
	{
		if (this.Fire)
		{
			this.Fire.SetActive(false);
		}
		base.CancelInvoke("HitFire");
		this.StopOnFireEvent();
	}

	private void HitReal(int damage)
	{
		this.Hit(damage);
	}

	private animalHealth.HitResult HitWithoutSound(int damage)
	{
		animalHealth.HitResult result;
		try
		{
			this.hitSoundEnabled = false;
			result = this.Hit(damage);
		}
		finally
		{
			this.hitSoundEnabled = true;
		}
		return result;
	}

	public animalHealth.HitResult Hit(int damage)
	{
		if (this.spawnFunctions.tortoise && this.animator.GetBool("inShell"))
		{
			return animalHealth.HitResult.Alive;
		}
		this.Health -= damage;
		this.Blood();
		this.pmBase.SendEvent("gotHit");
		this.RandomSplurt = UnityEngine.Random.Range(0, 3);
		if (this.RandomSplurt == 2)
		{
			if (this.MP)
			{
				if (this.MP.MyRandom == 0)
				{
					this.MyBody.GetComponent<Renderer>().material = this.Blood1;
				}
				if (this.MP.MyRandom == 1)
				{
					this.MyBody.GetComponent<Renderer>().material = this.Blood2;
				}
				if (this.MP.MyRandom == 2)
				{
					this.MyBody.GetComponent<Renderer>().material = this.Blood3;
				}
				if (this.MP.MyRandom == 3)
				{
					this.MyBody.GetComponent<Renderer>().material = this.Blood4;
				}
			}
			LocalPlayer.Transform.SendMessage("GotBloody");
		}
		if (this.Health < 1)
		{
			this.Die();
			return animalHealth.HitResult.Dead;
		}
		if (this.hitSoundEnabled)
		{
			this.PlayEvent(this.HitEvent);
		}
		return animalHealth.HitResult.Alive;
	}

	private void Blood()
	{
		if (this.BloodSplat1)
		{
			if (!this.BloodSplat1.activeSelf)
			{
				this.BloodSplat1.SetActive(true);
			}
			else if (!this.BloodSplat2.activeSelf)
			{
				this.BloodSplat2.SetActive(true);
			}
			else if (!this.BloodSplat3.activeSelf)
			{
				this.BloodSplat3.SetActive(true);
			}
		}
	}

	private void StopBlood()
	{
	}

	private void getCurrentHealth()
	{
		this.pmBase.FsmVariables.GetFsmInt("statHealth").Value = this.Health;
	}

	private void Die()
	{
		AnimalDespawner component = base.GetComponent<AnimalDespawner>();
		if (component && component.SpawnedFromZone)
		{
			component.SpawnedFromZone.MaxAnimalsTotal--;
			component.SpawnedFromZone.AddSpawnBackTimes.Enqueue(Time.time + component.SpawnedFromZone.DelayAfterKillTime);
		}
		if (this.spawnFunctions.lizard)
		{
			GameStats.LizardKilled.Invoke();
			this.scene.maxLizardAmount--;
		}
		if (this.spawnFunctions.rabbit)
		{
			GameStats.RabbitKilled.Invoke();
			this.scene.maxRabbitAmount -= 2;
		}
		if (this.spawnFunctions.turtle)
		{
			GameStats.TurtleKilled.Invoke();
			this.scene.maxTurtleAmount--;
			if (this.spawnFunctions.controller)
			{
				this.spawnFunctions.controller.addToSpawnDelay(120f);
			}
		}
		if (this.spawnFunctions.tortoise)
		{
			GameStats.TurtleKilled.Invoke();
			this.scene.maxTortoiseAmount--;
		}
		if (this.spawnFunctions.raccoon)
		{
			GameStats.RaccoonKilled.Invoke();
			this.scene.maxRaccoonAmount--;
		}
		if (this.spawnFunctions.deer)
		{
			GameStats.DeerKilled.Invoke();
			this.scene.maxDeerAmount--;
		}
		if (this.Fire)
		{
			if (this.Fire.activeSelf)
			{
				this.ai.goBurntRagdoll();
			}
			this.Fire.SetActive(false);
		}
		base.CancelInvoke("HitPoison");
		base.CancelInvoke("disablePoison");
		base.CancelInvoke("HitFire");
		if (!this.spawnFunctions.deer && !this.Fire)
		{
			this.PlayEvent(this.DieEvent);
		}
		this.ai.goRagdoll();
		this.resetHealth();
	}

	private void resetHealth()
	{
		this.Health = this.startHealth;
	}

	private void resetDeathBlock()
	{
	}

	private void DieTrap()
	{
		this.Hit(100);
		base.Invoke("resetHealth", 1f);
	}

	private void scaredTimeout()
	{
		this.pmBase.FsmVariables.GetFsmBool("scaredBool").Value = false;
	}

	private void setTrapped(GameObject go)
	{
		this.pmBase.SendEvent("toTrapped");
		Vector3 position = go.transform.position;
		position.y = base.transform.position.y;
		base.transform.position = position;
		if (this.spawnFunctions.lizard)
		{
			this.ai.animatorTr.rotation = go.transform.rotation;
		}
	}

	public void releaseFromTrap()
	{
		Debug.Log("releaseFromTrap1");
		this.pmBase.SendEvent("toTrapReset");
		Debug.Log("releaseFromTrap2");
	}

	public void Explosion()
	{
		this.ai.goRagdoll();
	}

	private void UpdateOnFireEvent()
	{
		if (this.onFireEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.onFireEventInstance.set3DAttributes(base.transform.to3DAttributes()));
		}
	}

	private void StartOnFireEvent()
	{
		if (this.onFireEventInstance == null && this.onFireEvent != null && this.onFireEvent.Length > 0)
		{
			this.onFireEventInstance = FMOD_StudioSystem.instance.GetEvent(this.onFireEvent);
			if (this.onFireEventInstance != null)
			{
				UnityUtil.ERRCHECK(this.onFireEventInstance.start());
			}
		}
	}

	private void StopOnFireEvent()
	{
		if (this.onFireEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.onFireEventInstance.stop(STOP_MODE.ALLOWFADEOUT));
			UnityUtil.ERRCHECK(this.onFireEventInstance.release());
			this.onFireEventInstance = null;
		}
	}
}
