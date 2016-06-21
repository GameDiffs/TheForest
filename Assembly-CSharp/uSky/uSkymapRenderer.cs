using System;
using UnityEngine;

namespace uSky
{
	[AddComponentMenu("uSky/uSkymap Renderer")]
	public class uSkymapRenderer : MonoBehaviour
	{
		private RenderTexture m_skyMap;

		public Material m_skymapMaterial;

		public Material m_oceanMaterial;

		public int SkymapResolution = 256;

		[Range(0f, 2f)]
		public float SkyReflection = 1f;

		[Range(0f, 2f)]
		public float CloudReflection = 1f;

		public float CloudTextureScale = 5.9f;

		public bool DebugSkymap;

		private int m_frameCount;

		private uSkyManager m_uSM;

		private DistanceCloud m_DC;

		private GameObject m_cloudCamera;

		private RenderTexture m_cloudRT;

		private bool RenderCloud;

		private Camera cam;

		protected uSkyManager uSM
		{
			get
			{
				if (this.m_uSM == null)
				{
					this.m_uSM = base.gameObject.GetComponent<uSkyManager>();
					if (this.m_uSM == null)
					{
						Debug.Log("Can't not find uSkyManager");
					}
				}
				return this.m_uSM;
			}
		}

		private DistanceCloud DC
		{
			get
			{
				if (this.m_DC == null)
				{
					this.m_DC = base.gameObject.GetComponent<DistanceCloud>();
					if (this.m_DC == null)
					{
						Debug.Log("Can't not find DistanceCloud");
					}
				}
				return this.m_DC;
			}
		}

		private void Start()
		{
			if (!SystemInfo.supportsRenderTextures)
			{
				Debug.LogWarning("RenderTexture is not supported with your Graphic Card");
				return;
			}
			if (this.uSM == null)
			{
				Debug.Log("Can NOT find uSkyManager, Please asign this uSkymapRenderer script to uSkyManager gameObject.");
				base.enabled = false;
				return;
			}
			this.m_skyMap = new RenderTexture(this.SkymapResolution, this.SkymapResolution, 0, RenderTextureFormat.ARGBHalf);
			this.m_skyMap.filterMode = FilterMode.Trilinear;
			this.m_skyMap.wrapMode = TextureWrapMode.Clamp;
			this.m_skyMap.anisoLevel = 1;
			this.m_skyMap.Create();
			if (this.m_skymapMaterial != null)
			{
				this.InitMaterial(this.m_skymapMaterial);
				Graphics.Blit(null, this.m_skyMap, this.m_skymapMaterial);
			}
			if (this.m_oceanMaterial != null)
			{
				this.m_oceanMaterial.SetTexture("_SkyMap", this.m_skyMap);
				this.m_oceanMaterial.SetVector("EARTH_POS", new Vector3(0f, 6360010f, 0f));
				this.updateOceanMaterial(this.m_oceanMaterial);
			}
			this.RenderCloud = (this.DC != null);
			if (this.RenderCloud)
			{
				this.m_cloudRT = new RenderTexture(this.SkymapResolution, this.SkymapResolution, 0, RenderTextureFormat.ARGBHalf);
				this.m_cloudRT.filterMode = FilterMode.Trilinear;
				this.m_cloudRT.wrapMode = TextureWrapMode.Clamp;
				this.m_cloudRT.anisoLevel = 1;
				this.m_cloudRT.Create();
				if (this.m_cloudCamera == null)
				{
					this.m_cloudCamera = new GameObject("cloudCamera", new Type[]
					{
						typeof(Camera)
					});
				}
				this.m_cloudCamera.hideFlags = HideFlags.HideInHierarchy;
				this.m_cloudCamera.transform.Rotate(new Vector3(270f, 0f, 0f));
				this.cam = this.m_cloudCamera.GetComponent<Camera>();
				this.cam.orthographic = true;
				this.cam.orthographicSize = this.CloudTextureScale;
				this.cam.aspect = 1f;
				this.cam.backgroundColor = Color.black;
				this.cam.clearFlags = CameraClearFlags.Color;
				this.cam.targetTexture = this.m_cloudRT;
				this.m_skymapMaterial.SetTexture("CloudSampler", this.m_cloudRT);
			}
		}

		private void Update()
		{
			if (this.m_skyMap != null && this.m_skymapMaterial != null)
			{
				if (this.m_frameCount == 1)
				{
					this.InitMaterial(this.m_skymapMaterial);
				}
				this.m_frameCount++;
				Graphics.Blit(null, this.m_skyMap, this.m_skymapMaterial);
				this.InitMaterial(this.m_skymapMaterial);
				if (this.m_oceanMaterial != null)
				{
					this.updateOceanMaterial(this.m_oceanMaterial);
				}
			}
		}

		private void InitMaterial(Material mat)
		{
			if (this.uSM != null)
			{
				mat.SetVector("_SunDir", this.uSM.SunDir);
				mat.SetVector("_betaR", this.uSM.BetaR);
				mat.SetVector("_betaM", new Vector3(0.004f, 0.004f, 0.004f) * 0.9f);
				mat.SetVector("_SkyMultiplier", new Vector3(this.uSM.skyMultiplier.x, this.uSM.skyMultiplier.y * this.SkyReflection, this.CloudReflection));
				mat.SetVector("_mieConst", this.uSM.mieConst);
				mat.SetVector("_miePhase_g", this.uSM.miePhase_g);
				mat.SetVector("_NightHorizonColor", this.uSM.getNightHorizonColor * this.SkyReflection);
				mat.SetVector("_NightZenithColor", this.uSM.getNightZenithColor * this.SkyReflection);
				mat.SetVector("_colorCorrection", this.uSM.ColorCorrection);
			}
		}

		private void updateOceanMaterial(Material mat)
		{
			mat.SetVector("SUN_DIR", this.uSM.SunDir);
			mat.SetFloat("SUN_INTENSITY", 100f * this.uSM.Exposure);
		}

		private void OnDestroy()
		{
			this.m_skyMap.Release();
			if (this.m_cloudRT != null)
			{
				this.m_cloudRT.Release();
			}
		}
	}
}
