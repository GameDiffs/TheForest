using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

public class SteamVR_Render : MonoBehaviour
{
	public float helpSeconds = 10f;

	public string helpText = "You may now put on your headset.";

	public GUIStyle helpStyle;

	public bool pauseGameWhenDashboardIsVisible = true;

	public bool lockPhysicsUpdateRateToRenderFrequency = true;

	public SteamVR_ExternalCamera externalCamera;

	public string externalCameraConfigPath = "externalcamera.cfg";

	public LayerMask leftMask;

	public LayerMask rightMask;

	private SteamVR_CameraMask cameraMask;

	public ETrackingUniverseOrigin trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;

	private static SteamVR_Render _instance;

	private static bool isQuitting;

	private SteamVR_Camera[] cameras = new SteamVR_Camera[0];

	public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[16];

	public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];

	public static bool pauseRendering;

	private float sceneResolutionScale = 1f;

	private float timeScale = 1f;

	public static EVREye eye
	{
		get;
		private set;
	}

	public static SteamVR_Render instance
	{
		get
		{
			if (SteamVR_Render._instance == null)
			{
				SteamVR_Render._instance = UnityEngine.Object.FindObjectOfType<SteamVR_Render>();
				if (SteamVR_Render._instance == null)
				{
					SteamVR_Render._instance = new GameObject("[SteamVR]").AddComponent<SteamVR_Render>();
				}
			}
			return SteamVR_Render._instance;
		}
	}

	private void OnDestroy()
	{
		SteamVR_Render._instance = null;
	}

	private void OnApplicationQuit()
	{
		SteamVR_Render.isQuitting = true;
		SteamVR.SafeDispose();
	}

	public static void Add(SteamVR_Camera vrcam)
	{
		if (!SteamVR_Render.isQuitting)
		{
			SteamVR_Render.instance.AddInternal(vrcam);
		}
	}

	public static void Remove(SteamVR_Camera vrcam)
	{
		if (!SteamVR_Render.isQuitting && SteamVR_Render._instance != null)
		{
			SteamVR_Render.instance.RemoveInternal(vrcam);
		}
	}

	public static SteamVR_Camera Top()
	{
		if (!SteamVR_Render.isQuitting)
		{
			return SteamVR_Render.instance.TopInternal();
		}
		return null;
	}

	private void AddInternal(SteamVR_Camera vrcam)
	{
		Camera component = vrcam.GetComponent<Camera>();
		int num = this.cameras.Length;
		SteamVR_Camera[] array = new SteamVR_Camera[num + 1];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			Camera component2 = this.cameras[i].GetComponent<Camera>();
			if (i == num2 && component2.depth > component.depth)
			{
				array[num2++] = vrcam;
			}
			array[num2++] = this.cameras[i];
		}
		if (num2 == num)
		{
			array[num2] = vrcam;
		}
		this.cameras = array;
	}

	private void RemoveInternal(SteamVR_Camera vrcam)
	{
		int num = this.cameras.Length;
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			SteamVR_Camera x = this.cameras[i];
			if (x == vrcam)
			{
				num2++;
			}
		}
		if (num2 == 0)
		{
			return;
		}
		SteamVR_Camera[] array = new SteamVR_Camera[num - num2];
		int num3 = 0;
		for (int j = 0; j < num; j++)
		{
			SteamVR_Camera steamVR_Camera = this.cameras[j];
			if (steamVR_Camera != vrcam)
			{
				array[num3++] = steamVR_Camera;
			}
		}
		this.cameras = array;
	}

	private SteamVR_Camera TopInternal()
	{
		if (this.cameras.Length > 0)
		{
			return this.cameras[this.cameras.Length - 1];
		}
		return null;
	}

	[DebuggerHidden]
	private IEnumerator RenderLoop()
	{
		SteamVR_Render.<RenderLoop>c__Iterator1DF <RenderLoop>c__Iterator1DF = new SteamVR_Render.<RenderLoop>c__Iterator1DF();
		<RenderLoop>c__Iterator1DF.<>f__this = this;
		return <RenderLoop>c__Iterator1DF;
	}

	private void RenderEye(SteamVR vr, EVREye eye)
	{
		SteamVR_Render.eye = eye;
		if (this.cameraMask != null)
		{
			this.cameraMask.Set(vr, eye);
		}
		SteamVR_Camera[] array = this.cameras;
		for (int i = 0; i < array.Length; i++)
		{
			SteamVR_Camera steamVR_Camera = array[i];
			steamVR_Camera.transform.localPosition = vr.eyes[(int)eye].pos;
			steamVR_Camera.transform.localRotation = vr.eyes[(int)eye].rot;
			this.cameraMask.transform.position = steamVR_Camera.transform.position;
			Camera component = steamVR_Camera.GetComponent<Camera>();
			component.targetTexture = SteamVR_Camera.GetSceneTexture(component.hdr);
			int cullingMask = component.cullingMask;
			if (eye == EVREye.Eye_Left)
			{
				component.cullingMask &= ~this.rightMask;
				component.cullingMask |= this.leftMask;
			}
			else
			{
				component.cullingMask &= ~this.leftMask;
				component.cullingMask |= this.rightMask;
			}
			component.Render();
			component.cullingMask = cullingMask;
		}
	}

	private void RenderExternalCamera()
	{
		if (this.externalCamera == null)
		{
			return;
		}
		if (!this.externalCamera.gameObject.activeInHierarchy)
		{
			return;
		}
		int num = (int)Mathf.Max(this.externalCamera.config.frameSkip, 0f);
		if (Time.frameCount % (num + 1) != 0)
		{
			return;
		}
		this.externalCamera.AttachToCamera(this.TopInternal());
		this.externalCamera.RenderNear();
		this.externalCamera.RenderFar();
	}

	private void OnInputFocus(params object[] args)
	{
		bool flag = (bool)args[0];
		if (flag)
		{
			if (this.pauseGameWhenDashboardIsVisible)
			{
				Time.timeScale = this.timeScale;
			}
			SteamVR_Camera.sceneResolutionScale = this.sceneResolutionScale;
		}
		else
		{
			if (this.pauseGameWhenDashboardIsVisible)
			{
				this.timeScale = Time.timeScale;
				Time.timeScale = 0f;
			}
			this.sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
			SteamVR_Camera.sceneResolutionScale = 0.5f;
		}
	}

	private void OnQuit(params object[] args)
	{
		Application.Quit();
	}

	private void OnEnable()
	{
		base.StartCoroutine("RenderLoop");
		SteamVR_Utils.Event.Listen("input_focus", new SteamVR_Utils.Event.Handler(this.OnInputFocus));
		SteamVR_Utils.Event.Listen("Quit", new SteamVR_Utils.Event.Handler(this.OnQuit));
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
		SteamVR_Utils.Event.Remove("input_focus", new SteamVR_Utils.Event.Handler(this.OnInputFocus));
		SteamVR_Utils.Event.Remove("Quit", new SteamVR_Utils.Event.Handler(this.OnQuit));
	}

	private void Awake()
	{
		this.cameraMask = new GameObject("cameraMask")
		{
			transform = 
			{
				parent = base.transform
			}
		}.AddComponent<SteamVR_CameraMask>();
		if (this.externalCamera == null && File.Exists(this.externalCameraConfigPath))
		{
			GameObject original = Resources.Load<GameObject>("SteamVR_ExternalCamera");
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
			gameObject.gameObject.name = "External Camera";
			this.externalCamera = gameObject.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
			this.externalCamera.configPath = this.externalCameraConfigPath;
			this.externalCamera.ReadConfig();
		}
	}

	private void FixedUpdate()
	{
		SteamVR_Utils.QueueEventOnRenderThread(201510024);
	}

	private void Update()
	{
		if (this.cameras.Length == 0)
		{
			base.enabled = false;
			return;
		}
		SteamVR_Utils.QueueEventOnRenderThread(201510024);
		SteamVR_Controller.Update();
		CVRSystem system = OpenVR.System;
		if (system != null)
		{
			VREvent_t vREvent_t = default(VREvent_t);
			uint uncbVREvent = (uint)Marshal.SizeOf(typeof(VREvent_t));
			for (int i = 0; i < 64; i++)
			{
				if (!system.PollNextEvent(ref vREvent_t, uncbVREvent))
				{
					break;
				}
				EVREventType eventType = (EVREventType)vREvent_t.eventType;
				if (eventType != EVREventType.VREvent_InputFocusCaptured)
				{
					if (eventType != EVREventType.VREvent_InputFocusReleased)
					{
						if (eventType != EVREventType.VREvent_HideRenderModels)
						{
							if (eventType != EVREventType.VREvent_ShowRenderModels)
							{
								string name = Enum.GetName(typeof(EVREventType), vREvent_t.eventType);
								if (name != null)
								{
									SteamVR_Utils.Event.Send(name.Substring(8), new object[]
									{
										vREvent_t
									});
								}
							}
							else
							{
								SteamVR_Utils.Event.Send("hide_render_models", new object[]
								{
									false
								});
							}
						}
						else
						{
							SteamVR_Utils.Event.Send("hide_render_models", new object[]
							{
								true
							});
						}
					}
					else
					{
						SteamVR_Utils.Event.Send("input_focus", new object[]
						{
							true
						});
					}
				}
				else
				{
					SteamVR_Utils.Event.Send("input_focus", new object[]
					{
						false
					});
				}
			}
		}
		Application.targetFrameRate = -1;
		Application.runInBackground = true;
		QualitySettings.maxQueuedFrames = -1;
		QualitySettings.vSyncCount = 0;
		if (this.lockPhysicsUpdateRateToRenderFrequency)
		{
			SteamVR instance = SteamVR.instance;
			if (instance != null)
			{
				Compositor_FrameTiming compositor_FrameTiming = default(Compositor_FrameTiming);
				compositor_FrameTiming.m_nSize = (uint)Marshal.SizeOf(typeof(Compositor_FrameTiming));
				instance.compositor.GetFrameTiming(ref compositor_FrameTiming, 0u);
				Time.fixedDeltaTime = Time.timeScale * compositor_FrameTiming.m_nNumFramePresents / SteamVR.instance.hmd_DisplayFrequency;
			}
		}
	}

	private void OnGUI()
	{
		float timeSinceLevelLoad = Time.timeSinceLevelLoad;
		if (timeSinceLevelLoad < this.helpSeconds)
		{
			if (this.helpStyle == null)
			{
				this.helpStyle = new GUIStyle(GUI.skin.label);
				this.helpStyle.fontSize = 32;
			}
			if (timeSinceLevelLoad > this.helpSeconds - 1f)
			{
				Color textColor = this.helpStyle.normal.textColor;
				textColor.a = this.helpSeconds - timeSinceLevelLoad;
				this.helpStyle.normal.textColor = textColor;
			}
			GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(this.helpText, this.helpStyle, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

	public static void ShowHelpText(string text, float seconds)
	{
		if (SteamVR_Render._instance != null)
		{
			SteamVR_Render._instance.helpText = text;
			SteamVR_Render._instance.helpSeconds = Time.timeSinceLevelLoad + seconds;
		}
	}
}
