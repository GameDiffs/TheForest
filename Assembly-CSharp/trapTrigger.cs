using Bolt;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class trapTrigger : EntityBehaviour
{
	public GameObject anim;

	public GameObject hitbox;

	public GameObject noosePivot;

	public GameObject nooseParent;

	public GameObject nooseRope1;

	public GameObject nooseRope2;

	public GameObject nooseFootPivot;

	public GameObject cutRope;

	public GameObject cutTrigger;

	public GameObject resetTrigger;

	public GameObject dummyGo;

	public GameObject spikeTrapBlockerGo;

	public GameObject swingingRock;

	public GameObject navBlockerGo;

	private Animator animator;

	public mutantScriptSetup mutantSetup;

	private Collider triggerCollider;

	public List<GameObject> trappedMutants = new List<GameObject>();

	private GameObject trappedGo;

	public bool largeSpike;

	public bool largeDeadfall;

	public bool largeNoose;

	public bool largeSwingingRock;

	public bool rabbitTrap;

	[Header("FMOD")]
	public Transform sfxPosition;

	public string triggerSFX;

	private trapHit hit;

	public bool sprung;

	public override void Attached()
	{
		if (this.entity.isOwner)
		{
			if (this.entity.StateIs<ITrapLargeState>())
			{
				this.entity.GetState<ITrapLargeState>().Sprung = this.sprung;
			}
			if (this.entity.StateIs<ITrapRabbitState>())
			{
				this.entity.GetState<ITrapRabbitState>().Sprung = this.sprung;
			}
		}
		else
		{
			if (this.entity.StateIs<ITrapLargeState>())
			{
				ITrapLargeState state = this.entity.GetState<ITrapLargeState>();
				if (state.Sprung)
				{
					this.OnSprungMP();
				}
				state.AddCallback("Sprung", new PropertyCallbackSimple(this.OnSprungMP));
				if (this.largeNoose)
				{
					state.AddCallback("CanCutDown", new PropertyCallbackSimple(this.OnCanCutDownChanged));
					state.AddCallback("CanReset", new PropertyCallbackSimple(this.OnCanResetChanged));
				}
			}
			if (this.entity.StateIs<ITrapRabbitState>())
			{
				if (this.entity.GetState<ITrapRabbitState>().Sprung)
				{
					this.OnSprungMP();
				}
				this.entity.GetState<ITrapRabbitState>().AddCallback("Sprung", new PropertyCallbackSimple(this.OnSprungMP));
			}
		}
	}

	private void OnCanCutDownChanged()
	{
		ITrapLargeState state = this.entity.GetState<ITrapLargeState>();
		if (state.CanCutDown)
		{
			this.cutTrigger.SetActive(true);
		}
		else
		{
			this.cutTrigger.SetActive(false);
		}
	}

	private void OnCanResetChanged()
	{
		ITrapLargeState state = this.entity.GetState<ITrapLargeState>();
		if (state.CanReset)
		{
			this.resetTrigger.SetActive(true);
		}
		else
		{
			this.resetTrigger.SetActive(false);
		}
	}

	private void Update()
	{
		if (this.entity.IsAttached() && this.entity.isOwner && this.largeNoose)
		{
			ITrapLargeState state = this.entity.GetState<ITrapLargeState>();
			state.CanCutDown = this.cutTrigger.activeInHierarchy;
			state.CanReset = this.resetTrigger.activeInHierarchy;
		}
	}

	public override void Detached()
	{
		Scene.HudGui.AddIcon.SetActive(false);
	}

	private void OnSprungMP()
	{
		if (this.entity.StateIs<ITrapLargeState>() && this.entity.GetState<ITrapLargeState>().Sprung)
		{
			this.TriggerLargeTrap(null);
		}
		if (this.entity.StateIs<ITrapRabbitState>() && this.entity.GetState<ITrapRabbitState>().Sprung)
		{
			this.TriggerRabbitTrap();
		}
	}

	private void Start()
	{
		if (this.hitbox)
		{
			this.hit = this.hitbox.GetComponent<trapHit>();
		}
		this.CheckAnimReference();
		if (this.anim.GetComponent<Animation>() && !this.sprung)
		{
			this.anim.GetComponent<Animation>().wrapMode = WrapMode.ClampForever;
			this.anim.GetComponent<Animation>().Stop();
		}
		if (this.largeDeadfall)
		{
			this.anim.GetComponent<Animation>()["trapFall"].speed = 1.5f;
		}
		if (this.largeSpike)
		{
			this.anim.GetComponent<Animation>()["trapSpring"].speed = 1.8f;
			if (!this.sprung)
			{
				this.anim.GetComponent<Animation>()["trapSet"].speed = -1f;
				this.anim.GetComponent<Animation>().Play("trapSet");
			}
		}
		if (this.largeNoose)
		{
			base.Invoke("disableAnimator", 0.2f);
			this.triggerCollider.enabled = false;
			base.Invoke("enableTrigger", 1f);
		}
		if (this.hitbox)
		{
			this.hitbox.SetActive(false);
		}
	}

	private void OnEnable()
	{
		this.triggerCollider = base.transform.GetComponent<Collider>();
		if (this.triggerCollider)
		{
			if (this.rabbitTrap)
			{
				this.triggerCollider.enabled = false;
				base.Invoke("enableTrigger", 4f);
			}
			if (this.largeSwingingRock)
			{
				this.triggerCollider.enabled = false;
				base.Invoke("enableTrigger", 2.5f);
			}
		}
	}

	private void CheckAnimReference()
	{
		if (!this.anim)
		{
			this.anim = base.transform.parent.gameObject;
		}
		if (!this.animator)
		{
			if (this.anim)
			{
				this.animator = this.anim.GetComponent<Animator>();
			}
			else
			{
				this.animator = base.transform.parent.GetComponentInChildren<Animator>();
			}
		}
	}

	private void disableAnimator()
	{
		this.CheckAnimReference();
		this.animator.enabled = false;
	}

	private void enableTrigger()
	{
		this.triggerCollider.enabled = true;
	}

	public void AnimateTrapMP()
	{
		if (this.hitbox)
		{
			this.hitbox.SetActive(true);
		}
		if (this.largeDeadfall)
		{
			this.CheckAnimReference();
			this.anim.GetComponent<Animation>().Play("trapFall");
			base.Invoke("enableTrapReset", 3f);
		}
		if (this.largeSpike)
		{
			this.CheckAnimReference();
			this.anim.GetComponent<Animation>().Play("trapSpring");
			base.Invoke("enableTrapReset", 3f);
		}
		if (this.largeNoose)
		{
		}
		if (this.largeSwingingRock)
		{
			base.Invoke("enableTrapReset", 3f);
		}
	}

	private void TriggerRabbitTrap()
	{
		this.CheckAnimReference();
		this.anim.GetComponent<Animation>().Play("trapFall");
		this.PlayTriggerSFX();
		base.Invoke("enableTrapReset", 2f);
	}

	public void TriggerLargeTrap(Collider other)
	{
		if (BoltNetwork.isClient && other == null && this.largeNoose)
		{
			base.Invoke("switchNooseRope", 0.5f);
			this.cutTrigger.SetActive(true);
			this.animator.enabled = true;
			this.animator.SetIntegerReflected("direction", 0);
			this.animator.SetBoolReflected("trapSpringBool", true);
		}
		if (this.sprung)
		{
			return;
		}
		this.CheckAnimReference();
		bool flag = !BoltNetwork.isRunning || BoltNetwork.isServer;
		if (this.hitbox)
		{
			this.hitbox.SetActive(true);
		}
		if (this.largeSwingingRock)
		{
			this.cutRope.SetActive(false);
			this.swingingRock.SendMessage("enableSwingingRock");
			base.Invoke("enableTrapReset", 3f);
		}
		if (this.largeDeadfall)
		{
			this.anim.GetComponent<Animation>().Play("trapFall");
			base.Invoke("enableTrapReset", 3f);
		}
		if (this.largeSpike)
		{
			this.anim.GetComponent<Animation>().Play("trapSpring");
			this.spikeTrapBlockerGo.SetActive(true);
			if (flag && other)
			{
				other.gameObject.SendMessageUpwards("enableController", SendMessageOptions.DontRequireReceiver);
				if (other.gameObject.CompareTag("enemyCollide"))
				{
					this.mutantSetup = other.transform.root.GetComponentInChildren<mutantScriptSetup>();
					if (this.mutantSetup && !this.mutantSetup.ai.creepy && !this.mutantSetup.ai.creepy_male && !this.mutantSetup.ai.creepy_fat && !this.mutantSetup.ai.creepy_baby)
					{
						other.gameObject.SendMessageUpwards("setCurrentTrap", base.gameObject, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			base.Invoke("enableTrapReset", 3f);
		}
		if (this.largeNoose)
		{
			if (flag && other)
			{
				if (base.transform.InverseTransformPoint(other.transform.position).x > 0f)
				{
					this.animator.SetIntegerReflected("direction", 0);
				}
				else
				{
					this.animator.SetIntegerReflected("direction", 1);
				}
				other.gameObject.SendMessageUpwards("setFootPivot", this.nooseFootPivot, SendMessageOptions.DontRequireReceiver);
				if (other.gameObject.CompareTag("enemyCollide"))
				{
					mutantHitReceiver component = other.transform.GetComponent<mutantHitReceiver>();
					if (component)
					{
						component.inNooseTrap = true;
					}
					this.mutantSetup = other.transform.root.GetComponentInChildren<mutantScriptSetup>();
				}
				this.animator.enabled = true;
				this.animator.SetBoolReflected("trapSpringBool", true);
				if (this.mutantSetup)
				{
					if (!this.mutantSetup.ai.creepy && !this.mutantSetup.ai.creepy_male && !this.mutantSetup.ai.creepy_fat && !this.mutantSetup.ai.creepy_baby)
					{
						other.gameObject.SendMessageUpwards("setInNooseTrap", this.noosePivot);
					}
					other.gameObject.SendMessageUpwards("setCurrentTrap", base.gameObject);
				}
			}
			if (other)
			{
				other.gameObject.SendMessageUpwards("DieTrap", 2, SendMessageOptions.DontRequireReceiver);
			}
			base.Invoke("switchNooseRope", 0.5f);
			this.cutTrigger.SetActive(true);
			if (this.entity.IsOwner() && this.largeNoose)
			{
				this.entity.GetState<ITrapLargeState>().CanCutDown = true;
				this.entity.GetState<ITrapLargeState>().CanReset = false;
			}
			if (other && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PlayerNet")))
			{
				base.Invoke("enableTrapReset", 2f);
			}
		}
		if (this.hitbox)
		{
			base.Invoke("disableHitbox", 1.5f);
		}
		base.transform.GetComponent<Collider>().enabled = false;
		this.sprung = true;
		this.PlayTriggerSFX();
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("enemyCollide")) && !this.rabbitTrap && !this.sprung)
		{
			if (other.gameObject.CompareTag("enemyCollide"))
			{
				mutantHitReceiver component = other.transform.GetComponent<mutantHitReceiver>();
				netId component2 = other.transform.GetComponent<netId>();
				if (component && component.inNooseTrap)
				{
					return;
				}
				if (component2)
				{
					return;
				}
				Animator componentInChildren = other.transform.root.GetComponentInChildren<Animator>();
				if (componentInChildren)
				{
					if (!componentInChildren.enabled)
					{
						return;
					}
					if (componentInChildren.GetBool("deathfinalBOOL"))
					{
						return;
					}
					if (componentInChildren.GetBool("sleepBOOL"))
					{
						return;
					}
				}
			}
			if (BoltNetwork.isClient)
			{
				if (other.gameObject.CompareTag("Player"))
				{
					TriggerLargeTrap triggerLargeTrap = global::TriggerLargeTrap.Create(GlobalTargets.OnlyServer);
					triggerLargeTrap.Player = LocalPlayer.Entity;
					triggerLargeTrap.Trap = this.entity;
					triggerLargeTrap.Send();
					this.TriggerLargeTrap(null);
				}
			}
			else
			{
				if (BoltNetwork.isServer && this.entity && this.entity.isAttached && this.entity.StateIs<ITrapLargeState>())
				{
					this.entity.GetState<ITrapLargeState>().Sprung = true;
				}
				this.TriggerLargeTrap(other);
			}
		}
		if (BoltNetwork.isClient)
		{
			return;
		}
		if ((other.gameObject.CompareTag("animalCollide") || other.gameObject.CompareTag("enemyCollide")) && this.rabbitTrap)
		{
			bool flag = false;
			if (this.rabbitTrap && !base.transform.parent.gameObject.CompareTag("trapSprung"))
			{
				this.trappedGo = other.gameObject;
				other.gameObject.SendMessageUpwards("setTrapped", base.gameObject, SendMessageOptions.DontRequireReceiver);
				base.transform.parent.gameObject.tag = "trapSprung";
				this.TriggerRabbitTrap();
				if (BoltNetwork.isServer && this.entity && this.entity.isAttached && this.entity.StateIs<ITrapRabbitState>())
				{
					this.entity.GetState<ITrapRabbitState>().Sprung = true;
				}
			}
			animalType component3 = other.GetComponent<animalType>();
			if (component3 && component3.deer && this.largeDeadfall)
			{
				if (this.hitbox)
				{
					this.hitbox.SetActive(true);
				}
				this.CheckAnimReference();
				this.anim.GetComponent<Animation>().Play("trapFall");
				flag = true;
				base.Invoke("enableTrapReset", 3f);
			}
			if (flag)
			{
				this.PlayTriggerSFX();
			}
		}
	}

	private void PlayTriggerSFX()
	{
		if (this.sfxPosition == null)
		{
			this.sfxPosition = base.transform;
		}
		FMODCommon.PlayOneshot(this.triggerSFX, this.sfxPosition);
	}

	private void OnDeserialized()
	{
		if (base.transform.parent.gameObject.CompareTag("trapSprung"))
		{
			this.enableTrapReset();
			this.CheckAnimReference();
			if (this.rabbitTrap)
			{
				this.anim.GetComponent<Animation>().Play("trapFall");
			}
			if (this.largeDeadfall)
			{
				this.anim.GetComponent<Animation>().Play("trapFall");
			}
			if (this.largeSpike)
			{
				this.anim.GetComponent<Animation>().Play("trapSpring");
			}
		}
	}

	private void switchNooseRope()
	{
		this.nooseRope1.SetActive(false);
		this.nooseRope2.SetActive(true);
		if (this.mutantSetup)
		{
			if (!this.mutantSetup.ai.creepy && !this.mutantSetup.ai.creepy_male && !this.mutantSetup.ai.creepy_baby && !this.mutantSetup.ai.creepy_fat)
			{
				this.nooseParent.transform.parent = this.mutantSetup.leftFoot;
				this.nooseParent.transform.localPosition = new Vector3(-0.134f, 0f, 0.056f);
				this.nooseParent.transform.localEulerAngles = new Vector3(0f, -90f, 90f);
			}
			else
			{
				this.nooseParent.transform.localPosition = new Vector3(0f, -0.834f, 0f);
				this.nooseParent.transform.localEulerAngles = new Vector3(-90f, 90f, 0f);
				if (BoltNetwork.isServer)
				{
					base.Invoke("enableTrapReset", 3f);
				}
			}
		}
		else
		{
			this.nooseParent.transform.localPosition = new Vector3(0f, -0.834f, 0f);
			this.nooseParent.transform.localEulerAngles = new Vector3(-90f, 90f, 0f);
			if (BoltNetwork.isServer)
			{
				base.Invoke("enableTrapReset", 3f);
			}
		}
	}

	private void disableHitbox()
	{
		if (this.hit)
		{
			this.hit.disable = true;
		}
	}

	public void releaseNooseTrap()
	{
		if (this.cutRope)
		{
			this.cutRope.SetActive(false);
		}
		if (this.nooseRope2)
		{
			this.nooseRope2.SetActive(false);
		}
		if (this.nooseParent)
		{
			this.nooseParent.transform.parent = base.transform;
		}
		if (this.mutantSetup)
		{
			this.mutantSetup.pmEncounter.SendEvent("toRelease");
		}
		if (this.dummyGo)
		{
			this.dummyGo.SendMessage("setTrapFall", SendMessageOptions.DontRequireReceiver);
		}
		base.Invoke("enableTrapReset", 3f);
	}

	public void releaseNooseTrapMP()
	{
		CutTriggerActivated cutTriggerActivated = CutTriggerActivated.Create(GlobalTargets.Everyone);
		cutTriggerActivated.Trap = this.entity;
		cutTriggerActivated.Send();
		Debug.Log("doing release noose trap MP in trap trigger");
	}

	public void enableTrapReset()
	{
		if (this.resetTrigger)
		{
			this.resetTrigger.SetActive(true);
			if (this.entity.IsOwner() && this.largeNoose)
			{
				this.entity.GetState<ITrapLargeState>().CanCutDown = false;
				this.entity.GetState<ITrapLargeState>().CanReset = true;
			}
		}
		if (this.largeSwingingRock && this.navBlockerGo)
		{
			this.navBlockerGo.SetActive(true);
		}
	}

	public void removeMutant(GameObject go)
	{
		if (this.trappedMutants.Contains(go))
		{
			this.trappedMutants.Remove(go);
		}
	}

	public void releaseAnimal()
	{
		if (this.trappedGo)
		{
			this.trappedGo.SendMessageUpwards("releaseFromTrap", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void addTrappedMutant(GameObject go)
	{
		this.trappedMutants.Add(go);
	}

	private void releaseAllMutants()
	{
		if (this.largeSpike)
		{
			foreach (GameObject current in this.trappedMutants)
			{
				if (current)
				{
					current.SendMessage("releaseFromSpikeTrap", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else if (this.largeNoose)
		{
			this.releaseNooseTrap();
		}
	}

	private void OnDestroy()
	{
		if (this.nooseParent && !this.nooseParent.transform.parent)
		{
			UnityEngine.Object.Destroy(this.nooseParent);
		}
		this.releaseAllMutants();
	}
}
