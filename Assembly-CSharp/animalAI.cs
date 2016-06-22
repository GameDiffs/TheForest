using HutongGames.PlayMaker;
using Pathfinding;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class animalAI : MonoBehaviour
{
	private animalAvoidance1 avoidScript;

	public SkinnedMeshRenderer skinRenderer;

	public GameObject[] disableWhenOffscreen;

	public bool parentSetup;

	public bool doObjectAvoidance;

	public float avoidanceRate;

	public bool followOtherAnimals;

	public string startleEvent;

	public clsragdollify ragdoll;

	public GameObject BurntRagdoll;

	private animalSpawnFunctions spawnFunctions;

	private sceneTracker sceneInfo;

	public animalSearchFunctions searchFunctions;

	public Animator avatar;

	private AnimatorStateInfo currentState;

	private AnimatorStateInfo nextState;

	public PlayMakerFSM playMaker;

	public PlayMakerFSM pmMove;

	public float smoothedDir;

	public float absDir;

	public float repathRate;

	public Transform forceTarget;

	public List<GameObject> allPlayers = new List<GameObject>();

	private Transform Tr;

	private Transform rootTr;

	public Transform animatorTr;

	public float approachDist;

	public float deadZone;

	public float rotationSpeed;

	public float smoothSpeed;

	private Vector3 getDir;

	public Vector3 wantedDir;

	public bool aimAtWaypoint;

	private Vector3 currentDir;

	private float angle;

	public bool doMove;

	public bool closeTurn;

	public int turnInt;

	private bool doEnable;

	private Vector3 playerTarget;

	private Vector3 animalTarget;

	private bool forceDir;

	private float animalAngle;

	public bool swimming;

	private FsmFloat fsmPathDir;

	private FsmFloat fsmSmoothPathDir;

	private FsmFloat fsmSpeed;

	private FsmFloat fsmTargetDir;

	private FsmVector3 fsmTargetVec;

	private FsmFloat fsmIkBlend;

	public FsmFloat fsmPlayerDist;

	private FsmFloat fsmPlayerAngle;

	private FsmGameObject fsmAnimatorGO;

	public FsmFloat fsmRotateSpeed;

	private FsmBool fsmTreeBool;

	private FsmGameObject fsmFollowGo;

	private FsmBool fsmIsAttacking;

	private int layerMask2;

	private RaycastHit hit2;

	private Vector3 pos;

	public Transform target;

	private Seeker seeker;

	public Path path;

	public float speed = 5f;

	public float nextWaypointDistance = 3f;

	public int currentWaypoint;

	public bool isFollowing;

	public bool inTree;

	public bool cansearch;

	private bool ragdollSpawned;

	private bool startedRun;

	private bool startedMove;

	private bool startedFollow;

	private bool startedTrot;

	private bool startedWalk;

	private float followDist;

	private void Awake()
	{
		this.avoidScript = base.transform.GetComponentInChildren<animalAvoidance1>();
		this.sceneInfo = Scene.SceneTracker;
		if (this.parentSetup)
		{
			this.avatar = base.GetComponentInChildren<Animator>();
		}
		else
		{
			this.avatar = base.GetComponent<Animator>();
		}
		this.spawnFunctions = base.GetComponent<animalSpawnFunctions>();
		this.rootTr = base.transform;
		this.Tr = this.avatar.transform;
		this.deadZone *= 0.0174532924f;
		this.searchFunctions = base.transform.GetComponent<animalSearchFunctions>();
		this.seeker = base.GetComponent<Seeker>();
		this.layerMask2 = 33556480;
		PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
		PlayMakerFSM[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			if (playMakerFSM.FsmName == "aiBaseFSM")
			{
				this.playMaker = playMakerFSM;
			}
			if (playMakerFSM.FsmName == "moveFSM")
			{
				this.pmMove = playMakerFSM;
			}
		}
	}

	private void Start()
	{
		this.avatar.enabled = true;
		this.fsmPathDir = this.playMaker.FsmVariables.GetFsmFloat("pathDir");
		this.fsmSmoothPathDir = this.playMaker.FsmVariables.GetFsmFloat("smoothedPathDir");
		this.fsmTargetDir = this.playMaker.FsmVariables.GetFsmFloat("targetDir");
		if (this.pmMove)
		{
			this.fsmTargetVec = this.pmMove.FsmVariables.GetFsmVector3("targetDir");
		}
		this.fsmPlayerDist = this.playMaker.FsmVariables.GetFsmFloat("playerDist");
		this.fsmPlayerAngle = this.playMaker.FsmVariables.GetFsmFloat("playerAngle");
		if (this.pmMove)
		{
			this.fsmRotateSpeed = this.pmMove.FsmVariables.GetFsmFloat("rotateSpeed");
		}
		this.fsmTreeBool = this.playMaker.FsmVariables.GetFsmBool("treeBool");
		this.fsmFollowGo = this.playMaker.FsmVariables.GetFsmGameObject("closeAnimalGo");
		this.fsmIsAttacking = this.playMaker.FsmVariables.GetFsmBool("isAttacking");
		this.animatorTr = base.transform;
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			if (transform.name == "lizard_BASE")
			{
				this.playMaker.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				this.animatorTr = transform;
			}
			if (transform.name == "rabbit_ANIM_base")
			{
				this.playMaker.FsmVariables.GetFsmGameObject("animatorGo").Value = transform.gameObject;
				if (this.pmMove)
				{
					this.pmMove.FsmVariables.GetFsmGameObject("animatorGo").Value = transform.gameObject;
				}
				this.animatorTr = transform;
			}
			if (transform.name == "tortoise_ANIM_base")
			{
				this.playMaker.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				if (this.pmMove)
				{
					this.pmMove.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				}
				this.animatorTr = transform;
			}
			if (transform.name == "raccoon_ANIM_base")
			{
				this.playMaker.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				if (this.pmMove)
				{
					this.pmMove.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				}
				this.animatorTr = transform;
			}
			if (transform.name == "deer_ANIM_base")
			{
				this.playMaker.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				if (this.pmMove)
				{
					this.pmMove.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				}
				this.animatorTr = transform;
			}
			if (transform.name == "boar_ANIM_base")
			{
				this.playMaker.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				if (this.pmMove)
				{
					this.pmMove.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				}
				this.animatorTr = transform;
			}
			if (transform.name == "crocodile_ANIM_base")
			{
				this.playMaker.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				if (this.pmMove)
				{
					this.pmMove.FsmVariables.GetFsmGameObject("animatorGO").Value = transform.gameObject;
				}
				this.animatorTr = transform;
			}
			this.allPlayers = new List<GameObject>(this.sceneInfo.allPlayers);
			float num = UnityEngine.Random.Range(0.1f, 1.3f);
			if (!base.IsInvoking("updatePlayerTargets"))
			{
				base.InvokeRepeating("updatePlayerTargets", num, 1.2f);
			}
			if (this.doObjectAvoidance && !base.IsInvoking("closeObjectAvoidance"))
			{
				base.InvokeRepeating("closeObjectAvoidance", UnityEngine.Random.Range(num, 2f), this.avoidanceRate);
			}
			this.doEnable = true;
		}
		this.target = LocalPlayer.Transform;
		if (this.followOtherAnimals)
		{
			this.playMaker.FsmVariables.GetFsmBool("followOthersBool").Value = true;
		}
	}

	private void OnEnable()
	{
		if (!this.avatar)
		{
			if (this.parentSetup)
			{
				this.avatar = base.GetComponentInChildren<Animator>();
			}
			else
			{
				this.avatar = base.GetComponent<Animator>();
			}
		}
		this.avatar.enabled = true;
		this.ragdollSpawned = false;
		if (this.disableWhenOffscreen.Length > 0)
		{
			for (int i = 0; i < this.disableWhenOffscreen.Length; i++)
			{
				this.disableWhenOffscreen[i].SetActive(true);
			}
		}
		float num = UnityEngine.Random.Range(0.1f, 1.3f);
		if (!base.IsInvoking("updatePlayerTargets"))
		{
			base.InvokeRepeating("updatePlayerTargets", num, 2f);
		}
		if (this.doObjectAvoidance && !base.IsInvoking("closeObjectAvoidance"))
		{
			base.InvokeRepeating("closeObjectAvoidance", UnityEngine.Random.Range(num, 2f), this.avoidanceRate);
		}
		if (this.doEnable)
		{
			this.isFollowing = false;
			this.inTree = false;
			if (this.playMaker)
			{
				this.playMaker.SendEvent("START");
			}
			if (this.spawnFunctions.deer)
			{
				float num2 = UnityEngine.Random.Range(1.1f, 1.27f);
				this.animatorTr.localScale = new Vector3(num2, num2, num2);
			}
			if (this.spawnFunctions.rabbit)
			{
				float num3 = UnityEngine.Random.Range(6.6f, 7.8f);
				this.animatorTr.localScale = new Vector3(num3, num3, num3);
			}
			if (this.spawnFunctions.raccoon)
			{
				float num4 = UnityEngine.Random.Range(0.95f, 1.1f);
				this.animatorTr.localScale = new Vector3(num4, num4, num4);
			}
		}
	}

	private void updatePlayerTargets()
	{
		this.allPlayers = new List<GameObject>(this.sceneInfo.allPlayers);
		this.allPlayers.RemoveAll((GameObject o) => o == null);
		if (this.allPlayers.Count > 1)
		{
			this.allPlayers.Sort((GameObject c1, GameObject c2) => Vector3.Distance(base.transform.position, c1.transform.position).CompareTo(Vector3.Distance(base.transform.position, c2.transform.position)));
		}
	}

	public void setInTree()
	{
		this.inTree = true;
	}

	public void disableFollowingBool()
	{
		this.isFollowing = false;
	}

	protected float XZSqrMagnitude(Vector3 a, Vector3 b)
	{
		float num = b.x - a.x;
		float num2 = b.z - a.z;
		return num * num + num2 * num2;
	}

	public void goRagdoll()
	{
		if (this.ragdollSpawned)
		{
			return;
		}
		this.ragdollSpawned = true;
		Transform transform = this.ragdoll.metgoragdoll(default(Vector3));
		if (PoolManager.Pools["creatures"].IsSpawned(this.rootTr))
		{
			this.spawnFunctions.despawn();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (BoltNetwork.isRunning)
		{
			BoltNetwork.Attach(transform.gameObject);
		}
	}

	public void goBurntRagdoll()
	{
		if (this.ragdollSpawned || !this.BurntRagdoll)
		{
			return;
		}
		this.ragdollSpawned = true;
		Transform transform = ((GameObject)UnityEngine.Object.Instantiate(this.BurntRagdoll, base.transform.position, base.transform.rotation)).transform;
		transform.localScale = base.transform.localScale;
		if (PoolManager.Pools["creatures"].IsSpawned(this.rootTr))
		{
			this.spawnFunctions.despawn();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (BoltNetwork.isRunning)
		{
			BoltNetwork.Attach(transform.gameObject);
		}
	}

	public virtual void SearchPath()
	{
		if (this.target == null || !this.cansearch)
		{
			return;
		}
		this.seeker.StartPath(this.Tr.position, this.target.position, new OnPathDelegate(this.OnPathComplete));
	}

	private void getPlayerAngle()
	{
		if (this.allPlayers.Count > 0 && this.allPlayers[0] != null)
		{
			this.playerTarget = this.animatorTr.InverseTransformPoint(this.allPlayers[0].transform.position);
			this.fsmPlayerAngle.Value = Mathf.Atan2(this.playerTarget.x, this.playerTarget.z) * 57.29578f;
		}
		if (LocalPlayer.Transform)
		{
			this.fsmPlayerDist.Value = Vector3.Distance(LocalPlayer.Transform.position, this.Tr.position);
		}
	}

	public void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			if (this.path != null)
			{
				this.path.Release(this);
			}
			this.path = p;
			this.currentWaypoint = 1;
			this.path.Claim(this);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("startRandomSwimSpeed");
		base.CancelInvoke("repeatSwimSpeed");
		base.CancelInvoke("updatePlayerTargets");
		base.CancelInvoke("closeObjectAvoidanc");
		base.StopAllCoroutines();
	}

	private void Update()
	{
		if (this.target == null && this.allPlayers.Count > 0)
		{
			this.target = LocalPlayer.Transform;
		}
		if (this.allPlayers.Count > 0 && this.allPlayers[0] != null)
		{
			this.fsmPlayerDist.Value = Vector3.Distance(this.allPlayers[0].transform.position, this.Tr.position);
			this.playerTarget = this.animatorTr.InverseTransformPoint(this.allPlayers[0].transform.position);
			this.fsmPlayerAngle.Value = Mathf.Atan2(this.playerTarget.x, this.playerTarget.z) * 57.29578f;
			if (this.spawnFunctions.deer || this.spawnFunctions.crocodile || this.spawnFunctions.boar)
			{
				this.avatar.SetFloatReflected("playerAngle", this.fsmPlayerAngle.Value);
			}
			this.animalTarget = LocalPlayer.Transform.InverseTransformPoint(this.animatorTr.position);
			this.animalAngle = Mathf.Atan2(this.animalTarget.x, this.animalTarget.z) * 57.29578f;
			if (!this.skinRenderer.isVisible && (this.animalAngle < -90f || this.animalAngle > 90f) && this.fsmPlayerDist.Value > 10f && !this.fsmTreeBool.Value && !BoltNetwork.isRunning && !this.spawnFunctions.crocodile)
			{
				if (this.avatar.enabled)
				{
					this.avatar.enabled = false;
				}
				if (this.disableWhenOffscreen.Length > 0)
				{
					for (int i = 0; i < this.disableWhenOffscreen.Length; i++)
					{
						this.disableWhenOffscreen[i].SetActive(false);
					}
				}
			}
			else
			{
				if (!this.avatar.enabled)
				{
					this.avatar.enabled = true;
				}
				if (this.disableWhenOffscreen.Length > 0)
				{
					for (int j = 0; j < this.disableWhenOffscreen.Length; j++)
					{
						this.disableWhenOffscreen[j].SetActive(true);
					}
				}
			}
		}
	}

	private void startRunning()
	{
		FMODCommon.PlayOneshotNetworked(this.startleEvent, base.transform, FMODCommon.NetworkRole.Server);
	}

	private void startMovement()
	{
		if (!base.IsInvoking("SearchPath"))
		{
			base.InvokeRepeating("SearchPath", 0f, 3f);
		}
		this.doMove = true;
		this.cansearch = true;
		base.StartCoroutine("doMovement");
	}

	private void stopMovement()
	{
		base.CancelInvoke("SearchPath");
		this.cansearch = false;
		this.doMove = false;
		base.StopCoroutine("doMovement");
	}

	[DebuggerHidden]
	private IEnumerator doMovement()
	{
		animalAI.<doMovement>c__Iterator37 <doMovement>c__Iterator = new animalAI.<doMovement>c__Iterator37();
		<doMovement>c__Iterator.<>f__this = this;
		return <doMovement>c__Iterator;
	}

	private void getTargetAngle()
	{
		if (!this.target)
		{
			return;
		}
		Vector3 vector = base.transform.InverseTransformPoint(this.target.position);
		float value = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		this.fsmTargetDir.Value = value;
	}

	private void closeObjectAvoidance()
	{
		if (this.doMove && this.fsmPlayerDist.Value < 60f && !this.fsmTreeBool.Value)
		{
			if (Physics.Raycast(this.Tr.position, base.transform.forward, out this.hit2, 4f, this.layerMask2))
			{
				if (this.hit2.collider.CompareTag("Tree") || this.hit2.collider.CompareTag("jumpObject"))
				{
					this.closeTurn = true;
				}
				else
				{
					this.closeTurn = false;
				}
			}
			else
			{
				this.closeTurn = false;
			}
		}
		else
		{
			this.closeTurn = false;
		}
	}

	private float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
	{
		if (toVector == Vector3.zero)
		{
			return 0f;
		}
		float num = Vector3.Angle(fromVector, toVector);
		Vector3 lhs = Vector3.Cross(fromVector, toVector);
		num *= Mathf.Sign(Vector3.Dot(lhs, upVector));
		return num * 0.0174532924f;
	}

	[DebuggerHidden]
	public IEnumerator enableForceTarget(GameObject target)
	{
		animalAI.<enableForceTarget>c__Iterator38 <enableForceTarget>c__Iterator = new animalAI.<enableForceTarget>c__Iterator38();
		<enableForceTarget>c__Iterator.target = target;
		<enableForceTarget>c__Iterator.<$>target = target;
		<enableForceTarget>c__Iterator.<>f__this = this;
		return <enableForceTarget>c__Iterator;
	}

	public void disableForceTarget()
	{
		this.forceDir = false;
		this.forceTarget = null;
	}

	[DebuggerHidden]
	private IEnumerator restartDoMovement()
	{
		animalAI.<restartDoMovement>c__Iterator39 <restartDoMovement>c__Iterator = new animalAI.<restartDoMovement>c__Iterator39();
		<restartDoMovement>c__Iterator.<>f__this = this;
		return <restartDoMovement>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator toMove()
	{
		animalAI.<toMove>c__Iterator3A <toMove>c__Iterator3A = new animalAI.<toMove>c__Iterator3A();
		<toMove>c__Iterator3A.<>f__this = this;
		return <toMove>c__Iterator3A;
	}

	[DebuggerHidden]
	private IEnumerator toWalk()
	{
		animalAI.<toWalk>c__Iterator3B <toWalk>c__Iterator3B = new animalAI.<toWalk>c__Iterator3B();
		<toWalk>c__Iterator3B.<>f__this = this;
		return <toWalk>c__Iterator3B;
	}

	[DebuggerHidden]
	private IEnumerator toTrot()
	{
		animalAI.<toTrot>c__Iterator3C <toTrot>c__Iterator3C = new animalAI.<toTrot>c__Iterator3C();
		<toTrot>c__Iterator3C.<>f__this = this;
		return <toTrot>c__Iterator3C;
	}

	[DebuggerHidden]
	private IEnumerator toRun()
	{
		animalAI.<toRun>c__Iterator3D <toRun>c__Iterator3D = new animalAI.<toRun>c__Iterator3D();
		<toRun>c__Iterator3D.<>f__this = this;
		return <toRun>c__Iterator3D;
	}

	[DebuggerHidden]
	private IEnumerator toFollow()
	{
		animalAI.<toFollow>c__Iterator3E <toFollow>c__Iterator3E = new animalAI.<toFollow>c__Iterator3E();
		<toFollow>c__Iterator3E.<>f__this = this;
		return <toFollow>c__Iterator3E;
	}

	[DebuggerHidden]
	private IEnumerator toStop()
	{
		animalAI.<toStop>c__Iterator3F <toStop>c__Iterator3F = new animalAI.<toStop>c__Iterator3F();
		<toStop>c__Iterator3F.<>f__this = this;
		return <toStop>c__Iterator3F;
	}

	public void startRandomSwimSpeed()
	{
		if (base.enabled && !base.IsInvoking("repeatSwimSpeed"))
		{
			base.InvokeRepeating("repeatSwimSpeed", 2f, (float)UnityEngine.Random.Range(15, 35));
		}
	}

	private void repeatSwimSpeed()
	{
		if (base.enabled)
		{
			base.StartCoroutine("enableRandomSwimSpeed");
		}
	}

	[DebuggerHidden]
	public IEnumerator enableRandomSwimSpeed()
	{
		animalAI.<enableRandomSwimSpeed>c__Iterator40 <enableRandomSwimSpeed>c__Iterator = new animalAI.<enableRandomSwimSpeed>c__Iterator40();
		<enableRandomSwimSpeed>c__Iterator.<>f__this = this;
		return <enableRandomSwimSpeed>c__Iterator;
	}

	public void disableRandomSwimSpeed()
	{
		base.CancelInvoke("repeatSwimSpeed");
	}

	public void getPlayerInWater()
	{
		if (this.allPlayers[0])
		{
			this.playMaker.FsmVariables.GetFsmBool("playerInWater").Value = this.allPlayers[0].GetComponent<playerAiInfo>().isSwimming;
		}
	}
}
