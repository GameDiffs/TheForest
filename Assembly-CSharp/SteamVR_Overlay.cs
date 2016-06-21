using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

public class SteamVR_Overlay : MonoBehaviour
{
	public struct IntersectionResults
	{
		public Vector3 point;

		public Vector3 normal;

		public Vector2 UVs;

		public float distance;
	}

	public Texture texture;

	public bool curved = true;

	public bool antialias = true;

	public bool highquality = true;

	public float scale = 3f;

	public float distance = 1.25f;

	public float alpha = 1f;

	public Vector4 uvOffset = new Vector4(0f, 0f, 1f, 1f);

	public Vector2 mouseScale = new Vector2(1f, 1f);

	public Vector2 curvedRange = new Vector2(1f, 2f);

	public VROverlayInputMethod inputMethod;

	private ulong handle;

	public static SteamVR_Overlay instance
	{
		get;
		private set;
	}

	public static string key
	{
		get
		{
			return "unity:" + Application.companyName + "." + Application.productName;
		}
	}

	private void OnEnable()
	{
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay != null)
		{
			EVROverlayError eVROverlayError = overlay.CreateOverlay(SteamVR_Overlay.key, base.gameObject.name, ref this.handle);
			if (eVROverlayError != EVROverlayError.None)
			{
				Debug.Log(overlay.GetOverlayErrorNameFromEnum(eVROverlayError));
				base.enabled = false;
				return;
			}
		}
		SteamVR_Overlay.instance = this;
	}

	private void OnDisable()
	{
		if (this.handle != 0uL)
		{
			CVROverlay overlay = OpenVR.Overlay;
			if (overlay != null)
			{
				overlay.DestroyOverlay(this.handle);
			}
			this.handle = 0uL;
		}
		SteamVR_Overlay.instance = null;
	}

	public void UpdateOverlay()
	{
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay == null)
		{
			return;
		}
		if (this.texture != null)
		{
			EVROverlayError eVROverlayError = overlay.ShowOverlay(this.handle);
			if ((eVROverlayError == EVROverlayError.InvalidHandle || eVROverlayError == EVROverlayError.UnknownOverlay) && overlay.FindOverlay(SteamVR_Overlay.key, ref this.handle) != EVROverlayError.None)
			{
				return;
			}
			Texture_t texture_t = default(Texture_t);
			texture_t.handle = this.texture.GetNativeTexturePtr();
			texture_t.eType = SteamVR.instance.graphicsAPI;
			texture_t.eColorSpace = EColorSpace.Auto;
			overlay.SetOverlayTexture(this.handle, ref texture_t);
			overlay.SetOverlayAlpha(this.handle, this.alpha);
			overlay.SetOverlayWidthInMeters(this.handle, this.scale);
			overlay.SetOverlayAutoCurveDistanceRangeInMeters(this.handle, this.curvedRange.x, this.curvedRange.y);
			VRTextureBounds_t vRTextureBounds_t = default(VRTextureBounds_t);
			vRTextureBounds_t.uMin = (0f + this.uvOffset.x) * this.uvOffset.z;
			vRTextureBounds_t.vMin = (1f + this.uvOffset.y) * this.uvOffset.w;
			vRTextureBounds_t.uMax = (1f + this.uvOffset.x) * this.uvOffset.z;
			vRTextureBounds_t.vMax = (0f + this.uvOffset.y) * this.uvOffset.w;
			overlay.SetOverlayTextureBounds(this.handle, ref vRTextureBounds_t);
			HmdVector2_t hmdVector2_t = default(HmdVector2_t);
			hmdVector2_t.v0 = this.mouseScale.x;
			hmdVector2_t.v1 = this.mouseScale.y;
			overlay.SetOverlayMouseScale(this.handle, ref hmdVector2_t);
			SteamVR_Camera steamVR_Camera = SteamVR_Render.Top();
			if (steamVR_Camera != null && steamVR_Camera.origin != null)
			{
				SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(steamVR_Camera.origin, base.transform);
				rigidTransform.pos.x = rigidTransform.pos.x / steamVR_Camera.origin.localScale.x;
				rigidTransform.pos.y = rigidTransform.pos.y / steamVR_Camera.origin.localScale.y;
				rigidTransform.pos.z = rigidTransform.pos.z / steamVR_Camera.origin.localScale.z;
				rigidTransform.pos.z = rigidTransform.pos.z + this.distance;
				HmdMatrix34_t hmdMatrix34_t = rigidTransform.ToHmdMatrix34();
				overlay.SetOverlayTransformAbsolute(this.handle, SteamVR_Render.instance.trackingSpace, ref hmdMatrix34_t);
			}
			overlay.SetOverlayInputMethod(this.handle, this.inputMethod);
			if (this.curved || this.antialias)
			{
				this.highquality = true;
			}
			if (this.highquality)
			{
				overlay.SetHighQualityOverlay(this.handle);
				overlay.SetOverlayFlag(this.handle, VROverlayFlags.Curved, this.curved);
				overlay.SetOverlayFlag(this.handle, VROverlayFlags.RGSS4X, this.antialias);
			}
			else if (overlay.GetHighQualityOverlay() == this.handle)
			{
				overlay.SetHighQualityOverlay(0uL);
			}
		}
		else
		{
			overlay.HideOverlay(this.handle);
		}
	}

	public bool PollNextEvent(ref VREvent_t pEvent)
	{
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay == null)
		{
			return false;
		}
		uint uncbVREvent = (uint)Marshal.SizeOf(typeof(VREvent_t));
		return overlay.PollNextOverlayEvent(this.handle, ref pEvent, uncbVREvent);
	}

	public bool ComputeIntersection(Vector3 source, Vector3 direction, ref SteamVR_Overlay.IntersectionResults results)
	{
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay == null)
		{
			return false;
		}
		VROverlayIntersectionParams_t vROverlayIntersectionParams_t = default(VROverlayIntersectionParams_t);
		vROverlayIntersectionParams_t.eOrigin = SteamVR_Render.instance.trackingSpace;
		vROverlayIntersectionParams_t.vSource.v0 = source.x;
		vROverlayIntersectionParams_t.vSource.v1 = source.y;
		vROverlayIntersectionParams_t.vSource.v2 = -source.z;
		vROverlayIntersectionParams_t.vDirection.v0 = direction.x;
		vROverlayIntersectionParams_t.vDirection.v1 = direction.y;
		vROverlayIntersectionParams_t.vDirection.v2 = -direction.z;
		VROverlayIntersectionResults_t vROverlayIntersectionResults_t = default(VROverlayIntersectionResults_t);
		if (!overlay.ComputeOverlayIntersection(this.handle, ref vROverlayIntersectionParams_t, ref vROverlayIntersectionResults_t))
		{
			return false;
		}
		results.point = new Vector3(vROverlayIntersectionResults_t.vPoint.v0, vROverlayIntersectionResults_t.vPoint.v1, -vROverlayIntersectionResults_t.vPoint.v2);
		results.normal = new Vector3(vROverlayIntersectionResults_t.vNormal.v0, vROverlayIntersectionResults_t.vNormal.v1, -vROverlayIntersectionResults_t.vNormal.v2);
		results.UVs = new Vector2(vROverlayIntersectionResults_t.vUVs.v0, vROverlayIntersectionResults_t.vUVs.v1);
		results.distance = vROverlayIntersectionResults_t.fDistance;
		return true;
	}
}
