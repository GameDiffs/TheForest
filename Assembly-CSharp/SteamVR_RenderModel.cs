using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class SteamVR_RenderModel : MonoBehaviour
{
	public class RenderModel
	{
		public Mesh mesh
		{
			get;
			private set;
		}

		public Material material
		{
			get;
			private set;
		}

		public RenderModel(Mesh mesh, Material material)
		{
			this.mesh = mesh;
			this.material = material;
		}
	}

	public sealed class RenderModelInterfaceHolder : IDisposable
	{
		private bool needsShutdown;

		private bool failedLoadInterface;

		private CVRRenderModels _instance;

		public CVRRenderModels instance
		{
			get
			{
				if (this._instance == null && !this.failedLoadInterface)
				{
					if (!SteamVR.active && !SteamVR.usingNativeSupport)
					{
						EVRInitError eVRInitError = EVRInitError.None;
						OpenVR.Init(ref eVRInitError, EVRApplicationType.VRApplication_Other);
						this.needsShutdown = true;
					}
					this._instance = OpenVR.RenderModels;
					if (this._instance == null)
					{
						UnityEngine.Debug.LogError("Failed to load IVRRenderModels interface version IVRRenderModels_005");
						this.failedLoadInterface = true;
					}
				}
				return this._instance;
			}
		}

		public void Dispose()
		{
			if (this.needsShutdown)
			{
				OpenVR.Shutdown();
			}
		}
	}

	public const string k_localTransformName = "attach";

	public SteamVR_TrackedObject.EIndex index = SteamVR_TrackedObject.EIndex.None;

	public string modelOverride;

	public Shader shader;

	public bool verbose;

	public bool createComponents = true;

	public bool updateDynamically = true;

	public static Hashtable models = new Hashtable();

	public static Hashtable materials = new Hashtable();

	public string renderModelName
	{
		get;
		private set;
	}

	private void OnHideRenderModels(params object[] args)
	{
		bool flag = (bool)args[0];
		MeshRenderer component = base.GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.enabled = !flag;
		}
		MeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshRenderer meshRenderer = componentsInChildren[i];
			meshRenderer.enabled = !flag;
		}
	}

	private void OnDeviceConnected(params object[] args)
	{
		int num = (int)args[0];
		if (num != (int)this.index)
		{
			return;
		}
		bool flag = (bool)args[1];
		if (flag)
		{
			this.UpdateModel();
		}
		else
		{
			this.StripMesh(base.gameObject);
		}
	}

	public void UpdateModel()
	{
		CVRSystem system = OpenVR.System;
		if (system == null)
		{
			return;
		}
		ETrackedPropertyError eTrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
		uint stringTrackedDeviceProperty = system.GetStringTrackedDeviceProperty((uint)this.index, ETrackedDeviceProperty.Prop_RenderModelName_String, null, 0u, ref eTrackedPropertyError);
		if (stringTrackedDeviceProperty <= 1u)
		{
			UnityEngine.Debug.LogError("Failed to get render model name for tracked object " + this.index);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
		system.GetStringTrackedDeviceProperty((uint)this.index, ETrackedDeviceProperty.Prop_RenderModelName_String, stringBuilder, stringTrackedDeviceProperty, ref eTrackedPropertyError);
		string text = stringBuilder.ToString();
		if (this.renderModelName != text)
		{
			this.renderModelName = text;
			base.StartCoroutine(this.SetModelAsync(text));
		}
	}

	[DebuggerHidden]
	private IEnumerator SetModelAsync(string renderModelName)
	{
		SteamVR_RenderModel.<SetModelAsync>c__Iterator1E0 <SetModelAsync>c__Iterator1E = new SteamVR_RenderModel.<SetModelAsync>c__Iterator1E0();
		<SetModelAsync>c__Iterator1E.renderModelName = renderModelName;
		<SetModelAsync>c__Iterator1E.<$>renderModelName = renderModelName;
		<SetModelAsync>c__Iterator1E.<>f__this = this;
		return <SetModelAsync>c__Iterator1E;
	}

	private bool SetModel(string renderModelName)
	{
		this.StripMesh(base.gameObject);
		using (SteamVR_RenderModel.RenderModelInterfaceHolder renderModelInterfaceHolder = new SteamVR_RenderModel.RenderModelInterfaceHolder())
		{
			if (this.createComponents)
			{
				if (this.LoadComponents(renderModelInterfaceHolder, renderModelName))
				{
					this.UpdateComponents();
					bool result = true;
					return result;
				}
				UnityEngine.Debug.Log("[" + base.gameObject.name + "] Render model does not support components, falling back to single mesh.");
			}
			if (!string.IsNullOrEmpty(renderModelName))
			{
				SteamVR_RenderModel.RenderModel renderModel = SteamVR_RenderModel.models[renderModelName] as SteamVR_RenderModel.RenderModel;
				bool result;
				if (renderModel == null || renderModel.mesh == null)
				{
					CVRRenderModels instance = renderModelInterfaceHolder.instance;
					if (instance == null)
					{
						result = false;
						return result;
					}
					if (this.verbose)
					{
						UnityEngine.Debug.Log("Loading render model " + renderModelName);
					}
					renderModel = this.LoadRenderModel(instance, renderModelName, renderModelName);
					if (renderModel == null)
					{
						result = false;
						return result;
					}
					SteamVR_RenderModel.models[renderModelName] = renderModel;
				}
				base.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
				base.gameObject.AddComponent<MeshRenderer>().sharedMaterial = renderModel.material;
				result = true;
				return result;
			}
		}
		return false;
	}

	private SteamVR_RenderModel.RenderModel LoadRenderModel(CVRRenderModels renderModels, string renderModelName, string baseName)
	{
		IntPtr zero = IntPtr.Zero;
		EVRRenderModelError eVRRenderModelError;
		while (true)
		{
			eVRRenderModelError = renderModels.LoadRenderModel_Async(renderModelName, ref zero);
			if (eVRRenderModelError != EVRRenderModelError.Loading)
			{
				break;
			}
			Thread.Sleep(1);
		}
		if (eVRRenderModelError != EVRRenderModelError.None)
		{
			UnityEngine.Debug.LogError(string.Format("Failed to load render model {0} - {1}", renderModelName, eVRRenderModelError.ToString()));
			return null;
		}
		RenderModel_t renderModel_t = (RenderModel_t)Marshal.PtrToStructure(zero, typeof(RenderModel_t));
		Vector3[] array = new Vector3[renderModel_t.unVertexCount];
		Vector3[] array2 = new Vector3[renderModel_t.unVertexCount];
		Vector2[] array3 = new Vector2[renderModel_t.unVertexCount];
		Type typeFromHandle = typeof(RenderModel_Vertex_t);
		int num = 0;
		while ((long)num < (long)((ulong)renderModel_t.unVertexCount))
		{
			IntPtr ptr = new IntPtr(renderModel_t.rVertexData.ToInt64() + (long)(num * Marshal.SizeOf(typeFromHandle)));
			RenderModel_Vertex_t renderModel_Vertex_t = (RenderModel_Vertex_t)Marshal.PtrToStructure(ptr, typeFromHandle);
			array[num] = new Vector3(renderModel_Vertex_t.vPosition.v0, renderModel_Vertex_t.vPosition.v1, -renderModel_Vertex_t.vPosition.v2);
			array2[num] = new Vector3(renderModel_Vertex_t.vNormal.v0, renderModel_Vertex_t.vNormal.v1, -renderModel_Vertex_t.vNormal.v2);
			array3[num] = new Vector2(renderModel_Vertex_t.rfTextureCoord0, renderModel_Vertex_t.rfTextureCoord1);
			num++;
		}
		int num2 = (int)(renderModel_t.unTriangleCount * 3u);
		short[] array4 = new short[num2];
		Marshal.Copy(renderModel_t.rIndexData, array4, 0, array4.Length);
		int[] array5 = new int[num2];
		int num3 = 0;
		while ((long)num3 < (long)((ulong)renderModel_t.unTriangleCount))
		{
			array5[num3 * 3] = (int)array4[num3 * 3 + 2];
			array5[num3 * 3 + 1] = (int)array4[num3 * 3 + 1];
			array5[num3 * 3 + 2] = (int)array4[num3 * 3];
			num3++;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.triangles = array5;
		mesh.Optimize();
		Material material = SteamVR_RenderModel.materials[renderModel_t.diffuseTextureId] as Material;
		if (material == null || material.mainTexture == null)
		{
			IntPtr zero2 = IntPtr.Zero;
			while (true)
			{
				eVRRenderModelError = renderModels.LoadTexture_Async(renderModel_t.diffuseTextureId, ref zero2);
				if (eVRRenderModelError != EVRRenderModelError.Loading)
				{
					break;
				}
				Thread.Sleep(1);
			}
			if (eVRRenderModelError == EVRRenderModelError.None)
			{
				RenderModel_TextureMap_t renderModel_TextureMap_t = (RenderModel_TextureMap_t)Marshal.PtrToStructure(zero2, typeof(RenderModel_TextureMap_t));
				Texture2D texture2D = new Texture2D((int)renderModel_TextureMap_t.unWidth, (int)renderModel_TextureMap_t.unHeight, TextureFormat.ARGB32, false);
				if (SteamVR.instance.graphicsAPI == EGraphicsAPIConvention.API_DirectX)
				{
					texture2D.Apply();
					while (true)
					{
						eVRRenderModelError = renderModels.LoadIntoTextureD3D11_Async(renderModel_t.diffuseTextureId, texture2D.GetNativeTexturePtr());
						if (eVRRenderModelError != EVRRenderModelError.Loading)
						{
							break;
						}
						Thread.Sleep(1);
					}
				}
				else
				{
					byte[] array6 = new byte[(int)(renderModel_TextureMap_t.unWidth * renderModel_TextureMap_t.unHeight * '\u0004')];
					Marshal.Copy(renderModel_TextureMap_t.rubTextureMapData, array6, 0, array6.Length);
					Color32[] array7 = new Color32[(int)(renderModel_TextureMap_t.unWidth * renderModel_TextureMap_t.unHeight)];
					int num4 = 0;
					for (int i = 0; i < (int)renderModel_TextureMap_t.unHeight; i++)
					{
						for (int j = 0; j < (int)renderModel_TextureMap_t.unWidth; j++)
						{
							byte r = array6[num4++];
							byte g = array6[num4++];
							byte b = array6[num4++];
							byte a = array6[num4++];
							array7[i * (int)renderModel_TextureMap_t.unWidth + j] = new Color32(r, g, b, a);
						}
					}
					texture2D.SetPixels32(array7);
					texture2D.Apply();
				}
				material = new Material((!(this.shader != null)) ? Shader.Find("Standard") : this.shader);
				material.mainTexture = texture2D;
				SteamVR_RenderModel.materials[renderModel_t.diffuseTextureId] = material;
				renderModels.FreeTexture(zero2);
			}
			else
			{
				UnityEngine.Debug.Log("Failed to load render model texture for render model " + renderModelName);
			}
		}
		base.StartCoroutine(this.FreeRenderModel(zero));
		return new SteamVR_RenderModel.RenderModel(mesh, material);
	}

	[DebuggerHidden]
	private IEnumerator FreeRenderModel(IntPtr pRenderModel)
	{
		SteamVR_RenderModel.<FreeRenderModel>c__Iterator1E1 <FreeRenderModel>c__Iterator1E = new SteamVR_RenderModel.<FreeRenderModel>c__Iterator1E1();
		<FreeRenderModel>c__Iterator1E.pRenderModel = pRenderModel;
		<FreeRenderModel>c__Iterator1E.<$>pRenderModel = pRenderModel;
		return <FreeRenderModel>c__Iterator1E;
	}

	public Transform FindComponent(string componentName)
	{
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.name == componentName)
			{
				return child;
			}
		}
		return null;
	}

	private void StripMesh(GameObject go)
	{
		MeshRenderer component = go.GetComponent<MeshRenderer>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		MeshFilter component2 = go.GetComponent<MeshFilter>();
		if (component2 != null)
		{
			UnityEngine.Object.DestroyImmediate(component2);
		}
	}

	private bool LoadComponents(SteamVR_RenderModel.RenderModelInterfaceHolder holder, string renderModelName)
	{
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			child.gameObject.SetActive(false);
			this.StripMesh(child.gameObject);
		}
		if (string.IsNullOrEmpty(renderModelName))
		{
			return true;
		}
		CVRRenderModels instance = holder.instance;
		if (instance == null)
		{
			return false;
		}
		uint componentCount = instance.GetComponentCount(renderModelName);
		if (componentCount == 0u)
		{
			return false;
		}
		int num = 0;
		while ((long)num < (long)((ulong)componentCount))
		{
			uint num2 = instance.GetComponentName(renderModelName, (uint)num, null, 0u);
			if (num2 != 0u)
			{
				StringBuilder stringBuilder = new StringBuilder((int)num2);
				if (instance.GetComponentName(renderModelName, (uint)num, stringBuilder, num2) != 0u)
				{
					transform = this.FindComponent(stringBuilder.ToString());
					if (transform != null)
					{
						transform.gameObject.SetActive(true);
					}
					else
					{
						transform = new GameObject(stringBuilder.ToString()).transform;
						transform.parent = base.transform;
						transform.gameObject.layer = base.gameObject.layer;
						Transform transform2 = new GameObject("attach").transform;
						transform2.parent = transform;
						transform2.localPosition = Vector3.zero;
						transform2.localRotation = Quaternion.identity;
						transform2.localScale = Vector3.one;
						transform2.gameObject.layer = base.gameObject.layer;
					}
					transform.localPosition = Vector3.zero;
					transform.localRotation = Quaternion.identity;
					transform.localScale = Vector3.one;
					num2 = instance.GetComponentRenderModelName(renderModelName, stringBuilder.ToString(), null, 0u);
					if (num2 != 0u)
					{
						StringBuilder stringBuilder2 = new StringBuilder((int)num2);
						if (instance.GetComponentRenderModelName(renderModelName, stringBuilder.ToString(), stringBuilder2, num2) != 0u)
						{
							SteamVR_RenderModel.RenderModel renderModel = SteamVR_RenderModel.models[stringBuilder2] as SteamVR_RenderModel.RenderModel;
							if (renderModel == null || renderModel.mesh == null)
							{
								if (this.verbose)
								{
									UnityEngine.Debug.Log("Loading render model " + stringBuilder2);
								}
								renderModel = this.LoadRenderModel(instance, stringBuilder2.ToString(), renderModelName);
								if (renderModel == null)
								{
									goto IL_265;
								}
								SteamVR_RenderModel.models[stringBuilder2] = renderModel;
							}
							transform.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
							transform.gameObject.AddComponent<MeshRenderer>().sharedMaterial = renderModel.material;
						}
					}
				}
			}
			IL_265:
			num++;
		}
		return true;
	}

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(this.modelOverride))
		{
			UnityEngine.Debug.Log("Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.");
			base.enabled = false;
			return;
		}
		CVRSystem system = OpenVR.System;
		if (system != null && system.IsTrackedDeviceConnected((uint)this.index))
		{
			this.UpdateModel();
		}
		SteamVR_Utils.Event.Listen("device_connected", new SteamVR_Utils.Event.Handler(this.OnDeviceConnected));
		SteamVR_Utils.Event.Listen("hide_render_models", new SteamVR_Utils.Event.Handler(this.OnHideRenderModels));
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("device_connected", new SteamVR_Utils.Event.Handler(this.OnDeviceConnected));
		SteamVR_Utils.Event.Remove("hide_render_models", new SteamVR_Utils.Event.Handler(this.OnHideRenderModels));
	}

	private void Update()
	{
		if (this.updateDynamically)
		{
			this.UpdateComponents();
		}
	}

	public void UpdateComponents()
	{
		Transform transform = base.transform;
		if (transform.childCount == 0)
		{
			return;
		}
		using (SteamVR_RenderModel.RenderModelInterfaceHolder renderModelInterfaceHolder = new SteamVR_RenderModel.RenderModelInterfaceHolder())
		{
			VRControllerState_t vRControllerState_t = (this.index == SteamVR_TrackedObject.EIndex.None) ? default(VRControllerState_t) : SteamVR_Controller.Input((int)this.index).GetState();
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				CVRRenderModels instance = renderModelInterfaceHolder.instance;
				if (instance == null)
				{
					break;
				}
				RenderModel_ComponentState_t renderModel_ComponentState_t = default(RenderModel_ComponentState_t);
				RenderModel_ControllerMode_State_t renderModel_ControllerMode_State_t = default(RenderModel_ControllerMode_State_t);
				if (instance.GetComponentState(this.renderModelName, child.name, ref vRControllerState_t, ref renderModel_ControllerMode_State_t, ref renderModel_ComponentState_t))
				{
					SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(renderModel_ComponentState_t.mTrackingToComponentRenderModel);
					child.localPosition = rigidTransform.pos;
					child.localRotation = rigidTransform.rot;
					Transform transform2 = child.FindChild("attach");
					if (transform2 != null)
					{
						SteamVR_Utils.RigidTransform rigidTransform2 = new SteamVR_Utils.RigidTransform(renderModel_ComponentState_t.mTrackingToComponentLocal);
						transform2.position = transform.TransformPoint(rigidTransform2.pos);
						transform2.rotation = transform.rotation * rigidTransform2.rot;
					}
					bool flag = (renderModel_ComponentState_t.uProperties & 2u) != 0u;
					if (flag != child.gameObject.activeSelf)
					{
						child.gameObject.SetActive(flag);
					}
				}
			}
		}
	}

	public void SetDeviceIndex(int index)
	{
		this.index = (SteamVR_TrackedObject.EIndex)index;
		this.modelOverride = string.Empty;
		if (base.enabled)
		{
			this.UpdateModel();
		}
	}
}
