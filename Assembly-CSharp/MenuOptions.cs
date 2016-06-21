using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class MenuOptions : MonoBehaviour
{
	public UIPopupList Preset;

	public UIPopupList MaterialQuality;

	public UIPopupList Antialias;

	public UIPopupList Gi;

	public UIPopupList Bloom;

	public UIPopupList FilmGrain;

	public UIPopupList ChromaticAberation;

	public UIPopupList Dof;

	public UIPopupList SunshineOcclusion;

	public UIPopupList RenderType;

	public UIPopupList Shadows;

	public UIPopupList FarShadows;

	public UIPopupList Scatter;

	public UIPopupList SSAOType;

	public UIPopupList SSAO;

	public UIPopupList MotionBlur;

	public UIPopupList Grass;

	public UIPopupList GrassD;

	public UIPopupList ColorGrading;

	public UIPopupList Resolution;

	public UIPopupList DrawDistance;

	public UIPopupList TerrainQuality;

	public UIPopupList TextureQuality;

	public UIPopupList MaxFrameRate;

	public UIPopupList ReflexionMode;

	public UIPopupList OceanQuality;

	public UIPopupList MouseInvert;

	public UISlider MouseSensitivity;

	public UIPopupList Fullscreen;

	public UIPopupList VSync;

	public UIPopupList ShowHud;

	public UIPopupList ShowOverlayIcons;

	public UIPopupList ShowProjectileReticle;

	public UIPopupList ShowPlayerNamesMP;

	public UIPopupList ShowStealthMeter;

	public UIPopupList UseXInput;

	public UIPopupList CrouchMode;

	public UIToggle LowMemoryMode;

	public UISlider Volume;

	public UISlider VolumeMusic;

	public UIPopupList VoiceCount;

	public UISlider Fov;

	private int ignoreCounter;

	private bool shouldSave;

	private bool IgnoreEvents
	{
		get
		{
			return this.ignoreCounter > 0;
		}
	}

	private void BeginIgnoreEvents()
	{
		this.ignoreCounter++;
	}

	private void EndIgnoreEvents()
	{
		this.ignoreCounter--;
	}

	private void OnDisable()
	{
		if (this.shouldSave)
		{
			this.Save();
		}
		TheForest.Utils.Input.player.controllers.maps.SetMapsEnabled(false, ControllerType.Joystick, "Menu");
	}

	private void OnEnable()
	{
		this.shouldSave = true;
		TheForest.Utils.Input.player.controllers.maps.SetMapsEnabled(true, ControllerType.Joystick, "Menu");
	}

	private void Update()
	{
		if (PlayerPreferences.LowMemoryMode)
		{
			this.SSAO.enabled = false;
		}
		else
		{
			this.SSAO.enabled = true;
		}
	}

	private void Awake()
	{
		Resolution[] resolutions = Screen.resolutions;
		this.Resolution.items = new List<string>();
		Resolution[] array = resolutions;
		for (int i = 0; i < array.Length; i++)
		{
			Resolution resolution = array[i];
			this.Resolution.items.Add(string.Format("{0}x{1}", resolution.width, resolution.height));
		}
		EventDelegate.Add(this.Preset.onChange, new EventDelegate.Callback(this.OnChangePreset));
		EventDelegate.Add(this.Antialias.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Shadows.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.FarShadows.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Scatter.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.SSAOType.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.SSAO.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.RenderType.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Bloom.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.FilmGrain.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.ChromaticAberation.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Dof.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.SunshineOcclusion.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.MotionBlur.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.MaterialQuality.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Grass.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.GrassD.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.ColorGrading.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Resolution.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.DrawDistance.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.TerrainQuality.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.TextureQuality.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.MaxFrameRate.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.ReflexionMode.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.OceanQuality.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.MouseInvert.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.MouseSensitivity.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Fov.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Fullscreen.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.VSync.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.ShowHud.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.ShowOverlayIcons.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.ShowProjectileReticle.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.ShowPlayerNamesMP.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.ShowStealthMeter.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.UseXInput.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.CrouchMode.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.LowMemoryMode.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.Volume.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.VolumeMusic.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Add(this.VoiceCount.onChange, new EventDelegate.Callback(this.OnChange));
		this.BeginIgnoreEvents();
	}

	[DebuggerHidden]
	private IEnumerator Start()
	{
		MenuOptions.<Start>c__Iterator176 <Start>c__Iterator = new MenuOptions.<Start>c__Iterator176();
		<Start>c__Iterator.<>f__this = this;
		return <Start>c__Iterator;
	}

	private void OnDestroy()
	{
		EventDelegate.Remove(this.Preset.onChange, new EventDelegate.Callback(this.OnChangePreset));
		EventDelegate.Remove(this.Antialias.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Shadows.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.FarShadows.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.SSAOType.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.SSAO.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Scatter.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.RenderType.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Bloom.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.FilmGrain.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.ChromaticAberation.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Dof.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.SunshineOcclusion.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.MotionBlur.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.MaterialQuality.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Grass.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.GrassD.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.ColorGrading.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Resolution.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.DrawDistance.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.TerrainQuality.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.TextureQuality.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.MaxFrameRate.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.ReflexionMode.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.OceanQuality.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.MouseInvert.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.MouseSensitivity.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Fov.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Fullscreen.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.VSync.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.ShowHud.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.ShowOverlayIcons.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.ShowProjectileReticle.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.ShowPlayerNamesMP.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.ShowStealthMeter.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.UseXInput.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.CrouchMode.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.LowMemoryMode.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.Volume.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.VolumeMusic.onChange, new EventDelegate.Callback(this.OnChange));
		EventDelegate.Remove(this.VoiceCount.onChange, new EventDelegate.Callback(this.OnChange));
	}

	private int PopupIndex(UIPopupList list)
	{
		return list.items.IndexOf(list.value);
	}

	private void OnChangePreset()
	{
		if (this.IgnoreEvents)
		{
			return;
		}
		this.BeginIgnoreEvents();
		int num = PlayerPreferences.Preset = this.PopupIndex(this.Preset);
		if (num < 0)
		{
			return;
		}
		bool flag = num == this.Preset.items.Count - 1;
		TheForestQualitySettings.UserSettings.Preset = (TheForestQualitySettings.PresetLevels)num;
		if (!flag)
		{
			TheForestQualitySettings.CopyPreset(num);
			QualitySettings.SetQualityLevel((int)TheForestQualitySettings.UserSettings.ShadowLevel);
			TheForestQualitySettings.CopyPreset(num);
		}
		try
		{
			if (flag)
			{
				int num2 = (int)TheForestQualitySettings.UserSettings.ShadowLevel;
				if (num2 >= 0)
				{
					if (PlayerPreferences.is32bit)
					{
						num2 = Mathf.Max(num2, 1);
					}
					this.Shadows.value = this.Shadows.items[num2];
					QualitySettings.SetQualityLevel(num2);
					if (PlayerPreferences.is32bit)
					{
						TheForestQualitySettings.UserSettings.SetTextureQuality(TheForestQualitySettings.UserSettings.TextureQuality);
					}
				}
				int scatterLevel = TheForestQualitySettings.GetScatterLevel();
				if (scatterLevel >= 0)
				{
					this.Scatter.value = this.Scatter.items[scatterLevel];
				}
				int grassLevel = TheForestQualitySettings.GetGrassLevel();
				int grassLevelDensity = TheForestQualitySettings.GetGrassLevelDensity();
				if (grassLevel >= 0)
				{
					this.Grass.value = this.Grass.items[grassLevel];
				}
				if (grassLevelDensity >= 0)
				{
					this.GrassD.value = this.GrassD.items[grassLevelDensity];
				}
			}
			else
			{
				int num3 = (int)TheForestQualitySettings.UserSettings.ShadowLevel;
				if (PlayerPreferences.is32bit)
				{
					num3 = Mathf.Max(num3, 1);
				}
				this.Shadows.value = this.Shadows.items[num3];
				this.Scatter.value = this.Scatter.items[num];
				this.Grass.value = this.Grass.items[num];
				this.GrassD.value = this.GrassD.items[num];
			}
		}
		catch
		{
		}
		try
		{
			this.FarShadows.value = this.FarShadows.items[(int)TheForestQualitySettings.UserSettings.FarShadowMode];
		}
		catch
		{
		}
		try
		{
			this.Antialias.value = this.Antialias.items[(int)TheForestQualitySettings.UserSettings.AntiAliasing];
		}
		catch
		{
		}
		try
		{
			this.SSAOType.value = this.SSAOType.items[(int)TheForestQualitySettings.UserSettings.SSAOType];
		}
		catch
		{
		}
		try
		{
			this.SSAO.value = this.SSAO.items[(int)TheForestQualitySettings.UserSettings.SSAO];
		}
		catch
		{
		}
		try
		{
			this.RenderType.value = this.RenderType.items[(int)TheForestQualitySettings.UserSettings.MyRenderType];
		}
		catch
		{
		}
		try
		{
			this.Bloom.value = this.Bloom.items[(int)TheForestQualitySettings.UserSettings.SEBloom];
		}
		catch
		{
		}
		try
		{
			this.FilmGrain.value = this.FilmGrain.items[(int)TheForestQualitySettings.UserSettings.Fg];
		}
		catch
		{
		}
		try
		{
			this.ChromaticAberation.value = this.ChromaticAberation.items[(int)TheForestQualitySettings.UserSettings.CA];
		}
		catch
		{
		}
		try
		{
			this.Dof.value = this.Dof.items[(int)TheForestQualitySettings.UserSettings.DofTech];
		}
		catch
		{
		}
		try
		{
			this.SunshineOcclusion.value = this.SunshineOcclusion.items[(int)TheForestQualitySettings.UserSettings.SunshineOcclusion];
		}
		catch
		{
		}
		try
		{
			this.MaterialQuality.value = this.MaterialQuality.items[(int)TheForestQualitySettings.UserSettings.MaterialQuality];
		}
		catch
		{
		}
		try
		{
			this.TerrainQuality.value = this.TerrainQuality.items[(int)TheForestQualitySettings.UserSettings.TerrainQuality];
		}
		catch
		{
		}
		try
		{
			this.ReflexionMode.value = this.ReflexionMode.items[(int)TheForestQualitySettings.UserSettings.ReflexionMode];
		}
		catch
		{
		}
		try
		{
			this.OceanQuality.value = ((!PlayerPreferences.is32bit) ? this.OceanQuality.items[(int)TheForestQualitySettings.UserSettings.OceanQuality] : this.OceanQuality.items[2]);
		}
		catch
		{
		}
		try
		{
			this.MotionBlur.value = this.MotionBlur.items[(int)TheForestQualitySettings.UserSettings.MotionBlur];
		}
		catch
		{
		}
		try
		{
			this.DrawDistance.value = this.DrawDistance.items[(int)TheForestQualitySettings.UserSettings.DrawDistance];
		}
		catch
		{
		}
		this.Resolution.value = string.Format("{0}x{1}", Screen.width, Screen.height);
		this.TextureQuality.value = this.TextureQuality.items[(int)TheForestQualitySettings.UserSettings.TextureQuality];
		this.MaxFrameRate.value = this.MaxFrameRate.items[(PlayerPreferences.MaxFrameRate != -1) ? 0 : 1];
		this.VSync.value = ((!PlayerPreferences.VSync) ? "Off" : "On");
		this.ShowHud.value = ((!PlayerPreferences.ShowHud) ? "Off" : "On");
		this.ShowOverlayIcons.value = ((!PlayerPreferences.ShowOverlayIcons) ? "Off" : "On");
		this.ShowProjectileReticle.value = ((!PlayerPreferences.ShowProjectileReticle) ? "Off" : "On");
		this.ShowPlayerNamesMP.value = ((!PlayerPreferences.ShowPlayerNamesMP) ? "Off" : "On");
		this.ShowStealthMeter.value = ((!PlayerPreferences.ShowStealthMeter) ? "Off" : "On");
		this.UseXInput.value = ((!PlayerPreferences.UseXInput) ? "Off" : "On");
		this.LowMemoryMode.value = PlayerPreferences.LowMemoryMode;
		this.Fullscreen.value = ((!Screen.fullScreen) ? "Off" : "On");
		this.ColorGrading.value = this.ColorGrading.items[PlayerPreferences.ColorGrading];
		this.MouseInvert.value = ((!PlayerPreferences.MouseInvert) ? "Off" : "On");
		this.MouseSensitivity.value = PlayerPreferences.MouseSensitivity;
		this.Fov.value = (PlayerPreferences.Fov - 60f) / 35f;
		this.Volume.value = PlayerPreferences.Volume;
		this.VolumeMusic.value = PlayerPreferences.MusicVolume;
		this.VoiceCount.value = PlayerPreferences.VoiceCount.ToString();
		PlayerPreferences.ApplyValues();
		this.EndIgnoreEvents();
	}

	private void CopySettingsFromGUI()
	{
		int num = this.PopupIndex(this.Shadows);
		if (PlayerPreferences.is32bit)
		{
			num = Mathf.Max(num, 1);
		}
		QualitySettings.SetQualityLevel(num);
		TheForestQualitySettings.UserSettings.CascadeCount = TheForestQualitySettings.GetPreset(num).CascadeCount;
		TheForestQualitySettings.UserSettings.LightmapResolution = TheForestQualitySettings.GetPreset(num).LightmapResolution;
		TheForestQualitySettings.UserSettings.LightDistance = TheForestQualitySettings.GetPreset(num).LightDistance;
		TheForestQualitySettings.UserSettings.LightmapUpdateIntervalFrames = TheForestQualitySettings.GetPreset(num).LightmapUpdateIntervalFrames;
		TheForestQualitySettings.UserSettings.ShadowLevel = (TheForestQualitySettings.ShadowLevels)num;
		TheForestQualitySettings.UserSettings.FarShadowMode = (TheForestQualitySettings.FarShadowModes)this.PopupIndex(this.FarShadows);
		PlayerPreferences.Preset = this.PopupIndex(this.Preset);
		TheForestQualitySettings.UserSettings.Preset = (TheForestQualitySettings.PresetLevels)PlayerPreferences.Preset;
		TheForestQualitySettings.UserSettings.AntiAliasing = (TheForestQualitySettings.AntiAliasingTechnique)this.PopupIndex(this.Antialias);
		int level = this.PopupIndex(this.Scatter);
		TheForestQualitySettings.UserSettings.ScatterResolution = TheForestQualitySettings.GetPreset(level).ScatterResolution;
		TheForestQualitySettings.UserSettings.ScatterSamplingQuality = TheForestQualitySettings.GetPreset(level).ScatterSamplingQuality;
		TheForestQualitySettings.UserSettings.SSAOType = (TheForestQualitySettings.SSAOTypes)this.PopupIndex(this.SSAOType);
		TheForestQualitySettings.UserSettings.SSAO = (TheForestQualitySettings.SSAOTechnique)this.PopupIndex(this.SSAO);
		TheForestQualitySettings.UserSettings.MyRenderType = (TheForestQualitySettings.RendererType)this.PopupIndex(this.RenderType);
		TheForestQualitySettings.UserSettings.SEBloom = (TheForestQualitySettings.SEBloomTechnique)this.PopupIndex(this.Bloom);
		TheForestQualitySettings.UserSettings.Fg = (TheForestQualitySettings.FilmGrain)this.PopupIndex(this.FilmGrain);
		TheForestQualitySettings.UserSettings.CA = (TheForestQualitySettings.ChromaticAberration)this.PopupIndex(this.ChromaticAberation);
		TheForestQualitySettings.UserSettings.DofTech = (TheForestQualitySettings.Dof)this.PopupIndex(this.Dof);
		TheForestQualitySettings.UserSettings.SunshineOcclusion = (TheForestQualitySettings.SunshineOcclusionOn)this.PopupIndex(this.SunshineOcclusion);
		TheForestQualitySettings.UserSettings.SetTerrainQuality((TheForestQualitySettings.TerrainQualities)this.PopupIndex(this.TerrainQuality));
		TheForestQualitySettings.UserSettings.SetTextureQuality((TheForestQualitySettings.TextureQualities)this.PopupIndex(this.TextureQuality));
		TheForestQualitySettings.UserSettings.SetMaterialQuality((TheForestQualitySettings.MaterialQualities)this.PopupIndex(this.MaterialQuality));
		TheForestQualitySettings.UserSettings.ReflexionMode = (TheForestQualitySettings.ReflexionModes)this.PopupIndex(this.ReflexionMode);
		if (!PlayerPreferences.is32bit)
		{
			TheForestQualitySettings.UserSettings.OceanQuality = (TheForestQualitySettings.OceanQualities)this.PopupIndex(this.OceanQuality);
		}
		TheForestQualitySettings.UserSettings.MotionBlur = (TheForestQualitySettings.MotionBlurQuality)this.PopupIndex(this.MotionBlur);
		TheForestQualitySettings.UserSettings.DrawDistance = (TheForestQualitySettings.DrawDistances)this.PopupIndex(this.DrawDistance);
		TheForestQualitySettings.UserSettings.GrassDistance = TheForestQualitySettings.GetPreset(this.PopupIndex(this.Grass)).GrassDistance;
		TheForestQualitySettings.UserSettings.GrassDensity = TheForestQualitySettings.GetPreset(this.PopupIndex(this.GrassD)).GrassDensity;
		PlayerPreferences.ColorGrading = this.PopupIndex(this.ColorGrading);
		PlayerPreferences.MouseInvert = (this.MouseInvert.value == "On");
		PlayerPreferences.MouseSensitivity = this.MouseSensitivity.value;
		PlayerPreferences.Fov = this.Fov.value * 35f + 60f;
		bool flag = this.VSync.value == "On";
		if (flag != PlayerPreferences.VSync)
		{
			PlayerPreferences.VSync = flag;
		}
		PlayerPreferences.ShowHud = (this.ShowHud.value == "On");
		PlayerPreferences.ShowOverlayIcons = (this.ShowOverlayIcons.value == "On");
		PlayerPreferences.ShowProjectileReticle = (this.ShowProjectileReticle.value == "On");
		PlayerPreferences.ShowPlayerNamesMP = (this.ShowPlayerNamesMP.value == "On");
		PlayerPreferences.ShowStealthMeter = (this.ShowStealthMeter.value == "On");
		PlayerPreferences.UseCrouchToggle = (this.CrouchMode.value == "Toggle");
		PlayerPreferences.UseXInput = (this.UseXInput.value == "On");
		PlayerPreferences.LowMemoryMode = this.LowMemoryMode.value;
		bool flag2 = this.Fullscreen.value == "On";
		int width = Screen.width;
		int height = Screen.height;
		string[] array = this.Resolution.value.Split(new char[]
		{
			'x'
		});
		if (array.Length == 2)
		{
			int.TryParse(array[0], out width);
			int.TryParse(array[1], out height);
		}
		if (width != Screen.width || height != Screen.height || flag2 != Screen.fullScreen)
		{
			Screen.SetResolution(width, height, flag2);
		}
		int num2 = (this.PopupIndex(this.MaxFrameRate) != 0) ? -1 : 60;
		Application.targetFrameRate = num2;
		PlayerPreferences.MaxFrameRate = num2;
		PlayerPreferences.Volume = this.Volume.value;
		PlayerPreferences.MusicVolume = this.VolumeMusic.value;
		int.TryParse(this.VoiceCount.value, out PlayerPreferences.VoiceCount);
		PlayerPreferences.ApplyValues();
		this.BeginIgnoreEvents();
		if (TheForestQualitySettings.IsCustomized)
		{
			this.Preset.value = this.Preset.items[this.Preset.items.Count - 1];
		}
		else
		{
			this.Preset.value = this.Preset.items[PlayerPreferences.Preset];
		}
		this.EndIgnoreEvents();
	}

	private void OnChange()
	{
		if (this.IgnoreEvents)
		{
			return;
		}
		this.CopySettingsFromGUI();
	}

	public void Save()
	{
		this.shouldSave = false;
		PlayerPreferences.Save();
	}
}
