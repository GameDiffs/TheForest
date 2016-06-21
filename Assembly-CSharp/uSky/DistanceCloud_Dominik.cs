using System;
using TheForest.Utils;
using UnityEngine;

namespace uSky
{
	[AddComponentMenu("uSky/Distance Cloud (beta)"), ExecuteInEditMode, RequireComponent(typeof(uSkyManager))]
	public class DistanceCloud_Dominik : MonoBehaviour
	{
		public Material CloudMaterial;

		public float DomeHeightOffset;

		public Camera CloudCam;

		public int CloudLayer = 13;

		private Mesh skyDome;

		private uSkyManager skyManager;

		public static Vector3 sunlightVector;

		public Color CurrentLightColor
		{
			get
			{
				return Scene.Atmosphere.Sun.color;
			}
		}

		public Color CurrentSkyColor
		{
			get
			{
				return this.colorOffset(RenderSettings.ambientSkyColor, 0.15f, 0.7f, false);
			}
		}

		protected Mesh InitSkyDomeMesh()
		{
			Mesh mesh = Resources.Load<Mesh>("Hemisphere_Mesh");
			Mesh mesh2 = new Mesh();
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3[] expr_26_cp_0 = vertices;
				int expr_26_cp_1 = i;
				expr_26_cp_0[expr_26_cp_1].y = expr_26_cp_0[expr_26_cp_1].y * 0.85f;
			}
			mesh2.vertices = vertices;
			mesh2.triangles = mesh.triangles;
			mesh2.normals = mesh.normals;
			mesh2.uv = mesh.uv;
			mesh2.uv2 = mesh.uv2;
			mesh2.bounds = new Bounds(Vector3.zero, Vector3.one * 2E+09f);
			mesh2.hideFlags = HideFlags.DontSave;
			mesh2.name = "skydomeMesh";
			return mesh2;
		}

		private void OnEnable()
		{
			if (this.skyDome == null)
			{
				this.skyDome = this.InitSkyDomeMesh();
			}
			if (this.skyManager == null)
			{
				this.skyManager = base.GetComponent<uSkyManager>();
			}
		}

		private void OnDisable()
		{
			if (this.skyDome)
			{
				UnityEngine.Object.DestroyImmediate(this.skyDome);
			}
		}

		private void Update()
		{
			this.UpdateCloudMaterial();
			Vector3 position = new Vector3(0f, this.DomeHeightOffset, 0f);
			if (this.skyDome)
			{
				Graphics.DrawMesh(this.skyDome, position, Quaternion.identity, this.CloudMaterial, this.CloudLayer, this.CloudCam, 0, null, false, false);
			}
		}

		private Color colorOffset(Color currentColor, float offsetRange, float rayleighOffsetRange, bool IsGround)
		{
			if (this.skyManager == null)
			{
				return currentColor;
			}
			Vector3 vector = (!(this.skyManager != null)) ? new Vector3(5.81f, 13.57f, 33.13f) : (this.skyManager.BetaR * 1000f);
			Vector3 to = new Vector3(0.5f, 0.5f, 0.5f);
			to = new Vector3(vector.x / 5.81f * 0.5f, vector.y / 13.57f * 0.5f, vector.z / 33.13f * 0.5f);
			to = Vector3.Lerp(new Vector3(Mathf.Abs(1f - to.x), Mathf.Abs(1f - to.y), Mathf.Abs(1f - to.z)), to, this.skyManager.SunsetTime);
			to = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), to, this.skyManager.DayTime);
			Vector3 vector2 = new Vector3(Mathf.Lerp(currentColor.r - offsetRange, currentColor.r + offsetRange, to.x), Mathf.Lerp(currentColor.g - offsetRange, currentColor.g + offsetRange, to.y), Mathf.Lerp(currentColor.b - offsetRange, currentColor.b + offsetRange, to.z));
			Vector3 to2 = new Vector3(vector2.x / vector.x, vector2.y / vector.y, vector2.z / vector.z) * 4f;
			vector2 = ((this.skyManager.RayleighScattering >= 1f) ? Vector3.Lerp(vector2, to2, Mathf.Max(0f, this.skyManager.RayleighScattering - 1f) / 4f * rayleighOffsetRange) : Vector3.Lerp(Vector3.zero, vector2, this.skyManager.RayleighScattering));
			return new Color(vector2.x, vector2.y, vector2.z, 1f) * this.skyManager.Exposure;
		}

		private void UpdateCloudMaterial()
		{
			if (Scene.Atmosphere)
			{
				float b = 1f;
				this.CloudMaterial.SetVector("ShadeColorFromSun", (!this.skyManager.LinearSpace) ? (this.CurrentLightColor * b) : (this.CurrentLightColor.linear * b));
				this.CloudMaterial.SetVector("ShadeColorFromSky", (!this.skyManager.LinearSpace) ? (this.CurrentSkyColor * b) : (this.CurrentSkyColor.linear * b));
				this.CloudMaterial.SetVector("_colorCorrection", this.skyManager.ColorCorrection);
			}
		}
	}
}
