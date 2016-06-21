using HutongGames.PlayMaker;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantAI : MonoBehaviour
{
	public bool leader;

	public bool maleSkinny;

	public bool femaleSkinny;

	public bool male;

	public bool female;

	public bool creepy;

	public bool creepy_male;

	public bool creepy_baby;

	public bool creepy_fat;

	public bool fireman;

	public bool fireman_dynamite;

	public bool pale;

	public bool cutScene;

	private Transform testSphere;

	protected CharacterController controller;

	protected Animator avatar;

	protected Seeker seeker;

	public Transform target;

	public Vector3 targetOffset;

	public float nextWaypointDistance;

	public int currentWaypoint;

	public float repathRate;

	public float distantRepathRate = 4f;

	private Transform tr;

	public Path path;

	private mutantScriptSetup setup;

	public float animSpeed;

	public bool awayFromPlayer;

	public Vector3 lastWalkablePos;

	public bool movingBool;

	private AnimatorStateInfo currentState;

	private AnimatorStateInfo nextState;

	public PlayMakerFSM playMaker;

	public PlayMakerFSM pmSearch;

	public PlayMakerFSM pmVision;

	public PlayMakerFSM pmTree;

	public PlayMakerFSM pmMotor;

	public PlayMakerFSM pmSleep;

	private Vector3 targetDirection;

	public float smoothedDir;

	public float absDir;

	public bool doMove;

	public bool canSearch;

	public List<GameObject> allPlayers = new List<GameObject>();

	public mutantAI_net ai_net;

	public Transform playerHead;

	public Transform playerHips;

	public GameObject headJoint;

	public float approachDist;

	public float deadZone;

	public float rotationSpeed;

	public Vector3 wantedDir;

	public Vector3 wantedPos;

	private Vector3 playerDir;

	private Vector3 localTarget;

	private Vector3 currentDir;

	public float targetAngle;

	private float targetDist;

	private Vector3 playerTarget;

	private Vector3 tempTarget;

	public float mainPlayerAngle;

	private float playerAngle;

	public float mainPlayerDist;

	private float getRotSpeed;

	public bool insideBase;

	private uint ugroundTag;

	private int groundTag;

	private GraphNode groundNode;

	private int stuckCount;

	private FsmFloat fsmPathDir;

	private FsmFloat fsmSmoothPathDir;

	private FsmFloat fsmSpeed;

	private FsmFloat fsmTargetDir;

	public FsmFloat fsmTargetDist;

	private FsmFloat fsmSearchTargetDist;

	private FsmFloat fsmIkBlend;

	private FsmGameObject fsmTarget;

	private FsmGameObject fsmSearchTarget;

	private FsmFloat fsmPlayerDist;

	private FsmFloat fsmStalkPlayerDist;

	private FsmVector3 fsmPlayerDir;

	private FsmFloat fsmPlayerAngle;

	private FsmVector3 fsmPathVector;

	private FsmVector3 fsmWantedDir;

	private FsmBool fsmMoving;

	private FsmGameObject fsmBrainTarget;

	private FsmFloat fsmBrainPlayerDist;

	private FsmFloat fsmBrainPlayerDist2D;

	private FsmFloat fsmBrainPlayerAngle;

	private FsmFloat fsmBrainTargetDist;

	private FsmFloat fsmBrainTargetDir;

	private FsmInt fsmBrainGroundTag;

	private FsmBool fsmBrainGroundWalkable;

	private FsmInt fsmPlayerGroundTag;

	private FsmGameObject fsmCurrentAttackerGo;

	public FsmFloat fsmRotateSpeed;

	private FsmFloat fsmClosestPlayerDist;

	public float playerDist;

	public float waypointAngle;

	private bool startedRun;

	private bool startedMove;

	private bool startedSearch;

	private float followDist;

	public void SyncAIConfiguration()
	{
		if (BoltNetwork.isRunning)
		{
			this.ai_net.TriggerSync(this);
		}
	}

	private void Start()
	{
		this.avatar = base.transform.GetComponentInChildren<Animator>();
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.controller = base.GetComponent<CharacterController>();
		this.seeker = base.GetComponent<Seeker>();
		this.tr = base.transform;
		this.deadZone *= 0.0174532924f;
		if (this.setup.pmMotor)
		{
			this.fsmRotateSpeed = this.setup.pmMotor.FsmVariables.GetFsmFloat("rotateSpeed");
		}
		this.fsmSmoothPathDir = this.setup.pmCombat.FsmVariables.GetFsmFloat("smoothedPathDir");
		this.fsmTargetDir = this.setup.pmCombat.FsmVariables.GetFsmFloat("targetDir");
		this.fsmTargetDist = this.setup.pmCombat.FsmVariables.GetFsmFloat("targetDist");
		if (this.setup.pmSearch)
		{
			this.fsmSearchTargetDist = this.setup.pmSearch.FsmVariables.GetFsmFloat("targetDist");
			this.fsmSearchTarget = this.setup.pmSearch.FsmVariables.GetFsmGameObject("playerGO");
		}
		if (this.creepy_male || this.creepy || this.creepy_fat || this.creepy_baby)
		{
			this.setup.pmCombat.FsmVariables.GetFsmFloat("closestPlayerDist");
		}
		this.fsmTarget = this.setup.pmCombat.FsmVariables.GetFsmGameObject("target");
		this.fsmPlayerDist = this.setup.pmCombat.FsmVariables.GetFsmFloat("playerDist");
		this.fsmClosestPlayerDist = this.setup.pmCombat.FsmVariables.GetFsmFloat("closestPlayerDist");
		if (this.setup.pmStalk)
		{
			this.fsmStalkPlayerDist = this.setup.pmStalk.FsmVariables.GetFsmFloat("playerDist");
		}
		this.fsmPlayerDir = this.setup.pmCombat.FsmVariables.GetFsmVector3("playerDir");
		this.fsmPlayerAngle = this.setup.pmCombat.FsmVariables.GetFsmFloat("playerAngle");
		if (this.setup.pmBrain)
		{
			this.fsmPathVector = this.setup.pmBrain.FsmVariables.GetFsmVector3("pathVector");
		}
		if (this.setup.pmMotor)
		{
			this.fsmWantedDir = this.setup.pmMotor.FsmVariables.GetFsmVector3("wantedDir");
			this.fsmMoving = this.setup.pmMotor.FsmVariables.GetFsmBool("movingBool");
		}
		this.fsmCurrentAttackerGo = this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentAttackerGo");
		if (this.setup.pmBrain)
		{
			this.fsmBrainTarget = this.setup.pmBrain.FsmVariables.GetFsmGameObject("target");
			this.fsmBrainPlayerDist = this.setup.pmBrain.FsmVariables.GetFsmFloat("playerDist");
			this.fsmBrainPlayerDist2D = this.setup.pmBrain.FsmVariables.GetFsmFloat("playerDist2D");
			this.fsmBrainPlayerAngle = this.setup.pmBrain.FsmVariables.GetFsmFloat("playerAngle");
			this.fsmBrainTargetDist = this.setup.pmBrain.FsmVariables.GetFsmFloat("targetDist");
			this.fsmBrainTargetDir = this.setup.pmBrain.FsmVariables.GetFsmFloat("targetDir");
			this.fsmBrainGroundTag = this.setup.pmBrain.FsmVariables.GetFsmInt("groundTag");
			this.fsmPlayerGroundTag = this.setup.pmBrain.FsmVariables.GetFsmInt("playerGroundTag");
			this.fsmBrainGroundWalkable = this.setup.pmBrain.FsmVariables.GetFsmBool("groundWalkable");
		}
		this.target = LocalPlayer.Transform;
		this.fsmTarget.Value = LocalPlayer.GameObject;
		if (this.setup.pmBrain)
		{
			this.fsmBrainTarget.Value = this.target.gameObject;
		}
		this.avatar.speed = 1.05f;
		this.animSpeed = 1.05f;
		this.getRotSpeed = this.rotationSpeed;
		this.allPlayers = new List<GameObject>(this.setup.sceneInfo.allPlayers);
	}

	private void OnSpawned()
	{
		float time = UnityEngine.Random.Range(0.1f, 3f);
		if (!base.IsInvoking("checkTags"))
		{
			base.InvokeRepeating("checkTags", time, 2f);
		}
		if (!base.IsInvoking("checkCloseTargets"))
		{
			base.InvokeRepeating("checkCloseTargets", time, 1.5f);
		}
		if (!base.IsInvoking("updatePlayerTargets"))
		{
			base.InvokeRepeating("updatePlayerTargets", time, 1.5f);
		}
		base.Invoke("SyncAIConfiguration", 0.5f);
		this.resetInsideBase();
		if (this.avatar)
		{
			this.avatar.SetInteger("sharpTurnDir", 0);
		}
	}

	private void OnDespawned()
	{
		base.CancelInvoke("checkTags");
		base.CancelInvoke("checkCloseTargets");
		base.CancelInvoke("updatePlayerTargets");
		if (this.setup.sceneInfo.closeEnemies.Contains(this.tr.parent.gameObject))
		{
			this.setup.sceneInfo.closeEnemies.Remove(this.tr.parent.gameObject);
		}
	}

	private void updatePlayerTargets()
	{
		this.allPlayers = new List<GameObject>(this.setup.sceneInfo.allPlayers);
		this.allPlayers.RemoveAll((GameObject o) => o == null);
		if (this.allPlayers.Count > 1)
		{
			this.allPlayers.Sort((GameObject c1, GameObject c2) => (this.tr.position - c1.transform.position).sqrMagnitude.CompareTo((this.tr.position - c2.transform.position).sqrMagnitude));
		}
	}

	private void checkCloseTargets()
	{
		if (!this.creepy && !this.creepy_baby && !this.creepy_male && !this.creepy_fat)
		{
			if (this.mainPlayerDist < 65f && this.tr.parent.gameObject.activeSelf)
			{
				if (!this.setup.sceneInfo.closeEnemies.Contains(this.tr.parent.gameObject))
				{
					this.setup.sceneInfo.closeEnemies.Add(this.tr.parent.gameObject);
				}
			}
			else if (this.setup.sceneInfo.closeEnemies.Contains(this.tr.parent.gameObject))
			{
				this.setup.sceneInfo.closeEnemies.Remove(this.tr.parent.gameObject);
			}
			if (!BoltNetwork.isRunning)
			{
				if (this.mainPlayerDist > 75f)
				{
					this.awayFromPlayer = true;
				}
				else
				{
					this.awayFromPlayer = false;
				}
			}
			else
			{
				this.awayFromPlayer = false;
			}
		}
	}

	private void checkTags()
	{
		if (!base.transform.parent.gameObject.activeSelf)
		{
			return;
		}
		if (this.setup.search.fsmInCave.Value)
		{
			if (this.setup.pmBrain)
			{
				this.fsmBrainGroundWalkable.Value = true;
			}
			return;
		}
		if (!AstarPath.active)
		{
			return;
		}
		this.groundNode = AstarPath.active.GetNearest(this.tr.position).node;
		this.ugroundTag = this.groundNode.Tag;
		this.groundTag = (int)this.ugroundTag;
		if (this.groundNode.Walkable)
		{
			this.lastWalkablePos = new Vector3((float)(this.groundNode.position[0] / 1000), (float)(this.groundNode.position[1] / 1000), (float)(this.groundNode.position[2] / 1000));
		}
		if (this.setup.pmBrain)
		{
			this.fsmBrainGroundTag.Value = this.groundTag;
			this.fsmBrainGroundWalkable.Value = this.groundNode.Walkable;
		}
		if (!this.groundNode.Walkable)
		{
			if (this.startedMove || this.startedRun)
			{
				this.avatar.SetFloatReflected("Speed", 0.3f);
			}
			this.stuckCount++;
			if (this.stuckCount > 10 && this.allPlayers.Count > 0 && this.allPlayers[0] && this.mainPlayerDist > 130f)
			{
				Vector2 vector = this.randomCircle(40f);
				Vector3 position = new Vector3(base.transform.position.x + vector.x, base.transform.position.y, base.transform.position.z + vector.y);
				GraphNode node = AstarPath.active.GetNearest(position).node;
				if (node.Walkable)
				{
					base.transform.root.position = new Vector3((float)(node.position[0] / 1000), (float)(node.position[1] / 1000), (float)(node.position[2] / 1000));
					this.stuckCount = 0;
				}
			}
		}
		else
		{
			this.stuckCount = 0;
		}
	}

	protected float XZSqrMagnitude(Vector3 a, Vector3 b)
	{
		float num = b.x - a.x;
		float num2 = b.z - a.z;
		return num * num + num2 * num2;
	}

	public virtual void SearchPath()
	{
		if (this.canSearch)
		{
			if (this.target == null)
			{
				return;
			}
			this.seeker.StartPath(this.tr.position, this.target.position + this.targetOffset, new OnPathDelegate(this.OnPathComplete));
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
			this.path.Claim(this);
			if (this.groundNode != null && this.groundNode.Walkable && !this.setup.search.fsmInCave.Value && !this.creepy && !this.creepy_baby && !this.creepy_male && !this.creepy_fat)
			{
				GraphNode node = AstarPath.active.GetNearest(this.tr.position, NNConstraint.Default).node;
				if (node != null)
				{
					uint area = node.Area;
					NNConstraint nNConstraint = new NNConstraint();
					nNConstraint.constrainArea = true;
					int area2 = (int)area;
					nNConstraint.area = area2;
					GraphNode node2 = AstarPath.active.GetNearest(this.target.position, nNConstraint).node;
					Vector3 vector = new Vector3((float)(node2.position[0] / 1000), (float)(node2.position[1] / 1000), (float)(node2.position[2] / 1000));
					if (Vector3.Distance(vector, this.target.position) > 10f)
					{
						this.setup.search.updateCurrentWaypoint(vector);
						this.setup.search.setToWaypoint();
						if (this.target.gameObject.CompareTag("Player") || this.target.gameObject.CompareTag("PlayerNet"))
						{
							if (this.setup.pmBrain)
							{
								this.setup.pmBrain.SendEvent("toSetPassive");
							}
						}
						else
						{
							if (this.setup.pmBrain)
							{
								this.setup.pmBrain.SendEvent("toSetAggressive");
							}
							if (this.setup.pmCombat)
							{
								this.setup.pmCombat.FsmVariables.GetFsmBool("fsmInsideBase").Value = true;
							}
							this.insideBase = true;
							base.Invoke("resetInsideBase", 25f);
						}
						if (this.setup.pmSearch)
						{
							this.setup.pmSearch.SendEvent("noValidPath");
						}
						if (this.setup.pmStalk)
						{
							this.setup.pmStalk.SendEvent("noValidPath");
						}
						if (this.setup.pmCombat)
						{
							this.setup.pmCombat.SendEvent("noValidPath");
						}
						if (this.setup.pmEncounter)
						{
							this.setup.pmEncounter.SendEvent("noValidPath");
						}
					}
				}
			}
			if (this.setup.pmSearch)
			{
				this.setup.pmSearch.SendEvent("validPath");
			}
			if (this.setup.pmStalk)
			{
				this.setup.pmStalk.SendEvent("validPath");
			}
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.SendEvent("validPath");
			}
			if (this.setup.pmEncounter)
			{
				this.setup.pmEncounter.SendEvent("validPath");
			}
			this.currentWaypoint = 1;
			return;
		}
		if (this.setup.pmSearch)
		{
			this.setup.pmSearch.SendEvent("noValidPath");
		}
		if (this.setup.pmStalk)
		{
			this.setup.pmStalk.SendEvent("noValidPath");
		}
		if (this.setup.pmCombat)
		{
			this.setup.pmCombat.SendEvent("noValidPath");
		}
		if (this.setup.pmEncounter)
		{
			this.setup.pmEncounter.SendEvent("noValidPath");
		}
	}

	private void resetInsideBase()
	{
		this.insideBase = false;
		if (this.setup && this.setup.pmCombat)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("fsmInsideBase").Value = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator setRepathRate(float rate)
	{
		mutantAI.<setRepathRate>c__Iterator62 <setRepathRate>c__Iterator = new mutantAI.<setRepathRate>c__Iterator62();
		<setRepathRate>c__Iterator.rate = rate;
		<setRepathRate>c__Iterator.<$>rate = rate;
		<setRepathRate>c__Iterator.<>f__this = this;
		return <setRepathRate>c__Iterator;
	}

	private void startMovement()
	{
		this.doMove = true;
		this.canSearch = true;
		if (!base.IsInvoking("SearchPath"))
		{
			if (this.mainPlayerDist > 80f)
			{
				base.InvokeRepeating("SearchPath", 0f, this.distantRepathRate);
			}
			else
			{
				base.InvokeRepeating("SearchPath", 0f, this.repathRate);
			}
		}
		base.StartCoroutine("doMovement");
	}

	private void stopMovement()
	{
		this.doMove = false;
		this.canSearch = false;
		base.StopCoroutine("doMovement");
		base.CancelInvoke("SearchPath");
	}

	private void enablePathSearch()
	{
		this.canSearch = true;
		this.SearchPath();
	}

	private void Update()
	{
		if (this.allPlayers.Count == 0)
		{
			return;
		}
		if (this.target == null && this.allPlayers[0] != null)
		{
			this.target = this.allPlayers[0].transform;
		}
		if (this.target)
		{
			this.fsmTarget.Value = this.target.gameObject;
			if (this.setup.pmSearch)
			{
				this.fsmSearchTarget.Value = this.target.gameObject;
			}
			if (this.setup.pmCombat && this.setup.search.currentTarget)
			{
				this.fsmCurrentAttackerGo.Value = this.setup.search.currentTarget;
			}
		}
		if (LocalPlayer.Transform != null)
		{
			LocalPlayer.Transform.position.y = this.tr.position.y;
			if (this.allPlayers[0])
			{
				if (this.allPlayers[0] != null)
				{
					this.mainPlayerDist = Vector3.Distance(this.tr.position, this.allPlayers[0].transform.position);
					this.tempTarget = base.transform.InverseTransformPoint(this.allPlayers[0].transform.position);
					this.mainPlayerAngle = Mathf.Atan2(this.tempTarget.x, this.tempTarget.z) * 57.29578f;
				}
			}
			else
			{
				this.mainPlayerDist = Vector3.Distance(this.tr.position, LocalPlayer.Transform.position);
				this.tempTarget = base.transform.InverseTransformPoint(LocalPlayer.Transform.position);
				this.mainPlayerAngle = Mathf.Atan2(this.tempTarget.x, this.tempTarget.z) * 57.29578f;
			}
		}
		if (this.setup.search.currentTarget)
		{
			this.playerDist = Vector3.Distance(this.setup.search.currentTarget.transform.position, this.tr.position);
			this.playerTarget = base.transform.InverseTransformPoint(this.setup.search.currentTarget.transform.position);
			if (this.avatar.enabled)
			{
				this.playerAngle = Mathf.Atan2(this.playerTarget.x, this.playerTarget.z) * 57.29578f;
				this.fsmPlayerAngle.Value = this.playerAngle;
			}
			else
			{
				this.playerAngle = 0f;
				this.fsmPlayerAngle.Value = this.playerAngle;
			}
			if (this.avatar.enabled)
			{
				if (this.creepy_fat)
				{
					this.avatar.SetFloatReflected("playerAngle", this.mainPlayerAngle);
				}
				else
				{
					this.avatar.SetFloatReflected("playerAngle", this.playerAngle);
				}
			}
		}
		if (this.creepy || this.creepy_male || this.creepy_baby || this.creepy_fat)
		{
			this.fsmPlayerDist.Value = this.mainPlayerDist;
		}
		else
		{
			this.fsmClosestPlayerDist.Value = this.mainPlayerDist;
			this.fsmPlayerDist.Value = this.playerDist;
		}
		if (this.setup.pmStalk)
		{
			this.fsmStalkPlayerDist.Value = this.playerDist;
		}
		if (this.setup.search.currentTarget && !this.awayFromPlayer)
		{
			Collider component = this.setup.search.currentTarget.GetComponent<Collider>();
			if (this.setup.headJoint)
			{
				if (component)
				{
					this.playerDir = component.bounds.center - this.setup.headJoint.transform.position;
				}
				else
				{
					this.playerDir = this.setup.search.currentTarget.transform.position - this.setup.headJoint.transform.position;
				}
				this.fsmPlayerDir.Value = this.playerDir;
			}
			else if (component)
			{
				this.playerDir = this.setup.search.currentTarget.GetComponent<Collider>().bounds.center - this.tr.position;
				this.fsmPlayerDir.Value = this.playerDir;
			}
		}
		if (this.target && !this.awayFromPlayer)
		{
			this.localTarget = base.transform.InverseTransformPoint(this.target.position + this.targetOffset);
			this.targetAngle = Mathf.Atan2(this.localTarget.x, this.localTarget.z) * 57.29578f;
			this.fsmTargetDir.Value = this.targetAngle;
			if (this.avatar.enabled)
			{
				this.avatar.SetFloatReflected("TargetDir", this.targetAngle);
			}
		}
		if (this.target)
		{
			this.targetDist = Vector3.Distance(this.target.position + this.targetOffset, this.tr.position);
		}
		this.fsmTargetDist.Value = this.targetDist;
		if (this.setup.pmSearch)
		{
			this.fsmSearchTargetDist.Value = this.targetDist;
		}
		if (this.setup.pmBrain)
		{
			this.fsmBrainPlayerAngle.Value = this.playerAngle;
			this.fsmBrainPlayerDist.Value = this.mainPlayerDist;
			this.fsmBrainPlayerDist2D.Value = this.mainPlayerDist;
			this.fsmBrainTargetDist.Value = this.targetDist;
			this.fsmBrainTargetDir.Value = this.targetAngle;
		}
	}

	[DebuggerHidden]
	private IEnumerator doMovement()
	{
		mutantAI.<doMovement>c__Iterator63 <doMovement>c__Iterator = new mutantAI.<doMovement>c__Iterator63();
		<doMovement>c__Iterator.<>f__this = this;
		return <doMovement>c__Iterator;
	}

	private void disableController()
	{
		this.controller.GetComponent<Collider>().enabled = false;
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

	private void restartAI()
	{
		if (!this.creepy && !this.creepy_male && !this.cutScene && !this.creepy_fat)
		{
			string activeStateName = this.setup.pmCombat.ActiveStateName;
			string activeStateName2 = this.setup.pmEncounter.ActiveStateName;
			string activeStateName3 = this.setup.pmSearch.ActiveStateName;
			string activeStateName4 = this.setup.pmSleep.ActiveStateName;
			if (!(activeStateName == "disabled") || !(activeStateName2 == "disabled") || !(activeStateName3 == "disabled") || activeStateName4 == "disabled")
			{
			}
		}
	}

	public void cancelDefaultActions()
	{
		if (!this.avatar.enabled)
		{
			return;
		}
		this.avatar.SetBoolReflected("onRockBOOL", false);
		this.avatar.SetBoolReflected("dodgeBOOL", false);
		this.avatar.SetBoolReflected("fearBOOL", false);
		this.avatar.SetBoolReflected("backAwayBOOL", false);
		this.avatar.SetBoolReflected("crouchBOOL", false);
		this.avatar.SetBoolReflected("sideWalkBOOL", false);
		this.avatar.SetBoolReflected("staggerBOOL", false);
		this.avatar.SetBoolReflected("freakoutBOOL", false);
	}

	public void resetCombatParams()
	{
		if (!this.avatar.enabled)
		{
			return;
		}
		this.avatar.SetBoolReflected("onRockBOOL", false);
		this.avatar.SetBoolReflected("dodgeBOOL", false);
		this.avatar.SetBoolReflected("idleWaryBOOL", false);
		this.avatar.SetBoolReflected("fearBOOL", false);
		this.avatar.SetBoolReflected("backAwayBOOL", false);
		this.avatar.SetBoolReflected("crouchBOOL", false);
		this.avatar.SetBoolReflected("sideWalkBOOL", false);
		this.avatar.SetBoolReflected("staggerBOOL", false);
		this.avatar.SetBoolReflected("freakoutBOOL", false);
		this.avatar.SetBoolReflected("jumpBlockBool", false);
		this.avatar.SetBoolReflected("recoverBool", false);
		this.avatar.SetBoolReflected("rescueBool1", false);
		this.avatar.SetBoolReflected("walkBOOL", false);
		this.avatar.SetBoolReflected("sideWalkBOOL", false);
		this.avatar.SetBoolReflected("actionBOOL1", false);
		this.avatar.SetBoolReflected("damageBOOL", false);
		this.avatar.SetBoolReflected("damageBehindBOOL", false);
		this.avatar.SetBoolReflected("screamBOOL", false);
		this.avatar.SetBoolReflected("runAwayBOOL", false);
		this.avatar.SetBoolReflected("crouchBOOL", false);
		this.avatar.SetBoolReflected("attackBOOL", false);
		this.avatar.SetBoolReflected("attackRightBOOL", false);
		this.avatar.SetBoolReflected("attackLeftBOOL", false);
		this.avatar.SetBoolReflected("attackJumpBOOL", false);
		this.avatar.SetBoolReflected("attackMovingBOOL", false);
		this.avatar.SetBoolReflected("turnLeftBOOL", false);
		this.avatar.SetBoolReflected("turnRightBOOL", false);
		this.avatar.SetBoolReflected("turnAroundBOOL", false);
		this.avatar.SetBoolReflected("sideStepBOOL", false);
		this.avatar.SetBoolReflected("dodgeBOOL", false);
		this.avatar.SetBoolReflected("treeJumpBOOL", false);
		this.avatar.SetBoolReflected("attackJumpBOOL", false);
		this.avatar.SetBoolReflected("backAwayBOOL", false);
		this.avatar.SetBoolReflected("sleepBOOL", false);
		this.avatar.SetBoolReflected("soundBool1", false);
	}

	public void forceTreeDown()
	{
		if (!this.avatar.enabled)
		{
			return;
		}
		if (this.avatar.GetBool("treeBOOL"))
		{
			this.avatar.SetIntegerReflected("randInt1", UnityEngine.Random.Range(0, 2));
			this.avatar.SetBoolReflected("treeBOOL", false);
			this.avatar.SetBoolReflected("treeJumpBOOL", false);
			this.avatar.SetBoolReflected("attackJumpBOOL", false);
			this.setup.pmCombat.FsmVariables.GetFsmBool("inTreeBool").Value = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator toWalk()
	{
		mutantAI.<toWalk>c__Iterator64 <toWalk>c__Iterator = new mutantAI.<toWalk>c__Iterator64();
		<toWalk>c__Iterator.<>f__this = this;
		return <toWalk>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator toRun()
	{
		mutantAI.<toRun>c__Iterator65 <toRun>c__Iterator = new mutantAI.<toRun>c__Iterator65();
		<toRun>c__Iterator.<>f__this = this;
		return <toRun>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator toSearch()
	{
		mutantAI.<toSearch>c__Iterator66 <toSearch>c__Iterator = new mutantAI.<toSearch>c__Iterator66();
		<toSearch>c__Iterator.<>f__this = this;
		return <toSearch>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator toStop()
	{
		mutantAI.<toStop>c__Iterator67 <toStop>c__Iterator = new mutantAI.<toStop>c__Iterator67();
		<toStop>c__Iterator.<>f__this = this;
		return <toStop>c__Iterator;
	}

	private Vector2 randomCircle(float radius)
	{
		Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
		return normalized * radius;
	}
}
