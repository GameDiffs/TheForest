using System;
using UnityEngine;

namespace ScionEngine
{
	public class PostProcessParameters
	{
		public bool tonemapping;

		public bool bloom;

		public bool lensDirt;

		public bool exposure;

		public bool depthOfField;

		public bool isFirstRender;

		public int width;

		public int height;

		public int halfWidth;

		public int halfHeight;

		public RenderTexture halfResSource;

		public RenderTexture halfResDepth;

		public RenderTexture bloomTexture;

		public RenderTexture dofTexture;

		public Texture lensDirtTexture;

		public BloomParameters bloomParams;

		public LensDirtParameters lensDirtParams;

		public CameraParameters cameraParams;

		public DepthOfFieldParameters DoFParams;

		public ColorGradingParameters colorGradingParams;

		public PreCalcValues preCalcValues;

		public CommonPostProcess commonPostProcess;

		public PostProcessParameters()
		{
			this.bloomParams = default(BloomParameters);
			this.lensDirtParams = default(LensDirtParameters);
			this.cameraParams = default(CameraParameters);
			this.DoFParams = default(DepthOfFieldParameters);
			this.colorGradingParams = default(ColorGradingParameters);
			this.preCalcValues = default(PreCalcValues);
			this.commonPostProcess = default(CommonPostProcess);
		}

		public void Fill(ScionPostProcess postProcess)
		{
			this.tonemapping = postProcess.tonemapping;
			this.bloom = postProcess.bloom;
			this.lensDirt = (postProcess.lensDirt && postProcess.lensDirtTexture != null);
			this.lensDirtTexture = postProcess.lensDirtTexture;
			this.bloomTexture = null;
			this.dofTexture = null;
			this.exposure = (postProcess.cameraMode != CameraMode.Off);
			this.depthOfField = postProcess.depthOfField;
			this.halfResSource = null;
			this.bloomParams.intensity = ScionUtility.Square(postProcess.bloomIntensity);
			this.bloomParams.brightness = postProcess.bloomBrightness;
			this.bloomParams.downsamples = postProcess.bloomDownsamples;
			this.lensDirtParams.intensity = ScionUtility.Square(postProcess.lensDirtIntensity);
			this.lensDirtParams.brightness = postProcess.lensDirtBrightness;
			this.DoFParams.depthFocusMode = postProcess.depthFocusMode;
			this.DoFParams.maxCoCRadius = postProcess.maxCoCRadius;
			this.DoFParams.quality = ((SystemInfo.graphicsShaderLevel >= 40) ? postProcess.depthOfFieldQuality : DepthOfFieldQuality.Normal);
			this.DoFParams.pointAveragePosition = postProcess.pointAveragePosition;
			this.DoFParams.pointAverageRange = postProcess.pointAverageRange;
			this.DoFParams.visualizePointFocus = postProcess.visualizePointFocus;
			this.DoFParams.depthAdaptionSpeed = postProcess.depthAdaptionSpeed;
			this.DoFParams.focalDistance = postProcess.focalDistance;
			this.DoFParams.focalRange = postProcess.focalRange;
			this.colorGradingParams.colorGradingMode = ((!(postProcess.colorGradingTex1 == null)) ? postProcess.colorGradingMode : ColorGradingMode.Off);
			this.colorGradingParams.colorGradingTex1 = postProcess.colorGradingTex1;
			this.colorGradingParams.colorGradingTex2 = postProcess.colorGradingTex2;
			this.colorGradingParams.colorGradingBlendFactor = postProcess.colorGradingBlendFactor;
			this.colorGradingParams.colorGradingCompatibility = postProcess.colorGradingCompatibility;
			this.cameraParams.cameraMode = postProcess.cameraMode;
			this.cameraParams.fNumber = postProcess.fNumber;
			this.cameraParams.ISO = postProcess.ISO;
			this.cameraParams.shutterSpeed = postProcess.shutterSpeed;
			this.cameraParams.adaptionSpeed = postProcess.adaptionSpeed;
			this.cameraParams.minMaxExposure = postProcess.minMaxExposure;
			this.cameraParams.exposureCompensation = postProcess.exposureCompensation;
			this.commonPostProcess.grainIntensity = ((!postProcess.grain) ? 0f : postProcess.grainIntensity);
			this.commonPostProcess.vignetteIntensity = ((!postProcess.vignette) ? 0f : postProcess.vignetteIntensity);
			this.commonPostProcess.vignetteScale = postProcess.vignetteScale;
			this.commonPostProcess.vignetteColor = postProcess.vignetteColor;
			this.commonPostProcess.chromaticAberration = postProcess.chromaticAberration;
			this.commonPostProcess.chromaticAberrationDistortion = postProcess.chromaticAberrationDistortion;
			this.commonPostProcess.chromaticAberrationIntensity = postProcess.chromaticAberrationIntensity;
			this.commonPostProcess.whitePoint = postProcess.whitePoint;
		}
	}
}
