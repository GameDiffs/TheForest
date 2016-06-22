using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantController : MonoBehaviour
{
	public mutantSpawnManager spawnManager;

	public GameObject spawnGo;

	public Transform divider;

	public Transform navRef;

	public bool overrideClockDay;

	public int setDay;

	public int maxActiveMutants;

	public int maxActiveSpawners;

	public int numActiveSkinnySpawns;

	public int numActiveSkinnyPaleSpawns;

	public int numActiveRegularSpawns;

	public int numActivePaleSpawns;

	public int numActiveCreepySpawns;

	public int numActiveSpawns;

	public int deadFamilies;

	public int numActiveCannibals;

	private GameObject[] allFamilySpawnsGo;

	private GameObject[] allPaleSpawnsGo;

	private GameObject[] allCreepySpawnsGo;

	private GameObject[] allSpawnPointsGo;

	private GameObject[] allCaveSpawnsGo;

	public List<GameObject> activeFamilies = new List<GameObject>();

	public List<GameObject> allSkinnySpawns = new List<GameObject>();

	public List<GameObject> allSkinnyPaleSpawns = new List<GameObject>();

	public List<GameObject> allRegularSpawns = new List<GameObject>();

	public List<GameObject> allPaleSpawns = new List<GameObject>();

	public List<GameObject> allCreepySpawns = new List<GameObject>();

	public List<GameObject> allWorldSpawns = new List<GameObject>();

	public List<GameObject> allCaveSpawns = new List<GameObject>();

	public List<GameObject> allSleepingSpawns = new List<GameObject>();

	public List<GameObject> activeCannibals = new List<GameObject>();

	public List<GameObject> activeCaveCannibals = new List<GameObject>();

	public List<GameObject> activeWorldCannibals = new List<GameObject>();

	public List<GameObject> activeSkinnyCannibals = new List<GameObject>();

	public List<GameObject> allSpawnPoints = new List<GameObject>();

	public List<GameObject> activeInstantSpawnedCannibals = new List<GameObject>();

	private int count;

	private int spawnCounter;

	private bool setupBreak;

	private bool startDelay;

	private bool firstTimeSetup;

	private bool worldMutantsActive;

	public bool skipInitialDelay;

	public bool disableAllEnemies;

	public bool hordeModeActive;

	public bool hordeModePaused;

	public bool hordeConstantSpawning;

	public int hordeLevel;

	public int startHordeSpawnDelay = 60;

	public int nextWaveSpawnDelay = 30;

	private bool doneNewHordeWave;

	private void OnDeserialized()
	{
		Clock.planecrash = false;
		this.startDelay = false;
	}

	private void Awake()
	{
		if (Cheats.NoEnemies)
		{
			this.maxActiveMutants = 0;
		}
	}

	private void Start()
	{
		base.Invoke("doStart", 5f);
		base.Invoke("doFirstTimeSetup", 120f);
	}

	private void doStart()
	{
		this.allCaveSpawnsGo = GameObject.FindGameObjectsWithTag("mutantSpawn");
		this.allPaleSpawnsGo = GameObject.FindGameObjectsWithTag("mutantPaleSpawn");
		this.allSpawnPointsGo = GameObject.FindGameObjectsWithTag("entrance");
		this.allRegularSpawns.Clear();
		this.allPaleSpawns.Clear();
		for (int i = 0; i < this.allCaveSpawnsGo.Length; i++)
		{
			this.allCaveSpawns.Add(this.allCaveSpawnsGo[i]);
		}
		for (int j = 0; j < this.allPaleSpawnsGo.Length; j++)
		{
			this.allPaleSpawns.Add(this.allPaleSpawnsGo[j]);
		}
		for (int k = 0; k < this.allSpawnPointsGo.Length; k++)
		{
			this.allSpawnPoints.Add(this.allSpawnPointsGo[k]);
		}
		if (Clock.Day == 0)
		{
			if (!this.skipInitialDelay)
			{
				this.startDelay = false;
				if (Clock.Dark)
				{
					base.Invoke("disableStartDelay", 100f);
				}
				else
				{
					base.Invoke("disableStartDelay", 270f);
				}
			}
			else if (!Clock.planecrash)
			{
				this.startSetupFamilies();
				if (this.hordeModeActive)
				{
					if (this.hordeConstantSpawning)
					{
						base.InvokeRepeating("doConstantHordeSpawning", 1f, 20f);
					}
					else
					{
						base.InvokeRepeating("doHordeSpawning", 12f, 3f);
					}
				}
			}
		}
		else if (!Clock.planecrash)
		{
			this.startSetupFamilies();
		}
	}

	private void Update()
	{
		if (this.overrideClockDay)
		{
			Clock.Day = this.setDay;
		}
	}

	private void doFirstTimeSetup()
	{
		if (!this.firstTimeSetup)
		{
			base.StartCoroutine("setupFamilies");
		}
	}

	private void enableDoDespawn()
	{
	}

	private void disableStartDelay()
	{
		this.startDelay = false;
		this.startSetupFamilies();
	}

	[DebuggerHidden]
	private IEnumerator despawnGo(GameObject go)
	{
		mutantController.<despawnGo>c__Iterator70 <despawnGo>c__Iterator = new mutantController.<despawnGo>c__Iterator70();
		<despawnGo>c__Iterator.go = go;
		<despawnGo>c__Iterator.<$>go = go;
		<despawnGo>c__Iterator.<>f__this = this;
		return <despawnGo>c__Iterator;
	}

	public void startSetupFamilies()
	{
		if (this.hordeModeActive)
		{
			return;
		}
		Scene.ActiveMB.StartCoroutine(this.setupFamilies());
	}

	[DebuggerHidden]
	public IEnumerator setupFamilies()
	{
		mutantController.<setupFamilies>c__Iterator71 <setupFamilies>c__Iterator = new mutantController.<setupFamilies>c__Iterator71();
		<setupFamilies>c__Iterator.<>f__this = this;
		return <setupFamilies>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator updateSpawns()
	{
		mutantController.<updateSpawns>c__Iterator72 <updateSpawns>c__Iterator = new mutantController.<updateSpawns>c__Iterator72();
		<updateSpawns>c__Iterator.<>f__this = this;
		return <updateSpawns>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator updateCaveSpawns()
	{
		mutantController.<updateCaveSpawns>c__Iterator73 <updateCaveSpawns>c__Iterator = new mutantController.<updateCaveSpawns>c__Iterator73();
		<updateCaveSpawns>c__Iterator.<>f__this = this;
		return <updateCaveSpawns>c__Iterator;
	}

	private void disableWorldMutantsActive()
	{
		this.worldMutantsActive = false;
	}

	public void activateNextSpawn()
	{
		base.StartCoroutine("updateSpawns");
	}

	public void updateFamilies()
	{
	}

	private void setDayConditions()
	{
		Scene.MutantSpawnManager.setMutantSpawnAmounts();
		if (Clock.InCave)
		{
			this.maxActiveMutants = 20;
		}
		else if (Clock.Day == 0 && !Clock.Dark)
		{
			this.maxActiveMutants = 15;
		}
		else if (Clock.Day == 0 && Clock.Dark)
		{
			this.maxActiveMutants = 16;
		}
		else if (Clock.Day == 1 && !Clock.Dark)
		{
			this.maxActiveMutants = 15;
		}
		else if (Clock.Day == 1 && Clock.Dark)
		{
			this.maxActiveMutants = 16;
		}
		else if (Clock.Day == 2 && !Clock.Dark)
		{
			this.maxActiveMutants = 15;
		}
		else if (Clock.Day == 2 && Clock.Dark)
		{
			this.maxActiveMutants = 16;
		}
		else if (Clock.Day == 3 && !Clock.Dark)
		{
			this.maxActiveMutants = 15;
		}
		else if (Clock.Day == 3 && Clock.Dark)
		{
			this.maxActiveMutants = 18;
		}
		else if (Clock.Day == 4 && !Clock.Dark)
		{
			this.maxActiveMutants = 15;
		}
		else if (Clock.Day == 4 && Clock.Dark)
		{
			this.maxActiveMutants = 18;
		}
		else if (Clock.Day >= 5 && !Clock.Dark)
		{
			this.maxActiveMutants = 16;
		}
		else if (Clock.Day >= 5 && Clock.Dark)
		{
			this.maxActiveMutants = 18;
		}
	}

	private void sortCaveSpawnsByDistance(Transform target)
	{
		this.allCaveSpawns.RemoveAll((GameObject o) => o == null);
		this.allCaveSpawns.TrimExcess();
		this.allCaveSpawns.Sort((GameObject c1, GameObject c2) => Vector3.Distance(target.position, c1.transform.position).CompareTo(Vector3.Distance(target.position, c2.transform.position)));
	}

	private void sortSpawnPointsByDistance()
	{
		this.allSpawnPoints.RemoveAll((GameObject o) => o == null);
		this.allCaveSpawns.RemoveAll((GameObject o) => o == null);
		this.allSpawnPoints.TrimExcess();
		this.allCaveSpawns.TrimExcess();
		this.allSpawnPoints.Sort((GameObject c1, GameObject c2) => Vector3.Distance(LocalPlayer.Transform.position, c1.transform.position).CompareTo(Vector3.Distance(LocalPlayer.Transform.position, c2.transform.position)));
		this.allCaveSpawns.Sort((GameObject c1, GameObject c2) => Vector3.Distance(LocalPlayer.Transform.position, c1.transform.position).CompareTo(Vector3.Distance(LocalPlayer.Transform.position, c2.transform.position)));
	}

	public Transform findClosestEnemy(Transform trn)
	{
		float num = float.PositiveInfinity;
		GameObject gameObject = null;
		foreach (GameObject current in this.activeCannibals)
		{
			if (current != null)
			{
				float magnitude = (current.transform.position - trn.position).magnitude;
				if (magnitude < num)
				{
					gameObject = current;
					num = magnitude;
				}
			}
		}
		if (gameObject != null)
		{
			return gameObject.transform;
		}
		return null;
	}

	private void resetSetupBreak()
	{
		this.setupBreak = false;
	}

	private bool pointOffCamera(Vector3 pos)
	{
		Vector3 vector = LocalPlayer.MainCam.WorldToViewportPoint(pos);
		return vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f;
	}

	private void setupSkinnySpawn(spawnMutants spawn)
	{
		if (Clock.Day < 6)
		{
			spawn.amount_female_skinny = UnityEngine.Random.Range(0, 2);
		}
		else
		{
			spawn.amount_female_skinny = UnityEngine.Random.Range(0, 3);
		}
		if (spawn.amount_female_skinny == 0)
		{
			spawn.amount_male_skinny = 2;
		}
		else if (Clock.Day < 6)
		{
			spawn.amount_male_skinny = UnityEngine.Random.Range(1, 3);
		}
		else
		{
			spawn.amount_male_skinny = UnityEngine.Random.Range(1, 4);
		}
		if (UnityEngine.Random.value > 0.1f && spawn.amount_male_skinny > 0)
		{
			spawn.leader = true;
		}
		else
		{
			spawn.leader = false;
		}
		this.numActiveSkinnySpawns++;
		this.numActiveSpawns++;
	}

	private void setupSkinnyPaleSpawn(spawnMutants spawn)
	{
		if (Clock.Day < 6)
		{
			spawn.amount_skinny_pale = UnityEngine.Random.Range(1, 4);
		}
		else
		{
			spawn.amount_skinny_pale = UnityEngine.Random.Range(2, 6);
		}
		spawn.leader = true;
		this.numActiveSkinnyPaleSpawns++;
		this.numActiveSpawns++;
	}

	private void setupRegularSpawn(spawnMutants spawn)
	{
		spawn.amount_male = UnityEngine.Random.Range(1, 4);
		if (Clock.Day > 8)
		{
			spawn.amount_female = UnityEngine.Random.Range(0, 3);
		}
		else
		{
			spawn.amount_female = UnityEngine.Random.Range(0, 2);
		}
		if (UnityEngine.Random.value > 0.7f && Clock.Day > 6)
		{
			spawn.amount_fireman = UnityEngine.Random.Range(0, 2);
		}
		if (UnityEngine.Random.value < 0.05f)
		{
			spawn.leader = false;
		}
		else
		{
			spawn.leader = true;
		}
		this.numActiveRegularSpawns++;
		this.numActiveSpawns++;
	}

	private void setupPaleSpawn(spawnMutants spawn)
	{
		if (Clock.Day < 10)
		{
			spawn.amount_pale = UnityEngine.Random.Range(1, 4);
		}
		else
		{
			spawn.amount_pale = UnityEngine.Random.Range(2, 6);
		}
		if (UnityEngine.Random.value < 0.2f)
		{
			spawn.leader = false;
		}
		else
		{
			spawn.leader = true;
		}
		spawn.pale = true;
		this.numActivePaleSpawns++;
		this.numActiveSpawns++;
	}

	private void setupCreepySpawn(spawnMutants spawn)
	{
		spawn.amount_vags = UnityEngine.Random.Range(0, 2);
		if (spawn.amount_vags == 0)
		{
			spawn.amount_armsy = UnityEngine.Random.Range(0, 2);
		}
		if (spawn.amount_armsy + spawn.amount_vags < 1 && Clock.Day > 12)
		{
			spawn.amount_fat = 1;
		}
		else if (UnityEngine.Random.value > 0.5f)
		{
			spawn.amount_vags = 1;
		}
		else
		{
			spawn.amount_armsy = 1;
		}
		if (spawn.amount_armsy > 0 || spawn.amount_fat > 0)
		{
			spawn.amount_skinny_pale = UnityEngine.Random.Range(1, 5);
		}
		if (spawn.amount_vags > 0)
		{
			spawn.amount_baby = UnityEngine.Random.Range(2, 6);
		}
		if (UnityEngine.Random.value > 0.65f && Clock.Day > 12)
		{
			spawn.pale = true;
		}
		spawn.leader = true;
		spawn.creepySpawner = true;
		this.numActiveCreepySpawns++;
		this.numActiveSpawns++;
	}

	public void enableMpCaveMutants()
	{
		if (!base.IsInvoking("activateClosestCaveSpawn"))
		{
			base.InvokeRepeating("activateClosestCaveSpawn", 3f, 1f);
		}
	}

	public void disableMpCaveMutants()
	{
		base.CancelInvoke("activeClosestCaveSpawn");
		if (Scene.SceneTracker.allPlayersInCave.Count == 0 && this.activeCaveCannibals.Count > 0)
		{
			foreach (GameObject current in this.activeCaveCannibals)
			{
				if (current)
				{
					base.StartCoroutine("despawnGo", current);
				}
			}
		}
		if (this.allCaveSpawns.Count > 0)
		{
			foreach (GameObject current2 in this.allCaveSpawns)
			{
				spawnMutants component = current2.GetComponent<spawnMutants>();
				component.enabled = false;
			}
		}
	}

	public void disableWorldMutants()
	{
		if (this.activeWorldCannibals.Count > 0)
		{
			foreach (GameObject current in this.activeWorldCannibals)
			{
				if (current)
				{
					base.StartCoroutine("despawnGo", current);
				}
			}
		}
	}

	private void activateClosestCaveSpawn()
	{
		if (this.allCaveSpawns.Count == 0)
		{
			return;
		}
		this.allCaveSpawns.RemoveAll((GameObject o) => o == null);
		foreach (GameObject current in Scene.SceneTracker.allPlayersInCave)
		{
			if (current)
			{
				if (this.allCaveSpawns.Count == 0)
				{
					break;
				}
				this.sortCaveSpawnsByDistance(current.transform);
				for (int i = 0; i < 6; i++)
				{
					if (Vector3.Distance(this.allCaveSpawns[i].transform.position, current.transform.position) < 180f)
					{
						spawnMutants component = this.allCaveSpawns[i].GetComponent<spawnMutants>();
						if (!component.enabled)
						{
							component.enabled = true;
							component.invokeSpawn();
						}
					}
				}
			}
		}
	}

	public void dawnSetupFamilies()
	{
		base.StartCoroutine("doDawnSetupFamilies");
	}

	[DebuggerHidden]
	public IEnumerator doDawnSetupFamilies()
	{
		mutantController.<doDawnSetupFamilies>c__Iterator74 <doDawnSetupFamilies>c__Iterator = new mutantController.<doDawnSetupFamilies>c__Iterator74();
		<doDawnSetupFamilies>c__Iterator.<>f__this = this;
		return <doDawnSetupFamilies>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator removeAllEnemies()
	{
		mutantController.<removeAllEnemies>c__Iterator75 <removeAllEnemies>c__Iterator = new mutantController.<removeAllEnemies>c__Iterator75();
		<removeAllEnemies>c__Iterator.<>f__this = this;
		return <removeAllEnemies>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator removeWorldMutants()
	{
		mutantController.<removeWorldMutants>c__Iterator76 <removeWorldMutants>c__Iterator = new mutantController.<removeWorldMutants>c__Iterator76();
		<removeWorldMutants>c__Iterator.<>f__this = this;
		return <removeWorldMutants>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator removeCaveMutants()
	{
		mutantController.<removeCaveMutants>c__Iterator77 <removeCaveMutants>c__Iterator = new mutantController.<removeCaveMutants>c__Iterator77();
		<removeCaveMutants>c__Iterator.<>f__this = this;
		return <removeCaveMutants>c__Iterator;
	}

	public void doHordeSpawning()
	{
		if (this.activeWorldCannibals.Count == 0 && !this.doneNewHordeWave && !this.hordeModePaused)
		{
			this.doNextHordeWave();
			this.doneNewHordeWave = true;
		}
		if (this.activeWorldCannibals.Count > 0)
		{
			this.doneNewHordeWave = false;
		}
	}

	private void doConstantHordeSpawning()
	{
		base.StartCoroutine("addToHordeSpawn");
	}

	public void doNextHordeWave()
	{
		this.hordeLevel++;
		if (this.hordeLevel == 1)
		{
			base.StartCoroutine("updateHordeSpawns", this.startHordeSpawnDelay);
		}
		else
		{
			base.StartCoroutine("updateHordeSpawns", this.nextWaveSpawnDelay);
		}
	}

	[DebuggerHidden]
	public IEnumerator updateHordeSpawns(int delay)
	{
		mutantController.<updateHordeSpawns>c__Iterator78 <updateHordeSpawns>c__Iterator = new mutantController.<updateHordeSpawns>c__Iterator78();
		<updateHordeSpawns>c__Iterator.delay = delay;
		<updateHordeSpawns>c__Iterator.<$>delay = delay;
		<updateHordeSpawns>c__Iterator.<>f__this = this;
		return <updateHordeSpawns>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator addToHordeSpawn()
	{
		mutantController.<addToHordeSpawn>c__Iterator79 <addToHordeSpawn>c__Iterator = new mutantController.<addToHordeSpawn>c__Iterator79();
		<addToHordeSpawn>c__Iterator.<>f__this = this;
		return <addToHordeSpawn>c__Iterator;
	}
}
