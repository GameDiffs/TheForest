using Bolt;
using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantTypeSetup : MonoBehaviour
{
	private mutantAI ai;

	private getAnimatorParams getParams;

	private mutantPropManager props;

	private setupBodyVariation propsFemale;

	private EnemyHealth health;

	private mutantFollowerFunctions followSetup;

	private mutantDayCycle dayCycle;

	public mutantScriptSetup setup;

	private mutantController mutantControl;

	private mutantSpawnManager spawnManager;

	private Animator animator;

	public Material creepyPaleMat;

	public Material creepyMat;

	public targetStats stats;

	public bool targetBusy;

	private bool instantSpawn;

	public spawnMutants spawner;

	public GameObject dummyMutant;

	private GameObject controlGo;

	private int waterLayer;

	public bool inWater;

	public bool inCave;

	private bool storeSkinnyBool;

	private bool storePaleMutantBool;

	private bool storeOnCeilingBool;

	private float storeMutantType;

	public int storedRagDollName;

	private bool removeSpawnBlock;

	private void OnSpawned()
	{
		base.StopCoroutine("doSpawnDummy");
		if (this.setup.pmCombat)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("hordeModeActive").Value = Scene.MutantControler.hordeModeActive;
		}
		if (this.setup.pmSearch)
		{
			this.setup.pmSearch.FsmVariables.GetFsmBool("hordeModeActive").Value = Scene.MutantControler.hordeModeActive;
		}
	}

	public void storeDefaultParams()
	{
		this.storeOnCeilingBool = this.animator.GetBool("onCeilingBool");
		this.storeSkinnyBool = this.animator.GetBool("skinnyBool");
		this.storePaleMutantBool = this.animator.GetBool("paleMutantBool");
		this.storeMutantType = this.animator.GetFloat("mutantType");
	}

	public void setDefaultParams()
	{
		this.animator.SetBoolReflected("onCeilingBool", this.storeOnCeilingBool);
		this.animator.SetBoolReflected("skinnyBool", this.storeSkinnyBool);
		this.animator.SetBoolReflected("paleMutantBool", this.storePaleMutantBool);
		this.animator.SetFloatReflected("mutantType", this.storeMutantType);
	}

	private void OnDespawned()
	{
		base.CancelInvoke("removeSpawnCoolDown");
		this.removeSpawnBlock = false;
		base.StopCoroutine("doSpawnDummy");
		if (BoltNetwork.isServer)
		{
			BoltEntity component = base.gameObject.GetComponent<BoltEntity>();
			if (component)
			{
				UnityEngine.Object.Destroy(component);
			}
			CoopMutantSetup component2 = base.gameObject.GetComponent<CoopMutantSetup>();
			if (component2)
			{
				UnityEngine.Object.Destroy(component2);
			}
		}
		this.removeFromLists();
		if (this.setup.ai.creepy || this.setup.ai.creepy_male || this.setup.ai.creepy_baby || this.setup.ai.creepy_fat)
		{
			this.ai.target = this.setup.currentWaypoint.transform;
			this.setup.ai.pale = false;
			this.health.Health = this.health.maxHealth;
			if (this.setup.pmMotor)
			{
				this.setup.pmMotor.SendEvent("toStop");
			}
			this.setup.pmCombat.FsmVariables.GetFsmBool("inCaveBool").Value = false;
			this.animator.SetBool("deathBool", false);
			this.animator.SetBool("onFireBool", false);
			this.animator.SetBool("burning", false);
		}
		else
		{
			if (this.setup.mutantStats)
			{
				this.setup.mutantStats.targetDown = false;
			}
			this.health.Health = this.health.maxHealth;
			this.setup.disableNonActiveFSM("temp");
			if (this.setup.pmSleep)
			{
				this.setup.pmSleep.enabled = true;
				this.setup.pmSleep.FsmVariables.GetFsmBool("getSleepPosBool").Value = true;
				this.setup.pmSleep.FsmVariables.GetFsmBool("inCaveBool").Value = false;
				this.setup.pmSleep.FsmVariables.GetFsmBool("spawnerInCave").Value = false;
			}
			this.ai.target = this.setup.currentWaypoint.transform;
			if (this.setup.pmVision)
			{
				this.setup.pmVision.SendEvent("toDisableFSM");
			}
			if (this.setup.pmMotor)
			{
				this.setup.pmMotor.SendEvent("toStop");
			}
			this.animator.enabled = true;
			this.animator.SetBoolReflected("deathfinalBOOL", false);
			this.animator.SetBoolReflected("deathBOOL", false);
			this.animator.SetBoolReflected("trapBool", false);
			this.animator.SetBoolReflected("dropFromTrap", false);
			this.animator.SetBoolReflected("enterTrapBool", false);
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathFinal").Value = false;
			this.setup.pmBrain.FsmVariables.GetFsmBool("deadBool").Value = false;
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathBool").Value = false;
			this.setup.pmCombat.FsmVariables.GetFsmBool("dynamiteMan").Value = false;
			this.setup.pmBrain.FsmVariables.GetFsmBool("leaderBool").Value = false;
			this.setup.pmBrain.FsmVariables.GetFsmBool("firemanBool").Value = false;
			this.setup.pmBrain.FsmVariables.GetFsmBool("paleBool").Value = false;
			this.setup.pmBrain.FsmVariables.GetFsmBool("eatBodyBool").Value = false;
			this.setup.pmBrain.FsmVariables.GetFsmBool("enableGravityBool").Value = true;
			this.setup.pmCombat.FsmVariables.GetFsmBool("eatBodyBool").Value = false;
			this.setup.pmCombat.FsmVariables.GetFsmBool("getFiremanBool").Value = false;
			this.setup.pmSearch.FsmVariables.GetFsmBool("getFiremanBool").Value = false;
			this.setup.pmSearch.FsmVariables.GetFsmBool("eatBodyBool").Value = false;
			this.setup.pmBrain.FsmVariables.GetFsmBool("femaleSkinnyBool").Value = false;
			this.setup.pmSleep.FsmVariables.GetFsmBool("paleOnCeiling").Value = false;
			if (this.health)
			{
				this.health.removeAllFeedingEffect();
			}
			this.setup.ai.fireman = false;
			this.setup.ai.leader = false;
			this.setup.ai.fireman_dynamite = false;
			this.setup.ai.pale = false;
			this.setup.ai.femaleSkinny = false;
			this.setup.ai.maleSkinny = false;
			this.animator.SetBoolReflected("paleMutantBool", false);
			this.animator.SetBoolReflected("skinnyBool", false);
			this.animator.SetFloatReflected("mutantType", 0f);
			this.props.resetProps();
			if (this.setup.pmBrain)
			{
				this.setup.pmBrain.FsmVariables.GetFsmGameObject("spawnGo").Value = null;
			}
			this.setup.spawnGo = null;
			base.transform.GetChild(0).localScale = new Vector3(1.1f, 1.1f, 1.1f);
			if (this.setup.pmBrain)
			{
				this.setup.pmBrain.FsmVariables.GetFsmGameObject("leaderGo").Value = null;
			}
			if (this.followSetup)
			{
				this.followSetup.followersList.Clear();
			}
		}
	}

	private void Awake()
	{
		this.animator = base.transform.GetComponentInChildren<Animator>();
		this.getParams = base.GetComponent<getAnimatorParams>();
		this.setup = base.transform.GetComponentInChildren<mutantScriptSetup>();
		this.ai = base.transform.GetComponentInChildren<mutantAI>();
		this.props = base.transform.GetComponentInChildren<mutantPropManager>();
		this.propsFemale = base.transform.GetComponentInChildren<setupBodyVariation>();
		this.health = base.transform.GetComponentInChildren<EnemyHealth>();
		this.followSetup = base.transform.GetComponent<mutantFollowerFunctions>();
		this.dayCycle = base.transform.GetComponentInChildren<mutantDayCycle>();
		this.controlGo = Scene.MutantControler.gameObject;
		if (this.controlGo)
		{
			this.mutantControl = Scene.MutantControler;
		}
		this.spawnManager = this.controlGo.GetComponent<mutantSpawnManager>();
		this.stats = base.transform.GetComponent<targetStats>();
		this.waterLayer = LayerMask.NameToLayer("Water");
	}

	public void AttachToNetwork(PrefabId prefabId, UniqueId state)
	{
		if (BoltNetwork.isServer)
		{
			base.gameObject.AddComponent<CoopMutantSetup>();
			BoltEntity boltEntity = base.gameObject.AddComponent<BoltEntity>();
			using (BoltEntitySettingsModifier boltEntitySettingsModifier = boltEntity.ModifySettings())
			{
				boltEntitySettingsModifier.persistThroughSceneLoads = true;
				boltEntitySettingsModifier.allowInstantiateOnClient = false;
				boltEntitySettingsModifier.clientPredicted = false;
				boltEntitySettingsModifier.prefabId = prefabId;
				boltEntitySettingsModifier.updateRate = 1;
				boltEntitySettingsModifier.sceneId = UniqueId.None;
				boltEntitySettingsModifier.serializerId = state;
			}
			BoltNetwork.Attach(boltEntity.gameObject);
		}
	}

	[DebuggerHidden]
	private IEnumerator setLeader()
	{
		mutantTypeSetup.<setLeader>c__IteratorAE <setLeader>c__IteratorAE = new mutantTypeSetup.<setLeader>c__IteratorAE();
		<setLeader>c__IteratorAE.<>f__this = this;
		return <setLeader>c__IteratorAE;
	}

	[DebuggerHidden]
	private IEnumerator setPaleLeader()
	{
		mutantTypeSetup.<setPaleLeader>c__IteratorAF <setPaleLeader>c__IteratorAF = new mutantTypeSetup.<setPaleLeader>c__IteratorAF();
		<setPaleLeader>c__IteratorAF.<>f__this = this;
		return <setPaleLeader>c__IteratorAF;
	}

	[DebuggerHidden]
	private IEnumerator setFireman()
	{
		mutantTypeSetup.<setFireman>c__IteratorB0 <setFireman>c__IteratorB = new mutantTypeSetup.<setFireman>c__IteratorB0();
		<setFireman>c__IteratorB.<>f__this = this;
		return <setFireman>c__IteratorB;
	}

	public void forceDynamiteMan(bool onoff)
	{
		if (onoff)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("dynamiteMan").Value = true;
			this.ai.fireman_dynamite = true;
			this.props.dynamiteMan = true;
		}
		else
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("dynamiteMan").Value = false;
			this.ai.fireman_dynamite = false;
			this.props.dynamiteMan = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator setPale()
	{
		mutantTypeSetup.<setPale>c__IteratorB1 <setPale>c__IteratorB = new mutantTypeSetup.<setPale>c__IteratorB1();
		<setPale>c__IteratorB.<>f__this = this;
		return <setPale>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator setCreepyPale(bool set)
	{
		mutantTypeSetup.<setCreepyPale>c__IteratorB2 <setCreepyPale>c__IteratorB = new mutantTypeSetup.<setCreepyPale>c__IteratorB2();
		<setCreepyPale>c__IteratorB.set = set;
		<setCreepyPale>c__IteratorB.<$>set = set;
		<setCreepyPale>c__IteratorB.<>f__this = this;
		return <setCreepyPale>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator setDefault()
	{
		mutantTypeSetup.<setDefault>c__IteratorB3 <setDefault>c__IteratorB = new mutantTypeSetup.<setDefault>c__IteratorB3();
		<setDefault>c__IteratorB.<>f__this = this;
		return <setDefault>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator setFemale()
	{
		mutantTypeSetup.<setFemale>c__IteratorB4 <setFemale>c__IteratorB = new mutantTypeSetup.<setFemale>c__IteratorB4();
		<setFemale>c__IteratorB.<>f__this = this;
		return <setFemale>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator setArmsy(GameObject go)
	{
		mutantTypeSetup.<setArmsy>c__IteratorB5 <setArmsy>c__IteratorB = new mutantTypeSetup.<setArmsy>c__IteratorB5();
		<setArmsy>c__IteratorB.go = go;
		<setArmsy>c__IteratorB.<$>go = go;
		<setArmsy>c__IteratorB.<>f__this = this;
		return <setArmsy>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator setVags(GameObject go)
	{
		mutantTypeSetup.<setVags>c__IteratorB6 <setVags>c__IteratorB = new mutantTypeSetup.<setVags>c__IteratorB6();
		<setVags>c__IteratorB.go = go;
		<setVags>c__IteratorB.<$>go = go;
		<setVags>c__IteratorB.<>f__this = this;
		return <setVags>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator setBaby(GameObject go)
	{
		mutantTypeSetup.<setBaby>c__IteratorB7 <setBaby>c__IteratorB = new mutantTypeSetup.<setBaby>c__IteratorB7();
		<setBaby>c__IteratorB.go = go;
		<setBaby>c__IteratorB.<$>go = go;
		<setBaby>c__IteratorB.<>f__this = this;
		return <setBaby>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator setFatCreepy(GameObject go)
	{
		mutantTypeSetup.<setFatCreepy>c__IteratorB8 <setFatCreepy>c__IteratorB = new mutantTypeSetup.<setFatCreepy>c__IteratorB8();
		<setFatCreepy>c__IteratorB.go = go;
		<setFatCreepy>c__IteratorB.<$>go = go;
		<setFatCreepy>c__IteratorB.<>f__this = this;
		return <setFatCreepy>c__IteratorB;
	}

	private void setSkinnyLeader()
	{
		this.setup.ai.leader = true;
	}

	[DebuggerHidden]
	private IEnumerator setMaleSkinny()
	{
		mutantTypeSetup.<setMaleSkinny>c__IteratorB9 <setMaleSkinny>c__IteratorB = new mutantTypeSetup.<setMaleSkinny>c__IteratorB9();
		<setMaleSkinny>c__IteratorB.<>f__this = this;
		return <setMaleSkinny>c__IteratorB;
	}

	[DebuggerHidden]
	private IEnumerator setFemaleSkinny()
	{
		mutantTypeSetup.<setFemaleSkinny>c__IteratorBA <setFemaleSkinny>c__IteratorBA = new mutantTypeSetup.<setFemaleSkinny>c__IteratorBA();
		<setFemaleSkinny>c__IteratorBA.<>f__this = this;
		return <setFemaleSkinny>c__IteratorBA;
	}

	[DebuggerHidden]
	private IEnumerator setFollower(GameObject leader)
	{
		mutantTypeSetup.<setFollower>c__IteratorBB <setFollower>c__IteratorBB = new mutantTypeSetup.<setFollower>c__IteratorBB();
		<setFollower>c__IteratorBB.leader = leader;
		<setFollower>c__IteratorBB.<$>leader = leader;
		<setFollower>c__IteratorBB.<>f__this = this;
		return <setFollower>c__IteratorBB;
	}

	private void setInCave(bool set)
	{
		if (this.ai.creepy || this.ai.creepy_baby || this.ai.creepy_male || this.ai.creepy_fat)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("inCaveBool").Value = set;
		}
		else
		{
			if (this.setup.pmSleep)
			{
				this.setup.pmSleep.FsmVariables.GetFsmBool("inCaveBool").Value = set;
			}
			if (this.setup.pmSleep)
			{
				this.setup.pmSleep.FsmVariables.GetFsmBool("spawnerInCave").Value = set;
			}
		}
		this.inCave = set;
	}

	private void setWakeUpInCave(bool set)
	{
		this.setup.dayCycle.sleepBlocker = set;
	}

	private void setSleeping(bool set)
	{
		if (this.ai.creepy || this.ai.creepy_baby || this.ai.creepy_male)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("sleepOnAwake").Value = set;
		}
	}

	[DebuggerHidden]
	private IEnumerator addFollower(GameObject follower)
	{
		mutantTypeSetup.<addFollower>c__IteratorBC <addFollower>c__IteratorBC = new mutantTypeSetup.<addFollower>c__IteratorBC();
		<addFollower>c__IteratorBC.follower = follower;
		<addFollower>c__IteratorBC.<$>follower = follower;
		<addFollower>c__IteratorBC.<>f__this = this;
		return <addFollower>c__IteratorBC;
	}

	private void setPaleOnCeiling(bool set)
	{
		this.animator.enabled = true;
		this.setup.pmSleep.FsmVariables.GetFsmBool("paleOnCeiling").Value = set;
		this.animator.SetBoolReflected("onCeilingBool", set);
	}

	private void forceCeilingBool()
	{
		if (this.setup.pmSleep.FsmVariables.GetFsmBool("paleOnCeiling").Value)
		{
			this.animator.SetBoolReflected("paleMutantBool", true);
			this.animator.SetBoolReflected("onCeilingBool", true);
		}
	}

	[DebuggerHidden]
	private IEnumerator addToSpawn(GameObject go)
	{
		mutantTypeSetup.<addToSpawn>c__IteratorBD <addToSpawn>c__IteratorBD = new mutantTypeSetup.<addToSpawn>c__IteratorBD();
		<addToSpawn>c__IteratorBD.go = go;
		<addToSpawn>c__IteratorBD.<$>go = go;
		<addToSpawn>c__IteratorBD.<>f__this = this;
		return <addToSpawn>c__IteratorBD;
	}

	public void removeFromSpawn()
	{
		if (!this.removeSpawnBlock)
		{
			if (this.ai.leader)
			{
				this.sendRemoveLeader();
			}
			if (this.spawner)
			{
				this.spawner.allMembers.Remove(base.gameObject);
			}
			if (this.controlGo && this.spawner && this.spawner.allMembers.Count == 0)
			{
				this.updateSpawnManagers();
			}
			this.removeFromLists();
			this.updateSpawnerAmounts();
			if ((this.setup.ai.male || this.setup.ai.female) && base.gameObject.activeSelf)
			{
				base.StartCoroutine("doSpawnDummy");
			}
			this.removeSpawnBlock = true;
			base.Invoke("removeSpawnCoolDown", 15f);
		}
	}

	private void removeSpawnCoolDown()
	{
		this.removeSpawnBlock = false;
	}

	public void removeFromSpawnAndExplode()
	{
		if (this.ai.leader)
		{
			this.sendRemoveLeader();
		}
		if (this.spawner)
		{
			this.spawner.allMembers.Remove(base.gameObject);
		}
		if (this.controlGo && this.spawner && this.spawner.allMembers.Count == 0)
		{
			this.updateSpawnManagers();
		}
		this.removeFromLists();
		this.updateSpawnerAmounts();
		if (PoolManager.Pools["enemies"].IsSpawned(base.transform))
		{
			PoolManager.Pools["enemies"].Despawn(base.transform);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void sendRemoveLeader()
	{
		foreach (GameObject current in this.spawner.allMembers)
		{
			if (current)
			{
				current.SendMessage("removeLeaderGo", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void removeLeaderGo()
	{
		if (this.setup.pmBrain)
		{
			this.setup.pmBrain.FsmVariables.GetFsmGameObject("leaderGo").Value = null;
		}
	}

	private void updateSpawnerAmounts()
	{
		if (this.spawner)
		{
			if (this.ai.maleSkinny && this.ai.pale)
			{
				this.spawner.amount_skinny_pale--;
			}
			if (this.ai.male && this.ai.pale && !this.ai.maleSkinny)
			{
				this.spawner.amount_pale--;
			}
			if (this.ai.creepy)
			{
				this.spawner.amount_vags--;
			}
			if (this.ai.creepy_baby)
			{
				this.spawner.amount_baby--;
			}
			if (this.ai.creepy_fat)
			{
				this.spawner.amount_fat--;
			}
			if (this.ai.creepy_male)
			{
				this.spawner.amount_armsy--;
			}
		}
	}

	private void removeFromLists()
	{
		if (this.mutantControl.activeCannibals.Contains(base.gameObject))
		{
			this.mutantControl.activeCannibals.Remove(base.gameObject);
		}
		if (this.mutantControl.activeCaveCannibals.Contains(base.gameObject))
		{
			this.mutantControl.activeCaveCannibals.Remove(base.gameObject);
		}
		if (this.mutantControl.activeWorldCannibals.Contains(base.gameObject))
		{
			this.mutantControl.activeWorldCannibals.Remove(base.gameObject);
		}
	}

	private void updateSpawnManagers()
	{
		if (Clock.Day > 1)
		{
			if (this.ai.pale && this.ai.maleSkinny)
			{
				if (this.spawnManager.offsetSkinnyPale > -5)
				{
					this.spawnManager.offsetSkinnyPale--;
				}
				this.mutantControl.numActiveSkinnyPaleSpawns--;
				if (this.spawnManager.desiredSkinnyPale > 0)
				{
					this.spawnManager.desiredSkinnyPale--;
				}
			}
			else if (this.ai.maleSkinny || this.ai.femaleSkinny)
			{
				if (this.spawnManager.offsetSkinny > -5)
				{
					this.spawnManager.offsetSkinny--;
				}
				this.mutantControl.numActiveSkinnySpawns--;
				if (this.spawnManager.desiredSkinny > 1)
				{
					this.spawnManager.desiredSkinny -= 2;
				}
				if (this.mutantControl.activeSkinnyCannibals.Contains(base.gameObject))
				{
					this.mutantControl.activeSkinnyCannibals.Remove(base.gameObject);
				}
			}
			else if (this.ai.pale && this.ai.male)
			{
				if (this.spawnManager.offsetPale > -5)
				{
					this.spawnManager.offsetPale--;
				}
				this.mutantControl.numActivePaleSpawns--;
				if (this.spawnManager.desiredPale > 0)
				{
					this.spawnManager.desiredPale--;
				}
			}
			else if (this.ai.male || this.ai.female)
			{
				if (this.spawnManager.offsetRegular > -5)
				{
					this.spawnManager.offsetRegular--;
				}
				this.mutantControl.numActiveRegularSpawns--;
				if (this.spawnManager.desiredRegular > 0)
				{
					this.spawnManager.desiredRegular--;
				}
			}
			else if (this.ai.creepy || this.ai.creepy_male || this.ai.creepy_fat)
			{
				if (this.spawnManager.offsetCreepy > -5)
				{
					this.spawnManager.offsetCreepy--;
				}
				this.mutantControl.numActiveCreepySpawns--;
				if (this.spawnManager.desiredCreepy > 0)
				{
					this.spawnManager.desiredCreepy--;
				}
			}
		}
		UnityEngine.Object.Destroy(this.spawner.gameObject);
		this.mutantControl.numActiveSpawns--;
		if (Scene.MutantControler.hordeModeActive && Scene.MutantControler.activeWorldCannibals.Count == 0)
		{
			UnityEngine.Debug.Log("all mutants dead, preparing next horde level..");
			Scene.MutantControler.doNextHordeWave();
		}
	}

	[DebuggerHidden]
	private IEnumerator doSpawnDummy()
	{
		mutantTypeSetup.<doSpawnDummy>c__IteratorBE <doSpawnDummy>c__IteratorBE = new mutantTypeSetup.<doSpawnDummy>c__IteratorBE();
		<doSpawnDummy>c__IteratorBE.<>f__this = this;
		return <doSpawnDummy>c__IteratorBE;
	}

	public void armsyInCave()
	{
		this.dayCycle.armsyInCave();
	}

	private void enableWeapon()
	{
		if (!this.setup.ai.pale && !this.setup.ai.female && this.setup.weapon)
		{
			this.setup.weapon.SetActive(true);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == this.waterLayer)
		{
			this.inWater = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == this.waterLayer)
		{
			this.inWater = false;
		}
	}

	public void setVisionRange(float range)
	{
		this.setup.search.modifiedVisRange = range;
	}

	public void setLighterRange(float range)
	{
		this.setup.search.modLighterRange = range;
	}

	public void setMudRange(float range)
	{
		this.setup.search.modMudRange = range;
	}

	public void setCrouchRange(float range)
	{
		this.setup.search.modCrouchRange = range;
	}

	private void setVisionLayersOn()
	{
		if (this.setup.pmVision)
		{
			this.setup.pmVision.FsmVariables.GetFsmInt("smallTreeLayer").Value = 2;
		}
		if (this.setup.pmSearch)
		{
			this.setup.search.setupCrouchedVisLayerMask();
		}
	}

	private void setVisionLayersOff()
	{
		if (this.setup.pmVision)
		{
			this.setup.pmVision.FsmVariables.GetFsmInt("smallTreeLayer").Value = 12;
		}
		if (this.setup.pmSearch)
		{
			this.setup.search.setupStandingVisLayerMask();
		}
	}

	private void killThisEnemy()
	{
		this.health.HitReal(1000);
	}

	public void initWakeUp()
	{
		if (this.setup.dayCycle)
		{
			this.setup.dayCycle.initWakeUp();
		}
	}

	private void setPatrolling(bool on)
	{
		if (on)
		{
			this.setup.pmSearch.FsmVariables.GetFsmBool("doPatrolBool").Value = true;
		}
		else
		{
			this.setup.pmSearch.FsmVariables.GetFsmBool("doPatrolBool").Value = false;
		}
	}

	private void eatingOnSpawn(bool onoff)
	{
		if (onoff)
		{
			this.setup.pmSleep.FsmVariables.GetFsmBool("doEating").Value = true;
		}
		else
		{
			this.setup.pmSleep.FsmVariables.GetFsmBool("doEating").Value = false;
		}
	}

	private void setInstantSpawn(bool set)
	{
		if (set && !Scene.MutantControler.activeInstantSpawnedCannibals.Contains(base.gameObject))
		{
			Scene.MutantControler.activeInstantSpawnedCannibals.Add(base.gameObject);
		}
		this.instantSpawn = set;
	}

	public void getRagDollName(int name)
	{
		this.storedRagDollName = name;
	}
}
