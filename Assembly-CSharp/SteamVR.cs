using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Valve.VR;

public class SteamVR : IDisposable
{
	public class Unity
	{
		public const int k_nRenderEventID_WaitGetPoses = 201510020;

		public const int k_nRenderEventID_SubmitL = 201510021;

		public const int k_nRenderEventID_SubmitR = 201510022;

		public const int k_nRenderEventID_Flush = 201510023;

		public const int k_nRenderEventID_PostPresentHandoff = 201510024;

		[DllImport("openvr_api", EntryPoint = "UnityHooks_GetRenderEventFunc")]
		public static extern IntPtr GetRenderEventFunc();

		[DllImport("openvr_api", EntryPoint = "UnityHooks_SetSubmitParams")]
		public static extern void SetSubmitParams(VRTextureBounds_t boundsL, VRTextureBounds_t boundsR, EVRSubmitFlags nSubmitFlags);

		[DllImport("openvr_api", EntryPoint = "UnityHooks_SetColorSpace")]
		public static extern void SetColorSpace(EColorSpace eColorSpace);

		[DllImport("openvr_api", EntryPoint = "UnityHooks_EventWriteString")]
		public static extern void EventWriteString([MarshalAs(UnmanagedType.LPWStr)] [In] string sEvent);
	}

	private static bool _enabled = true;

	private static SteamVR _instance;

	public static bool[] connected = new bool[16];

	public EGraphicsAPIConvention graphicsAPI;

	public static bool active
	{
		get
		{
			return SteamVR._instance != null;
		}
	}

	public static bool enabled
	{
		get
		{
			return SteamVR._enabled;
		}
		set
		{
			SteamVR._enabled = value;
			if (!SteamVR._enabled)
			{
				SteamVR.SafeDispose();
			}
		}
	}

	public static SteamVR instance
	{
		get
		{
			if (!SteamVR.enabled)
			{
				return null;
			}
			if (SteamVR._instance == null)
			{
				SteamVR._instance = SteamVR.CreateInstance();
				if (SteamVR._instance == null)
				{
					SteamVR._enabled = false;
				}
			}
			return SteamVR._instance;
		}
	}

	public static bool usingNativeSupport
	{
		get
		{
			return false;
		}
	}

	public CVRSystem hmd
	{
		get;
		private set;
	}

	public CVRCompositor compositor
	{
		get;
		private set;
	}

	public CVROverlay overlay
	{
		get;
		private set;
	}

	public static bool initializing
	{
		get;
		private set;
	}

	public static bool calibrating
	{
		get;
		private set;
	}

	public static bool outOfRange
	{
		get;
		private set;
	}

	public float sceneWidth
	{
		get;
		private set;
	}

	public float sceneHeight
	{
		get;
		private set;
	}

	public float aspect
	{
		get;
		private set;
	}

	public float fieldOfView
	{
		get;
		private set;
	}

	public Vector2 tanHalfFov
	{
		get;
		private set;
	}

	public VRTextureBounds_t[] textureBounds
	{
		get;
		private set;
	}

	public SteamVR_Utils.RigidTransform[] eyes
	{
		get;
		private set;
	}

	public string hmd_TrackingSystemName
	{
		get
		{
			return this.GetStringProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String);
		}
	}

	public string hmd_ModelNumber
	{
		get
		{
			return this.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String);
		}
	}

	public string hmd_SerialNumber
	{
		get
		{
			return this.GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String);
		}
	}

	public float hmd_SecondsFromVsyncToPhotons
	{
		get
		{
			return this.GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float);
		}
	}

	public float hmd_DisplayFrequency
	{
		get
		{
			return this.GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float);
		}
	}

	private SteamVR()
	{
		this.hmd = OpenVR.System;
		Debug.Log("Connected to " + this.hmd_TrackingSystemName + ":" + this.hmd_SerialNumber);
		this.compositor = OpenVR.Compositor;
		this.overlay = OpenVR.Overlay;
		uint num = 0u;
		uint num2 = 0u;
		this.hmd.GetRecommendedRenderTargetSize(ref num, ref num2);
		this.sceneWidth = num;
		this.sceneHeight = num2;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		this.hmd.GetProjectionRaw(EVREye.Eye_Left, ref num3, ref num4, ref num5, ref num6);
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		this.hmd.GetProjectionRaw(EVREye.Eye_Right, ref num7, ref num8, ref num9, ref num10);
		this.tanHalfFov = new Vector2(Mathf.Max(new float[]
		{
			-num3,
			num4,
			-num7,
			num8
		}), Mathf.Max(new float[]
		{
			-num5,
			num6,
			-num9,
			num10
		}));
		this.textureBounds = new VRTextureBounds_t[2];
		this.textureBounds[0].uMin = 0.5f + 0.5f * num3 / this.tanHalfFov.x;
		this.textureBounds[0].uMax = 0.5f + 0.5f * num4 / this.tanHalfFov.x;
		this.textureBounds[0].vMin = 0.5f - 0.5f * num6 / this.tanHalfFov.y;
		this.textureBounds[0].vMax = 0.5f - 0.5f * num5 / this.tanHalfFov.y;
		this.textureBounds[1].uMin = 0.5f + 0.5f * num7 / this.tanHalfFov.x;
		this.textureBounds[1].uMax = 0.5f + 0.5f * num8 / this.tanHalfFov.x;
		this.textureBounds[1].vMin = 0.5f - 0.5f * num10 / this.tanHalfFov.y;
		this.textureBounds[1].vMax = 0.5f - 0.5f * num9 / this.tanHalfFov.y;
		SteamVR.Unity.SetSubmitParams(this.textureBounds[0], this.textureBounds[1], EVRSubmitFlags.Submit_Default);
		this.sceneWidth /= Mathf.Max(this.textureBounds[0].uMax - this.textureBounds[0].uMin, this.textureBounds[1].uMax - this.textureBounds[1].uMin);
		this.sceneHeight /= Mathf.Max(this.textureBounds[0].vMax - this.textureBounds[0].vMin, this.textureBounds[1].vMax - this.textureBounds[1].vMin);
		this.aspect = this.tanHalfFov.x / this.tanHalfFov.y;
		this.fieldOfView = 2f * Mathf.Atan(this.tanHalfFov.y) * 57.29578f;
		this.eyes = new SteamVR_Utils.RigidTransform[]
		{
			new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Left)),
			new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Right))
		};
		if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
		{
			this.graphicsAPI = EGraphicsAPIConvention.API_OpenGL;
		}
		else
		{
			this.graphicsAPI = EGraphicsAPIConvention.API_DirectX;
		}
		SteamVR_Utils.Event.Listen("initializing", new SteamVR_Utils.Event.Handler(this.OnInitializing));
		SteamVR_Utils.Event.Listen("calibrating", new SteamVR_Utils.Event.Handler(this.OnCalibrating));
		SteamVR_Utils.Event.Listen("out_of_range", new SteamVR_Utils.Event.Handler(this.OnOutOfRange));
		SteamVR_Utils.Event.Listen("device_connected", new SteamVR_Utils.Event.Handler(this.OnDeviceConnected));
		SteamVR_Utils.Event.Listen("new_poses", new SteamVR_Utils.Event.Handler(this.OnNewPoses));
	}

	private static SteamVR CreateInstance()
	{
		try
		{
			EVRInitError eVRInitError = EVRInitError.None;
			if (!SteamVR.usingNativeSupport)
			{
				OpenVR.Init(ref eVRInitError, EVRApplicationType.VRApplication_Scene);
				if (eVRInitError != EVRInitError.None)
				{
					SteamVR.ReportError(eVRInitError);
					SteamVR.ShutdownSystems();
					SteamVR result = null;
					return result;
				}
			}
			OpenVR.GetGenericInterface("IVRCompositor_013", ref eVRInitError);
			if (eVRInitError != EVRInitError.None)
			{
				SteamVR.ReportError(eVRInitError);
				SteamVR.ShutdownSystems();
				SteamVR result = null;
				return result;
			}
			OpenVR.GetGenericInterface("IVROverlay_010", ref eVRInitError);
			if (eVRInitError != EVRInitError.None)
			{
				SteamVR.ReportError(eVRInitError);
				SteamVR.ShutdownSystems();
				SteamVR result = null;
				return result;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			SteamVR result = null;
			return result;
		}
		return new SteamVR();
	}

	private static void ReportError(EVRInitError error)
	{
		if (error != EVRInitError.None)
		{
			if (error != EVRInitError.Init_VRClientDLLNotFound)
			{
				if (error != EVRInitError.Driver_RuntimeOutOfDate)
				{
					if (error != EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime)
					{
						Debug.Log(OpenVR.GetStringForHmdError(error));
					}
					else
					{
						Debug.Log("SteamVR Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
					}
				}
				else
				{
					Debug.Log("SteamVR Initialization Failed!  Make sure device's runtime is up to date.");
				}
			}
			else
			{
				Debug.Log("SteamVR drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
			}
		}
	}

	public string GetTrackedDeviceString(uint deviceId)
	{
		ETrackedPropertyError eTrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
		uint stringTrackedDeviceProperty = this.hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, null, 0u, ref eTrackedPropertyError);
		if (stringTrackedDeviceProperty > 1u)
		{
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			this.hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, stringBuilder, stringTrackedDeviceProperty, ref eTrackedPropertyError);
			return stringBuilder.ToString();
		}
		return null;
	}

	private string GetStringProperty(ETrackedDeviceProperty prop)
	{
		ETrackedPropertyError eTrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
		uint stringTrackedDeviceProperty = this.hmd.GetStringTrackedDeviceProperty(0u, prop, null, 0u, ref eTrackedPropertyError);
		if (stringTrackedDeviceProperty > 1u)
		{
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			this.hmd.GetStringTrackedDeviceProperty(0u, prop, stringBuilder, stringTrackedDeviceProperty, ref eTrackedPropertyError);
			return stringBuilder.ToString();
		}
		return (eTrackedPropertyError == ETrackedPropertyError.TrackedProp_Success) ? "<unknown>" : eTrackedPropertyError.ToString();
	}

	private float GetFloatProperty(ETrackedDeviceProperty prop)
	{
		ETrackedPropertyError eTrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
		return this.hmd.GetFloatTrackedDeviceProperty(0u, prop, ref eTrackedPropertyError);
	}

	private void OnInitializing(params object[] args)
	{
		SteamVR.initializing = (bool)args[0];
	}

	private void OnCalibrating(params object[] args)
	{
		SteamVR.calibrating = (bool)args[0];
	}

	private void OnOutOfRange(params object[] args)
	{
		SteamVR.outOfRange = (bool)args[0];
	}

	private void OnDeviceConnected(params object[] args)
	{
		int num = (int)args[0];
		SteamVR.connected[num] = (bool)args[1];
	}

	private void OnNewPoses(params object[] args)
	{
		TrackedDevicePose_t[] array = (TrackedDevicePose_t[])args[0];
		this.eyes[0] = new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Left));
		this.eyes[1] = new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Right));
		for (int i = 0; i < array.Length; i++)
		{
			bool bDeviceIsConnected = array[i].bDeviceIsConnected;
			if (bDeviceIsConnected != SteamVR.connected[i])
			{
				SteamVR_Utils.Event.Send("device_connected", new object[]
				{
					i,
					bDeviceIsConnected
				});
			}
		}
		if ((long)array.Length > 0L)
		{
			ETrackingResult eTrackingResult = array[(int)((UIntPtr)0)].eTrackingResult;
			bool flag = eTrackingResult == ETrackingResult.Uninitialized;
			if (flag != SteamVR.initializing)
			{
				SteamVR_Utils.Event.Send("initializing", new object[]
				{
					flag
				});
			}
			bool flag2 = eTrackingResult == ETrackingResult.Calibrating_InProgress || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			if (flag2 != SteamVR.calibrating)
			{
				SteamVR_Utils.Event.Send("calibrating", new object[]
				{
					flag2
				});
			}
			bool flag3 = eTrackingResult == ETrackingResult.Running_OutOfRange || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			if (flag3 != SteamVR.outOfRange)
			{
				SteamVR_Utils.Event.Send("out_of_range", new object[]
				{
					flag3
				});
			}
		}
	}

	~SteamVR()
	{
		this.Dispose(false);
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		SteamVR_Utils.Event.Remove("initializing", new SteamVR_Utils.Event.Handler(this.OnInitializing));
		SteamVR_Utils.Event.Remove("calibrating", new SteamVR_Utils.Event.Handler(this.OnCalibrating));
		SteamVR_Utils.Event.Remove("out_of_range", new SteamVR_Utils.Event.Handler(this.OnOutOfRange));
		SteamVR_Utils.Event.Remove("device_connected", new SteamVR_Utils.Event.Handler(this.OnDeviceConnected));
		SteamVR_Utils.Event.Remove("new_poses", new SteamVR_Utils.Event.Handler(this.OnNewPoses));
		SteamVR.ShutdownSystems();
		SteamVR._instance = null;
	}

	private static void ShutdownSystems()
	{
		OpenVR.Shutdown();
	}

	public static void SafeDispose()
	{
		if (SteamVR._instance != null)
		{
			SteamVR._instance.Dispose();
		}
	}
}
