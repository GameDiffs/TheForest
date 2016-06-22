using System;
using UnityEngine;

[AddComponentMenu("Image Effects/HBAO"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class HBAO : MonoBehaviour
{
	public enum Preset
	{
		FastestPerformance,
		FastPerformance,
		Normal,
		HighQuality,
		HighestQuality,
		Custom
	}

	public enum Quality
	{
		Lowest,
		Low,
		Medium,
		High,
		Highest
	}

	public enum Resolution
	{
		Full,
		Half
	}

	public enum DisplayMode
	{
		Normal,
		AOOnly,
		ColorBleedingOnly,
		SplitWithoutAOAndWithAO,
		SplitWithAOAndAOOnly,
		SplitWithoutAOAndAOOnly
	}

	public enum Blur
	{
		None,
		Narrow,
		Medium,
		Wide,
		ExtraWide
	}

	public enum NoiseType
	{
		Random,
		Dither
	}

	public enum PerPixelNormals
	{
		GBuffer,
		Camera
	}

	[Serializable]
	public struct Presets
	{
		public HBAO.Preset preset;

		[SerializeField]
		public static HBAO.Presets defaultPresets
		{
			get
			{
				return new HBAO.Presets
				{
					preset = HBAO.Preset.Normal
				};
			}
		}
	}

	[Serializable]
	public struct GeneralSettings
	{
		[Space(6f), Tooltip("The quality of the AO.")]
		public HBAO.Quality quality;

		[Tooltip("The resolution at which the AO is calculated.")]
		public HBAO.Resolution resolution;

		[Space(10f), Tooltip("The type of noise to use.")]
		public HBAO.NoiseType noiseType;

		[Space(10f), Tooltip("The way the AO is displayed on screen.")]
		public HBAO.DisplayMode displayMode;

		[SerializeField]
		public static HBAO.GeneralSettings defaultSettings
		{
			get
			{
				return new HBAO.GeneralSettings
				{
					quality = HBAO.Quality.Medium,
					noiseType = HBAO.NoiseType.Dither
				};
			}
		}
	}

	[Serializable]
	public struct AOSettings
	{
		[Range(0f, 2f), Space(6f), Tooltip("Eye-space AO radius: this is the distance outside which occluders are ignored.")]
		public float radius;

		[Range(32f, 256f), Tooltip("Maximum radius in pixels: this prevents the radius to grow too much with close-up object and impact on performances.")]
		public float maxRadiusPixels;

		[Range(0f, 0.5f), Tooltip("For low-tessellated geometry, occlusion variations tend to appear at creases and ridges, which betray the underlying tessellation. To remove these artifacts, we use an angle bias parameter which restricts the hemisphere.")]
		public float bias;

		[Range(0f, 10f), Tooltip("This value allows to scale up the ambient occlusion values.")]
		public float intensity;

		[Range(0f, 1f), Tooltip("This value allows to attenuate ambient occlusion depending on final color luminance.")]
		public float luminanceInfluence;

		[Range(0f, 1500f), Tooltip("The max distance to display AO.")]
		public float maxDistance;

		[Range(0f, 500f), Tooltip("The distance before max distance at which AO start to decrease.")]
		public float distanceFalloff;

		[Space(10f), Tooltip("The type of per pixel normals to use.")]
		public HBAO.PerPixelNormals perPixelNormals;

		[Space(10f), Tooltip("This setting allow you to set the base color if the AO, the alpha channel value is unused.")]
		public Color baseColor;

		[SerializeField]
		public static HBAO.AOSettings defaultSettings
		{
			get
			{
				return new HBAO.AOSettings
				{
					radius = 1f,
					maxRadiusPixels = 256f,
					bias = 0.05f,
					intensity = 1f,
					maxDistance = 150f,
					distanceFalloff = 50f,
					baseColor = Color.black
				};
			}
		}
	}

	[Serializable]
	public struct ColorBleedingSettings
	{
		[Space(6f)]
		public bool enabled;

		[Range(0f, 4f), Space(10f), Tooltip("This value allows to control the saturation of the color bleeding.")]
		public float saturation;

		[Range(0f, 32f), Tooltip("This value allows to scale the contribution of the color bleeding samples.")]
		public float albedoMultiplier;

		[SerializeField]
		public static HBAO.ColorBleedingSettings defaultSettings
		{
			get
			{
				return new HBAO.ColorBleedingSettings
				{
					saturation = 1f,
					albedoMultiplier = 4f
				};
			}
		}
	}

	[Serializable]
	public struct BlurSettings
	{
		[Space(6f), Tooltip("The type of blur to use.")]
		public HBAO.Blur amount;

		[Range(0f, 16f), Space(10f), Tooltip("This parameter controls the depth-dependent weight of the bilateral filter, to avoid bleeding across edges. A zero sharpness is a pure Gaussian blur. Increasing the blur sharpness removes bleeding by using lower weights for samples with large depth delta from the current pixel.")]
		public float sharpness;

		[SerializeField]
		public static HBAO.BlurSettings defaultSettings
		{
			get
			{
				return new HBAO.BlurSettings
				{
					amount = HBAO.Blur.Medium,
					sharpness = 8f
				};
			}
		}
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class SettingsGroup : Attribute
	{
	}

	public Texture2D noiseTex;

	public Shader hbaoShader;

	[HBAO.SettingsGroup, SerializeField]
	private HBAO.Presets m_Presets = HBAO.Presets.defaultPresets;

	[HBAO.SettingsGroup, SerializeField]
	private HBAO.GeneralSettings m_GeneralSettings = HBAO.GeneralSettings.defaultSettings;

	[HBAO.SettingsGroup, SerializeField]
	private HBAO.AOSettings m_AOSettings = HBAO.AOSettings.defaultSettings;

	[HBAO.SettingsGroup, SerializeField]
	private HBAO.ColorBleedingSettings m_ColorBleedingSettings = HBAO.ColorBleedingSettings.defaultSettings;

	[HBAO.SettingsGroup, SerializeField]
	private HBAO.BlurSettings m_BlurSettings = HBAO.BlurSettings.defaultSettings;

	private HBAO.Quality _quality;

	private HBAO.NoiseType _noiseType;

	private float _radius;

	private float _maxRadiusPixels;

	private float _bias;

	private float _intensity;

	private float _luminanceInfluence;

	private float _maxDistance;

	private float _distanceFalloff;

	private HBAO.PerPixelNormals _perPixelNormals;

	private Color _aoBaseColor;

	private bool _colorBleedingEnabled;

	private float _colorBleedSaturation;

	private float _albedoMultiplier;

	private HBAO.Blur _blurAmount;

	private float _blurSharpness;

	private RenderingPath _renderingPath;

	private string[] _hbaoShaderKeywords = new string[3];

	private Material _hbaoMaterial;

	private Camera _hbaoCamera;

	private Matrix4x4 _projMatrix;

	private int[] _numSampleDirections = new int[]
	{
		3,
		4,
		6,
		8,
		8
	};

	public HBAO.Presets presets
	{
		get
		{
			return this.m_Presets;
		}
		set
		{
			this.m_Presets = value;
		}
	}

	public HBAO.GeneralSettings generalSettings
	{
		get
		{
			return this.m_GeneralSettings;
		}
		set
		{
			this.m_GeneralSettings = value;
		}
	}

	public HBAO.AOSettings aoSettings
	{
		get
		{
			return this.m_AOSettings;
		}
		set
		{
			this.m_AOSettings = value;
		}
	}

	public HBAO.ColorBleedingSettings colorBleedingSettings
	{
		get
		{
			return this.m_ColorBleedingSettings;
		}
		set
		{
			this.m_ColorBleedingSettings = value;
		}
	}

	public HBAO.BlurSettings blurSettings
	{
		get
		{
			return this.m_BlurSettings;
		}
		set
		{
			this.m_BlurSettings = value;
		}
	}

	private void OnEnable()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarning("HBAO shader is not supported on this platform.");
			base.enabled = false;
			return;
		}
		if (this.hbaoShader != null && !this.hbaoShader.isSupported)
		{
			Debug.LogWarning("HBAO shader is not supported on this platform.");
			base.enabled = false;
			return;
		}
		if (this.hbaoShader == null)
		{
			return;
		}
		this.CreateMaterial();
		this._hbaoCamera.depthTextureMode |= DepthTextureMode.Depth;
		if (this.aoSettings.perPixelNormals == HBAO.PerPixelNormals.Camera && this._hbaoShaderKeywords[1] != "__")
		{
			this._hbaoCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		}
	}

	private void OnDisable()
	{
		if (this._hbaoMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(this._hbaoMaterial);
		}
		if (this.noiseTex != null)
		{
			UnityEngine.Object.DestroyImmediate(this.noiseTex);
		}
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.hbaoShader == null || this._hbaoCamera == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.CreateMaterial();
		this._hbaoCamera.depthTextureMode |= DepthTextureMode.Depth;
		if (this.aoSettings.perPixelNormals == HBAO.PerPixelNormals.Camera && this._hbaoShaderKeywords[1] != "__")
		{
			this._hbaoCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		}
		Vector4 vector = new Vector4(-2f / ((float)Screen.width * this._projMatrix[0]), -2f / ((float)Screen.height * this._projMatrix[5]), (1f - this._projMatrix[2]) / this._projMatrix[0], (1f + this._projMatrix[6]) / this._projMatrix[5]);
		this._hbaoMaterial.SetVector("_ProjInfo", vector);
		this._hbaoMaterial.SetMatrix("_WorldToCameraMatrix", this._hbaoCamera.worldToCameraMatrix);
		if (this._quality != this.generalSettings.quality || this._noiseType != this.generalSettings.noiseType || this._radius != this.aoSettings.radius || this._maxRadiusPixels != this.aoSettings.maxRadiusPixels || this._bias != this.aoSettings.bias || this._intensity != this.aoSettings.intensity || this._luminanceInfluence != this.aoSettings.luminanceInfluence || this._maxDistance != this.aoSettings.maxDistance || this._distanceFalloff != this.aoSettings.distanceFalloff || this._perPixelNormals != this.aoSettings.perPixelNormals || this._aoBaseColor != this.aoSettings.baseColor || this._colorBleedingEnabled != this.colorBleedingSettings.enabled || this._colorBleedSaturation != this.colorBleedingSettings.saturation || this._albedoMultiplier != this.colorBleedingSettings.albedoMultiplier || this._blurAmount != this.blurSettings.amount || this._blurSharpness != this.blurSettings.sharpness || this._renderingPath != this._hbaoCamera.renderingPath)
		{
			this.UpdateMaterialProperties();
		}
		this.RenderHBAO(source, destination);
	}

	private void RenderHBAO(RenderTexture source, RenderTexture destination)
	{
		int downsampling = this.GetDownsampling();
		RenderTexture temporary = RenderTexture.GetTemporary(source.width >> downsampling, source.height >> downsampling);
		Graphics.Blit(source, temporary, this._hbaoMaterial, this.GetAoPass());
		if (this._blurAmount != HBAO.Blur.None)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(temporary, temporary2, this._hbaoMaterial, this.GetBlurXPass());
			temporary.DiscardContents();
			Graphics.Blit(temporary2, temporary, this._hbaoMaterial, this.GetBlurYPass());
			RenderTexture.ReleaseTemporary(temporary2);
		}
		this._hbaoMaterial.SetTexture("_HBAOTex", temporary);
		Graphics.Blit(source, destination, this._hbaoMaterial, this.GetFinalPass());
		RenderTexture.ReleaseTemporary(temporary);
	}

	private void CreateMaterial()
	{
		if (this._hbaoMaterial == null)
		{
			this._hbaoMaterial = new Material(this.hbaoShader);
			this._hbaoMaterial.hideFlags = HideFlags.HideAndDontSave;
			this._hbaoCamera = base.GetComponent<Camera>();
			this._projMatrix = this._hbaoCamera.projectionMatrix;
			this.UpdateMaterialProperties();
		}
	}

	private void UpdateMaterialProperties()
	{
		if (this.noiseTex == null || this._quality != this.generalSettings.quality || this._noiseType != this.generalSettings.noiseType)
		{
			if (this.noiseTex != null)
			{
				UnityEngine.Object.DestroyImmediate(this.noiseTex);
			}
			float num = (float)((this.generalSettings.noiseType != HBAO.NoiseType.Dither) ? 64 : 4);
			this.CreateRandomTexture((int)num);
		}
		this._quality = this.generalSettings.quality;
		this._noiseType = this.generalSettings.noiseType;
		this._radius = this.aoSettings.radius;
		this._maxRadiusPixels = this.aoSettings.maxRadiusPixels;
		this._bias = this.aoSettings.bias;
		this._intensity = this.aoSettings.intensity;
		this._luminanceInfluence = this.aoSettings.luminanceInfluence;
		this._maxDistance = this.aoSettings.maxDistance;
		this._distanceFalloff = this.aoSettings.distanceFalloff;
		this._perPixelNormals = this.aoSettings.perPixelNormals;
		this._aoBaseColor = this.aoSettings.baseColor;
		this._colorBleedingEnabled = this.colorBleedingSettings.enabled;
		this._colorBleedSaturation = this.colorBleedingSettings.saturation;
		this._albedoMultiplier = this.colorBleedingSettings.albedoMultiplier;
		this._blurAmount = this.blurSettings.amount;
		this._blurSharpness = this.blurSettings.sharpness;
		this._renderingPath = this._hbaoCamera.renderingPath;
		this._hbaoMaterial.SetTexture("_NoiseTex", this.noiseTex);
		this._hbaoMaterial.SetFloat("_NoiseTexSize", (float)((this._noiseType != HBAO.NoiseType.Dither) ? 64 : 4));
		this._hbaoMaterial.SetFloat("_Radius", this._radius);
		this._hbaoMaterial.SetFloat("_MaxRadiusPixels", this._maxRadiusPixels);
		this._hbaoMaterial.SetFloat("_NegInvRadius2", -1f / (this._radius * this._radius));
		this._hbaoMaterial.SetFloat("_AngleBias", this._bias);
		this._hbaoMaterial.SetFloat("_AOmultiplier", 2f * (1f / (1f - this._bias)));
		this._hbaoMaterial.SetFloat("_Intensity", this._intensity);
		this._hbaoMaterial.SetFloat("_LuminanceInfluence", this._luminanceInfluence);
		this._hbaoMaterial.SetFloat("_MaxDistance", this._maxDistance);
		this._hbaoMaterial.SetFloat("_DistanceFalloff", this._distanceFalloff);
		this._hbaoMaterial.SetColor("_BaseColor", this._aoBaseColor);
		this._hbaoMaterial.SetFloat("_ColorBleedSaturation", this._colorBleedSaturation);
		this._hbaoMaterial.SetFloat("_AlbedoMultiplier", this._albedoMultiplier);
		this._hbaoMaterial.SetFloat("_BlurSharpness", this._blurSharpness);
	}

	private int GetDownsampling()
	{
		HBAO.Resolution resolution = this.generalSettings.resolution;
		if (resolution == HBAO.Resolution.Full)
		{
			return 0;
		}
		if (resolution != HBAO.Resolution.Half)
		{
			return 0;
		}
		return 1;
	}

	private int GetAoPass()
	{
		switch (this.generalSettings.quality)
		{
		case HBAO.Quality.Lowest:
			return 0;
		case HBAO.Quality.Low:
			return 1;
		case HBAO.Quality.Medium:
			return 2;
		case HBAO.Quality.High:
			return 3;
		case HBAO.Quality.Highest:
			return 4;
		default:
			return 2;
		}
	}

	private int GetBlurXPass()
	{
		switch (this.blurSettings.amount)
		{
		case HBAO.Blur.Narrow:
			return 5;
		case HBAO.Blur.Medium:
			return 7;
		case HBAO.Blur.Wide:
			return 9;
		case HBAO.Blur.ExtraWide:
			return 11;
		default:
			return 7;
		}
	}

	private int GetBlurYPass()
	{
		switch (this.blurSettings.amount)
		{
		case HBAO.Blur.Narrow:
			return 6;
		case HBAO.Blur.Medium:
			return 8;
		case HBAO.Blur.Wide:
			return 10;
		case HBAO.Blur.ExtraWide:
			return 12;
		default:
			return 8;
		}
	}

	private int GetFinalPass()
	{
		switch (this.generalSettings.displayMode)
		{
		case HBAO.DisplayMode.Normal:
			return 13;
		case HBAO.DisplayMode.AOOnly:
			return 14;
		case HBAO.DisplayMode.ColorBleedingOnly:
			return 15;
		case HBAO.DisplayMode.SplitWithoutAOAndWithAO:
			return 16;
		case HBAO.DisplayMode.SplitWithAOAndAOOnly:
			return 17;
		case HBAO.DisplayMode.SplitWithoutAOAndAOOnly:
			return 18;
		default:
			return 13;
		}
	}

	private void CreateRandomTexture(int size)
	{
		this.noiseTex = new Texture2D(size, size, TextureFormat.RGB24, false, true);
		this.noiseTex.filterMode = FilterMode.Point;
		this.noiseTex.wrapMode = TextureWrapMode.Repeat;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				float b = UnityEngine.Random.Range(0f, 1f);
				float f = 6.28318548f * num / (float)this._numSampleDirections[this.GetAoPass()];
				Color color = new Color(Mathf.Cos(f), Mathf.Sin(f), b);
				this.noiseTex.SetPixel(i, j, color);
			}
		}
		this.noiseTex.Apply();
	}

	public void ApplyPreset(HBAO.Preset preset)
	{
		if (preset == HBAO.Preset.Custom)
		{
			this.m_Presets.preset = preset;
			return;
		}
		HBAO.DisplayMode displayMode = this.generalSettings.displayMode;
		this.m_GeneralSettings = HBAO.GeneralSettings.defaultSettings;
		this.m_AOSettings = HBAO.AOSettings.defaultSettings;
		this.m_ColorBleedingSettings = HBAO.ColorBleedingSettings.defaultSettings;
		this.m_BlurSettings = HBAO.BlurSettings.defaultSettings;
		this.m_GeneralSettings.displayMode = displayMode;
		switch (preset)
		{
		case HBAO.Preset.FastestPerformance:
			this.m_GeneralSettings.quality = HBAO.Quality.Lowest;
			this.m_AOSettings.radius = 0.5f;
			this.m_AOSettings.maxRadiusPixels = 64f;
			this.m_BlurSettings.amount = HBAO.Blur.ExtraWide;
			goto IL_149;
		case HBAO.Preset.FastPerformance:
			this.m_GeneralSettings.quality = HBAO.Quality.Low;
			this.m_AOSettings.maxRadiusPixels = 128f;
			this.m_AOSettings.radius = 0.5f;
			this.m_BlurSettings.amount = HBAO.Blur.Wide;
			goto IL_149;
		case HBAO.Preset.HighQuality:
			this.m_GeneralSettings.quality = HBAO.Quality.High;
			goto IL_149;
		case HBAO.Preset.HighestQuality:
			this.m_GeneralSettings.quality = HBAO.Quality.Highest;
			this.m_AOSettings.radius = 1.2f;
			this.m_BlurSettings.amount = HBAO.Blur.Narrow;
			goto IL_149;
		}
		this.m_AOSettings.radius = 0.8f;
		IL_149:
		this.m_Presets.preset = preset;
	}
}
