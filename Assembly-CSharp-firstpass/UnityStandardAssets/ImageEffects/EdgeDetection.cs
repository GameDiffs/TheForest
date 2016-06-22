using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Edge Detection/Edge Detection"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class EdgeDetection : PostEffectsBase
	{
		public enum EdgeDetectMode
		{
			TriangleDepthNormals,
			RobertsCrossDepthNormals,
			SobelDepth,
			SobelDepthThin,
			TriangleLuminance
		}

		public EdgeDetection.EdgeDetectMode mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;

		public float sensitivityDepth = 1f;

		public float sensitivityNormals = 1f;

		public float lumThreshold = 0.2f;

		public float edgeExp = 1f;

		public float sampleDist = 1f;

		public float edgesOnly;

		public Color edgesOnlyBgColor = Color.white;

		public Shader edgeDetectShader;

		private Material edgeDetectMaterial;

		private EdgeDetection.EdgeDetectMode oldMode = EdgeDetection.EdgeDetectMode.SobelDepthThin;

		public override bool CheckResources()
		{
			base.CheckSupport(true);
			this.edgeDetectMaterial = base.CheckShaderAndCreateMaterial(this.edgeDetectShader, this.edgeDetectMaterial);
			if (this.mode != this.oldMode)
			{
				this.SetCameraFlag();
			}
			this.oldMode = this.mode;
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		private new void Start()
		{
			this.oldMode = this.mode;
		}

		private void SetCameraFlag()
		{
			if (this.mode == EdgeDetection.EdgeDetectMode.SobelDepth || this.mode == EdgeDetection.EdgeDetectMode.SobelDepthThin)
			{
				base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
			}
			else if (this.mode == EdgeDetection.EdgeDetectMode.TriangleDepthNormals || this.mode == EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals)
			{
				base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
			}
		}

		private void OnEnable()
		{
			this.SetCameraFlag();
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			Vector2 vector = new Vector2(this.sensitivityDepth, this.sensitivityNormals);
			this.edgeDetectMaterial.SetVector("_Sensitivity", new Vector4(vector.x, vector.y, 1f, vector.y));
			this.edgeDetectMaterial.SetFloat("_BgFade", this.edgesOnly);
			this.edgeDetectMaterial.SetFloat("_SampleDistance", this.sampleDist);
			this.edgeDetectMaterial.SetVector("_BgColor", this.edgesOnlyBgColor);
			this.edgeDetectMaterial.SetFloat("_Exponent", this.edgeExp);
			this.edgeDetectMaterial.SetFloat("_Threshold", this.lumThreshold);
			Graphics.Blit(source, destination, this.edgeDetectMaterial, (int)this.mode);
		}
	}
}
