using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class animalController : MonoBehaviour
{
	private sceneTracker sceneInfo;

	public int lizardAmount;

	public int turtleAmount;

	public int gooseAmount;

	public int rabbitAmount;

	public int fishAmount;

	public int tortoiseAmount;

	public int raccoonAmount;

	public int deerAmount;

	public GameObject lizardGo;

	public GameObject turtleGo;

	public GameObject gooseGo;

	public GameObject rabbitGo;

	public GameObject[] fishGo;

	public GameObject tortoiseGo;

	public GameObject raccoonGo;

	public GameObject deerGo;

	public bool oceanFish;

	public bool caveFish;

	public float lizardSpawnDist;

	public float turtleSpawnDist;

	public float rabbitSpawnDist;

	public float fishSpawnDist;

	public float fishSpawnRange;

	public float tortoiseSpawnDist;

	public float raccoonSpawnDist;

	public float deerSpawnDist;

	public List<GameObject> allFish = new List<GameObject>();

	private GameObject currentCamGo;

	private Camera currentCamera;

	private Transform currLizard;

	private Transform currTurtle;

	private Transform currRabbit;

	private Transform currFish;

	private Transform currSpawn;

	private Transform currTortoise;

	private Transform currRaccoon;

	private Transform currDeer;

	private Transform currAnimal;

	private int currTurtleAmount;

	public float addSpawnDelay = 1f;

	[SerializeThis]
	public float setSpawnDelay;

	private float delayUpdateInterval = 1f;

	private Transform thisTr;

	public int totalFish;

	private Vector3 screenPos;

	private Vector3 spawnPos;

	private Vector2 tempPos;

	private float terrainPosY;

	private bool foundSpawn;

	private int type;

	public bool fishSpawned;

	private RaycastHit hit;

	private int layer;

	private int layerMask;

	public float fishPlayerDist;

	private float closestDist = float.PositiveInfinity;

	private Transform closestPlayer;

	private void Awake()
	{
		this.sceneInfo = Scene.SceneTracker;
		this.thisTr = base.transform;
		this.layer = 26;
		this.layerMask = 1 << this.layer;
	}

	private void OnEnable()
	{
		base.InvokeRepeating("callSpawnCreatures", UnityEngine.Random.Range(0.1f, 4f), 4f);
	}

	private void OnDisable()
	{
		base.CancelInvoke("callSpawnCreatures");
		base.StopAllCoroutines();
	}

	private void Update()
	{
		if (this.currentCamGo)
		{
			return;
		}
		if (this.currentCamGo == null)
		{
			if (CoopPeerStarter.DedicatedHost)
			{
				if (Camera.main)
				{
					this.currentCamGo = GameObject.FindWithTag("MainCamera");
					this.currentCamera = this.currentCamGo.GetComponent<Camera>();
				}
			}
			else
			{
				this.currentCamGo = LocalPlayer.GameObject;
				this.currentCamera = LocalPlayer.MainCam;
			}
		}
	}

	private void callSpawnCreatures()
	{
		if (!Clock.planecrash && this.currentCamera)
		{
			if (!BoltNetwork.isRunning)
			{
				if (this.lizardAmount > 0)
				{
					base.StartCoroutine("spawnLizard");
				}
				if (this.rabbitAmount > 0)
				{
					base.StartCoroutine("spawnRabbit");
				}
				if (this.tortoiseAmount > 0)
				{
					base.StartCoroutine("spawnTortoise");
				}
				if (this.raccoonAmount > 0)
				{
					base.StartCoroutine("spawnRaccoon");
				}
				if (this.deerAmount > 0)
				{
					base.StartCoroutine("spawnDeer");
				}
			}
			if (this.fishAmount > 0)
			{
				base.StartCoroutine("spawnFish");
			}
			if (this.turtleAmount > 0)
			{
				base.StartCoroutine("spawnTurtles");
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator spawnLizard()
	{
		animalController.<spawnLizard>c__Iterator40 <spawnLizard>c__Iterator = new animalController.<spawnLizard>c__Iterator40();
		<spawnLizard>c__Iterator.<>f__this = this;
		return <spawnLizard>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator spawnRabbit()
	{
		animalController.<spawnRabbit>c__Iterator41 <spawnRabbit>c__Iterator = new animalController.<spawnRabbit>c__Iterator41();
		<spawnRabbit>c__Iterator.<>f__this = this;
		return <spawnRabbit>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator spawnTortoise()
	{
		animalController.<spawnTortoise>c__Iterator42 <spawnTortoise>c__Iterator = new animalController.<spawnTortoise>c__Iterator42();
		<spawnTortoise>c__Iterator.<>f__this = this;
		return <spawnTortoise>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator spawnRaccoon()
	{
		animalController.<spawnRaccoon>c__Iterator43 <spawnRaccoon>c__Iterator = new animalController.<spawnRaccoon>c__Iterator43();
		<spawnRaccoon>c__Iterator.<>f__this = this;
		return <spawnRaccoon>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator spawnDeer()
	{
		animalController.<spawnDeer>c__Iterator44 <spawnDeer>c__Iterator = new animalController.<spawnDeer>c__Iterator44();
		<spawnDeer>c__Iterator.<>f__this = this;
		return <spawnDeer>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator spawnTurtles()
	{
		animalController.<spawnTurtles>c__Iterator45 <spawnTurtles>c__Iterator = new animalController.<spawnTurtles>c__Iterator45();
		<spawnTurtles>c__Iterator.<>f__this = this;
		return <spawnTurtles>c__Iterator;
	}

	private void AttachAnimalToNetwork(GameObject gameObject)
	{
		if (BoltNetwork.isServer)
		{
			BoltEntity boltEntity = gameObject.AddComponent<BoltEntity>();
			BoltEntity component = gameObject.GetComponent<CoopAnimalServer>().NetworkContainerPrefab.GetComponent<BoltEntity>();
			using (BoltEntitySettingsModifier boltEntitySettingsModifier = component.ModifySettings())
			{
				using (BoltEntitySettingsModifier boltEntitySettingsModifier2 = boltEntity.ModifySettings())
				{
					boltEntitySettingsModifier2.clientPredicted = boltEntitySettingsModifier.clientPredicted;
					boltEntitySettingsModifier2.persistThroughSceneLoads = boltEntitySettingsModifier.persistThroughSceneLoads;
					boltEntitySettingsModifier2.allowInstantiateOnClient = boltEntitySettingsModifier.allowInstantiateOnClient;
					boltEntitySettingsModifier2.prefabId = boltEntitySettingsModifier.prefabId;
					boltEntitySettingsModifier2.updateRate = boltEntitySettingsModifier.updateRate;
					boltEntitySettingsModifier2.serializerId = boltEntitySettingsModifier.serializerId;
				}
			}
			BoltNetwork.Attach(gameObject);
		}
	}

	[DebuggerHidden]
	private IEnumerator spawnFish()
	{
		animalController.<spawnFish>c__Iterator46 <spawnFish>c__Iterator = new animalController.<spawnFish>c__Iterator46();
		<spawnFish>c__Iterator.<>f__this = this;
		return <spawnFish>c__Iterator;
	}

	private bool pointOffCamera(Vector3 pos)
	{
		this.screenPos = this.currentCamera.WorldToViewportPoint(pos);
		return this.screenPos.x < 0f || this.screenPos.x > 1f || this.screenPos.y < 0f || this.screenPos.y > 1f;
	}

	private Vector2 RandomSpawnPos(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	public void seCurrTurtleAmount(int amount)
	{
		this.currTurtleAmount += amount;
	}

	private Transform getClosestPlayer(bool cave)
	{
		this.closestDist = float.PositiveInfinity;
		List<GameObject> list = new List<GameObject>();
		if (cave)
		{
			list = new List<GameObject>(this.sceneInfo.allPlayersInCave);
		}
		else
		{
			list = new List<GameObject>(this.sceneInfo.allPlayers);
		}
		if (list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i])
				{
					float sqrMagnitude = (base.transform.position - list[i].transform.position).sqrMagnitude;
					if (sqrMagnitude < this.closestDist)
					{
						this.closestDist = sqrMagnitude;
						this.closestPlayer = list[i].transform;
					}
				}
			}
		}
		return this.closestPlayer;
	}

	public void addToSpawnDelay(float val)
	{
		this.addSpawnDelay += val;
		this.setSpawnDelay = this.addSpawnDelay + Time.time;
	}
}
