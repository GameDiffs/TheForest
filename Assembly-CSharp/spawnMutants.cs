using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class spawnMutants : MonoBehaviour
{
	private List<GameObject> allPlayers = new List<GameObject>();

	public float checkPlayerDist;

	public int amount_male_skinny;

	public int amount_female_skinny;

	public int amount_skinny_pale;

	public int amount_male;

	public int amount_female;

	public int amount_fireman;

	public bool useDynamiteMan;

	public int amount_pale;

	public int amount_armsy;

	public int amount_vags;

	public int amount_baby;

	public int amount_fat;

	public float range;

	public int daysTillSpawn;

	public int daysTillActiveDuringDay;

	public bool leader;

	public bool pale;

	public bool paleOnCeiling;

	public bool patrolling;

	public bool wakeUpOnSpawn;

	public bool eatingOnSpawn;

	public GameObject mutant;

	public GameObject mutant_female;

	public GameObject mutant_pale;

	public GameObject armsy;

	public GameObject vags;

	public GameObject baby;

	public GameObject fat;

	public bool sleepingSpawn;

	public bool spawnInCave;

	public bool sinkholeSpawn;

	public bool alignRotationToSpawn;

	public bool creepySpawner;

	public bool instantSpawn;

	public bool debugAddToLists;

	private bool alreadySpawned;

	public List<GameObject> allMembers = new List<GameObject>();

	private Transform clone;

	private GameObject leaderGo;

	private sceneTracker sceneInfo;

	private mutantController mutantControl;

	private Transform tr;

	private int nameCount;

	private bool doName;

	private void Awake()
	{
		if (BoltNetwork.isClient)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (Cheats.NoEnemies)
		{
			this.amount_skinny_pale = 0;
			this.amount_male_skinny = 0;
			this.amount_female_skinny = 0;
			this.amount_male = 0;
			this.amount_female = 0;
			this.amount_fireman = 0;
			this.amount_pale = 0;
			this.amount_armsy = 0;
			this.amount_vags = 0;
			this.amount_baby = 0;
			this.amount_fat = 0;
		}
		this.tr = base.transform;
		this.sceneInfo = Scene.SceneTracker;
		this.mutantControl = Scene.MutantControler;
	}

	private void Start()
	{
		if (Scene.LoadSave)
		{
			LoadSave.OnGameStart += delegate
			{
				if (this.instantSpawn || this.sinkholeSpawn)
				{
					base.InvokeRepeating("checkSpawn", 4f, 4f);
				}
			};
		}
		else if (this.instantSpawn || this.sinkholeSpawn)
		{
			base.InvokeRepeating("checkSpawn", 4f, 4f);
		}
	}

	private void OnDisable()
	{
		this.alreadySpawned = false;
		base.CancelInvoke("checkSpawn");
		base.CancelInvoke("updateSpawnConditions");
	}

	private void OnDestroy()
	{
		base.StopAllCoroutines();
		base.CancelInvoke("checkSpawn");
		base.CancelInvoke("updateSpawnConditions");
	}

	public void invokeSpawn()
	{
		this.updateSpawnConditions();
		base.InvokeRepeating("checkSpawn", 0f, 3f);
	}

	public void updateSpawnConditions()
	{
		int num = this.amount_male_skinny + this.amount_female_skinny + this.amount_skinny_pale + this.amount_male + this.amount_female + this.amount_fireman + this.amount_pale + this.amount_armsy + this.amount_vags + this.amount_baby + this.amount_fat;
		if (num < 1)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (!Clock.InCave)
		{
			this.mutantControl.allSleepingSpawns.RemoveAll((GameObject o) => o == null);
			if (this.mutantControl.allSleepingSpawns.Count < this.mutantControl.allWorldSpawns.Count / 2 && !Clock.Dark && this.mutantControl.allSleepingSpawns.Count < Scene.MutantSpawnManager.maxSleepingSpawns && Clock.Day < 7)
			{
				this.sleepingSpawn = true;
				if (!this.mutantControl.allSleepingSpawns.Contains(base.gameObject))
				{
					this.mutantControl.allSleepingSpawns.Add(base.gameObject);
				}
			}
			else
			{
				this.sleepingSpawn = false;
				if (this.mutantControl.allSleepingSpawns.Contains(base.gameObject))
				{
					this.mutantControl.allSleepingSpawns.Remove(base.gameObject);
				}
			}
		}
		foreach (GameObject current in this.allMembers)
		{
			if (current)
			{
				current.SendMessage("initWakeUp", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void addToWorldSpawns()
	{
		if (!this.instantSpawn)
		{
			this.mutantControl.allWorldSpawns.Add(base.gameObject);
		}
	}

	private void disableDuringDay()
	{
		foreach (GameObject current in this.allMembers)
		{
			if (!this.spawnInCave && !Clock.Dark)
			{
				if (current)
				{
					current.SetActive(false);
				}
				else
				{
					if (current)
					{
						current.SetActive(true);
					}
					base.CancelInvoke("disableDuringDay");
				}
			}
		}
	}

	private void checkSpawn()
	{
		if (!base.enabled)
		{
			return;
		}
		if (Scene.SceneTracker.allPlayers.Count == 0)
		{
			return;
		}
		this.allPlayers = new List<GameObject>(Scene.SceneTracker.allPlayers);
		this.allPlayers.RemoveAll((GameObject o) => o == null);
		if (this.allPlayers[0] == null)
		{
			return;
		}
		if (this.allPlayers.Count > 1)
		{
			this.allPlayers.Sort((GameObject c1, GameObject c2) => Vector3.Distance(base.transform.position, c1.transform.position).CompareTo(Vector3.Distance(base.transform.position, c2.transform.position)));
		}
		if (Clock.Day < this.daysTillSpawn)
		{
			return;
		}
		if (!this)
		{
			return;
		}
		this.checkPlayerDist = Vector3.Distance(this.allPlayers[0].transform.position, base.transform.position);
		if (this.spawnInCave && Scene.SceneTracker.allPlayersInCave.Count > 0 && this.checkPlayerDist < 130f && !this.alreadySpawned)
		{
			base.StartCoroutine("doSpawn");
			this.alreadySpawned = true;
		}
		else if (this.sinkholeSpawn && this.checkPlayerDist < 200f && !this.alreadySpawned)
		{
			base.StartCoroutine("doSpawn");
			this.alreadySpawned = true;
		}
		else if (!Clock.InCave && !this.spawnInCave && !this.sinkholeSpawn)
		{
			base.StartCoroutine("doSpawn");
			base.CancelInvoke("checkSpawn");
		}
		float num = 160f;
		if (this.sinkholeSpawn)
		{
			num = 225f;
		}
		if ((this.spawnInCave || this.sinkholeSpawn) && this.checkPlayerDist > num && this.alreadySpawned)
		{
			bool flag = false;
			if (Scene.SceneTracker.allPlayersInCave.Count > 0 && this.spawnInCave)
			{
				flag = true;
			}
			if (this.sinkholeSpawn)
			{
				flag = true;
			}
			if (flag)
			{
				bool flag2 = false;
				foreach (GameObject current in this.allMembers)
				{
					if (current && current.activeSelf && Vector3.Distance(current.transform.position, this.allPlayers[0].transform.position) < 160f)
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					base.StartCoroutine("despawnAll");
					this.alreadySpawned = false;
				}
			}
		}
	}

	public void clearSpawn()
	{
		this.allMembers.Clear();
	}

	[DebuggerHidden]
	private IEnumerator despawnAll()
	{
		spawnMutants.<despawnAll>c__IteratorFB <despawnAll>c__IteratorFB = new spawnMutants.<despawnAll>c__IteratorFB();
		<despawnAll>c__IteratorFB.<>f__this = this;
		return <despawnAll>c__IteratorFB;
	}

	[DebuggerHidden]
	private IEnumerator doSpawn()
	{
		spawnMutants.<doSpawn>c__IteratorFC <doSpawn>c__IteratorFC = new spawnMutants.<doSpawn>c__IteratorFC();
		<doSpawn>c__IteratorFC.<>f__this = this;
		return <doSpawn>c__IteratorFC;
	}

	public Vector2 Circle2(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	public GameObject findClosestCeilingPos()
	{
		GameObject gameObject = null;
		float num = float.PositiveInfinity;
		GameObject[] ceilingMarkers = this.sceneInfo.ceilingMarkers;
		for (int i = 0; i < ceilingMarkers.Length; i++)
		{
			GameObject gameObject2 = ceilingMarkers[i];
			float magnitude = (this.tr.position - gameObject2.transform.position).magnitude;
			if (magnitude < num && !gameObject2.CompareTag("ceilingOccupied"))
			{
				num = magnitude;
				gameObject = gameObject2;
			}
		}
		if (gameObject)
		{
			gameObject.tag = "ceilingOccupied";
		}
		return gameObject;
	}
}
