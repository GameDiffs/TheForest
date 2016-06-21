using System;
using UnityEngine;

namespace ScionEngine
{
	public class DepthOfField
	{
		private Material m_DoFMat;

		private Material m_DoFMatDX11;

		private RenderTexture previousPointAverage;

		public DepthOfField()
		{
			this.m_DoFMat = new Material(Shader.Find("Hidden/ScionDepthOfField"));
			this.m_DoFMat.hideFlags = HideFlags.HideAndDontSave;
			this.CreateDX11Mat();
		}

		private void CreateDX11Mat()
		{
			if (SystemInfo.graphicsShaderLevel >= 40 && this.m_DoFMatDX11 == null)
			{
				this.m_DoFMatDX11 = new Material(Shader.Find("Hidden/ScionDepthOfFieldDX11"));
				this.m_DoFMatDX11.hideFlags = HideFlags.HideAndDontSave;
			}
		}

		public bool PlatformCompatibility()
		{
			if (!Shader.Find("Hidden/ScionDepthOfField").isSupported)
			{
				Debug.LogWarning("Depth of Field shader not supported");
				return false;
			}
			if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB2101010))
			{
				Debug.LogWarning("ARGB2101010 texture format not supposed");
				return false;
			}
			return true;
		}

		public void EndOfFrameCleanup()
		{
		}

		private float Min(float val1, float val2)
		{
			return (val1 <= val2) ? val1 : val2;
		}

		private float Max(float val1, float val2)
		{
			return (val1 >= val2) ? val1 : val2;
		}

		private int Min(int val1, int val2)
		{
			return (val1 <= val2) ? val1 : val2;
		}

		private int Max(int val1, int val2)
		{
			return (val1 >= val2) ? val1 : val2;
		}

		private RenderTexture PrepatePointAverage(PostProcessParameters postProcessParams, RenderTexture dest)
		{
			float num = this.Max(10f / (float)postProcessParams.halfResDepth.width, postProcessParams.DoFParams.pointAverageRange);
			Vector4 vector = default(Vector4);
			vector.x = Mathf.Clamp01(postProcessParams.DoFParams.pointAveragePosition.x);
			vector.y = Mathf.Clamp01(postProcessParams.DoFParams.pointAveragePosition.y);
			vector.z = num * num;
			vector.w = 1f / (num * num);
			this.m_DoFMat.SetVector("_DownsampleWeightedParams", vector);
			if (this.previousPointAverage != null && !postProcessParams.isFirstRender)
			{
				this.m_DoFMat.SetFloat("_DownsampleWeightedAdaptionSpeed", 1f - Mathf.Exp(-Time.deltaTime * postProcessParams.DoFParams.depthAdaptionSpeed));
				this.m_DoFMat.SetTexture("_PreviousWeightedResult", this.previousPointAverage);
			}
			else
			{
				this.m_DoFMat.SetFloat("_DownsampleWeightedAdaptionSpeed", 1f);
				this.m_DoFMat.SetTexture("_PreviousWeightedResult", null);
			}
			postProcessParams.halfResDepth.filterMode = FilterMode.Bilinear;
			int num2 = this.Max(postProcessParams.halfWidth / 2, 1);
			int num3 = this.Max(postProcessParams.halfHeight / 2, 1);
			RenderTexture temporary = RenderTexture.GetTemporary(num2, num3, 0, RenderTextureFormat.RGHalf);
			temporary.filterMode = FilterMode.Bilinear;
			temporary.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(postProcessParams.halfResDepth, temporary, this.m_DoFMat, 7);
			if (postProcessParams.DoFParams.visualizePointFocus)
			{
				RenderTexture temporary2 = RenderTexture.GetTemporary(num2, num3, 0, RenderTextureFormat.ARGB32);
				Graphics.Blit(temporary, temporary2, this.m_DoFMat, 9);
				ScionPostProcess.ActiveDebug.RegisterTextureForVisualization(temporary2, true, true, false);
			}
			RenderTexture renderTexture = temporary;
			int i = this.Max(num2, num3);
			while (i > 1)
			{
				num2 = this.Max(1, num2 / 2 + num2 % 2);
				num3 = this.Max(1, num3 / 2 + num3 % 2);
				i = i / 2 + i % 2;
				RenderTexture temporary3;
				if (i > 1)
				{
					temporary3 = RenderTexture.GetTemporary(num2, num3, 0, RenderTextureFormat.RGHalf);
					temporary3.filterMode = FilterMode.Bilinear;
					temporary3.wrapMode = TextureWrapMode.Clamp;
					Graphics.Blit(renderTexture, temporary3, this.m_DoFMat, 10);
				}
				else
				{
					temporary3 = RenderTexture.GetTemporary(num2, num3, 0, RenderTextureFormat.RHalf);
					temporary3.filterMode = FilterMode.Bilinear;
					temporary3.wrapMode = TextureWrapMode.Clamp;
					Graphics.Blit(renderTexture, temporary3, this.m_DoFMat, 8);
				}
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary3;
			}
			RenderTexture result = renderTexture;
			if (this.previousPointAverage != null)
			{
				RenderTexture.ReleaseTemporary(this.previousPointAverage);
			}
			this.previousPointAverage = result;
			return result;
		}

		public RenderTexture RenderDepthOfField(PostProcessParameters postProcessParams, RenderTexture source, RenderTexture dest, VirtualCamera virtualCamera)
		{
			this.CreateDX11Mat();
			virtualCamera.BindVirtualCameraTextures(this.m_DoFMat);
			if (postProcessParams.DoFParams.quality == DepthOfFieldQuality.High_DX11)
			{
				virtualCamera.BindVirtualCameraTextures(this.m_DoFMatDX11);
			}
			virtualCamera.BindVirtualCameraParams(this.m_DoFMat, postProcessParams.cameraParams, postProcessParams.DoFParams.focalDistance, (float)postProcessParams.halfWidth, postProcessParams.isFirstRender);
			if (postProcessParams.DoFParams.quality == DepthOfFieldQuality.High_DX11)
			{
				virtualCamera.BindVirtualCameraParams(this.m_DoFMatDX11, postProcessParams.cameraParams, postProcessParams.DoFParams.focalDistance, (float)postProcessParams.halfWidth, postProcessParams.isFirstRender);
			}
			RenderTexture depthCenterAverage = null;
			if (postProcessParams.DoFParams.depthFocusMode == DepthFocusMode.PointAverage)
			{
				depthCenterAverage = this.PrepatePointAverage(postProcessParams, dest);
			}
			RenderTexture renderTexture = this.CreateTiledData(postProcessParams.halfResDepth, postProcessParams.preCalcValues.tanHalfFoV, postProcessParams.cameraParams.fNumber, postProcessParams.DoFParams.focalDistance, postProcessParams.DoFParams.focalRange, postProcessParams.cameraParams.apertureDiameter, postProcessParams.cameraParams.focalLength, postProcessParams.DoFParams.maxCoCRadius, postProcessParams.cameraParams.nearPlane, postProcessParams.cameraParams.farPlane);
			RenderTexture renderTexture2 = this.TileNeighbourhoodDataGathering(renderTexture);
			RenderTexture renderTexture3 = this.PrefilterSource(postProcessParams.halfResSource);
			RenderTexture renderTexture4 = this.BlurTapPass(renderTexture3, renderTexture, renderTexture2, depthCenterAverage, postProcessParams.DoFParams.quality);
			if (postProcessParams.DoFParams.useMedianFilter)
			{
				renderTexture4 = this.MedianFilterPass(renderTexture4);
			}
			RenderTexture result = this.UpsampleDepthOfField(source, renderTexture4, renderTexture2);
			RenderTexture.ReleaseTemporary(renderTexture);
			RenderTexture.ReleaseTemporary(renderTexture2);
			RenderTexture.ReleaseTemporary(renderTexture3);
			RenderTexture.ReleaseTemporary(renderTexture4);
			return result;
		}

		private RenderTexture CreateTiledData(RenderTexture halfResDepth, float tanHalfFoV, float fNumber, float focalDistance, float focalRange, float apertureDiameter, float focalLength, float maxCoCRadius, float nearPlane, float farPlane)
		{
			int width = halfResDepth.width / 10 + ((halfResDepth.width % 10 != 0) ? 1 : 0);
			int height = halfResDepth.height / 10 + ((halfResDepth.height % 10 != 0) ? 1 : 0);
			float num = apertureDiameter * focalLength * focalDistance / (focalDistance - focalLength);
			float num2 = -apertureDiameter * focalLength / (focalDistance - focalLength);
			float num3 = ScionUtility.CoCToPixels((float)halfResDepth.width);
			num *= num3;
			num2 *= num3;
			Vector4 vector = default(Vector4);
			vector.x = num;
			vector.y = num2;
			vector.z = focalDistance;
			vector.w = focalRange * 0.5f;
			this.m_DoFMat.SetVector("_CoCParams1", vector);
			Vector4 vector2 = default(Vector4);
			vector2.x = maxCoCRadius * 0.5f;
			vector2.y = 1f / maxCoCRadius;
			this.m_DoFMat.SetVector("_CoCParams2", vector2);
			if (this.m_DoFMatDX11 != null)
			{
				this.m_DoFMatDX11.SetVector("_CoCParams1", vector);
				this.m_DoFMatDX11.SetVector("_CoCParams2", vector2);
			}
			this.m_DoFMat.SetFloat("_CoCUVOffset", 1f / (float)halfResDepth.width);
			RenderTexture temporary = RenderTexture.GetTemporary(width, halfResDepth.height, 0, RenderTextureFormat.RHalf);
			temporary.filterMode = FilterMode.Point;
			temporary.wrapMode = TextureWrapMode.Clamp;
			RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.RHalf);
			temporary2.filterMode = FilterMode.Point;
			temporary2.wrapMode = TextureWrapMode.Clamp;
			halfResDepth.filterMode = FilterMode.Point;
			ScionGraphics.Blit(temporary, this.m_DoFMat, 0);
			this.m_DoFMat.SetTexture("_HorizontalTileResult", temporary);
			this.m_DoFMat.SetFloat("_CoCUVOffset", 1f / (float)halfResDepth.height);
			ScionGraphics.Blit(temporary2, this.m_DoFMat, 1);
			RenderTexture.ReleaseTemporary(temporary);
			return temporary2;
		}

		private RenderTexture TileNeighbourhoodDataGathering(RenderTexture tiledData)
		{
			Vector4 vector = default(Vector4);
			vector.x = 1f / (float)tiledData.width;
			vector.y = 1f / (float)tiledData.height;
			this.m_DoFMat.SetVector("_NeighbourhoodParams", vector);
			RenderTexture temporary = RenderTexture.GetTemporary(tiledData.width, tiledData.height, 0, RenderTextureFormat.RHalf);
			temporary.filterMode = FilterMode.Point;
			temporary.wrapMode = TextureWrapMode.Clamp;
			this.m_DoFMat.SetTexture("_TiledData", tiledData);
			ScionGraphics.Blit(temporary, this.m_DoFMat, 2);
			return temporary;
		}

		private RenderTexture PrefilterSource(RenderTexture halfResSource)
		{
			this.m_DoFMat.SetTexture("_HalfResSourceTexture", halfResSource);
			halfResSource.filterMode = FilterMode.Point;
			RenderTexture temporary = RenderTexture.GetTemporary(halfResSource.width, halfResSource.height, 0, halfResSource.format);
			temporary.filterMode = FilterMode.Point;
			temporary.wrapMode = TextureWrapMode.Clamp;
			ScionGraphics.Blit(temporary, this.m_DoFMat, 4);
			return temporary;
		}

		private RenderTexture BlurTapPass(RenderTexture halfResSource, RenderTexture tiledData, RenderTexture neighbourhoodData, RenderTexture depthCenterAverage, DepthOfFieldQuality qualityLevel)
		{
			Material material = (qualityLevel != DepthOfFieldQuality.Normal) ? this.m_DoFMatDX11 : this.m_DoFMat;
			material.SetTexture("_TiledData", tiledData);
			material.SetTexture("_TiledNeighbourhoodData", neighbourhoodData);
			material.SetTexture("_HalfResSourceTexture", halfResSource);
			if (depthCenterAverage != null)
			{
				material.SetTexture("_AvgCenterDepth", depthCenterAverage);
			}
			halfResSource.filterMode = FilterMode.Point;
			RenderTexture temporary = RenderTexture.GetTemporary(halfResSource.width, halfResSource.height, 0, halfResSource.format);
			temporary.filterMode = FilterMode.Point;
			temporary.wrapMode = TextureWrapMode.Clamp;
			if (qualityLevel == DepthOfFieldQuality.Normal)
			{
				ScionGraphics.Blit(temporary, this.m_DoFMat, 5);
			}
			else if (qualityLevel == DepthOfFieldQuality.High_DX11)
			{
				ScionGraphics.Blit(temporary, this.m_DoFMatDX11, 0);
			}
			return temporary;
		}

		private RenderTexture MedianFilterPass(RenderTexture inputTexture)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(inputTexture.width, inputTexture.height, 0, inputTexture.format);
			temporary.filterMode = FilterMode.Point;
			temporary.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(inputTexture, temporary, this.m_DoFMat, 3);
			RenderTexture.ReleaseTemporary(inputTexture);
			return temporary;
		}

		private RenderTexture UpsampleDepthOfField(RenderTexture source, RenderTexture depthOfFieldTexture, RenderTexture neighbourhoodData)
		{
			this.m_DoFMat.SetTexture("_DepthOfFieldTexture", depthOfFieldTexture);
			this.m_DoFMat.SetTexture("_FullResolutionSource", source);
			this.m_DoFMat.SetTexture("_TiledNeighbourhoodData", neighbourhoodData);
			neighbourhoodData.filterMode = FilterMode.Bilinear;
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
			source.filterMode = FilterMode.Point;
			source.wrapMode = TextureWrapMode.Clamp;
			ScionGraphics.Blit(temporary, this.m_DoFMat, 6);
			return temporary;
		}
	}
}
