using System;
using UnityEngine;

namespace Smaa
{
	[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class SMAA : MonoBehaviour
	{
		public HDRMode Hdr;

		public DebugPass DebugPass;

		public QualityPreset Quality = QualityPreset.High;

		public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

		public bool UsePredication;

		public Preset CustomPreset;

		public PredicationPreset CustomPredicationPreset;

		public Shader Shader;

		public Texture2D AreaTex;

		public Texture2D SearchTex;

		protected Camera m_Camera;

		protected Preset[] m_StdPresets;

		protected Material m_Material;

		public Material Material
		{
			get
			{
				if (this.m_Material == null)
				{
					this.m_Material = new Material(this.Shader);
					this.m_Material.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_Material;
			}
		}

		private void OnEnable()
		{
			if (this.AreaTex == null)
			{
				this.AreaTex = Resources.Load<Texture2D>("AreaTex");
			}
			if (this.SearchTex == null)
			{
				this.SearchTex = Resources.Load<Texture2D>("SearchTex");
			}
			this.m_Camera = base.GetComponent<Camera>();
			this.CreatePresets();
		}

		private void Start()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				base.enabled = false;
				return;
			}
			if (!this.Shader || !this.Shader.isSupported)
			{
				base.enabled = false;
			}
		}

		private void OnDisable()
		{
			if (this.m_Material != null)
			{
				UnityEngine.Object.Destroy(this.m_Material);
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			int pixelWidth = this.m_Camera.pixelWidth;
			int pixelHeight = this.m_Camera.pixelHeight;
			Preset preset = this.CustomPreset;
			if (this.Quality != QualityPreset.Custom)
			{
				preset = this.m_StdPresets[(int)this.Quality];
			}
			int detectionMethod = (int)this.DetectionMethod;
			int pass = 4;
			int pass2 = 5;
			RenderTextureFormat format = source.format;
			if (this.Hdr == HDRMode.Off)
			{
				format = RenderTextureFormat.ARGB32;
			}
			else if (this.Hdr == HDRMode.On)
			{
				format = RenderTextureFormat.ARGBHalf;
			}
			this.Material.SetTexture("_AreaTex", this.AreaTex);
			this.Material.SetTexture("_SearchTex", this.SearchTex);
			this.Material.SetVector("_Metrics", new Vector4(1f / (float)pixelWidth, 1f / (float)pixelHeight, (float)pixelWidth, (float)pixelHeight));
			this.Material.SetVector("_Params1", new Vector4(preset.Threshold, preset.DepthThreshold, (float)preset.MaxSearchSteps, (float)preset.MaxSearchStepsDiag));
			this.Material.SetVector("_Params2", new Vector2((float)preset.CornerRounding, preset.LocalContrastAdaptationFactor));
			if (this.DetectionMethod == EdgeDetectionMethod.Depth)
			{
				this.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
			}
			else if (this.UsePredication)
			{
				this.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
				this.Material.SetVector("_Params3", new Vector3(this.CustomPredicationPreset.Threshold, this.CustomPredicationPreset.Scale, this.CustomPredicationPreset.Strength));
			}
			Shader.DisableKeyword("USE_DIAG_SEARCH");
			Shader.DisableKeyword("USE_CORNER_DETECTION");
			if (preset.DiagDetection)
			{
				Shader.EnableKeyword("USE_DIAG_SEARCH");
			}
			if (preset.CornerDetection)
			{
				Shader.EnableKeyword("USE_CORNER_DETECTION");
			}
			RenderTexture renderTexture = this.TempRT(pixelWidth, pixelHeight, format);
			RenderTexture renderTexture2 = this.TempRT(pixelWidth, pixelHeight, format);
			this.Clear(renderTexture);
			this.Clear(renderTexture2);
			Graphics.Blit(source, renderTexture, this.Material, detectionMethod);
			if (this.DebugPass == DebugPass.Edges)
			{
				Graphics.Blit(renderTexture, destination);
			}
			else
			{
				Graphics.Blit(renderTexture, renderTexture2, this.Material, pass);
				if (this.DebugPass == DebugPass.Weights)
				{
					Graphics.Blit(renderTexture2, destination);
				}
				else
				{
					this.Material.SetTexture("_BlendTex", renderTexture2);
					Graphics.Blit(source, destination, this.Material, pass2);
				}
			}
			RenderTexture.ReleaseTemporary(renderTexture);
			RenderTexture.ReleaseTemporary(renderTexture2);
		}

		private void Clear(RenderTexture rt)
		{
			Graphics.Blit(rt, rt, this.Material, 0);
		}

		private RenderTexture TempRT(int width, int height, RenderTextureFormat format)
		{
			int depthBuffer = 0;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, RenderTextureReadWrite.Linear);
		}

		private void CreatePresets()
		{
			this.m_StdPresets = new Preset[4];
			this.m_StdPresets[0] = new Preset
			{
				Threshold = 0.15f,
				MaxSearchSteps = 4
			};
			this.m_StdPresets[0].DiagDetection = false;
			this.m_StdPresets[0].CornerDetection = false;
			this.m_StdPresets[1] = new Preset
			{
				Threshold = 0.1f,
				MaxSearchSteps = 8
			};
			this.m_StdPresets[1].DiagDetection = false;
			this.m_StdPresets[1].CornerDetection = false;
			this.m_StdPresets[2] = new Preset
			{
				Threshold = 0.1f,
				MaxSearchSteps = 16,
				MaxSearchStepsDiag = 8,
				CornerRounding = 25
			};
			this.m_StdPresets[3] = new Preset
			{
				Threshold = 0.05f,
				MaxSearchSteps = 32,
				MaxSearchStepsDiag = 16,
				CornerRounding = 25
			};
		}
	}
}
