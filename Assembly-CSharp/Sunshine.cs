using System;
using UnityEngine;

[ExecuteInEditMode]
public class Sunshine : MonoBehaviour
{
	public const string Version = "1.7.0";

	public const string OccluderShaderName = "Hidden/Sunshine/Occluder";

	public const string PostScatterShaderName = "Hidden/Sunshine/PostProcess/Scatter";

	public const string PostBlurShaderName = "Hidden/Sunshine/PostProcess/Blur";

	public const string PostDebugShaderName = "Hidden/Sunshine/PostProcess/Debug";

	public const int MAX_CASCADES = 4;

	public static Sunshine Instance;

	[NonSerialized]
	public bool Ready;

	[NonSerialized]
	public bool Supported = true;

	public Light SunLight;

	public int Occluders = -1;

	public bool UsePerCascadeOccluders;

	public int Occluders1 = -1;

	public int Occluders2 = -1;

	public int Occluders3 = -1;

	public SunshineShaderSets ShaderSet;

	public SunshineUpdateInterval UpdateInterval;

	public int UpdateIntervalFrames = 2;

	public float UpdateIntervalPadding;

	public float UpdateIntervalMovement = 1f;

	public Transform CustomLightBoundsOrigin;

	public float CustomLightBoundsRadius = 1f;

	public SunshineLightResolutions LightmapResolution = SunshineLightResolutions.MediumResolution;

	public int CustomLightmapResolution = 512;

	public bool UseOcclusionCulling = true;

	public float LightPaddingZ = 100f;

	public float LightFadeRatio = 0.1f;

	public float CascadeSpacing = 0.425f;

	public bool UseManualCascadeSplits;

	public float ManualCascadeSplit0 = 0.425f;

	public float ManualCascadeSplit1 = 0.425f;

	public float ManualCascadeSplit2 = 0.425f;

	public float CascadeFade = 0.1f;

	public float TerrainLODTweak;

	public SunshineShadowFilters ShadowFilter = SunshineShadowFilters.PCF3x3;

	[NonSerialized]
	public RenderTexture Lightmap;

	public SunshineRelativeResolutions ScatterResolution = SunshineRelativeResolutions.Half;

	public SunshineScatterSamplingQualities ScatterSamplingQuality = SunshineScatterSamplingQualities.Medium;

	[NonSerialized]
	public Texture2D ScatterDitherTexture;

	public bool ScatterBlur = true;

	public float ScatterBlurDepthTollerance = 0.1f;

	public bool ScatterAnimateNoise = true;

	public float ScatterAnimateNoiseSpeed = 0.1f;

	public Color ScatterColor = new Color(0.6f, 0.6f, 0.6f, 1f);

	public bool ScatterEnabled = true;

	public float ScatterIntensity = 0.5f;

	public float ScatterExaggeration = 0.5f;

	public float ScatterSky;

	public Texture2D ScatterRamp;

	public Texture2D OvercastTexture;

	public float OvercastScale = 10f;

	public Vector2 OvercastMovement = new Vector2(1f, 0.5f);

	public float OvercastPlaneHeight = 100f;

	public bool OvercastAffectsScatter;

	public bool CustomScatterOvercast;

	public Texture2D ScatterOvercastTexture;

	public float ScatterOvercastScale = 10f;

	public Vector2 ScatterOvercastMovement = new Vector2(1f, 0.5f);

	public float ScatterOvercastPlaneHeight = 100f;

	public Texture2D BlankOvercastTexture;

	public Shader OccluderShader;

	public Shader PostScatterShader;

	[NonSerialized]
	public Material PostScatterMaterial;

	public Shader PostBlurShader;

	[NonSerialized]
	public Material PostBlurMaterial;

	public Shader PostDebugShader;

	[NonSerialized]
	public Material PostDebugMaterial;

	public SunshineDebugViews DebugView;

	[NonSerialized]
	public Camera[] SunLightCameras = new Camera[4];

	public int CustomCascadeCount = 1;

	public float CustomLightDistance = 40f;

	public bool RequiresPostprocessing
	{
		get
		{
			return (this.ScatterActive || this.DebugView == SunshineDebugViews.Cascades) && this.PostProcessSupported;
		}
	}

	public float ShadowFilterKernelRadiusDiagonal
	{
		get
		{
			return Mathf.Sqrt(this.ShadowFilterKernelRadius * 2f);
		}
	}

	public float ShadowFilterKernelRadius
	{
		get
		{
			switch (this.ShadowFilter)
			{
			case SunshineShadowFilters.PCF2x2:
				return 1f;
			case SunshineShadowFilters.PCF3x3:
				return 1.5f;
			case SunshineShadowFilters.PCF4x4:
				return 2f;
			default:
				return 0.5f;
			}
		}
	}

	public float ShadowBias
	{
		get
		{
			return this.SunLight.shadowBias;
		}
	}

	public bool UsingCustomBounds
	{
		get
		{
			return this.CustomLightBoundsOrigin != null;
		}
	}

	public SunshineMath.BoundingSphere CustomBounds
	{
		get
		{
			return new SunshineMath.BoundingSphere
			{
				origin = this.CustomLightBoundsOrigin.position,
				radius = this.CustomLightBoundsRadius
			};
		}
	}

	public bool IsMobile
	{
		get
		{
			SunshineShaderSets shaderSet = this.ShaderSet;
			return shaderSet != SunshineShaderSets.DesktopShaders && shaderSet == SunshineShaderSets.MobileShaders;
		}
	}

	public int IdealLightmapResolution
	{
		get
		{
			if (this.LightmapResolution == SunshineLightResolutions.Custom)
			{
				return Mathf.Clamp(this.CustomLightmapResolution, 1, 4096);
			}
			return Mathf.Max(SunshineMath.UnityStyleLightmapResolution(this.LightmapResolution), 1);
		}
	}

	public int IdealLightmapWidth
	{
		get
		{
			if (this.CascadeCount == 2)
			{
				return this.IdealLightmapResolution / 2;
			}
			return this.IdealLightmapResolution;
		}
	}

	public int IdealLightmapHeight
	{
		get
		{
			return this.IdealLightmapResolution;
		}
	}

	public int CascadeResolutionInverseRatio
	{
		get
		{
			return (this.CascadeCount <= 1) ? 1 : 2;
		}
	}

	public int CascadeMapResolution
	{
		get
		{
			return this.IdealLightmapResolution / this.CascadeResolutionInverseRatio;
		}
	}

	public SunshineShadowFormats ShadowFormat
	{
		get
		{
			return SunshineShadowFormats.Linear;
		}
	}

	public RenderTextureFormat LightmapFormat
	{
		get
		{
			return RenderTextureFormat.ARGB32;
		}
	}

	public FilterMode LightmapFilterMode
	{
		get
		{
			return FilterMode.Point;
		}
	}

	public bool PostProcessSupported
	{
		get
		{
			return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth) && SystemInfo.supportsImageEffects && this.PostScatterShader.isSupported && (this.PostScatterMaterial && this.PostScatterMaterial.passCount == 2) && this.ScatterDitherTexture != null;
		}
	}

	public bool PostBlurSupported
	{
		get
		{
			return this.PostBlurShader.isSupported && this.PostBlurMaterial.passCount == 1;
		}
	}

	public bool ScatterActive
	{
		get
		{
			return this.ScatterEnabled && this.ScatterIntensity > 0f && this.PostProcessSupported;
		}
	}

	public Camera SunLightCamera
	{
		get
		{
			return this.SunLightCameras[0];
		}
	}

	public int CascadeCount
	{
		get
		{
			if (this.UsingCustomBounds || this.IsMobile || !SunshineProjectPreferences.Instance.UseCustomShadows)
			{
				return 1;
			}
			if (this.IdealLightmapResolution < 32)
			{
				return 1;
			}
			return this.CustomCascadeCount;
		}
	}

	public float LightDistance
	{
		get
		{
			if (this.UsingCustomBounds)
			{
				return 9999f;
			}
			return this.CustomLightDistance;
		}
	}

	public Rect[] CascadeRects
	{
		get
		{
			return SunshineMath.CascadeViewportArrangements[Mathf.Clamp(this.CascadeCount - 1, 0, 3)];
		}
	}

	public static string FormatMessage(string message)
	{
		return string.Format("Sunshine {0}: {1}", "1.7.0", message);
	}

	public static void LogMessage(string message, bool showInEditor = false)
	{
		if (showInEditor || Application.isPlaying)
		{
			Debug.Log(Sunshine.FormatMessage(message));
		}
	}

	public LayerMask GetCascadeOccluders(int cascade)
	{
		switch (cascade)
		{
		case 0:
			return this.Occluders;
		case 1:
			return (!this.UsePerCascadeOccluders) ? this.Occluders : this.Occluders1;
		case 2:
			return (!this.UsePerCascadeOccluders) ? this.Occluders : this.Occluders2;
		case 3:
			return (!this.UsePerCascadeOccluders) ? this.Occluders : this.Occluders3;
		default:
			return this.Occluders;
		}
	}

	public float LightmapTexelPhysicalSize(int cascadeID)
	{
		return this.SunLightCameras[cascadeID].orthographicSize * 2f / (float)this.CascadeMapResolution;
	}

	public float ShadowSlopeBias(int cID)
	{
		return this.SunLight.shadowBias * this.LightmapTexelPhysicalSize(cID) * 82f * this.ShadowFilterKernelRadius;
	}

	public Rect CascadeRect(int cID)
	{
		return this.CascadeRects[cID];
	}

	public Rect CascadePixelRect(int cID)
	{
		Rect result = this.CascadeRects[cID];
		float num = (float)((!(this.Lightmap != null)) ? 1 : this.Lightmap.width);
		float num2 = (float)((!(this.Lightmap != null)) ? 1 : this.Lightmap.height);
		result.x *= num;
		result.y *= num2;
		result.width *= num;
		result.height *= num2;
		return result;
	}

	public float CascadeNearClip(int cID)
	{
		return this.CascadeNearClipScale(cID) * this.LightDistance;
	}

	public float CascadeFarClip(int cID)
	{
		return this.CascadeFarClipScale(cID) * this.LightDistance;
	}

	public float CascadeNearClipScale(int cID)
	{
		int num = cID - 1;
		return (num >= 0) ? this.CascadeFarClipScale(num) : 0f;
	}

	public float CascadeFarClipScale(int cID)
	{
		if (cID >= this.CascadeCount - 1)
		{
			return 1f;
		}
		float num = 1f;
		if (this.UseManualCascadeSplits)
		{
			num = 0f;
			for (int i = 0; i <= cID; i++)
			{
				switch (i)
				{
				case 0:
					num += (1f - num) * this.ManualCascadeSplit0;
					break;
				case 1:
					num += (1f - num) * this.ManualCascadeSplit1;
					break;
				case 2:
					num += (1f - num) * this.ManualCascadeSplit2;
					break;
				}
			}
		}
		else
		{
			for (int j = this.CascadeCount - 1; j > cID; j--)
			{
				num *= this.CascadeSpacing;
			}
		}
		return num;
	}

	public Light FindAppropriateSunLight()
	{
		Light[] lights = Light.GetLights(LightType.Directional, -1);
		if (lights.Length > 0)
		{
			return lights[0];
		}
		return null;
	}

	public bool Setup()
	{
		this.SetupSingleton();
		if (Application.isPlaying)
		{
			this.Supported = (this.Supported && SystemInfo.supportsRenderTextures);
		}
		else
		{
			this.Supported = SystemInfo.supportsRenderTextures;
		}
		if (!this.Supported)
		{
			this.DestroyLightmap();
			this.DisableShadows();
			return false;
		}
		this.SetupLightmap();
		if (this.Ready)
		{
			return true;
		}
		if (!this.Lightmap)
		{
			this.Supported = false;
			Sunshine.LogMessage("Unable to create Lightmap", false);
		}
		if (!this.SunLight && Application.isPlaying)
		{
			this.SunLight = this.FindAppropriateSunLight();
		}
		if (!this.SunLight)
		{
			Sunshine.LogMessage("Sun Light was not configured, and couldn't find appropriate Direction Light...", false);
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			return false;
		}
		if (!this.OccluderShader)
		{
			Sunshine.LogMessage("Occluder Shader Missing...", false);
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			return false;
		}
		if (!this.OccluderShader.isSupported)
		{
			Sunshine.LogMessage("Occluder Shader Not Supported...", false);
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			return false;
		}
		if (!this.PostScatterShader)
		{
			Sunshine.LogMessage("Post Process Scatter Shader Missing...", false);
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			return false;
		}
		if (!this.PostBlurShader)
		{
			Sunshine.LogMessage("Post Process Blur Shader Missing...", false);
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			return false;
		}
		if (!this.PostDebugShader)
		{
			Sunshine.LogMessage("Post Process Debug Shader Missing...", false);
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			return false;
		}
		if (!this.BlankOvercastTexture)
		{
			Sunshine.LogMessage("Blank Overcast Texture Missing...", false);
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			return false;
		}
		this.RecreateMaterials();
		this.RecreateRenderCameras();
		this.RecreateTextures();
		this.Ready = true;
		return true;
	}

	private void OnDrawGizmos()
	{
		if (this.UsingCustomBounds)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(this.CustomLightBoundsOrigin.position, this.CustomLightBoundsRadius);
		}
	}

	private void RecreateMaterials()
	{
		this.DestroyMaterials();
		if (!this.PostScatterMaterial)
		{
			this.PostScatterMaterial = new Material(this.PostScatterShader);
			this.PostScatterMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!this.PostBlurMaterial)
		{
			this.PostBlurMaterial = new Material(this.PostBlurShader);
			this.PostBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!this.PostDebugMaterial)
		{
			this.PostDebugMaterial = new Material(this.PostDebugShader);
			this.PostDebugMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	private void DestroyMaterials()
	{
		if (this.PostScatterMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.PostScatterMaterial);
			this.PostScatterMaterial = null;
		}
		if (this.PostBlurMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.PostBlurMaterial);
			this.PostBlurMaterial = null;
		}
		if (this.PostDebugMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.PostDebugMaterial);
			this.PostDebugMaterial = null;
		}
	}

	public bool IsCascadeCamera(Camera camera)
	{
		for (int i = 0; i < 4; i++)
		{
			if (this.SunLightCameras[i] == camera)
			{
				return true;
			}
		}
		return false;
	}

	private void RecreateRenderCameras()
	{
		this.DestroyRenderCameras();
		for (int i = 0; i < 4; i++)
		{
			if (!this.SunLightCameras[i])
			{
				this.SunLightCameras[i] = Sunshine.CreateRenderCamera(string.Format("Sunshine Cascade Camera {0}", i));
			}
		}
	}

	private void DestroyRenderCameras()
	{
		for (int i = 0; i < 4; i++)
		{
			if (!(this.SunLightCameras[i] == null))
			{
				UnityEngine.Object.DestroyImmediate(this.SunLightCameras[i].gameObject);
				this.SunLightCameras[i] = null;
			}
		}
	}

	private void RecreateTextures()
	{
		this.DestroyTextures();
		int num = 16;
		this.ScatterDitherTexture = new Texture2D(4, 4, TextureFormat.ARGB32, false, false);
		this.ScatterDitherTexture.filterMode = FilterMode.Point;
		int[] array = new int[]
		{
			0,
			8,
			2,
			10,
			12,
			4,
			14,
			6,
			3,
			11,
			1,
			9,
			15,
			7,
			13,
			5
		};
		Color[] array2 = new Color[num];
		for (int i = 0; i < num; i++)
		{
			array2[i] = new Color(0f, 0f, 0f, (float)array[i] / (float)num);
		}
		this.ScatterDitherTexture.SetPixels(array2);
		this.ScatterDitherTexture.Apply();
	}

	private void DestroyTextures()
	{
		if (this.ScatterDitherTexture != null)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(this.ScatterDitherTexture);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.ScatterDitherTexture);
			}
			this.ScatterDitherTexture = null;
		}
	}

	private void SetupSingleton()
	{
		if (Sunshine.Instance == null)
		{
			Sunshine.Instance = this;
		}
		else if (Sunshine.Instance != this && Application.isPlaying)
		{
			Sunshine.LogMessage("Multiple Sunshine Instances detected!", true);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Awake()
	{
		this.SetupSingleton();
		this.Setup();
	}

	private void OnEnable()
	{
		this.SetupSingleton();
		this.Setup();
	}

	private void Start()
	{
		this.Setup();
	}

	private void OnDisable()
	{
		this.DisableShadows();
		this.DestroyResources();
	}

	private void OnDestroy()
	{
		this.OnDisable();
		if (Sunshine.Instance == this)
		{
			Sunshine.Instance = null;
		}
	}

	private void Update()
	{
		if (!this.Setup())
		{
			return;
		}
	}

	private void DestroyResources()
	{
		this.DestroyLightmap();
		this.DestroyRenderCameras();
		this.DestroyMaterials();
		this.DestroyTextures();
		this.Ready = false;
		this.Supported = true;
	}

	private void RecreateLightmap()
	{
		this.DestroyLightmap();
		this.Lightmap = new RenderTexture(this.IdealLightmapWidth, this.IdealLightmapHeight, 16, this.LightmapFormat, RenderTextureReadWrite.Linear);
		if (this.Lightmap)
		{
			this.Lightmap.name = "Sunshine Lightmap";
			this.Lightmap.hideFlags = HideFlags.HideAndDontSave;
			this.Lightmap.useMipMap = false;
			this.Lightmap.filterMode = this.LightmapFilterMode;
			this.Lightmap.wrapMode = TextureWrapMode.Clamp;
			this.Lightmap.Create();
			Shader.SetGlobalTexture("sunshine_Lightmap", this.Lightmap);
		}
	}

	private void SetupLightmap()
	{
		if (!this.Lightmap || this.Lightmap.width != this.IdealLightmapWidth || this.Lightmap.height != this.IdealLightmapHeight || this.Lightmap.format != this.LightmapFormat)
		{
			this.RecreateLightmap();
		}
	}

	private void DestroyLightmap()
	{
		if (this.Lightmap)
		{
			UnityEngine.Object.DestroyImmediate(this.Lightmap);
			this.Lightmap = null;
		}
	}

	public void DisableShadows()
	{
		SunshineKeywords.DisableShadows();
	}

	private static Camera CreateRenderCamera(string name)
	{
		GameObject gameObject = GameObject.Find(name);
		if (!gameObject)
		{
			gameObject = new GameObject(name);
		}
		Camera camera = gameObject.GetComponent<Camera>();
		if (!camera)
		{
			camera = gameObject.AddComponent<Camera>();
		}
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		camera.enabled = false;
		camera.nearClipPlane = 0.1f;
		camera.farClipPlane = 100f;
		camera.depthTextureMode = DepthTextureMode.None;
		camera.clearFlags = CameraClearFlags.Color;
		camera.backgroundColor = Color.white;
		camera.orthographic = true;
		camera.hideFlags = HideFlags.HideAndDontSave;
		gameObject.SetActive(false);
		return camera;
	}
}
