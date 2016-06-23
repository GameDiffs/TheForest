using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
	private sceneTracker sceneInfo;

	public clsragdollify ragdoll;

	public animalController controlScript;

	public bool typeShark;

	public bool typeCaveFish;

	public LayerMask collideMask;

	public int health = 1;

	public float showAnimatorSpeed;

	public float Speed;

	private float initSpeed;

	public float rotateSpeed;

	private float initRotateSpeed;

	public float blendSpeed;

	private float minMagnitude = 0.05f;

	private float currentY;

	private float lastY;

	private Vector3 direction;

	private Vector3 PlayerPos;

	private Transform oceanTr;

	private float oceanHeight;

	public float oceanOffset;

	private bool oceanCheck;

	private Vector3 fishPos;

	private float spawnDist;

	public bool Dead;

	private bool Flee;

	private bool home;

	public bool attack;

	public bool attackCoolDown;

	public bool hitBool;

	public bool spearedBool;

	public List<GameObject> allPlayers = new List<GameObject>();

	private float distance;

	public GameObject MyTrigger;

	public GameObject Blood;

	public Material Speared;

	public Renderer MyBody;

	public SkinnedMeshRenderer mySkin;

	public Mesh[] fishTypeMesh;

	public Material[] fishTypeMat;

	public int fishTypeInt;

	[Header("FMOD")]
	public string dieEvent;

	public string sharkAttackEvent;

	private GameObject spawner;

	private Animator animator;

	private Transform tr;

	private Quaternion lastRotation;

	private Quaternion desiredRotation;

	private Vector3 desiredDirection;

	private Vector3 diff;

	private Vector3 lastDir;

	private Vector3 currentDir;

	private Vector3 tempPos;

	private float absDir;

	public float absDirX;

	private float smoothedDir;

	private float randScale;

	private float collideDist;

	private Vector3 checkPos;

	private float depthFactor = 0.5f;

	private float depthFactorTarget = 0.5f;

	private float changeDepthDelay;

	private float swimDepth;

	private float finalDepth;

	private Rigidbody rb;

	private Collider col;

	private RaycastHit hit;

	private Vector3 pos;

	private bool initBool;

	private bool exploded;

	private bool hitBlock;

	private void Awake()
	{
		this.initSpeed = this.Speed;
		this.initRotateSpeed = this.rotateSpeed;
	}

	private void Start()
	{
		this.rb = base.transform.GetComponent<Rigidbody>();
		this.col = base.transform.GetComponent<Collider>();
		this.ragdoll = base.transform.GetComponent<clsragdollify>();
		this.animator = base.GetComponent<Animator>();
		this.mySkin = base.transform.GetComponentInChildren<SkinnedMeshRenderer>();
		this.sceneInfo = Scene.SceneTracker;
		GameObject gameObject = GameObject.FindGameObjectWithTag("OceanHeight");
		if (gameObject)
		{
			this.oceanTr = gameObject.transform;
		}
		this.tr = base.transform;
		this.initBool = true;
		this.setupInvokes();
		if (!base.GetComponent<Rigidbody>())
		{
			base.gameObject.AddComponent<Rigidbody>();
		}
		base.GetComponent<Rigidbody>().isKinematic = true;
		this.animator.speed = this.Speed;
		if (this.typeCaveFish)
		{
			this.randScale = UnityEngine.Random.Range(0.35f, 0.5f);
			base.transform.localScale = new Vector3(this.randScale, this.randScale, this.randScale);
		}
		this.allPlayers = new List<GameObject>(Scene.SceneTracker.allPlayers);
		this.updatePlayerTargets();
	}

	private void setupInvokes()
	{
		if (!this.initBool)
		{
			return;
		}
		this.oceanHeight = UnityEngine.Random.Range(0.5f, 1f);
		if (this.typeShark)
		{
			if (!base.IsInvoking("SwitchDirection"))
			{
				base.InvokeRepeating("SwitchDirection", UnityEngine.Random.Range(1f, 4f), UnityEngine.Random.Range(7f, 10f));
			}
			if (!base.IsInvoking("checkDistances"))
			{
				base.InvokeRepeating("checkDistances", 2f, 3f);
			}
		}
		else if (!base.IsInvoking("SwitchDirection"))
		{
			base.InvokeRepeating("SwitchDirection", UnityEngine.Random.Range(1f, 4f), UnityEngine.Random.Range(2f, 4.5f));
		}
		if (!this.typeShark)
		{
			this.setupFishType();
		}
		if (!base.IsInvoking("checkForwardCollide"))
		{
			base.InvokeRepeating("checkForwardCollide", UnityEngine.Random.Range(1f, 3f), 0.4f);
		}
	}

	private void OnEnable()
	{
		FMODCommon.PreloadEvents(new string[]
		{
			this.dieEvent,
			this.sharkAttackEvent
		});
		if (!this.initBool)
		{
			return;
		}
		this.hitBlock = false;
		this.resetAttack();
		this.setupInvokes();
		if (this.animator)
		{
			this.animator.SetBool("Dead", false);
		}
		float time = UnityEngine.Random.Range(0.1f, 3f);
		if (!base.IsInvoking("updatePlayerTargets"))
		{
			base.InvokeRepeating("updatePlayerTargets", time, 2f);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("updatePlayerTargets");
		FMODCommon.UnloadEvents(new string[]
		{
			this.dieEvent,
			this.sharkAttackEvent
		});
		if (this.MyTrigger)
		{
			this.MyTrigger.SetActive(false);
		}
		destroyAfter component = base.transform.GetComponent<destroyAfter>();
		if (component)
		{
			component.enabled = false;
		}
		if (this.rb)
		{
			this.rb.useGravity = false;
			this.rb.isKinematic = true;
			this.spearedBool = false;
		}
		if (this.col)
		{
			this.col.isTrigger = true;
		}
		this.Dead = false;
		this.exploded = false;
		base.CancelInvoke("enableGrabTrigger");
	}

	private void OnDespawned()
	{
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
		}
	}

	private void setupFishType()
	{
		if (!this.typeCaveFish)
		{
			this.fishTypeInt = UnityEngine.Random.Range(0, this.fishTypeMesh.Length);
			this.mySkin.sharedMaterial = this.fishTypeMat[this.fishTypeInt];
			this.mySkin.sharedMesh = this.fishTypeMesh[this.fishTypeInt];
		}
	}

	private void updatePlayerTargets()
	{
		this.allPlayers = new List<GameObject>(Scene.SceneTracker.allPlayers);
		this.allPlayers.RemoveAll((GameObject o) => o == null);
		if (this.allPlayers.Count > 1)
		{
			this.allPlayers.Sort((GameObject c1, GameObject c2) => Vector3.Distance(base.transform.position, c1.transform.position).CompareTo(Vector3.Distance(base.transform.position, c2.transform.position)));
		}
	}

	private void LateUpdate()
	{
		this.showAnimatorSpeed = this.animator.speed;
		if (this.allPlayers.Count == 0)
		{
			return;
		}
		if (this.allPlayers[0] == null)
		{
			return;
		}
		if (this.typeShark && this.Dead)
		{
			if (this.animator)
			{
				this.animator.speed = 1f;
			}
			this.tr.position += this.tr.forward * this.Speed * Time.deltaTime;
		}
		else if (this.typeShark && !this.Dead)
		{
			if (this.animator)
			{
				this.animator.speed = this.Speed / 8f;
			}
		}
		else if (this.typeCaveFish)
		{
			if (this.animator)
			{
				this.animator.speed = this.Speed * 2.3f;
			}
		}
		else if (this.animator)
		{
			this.animator.speed = this.Speed;
		}
		if (!this.Dead)
		{
			if (this.spawner && !this.typeShark)
			{
				this.fishPos = this.tr.position;
				this.fishPos.y = this.spawner.transform.position.y;
				this.fishPos += this.tr.forward * this.Speed * Time.deltaTime;
				this.tr.position = this.fishPos;
			}
			else
			{
				this.tr.position += this.tr.forward * this.Speed * Time.deltaTime;
			}
			if (this.oceanCheck)
			{
				this.fishPos = this.tr.position;
				if (this.oceanTr)
				{
					this.depthFactor = Mathf.Lerp(this.depthFactor, this.depthFactorTarget, Time.deltaTime * 0.2f);
					if (Time.time > this.changeDepthDelay)
					{
						this.depthFactorTarget = UnityEngine.Random.Range(0.25f, 0.95f);
						this.changeDepthDelay = Time.time + (float)UnityEngine.Random.Range(9, 30);
					}
					float num = Terrain.activeTerrain.SampleHeight(this.tr.position) + Terrain.activeTerrain.transform.position.y;
					if (this.attack)
					{
						this.swimDepth = this.allPlayers[0].transform.position.y;
						if (this.swimDepth > this.oceanTr.position.y)
						{
							this.swimDepth = this.oceanTr.position.y - 0.6f;
						}
					}
					else
					{
						this.swimDepth = num + (this.oceanTr.position.y - num) * this.depthFactor;
					}
					this.finalDepth = Mathf.Lerp(this.finalDepth, this.swimDepth, Time.deltaTime * 0.5f);
					if (this.typeShark)
					{
						this.fishPos.y = this.finalDepth;
					}
					else
					{
						this.fishPos.y = this.oceanTr.position.y + this.oceanOffset - this.oceanHeight;
					}
				}
				this.tr.position = this.fishPos;
			}
			if (this.attack && !this.attackCoolDown && this.typeShark)
			{
				this.tempPos = this.allPlayers[0].transform.position;
				this.direction = (this.tempPos - this.tr.position).normalized;
				if (Vector3.Distance(this.allPlayers[0].transform.position, this.tr.position) < 8.7f)
				{
					this.animator.SetTriggerReflected("attackTrigger");
					FMODCommon.PlayOneshot(this.sharkAttackEvent, base.transform);
					this.attackCoolDown = true;
					base.Invoke("enableHit", 0.1f);
					base.Invoke("resetAttack", 2.3f);
					base.Invoke("resetAttackCoolDown", 7f);
				}
			}
			this.currentDir = (this.tr.rotation * Vector3.forward).normalized;
			this.lastDir = (this.desiredRotation * Vector3.forward).normalized;
			if (Vector3.Dot(this.lastDir, this.currentDir) > 0f)
			{
				this.absDir = Vector3.Cross(this.lastDir, this.currentDir).y;
			}
			else
			{
				this.absDir = (float)((Vector3.Cross(this.lastDir, this.currentDir).y <= 0f) ? -1 : 1);
			}
			this.smoothedDir = this.absDir;
			if (this.typeShark)
			{
				this.smoothedDir *= 10f;
			}
			if (this.animator)
			{
				this.animator.SetFloatReflected("Direction", this.smoothedDir);
			}
			this.diff = Vector3.Slerp(this.diff, this.direction, this.rotateSpeed * Time.deltaTime);
			this.currentY = this.tr.position.y;
			float y = (this.currentY - this.lastY) * 13f;
			this.lastY = this.tr.position.y;
			this.diff.y = y;
			if (this.diff.sqrMagnitude > this.minMagnitude)
			{
				this.desiredRotation = Quaternion.LookRotation(this.diff, Vector3.up);
			}
			this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.rotateSpeed * Time.deltaTime);
			this.tr.rotation = this.lastRotation;
		}
	}

	private void checkDistances()
	{
		if (this.allPlayers[0] != null)
		{
			this.spawnDist = Vector3.Distance(this.spawner.transform.position, this.tr.position);
			if (LocalPlayer.AnimControl.swimming && this.typeShark && !this.attackCoolDown && this.spawnDist < 70f && Vector3.Distance(this.allPlayers[0].transform.position, this.tr.position) < 40f)
			{
				this.switchToAttack();
			}
			if (this.spawnDist > 65f)
			{
				this.tempPos = this.spawner.transform.position;
				this.tempPos.y = this.tr.position.y;
				this.direction = (this.tempPos - this.tr.position).normalized;
				this.home = true;
				base.Invoke("resetHomeBool", 5f);
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

	private void resetAttackCoolDown()
	{
		this.attackCoolDown = false;
	}

	private void resetAttack()
	{
		this.direction *= -1f;
		this.rotateSpeed = this.initRotateSpeed;
		this.Speed = this.initSpeed;
		this.attack = false;
		this.hitBool = false;
	}

	private void switchToAttack()
	{
		this.rotateSpeed += 3f;
		this.Speed += 4f;
		this.attack = true;
		base.Invoke("resetAttack", 12f);
	}

	private void SwitchDirection()
	{
		if (!this.Flee && !this.home && !this.attack)
		{
			Vector3 vector = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0f, UnityEngine.Random.Range(-2f, 2f));
			this.direction = vector.normalized;
		}
	}

	private void fleeDirection()
	{
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
		base.Invoke("Calm", 2f);
	}

	[DebuggerHidden]
	private IEnumerator resetSpeed()
	{
		return new Fish.<resetSpeed>c__Iterator161();
	}

	private void reverseDirection()
	{
		this.direction = this.tr.forward * -1f;
		this.direction += this.tr.right * UnityEngine.Random.Range(-0.4f, 0.4f);
	}

	private void Retreat()
	{
		this.Flee = true;
		this.Speed += 2f;
		base.Invoke("Calm", (float)UnityEngine.Random.Range(6, 9));
	}

	private void Calm()
	{
		this.rotateSpeed -= 1f;
		this.Flee = false;
	}

	private void resetFlee()
	{
		this.Flee = false;
	}

	public void startFish(GameObject go)
	{
		this.spawner = go;
		this.controlScript = go.GetComponent<animalController>();
	}

	private void setTrapped(GameObject go)
	{
		base.transform.parent = go.transform;
		this.DieSpear();
	}

	private void DieSpear()
	{
		UnityEngine.Debug.Log("die spear");
		destroyAfter component = base.transform.GetComponent<destroyAfter>();
		if (component)
		{
			component.enabled = false;
		}
		if (this.controlScript.allFish.Contains(base.gameObject))
		{
			this.controlScript.allFish.Remove(base.gameObject);
		}
		this.spearedBool = true;
		this.rb.useGravity = false;
		this.rb.isKinematic = true;
		this.col.isTrigger = true;
		if (!this.Dead)
		{
			FMODCommon.PlayOneshot(this.dieEvent, base.transform);
		}
		if (this.Blood)
		{
			this.Blood.SetActive(true);
			this.Blood.transform.parent = null;
		}
		this.Dead = true;
		if (this.animator)
		{
			this.animator.SetBoolReflected("Dead", true);
		}
		base.CancelInvoke("switchDirection");
		base.CancelInvoke("checkForwardCollide");
		if (this.MyTrigger)
		{
			this.MyTrigger.SetActive(true);
		}
		base.StopCoroutine("setFleeSpeed");
		if (this.typeShark)
		{
			base.StartCoroutine("setDeathSpeedShark");
		}
		else
		{
			base.StartCoroutine("setDeathSpeed");
		}
	}

	public void Hit(int damage)
	{
		if (this.hitBlock)
		{
			return;
		}
		this.hitBlock = true;
		base.Invoke("resetHitBlock", 0.5f);
		this.health -= damage;
		if (this.health < 1)
		{
			this.Die();
		}
		else if (this.typeShark)
		{
			this.fleeDirection();
			this.animator.SetBoolReflected("hit", true);
			base.Invoke("resetHit", 0.5f);
		}
	}

	private void resetHitBlock()
	{
		this.hitBlock = false;
	}

	private void resetHit()
	{
		this.animator.SetBoolReflected("hit", false);
	}

	private void Explosion(float dist)
	{
		if (dist < 10f)
		{
			this.health -= 50;
		}
		if (this.health < 1)
		{
			this.exploded = true;
			this.Die();
		}
	}

	private void Die()
	{
		if (!this.Dead)
		{
			this.Dead = true;
			FMODCommon.PlayOneshot(this.dieEvent, base.transform);
			if (this.Blood)
			{
				this.Blood.SetActive(true);
				this.Blood.transform.parent = null;
			}
			if (this.exploded)
			{
				base.Invoke("enableGrabTrigger", 5f);
			}
			else if (this.MyTrigger)
			{
				this.MyTrigger.SetActive(true);
			}
			base.CancelInvoke("switchDirection");
			base.CancelInvoke("checkForwardCollide");
			base.StopCoroutine("setFleeSpeed");
			if (this.typeShark)
			{
				base.StartCoroutine("setDeathSpeedShark");
			}
			else
			{
				base.StartCoroutine("setDeathSpeed");
			}
			destroyAfter component = base.transform.GetComponent<destroyAfter>();
			if (component)
			{
				component.enabled = true;
			}
			if (!this.typeShark)
			{
				this.rb.useGravity = true;
				this.rb.isKinematic = false;
				base.transform.GetComponent<Collider>().isTrigger = false;
			}
			if (this.exploded)
			{
				this.rb.AddTorque((float)UnityEngine.Random.Range(-2000, 2000), (float)UnityEngine.Random.Range(-2000, 2000), (float)UnityEngine.Random.Range(-2000, 2000), ForceMode.Force);
				this.rb.AddForce(0f, (float)UnityEngine.Random.Range(700, 1500), 0f, ForceMode.Force);
			}
			else if (!this.typeShark)
			{
				this.rb.AddTorque(base.transform.forward * 8000f, ForceMode.Force);
			}
			if (this.typeShark)
			{
				this.animator.SetBoolReflected("Dead", true);
				this.animator.speed = 1f;
				base.Invoke("doRagDoll", 3.3f);
			}
		}
	}

	private void doRagDoll()
	{
		this.ragdoll.metgoragdoll(default(Vector3));
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.allPlayers.Count > 0)
		{
			if (!this.typeShark && other.gameObject.CompareTag("soundAlert") && Vector3.Distance(this.allPlayers[0].transform.position, this.tr.position) < 13f)
			{
				base.Invoke("fleeDirection", UnityEngine.Random.Range(0f, 0.4f));
			}
			if (this.hitBool && this.typeShark)
			{
				if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PlayerNet") || other.gameObject.CompareTag("playerHitDetect"))
				{
					other.gameObject.SendMessageUpwards("HitShark", 55);
				}
				this.hitBool = false;
			}
		}
	}

	private void enableGrabTrigger()
	{
		if (this.MyTrigger)
		{
			this.MyTrigger.SetActive(true);
		}
	}

	private Vector2 Circle2(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	private void checkForwardCollide()
	{
		if (this.typeShark)
		{
			this.checkPos = this.tr.position + this.tr.forward * 22f;
		}
		else
		{
			this.checkPos = this.tr.position + this.tr.forward * 6f;
		}
		if (!this.typeCaveFish)
		{
			float num = Terrain.activeTerrain.SampleHeight(this.checkPos) + Terrain.activeTerrain.transform.position.y;
			if (this.tr.position.y - num < 0f)
			{
				this.reverseDirection();
				this.Flee = true;
				base.Invoke("resetFlee", 2f);
			}
		}
		if (this.typeShark || this.typeCaveFish)
		{
			float num2;
			if (this.typeShark)
			{
				num2 = 10f;
			}
			else
			{
				num2 = 7f;
			}
			UnityEngine.Debug.DrawRay(this.tr.position, this.tr.forward * num2, Color.red, 1f);
			RaycastHit raycastHit;
			if (Physics.Raycast(this.tr.position, this.tr.forward, out raycastHit, num2, this.collideMask))
			{
				this.reverseDirection();
				this.Flee = true;
				base.Invoke("resetFlee", 3f);
			}
		}
	}

	private void releaseFish()
	{
		if (this.MyTrigger)
		{
			this.MyTrigger.SetActive(true);
		}
		base.CancelInvoke("switchDirection");
		base.CancelInvoke("checkForwardCollide");
		base.StopCoroutine("setFleeSpeed");
		base.StartCoroutine("setDeathSpeed");
		destroyAfter component = base.transform.GetComponent<destroyAfter>();
		if (component)
		{
			component.enabled = false;
		}
		this.spearedBool = false;
		this.rb.useGravity = true;
		this.rb.isKinematic = false;
		base.transform.GetComponent<Collider>().isTrigger = false;
	}

	private void setOcean(bool on)
	{
		if (on)
		{
			this.oceanCheck = true;
		}
		else
		{
			this.oceanCheck = false;
		}
	}

	private void setCave(bool on)
	{
		if (on)
		{
			this.typeCaveFish = true;
		}
		else
		{
			this.typeCaveFish = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator setDeathSpeed()
	{
		Fish.<setDeathSpeed>c__Iterator162 <setDeathSpeed>c__Iterator = new Fish.<setDeathSpeed>c__Iterator162();
		<setDeathSpeed>c__Iterator.<>f__this = this;
		return <setDeathSpeed>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator setDeathSpeedShark()
	{
		Fish.<setDeathSpeedShark>c__Iterator163 <setDeathSpeedShark>c__Iterator = new Fish.<setDeathSpeedShark>c__Iterator163();
		<setDeathSpeedShark>c__Iterator.<>f__this = this;
		return <setDeathSpeedShark>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator setFleeSpeed()
	{
		Fish.<setFleeSpeed>c__Iterator164 <setFleeSpeed>c__Iterator = new Fish.<setFleeSpeed>c__Iterator164();
		<setFleeSpeed>c__Iterator.<>f__this = this;
		return <setFleeSpeed>c__Iterator;
	}
}
