using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantSearchFunctions : MonoBehaviour
{
	public targetStats targetSetup;

	private GameObject player;

	private GameObject closePlayerTarget;

	public GameObject currentWaypoint;

	public GameObject currentTarget;

	public GameObject lastTarget;

	public GameObject lastSighting;

	public GameObject nearestTree;

	private Transform tr;

	private Transform rootTr;

	public GameObject testSphere;

	private Transform lookatTr;

	private Transform tempTarget;

	private sceneTracker sceneInfo;

	private mutantFamilyFunctions familyFunctions;

	private mutantCollisionDetect collideDetect;

	private jumpObject currentJumpObj;

	private GameObject currentWaypointGo;

	private List<Transform> tempList;

	private GameObject newTarget;

	private wayPointSetup wpSetup;

	private float tempTargetDist;

	private float currentTargetDist;

	private float currTargetAngle;

	private float attachDist;

	public bool playerAware;

	private bool search;

	private int count;

	private int searchCount;

	private Vector2 randomPoint;

	private Vector3 pos;

	private RaycastHit hit;

	private Vector3 newWaypoint;

	private int layerMask;

	private int visLayerMask;

	private bool targetFound;

	private Vector3 castPos;

	private bool enableSearch;

	private bool screamCooldown;

	private mutantAI ai;

	private Animator animator;

	private mutantScriptSetup setup;

	public float cautiousReset;

	public float searchReset;

	public float playerAwareReset;

	public float closeVisRange;

	public float longVisRange;

	public float setCloseVisRange;

	public float setLongVisRange;

	public float modifiedVisRange;

	public float modLighterRange;

	public float modMudRange;

	public float modEncounterRange;

	public float modCrouchRange;

	public float modBushRange;

	private float lighterRange;

	private FsmGameObject fsmVisionTarget;

	private FsmFloat fsmObjectAngle;

	private FsmGameObject fsmCaveEntrance;

	private FsmGameObject fsmLeaderGo;

	private FsmBool fsmplayerAwareBool;

	private FsmGameObject fsmHitGo;

	private FsmFloat fsmVisionRangeLong;

	private FsmFloat fsmVisionRangeClose;

	private FsmGameObject fsmCurrentTargetGo;

	private FsmVector3 fsmTargetDir;

	public FsmBool fsmInCave;

	private FsmFloat fsmSearchClosestPlayerDist;

	private FsmFloat fsmCombatClosestPlayerDist;

	private FsmFloat fsmTargetStopDist;

	private FsmFloat fsmClosestPlayerAngle;

	public bool lookingForTarget;

	private int targetCounter;

	private int trackCounter;

	public int visRetry = 6;

	public GameObject hitGo;

	public bool startedLook;

	public bool startedTrack;

	private void OnDeserialized()
	{
		base.StopAllCoroutines();
	}

	private void Awake()
	{
		this.tr = base.transform;
		this.rootTr = base.transform.parent;
		this.player = LocalPlayer.GameObject;
		this.ai = base.GetComponent<mutantAI>();
		this.sceneInfo = Scene.SceneTracker;
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.animator = base.GetComponent<Animator>();
		this.collideDetect = base.transform.root.GetComponentInChildren<mutantCollisionDetect>();
		this.currentWaypoint = this.setup.currentWaypoint;
		this.lastSighting = this.setup.lastSighting;
		this.lookatTr = this.setup.lookatTr;
		this.familyFunctions = base.transform.root.GetComponent<mutantFamilyFunctions>();
	}

	private void Start()
	{
		if (this.setup.pmCombat)
		{
			this.fsmClosestPlayerAngle = this.setup.pmCombat.FsmVariables.GetFsmFloat("closestPlayerAngle");
			this.fsmObjectAngle = this.setup.pmCombat.FsmVariables.GetFsmFloat("objectAngle");
			this.fsmCurrentTargetGo = this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentTargetGo");
			this.fsmCombatClosestPlayerDist = this.setup.pmCombat.FsmVariables.GetFsmFloat("closestPlayerDist");
		}
		if (this.setup.pmSearch)
		{
			this.fsmSearchClosestPlayerDist = this.setup.pmSearch.FsmVariables.GetFsmFloat("closestPlayerDist");
			this.fsmTargetStopDist = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmTargetStopDist");
		}
		if (this.setup.pmSleep)
		{
			this.fsmCaveEntrance = this.setup.pmSleep.FsmVariables.GetFsmGameObject("closestEntranceGo");
			this.fsmInCave = this.setup.pmSleep.FsmVariables.GetFsmBool("inCaveBool");
		}
		if (this.setup.pmBrain)
		{
			this.fsmLeaderGo = this.setup.pmBrain.FsmVariables.GetFsmGameObject("leaderGo");
		}
		if (this.ai.creepy_baby)
		{
			this.fsmLeaderGo = this.setup.pmCombat.FsmVariables.GetFsmGameObject("leaderGo");
		}
		if (this.setup.pmVision)
		{
			this.fsmplayerAwareBool = this.setup.pmVision.FsmVariables.GetFsmBool("playerAwareBool");
			this.fsmHitGo = this.setup.pmVision.FsmVariables.GetFsmGameObject("hitGO");
			this.fsmVisionTarget = this.setup.pmVision.FsmVariables.GetFsmGameObject("currentTarget");
			this.fsmVisionRangeLong = this.setup.pmVision.FsmVariables.GetFsmFloat("visionRangeLong");
			this.fsmVisionRangeClose = this.setup.pmVision.FsmVariables.GetFsmFloat("visionRangeClose");
			this.fsmTargetDir = this.setup.pmVision.FsmVariables.GetFsmVector3("targetDir");
		}
		if (this.ai.creepy || this.ai.creepy_baby || this.ai.creepy_male || this.ai.creepy_fat)
		{
			this.fsmInCave = this.setup.pmCombat.FsmVariables.GetFsmBool("inCaveBool");
		}
		this.currentWaypoint.transform.parent = null;
		this.lastSighting.transform.parent = null;
		this.currentTarget = this.player;
		this.lastTarget = this.player;
		this.layerMask = 67239936;
		this.visLayerMask = 102115328;
	}

	private void OnEnable()
	{
		if (!base.IsInvoking("updateSearchParams"))
		{
			base.InvokeRepeating("updateSearchParams", 1f, 1f);
		}
		if (!base.IsInvoking("getClosestPlayerDist"))
		{
			base.InvokeRepeating("getClosestPlayerDist", 1f, 1f);
		}
		this.setModEncounterRange(0f);
	}

	private void OnDisable()
	{
		this.screamCooldown = false;
		this.playerAware = false;
		if (LocalPlayer.ScriptSetup && LocalPlayer.ScriptSetup.targetFunctions.visibleEnemies.Contains(base.transform.parent.gameObject))
		{
			LocalPlayer.ScriptSetup.targetFunctions.visibleEnemies.Remove(base.transform.parent.gameObject);
		}
		base.CancelInvoke("updateSearchParams");
		base.CancelInvoke("getClosestPlayerDist");
		base.StopCoroutine("toLook");
		base.StopCoroutine("toTrack");
		this.trackCounter = 0;
		this.startedTrack = false;
		this.startedLook = false;
		if (Scene.SceneTracker)
		{
			Scene.SceneTracker.closeStructureTarget = null;
		}
	}

	public void setupCrouchedVisLayerMask()
	{
		this.visLayerMask = 102111232;
	}

	public void setupStandingVisLayerMask()
	{
		this.visLayerMask = 102107136;
	}

	public void setModEncounterRange(float amount)
	{
		this.modEncounterRange = amount;
	}

	private void updateSearchParams()
	{
		if (Clock.Dark || this.setup.typeSetup.inCave)
		{
			this.setCloseVisRange = this.closeVisRange;
			this.setLongVisRange = this.longVisRange;
			if (this.setCloseVisRange < 8f)
			{
				this.setCloseVisRange = 8f;
			}
			if (this.setLongVisRange < 25f)
			{
				this.setLongVisRange = 25f;
			}
			if (this.setup.pmVision)
			{
				this.fsmVisionRangeClose.Value = this.setCloseVisRange;
			}
			if (this.setup.pmVision)
			{
				this.fsmVisionRangeLong.Value = this.setLongVisRange;
			}
		}
		else
		{
			this.setCloseVisRange = this.closeVisRange;
			this.setLongVisRange = this.longVisRange;
			if (this.setCloseVisRange < 12f)
			{
				this.setCloseVisRange = 12f;
			}
			if (this.setLongVisRange < 30f)
			{
				this.setLongVisRange = 30f;
			}
			if (this.setup.pmVision)
			{
				this.fsmVisionRangeClose.Value = this.setCloseVisRange;
			}
			if (this.setup.pmVision)
			{
				this.fsmVisionRangeLong.Value = this.setLongVisRange;
			}
		}
		if (!this.ai.creepy && !this.ai.creepy_baby && !this.ai.creepy_male && !this.ai.creepy_fat)
		{
			if (this.fsmInCave.Value)
			{
				this.fsmTargetStopDist.Value = 6f;
			}
			else
			{
				this.fsmTargetStopDist.Value = 15f;
			}
		}
	}

	private void Update()
	{
		if (this.currentTarget)
		{
			this.currentTargetDist = Vector3.Distance(this.tr.position, this.currentTarget.transform.position);
		}
		if (!this.currentTarget)
		{
			this.currentTarget = this.currentWaypoint;
		}
	}

	private void setCeilingPos()
	{
		this.layerMask = 131072;
		this.pos = this.tr.position;
		if (Physics.Raycast(this.pos, Vector3.up, out this.hit, 90f, this.layerMask))
		{
			this.rootTr.position = this.hit.point;
			this.rootTr.position = new Vector3(this.rootTr.position.x, this.rootTr.position.y - 1.2f, this.rootTr.position.z);
			this.setup.pmSleep.FsmVariables.GetFsmVector3("sleepPos").Value = this.rootTr.position;
			this.setup.pmSleep.FsmVariables.GetFsmVector3("sleepAngle").Value = this.hit.normal * -1f;
		}
	}

	public void findCloseTrap()
	{
		GameObject gameObject = null;
		foreach (GameObject current in this.sceneInfo.allRabbitTraps)
		{
			float magnitude = (this.tr.position - current.transform.position).magnitude;
			if (magnitude < 80f && magnitude > 8f && !current.CompareTag("trapSprung"))
			{
				gameObject = current;
			}
		}
		if (gameObject != null)
		{
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.FsmVariables.GetFsmGameObject("targetObjectGO").Value = gameObject;
				this.setup.pmCombat.SendEvent("doAction");
			}
			this.updateCurrentWaypoint(gameObject.transform.position);
			this.setToWaypoint();
		}
		else if (this.setup.pmCombat)
		{
			this.setup.pmCombat.SendEvent("noValidTarget");
		}
	}

	public void findCloseTrapTrigger()
	{
		GameObject gameObject = null;
		if (Scene.SceneTracker.allTrapTriggers.Count == 0 && this.setup.pmCombat)
		{
			this.setup.pmCombat.SendEvent("noValidTarget");
		}
		foreach (GameObject current in this.sceneInfo.allTrapTriggers)
		{
			float magnitude = (this.tr.position - current.transform.position).magnitude;
			if (magnitude < 60f && magnitude > 12f)
			{
				gameObject = current;
			}
		}
		if (!(gameObject != null))
		{
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.SendEvent("noValidTarget");
			}
			return;
		}
		if (!gameObject.activeSelf)
		{
			return;
		}
		if (this.setup.pmCombat)
		{
			this.setup.pmCombat.FsmVariables.GetFsmGameObject("structureGo").Value = gameObject;
			this.setup.pmCombat.SendEvent("doAction");
		}
		this.updateCurrentWaypoint(gameObject.transform.position);
		this.setToWaypoint();
	}

	[DebuggerHidden]
	private IEnumerator castPointAroundPlayer(float dist)
	{
		mutantSearchFunctions.<castPointAroundPlayer>c__Iterator82 <castPointAroundPlayer>c__Iterator = new mutantSearchFunctions.<castPointAroundPlayer>c__Iterator82();
		<castPointAroundPlayer>c__Iterator.dist = dist;
		<castPointAroundPlayer>c__Iterator.<$>dist = dist;
		<castPointAroundPlayer>c__Iterator.<>f__this = this;
		return <castPointAroundPlayer>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator castPointAroundSpawn(float dist)
	{
		mutantSearchFunctions.<castPointAroundSpawn>c__Iterator83 <castPointAroundSpawn>c__Iterator = new mutantSearchFunctions.<castPointAroundSpawn>c__Iterator83();
		<castPointAroundSpawn>c__Iterator.dist = dist;
		<castPointAroundSpawn>c__Iterator.<$>dist = dist;
		<castPointAroundSpawn>c__Iterator.<>f__this = this;
		return <castPointAroundSpawn>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator castPointAroundLastSighting(float dist)
	{
		mutantSearchFunctions.<castPointAroundLastSighting>c__Iterator84 <castPointAroundLastSighting>c__Iterator = new mutantSearchFunctions.<castPointAroundLastSighting>c__Iterator84();
		<castPointAroundLastSighting>c__Iterator.dist = dist;
		<castPointAroundLastSighting>c__Iterator.<$>dist = dist;
		<castPointAroundLastSighting>c__Iterator.<>f__this = this;
		return <castPointAroundLastSighting>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findRandomPoint(float dist)
	{
		mutantSearchFunctions.<findRandomPoint>c__Iterator85 <findRandomPoint>c__Iterator = new mutantSearchFunctions.<findRandomPoint>c__Iterator85();
		<findRandomPoint>c__Iterator.dist = dist;
		<findRandomPoint>c__Iterator.<$>dist = dist;
		<findRandomPoint>c__Iterator.<>f__this = this;
		return <findRandomPoint>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findCloseWaypoint()
	{
		mutantSearchFunctions.<findCloseWaypoint>c__Iterator86 <findCloseWaypoint>c__Iterator = new mutantSearchFunctions.<findCloseWaypoint>c__Iterator86();
		<findCloseWaypoint>c__Iterator.<>f__this = this;
		return <findCloseWaypoint>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findRandomWaypoint()
	{
		mutantSearchFunctions.<findRandomWaypoint>c__Iterator87 <findRandomWaypoint>c__Iterator = new mutantSearchFunctions.<findRandomWaypoint>c__Iterator87();
		<findRandomWaypoint>c__Iterator.<>f__this = this;
		return <findRandomWaypoint>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findRandomStructure(float dist)
	{
		mutantSearchFunctions.<findRandomStructure>c__Iterator88 <findRandomStructure>c__Iterator = new mutantSearchFunctions.<findRandomStructure>c__Iterator88();
		<findRandomStructure>c__Iterator.dist = dist;
		<findRandomStructure>c__Iterator.<$>dist = dist;
		<findRandomStructure>c__Iterator.<>f__this = this;
		return <findRandomStructure>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findCloseStructure()
	{
		mutantSearchFunctions.<findCloseStructure>c__Iterator89 <findCloseStructure>c__Iterator = new mutantSearchFunctions.<findCloseStructure>c__Iterator89();
		<findCloseStructure>c__Iterator.<>f__this = this;
		return <findCloseStructure>c__Iterator;
	}

	private void setToStructure()
	{
		GameObject value = this.setup.pmCombat.FsmVariables.GetFsmGameObject("structureGo").Value;
		if (value)
		{
			Collider component = value.GetComponent<Collider>();
			Vector3 center = component.bounds.center;
			center.y = Terrain.activeTerrain.SampleHeight(center) + Terrain.activeTerrain.transform.position.y;
			this.updateCurrentWaypoint(center);
		}
		else
		{
			this.setup.pmCombat.SendEvent("noValidTarget");
			this.setup.pmSearch.SendEvent("noValidTarget");
		}
	}

	[DebuggerHidden]
	private IEnumerator findCloseBurnStructure()
	{
		mutantSearchFunctions.<findCloseBurnStructure>c__Iterator8A <findCloseBurnStructure>c__Iterator8A = new mutantSearchFunctions.<findCloseBurnStructure>c__Iterator8A();
		<findCloseBurnStructure>c__Iterator8A.<>f__this = this;
		return <findCloseBurnStructure>c__Iterator8A;
	}

	[DebuggerHidden]
	private IEnumerator findClosePlayerFire()
	{
		mutantSearchFunctions.<findClosePlayerFire>c__Iterator8B <findClosePlayerFire>c__Iterator8B = new mutantSearchFunctions.<findClosePlayerFire>c__Iterator8B();
		<findClosePlayerFire>c__Iterator8B.<>f__this = this;
		return <findClosePlayerFire>c__Iterator8B;
	}

	[DebuggerHidden]
	private IEnumerator findPointAwayFromPlayer(float dist)
	{
		mutantSearchFunctions.<findPointAwayFromPlayer>c__Iterator8C <findPointAwayFromPlayer>c__Iterator8C = new mutantSearchFunctions.<findPointAwayFromPlayer>c__Iterator8C();
		<findPointAwayFromPlayer>c__Iterator8C.dist = dist;
		<findPointAwayFromPlayer>c__Iterator8C.<$>dist = dist;
		<findPointAwayFromPlayer>c__Iterator8C.<>f__this = this;
		return <findPointAwayFromPlayer>c__Iterator8C;
	}

	[DebuggerHidden]
	private IEnumerator findPointInFrontOfPlayer(float dist)
	{
		mutantSearchFunctions.<findPointInFrontOfPlayer>c__Iterator8D <findPointInFrontOfPlayer>c__Iterator8D = new mutantSearchFunctions.<findPointInFrontOfPlayer>c__Iterator8D();
		<findPointInFrontOfPlayer>c__Iterator8D.dist = dist;
		<findPointInFrontOfPlayer>c__Iterator8D.<$>dist = dist;
		<findPointInFrontOfPlayer>c__Iterator8D.<>f__this = this;
		return <findPointInFrontOfPlayer>c__Iterator8D;
	}

	[DebuggerHidden]
	private IEnumerator findPointAwayFromExplosion(float dist)
	{
		mutantSearchFunctions.<findPointAwayFromExplosion>c__Iterator8E <findPointAwayFromExplosion>c__Iterator8E = new mutantSearchFunctions.<findPointAwayFromExplosion>c__Iterator8E();
		<findPointAwayFromExplosion>c__Iterator8E.dist = dist;
		<findPointAwayFromExplosion>c__Iterator8E.<$>dist = dist;
		<findPointAwayFromExplosion>c__Iterator8E.<>f__this = this;
		return <findPointAwayFromExplosion>c__Iterator8E;
	}

	[DebuggerHidden]
	private IEnumerator findAmbushPoint(float dist)
	{
		mutantSearchFunctions.<findAmbushPoint>c__Iterator8F <findAmbushPoint>c__Iterator8F = new mutantSearchFunctions.<findAmbushPoint>c__Iterator8F();
		<findAmbushPoint>c__Iterator8F.<>f__this = this;
		return <findAmbushPoint>c__Iterator8F;
	}

	[DebuggerHidden]
	private IEnumerator findFlankPointToPlayer(float dist)
	{
		mutantSearchFunctions.<findFlankPointToPlayer>c__Iterator90 <findFlankPointToPlayer>c__Iterator = new mutantSearchFunctions.<findFlankPointToPlayer>c__Iterator90();
		<findFlankPointToPlayer>c__Iterator.<>f__this = this;
		return <findFlankPointToPlayer>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findPerpToPlayer(int side)
	{
		mutantSearchFunctions.<findPerpToPlayer>c__Iterator91 <findPerpToPlayer>c__Iterator = new mutantSearchFunctions.<findPerpToPlayer>c__Iterator91();
		<findPerpToPlayer>c__Iterator.side = side;
		<findPerpToPlayer>c__Iterator.<$>side = side;
		<findPerpToPlayer>c__Iterator.<>f__this = this;
		return <findPerpToPlayer>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findPerpToEnemy(int side)
	{
		mutantSearchFunctions.<findPerpToEnemy>c__Iterator92 <findPerpToEnemy>c__Iterator = new mutantSearchFunctions.<findPerpToEnemy>c__Iterator92();
		<findPerpToEnemy>c__Iterator.side = side;
		<findPerpToEnemy>c__Iterator.<$>side = side;
		<findPerpToEnemy>c__Iterator.<>f__this = this;
		return <findPerpToEnemy>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findCloseBush(float dist)
	{
		mutantSearchFunctions.<findCloseBush>c__Iterator93 <findCloseBush>c__Iterator = new mutantSearchFunctions.<findCloseBush>c__Iterator93();
		<findCloseBush>c__Iterator.dist = dist;
		<findCloseBush>c__Iterator.<$>dist = dist;
		<findCloseBush>c__Iterator.<>f__this = this;
		return <findCloseBush>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findCloseTree(float dist)
	{
		mutantSearchFunctions.<findCloseTree>c__Iterator94 <findCloseTree>c__Iterator = new mutantSearchFunctions.<findCloseTree>c__Iterator94();
		<findCloseTree>c__Iterator.dist = dist;
		<findCloseTree>c__Iterator.<$>dist = dist;
		<findCloseTree>c__Iterator.<>f__this = this;
		return <findCloseTree>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator creepyFindCloseTree(float dist)
	{
		mutantSearchFunctions.<creepyFindCloseTree>c__Iterator95 <creepyFindCloseTree>c__Iterator = new mutantSearchFunctions.<creepyFindCloseTree>c__Iterator95();
		<creepyFindCloseTree>c__Iterator.dist = dist;
		<creepyFindCloseTree>c__Iterator.<$>dist = dist;
		<creepyFindCloseTree>c__Iterator.<>f__this = this;
		return <creepyFindCloseTree>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findJumpTree(bool towards)
	{
		mutantSearchFunctions.<findJumpTree>c__Iterator96 <findJumpTree>c__Iterator = new mutantSearchFunctions.<findJumpTree>c__Iterator96();
		<findJumpTree>c__Iterator.towards = towards;
		<findJumpTree>c__Iterator.<$>towards = towards;
		<findJumpTree>c__Iterator.<>f__this = this;
		return <findJumpTree>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findJumpAttackObj()
	{
		mutantSearchFunctions.<findJumpAttackObj>c__Iterator97 <findJumpAttackObj>c__Iterator = new mutantSearchFunctions.<findJumpAttackObj>c__Iterator97();
		<findJumpAttackObj>c__Iterator.<>f__this = this;
		return <findJumpAttackObj>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator findCloseJumpObj()
	{
		mutantSearchFunctions.<findCloseJumpObj>c__Iterator98 <findCloseJumpObj>c__Iterator = new mutantSearchFunctions.<findCloseJumpObj>c__Iterator98();
		<findCloseJumpObj>c__Iterator.<>f__this = this;
		return <findCloseJumpObj>c__Iterator;
	}

	private void findPlaneCrash()
	{
		if (this.sceneInfo.planeCrash != null)
		{
			this.ai.target = this.sceneInfo.planeCrash.transform;
			this.setup.pmSearch.SendEvent("doAction");
			this.ai.SearchPath();
		}
		else
		{
			this.setup.pmSearch.SendEvent("noValidTarget");
		}
	}

	private void resetCurrentJumpObj()
	{
		if (this.currentJumpObj)
		{
			this.currentJumpObj.occupied = false;
		}
	}

	private void checkNullTarget()
	{
		if (this.setup.ai.allPlayers.Count == 0)
		{
			return;
		}
		if (this.currentTarget == null)
		{
			this.currentTarget = this.sceneInfo.allPlayers[0];
		}
	}

	public void updateCurrentTarget()
	{
		if (this.setup.ai.creepy || this.setup.ai.creepy_fat || this.setup.ai.creepy_male || this.setup.ai.creepy_baby)
		{
			return;
		}
		this.newTarget = this.hitGo;
		this.ai.target = this.newTarget.transform;
		if (this.currentTarget != null)
		{
			this.lastTarget = this.currentTarget;
		}
		this.currentTarget = this.hitGo;
		this.fsmCurrentTargetGo.Value = this.hitGo;
		this.targetSetup = this.currentTarget.GetComponent<targetStats>();
		if (this.currentTarget.CompareTag("enemyRoot"))
		{
			this.setup.aiManager.setAggressiveCombat();
			this.setup.aiManager.Invoke("setDefaultCombat", 30f);
			this.setup.pmBrain.SendEvent("toSetAggressive");
		}
	}

	public void switchToNewTarget(GameObject go)
	{
		if (!go)
		{
			return;
		}
		if (this.currentTarget != go && !this.setup.ai.creepy_baby)
		{
			this.hitGo = go;
			this.fsmCurrentTargetGo.Value = go;
			this.ai.target = go.transform;
			if (this.currentTarget != null)
			{
				this.lastTarget = this.currentTarget;
			}
			this.currentTarget = go;
			this.targetSetup = go.GetComponent<targetStats>();
			base.StartCoroutine("toTrack");
		}
		if (this.ai.target != go.transform)
		{
			this.ai.target = go.transform;
		}
	}

	private void getFSMCurrentTarget()
	{
		this.fsmVisionTarget.Value = this.ai.target.gameObject;
	}

	public void updateCurrentWaypoint(Vector3 vect)
	{
		this.currentWaypoint.transform.position = vect;
		this.ai.target = this.currentWaypoint.transform;
		this.ai.SearchPath();
	}

	private Vector2 Circle2(float radius)
	{
		Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
		return normalized * radius;
	}

	private void setToHome()
	{
		this.currentWaypoint.transform.position = this.setup.homeGo.transform.position;
		this.ai.target = this.currentWaypoint.transform;
		this.ai.SearchPath();
	}

	public void setToWaypoint()
	{
		this.ai.target = this.currentWaypoint.transform;
		this.ai.SearchPath();
	}

	private void setWaypointToTarget()
	{
		this.currentWaypoint.transform.position = this.ai.target.position;
	}

	private void setToLastSighting()
	{
		this.ai.target = this.lastSighting.transform;
		this.ai.SearchPath();
	}

	private void setToTree()
	{
		this.ai.target = this.nearestTree.transform;
		this.ai.SearchPath();
	}

	private void setToPlayer()
	{
		if (this.setup.ai.allPlayers.Count == 0)
		{
			return;
		}
		if (this.currentTarget)
		{
			this.ai.target = this.currentTarget.transform;
		}
		else if (this.lastTarget)
		{
			this.ai.target = this.lastTarget.transform;
		}
		else
		{
			this.ai.target = this.ai.allPlayers[0].transform;
		}
		this.ai.SearchPath();
	}

	private void setToClosestPlayer()
	{
		if (this.setup.ai.allPlayers.Count == 0)
		{
			return;
		}
		if (this.ai.allPlayers[0])
		{
			this.currentWaypoint.transform.position = this.ai.allPlayers[0].transform.position;
			this.ai.target = this.ai.allPlayers[0].transform;
			this.ai.SearchPath();
		}
	}

	public void refreshCurrentTarget()
	{
		if (this.currentTarget == null)
		{
			this.ai.target = this.currentWaypoint.transform;
		}
	}

	private void setToCave()
	{
		this.ai.target = this.fsmCaveEntrance.Value.transform;
		this.ai.SearchPath();
	}

	private void setToHomeOffset()
	{
		if (this.setup.spawnGo)
		{
			this.currentWaypoint.transform.position = this.setup.spawnGo.transform.position;
		}
		this.ai.target = this.currentWaypoint.transform;
		base.StartCoroutine("getRandomTargetOffset", 1f);
		this.ai.SearchPath();
	}

	private void setToLeader()
	{
		if (this.fsmLeaderGo.Value)
		{
			this.ai.target = this.fsmLeaderGo.Value.transform;
			this.ai.SearchPath();
		}
	}

	private void setToMember()
	{
		if (this.familyFunctions.currentMemberTarget)
		{
			this.ai.target = this.familyFunctions.currentMemberTarget.transform;
		}
		else
		{
			this.ai.target = this.currentWaypoint.transform;
		}
		this.ai.SearchPath();
	}

	private void setToCurrentMember()
	{
		if (this.familyFunctions.currentMemberTarget)
		{
			this.ai.target = this.familyFunctions.currentMemberTarget.transform;
			this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentMemberGo").Value = this.familyFunctions.currentMemberTarget;
		}
		else
		{
			this.ai.target = this.currentWaypoint.transform;
			this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentMemberGo").Value = null;
		}
		this.ai.SearchPath();
	}

	private void setToBody()
	{
		if (this.familyFunctions.currentMemberTarget)
		{
			this.ai.target = this.familyFunctions.currentMemberTarget.transform;
		}
		else
		{
			if (this.setup.pmSearch)
			{
				this.setup.pmSearch.SendEvent("noValidPath");
			}
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.SendEvent("noValidPath");
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator getRandomTargetOffset(float dist)
	{
		mutantSearchFunctions.<getRandomTargetOffset>c__Iterator99 <getRandomTargetOffset>c__Iterator = new mutantSearchFunctions.<getRandomTargetOffset>c__Iterator99();
		<getRandomTargetOffset>c__Iterator.<>f__this = this;
		return <getRandomTargetOffset>c__Iterator;
	}

	private void resetTargetOffset()
	{
		this.ai.targetOffset = new Vector3(0f, 0f, 0f);
	}

	[DebuggerHidden]
	private IEnumerator findAngleToTarget(Vector3 target)
	{
		mutantSearchFunctions.<findAngleToTarget>c__Iterator9A <findAngleToTarget>c__Iterator9A = new mutantSearchFunctions.<findAngleToTarget>c__Iterator9A();
		<findAngleToTarget>c__Iterator9A.target = target;
		<findAngleToTarget>c__Iterator9A.<$>target = target;
		<findAngleToTarget>c__Iterator9A.<>f__this = this;
		return <findAngleToTarget>c__Iterator9A;
	}

	[DebuggerHidden]
	private IEnumerator findAngleFromPlayer()
	{
		mutantSearchFunctions.<findAngleFromPlayer>c__Iterator9B <findAngleFromPlayer>c__Iterator9B = new mutantSearchFunctions.<findAngleFromPlayer>c__Iterator9B();
		<findAngleFromPlayer>c__Iterator9B.<>f__this = this;
		return <findAngleFromPlayer>c__Iterator9B;
	}

	[DebuggerHidden]
	private IEnumerator findAngleToPlayer()
	{
		mutantSearchFunctions.<findAngleToPlayer>c__Iterator9C <findAngleToPlayer>c__Iterator9C = new mutantSearchFunctions.<findAngleToPlayer>c__Iterator9C();
		<findAngleToPlayer>c__Iterator9C.<>f__this = this;
		return <findAngleToPlayer>c__Iterator9C;
	}

	[DebuggerHidden]
	private IEnumerator setTreeAttatchDist(float dist)
	{
		mutantSearchFunctions.<setTreeAttatchDist>c__Iterator9D <setTreeAttatchDist>c__Iterator9D = new mutantSearchFunctions.<setTreeAttatchDist>c__Iterator9D();
		<setTreeAttatchDist>c__Iterator9D.dist = dist;
		<setTreeAttatchDist>c__Iterator9D.<$>dist = dist;
		<setTreeAttatchDist>c__Iterator9D.<>f__this = this;
		return <setTreeAttatchDist>c__Iterator9D;
	}

	[DebuggerHidden]
	private IEnumerator findTreeAttachPos(Vector3 pos)
	{
		mutantSearchFunctions.<findTreeAttachPos>c__Iterator9E <findTreeAttachPos>c__Iterator9E = new mutantSearchFunctions.<findTreeAttachPos>c__Iterator9E();
		<findTreeAttachPos>c__Iterator9E.pos = pos;
		<findTreeAttachPos>c__Iterator9E.<$>pos = pos;
		<findTreeAttachPos>c__Iterator9E.<>f__this = this;
		return <findTreeAttachPos>c__Iterator9E;
	}

	[DebuggerHidden]
	private IEnumerator findEatingPos(Vector3 eatPos)
	{
		mutantSearchFunctions.<findEatingPos>c__Iterator9F <findEatingPos>c__Iterator9F = new mutantSearchFunctions.<findEatingPos>c__Iterator9F();
		<findEatingPos>c__Iterator9F.eatPos = eatPos;
		<findEatingPos>c__Iterator9F.<$>eatPos = eatPos;
		<findEatingPos>c__Iterator9F.<>f__this = this;
		return <findEatingPos>c__Iterator9F;
	}

	[DebuggerHidden]
	private IEnumerator setToGuardPosition(GameObject go)
	{
		mutantSearchFunctions.<setToGuardPosition>c__IteratorA0 <setToGuardPosition>c__IteratorA = new mutantSearchFunctions.<setToGuardPosition>c__IteratorA0();
		<setToGuardPosition>c__IteratorA.go = go;
		<setToGuardPosition>c__IteratorA.<$>go = go;
		<setToGuardPosition>c__IteratorA.<>f__this = this;
		return <setToGuardPosition>c__IteratorA;
	}

	private void setToRescueRunPos()
	{
		Vector3 position = this.familyFunctions.currentMemberTarget.transform.GetChild(0).TransformPoint(Vector3.forward * -20f);
		this.currentWaypoint.transform.position = position;
		this.ai.target = this.currentWaypoint.transform;
	}

	[DebuggerHidden]
	private IEnumerator enableAwareOfPlayer()
	{
		mutantSearchFunctions.<enableAwareOfPlayer>c__IteratorA1 <enableAwareOfPlayer>c__IteratorA = new mutantSearchFunctions.<enableAwareOfPlayer>c__IteratorA1();
		<enableAwareOfPlayer>c__IteratorA.<>f__this = this;
		return <enableAwareOfPlayer>c__IteratorA;
	}

	private void disableAwareOfPlayer()
	{
		if (this.ai.creepy || this.ai.creepy_baby || this.ai.creepy_fat || this.ai.creepy_male)
		{
			return;
		}
		this.playerAware = false;
		this.fsmplayerAwareBool.Value = false;
		if (LocalPlayer.ScriptSetup && LocalPlayer.ScriptSetup.targetFunctions.visibleEnemies.Contains(base.transform.parent.gameObject))
		{
			LocalPlayer.ScriptSetup.targetFunctions.visibleEnemies.Remove(base.transform.parent.gameObject);
		}
		this.fsmplayerAwareBool.Value = false;
		base.StopCoroutine("enableAwareOfPlayer");
	}

	[DebuggerHidden]
	private IEnumerator enableSearchTimeout()
	{
		mutantSearchFunctions.<enableSearchTimeout>c__IteratorA2 <enableSearchTimeout>c__IteratorA = new mutantSearchFunctions.<enableSearchTimeout>c__IteratorA2();
		<enableSearchTimeout>c__IteratorA.<>f__this = this;
		return <enableSearchTimeout>c__IteratorA;
	}

	private void disableSearchTimeout()
	{
		base.StopCoroutine("enableSearchTimeout");
		this.enableSearch = false;
	}

	private void sortRecentlyBuiltStrctures()
	{
		this.sceneInfo.recentlyBuilt.Sort((GameObject c1, GameObject c2) => Vector3.Distance(this.tr.position, c1.transform.position).CompareTo(Vector3.Distance(this.tr.position, c2.transform.position)));
	}

	private void getClosestPlayerDist()
	{
		float num = float.PositiveInfinity;
		Transform transform = null;
		foreach (GameObject current in this.setup.sceneInfo.allPlayers)
		{
			if (current != null)
			{
				float num2 = Vector3.Distance(current.transform.position, this.tr.position);
				if (num2 < num)
				{
					transform = current.transform;
					num = num2;
				}
			}
		}
		if (this.setup.pmCombat)
		{
			this.fsmCombatClosestPlayerDist.Value = num;
		}
		if (this.setup.pmSearch)
		{
			this.fsmSearchClosestPlayerDist.Value = num;
		}
		if (transform)
		{
			Vector3 vector = base.transform.InverseTransformPoint(transform.position);
			this.fsmClosestPlayerAngle.Value = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		}
	}

	private void getPlaneDistanceToWaypoint()
	{
		if (this.setup.planeCrashGo == null)
		{
			this.setup.planeCrashGo = GameObject.FindWithTag("planeCrash");
		}
		if (this.setup.planeCrashGo != null)
		{
			this.setup.pmSearch.FsmVariables.GetFsmFloat("planeDist").Value = Vector3.Distance(this.currentWaypointGo.transform.position, this.setup.planeCrashGo.transform.position);
		}
		else
		{
			this.setup.pmSearch.SendEvent("noPlaneFound");
		}
	}

	private void playSearchScream()
	{
		if (!this.screamCooldown && UnityEngine.Random.value < 0.8f)
		{
			this.animator.SetIntegerReflected("randInt1", UnityEngine.Random.Range(4, 6));
			this.animator.SetBoolReflected("screamBOOL", true);
			this.screamCooldown = true;
			UnityEngine.Object.Instantiate(this.setup.soundGo, base.transform.position, base.transform.rotation);
			base.Invoke("resetScreamCooldown", 10f);
		}
	}

	private void resetScreamCooldown()
	{
		this.screamCooldown = false;
	}

	private void findCloseCaveWayPoint()
	{
		this.sceneInfo.caveWayPoints.Sort((Transform c1, Transform c2) => (this.tr.position - c1.position).sqrMagnitude.CompareTo((this.tr.position - c2.position).sqrMagnitude));
		if (Vector3.Distance(this.sceneInfo.caveWayPoints[0].position, this.tr.position) < 30f)
		{
			this.currentWaypointGo = this.sceneInfo.caveWayPoints[0].gameObject;
			this.wpSetup = this.currentWaypointGo.GetComponent<wayPointSetup>();
			this.updateCurrentWaypoint(this.currentWaypointGo.transform.position);
			if (this.setup.pmSearch)
			{
				this.setup.pmSearch.SendEvent("doAction");
			}
		}
		else if (this.setup.pmSearch)
		{
			this.setup.pmSearch.SendEvent("noValidTarget");
		}
	}

	private void getNextWayPoint()
	{
		if (this.wpSetup)
		{
			this.currentWaypointGo = this.wpSetup.nextWaypoint.gameObject;
			this.updateCurrentWaypoint(this.currentWaypointGo.transform.position);
			if (this.setup.pmSearch)
			{
				this.setup.pmSearch.SendEvent("doAction");
			}
			this.wpSetup = this.currentWaypointGo.GetComponent<wayPointSetup>();
		}
		else if (this.setup.pmSearch)
		{
			this.setup.pmSearch.SendEvent("noValidTarget");
		}
	}

	private void setWayPointParams()
	{
		if (this.wpSetup)
		{
			this.setup.pmSearch.FsmVariables.GetFsmBool("wayPointStopBool").Value = this.wpSetup.stopAtWaypoint;
			this.setup.pmSearch.FsmVariables.GetFsmFloat("wayPointMinWaitTime").Value = this.wpSetup.minWaitTime;
			this.setup.pmSearch.FsmVariables.GetFsmFloat("wayPointMaxWaitTime").Value = this.wpSetup.maxWaitTime;
		}
	}

	private void findWalkableGround()
	{
		if (!this.setup.pmSleep.FsmVariables.GetFsmBool("inCaveBool").Value)
		{
			this.updateCurrentWaypoint(this.ai.lastWalkablePos);
			this.setToWaypoint();
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.SendEvent("doAction");
			}
		}
		else
		{
			this.updateCurrentWaypoint(this.setup.spawnGo.transform.position);
			this.setToWaypoint();
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.SendEvent("doAction");
			}
		}
	}

	[DebuggerHidden]
	public IEnumerator setVisRetry(int set)
	{
		mutantSearchFunctions.<setVisRetry>c__IteratorA3 <setVisRetry>c__IteratorA = new mutantSearchFunctions.<setVisRetry>c__IteratorA3();
		<setVisRetry>c__IteratorA.set = set;
		<setVisRetry>c__IteratorA.<$>set = set;
		<setVisRetry>c__IteratorA.<>f__this = this;
		return <setVisRetry>c__IteratorA;
	}

	[DebuggerHidden]
	public IEnumerator toLook()
	{
		mutantSearchFunctions.<toLook>c__IteratorA4 <toLook>c__IteratorA = new mutantSearchFunctions.<toLook>c__IteratorA4();
		<toLook>c__IteratorA.<>f__this = this;
		return <toLook>c__IteratorA;
	}

	[DebuggerHidden]
	public IEnumerator toTrack()
	{
		mutantSearchFunctions.<toTrack>c__IteratorA5 <toTrack>c__IteratorA = new mutantSearchFunctions.<toTrack>c__IteratorA5();
		<toTrack>c__IteratorA.<>f__this = this;
		return <toTrack>c__IteratorA;
	}

	[DebuggerHidden]
	public IEnumerator toDisableVis()
	{
		mutantSearchFunctions.<toDisableVis>c__IteratorA6 <toDisableVis>c__IteratorA = new mutantSearchFunctions.<toDisableVis>c__IteratorA6();
		<toDisableVis>c__IteratorA.<>f__this = this;
		return <toDisableVis>c__IteratorA;
	}

	private void storeLastSighting()
	{
		Scene.SceneTracker.addToVisible(this.rootTr.gameObject);
		this.playerAware = true;
		if (this.hitGo.CompareTag("Player") && LocalPlayer.ScriptSetup && !LocalPlayer.ScriptSetup.targetFunctions.visibleEnemies.Contains(base.transform.parent.gameObject))
		{
			LocalPlayer.ScriptSetup.targetFunctions.visibleEnemies.Add(base.transform.parent.gameObject);
		}
		if (this.setup.pmBrain)
		{
			this.setup.pmBrain.FsmVariables.GetFsmBool("targetSeenBool").Value = true;
		}
		this.targetCounter = 0;
		this.lastSighting.transform.position = this.hitGo.transform.position;
	}

	private void toLostTarget()
	{
		this.enableAwareOfPlayer();
		Scene.SceneTracker.removeFromVisible(this.rootTr.gameObject);
		this.trackCounter = 0;
		if (this.setup.animator.GetCurrentAnimatorStateInfo(0).tagHash != this.setup.hashs.deathTag)
		{
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.SendEvent("toTargetLost");
			}
			if (this.setup.pmSearch)
			{
				this.setup.pmSearch.SendEvent("targetLost");
			}
			if (this.setup.pmStalk)
			{
				this.setup.pmStalk.SendEvent("targetLost");
			}
		}
		base.StopCoroutine("toTrack");
		base.StopCoroutine("toDisable");
		base.StartCoroutine("toLook");
	}

	private void setNewTarget()
	{
		this.lookingForTarget = false;
		Scene.SceneTracker.addToVisible(this.rootTr.gameObject);
		this.lastSighting.transform.position = this.hitGo.transform.position;
		if (this.setup.pmBrain)
		{
			this.setup.pmBrain.FsmVariables.GetFsmBool("targetSeenBool").Value = true;
		}
		if (!this.ai.creepy && !this.ai.creepy_baby && !this.ai.creepy_fat && !this.ai.creepy_male)
		{
			this.disableAwareOfPlayer();
			if (this.setup.animator.GetCurrentAnimatorStateInfo(0).tagHash != this.setup.hashs.deathTag)
			{
				this.setup.enemyEvents.playPlayerSighted();
				this.setup.pmSearch.SendEvent("targetFound");
				this.setup.pmCombat.SendEvent("targetFound");
				this.setup.pmEncounter.SendEvent("targetFound");
				this.setup.pmSleep.SendEvent("targetFound");
			}
		}
		this.updateCurrentTarget();
		base.StopCoroutine("toLook");
		base.StartCoroutine("toTrack");
	}

	private bool raycastTargets()
	{
		this.targetFound = false;
		if (this.setup.sceneInfo.allPlayers.Count > 0)
		{
			foreach (GameObject current in this.setup.sceneInfo.allPlayers)
			{
				if (current && !this.targetFound)
				{
					this.tempTargetDist = Vector3.Distance(this.tr.position, current.transform.position);
					if (this.tempTargetDist < 100f)
					{
						Vector3 vector = this.tr.InverseTransformPoint(current.transform.position);
						this.currTargetAngle = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
						if (this.currTargetAngle > -65f && this.currTargetAngle < 65f)
						{
							Vector3 direction = Vector3.zero;
							Collider component = current.GetComponent<Collider>();
							if (this.setup.headJoint)
							{
								if (component)
								{
									direction = current.GetComponent<Collider>().bounds.center - this.setup.headJoint.transform.position;
								}
								else
								{
									direction = current.transform.position - this.setup.headJoint.transform.position;
								}
							}
							else if (component)
							{
								direction = current.GetComponent<Collider>().bounds.center - this.setup.animator.rootPosition;
							}
							else
							{
								direction = current.transform.position - this.setup.animator.rootPosition;
							}
							if (this.setup.headJoint)
							{
								this.castPos = this.setup.headJoint.transform.position;
							}
							else
							{
								this.castPos = this.setup.animator.rootPosition;
							}
							if (Physics.Raycast(this.castPos, direction, out this.hit, 100f, this.visLayerMask))
							{
								this.hitGo = this.hit.transform.gameObject;
								this.targetFound = true;
								return true;
							}
						}
					}
				}
			}
		}
		this.targetFound = false;
		return false;
	}

	private bool rayCastActiveTarget()
	{
		if (!this.hitGo)
		{
			return false;
		}
		this.tempTargetDist = Vector3.Distance(this.tr.position, this.hitGo.transform.position);
		if (this.tempTargetDist > this.setLongVisRange)
		{
			return false;
		}
		if (this.setup.headJoint)
		{
			this.castPos = this.setup.headJoint.transform.position;
		}
		else
		{
			this.castPos = this.setup.animator.rootPosition;
		}
		Collider component = this.hitGo.GetComponent<Collider>();
		Vector3 direction;
		if (component)
		{
			direction = this.hitGo.GetComponent<Collider>().bounds.center - this.castPos;
		}
		else
		{
			direction = this.hitGo.transform.position - this.castPos;
		}
		if (Physics.Raycast(this.castPos, direction, out this.hit, this.setLongVisRange, this.visLayerMask))
		{
			this.hitGo = this.hit.transform.gameObject;
			return true;
		}
		return false;
	}

	private bool checkEnemyTargets()
	{
		if (this.ai.allPlayers.Count == 0)
		{
			return false;
		}
		if (this.ai.allPlayers[0] == null)
		{
			return false;
		}
		if (this.ai.awayFromPlayer)
		{
			return false;
		}
		for (int i = 0; i < this.sceneInfo.closeEnemies.Count; i++)
		{
			GameObject value = this.sceneInfo.closeEnemies[i];
			int index = UnityEngine.Random.Range(i, this.sceneInfo.closeEnemies.Count);
			this.sceneInfo.closeEnemies[i] = this.sceneInfo.closeEnemies[index];
			this.sceneInfo.closeEnemies[index] = value;
		}
		if (this.setup.sceneInfo.closeEnemies.Count > 0 && !this.targetFound)
		{
			foreach (GameObject current in this.setup.sceneInfo.closeEnemies)
			{
				if (current && !this.targetFound && current.name != base.transform.parent.gameObject.name && current.activeSelf)
				{
					this.tempTargetDist = Vector3.Distance(this.tr.position, current.transform.position);
					if (this.tempTargetDist < this.setCloseVisRange)
					{
						Vector3 vector = this.tr.InverseTransformPoint(current.transform.position);
						this.currTargetAngle = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
						if (this.currTargetAngle > -90f && this.currTargetAngle < 90f)
						{
							this.hitGo = current;
							return true;
						}
					}
				}
			}
		}
		this.targetFound = false;
		return false;
	}

	private bool checkForClosePlayers()
	{
		if (this.setup.sceneInfo.allPlayers.Count > 0)
		{
			foreach (GameObject current in this.setup.sceneInfo.allPlayers)
			{
				if (current)
				{
					this.tempTargetDist = Vector3.Distance(this.tr.position, current.transform.position);
					if (this.tempTargetDist < this.ai.playerDist && this.tempTargetDist < 35f && current.name != this.currentTarget.name)
					{
						Vector3 vector = this.tr.InverseTransformPoint(current.transform.position);
						this.currTargetAngle = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
						if (this.currTargetAngle > -70f && this.currTargetAngle < 70f)
						{
							if (this.setup.headJoint)
							{
								this.castPos = this.setup.headJoint.transform.position;
							}
							else
							{
								this.castPos = this.setup.animator.rootPosition;
							}
							Vector3 direction = current.GetComponent<Collider>().bounds.center - this.castPos;
							if (Physics.Raycast(this.castPos, direction, out this.hit, this.setCloseVisRange, this.visLayerMask))
							{
								this.hitGo = this.hit.transform.gameObject;
								this.closePlayerTarget = current;
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		return false;
	}

	private bool checkForPlayers()
	{
		if (this.setup.sceneInfo.allPlayers.Count > 0)
		{
			foreach (GameObject current in this.setup.sceneInfo.allPlayers)
			{
				if (current)
				{
					Vector3 vector = this.tr.InverseTransformPoint(current.transform.position);
					this.currTargetAngle = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
					if (this.currTargetAngle > -70f && this.currTargetAngle < 70f)
					{
						if (this.setup.headJoint)
						{
							this.castPos = this.setup.headJoint.transform.position;
						}
						else
						{
							this.castPos = this.setup.animator.rootPosition;
						}
						Vector3 direction = current.GetComponent<Collider>().bounds.center - this.castPos;
						if (Physics.Raycast(this.castPos, direction, out this.hit, this.setCloseVisRange, this.visLayerMask))
						{
							this.closePlayerTarget = current;
							return true;
						}
					}
				}
			}
			return false;
		}
		return false;
	}

	public bool checkValidTarget()
	{
		if (!this.hitGo)
		{
			return false;
		}
		if (!this.hitGo.activeSelf)
		{
			return false;
		}
		if (this.hitGo.CompareTag("Player") || this.hitGo.CompareTag("Player_Remote") || this.hitGo.CompareTag("PlayerNet") || this.hitGo.CompareTag("PlayerRemote") || this.hitGo.CompareTag("playerFire"))
		{
			visRangeSetup component = this.hitGo.GetComponent<visRangeSetup>();
			targetStats component2 = this.hitGo.GetComponent<targetStats>();
			if (component2 && component2.targetDown)
			{
				return false;
			}
			if (!component)
			{
				return true;
			}
			if (this.startedTrack)
			{
				if (Vector3.Distance(this.rootTr.position, this.hitGo.transform.position) < this.setLongVisRange)
				{
					component.isTarget = true;
					return true;
				}
				return false;
			}
			else
			{
				if (Vector3.Distance(this.rootTr.position, this.hitGo.transform.position) < component.unscaledVisRange)
				{
					component.isTarget = true;
					return true;
				}
				return false;
			}
		}
		else
		{
			if (!this.hitGo.CompareTag("enemyRoot") || !(this.hitGo.name != base.transform.parent.gameObject.name))
			{
				return false;
			}
			mutantTypeSetup component3 = this.hitGo.GetComponent<mutantTypeSetup>();
			if (!component3)
			{
				return false;
			}
			if (!component3.stats.targetDown)
			{
				if (this.setup.ai.maleSkinny || this.setup.ai.femaleSkinny)
				{
					if (!component3.setup.ai.femaleSkinny && !component3.setup.ai.maleSkinny)
					{
						return true;
					}
				}
				else if (this.setup.ai.pale)
				{
					if (!component3.setup.ai.pale && !component3.setup.ai.creepy && !component3.setup.ai.creepy_male && component3.setup.ai.creepy_fat)
					{
						return true;
					}
				}
				else if (this.setup.ai.male || this.setup.ai.female)
				{
					if (component3.setup.ai.pale || component3.setup.ai.maleSkinny || component3.setup.ai.femaleSkinny || component3.setup.ai.creepy || component3.setup.ai.creepy_male || component3.setup.ai.creepy_fat)
					{
						return true;
					}
				}
				else
				{
					if (!this.setup.ai.creepy && !this.setup.ai.creepy_male && !component3.setup.ai.creepy_fat)
					{
						return false;
					}
					if (component3.setup.ai.maleSkinny || component3.setup.ai.femaleSkinny || component3.setup.ai.male || component3.setup.ai.female)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	private bool checkTargetStatus()
	{
		if (!this.hitGo)
		{
			return false;
		}
		if (!this.hitGo.activeSelf || this.hitGo.name == base.transform.parent.gameObject.name)
		{
			return false;
		}
		this.targetSetup = this.hitGo.GetComponent<targetStats>();
		return !this.targetSetup || !this.targetSetup.targetDown;
	}

	private bool checkCurrentTargetStatus()
	{
		if (!this.currentTarget)
		{
			return false;
		}
		if (!this.currentTarget.activeSelf || this.currentTarget.name == base.transform.parent.gameObject.name)
		{
			return false;
		}
		this.targetSetup = this.currentTarget.GetComponent<targetStats>();
		return !this.targetSetup || !this.targetSetup.targetDown;
	}

	public void updateClosePlayerTarget()
	{
		this.newTarget = this.closePlayerTarget;
		this.ai.target = this.closePlayerTarget.transform;
		this.hitGo = this.closePlayerTarget;
		if (this.currentTarget != null)
		{
			this.lastTarget = this.currentTarget;
		}
		this.currentTarget = this.closePlayerTarget;
		this.fsmCurrentTargetGo.Value = this.closePlayerTarget;
	}

	private Vector3 getTargetDir()
	{
		if (!(this.currentTarget != null))
		{
			return Vector3.zero;
		}
		Collider component = this.currentTarget.GetComponent<Collider>();
		if (component)
		{
			return component.bounds.center - this.setup.headJoint.transform.position;
		}
		return this.currentTarget.transform.position - this.setup.headJoint.transform.position;
	}

	private void setStructureTimeout()
	{
		this.setup.pmCombat.FsmVariables.GetFsmBool("structureTimeout").Value = true;
		base.Invoke("resetStructureTimeout", 30f);
	}

	private void resetStructureTimeout()
	{
		this.setup.pmCombat.FsmVariables.GetFsmBool("structureTimeout").Value = false;
	}

	private void resetClosestStructureTarget()
	{
		Scene.SceneTracker.closeStructureTarget = null;
	}
}
