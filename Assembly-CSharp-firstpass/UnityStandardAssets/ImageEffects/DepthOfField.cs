using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Camera/Depth of Field (Lens Blur, Scatter, DX11)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class DepthOfField : PostEffectsBase
	{
		public enum BlurType
		{
			DiscBlur,
			DX11
		}

		public enum BlurSampleCount
		{
			Low,
			Medium,
			High
		}

		public bool visualizeFocus;

		public float focalLength = 10f;

		public float focalSize = 0.05f;

		public float aperture = 11.5f;

		public Transform focalTransform;

		public float maxBlurSize = 2f;

		public bool highResolution;

		public DepthOfField.BlurType blurType;

		public DepthOfField.BlurSampleCount blurSampleCount = DepthOfField.BlurSampleCount.High;

		public bool nearBlur;

		public float foregroundOverlap = 1f;

		public Shader dofHdrShader;

		private Material dofHdrMaterial;

		public Shader dx11BokehShader;

		private Material dx11bokehMaterial;

		public float dx11BokehThreshold = 0.5f;

		public float dx11SpawnHeuristic = 0.0875f;

		public Texture2D dx11BokehTexture;

		public float dx11BokehScale = 1.2f;

		public float dx11BokehIntensity = 2.5f;

		private float focalDistance01 = 10f;

		private ComputeBuffer cbDrawArgs;

		private ComputeBuffer cbPoints;

		private float internalBlurWidth = 1f;

		public override bool CheckResources()
		{
			base.CheckSupport(true);
			this.dofHdrMaterial = base.CheckShaderAndCreateMaterial(this.dofHdrShader, this.dofHdrMaterial);
			if (this.supportDX11 && this.blurType == DepthOfField.BlurType.DX11)
			{
				this.dx11bokehMaterial = base.CheckShaderAndCreateMaterial(this.dx11BokehShader, this.dx11bokehMaterial);
				this.CreateComputeResources();
			}
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		private void OnEnable()
		{
			base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}

		private void OnDisable()
		{
			this.ReleaseComputeResources();
			if (this.dofHdrMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.dofHdrMaterial);
			}
			this.dofHdrMaterial = null;
			if (this.dx11bokehMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.dx11bokehMaterial);
			}
			this.dx11bokehMaterial = null;
		}

		private void ReleaseComputeResources()
		{
			if (this.cbDrawArgs != null)
			{
				this.cbDrawArgs.Release();
			}
			this.cbDrawArgs = null;
			if (this.cbPoints != null)
			{
				this.cbPoints.Release();
			}
			this.cbPoints = null;
		}

		private void CreateComputeResources()
		{
			if (this.cbDrawArgs == null)
			{
				this.cbDrawArgs = new ComputeBuffer(1, 16, ComputeBufferType.DrawIndirect);
				int[] data = new int[]
				{
					0,
					1,
					0,
					0
				};
				this.cbDrawArgs.SetData(data);
			}
			if (this.cbPoints == null)
			{
				this.cbPoints = new ComputeBuffer(90000, 28, ComputeBufferType.Append);
			}
		}

		private float FocalDistance01(float worldDist)
		{
			return base.GetComponent<Camera>().WorldToViewportPoint((worldDist - base.GetComponent<Camera>().nearClipPlane) * base.GetComponent<Camera>().transform.forward + base.GetComponent<Camera>().transform.position).z / (base.GetComponent<Camera>().farClipPlane - base.GetComponent<Camera>().nearClipPlane);
		}

		private void WriteCoc(RenderTexture fromTo, bool fgDilate)
		{
			this.dofHdrMaterial.SetTexture("_FgOverlap", null);
			if (this.nearBlur && fgDilate)
			{
				int width = fromTo.width / 2;
				int height = fromTo.height / 2;
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, fromTo.format);
				Graphics.Blit(fromTo, temporary, this.dofHdrMaterial, 4);
				float num = this.internalBlurWidth * this.foregroundOverlap;
				this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, num, 0f, num));
				RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, fromTo.format);
				Graphics.Blit(temporary, temporary2, this.dofHdrMaterial, 2);
				RenderTexture.ReleaseTemporary(temporary);
				this.dofHdrMaterial.SetVector("_Offsets", new Vector4(num, 0f, 0f, num));
				temporary = RenderTexture.GetTemporary(width, height, 0, fromTo.format);
				Graphics.Blit(temporary2, temporary, this.dofHdrMaterial, 2);
				RenderTexture.ReleaseTemporary(temporary2);
				this.dofHdrMaterial.SetTexture("_FgOverlap", temporary);
				fromTo.MarkRestoreExpected();
				Graphics.Blit(fromTo, fromTo, this.dofHdrMaterial, 13);
				RenderTexture.ReleaseTemporary(temporary);
			}
			else
			{
				fromTo.MarkRestoreExpected();
				Graphics.Blit(fromTo, fromTo, this.dofHdrMaterial, 0);
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (this.aperture < 0f)
			{
				this.aperture = 0f;
			}
			if (this.maxBlurSize < 0.1f)
			{
				this.maxBlurSize = 0.1f;
			}
			this.focalSize = Mathf.Clamp(this.focalSize, 0f, 2f);
			this.internalBlurWidth = Mathf.Max(this.maxBlurSize, 0f);
			this.focalDistance01 = ((!this.focalTransform) ? this.FocalDistance01(this.focalLength) : (base.GetComponent<Camera>().WorldToViewportPoint(this.focalTransform.position).z / base.GetComponent<Camera>().farClipPlane));
			this.dofHdrMaterial.SetVector("_CurveParams", new Vector4(1f, this.focalSize, this.aperture / 10f, this.focalDistance01));
			RenderTexture renderTexture = null;
			RenderTexture renderTexture2 = null;
			float num = this.internalBlurWidth * this.foregroundOverlap;
			if (this.visualizeFocus)
			{
				this.WriteCoc(source, true);
				Graphics.Blit(source, destination, this.dofHdrMaterial, 16);
			}
			else if (this.blurType == DepthOfField.BlurType.DX11 && this.dx11bokehMaterial)
			{
				if (this.highResolution)
				{
					this.internalBlurWidth = ((this.internalBlurWidth >= 0.1f) ? this.internalBlurWidth : 0.1f);
					num = this.internalBlurWidth * this.foregroundOverlap;
					renderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
					RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
					this.WriteCoc(source, false);
					RenderTexture temporary2 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
					RenderTexture temporary3 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
					Graphics.Blit(source, temporary2, this.dofHdrMaterial, 15);
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, 1.5f, 0f, 1.5f));
					Graphics.Blit(temporary2, temporary3, this.dofHdrMaterial, 19);
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(1.5f, 0f, 0f, 1.5f));
					Graphics.Blit(temporary3, temporary2, this.dofHdrMaterial, 19);
					if (this.nearBlur)
					{
						Graphics.Blit(source, temporary3, this.dofHdrMaterial, 4);
					}
					this.dx11bokehMaterial.SetTexture("_BlurredColor", temporary2);
					this.dx11bokehMaterial.SetFloat("_SpawnHeuristic", this.dx11SpawnHeuristic);
					this.dx11bokehMaterial.SetVector("_BokehParams", new Vector4(this.dx11BokehScale, this.dx11BokehIntensity, Mathf.Clamp(this.dx11BokehThreshold, 0.005f, 4f), this.internalBlurWidth));
					this.dx11bokehMaterial.SetTexture("_FgCocMask", (!this.nearBlur) ? null : temporary3);
					Graphics.SetRandomWriteTarget(1, this.cbPoints);
					Graphics.Blit(source, renderTexture, this.dx11bokehMaterial, 0);
					Graphics.ClearRandomWriteTargets();
					if (this.nearBlur)
					{
						this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, num, 0f, num));
						Graphics.Blit(temporary3, temporary2, this.dofHdrMaterial, 2);
						this.dofHdrMaterial.SetVector("_Offsets", new Vector4(num, 0f, 0f, num));
						Graphics.Blit(temporary2, temporary3, this.dofHdrMaterial, 2);
						Graphics.Blit(temporary3, renderTexture, this.dofHdrMaterial, 3);
					}
					Graphics.Blit(renderTexture, temporary, this.dofHdrMaterial, 20);
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(this.internalBlurWidth, 0f, 0f, this.internalBlurWidth));
					Graphics.Blit(renderTexture, source, this.dofHdrMaterial, 5);
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, this.internalBlurWidth, 0f, this.internalBlurWidth));
					Graphics.Blit(source, temporary, this.dofHdrMaterial, 21);
					Graphics.SetRenderTarget(temporary);
					ComputeBuffer.CopyCount(this.cbPoints, this.cbDrawArgs, 0);
					this.dx11bokehMaterial.SetBuffer("pointBuffer", this.cbPoints);
					this.dx11bokehMaterial.SetTexture("_MainTex", this.dx11BokehTexture);
					this.dx11bokehMaterial.SetVector("_Screen", new Vector3(1f / (1f * (float)source.width), 1f / (1f * (float)source.height), this.internalBlurWidth));
					this.dx11bokehMaterial.SetPass(2);
					Graphics.DrawProceduralIndirect(MeshTopology.Points, this.cbDrawArgs, 0);
					Graphics.Blit(temporary, destination);
					RenderTexture.ReleaseTemporary(temporary);
					RenderTexture.ReleaseTemporary(temporary2);
					RenderTexture.ReleaseTemporary(temporary3);
				}
				else
				{
					renderTexture = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
					renderTexture2 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
					num = this.internalBlurWidth * this.foregroundOverlap;
					this.WriteCoc(source, false);
					source.filterMode = FilterMode.Bilinear;
					Graphics.Blit(source, renderTexture, this.dofHdrMaterial, 6);
					RenderTexture temporary2 = RenderTexture.GetTemporary(renderTexture.width >> 1, renderTexture.height >> 1, 0, renderTexture.format);
					RenderTexture temporary3 = RenderTexture.GetTemporary(renderTexture.width >> 1, renderTexture.height >> 1, 0, renderTexture.format);
					Graphics.Blit(renderTexture, temporary2, this.dofHdrMaterial, 15);
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, 1.5f, 0f, 1.5f));
					Graphics.Blit(temporary2, temporary3, this.dofHdrMaterial, 19);
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(1.5f, 0f, 0f, 1.5f));
					Graphics.Blit(temporary3, temporary2, this.dofHdrMaterial, 19);
					RenderTexture renderTexture3 = null;
					if (this.nearBlur)
					{
						renderTexture3 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
						Graphics.Blit(source, renderTexture3, this.dofHdrMaterial, 4);
					}
					this.dx11bokehMaterial.SetTexture("_BlurredColor", temporary2);
					this.dx11bokehMaterial.SetFloat("_SpawnHeuristic", this.dx11SpawnHeuristic);
					this.dx11bokehMaterial.SetVector("_BokehParams", new Vector4(this.dx11BokehScale, this.dx11BokehIntensity, Mathf.Clamp(this.dx11BokehThreshold, 0.005f, 4f), this.internalBlurWidth));
					this.dx11bokehMaterial.SetTexture("_FgCocMask", renderTexture3);
					Graphics.SetRandomWriteTarget(1, this.cbPoints);
					Graphics.Blit(renderTexture, renderTexture2, this.dx11bokehMaterial, 0);
					Graphics.ClearRandomWriteTargets();
					RenderTexture.ReleaseTemporary(temporary2);
					RenderTexture.ReleaseTemporary(temporary3);
					if (this.nearBlur)
					{
						this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, num, 0f, num));
						Graphics.Blit(renderTexture3, renderTexture, this.dofHdrMaterial, 2);
						this.dofHdrMaterial.SetVector("_Offsets", new Vector4(num, 0f, 0f, num));
						Graphics.Blit(renderTexture, renderTexture3, this.dofHdrMaterial, 2);
						Graphics.Blit(renderTexture3, renderTexture2, this.dofHdrMaterial, 3);
					}
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(this.internalBlurWidth, 0f, 0f, this.internalBlurWidth));
					Graphics.Blit(renderTexture2, renderTexture, this.dofHdrMaterial, 5);
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, this.internalBlurWidth, 0f, this.internalBlurWidth));
					Graphics.Blit(renderTexture, renderTexture2, this.dofHdrMaterial, 5);
					Graphics.SetRenderTarget(renderTexture2);
					ComputeBuffer.CopyCount(this.cbPoints, this.cbDrawArgs, 0);
					this.dx11bokehMaterial.SetBuffer("pointBuffer", this.cbPoints);
					this.dx11bokehMaterial.SetTexture("_MainTex", this.dx11BokehTexture);
					this.dx11bokehMaterial.SetVector("_Screen", new Vector3(1f / (1f * (float)renderTexture2.width), 1f / (1f * (float)renderTexture2.height), this.internalBlurWidth));
					this.dx11bokehMaterial.SetPass(1);
					Graphics.DrawProceduralIndirect(MeshTopology.Points, this.cbDrawArgs, 0);
					this.dofHdrMaterial.SetTexture("_LowRez", renderTexture2);
					this.dofHdrMaterial.SetTexture("_FgOverlap", renderTexture3);
					this.dofHdrMaterial.SetVector("_Offsets", 1f * (float)source.width / (1f * (float)renderTexture2.width) * this.internalBlurWidth * Vector4.one);
					Graphics.Blit(source, destination, this.dofHdrMaterial, 9);
					if (renderTexture3)
					{
						RenderTexture.ReleaseTemporary(renderTexture3);
					}
				}
			}
			else
			{
				source.filterMode = FilterMode.Bilinear;
				if (this.highResolution)
				{
					this.internalBlurWidth *= 2f;
				}
				this.WriteCoc(source, true);
				renderTexture = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
				renderTexture2 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
				int pass = (this.blurSampleCount != DepthOfField.BlurSampleCount.High && this.blurSampleCount != DepthOfField.BlurSampleCount.Medium) ? 11 : 17;
				if (this.highResolution)
				{
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, this.internalBlurWidth, 0.025f, this.internalBlurWidth));
					Graphics.Blit(source, destination, this.dofHdrMaterial, pass);
				}
				else
				{
					this.dofHdrMaterial.SetVector("_Offsets", new Vector4(0f, this.internalBlurWidth, 0.1f, this.internalBlurWidth));
					Graphics.Blit(source, renderTexture, this.dofHdrMaterial, 6);
					Graphics.Blit(renderTexture, renderTexture2, this.dofHdrMaterial, pass);
					this.dofHdrMaterial.SetTexture("_LowRez", renderTexture2);
					this.dofHdrMaterial.SetTexture("_FgOverlap", null);
					this.dofHdrMaterial.SetVector("_Offsets", Vector4.one * (1f * (float)source.width / (1f * (float)renderTexture2.width)) * this.internalBlurWidth);
					Graphics.Blit(source, destination, this.dofHdrMaterial, (this.blurSampleCount != DepthOfField.BlurSampleCount.High) ? 12 : 18);
				}
			}
			if (renderTexture)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			if (renderTexture2)
			{
				RenderTexture.ReleaseTemporary(renderTexture2);
			}
		}
	}
}
