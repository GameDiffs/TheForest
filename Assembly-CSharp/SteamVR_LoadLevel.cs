using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Valve.VR;

public class SteamVR_LoadLevel : MonoBehaviour
{
	private static SteamVR_LoadLevel _active;

	public string levelName;

	public bool loadExternalApp;

	public string externalAppPath;

	public string externalAppArgs;

	public bool loadAdditive;

	public bool loadAsync = true;

	public Texture loadingScreen;

	public Texture progressBarEmpty;

	public Texture progressBarFull;

	public float loadingScreenWidthInMeters = 6f;

	public float progressBarWidthInMeters = 3f;

	public float loadingScreenDistance;

	public Transform loadingScreenTransform;

	public Transform progressBarTransform;

	public Texture front;

	public Texture back;

	public Texture left;

	public Texture right;

	public Texture top;

	public Texture bottom;

	public Color backgroundColor = Color.black;

	public bool showGrid;

	public float fadeOutTime = 0.5f;

	public float fadeInTime = 0.5f;

	public float postLoadSettleTime;

	public float loadingScreenFadeInTime = 1f;

	public float loadingScreenFadeOutTime = 0.25f;

	private float fadeRate = 1f;

	private float alpha;

	private AsyncOperation async;

	private RenderTexture renderTexture;

	private ulong loadingScreenOverlayHandle;

	private ulong progressBarOverlayHandle;

	public bool autoTriggerOnEnable;

	public static bool loading
	{
		get
		{
			return SteamVR_LoadLevel._active != null;
		}
	}

	public static float progress
	{
		get
		{
			return (!(SteamVR_LoadLevel._active != null) || SteamVR_LoadLevel._active.async == null) ? 0f : SteamVR_LoadLevel._active.async.progress;
		}
	}

	public static Texture progressTexture
	{
		get
		{
			return (!(SteamVR_LoadLevel._active != null)) ? null : SteamVR_LoadLevel._active.renderTexture;
		}
	}

	private void OnEnable()
	{
		if (this.autoTriggerOnEnable)
		{
			this.Trigger();
		}
	}

	public void Trigger()
	{
		if (!SteamVR_LoadLevel.loading && !string.IsNullOrEmpty(this.levelName))
		{
			base.StartCoroutine("LoadLevel");
		}
	}

	public static void Begin(string levelName, bool showGrid = false, float fadeOutTime = 0.5f, float r = 0f, float g = 0f, float b = 0f, float a = 1f)
	{
		SteamVR_LoadLevel steamVR_LoadLevel = new GameObject("loader").AddComponent<SteamVR_LoadLevel>();
		steamVR_LoadLevel.levelName = levelName;
		steamVR_LoadLevel.showGrid = showGrid;
		steamVR_LoadLevel.fadeOutTime = fadeOutTime;
		steamVR_LoadLevel.backgroundColor = new Color(r, g, b, a);
		steamVR_LoadLevel.Trigger();
	}

	private void OnGUI()
	{
		if (SteamVR_LoadLevel._active != this)
		{
			return;
		}
		if (this.progressBarEmpty != null && this.progressBarFull != null)
		{
			if (this.progressBarOverlayHandle == 0uL)
			{
				this.progressBarOverlayHandle = this.GetOverlayHandle("progressBar", (!(this.progressBarTransform != null)) ? base.transform : this.progressBarTransform, this.progressBarWidthInMeters);
			}
			if (this.progressBarOverlayHandle != 0uL)
			{
				float num = (this.async == null) ? 0f : this.async.progress;
				int width = this.progressBarFull.width;
				int height = this.progressBarFull.height;
				if (this.renderTexture == null)
				{
					this.renderTexture = new RenderTexture(width, height, 0);
					this.renderTexture.Create();
				}
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = this.renderTexture;
				if (Event.current.type == EventType.Repaint)
				{
					GL.Clear(false, true, Color.clear);
				}
				GUILayout.BeginArea(new Rect(0f, 0f, (float)width, (float)height));
				GUI.DrawTexture(new Rect(0f, 0f, (float)width, (float)height), this.progressBarEmpty);
				GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, num * (float)width, (float)height), this.progressBarFull, new Rect(0f, 0f, num, 1f));
				GUILayout.EndArea();
				RenderTexture.active = active;
				CVROverlay overlay = OpenVR.Overlay;
				if (overlay != null)
				{
					Texture_t texture_t = default(Texture_t);
					texture_t.handle = this.renderTexture.GetNativeTexturePtr();
					texture_t.eType = SteamVR.instance.graphicsAPI;
					texture_t.eColorSpace = EColorSpace.Auto;
					overlay.SetOverlayTexture(this.progressBarOverlayHandle, ref texture_t);
				}
			}
		}
	}

	private void Update()
	{
		if (SteamVR_LoadLevel._active != this)
		{
			return;
		}
		this.alpha = Mathf.Clamp01(this.alpha + this.fadeRate * Time.deltaTime);
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay != null)
		{
			if (this.loadingScreenOverlayHandle != 0uL)
			{
				overlay.SetOverlayAlpha(this.loadingScreenOverlayHandle, this.alpha);
			}
			if (this.progressBarOverlayHandle != 0uL)
			{
				overlay.SetOverlayAlpha(this.progressBarOverlayHandle, this.alpha);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator LoadLevel()
	{
		SteamVR_LoadLevel.<LoadLevel>c__Iterator1D5 <LoadLevel>c__Iterator1D = new SteamVR_LoadLevel.<LoadLevel>c__Iterator1D5();
		<LoadLevel>c__Iterator1D.<>f__this = this;
		return <LoadLevel>c__Iterator1D;
	}

	private ulong GetOverlayHandle(string overlayName, Transform transform, float widthInMeters = 1f)
	{
		ulong num = 0uL;
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay == null)
		{
			return num;
		}
		string pchOverlayKey = SteamVR_Overlay.key + "." + overlayName;
		EVROverlayError eVROverlayError = overlay.FindOverlay(pchOverlayKey, ref num);
		if (eVROverlayError != EVROverlayError.None)
		{
			eVROverlayError = overlay.CreateOverlay(pchOverlayKey, overlayName, ref num);
		}
		if (eVROverlayError == EVROverlayError.None)
		{
			overlay.ShowOverlay(num);
			overlay.SetOverlayAlpha(num, this.alpha);
			overlay.SetOverlayWidthInMeters(num, widthInMeters);
			if (SteamVR.instance.graphicsAPI == EGraphicsAPIConvention.API_DirectX)
			{
				VRTextureBounds_t vRTextureBounds_t = default(VRTextureBounds_t);
				vRTextureBounds_t.uMin = 0f;
				vRTextureBounds_t.vMin = 1f;
				vRTextureBounds_t.uMax = 1f;
				vRTextureBounds_t.vMax = 0f;
				overlay.SetOverlayTextureBounds(num, ref vRTextureBounds_t);
			}
			SteamVR_Camera steamVR_Camera = (this.loadingScreenDistance != 0f) ? null : SteamVR_Render.Top();
			if (steamVR_Camera != null && steamVR_Camera.origin != null)
			{
				SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(steamVR_Camera.origin, transform);
				rigidTransform.pos.x = rigidTransform.pos.x / steamVR_Camera.origin.localScale.x;
				rigidTransform.pos.y = rigidTransform.pos.y / steamVR_Camera.origin.localScale.y;
				rigidTransform.pos.z = rigidTransform.pos.z / steamVR_Camera.origin.localScale.z;
				HmdMatrix34_t hmdMatrix34_t = rigidTransform.ToHmdMatrix34();
				overlay.SetOverlayTransformAbsolute(num, SteamVR_Render.instance.trackingSpace, ref hmdMatrix34_t);
			}
			else
			{
				SteamVR_Utils.RigidTransform rigidTransform2 = new SteamVR_Utils.RigidTransform(transform);
				HmdMatrix34_t hmdMatrix34_t2 = rigidTransform2.ToHmdMatrix34();
				overlay.SetOverlayTransformAbsolute(num, SteamVR_Render.instance.trackingSpace, ref hmdMatrix34_t2);
			}
		}
		return num;
	}
}
