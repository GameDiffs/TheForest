using Ceto.Common.Unity.Utility;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	[AddComponentMenu("Ceto/Components/UnderWater"), DisallowMultipleComponent, RequireComponent(typeof(Ocean))]
	public class UnderWater : UnderWaterBase
	{
		public const float MAX_REFRACTION_INTENSITY = 2f;

		public const float MAX_REFRACTION_DISORTION = 4f;

		private readonly float OCEAN_BOTTOM_DEPTH = 1000f;

		private readonly float MAX_DEPTH_DIST = 500f;

		public UNDERWATER_MODE underwaterMode;

		public DEPTH_MODE depthMode;

		public LayerMask oceanDepthsMask = 1;

		[Range(0f, 1f)]
		public float depthBlend = 0.2f;

		[Range(0f, 1f)]
		public float edgeFade = 0.2f;

		[Range(0f, 2f)]
		public float refractionIntensity = 0.5f;

		[Range(0f, 4f)]
		public float refractionDistortion = 0.5f;

		[Range(0f, 1f)]
		public float absorptionR = 0.45f;

		[Range(0f, 1f)]
		public float absorptionG = 0.029f;

		[Range(0f, 1f)]
		public float absorptionB = 0.018f;

		public AbsorptionModifier aboveAbsorptionModifier = new AbsorptionModifier(2f, 1f, Color.white);

		public AbsorptionModifier belowAbsorptionModifier = new AbsorptionModifier(0.1f, 1f, Color.white);

		public AbsorptionModifier subSurfaceScatterModifier = new AbsorptionModifier(10f, 1.5f, new Color32(220, 250, 180, 255));

		public InscatterModifier aboveInscatterModifier = new InscatterModifier(50f, 1f, new Color32(2, 25, 43, 255), INSCATTER_MODE.EXP);

		public InscatterModifier belowInscatterModifier = new InscatterModifier(60f, 1f, new Color32(7, 51, 77, 255), INSCATTER_MODE.EXP);

		public CausticModifier causticModifier = new CausticModifier(0.5f, 0.1f, 0.75f, Color.white);

		public CausticTexture causticTexture;

		private GameObject m_bottomMask;

		[HideInInspector]
		public Shader oceanBottomSdr;

		[HideInInspector]
		public Shader oceanDepthSdr;

		[HideInInspector]
		public Shader copyDepthSdr;

		[HideInInspector]
		public Shader oceanMaskSdr;

		[HideInInspector]
		public Shader oceanMaskFlippedSdr;

		[HideInInspector]
		public Shader normalFadeSdr;

		public override UNDERWATER_MODE Mode
		{
			get
			{
				return this.underwaterMode;
			}
		}

		public override DEPTH_MODE DepthMode
		{
			get
			{
				return this.depthMode;
			}
		}

		private void Start()
		{
			try
			{
				Mesh mesh = this.CreateBottomMesh(32, 512);
				this.m_bottomMask = new GameObject("Ceto Bottom Mask Gameobject");
				MeshFilter meshFilter = this.m_bottomMask.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = this.m_bottomMask.AddComponent<MeshRenderer>();
				NotifyOnWillRender notifyOnWillRender = this.m_bottomMask.AddComponent<NotifyOnWillRender>();
				meshFilter.sharedMesh = mesh;
				meshRenderer.receiveShadows = false;
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
				meshRenderer.material = new Material(this.oceanBottomSdr);
				notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderWaveOverlays));
				notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanMask));
				notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanDepth));
				this.m_bottomMask.layer = LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
				this.m_bottomMask.hideFlags = HideFlags.HideAndDontSave;
				this.UpdateBottomBounds();
				UnityEngine.Object.Destroy(mesh);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			try
			{
				Shader.EnableKeyword("CETO_UNDERWATER_ON");
				this.SetBottomActive(this.m_bottomMask, true);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			try
			{
				Shader.DisableKeyword("CETO_UNDERWATER_ON");
				this.SetBottomActive(this.m_bottomMask, false);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			try
			{
				if (this.m_bottomMask != null)
				{
					Mesh mesh = this.m_bottomMask.GetComponent<MeshFilter>().mesh;
					UnityEngine.Object.Destroy(this.m_bottomMask);
					UnityEngine.Object.Destroy(mesh);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		private void Update()
		{
			try
			{
				Vector4 vector = new Vector4(this.absorptionR, this.absorptionG, this.absorptionB, 1f);
				Vector4 vec = vector;
				Vector4 vec2 = vector;
				vector.w = Mathf.Max(0f, this.aboveAbsorptionModifier.scale);
				vec.w = Mathf.Max(0f, this.subSurfaceScatterModifier.scale);
				vec2.w = Mathf.Max(0f, this.belowAbsorptionModifier.scale);
				Color color = this.aboveAbsorptionModifier.tint * Mathf.Max(0f, this.aboveAbsorptionModifier.intensity);
				Color color2 = this.subSurfaceScatterModifier.tint * Mathf.Max(0f, this.subSurfaceScatterModifier.intensity);
				Color color3 = this.belowAbsorptionModifier.tint * Mathf.Max(0f, this.belowAbsorptionModifier.intensity);
				Vector4 vec3 = default(Vector4);
				vec3.x = ((this.causticTexture.scale.x == 0f) ? 1f : (1f / this.causticTexture.scale.x));
				vec3.y = ((this.causticTexture.scale.y == 0f) ? 1f : (1f / this.causticTexture.scale.y));
				vec3.z = this.causticModifier.distortion;
				vec3.w = Mathf.Clamp01(this.causticModifier.depthFade);
				Shader.SetGlobalVector("Ceto_AbsCof", vector);
				Shader.SetGlobalColor("Ceto_AbsTint", color);
				Shader.SetGlobalVector("Ceto_SSSCof", vec);
				Shader.SetGlobalColor("Ceto_SSSTint", color2);
				Shader.SetGlobalVector("Ceto_BelowCof", vec2);
				Shader.SetGlobalColor("Ceto_BelowTint", color3);
				Color color4 = this.aboveInscatterModifier.color;
				color4.a = Mathf.Clamp01(this.aboveInscatterModifier.intensity);
				Shader.SetGlobalFloat("Ceto_AboveInscatterScale", Mathf.Max(0.1f, this.aboveInscatterModifier.scale));
				Shader.SetGlobalVector("Ceto_AboveInscatterMode", this.InscatterModeToMask(this.aboveInscatterModifier.mode));
				Shader.SetGlobalColor("Ceto_AboveInscatterColor", color4);
				Color color5 = this.belowInscatterModifier.color;
				color5.a = Mathf.Clamp01(this.belowInscatterModifier.intensity);
				Shader.SetGlobalFloat("Ceto_BelowInscatterScale", Mathf.Max(0.1f, this.belowInscatterModifier.scale));
				Shader.SetGlobalVector("Ceto_BelowInscatterMode", this.InscatterModeToMask(this.belowInscatterModifier.mode));
				Shader.SetGlobalColor("Ceto_BelowInscatterColor", color5);
				Shader.SetGlobalFloat("Ceto_RefractionIntensity", Mathf.Max(0f, this.refractionIntensity));
				Shader.SetGlobalFloat("Ceto_RefractionDistortion", this.refractionDistortion * 0.05f);
				Shader.SetGlobalFloat("Ceto_MaxDepthDist", Mathf.Max(0f, this.MAX_DEPTH_DIST));
				Shader.SetGlobalFloat("Ceto_DepthBlend", Mathf.Clamp01(this.depthBlend));
				Shader.SetGlobalFloat("Ceto_EdgeFade", Mathf.Lerp(20f, 2f, Mathf.Clamp01(this.edgeFade)));
				Shader.SetGlobalTexture("Ceto_CausticTexture", (!(this.causticTexture.tex != null)) ? Texture2D.blackTexture : this.causticTexture.tex);
				Shader.SetGlobalVector("Ceto_CausticTextureScale", vec3);
				Shader.SetGlobalColor("Ceto_CausticTint", this.causticModifier.tint * this.causticModifier.intensity);
				if (this.depthMode == DEPTH_MODE.USE_OCEAN_DEPTH_PASS)
				{
					Shader.EnableKeyword("CETO_USE_OCEAN_DEPTHS_BUFFER");
					if (this.underwaterMode == UNDERWATER_MODE.ABOVE_ONLY)
					{
						this.SetBottomActive(this.m_bottomMask, false);
					}
					else
					{
						this.SetBottomActive(this.m_bottomMask, true);
						this.UpdateBottomBounds();
					}
				}
				else
				{
					Shader.DisableKeyword("CETO_USE_OCEAN_DEPTHS_BUFFER");
					if (this.underwaterMode == UNDERWATER_MODE.ABOVE_ONLY)
					{
						this.SetBottomActive(this.m_bottomMask, false);
					}
					else
					{
						this.SetBottomActive(this.m_bottomMask, true);
						this.UpdateBottomBounds();
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		private void SetBottomActive(GameObject bottom, bool active)
		{
			if (bottom != null)
			{
				bottom.SetActive(active);
			}
		}

		private Vector3 InscatterModeToMask(INSCATTER_MODE mode)
		{
			switch (mode)
			{
			case INSCATTER_MODE.LINEAR:
				return new Vector3(1f, 0f, 0f);
			case INSCATTER_MODE.EXP:
				return new Vector3(0f, 1f, 0f);
			case INSCATTER_MODE.EXP2:
				return new Vector3(0f, 0f, 1f);
			default:
				return new Vector3(0f, 0f, 1f);
			}
		}

		private void FitBottomToCamera()
		{
			if (!base.enabled || this.m_bottomMask == null)
			{
				return;
			}
			Camera current = Camera.current;
			Vector3 position = current.transform.position;
			float num = current.farClipPlane * 0.85f;
			this.m_bottomMask.transform.localScale = new Vector3(num, this.OCEAN_BOTTOM_DEPTH, num);
			float num2 = 0f;
			this.m_bottomMask.transform.localPosition = new Vector3(position.x, -this.OCEAN_BOTTOM_DEPTH + this.m_ocean.level - num2, position.z);
		}

		private void UpdateBottomBounds()
		{
			float num = 1E+08f;
			float level = this.m_ocean.level;
			if (this.m_bottomMask != null && this.m_bottomMask.activeSelf)
			{
				Bounds bounds = new Bounds(new Vector3(0f, level, 0f), new Vector3(num, this.OCEAN_BOTTOM_DEPTH, num));
				this.m_bottomMask.GetComponent<MeshFilter>().mesh.bounds = bounds;
			}
		}

		private LayerMask GetOceanDepthsLayermask(OceanCameraSettings settings)
		{
			return (!(settings != null)) ? this.oceanDepthsMask : settings.oceanDepthsMask;
		}

		private bool GetDisableUnderwater(OceanCameraSettings settings)
		{
			return settings != null && settings.disableUnderwater;
		}

		public override void RenderOceanMask(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					if (!(this.oceanMaskSdr == null))
					{
						if (this.underwaterMode != UNDERWATER_MODE.ABOVE_ONLY)
						{
							Camera current = Camera.current;
							if (!(current == null))
							{
								CameraData cameraData = this.m_ocean.FindCameraData(current);
								if (cameraData.mask == null)
								{
									cameraData.mask = new MaskData();
								}
								if (!cameraData.mask.updated)
								{
									if (current.name == "SceneCamera" || current.GetComponent<UnderWaterPostEffect>() == null || SystemInfo.graphicsShaderLevel < 30 || this.GetDisableUnderwater(cameraData.settings))
									{
										Shader.SetGlobalTexture(Ocean.OCEAN_MASK_TEXTURE_NAME, Texture2D.blackTexture);
										cameraData.mask.updated = true;
									}
									else
									{
										this.CreateMaskCameraFor(current, cameraData.mask);
										this.FitBottomToCamera();
										NotifyOnEvent.Disable = true;
										if (this.m_ocean.Projection.IsFlipped)
										{
											cameraData.mask.cam.RenderWithShader(this.oceanMaskFlippedSdr, "OceanMask");
										}
										else
										{
											cameraData.mask.cam.RenderWithShader(this.oceanMaskSdr, "OceanMask");
										}
										NotifyOnEvent.Disable = false;
										Shader.SetGlobalTexture(Ocean.OCEAN_MASK_TEXTURE_NAME, cameraData.mask.cam.targetTexture);
										cameraData.mask.updated = true;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		public override void RenderOceanDepth(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					Camera current = Camera.current;
					if (!(current == null))
					{
						CameraData cameraData = this.m_ocean.FindCameraData(current);
						if (cameraData.depth == null)
						{
							cameraData.depth = new DepthData();
						}
						if (!cameraData.depth.updated)
						{
							if (this.GetDisableUnderwater(cameraData.settings))
							{
								Shader.DisableKeyword("CETO_UNDERWATER_ON");
								cameraData.depth.updated = true;
							}
							else
							{
								Shader.EnableKeyword("CETO_UNDERWATER_ON");
								if (SystemInfo.graphicsShaderLevel < 30)
								{
									Shader.SetGlobalTexture(Ocean.OCEAN_DEPTH_TEXTURE_NAME, Texture2D.whiteTexture);
									Shader.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, Texture2D.whiteTexture);
									Shader.SetGlobalTexture(Ocean.NORMAL_FADE_TEXTURE_NAME, Texture2D.blackTexture);
									Shader.SetGlobalMatrix("Ceto_Camera_IVP", (current.projectionMatrix * current.worldToCameraMatrix).inverse);
									cameraData.depth.updated = true;
								}
								else if (this.depthMode == DEPTH_MODE.USE_DEPTH_BUFFER)
								{
									Shader.SetGlobalTexture(Ocean.OCEAN_DEPTH_TEXTURE_NAME, Texture2D.whiteTexture);
									current.depthTextureMode |= DepthTextureMode.Depth;
									current.depthTextureMode |= DepthTextureMode.DepthNormals;
									Shader.SetGlobalMatrix("Ceto_Camera_IVP", (current.projectionMatrix * current.worldToCameraMatrix).inverse);
									this.CreateRefractionCommand(current, cameraData.depth);
									cameraData.depth.updated = true;
								}
								else if (this.depthMode == DEPTH_MODE.USE_OCEAN_DEPTH_PASS)
								{
									Shader.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, Texture2D.whiteTexture);
									Shader.SetGlobalTexture(Ocean.NORMAL_FADE_TEXTURE_NAME, Texture2D.blackTexture);
									this.CreateDepthCameraFor(current, cameraData.depth);
									this.CreateRefractionCommand(current, cameraData.depth);
									cameraData.depth.cam.cullingMask = this.GetOceanDepthsLayermask(cameraData.settings);
									cameraData.depth.cam.cullingMask = OceanUtility.HideLayer(cameraData.depth.cam.cullingMask, Ocean.OCEAN_LAYER);
									NotifyOnEvent.Disable = true;
									cameraData.depth.cam.RenderWithShader(this.oceanDepthSdr, "RenderType");
									NotifyOnEvent.Disable = false;
									Shader.SetGlobalTexture(Ocean.OCEAN_DEPTH_TEXTURE_NAME, cameraData.depth.cam.targetTexture);
									cameraData.depth.updated = true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		private void CreateMaskCameraFor(Camera cam, MaskData data)
		{
			if (data.cam == null)
			{
				GameObject gameObject = new GameObject("Ceto Mask Camera: " + cam.name);
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.AddComponent<IgnoreOceanEvents>();
				gameObject.AddComponent<DisableFog>();
				gameObject.AddComponent<DisableShadows>();
				data.cam = gameObject.AddComponent<Camera>();
				data.cam.clearFlags = CameraClearFlags.Color;
				data.cam.backgroundColor = Color.black;
				data.cam.cullingMask = 1 << LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
				data.cam.enabled = false;
				data.cam.renderingPath = RenderingPath.Forward;
				data.cam.targetTexture = null;
				data.cam.useOcclusionCulling = false;
				data.cam.RemoveAllCommandBuffers();
				data.cam.targetTexture = null;
			}
			data.cam.fieldOfView = cam.fieldOfView;
			data.cam.nearClipPlane = cam.nearClipPlane;
			data.cam.farClipPlane = cam.farClipPlane;
			data.cam.transform.position = cam.transform.position;
			data.cam.transform.rotation = cam.transform.rotation;
			data.cam.worldToCameraMatrix = cam.worldToCameraMatrix;
			data.cam.projectionMatrix = cam.projectionMatrix;
			data.cam.orthographic = cam.orthographic;
			data.cam.aspect = cam.aspect;
			data.cam.orthographicSize = cam.orthographicSize;
			data.cam.rect = new Rect(0f, 0f, 1f, 1f);
			if (data.cam.farClipPlane < this.OCEAN_BOTTOM_DEPTH * 2f)
			{
				data.cam.farClipPlane = this.OCEAN_BOTTOM_DEPTH * 2f;
				data.cam.ResetProjectionMatrix();
			}
			RenderTexture targetTexture = data.cam.targetTexture;
			if (targetTexture == null || targetTexture.width != cam.pixelWidth || targetTexture.height != cam.pixelHeight)
			{
				if (targetTexture != null)
				{
					RTUtility.ReleaseAndDestroy(targetTexture);
				}
				int pixelWidth = cam.pixelWidth;
				int pixelHeight = cam.pixelHeight;
				int depth = 32;
				RenderTextureFormat format = RenderTextureFormat.RGHalf;
				if (!SystemInfo.SupportsRenderTextureFormat(format))
				{
					format = RenderTextureFormat.ARGBHalf;
				}
				data.cam.targetTexture = new RenderTexture(pixelWidth, pixelHeight, depth, format, RenderTextureReadWrite.Linear);
				data.cam.targetTexture.filterMode = FilterMode.Point;
				data.cam.targetTexture.hideFlags = HideFlags.DontSave;
				data.cam.targetTexture.name = "Ceto Mask Render Target: " + cam.name;
			}
		}

		private void CreateDepthCameraFor(Camera cam, DepthData data)
		{
			if (data.cam == null)
			{
				GameObject gameObject = new GameObject("Ceto Depth Camera: " + cam.name);
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.AddComponent<IgnoreOceanEvents>();
				gameObject.AddComponent<DisableFog>();
				gameObject.AddComponent<DisableShadows>();
				data.cam = gameObject.AddComponent<Camera>();
				data.cam.clearFlags = CameraClearFlags.Color;
				data.cam.backgroundColor = Color.white;
				data.cam.enabled = false;
				data.cam.renderingPath = RenderingPath.Forward;
				data.cam.targetTexture = null;
				data.cam.useOcclusionCulling = false;
				data.cam.RemoveAllCommandBuffers();
				data.cam.targetTexture = null;
			}
			data.cam.fieldOfView = cam.fieldOfView;
			data.cam.nearClipPlane = cam.nearClipPlane;
			data.cam.farClipPlane = cam.farClipPlane;
			data.cam.transform.position = cam.transform.position;
			data.cam.transform.rotation = cam.transform.rotation;
			data.cam.worldToCameraMatrix = cam.worldToCameraMatrix;
			data.cam.projectionMatrix = cam.projectionMatrix;
			data.cam.orthographic = cam.orthographic;
			data.cam.aspect = cam.aspect;
			data.cam.orthographicSize = cam.orthographicSize;
			data.cam.rect = new Rect(0f, 0f, 1f, 1f);
			data.cam.layerCullDistances = cam.layerCullDistances;
			data.cam.layerCullSpherical = cam.layerCullSpherical;
			RenderTexture targetTexture = data.cam.targetTexture;
			if (targetTexture == null || targetTexture.width != cam.pixelWidth || targetTexture.height != cam.pixelHeight)
			{
				if (targetTexture != null)
				{
					RTUtility.ReleaseAndDestroy(targetTexture);
				}
				int pixelWidth = cam.pixelWidth;
				int pixelHeight = cam.pixelHeight;
				int depth = 24;
				RenderTextureFormat format = RenderTextureFormat.RGFloat;
				if (!SystemInfo.SupportsRenderTextureFormat(format))
				{
					format = RenderTextureFormat.RGHalf;
				}
				if (!SystemInfo.SupportsRenderTextureFormat(format))
				{
					format = RenderTextureFormat.ARGBHalf;
				}
				data.cam.targetTexture = new RenderTexture(pixelWidth, pixelHeight, depth, format, RenderTextureReadWrite.Linear);
				data.cam.targetTexture.filterMode = FilterMode.Bilinear;
				data.cam.targetTexture.hideFlags = HideFlags.DontSave;
				data.cam.targetTexture.name = "Ceto Ocean Depths Render Target: " + cam.name;
			}
		}

		private void CreateRefractionCommand(Camera cam, DepthData data)
		{
			if (this.depthMode == DEPTH_MODE.USE_DEPTH_BUFFER)
			{
				if (data.refractionCommand == null)
				{
					data.refractionCommand = new RefractionCommand(cam, this.copyDepthSdr, this.normalFadeSdr);
				}
				if (!data.refractionCommand.DisableCopyDepthCmd && base.DisableCopyDepthCmd)
				{
					Shader.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, Texture2D.whiteTexture);
				}
				if (!data.refractionCommand.DisableNormalFadeCmd && base.DisableNormalFadeCmd)
				{
					Shader.SetGlobalTexture(Ocean.NORMAL_FADE_TEXTURE_NAME, Texture2D.blackTexture);
				}
				data.refractionCommand.DisableCopyDepthCmd = base.DisableCopyDepthCmd;
				data.refractionCommand.DisableNormalFadeCmd = base.DisableNormalFadeCmd;
				data.refractionCommand.UpdateCommands();
			}
			else if (data.refractionCommand != null)
			{
				data.refractionCommand.ClearCommands();
				data.refractionCommand = null;
			}
		}

		private Mesh CreateBottomMesh(int segementsX, int segementsY)
		{
			Vector3[] array = new Vector3[segementsX * segementsY];
			Vector2[] uv = new Vector2[segementsX * segementsY];
			float num = 6.28318548f;
			for (int i = 0; i < segementsX; i++)
			{
				for (int j = 0; j < segementsY; j++)
				{
					float num2 = (float)i / (float)(segementsX - 1);
					array[i + j * segementsX].x = num2 * Mathf.Cos(num * (float)j / (float)(segementsY - 1));
					array[i + j * segementsX].y = 0f;
					array[i + j * segementsX].z = num2 * Mathf.Sin(num * (float)j / (float)(segementsY - 1));
					if (i == segementsX - 1)
					{
						array[i + j * segementsX].y = 1f;
					}
				}
			}
			int[] array2 = new int[segementsX * segementsY * 6];
			int num3 = 0;
			for (int k = 0; k < segementsX - 1; k++)
			{
				for (int l = 0; l < segementsY - 1; l++)
				{
					array2[num3++] = k + l * segementsX;
					array2[num3++] = k + (l + 1) * segementsX;
					array2[num3++] = k + 1 + l * segementsX;
					array2[num3++] = k + (l + 1) * segementsX;
					array2[num3++] = k + 1 + (l + 1) * segementsX;
					array2[num3++] = k + 1 + l * segementsX;
				}
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.uv = uv;
			mesh.triangles = array2;
			mesh.name = "Ceto Bottom Mesh";
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.RecalculateNormals();
			mesh.Optimize();
			return mesh;
		}
	}
}
