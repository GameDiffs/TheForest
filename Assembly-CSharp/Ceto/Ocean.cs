using Ceto.Common.Threading.Scheduling;
using Ceto.Common.Unity.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Components/Ocean"), DisallowMultipleComponent]
	public class Ocean : MonoBehaviour
	{
		public const float MAX_SPECTRUM_WAVE_HEIGHT = 40f;

		public const float MAX_OVERLAY_WAVE_HEIGHT = 20f;

		public static readonly bool DISABLE_FOURIER_MULTITHREADING;

		public static readonly bool DISABLE_PROJECTED_GRID_BORDER;

		public static bool DISABLE_ALL_MULTITHREADING;

		public static readonly bool DISABLE_PROJECTION_FLIPPING;

		public static readonly string VERSION = "1.1.1";

		public static string OCEAN_LAYER = "Water";

		public static readonly string REFLECTION_TEXTURE_NAME = "Ceto_Reflections";

		public static readonly string REFRACTION_GRAB_TEXTURE_NAME = "Ceto_RefractionGrab";

		public static readonly string DEPTH_GRAB_TEXTURE_NAME = "Ceto_DepthBuffer";

		public static readonly string OCEAN_DEPTH_TEXTURE_NAME = "Ceto_OceanDepth";

		public static readonly string OCEAN_MASK_TEXTURE_NAME = "Ceto_OceanMask";

		public static readonly string NORMAL_FADE_TEXTURE_NAME = "Ceto_NormalFade";

		public bool disableWarnings;

		public bool disableInfo;

		public bool doublePrecisionProjection = true;

		public GameObject m_sun;

		public float level;

		public OVERLAY_MAP_SIZE heightOverlaySize = OVERLAY_MAP_SIZE.HALF;

		public OVERLAY_MAP_SIZE normalOverlaySize = OVERLAY_MAP_SIZE.FULL;

		public OVERLAY_MAP_SIZE foamOverlaySize = OVERLAY_MAP_SIZE.FULL;

		public OVERLAY_MAP_SIZE clipOverlaySize = OVERLAY_MAP_SIZE.HALF;

		public OVERLAY_BLEND_MODE heightBlendMode;

		public OVERLAY_BLEND_MODE foamBlendMode;

		public Color defaultSkyColor = new Color32(96, 147, 210, 255);

		public Color defaultOceanColor = new Color32(0, 19, 30, 255);

		[Range(0f, 360f)]
		public float windDir;

		[Range(0f, 1f)]
		public float specularRoughness = 0.2f;

		[Range(0f, 1f)]
		public float specularIntensity = 0.2f;

		[Range(0f, 10f)]
		public float fresnelPower = 5f;

		[Range(0f, 1f)]
		public float minFresnel = 0.02f;

		public Color foamTint = Color.white;

		[Range(0f, 3f)]
		public float foamIntensity = 1f;

		public FoamTexture foamTexture0;

		public FoamTexture foamTexture1;

		[HideInInspector]
		public Shader waveOverlaySdr;

		private Vector3 m_positionOffset;

		private Dictionary<Camera, CameraData> m_cameraData = new Dictionary<Camera, CameraData>();

		private Material m_waveOverlayMat;

		private Scheduler m_scheduler;

		private WaveQuery m_query = new WaveQuery(0f, 0f);

		public static Ocean Instance
		{
			get;
			private set;
		}

		public static float MAX_WAVE_HEIGHT
		{
			get
			{
				return 60f;
			}
		}

		public bool ProjectSceneView
		{
			get
			{
				return true;
			}
		}

		public Vector3 WindDirVector
		{
			get;
			private set;
		}

		public OceanGridBase Grid
		{
			get;
			set;
		}

		public ReflectionBase Reflection
		{
			get;
			set;
		}

		public WaveSpectrumBase Spectrum
		{
			get;
			set;
		}

		public UnderWaterBase UnderWater
		{
			get;
			set;
		}

		public IOceanTime OceanTime
		{
			get;
			set;
		}

		public IProjection Projection
		{
			get;
			private set;
		}

		public Vector3 PositionOffset
		{
			get
			{
				return this.m_positionOffset;
			}
			set
			{
				this.m_positionOffset = value;
				Shader.SetGlobalVector("Ceto_PosOffset", this.m_positionOffset);
			}
		}

		public int CameraCount
		{
			get
			{
				return this.m_cameraData.Count;
			}
		}

		public OverlayManager OverlayManager
		{
			get;
			private set;
		}

		public bool WasError
		{
			get;
			private set;
		}

		private void Awake()
		{
			try
			{
				if (Ocean.Instance != null)
				{
					throw new InvalidOperationException("There can only be one ocean instance.");
				}
				Ocean.Instance = this;
				this.WindDirVector = this.CalculateWindDirVector();
				if (this.doublePrecisionProjection)
				{
					this.Projection = new Projection3d(this);
				}
				else
				{
					this.Projection = new Projection3f(this);
				}
				this.OceanTime = new OceanTime();
				this.m_waveOverlayMat = new Material(this.waveOverlaySdr);
				this.OverlayManager = new OverlayManager(this.m_waveOverlayMat);
				this.m_scheduler = new Scheduler();
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		private void Start()
		{
			try
			{
				Matrix4x4 identity = Matrix4x4.identity;
				identity.m00 = 2f;
				identity.m03 = -1f;
				identity.m11 = 2f;
				identity.m13 = -1f;
				for (int i = 0; i < 4; i++)
				{
					identity[1, i] = -identity[1, i];
				}
				Shader.SetGlobalMatrix("Ceto_T2S", identity);
				Shader.SetGlobalTexture("Ceto_Overlay_NormalMap", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_Overlay_HeightMap", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_Overlay_FoamMap", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_Overlay_ClipMap", Texture2D.blackTexture);
				Shader.SetGlobalTexture(Ocean.REFRACTION_GRAB_TEXTURE_NAME, Texture2D.blackTexture);
				Shader.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, Texture2D.whiteTexture);
				Shader.SetGlobalTexture(Ocean.OCEAN_MASK_TEXTURE_NAME, Texture2D.blackTexture);
				Shader.SetGlobalTexture(Ocean.OCEAN_DEPTH_TEXTURE_NAME, Texture2D.blackTexture);
				Shader.SetGlobalTexture(Ocean.NORMAL_FADE_TEXTURE_NAME, Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_FoamMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_FoamMap1", Texture2D.blackTexture);
				Shader.SetGlobalVector("Ceto_GridSizes", Vector4.one);
				Shader.SetGlobalVector("Ceto_GridScale", Vector4.one);
				Shader.SetGlobalVector("Ceto_Choppyness", Vector4.one);
				Shader.SetGlobalFloat("Ceto_MapSize", 1f);
				Shader.SetGlobalColor("Ceto_FoamTint", Color.white);
				Shader.SetGlobalTexture("Ceto_FoamTexture0", Texture2D.whiteTexture);
				Shader.SetGlobalTexture("Ceto_FoamTexture1", Texture2D.whiteTexture);
				Shader.SetGlobalVector("Ceto_FoamTextureScale0", Vector4.one);
				Shader.SetGlobalVector("Ceto_FoamTextureScale1", Vector4.one);
				Shader.SetGlobalFloat("Ceto_MaxWaveHeight", 40f);
				Shader.SetGlobalVector("Ceto_PosOffset", Vector3.zero);
				Shader.SetGlobalTexture("Ceto_CausticTexture", Texture2D.blackTexture);
				Shader.SetGlobalVector("Ceto_CausticTextureScale", new Vector4(1f, 1f, 0f, 0f));
				Shader.SetGlobalColor("Ceto_CausticTint", Color.white);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		private void OnEnable()
		{
			if (this.WasError)
			{
				this.DisableOcean();
			}
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(this.OceanOnPostRender));
		}

		private void OnDisable()
		{
			if (!this.WasError)
			{
				base.enabled = true;
			}
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(this.OceanOnPostRender));
		}

		private void DisableOcean()
		{
			this.WasError = true;
			base.enabled = false;
			base.gameObject.AddComponent<DisableGameObject>();
		}

		private void Update()
		{
			try
			{
				this.WindDirVector = this.CalculateWindDirVector();
				this.UpdateOceanScheduler();
				this.OverlayManager.Update();
				this.specularRoughness = Mathf.Clamp01(this.specularRoughness);
				this.specularIntensity = Mathf.Max(0f, this.specularIntensity);
				this.minFresnel = Mathf.Clamp01(this.minFresnel);
				this.fresnelPower = Mathf.Max(0f, this.fresnelPower);
				this.foamIntensity = Math.Max(0f, this.foamIntensity);
				float value = Mathf.Lerp(2E-05f, 0.02f, this.specularRoughness);
				Shader.SetGlobalColor("Ceto_DefaultSkyColor", this.defaultSkyColor);
				Shader.SetGlobalColor("Ceto_DefaultOceanColor", this.defaultOceanColor);
				Shader.SetGlobalFloat("Ceto_SpecularRoughness", value);
				Shader.SetGlobalFloat("Ceto_FresnelPower", this.fresnelPower);
				Shader.SetGlobalFloat("Ceto_SpecularIntensity", this.specularIntensity);
				Shader.SetGlobalFloat("Ceto_MinFresnel", this.minFresnel);
				Shader.SetGlobalFloat("Ceto_OceanLevel", this.level);
				Shader.SetGlobalFloat("Ceto_MaxWaveHeight", 40f);
				Shader.SetGlobalColor("Ceto_FoamTint", this.foamTint * this.foamIntensity);
				Shader.SetGlobalVector("Ceto_SunDir", this.SunDir());
				Shader.SetGlobalVector("Ceto_SunColor", this.SunColor());
				Vector4 vec = default(Vector4);
				vec.x = ((this.foamTexture0.scale.x == 0f) ? 1f : (1f / this.foamTexture0.scale.x));
				vec.y = ((this.foamTexture0.scale.y == 0f) ? 1f : (1f / this.foamTexture0.scale.y));
				vec.z = this.foamTexture0.scrollSpeed * this.OceanTime.Now;
				vec.w = 0f;
				Vector4 vec2 = default(Vector4);
				vec2.x = ((this.foamTexture1.scale.x == 0f) ? 1f : (1f / this.foamTexture1.scale.x));
				vec2.y = ((this.foamTexture1.scale.y == 0f) ? 1f : (1f / this.foamTexture1.scale.y));
				vec2.z = this.foamTexture1.scrollSpeed * this.OceanTime.Now;
				vec2.w = 0f;
				Shader.SetGlobalTexture("Ceto_FoamTexture0", (!(this.foamTexture0.tex != null)) ? Texture2D.whiteTexture : this.foamTexture0.tex);
				Shader.SetGlobalVector("Ceto_FoamTextureScale0", vec);
				Shader.SetGlobalTexture("Ceto_FoamTexture1", (!(this.foamTexture1.tex != null)) ? Texture2D.whiteTexture : this.foamTexture1.tex);
				Shader.SetGlobalVector("Ceto_FoamTextureScale1", vec2);
				Dictionary<Camera, CameraData>.Enumerator enumerator = this.m_cameraData.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<Camera, CameraData> current = enumerator.Current;
					CameraData value2 = current.Value;
					if (value2.mask != null)
					{
						value2.mask.updated = false;
					}
					if (value2.depth != null)
					{
						value2.depth.updated = false;
					}
					if (value2.overlay != null)
					{
						value2.overlay.updated = false;
					}
					if (value2.reflection != null)
					{
						value2.reflection.updated = false;
					}
					if (value2.projection != null)
					{
						value2.projection.updated = false;
						value2.projection.checkedForFlipping = false;
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		private void OnDestroy()
		{
			try
			{
				Ocean.Instance = null;
				if (this.OverlayManager != null)
				{
					this.OverlayManager.Release();
				}
				if (this.m_scheduler != null)
				{
					this.m_scheduler.ShutingDown = true;
					this.m_scheduler.CancelAllTasks();
				}
				List<Camera> list = new List<Camera>(this.m_cameraData.Keys);
				foreach (Camera current in list)
				{
					this.RemoveCameraData(current);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		public void Register(OceanComponent component)
		{
			if (component is OceanGridBase)
			{
				this.Grid = (component as OceanGridBase);
			}
			else if (component is WaveSpectrumBase)
			{
				this.Spectrum = (component as WaveSpectrumBase);
			}
			else if (component is ReflectionBase)
			{
				this.Reflection = (component as ReflectionBase);
			}
			else
			{
				if (!(component is UnderWaterBase))
				{
					throw new InvalidCastException("Could not cast ocean component " + component.GetType());
				}
				this.UnderWater = (component as UnderWaterBase);
			}
		}

		public void Deregister(OceanComponent component)
		{
			if (component is OceanGridBase)
			{
				this.Grid = null;
			}
			else if (component is WaveSpectrumBase)
			{
				this.Spectrum = null;
			}
			else if (component is ReflectionBase)
			{
				this.Reflection = null;
			}
			else
			{
				if (!(component is UnderWaterBase))
				{
					throw new InvalidCastException("Could not cast ocean component " + component.GetType());
				}
				this.UnderWater = null;
			}
		}

		public static void LogError(string msg)
		{
			Debug.Log("<color=red>Ceto (" + Ocean.VERSION + ") Error:</color> " + msg);
		}

		public static void LogWarning(string msg)
		{
			if (Ocean.Instance != null && Ocean.Instance.disableWarnings)
			{
				return;
			}
			Debug.Log("<color=yellow>Ceto (" + Ocean.VERSION + ") Warning:</color> " + msg);
		}

		public static void LogInfo(string msg)
		{
			if (Ocean.Instance != null && Ocean.Instance.disableInfo)
			{
				return;
			}
			Debug.Log("<color=cyan>Ceto (" + Ocean.VERSION + ") Info:</color> " + msg);
		}

		private void UpdateOceanScheduler()
		{
			try
			{
				if (this.m_scheduler != null)
				{
					this.m_scheduler.DisableMultithreading = Ocean.DISABLE_ALL_MULTITHREADING;
					this.m_scheduler.CheckForException();
					this.m_scheduler.Update();
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		public Vector3 CalculateWindDirVector()
		{
			float f = this.windDir * 3.14159274f / 180f;
			float x = -Mathf.Cos(f);
			float z = Mathf.Sin(f);
			Vector3 vector = new Vector3(x, 0f, z);
			return vector.normalized;
		}

		public Vector3 SunDir()
		{
			if (this.m_sun == null || this.m_sun.GetComponent<Light>() == null)
			{
				return Vector3.up;
			}
			return this.m_sun.transform.forward * -1f;
		}

		public Color SunColor()
		{
			if (this.m_sun == null || this.m_sun.GetComponent<Light>() == null)
			{
				return Color.white;
			}
			return this.m_sun.GetComponent<Light>().color;
		}

		public CameraData FindCameraData(Camera cam)
		{
			if (cam == null)
			{
				throw new InvalidOperationException("Can not find camera data for null camera");
			}
			CameraData cameraData = null;
			if (!this.m_cameraData.TryGetValue(cam, out cameraData))
			{
				cameraData = new CameraData();
				this.m_cameraData.Add(cam, cameraData);
			}
			if (cameraData.settings == null && !cameraData.checkedForSettings)
			{
				cameraData.settings = cam.GetComponent<OceanCameraSettings>();
				cameraData.checkedForSettings = true;
			}
			return cameraData;
		}

		public void RemoveCameraData(Camera cam)
		{
			if (!this.m_cameraData.ContainsKey(cam))
			{
				return;
			}
			CameraData cameraData = this.m_cameraData[cam];
			if (cameraData.overlay != null)
			{
				this.OverlayManager.DestroyBuffers(cameraData.overlay);
			}
			if (cameraData.reflection != null)
			{
				RTUtility.ReleaseAndDestroy(cameraData.reflection.tex);
				cameraData.reflection.tex = null;
				if (cameraData.reflection.cam != null)
				{
					RTUtility.ReleaseAndDestroy(cameraData.reflection.cam.targetTexture);
					cameraData.reflection.cam.targetTexture = null;
					UnityEngine.Object.Destroy(cameraData.reflection.cam.gameObject);
					UnityEngine.Object.Destroy(cameraData.reflection.cam);
					cameraData.reflection.cam = null;
				}
			}
			if (cameraData.depth != null && cameraData.depth.cam != null)
			{
				RTUtility.ReleaseAndDestroy(cameraData.depth.cam.targetTexture);
				cameraData.depth.cam.targetTexture = null;
				UnityEngine.Object.Destroy(cameraData.depth.cam.gameObject);
				UnityEngine.Object.Destroy(cameraData.depth.cam);
				cameraData.depth.cam = null;
			}
			if (cameraData.mask != null && cameraData.mask.cam != null)
			{
				RTUtility.ReleaseAndDestroy(cameraData.mask.cam.targetTexture);
				cameraData.mask.cam.targetTexture = null;
				UnityEngine.Object.Destroy(cameraData.mask.cam.gameObject);
				UnityEngine.Object.Destroy(cameraData.mask.cam);
				cameraData.mask.cam = null;
			}
			this.m_cameraData.Remove(cam);
		}

		public float FindMaxDisplacement(bool inculdeOverlays)
		{
			float num = 0f;
			if (this.Spectrum != null)
			{
				num += this.Spectrum.MaxDisplacement.y;
			}
			if (inculdeOverlays && this.OverlayManager != null)
			{
				num += this.OverlayManager.MaxDisplacement;
			}
			return Mathf.Max(0f, num);
		}

		public void SetQueryWavesSampling(bool overlays, bool grid0, bool grid1, bool grid2, bool grid3)
		{
			this.m_query.sampleOverlay = overlays;
			this.m_query.sampleSpectrum[0] = grid0;
			this.m_query.sampleSpectrum[1] = grid1;
			this.m_query.sampleSpectrum[2] = grid2;
			this.m_query.sampleSpectrum[3] = grid3;
		}

		public Vector3 QueryNormal(float x, float z)
		{
			float num = 0.5f;
			float num2 = this.QueryWaves(x - num, z);
			float num3 = this.QueryWaves(x, z - num);
			float num4 = this.QueryWaves(x + num, z);
			float num5 = this.QueryWaves(x, z + num);
			float num6 = num2 - num4;
			float num7 = num3 - num5;
			Vector3 result = new Vector3(num6, Mathf.Sqrt(1f - num6 * num6 - num7 * num7), num7);
			return result;
		}

		public float QueryWaves(float x, float z)
		{
			this.m_query.result.Clear();
			this.m_query.posX = x;
			this.m_query.posZ = z;
			if (base.enabled)
			{
				if (this.Spectrum != null)
				{
					this.Spectrum.QueryWaves(this.m_query);
				}
				if (this.OverlayManager != null)
				{
					this.OverlayManager.QueryWaves(this.m_query);
				}
			}
			return this.m_query.result.height + this.level;
		}

		public float QueryWaves(float x, float z, out bool isClipped)
		{
			this.m_query.result.Clear();
			this.m_query.posX = x;
			this.m_query.posZ = z;
			if (base.enabled)
			{
				if (this.Spectrum != null)
				{
					this.Spectrum.QueryWaves(this.m_query);
				}
				if (this.OverlayManager != null)
				{
					this.OverlayManager.QueryWaves(this.m_query);
				}
			}
			isClipped = this.m_query.result.isClipped;
			return this.m_query.result.height + this.level;
		}

		public void QueryWaves(WaveQuery query)
		{
			query.result.Clear();
			if (base.enabled)
			{
				if (this.Spectrum != null)
				{
					this.Spectrum.QueryWaves(query);
				}
				if (this.OverlayManager != null)
				{
					this.OverlayManager.QueryWaves(query);
				}
			}
			query.result.height = query.result.height + this.level;
		}

		public void QueryWaves(IEnumerable<WaveQuery> querys)
		{
			foreach (WaveQuery current in querys)
			{
				current.result.Clear();
				if (base.enabled)
				{
					if (this.Spectrum != null)
					{
						this.Spectrum.QueryWaves(current);
					}
					if (this.OverlayManager != null)
					{
						this.OverlayManager.QueryWaves(current);
					}
				}
				WaveQuery expr_63_cp_0 = current;
				expr_63_cp_0.result.height = expr_63_cp_0.result.height + this.level;
			}
		}

		public void QueryWavesAsync(IEnumerable<WaveQuery> querys, Action<IEnumerable<WaveQuery>> callBack)
		{
			IDisplacementBuffer displacementBuffer = this.Spectrum.DisplacementBuffer;
			if (base.enabled && this.Spectrum != null && displacementBuffer != null && (!displacementBuffer.IsGPU || !this.Spectrum.DisableReadBack))
			{
				WaveQueryTask task = new WaveQueryTask(this.Spectrum, this.level, this.PositionOffset, querys, callBack);
				this.m_scheduler.Run(task);
			}
			else
			{
				foreach (WaveQuery current in querys)
				{
					current.result.Clear();
					current.result.height = this.level;
				}
				callBack(querys);
			}
		}

		private void OceanOnPreRender(Camera cam)
		{
			if (!cam.enabled)
			{
				return;
			}
			CameraData data = this.FindCameraData(cam);
			if (this.Grid != null)
			{
				this.Grid.OceanOnPreRender(cam, data);
			}
			if (this.Spectrum != null)
			{
				this.Spectrum.OceanOnPreRender(cam, data);
			}
			if (this.Reflection != null)
			{
				this.Reflection.OceanOnPreRender(cam, data);
			}
			if (this.UnderWater != null)
			{
				this.UnderWater.OceanOnPreRender(cam, data);
			}
		}

		private void OceanOnPreCull(Camera cam)
		{
			if (!cam.enabled)
			{
				return;
			}
			CameraData data = this.FindCameraData(cam);
			if (this.Grid != null)
			{
				this.Grid.OceanOnPreCull(cam, data);
			}
			if (this.Spectrum != null)
			{
				this.Spectrum.OceanOnPreCull(cam, data);
			}
			if (this.Reflection != null)
			{
				this.Reflection.OceanOnPreCull(cam, data);
			}
			if (this.UnderWater != null)
			{
				this.UnderWater.OceanOnPreCull(cam, data);
			}
		}

		private void OceanOnPostRender(Camera cam)
		{
			if (!cam.enabled)
			{
				return;
			}
			CameraData data = this.FindCameraData(cam);
			if (this.Grid != null)
			{
				this.Grid.OceanOnPostRender(cam, data);
			}
			if (this.Spectrum != null)
			{
				this.Spectrum.OceanOnPostRender(cam, data);
			}
			if (this.Reflection != null)
			{
				this.Reflection.OceanOnPostRender(cam, data);
			}
			if (this.UnderWater != null)
			{
				this.UnderWater.OceanOnPostRender(cam, data);
			}
		}

		private bool GetDisableAllOverlays(OceanCameraSettings settings)
		{
			return settings != null && settings.disableAllOverlays;
		}

		public void RenderWaveOverlays(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					Camera current = Camera.current;
					if (!this.m_cameraData.ContainsKey(current))
					{
						this.m_cameraData.Add(current, new CameraData());
					}
					CameraData cameraData = this.m_cameraData[current];
					if (cameraData.overlay == null)
					{
						cameraData.overlay = new WaveOverlayData();
					}
					if (cameraData.projection == null)
					{
						cameraData.projection = new ProjectionData();
					}
					if (!cameraData.overlay.updated)
					{
						if (!cameraData.projection.updated)
						{
							this.Projection.UpdateProjection(current, cameraData, this.ProjectSceneView);
							Shader.SetGlobalMatrix("Ceto_Interpolation", cameraData.projection.interpolation);
							Shader.SetGlobalMatrix("Ceto_ProjectorVP", cameraData.projection.projectorVP);
						}
						if (this.GetDisableAllOverlays(cameraData.settings))
						{
							this.OverlayManager.DestroyBuffers(cameraData.overlay);
							Shader.SetGlobalTexture("Ceto_Overlay_NormalMap", Texture2D.blackTexture);
							Shader.SetGlobalTexture("Ceto_Overlay_HeightMap", Texture2D.blackTexture);
							Shader.SetGlobalTexture("Ceto_Overlay_FoamMap", Texture2D.blackTexture);
							Shader.SetGlobalTexture("Ceto_Overlay_ClipMap", Texture2D.blackTexture);
						}
						else
						{
							OVERLAY_MAP_SIZE oVERLAY_MAP_SIZE = (!(cameraData.settings != null)) ? this.normalOverlaySize : cameraData.settings.normalOverlaySize;
							OVERLAY_MAP_SIZE oVERLAY_MAP_SIZE2 = (!(cameraData.settings != null)) ? this.heightOverlaySize : cameraData.settings.heightOverlaySize;
							OVERLAY_MAP_SIZE oVERLAY_MAP_SIZE3 = (!(cameraData.settings != null)) ? this.foamOverlaySize : cameraData.settings.foamOverlaySize;
							OVERLAY_MAP_SIZE oVERLAY_MAP_SIZE4 = (!(cameraData.settings != null)) ? this.clipOverlaySize : cameraData.settings.clipOverlaySize;
							this.OverlayManager.CreateOverlays(current, cameraData.overlay, oVERLAY_MAP_SIZE, oVERLAY_MAP_SIZE2, oVERLAY_MAP_SIZE3, oVERLAY_MAP_SIZE4);
							this.OverlayManager.HeightOverlayBlendMode = this.heightBlendMode;
							this.OverlayManager.FoamOverlayBlendMode = this.foamBlendMode;
							this.OverlayManager.RenderWaveOverlays(current, cameraData.overlay);
						}
						cameraData.overlay.updated = true;
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		public void RenderReflection(GameObject go)
		{
			if (!base.enabled || this.Reflection == null)
			{
				return;
			}
			this.Reflection.RenderReflection(go);
		}

		public void RenderOceanMask(GameObject go)
		{
			if (!base.enabled || this.UnderWater == null)
			{
				return;
			}
			this.UnderWater.RenderOceanMask(go);
		}

		public void RenderOceanDepth(GameObject go)
		{
			if (!base.enabled || this.UnderWater == null)
			{
				return;
			}
			this.UnderWater.RenderOceanDepth(go);
		}
	}
}
