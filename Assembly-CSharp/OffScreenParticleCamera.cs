using System;
using TheForest.Utils;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class OffScreenParticleCamera : MonoBehaviour
{
	public enum Factor
	{
		Full = 1,
		Half,
		Quarter = 4,
		Eighth = 8
	}

	[Serializable]
	public class DebugOptions
	{
		[Tooltip("Draws buffers in top-left side of screen for debugging")]
		public bool debugDrawBuffers;
	}

	[Tooltip("Layer to render in low resolution")]
	public LayerMask ParticleLayers;

	[Tooltip("How much should we scale down the rendering. Lower scales have greater chances of artifacting, but better performance")]
	public OffScreenParticleCamera.Factor factor = OffScreenParticleCamera.Factor.Half;

	[Range(0.0001f, 0.01f), Tooltip("Depth threshold; essentially an edge width for when to use standard bilinear instead of uv offsets")]
	public float depthThreshold = 0.005f;

	[Tooltip("Clear color for particle camera")]
	public Color clearColor = new Color(0f, 0f, 0f, 0f);

	[Tooltip("Shader for downsampling")]
	public Shader downsampleDepthFastShader;

	[Tooltip("Shader for upsampling")]
	public Shader compositeShader;

	private Material compositeMaterial;

	private Material downsampleFastMaterial;

	public Camera shaderCamera;

	private Camera mCamera;

	public Light[] ShadowCastingLights;

	public OffScreenParticleCamera.DebugOptions debugOptions = new OffScreenParticleCamera.DebugOptions();

	private void Awake()
	{
		this.mCamera = base.GetComponent<Camera>();
		this.mCamera.depthTextureMode |= DepthTextureMode.Depth;
		if (Application.isPlaying)
		{
			this.ShadowCastingLights[0] = Scene.Atmosphere.Sun;
			this.ShadowCastingLights[1] = Scene.Atmosphere.Moon;
		}
	}

	private RenderTexture DownsampleDepth(int ssX, int ssY, Texture src, Material mat, int downsampleFactor)
	{
		Vector2 v = new Vector2(1f / (float)ssX, 1f / (float)ssX);
		ssX /= downsampleFactor;
		ssY /= downsampleFactor;
		RenderTexture temporary = RenderTexture.GetTemporary(ssX, ssY, 0);
		mat.SetVector("_PixelSize", v);
		Graphics.Blit(src, temporary, mat);
		return temporary;
	}

	private void OnDisable()
	{
		if (this.compositeMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(this.compositeMaterial);
			UnityEngine.Object.DestroyImmediate(this.downsampleFastMaterial);
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (!base.enabled || this.compositeShader == null || this.downsampleDepthFastShader == null)
		{
			if (this.compositeShader == null)
			{
				Debug.Log("OffScreenParticle: composite shader not assigned");
			}
			if (this.downsampleDepthFastShader == null)
			{
				Debug.Log("OffScreenParticle: downsample shader not assigned");
			}
			Graphics.Blit(src, dest);
			return;
		}
		for (int i = 0; i < this.ShadowCastingLights.Length; i++)
		{
			if (this.ShadowCastingLights[i])
			{
				this.ShadowCastingLights[i].shadows = LightShadows.None;
			}
		}
		if (this.compositeMaterial == null)
		{
			this.compositeMaterial = new Material(this.compositeShader);
		}
		if (this.downsampleFastMaterial == null)
		{
			this.downsampleFastMaterial = new Material(this.downsampleDepthFastShader);
		}
		if (this.shaderCamera == null)
		{
			this.shaderCamera = new GameObject("ParticleCam", new Type[]
			{
				typeof(Camera)
			}).GetComponent<Camera>();
			this.shaderCamera.enabled = false;
			this.shaderCamera.transform.parent = base.transform;
		}
		if (this.factor == OffScreenParticleCamera.Factor.Full)
		{
			Graphics.Blit(src, dest);
			this.shaderCamera.CopyFrom(this.mCamera);
			this.shaderCamera.cullingMask = this.ParticleLayers;
			this.shaderCamera.clearFlags = CameraClearFlags.Nothing;
			this.shaderCamera.depthTextureMode = DepthTextureMode.None;
			this.shaderCamera.targetTexture = dest;
			this.shaderCamera.renderingPath = RenderingPath.Forward;
			this.shaderCamera.hdr = this.mCamera.hdr;
			this.shaderCamera.useOcclusionCulling = false;
			this.shaderCamera.Render();
			for (int j = 0; j < this.ShadowCastingLights.Length; j++)
			{
				if (this.ShadowCastingLights[j])
				{
					this.ShadowCastingLights[j].shadows = LightShadows.Soft;
				}
			}
			return;
		}
		RenderTexture renderTexture = this.DownsampleDepth(Screen.width, Screen.height, src, this.downsampleFastMaterial, (int)this.factor);
		Shader.SetGlobalTexture("_CameraDepthLowRes", renderTexture);
		RenderTexture temporary = RenderTexture.GetTemporary(Screen.width / (int)this.factor, Screen.height / (int)this.factor, 0);
		this.shaderCamera.CopyFrom(this.mCamera);
		this.shaderCamera.cullingMask = this.ParticleLayers;
		this.clearColor.a = 0f;
		this.shaderCamera.backgroundColor = this.clearColor;
		this.shaderCamera.clearFlags = CameraClearFlags.Color;
		this.shaderCamera.depthTextureMode = DepthTextureMode.None;
		this.shaderCamera.targetTexture = temporary;
		this.shaderCamera.useOcclusionCulling = false;
		this.shaderCamera.renderingPath = RenderingPath.Forward;
		this.shaderCamera.hdr = this.mCamera.hdr;
		this.shaderCamera.Render();
		Vector2 v = new Vector2(1f / (float)renderTexture.width, 1f / (float)renderTexture.height);
		this.compositeMaterial.SetVector("_LowResPixelSize", v);
		this.compositeMaterial.SetVector("_LowResTextureSize", new Vector2((float)renderTexture.width, (float)renderTexture.height));
		this.compositeMaterial.SetFloat("_DepthMult", 32f);
		this.compositeMaterial.SetFloat("_Threshold", this.depthThreshold);
		this.compositeMaterial.SetTexture("_ParticleRT", temporary);
		Graphics.Blit(src, dest);
		Graphics.Blit(temporary, dest, this.compositeMaterial);
		if (this.debugOptions.debugDrawBuffers)
		{
			GL.PushMatrix();
			GL.LoadPixelMatrix(0f, (float)Screen.width, (float)Screen.height, 0f);
			Graphics.DrawTexture(new Rect(0f, 0f, 128f, 128f), renderTexture);
			Graphics.DrawTexture(new Rect(0f, 128f, 128f, 128f), src);
			Graphics.DrawTexture(new Rect(128f, 128f, 128f, 128f), temporary);
			GL.PopMatrix();
		}
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(renderTexture);
		for (int k = 0; k < this.ShadowCastingLights.Length; k++)
		{
			if (this.ShadowCastingLights[k])
			{
				this.ShadowCastingLights[k].shadows = LightShadows.Soft;
			}
		}
	}
}
