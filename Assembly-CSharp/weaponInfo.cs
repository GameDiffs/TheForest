using Bolt;
using HutongGames.PlayMaker;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Items.World;
using TheForest.Utils;
using TheForest.World;
using UnityEngine;

[DoNotSerializePublic]
public class weaponInfo : EntityEventListener
{
	private PlayMakerFSM pmControl;

	private PlayMakerFSM pmNoise;

	private PlayMakerFSM pmRotate;

	private int SapDice;

	public PlayerInventory PlayerInv;

	private treeHitTrigger tht;

	private Animator animator;

	private playerHitReactions hitReactions;

	private Transform playerTr;

	public GameObject playerBase;

	public playerAnimatorControl animControl;

	public playerScriptSetup setup;

	private weaponInfo mainTriggerScript;

	public weaponInfo currentWeaponScript;

	public Transform weaponAudio;

	private animEventsManager animEvents;

	private FsmBool fsmJumpAttackBool;

	private FsmBool fsmHeavyAttackBool;

	private Transform MyFish;

	private Transform SpearTip;

	private Transform SpearTip2;

	public List<GameObject> spearedFish = new List<GameObject>();

	private SphereCollider soundCollider;

	public GameObject soundDetectGo;

	public GameObject brokenGO;

	public GameObject hitTriggerRange;

	public bool doSingleArmBlock;

	public bool noTreeCut;

	public bool canDoGroundAxeChop;

	public bool noBodyCut;

	public bool allowBodyCut;

	public bool mainTrigger;

	public bool stick;

	public bool axe;

	public bool rock;

	public bool fireStick;

	public bool spear;

	public bool shell;

	public string treeHitEvent;

	public string fleshHitEvent;

	public string hackBodyEvent;

	public string animalHitEvent;

	public string shellHitEvent;

	public string planeHitEvent;

	public string plantHitEvent;

	public string ropeHitEvent;

	public string dirtHitEvent;

	public string rockHitEvent;

	public string waterHitEvent;

	public string smashHitEvent;

	private bool Delay;

	private bool GroundHitDelay;

	private GameObject Player;

	private int damageAmount;

	private bool activeBool;

	private bool remotePlayer;

	public float pushForce = 10000f;

	[SerializeThis]
	public float weaponDamage;

	[SerializeThis]
	public float smashDamage;

	[SerializeThis]
	public float weaponSpeed;

	[SerializeThis]
	public float tiredSpeed;

	[SerializeThis]
	public float staminaDrain;

	[SerializeThis]
	public float soundDetectRange;

	[SerializeThis]
	public float weaponRange;

	[SerializeThis]
	public float treeDamage;

	[SerializeThis]
	public float blockStaminaDrain = 20f;

	public float blockDamagePercent;

	private float soundDetectRangeInit;

	private float animSpeed;

	private float animTiredSpeed;

	private bool Tired;

	private bool enemyDelay;

	private bool plantSoundBreak;

	private bool smashSoundEnabled = true;

	public Transform MyParticle;

	private float lastPlayerHit;

	public float baseWeaponDamage
	{
		get;
		private set;
	}

	public float baseWeaponSpeed
	{
		get;
		private set;
	}

	public float baseSmashDamage
	{
		get;
		private set;
	}

	public float baseTiredSpeed
	{
		get;
		private set;
	}

	public float baseStaminaDrain
	{
		get;
		private set;
	}

	public float baseSoundDetectRange
	{
		get;
		private set;
	}

	public float baseWeaponRange
	{
		get;
		private set;
	}

	public float WeaponDamage
	{
		get
		{
			float num = 1f + LocalPlayer.Stats.PhysicalStrength.CurrentStrengthScaled / 140f;
			if (LocalPlayer.Stats.IsHealthInGreyZone)
			{
				return this.weaponDamage * 1.3f * num;
			}
			return this.weaponDamage * num;
		}
	}

	public int DamageAmount
	{
		get
		{
			if (LocalPlayer.Stats.IsHealthInGreyZone)
			{
				return Mathf.RoundToInt((float)this.damageAmount * 1.3f);
			}
			return this.damageAmount;
		}
	}

	private void PlayEvent(string path, Transform t = null)
	{
		if (!string.IsNullOrEmpty(path))
		{
			if (t == null)
			{
				t = base.transform;
			}
			if (FMOD_StudioSystem.instance)
			{
				FMOD_StudioSystem.instance.PlayOneShot(path, t.position, null);
			}
		}
	}

	public override void OnEvent(SfxPlantHit evnt)
	{
		this.PlayEvent(this.plantHitEvent, null);
	}

	private void Awake()
	{
		this.baseWeaponDamage = this.weaponDamage;
		this.baseWeaponSpeed = this.weaponSpeed;
		this.baseSmashDamage = this.smashDamage;
		this.baseTiredSpeed = this.tiredSpeed;
		this.baseStaminaDrain = this.staminaDrain;
		this.baseSoundDetectRange = this.soundDetectRange;
		this.baseWeaponRange = this.weaponRange;
		this.setup = base.transform.root.GetComponentInChildren<playerScriptSetup>();
		if (BoltNetwork.isRunning && !this.setup)
		{
			return;
		}
		base.enabled = false;
		base.StartCoroutine(this.DelayedAwake());
	}

	[DebuggerHidden]
	private IEnumerator DelayedAwake()
	{
		weaponInfo.<DelayedAwake>c__Iterator105 <DelayedAwake>c__Iterator = new weaponInfo.<DelayedAwake>c__Iterator105();
		<DelayedAwake>c__Iterator.<>f__this = this;
		return <DelayedAwake>c__Iterator;
	}

	private void Start()
	{
		if (!this.setup && base.GetComponent<EmptyObjectIdentifier>())
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (this.setup.pmControl)
		{
			this.fsmJumpAttackBool = this.setup.pmControl.FsmVariables.GetFsmBool("doingJumpAttack");
			this.fsmHeavyAttackBool = this.setup.pmControl.FsmVariables.GetFsmBool("heavyAttackBool");
		}
		if (this.soundDetectGo)
		{
			this.soundCollider = this.soundDetectGo.GetComponent<SphereCollider>();
			this.soundDetectRangeInit = this.soundCollider.radius;
		}
		if (this.spear)
		{
			if (base.transform && base.transform.parent && base.transform.parent.FindChild("SpearTip"))
			{
				this.SpearTip = base.transform.parent.FindChild("SpearTip").transform;
			}
			if (base.transform && base.transform.parent && base.transform.parent.FindChild("SpearTip2"))
			{
				this.SpearTip2 = base.transform.parent.FindChild("SpearTip2").transform;
			}
		}
		this.weaponAudio = base.transform;
		this.currentWeaponScript = base.transform.GetComponent<weaponInfo>();
	}

	private void setupMainTrigger()
	{
		if (this.mainTriggerScript)
		{
			if (this.stick)
			{
				this.mainTriggerScript.stick = true;
			}
			else
			{
				this.mainTriggerScript.stick = false;
			}
			if (this.axe)
			{
				this.mainTriggerScript.axe = true;
			}
			else
			{
				this.mainTriggerScript.axe = false;
			}
			if (this.rock)
			{
				this.mainTriggerScript.rock = true;
			}
			else
			{
				this.mainTriggerScript.rock = false;
			}
			if (this.fireStick)
			{
				this.mainTriggerScript.fireStick = true;
			}
			else
			{
				this.mainTriggerScript.fireStick = false;
			}
			if (this.spear)
			{
				this.mainTriggerScript.spear = true;
			}
			else
			{
				this.mainTriggerScript.spear = false;
			}
			if (this.shell)
			{
				this.mainTriggerScript.spear = true;
			}
			else
			{
				this.mainTriggerScript.shell = false;
			}
			this.mainTriggerScript.weaponDamage = this.WeaponDamage;
			this.mainTriggerScript.smashDamage = this.smashDamage;
			if (this.weaponRange == 0f)
			{
				this.mainTriggerScript.hitTriggerRange.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				this.mainTriggerScript.hitTriggerRange.transform.localScale = new Vector3(1f, 1f, this.weaponRange);
			}
		}
	}

	public void enableSpecialWeaponVars()
	{
		if (this.setup && this.setup.pmControl)
		{
			this.setup.pmControl.FsmVariables.GetFsmBool("groundAxeChop").Value = true;
		}
		if (this.animator)
		{
			this.animator.SetBoolReflected("groundAxeChopBool", true);
		}
	}

	public void disableSpecialWeaponVars()
	{
		if (this.setup && this.setup.pmControl)
		{
			this.setup.pmControl.FsmVariables.GetFsmBool("groundAxeChop").Value = false;
		}
		if (this.animator)
		{
			this.animator.SetBoolReflected("groundAxeChopBool", false);
		}
	}

	private void resetConnectFloat()
	{
		this.animator.SetFloatReflected("connectFloat", 0f);
	}

	private void resetEnemyDelay()
	{
		this.enemyDelay = false;
	}

	private void resetMyFish()
	{
		this.MyFish = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			return;
		}
		if (other.gameObject.CompareTag("Tree") || other.gameObject.CompareTag("MidTree") || other.gameObject.CompareTag("enemyCollide") || other.gameObject.tag == "lb_bird" || other.gameObject.CompareTag("animalCollide") || other.gameObject.CompareTag("Fish") || other.gameObject.CompareTag("EnemyBodyPart") || other.gameObject.CompareTag("jumpObject"))
		{
			Vector3 vector = LocalPlayer.Transform.InverseTransformPoint(other.bounds.center);
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			if (num > 37f || num < -37f)
			{
				return;
			}
		}
		PlayerHitEnemy playerHitEnemy = null;
		try
		{
			if (other.gameObject.CompareTag("PlayerNet") && (this.mainTrigger || (!this.mainTrigger && this.animControl.smashBool)))
			{
				BoltEntity component = other.GetComponent<BoltEntity>();
				BoltEntity component2 = base.GetComponent<BoltEntity>();
				if (!object.ReferenceEquals(component, component2))
				{
					if (this.lastPlayerHit + 0.4f < Time.time)
					{
						other.transform.root.SendMessage("getClientHitDirection", this.animator.GetInteger("hitDirection"), SendMessageOptions.DontRequireReceiver);
						other.transform.root.SendMessage("StartPrediction", SendMessageOptions.DontRequireReceiver);
						this.lastPlayerHit = Time.time;
						if (BoltNetwork.isRunning)
						{
							HitPlayer.Raise(component, EntityTargets.Everyone).Send();
						}
					}
				}
			}
			else
			{
				if (BoltNetwork.isClient)
				{
					playerHitEnemy = PlayerHitEnemy.Raise(GlobalTargets.OnlyServer);
					playerHitEnemy.Target = other.GetComponentInParent<BoltEntity>();
				}
				if (other.gameObject.CompareTag("enemyHead") && !this.mainTrigger)
				{
					other.transform.SendMessageUpwards("HitHead", SendMessageOptions.DontRequireReceiver);
					if (playerHitEnemy != null)
					{
						playerHitEnemy.HitHead = true;
					}
				}
				if (other.gameObject.CompareTag("enemyCollide") && !this.mainTrigger && !this.animControl.smashBool)
				{
					other.transform.SendMessage("getSkinHitPosition", base.transform, SendMessageOptions.DontRequireReceiver);
				}
				if (other.gameObject.CompareTag("structure"))
				{
					other.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
					float damage = this.WeaponDamage * 4f;
					if (this.tht.atEnemy)
					{
						damage = this.WeaponDamage / 2f;
					}
					other.SendMessage("LocalizedHit", new LocalizedHitData(base.transform.position, damage), SendMessageOptions.DontRequireReceiver);
				}
				if (BoltNetwork.isClient && (other.CompareTag("jumpObject") || other.CompareTag("UnderfootWood")))
				{
					FauxWeaponHit fauxWeaponHit = FauxWeaponHit.Create(GlobalTargets.Others);
					fauxWeaponHit.Damage = Mathf.CeilToInt(this.WeaponDamage * 4f);
					fauxWeaponHit.Position = base.GetComponent<Collider>().transform.position;
					fauxWeaponHit.Send();
				}
				string tag = other.gameObject.tag;
				if (tag != null)
				{
					if (weaponInfo.<>f__switch$mapA == null)
					{
						weaponInfo.<>f__switch$mapA = new Dictionary<string, int>(7)
						{
							{
								"jumpObject",
								0
							},
							{
								"UnderfootWood",
								0
							},
							{
								"SLTier1",
								0
							},
							{
								"SLTier2",
								0
							},
							{
								"SLTier3",
								0
							},
							{
								"UnderfootRock",
								0
							},
							{
								"Target",
								0
							}
						};
					}
					int num2;
					if (weaponInfo.<>f__switch$mapA.TryGetValue(tag, out num2))
					{
						if (num2 == 0)
						{
							other.SendMessage("LocalizedHit", new LocalizedHitData(base.transform.position, this.WeaponDamage * 4f), SendMessageOptions.DontRequireReceiver);
						}
					}
				}
				this.PlaySurfaceHit(other);
				if (this.spear && other.gameObject.CompareTag("Fish") && this.MyFish == null && !this.mainTrigger)
				{
					base.transform.parent.SendMessage("GotBloody", SendMessageOptions.DontRequireReceiver);
					FMODCommon.PlayOneshotNetworked(this.fleshHitEvent, base.transform, FMODCommon.NetworkRole.Any);
					this.MyFish = other.transform;
					this.spearedFish.Add(other.gameObject);
					other.transform.parent = base.transform;
					other.transform.position = this.SpearTip.position;
					other.transform.rotation = this.SpearTip.rotation;
					Fish component3 = other.transform.GetComponent<Fish>();
					if (component3 && component3.typeCaveFish)
					{
						other.transform.position = this.SpearTip2.position;
						other.transform.rotation = this.SpearTip2.rotation;
					}
					other.SendMessage("DieSpear", SendMessageOptions.DontRequireReceiver);
					base.Invoke("resetMyFish", 1f);
				}
				if (other.gameObject.CompareTag("hanging") || (other.gameObject.CompareTag("BreakableWood") && !this.mainTrigger))
				{
					Rigidbody component4 = other.GetComponent<Rigidbody>();
					float d = this.pushForce;
					if (other.gameObject.CompareTag("BreakableWood"))
					{
						d = 4500f;
					}
					if (component4)
					{
						component4.AddForceAtPosition(this.playerTr.forward * d * 0.75f, base.transform.position, ForceMode.Force);
					}
					if (other.gameObject.CompareTag("hanging"))
					{
						FMODCommon.PlayOneshotNetworked(this.currentWeaponScript.fleshHitEvent, this.weaponAudio.transform, FMODCommon.NetworkRole.Any);
					}
				}
				if (this.spear && !this.mainTrigger && (other.gameObject.CompareTag("Water") || other.gameObject.CompareTag("Ocean")))
				{
					this.PlayGroundHit(this.waterHitEvent);
					Transform transform = UnityEngine.Object.Instantiate(this.MyParticle, base.transform.position, base.transform.rotation) as Transform;
					transform.transform.parent = null;
					this.setup.pmNoise.SendEvent("toWeaponNoise");
				}
				if (!this.spear && !this.mainTrigger && (other.gameObject.CompareTag("Water") || other.gameObject.CompareTag("Ocean")))
				{
					this.PlayGroundHit(this.waterHitEvent);
				}
				if (other.gameObject.CompareTag("Shell") && !this.mainTrigger)
				{
					other.gameObject.SendMessage("getAttackerType", 4, SendMessageOptions.DontRequireReceiver);
					other.gameObject.SendMessage("getAttacker", this.Player, SendMessageOptions.DontRequireReceiver);
					other.transform.SendMessageUpwards("Hit", 1, SendMessageOptions.DontRequireReceiver);
					this.PlayEvent(this.currentWeaponScript.shellHitEvent, this.weaponAudio);
				}
				if (other.gameObject.CompareTag("PlaneHull") && !this.mainTrigger)
				{
					this.PlayEvent(this.currentWeaponScript.planeHitEvent, this.weaponAudio);
				}
				mutantHitReceiver component5 = other.GetComponent<mutantHitReceiver>();
				if ((other.gameObject.CompareTag("enemyCollide") || other.gameObject.CompareTag("animalCollide")) && this.mainTrigger && !this.enemyDelay && !this.animControl.smashBool)
				{
					if (BoltNetwork.isClient && other.gameObject.CompareTag("enemyCollide"))
					{
						CoopMutantClientHitPrediction componentInChildren = other.transform.root.gameObject.GetComponentInChildren<CoopMutantClientHitPrediction>();
						if (componentInChildren)
						{
							componentInChildren.getClientHitDirection(this.animator.GetInteger("hitDirection"));
							componentInChildren.StartPrediction();
						}
					}
					if (this.currentWeaponScript)
					{
						this.currentWeaponScript.transform.parent.SendMessage("GotBloody", SendMessageOptions.DontRequireReceiver);
					}
					Vector3 vector2 = other.transform.root.GetChild(0).InverseTransformPoint(this.playerTr.position);
					float num3 = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
					other.gameObject.SendMessage("getAttackerType", 4, SendMessageOptions.DontRequireReceiver);
					other.gameObject.SendMessage("getAttacker", this.Player, SendMessageOptions.DontRequireReceiver);
					if (playerHitEnemy != null)
					{
						playerHitEnemy.getAttackerType = 4;
					}
					this.animator.SetFloatReflected("connectFloat", 1f);
					base.Invoke("resetConnectFloat", 0.3f);
					if (num3 < -140f || num3 > 140f)
					{
						if (component5)
						{
							component5.takeDamage(1);
						}
						else
						{
							other.transform.SendMessageUpwards("takeDamage", 1, SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							playerHitEnemy.takeDamage = 1;
						}
					}
					else
					{
						if (component5)
						{
							component5.takeDamage(0);
						}
						else
						{
							other.transform.SendMessageUpwards("takeDamage", 0, SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							playerHitEnemy.takeDamage = 0;
						}
					}
					if (this.spear || this.shell)
					{
						other.transform.SendMessageUpwards("getAttackDirection", 3, SendMessageOptions.DontRequireReceiver);
						if (playerHitEnemy != null)
						{
							playerHitEnemy.getAttackDirection = 3;
						}
					}
					else if (this.axe || this.rock || this.stick)
					{
						int integer = this.animator.GetInteger("hitDirection");
						if (this.axe)
						{
							if (component5)
							{
								component5.getAttackDirection(integer);
								component5.getStealthAttack();
							}
							else
							{
								other.transform.SendMessageUpwards("getAttackDirection", integer, SendMessageOptions.DontRequireReceiver);
								other.transform.SendMessageUpwards("getStealthAttack", SendMessageOptions.DontRequireReceiver);
							}
						}
						else if (this.stick)
						{
							if (component5)
							{
								component5.getAttackDirection(integer);
							}
							else
							{
								other.transform.SendMessageUpwards("getAttackDirection", integer, SendMessageOptions.DontRequireReceiver);
							}
						}
						else if (component5)
						{
							component5.getAttackDirection(0);
							component5.getStealthAttack();
						}
						else
						{
							other.transform.SendMessageUpwards("getAttackDirection", 0, SendMessageOptions.DontRequireReceiver);
							other.transform.SendMessageUpwards("getStealthAttack", SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							if (this.axe)
							{
								playerHitEnemy.getAttackDirection = integer;
							}
							else if (this.stick)
							{
								playerHitEnemy.getAttackDirection = integer;
							}
							else
							{
								playerHitEnemy.getAttackDirection = 0;
							}
							playerHitEnemy.getStealthAttack = true;
						}
					}
					else
					{
						int integer2 = this.animator.GetInteger("hitDirection");
						if (component5)
						{
							component5.getAttackDirection(integer2);
						}
						else
						{
							other.transform.SendMessageUpwards("getAttackDirection", integer2, SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							playerHitEnemy.getAttackDirection = integer2;
						}
					}
					if (this.fireStick && UnityEngine.Random.value > 0.8f)
					{
						if (component5)
						{
							component5.Burn();
						}
						else
						{
							other.transform.SendMessageUpwards("Burn", SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							playerHitEnemy.Burn = true;
						}
					}
					if (this.hitReactions.kingHitBool || this.fsmHeavyAttackBool.Value)
					{
						if (component5)
						{
							component5.getCombo(3);
							component5.hitRelay((int)this.WeaponDamage * 3);
						}
						else
						{
							other.transform.SendMessageUpwards("getCombo", 3, SendMessageOptions.DontRequireReceiver);
							other.transform.SendMessageUpwards("Hit", this.WeaponDamage * 3f, SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							playerHitEnemy.Hit = (int)this.WeaponDamage * 3;
							playerHitEnemy.getCombo = 3;
						}
						FMODCommon.PlayOneshotNetworked(this.currentWeaponScript.fleshHitEvent, this.weaponAudio.transform, FMODCommon.NetworkRole.Any);
					}
					else
					{
						if (component5)
						{
							component5.hitRelay((int)this.WeaponDamage);
						}
						else
						{
							other.transform.SendMessageUpwards("Hit", this.WeaponDamage, SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							playerHitEnemy.Hit = (int)this.WeaponDamage;
						}
						FMODCommon.PlayOneshotNetworked(this.currentWeaponScript.fleshHitEvent, this.weaponAudio.transform, FMODCommon.NetworkRole.Any);
					}
					this.setup.pmNoise.SendEvent("toWeaponNoise");
					this.hitReactions.enableWeaponHitState();
					this.animControl.hitCombo();
					if (((this.axe || this.rock) && !this.animator.GetBool("smallAxe")) || this.fsmHeavyAttackBool.Value)
					{
						if (component5)
						{
							component5.getCombo(3);
						}
						else
						{
							other.transform.SendMessageUpwards("getCombo", 3, SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							playerHitEnemy.getCombo = 3;
						}
					}
					else if (!this.hitReactions.kingHitBool)
					{
						if (component5)
						{
							component5.getCombo(this.animControl.combo);
						}
						else
						{
							other.transform.SendMessageUpwards("getCombo", this.animControl.combo, SendMessageOptions.DontRequireReceiver);
						}
						if (playerHitEnemy != null)
						{
							playerHitEnemy.getCombo = this.animControl.combo;
						}
					}
				}
				if ((other.gameObject.CompareTag("suitCase") || other.gameObject.CompareTag("metalProp")) && this.animControl.smashBool)
				{
					other.transform.SendMessage("Hit", this.smashDamage, SendMessageOptions.DontRequireReceiver);
					if (playerHitEnemy != null)
					{
						playerHitEnemy.Hit = (int)this.smashDamage;
					}
					if (BoltNetwork.isRunning && other.gameObject.CompareTag("suitCase"))
					{
						OpenSuitcase openSuitcase = OpenSuitcase.Raise(GlobalTargets.Others);
						openSuitcase.Position = base.GetComponent<Collider>().transform.position;
						openSuitcase.Damage = (int)this.smashDamage;
						openSuitcase.Send();
					}
					if (this.smashSoundEnabled)
					{
						this.smashSoundEnabled = false;
						base.Invoke("EnableSmashSound", 0.3f);
						this.PlayEvent(this.smashHitEvent, null);
						if (BoltNetwork.isRunning)
						{
							FmodOneShot fmodOneShot = FmodOneShot.Create(GlobalTargets.Others, ReliabilityModes.Unreliable);
							fmodOneShot.EventPath = CoopAudioEventDb.FindId(this.smashHitEvent);
							fmodOneShot.Position = base.transform.position;
							fmodOneShot.Send();
						}
					}
					this.setup.pmNoise.SendEvent("toWeaponNoise");
					this.hitReactions.enableWeaponHitState();
					if (other.gameObject.CompareTag("metalProp"))
					{
						Rigidbody component6 = other.GetComponent<Rigidbody>();
						if (component6)
						{
							component6.AddForceAtPosition((Vector3.down + LocalPlayer.Transform.forward * 0.2f) * this.pushForce * 2f, base.transform.position, ForceMode.Force);
						}
					}
				}
				if ((other.gameObject.CompareTag("enemyCollide") || other.gameObject.CompareTag("animalCollide") || other.gameObject.CompareTag("Fish") || other.gameObject.CompareTag("EnemyBodyPart")) && this.mainTrigger)
				{
					mutantTargetSwitching component7 = other.transform.GetComponent<mutantTargetSwitching>();
					bool flag = false;
					if (component7 && component7.regular)
					{
						flag = true;
					}
					if (!flag)
					{
						this.spawnWeaponBlood(other);
						if (other.gameObject.CompareTag("enemyCollide"))
						{
							this.spawnWeaponBlood(other);
							this.spawnWeaponBlood(other);
						}
					}
				}
				if ((other.gameObject.CompareTag("enemyCollide") || other.gameObject.tag == "lb_bird" || other.gameObject.CompareTag("animalCollide") || other.gameObject.CompareTag("Fish") || other.gameObject.CompareTag("EnemyBodyPart")) && !this.mainTrigger && !this.enemyDelay && this.animControl.smashBool)
				{
					base.transform.parent.SendMessage("GotBloody", SendMessageOptions.DontRequireReceiver);
					this.enemyDelay = true;
					base.Invoke("resetEnemyDelay", 0.25f);
					if ((this.rock || this.stick || this.spear || this.noBodyCut) && !this.allowBodyCut)
					{
						other.transform.SendMessageUpwards("ignoreCutting", SendMessageOptions.DontRequireReceiver);
					}
					other.transform.SendMessage("getSkinHitPosition", base.transform, SendMessageOptions.DontRequireReceiver);
					other.transform.SendMessage("hitSuitCase", this.smashDamage, SendMessageOptions.DontRequireReceiver);
					other.gameObject.SendMessage("getAttacker", this.Player, SendMessageOptions.DontRequireReceiver);
					other.gameObject.SendMessage("getAttackerType", 4, SendMessageOptions.DontRequireReceiver);
					if (this.fsmJumpAttackBool.Value && LocalPlayer.FpCharacter.jumpingTimer > 1.3f)
					{
						other.transform.SendMessageUpwards("Explosion", 7f, SendMessageOptions.DontRequireReceiver);
					}
					else if (!other.gameObject.CompareTag("Fish"))
					{
						other.transform.SendMessageUpwards("Hit", this.smashDamage, SendMessageOptions.DontRequireReceiver);
					}
					else if (other.gameObject.CompareTag("Fish") && !this.spear)
					{
						other.transform.SendMessage("Hit", this.smashDamage, SendMessageOptions.DontRequireReceiver);
					}
					if (playerHitEnemy != null)
					{
						playerHitEnemy.getAttackerType = 4;
						playerHitEnemy.Hit = 4;
					}
					if (this.axe)
					{
						other.transform.SendMessageUpwards("HitAxe", SendMessageOptions.DontRequireReceiver);
						if (playerHitEnemy != null)
						{
							playerHitEnemy.HitAxe = true;
						}
					}
					if (other.gameObject.tag == "lb_bird" || other.gameObject.CompareTag("animalCollide"))
					{
						FMODCommon.PlayOneshotNetworked(this.animalHitEvent, base.transform, FMODCommon.NetworkRole.Any);
					}
					if (other.gameObject.CompareTag("enemyCollide"))
					{
						FMODCommon.PlayOneshotNetworked(this.fleshHitEvent, base.transform, FMODCommon.NetworkRole.Any);
					}
					if (other.gameObject.CompareTag("EnemyBodyPart"))
					{
						FMODCommon.PlayOneshotNetworked(this.hackBodyEvent, base.transform, FMODCommon.NetworkRole.Any);
					}
					this.setup.pmNoise.SendEvent("toWeaponNoise");
					this.hitReactions.enableWeaponHitState();
				}
				if (!this.mainTrigger && (other.gameObject.CompareTag("BreakableWood") || other.gameObject.CompareTag("BreakableRock")))
				{
					other.transform.SendMessage("Hit", this.WeaponDamage, SendMessageOptions.DontRequireReceiver);
					other.SendMessage("LocalizedHit", new LocalizedHitData(base.transform.position, this.WeaponDamage), SendMessageOptions.DontRequireReceiver);
				}
				if (other.gameObject.tag == "lb_bird" && !this.mainTrigger)
				{
					base.transform.parent.SendMessage("GotBloody", SendMessageOptions.DontRequireReceiver);
					other.transform.SendMessage("Hit", this.WeaponDamage, SendMessageOptions.DontRequireReceiver);
					FMODCommon.PlayOneshotNetworked(this.animalHitEvent, base.transform, FMODCommon.NetworkRole.Any);
					this.setup.pmNoise.SendEvent("toWeaponNoise");
					this.hitReactions.enableWeaponHitState();
					if (playerHitEnemy != null)
					{
						playerHitEnemy.Hit = (int)this.WeaponDamage;
					}
				}
				if ((other.gameObject.CompareTag("Tree") && !this.mainTrigger) || (other.gameObject.CompareTag("MidTree") && !this.mainTrigger))
				{
					this.animEvents.cuttingTree = true;
					this.animEvents.Invoke("resetCuttingTree", 0.5f);
					if (this.stick || this.fireStick)
					{
						other.SendMessage("HitStick", SendMessageOptions.DontRequireReceiver);
						this.setup.pmNoise.SendEvent("toWeaponNoise");
						this.animator.SetFloatReflected("weaponHit", 1f);
						this.PlayEvent(this.treeHitEvent, null);
						if (BoltNetwork.isRunning && this.entity.isOwner)
						{
							FmodOneShot fmodOneShot2 = FmodOneShot.Raise(GlobalTargets.Others, ReliabilityModes.Unreliable);
							fmodOneShot2.Position = base.transform.position;
							fmodOneShot2.EventPath = CoopAudioEventDb.FindId(this.treeHitEvent);
							fmodOneShot2.Send();
						}
					}
					else if (!this.Delay)
					{
						this.Delay = true;
						base.Invoke("ResetDelay", 0.2f);
						this.SapDice = UnityEngine.Random.Range(0, 5);
						if (this.SapDice == 1)
						{
							this.PlayerInv.GotSap(null);
						}
						this.setup.pmNoise.SendEvent("toWeaponNoise");
						if (!this.noTreeCut)
						{
							other.SendMessage("Hit", this.treeDamage, SendMessageOptions.DontRequireReceiver);
						}
						this.PlayEvent(this.treeHitEvent, null);
						if (BoltNetwork.isRunning && this.entity.isOwner)
						{
							FmodOneShot fmodOneShot3 = FmodOneShot.Raise(GlobalTargets.Others, ReliabilityModes.Unreliable);
							fmodOneShot3.Position = base.transform.position;
							fmodOneShot3.EventPath = CoopAudioEventDb.FindId(this.treeHitEvent);
							fmodOneShot3.Send();
						}
					}
				}
				if ((other.gameObject.CompareTag("SmallTree") || other.gameObject.CompareTag("Rope")) && !this.mainTrigger)
				{
					this.setup.pmNoise.SendEvent("toWeaponNoise");
					int integer3 = this.animator.GetInteger("hitDirection");
					other.transform.SendMessage("getAttackDirection", integer3, SendMessageOptions.DontRequireReceiver);
					other.SendMessage("Hit", this.DamageAmount, SendMessageOptions.DontRequireReceiver);
					if (BoltNetwork.isRunning)
					{
						FauxWeaponHit fauxWeaponHit2 = FauxWeaponHit.Raise(GlobalTargets.Others);
						fauxWeaponHit2.Damage = this.damageAmount;
						fauxWeaponHit2.Position = base.GetComponent<Collider>().transform.position;
						fauxWeaponHit2.Send();
					}
					if (!this.plantSoundBreak)
					{
						if (other.gameObject.CompareTag("SmallTree"))
						{
							if (!string.IsNullOrEmpty(this.plantHitEvent))
							{
								FMODCommon.PlayOneshotNetworked(this.plantHitEvent, base.transform, FMODCommon.NetworkRole.Any);
							}
						}
						else if (other.gameObject.CompareTag("Rope"))
						{
							this.PlayEvent(this.ropeHitEvent, null);
						}
						this.plantSoundBreak = true;
						base.Invoke("disablePlantBreak", 0.3f);
					}
					if (other.gameObject.CompareTag("SmallTree"))
					{
						this.PlayerInv.GotLeaf();
					}
				}
				if (other.gameObject.CompareTag("fire") && !this.mainTrigger && this.fireStick)
				{
					other.SendMessage("startFire");
				}
			}
		}
		finally
		{
			if (playerHitEnemy != null && playerHitEnemy.Target && playerHitEnemy.Hit > 0)
			{
				playerHitEnemy.Send();
			}
		}
	}

	private void spawnWeaponBlood(Collider other)
	{
		int index = UnityEngine.Random.Range(0, Prefabs.Instance.BloodHitPSPrefabs.Length);
		Vector3 pos = this.currentWeaponScript.transform.position + LocalPlayer.MainCamTr.forward * 0.2f;
		if (other.gameObject.CompareTag("EnemyBodyPart"))
		{
			pos = this.currentWeaponScript.transform.position;
		}
		Prefabs.Instance.SpawnBloodHitPS(index, pos, LocalPlayer.MainCamTr.transform.rotation);
	}

	private void PlaySurfaceHit(Collider collider)
	{
		if (this.Delay)
		{
			return;
		}
		if (this.mainTrigger)
		{
			return;
		}
		if (collider.gameObject.CompareTag("TerrainMain"))
		{
			this.PlayGroundHit(this.dirtHitEvent);
		}
		else
		{
			UnderfootSurfaceDetector.SurfaceType surfaceType = UnderfootSurfaceDetector.GetSurfaceType(collider);
			string empty = string.Empty;
			UnderfootSurfaceDetector.SurfaceType surfaceType2 = surfaceType;
			if (surfaceType2 != UnderfootSurfaceDetector.SurfaceType.Wood)
			{
				if (surfaceType2 != UnderfootSurfaceDetector.SurfaceType.Rock)
				{
					return;
				}
				empty = this.rockHitEvent;
			}
			else
			{
				empty = this.treeHitEvent;
			}
			FMODCommon.PlayOneshotNetworked(empty, base.transform, FMODCommon.NetworkRole.Any);
		}
	}

	private void PlayGroundHit(string path)
	{
		if (!this.GroundHitDelay)
		{
			FMODCommon.PlayOneshotNetworked(path, base.transform, FMODCommon.NetworkRole.Any);
			this.GroundHitDelay = true;
			base.Invoke("ResetGroundHitDelay", UnityEngine.Random.Range(0.15f, 0.25f));
		}
	}

	private void disablePlantBreak()
	{
		this.plantSoundBreak = false;
	}

	private void EnableSmashSound()
	{
		this.smashSoundEnabled = true;
	}

	private void resetGroundHeight()
	{
		this.animator.SetFloatReflected("groundHeight", 0f);
	}

	private void ResetDelay()
	{
		this.Delay = false;
	}

	private void ResetGroundHitDelay()
	{
		this.GroundHitDelay = false;
	}

	private void enableSound()
	{
		if (this.soundDetectGo)
		{
			this.soundDetectGo.GetComponent<Collider>().enabled = true;
			this.soundCollider.radius = this.soundDetectRange;
		}
	}

	private void disableSound()
	{
		if (this.soundDetectGo)
		{
			this.soundCollider.radius = this.soundDetectRangeInit;
		}
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonDown("AltFire"))
		{
			this.checkBurnableCloth();
		}
	}

	private void OnEnable()
	{
		if (this.activeBool && !this.mainTrigger && !this.remotePlayer)
		{
			if (this.mainTriggerScript)
			{
				this.mainTriggerScript.currentWeaponScript = base.transform.GetComponent<weaponInfo>();
				this.mainTriggerScript.weaponAudio = base.transform;
				this.setupMainTrigger();
				if (this.canDoGroundAxeChop)
				{
					this.mainTriggerScript.enableSpecialWeaponVars();
				}
			}
			base.Invoke("cleanUpSpearedFish", 0.2f);
			if (this.animator)
			{
				this.checkBurnableCloth();
			}
			this.animSpeed = this.weaponSpeed / 20f;
			this.animSpeed += 0.5f;
			this.animTiredSpeed = this.tiredSpeed / 10f * (this.animSpeed - 0.5f);
			this.animTiredSpeed += 0.5f;
			if (!this.setup)
			{
				return;
			}
			if (this.setup.pmStamina)
			{
				if (this.animControl.tiredCheck)
				{
					this.animControl.tempTired = this.animSpeed;
					this.setup.pmStamina.FsmVariables.GetFsmFloat("notTiredSpeed").Value = this.animSpeed / 1f;
				}
				else
				{
					this.setup.pmStamina.FsmVariables.GetFsmFloat("notTiredSpeed").Value = this.animSpeed;
				}
			}
			if (this.setup && this.setup.pmStamina)
			{
				this.setup.pmStamina.FsmVariables.GetFsmFloat("tiredSpeed").Value = this.animTiredSpeed;
			}
			if (this.setup && this.setup.pmControl)
			{
				this.setup.pmControl.FsmVariables.GetFsmFloat("staminaDrain").Value = this.staminaDrain * -1f;
			}
			if (this.setup && this.setup.pmControl)
			{
				this.setup.pmControl.FsmVariables.GetFsmFloat("blockStaminaDrain").Value = this.blockStaminaDrain * -1f;
			}
			if (LocalPlayer.Stats)
			{
				LocalPlayer.Stats.blockDamagePercent = this.blockDamagePercent;
			}
			this.damageAmount = (int)this.WeaponDamage;
			if (this.setup && this.setup.pmStamina)
			{
				this.setup.pmStamina.SendEvent("toSetStats");
			}
		}
	}

	private void OnDisable()
	{
		if (this.activeBool && !this.mainTrigger && !this.remotePlayer)
		{
			foreach (GameObject current in this.spearedFish)
			{
				if (current)
				{
					PoolManager.Pools["creatures"].Despawn(current.transform);
				}
			}
			if (this.canDoGroundAxeChop && this.mainTriggerScript)
			{
				this.mainTriggerScript.disableSpecialWeaponVars();
			}
		}
	}

	private void checkBurnableCloth()
	{
		if (this.activeBool && !this.mainTrigger && !this.remotePlayer)
		{
			if (this.doSingleArmBlock)
			{
				this.animator.SetFloatReflected("singleArmBlock", 1f);
			}
			else
			{
				this.animator.SetFloatReflected("singleArmBlock", 0f);
			}
			BurnableCloth componentInChildren = base.transform.parent.GetComponentInChildren<BurnableCloth>();
			if (componentInChildren && !this.doSingleArmBlock)
			{
				if (componentInChildren.enabled)
				{
					this.animator.SetFloatReflected("singleArmBlock", 1f);
				}
				else
				{
					this.animator.SetFloatReflected("singleArmBlock", 0f);
				}
			}
			if (this.axe)
			{
				this.animator.SetFloatReflected("singleArmBlock", 1f);
			}
		}
	}

	private void cleanUpSpearedFish()
	{
		foreach (GameObject current in this.spearedFish)
		{
			if (current)
			{
				current.transform.parent = null;
			}
		}
		this.spearedFish.Clear();
	}
}
