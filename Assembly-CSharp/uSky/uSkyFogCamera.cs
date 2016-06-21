using System;
using UnityEngine;

namespace uSky
{
	[AddComponentMenu("uSky/uSky Fog Camera"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class uSkyFogCamera : MonoBehaviour
	{
		public Material fogMaterial;

		protected bool CheckSupport()
		{
			if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
			{
				base.enabled = false;
				return false;
			}
			if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
			{
				base.enabled = false;
				return false;
			}
			base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
			return true;
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckSupport())
			{
				Graphics.Blit(source, destination);
				return;
			}
			Camera component = base.GetComponent<Camera>();
			Transform transform = component.transform;
			float nearClipPlane = component.nearClipPlane;
			float farClipPlane = component.farClipPlane;
			float fieldOfView = component.fieldOfView;
			float aspect = component.aspect;
			Matrix4x4 identity = Matrix4x4.identity;
			float num = fieldOfView * 0.5f;
			Vector3 b = transform.right * nearClipPlane * Mathf.Tan(num * 0.0174532924f) * aspect;
			Vector3 b2 = transform.up * nearClipPlane * Mathf.Tan(num * 0.0174532924f);
			Vector3 vector = transform.forward * nearClipPlane - b + b2;
			float d = vector.magnitude * farClipPlane / nearClipPlane;
			vector.Normalize();
			vector *= d;
			Vector3 vector2 = transform.forward * nearClipPlane + b + b2;
			vector2.Normalize();
			vector2 *= d;
			Vector3 vector3 = transform.forward * nearClipPlane + b - b2;
			vector3.Normalize();
			vector3 *= d;
			Vector3 vector4 = transform.forward * nearClipPlane - b - b2;
			vector4.Normalize();
			vector4 *= d;
			identity.SetRow(0, vector);
			identity.SetRow(1, vector2);
			identity.SetRow(2, vector3);
			identity.SetRow(3, vector4);
			this.fogMaterial.SetMatrix("_FrustumCornersWS", identity);
			this.fogMaterial.SetVector("_CameraWS", transform.position);
			uSkyFogCamera.CustomGraphicsBlit(source, destination, this.fogMaterial, 0);
		}

		private static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
		{
			RenderTexture.active = dest;
			fxMaterial.SetTexture("_MainTex", source);
			GL.PushMatrix();
			GL.LoadOrtho();
			fxMaterial.SetPass(passNr);
			GL.Begin(7);
			GL.MultiTexCoord2(0, 0f, 0f);
			GL.Vertex3(0f, 0f, 3f);
			GL.MultiTexCoord2(0, 1f, 0f);
			GL.Vertex3(1f, 0f, 2f);
			GL.MultiTexCoord2(0, 1f, 1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.MultiTexCoord2(0, 0f, 1f);
			GL.Vertex3(0f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
