using AmplifyMotion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[AddComponentMenu(""), RequireComponent(typeof(Camera))]
public class AmplifyMotionEffectBase : MonoBehaviour
{
	private const CameraEvent m_updateCBEvent = CameraEvent.BeforeImageEffectsOpaque;

	private const CameraEvent m_fixedUpdateCBEvent = CameraEvent.BeforeImageEffectsOpaque;

	private const CameraEvent m_renderCBEvent = CameraEvent.BeforeImageEffects;

	public Quality QualityLevel = Quality.Standard;

	public bool AutoRegisterObjs = true;

	public Camera[] OverlayCameras = new Camera[0];

	public LayerMask CullingMask = -1;

	public int QualitySteps = 1;

	public float MotionScale = 3f;

	public float CameraMotionMult = 1f;

	public float MinVelocity = 1f;

	public float MaxVelocity = 10f;

	public float DepthThreshold = 0.001f;

	[FormerlySerializedAs("workerThreads")]
	public int WorkerThreads;

	public bool SystemThreadPool;

	public bool ForceCPUOnly;

	public bool DebugMode;

	private amplifyDisableHook adh;

	private Camera m_camera;

	private bool m_starting = true;

	private int m_width;

	private int m_height;

	private RenderTexture m_motionRT;

	private Material m_blurMaterial;

	private Material m_solidVectorsMaterial;

	private Material m_skinnedVectorsMaterial;

	private Material m_clothVectorsMaterial;

	private Material m_reprojectionMaterial;

	private Material m_combineMaterial;

	private Material m_dilationMaterial;

	private Material m_depthMaterial;

	private Material m_debugMaterial;

	private Dictionary<Camera, AmplifyMotionCamera> m_linkedCameras = new Dictionary<Camera, AmplifyMotionCamera>();

	internal Camera[] m_linkedCameraKeys;

	internal AmplifyMotionCamera[] m_linkedCameraValues;

	internal bool m_linkedCamerasChanged = true;

	private AmplifyMotionPostProcess m_currentPostProcess;

	private int m_globalObjectId = 1;

	private float m_deltaTime;

	private float m_fixedDeltaTime;

	private float m_motionScaleNorm;

	private float m_fixedMotionScaleNorm;

	private Quality m_qualityLevel;

	private AmplifyMotionCamera m_baseCamera;

	private WorkerThreadPool m_workerThreadPool;

	public static Dictionary<GameObject, AmplifyMotionObjectBase> m_activeObjects = new Dictionary<GameObject, AmplifyMotionObjectBase>();

	public static Dictionary<Camera, AmplifyMotionCamera> m_activeCameras = new Dictionary<Camera, AmplifyMotionCamera>();

	private static bool m_isD3D = false;

	private bool m_canUseGPU;

	private CommandBuffer m_updateCB;

	private CommandBuffer m_fixedUpdateCB;

	private CommandBuffer m_renderCB;

	private static bool m_ignoreMotionScaleWarning = false;

	private static AmplifyMotionEffectBase m_firstInstance = null;

	[Obsolete("workerThreads is deprecated, please use WorkerThreads instead.")]
	public int workerThreads
	{
		get
		{
			return this.WorkerThreads;
		}
		set
		{
			this.WorkerThreads = value;
		}
	}

	internal Material SolidVectorsMaterial
	{
		get
		{
			return this.m_solidVectorsMaterial;
		}
	}

	internal Material SkinnedVectorsMaterial
	{
		get
		{
			return this.m_skinnedVectorsMaterial;
		}
	}

	internal Material ClothVectorsMaterial
	{
		get
		{
			return this.m_clothVectorsMaterial;
		}
	}

	internal RenderTexture MotionRenderTexture
	{
		get
		{
			return this.m_motionRT;
		}
	}

	public Dictionary<Camera, AmplifyMotionCamera> LinkedCameras
	{
		get
		{
			return this.m_linkedCameras;
		}
	}

	internal float MotionScaleNorm
	{
		get
		{
			return this.m_motionScaleNorm;
		}
	}

	internal float FixedMotionScaleNorm
	{
		get
		{
			return this.m_fixedMotionScaleNorm;
		}
	}

	public AmplifyMotionCamera BaseCamera
	{
		get
		{
			return this.m_baseCamera;
		}
	}

	internal WorkerThreadPool WorkerPool
	{
		get
		{
			return this.m_workerThreadPool;
		}
	}

	public static bool IsD3D
	{
		get
		{
			return AmplifyMotionEffectBase.m_isD3D;
		}
	}

	public bool CanUseGPU
	{
		get
		{
			return this.m_canUseGPU;
		}
	}

	public static bool IgnoreMotionScaleWarning
	{
		get
		{
			return AmplifyMotionEffectBase.m_ignoreMotionScaleWarning;
		}
	}

	public static AmplifyMotionEffectBase FirstInstance
	{
		get
		{
			return AmplifyMotionEffectBase.m_firstInstance;
		}
	}

	public static AmplifyMotionEffectBase Instance
	{
		get
		{
			return AmplifyMotionEffectBase.m_firstInstance;
		}
	}

	private void Awake()
	{
		if (AmplifyMotionEffectBase.m_firstInstance == null)
		{
			AmplifyMotionEffectBase.m_firstInstance = this;
		}
		AmplifyMotionEffectBase.m_isD3D = SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D");
		this.m_globalObjectId = 1;
		this.m_width = (this.m_height = 0);
		if (this.ForceCPUOnly)
		{
			this.m_canUseGPU = false;
		}
		else
		{
			bool supportsRenderTextures = SystemInfo.supportsRenderTextures;
			bool flag = SystemInfo.graphicsShaderLevel >= 30;
			bool flag2 = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf);
			bool flag3 = SystemInfo.SupportsTextureFormat(TextureFormat.RGHalf);
			bool flag4 = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf);
			bool flag5 = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat);
			this.m_canUseGPU = (supportsRenderTextures && flag && flag2 && flag3 && flag4 && flag5);
		}
	}

	internal void ResetObjectId()
	{
		this.m_globalObjectId = 1;
	}

	internal int GenerateObjectId(GameObject obj)
	{
		if (obj.isStatic)
		{
			return 0;
		}
		this.m_globalObjectId++;
		if (this.m_globalObjectId > 254)
		{
			this.m_globalObjectId = 1;
		}
		return this.m_globalObjectId;
	}

	private void SafeDestroyMaterial(ref Material mat)
	{
		if (mat != null)
		{
			UnityEngine.Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	private bool CheckMaterialAndShader(Material material, string name)
	{
		bool result = true;
		if (material == null || material.shader == null)
		{
			Debug.LogWarning("[AmplifyMotion] Error creating " + name + " material");
			result = false;
		}
		else if (!material.shader.isSupported)
		{
			Debug.LogWarning("[AmplifyMotion] " + name + " shader not supported on this platform");
			result = false;
		}
		return result;
	}

	private void DestroyMaterials()
	{
		this.SafeDestroyMaterial(ref this.m_blurMaterial);
		this.SafeDestroyMaterial(ref this.m_solidVectorsMaterial);
		this.SafeDestroyMaterial(ref this.m_skinnedVectorsMaterial);
		this.SafeDestroyMaterial(ref this.m_clothVectorsMaterial);
		this.SafeDestroyMaterial(ref this.m_reprojectionMaterial);
		this.SafeDestroyMaterial(ref this.m_combineMaterial);
		this.SafeDestroyMaterial(ref this.m_dilationMaterial);
		this.SafeDestroyMaterial(ref this.m_depthMaterial);
		this.SafeDestroyMaterial(ref this.m_debugMaterial);
	}

	private bool CreateMaterials()
	{
		this.DestroyMaterials();
		int num = (SystemInfo.graphicsShaderLevel < 30) ? 2 : 3;
		string name = "Hidden/Amplify Motion/MotionBlurSM" + num;
		string name2 = "Hidden/Amplify Motion/SolidVectors";
		string name3 = "Hidden/Amplify Motion/SkinnedVectors";
		string name4 = "Hidden/Amplify Motion/ClothVectors";
		string name5 = "Hidden/Amplify Motion/ReprojectionVectors";
		string name6 = "Hidden/Amplify Motion/Combine";
		string name7 = "Hidden/Amplify Motion/Dilation";
		string name8 = "Hidden/Amplify Motion/Depth";
		string name9 = "Hidden/Amplify Motion/Debug";
		try
		{
			this.m_blurMaterial = new Material(Shader.Find(name))
			{
				hideFlags = HideFlags.DontSave
			};
			this.m_solidVectorsMaterial = new Material(Shader.Find(name2))
			{
				hideFlags = HideFlags.DontSave
			};
			this.m_skinnedVectorsMaterial = new Material(Shader.Find(name3))
			{
				hideFlags = HideFlags.DontSave
			};
			this.m_clothVectorsMaterial = new Material(Shader.Find(name4))
			{
				hideFlags = HideFlags.DontSave
			};
			this.m_reprojectionMaterial = new Material(Shader.Find(name5))
			{
				hideFlags = HideFlags.DontSave
			};
			this.m_combineMaterial = new Material(Shader.Find(name6))
			{
				hideFlags = HideFlags.DontSave
			};
			this.m_dilationMaterial = new Material(Shader.Find(name7))
			{
				hideFlags = HideFlags.DontSave
			};
			this.m_depthMaterial = new Material(Shader.Find(name8))
			{
				hideFlags = HideFlags.DontSave
			};
			this.m_debugMaterial = new Material(Shader.Find(name9))
			{
				hideFlags = HideFlags.DontSave
			};
		}
		catch (Exception)
		{
		}
		bool flag = this.CheckMaterialAndShader(this.m_blurMaterial, name);
		flag = (flag && this.CheckMaterialAndShader(this.m_solidVectorsMaterial, name2));
		flag = (flag && this.CheckMaterialAndShader(this.m_skinnedVectorsMaterial, name3));
		flag = (flag && this.CheckMaterialAndShader(this.m_clothVectorsMaterial, name4));
		flag = (flag && this.CheckMaterialAndShader(this.m_reprojectionMaterial, name5));
		flag = (flag && this.CheckMaterialAndShader(this.m_combineMaterial, name6));
		flag = (flag && this.CheckMaterialAndShader(this.m_dilationMaterial, name7));
		flag = (flag && this.CheckMaterialAndShader(this.m_depthMaterial, name8));
		return flag && this.CheckMaterialAndShader(this.m_debugMaterial, name9);
	}

	private RenderTexture CreateRenderTexture(string name, int depth, RenderTextureFormat fmt, RenderTextureReadWrite rw, FilterMode fm)
	{
		RenderTexture renderTexture = new RenderTexture(this.m_width, this.m_height, depth, fmt, rw);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = fm;
		renderTexture.Create();
		return renderTexture;
	}

	private void SafeDestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt != null)
		{
			RenderTexture.active = null;
			rt.Release();
			UnityEngine.Object.DestroyImmediate(rt);
			rt = null;
		}
	}

	private void SafeDestroyTexture(ref Texture tex)
	{
		if (tex != null)
		{
			UnityEngine.Object.DestroyImmediate(tex);
			tex = null;
		}
	}

	private void DestroyRenderTextures()
	{
		RenderTexture.active = null;
		this.SafeDestroyRenderTexture(ref this.m_motionRT);
	}

	private void UpdateRenderTextures(bool qualityChanged)
	{
		int num = Mathf.FloorToInt((float)this.m_camera.pixelWidth + 0.5f);
		int num2 = Mathf.FloorToInt((float)this.m_camera.pixelHeight + 0.5f);
		if (this.QualityLevel == Quality.Mobile)
		{
			num /= 2;
			num2 /= 2;
		}
		if (this.m_width != num || this.m_height != num2 || qualityChanged)
		{
			this.m_width = num;
			this.m_height = num2;
			this.DestroyRenderTextures();
		}
		if (this.m_motionRT == null)
		{
			this.m_motionRT = this.CreateRenderTexture("AM-MotionVectors", 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, FilterMode.Point);
		}
	}

	public bool CheckSupport()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			Debug.LogError("[AmplifyMotion] Initialization failed. This plugin requires support for Image Effects and Render Textures.");
			return false;
		}
		return true;
	}

	private void InitializeThreadPool()
	{
		if (this.WorkerThreads <= 0)
		{
			this.WorkerThreads = Mathf.Max(Environment.ProcessorCount / 2, 1);
		}
		this.m_workerThreadPool = new WorkerThreadPool();
		this.m_workerThreadPool.InitializeAsyncUpdateThreads(this.WorkerThreads, this.SystemThreadPool);
	}

	private void ShutdownThreadPool()
	{
		if (this.m_workerThreadPool != null)
		{
			this.m_workerThreadPool.FinalizeAsyncUpdateThreads();
			this.m_workerThreadPool = null;
		}
	}

	private void InitializeCommandBuffers()
	{
		this.ShutdownCommandBuffers();
		this.m_updateCB = new CommandBuffer();
		this.m_updateCB.name = "AmplifyMotion.Update";
		this.m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_updateCB);
		this.m_fixedUpdateCB = new CommandBuffer();
		this.m_fixedUpdateCB.name = "AmplifyMotion.FixedUpdate";
		this.m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_fixedUpdateCB);
		this.m_renderCB = new CommandBuffer();
		this.m_renderCB.name = "AmplifyMotion.Render";
		this.m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, this.m_renderCB);
	}

	private void ShutdownCommandBuffers()
	{
		if (this.m_updateCB != null)
		{
			this.m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_updateCB);
			this.m_updateCB.Release();
			this.m_updateCB = null;
		}
		if (this.m_fixedUpdateCB != null)
		{
			this.m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, this.m_fixedUpdateCB);
			this.m_fixedUpdateCB.Release();
			this.m_fixedUpdateCB = null;
		}
		if (this.m_renderCB != null)
		{
			this.m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, this.m_renderCB);
			this.m_renderCB.Release();
			this.m_renderCB = null;
		}
	}

	private void OnEnable()
	{
		this.m_camera = base.GetComponent<Camera>();
		if (!this.CheckSupport())
		{
			base.enabled = false;
			return;
		}
		this.InitializeThreadPool();
		this.m_starting = true;
		if (!this.CreateMaterials())
		{
			Debug.LogError("[AmplifyMotion] Failed loading or compiling necessary shaders. Please try reinstalling Amplify Motion or contact support@amplify.pt");
			base.enabled = false;
			return;
		}
		if (this.AutoRegisterObjs)
		{
			this.UpdateActiveObjects();
		}
		this.InitializeCameras();
		this.InitializeCommandBuffers();
		this.UpdateRenderTextures(true);
		this.m_linkedCameras.TryGetValue(this.m_camera, out this.m_baseCamera);
		if (this.m_baseCamera == null)
		{
			Debug.LogError("[AmplifyMotion] Failed setting up Base Camera. Please contact support@amplify.pt");
			base.enabled = false;
			return;
		}
		if (this.m_currentPostProcess != null)
		{
			this.m_currentPostProcess.enabled = true;
		}
		this.m_qualityLevel = this.QualityLevel;
	}

	private void OnDisable()
	{
		if (this.m_currentPostProcess != null)
		{
			this.m_currentPostProcess.enabled = false;
		}
		this.ShutdownCommandBuffers();
		this.ShutdownThreadPool();
	}

	private void Start()
	{
		this.UpdatePostProcess();
	}

	internal void RemoveCamera(Camera reference)
	{
		this.m_linkedCameras.Remove(reference);
	}

	private void OnDestroy()
	{
		AmplifyMotionCamera[] array = this.m_linkedCameras.Values.ToArray<AmplifyMotionCamera>();
		AmplifyMotionCamera[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			AmplifyMotionCamera amplifyMotionCamera = array2[i];
			if (amplifyMotionCamera != null && amplifyMotionCamera.gameObject != base.gameObject)
			{
				Camera component = amplifyMotionCamera.GetComponent<Camera>();
				if (component != null)
				{
					component.targetTexture = null;
				}
				UnityEngine.Object.DestroyImmediate(amplifyMotionCamera);
			}
		}
		this.DestroyRenderTextures();
		this.DestroyMaterials();
	}

	private GameObject RecursiveFindCamera(GameObject obj, string auxCameraName)
	{
		GameObject gameObject = null;
		if (obj.name == auxCameraName)
		{
			gameObject = obj;
		}
		else
		{
			foreach (Transform transform in obj.transform)
			{
				gameObject = this.RecursiveFindCamera(transform.gameObject, auxCameraName);
				if (gameObject != null)
				{
					break;
				}
			}
		}
		return gameObject;
	}

	private void InitializeCameras()
	{
		List<Camera> list = new List<Camera>(this.OverlayCameras.Length);
		for (int i = 0; i < this.OverlayCameras.Length; i++)
		{
			if (this.OverlayCameras[i] != null)
			{
				list.Add(this.OverlayCameras[i]);
			}
		}
		Camera[] array = new Camera[list.Count + 1];
		array[0] = this.m_camera;
		for (int j = 0; j < list.Count; j++)
		{
			array[j + 1] = list[j];
		}
		this.m_linkedCameras.Clear();
		for (int k = 0; k < array.Length; k++)
		{
			Camera camera = array[k];
			if (!this.m_linkedCameras.ContainsKey(camera))
			{
				AmplifyMotionCamera amplifyMotionCamera = camera.gameObject.GetComponent<AmplifyMotionCamera>();
				if (amplifyMotionCamera != null)
				{
					amplifyMotionCamera.enabled = false;
					amplifyMotionCamera.enabled = true;
				}
				else
				{
					amplifyMotionCamera = camera.gameObject.AddComponent<AmplifyMotionCamera>();
				}
				amplifyMotionCamera.LinkTo(this, k > 0);
				this.m_linkedCameras.Add(camera, amplifyMotionCamera);
				this.m_linkedCamerasChanged = true;
			}
		}
	}

	public void UpdateActiveCameras()
	{
		this.InitializeCameras();
	}

	internal static void RegisterCamera(AmplifyMotionCamera cam)
	{
		AmplifyMotionEffectBase.m_activeCameras.Add(cam.GetComponent<Camera>(), cam);
		foreach (AmplifyMotionObjectBase current in AmplifyMotionEffectBase.m_activeObjects.Values)
		{
			current.RegisterCamera(cam);
		}
	}

	internal static void UnregisterCamera(AmplifyMotionCamera cam)
	{
		foreach (AmplifyMotionObjectBase current in AmplifyMotionEffectBase.m_activeObjects.Values)
		{
			current.UnregisterCamera(cam);
		}
		AmplifyMotionEffectBase.m_activeCameras.Remove(cam.GetComponent<Camera>());
	}

	public void UpdateActiveObjects()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		for (int i = 0; i < array.Length; i++)
		{
			if (!AmplifyMotionEffectBase.m_activeObjects.ContainsKey(array[i]))
			{
				AmplifyMotionEffectBase.TryRegister(array[i], true);
			}
		}
	}

	internal static void RegisterObject(AmplifyMotionObjectBase obj)
	{
		AmplifyMotionEffectBase.m_activeObjects.Add(obj.gameObject, obj);
		foreach (AmplifyMotionCamera current in AmplifyMotionEffectBase.m_activeCameras.Values)
		{
			obj.RegisterCamera(current);
		}
	}

	internal static void UnregisterObject(AmplifyMotionObjectBase obj)
	{
		foreach (AmplifyMotionCamera current in AmplifyMotionEffectBase.m_activeCameras.Values)
		{
			obj.UnregisterCamera(current);
		}
		AmplifyMotionEffectBase.m_activeObjects.Remove(obj.gameObject);
	}

	internal static bool FindValidTag(Material[] materials)
	{
		for (int i = 0; i < materials.Length; i++)
		{
			Material material = materials[i];
			if (material != null)
			{
				string tag = material.GetTag("RenderType", false);
				if (tag == "Opaque" || tag == "TransparentCutout")
				{
					return !material.IsKeywordEnabled("_ALPHABLEND_ON") && !material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON");
				}
			}
		}
		return false;
	}

	internal static bool CanRegister(GameObject gameObj, bool autoReg)
	{
		if (gameObj.isStatic)
		{
			return false;
		}
		Renderer component = gameObj.GetComponent<Renderer>();
		if (component == null || component.sharedMaterials == null || component.isPartOfStaticBatch)
		{
			return false;
		}
		if (!component.enabled)
		{
			return false;
		}
		if (component.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
		{
			return false;
		}
		if (component.GetType() == typeof(SpriteRenderer))
		{
			return false;
		}
		if (!AmplifyMotionEffectBase.FindValidTag(component.sharedMaterials))
		{
			return false;
		}
		Type type = component.GetType();
		return type == typeof(MeshRenderer) || type == typeof(SkinnedMeshRenderer);
	}

	internal static void TryRegister(GameObject gameObj, bool autoReg)
	{
		if (AmplifyMotionEffectBase.CanRegister(gameObj, autoReg) && gameObj.GetComponent<AmplifyMotionObjectBase>() == null)
		{
			AmplifyMotionObjectBase.ApplyToChildren = false;
			gameObj.AddComponent<AmplifyMotionObjectBase>();
			AmplifyMotionObjectBase.ApplyToChildren = true;
		}
	}

	internal static void TryUnregister(GameObject gameObj)
	{
		AmplifyMotionObjectBase component = gameObj.GetComponent<AmplifyMotionObjectBase>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	public void Register(GameObject gameObj)
	{
		if (!AmplifyMotionEffectBase.m_activeObjects.ContainsKey(gameObj))
		{
			AmplifyMotionEffectBase.TryRegister(gameObj, false);
		}
	}

	public static void RegisterS(GameObject gameObj)
	{
		if (!AmplifyMotionEffectBase.m_activeObjects.ContainsKey(gameObj))
		{
			AmplifyMotionEffectBase.TryRegister(gameObj, false);
		}
	}

	public void RegisterRecursively(GameObject gameObj)
	{
		if (!AmplifyMotionEffectBase.m_activeObjects.ContainsKey(gameObj))
		{
			AmplifyMotionEffectBase.TryRegister(gameObj, false);
		}
		foreach (Transform transform in gameObj.transform)
		{
			this.RegisterRecursively(transform.gameObject);
		}
	}

	public static void RegisterRecursivelyS(GameObject gameObj)
	{
		if (!AmplifyMotionEffectBase.m_activeObjects.ContainsKey(gameObj))
		{
			AmplifyMotionEffectBase.TryRegister(gameObj, false);
		}
		foreach (Transform transform in gameObj.transform)
		{
			AmplifyMotionEffectBase.RegisterRecursivelyS(transform.gameObject);
		}
	}

	public void Unregister(GameObject gameObj)
	{
		if (AmplifyMotionEffectBase.m_activeObjects.ContainsKey(gameObj))
		{
			AmplifyMotionEffectBase.TryUnregister(gameObj);
		}
	}

	public static void UnregisterS(GameObject gameObj)
	{
		if (AmplifyMotionEffectBase.m_activeObjects.ContainsKey(gameObj))
		{
			AmplifyMotionEffectBase.TryUnregister(gameObj);
		}
	}

	public void UnregisterRecursively(GameObject gameObj)
	{
		if (AmplifyMotionEffectBase.m_activeObjects.ContainsKey(gameObj))
		{
			AmplifyMotionEffectBase.TryUnregister(gameObj);
		}
		foreach (Transform transform in gameObj.transform)
		{
			this.UnregisterRecursively(transform.gameObject);
		}
	}

	public static void UnregisterRecursivelyS(GameObject gameObj)
	{
		if (AmplifyMotionEffectBase.m_activeObjects.ContainsKey(gameObj))
		{
			AmplifyMotionEffectBase.TryUnregister(gameObj);
		}
		foreach (Transform transform in gameObj.transform)
		{
			AmplifyMotionEffectBase.UnregisterRecursivelyS(transform.gameObject);
		}
	}

	private void UpdatePostProcess()
	{
		Camera camera = null;
		float num = -3.40282347E+38f;
		if (this.m_linkedCamerasChanged)
		{
			this.UpdateLinkedCameras();
		}
		for (int i = 0; i < this.m_linkedCameraKeys.Length; i++)
		{
			if (this.m_linkedCameraKeys[i] != null && this.m_linkedCameraKeys[i].isActiveAndEnabled && this.m_linkedCameraKeys[i].depth > num)
			{
				camera = this.m_linkedCameraKeys[i];
				num = this.m_linkedCameraKeys[i].depth;
			}
		}
		if (this.m_currentPostProcess != null && this.m_currentPostProcess.gameObject != camera.gameObject)
		{
			UnityEngine.Object.DestroyImmediate(this.m_currentPostProcess);
			this.m_currentPostProcess = null;
		}
		if (this.m_currentPostProcess == null && camera != null && camera != this.m_camera)
		{
			AmplifyMotionPostProcess[] components = base.gameObject.GetComponents<AmplifyMotionPostProcess>();
			if (components != null && components.Length > 0)
			{
				for (int j = 0; j < components.Length; j++)
				{
					UnityEngine.Object.DestroyImmediate(components[j]);
				}
			}
			this.m_currentPostProcess = camera.gameObject.AddComponent<AmplifyMotionPostProcess>();
			this.m_currentPostProcess.Instance = this;
		}
	}

	private void LateUpdate()
	{
		if (this.m_baseCamera.AutoStep)
		{
			float num = (!Application.isPlaying) ? Time.fixedDeltaTime : Time.deltaTime;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.m_deltaTime = ((num <= 1.401298E-45f) ? this.m_deltaTime : num);
			this.m_fixedDeltaTime = ((num <= 1.401298E-45f) ? this.m_fixedDeltaTime : fixedDeltaTime);
		}
		this.QualitySteps = Mathf.Clamp(this.QualitySteps, 0, 16);
		this.MotionScale = Mathf.Max(this.MotionScale, 0f);
		this.MinVelocity = Mathf.Min(this.MinVelocity, this.MaxVelocity);
		this.DepthThreshold = Mathf.Max(this.DepthThreshold, 0f);
		this.UpdatePostProcess();
	}

	public void StopAutoStep()
	{
		foreach (AmplifyMotionCamera current in this.m_linkedCameras.Values)
		{
			current.StopAutoStep();
		}
	}

	public void StartAutoStep()
	{
		foreach (AmplifyMotionCamera current in this.m_linkedCameras.Values)
		{
			current.StartAutoStep();
		}
	}

	public void Step(float delta)
	{
		this.m_deltaTime = delta;
		this.m_fixedDeltaTime = delta;
		foreach (AmplifyMotionCamera current in this.m_linkedCameras.Values)
		{
			current.Step();
		}
	}

	private void UpdateLinkedCameras()
	{
		Dictionary<Camera, AmplifyMotionCamera>.KeyCollection keys = this.m_linkedCameras.Keys;
		Dictionary<Camera, AmplifyMotionCamera>.ValueCollection values = this.m_linkedCameras.Values;
		if (this.m_linkedCameraKeys == null || keys.Count != this.m_linkedCameraKeys.Length)
		{
			this.m_linkedCameraKeys = new Camera[keys.Count];
		}
		if (this.m_linkedCameraValues == null || values.Count != this.m_linkedCameraValues.Length)
		{
			this.m_linkedCameraValues = new AmplifyMotionCamera[values.Count];
		}
		keys.CopyTo(this.m_linkedCameraKeys, 0);
		values.CopyTo(this.m_linkedCameraValues, 0);
		this.m_linkedCamerasChanged = false;
	}

	private void FixedUpdate()
	{
		if (this.m_camera.enabled)
		{
			if (this.m_linkedCamerasChanged)
			{
				this.UpdateLinkedCameras();
			}
			this.m_fixedUpdateCB.Clear();
			for (int i = 0; i < this.m_linkedCameraValues.Length; i++)
			{
				if (this.m_linkedCameraValues[i] != null && this.m_linkedCameraValues[i].isActiveAndEnabled)
				{
					this.m_linkedCameraValues[i].FixedUpdateTransform(this.m_fixedUpdateCB);
				}
			}
		}
	}

	private void OnPreRender()
	{
		if (this.m_camera.enabled && (Time.frameCount == 1 || Mathf.Abs(Time.deltaTime) > 1.401298E-45f))
		{
			if (this.m_linkedCamerasChanged)
			{
				this.UpdateLinkedCameras();
			}
			this.m_updateCB.Clear();
			for (int i = 0; i < this.m_linkedCameraValues.Length; i++)
			{
				if (this.m_linkedCameraValues[i] != null && this.m_linkedCameraValues[i].isActiveAndEnabled)
				{
					this.m_linkedCameraValues[i].UpdateTransform(this.m_updateCB);
				}
			}
		}
	}

	private void RenderReprojectionVectors(CommandBuffer commandBuffer, RenderTexture destination, float scale)
	{
		commandBuffer.SetGlobalMatrix("_AM_MATRIX_CURR_REPROJ", this.m_baseCamera.PrevViewProjMatrix * this.m_baseCamera.InvViewProjMatrix);
		commandBuffer.SetGlobalFloat("_AM_MOTION_SCALE", scale);
		RenderTexture rt = null;
		commandBuffer.Blit(new RenderTargetIdentifier(rt), destination, this.m_reprojectionMaterial);
	}

	public static void DiscardContents(RenderTexture rtex)
	{
		rtex.DiscardContents();
	}

	private void OnPostRender()
	{
		bool flag = this.QualityLevel != this.m_qualityLevel;
		if (flag)
		{
			this.CreateMaterials();
			this.m_qualityLevel = this.QualityLevel;
		}
		this.UpdateRenderTextures(flag);
		this.ResetObjectId();
		bool flag2 = this.CameraMotionMult > 1.401298E-45f;
		bool clearColor = !flag2 || this.m_starting;
		float y = (this.DepthThreshold <= 1.401298E-45f) ? 3.40282347E+38f : (1f / this.DepthThreshold);
		this.m_motionScaleNorm = ((this.m_deltaTime < 1.401298E-45f) ? 0f : (this.MotionScale * (1f / this.m_deltaTime)));
		this.m_fixedMotionScaleNorm = ((this.m_fixedDeltaTime < 1.401298E-45f) ? 0f : (this.MotionScale * (1f / this.m_fixedDeltaTime)));
		float scale = this.m_starting ? 0f : this.m_motionScaleNorm;
		float fixedScale = this.m_starting ? 0f : this.m_fixedMotionScaleNorm;
		AmplifyMotionEffectBase.DiscardContents(this.m_motionRT);
		this.m_updateCB.Clear();
		this.m_renderCB.Clear();
		this.m_renderCB.SetGlobalFloat("_AM_MIN_VELOCITY", this.MinVelocity);
		this.m_renderCB.SetGlobalFloat("_AM_MAX_VELOCITY", this.MaxVelocity);
		this.m_renderCB.SetGlobalFloat("_AM_RCP_TOTAL_VELOCITY", 1f / (this.MaxVelocity - this.MinVelocity));
		this.m_renderCB.SetGlobalVector("_AM_DEPTH_THRESHOLD", new Vector2(this.DepthThreshold, y));
		this.m_renderCB.SetRenderTarget(this.m_motionRT);
		this.m_renderCB.ClearRenderTarget(true, clearColor, Color.black);
		if (flag2)
		{
			float num = (this.m_deltaTime < 1.401298E-45f) ? 0f : (this.MotionScale * this.CameraMotionMult * (1f / this.m_deltaTime));
			float scale2 = this.m_starting ? 0f : num;
			this.RenderReprojectionVectors(this.m_renderCB, this.m_motionRT, scale2);
		}
		this.m_baseCamera.RenderVectors(this.m_renderCB, scale, fixedScale, this.QualityLevel);
		for (int i = 0; i < this.m_linkedCameraValues.Length; i++)
		{
			AmplifyMotionCamera amplifyMotionCamera = this.m_linkedCameraValues[i];
			if (amplifyMotionCamera != null && amplifyMotionCamera.Overlay && amplifyMotionCamera.isActiveAndEnabled)
			{
				this.m_linkedCameraValues[i].RenderVectors(this.m_renderCB, scale, fixedScale, this.QualityLevel);
			}
		}
		this.m_starting = false;
	}

	private void ApplyMotionBlur(RenderTexture source, RenderTexture destination, Vector4 blurStep)
	{
		bool flag = this.QualityLevel == Quality.Mobile;
		int qualityLevel = (int)this.QualityLevel;
		RenderTexture renderTexture = null;
		if (flag)
		{
			renderTexture = RenderTexture.GetTemporary(this.m_width, this.m_height, 0, RenderTextureFormat.ARGB32);
			renderTexture.name = "AM-DepthTemp";
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			renderTexture.filterMode = FilterMode.Point;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(this.m_width, this.m_height, 0, source.format);
		temporary.name = "AM-CombinedTemp";
		temporary.wrapMode = TextureWrapMode.Clamp;
		temporary.filterMode = FilterMode.Point;
		AmplifyMotionEffectBase.DiscardContents(temporary);
		this.m_combineMaterial.SetTexture("_MotionTex", this.m_motionRT);
		source.filterMode = FilterMode.Point;
		Graphics.Blit(source, temporary, this.m_combineMaterial, 0);
		this.m_blurMaterial.SetTexture("_MotionTex", this.m_motionRT);
		if (flag)
		{
			Graphics.Blit(null, renderTexture, this.m_depthMaterial, 0);
			this.m_blurMaterial.SetTexture("_DepthTex", renderTexture);
		}
		if (this.QualitySteps > 1)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(this.m_width, this.m_height, 0, source.format);
			temporary2.name = "AM-CombinedTemp2";
			temporary2.filterMode = FilterMode.Point;
			float num = 1f / (float)this.QualitySteps;
			float num2 = 1f;
			RenderTexture renderTexture2 = temporary;
			RenderTexture renderTexture3 = temporary2;
			for (int i = 0; i < this.QualitySteps; i++)
			{
				if (renderTexture3 != destination)
				{
					AmplifyMotionEffectBase.DiscardContents(renderTexture3);
				}
				this.m_blurMaterial.SetVector("_AM_BLUR_STEP", blurStep * num2);
				Graphics.Blit(renderTexture2, renderTexture3, this.m_blurMaterial, qualityLevel);
				if (i < this.QualitySteps - 2)
				{
					RenderTexture renderTexture4 = renderTexture3;
					renderTexture3 = renderTexture2;
					renderTexture2 = renderTexture4;
				}
				else
				{
					renderTexture2 = renderTexture3;
					renderTexture3 = destination;
				}
				num2 -= num;
			}
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			this.m_blurMaterial.SetVector("_AM_BLUR_STEP", blurStep);
			Graphics.Blit(temporary, destination, this.m_blurMaterial, qualityLevel);
		}
		if (flag)
		{
			this.m_combineMaterial.SetTexture("_MotionTex", this.m_motionRT);
			Graphics.Blit(source, destination, this.m_combineMaterial, 1);
		}
		RenderTexture.ReleaseTemporary(temporary);
		if (renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.m_currentPostProcess == null)
		{
			this.PostProcess(source, destination);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}

	public void PostProcess(RenderTexture source, RenderTexture destination)
	{
		Vector4 zero = Vector4.zero;
		zero.x = this.MaxVelocity / 1000f;
		zero.y = this.MaxVelocity / 1000f;
		RenderTexture renderTexture = null;
		if (QualitySettings.antiAliasing > 1)
		{
			renderTexture = RenderTexture.GetTemporary(this.m_width, this.m_height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			renderTexture.name = "AM-DilatedTemp";
			renderTexture.filterMode = FilterMode.Point;
			this.m_dilationMaterial.SetTexture("_MotionTex", this.m_motionRT);
			Graphics.Blit(this.m_motionRT, renderTexture, this.m_dilationMaterial, 0);
			this.m_dilationMaterial.SetTexture("_MotionTex", renderTexture);
			Graphics.Blit(renderTexture, this.m_motionRT, this.m_dilationMaterial, 1);
		}
		if (this.DebugMode)
		{
			this.m_debugMaterial.SetTexture("_MotionTex", this.m_motionRT);
			Graphics.Blit(source, destination, this.m_debugMaterial);
		}
		else
		{
			this.ApplyMotionBlur(source, destination, zero);
		}
		if (renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}
}
