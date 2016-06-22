using Bolt;
using FMOD.Studio;
using HutongGames.PlayMaker;
using PathologicalGames;
using System;
using TheForest.Utils;
using UnityEngine;

public class EnemyHealth : EntityBehaviour
{
	private PlayMakerFSM pmBase;

	private PlayMakerFSM pmSearch;

	private PlayMakerFSM pmSleep;

	public GameObject clubPickup;

	public GameObject EatenTorso;

	public GameObject TorsoBlood;

	public Material eatenBody;

	private FsmBool fsmLookingForTarget;

	public Transform bloodAngleTr;

	public Renderer MySkin;

	private bool deadBlock;

	private bool explodeBlock;

	public GameObject ClubTrigger;

	public GameObject hips;

	public GameObject trapParent;

	private mutantScriptSetup setup;

	private mutantTypeSetup typeSetup;

	private mutantAI ai;

	private clsragdollify ragDollSetup;

	private mutantFamilyFunctions familyFunctions;

	private mutantTargetSwitching targetSwitcher;

	public bool simplifyHitsForMp;

	public int Health;

	public int maxHealth;

	public int recoverValue;

	private float maxHealthFloat;

	public GameObject[] Fire;

	public GameObject RagDollExploded;

	private int RandomSplurt;

	private float tired;

	private int hitDir;

	private Vector3 trapLookatDir;

	public GameObject trapGo;

	public bool hitBlock;

	private int damageMult;

	private bool doStealthKill;

	public bool onFire;

	private bool hitEventBlock;

	public bool alreadyBurnt;

	private bool doused;

	private int douseMult = 1;

	public int lastHitDir;

	private int deathTag = Animator.StringToHash("death");

	private GameObject AiControl;

	private Animator animator;

	private float smoothTired;

	public GameObject MyBody;

	public GameObject BloodSplat;

	public Transform BloodPos;

	public Material defaultMat;

	public Material Burnt;

	public Material Blood1;

	public Material Blood2;

	public Material Blood3;

	public Material Blood4;

	[Header("FMOD Events (played for creepy mutants)")]
	public string OnFireEvent;

	public string HurtEvent;

	public string DieEvent;

	public string DieByFireEvent;

	private mutantPropManager MP;

	private EventInstance onFireEventInstance;

	private ParameterInstance onFireHealthParameter;

	private MaterialPropertyBlock bloodPropertyBlock;

	public static BoltEntity CurrentAttacker;

	private float HealthPercentage
	{
		get
		{
			return (float)this.Health / this.maxHealthFloat * 100f;
		}
	}

	private void Awake()
	{
		this.ragDollSetup = base.GetComponent<clsragdollify>();
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.typeSetup = base.transform.parent.GetComponent<mutantTypeSetup>();
		this.ai = base.GetComponent<mutantAI>();
		this.familyFunctions = base.transform.parent.GetComponent<mutantFamilyFunctions>();
		this.targetSwitcher = base.GetComponentInChildren<mutantTargetSwitching>();
		if (!this.ai.creepy)
		{
			this.animator = base.GetComponent<Animator>();
		}
		else
		{
			this.animator = base.GetComponentInChildren<Animator>();
		}
		this.MP = base.gameObject.GetComponent<mutantPropManager>();
		this.bloodPropertyBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		this.deadBlock = false;
		this.damageMult = 1;
		this.douseMult = 1;
		this.maxHealth = this.Health;
		this.maxHealthFloat = (float)this.Health;
		base.InvokeRepeating("rechargeHealth", 0f, 1.5f);
		if (this.setup.pmVision)
		{
			this.fsmLookingForTarget = this.setup.pmVision.FsmVariables.GetFsmBool("lookingForTarget");
			this.resetSkinDamage();
		}
		if (this.ai.pale && !this.ai.maleSkinny)
		{
			this.recoverValue = 27;
		}
		else
		{
			this.recoverValue = 17;
		}
	}

	private void OnEnable()
	{
		this.deadBlock = false;
		this.alreadyBurnt = false;
		this.onFire = false;
		this.doStealthKill = false;
		this.douseMult = 1;
		if (this.animator)
		{
			this.animator.SetBoolReflected("burning", false);
		}
		if (this.setup && this.setup.pmCombat)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("onFireBool").Value = false;
		}
		this.resetSkinDamage();
	}

	private void resetSkinDamage()
	{
		if (!this.MySkin)
		{
			return;
		}
		this.MySkin.GetPropertyBlock(this.bloodPropertyBlock);
		this.bloodPropertyBlock.SetFloat("_Damage1", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage2", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage3", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage4", 0f);
		this.MySkin.SetPropertyBlock(this.bloodPropertyBlock);
		this.animator.SetFloatReflected("skinDamage1", 0f);
		this.animator.SetFloatReflected("skinDamage2", 0f);
		this.animator.SetFloatReflected("skinDamage3", 0f);
		this.animator.SetFloatReflected("skinDamage4", 0f);
	}

	private void OnDisable()
	{
		this.doused = false;
		this.douseMult = 1;
		if (this.defaultMat)
		{
			this.MySkin.sharedMaterial = this.defaultMat;
		}
		this.StopOnFireEvent();
		this.hitEventBlock = false;
		base.CancelInvoke("disableBurn");
		base.CancelInvoke("HitFire");
		base.CancelInvoke("HitPoison");
		GameObject[] fire = this.Fire;
		for (int i = 0; i < fire.Length; i++)
		{
			GameObject gameObject = fire[i];
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
		if (this.setup && this.setup.pmCombat)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("onFireBool").Value = false;
		}
	}

	private void Update()
	{
		if (this.Health > this.recoverValue)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathRecoverBool").Value = true;
		}
		else
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathRecoverBool").Value = false;
		}
		if (this.ai.male || this.ai.female)
		{
			this.animator.SetIntegerReflected("ClientHealth", this.Health);
		}
		if (!this.animator.enabled)
		{
			return;
		}
		this.tired = (float)this.Health / this.maxHealthFloat;
		this.tired = 1f - this.tired;
		this.smoothTired = Mathf.Lerp(this.smoothTired, this.tired, Time.deltaTime);
		if (!this.ai.creepy && !this.ai.creepy_male && !this.ai.creepy_fat && !this.ai.creepy_baby)
		{
			if (!this.ai.female && !this.ai.pale && !this.ai.maleSkinny && !this.ai.femaleSkinny)
			{
				float value = this.smoothTired * -1f;
				this.animator.SetFloatReflected("mutantType", value);
			}
			else if (this.ai.female && !this.ai.femaleSkinny)
			{
				float value2 = this.smoothTired + 1f;
				this.animator.SetFloatReflected("mutantType", value2);
			}
		}
		this.UpdateOnFireEvent();
	}

	private void UpdateOnFireEvent()
	{
		if (this.onFireEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.onFireEventInstance.set3DAttributes(base.transform.to3DAttributes()));
			if (this.onFireHealthParameter != null)
			{
				UnityUtil.ERRCHECK(this.onFireHealthParameter.setValue(this.HealthPercentage));
			}
		}
	}

	public void addHealth()
	{
		this.Health += 20;
	}

	private void rechargeHealth()
	{
		if (this.animator.GetCurrentAnimatorStateInfo(0).tagHash == this.deathTag && this.Health < this.maxHealth)
		{
			this.Health++;
		}
	}

	public void Explosion(float explodeDist)
	{
		if (!this.explodeBlock)
		{
			if (this.ai.creepy_male || this.ai.creepy || this.ai.creepy_fat || this.ai.creepy_baby)
			{
				this.Health -= 65;
				if (this.Burnt && this.MySkin)
				{
					if (this.setup.propManager && this.setup.propManager.lowSkinnyBody)
					{
						this.setup.propManager.lowSkinnyBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial = this.Burnt;
					}
					if (this.setup.propManager && this.setup.propManager.lowBody)
					{
						this.setup.propManager.lowBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial = this.Burnt;
					}
					this.MySkin.sharedMaterial = this.Burnt;
					this.alreadyBurnt = true;
				}
				this.setSkinDamage(UnityEngine.Random.Range(0, 3));
				if (this.Health <= 0)
				{
					this.dieExplode();
				}
				else
				{
					this.setup.pmCombat.SendEvent("toHitExplode");
				}
			}
			else if (this.ai.male || this.ai.female)
			{
				if (explodeDist < 10.5f)
				{
					UnityEngine.Object.Instantiate(this.RagDollExploded, base.transform.position, base.transform.rotation);
					this.typeSetup.removeFromSpawnAndExplode();
				}
				else if (explodeDist > 10.5f && explodeDist < 18f)
				{
					this.getAttackDirection(5);
					this.targetSwitcher.attackerType = 4;
					this.animator.SetIntegerReflected("hurtLevelInt", 4);
					this.animator.SetTriggerReflected("damageTrigger");
					if (this.Burnt && this.MySkin)
					{
						if (this.setup.propManager && this.setup.propManager.lowSkinnyBody)
						{
							this.setup.propManager.lowSkinnyBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial = this.Burnt;
						}
						if (this.setup.propManager && this.setup.propManager.lowBody)
						{
							this.setup.propManager.lowBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial = this.Burnt;
						}
						this.MySkin.sharedMaterial = this.Burnt;
						this.alreadyBurnt = true;
					}
					this.setSkinDamage(UnityEngine.Random.Range(0, 3));
					this.Health -= 50;
					if (this.Health < 1)
					{
						this.HitReal(1);
						return;
					}
					if (this.setup.pmCombat)
					{
						this.setup.pmCombat.enabled = true;
						this.setup.pmCombat.SendEvent("gotHit");
					}
					if (this.setup.pmSearch && this.setup.pmSearch.enabled)
					{
						this.setup.pmSearch.SendEvent("gotHit");
					}
					if (this.setup.pmStalk && this.setup.pmStalk.enabled)
					{
						this.setup.pmStalk.SendEvent("gotHit");
					}
					if (this.setup.pmEncounter && this.setup.pmEncounter.enabled)
					{
						this.setup.pmEncounter.SendEvent("gotHit");
					}
				}
				else if (explodeDist > 18f && explodeDist < 25f)
				{
					Debug.Log("doing stagger explosion");
					this.getAttackDirection(3);
					this.targetSwitcher.attackerType = 4;
					this.animator.SetIntegerReflected("hurtLevelInt", 0);
					this.Hit(15);
				}
			}
			else
			{
				this.setup.pmCombat.enabled = true;
				this.setup.pmCombat.FsmVariables.GetFsmBool("deathFinal").Value = true;
				this.setup.pmCombat.SendEvent("toDeath");
				if (this.familyFunctions)
				{
					this.familyFunctions.cancelEatMeEvent();
					this.familyFunctions.cancelRescueEvent();
				}
				UnityEngine.Object.Instantiate(this.RagDollExploded, base.transform.position, base.transform.rotation);
				this.typeSetup.removeFromSpawnAndExplode();
			}
			this.explodeBlock = true;
		}
		base.Invoke("resetExplodeBlock", 0.1f);
	}

	private void resetExplodeBlock()
	{
		this.explodeBlock = false;
	}

	public void douseEnemy()
	{
		this.doused = true;
		this.douseMult++;
		base.Invoke("resetDouse", 30f);
	}

	public void setFireDouse()
	{
		this.doused = true;
		this.douseMult = 2;
		base.Invoke("resetDouse", 30f);
	}

	private void resetDouse()
	{
		this.douseMult--;
		if (this.douseMult < 1)
		{
			this.douseMult = 1;
			this.doused = false;
		}
	}

	public void Poison()
	{
		base.CancelInvoke("HitPoison");
		base.CancelInvoke("disablePoison");
		base.InvokeRepeating("HitPoison", 0.5f, UnityEngine.Random.Range(5f, 6f));
		if (this.setup.ai.creepy_fat)
		{
			base.Invoke("disablePoison", 30f);
		}
		else
		{
			base.Invoke("disablePoison", UnityEngine.Random.Range(40f, 60f));
		}
	}

	private void disablePoison()
	{
		base.CancelInvoke("HitPoison");
	}

	public void Burn()
	{
		if (this.ai.fireman_dynamite)
		{
			base.Invoke("doBeltExplosion", 4f);
		}
		if (this.douseMult > 1)
		{
			GameStats.BurntEnemy.Invoke();
			if (this.Burnt && this.MySkin)
			{
				if (this.setup.propManager && this.setup.propManager.lowSkinnyBody)
				{
					this.setup.propManager.lowSkinnyBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial = this.Burnt;
				}
				if (this.setup.propManager && this.setup.propManager.lowBody)
				{
					this.setup.propManager.lowBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial = this.Burnt;
				}
				this.MySkin.sharedMaterial = this.Burnt;
				this.alreadyBurnt = true;
			}
			if (this.Fire[0])
			{
				GameObject[] fire = this.Fire;
				for (int i = 0; i < fire.Length; i++)
				{
					GameObject gameObject = fire[i];
					gameObject.SetActive(true);
				}
			}
			this.onFire = true;
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.FsmVariables.GetFsmBool("onFireBool").Value = true;
			}
			this.targetSwitcher.attackerType = 4;
			int num = this.douseMult - 1;
			if (num < 1)
			{
				num = 1;
			}
			this.Hit(5 * num);
			if (this.ai.creepy_male || this.ai.creepy || this.ai.creepy_fat || this.ai.creepy_baby)
			{
				if (this.animator.enabled)
				{
					this.animator.SetIntegerReflected("randInt1", UnityEngine.Random.Range(0, 3));
				}
				this.animator.SetBoolReflected("onFireBool", true);
				this.setup.pmCombat.SendEvent("goToOnFire");
				this.StartOnFireEvent();
			}
			base.InvokeRepeating("HitFire", UnityEngine.Random.Range(1f, 2f), 1f);
			if (this.setup.ai.creepy_fat)
			{
				base.Invoke("disableBurn", 10f);
			}
			else if (this.setup.ai.fireman)
			{
				base.Invoke("disableBurn", 5f);
			}
			else
			{
				base.Invoke("disableBurn", UnityEngine.Random.Range(7f, 14f));
			}
		}
		else
		{
			this.singeBurn();
		}
	}

	private void disableBurn()
	{
		base.CancelInvoke("HitFire");
		if (this.Fire[0])
		{
			GameObject[] fire = this.Fire;
			for (int i = 0; i < fire.Length; i++)
			{
				GameObject gameObject = fire[i];
				gameObject.SetActive(false);
			}
		}
		this.onFire = false;
		this.StopOnFireEvent();
		this.hitEventBlock = false;
		if (this.ai.creepy || this.ai.creepy_male || this.ai.creepy_fat)
		{
			if (this.animator.enabled)
			{
				this.animator.SetBoolReflected("onFireBool", false);
			}
		}
		else
		{
			this.setup.pmCombat.SendEvent("toBurnRecover");
			if (this.animator.enabled)
			{
				this.animator.SetBoolReflected("burning", false);
			}
		}
		this.douseMult = 1;
	}

	private void doBeltExplosion()
	{
	}

	private void StartOnFireEvent()
	{
		if (this.onFireEventInstance == null && this.OnFireEvent != null && this.OnFireEvent.Length > 0)
		{
			this.onFireEventInstance = FMOD_StudioSystem.instance.GetEvent(this.OnFireEvent);
			if (this.onFireEventInstance != null)
			{
				UnityUtil.ERRCHECK(this.onFireEventInstance.getParameter("health", out this.onFireHealthParameter));
				this.UpdateOnFireEvent();
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
			this.onFireHealthParameter = null;
		}
	}

	private void TrapDamage()
	{
	}

	private void singeBurn()
	{
		if (this.Fire[0])
		{
			GameObject[] fire = this.Fire;
			for (int i = 0; i < fire.Length; i++)
			{
				GameObject gameObject = fire[i];
				gameObject.SetActive(true);
			}
		}
		this.onFire = true;
		if (this.Burnt && this.MySkin)
		{
			if (this.setup.propManager && this.setup.propManager.lowSkinnyBody)
			{
				this.setup.propManager.lowSkinnyBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial = this.Burnt;
			}
			if (this.setup.propManager && this.setup.propManager.lowBody)
			{
				this.setup.propManager.lowBody.GetComponent<SkinnedMeshRenderer>().sharedMaterial = this.Burnt;
			}
			this.MySkin.sharedMaterial = this.Burnt;
			this.alreadyBurnt = true;
		}
		if (this.ai.creepy_male || this.ai.creepy || this.ai.creepy_fat || this.ai.creepy_baby)
		{
			if (this.animator.enabled)
			{
				this.animator.SetBoolReflected("onFireBool", true);
			}
			this.setup.pmCombat.SendEvent("toHitExplode");
		}
		else
		{
			this.getAttackDirection(3);
			this.animator.SetIntegerReflected("randInt1", 0);
			this.targetSwitcher.attackerType = 4;
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.FsmVariables.GetFsmBool("onFireBool").Value = true;
			}
		}
		int num = this.douseMult - 1;
		if (num < 1)
		{
			num = 1;
		}
		this.Hit(5 * num);
		base.InvokeRepeating("HitFire", UnityEngine.Random.Range(1f, 1.5f), 1f);
		base.Invoke("disableBurn", 4f);
	}

	private void HitFire()
	{
		if (!this.deadBlock)
		{
			this.setSkinDamage(UnityEngine.Random.Range(0, 3));
			this.targetSwitcher.attackerType = 4;
			int num = this.douseMult - 1;
			if (num < 1)
			{
				num = 1;
			}
			if (this.ai.creepy || this.ai.creepy_male || this.ai.creepy_fat || this.ai.creepy_baby)
			{
				this.Hit(UnityEngine.Random.Range(3, 6) * num);
			}
			else
			{
				this.Hit(UnityEngine.Random.Range(4, 6) * num);
			}
		}
	}

	private void HitPoison()
	{
		if (!this.deadBlock)
		{
			this.targetSwitcher.attackerType = 4;
			int max = 4;
			int min;
			if (this.ai.creepy || this.ai.creepy_male || this.ai.creepy_fat || this.ai.creepy_baby)
			{
				min = 1;
			}
			else
			{
				min = 2;
			}
			int damage = UnityEngine.Random.Range(min, max);
			this.Hit(damage);
		}
	}

	private void hitBlockReset()
	{
		this.hitBlock = false;
	}

	private void HitHead()
	{
	}

	public void setSkinDamage(int d)
	{
		float num = 0f;
		this.MySkin.GetPropertyBlock(this.bloodPropertyBlock);
		if (this.ai.creepy || this.ai.creepy_fat || this.ai.creepy_male || this.ai.creepy_baby)
		{
			num = 1f - (float)this.Health / (float)this.maxHealth;
			this.bloodPropertyBlock.SetFloat("_Damage1", num);
			this.bloodPropertyBlock.SetFloat("_Damage2", num);
			this.bloodPropertyBlock.SetFloat("_Damage3", num);
			this.bloodPropertyBlock.SetFloat("_Damage4", num);
			this.animator.SetFloatReflected("skinDamage1", num);
			this.animator.SetFloatReflected("skinDamage2", num);
			this.animator.SetFloatReflected("skinDamage3", num);
			this.animator.SetFloatReflected("skinDamage4", num);
		}
		else
		{
			if (d == 0)
			{
				num = this.bloodPropertyBlock.GetFloat("_Damage1");
				if (num < 1f)
				{
					num += 0.5f;
					this.bloodPropertyBlock.SetFloat("_Damage1", num);
					this.animator.SetFloatReflected("skinDamage1", num);
				}
			}
			else if (d == 1)
			{
				num = this.bloodPropertyBlock.GetFloat("_Damage3");
				if (num < 1f)
				{
					num += 0.5f;
					this.bloodPropertyBlock.SetFloat("_Damage3", num);
					this.animator.SetFloatReflected("skinDamage3", num);
				}
			}
			else
			{
				this.bloodPropertyBlock.GetFloat("_Damage2");
				if (num < 1f)
				{
					num += 0.5f;
					this.bloodPropertyBlock.SetFloat("_Damage2", num);
					this.animator.SetFloatReflected("skinDamage2", num);
				}
			}
			num = this.bloodPropertyBlock.GetFloat("_Damage4");
			if (num < 1f)
			{
				num += 0.5f;
				this.bloodPropertyBlock.SetFloat("_Damage4", num);
				this.animator.SetFloatReflected("skinDamage4", num);
			}
		}
		this.MySkin.SetPropertyBlock(this.bloodPropertyBlock);
	}

	private void disableHeadDamage()
	{
		if (this.animator.enabled)
		{
			this.animator.SetBoolReflected("headDamageBool", false);
		}
		this.damageMult = 1;
	}

	public void getStealthAttack()
	{
		if (!this.setup.ai.creepy && !this.setup.ai.creepy_baby && !this.setup.ai.creepy_male && !this.setup.ai.creepy_fat)
		{
			if (this.hitDir == 1 && this.setup.search.lookingForTarget)
			{
				this.doStealthKill = true;
			}
			else
			{
				this.doStealthKill = false;
			}
		}
	}

	public void Hit(int damage)
	{
		if ((this.setup.ai.male || this.setup.ai.female) && !this.setup.ai.maleSkinny && !this.setup.ai.femaleSkinny && this.targetSwitcher.attackerType == 0)
		{
			return;
		}
		this.HitReal(damage);
	}

	public void HitReal(int damage)
	{
		if (this.hitBlock || this.deadBlock)
		{
			return;
		}
		if (!base.enabled)
		{
			return;
		}
		if (this.ai.creepy || this.ai.creepy_male || this.ai.creepy_fat || this.ai.creepy_baby)
		{
			this.Health -= damage;
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.FsmVariables.GetFsmBool("gettingHit").Value = true;
				this.setup.pmCombat.SendEvent("gotHit");
				base.Invoke("resetGettingHit", 1.3f);
			}
			if (this.Health < 1)
			{
				this.Die();
			}
			else if (this.onFireEventInstance == null)
			{
				FMODCommon.PlayOneshot(this.HurtEvent, base.transform.position, new object[]
				{
					"mutant_health",
					this.HealthPercentage
				});
			}
			return;
		}
		if (this.targetSwitcher)
		{
			this.targetSwitcher.attackerType = 4;
		}
		this.hitBlock = true;
		base.Invoke("hitBlockReset", 0.25f);
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
			this.Health -= 100;
		}
		else
		{
			this.Health -= damage * this.damageMult;
		}
		if (this.Health <= 80 && this.Health >= 45)
		{
			this.animator.SetIntegerReflected("hurtLevelInt", 1);
		}
		if (this.Health < 45 && this.Health >= 30)
		{
			this.animator.SetIntegerReflected("hurtLevelInt", 2);
		}
		if (this.Health < 30 && this.Health >= 1 && this.animator)
		{
			this.animator.SetIntegerReflected("hurtLevelInt", 3);
		}
		base.Invoke("setMpRandInt", 1f);
		if (this.Health < 1)
		{
			if (this.animator)
			{
				this.animator.SetIntegerReflected("hurtLevelInt", 4);
			}
			this.setup.pmCombat.enabled = true;
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathFinal").Value = true;
			if (this.onFire)
			{
				this.animator.SetBoolReflected("burning", true);
				this.animator.SetBoolReflected("deathBOOL", true);
			}
			if (this.animator)
			{
				this.animator.SetBoolReflected("deathfinalBOOL", true);
			}
			if (!this.doStealthKill && !this.onFire && this.animator)
			{
				this.animator.SetIntegerReflected("randInt1", UnityEngine.Random.Range(0, 8));
				this.animator.SetTriggerReflected("deathTrigger");
			}
			this.Die();
		}
		else if (this.hitDir == 1)
		{
			if (!this.setup.ai.creepy && !this.setup.ai.creepy_male && !this.setup.ai.creepy_fat && !this.setup.ai.creepy_baby && this.setup.search.lookingForTarget && this.doStealthKill)
			{
				this.Die();
			}
			else
			{
				if (this.animator)
				{
					if (this.onFire && !this.hitEventBlock)
					{
						this.animator.SetBoolReflected("burning", true);
						if (!this.animator.GetBool("trapBool"))
						{
							this.animator.SetTriggerReflected("damageTrigger");
						}
					}
					else if (!this.hitEventBlock)
					{
						this.animator.SetTriggerReflected("damageBehindTrigger");
						this.animator.SetBoolReflected("burning", false);
					}
				}
				if (this.setup.pmCombat)
				{
					if (this.onFire && !this.hitEventBlock)
					{
						this.setup.pmCombat.enabled = true;
						this.setup.pmCombat.FsmVariables.GetFsmBool("onFireBool").Value = true;
						this.setup.pmCombat.SendEvent("gotHit");
						this.hitEventBlock = true;
					}
					else if (!this.onFire)
					{
						this.setup.pmCombat.SendEvent("gotHit");
						this.setup.pmCombat.FsmVariables.GetFsmBool("onFireBool").Value = false;
					}
				}
				if (this.setup.pmSearch && this.setup.pmSearch.enabled)
				{
					this.setup.pmSearch.SendEvent("gotHit");
				}
				if (this.setup.pmEncounter && this.setup.pmEncounter.enabled)
				{
					this.setup.pmEncounter.SendEvent("gotHit");
				}
			}
		}
		else if (this.hitDir == 0)
		{
			this.doStealthKill = false;
			if (this.animator)
			{
				if (this.simplifyHitsForMp || BoltNetwork.isRunning)
				{
					this.animator.SetTriggerReflected("simpleHitTrigger");
				}
				else if (this.onFire && !this.hitEventBlock)
				{
					this.animator.SetBoolReflected("burning", true);
					if (!this.animator.GetBool("trapBool"))
					{
						this.animator.SetTriggerReflected("damageTrigger");
					}
				}
				else if (!this.hitEventBlock)
				{
					this.animator.SetBoolReflected("burning", false);
					this.animator.SetTriggerReflected("damageTrigger");
				}
			}
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.enabled = true;
				if (this.onFire && !this.hitEventBlock)
				{
					this.setup.pmCombat.enabled = true;
					this.setup.pmCombat.FsmVariables.GetFsmBool("onFireBool").Value = true;
					this.setup.pmCombat.SendEvent("gotHit");
					this.hitEventBlock = true;
				}
				else if (!this.onFire)
				{
					this.setup.pmCombat.SendEvent("gotHit");
					this.setup.pmCombat.FsmVariables.GetFsmBool("onFireBool").Value = false;
				}
			}
			if (this.setup.pmSearch && this.setup.pmSearch.enabled)
			{
				this.setup.pmSearch.SendEvent("gotHit");
			}
			if (this.setup.pmEncounter && this.setup.pmEncounter.enabled)
			{
				this.setup.pmEncounter.SendEvent("gotHit");
			}
		}
		this.Blood();
		this.RandomSplurt = UnityEngine.Random.Range(0, 10);
		if (!this.ai.creepy && !this.ai.creepy_male && !this.ai.maleSkinny && !this.ai.femaleSkinny && !this.ai.pale && !this.ai.creepy_fat && !this.ai.creepy_baby && !this.alreadyBurnt && this.RandomSplurt == 2)
		{
			if (this.MP.MyRandom != 0 || this.MySkin)
			{
			}
			if (this.MP.MyRandom != 1 || this.MySkin)
			{
			}
			if (this.MP.MyRandom != 2 || this.MySkin)
			{
			}
			if (this.MP.MyRandom != 3 || this.MySkin)
			{
			}
			if (EnemyHealth.CurrentAttacker)
			{
				SendMessageEvent sendMessageEvent = SendMessageEvent.Raise(EnemyHealth.CurrentAttacker, EntityTargets.OnlyOwner);
				sendMessageEvent.Message = "GotBloody";
				sendMessageEvent.Send();
			}
			else
			{
				LocalPlayer.GameObject.SendMessage("GotBloody");
			}
		}
	}

	public void resetPredictionBool()
	{
		this.animator.SetBoolReflected("ClientPredictionBool", false);
	}

	public void getAttackDirection(int hitDir)
	{
		if (!this.ai.creepy && !this.ai.creepy_male && !this.setup.ai.creepy_fat && !this.setup.ai.creepy_baby)
		{
			this.animator.SetIntegerReflected("hitDirection", hitDir);
		}
	}

	public void getCombo(int combo)
	{
		if (!this.ai.creepy && !this.ai.creepy_male && !this.ai.creepy_baby && !this.ai.creepy_fat)
		{
			this.animator.SetIntegerReflected("hitCombo", combo);
		}
	}

	public void takeDamage(int direction)
	{
		this.hitDir = direction;
	}

	private void setMpRandInt()
	{
	}

	private void Die()
	{
		GameStats.EnemyKilled.Invoke();
		if (this.setup.ai.creepy || this.setup.ai.creepy_male || this.setup.ai.creepy_baby || this.setup.ai.creepy_fat)
		{
			base.CancelInvoke("HitFire");
			base.CancelInvoke("HitPoison");
			base.CancelInvoke("disablePoison");
			this.deadBlock = true;
			this.douseMult = 1;
			if (this.onFire)
			{
				this.ragDollSetup.burning = true;
			}
			if (this.alreadyBurnt)
			{
				this.ragDollSetup.alreadyBurnt = true;
			}
			if (this.onFire && !string.IsNullOrEmpty(this.DieByFireEvent))
			{
				FMODCommon.PlayOneshot(this.DieByFireEvent, base.transform);
			}
			else
			{
				FMODCommon.PlayOneshot(this.DieEvent, base.transform);
			}
			this.typeSetup.removeFromSpawn();
			if (this.ai.creepy_fat || this.ai.creepy_male)
			{
				this.animator.SetBoolReflected("deathBool", true);
				return;
			}
			if (this.ai.creepy_baby)
			{
				if (this.onFire)
				{
					return;
				}
				this.ragDollSetup.spinRagdoll = true;
				if (this.targetSwitcher.currentAttackerGo)
				{
					this.ragDollSetup.hitTr = this.targetSwitcher.currentAttackerGo.transform;
					this.ragDollSetup.metgoragdoll(this.targetSwitcher.currentAttackerGo.transform.forward * 10f);
				}
				else
				{
					this.ragDollSetup.metgoragdoll(default(Vector3));
				}
			}
			else
			{
				this.ragDollSetup.metgoragdoll(default(Vector3));
			}
			if (PoolManager.Pools["enemies"].IsSpawned(base.transform.parent))
			{
				PoolManager.Pools["enemies"].Despawn(base.transform.parent);
			}
			else
			{
				UnityEngine.Object.Destroy(base.transform.root.gameObject);
			}
		}
		else
		{
			base.CancelInvoke("HitFire");
			base.CancelInvoke("HitPoison");
			base.CancelInvoke("disablePoison");
			this.animator.SetBoolReflected("attackBOOL", false);
			this.deadBlock = true;
			this.douseMult = 1;
			if (this.doStealthKill && !this.animator.GetBool("trapBool"))
			{
				this.setup.pmCombat.FsmVariables.GetFsmBool("stealthKillBool").Value = true;
				this.animator.SetBoolReflected("stealthDeathBool", true);
			}
			this.setup.pmCombat.enabled = true;
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathFinal").Value = true;
			this.setup.pmCombat.SendEvent("toDeath");
			this.setup.pmBrain.SendEvent("toDeath");
			if (this.setup.pmEncounter)
			{
				this.setup.pmEncounter.SendEvent("toDeath");
			}
			this.doStealthKill = false;
		}
	}

	private void dieExplode()
	{
		GameStats.ExplodedEnemy.Invoke();
		GameStats.EnemyKilled.Invoke();
		GameObject gameObject = UnityEngine.Object.Instantiate(this.RagDollExploded, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.SendMessage("setSkin", this.MySkin.sharedMaterial, SendMessageOptions.DontRequireReceiver);
		this.typeSetup.removeFromSpawnAndExplode();
	}

	private void HitAxe()
	{
		this.Blood();
		if (this.Health >= -20 || this.animator.GetBool("deathfinalBOOL"))
		{
		}
		if (EnemyHealth.CurrentAttacker)
		{
			SendMessageEvent sendMessageEvent = SendMessageEvent.Raise(EnemyHealth.CurrentAttacker, EntityTargets.OnlyOwner);
			sendMessageEvent.Message = "GotBloody";
			sendMessageEvent.Send();
		}
		else
		{
			LocalPlayer.GameObject.SendMessage("GotBloody");
		}
	}

	private void BitShark()
	{
		this.Health--;
		this.Blood();
	}

	private void Blood()
	{
		if (this.BloodSplat)
		{
			base.Invoke("BloodActual", 0.1f);
		}
		if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
		{
			FxEnemeyBlood.Raise(this.entity).Send();
		}
	}

	public void BloodActual()
	{
		int index = UnityEngine.Random.Range(0, Prefabs.Instance.BloodHitPSPrefabs.Length);
		Prefabs.Instance.SpawnBloodHitPS(index, this.BloodPos.position, base.transform.rotation);
		GameObject gameObject = UnityEngine.Object.Instantiate(this.BloodSplat, this.BloodPos.position, base.transform.rotation) as GameObject;
	}

	private void removeBlood()
	{
	}

	public void getCurrentHealth()
	{
		this.setup.pmCombat.FsmVariables.GetFsmInt("statHealth").Value = this.Health;
	}

	private void checkDeathState()
	{
		if (this.animator.GetInteger("hitCombo") == 3 && this.animator.GetInteger("hurtLevelInt") > 2)
		{
			this.setup.pmCombat.SendEvent("toDeath");
		}
	}

	private void setTrapped(GameObject go)
	{
		if (this.setup.pmCombat)
		{
			this.setup.pmCombat.SendEvent("toTrapped");
		}
		Vector3 position = go.transform.position;
		position.y = base.transform.position.y;
		base.transform.parent.transform.position = position;
		base.transform.rotation = go.transform.rotation;
	}

	public void releaseFromTrap()
	{
		if (this.setup.pmCombat)
		{
			this.setup.pmCombat.SendEvent("toTrapReset");
		}
	}

	private void DieTrap(int type)
	{
		if (!this.ai.creepy && !this.ai.creepy_male && !this.ai.creepy_baby && !this.ai.creepy_fat)
		{
			if (type == 0)
			{
				this.animator.SetIntegerReflected("trapTypeInt1", 0);
			}
			if (type == 1)
			{
				this.animator.SetIntegerReflected("trapTypeInt1", 1);
			}
			if (type == 2)
			{
				this.animator.SetIntegerReflected("trapTypeInt1", 2);
			}
			if (type == 3)
			{
				FMODCommon.PlayOneshotNetworked("event:/combat/damage/body_impact", base.transform, FMODCommon.NetworkRole.Server);
				this.Hit(100);
			}
			else
			{
				this.animator.SetBoolReflected("trapBool", true);
				if (type == 2)
				{
					this.animator.SetBoolReflected("enterTrapBool", true);
					this.animator.SetBoolReflected("deathBOOL", true);
				}
				else
				{
					this.animator.SetTriggerReflected("deathTrigger");
				}
				GameStats.EnemyKilled.Invoke();
				if (type == 2)
				{
					this.setup.pmBrain.SendEvent("toDeathTrapNoose");
				}
				else
				{
					this.setup.pmBrain.SendEvent("toDeathTrap");
				}
			}
		}
		else if (type == 3)
		{
			this.Hit(65);
		}
	}

	private void setCurrentTrap(GameObject getTrap)
	{
		this.trapGo = getTrap;
	}

	private void setInNooseTrap(GameObject parent)
	{
		if (!this.ai.creepy && !this.ai.creepy_male && !this.ai.creepy_fat && !this.ai.creepy_baby)
		{
			base.transform.root.parent = parent.transform;
			this.trapParent = parent;
			this.setup.bodyCollisionCollider.enabled = false;
		}
	}

	private void setFootPivot(GameObject pivot)
	{
		if (!this.ai.creepy && !this.ai.creepy_male && !this.ai.creepy_fat && !this.ai.creepy_baby)
		{
			this.setup.pmEncounter.FsmVariables.GetFsmGameObject("footPivotGo").Value = pivot;
		}
	}

	private void setTrapLookat(GameObject go)
	{
		if (!this.ai.creepy && !this.ai.creepy_male && !this.ai.creepy_fat && !this.ai.creepy_baby)
		{
			this.setup.pmBrain.FsmVariables.GetFsmVector3("trapLookatDir").Value = go.transform.forward;
		}
	}

	private void enableFeedingEffect()
	{
		this.MyBody.GetComponent<Renderer>().material = this.eatenBody;
	}

	private void disableFeedingEffect()
	{
		this.TorsoBlood.SetActive(false);
	}

	public void removeAllFeedingEffect()
	{
		this.EatenTorso.SetActive(false);
		this.TorsoBlood.SetActive(false);
	}

	public void enableBodyCollider()
	{
		this.setup.bodyCollisionCollider.enabled = true;
	}

	public void enableBodyColliderGo()
	{
		this.setup.bodyCollider.SetActive(true);
		if (this.setup.headColliderGo)
		{
			this.setup.headColliderGo.SetActive(true);
		}
	}

	public void disableBodyColliderGo()
	{
		this.setup.bodyCollider.SetActive(false);
		if (this.setup.headColliderGo)
		{
			this.setup.headColliderGo.SetActive(false);
		}
		base.Invoke("enableBodyColliderGo", 5f);
	}

	private void resetGettingHit()
	{
		this.setup.pmCombat.FsmVariables.GetFsmBool("gettingHit").Value = false;
	}
}
