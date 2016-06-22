using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class newGooseAi : MonoBehaviour
{
	private sceneTracker sceneInfo;

	public gooseController controller;

	public newGooseAi targetAi;

	private simpleVis vis;

	private gooseAvoidance avoidance;

	private GameObject collisionGo;

	private CapsuleCollider collisionCol;

	private Rigidbody collisionRb;

	private Transform rotateTr;

	public Transform currLake;

	public Transform currLandingPoint;

	private Vector3 offsetLandingPos;

	public float Speed = 1f;

	public float cruisingSpeed;

	private float initSpeed;

	public float rotateSpeed = 1f;

	public float flyingRotateSpeed;

	private float initRotateSpeed;

	private float minMagnitude = 0.03f;

	private Vector3 direction;

	private Vector3 PlayerPos;

	private Vector3 fishPos;

	private Vector3 landingPos;

	public bool Dead;

	public bool Flee;

	public bool home;

	private bool enableGoose;

	public bool following;

	private bool hitBool;

	public bool takeOff;

	public bool flying;

	private bool landing;

	private bool onWater;

	private bool headingToLanding;

	public bool leader;

	private bool follower;

	private bool visible;

	public int turnInt;

	private float tVal;

	private float pitchRate;

	public float cruiseHeight;

	public List<GameObject> allPlayers = new List<GameObject>();

	private float distance;

	public GameObject Blood;

	public GameObject targetGo;

	public SkinnedMeshRenderer lowMesh;

	public SkinnedMeshRenderer highMesh;

	private GameObject spawner;

	private Animator animator;

	private Transform tr;

	private Quaternion lastRotation;

	private Quaternion desiredRotation;

	private Vector3 desiredDirection;

	private Vector3 diff;

	public Vector3 localDiff;

	private Vector3 lastDir;

	private Vector3 currentDir;

	private Vector3 tempPos;

	private float absDir;

	private float smoothedDir;

	private float randScale;

	private float collideDist;

	private float zDiff;

	private Vector3 checkPos;

	private Vector2 randPoint;

	public float mag;

	private RaycastHit hit;

	private Vector3 pos;

	private Rigidbody rb;

	private float tParam;

	private float newSpeed;

	private float tempVal;

	private float tParam_pitch;

	private float val_pitch;

	private void Start()
	{
		this.randPoint = this.Circle2(UnityEngine.Random.Range(8f, 30f));
		this.rb = base.GetComponent<Rigidbody>();
		this.sceneInfo = Scene.SceneTracker;
		this.collisionGo = base.transform.GetComponentInChildren<gooseAvoidance>().gameObject;
		this.collisionCol = this.collisionGo.GetComponent<CapsuleCollider>();
		this.collisionRb = this.collisionGo.GetComponent<Rigidbody>();
		this.avoidance = this.collisionGo.GetComponent<gooseAvoidance>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.vis = base.GetComponentInChildren<simpleVis>();
		this.tr = base.transform;
		this.rotateTr = this.animator.transform;
		this.initSpeed = this.Speed;
		this.initRotateSpeed = this.rotateSpeed;
		base.InvokeRepeating("SwitchDirection", UnityEngine.Random.Range(2f, 4f), UnityEngine.Random.Range(5f, 10f));
		base.InvokeRepeating("checkForwardCollide", UnityEngine.Random.Range(1f, 3f), 0.3f);
		base.InvokeRepeating("setRandomSpeed", UnityEngine.Random.Range(2f, 5f), UnityEngine.Random.Range(5f, 10f));
		base.InvokeRepeating("checkDistances", UnityEngine.Random.Range(0f, 5f), 1.5f);
		this.allPlayers = new List<GameObject>(this.sceneInfo.allPlayers);
		this.updatePlayerTargets();
	}

	private void OnEnable()
	{
		float time = UnityEngine.Random.Range(0.1f, 3f);
		if (!base.IsInvoking("updatePlayerTargets"))
		{
			base.InvokeRepeating("updatePlayerTargets", time, 2f);
		}
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
		base.CancelInvoke();
		this.flying = false;
		this.leader = false;
		this.following = false;
		this.follower = false;
		this.landing = false;
		this.controller.leader = null;
	}

	private void setRandomSpeed()
	{
		if (this.flying)
		{
			return;
		}
		if (!this.Flee && !this.home)
		{
			base.StartCoroutine("smoothRandomSpeed");
			this.rotateSpeed = UnityEngine.Random.Range(0.3f, 1.2f);
		}
	}

	private void findFollowTarget()
	{
		if (this.following || this.flying)
		{
			return;
		}
		this.targetAi = null;
		if (this.controller.spawnedGeese.Count == 0)
		{
			return;
		}
		this.controller.spawnedGeese.RemoveAll((GameObject o) => o == null);
		GameObject gameObject = this.controller.spawnedGeese[UnityEngine.Random.Range(0, this.controller.spawnedGeese.Count)];
		if (Vector3.Distance(this.tr.position, gameObject.transform.position) < 60f && gameObject.name != base.gameObject.name)
		{
			this.targetAi = gameObject.GetComponent<newGooseAi>();
			if (this.targetAi.following)
			{
				return;
			}
			this.following = true;
			this.targetGo = gameObject;
			base.StartCoroutine("doFollowTarget", gameObject.transform);
			base.InvokeRepeating("setFollowSpeed", 1f, 1f);
			base.Invoke("resetFollow", 30f);
		}
	}

	[DebuggerHidden]
	private IEnumerator doFollowTarget(Transform target)
	{
		newGooseAi.<doFollowTarget>c__IteratorCA <doFollowTarget>c__IteratorCA = new newGooseAi.<doFollowTarget>c__IteratorCA();
		<doFollowTarget>c__IteratorCA.target = target;
		<doFollowTarget>c__IteratorCA.<$>target = target;
		<doFollowTarget>c__IteratorCA.<>f__this = this;
		return <doFollowTarget>c__IteratorCA;
	}

	private void setFollowSpeed()
	{
		if (this.targetGo == null)
		{
			return;
		}
		if (this.targetAi == null)
		{
			return;
		}
		float num = Vector3.Distance(this.tr.position, this.targetGo.transform.position);
		if (num > 10f)
		{
			this.setSmoothSpeed(1.1f, 1f);
		}
		else if (num < 5f)
		{
			this.setSmoothSpeed(0.1f, 1f);
		}
		else
		{
			this.setSmoothSpeed(this.targetAi.Speed, 1f);
		}
	}

	private void setFlyingFollowSpeed()
	{
		if (this.targetAi == null)
		{
			this.targetAi = this.controller.leader.GetComponent<newGooseAi>();
		}
		if (this.tr.InverseTransformPoint(this.tempPos).z < 0f)
		{
			this.setSmoothSpeed(this.targetAi.Speed * 0.8f, 2f);
			this.rotateSpeed = this.flyingRotateSpeed * 0.5f;
			return;
		}
		float num = Vector3.Distance(this.tr.position, this.tempPos);
		this.rotateSpeed = this.flyingRotateSpeed;
		if (num > 25f)
		{
			this.setSmoothSpeed(this.targetAi.Speed * UnityEngine.Random.Range(1.1f, 1.3f), 2f);
		}
		else
		{
			this.setSmoothSpeed(this.targetAi.Speed * UnityEngine.Random.Range(0.97f, 1.03f), 2f);
		}
	}

	private void resetFollow()
	{
		base.CancelInvoke("resetFollow");
		base.StopCoroutine("doFollowTarget");
		this.following = false;
		base.CancelInvoke("setFollowSpeed");
	}

	private void updatePlayerTargets()
	{
		this.allPlayers = new List<GameObject>(this.sceneInfo.allPlayers);
		this.allPlayers.RemoveAll((GameObject o) => o == null);
		if (this.allPlayers.Count > 1)
		{
			this.allPlayers.Sort((GameObject c1, GameObject c2) => (base.transform.position - c1.transform.position).sqrMagnitude.CompareTo((base.transform.position - c2.transform.position).sqrMagnitude));
		}
	}

	public void setCollisionDir()
	{
		if (this.flying)
		{
			return;
		}
		if (this.turnInt == 1)
		{
			this.direction = this.tr.right;
		}
		else if (this.turnInt == -1)
		{
			this.direction = this.tr.right * -1f;
		}
		base.Invoke("resetCollisionDir", 2f);
	}

	private void resetCollisionDir()
	{
		this.turnInt = 0;
	}

	private void Update()
	{
		if (!this.enableGoose)
		{
			return;
		}
		if (!this.flying && !this.visible)
		{
			return;
		}
		Vector3 vector = this.tr.position;
		vector += this.tr.forward * this.Speed * Time.deltaTime;
		if (!this.flying)
		{
			vector.y = this.currLake.position.y;
			this.animator.speed = 1.3f;
			this.diff.y = 0f;
		}
		this.tr.position = vector;
		this.localDiff = this.tr.InverseTransformDirection(this.direction);
		this.mag = this.localDiff.sqrMagnitude;
		if (this.direction.sqrMagnitude > this.minMagnitude && (this.localDiff.x > 0.05f || this.localDiff.x < -0.05f || this.localDiff.y > 0.02f || this.localDiff.y < -0.02f))
		{
			if (this.visible)
			{
				this.diff = Vector3.Slerp(this.diff, this.direction, this.rotateSpeed * Time.deltaTime);
			}
			else
			{
				this.diff = this.direction;
			}
			this.desiredRotation = Quaternion.LookRotation(this.diff, Vector3.up);
			this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.rotateSpeed * Time.deltaTime);
			this.tr.rotation = this.lastRotation;
			if (this.visible)
			{
				float num = 0f;
				this.smoothedDir = Mathf.SmoothDamp(this.smoothedDir, this.localDiff.x * -1f, ref num, 0.15f);
				if (this.flying && !this.onWater)
				{
					this.rotateTr.localEulerAngles = new Vector3(0f, 0f, this.smoothedDir * 65f);
				}
				this.animator.SetFloatReflected("Direction", this.smoothedDir);
			}
		}
	}

	private void enableHit()
	{
		this.hitBool = true;
	}

	private void resetHomeBool()
	{
		this.home = false;
	}

	private void SwitchDirection()
	{
		if (UnityEngine.Random.value < 0.5f && !this.flying)
		{
			this.animator.SetIntegerReflected("idleType", 1);
		}
		else
		{
			this.animator.SetIntegerReflected("idleType", 0);
		}
		if (this.following || this.flying)
		{
			return;
		}
		if (UnityEngine.Random.value < 0.1f)
		{
			this.animator.SetTriggerReflected("idleVarTrigger");
			this.setSmoothSpeed(0.4f, 1f);
			return;
		}
		if (UnityEngine.Random.value < 0.2f)
		{
			this.findFollowTarget();
		}
		if (!this.Flee && !this.home && !this.following && !this.flying)
		{
			Vector3 vector = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0f, UnityEngine.Random.Range(-2f, 2f));
			this.direction = vector.normalized;
		}
	}

	private void fleeDirection()
	{
		if (this.flying)
		{
			return;
		}
		this.rotateSpeed += 1f;
		if (Vector3.Distance(this.spawner.transform.position, this.tr.position) < 12f)
		{
			if (this.allPlayers[0] != null)
			{
				this.tempPos = this.allPlayers[0].transform.position;
			}
			this.tempPos.y = this.tr.position.y;
			this.direction = (this.tempPos - this.tr.position).normalized;
			this.direction *= -1f;
		}
		else
		{
			this.tempPos = this.spawner.transform.position;
			this.tempPos.y = this.tr.position.y;
			this.direction = (this.tempPos - this.tr.position).normalized;
		}
		this.Flee = true;
		base.StartCoroutine("setFleeSpeed");
		base.Invoke("Calm", 2.5f);
	}

	[DebuggerHidden]
	private IEnumerator resetSpeed()
	{
		return new newGooseAi.<resetSpeed>c__IteratorCB();
	}

	private void reverseDirection()
	{
		if (this.following || this.flying)
		{
			return;
		}
		this.direction = this.tr.forward * -1f;
		this.direction += this.tr.right * UnityEngine.Random.Range(-0.4f, 0.4f);
	}

	private void Calm()
	{
		if (this.flying)
		{
			return;
		}
		this.rotateSpeed -= 1f;
		this.smoothRandomSpeed();
		this.Flee = false;
	}

	private void resetFlee()
	{
		this.Flee = false;
	}

	private void Die()
	{
		this.Dead = true;
		base.CancelInvoke("switchDirection");
		base.CancelInvoke("checkForwardCollide");
		base.StopCoroutine("setFleeSpeed");
	}

	private void checkForwardCollide()
	{
		if (this.takeOff || this.landing)
		{
			return;
		}
		float num = Terrain.activeTerrain.SampleHeight(this.checkPos) + Terrain.activeTerrain.transform.position.y;
		if (this.flying)
		{
			this.checkPos = this.tr.position + this.tr.forward * 40f;
			if (this.tr.position.y - num < 100f)
			{
				this.direction.y = 0.4f;
			}
			else
			{
				this.direction.y = 0f;
			}
		}
		else
		{
			this.checkPos = this.tr.position + this.tr.forward * 4f;
			if (this.tr.position.y - num < 0f)
			{
				this.reverseDirection();
				this.Flee = true;
				base.Invoke("resetFlee", 2f);
			}
		}
	}

	private void checkDistances()
	{
		if (!this.enableGoose || this.allPlayers.Count == 0)
		{
			return;
		}
		if (this.allPlayers[0] == null)
		{
			return;
		}
		float num = Vector3.Distance(this.allPlayers[0].transform.position, this.tr.position);
		if (num < 45f)
		{
			this.highMesh.enabled = true;
			this.lowMesh.enabled = false;
		}
		else
		{
			this.highMesh.enabled = false;
			this.lowMesh.enabled = true;
		}
		if (this.flying)
		{
			this.highMesh.gameObject.layer = 10;
			this.lowMesh.gameObject.layer = 10;
		}
		else
		{
			this.highMesh.gameObject.layer = 21;
			this.lowMesh.gameObject.layer = 21;
		}
		if (!this.visible || num > 100f)
		{
			this.collisionRb.Sleep();
			this.collisionCol.enabled = false;
			this.avoidance.enabled = false;
			this.collisionGo.SetActive(false);
		}
		else
		{
			this.collisionGo.SetActive(true);
			this.collisionRb.WakeUp();
			this.collisionCol.enabled = true;
			this.avoidance.enabled = true;
			this.collisionGo.SetActive(true);
		}
		if (this.highMesh.isVisible || this.lowMesh.isVisible)
		{
			this.visible = true;
		}
		else if (!this.highMesh.isVisible && !this.lowMesh.isVisible)
		{
			this.visible = false;
		}
		if (this.flying)
		{
			return;
		}
		if (this.allPlayers[0] != null && num < 12f)
		{
			this.fleeDirection();
			return;
		}
		if (this.following)
		{
			return;
		}
		if (!this.currLake)
		{
			return;
		}
		float num2 = Vector3.Distance(this.tr.position, this.currLake.position);
		if (num2 > 30f)
		{
			this.tempPos = this.spawner.transform.position;
			this.tempPos.y = this.tr.position.y;
			this.direction = (this.tempPos - this.tr.position).normalized;
			this.home = true;
			base.Invoke("resetHomeBool", 5f);
		}
	}

	[DebuggerHidden]
	private IEnumerator setDeathSpeed()
	{
		newGooseAi.<setDeathSpeed>c__IteratorCC <setDeathSpeed>c__IteratorCC = new newGooseAi.<setDeathSpeed>c__IteratorCC();
		<setDeathSpeed>c__IteratorCC.<>f__this = this;
		return <setDeathSpeed>c__IteratorCC;
	}

	[DebuggerHidden]
	private IEnumerator setFleeSpeed()
	{
		newGooseAi.<setFleeSpeed>c__IteratorCD <setFleeSpeed>c__IteratorCD = new newGooseAi.<setFleeSpeed>c__IteratorCD();
		<setFleeSpeed>c__IteratorCD.<>f__this = this;
		return <setFleeSpeed>c__IteratorCD;
	}

	[DebuggerHidden]
	private IEnumerator smoothRandomSpeed()
	{
		newGooseAi.<smoothRandomSpeed>c__IteratorCE <smoothRandomSpeed>c__IteratorCE = new newGooseAi.<smoothRandomSpeed>c__IteratorCE();
		<smoothRandomSpeed>c__IteratorCE.<>f__this = this;
		return <smoothRandomSpeed>c__IteratorCE;
	}

	private void setSmoothSpeed(float getSpeed, float t)
	{
		this.tVal = t;
		base.StartCoroutine("doSmoothSpeed", getSpeed);
	}

	[DebuggerHidden]
	private IEnumerator doSmoothSpeed(float newSpeed)
	{
		newGooseAi.<doSmoothSpeed>c__IteratorCF <doSmoothSpeed>c__IteratorCF = new newGooseAi.<doSmoothSpeed>c__IteratorCF();
		<doSmoothSpeed>c__IteratorCF.newSpeed = newSpeed;
		<doSmoothSpeed>c__IteratorCF.<$>newSpeed = newSpeed;
		<doSmoothSpeed>c__IteratorCF.<>f__this = this;
		return <doSmoothSpeed>c__IteratorCF;
	}

	private void startGoose(GameObject go)
	{
		this.controller = go.GetComponent<gooseController>();
		this.spawner = go;
		this.setClosestLake();
		this.enableGoose = true;
		this.onWater = true;
		this.diff = base.transform.forward;
	}

	private void setClosestLake()
	{
		float num = float.PositiveInfinity;
		Transform[] lakes = this.controller.lakes;
		for (int i = 0; i < lakes.Length; i++)
		{
			Transform transform = lakes[i];
			float sqrMagnitude = (this.tr.position - transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				this.currLake = transform;
			}
		}
	}

	private Vector2 Circle2(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	[DebuggerHidden]
	public IEnumerator startFlying()
	{
		newGooseAi.<startFlying>c__IteratorD0 <startFlying>c__IteratorD = new newGooseAi.<startFlying>c__IteratorD0();
		<startFlying>c__IteratorD.<>f__this = this;
		return <startFlying>c__IteratorD;
	}

	public void initLanding()
	{
		if (!this.leader)
		{
			base.StartCoroutine("startLanding");
		}
	}

	[DebuggerHidden]
	public IEnumerator startLanding()
	{
		newGooseAi.<startLanding>c__IteratorD1 <startLanding>c__IteratorD = new newGooseAi.<startLanding>c__IteratorD1();
		<startLanding>c__IteratorD.<>f__this = this;
		return <startLanding>c__IteratorD;
	}

	private void setLandingGear()
	{
		this.animator.SetBoolReflected("landing", true);
		this.animator.SetBoolReflected("fly", false);
		this.setSmoothSpeed(0.5f, 0.2f);
	}

	[DebuggerHidden]
	private IEnumerator doLandingHeight()
	{
		newGooseAi.<doLandingHeight>c__IteratorD2 <doLandingHeight>c__IteratorD = new newGooseAi.<doLandingHeight>c__IteratorD2();
		<doLandingHeight>c__IteratorD.<>f__this = this;
		return <doLandingHeight>c__IteratorD;
	}

	private void setDirLandingPoint()
	{
		if (this.controller.debugGeese)
		{
			this.currLandingPoint = this.controller.forceLandingPoint.transform;
			this.controller.currLandingPoint = this.currLandingPoint;
		}
		else
		{
			this.currLandingPoint = this.controller.landingPoints[UnityEngine.Random.Range(0, this.controller.landingPoints.Length)];
			this.controller.currLandingPoint = this.currLandingPoint;
		}
		this.offsetLandingPos = this.currLandingPoint.position + this.currLandingPoint.forward * -50f + this.currLandingPoint.right * 100f;
		this.offsetLandingPos.y = this.offsetLandingPos.y + 110f;
		this.controller.landingPos = this.offsetLandingPos;
		this.direction = (this.offsetLandingPos - this.tr.position).normalized;
	}

	private void setDirRunupPoint()
	{
		this.offsetLandingPos = this.currLandingPoint.position + this.currLandingPoint.forward * -185f;
		this.offsetLandingPos.y = this.offsetLandingPos.y + 60f;
		this.direction = (this.offsetLandingPos - this.tr.position).normalized;
	}

	private void setTouchDownPoint()
	{
		this.direction = (this.currLandingPoint.position - this.tr.position).normalized;
	}

	private void setRandomFlap()
	{
		this.animator.SetBoolReflected("glide", false);
		base.Invoke("resetFlap", (float)UnityEngine.Random.Range(3, 7));
	}

	private void resetFlap()
	{
		this.animator.SetBoolReflected("glide", true);
		base.Invoke("setRandomFlap", (float)UnityEngine.Random.Range(8, 15));
	}

	private void setRandomFlyDir()
	{
		Vector3 vector = this.Circle2((float)UnityEngine.Random.Range(80, 1000));
		vector.z += 700f;
		vector.y = this.tr.position.y;
		vector.y = Terrain.activeTerrain.SampleHeight(vector) + Terrain.activeTerrain.transform.position.y + 120f;
		this.direction = (vector - base.transform.position).normalized;
	}

	private void setSmoothPitch(float height, float rate)
	{
		this.pitchRate = rate;
		base.StartCoroutine("doSmoothPitch", height);
	}

	[DebuggerHidden]
	private IEnumerator doSmoothPitch(float getHeight)
	{
		newGooseAi.<doSmoothPitch>c__IteratorD3 <doSmoothPitch>c__IteratorD = new newGooseAi.<doSmoothPitch>c__IteratorD3();
		<doSmoothPitch>c__IteratorD.getHeight = getHeight;
		<doSmoothPitch>c__IteratorD.<$>getHeight = getHeight;
		<doSmoothPitch>c__IteratorD.<>f__this = this;
		return <doSmoothPitch>c__IteratorD;
	}

	private void setNewLeader()
	{
		if (!this.leader)
		{
			this.targetAi = this.controller.leader.GetComponent<newGooseAi>();
			if (this.flying && !this.headingToLanding && !this.landing && !this.takeOff)
			{
				base.StopCoroutine("doFollowTarget");
				this.following = true;
				base.StartCoroutine("doFollowTarget", this.controller.leader.transform);
				if (!base.IsInvoking("setFlyingFollowSpeed"))
				{
					base.InvokeRepeating("setFlyingFollowSpeed", 1f, 2f);
				}
			}
		}
	}

	private void setAsLeader()
	{
		if (this.flying)
		{
			this.leader = true;
			this.following = false;
			this.follower = false;
		}
		if (this.headingToLanding)
		{
			base.StopCoroutine("startLanding");
			base.StartCoroutine("startLanding");
		}
	}
}
