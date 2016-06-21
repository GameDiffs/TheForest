using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;

public class animalSearchFunctions : MonoBehaviour
{
	public GameObject currentWaypoint;

	public GameObject nearestTree;

	public GameObject activeTreeGo;

	public GameObject currentMarker;

	public Transform currentDrinkMarker;

	private Transform tr;

	private float attachDist;

	private float treeGirth;

	public bool onTree;

	private PlayMakerFSM pmBase;

	private animalAI ai;

	public animalAI closeAnimalAi;

	private sceneTracker info;

	private animalSpawnFunctions spawn;

	private List<GameObject> tempList;

	private RaycastHit hit;

	private int layer;

	private int layerMask;

	private bool search;

	private bool followCoolDown;

	private FsmFloat fsmObjectAngle;

	private FsmGameObject fsmTarget;

	private FsmGameObject fsmWaypointGO;

	public GameObject closeAnimalGo;

	private FsmFloat fsmPlayerDist;

	private void Awake()
	{
		this.tr = base.transform;
		this.info = Scene.SceneTracker;
		this.ai = base.gameObject.GetComponent<animalAI>();
		this.spawn = base.GetComponent<animalSpawnFunctions>();
	}

	private void OnEnable()
	{
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
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array2 = componentsInChildren;
		for (int j = 0; j < array2.Length; j++)
		{
			Transform transform = array2[j];
			if (transform.name == "currentWaypoint")
			{
				this.currentWaypoint = transform.gameObject;
			}
		}
		this.layer = 26;
		this.layerMask = 1 << this.layer;
		this.fsmTarget = this.pmBase.FsmVariables.GetFsmGameObject("targetGO");
		this.fsmObjectAngle = this.pmBase.FsmVariables.GetFsmFloat("objectAngle");
		this.fsmWaypointGO = this.pmBase.FsmVariables.GetFsmGameObject("waypointGO");
		this.fsmWaypointGO.Value = this.currentWaypoint.gameObject;
		this.currentWaypoint.transform.parent = null;
		this.pmBase.FsmVariables.GetFsmBool("drinkCheck").Value = false;
	}

	private void Update()
	{
		if (this.ai.isFollowing && this.closeAnimalGo && this.closeAnimalGo.activeSelf)
		{
			Vector3 vector;
			base.transform.position.y = vector.y + 2f;
			Vector3 vector2;
			this.closeAnimalGo.transform.position.y = vector2.y + 2f;
		}
	}

	[DebuggerHidden]
	public IEnumerator findCloseWater()
	{
		animalSearchFunctions.<findCloseWater>c__Iterator47 <findCloseWater>c__Iterator = new animalSearchFunctions.<findCloseWater>c__Iterator47();
		<findCloseWater>c__Iterator.<>f__this = this;
		return <findCloseWater>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findCloseBush()
	{
		animalSearchFunctions.<findCloseBush>c__Iterator48 <findCloseBush>c__Iterator = new animalSearchFunctions.<findCloseBush>c__Iterator48();
		<findCloseBush>c__Iterator.<>f__this = this;
		return <findCloseBush>c__Iterator;
	}

	public void findCloseAnimal()
	{
		GameObject gameObject = null;
		if (this.spawn.raccoon)
		{
			this.info.allRaccoons.RemoveAll((GameObject o) => o == null);
			for (int i = 0; i < this.info.allRaccoons.Count; i++)
			{
				float magnitude = (this.tr.position - this.info.allRaccoons[i].transform.position).magnitude;
				if (magnitude < 15f && this.info.allRaccoons[i].name != base.gameObject.name)
				{
					gameObject = this.info.allRaccoons[i];
				}
			}
		}
		if (this.spawn.deer)
		{
			this.info.allDeer.RemoveAll((GameObject o) => o == null);
			for (int j = 0; j < this.info.allDeer.Count; j++)
			{
				float magnitude2 = (this.tr.position - this.info.allDeer[j].transform.position).magnitude;
				if (magnitude2 < 15f && this.info.allDeer[j].name != base.gameObject.name)
				{
					gameObject = this.info.allDeer[j];
				}
			}
		}
		if (this.spawn.squirrel)
		{
			this.info.allSquirrel.RemoveAll((GameObject o) => o == null);
			for (int k = 0; k < this.info.allSquirrel.Count; k++)
			{
				float magnitude3 = (this.tr.position - this.info.allSquirrel[k].transform.position).magnitude;
				if (magnitude3 < 15f && this.info.allSquirrel[k].name != base.gameObject.name)
				{
					gameObject = this.info.allSquirrel[k];
				}
			}
		}
		if (this.spawn.boar)
		{
			this.info.allBoar.RemoveAll((GameObject o) => o == null);
			for (int l = 0; l < this.info.allBoar.Count; l++)
			{
				float magnitude4 = (this.tr.position - this.info.allBoar[l].transform.position).magnitude;
				if (magnitude4 < 15f && this.info.allBoar[l].name != base.gameObject.name)
				{
					gameObject = this.info.allBoar[l];
				}
			}
		}
		if (gameObject != null)
		{
			this.closeAnimalAi = gameObject.GetComponent<animalAI>();
			if (!this.closeAnimalAi.isFollowing && !this.closeAnimalAi.inTree)
			{
				this.pmBase.FsmVariables.GetFsmGameObject("closeAnimalGo").Value = gameObject;
				this.pmBase.SendEvent("doAction");
				this.pmBase.FsmVariables.GetFsmBool("followingBool").Value = true;
				this.ai.isFollowing = true;
				this.closeAnimalGo = gameObject;
			}
			else
			{
				this.pmBase.SendEvent("noValidTarget");
				this.pmBase.FsmVariables.GetFsmBool("followingBool").Value = false;
			}
		}
		else
		{
			this.pmBase.SendEvent("noValidTarget");
			this.pmBase.FsmVariables.GetFsmBool("followingBool").Value = false;
		}
	}

	public void findPositionNearAnimal()
	{
		GameObject gameObject = null;
		if (this.spawn.raccoon)
		{
			this.info.allRaccoons.RemoveAll((GameObject o) => o == null);
			for (int i = 0; i < this.info.allRaccoons.Count; i++)
			{
				float magnitude = (this.tr.position - this.info.allRaccoons[i].transform.position).magnitude;
				if (magnitude < 15f && this.info.allRaccoons[i].name != base.gameObject.name)
				{
					gameObject = this.info.allRaccoons[i];
				}
			}
		}
		if (this.spawn.deer)
		{
			this.info.allDeer.RemoveAll((GameObject o) => o == null);
			for (int j = 0; j < this.info.allDeer.Count; j++)
			{
				float magnitude2 = (this.tr.position - this.info.allDeer[j].transform.position).magnitude;
				if (magnitude2 < 15f && this.info.allDeer[j].name != base.gameObject.name)
				{
					gameObject = this.info.allDeer[j];
				}
			}
		}
		if (this.spawn.squirrel)
		{
			this.info.allSquirrel.RemoveAll((GameObject o) => o == null);
			for (int k = 0; k < this.info.allSquirrel.Count; k++)
			{
				float magnitude3 = (this.tr.position - this.info.allSquirrel[k].transform.position).magnitude;
				if (magnitude3 < 15f && this.info.allSquirrel[k].name != base.gameObject.name)
				{
					gameObject = this.info.allSquirrel[k];
				}
			}
		}
		if (this.spawn.boar)
		{
			this.info.allBoar.RemoveAll((GameObject o) => o == null);
			for (int l = 0; l < this.info.allBoar.Count; l++)
			{
				float magnitude4 = (this.tr.position - this.info.allBoar[l].transform.position).magnitude;
				if (magnitude4 < 15f && this.info.allBoar[l].name != base.gameObject.name)
				{
					gameObject = this.info.allBoar[l];
				}
			}
		}
		if (gameObject != null)
		{
			this.closeAnimalAi = gameObject.GetComponent<animalAI>();
			if (!this.closeAnimalAi.inTree)
			{
				Vector3 vector = (this.tr.position - gameObject.transform.position).normalized;
				vector = vector * 8f + gameObject.transform.position;
				float y = Terrain.activeTerrain.SampleHeight(vector) + Terrain.activeTerrain.transform.position.y;
				vector.y = y;
				UnityEngine.Debug.DrawRay(vector, Vector3.up, Color.green);
				this.updateCurrentWaypoint(vector);
				this.setToWaypoint();
				this.pmBase.SendEvent("doAction");
				return;
			}
			this.pmBase.SendEvent("noValidTarget");
		}
		this.pmBase.SendEvent("noValidTarget");
	}

	public void findCloseTrap()
	{
		GameObject gameObject = null;
		this.info.allRabbitTraps.RemoveAll((GameObject o) => o == null);
		for (int i = 0; i < this.info.allRabbitTraps.Count; i++)
		{
			float magnitude = (this.tr.position - this.info.allRabbitTraps[i].transform.position).magnitude;
			if (magnitude < 80f && magnitude > 12f && !this.info.allRabbitTraps[i].CompareTag("trapSprung"))
			{
				gameObject = this.info.allRabbitTraps[i];
			}
		}
		if (gameObject != null)
		{
			this.pmBase.FsmVariables.GetFsmGameObject("targetObjectGO").Value = gameObject;
			this.pmBase.SendEvent("doAction");
			this.updateCurrentWaypoint(gameObject.transform.position);
			this.setToWaypoint();
		}
		else
		{
			this.pmBase.SendEvent("noValidTarget");
		}
	}

	[DebuggerHidden]
	private IEnumerator findCloseTree(float dist)
	{
		animalSearchFunctions.<findCloseTree>c__Iterator49 <findCloseTree>c__Iterator = new animalSearchFunctions.<findCloseTree>c__Iterator49();
		<findCloseTree>c__Iterator.dist = dist;
		<findCloseTree>c__Iterator.<$>dist = dist;
		<findCloseTree>c__Iterator.<>f__this = this;
		return <findCloseTree>c__Iterator;
	}

	private void castPointAroundPlayer()
	{
		Vector2 vector = this.Circle2(UnityEngine.Random.Range(50f, 100f));
		Vector3 origin = new Vector3(this.info.allPlayers[0].transform.position.x + vector.x, this.info.allPlayers[0].transform.position.y + 50f, this.info.allPlayers[0].transform.position.z + vector.y);
		int num = 26;
		int num2 = 1 << num;
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, Vector3.down, out raycastHit, 200f, num2))
		{
			if (raycastHit.collider.CompareTag("TerrainMain"))
			{
				this.updateCurrentWaypoint(raycastHit.point);
			}
			else
			{
				this.pmBase.SendEvent("noValidTarget");
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator findClosestBeachMarker()
	{
		animalSearchFunctions.<findClosestBeachMarker>c__Iterator4A <findClosestBeachMarker>c__Iterator4A = new animalSearchFunctions.<findClosestBeachMarker>c__Iterator4A();
		<findClosestBeachMarker>c__Iterator4A.<>f__this = this;
		return <findClosestBeachMarker>c__Iterator4A;
	}

	[DebuggerHidden]
	private IEnumerator findClosestSwimMarker()
	{
		animalSearchFunctions.<findClosestSwimMarker>c__Iterator4B <findClosestSwimMarker>c__Iterator4B = new animalSearchFunctions.<findClosestSwimMarker>c__Iterator4B();
		<findClosestSwimMarker>c__Iterator4B.<>f__this = this;
		return <findClosestSwimMarker>c__Iterator4B;
	}

	[DebuggerHidden]
	private IEnumerator findRandomSwimMarker()
	{
		animalSearchFunctions.<findRandomSwimMarker>c__Iterator4C <findRandomSwimMarker>c__Iterator4C = new animalSearchFunctions.<findRandomSwimMarker>c__Iterator4C();
		<findRandomSwimMarker>c__Iterator4C.<>f__this = this;
		return <findRandomSwimMarker>c__Iterator4C;
	}

	[DebuggerHidden]
	private IEnumerator findRandomPoint(float dist)
	{
		animalSearchFunctions.<findRandomPoint>c__Iterator4D <findRandomPoint>c__Iterator4D = new animalSearchFunctions.<findRandomPoint>c__Iterator4D();
		<findRandomPoint>c__Iterator4D.dist = dist;
		<findRandomPoint>c__Iterator4D.<$>dist = dist;
		<findRandomPoint>c__Iterator4D.<>f__this = this;
		return <findRandomPoint>c__Iterator4D;
	}

	[DebuggerHidden]
	private IEnumerator findPointAwayFromPlayer(float dist)
	{
		animalSearchFunctions.<findPointAwayFromPlayer>c__Iterator4E <findPointAwayFromPlayer>c__Iterator4E = new animalSearchFunctions.<findPointAwayFromPlayer>c__Iterator4E();
		<findPointAwayFromPlayer>c__Iterator4E.dist = dist;
		<findPointAwayFromPlayer>c__Iterator4E.<$>dist = dist;
		<findPointAwayFromPlayer>c__Iterator4E.<>f__this = this;
		return <findPointAwayFromPlayer>c__Iterator4E;
	}

	private void updateCurrentTarget()
	{
		GameObject value = this.pmBase.FsmVariables.GetFsmGameObject("targetObjectGO").Value;
		this.ai.target = value.transform;
		this.fsmTarget.Value = value;
		this.fsmWaypointGO.Value.transform.position = value.transform.position;
	}

	private void updateCurrentColliderTarget()
	{
		GameObject value = this.pmBase.FsmVariables.GetFsmGameObject("targetObjectGO").Value;
		this.ai.target = value.transform;
		this.fsmTarget.Value = value;
		this.fsmWaypointGO.Value.transform.position = new Vector3(value.GetComponent<Collider>().bounds.center.x, value.transform.position.y, value.GetComponent<Collider>().bounds.center.z);
	}

	private void updateCurrentWaypoint(Vector3 vect)
	{
		this.currentWaypoint.transform.position = vect;
		this.ai.target = this.currentWaypoint.transform;
		this.fsmTarget.Value = this.currentWaypoint.gameObject;
	}

	public Vector2 Circle2(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	private void setToWaypoint()
	{
		this.ai.target = this.currentWaypoint.transform;
		this.fsmTarget.Value = this.currentWaypoint.gameObject;
		this.ai.SearchPath();
	}

	private void setToCurrentWater()
	{
		this.currentWaypoint.transform.position = this.currentDrinkMarker.position;
		this.ai.target = this.currentWaypoint.transform;
		this.fsmTarget.Value = this.currentWaypoint.gameObject;
		this.ai.SearchPath();
	}

	private void setToPlayer()
	{
		if (this.info.allPlayers.Count > 0)
		{
			if (this.info.allPlayers[0] == null)
			{
				return;
			}
			this.ai.target = this.info.allPlayers[0].transform;
			this.fsmTarget.Value = this.info.allPlayers[0].gameObject;
			if (this.ai.swimming)
			{
				this.ai.StartCoroutine("enableForceTarget", this.info.allPlayers[0].gameObject);
			}
		}
	}

	private void setToCloseAnimal()
	{
		if (!this.followCoolDown)
		{
			this.followCoolDown = true;
			base.Invoke("setFollowTimeout", (float)UnityEngine.Random.Range(30, 60));
		}
		this.ai.target = this.closeAnimalGo.transform;
		this.fsmTarget.Value = this.closeAnimalGo;
	}

	private void setFollowTimeout()
	{
		this.pmBase.FsmVariables.GetFsmBool("followingBool").Value = false;
		this.ai.isFollowing = false;
		this.followCoolDown = false;
	}

	[DebuggerHidden]
	private IEnumerator findAngleToTarget(Vector3 target)
	{
		animalSearchFunctions.<findAngleToTarget>c__Iterator4F <findAngleToTarget>c__Iterator4F = new animalSearchFunctions.<findAngleToTarget>c__Iterator4F();
		<findAngleToTarget>c__Iterator4F.target = target;
		<findAngleToTarget>c__Iterator4F.<$>target = target;
		<findAngleToTarget>c__Iterator4F.<>f__this = this;
		return <findAngleToTarget>c__Iterator4F;
	}

	[DebuggerHidden]
	private IEnumerator setTreeAttachDist(float dist)
	{
		animalSearchFunctions.<setTreeAttachDist>c__Iterator50 <setTreeAttachDist>c__Iterator = new animalSearchFunctions.<setTreeAttachDist>c__Iterator50();
		<setTreeAttachDist>c__Iterator.dist = dist;
		<setTreeAttachDist>c__Iterator.<$>dist = dist;
		<setTreeAttachDist>c__Iterator.<>f__this = this;
		return <setTreeAttachDist>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findTreeAttachPos(Vector3 pos)
	{
		animalSearchFunctions.<findTreeAttachPos>c__Iterator51 <findTreeAttachPos>c__Iterator = new animalSearchFunctions.<findTreeAttachPos>c__Iterator51();
		<findTreeAttachPos>c__Iterator.pos = pos;
		<findTreeAttachPos>c__Iterator.<$>pos = pos;
		<findTreeAttachPos>c__Iterator.<>f__this = this;
		return <findTreeAttachPos>c__Iterator;
	}

	private void checkActiveAnimal()
	{
		if (this.closeAnimalAi && this.closeAnimalAi.inTree)
		{
			this.pmBase.SendEvent("toStopFollowing");
		}
	}

	private void setDrinkCoolDown()
	{
		this.pmBase.FsmVariables.GetFsmBool("drinkCheck").Value = true;
		base.Invoke("resetDrinkCoolDown", 120f);
	}

	private void resetDrinkCoolDown()
	{
		if (base.enabled)
		{
			this.pmBase.FsmVariables.GetFsmBool("drinkCheck").Value = false;
		}
	}

	public void enableForceDirectionToTarget(Transform target)
	{
	}

	public void setupTreeListener()
	{
		this.onTree = true;
		TreeHealth.OnTreeCutDown.AddListener(new UnityAction<Vector3>(this.OnCutDownTree));
	}

	private void OnDisable()
	{
		if (this.onTree)
		{
			TreeHealth.OnTreeCutDown.RemoveListener(new UnityAction<Vector3>(this.OnCutDownTree));
			this.onTree = false;
		}
	}

	private void OnDestroy()
	{
		if (this.onTree)
		{
			TreeHealth.OnTreeCutDown.RemoveListener(new UnityAction<Vector3>(this.OnCutDownTree));
			this.onTree = false;
		}
	}

	public void OnCutDownTree(Vector3 treePos)
	{
		Vector3 position = base.transform.position;
		position.y = 0f;
		treePos.y = 0f;
		if (Vector3.Distance(position, treePos) < 5f && this.onTree)
		{
			this.ai.goRagdoll();
		}
	}
}
