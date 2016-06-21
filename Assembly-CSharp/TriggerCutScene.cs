using FMOD.Studio;
using ScionEngine;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class TriggerCutScene : MonoBehaviour
{
	public GameObject PMGui;

	public GameObject IconsAndTextMain;

	public PlayMakerFSM pmTrigger;

	public GameObject SpaceTut;

	public TheForestAtmosphere Atm;

	public planeCrashHeight planeController;

	private planeEvents pEvents;

	private setupPassengers[] passengersScript;

	public Animator planeAnim;

	public GameObject testSavePos;

	public GameObject savedCrashPos;

	public CrashClearing Hull;

	public GameObject MutantOnPlane;

	public GameObject Passengers;

	public GameObject debrisInterior1;

	public GameObject debrisInterior2;

	public GameObject playerPosGo;

	public GameObject[] enableAfterCrash;

	private GameObject Mutant;

	public Transform playerSeatPos;

	public GameObject timmySeatGo;

	private GameObject timmySeat;

	public GameObject PlaneReal;

	public GameObject timmySleepGo;

	public GameObject playerSeatGo;

	public int SmokeTime;

	public GameObject SFX_inplane;

	public GameObject SFX_TakeTimmy;

	public GameObject SFX_Music;

	public GameObject Smoke;

	public int GroundElementsTime;

	public GameObject GroundElements;

	public GameObject GlassAndStuff;

	public GameObject Clouds;

	public GameObject LightsCrash;

	public GameObject LightsFlight;

	public GameObject FrontOfPlane;

	public GameObject clouds;

	public Transform mainHull;

	public Camera Hud;

	public GameObject FakeBody;

	public GameObject Stewardess;

	public Material PlaneCrashOff;

	public Renderer PlaneInterior;

	public Material FlashingSign;

	public Material cloudsMaterial;

	public GameObject PlaneAfterSound;

	private string[] preloadEvents;

	private bool hasPreloadedEvents;

	public bool getBook;

	public bool fakePlaneActive;

	private AmplifyMotionObjectBase[] amplifyBase;

	private bool disabled;

	private float prevMaxVel;

	private ScionPostProcess EyeAdapt;

	private FMOD.Studio.EventInstance attendantDialogueEvent;

	public bool skipOpening
	{
		get;
		private set;
	}

	private void Awake()
	{
		debrisPart2[] componentsInChildren = this.debrisInterior1.GetComponentsInChildren<debrisPart2>(true);
		if (componentsInChildren[0])
		{
			this.debrisInterior2 = componentsInChildren[0].gameObject;
		}
		this.timmySleepGo = this.playerSeatGo.transform.parent.FindChild("planecrash_ANIM_timmyIdle").gameObject;
		this.pEvents = base.transform.root.GetComponent<planeEvents>();
		this.passengersScript = this.Passengers.GetComponentsInChildren<setupPassengers>();
		if (LevelSerializer.IsDeserializing || !Scene.PlaneCrash || !Scene.PlaneCrash.ShowCrash)
		{
			base.enabled = false;
		}
		else if (CoopPeerStarter.DedicatedHost)
		{
			this.planeController.setPlanePosition();
			this.FinalizePlanePosition();
			UnityEngine.Object.Destroy(this);
		}
	}

	[DebuggerHidden]
	public IEnumerator beginPlaneCrash()
	{
		TriggerCutScene.<beginPlaneCrash>c__Iterator1AD <beginPlaneCrash>c__Iterator1AD = new TriggerCutScene.<beginPlaneCrash>c__Iterator1AD();
		<beginPlaneCrash>c__Iterator1AD.<>f__this = this;
		return <beginPlaneCrash>c__Iterator1AD;
	}

	private void resetBook()
	{
		this.playerSeatGo.GetComponent<Animator>().SetBool("toBook", false);
	}

	private void enablePlayerControl()
	{
		UnityEngine.Debug.Log("playerControl enabled at " + Time.time);
		LocalPlayer.FpCharacter.UnLockView();
		LocalPlayer.CamRotator.enabled = true;
		LocalPlayer.MainRotator.enabled = true;
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonDown("Jump") && !this.skipOpening && Clock.planecrash)
		{
			this.SpaceTut.SetActive(false);
			this.LightsFlight.SetActive(false);
			this.pmTrigger.SendEvent("toSkipOpening");
			this.skipOpening = true;
		}
		else
		{
			TheForest.Utils.Input.LockMouse();
		}
	}

	private void setupClouds()
	{
		this.clouds.transform.position = this.mainHull.position;
		this.clouds.transform.rotation = this.mainHull.rotation;
		this.clouds.SetActive(true);
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("CutScene/PlaneClouds"), this.clouds.transform.position, this.clouds.transform.rotation);
		gameObject.transform.parent = this.clouds.transform;
		base.StartCoroutine("moveClouds");
	}

	[DebuggerHidden]
	private IEnumerator moveClouds()
	{
		TriggerCutScene.<moveClouds>c__Iterator1AE <moveClouds>c__Iterator1AE = new TriggerCutScene.<moveClouds>c__Iterator1AE();
		<moveClouds>c__Iterator1AE.<>f__this = this;
		return <moveClouds>c__Iterator1AE;
	}

	private void enableMoBlur()
	{
	}

	private void playAttendantDialogue()
	{
		this.attendantDialogueEvent = FMODCommon.PlayOneshot("event:/ambient/plane_start/flight_attendant", Vector3.zero, new object[0]);
	}

	private void enableAnimation()
	{
		if (this.debrisInterior1)
		{
			this.debrisInterior1.GetComponentInChildren<Animator>().SetBool("startCrash", true);
		}
		this.planeAnim.SetBoolReflected("begin", true);
		this.StopEventEmitter(this.SFX_inplane);
		if (this.timmySleepGo)
		{
			this.timmySleepGo.GetComponent<Animator>().SetBool("crashBegin", true);
		}
		if (this.playerSeatGo)
		{
			this.playerSeatGo.GetComponent<Animator>().SetBool("crashBegin", true);
		}
		setupPassengers[] array = this.passengersScript;
		for (int i = 0; i < array.Length; i++)
		{
			setupPassengers setupPassengers = array[i];
			if (setupPassengers)
			{
				setupPassengers.Invoke("triggerFall1", UnityEngine.Random.Range(0.1f, 0.4f));
				setupPassengers.Invoke("triggerFlyBack", 11.5f);
				setupPassengers.Invoke("triggerFrontSeats", 5.2f);
			}
		}
		this.pEvents.fallForward1();
	}

	private void PlaySounds()
	{
		this.StartEventEmitter(this.SFX_inplane);
		this.StartEventEmitter(this.SFX_Music);
	}

	private void StartEventEmitter(GameObject parent)
	{
		parent.GetComponent<FMOD_StudioEventEmitter>().Play();
	}

	private void StopEventEmitter(GameObject parent)
	{
		parent.GetComponent<FMOD_StudioEventEmitter>().Stop();
	}

	public void StopSounds()
	{
		this.pEvents.stopFMODEvents();
		if (base.transform && base.transform.parent)
		{
			FMOD_AnimationEventHandler[] componentsInChildren = base.transform.parent.GetComponentsInChildren<FMOD_AnimationEventHandler>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				FMOD_AnimationEventHandler fMOD_AnimationEventHandler = componentsInChildren[i];
				fMOD_AnimationEventHandler.enabled = false;
			}
		}
		FMODCommon.ReleaseIfValid(this.attendantDialogueEvent, STOP_MODE.ALLOWFADEOUT);
		this.attendantDialogueEvent = null;
		this.StopEventEmitter(this.SFX_inplane);
		this.StopEventEmitter(this.SFX_TakeTimmy);
		PlaneCrashAudioState.Disable();
	}

	private void ShowDamage()
	{
		if (!this.skipOpening)
		{
			this.GlassAndStuff.SetActive(true);
		}
	}

	private void SmokeOn()
	{
		this.Smoke.SetActive(true);
	}

	private void GroundOn()
	{
		if (this.GroundElements)
		{
			this.GroundElements.SetActive(true);
		}
	}

	[DebuggerHidden]
	private IEnumerator startTimmyCutscene()
	{
		TriggerCutScene.<startTimmyCutscene>c__Iterator1AF <startTimmyCutscene>c__Iterator1AF = new TriggerCutScene.<startTimmyCutscene>c__Iterator1AF();
		<startTimmyCutscene>c__Iterator1AF.<>f__this = this;
		return <startTimmyCutscene>c__Iterator1AF;
	}

	private void startPlayerCrawl()
	{
		setupPlayerCrawl componentInChildren = this.PlaneReal.GetComponentInChildren<setupPlayerCrawl>();
		if (componentInChildren)
		{
			componentInChildren.transform.SendMessage("startCrawl", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void ShowEnemies()
	{
		this.LightsFlight.SetActive(false);
		this.GlassAndStuff.SetActive(false);
		this.PlaneInterior.material = this.PlaneCrashOff;
		this.LightsCrash.SetActive(true);
		this.Mutant = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load("CutScene/mutantTimmyWalkAwayGo"), this.MutantOnPlane.transform.position, this.MutantOnPlane.transform.rotation);
		this.Mutant.transform.localScale = this.MutantOnPlane.transform.localScale;
		this.Mutant.transform.parent = this.PlaneReal.transform;
		if (this.timmySeat)
		{
			UnityEngine.Object.Destroy(this.timmySeat);
		}
		if (this.timmySleepGo)
		{
			UnityEngine.Object.Destroy(this.timmySleepGo);
		}
		if (this.playerSeatGo)
		{
			UnityEngine.Object.Destroy(this.playerSeatGo);
		}
		if (this.Passengers)
		{
			UnityEngine.Object.Destroy(this.Passengers);
		}
		if (this.debrisInterior1)
		{
			UnityEngine.Object.Destroy(this.debrisInterior1);
		}
		Resources.UnloadUnusedAssets();
		UnityEngine.Object.Destroy(this.playerSeatGo);
		base.Invoke("PlayTimmySounds", 1f);
		this.Hull.OnCrash();
	}

	private void PlayTimmySounds()
	{
		this.StartEventEmitter(this.SFX_TakeTimmy);
	}

	private void FinalizePlanePosition()
	{
		if (Scene.TriggerCutScene && Scene.TriggerCutScene.planeController)
		{
			Scene.TriggerCutScene.planeController.enabled = false;
		}
		if (BoltNetwork.isClient)
		{
			this.planeController.transform.position = CoopServerInfo.Instance.state.PlanePosition;
			this.planeController.transform.rotation = CoopServerInfo.Instance.state.PlaneRotation;
		}
		else
		{
			Transform transform = PlaneCrashLocations.finalPositions[PlaneCrashLocations.crashSite].transform;
			this.planeController.transform.position = transform.position;
			this.planeController.transform.rotation = transform.rotation;
		}
		GameObject[] array = this.enableAfterCrash;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			if (gameObject)
			{
				gameObject.SetActive(true);
			}
		}
		Scene.PlaneGreebles.transform.position = this.planeController.transform.position;
		Scene.PlaneGreebles.transform.rotation = this.planeController.transform.rotation;
		Scene.PlaneGreebles.SetActive(true);
	}

	[DebuggerHidden]
	private IEnumerator CleanUp()
	{
		TriggerCutScene.<CleanUp>c__Iterator1B0 <CleanUp>c__Iterator1B = new TriggerCutScene.<CleanUp>c__Iterator1B0();
		<CleanUp>c__Iterator1B.<>f__this = this;
		return <CleanUp>c__Iterator1B;
	}

	[DebuggerHidden]
	private IEnumerator startPlayerInPlane()
	{
		TriggerCutScene.<startPlayerInPlane>c__Iterator1B1 <startPlayerInPlane>c__Iterator1B = new TriggerCutScene.<startPlayerInPlane>c__Iterator1B1();
		<startPlayerInPlane>c__Iterator1B.<>f__this = this;
		return <startPlayerInPlane>c__Iterator1B;
	}

	[DebuggerHidden]
	private IEnumerator resetFov(float set)
	{
		TriggerCutScene.<resetFov>c__Iterator1B2 <resetFov>c__Iterator1B = new TriggerCutScene.<resetFov>c__Iterator1B2();
		<resetFov>c__Iterator1B.set = set;
		<resetFov>c__Iterator1B.<$>set = set;
		return <resetFov>c__Iterator1B;
	}

	private void resetStandBool()
	{
		LocalPlayer.Animator.SetBoolReflected("introStandBool", false);
	}

	private void TurnOffPM()
	{
		this.PMGui.SetActive(false);
	}

	private void skipOpeningAnimation()
	{
		this.EyeAdapt.exposureCompensation = 0f;
		this.skipOpening = true;
		base.StopCoroutine("beginPlaneCrash");
		base.StopCoroutine("moveClouds");
		base.StopCoroutine("startTimmyCutscene");
		base.Invoke("TurnOffPM", 17f);
		base.CancelInvoke("SmokeOn");
		base.CancelInvoke("GroundOn");
		base.CancelInvoke("PlaySounds");
		base.CancelInvoke("ShowDamage");
		this.ShowDamage();
		this.StopSounds();
		base.Invoke("GroundOn", 2f);
		this.enableAnimation();
		this.planeAnim.speed = 60f;
		this.planeController.skipPlaneCrash();
		this.GlassAndStuff.SetActive(false);
		this.LightsFlight.SetActive(false);
		this.PlaneInterior.material = this.PlaneCrashOff;
		this.FrontOfPlane.SetActive(false);
	}

	private void disablePlaneCrash()
	{
		WorkScheduler.FullCycle = true;
		Scene.WorkScheduler.gameObject.SetActive(true);
		Clock.planecrash = false;
		Scene.MutantControler.startSetupFamilies();
		Scene.HudGui.GuiCamC.enabled = true;
		if (this.hasPreloadedEvents)
		{
			FMODCommon.UnloadEvents(this.preloadEvents);
		}
		base.transform.parent = null;
	}

	private void doNavMesh()
	{
		this.disablePlaneAnim();
		this.planeController.doNavMesh();
	}

	private void disablePlaneAnim()
	{
		if (!this.disabled)
		{
			this.planeController.transform.parent = null;
			UnityEngine.Object.Instantiate(this.testSavePos, this.planeController.transform.position, this.planeController.transform.rotation);
			this.planeAnim.enabled = false;
			this.planeController.enabled = false;
			this.disabled = true;
		}
	}
}
