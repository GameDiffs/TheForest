using PathologicalGames;
using System;
using TheForest.Utils;
using UnityEngine;

public class animalSpawnFunctions : MonoBehaviour
{
	public bool lizard;

	public bool turtle;

	public bool rabbit;

	public bool fish;

	public bool tortoise;

	public bool raccoon;

	public bool deer;

	public bool squirrel;

	public bool boar;

	public bool crocodile;

	public Renderer meshRenderer;

	public Material snowMat;

	public Material defaultMat;

	private animalAI ai;

	private Fish fishAi;

	private Animator animator;

	public GameObject spawnGo;

	public animalController controller;

	public sceneTracker sceneInfo;

	private Transform thisTr;

	public Transform spawnInstance;

	public float despawnDistance;

	public float playerDist;

	private Vector3 screenPos;

	private bool started;

	private float initDespawnDistance;

	private bool inTrap;

	private void Awake()
	{
		if (this.fish)
		{
			this.fishAi = base.GetComponent<Fish>();
		}
		this.animator = base.GetComponent<Animator>();
		if (this.animator == null)
		{
			this.animator = base.GetComponentInChildren<Animator>();
		}
		this.ai = base.GetComponent<animalAI>();
		this.thisTr = base.transform;
		this.sceneInfo = Scene.SceneTracker;
	}

	private void Start()
	{
		this.animator.enabled = true;
		this.initDespawnDistance = this.despawnDistance;
	}

	private void startUpdateSpawn()
	{
	}

	private void setController(GameObject go)
	{
		this.spawnGo = go;
		this.controller = go.GetComponent<animalController>();
	}

	private void enableTrapDespawnDist()
	{
		this.inTrap = true;
		this.initDespawnDistance = this.despawnDistance;
		this.despawnDistance = 800f;
	}

	private void disableTrapDespawnDist()
	{
		this.inTrap = false;
		this.despawnDistance = this.initDespawnDistance;
	}

	private void OnEnable()
	{
		base.InvokeRepeating("updateSpawn", 4f, 2f);
		this.animator.enabled = true;
		if (this.raccoon && !this.sceneInfo.allRaccoons.Contains(base.gameObject))
		{
			this.sceneInfo.allRaccoons.Add(base.gameObject);
		}
		if (this.rabbit && !this.sceneInfo.allRabbits.Contains(base.gameObject))
		{
			this.sceneInfo.allRabbits.Add(base.gameObject);
		}
		if (this.deer && !this.sceneInfo.allDeer.Contains(base.gameObject))
		{
			this.sceneInfo.allDeer.Add(base.gameObject);
		}
		if (this.squirrel && !this.sceneInfo.allSquirrel.Contains(base.gameObject))
		{
			this.sceneInfo.allSquirrel.Add(base.gameObject);
		}
		if (this.boar && !this.sceneInfo.allBoar.Contains(base.gameObject))
		{
			this.sceneInfo.allBoar.Add(base.gameObject);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("updateSpawn");
	}

	private void updateSpawn()
	{
		if (this.fish)
		{
			if (this.fishAi.allPlayers.Count == 0)
			{
				return;
			}
			if (this.fishAi.allPlayers[0] != null && this.fishAi.allPlayers[0] != null)
			{
				this.playerDist = Vector3.Distance(this.fishAi.allPlayers[0].transform.position, this.thisTr.position);
			}
		}
		if (this.turtle)
		{
			if (this.ai.allPlayers.Count == 0)
			{
				return;
			}
			if (this.ai.allPlayers[0] != null)
			{
				if (this.ai.allPlayers[0] != null)
				{
					this.playerDist = Vector3.Distance(this.ai.allPlayers[0].transform.position, this.thisTr.position);
				}
				if (this.playerDist > this.despawnDistance || this.playerDist > 100f)
				{
					this.despawn();
				}
			}
		}
	}

	public void despawn()
	{
		this.animator.enabled = false;
		base.CancelInvoke("updateSpawn");
		if (this.turtle && BoltNetwork.isRunning)
		{
			BoltNetwork.Detach(base.gameObject);
			UnityEngine.Object.Destroy(base.gameObject.GetComponent<BoltEntity>());
			UnityEngine.Object.Destroy(base.gameObject.GetComponent<AnimalDespawner>());
		}
		if (PoolManager.Pools["creatures"].IsSpawned(base.transform))
		{
			PoolManager.Pools["creatures"].Despawn(base.transform);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (this.lizard)
		{
			this.sceneInfo.currLizardAmount--;
		}
		if (this.rabbit)
		{
			this.sceneInfo.currRabbitAmount--;
			this.sceneInfo.allRabbits.Remove(base.gameObject);
		}
		if (this.turtle)
		{
			this.controller.seCurrTurtleAmount(-1);
		}
		if (this.tortoise)
		{
			this.sceneInfo.currTortoiseAmount--;
		}
		if (this.raccoon)
		{
			this.sceneInfo.currRaccoonAmount--;
			this.sceneInfo.allRaccoons.Remove(base.gameObject);
		}
		if (this.deer)
		{
			this.sceneInfo.currDeerAmount--;
			this.sceneInfo.allDeer.Remove(base.gameObject);
		}
	}

	private bool pointOffCamera(Vector3 pos)
	{
		this.screenPos = LocalPlayer.MainCam.WorldToViewportPoint(pos);
		return this.screenPos.x < 0f || this.screenPos.x > 1f || this.screenPos.y < 0f || this.screenPos.y > 1f;
	}

	private bool rendererOffCamera(Vector3 pos)
	{
		return !this.meshRenderer.IsVisibleFrom(LocalPlayer.MainCam);
	}

	public void getInstance(Transform instance)
	{
		this.spawnInstance = instance;
	}

	public void setSnowType(bool onoff)
	{
		if (!this.snowMat)
		{
			return;
		}
		this.ai.ragdoll.animal = true;
		if (onoff)
		{
			this.meshRenderer.sharedMaterial = this.snowMat;
		}
		else
		{
			this.meshRenderer.sharedMaterial = this.defaultMat;
		}
	}
}
