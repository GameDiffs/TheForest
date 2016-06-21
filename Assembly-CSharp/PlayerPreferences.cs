using FMOD.Studio;
using System;
using TheForest.Utils;
using UnityEngine;

public class PlayerPreferences : MonoBehaviour
{
	private const bool RespectUnityDialogSettings = false;

	private const string CustomQualityPath = "/TheForestQualitySettings.dat";

	public bool TitleScene;

	public GameObject Warning32Bits;

	public static int Preset = 1;

	public static float Brightness = 0.5f;

	public static float Volume = 0.5f;

	public static float MusicVolume = 1f;

	public static int VoiceCount = 128;

	public static bool MouseInvert;

	public static float MouseSensitivity = 0.5f;

	public static float Fov = 75f;

	public static int MaxFrameRate = -1;

	public static int ColorGrading;

	public static bool VSync;

	public static bool ShowHud = true;

	public static bool ShowOverlayIcons = true;

	public static bool ShowProjectileReticle = true;

	public static bool UseXInput;

	public static bool LowMemoryMode;

	public static bool ShowPlayerNamesMP = true;

	public static bool ShowStealthMeter = true;

	public static bool UseCrouchToggle;

	private bool CausticsOn;

	public Terrain activeTerrain;

	private string SystemStuff;

	public RTP_LODmanager terrainLOD;

	public static bool is32bit;

	private static bool alreadyLoaded;

	private VCA SFXControl;

	private Bus MusicBus;

	private void Awake()
	{
		PlayerPreferences.Load();
		this.SystemStuff = SystemInfo.operatingSystem;
		if (!this.SystemStuff.Contains("64bit"))
		{
			PlayerPreferences.is32bit = true;
			QualitySettings.masterTextureLimit = 1;
			PlayerPreferences.LowMemoryMode = true;
			if (this.TitleScene && this.Warning32Bits)
			{
				this.Warning32Bits.SetActive(true);
			}
		}
	}

	private void Start()
	{
		if (FMOD_StudioSystem.instance)
		{
			UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getVCA("vca:/SFX", out this.SFXControl));
			UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getBus("bus:/Music", out this.MusicBus));
		}
		else
		{
			Debug.LogError("FMOD_StudioSystem.instance is null, failed to initialize SFXControl & MusicBus");
		}
	}

	public static void Load()
	{
		Debug.Log("PlayerPreferences.Load");
		if (PlayerPreferences.alreadyLoaded)
		{
			return;
		}
		PlayerPreferences.Preset = PlayerPrefs.GetInt("Preset_v16", PlayerPreferences.Preset);
		PlayerPreferences.LowMemoryMode = (PlayerPrefs.GetInt("LowMemoryMode", 0) > 0);
		PlayerPreferences.Brightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
		PlayerPreferences.Volume = PlayerPrefs.GetFloat("Volume", 0.5f);
		PlayerPreferences.MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
		PlayerPreferences.VoiceCount = PlayerPrefs.GetInt("VoiceCount", 128);
		PlayerPreferences.MouseInvert = (PlayerPrefs.GetInt("MouseInvert", (!PlayerPreferences.MouseInvert) ? 0 : 1) > 0);
		PlayerPreferences.MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
		PlayerPreferences.Fov = PlayerPrefs.GetFloat("Fov", 75f);
		int @int = PlayerPrefs.GetInt("MaxFrameRate2", -1);
		Application.targetFrameRate = @int;
		PlayerPreferences.MaxFrameRate = @int;
		PlayerPreferences.VSync = (PlayerPrefs.GetInt("VSync", QualitySettings.vSyncCount) > 0);
		PlayerPreferences.ShowHud = (PlayerPrefs.GetInt("ShowHud", 1) > 0);
		PlayerPreferences.ShowOverlayIcons = (PlayerPrefs.GetInt("ShowOverlayIcons", 1) > 0);
		PlayerPreferences.ShowProjectileReticle = (PlayerPrefs.GetInt("ShowProjectileReticle", 1) > 0);
		PlayerPreferences.UseXInput = (PlayerPrefs.GetInt("UseXInput", 0) > 0);
		PlayerPreferences.ShowPlayerNamesMP = (PlayerPrefs.GetInt("ShowPlayerNamesMP", 1) > 0);
		PlayerPreferences.ShowStealthMeter = (PlayerPrefs.GetInt("ShowStealthMeter", 1) > 0);
		PlayerPreferences.UseCrouchToggle = (PlayerPrefs.GetInt("UseCrouchToggle", 0) > 0);
		if (!TheForestQualitySettings.Load())
		{
			TheForestQualitySettings.CopyPreset(-1);
		}
		QualitySettings.SetQualityLevel((int)TheForestQualitySettings.UserSettings.ShadowLevel);
		if (!TheForestQualitySettings.Load())
		{
			TheForestQualitySettings.CopyPreset(-1);
		}
		PlayerPreferences.ApplyValues();
		PlayerPreferences.alreadyLoaded = true;
	}

	public static void Save()
	{
		Debug.Log("Saving Preferences");
		PlayerPrefs.SetInt("Preset_v16", PlayerPreferences.Preset);
		PlayerPrefs.SetInt("LowMemoryMode", (!PlayerPreferences.LowMemoryMode) ? 0 : 1);
		PlayerPrefs.SetFloat("Brightness", PlayerPreferences.Brightness);
		PlayerPrefs.SetFloat("Volume", PlayerPreferences.Volume);
		PlayerPrefs.SetFloat("MusicVolume", PlayerPreferences.MusicVolume);
		PlayerPrefs.SetInt("VoiceCount", PlayerPreferences.VoiceCount);
		PlayerPrefs.SetInt("MouseInvert", (!PlayerPreferences.MouseInvert) ? 0 : 1);
		PlayerPrefs.SetFloat("MouseSensitivity", PlayerPreferences.MouseSensitivity);
		PlayerPrefs.SetFloat("Fov", PlayerPreferences.Fov);
		PlayerPrefs.SetInt("ColorGrading", PlayerPreferences.ColorGrading);
		TheForestQualitySettings.Save();
		PlayerPrefs.SetInt("VSync", (!PlayerPreferences.VSync) ? 0 : 1);
		PlayerPrefs.SetInt("ShowHud", (!PlayerPreferences.ShowHud) ? 0 : 1);
		PlayerPrefs.SetInt("ShowOverlayIcons", (!PlayerPreferences.ShowOverlayIcons) ? 0 : 1);
		PlayerPrefs.SetInt("ShowProjectileReticle", (!PlayerPreferences.ShowProjectileReticle) ? 0 : 1);
		PlayerPrefs.SetInt("UseXInput", (!PlayerPreferences.UseXInput) ? 0 : 1);
		PlayerPrefs.SetInt("ShowPlayerNamesMP", (!PlayerPreferences.ShowPlayerNamesMP) ? 0 : 1);
		PlayerPrefs.SetInt("ShowStealthMeter", (!PlayerPreferences.ShowStealthMeter) ? 0 : 1);
		PlayerPrefs.SetInt("MaxFrameRate2", PlayerPreferences.MaxFrameRate);
		PlayerPrefs.SetInt("UseCrouchToggle", (!PlayerPreferences.UseCrouchToggle) ? 0 : 1);
		PlayerPrefs.Save();
	}

	public static void ApplyValues()
	{
		Terrain terrain = Terrain.activeTerrain;
		if (terrain)
		{
			if (terrain.detailObjectDistance != TheForestQualitySettings.UserSettings.GrassDistance)
			{
				terrain.detailObjectDistance = TheForestQualitySettings.UserSettings.GrassDistance;
			}
			if (terrain.detailObjectDensity != TheForestQualitySettings.UserSettings.GrassDensity)
			{
				terrain.detailObjectDensity = TheForestQualitySettings.UserSettings.GrassDensity;
			}
		}
		if (Scene.HudGui)
		{
			Scene.HudGui.CheckHudState();
		}
	}

	private void Update()
	{
		Shader.globalMaximumLOD = TheForestQualitySettings.UserSettings.MaterialQualityShaderLOD;
		if (this.activeTerrain)
		{
			this.activeTerrain.heightmapPixelError = (float)Screen.height * TheForestQualitySettings.UserSettings.TerrainQualityPixelErrorPercentage;
		}
		if (AudioListener.volume != PlayerPreferences.Volume)
		{
			AudioListener.volume = PlayerPreferences.Volume;
		}
		if (this.SFXControl != null)
		{
			UnityUtil.ERRCHECK(this.SFXControl.setFaderLevel(PlayerPreferences.Volume));
		}
		if (this.MusicBus != null)
		{
			UnityUtil.ERRCHECK(this.MusicBus.setFaderLevel(PlayerPreferences.MusicVolume));
		}
		if (!this.TitleScene && LocalPlayer.MainCam && LocalPlayer.MainCam.fieldOfView != PlayerPreferences.Fov)
		{
			LocalPlayer.MainCam.fieldOfView = PlayerPreferences.Fov;
		}
		int num = (!PlayerPreferences.VSync) ? 0 : 1;
		if (QualitySettings.vSyncCount != num)
		{
			QualitySettings.vSyncCount = num;
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		if (level == 1)
		{
			TheForestQualitySettings.UserSettings.SetTerrainQuality(TheForestQualitySettings.UserSettings.TerrainQuality);
			PlayerPreferences.ApplyValues();
		}
	}
}
