using Inspector;
using Inspector.Decorations;
using System;
using UnityEngine;

namespace ScionEngine
{
	[AddComponentMenu("Image Effects/Scion Post Process"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class ScionPostProcess : MonoBehaviour
	{
		[Inspector.Decorations.Header(0, "Grain"), Toggle("Active", useProperty = "grain", tooltip = "Determines if grain is used")]
		public bool m_grain = true;

		[Slider("Intensity", useProperty = "grainIntensity", visibleCheck = "ShowGrain", minValue = 0f, maxValue = 1f, tooltip = "How strong the grain effect is")]
		public float m_grainIntensity = 0.12f;

		[Inspector.Decorations.Header(0, "Vignette"), Toggle("Active", useProperty = "vignette", tooltip = "Determines if vignette is used")]
		public bool m_vignette = true;

		[Slider("Intensity", useProperty = "vignetteIntensity", visibleCheck = "ShowVignette", minValue = 0f, maxValue = 1f, tooltip = "How strong the vignette effect is")]
		public float m_vignetteIntensity = 0.7f;

		[Slider("Scale", useProperty = "vignetteScale", visibleCheck = "ShowVignette", minValue = 0f, maxValue = 1f, tooltip = "How much of the screen is affected")]
		public float m_vignetteScale = 0.7f;

		[Field("Color", useProperty = "vignetteColor", visibleCheck = "ShowVignette", tooltip = "What color the vignette effect has")]
		public Color m_vignetteColor = Color.black;

		[Inspector.Decorations.Header(0, "Chromatic Aberration"), Toggle("Active", useProperty = "chromaticAberration", tooltip = "Determines if chromatic aberration is used")]
		public bool m_chromaticAberration = true;

		[Slider("Distortion Scale", useProperty = "chromaticAberrationDistortion", visibleCheck = "ShowChromaticAberration", minValue = 0f, maxValue = 1f, tooltip = "How much of the screen is affected")]
		public float m_chromaticAberrationDistortion = 0.5f;

		[Slider("Intensity", useProperty = "chromaticAberrationIntensity", visibleCheck = "ShowChromaticAberration", minValue = -30f, maxValue = 30f, tooltip = "How strong the distortion effect is")]
		public float m_chromaticAberrationIntensity = 10f;

		[Inspector.Decorations.Header(0, "Tonemapping"), Toggle("Active", useProperty = "tonemapping", tooltip = "Determines if tonemapping is used")]
		public bool m_tonemapping = true;

		[Field("Mode", useProperty = "tonemappingMode", visibleCheck = "ShowTonemapping", tooltip = "What type of tonemapping algorithm is used")]
		public TonemappingMode m_tonemappingMode = TonemappingMode.Filmic;

		[Slider("White Point", useProperty = "whitePoint", visibleCheck = "ShowTonemapping", minValue = 0.5f, maxValue = 20f, tooltip = "At what intensity pixels will become white")]
		public float m_whitePoint = 5f;

		[Inspector.Decorations.Header(0, "Bloom"), Toggle("Active", useProperty = "bloom", tooltip = "Determines if bloom is used")]
		public bool m_bloom = true;

		[Slider("Intensity", useProperty = "bloomIntensity", visibleCheck = "ShowBloom", minValue = 0f, maxValue = 1f, tooltip = "How strong the bloom effect is")]
		public float m_bloomIntensity = 0.35f;

		[Slider("Brightness", useProperty = "bloomBrightness", visibleCheck = "ShowBloom", minValue = 0.5f, maxValue = 2f, tooltip = "How bright the bloom effect is")]
		public float m_bloomBrightness = 1.2f;

		[Slider("Downsamples", useProperty = "bloomDownsamples", visibleCheck = "ShowBloom", minValue = 2f, maxValue = 9f, tooltip = "Number of downsamples")]
		public int m_bloomDownsamples = 7;

		[Inspector.Decorations.Header(0, "Lens Dirt", visibleCheck = "ShowBloom"), Toggle("Active", useProperty = "lensDirt", visibleCheck = "ShowBloom", tooltip = "Determines if lens dirt is used")]
		public bool m_lensDirt;

		[Field("Dirt Texture", useProperty = "lensDirtTexture", visibleCheck = "ShowLensDirt", tooltip = "What type of tonemapping algorithm is used")]
		public Texture2D m_lensDirtTexture;

		[Slider("Intensity", useProperty = "lensDirtIntensity", visibleCheck = "ShowLensDirtSettings", minValue = 0f, maxValue = 1f, tooltip = "How strong the lens dirt effect is")]
		public float m_lensDirtIntensity = 0.5f;

		[Slider("Brightness", useProperty = "lensDirtBrightness", visibleCheck = "ShowLensDirtSettings", minValue = 0.5f, maxValue = 2f, tooltip = "How bright the lens dirt effect is")]
		public float m_lensDirtBrightness = 1.2f;

		[Inspector.Decorations.Header(0, "Camera Mode"), Field("Camera Mode", useProperty = "cameraMode", visibleCheck = "ShowCameraMode", tooltip = "What camera mode is used")]
		public CameraMode m_cameraMode = CameraMode.AutoPriority;

		[Slider("F Number", useProperty = "fNumber", visibleCheck = "ShowFNumber", minValue = 1f, maxValue = 22f, tooltip = "The F number of the camera")]
		public float m_fNumber = 4f;

		[Slider("ISO", useProperty = "ISO", visibleCheck = "ShowISO", minValue = 100f, maxValue = 6400f, tooltip = "The ISO setting of the camera")]
		public float m_ISO = 100f;

		[Slider("Shutter Speed", useProperty = "shutterSpeed", visibleCheck = "ShowShutterSpeed", minValue = 0.00025f, maxValue = 0.0333333351f, tooltip = "The shutted speed of the camera")]
		public float m_shutterSpeed = 0.01f;

		[Toggle("Custom Focal Length", useProperty = "userControlledFocalLength", tooltip = "If false the focal length will instead be derived from the camera's field of view")]
		public bool m_userControlledFocalLength;

		[Slider("Focal Length", useProperty = "focalLength", visibleCheck = "ShowFocalLength", minValue = 10f, maxValue = 250f, tooltip = "The focal length of the camera in millimeters")]
		public float m_focalLength = 15f;

		[Inspector.Decorations.Header(0, "Exposure Settings"), Slider("Exposure Compensation", useProperty = "exposureCompensation", visibleCheck = "ShowExposureComp", minValue = -8f, maxValue = 8f, tooltip = "Allows you to manually compensate towards the desired exposure")]
		public float m_exposureCompensation;

		[MinMaxSlider("Min Max Exposure", -8f, 24f, useProperty = "minMaxExposure")]
		public Vector2 m_minMaxExposure = new Vector2(-8f, 24f);

		[Slider("Adaption Speed", useProperty = "adaptionSpeed", visibleCheck = "ShowExposureAdaption", minValue = 0.1f, maxValue = 8f, tooltip = "How fast the exposure is allowed to change")]
		public float m_adaptionSpeed = 1f;

		[Inspector.Decorations.Header(0, "Depth of Field"), Toggle("Active", useProperty = "depthOfField", tooltip = "Determines if depth of field is used")]
		public bool m_depthOfField = true;

		[Slider("Max Radius", useProperty = "maxCoCRadius", visibleCheck = "ShowDepthOfField", minValue = 10f, maxValue = 20f, tooltip = "The maximum radius the blur can be. Lower values might have less artifacts")]
		public float m_maxCoCRadius = 20f;

		[Field("Quality Level", useProperty = "depthOfFieldQuality", visibleCheck = "ShowDepthOfField", tooltip = "Dictates how many samples the algorithm does")]
		public DepthOfFieldQuality m_depthOfFieldQuality;

		[Field("Depth Focus Mode", useProperty = "depthFocusMode", visibleCheck = "ShowDepthOfField", tooltip = "How the depth focus point is chosen")]
		public DepthFocusMode m_depthFocusMode = DepthFocusMode.PointAverage;

		[Field("Point Center", useProperty = "pointAveragePosition", visibleCheck = "ShowPointAverage", tooltip = "Where the center of focus is on the screen. [0,0] is the bottom left corner and [1,1] is the top right")]
		public Vector2 m_pointAveragePosition = new Vector2(0.5f, 0.5f);

		[Inspector.Decorations.Space(0, 1), Slider("Point Range", useProperty = "pointAverageRange", visibleCheck = "ShowPointAverage", minValue = 0.01f, maxValue = 1f, tooltip = "How far the point average calculation reaches")]
		public float m_pointAverageRange = 0.2f;

		[Toggle("Visualize", useProperty = "visualizePointFocus", visibleCheck = "ShowPointAverage", tooltip = "Show the area of influence on the main screen for visualizaiton")]
		public bool m_visualizePointFocus;

		[Slider("Adaption Speed", useProperty = "depthAdaptionSpeed", visibleCheck = "ShowPointAverage", minValue = 1f, maxValue = 30f, tooltip = "Dictates how fast the focal distance changes")]
		public float m_depthAdaptionSpeed = 10f;

		[Field("Focal Distance", useProperty = "focalDistance", visibleCheck = "ShowFocalDistance", tooltip = "The focal distance in meters")]
		public float m_focalDistance = 10f;

		[Slider("Depth Range", useProperty = "focalRange", visibleCheck = "ShowFocalRange", minValue = 0f, maxValue = 50f, tooltip = "The length of the range that is 100% in focus")]
		public float m_focalRange = 10f;

		[Inspector.Decorations.Header(0, "Color Correction"), Field("Mode", useProperty = "colorGradingMode", tooltip = "Which color correction mode is currently active")]
		public ColorGradingMode m_colorGradingMode;

		[Field("Compatibility", useProperty = "colorGradingCompatibility", visibleCheck = "ShowCCTex1", tooltip = "Sets the algorithm to be compatible with the lookup textures used by different shaders")]
		public ColorGradingCompatibility m_colorGradingCompatibility = ColorGradingCompatibility.Amplify;

		[Field("Lookup Texture", useProperty = "colorGradingTex1", visibleCheck = "ShowCCTex1", tooltip = "The lookup texture used for color correction")]
		public Texture2D m_colorGradingTex1;

		[Field("Blend Lookup Texture", useProperty = "colorGradingTex2", visibleCheck = "ShowCCTex2", tooltip = "The lookup texture blended in as the blend factor increases")]
		public Texture2D m_colorGradingTex2;

		[Slider("Blend Factor", useProperty = "colorGradingBlendFactor", visibleCheck = "ShowCCTex2", minValue = 0f, maxValue = 1f, tooltip = "Interpolates between the original color correction texture and the blend target color correction texture")]
		public float m_colorGradingBlendFactor;

		private bool m_isFirstRender;

		private float prevCamFoV;

		private Camera m_camera;

		private Bloom m_bloomClass;

		private VirtualCamera m_virtualCamera;

		private CombinationPass m_combinationPass;

		private Downsampling m_downsampling;

		private DepthOfField m_depthOfFieldClass;

		private ColorGrading m_colorGrading;

		private PostProcessParameters postProcessParams = new PostProcessParameters();

		private ScionDebug m_scionDebug;

		public static ScionDebug ActiveDebug;

		public CameraMode cameraMode
		{
			get
			{
				return this.m_cameraMode;
			}
			set
			{
				this.m_cameraMode = value;
				this.postProcessParams.cameraParams.cameraMode = value;
				this.postProcessParams.exposure = (value != CameraMode.Off);
			}
		}

		public bool tonemapping
		{
			get
			{
				return this.m_tonemapping;
			}
			set
			{
				this.m_tonemapping = value;
				this.postProcessParams.tonemapping = value;
			}
		}

		public bool bloom
		{
			get
			{
				return this.m_bloom;
			}
			set
			{
				this.m_bloom = value;
				this.postProcessParams.bloom = value;
			}
		}

		public bool lensDirt
		{
			get
			{
				return this.m_lensDirt;
			}
			set
			{
				this.m_lensDirt = value;
				this.postProcessParams.lensDirt = value;
			}
		}

		public Texture2D lensDirtTexture
		{
			get
			{
				return this.m_lensDirtTexture;
			}
			set
			{
				this.m_lensDirtTexture = value;
				this.postProcessParams.lensDirtTexture = value;
			}
		}

		public bool depthOfField
		{
			get
			{
				return this.m_depthOfField;
			}
			set
			{
				this.m_depthOfField = value;
				this.postProcessParams.depthOfField = value;
				this.PlatformCompatibility();
			}
		}

		public float bloomIntensity
		{
			get
			{
				return this.m_bloomIntensity;
			}
			set
			{
				this.m_bloomIntensity = value;
				this.postProcessParams.bloomParams.intensity = ScionUtility.Square(value);
			}
		}

		public float bloomBrightness
		{
			get
			{
				return this.m_bloomBrightness;
			}
			set
			{
				this.m_bloomBrightness = value;
				this.postProcessParams.bloomParams.brightness = value;
			}
		}

		public int bloomDownsamples
		{
			get
			{
				return this.m_bloomDownsamples;
			}
			set
			{
				this.m_bloomDownsamples = value;
				this.postProcessParams.bloomParams.downsamples = value;
				Debug.Log("set to " + value);
			}
		}

		public float lensDirtIntensity
		{
			get
			{
				return this.m_lensDirtIntensity;
			}
			set
			{
				this.m_lensDirtIntensity = value;
				this.postProcessParams.lensDirtParams.intensity = ScionUtility.Square(value);
			}
		}

		public float lensDirtBrightness
		{
			get
			{
				return this.m_lensDirtBrightness;
			}
			set
			{
				this.m_lensDirtBrightness = value;
				this.postProcessParams.lensDirtParams.brightness = value;
			}
		}

		public DepthFocusMode depthFocusMode
		{
			get
			{
				return this.m_depthFocusMode;
			}
			set
			{
				this.m_depthFocusMode = value;
				this.postProcessParams.DoFParams.depthFocusMode = value;
			}
		}

		public float maxCoCRadius
		{
			get
			{
				return this.m_maxCoCRadius;
			}
			set
			{
				this.m_maxCoCRadius = value;
				this.postProcessParams.DoFParams.maxCoCRadius = value;
			}
		}

		public DepthOfFieldQuality depthOfFieldQuality
		{
			get
			{
				return this.m_depthOfFieldQuality;
			}
			set
			{
				this.m_depthOfFieldQuality = value;
				this.postProcessParams.DoFParams.quality = ((SystemInfo.graphicsShaderLevel >= 40) ? value : DepthOfFieldQuality.Normal);
			}
		}

		public Vector2 pointAveragePosition
		{
			get
			{
				return this.m_pointAveragePosition;
			}
			set
			{
				this.m_pointAveragePosition = value;
				this.postProcessParams.DoFParams.pointAveragePosition = value;
			}
		}

		public float pointAverageRange
		{
			get
			{
				return this.m_pointAverageRange;
			}
			set
			{
				this.m_pointAverageRange = value;
				this.postProcessParams.DoFParams.pointAverageRange = value;
			}
		}

		public bool visualizePointFocus
		{
			get
			{
				return this.m_visualizePointFocus;
			}
			set
			{
				this.m_visualizePointFocus = value;
				this.postProcessParams.DoFParams.visualizePointFocus = value;
			}
		}

		public float depthAdaptionSpeed
		{
			get
			{
				return this.m_depthAdaptionSpeed;
			}
			set
			{
				this.m_depthAdaptionSpeed = value;
				this.postProcessParams.DoFParams.depthAdaptionSpeed = value;
			}
		}

		public float focalDistance
		{
			get
			{
				return this.m_focalDistance;
			}
			set
			{
				this.m_focalDistance = value;
				this.postProcessParams.DoFParams.focalDistance = value;
			}
		}

		public float focalRange
		{
			get
			{
				return this.m_focalRange;
			}
			set
			{
				this.m_focalRange = value;
				this.postProcessParams.DoFParams.focalRange = value;
			}
		}

		public ColorGradingMode colorGradingMode
		{
			get
			{
				return this.m_colorGradingMode;
			}
			set
			{
				this.m_colorGradingMode = value;
				this.postProcessParams.colorGradingParams.colorGradingMode = ((!(this.colorGradingTex1 == null)) ? value : ColorGradingMode.Off);
			}
		}

		public Texture2D colorGradingTex1
		{
			get
			{
				return this.m_colorGradingTex1;
			}
			set
			{
				this.m_colorGradingTex1 = value;
				this.postProcessParams.colorGradingParams.colorGradingTex1 = value;
				this.colorGradingMode = this.colorGradingMode;
				this.ProcessColorGradingTexture1();
			}
		}

		public Texture2D colorGradingTex2
		{
			get
			{
				return this.m_colorGradingTex2;
			}
			set
			{
				this.m_colorGradingTex2 = value;
				this.postProcessParams.colorGradingParams.colorGradingTex2 = value;
				this.ProcessColorGradingTexture2();
			}
		}

		public float colorGradingBlendFactor
		{
			get
			{
				return this.m_colorGradingBlendFactor;
			}
			set
			{
				this.m_colorGradingBlendFactor = value;
				this.postProcessParams.colorGradingParams.colorGradingBlendFactor = value;
			}
		}

		public ColorGradingCompatibility colorGradingCompatibility
		{
			get
			{
				return this.m_colorGradingCompatibility;
			}
			set
			{
				this.m_colorGradingCompatibility = value;
				this.postProcessParams.colorGradingParams.colorGradingCompatibility = value;
				this.ProcessColorGradingTexture1();
				this.ProcessColorGradingTexture2();
			}
		}

		public bool userControlledFocalLength
		{
			get
			{
				return this.m_userControlledFocalLength;
			}
			set
			{
				this.m_userControlledFocalLength = value;
			}
		}

		public float focalLength
		{
			get
			{
				return this.m_focalLength;
			}
			set
			{
				this.m_focalLength = value;
				this.postProcessParams.cameraParams.focalLength = value;
			}
		}

		public float fNumber
		{
			get
			{
				return this.m_fNumber;
			}
			set
			{
				this.m_fNumber = value;
				this.postProcessParams.cameraParams.fNumber = value;
			}
		}

		public float ISO
		{
			get
			{
				return this.m_ISO;
			}
			set
			{
				this.m_ISO = value;
				this.postProcessParams.cameraParams.ISO = value;
			}
		}

		public float shutterSpeed
		{
			get
			{
				return this.m_shutterSpeed;
			}
			set
			{
				this.m_shutterSpeed = value;
				this.postProcessParams.cameraParams.shutterSpeed = value;
			}
		}

		public float adaptionSpeed
		{
			get
			{
				return this.m_adaptionSpeed;
			}
			set
			{
				this.m_adaptionSpeed = value;
				this.postProcessParams.cameraParams.adaptionSpeed = value;
			}
		}

		public Vector2 minMaxExposure
		{
			get
			{
				return this.m_minMaxExposure;
			}
			set
			{
				this.m_minMaxExposure = value;
				this.postProcessParams.cameraParams.minMaxExposure = value;
			}
		}

		public float exposureCompensation
		{
			get
			{
				return this.m_exposureCompensation;
			}
			set
			{
				this.m_exposureCompensation = value;
				this.postProcessParams.cameraParams.exposureCompensation = value;
			}
		}

		public bool grain
		{
			get
			{
				return this.m_grain;
			}
			set
			{
				this.m_grain = value;
				this.postProcessParams.commonPostProcess.grainIntensity = ((!this.m_grain) ? 0f : this.grainIntensity);
			}
		}

		public float grainIntensity
		{
			get
			{
				return this.m_grainIntensity;
			}
			set
			{
				this.m_grainIntensity = value;
				this.postProcessParams.commonPostProcess.grainIntensity = ((!this.m_grain) ? 0f : this.grainIntensity);
			}
		}

		public bool vignette
		{
			get
			{
				return this.m_vignette;
			}
			set
			{
				this.m_vignette = value;
				this.postProcessParams.commonPostProcess.vignetteIntensity = ((!this.m_vignette) ? 0f : this.vignetteIntensity);
			}
		}

		public float vignetteIntensity
		{
			get
			{
				return this.m_vignetteIntensity;
			}
			set
			{
				this.m_vignetteIntensity = value;
				this.postProcessParams.commonPostProcess.vignetteIntensity = ((!this.m_vignette) ? 0f : this.vignetteIntensity);
			}
		}

		public float vignetteScale
		{
			get
			{
				return this.m_vignetteScale;
			}
			set
			{
				this.m_vignetteScale = value;
				this.postProcessParams.commonPostProcess.vignetteScale = value;
			}
		}

		public Color vignetteColor
		{
			get
			{
				return this.m_vignetteColor;
			}
			set
			{
				this.m_vignetteColor = value;
				this.postProcessParams.commonPostProcess.vignetteColor = value;
			}
		}

		public bool chromaticAberration
		{
			get
			{
				return this.m_chromaticAberration;
			}
			set
			{
				this.m_chromaticAberration = value;
				this.postProcessParams.commonPostProcess.chromaticAberration = value;
			}
		}

		public float chromaticAberrationDistortion
		{
			get
			{
				return this.m_chromaticAberrationDistortion;
			}
			set
			{
				this.m_chromaticAberrationDistortion = value;
				this.postProcessParams.commonPostProcess.chromaticAberrationDistortion = value;
			}
		}

		public float chromaticAberrationIntensity
		{
			get
			{
				return this.m_chromaticAberrationIntensity;
			}
			set
			{
				this.m_chromaticAberrationIntensity = value;
				this.postProcessParams.commonPostProcess.chromaticAberrationIntensity = value;
			}
		}

		public TonemappingMode tonemappingMode
		{
			get
			{
				return this.m_tonemappingMode;
			}
			set
			{
				this.m_tonemappingMode = value;
			}
		}

		public float whitePoint
		{
			get
			{
				return this.m_whitePoint;
			}
			set
			{
				this.m_whitePoint = value;
				this.postProcessParams.commonPostProcess.whitePoint = value;
			}
		}

		private bool ShowGrain()
		{
			return this.m_grain;
		}

		private bool ShowVignette()
		{
			return this.m_vignette;
		}

		private bool ShowChromaticAberration()
		{
			return this.m_chromaticAberration;
		}

		private bool ShowTonemapping()
		{
			return this.m_tonemapping;
		}

		private bool ShowBloom()
		{
			return this.bloom;
		}

		private bool ShowLensDirt()
		{
			return this.ShowBloom() && this.lensDirt;
		}

		private bool ShowLensDirtSettings()
		{
			return this.lensDirtTexture != null && this.ShowLensDirt();
		}

		private bool ShowCameraMode()
		{
			return true;
		}

		private bool ShowExposureComp()
		{
			return this.cameraMode != CameraMode.Off;
		}

		private bool ShowExposureAdaption()
		{
			return this.cameraMode != CameraMode.Off && this.cameraMode != CameraMode.Manual;
		}

		private bool ShowDownsampleBloomExposure()
		{
			return this.ShowExposureAdaption() && this.ShowBloom();
		}

		private bool ShowFocalLength()
		{
			return this.m_userControlledFocalLength;
		}

		private bool ShowFNumber()
		{
			return this.cameraMode == CameraMode.AperturePriority || this.cameraMode == CameraMode.Manual || (this.cameraMode == CameraMode.Off && this.depthOfField);
		}

		private bool ShowISO()
		{
			return this.cameraMode == CameraMode.Manual;
		}

		private bool ShowShutterSpeed()
		{
			return this.cameraMode == CameraMode.Manual;
		}

		private bool ShowDepthOfField()
		{
			return this.depthOfField;
		}

		private bool ShowPointAverage()
		{
			return this.m_depthFocusMode == DepthFocusMode.PointAverage && this.ShowDepthOfField();
		}

		private bool ShowFocalDistance()
		{
			return false;
		}

		private bool ShowFocalRange()
		{
			return false;
		}

		private bool ShowCCTex1()
		{
			return this.colorGradingMode == ColorGradingMode.On;
		}

		private bool ShowCCTex2()
		{
			return false;
		}

		private void OnEnable()
		{
			this.m_camera = base.GetComponent<Camera>();
			this.m_bloomClass = new Bloom();
			this.m_combinationPass = new CombinationPass();
			this.m_downsampling = new Downsampling();
			this.m_virtualCamera = new VirtualCamera();
			this.m_depthOfFieldClass = new DepthOfField();
			this.m_scionDebug = new ScionDebug();
			this.m_colorGrading = new ColorGrading();
			this.m_isFirstRender = true;
			this.postProcessParams.Fill(this);
			if (!this.PlatformCompatibility())
			{
				base.enabled = false;
			}
		}

		private void OnDisable()
		{
			if (this.m_bloomClass != null)
			{
				this.m_bloomClass.ReleaseResources();
			}
		}

		private void OnPreRender()
		{
			this.m_camera.depthTextureMode |= DepthTextureMode.Depth;
		}

		private void ProcessColorGradingTexture1()
		{
		}

		private void ProcessColorGradingTexture2()
		{
		}

		private bool PlatformCompatibility()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				Debug.LogWarning("Image Effects are not supported on this platform");
				return false;
			}
			if (!SystemInfo.supportsRenderTextures)
			{
				Debug.LogWarning("RenderTextures are not supported on this platform");
				return false;
			}
			if (!this.m_bloomClass.PlatformCompatibility())
			{
				Debug.LogWarning("Bloom shader not supported on this platform");
				return false;
			}
			if (!this.m_combinationPass.PlatformCompatibility())
			{
				Debug.LogWarning("Combination shader not supported on this platform");
				return false;
			}
			if (!this.m_virtualCamera.PlatformCompatibility())
			{
				Debug.LogWarning("Virtual camera shader not supported on this platform");
				return false;
			}
			return this.m_depthOfFieldClass.PlatformCompatibility() || !this.depthOfField;
		}

		private void SetupPostProcessParameters(PostProcessParameters postProcessParams, RenderTexture source)
		{
			this.focalDistance = ((this.focalDistance >= this.m_camera.nearClipPlane + 0.3f) ? this.focalDistance : (this.m_camera.nearClipPlane + 0.3f));
			postProcessParams.halfResSource = null;
			postProcessParams.halfResDepth = this.m_downsampling.DownsampleDepthTexture(source.width, source.height);
			postProcessParams.width = source.width;
			postProcessParams.height = source.height;
			postProcessParams.halfWidth = source.width / 2;
			postProcessParams.halfHeight = source.height / 2;
			if (this.prevCamFoV != this.m_camera.fieldOfView)
			{
				postProcessParams.preCalcValues.tanHalfFoV = Mathf.Tan(this.m_camera.fieldOfView * 0.5f * 0.0174532924f);
				this.prevCamFoV = this.m_camera.fieldOfView;
			}
			postProcessParams.DoFParams.useMedianFilter = true;
			if (!this.userControlledFocalLength)
			{
				postProcessParams.cameraParams.focalLength = ScionUtility.GetFocalLength(postProcessParams.preCalcValues.tanHalfFoV);
			}
			else
			{
				postProcessParams.cameraParams.focalLength = this.focalLength * 0.001f;
			}
			postProcessParams.cameraParams.apertureDiameter = ScionUtility.ComputeApertureDiameter(this.fNumber, postProcessParams.cameraParams.focalLength);
			postProcessParams.cameraParams.fieldOfView = this.m_camera.fieldOfView;
			postProcessParams.cameraParams.aspect = this.m_camera.aspect;
			postProcessParams.cameraParams.nearPlane = this.m_camera.nearClipPlane;
			postProcessParams.cameraParams.farPlane = this.m_camera.farClipPlane;
			postProcessParams.isFirstRender = this.m_isFirstRender;
			this.m_isFirstRender = false;
		}

		private void SetGlobalParameters(PostProcessParameters postProcessParams)
		{
			Vector4 vec = default(Vector4);
			vec.x = postProcessParams.cameraParams.nearPlane;
			vec.y = postProcessParams.cameraParams.farPlane;
			vec.z = 1f / vec.y;
			vec.w = vec.x * vec.z;
			Shader.SetGlobalVector("_ScionNearFarParams", vec);
			Shader.SetGlobalTexture("_HalfResSourceTexture", postProcessParams.halfResSource);
			Shader.SetGlobalTexture("_HalfResDepthTexture", postProcessParams.halfResDepth);
			Shader.SetGlobalVector("_ScionResolutionParameters1", new Vector4
			{
				x = (float)postProcessParams.halfWidth,
				y = (float)postProcessParams.halfHeight,
				z = (float)postProcessParams.width,
				w = (float)postProcessParams.height
			});
			Shader.SetGlobalVector("_ScionResolutionParameters2", new Vector4
			{
				x = 1f / (float)postProcessParams.halfWidth,
				y = 1f / (float)postProcessParams.halfHeight,
				z = 1f / (float)postProcessParams.width,
				w = 1f / (float)postProcessParams.height
			});
			Shader.SetGlobalVector("_ScionCameraParams1", new Vector4
			{
				x = postProcessParams.cameraParams.apertureDiameter,
				y = postProcessParams.cameraParams.focalLength,
				z = postProcessParams.cameraParams.aspect
			});
		}

		private void SetShaderKeyWords(PostProcessParameters postProcessParams)
		{
			if (postProcessParams.cameraParams.cameraMode == CameraMode.Off || postProcessParams.cameraParams.cameraMode == CameraMode.Manual)
			{
				Debug.LogError("This functionality has been removed for The Forest to save on keywords!");
			}
			ColorGradingMode colorGradingMode = postProcessParams.colorGradingParams.colorGradingMode;
			if (colorGradingMode != ColorGradingMode.Off)
			{
				if (colorGradingMode == ColorGradingMode.On)
				{
					ShaderSettings.ColorGradingSettings.SetIndex(1);
				}
			}
			else
			{
				ShaderSettings.ColorGradingSettings.SetIndex(0);
			}
			if (postProcessParams.commonPostProcess.chromaticAberration)
			{
				ShaderSettings.ChromaticAberrationSettings.SetIndex(1);
			}
			else
			{
				ShaderSettings.ChromaticAberrationSettings.SetIndex(0);
			}
		}

		[ImageEffectTransformsToLDR]
		private void OnRenderImage(RenderTexture source, RenderTexture dest)
		{
			ScionPostProcess.ActiveDebug = this.m_scionDebug;
			this.SetupPostProcessParameters(this.postProcessParams, source);
			this.SetGlobalParameters(this.postProcessParams);
			this.SetShaderKeyWords(this.postProcessParams);
			this.PerformPostProcessing(source, dest, this.postProcessParams);
			ScionPostProcess.ActiveDebug = null;
		}

		private void PerformPostProcessing(RenderTexture source, RenderTexture dest, PostProcessParameters postProcessParams)
		{
			if (postProcessParams.depthOfField)
			{
				postProcessParams.halfResSource = this.m_downsampling.DownsampleFireflyRemovingBilateral(source, postProcessParams.halfResDepth);
				source = this.m_depthOfFieldClass.RenderDepthOfField(postProcessParams, source, dest, this.m_virtualCamera);
				RenderTexture.ReleaseTemporary(postProcessParams.halfResSource);
			}
			postProcessParams.halfResSource = this.m_downsampling.DownsampleFireflyRemoving(source);
			if (postProcessParams.bloom)
			{
				postProcessParams.bloomTexture = this.m_bloomClass.CreateBloomTexture(postProcessParams.halfResSource, postProcessParams.bloomParams);
				if (postProcessParams.exposure)
				{
					RenderTexture renderTexture = this.m_bloomClass.TryGetSmallBloomTexture(50);
					float energyNormalizer = this.m_bloomClass.GetEnergyNormalizer();
					if (renderTexture == null)
					{
						renderTexture = postProcessParams.halfResSource;
						energyNormalizer = 1f;
					}
					this.m_virtualCamera.CalculateVirtualCamera(postProcessParams.cameraParams, renderTexture, (float)postProcessParams.halfWidth, postProcessParams.preCalcValues.tanHalfFoV, energyNormalizer, postProcessParams.DoFParams.focalDistance, postProcessParams.isFirstRender);
				}
			}
			else if (postProcessParams.exposure)
			{
				this.m_virtualCamera.CalculateVirtualCamera(postProcessParams.cameraParams, postProcessParams.halfResSource, (float)postProcessParams.halfWidth, postProcessParams.preCalcValues.tanHalfFoV, 1f, postProcessParams.DoFParams.focalDistance, postProcessParams.isFirstRender);
			}
			this.m_combinationPass.Combine(source, dest, postProcessParams, this.m_virtualCamera);
			this.m_scionDebug.VisualizeDebug(dest);
			RenderTexture.ReleaseTemporary(postProcessParams.halfResSource);
			RenderTexture.ReleaseTemporary(postProcessParams.halfResDepth);
			RenderTexture.ReleaseTemporary(postProcessParams.dofTexture);
			this.m_bloomClass.EndOfFrameCleanup();
			this.m_virtualCamera.EndOfFrameCleanup();
			this.m_depthOfFieldClass.EndOfFrameCleanup();
			if (postProcessParams.depthOfField)
			{
				RenderTexture.ReleaseTemporary(source);
			}
		}
	}
}
