using Bolt;
using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class animEventsManager : EntityEventListener
{
	public bool Remote;

	private UnderfootSurfaceDetector underFoot;

	private FirstPersonHeadBob headBob;

	private Animator animator;

	private treeHitTrigger hitTrigger;

	private playerScriptSetup setup;

	private PlayerInventory player;

	private playerAnimatorControl animControl;

	private simpleIkSolver armIK;

	public Digger dig;

	public GameObject bowArrow;

	public GameObject spearSpawn;

	public GameObject spearSpawn_upgraded;

	public Transform spearThrowPos;

	public Transform spearThrowPos_upgraded;

	public GameObject bookHeld;

	public float throwForce = 1000f;

	[ItemIdPicker]
	public int _spearId;

	[ItemIdPicker]
	public int _spearUpgradedId;

	private Transform hitTriggerTr;

	private int hashIdle;

	private Collider mainWeaponCollider;

	private BoxCollider stickCollider1;

	private CapsuleCollider stickCollider2;

	private BoxCollider fireStickCollider1;

	private CapsuleCollider fireStickCollider2;

	private Collider axeCollider;

	private Collider axeCraftedCollider;

	private Collider rockCollider;

	private Collider spearCollider;

	private Collider axeRustyCollider;

	private Collider axePlaneCollider;

	private Collider armHeldCollider;

	private Collider legHeldCollider;

	private Collider headHeldCollider;

	private Collider clubHeldCollider;

	private Collider clubCraftedHeldCollider;

	private Collider skullHeldCollider;

	private Collider stickUpgradedHeldCollider;

	private Collider stickUpgradedHeldCollider2;

	private Collider rockUpgradedHeldCollider;

	private Collider climbingAxeCollider;

	private Collider boneHeldCollider;

	private Collider turtleShellHeldCollider;

	private Collider katanaHeldCollider;

	private Collider spearUpgradedCollider;

	private Collider tennisRacketHeldCollider;

	[ItemIdPicker]
	public int _lighterId;

	public ParticleSystem leafFootParticle;

	public ParticleSystem snowFootParticle;

	public Transform leftFootSpawnPos;

	public Transform rightFootSpawnPos;

	public Collider underFootCollider;

	public ItemGroupEvent bowDrawEvent;

	public ItemGroupEvent stickSwooshEvent;

	public ItemGroupEvent axeSwooshEvent;

	public ItemGroupEvent rockSwooshEvent;

	public ItemGroupEvent spearSwooshEvent;

	public ItemGroupEvent swordSwooshEvent;

	public ItemGroupEvent fireStickSwooshEvent;

	private Dictionary<int, ItemGroupEvent> eventsByItemIdCache;

	public string breatheInEvent;

	public string breatheOutEvent;

	public string fallEvent;

	public string pushSledEvent;

	private FMOD.Studio.EventInstance sledEvent;

	private ParameterInstance sledSpeedParameter;

	private Vector3 previousPosition;

	[NonSerialized]
	public bool IsSledTurning;

	private static int idleToPushSledHash = Animator.StringToHash("idleToPushSled");

	private static int pushSledIdleHash = Animator.StringToHash("pushSledIdle");

	private static int locomotionHash = Animator.StringToHash("locomotion");

	public bool smoothBlock;

	public bool cuttingTree;

	public bool introCutScene;

	private void OnDeserialized()
	{
		this.animator = base.gameObject.GetComponent<Animator>();
		if (this.Remote)
		{
			return;
		}
		this.SetUpWeapons();
	}

	private void Awake()
	{
		this.underFoot = base.transform.parent.GetComponentInChildren<UnderfootSurfaceDetector>();
		this.headBob = base.transform.parent.GetComponent<FirstPersonHeadBob>();
		this.animator = base.gameObject.GetComponent<Animator>();
		this.hitTrigger = base.transform.GetComponentInChildren<treeHitTrigger>();
		if (this.Remote)
		{
			return;
		}
		this.SetUpWeapons();
	}

	private ItemGroupEvent[] AllItemGroupEvents()
	{
		return new ItemGroupEvent[]
		{
			this.bowDrawEvent,
			this.stickSwooshEvent,
			this.axeSwooshEvent,
			this.rockSwooshEvent,
			this.spearSwooshEvent,
			this.swordSwooshEvent,
			this.fireStickSwooshEvent
		};
	}

	private void Start()
	{
		this.hashIdle = Animator.StringToHash("idling");
		this.previousPosition = base.transform.position;
		this.eventsByItemIdCache = new Dictionary<int, ItemGroupEvent>();
		if (FMOD_StudioSystem.instance)
		{
			this.sledEvent = FMOD_StudioSystem.instance.GetEvent(this.pushSledEvent);
			if (this.sledEvent != null)
			{
				this.sledEvent.getParameter("speed", out this.sledSpeedParameter);
			}
			if (this.Remote)
			{
				return;
			}
			ItemGroupEvent[] array = this.AllItemGroupEvents();
			for (int i = 0; i < array.Length; i++)
			{
				ItemGroupEvent itemGroupEvent = array[i];
				int[] itemIds = itemGroupEvent._itemIds;
				for (int j = 0; j < itemIds.Length; j++)
				{
					int key = itemIds[j];
					this.eventsByItemIdCache[key] = itemGroupEvent;
					FMOD_StudioSystem.PreloadEvent(itemGroupEvent.eventPath);
				}
			}
			FMOD_StudioSystem.PreloadEvent(this.breatheInEvent);
			FMOD_StudioSystem.PreloadEvent(this.breatheOutEvent);
			FMOD_StudioSystem.PreloadEvent(this.fallEvent);
		}
		else
		{
			UnityEngine.Debug.LogError("FMOD_StudioSystem.instance is null, could not initialize animEventManager audio");
		}
	}

	private void Update()
	{
		if (this.sledEvent != null)
		{
			PLAYBACK_STATE state;
			UnityUtil.ERRCHECK(this.sledEvent.getPlaybackState(out state));
			bool flag = false;
			int shortNameHash = this.animator.GetCurrentAnimatorStateInfo(1).shortNameHash;
			int num = (!this.animator.IsInTransition(1)) ? 0 : this.animator.GetNextAnimatorStateInfo(1).shortNameHash;
			if (shortNameHash == animEventsManager.idleToPushSledHash || num == animEventsManager.idleToPushSledHash || shortNameHash == animEventsManager.pushSledIdleHash || num == animEventsManager.pushSledIdleHash)
			{
				int shortNameHash2 = this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
				int num2 = (!this.animator.IsInTransition(0)) ? 0 : this.animator.GetNextAnimatorStateInfo(0).shortNameHash;
				if (this.IsSledTurning)
				{
					flag = true;
				}
				else if (shortNameHash2 == animEventsManager.locomotionHash || num2 == animEventsManager.locomotionHash)
				{
					flag = true;
				}
			}
			if (flag)
			{
				UnityUtil.ERRCHECK(this.sledEvent.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
				Vector3 vector = (base.transform.position - this.previousPosition) / Time.deltaTime;
				vector.y = 0f;
				UnityUtil.ERRCHECK(this.sledSpeedParameter.setValue(Mathf.Clamp01(vector.magnitude / LocalPlayer.FpCharacter.runSpeed)));
				if (!state.isPlaying())
				{
					UnityUtil.ERRCHECK(this.sledEvent.start());
				}
			}
			else if (state.isPlaying())
			{
				UnityUtil.ERRCHECK(this.sledEvent.stop(STOP_MODE.ALLOWFADEOUT));
			}
		}
		this.previousPosition = base.transform.position;
	}

	private void SetUpWeapons()
	{
		if (this.Remote)
		{
			return;
		}
		this.dig = base.transform.parent.GetComponentInChildren<Digger>();
		this.setup = base.transform.parent.GetComponentInChildren<playerScriptSetup>();
		this.animControl = base.GetComponent<playerAnimatorControl>();
		this.player = base.transform.parent.GetComponent<PlayerInventory>();
		this.armIK = base.transform.GetComponent<simpleIkSolver>();
		Transform[] componentsInChildren = base.transform.root.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			if (transform.name == "hitTrigger")
			{
				this.mainWeaponCollider = transform.GetComponent<Collider>();
				if (this.mainWeaponCollider)
				{
					this.mainWeaponCollider.enabled = false;
				}
				this.hitTriggerTr = transform.parent.transform;
			}
			if (transform.name == "stickHeld")
			{
				this.stickCollider1 = transform.GetComponentInChildren<BoxCollider>();
				this.stickCollider2 = transform.GetComponentInChildren<CapsuleCollider>();
				this.stickCollider2.enabled = false;
			}
			if (transform.name == "FireStick")
			{
				this.fireStickCollider1 = transform.GetComponentInChildren<BoxCollider>();
				this.fireStickCollider2 = transform.GetComponentInChildren<CapsuleCollider>();
				this.fireStickCollider2.enabled = false;
			}
			if (transform.name == "AxeHeld")
			{
				this.axeCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "AxeCraftedHeld")
			{
				this.axeCraftedCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "RockHeld")
			{
				this.rockCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "spearHeld")
			{
				this.spearCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "AxeHeldRusty")
			{
				this.axeRustyCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "AxePlaneHeld")
			{
				this.axePlaneCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "testCube")
			{
			}
			if (transform.name == "armHeld")
			{
				this.armHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "legHeld")
			{
				this.legHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "HeadHeld")
			{
				this.headHeldCollider = transform.GetComponentInChildren<CapsuleCollider>();
			}
			if (transform.name == "ClubCraftedHeld")
			{
				this.clubCraftedHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "ClubHeld")
			{
				this.clubHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "SkullHeld")
			{
				this.skullHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "stickHeldUpgraded")
			{
				this.stickUpgradedHeldCollider = transform.GetComponentInChildren<BoxCollider>();
				this.stickUpgradedHeldCollider2 = transform.GetComponentInChildren<CapsuleCollider>();
			}
			if (transform.name == "RockHeldUpgraded")
			{
				this.rockUpgradedHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "AxeClimbingHeld")
			{
				this.climbingAxeCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "boneHeld")
			{
				this.boneHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "turtleShellHeld")
			{
				this.turtleShellHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "KatanaHeld")
			{
				this.katanaHeldCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "spearHeldUpgraded")
			{
				this.spearUpgradedCollider = transform.GetComponentInChildren<Collider>();
			}
			if (transform.name == "TennisRacketHeld")
			{
				this.tennisRacketHeldCollider = transform.GetComponentInChildren<Collider>();
			}
		}
	}

	private void setAttackFalse(bool setAttack)
	{
		if (this.Remote)
		{
			return;
		}
		int value = UnityEngine.Random.Range(0, 2);
		this.animator.SetIntegerReflected("randInt1", value);
		this.animator.SetBoolReflected("attack", setAttack);
		this.animator.SetBoolReflected("AxeAttack", setAttack);
	}

	private void enableWeapon()
	{
		if (this.Remote)
		{
			return;
		}
		if (this.mainWeaponCollider)
		{
			this.mainWeaponCollider.enabled = true;
		}
		this.axePlaneCollider.enabled = true;
		this.axeCraftedCollider.enabled = true;
		this.stickCollider1.enabled = true;
		this.axeCollider.enabled = true;
		this.rockCollider.enabled = true;
		this.spearCollider.enabled = true;
		this.fireStickCollider1.enabled = true;
		this.axeRustyCollider.enabled = true;
		this.armHeldCollider.enabled = true;
		this.legHeldCollider.enabled = true;
		this.headHeldCollider.enabled = true;
		this.clubCraftedHeldCollider.enabled = true;
		this.clubHeldCollider.enabled = true;
		this.skullHeldCollider.enabled = true;
		this.stickUpgradedHeldCollider.enabled = true;
		this.rockUpgradedHeldCollider.enabled = true;
		this.climbingAxeCollider.enabled = true;
		this.boneHeldCollider.enabled = true;
		this.turtleShellHeldCollider.enabled = true;
		this.katanaHeldCollider.enabled = true;
		this.spearUpgradedCollider.enabled = true;
		this.tennisRacketHeldCollider.enabled = true;
	}

	private void enableWeapon2()
	{
		if (this.Remote)
		{
			return;
		}
		this.stickCollider2.enabled = true;
		this.stickUpgradedHeldCollider2.enabled = true;
		this.fireStickCollider2.enabled = true;
	}

	private void enableSmashWeapon()
	{
		if (this.Remote)
		{
			return;
		}
		this.axeCraftedCollider.enabled = true;
		this.stickCollider2.enabled = true;
		this.axeCollider.enabled = true;
		this.rockCollider.enabled = true;
		this.spearCollider.enabled = true;
		this.fireStickCollider1.enabled = true;
		this.axePlaneCollider.enabled = true;
		this.axeRustyCollider.enabled = true;
		this.headHeldCollider.enabled = true;
		this.animControl.smashBool = true;
		this.armHeldCollider.enabled = true;
		this.legHeldCollider.enabled = true;
		this.skullHeldCollider.enabled = true;
		this.clubHeldCollider.enabled = true;
		this.clubCraftedHeldCollider.enabled = true;
		this.stickUpgradedHeldCollider2.enabled = true;
		this.rockUpgradedHeldCollider.enabled = true;
		this.climbingAxeCollider.enabled = true;
		this.boneHeldCollider.enabled = true;
		this.katanaHeldCollider.enabled = true;
		this.spearUpgradedCollider.enabled = true;
		this.tennisRacketHeldCollider.enabled = true;
	}

	public void disableWeapon()
	{
		if (this.Remote)
		{
			return;
		}
		LocalPlayer.Inventory.AttackEnded.Invoke();
		if (this.mainWeaponCollider)
		{
			this.mainWeaponCollider.enabled = false;
		}
		this.axeCraftedCollider.enabled = false;
		this.stickCollider1.enabled = false;
		this.stickCollider2.enabled = false;
		this.axeCollider.enabled = false;
		this.rockCollider.enabled = false;
		this.spearCollider.enabled = false;
		this.fireStickCollider1.enabled = false;
		this.axeRustyCollider.enabled = false;
		this.axePlaneCollider.enabled = false;
		this.armHeldCollider.enabled = false;
		this.legHeldCollider.enabled = false;
		this.headHeldCollider.enabled = false;
		this.clubCraftedHeldCollider.enabled = false;
		this.clubHeldCollider.enabled = false;
		this.skullHeldCollider.enabled = false;
		this.stickUpgradedHeldCollider.enabled = false;
		this.stickUpgradedHeldCollider2.enabled = false;
		this.rockUpgradedHeldCollider.enabled = false;
		this.climbingAxeCollider.enabled = false;
		this.boneHeldCollider.enabled = false;
		this.turtleShellHeldCollider.enabled = false;
		this.katanaHeldCollider.enabled = false;
		this.spearUpgradedCollider.enabled = false;
		this.tennisRacketHeldCollider.enabled = false;
	}

	public void disableWeapon2()
	{
		if (this.Remote)
		{
			return;
		}
		LocalPlayer.Inventory.AttackEnded.Invoke();
		this.stickCollider1.enabled = false;
		this.stickCollider2.enabled = false;
		this.stickUpgradedHeldCollider2.enabled = false;
		this.fireStickCollider2.enabled = false;
	}

	public void disableSmashWeapon()
	{
		if (this.Remote)
		{
			return;
		}
		LocalPlayer.Inventory.AttackEnded.Invoke();
		this.axeCraftedCollider.enabled = false;
		this.stickCollider1.enabled = false;
		this.stickCollider2.enabled = false;
		this.axeCollider.enabled = false;
		this.rockCollider.enabled = false;
		this.spearCollider.enabled = false;
		this.fireStickCollider1.enabled = false;
		this.animControl.smashBool = false;
		this.axeRustyCollider.enabled = false;
		this.axePlaneCollider.enabled = false;
		this.armHeldCollider.enabled = false;
		this.legHeldCollider.enabled = false;
		this.headHeldCollider.enabled = false;
		this.skullHeldCollider.enabled = false;
		this.stickUpgradedHeldCollider.enabled = false;
		this.stickUpgradedHeldCollider2.enabled = false;
		this.rockUpgradedHeldCollider.enabled = false;
		this.clubHeldCollider.enabled = false;
		this.clubCraftedHeldCollider.enabled = false;
		this.climbingAxeCollider.enabled = false;
		this.boneHeldCollider.enabled = false;
		this.katanaHeldCollider.enabled = false;
		this.spearUpgradedCollider.enabled = false;
		this.tennisRacketHeldCollider.enabled = false;
	}

	private void disableHitFloat()
	{
		if (this.Remote)
		{
			return;
		}
		this.animator.SetFloatReflected("weaponHit", 0f);
	}

	private void enableDig()
	{
		if (this.Remote)
		{
			return;
		}
		this.dig.doDig();
	}

	private void goToCombo()
	{
		if (this.Remote)
		{
			return;
		}
		this.setup.pmControl.SendEvent("goToCombo");
	}

	private void goToStickCombo()
	{
		if (this.Remote)
		{
			return;
		}
		this.setup.pmControl.SendEvent("goToStickCombo");
	}

	private void testEvent()
	{
		if (this.Remote)
		{
			return;
		}
	}

	private void setHitDirection(int dir)
	{
		if (this.Remote)
		{
			return;
		}
		this.animator.SetIntegerReflected("hitDirection", dir);
	}

	private void goToReset()
	{
		if (this.Remote)
		{
			return;
		}
		this.setup.pmControl.SendEvent("goToReset");
	}

	private void PlayWeaponOneshot(string path)
	{
		FMODCommon.PlayOneshotNetworked(path, this.setup.weaponRight, FMODCommon.NetworkRole.Server);
	}

	private void soundStickSwoosh()
	{
		if (this.Remote)
		{
			return;
		}
		if (!LocalPlayer.Inventory.IsRightHandEmpty())
		{
			if (this.player.IsWeaponBurning)
			{
				this.PlayWeaponOneshot(this.fireStickSwooshEvent.eventPath);
			}
			else
			{
				int itemId = this.player.RightHand._itemId;
				if (this.eventsByItemIdCache.ContainsKey(itemId))
				{
					this.PlayWeaponOneshot(this.eventsByItemIdCache[itemId].eventPath);
				}
			}
		}
	}

	private void soundAxeSwoosh()
	{
		if (this.Remote)
		{
			return;
		}
		if (this.player.IsWeaponBurning)
		{
			this.PlayWeaponOneshot(this.fireStickSwooshEvent.eventPath);
		}
		else
		{
			this.PlayWeaponOneshot(this.axeSwooshEvent.eventPath);
		}
	}

	private void soundRockSwoosh()
	{
		if (this.Remote)
		{
			return;
		}
		this.PlayWeaponOneshot(this.rockSwooshEvent.eventPath);
	}

	private void soundSpearSwoosh()
	{
		if (this.Remote)
		{
			return;
		}
		this.PlayWeaponOneshot(this.spearSwooshEvent.eventPath);
	}

	private void soundBreatheIn()
	{
		if (this.Remote)
		{
			return;
		}
		FMODCommon.PlayOneshot(this.breatheInEvent, base.transform);
	}

	private void soundBreatheOut()
	{
		if (this.Remote)
		{
			return;
		}
		FMODCommon.PlayOneshot(this.breatheOutEvent, base.transform);
	}

	private void PlayFallEvent(float fallParameterValue)
	{
		FMOD.Studio.EventInstance @event = FMOD_StudioSystem.instance.GetEvent(this.fallEvent);
		UnityUtil.ERRCHECK(@event.setParameterValue("fall", fallParameterValue));
		UnityUtil.ERRCHECK(@event.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
		UnityUtil.ERRCHECK(@event.start());
		UnityUtil.ERRCHECK(@event.release());
	}

	public override void OnEvent(SfxFallLight evnt)
	{
		this.PlayFallEvent(0f);
	}

	private void soundFallLight()
	{
		if (this.Remote)
		{
			return;
		}
		if (BoltNetwork.isRunning)
		{
			SfxFallLight.Raise(this.entity, EntityTargets.EveryoneExceptOwner).Send();
		}
		this.PlayFallEvent(0f);
	}

	public override void OnEvent(SfxFallHeavy evnt)
	{
		this.PlayFallEvent(1f);
	}

	private void soundFallHeavy()
	{
		if (this.Remote)
		{
			return;
		}
		if (BoltNetwork.isRunning)
		{
			SfxFallHeavy.Raise(this.entity, EntityTargets.EveryoneExceptOwner).Send();
		}
		this.PlayFallEvent(1f);
	}

	private void playDrawBow()
	{
		if (this.Remote)
		{
			return;
		}
		FMODCommon.PlayOneshot(this.bowDrawEvent.eventPath, this.setup.leftHand);
	}

	private void playChargedSound()
	{
		if (this.Remote)
		{
			return;
		}
	}

	private void resetAnimLayer(int l)
	{
		if (this.Remote)
		{
			return;
		}
		this.animator.SetLayerWeightReflected(l, 0f);
	}

	private void exitClimbRope()
	{
		if (this.Remote)
		{
			return;
		}
		this.animControl.exitClimbMode();
	}

	private void resetAnimSpine()
	{
		if (this.Remote)
		{
			return;
		}
		this.setup.pmControl.SendEvent("toResetSpine");
	}

	private void enableArmIK()
	{
	}

	public void disableArmIK()
	{
		if (this.Remote)
		{
			return;
		}
		if (this.armIK.IsActive)
		{
			this.armIK.IsActive = false;
		}
	}

	private void disableSpine()
	{
		if (this.Remote)
		{
			return;
		}
		base.StartCoroutine("smoothDisableSpine");
	}

	public void enableSpine()
	{
		if (this.Remote)
		{
			return;
		}
		base.StopCoroutine("smoothDisableSpine");
		this.animator.SetLayerWeightReflected(4, 1f);
	}

	private void enableDrawBowBlend()
	{
		if (this.Remote)
		{
			return;
		}
		base.StartCoroutine("drawBowBlend");
	}

	private void disableMainRotator()
	{
		if (this.Remote)
		{
			return;
		}
	}

	private void enableMainRotator()
	{
		if (this.Remote)
		{
			return;
		}
	}

	private void enableHeavyAttackRange()
	{
		if (this.Remote)
		{
			return;
		}
		this.hitTriggerTr.localScale = new Vector3(3f, this.hitTriggerTr.localScale.y, this.hitTriggerTr.localScale.z);
		base.Invoke("disableHeavyAttackRange", 1f);
	}

	private void disableHeavyAttackRange()
	{
		if (this.Remote)
		{
			return;
		}
		this.hitTriggerTr.localScale = new Vector3(1f, this.hitTriggerTr.localScale.y, this.hitTriggerTr.localScale.z);
	}

	private void staminaDrain(float amount)
	{
		if (this.Remote)
		{
			return;
		}
		LocalPlayer.Stats.Stamina -= amount;
	}

	private void lightFire()
	{
		if (this.Remote)
		{
			return;
		}
		int layerMask = 268435456;
		Collider[] array = Physics.OverlapSphere(LocalPlayer.ScriptSetup.leftHand.position, 5f, layerMask);
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Collider collider = array2[i];
			collider.gameObject.SendMessage("receiveLightFire", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void resetDrinkParams()
	{
		if (this.Remote)
		{
			return;
		}
		if (LocalPlayer.FpCharacter.drinking)
		{
			LocalPlayer.AnimControl.playerHeadCollider.enabled = true;
			LocalPlayer.FpCharacter.Locked = false;
			LocalPlayer.FpCharacter.CanJump = true;
			LocalPlayer.CamFollowHead.lockYCam = false;
			LocalPlayer.CamFollowHead.smoothLock = false;
			LocalPlayer.MainRotator.enabled = true;
			LocalPlayer.CamRotator.stopInput = false;
			LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
			LocalPlayer.FpCharacter.enabled = true;
			LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
			LocalPlayer.FpCharacter.Locked = false;
			base.StartCoroutine("smoothEnableSpine");
			LocalPlayer.FpCharacter.drinking = false;
			LocalPlayer.Inventory.ShowAllEquiped();
		}
	}

	private void resetLegTurns()
	{
		if (this.Remote)
		{
			return;
		}
		if ((double)this.animator.GetFloat("overallSpeed") > 0.2)
		{
			LocalPlayer.AnimControl.resetLegInt();
		}
	}

	private void throwSpear()
	{
		if (this.Remote)
		{
			return;
		}
		if (!LocalPlayer.Inventory.IsSlotLocked(Item.EquipmentSlot.RightHand))
		{
			if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._spearId))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.spearSpawn, this.spearThrowPos.position, this.spearThrowPos.rotation) as GameObject;
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				component.AddForce(gameObject.transform.up * this.throwForce, ForceMode.Force);
				LocalPlayer.Inventory.RightHand._held.SendMessage("OnProjectileThrown", gameObject, SendMessageOptions.DontRequireReceiver);
				LocalPlayer.Inventory.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
			}
			else if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._spearUpgradedId))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(this.spearSpawn_upgraded, this.spearThrowPos_upgraded.position, this.spearThrowPos_upgraded.rotation) as GameObject;
				Rigidbody component2 = gameObject2.GetComponent<Rigidbody>();
				component2.AddForce(gameObject2.transform.up * this.throwForce * 1.25f, ForceMode.Force);
				LocalPlayer.Inventory.RightHand._held.SendMessage("OnProjectileThrown", gameObject2, SendMessageOptions.DontRequireReceiver);
				LocalPlayer.Inventory.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
			}
		}
	}

	public void smoothLockCam()
	{
	}

	[DebuggerHidden]
	public IEnumerator smoothDisableSpine()
	{
		animEventsManager.<smoothDisableSpine>c__Iterator31 <smoothDisableSpine>c__Iterator = new animEventsManager.<smoothDisableSpine>c__Iterator31();
		<smoothDisableSpine>c__Iterator.<>f__this = this;
		return <smoothDisableSpine>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator smoothEnableSpine()
	{
		animEventsManager.<smoothEnableSpine>c__Iterator32 <smoothEnableSpine>c__Iterator = new animEventsManager.<smoothEnableSpine>c__Iterator32();
		<smoothEnableSpine>c__Iterator.<>f__this = this;
		return <smoothEnableSpine>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator drawBowBlend()
	{
		animEventsManager.<drawBowBlend>c__Iterator33 <drawBowBlend>c__Iterator = new animEventsManager.<drawBowBlend>c__Iterator33();
		<drawBowBlend>c__Iterator.<>f__this = this;
		return <drawBowBlend>c__Iterator;
	}

	private void syncBookHeld()
	{
		if (this.bookHeld)
		{
			this.bookHeld.SetActive(true);
			this.bookHeld.SendMessage("setOpenBook", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void lockRotation()
	{
	}

	private void unlockRotation()
	{
	}

	[DebuggerHidden]
	private IEnumerator axeStuckInGround()
	{
		animEventsManager.<axeStuckInGround>c__Iterator34 <axeStuckInGround>c__Iterator = new animEventsManager.<axeStuckInGround>c__Iterator34();
		<axeStuckInGround>c__Iterator.<>f__this = this;
		return <axeStuckInGround>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator axeHitTree()
	{
		animEventsManager.<axeHitTree>c__Iterator35 <axeHitTree>c__Iterator = new animEventsManager.<axeHitTree>c__Iterator35();
		<axeHitTree>c__Iterator.<>f__this = this;
		return <axeHitTree>c__Iterator;
	}

	public void resetCuttingTree()
	{
		this.cuttingTree = false;
	}

	[DebuggerHidden]
	private IEnumerator fixRotation()
	{
		animEventsManager.<fixRotation>c__Iterator36 <fixRotation>c__Iterator = new animEventsManager.<fixRotation>c__Iterator36();
		<fixRotation>c__Iterator.<>f__this = this;
		return <fixRotation>c__Iterator;
	}

	private void startCheckArms()
	{
		if (this.Remote)
		{
			return;
		}
		if (!this.introCutScene)
		{
			return;
		}
		if (this.Remote)
		{
			return;
		}
		LocalPlayer.CamFollowHead.smoothUnLock = true;
		LocalPlayer.AnimControl.StartCoroutine("smoothEnableLayerNew", 1);
		LocalPlayer.AnimControl.StartCoroutine("smoothEnableLayerNew", 4);
		base.Invoke("setCheckArms", 0.5f);
		base.Invoke("resetCheckArms", 2.5f);
		LocalPlayer.Stats.PlayWakeMusic();
		base.Invoke("disableCutSceneBool", 1.2f);
		base.Invoke("disableBlockCam", 1.5f);
		this.introCutScene = false;
	}

	private void disableBlockCam()
	{
		if (this.Remote)
		{
			return;
		}
		LocalPlayer.AnimControl.blockCamX = false;
	}

	private void disableCutSceneBool()
	{
		if (this.Remote)
		{
			return;
		}
		LocalPlayer.AnimControl.introCutScene = false;
	}

	private void setCheckArms()
	{
		if (this.Remote)
		{
			return;
		}
		this.animator.SetBoolReflected("checkArms", true);
	}

	private void resetCheckArms()
	{
		if (this.Remote)
		{
			return;
		}
		LocalPlayer.Animator.SetBoolReflected("checkArms", false);
	}

	private void stepLeft()
	{
		if (this.Remote)
		{
			return;
		}
		string footstepForPosition = this.headBob.GetFootstepForPosition();
		if (LocalPlayer.FpCharacter.Grounded && !Clock.InCave && !LocalPlayer.FpCharacter.swimming)
		{
			int prominantTextureIndex = TerrainHelper.GetProminantTextureIndex(base.transform.position);
			if (this.headBob.isSnow)
			{
				Prefabs.Instance.SpawnFootPS(1, this.leftFootSpawnPos.position, this.leftFootSpawnPos.rotation);
			}
			else if (prominantTextureIndex == 6)
			{
				Prefabs.Instance.SpawnFootPS(0, this.leftFootSpawnPos.position, this.leftFootSpawnPos.rotation);
			}
		}
	}

	private void stepRight()
	{
		if (this.Remote)
		{
			return;
		}
		string footstepForPosition = this.headBob.GetFootstepForPosition();
		if (LocalPlayer.FpCharacter.Grounded && !Clock.InCave && !LocalPlayer.FpCharacter.swimming)
		{
			int prominantTextureIndex = TerrainHelper.GetProminantTextureIndex(base.transform.position);
			if (this.headBob.isSnow)
			{
				Prefabs.Instance.SpawnFootPS(1, this.rightFootSpawnPos.position, this.rightFootSpawnPos.rotation);
			}
			else if (prominantTextureIndex == 6)
			{
				Prefabs.Instance.SpawnFootPS(0, this.rightFootSpawnPos.position, this.rightFootSpawnPos.rotation);
			}
		}
	}
}
