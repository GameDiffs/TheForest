using FMOD.Studio;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class lb_Bird : MonoBehaviour
{
	private enum birdBehaviors
	{
		sing,
		preen,
		ruffle,
		peck,
		hopForward,
		hopBackward,
		hopLeft,
		hopRight
	}

	[Header("FMOD")]
	public string SongEvent;

	public string FlyAwayEvent;

	public string HitEvent;

	public string DieEvent;

	public string StartleEvent;

	[Space(10f)]
	public bool typeSparrow;

	public bool typeSeagull;

	public bool typeCrow;

	public bool typeRedBird;

	public bool typeBlueBird;

	public float rotateSpeed;

	public float flyingSpeed;

	private float initFlyingSpeed;

	private float initRotateSpeed;

	public GameObject MyFeather;

	public int health;

	private bool songBreak;

	public bool birdVisible;

	public GameObject currentPerchTarget;

	private Animator anim;

	public lb_BirdController controller;

	private clsragdollify ragDoll;

	public SkinnedMeshRenderer skin;

	private Transform tr;

	private bool paused;

	public bool idle = true;

	public bool flying;

	public bool landing;

	public bool perched;

	public bool onGround = true;

	public bool takeoff;

	public bool fleeing;

	public bool flyingToPerch;

	public float distanceToTarget;

	private float agitationLevel = 0.5f;

	private float originalAnimSpeed = 1f;

	public float repathRate;

	public float approachDist;

	public float deadZone;

	private Vector3 getDir;

	public Vector3 wantedDir;

	public bool aimAtWaypoint;

	public Transform pathTarget;

	public Seeker seeker;

	public Path path;

	public float speed = 5f;

	public float nextWaypointDistance = 3f;

	public int currentWaypoint;

	public bool isFollowing;

	public bool inTree;

	public bool cansearch;

	private List<FMOD.Studio.EventInstance> eventInstances = new List<FMOD.Studio.EventInstance>();

	private FMOD.Studio.EventInstance songInstance;

	private int idleAnimationHash;

	private int singAnimationHash;

	private int ruffleAnimationHash;

	private int preenAnimationHash;

	private int peckAnimationHash;

	private int hopForwardAnimationHash;

	private int hopBackwardAnimationHash;

	private int hopLeftAnimationHash;

	private int hopRightAnimationHash;

	private int worriedAnimationHash;

	private int landingAnimationHash;

	private int flyTagHash;

	private int hopIntHash;

	private int flyingBoolHash;

	private int peckBoolHash;

	private int ruffleBoolHash;

	private int preenBoolHash;

	private int landingBoolHash;

	private int singTriggerHash;

	private int flyingDirectionHash;

	private int FeatherDice;

	private Quaternion lastRotation;

	private Quaternion desiredRotation;

	private Quaternion startingRotation;

	private Quaternion finalRotation;

	private Vector3 vectorDirectionToTarget;

	private Vector3 desiredDirection;

	private Vector3 farAwayTarget;

	private Vector3 diff;

	private Vector3 lastDir;

	private Vector3 currentDir;

	public Vector3 target;

	private float hitPos;

	private Vector2 tempPos;

	private float absDir;

	private float smoothedDir;

	private int layerMask1;

	private RaycastHit hit;

	private bool initBool;

	private bool groundCoolDown;

	private bool flyAwayCoolDown;

	public Vector3 targetWaypoint;

	private string[] AllEventPaths()
	{
		return new string[]
		{
			this.SongEvent,
			this.FlyAwayEvent,
			this.HitEvent,
			this.DieEvent,
			this.StartleEvent
		};
	}

	private void Awake()
	{
		this.seeker = base.transform.GetComponent<Seeker>();
		this.anim = base.gameObject.GetComponent<Animator>();
	}

	private void Start()
	{
		this.skin = base.transform.GetComponentInChildren<SkinnedMeshRenderer>();
		this.tr = base.transform;
		this.ragDoll = base.GetComponent<clsragdollify>();
		this.initFlyingSpeed = this.flyingSpeed;
		this.initRotateSpeed = this.rotateSpeed;
		this.layerMask1 = 67108864;
		this.ragDoll.bird = true;
		base.Invoke("initThis", 0.5f);
		base.InvokeRepeating("CleanupEventInstances", 1f, 1f);
	}

	private void initThis()
	{
		this.initBool = true;
	}

	private void OnDeserialized()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnEnable()
	{
		this.tr = base.transform;
		this.anim = base.gameObject.GetComponent<Animator>();
		this.ragDoll = base.GetComponent<clsragdollify>();
		this.layerMask1 = 67108864;
		this.idleAnimationHash = Animator.StringToHash("Base Layer.Idle");
		this.flyTagHash = Animator.StringToHash("flying");
		this.hopIntHash = Animator.StringToHash("hop");
		this.flyingBoolHash = Animator.StringToHash("flying");
		this.peckBoolHash = Animator.StringToHash("peck");
		this.ruffleBoolHash = Animator.StringToHash("ruffle");
		this.preenBoolHash = Animator.StringToHash("preen");
		this.landingBoolHash = Animator.StringToHash("landing");
		this.singTriggerHash = Animator.StringToHash("sing");
		this.flyingDirectionHash = Animator.StringToHash("flyingDirectionX");
		this.anim.SetFloatReflected("IdleAgitated", this.agitationLevel);
		if (this.anim.enabled)
		{
			this.anim.SetBool(this.flyingBoolHash, true);
			this.anim.SetBool(this.landingBoolHash, false);
		}
		if (!base.IsInvoking("groundHeightCheck"))
		{
			FMODCommon.PreloadEvents(this.AllEventPaths());
		}
		this.fleeing = false;
	}

	private void OnDisable()
	{
		if (!this.initBool)
		{
			return;
		}
		base.CancelInvoke("groundHeightCheck");
		this.fleeing = false;
		base.StopAllCoroutines();
		this.StopEvents();
		FMODCommon.UnloadEvents(this.AllEventPaths());
		this.flyAwayCoolDown = false;
		this.flying = false;
		this.landing = false;
		this.onGround = false;
		this.idle = false;
	}

	private void groundHeightCheck()
	{
		this.hitPos = Terrain.activeTerrain.SampleHeight(this.tr.position) + Terrain.activeTerrain.transform.position.y;
		if (this.tr.position.y - this.hitPos < 2f && !this.landing && !this.onGround && this.distanceToTarget > 10f && !this.groundCoolDown)
		{
			this.Flee();
			this.groundCoolDown = true;
			base.Invoke("resetGroundCoolDown", 3f);
		}
	}

	private void resetGroundCoolDown()
	{
		this.groundCoolDown = false;
	}

	private void PauseBird()
	{
		this.originalAnimSpeed = this.anim.speed;
		this.anim.speed = 0f;
		this.StopEvents();
		this.paused = true;
	}

	private void UnPauseBird()
	{
		this.anim.speed = this.originalAnimSpeed;
		this.paused = false;
	}

	[DebuggerHidden]
	private IEnumerator FlyToTarget(Vector3 getTarget)
	{
		lb_Bird.<FlyToTarget>c__Iterator210 <FlyToTarget>c__Iterator = new lb_Bird.<FlyToTarget>c__Iterator210();
		<FlyToTarget>c__Iterator.getTarget = getTarget;
		<FlyToTarget>c__Iterator.<$>getTarget = getTarget;
		<FlyToTarget>c__Iterator.<>f__this = this;
		return <FlyToTarget>c__Iterator;
	}

	private void OnGroundBehaviors()
	{
		this.idle = (this.anim.GetCurrentAnimatorStateInfo(0).nameHash == this.idleAnimationHash);
		if (!base.GetComponent<Rigidbody>().isKinematic)
		{
			base.GetComponent<Rigidbody>().isKinematic = true;
		}
		if (this.currentPerchTarget && !this.currentPerchTarget.activeInHierarchy)
		{
			this.FlyAway();
		}
		if (this.idle)
		{
			if (!this.songBreak)
			{
				this.PlaySong();
			}
			if ((double)UnityEngine.Random.value < (double)Time.deltaTime * 0.33)
			{
				float value = UnityEngine.Random.value;
				if ((double)value < 0.3)
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.sing);
				}
				else if ((double)value < 0.5)
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.peck);
				}
				else if ((double)value < 0.6)
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.preen);
				}
				else if (!this.perched && (double)value < 0.7)
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.ruffle);
				}
				else if (!this.perched && (double)value < 0.85)
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.hopForward);
				}
				else if (!this.perched && (double)value < 0.9)
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.hopLeft);
				}
				else if (!this.perched && (double)value < 0.95)
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.hopRight);
				}
				else if (!this.perched && value <= 1f)
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.hopBackward);
				}
				else
				{
					this.DisplayBehavior(lb_Bird.birdBehaviors.sing);
				}
				this.anim.SetFloatReflected("IdleAgitated", UnityEngine.Random.value);
			}
			if ((double)UnityEngine.Random.value < (double)Time.deltaTime * 0.1)
			{
				this.FlyAway();
			}
		}
	}

	private void DisplayBehavior(lb_Bird.birdBehaviors behavior)
	{
		this.idle = false;
		switch (behavior)
		{
		case lb_Bird.birdBehaviors.sing:
			this.ResetHopInt();
			this.anim.SetTrigger(this.singTriggerHash);
			break;
		case lb_Bird.birdBehaviors.preen:
			this.ResetHopInt();
			this.anim.SetTrigger(this.preenBoolHash);
			break;
		case lb_Bird.birdBehaviors.ruffle:
			this.ResetHopInt();
			this.anim.SetTrigger(this.ruffleBoolHash);
			break;
		case lb_Bird.birdBehaviors.peck:
			this.ResetHopInt();
			this.anim.SetTrigger(this.peckBoolHash);
			break;
		case lb_Bird.birdBehaviors.hopForward:
			this.anim.SetInteger(this.hopIntHash, 1);
			base.Invoke("ResetHopInt", UnityEngine.Random.Range(0.5f, 1f));
			break;
		case lb_Bird.birdBehaviors.hopBackward:
			this.anim.SetInteger(this.hopIntHash, -1);
			base.Invoke("ResetHopInt", UnityEngine.Random.Range(0.5f, 1f));
			break;
		case lb_Bird.birdBehaviors.hopLeft:
			this.anim.SetInteger(this.hopIntHash, -2);
			base.Invoke("ResetHopInt", UnityEngine.Random.Range(0.5f, 1f));
			break;
		case lb_Bird.birdBehaviors.hopRight:
			this.anim.SetInteger(this.hopIntHash, 2);
			base.Invoke("ResetHopInt", UnityEngine.Random.Range(0.5f, 1f));
			break;
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "lb_bird")
		{
			this.FlyAway();
		}
		if ((col.CompareTag("soundAlert") || col.CompareTag("enemyRoot")) && Vector3.Distance(col.transform.position, base.transform.position) < 11f)
		{
			if (!this.flying)
			{
				this.PlayEvent(this.StartleEvent);
			}
			this.FlyAway();
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (!this.onGround || col.tag == "lb_groundTarget" || col.tag == "lb_perchTarget")
		{
		}
	}

	public void Burn()
	{
		this.DieFire();
	}

	private void FlyAway()
	{
		if (!this.flyAwayCoolDown && !this.flying)
		{
			this.sendDisablePerchTarget();
			base.StopCoroutine("FlyToTarget");
			this.PlayEvent(this.FlyAwayEvent);
			this.takeoff = true;
			base.Invoke("resetTakeoff", 4f);
			this.anim.SetBool(this.landingBoolHash, false);
			base.StopCoroutine("FleeHigh");
			base.StartCoroutine("FleeHigh");
			this.flyAwayCoolDown = true;
			this.FeatherDice = UnityEngine.Random.Range(0, 10);
			if (this.FeatherDice == 1)
			{
				this.SpawnAFeather();
			}
		}
	}

	private void setNewFlyTarget(Vector3 getPos)
	{
		if (!this.flying)
		{
			base.StopCoroutine("FlyToTarget");
			base.StartCoroutine("FlyToTarget", getPos);
		}
		else if (!this.landing && this.flying)
		{
			this.target = getPos;
			this.generatePathToTarget();
		}
	}

	private void findNewTarget()
	{
		if (!this.controller)
		{
			return;
		}
		this.controller.SendMessage("BirdFindTarget", base.gameObject);
	}

	private void findNewLandingTarget()
	{
		if (!this.controller)
		{
			return;
		}
		this.controller.SendMessage("BirdFindTarget", base.gameObject);
	}

	private void findNewRandomTarget()
	{
		Vector2 vector = this.Circle2(75f);
		Vector3 vector2 = new Vector3(this.tr.position.x + vector.x, this.tr.position.y + (float)UnityEngine.Random.Range(-5, 5), vector.y + this.tr.position.z);
		if (!this.landing && this.flying)
		{
			this.target = vector2;
			this.generatePathToTarget();
			this.flyingToPerch = false;
		}
	}

	private void findNewTargetIgnorePathing()
	{
	}

	private void resetIgnorePathing()
	{
	}

	private void setFlyingToPerch()
	{
		this.flyingToPerch = true;
	}

	public void flyToRandomTarget(Vector3 randomPos)
	{
		if (!this.landing && this.flying)
		{
			this.target = randomPos;
			this.generatePathToTarget();
		}
		this.flyingToPerch = false;
	}

	private void retryFlyToTarget()
	{
		base.Invoke("findNewTarget", (float)UnityEngine.Random.Range(6, 9));
		base.CancelInvoke("setRandomTarget");
	}

	private void resetTakeoff()
	{
		this.flyAwayCoolDown = false;
		this.takeoff = false;
	}

	private void SpawnAFeather()
	{
		UnityEngine.Object.Instantiate(this.MyFeather, base.transform.position, base.transform.rotation);
	}

	private FMOD.Studio.EventInstance PlayEvent(string path)
	{
		FMOD.Studio.EventInstance eventInstance = FMODCommon.PlayOneshot(path, base.transform);
		if (eventInstance != null)
		{
			this.eventInstances.Add(eventInstance);
		}
		return eventInstance;
	}

	private void StopEvents()
	{
		foreach (FMOD.Studio.EventInstance current in this.eventInstances)
		{
			FMODCommon.StopEvent(current);
		}
		this.eventInstances.Clear();
		this.StopSong();
	}

	private void StopSong()
	{
		FMODCommon.StopEvent(this.songInstance);
		this.songInstance = null;
	}

	private void CleanupEventInstances()
	{
		int i = 0;
		while (i < this.eventInstances.Count)
		{
			if (!this.eventInstances[i].isValid())
			{
				this.eventInstances.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	private void Hit(int damage)
	{
		this.health -= damage;
		if (this.health < 1)
		{
			this.die();
		}
		else
		{
			this.PlayEvent(this.HitEvent);
		}
	}

	private void Explosion(float dist)
	{
		this.die();
	}

	private void die()
	{
		if (!this.controller)
		{
			return;
		}
		this.sendDisablePerchTarget();
		if (this.controller.initIdealBirds > 0)
		{
			this.controller.initIdealBirds--;
		}
		if (this.controller.initMaxBirds > 0)
		{
			this.controller.initMaxBirds--;
		}
		base.StopAllCoroutines();
		FMODCommon.PlayOneshot(this.DieEvent, base.transform);
		this.ragDoll.metgoragdoll(default(Vector3));
		this.controller.Unspawn(base.gameObject);
	}

	private void DieFire()
	{
		if (!this.controller)
		{
			return;
		}
		this.sendDisablePerchTarget();
		if (this.controller.initIdealBirds > 0)
		{
			this.controller.initIdealBirds--;
		}
		if (this.controller.initMaxBirds > 0)
		{
			this.controller.initMaxBirds--;
		}
		base.StopAllCoroutines();
		FMODCommon.PlayOneshot(this.DieEvent, base.transform);
		this.ragDoll.burning = true;
		this.ragDoll.metgoragdoll(default(Vector3));
		this.controller.Unspawn(base.gameObject);
	}

	private void Flee()
	{
		this.sendDisablePerchTarget();
		this.StopSong();
		this.tempPos = this.Circle2(65f);
		this.farAwayTarget = new Vector3(this.tr.position.x + this.tempPos.x, this.tr.position.y + (float)UnityEngine.Random.Range(20, 35), this.tempPos.y + this.tr.position.z);
		if (!this.flying)
		{
			base.StopCoroutine("FlyToTarget");
			base.StartCoroutine("FlyToTarget", this.farAwayTarget);
		}
		else if (!this.landing && this.flying)
		{
			this.target = this.farAwayTarget;
			this.generatePathToTarget();
		}
		base.Invoke("findNewTarget", UnityEngine.Random.Range(5f, 8f));
		this.flyingToPerch = false;
	}

	[DebuggerHidden]
	private IEnumerator FleeHigh()
	{
		lb_Bird.<FleeHigh>c__Iterator211 <FleeHigh>c__Iterator = new lb_Bird.<FleeHigh>c__Iterator211();
		<FleeHigh>c__Iterator.<>f__this = this;
		return <FleeHigh>c__Iterator;
	}

	public void FleeBehind()
	{
		this.sendDisablePerchTarget();
		this.StopSong();
		this.farAwayTarget = this.tr.position + this.tr.forward * -30f + (this.tr.right * (float)UnityEngine.Random.Range(-50, 50) + new Vector3(0f, (float)UnityEngine.Random.Range(0, 10), 0f));
		if (!this.flying)
		{
			base.StopCoroutine("FlyToTarget");
			base.StartCoroutine("FlyToTarget", this.farAwayTarget);
		}
		else if (!this.landing && this.flying)
		{
			this.target = this.farAwayTarget;
			this.generatePathToTarget();
		}
		base.Invoke("findNewTarget", UnityEngine.Random.Range(4f, 7f));
		this.fleeing = true;
		base.Invoke("resetFleeing", 1.5f);
		this.flyingToPerch = false;
	}

	public void FleeDodgeTree()
	{
		this.sendDisablePerchTarget();
		this.StopSong();
		float d = 1f;
		if (UnityEngine.Random.value > 0.5f)
		{
			d = -1f;
		}
		this.target = this.tr.position + this.tr.forward * 5f + this.tr.right * 30f * d;
		base.Invoke("findNewTarget", UnityEngine.Random.Range(2f, 3f));
		this.fleeing = true;
		base.Invoke("resetFleeing", 1.5f);
		this.flyingToPerch = false;
	}

	private void resetFleeing()
	{
		this.fleeing = false;
	}

	private void setRandomTarget()
	{
		this.tempPos = this.Circle2(65f);
		this.farAwayTarget = new Vector3(this.tr.position.x + this.tempPos.x, this.tr.position.y + (float)UnityEngine.Random.Range(-3, 10), this.tempPos.y + this.tr.position.z);
		this.target = this.farAwayTarget;
	}

	private void SetController(lb_BirdController cont)
	{
		this.controller = cont;
	}

	private void ResetHopInt()
	{
		if (base.gameObject.activeSelf)
		{
			this.anim.SetInteger(this.hopIntHash, 0);
		}
	}

	private void ResetFlyingLandingVariables()
	{
		if (this.flying || this.landing)
		{
			this.flying = false;
			this.landing = false;
		}
	}

	private void PlaySong()
	{
		if (UnityEngine.Random.value < 0.005f && !Clock.planecrash)
		{
			if (base.gameObject.activeSelf)
			{
				this.songInstance = FMODCommon.PlayOneshot(this.SongEvent, base.transform);
			}
			this.songBreak = true;
			base.Invoke("resetSongBreak", 12f);
		}
	}

	private void resetSongBreak()
	{
		this.songBreak = false;
	}

	private Vector2 Circle2(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	private void setPerchTarget(GameObject target)
	{
		if (target)
		{
			this.currentPerchTarget = target;
		}
	}

	private void clearPerchTarget()
	{
		this.currentPerchTarget = null;
	}

	private void sendDisablePerchTarget()
	{
		if (this.currentPerchTarget)
		{
			perchTargetSetup component = this.currentPerchTarget.GetComponent<perchTargetSetup>();
			if (!component)
			{
				this.currentPerchTarget.AddComponent<perchTargetSetup>();
			}
			this.currentPerchTarget.SendMessage("disableThisTarget", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Update()
	{
		if (this.onGround && !this.paused)
		{
			this.OnGroundBehaviors();
		}
		if (!this.controller)
		{
			return;
		}
		if (this.controller.currentCamera)
		{
			if (this.skin.IsVisibleFrom(this.controller.currentCamera))
			{
				this.birdVisible = true;
			}
			else
			{
				this.birdVisible = false;
			}
		}
	}

	public virtual void SearchPath()
	{
		this.seeker.StartPath(base.transform.position, this.target, new OnPathDelegate(this.OnPathComplete));
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

	private void generatePathToTarget()
	{
		this.SearchPath();
		base.StopCoroutine("doMovement");
		base.StartCoroutine("doMovement");
	}

	[DebuggerHidden]
	private IEnumerator doMovement()
	{
		lb_Bird.<doMovement>c__Iterator212 <doMovement>c__Iterator = new lb_Bird.<doMovement>c__Iterator212();
		<doMovement>c__Iterator.<>f__this = this;
		return <doMovement>c__Iterator;
	}

	protected float XZSqrMagnitude(Vector3 a, Vector3 b)
	{
		float num = b.x - a.x;
		float num2 = b.z - a.z;
		return num * num + num2 * num2;
	}
}
