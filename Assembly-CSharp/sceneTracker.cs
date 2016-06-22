using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Tools;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;

public class sceneTracker : MonoBehaviour
{
	public class EnemyInSightEvent : UnityEvent<GameObject>
	{
	}

	private enum EnemyPresence
	{
		None,
		Nearby,
		Watching,
		Attacking
	}

	private const float MUSIC_PREAMBIENT = 0f;

	private const float MUSIC_AMBIENT = 1.5f;

	private const float MUSIC_STRESS = 2.5f;

	private const float MUSIC_COMBAT = 3.5f;

	public PlayerVis Eye;

	public GameObject worldCollision;

	public string SurfaceMusicPath;

	public string CaveMusicPath;

	[Range(0f, 100f), Tooltip("Percentage chance that stress music will play when above ground and enemies are nearby")]
	public float SurfaceStressMusicChance = 50f;

	[Range(0f, 100f), Tooltip("Percentage chance that stress music will play when in a cave and enemies are nearby")]
	public float CaveStressMusicChance = 50f;

	public TreeLodGrid treeLodGrid;

	public GameObject[] ceilingMarkers;

	public GameObject[] groupEncounters;

	public GameObject[] singleEncounters;

	public GameObject[] crashMarkers;

	public GameObject planeCrash;

	public GameObject[] playerHangingMarkers;

	public GameObject[] timmyCaveMarkers;

	public GameObject closeStructureTarget;

	public List<GameObject> storedRagDollPrefabs = new List<GameObject>();

	public List<GameObject> closeTrees = new List<GameObject>();

	public List<GameObject> beachMarkers = new List<GameObject>();

	public List<GameObject> swimMarkers = new List<GameObject>();

	public List<GameObject> caveMarkers = new List<GameObject>();

	public List<GameObject> waypointMarkers = new List<GameObject>();

	public List<Transform> drinkMarkers = new List<Transform>();

	public List<Transform> dragMarkers = new List<Transform>();

	public List<GameObject> visibleEnemies = new List<GameObject>();

	public List<GameObject> closeEnemies = new List<GameObject>();

	public List<GameObject> allPlayers = new List<GameObject>();

	public List<BoltEntity> allPlayerEntities = new List<BoltEntity>();

	public List<Transform> jumpObjects = new List<Transform>();

	public List<GameObject> recentlyBuilt = new List<GameObject>();

	public List<GameObject> structuresBuilt = new List<GameObject>();

	public List<GameObject> encounters = new List<GameObject>();

	public List<GameObject> feedingEncounters = new List<GameObject>();

	public List<Transform> caveWayPoints = new List<Transform>();

	public List<GameObject> allRaccoons = new List<GameObject>();

	public List<GameObject> allRabbits = new List<GameObject>();

	public List<GameObject> allDeer = new List<GameObject>();

	public List<GameObject> allSquirrel = new List<GameObject>();

	public List<GameObject> allBoar = new List<GameObject>();

	public List<GameObject> allRabbitTraps = new List<GameObject>();

	public List<GameObject> allTrapTriggers = new List<GameObject>();

	public List<GameObject> allPlayerFires = new List<GameObject>();

	public List<GameObject> allPlayersInCave = new List<GameObject>();

	public List<mutantAiManager> aiManagers = new List<mutantAiManager>();

	public Vector3 playerVisDir;

	public static bool NearEnemies;

	public static bool AlertedEnemies;

	public int currLizardAmount;

	public int currTurtleAmount;

	public int currRabbitAmount;

	public int currTortoiseAmount;

	public int currRaccoonAmount;

	public int currDeerAmount;

	public int maxLizardAmount;

	public int maxTurtleAmount;

	public int maxRabbitAmount;

	public int maxSharkAmount;

	public int maxTortoiseAmount;

	public int maxRaccoonAmount;

	public int maxDeerAmount;

	private int initLizardAmount;

	private int initTurtleAmount;

	private int initRabbitAmount;

	private int initTortoiseAmount;

	private int initRaccoonAmount;

	private int initDeerAmount;

	private int initMaxLizards;

	private int initMaxRabbits;

	private int initMaxTortoise;

	private int initMaxRaccoon;

	private int initMaxDeer;

	public bool hasSearchedFire;

	public bool hasSearchedTree;

	public bool hasAttackedPlayer;

	private FMOD.Studio.EventInstance SurfaceMusic;

	private FMOD.Studio.EventInstance CaveMusic;

	private FMOD.Studio.EventInstance ActiveMusic;

	private ParameterInstance TransitionParameter;

	private bool MusicEnabled = true;

	private bool CombatMusicEnabled = true;

	private sceneTracker.EnemyPresence CurrentEnemyPresence;

	private void OnDeserialized()
	{
		this.doAwake();
	}

	private void Awake()
	{
		this.doAwake();
	}

	private void doAwake()
	{
		this.initMaxLizards = this.maxLizardAmount;
		this.initMaxRabbits = this.maxRabbitAmount;
		this.crashMarkers = GameObject.FindGameObjectsWithTag("crashMarker");
		this.planeCrash = GameObject.FindGameObjectWithTag("planeCrash");
		if (!this.planeCrash)
		{
			this.planeCrash = GameObject.FindGameObjectWithTag("savePlanePos");
		}
	}

	private void Start()
	{
		this.resetHasAttackedPlayer();
		this.resetSearchingFire();
		this.ceilingMarkers = GameObject.FindGameObjectsWithTag("ceiling");
		this.initLizardAmount = this.maxLizardAmount;
		this.initRabbitAmount = this.maxRabbitAmount;
		this.initTurtleAmount = this.maxTurtleAmount;
		this.initTortoiseAmount = this.maxTortoiseAmount;
		this.initRaccoonAmount = this.maxRaccoonAmount;
		this.initDeerAmount = this.maxDeerAmount;
		if (FMOD_StudioSystem.instance)
		{
			this.SurfaceMusic = FMOD_StudioSystem.instance.GetEvent(this.SurfaceMusicPath);
			this.CaveMusic = FMOD_StudioSystem.instance.GetEvent(this.CaveMusicPath);
		}
		else
		{
			UnityEngine.Debug.LogError("FMOD_StudioSystem.instance is null, could not initialize sceneTracker audio");
		}
		this.MusicEnabled = true;
		this.CombatMusicEnabled = true;
		base.InvokeRepeating("updateAnimalCount", 1f, 60f);
		base.Invoke("checkPlanePos", 0.5f);
		base.InvokeRepeating("cleanupPlayerLists", 1f, 1f);
	}

	private void OnDisable()
	{
		this.StopMusic();
		FMODCommon.ReleaseIfValid(this.SurfaceMusic, STOP_MODE.IMMEDIATE);
		FMODCommon.ReleaseIfValid(this.CaveMusic, STOP_MODE.IMMEDIATE);
	}

	public GameObject GetClosestPlayerFromPos(Vector3 worldPos)
	{
		GameObject result = null;
		int count = this.allPlayers.Count;
		if (count > 0)
		{
			float num = 3.40282347E+38f;
			for (int i = 0; i < this.allPlayers.Count; i++)
			{
				GameObject gameObject = this.allPlayers[i];
				if (gameObject)
				{
					float num2 = Vector3.Distance(worldPos, gameObject.transform.position);
					if (num2 < num)
					{
						num = num2;
						result = gameObject;
					}
				}
			}
		}
		return result;
	}

	public float GetClosestPlayerDistanceFromPos(Vector3 worldPos)
	{
		int count = this.allPlayers.Count;
		float num;
		if (count > 0)
		{
			num = 3.40282347E+38f;
			for (int i = 0; i < this.allPlayers.Count; i++)
			{
				GameObject gameObject = this.allPlayers[i];
				if (gameObject)
				{
					float num2 = Vector3.Distance(worldPos, gameObject.transform.position);
					if (num2 < num)
					{
						num = num2;
					}
				}
			}
		}
		else
		{
			num = 3.40282347E+38f;
		}
		return num;
	}

	private void cleanupPlayerLists()
	{
		this.allPlayers.RemoveAll((GameObject o) => o == null);
		this.allPlayersInCave.RemoveAll((GameObject o) => o == null);
		this.allPlayers.TrimExcess();
		this.allPlayersInCave.TrimExcess();
	}

	private void updateAnimalCount()
	{
		if (this.maxLizardAmount < this.initLizardAmount)
		{
			this.maxLizardAmount++;
		}
		if (this.maxRabbitAmount < this.initRabbitAmount)
		{
			this.maxRabbitAmount++;
		}
		if (this.maxTurtleAmount < this.initTurtleAmount)
		{
			this.maxTurtleAmount++;
		}
		if (this.maxTortoiseAmount < this.initTortoiseAmount)
		{
			this.maxTortoiseAmount++;
		}
		if (this.maxRaccoonAmount < this.initRaccoonAmount)
		{
			this.maxRaccoonAmount++;
		}
		if (this.maxDeerAmount < this.initDeerAmount)
		{
			this.maxDeerAmount++;
		}
	}

	private void StopMusic()
	{
		if (this.ActiveMusic != null && this.ActiveMusic.isValid())
		{
			UnityUtil.ERRCHECK(this.ActiveMusic.stop(STOP_MODE.ALLOWFADEOUT));
			this.ActiveMusic = null;
		}
	}

	private void SetActiveMusic(FMOD.Studio.EventInstance newMusic)
	{
		if (this.MusicEnabled && this.ActiveMusic != newMusic)
		{
			float value = 0f;
			if (this.TransitionParameter != null)
			{
				UnityUtil.ERRCHECK(this.TransitionParameter.getValue(out value));
			}
			this.StopMusic();
			this.ActiveMusic = newMusic;
			UnityUtil.ERRCHECK(this.ActiveMusic.getParameter("Transition", out this.TransitionParameter));
			UnityUtil.ERRCHECK(this.TransitionParameter.setValue(value));
			UnityUtil.ERRCHECK(this.ActiveMusic.start());
		}
	}

	public void DisableMusic()
	{
		this.MusicEnabled = false;
		this.StopMusic();
	}

	public void EnableMusic()
	{
		this.MusicEnabled = true;
		this.CombatMusicEnabled = true;
	}

	private sceneTracker.EnemyPresence CalculateEnemyPresence()
	{
		if (this.visibleEnemies.Count > 0 && LocalPlayer.ScriptSetup.proxyAttackers.arrayList.Count > 0)
		{
			return sceneTracker.EnemyPresence.Attacking;
		}
		if (this.closeEnemies.Count > 0)
		{
			return sceneTracker.EnemyPresence.Nearby;
		}
		return sceneTracker.EnemyPresence.None;
	}

	private void EnableCombatMusic()
	{
		this.CombatMusicEnabled = true;
	}

	private void Update()
	{
		if (Clock.InCave)
		{
			this.SetActiveMusic(this.CaveMusic);
		}
		else
		{
			this.SetActiveMusic(this.SurfaceMusic);
		}
		try
		{
			sceneTracker.EnemyPresence enemyPresence = this.CalculateEnemyPresence();
			bool flag = enemyPresence == sceneTracker.EnemyPresence.Attacking && !this.CombatMusicEnabled;
			if (enemyPresence != this.CurrentEnemyPresence && !flag)
			{
				if (this.CurrentEnemyPresence == sceneTracker.EnemyPresence.Attacking)
				{
					this.CombatMusicEnabled = false;
					base.Invoke("EnableCombatMusic", 180f);
				}
				this.CurrentEnemyPresence = enemyPresence;
				switch (this.CurrentEnemyPresence)
				{
				case sceneTracker.EnemyPresence.None:
					UnityUtil.ERRCHECK(this.TransitionParameter.setValue(0f));
					break;
				case sceneTracker.EnemyPresence.Nearby:
				{
					float num = (!Clock.InCave) ? this.SurfaceStressMusicChance : this.CaveStressMusicChance;
					if (UnityEngine.Random.Range(0f, 100f) < num)
					{
						UnityUtil.ERRCHECK(this.TransitionParameter.setValue(2.5f));
					}
					else
					{
						UnityUtil.ERRCHECK(this.TransitionParameter.setValue(0f));
					}
					break;
				}
				case sceneTracker.EnemyPresence.Attacking:
					UnityUtil.ERRCHECK(this.TransitionParameter.setValue(3.5f));
					break;
				}
			}
			if (this.closeEnemies.Count <= 0)
			{
				sceneTracker.NearEnemies = false;
			}
		}
		catch
		{
		}
	}

	private void checkPlanePos()
	{
		if (!this.planeCrash)
		{
			this.planeCrash = GameObject.FindGameObjectWithTag("savePlanePos");
		}
	}

	private void sendSetupFamilies()
	{
		if (Scene.MutantControler)
		{
			Scene.MutantControler.startSetupFamilies();
		}
	}

	public void WentDark()
	{
		this.setNightAnimalAmount();
		this.updateMutantSpawners();
	}

	public void WentLight()
	{
		this.setDayAnimalAmount();
		if (Cheats.NoEnemiesDuringDay)
		{
			Scene.MutantControler.StartCoroutine("removeAllEnemies");
		}
		else
		{
			if (Scene.MutantSpawnManager.gameObject.activeSelf && !Clock.planecrash)
			{
				Scene.MutantSpawnManager.setMutantSpawnAmounts();
			}
			if (Scene.MutantControler.gameObject.activeSelf && !Clock.planecrash && !Clock.InCave)
			{
				foreach (GameObject current in Scene.MutantControler.activeCannibals)
				{
					if (current)
					{
						current.SendMessage("switchToSleep", SendMessageOptions.DontRequireReceiver);
					}
				}
				Scene.MutantSpawnManager.setMutantSpawnAmounts();
				Scene.MutantControler.activateNextSpawn();
			}
		}
	}

	public void updateMutantSpawners()
	{
		if (Scene.MutantSpawnManager.gameObject.activeSelf && !Clock.planecrash)
		{
			Scene.MutantSpawnManager.setMutantSpawnAmounts();
		}
		if (Scene.MutantControler.gameObject.activeSelf && !Clock.planecrash)
		{
			Scene.MutantControler.activateNextSpawn();
		}
	}

	private void setNightAnimalAmount()
	{
		this.maxLizardAmount = this.initMaxLizards;
		this.maxRabbitAmount = this.initMaxRabbits;
	}

	private void setDayAnimalAmount()
	{
		this.maxLizardAmount = this.initMaxLizards;
		this.maxRabbitAmount = this.initMaxRabbits;
	}

	[DebuggerHidden]
	public IEnumerator updatePlayerVisDir(Vector3 dir)
	{
		sceneTracker.<updatePlayerVisDir>c__IteratorF5 <updatePlayerVisDir>c__IteratorF = new sceneTracker.<updatePlayerVisDir>c__IteratorF5();
		<updatePlayerVisDir>c__IteratorF.dir = dir;
		<updatePlayerVisDir>c__IteratorF.<$>dir = dir;
		<updatePlayerVisDir>c__IteratorF.<>f__this = this;
		return <updatePlayerVisDir>c__IteratorF;
	}

	public void addToBuilt(GameObject go)
	{
		if (this.recentlyBuilt.Count > 10)
		{
			this.recentlyBuilt.RemoveAll((GameObject o) => o == null);
		}
		if (this.recentlyBuilt.Count > 10)
		{
			this.recentlyBuilt.RemoveAt(0);
		}
		this.recentlyBuilt.Add(go);
	}

	public void addToStructures(GameObject go)
	{
		this.structuresBuilt.Add(go);
		this.structuresBuilt.RemoveAll((GameObject o) => o == null);
	}

	public void addToVisible(GameObject go)
	{
		if (!this.visibleEnemies.Contains(go))
		{
			EventRegistry.Enemy.Publish(TfEvent.EnemyInSight, go);
			this.visibleEnemies.Add(go);
		}
	}

	public void addToEncounters(GameObject go)
	{
		if (!this.encounters.Contains(go))
		{
			this.encounters.Add(go);
		}
	}

	public void removeFromEncounters(GameObject go)
	{
		if (this.encounters.Contains(go))
		{
			this.encounters.Remove(go);
		}
	}

	public void removeFromVisible(GameObject go)
	{
		if (this.visibleEnemies.Contains(go))
		{
			this.visibleEnemies.Remove(go);
		}
	}

	public void addToJump(Transform tr)
	{
		if (!this.jumpObjects.Contains(tr))
		{
			this.jumpObjects.Add(tr);
		}
	}

	public void removeFromJump(Transform tr)
	{
		if (this.jumpObjects.Contains(tr))
		{
			this.jumpObjects.Remove(tr);
		}
	}

	public void resetSearchingFire()
	{
		this.hasSearchedFire = false;
	}

	public void resetHasAttackedPlayer()
	{
		this.hasAttackedPlayer = false;
	}
}
