using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Rendering/Global Fog"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	internal class GlobalFog : PostEffectsBase
	{
		[Tooltip("Apply distance-based fog?")]
		public bool distanceFog = true;

		[Tooltip("Distance fog is based on radial distance from camera when checked")]
		public bool useRadialDistance;

		[Tooltip("Apply height-based fog?")]
		public bool heightFog = true;

		[Tooltip("Fog top Y coordinate")]
		public float height = 1f;

		[Range(0.001f, 10f)]
		public float heightDensity = 2f;

		[Tooltip("Push fog away from the camera by this amount")]
		public float startDistance;

		public Shader fogShader;

		private Material fogMaterial;

		public override bool CheckResources()
		{
			base.CheckSupport(true);
			this.fogMaterial = base.CheckShaderAndCreateMaterial(this.fogShader, this.fogMaterial);
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources() || (!this.distanceFog && !this.heightFog))
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
			Vector3 position = transform.position;
			float num2 = position.y - this.height;
			float z = (num2 > 0f) ? 0f : 1f;
			this.fogMaterial.SetMatrix("_FrustumCornersWS", identity);
			this.fogMaterial.SetVector("_CameraWS", position);
			this.fogMaterial.SetVector("_HeightParams", new Vector4(this.height, num2, z, this.heightDensity * 0.5f));
			this.fogMaterial.SetVector("_DistanceParams", new Vector4(-Mathf.Max(this.startDistance, 0f), 0f, 0f, 0f));
			FogMode fogMode = RenderSettings.fogMode;
			float fogDensity = RenderSettings.fogDensity;
			float fogStartDistance = RenderSettings.fogStartDistance;
			float fogEndDistance = RenderSettings.fogEndDistance;
			bool flag = fogMode == FogMode.Linear;
			float num3 = (!flag) ? 0f : (fogEndDistance - fogStartDistance);
			float num4 = (Mathf.Abs(num3) <= 0.0001f) ? 0f : (1f / num3);
			Vector4 vector5;
			vector5.x = fogDensity * 1.2011224f;
			vector5.y = fogDensity * 1.442695f;
			vector5.z = ((!flag) ? 0f : (-num4));
			vector5.w = ((!flag) ? 0f : (fogEndDistance * num4));
			this.fogMaterial.SetVector("_SceneFogParams", vector5);
			this.fogMaterial.SetVector("_SceneFogMode", new Vector4((float)fogMode, (float)((!this.useRadialDistance) ? 0 : 1), 0f, 0f));
			int passNr;
			if (this.distanceFog && this.heightFog)
			{
				passNr = 0;
			}
			else if (this.distanceFog)
			{
				passNr = 1;
			}
			else
			{
				passNr = 2;
			}
			GlobalFog.CustomGraphicsBlit(source, destination, this.fogMaterial, passNr);
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
