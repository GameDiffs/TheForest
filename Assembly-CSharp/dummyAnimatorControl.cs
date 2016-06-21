using System;
using TheForest.Utils;
using UnityEngine;

public class dummyAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private dummyAnimatorControl control;

	public GameObject parentGo;

	public chopEnemy chop;

	private Transform Tr;

	private Vector3 pos;

	private int layer;

	private int layerMask;

	public LayerMask groundMask;

	public bool setupFeedingEncounter;

	public bool setupMourningEncounter;

	public Transform hips;

	private float rVal;

	private bool randomSeed;

	public GameObject trapGo;

	private bool aligning;

	public bool calledFromDeath;

	private float dropTimeOffset;

	public bool doneDropCheck;

	private Vector3 currPos;

	private Vector3 lastPos;

	private RaycastHit hit;

	private RaycastHit[] allHit;

	private float timerAnimatorOffset;

	private float timerOffsetPosition;

	private RaycastHit finalHit;

	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.control = base.GetComponent<dummyAnimatorControl>();
		this.Tr = base.transform;
		this.layer = 26;
		this.layerMask = 1 << this.layer;
	}

	private void Start()
	{
		base.Invoke("disableControl", 5f);
		if (this.setupFeedingEncounter)
		{
			this.setFeedingEncounter();
		}
	}

	private void OnEnable()
	{
		this.animator.enabled = true;
		this.control.enabled = true;
		base.Invoke("disableControl", 5f);
		if (this.setupFeedingEncounter)
		{
			this.setFeedingEncounter();
		}
	}

	private void OnDisable()
	{
		this.doPickupDummy();
	}

	private void doPickupDummy()
	{
		if ((this.setupFeedingEncounter || this.setupMourningEncounter) && this.Tr)
		{
			this.Tr.localPosition = Vector3.zero;
		}
		if (!this.setupFeedingEncounter && !this.setupMourningEncounter && !base.transform.root.CompareTag("Multisled"))
		{
			if (base.transform.parent != null)
			{
				base.Invoke("setParentNull", 0.25f);
			}
			if (BoltNetwork.isRunning)
			{
				base.transform.SendMessageUpwards("releaseNooseTrapMP", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				base.transform.SendMessageUpwards("releaseNooseTrap", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (this.trapGo)
		{
			this.trapGo.SendMessage("removeMutant", base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
		base.CancelInvoke("disableControl");
		this.calledFromDeath = false;
		if (!BoltNetwork.isClient && this.hips)
		{
			this.hips.position = base.transform.position;
			this.hips.rotation = base.transform.rotation;
		}
		if (BoltNetwork.isClient)
		{
			this.Tr.position = new Vector3(-98f, -98f, -98f);
			this.timerOffsetPosition = Time.time + 0.6f;
		}
	}

	private void OnDestroy()
	{
		if (this.hips)
		{
			this.hips.position = base.transform.position;
			this.hips.rotation = base.transform.rotation;
		}
	}

	private void setParentNull()
	{
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
		}
	}

	private void OnAnimatorMove()
	{
		if (this.calledFromDeath)
		{
			return;
		}
		if (base.enabled && this.animator.enabled && !this.setupMourningEncounter && !this.animator.GetBool("trapBool"))
		{
			this.pos = new Vector3(this.Tr.position.x, this.Tr.position.y + 2f, this.Tr.position.z);
			this.allHit = Physics.RaycastAll(this.pos, Vector3.down, 8f, this.groundMask);
			float num = float.PositiveInfinity;
			Collider exists = null;
			for (int i = 0; i < this.allHit.Length; i++)
			{
				if (!this.allHit[i].collider.isTrigger)
				{
					float distance = this.allHit[i].distance;
					if (distance < num)
					{
						num = distance;
						exists = this.allHit[i].collider;
						this.hit = this.allHit[i];
					}
				}
			}
			if (exists)
			{
				if (!BoltNetwork.isClient)
				{
					Vector3 deltaPosition = this.animator.deltaPosition;
					this.Tr.Translate(deltaPosition, Space.World);
					this.Tr.position = new Vector3(this.Tr.position.x, this.hit.point.y, this.Tr.position.z);
				}
				this.Tr.rotation = this.animator.rootRotation;
				this.Tr.rotation = Quaternion.Lerp(this.Tr.rotation, Quaternion.LookRotation(Vector3.Cross(this.Tr.right, this.hit.normal), this.hit.normal), Time.deltaTime * 8f);
			}
		}
	}

	private void disableControl()
	{
		if (!BoltNetwork.isRunning)
		{
			this.animator.enabled = false;
		}
		this.calledFromDeath = false;
	}

	private void setTrapGo(GameObject trap)
	{
		this.trapGo = trap;
	}

	private void setTrapFall()
	{
		this.animator.enabled = true;
		base.CancelInvoke("disableControl");
		base.Invoke("disableControl", 5f);
		this.animator.SetBoolReflected("trapBool", false);
		this.animator.SetBoolReflected("dropFromTrap", true);
		this.setParentNull();
	}

	private void releaseFromSpikeTrap()
	{
		this.animator.enabled = true;
		base.CancelInvoke("disableControl");
		base.Invoke("disableControl", 5f);
		this.animator.CrossFade("Base Layer.deathStealth1", 0f, 0, 0.4f);
	}

	public void dropFromCarry()
	{
		if (this.setupMourningEncounter || this.setupFeedingEncounter)
		{
			this.Tr.position = this.parentGo.transform.position;
		}
		if (this.setupMourningEncounter)
		{
			this.parentGo.SendMessage("doAlignForEncounter", SendMessageOptions.DontRequireReceiver);
		}
		this.hips.localPosition = new Vector3(0f, 0f, 0f);
		this.animator.CrossFade("Base Layer.deathStealth1", 0f, 0, 0.4f);
		if (BoltNetwork.isClient)
		{
			this.dropTimeOffset = Time.time + 1.1f;
		}
	}

	private void clientDrop(Quaternion r)
	{
		this.Tr.rotation = r;
		this.animator.CrossFade("Base Layer.deathStealth1", 0f, 0, 0.28f);
	}

	private void Update()
	{
		if (BoltNetwork.isClient)
		{
			if (this.timerOffsetPosition > Time.time)
			{
				this.Tr.position = new Vector3(-98f, -98f, -98f);
			}
			this.currPos = this.Tr.position;
			if ((this.currPos - this.lastPos).sqrMagnitude * 1000f > 0.1f)
			{
				this.animator.enabled = true;
				this.timerAnimatorOffset = Time.time + 5f;
			}
			else if (Time.time > this.timerAnimatorOffset)
			{
				this.animator.enabled = false;
			}
		}
		this.lastPos = this.Tr.position;
		if (Time.time < this.dropTimeOffset && LocalPlayer.Transform)
		{
			if (Vector3.Distance(this.Tr.position, LocalPlayer.Transform.position) < 10f && !this.doneDropCheck)
			{
				this.animator.CrossFade("Base Layer.deathStealth1", 0f, 0, 0.3f);
				Vector3 vector = LocalPlayer.Transform.position + LocalPlayer.Transform.forward * 2f;
				this.doneDropCheck = true;
			}
		}
		else
		{
			this.doneDropCheck = false;
		}
	}

	public void setFeedingEncounter()
	{
		if (!this.randomSeed)
		{
			this.rVal = UnityEngine.Random.value;
			this.randomSeed = true;
		}
		if (this.animator)
		{
			if (this.rVal < 0.33f)
			{
				this.animator.CrossFade("Base Layer.deathStealth1", 0f, 0, 1f);
			}
			else if (this.rVal < 0.66f)
			{
				this.animator.CrossFade("Base Layer.dyingToDead", 0f, 0, 1f);
			}
			else
			{
				this.animator.CrossFade("Base Layer.deathMir1", 0f, 0, 1f);
			}
		}
	}

	public void hideAllGo()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void setCalledFromDeath()
	{
		this.calledFromDeath = true;
	}

	private Vector3 getDropPosition(Vector3 dropPos)
	{
		Vector3 origin = dropPos;
		origin.y += 5f;
		RaycastHit[] array = Physics.RaycastAll(origin, Vector3.down, 20f, this.groundMask);
		float num = float.PositiveInfinity;
		Collider exists = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].collider.isTrigger)
			{
				float distance = array[i].distance;
				if (distance < num)
				{
					num = distance;
					exists = array[i].collider;
					this.finalHit = array[i];
				}
			}
		}
		if (exists)
		{
			return this.finalHit.point;
		}
		return dropPos;
	}
}
