using System;
using UnityEngine;

namespace ScionEngine
{
	public class CombinationPass
	{
		private const float MinValue = 0.0001f;

		private Material m_combinationMat;

		public CombinationPass()
		{
			this.m_combinationMat = new Material(Shader.Find("Hidden/ScionCombinationPass"));
			this.m_combinationMat.hideFlags = HideFlags.HideAndDontSave;
		}

		public void ReleaseResources()
		{
			if (this.m_combinationMat != null)
			{
				UnityEngine.Object.Destroy(this.m_combinationMat);
				this.m_combinationMat = null;
			}
		}

		public bool PlatformCompatibility()
		{
			return Shader.Find("Hidden/ScionCombinationPass").isSupported;
		}

		private void PrepareBloomSampling(RenderTexture bloomTexture, BloomParameters bloomParams)
		{
			this.m_combinationMat.SetTexture("_BloomTexture", bloomTexture);
			Vector4 vector = default(Vector4);
			vector.x = ((bloomParams.intensity <= 0.0001f) ? 0.0001f : bloomParams.intensity);
			vector.y = bloomParams.brightness;
			this.m_combinationMat.SetVector("_BloomParameters", vector);
		}

		private void PrepareLensDirtSampling(Texture lensDirtTexture, LensDirtParameters lensDirtParams)
		{
			this.m_combinationMat.SetTexture("_LensDirtTexture", lensDirtTexture);
			Vector4 vector = default(Vector4);
			vector.x = ((lensDirtParams.intensity <= 0.0001f) ? 0.0001f : lensDirtParams.intensity);
			vector.y = lensDirtParams.brightness;
			this.m_combinationMat.SetVector("_LensDirtParameters", vector);
		}

		private void PrepareExposure(CameraParameters cameraParams, VirtualCamera virtualCamera)
		{
			if (cameraParams.cameraMode == CameraMode.Off)
			{
				this.m_combinationMat.SetFloat("_ManualExposure", 1f);
			}
			else if (cameraParams.cameraMode != CameraMode.Manual)
			{
				virtualCamera.BindExposureTexture(this.m_combinationMat);
			}
			else
			{
				this.m_combinationMat.SetFloat("_ManualExposure", virtualCamera.CalculateManualExposure(cameraParams, 0.18f));
			}
		}

		private void UploadVariables(CommonPostProcess commonPostProcess)
		{
			Vector4 vector = default(Vector4);
			vector.x = commonPostProcess.grainIntensity;
			vector.y = commonPostProcess.vignetteIntensity;
			vector.z = commonPostProcess.vignetteScale;
			vector.w = commonPostProcess.chromaticAberrationDistortion;
			this.m_combinationMat.SetVector("_PostProcessParams1", vector);
			Vector4 vector2 = default(Vector4);
			vector2.x = commonPostProcess.vignetteColor.r;
			vector2.y = commonPostProcess.vignetteColor.g;
			vector2.z = commonPostProcess.vignetteColor.b;
			vector2.w = commonPostProcess.chromaticAberrationIntensity;
			this.m_combinationMat.SetVector("_PostProcessParams2", vector2);
			Vector4 vector3 = default(Vector4);
			vector3.x = UnityEngine.Random.value;
			vector3.y = 5f / commonPostProcess.whitePoint;
			vector3.z = 1f / commonPostProcess.whitePoint;
			this.m_combinationMat.SetVector("_PostProcessParams3", vector3);
		}

		private void PrepareColorGrading(ColorGradingParameters colorGradingParams)
		{
			if (colorGradingParams.colorGradingMode == ColorGradingMode.Off)
			{
				return;
			}
			this.m_combinationMat.SetTexture("_ColorGradingLUT1", colorGradingParams.colorGradingTex1);
			Vector2 vector = default(Vector2);
			float num = 32f;
			vector.x = 1024f;
			vector.y = 32f;
			float num2 = 1f / num;
			Vector4 vector2 = default(Vector4);
			vector2.x = 1f * num2 - 1f / vector.x;
			vector2.y = 1f - 1f * num2;
			vector2.z = num - 1f;
			vector2.w = num;
			Vector4 vector3 = default(Vector4);
			vector3.x = 0.5f / vector.x;
			vector3.y = 0.5f / vector.y;
			vector3.z = 0f;
			vector3.w = num2;
			this.m_combinationMat.SetVector("_ColorGradingParams1", vector2);
			this.m_combinationMat.SetVector("_ColorGradingParams2", vector3);
		}

		public void Combine(RenderTexture source, RenderTexture dest, PostProcessParameters postProcessParams, VirtualCamera virtualCamera)
		{
			if (postProcessParams.bloom)
			{
				this.PrepareBloomSampling(postProcessParams.bloomTexture, postProcessParams.bloomParams);
			}
			if (postProcessParams.lensDirt)
			{
				this.PrepareLensDirtSampling(postProcessParams.lensDirtTexture, postProcessParams.lensDirtParams);
			}
			this.PrepareExposure(postProcessParams.cameraParams, virtualCamera);
			this.PrepareColorGrading(postProcessParams.colorGradingParams);
			this.UploadVariables(postProcessParams.commonPostProcess);
			int num = 0;
			if (!postProcessParams.tonemapping)
			{
				num += 3;
			}
			if (!postProcessParams.bloom)
			{
				num += 2;
			}
			else if (!postProcessParams.lensDirt)
			{
				num++;
			}
			source.filterMode = FilterMode.Bilinear;
			source.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(source, dest, this.m_combinationMat, num);
		}
	}
}
