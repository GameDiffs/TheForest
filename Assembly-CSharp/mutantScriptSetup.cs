using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantScriptSetup : MonoBehaviour
{
	public mutantFamilyFunctions familyFunctions;

	public Animator animator;

	public mutantAI ai;

	public mutantAiManager aiManager;

	public sceneTracker sceneInfo;

	public mutantTypeSetup typeSetup;

	public mutantSearchFunctions search;

	public mutantWorldSearchFunctions worldSearch;

	public targetStats mutantStats;

	public EnemyHealth health;

	public mutantDayCycle dayCycle;

	public enemyAnimEvents enemyEvents;

	public CharacterController controller;

	public mutantMaleHashId hashs;

	public mutantPropManager propManager;

	public setupBodyVariation bodyVariation;

	public mutantCollisionDetect collisionDetect;

	public mutantHitReceiver hitReceiver;

	public mutantAnimatorControl animControl;

	private PlayMakerFSM[] allFSM;

	public PlayMakerFSM pmCombat;

	public PlayMakerFSM pmSearch;

	public PlayMakerFSM pmStalk;

	public PlayMakerFSM pmVision;

	public PlayMakerFSM pmTree;

	public PlayMakerFSM pmMotor;

	public PlayMakerFSM pmSleep;

	public PlayMakerFSM pmBrain;

	public PlayMakerFSM pmAlert;

	public PlayMakerFSM pmTargetManager;

	public PlayMakerFSM pmEncounter;

	public List<PlayMakerFSM> actionFSM = new List<PlayMakerFSM>();

	public GameObject fireBombGo;

	public GameObject thrownFireBombGo;

	public GameObject dynamiteBeltGo;

	public GameObject soundGo;

	public GameObject thisGo;

	public Transform rootTr;

	public Transform rotateTr;

	public GameObject headJoint;

	public Transform lookatTr;

	public GameObject currentWaypoint;

	public GameObject lastSighting;

	public GameObject weapon;

	public GameObject fireWeapon;

	public GameObject clawsWeapon;

	public GameObject leftWeapon;

	public GameObject leftWeapon1;

	public GameObject rightWeapon;

	public GameObject mainWeapon;

	public GameObject charLeftWeaponGo;

	public GameObject clubPickup;

	public GameObject spawnGo;

	public GameObject homeGo;

	public GameObject feederGo;

	public GameObject bodyCollider;

	public GameObject planeCrashGo;

	public CapsuleCollider bodyCollisionCollider;

	public GameObject headColliderGo;

	private HashSet<Type> allowedTypes;

	public Transform leftFoot;

	public Transform rightHand;

	public GameObject[] feedingProps;

	public GameObject heldMeat;

	public GameObject spawnedMeat;

	public int hashName;

	public FsmGameObject fsmTarget;

	public bool disableAiForDebug;

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
		if (this.disableAiForDebug)
		{
			this.disableForDebug();
		}
		this.allFSM = base.gameObject.GetComponents<PlayMakerFSM>();
		PlayMakerFSM[] array = this.allFSM;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			if (playMakerFSM.FsmName == "action_combatFSM")
			{
				this.pmCombat = playMakerFSM;
				this.actionFSM.Add(playMakerFSM);
			}
			if (playMakerFSM.FsmName == "action_inTreeFSM")
			{
				this.pmTree = playMakerFSM;
				this.actionFSM.Add(playMakerFSM);
			}
			if (playMakerFSM.FsmName == "global_visionFSM")
			{
				this.pmVision = playMakerFSM;
			}
			if (playMakerFSM.FsmName == "action_searchFSM")
			{
				this.pmSearch = playMakerFSM;
				this.actionFSM.Add(playMakerFSM);
			}
			if (playMakerFSM.FsmName == "action_stalkingFSM")
			{
				this.pmStalk = playMakerFSM;
				this.actionFSM.Add(playMakerFSM);
			}
			if (playMakerFSM.FsmName == "global_motorFSM")
			{
				this.pmMotor = playMakerFSM;
			}
			if (playMakerFSM.FsmName == "action_sleepingFSM")
			{
				this.pmSleep = playMakerFSM;
				this.actionFSM.Add(playMakerFSM);
			}
			if (playMakerFSM.FsmName == "global_brainFSM")
			{
				this.pmBrain = playMakerFSM;
			}
			if (playMakerFSM.FsmName == "global_alertManagerFSM")
			{
				this.pmAlert = playMakerFSM;
			}
			if (playMakerFSM.FsmName == "action_encounterFSM")
			{
				this.pmEncounter = playMakerFSM;
				this.actionFSM.Add(playMakerFSM);
			}
			if (playMakerFSM.FsmName == "global_targetManagerFSM")
			{
				this.pmTargetManager = playMakerFSM;
			}
		}
		this.thisGo = base.gameObject;
		this.rootTr = base.transform.root.transform;
		this.rotateTr = base.transform;
		this.familyFunctions = base.transform.parent.GetComponent<mutantFamilyFunctions>();
		this.animator = base.GetComponent<Animator>();
		this.ai = base.GetComponent<mutantAI>();
		this.aiManager = base.GetComponent<mutantAiManager>();
		this.typeSetup = base.transform.parent.GetComponent<mutantTypeSetup>();
		this.health = base.GetComponent<EnemyHealth>();
		this.dayCycle = base.GetComponentInChildren<mutantDayCycle>();
		this.enemyEvents = base.transform.GetComponent<enemyAnimEvents>();
		this.controller = base.transform.root.GetComponent<CharacterController>();
		this.hashs = base.transform.GetComponent<mutantMaleHashId>();
		this.propManager = base.transform.GetComponent<mutantPropManager>();
		this.bodyVariation = base.transform.GetComponentInChildren<setupBodyVariation>();
		this.collisionDetect = base.transform.GetComponentInChildren<mutantCollisionDetect>();
		this.hitReceiver = base.transform.GetComponentInChildren<mutantHitReceiver>();
		this.animControl = base.transform.GetComponentInChildren<mutantAnimatorControl>();
		if (!this.disableAiForDebug)
		{
			this.sceneInfo = Scene.SceneTracker;
		}
		this.search = base.GetComponent<mutantSearchFunctions>();
		this.worldSearch = base.transform.parent.GetComponent<mutantWorldSearchFunctions>();
		this.mutantStats = base.transform.parent.GetComponent<targetStats>();
		Transform[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Transform>();
		Transform[] array2 = componentsInChildren;
		for (int j = 0; j < array2.Length; j++)
		{
			Transform transform = array2[j];
			if (transform.name == "char_Head")
			{
				this.headJoint = transform.gameObject;
			}
			if (transform.name == "char_LeftHandWeapon")
			{
				this.charLeftWeaponGo = transform.gameObject;
			}
			if (transform.name == "currentWaypoint")
			{
				this.currentWaypoint = transform.gameObject;
			}
			if (transform.name == "lastPlayerSighting")
			{
				this.lastSighting = transform.gameObject;
			}
			if (transform.name == "tempLookat")
			{
				this.lookatTr = transform;
			}
			if (transform.name == "char_club_mesh1")
			{
				this.weapon = transform.gameObject;
			}
			if (transform.name == "FireStick")
			{
				this.fireWeapon = transform.gameObject;
			}
			if (transform.name == "char_RightHand")
			{
				this.clawsWeapon = transform.gameObject;
			}
			if (transform.name == "weaponLeftGO")
			{
				this.leftWeapon = transform.gameObject;
			}
			if (transform.name == "weaponLeftGO1")
			{
				this.leftWeapon1 = transform.gameObject;
			}
			if (transform.name == "weaponRightGO")
			{
				this.rightWeapon = transform.gameObject;
			}
			if (transform.name == "mainHitTrigger")
			{
				this.mainWeapon = transform.gameObject;
			}
			if (transform.name == "fireBomb")
			{
				this.fireBombGo = transform.gameObject;
			}
			if (transform.name == "dragPointGo")
			{
				this.familyFunctions.dragPoint = transform.gameObject;
			}
			if (transform.name == "bodyCollision")
			{
				if (this.pmCombat)
				{
					this.pmCombat.FsmVariables.GetFsmGameObject("bodyCollisionGo").Value = transform.gameObject;
				}
				this.bodyCollisionCollider = transform.GetComponent<CapsuleCollider>();
			}
			if (transform.name == "char_LeftFoot")
			{
				this.leftFoot = transform;
			}
			if (transform.name == "headCollision")
			{
				this.headColliderGo = transform.gameObject;
			}
		}
	}

	private void Start()
	{
		this.hashName = Animator.StringToHash(base.transform.parent.gameObject.name);
		if (!this.disableAiForDebug && this.pmSearch)
		{
			this.pmSearch.FsmVariables.GetFsmGameObject("planeCrashGo").Value = this.sceneInfo.planeCrash;
		}
		if (this.pmVision)
		{
			this.pmVision.FsmVariables.GetFsmGameObject("headJntGO").Value = this.headJoint;
		}
		if (this.pmSleep)
		{
			this.pmSleep.FsmVariables.GetFsmGameObject("leftHandGo").Value = this.charLeftWeaponGo;
		}
		if (this.pmEncounter)
		{
			this.pmEncounter.FsmVariables.GetFsmGameObject("leftHandGo").Value = this.charLeftWeaponGo;
		}
		if (this.pmCombat)
		{
			this.pmCombat.FsmVariables.GetFsmGameObject("wayPointGO").Value = this.currentWaypoint;
		}
		if (this.pmSearch)
		{
			this.pmSearch.FsmVariables.GetFsmGameObject("wayPointGO").Value = this.currentWaypoint;
		}
		if (this.pmVision)
		{
			this.pmVision.FsmVariables.GetFsmGameObject("lastSightingGo").Value = this.lastSighting;
		}
		if (this.pmCombat)
		{
			this.pmCombat.FsmVariables.GetFsmInt("HashName").Value = this.hashName;
		}
		if (this.pmCombat)
		{
			this.pmCombat.FsmVariables.GetFsmGameObject("playerFsmGO").Value = LocalPlayer.PlayerBase;
			this.fsmTarget = this.pmCombat.FsmVariables.GetFsmGameObject("target");
			this.pmCombat.FsmVariables.GetFsmGameObject("playerGO").Value = LocalPlayer.GameObject;
		}
		if (this.pmTree)
		{
			this.pmTree.FsmVariables.GetFsmGameObject("playerGO").Value = LocalPlayer.PlayerBase;
		}
		if (this.pmSearch)
		{
			this.pmSearch.FsmVariables.GetFsmGameObject("playerGO").Value = LocalPlayer.GameObject;
		}
		if (this.pmVision)
		{
			this.pmVision.FsmVariables.GetFsmGameObject("aiControlGo").Value = GameObject.Find("AiMaster");
			this.pmVision.FsmVariables.GetFsmGameObject("playerGO").Value = LocalPlayer.GameObject;
		}
	}

	public void setupPlayer()
	{
		if (this.pmCombat)
		{
			this.pmCombat.FsmVariables.GetFsmGameObject("playerFsmGO").Value = LocalPlayer.PlayerBase;
			this.fsmTarget = this.pmCombat.FsmVariables.GetFsmGameObject("target");
			this.pmCombat.FsmVariables.GetFsmGameObject("playerGO").Value = LocalPlayer.GameObject;
		}
		if (this.pmTree)
		{
			this.pmTree.FsmVariables.GetFsmGameObject("playerGO").Value = LocalPlayer.PlayerBase;
		}
		if (this.pmSearch)
		{
			this.pmSearch.FsmVariables.GetFsmGameObject("playerGO").Value = LocalPlayer.GameObject;
		}
		if (this.pmVision)
		{
			this.pmVision.FsmVariables.GetFsmGameObject("playerGO").Value = LocalPlayer.GameObject;
		}
	}

	[DebuggerHidden]
	public IEnumerator disableNonActiveFSM(string name)
	{
		mutantScriptSetup.<disableNonActiveFSM>c__Iterator83 <disableNonActiveFSM>c__Iterator = new mutantScriptSetup.<disableNonActiveFSM>c__Iterator83();
		<disableNonActiveFSM>c__Iterator.name = name;
		<disableNonActiveFSM>c__Iterator.<$>name = name;
		<disableNonActiveFSM>c__Iterator.<>f__this = this;
		return <disableNonActiveFSM>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator disableAllFSM()
	{
		mutantScriptSetup.<disableAllFSM>c__Iterator84 <disableAllFSM>c__Iterator = new mutantScriptSetup.<disableAllFSM>c__Iterator84();
		<disableAllFSM>c__Iterator.<>f__this = this;
		return <disableAllFSM>c__Iterator;
	}

	public void disableForDebug()
	{
		PlayMakerFSM[] components = base.GetComponents<PlayMakerFSM>();
		PlayMakerFSM[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			playMakerFSM.enabled = false;
			UnityEngine.Debug.Log(playMakerFSM.FsmName + " was disabled");
		}
		PlayMakerArrayListProxy[] components2 = base.GetComponents<PlayMakerArrayListProxy>();
		PlayMakerArrayListProxy[] array2 = components2;
		for (int j = 0; j < array2.Length; j++)
		{
			PlayMakerArrayListProxy playMakerArrayListProxy = array2[j];
			playMakerArrayListProxy.enabled = false;
		}
		base.GetComponent<mutantAI>().enabled = false;
		mutantAnimatorControl component = base.GetComponent<mutantAnimatorControl>();
		if (component)
		{
			component.enabled = false;
		}
		creepyAnimatorControl component2 = base.GetComponent<creepyAnimatorControl>();
		if (component2)
		{
			component2.enabled = false;
		}
		mutantBabyAnimatorControl component3 = base.GetComponent<mutantBabyAnimatorControl>();
		if (component3)
		{
			component3.enabled = false;
		}
	}
}
