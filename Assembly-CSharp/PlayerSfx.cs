using Bolt;
using FMOD.Studio;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

public class PlayerSfx : EntityEventListener<IPlayerState>
{
	private bool Playing;

	public bool Remote;

	public GameObject SfxPlayer;

	public GameObject SfxGUI;

	public GameObject SfxRustle;

	public GameObject MPCas;

	public GameObject DeathMusic;

	public static bool MusicPlaying;

	public string PlantRustleEvent;

	public string SplashEvent;

	public string StructureBreakEvent;

	public string StructureFallEvent;

	public float SplashSpeedMinimum = 10f;

	public float SplashSpeedMaximum = 30f;

	public string WhooshEvent;

	public string PageTurnEvent;

	public string FlareEvent;

	public string FlareDryFireEvent;

	public string ThrowEvent;

	public string PickUpEvent;

	public string PutDownEvent;

	public string PlaceGhostEvent;

	public string EatSoftEvent;

	public string EatMeatEvent;

	public string EatPoisonEvent;

	public string EatMedsEvent;

	public string DrinkEvent;

	public string DrinkFromWaterSourceEvent;

	public string StaminaBreathEvent;

	public string LighterSparkEvent;

	public string HammerEvent;

	public string HurtEvent;

	public string JumpEvent;

	public string OpenInventoryEvent;

	public string CloseInventoryEvent;

	public string visWarningEvent;

	public string bowSnapEvent;

	public string bowDrawEvent;

	public string MusicInjuredEvent;

	public string CoinEvent;

	public string TorchLightOnEvent;

	public string TorchLightOffEvent;

	public string TwinkleEvent;

	public string RemovalEvent;

	public string FindBodyTwinkleEvent;

	public string WalkyTalkyEvent;

	public string WokenByEnemiesEvent;

	public string SightedByEnemyEvent;

	public string DigEvent;

	public string PencilTickEvent;

	public string TaskCompletedEvent;

	public string BreakWoodEvent;

	public string UpgradeSuccessEvent;

	public string[] MusicTrackPaths;

	private Buoyancy Buoyancy;

	private FMOD.Studio.EventInstance musicTrack;

	private FMOD.Studio.EventInstance staminaBreathInstance;

	private FMOD.Studio.EventInstance visWarningInstance;

	private FMOD.Studio.EventInstance walkyTalkyInstance;

	private FMOD.Studio.EventInstance afterStormInstance;

	private bool plantRustleEnabled = true;

	private bool jumpEnabled = true;

	private bool sightedByEnemyEnabled = true;

	private Vector3 prevPosition;

	private float flatVelocity;

	private bool immersed;

	private void Awake()
	{
		if (this.Remote)
		{
			return;
		}
		this.Buoyancy = base.GetComponent<Buoyancy>();
	}

	private string[] AllEventPaths()
	{
		return new string[]
		{
			this.PlantRustleEvent,
			this.StructureBreakEvent,
			this.StructureFallEvent,
			this.SplashEvent,
			this.WhooshEvent,
			this.PageTurnEvent,
			this.FlareEvent,
			this.FlareDryFireEvent,
			this.ThrowEvent,
			this.PickUpEvent,
			this.PutDownEvent,
			this.PlaceGhostEvent,
			this.EatSoftEvent,
			this.EatMeatEvent,
			this.EatPoisonEvent,
			this.EatMedsEvent,
			this.DrinkEvent,
			this.DrinkFromWaterSourceEvent,
			this.StaminaBreathEvent,
			this.LighterSparkEvent,
			this.HammerEvent,
			this.HurtEvent,
			this.JumpEvent,
			this.OpenInventoryEvent,
			this.CloseInventoryEvent,
			this.visWarningEvent,
			this.bowSnapEvent,
			this.bowDrawEvent,
			this.MusicInjuredEvent,
			this.CoinEvent,
			this.TorchLightOnEvent,
			this.TorchLightOffEvent,
			this.TwinkleEvent,
			this.RemovalEvent,
			this.FindBodyTwinkleEvent,
			this.WalkyTalkyEvent,
			this.WokenByEnemiesEvent,
			this.SightedByEnemyEvent,
			this.DigEvent,
			this.PencilTickEvent,
			this.TaskCompletedEvent,
			this.BreakWoodEvent,
			this.UpgradeSuccessEvent
		};
	}

	private void Start()
	{
		string[] array = this.AllEventPaths();
		for (int i = 0; i < array.Length; i++)
		{
			string path = array[i];
			FMOD_StudioSystem.PreloadEvent(path);
		}
		if (this.Remote)
		{
			return;
		}
		this.prevPosition = base.GetComponent<Rigidbody>().position;
	}

	private static void Set3DAttributes(FMOD.Studio.EventInstance evt, ATTRIBUTES_3D attributes)
	{
		if (evt != null && evt.isValid())
		{
			UnityUtil.ERRCHECK(evt.set3DAttributes(attributes));
		}
	}

	private void Update()
	{
		if (this.Remote)
		{
			return;
		}
		PLAYBACK_STATE pLAYBACK_STATE = PLAYBACK_STATE.STOPPED;
		if (this.musicTrack != null)
		{
			UnityUtil.ERRCHECK(this.musicTrack.getPlaybackState(out pLAYBACK_STATE));
		}
		if (pLAYBACK_STATE != PLAYBACK_STATE.STOPPED)
		{
			PlayerSfx.MusicPlaying = true;
		}
		else
		{
			PlayerSfx.MusicPlaying = false;
		}
		if (base.transform.hasChanged)
		{
			base.transform.hasChanged = false;
			ATTRIBUTES_3D attributes = UnityUtil.to3DAttributes(this.SfxPlayer, null);
			PlayerSfx.Set3DAttributes(this.staminaBreathInstance, attributes);
			PlayerSfx.Set3DAttributes(this.walkyTalkyInstance, attributes);
			PlayerSfx.Set3DAttributes(this.musicTrack, attributes);
			PlayerSfx.Set3DAttributes(this.afterStormInstance, this.SfxPlayer.transform.position.to3DAttributes());
		}
		if (this.afterStormInstance != null && !this.afterStormInstance.isValid())
		{
			this.afterStormInstance = null;
		}
		Vector3 vector = (base.GetComponent<Rigidbody>().position - this.prevPosition) / Time.deltaTime;
		this.prevPosition = base.GetComponent<Rigidbody>().position;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		this.flatVelocity = vector2.magnitude;
		if (!this.Buoyancy.InWater)
		{
			this.immersed = false;
		}
		else if (!this.immersed && !LocalPlayer.FpCharacter.Grounded && LocalPlayer.FpCharacter.IsAboveWaistDeep())
		{
			this.immersed = true;
			float num = Mathf.Clamp(this.SplashSpeedMaximum - this.SplashSpeedMinimum, 0f, this.SplashSpeedMaximum);
			float num2 = (this.Buoyancy.LastWaterEnterSpeed - this.SplashSpeedMinimum) / num;
			if (num2 >= 0f)
			{
				FMOD.Studio.EventInstance @event = FMOD_StudioSystem.instance.GetEvent(this.SplashEvent);
				UnityUtil.ERRCHECK(@event.set3DAttributes(UnityUtil.to3DAttributes(this.SfxPlayer, null)));
				UnityUtil.ERRCHECK(@event.setParameterValue("speed", Mathf.Clamp01(num2)));
				UnityUtil.ERRCHECK(@event.start());
				UnityUtil.ERRCHECK(@event.release());
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.Remote)
		{
			return;
		}
		if (other.gameObject.CompareTag("SmallTree"))
		{
			this.PlayPlantRustle();
		}
	}

	public void PlayStructureBreak(GameObject emiter, float size)
	{
		FMOD.Studio.EventInstance @event = FMOD_StudioSystem.instance.GetEvent(this.StructureBreakEvent);
		UnityUtil.ERRCHECK(@event.set3DAttributes(UnityUtil.to3DAttributes(emiter, null)));
		UnityUtil.ERRCHECK(@event.setParameterValue("size", Mathf.Clamp01(size)));
		UnityUtil.ERRCHECK(@event.start());
		UnityUtil.ERRCHECK(@event.release());
	}

	public void PlayStructureFall(GameObject emiter, float size)
	{
		FMOD.Studio.EventInstance @event = FMOD_StudioSystem.instance.GetEvent(this.StructureFallEvent);
		UnityUtil.ERRCHECK(@event.set3DAttributes(UnityUtil.to3DAttributes(emiter, null)));
		UnityUtil.ERRCHECK(@event.setParameterValue("size", Mathf.Clamp01(size)));
		UnityUtil.ERRCHECK(@event.start());
		UnityUtil.ERRCHECK(@event.release());
	}

	public void PlayPlantRustle()
	{
		if (this.plantRustleEnabled && FMOD_StudioSystem.instance)
		{
			FMOD.Studio.EventInstance @event = FMOD_StudioSystem.instance.GetEvent(this.PlantRustleEvent);
			UnityUtil.ERRCHECK(@event.set3DAttributes(UnityUtil.to3DAttributes(this.SfxRustle, null)));
			UnityUtil.ERRCHECK(@event.setParameterValue("speed", LocalPlayer.FpCharacter.CalculateSpeedParameter(this.flatVelocity)));
			UnityUtil.ERRCHECK(@event.start());
			UnityUtil.ERRCHECK(@event.release());
			this.plantRustleEnabled = false;
			base.Invoke("EnablePlantRustle", 0.2f);
		}
	}

	private void EnablePlantRustle()
	{
		this.plantRustleEnabled = true;
	}

	private FMOD.Studio.EventInstance PlayEvent(string path, GameObject gameObject)
	{
		return this.PlayEvent(path, gameObject.transform.position);
	}

	private FMOD.Studio.EventInstance PlayEvent(string path, Vector3 position)
	{
		if (!this.Remote && this.entity.IsAttached())
		{
			FmodOneShot fmodOneShot = FmodOneShot.Create(GlobalTargets.Others, ReliabilityModes.Unreliable);
			fmodOneShot.EventPath = CoopAudioEventDb.FindId(path);
			fmodOneShot.Position = position;
			fmodOneShot.Send();
		}
		return (!FMOD_StudioSystem.instance) ? null : FMOD_StudioSystem.instance.PlayOneShot(path, position, null);
	}

	public void PlayTorchOn()
	{
		this.PlayEvent(this.TorchLightOnEvent, this.SfxPlayer);
	}

	public void PlayTorchOff()
	{
		this.PlayEvent(this.TorchLightOffEvent, this.SfxPlayer);
	}

	public void PlayCoinsSfx()
	{
		this.PlayEvent(this.CoinEvent, this.SfxPlayer);
	}

	public void PlayShootFlareSfx()
	{
		this.PlayEvent(this.FlareEvent, this.SfxPlayer);
	}

	public void PlayDryFlareFireSfx()
	{
		this.PlayEvent(this.FlareDryFireEvent, this.SfxPlayer);
	}

	public void PlayPickUp()
	{
		this.PlayEvent(this.PickUpEvent, this.SfxPlayer);
	}

	public void PlayWhoosh()
	{
		this.PlayEvent(this.WhooshEvent, this.SfxPlayer);
	}

	public void PlayThrow()
	{
		this.PlayEvent(this.ThrowEvent, this.SfxPlayer);
	}

	public void PlayPutDown(GameObject location)
	{
		this.PlayEvent(this.PutDownEvent, location);
	}

	public void PlayPlaceGhost()
	{
		this.PlayEvent(this.PlaceGhostEvent, this.SfxPlayer);
	}

	public void PlayTwinkle()
	{
		this.PlayEvent(this.TwinkleEvent, this.SfxGUI);
	}

	public void PlayInventorySound(Item.SFXCommands command)
	{
		string path = null;
		switch (command)
		{
		case Item.SFXCommands.None:
			return;
		case Item.SFXCommands.PlayPlantRustle:
			path = this.PlantRustleEvent;
			break;
		case Item.SFXCommands.PlayTorchOn:
			path = this.TorchLightOnEvent;
			break;
		case Item.SFXCommands.PlayTorchOff:
			path = this.TorchLightOffEvent;
			break;
		case Item.SFXCommands.PlayCoinsSfx:
			path = this.CoinEvent;
			break;
		case Item.SFXCommands.PlayShootFlareSfx:
			path = this.FlareEvent;
			break;
		case Item.SFXCommands.PlayPickUp:
			path = this.PickUpEvent;
			break;
		case Item.SFXCommands.PlayWhoosh:
			path = this.WhooshEvent;
			break;
		case Item.SFXCommands.PlayTwinkle:
			path = this.TwinkleEvent;
			break;
		case Item.SFXCommands.PlayRemove:
			path = this.RemovalEvent;
			break;
		case Item.SFXCommands.PlayEat:
			path = this.EatSoftEvent;
			break;
		case Item.SFXCommands.PlayDrink:
			path = this.DrinkEvent;
			break;
		case Item.SFXCommands.PlayHurtSound:
			path = this.HurtEvent;
			break;
		case Item.SFXCommands.PlayHammer:
			path = this.HammerEvent;
			break;
		case Item.SFXCommands.PlayFindBodyTwinkle:
			path = this.FindBodyTwinkleEvent;
			break;
		case Item.SFXCommands.PlayColdSfx:
		case Item.SFXCommands.StopColdSfx:
		case Item.SFXCommands.PlayCough:
		case Item.SFXCommands.PlayKillRabbit:
		case Item.SFXCommands.playVisWarning:
		case Item.SFXCommands.stopVisWarning:
		case Item.SFXCommands.PlayMusicTrack:
		case Item.SFXCommands.StopMusic:
		case Item.SFXCommands.PlayInjured:
		case Item.SFXCommands.StopPlaying:
		case Item.SFXCommands.StartWalkyTalky:
		case Item.SFXCommands.StopWalkyTalky:
			base.SendMessage(command.ToString());
			return;
		case Item.SFXCommands.PlayStaminaBreath:
			path = this.StaminaBreathEvent;
			break;
		case Item.SFXCommands.PlayLighterSound:
			path = this.LighterSparkEvent;
			break;
		case Item.SFXCommands.PlayTurnPage:
			path = this.PageTurnEvent;
			break;
		case Item.SFXCommands.PlayOpenInventory:
			path = this.OpenInventoryEvent;
			break;
		case Item.SFXCommands.PlayCloseInventory:
			path = this.CloseInventoryEvent;
			break;
		case Item.SFXCommands.PlayBowSnap:
			path = this.bowSnapEvent;
			break;
		case Item.SFXCommands.PlayBowDraw:
			path = this.bowDrawEvent;
			break;
		case Item.SFXCommands.PlayDryFlareFireSfx:
			path = this.FlareDryFireEvent;
			break;
		case Item.SFXCommands.PlayEatMeds:
			path = this.EatMedsEvent;
			break;
		}
		FMODCommon.PlayOneshot(path, this.SfxGUI.transform);
	}

	private static Vector3 FindClosestPoint(GameObject gameObject, Vector3 position)
	{
		Vector3 vector = gameObject.transform.position;
		float num = Vector3.SqrMagnitude(vector - position);
		Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Collider collider = componentsInChildren[i];
			Vector3 vector2 = collider.ClosestPointOnBounds(position);
			float num2 = Vector3.SqrMagnitude(vector2 - position);
			if (num2 < num)
			{
				vector = vector2;
				num = num2;
			}
		}
		return vector;
	}

	public void PlayBuildingComplete(GameObject building, bool networkSync = false)
	{
		Vector3 position = PlayerSfx.FindClosestPoint(building, base.transform.position);
		if (networkSync)
		{
			this.PlayEvent(this.TwinkleEvent, position);
		}
		else
		{
			FMOD_StudioSystem.instance.PlayOneShot(this.TwinkleEvent, position, null);
		}
	}

	public void PlayBuildingRepair(GameObject building)
	{
		Vector3 position = PlayerSfx.FindClosestPoint(building, base.transform.position);
		FMOD_StudioSystem.instance.PlayOneShot(this.HammerEvent, position, null);
	}

	public void PlayRemove()
	{
		this.PlayEvent(this.RemovalEvent, this.SfxGUI);
	}

	public override void OnEvent(SfxEat evnt)
	{
		this.PlayEat();
	}

	public void PlayEat()
	{
		this.PlayEvent(this.EatSoftEvent, this.SfxPlayer);
	}

	public void PlayEatPoison()
	{
		this.PlayEvent(this.EatPoisonEvent, this.SfxPlayer);
	}

	public void PlayEatMeat()
	{
		this.PlayEvent(this.EatMeatEvent, this.SfxPlayer);
	}

	public void PlayEatMeds()
	{
		this.PlayEvent(this.EatMedsEvent, this.SfxPlayer);
	}

	public override void OnEvent(SfxDrink evnt)
	{
		this.PlayDrink();
	}

	public void PlayDrink()
	{
		this.PlayEvent(this.DrinkEvent, this.SfxPlayer);
	}

	public void PlayDrinkFromWaterSource()
	{
		this.PlayEvent(this.DrinkFromWaterSourceEvent, this.SfxPlayer);
	}

	public override void OnEvent(SfxHurt evnt)
	{
		this.PlayHurtSound();
	}

	public void PlayHurtSound()
	{
		this.PlayEvent(this.HurtEvent, this.SfxPlayer);
	}

	public void PlayJumpSound()
	{
		if (this.jumpEnabled)
		{
			this.PlayEvent(this.JumpEvent, this.SfxPlayer);
			this.jumpEnabled = false;
			base.Invoke("EnableJump", 0.333333343f);
		}
	}

	private void EnableJump()
	{
		this.jumpEnabled = true;
	}

	public override void OnEvent(SfxHammer evnt)
	{
		this.PlayHammer();
	}

	public void PlayHammer()
	{
		this.PlayEvent(this.HammerEvent, this.SfxPlayer);
	}

	public void PlayFindBodyTwinkle()
	{
		this.PlayEvent(this.FindBodyTwinkleEvent, this.SfxGUI);
	}

	public void PlayColdSfx()
	{
	}

	public void StopColdSfx()
	{
	}

	public void PlayCough()
	{
	}

	public void PlayStaminaBreath()
	{
		PLAYBACK_STATE pLAYBACK_STATE = PLAYBACK_STATE.STOPPED;
		if (this.staminaBreathInstance != null && this.staminaBreathInstance.isValid())
		{
			UnityUtil.ERRCHECK(this.staminaBreathInstance.getPlaybackState(out pLAYBACK_STATE));
		}
		if (pLAYBACK_STATE == PLAYBACK_STATE.STOPPED)
		{
			this.staminaBreathInstance = this.PlayEvent(this.StaminaBreathEvent, this.SfxPlayer);
		}
	}

	public void PlayLighterSound()
	{
		this.PlayEvent(this.LighterSparkEvent, this.SfxPlayer);
	}

	public void PlayKillRabbit()
	{
	}

	public void PlayTurnPage()
	{
		this.PlayEvent(this.PageTurnEvent, this.SfxPlayer);
	}

	public void PlayOpenInventory()
	{
		this.PlayEvent(this.OpenInventoryEvent, this.SfxPlayer);
	}

	public void PlayCloseInventory()
	{
		this.PlayEvent(this.CloseInventoryEvent, this.SfxPlayer);
	}

	[DebuggerHidden]
	public IEnumerator playVisWarning()
	{
		PlayerSfx.<playVisWarning>c__Iterator190 <playVisWarning>c__Iterator = new PlayerSfx.<playVisWarning>c__Iterator190();
		<playVisWarning>c__Iterator.<>f__this = this;
		return <playVisWarning>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator stopVisWarning()
	{
		PlayerSfx.<stopVisWarning>c__Iterator191 <stopVisWarning>c__Iterator = new PlayerSfx.<stopVisWarning>c__Iterator191();
		<stopVisWarning>c__Iterator.<>f__this = this;
		return <stopVisWarning>c__Iterator;
	}

	public void PlayBowSnap()
	{
		this.PlayEvent(this.bowSnapEvent, this.SfxPlayer);
	}

	public void PlayBowDraw()
	{
		this.PlayEvent(this.bowDrawEvent, this.SfxPlayer);
	}

	public FMOD.Studio.EventInstance InstantiateMusicTrack(int trackNum)
	{
		return FMOD_StudioSystem.instance.GetEvent(this.MusicTrackPaths[trackNum]);
	}

	public void PlayMusicTrack(int trackNum)
	{
		this.StopMusic();
		this.musicTrack = this.InstantiateMusicTrack(trackNum);
		UnityUtil.ERRCHECK(this.musicTrack.set3DAttributes(UnityUtil.to3DAttributes(this.SfxPlayer, null)));
		UnityUtil.ERRCHECK(this.musicTrack.start());
	}

	public void StopMusic()
	{
		if (this.musicTrack != null)
		{
			UnityUtil.ERRCHECK(this.musicTrack.stop(STOP_MODE.ALLOWFADEOUT));
			UnityUtil.ERRCHECK(this.musicTrack.release());
			this.musicTrack = null;
		}
	}

	public void PlayInjured()
	{
		if (!this.Playing)
		{
			this.DeathMusic.SetActive(true);
			base.Invoke("StopPlaying", 6f);
			this.Playing = true;
		}
	}

	public void StopPlaying()
	{
		this.DeathMusic.SetActive(false);
		this.Playing = false;
	}

	public FMOD.Studio.EventInstance RelinquishMusicTrack()
	{
		FMOD.Studio.EventInstance result = this.musicTrack;
		this.musicTrack = null;
		return result;
	}

	public void SetMusicTrack(FMOD.Studio.EventInstance evt)
	{
		this.StopMusic();
		this.musicTrack = evt;
		UnityUtil.ERRCHECK(this.musicTrack.set3DAttributes(UnityUtil.to3DAttributes(this.SfxPlayer, null)));
	}

	public void StartWalkyTalky()
	{
		this.walkyTalkyInstance = FMOD_StudioSystem.instance.GetEvent(this.WalkyTalkyEvent);
		UnityUtil.ERRCHECK(this.walkyTalkyInstance.set3DAttributes(UnityUtil.to3DAttributes(this.SfxPlayer, null)));
		UnityUtil.ERRCHECK(this.walkyTalkyInstance.start());
	}

	public void StopWalkyTalky()
	{
		if (this.walkyTalkyInstance != null)
		{
			CueInstance cueInstance = null;
			UnityUtil.ERRCHECK(this.walkyTalkyInstance.getCue("KeyOff", out cueInstance));
			UnityUtil.ERRCHECK(cueInstance.trigger());
			UnityUtil.ERRCHECK(this.walkyTalkyInstance.release());
			this.walkyTalkyInstance = null;
		}
	}

	public void PlayWokenByEnemies()
	{
		this.PlayEvent(this.WokenByEnemiesEvent, this.SfxPlayer);
	}

	public void PlaySightedByEnemy(GameObject location)
	{
		if (this.sightedByEnemyEnabled)
		{
			this.PlayEvent(this.SightedByEnemyEvent, location);
			this.sightedByEnemyEnabled = false;
			base.Invoke("EnableSightedByEnemy", 5f);
		}
	}

	public void PlayDigDirtPile(GameObject dirtPile)
	{
		this.PlayEvent(this.DigEvent, dirtPile);
	}

	public void PlayTaskAvailable()
	{
		this.PlayEvent(this.PencilTickEvent, this.SfxPlayer);
	}

	public void PlayTaskCompleted()
	{
		this.PlayEvent(this.TaskCompletedEvent, this.SfxPlayer);
	}

	public void PlayBreakWood(GameObject wood)
	{
		this.PlayEvent(this.BreakWoodEvent, wood);
	}

	public void PlayUpgradeSuccess(GameObject upgradeView)
	{
		this.PlayEvent(this.UpgradeSuccessEvent, upgradeView);
	}

	public void PlayAfterStorm()
	{
		FMODCommon.ReleaseIfValid(this.afterStormInstance, STOP_MODE.ALLOWFADEOUT);
		this.afterStormInstance = FMODCommon.PlayOneshot("event:/ambient/amb_streamed/after_storm", base.transform);
	}

	private void EnableSightedByEnemy()
	{
		this.sightedByEnemyEnabled = true;
	}

	private void DieTrap(int type)
	{
		if (type == 3)
		{
			FMODCommon.PlayOneshotNetworked("event:/player/player_vox/jump_vox", base.transform, FMODCommon.NetworkRole.Server);
			FMODCommon.PlayOneshot("event:/player/foley/body_fall", base.transform.position, FMODCommon.NetworkRole.Server, new object[]
			{
				"fall",
				0.8f
			});
		}
	}
}
