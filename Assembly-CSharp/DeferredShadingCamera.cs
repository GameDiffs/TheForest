using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class DeferredShadingCamera : MonoBehaviour
{
	public enum DebugDisplay
	{
		gbuffer0,
		gbuffer1,
		gbuffer2,
		composite
	}

	private const int numBuffers = 3;

	private Camera c;

	private Camera transparentCamera;

	private RenderTexture lastActive;

	[HideInInspector]
	public Shader gbufferShader;

	[HideInInspector]
	public Shader ssaoShader;

	[HideInInspector]
	public Shader postBlurShader;

	public Texture2D uniformNoise;

	private Material ssaoMaterial;

	private Material postBlurMaterial;

	private Material compositeMaterial;

	[HideInInspector]
	public Shader compositeShader;

	public LayerMask gbufferCullingMask;

	public LayerMask waterRenderingMask;

	public LayerMask particleRenderingMask;

	private RenderTexture[] gbufferTextures;

	private RenderBuffer[] gbufferBuffers;

	private RenderTexture depthTexture;

	private RenderBuffer depthBuffer;

	private Vector2 previousRenderSize;

	private Vector2 currentRenderSize;

	private SunshinePostprocess sunshinePostProcess;

	private static List<DeferredLight> sceneLights;

	[HideInInspector]
	public static Vector3 directionalLightVector;

	[HideInInspector]
	public static Color directionalLightColor;

	public bool renderPointLights = true;

	public bool renderSSAO = true;

	public bool renderSSSSS = true;

	private object initChecker;

	public DeferredShadingCamera.DebugDisplay displayMode;

	private Camera gbufferCamera;

	private int actualCullingMask;

	private void SetupGbufferTextures()
	{
		this.previousRenderSize = new Vector2((float)base.GetComponent<Camera>().pixelWidth, (float)base.GetComponent<Camera>().pixelHeight);
		for (int i = 0; i < 3; i++)
		{
			this.gbufferTextures[i] = new RenderTexture((int)this.previousRenderSize.x, (int)this.previousRenderSize.y, 0, (i != 0) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGBHalf, (i != 2) ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
			this.gbufferTextures[i].useMipMap = false;
			this.gbufferTextures[i].hideFlags = HideFlags.HideAndDontSave;
			this.gbufferTextures[i].filterMode = ((i != 2) ? FilterMode.Bilinear : FilterMode.Point);
			this.gbufferBuffers[i] = this.gbufferTextures[i].colorBuffer;
		}
		this.depthTexture = new RenderTexture((int)this.previousRenderSize.x, (int)this.previousRenderSize.y, 16, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
		this.depthTexture.filterMode = FilterMode.Point;
		this.depthTexture.useMipMap = false;
		this.depthTexture.hideFlags = HideFlags.HideAndDontSave;
		this.depthBuffer = this.depthTexture.depthBuffer;
	}

	private void SetupGbufferCamera()
	{
		GameObject gameObject = new GameObject("GBUFFER CAMERA");
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		gameObject.AddComponent<Camera>();
		this.gbufferCamera = gameObject.GetComponent<Camera>();
		this.gbufferCamera.clearFlags = CameraClearFlags.Color;
		this.gbufferCamera.farClipPlane = base.GetComponent<Camera>().farClipPlane;
		this.gbufferCamera.nearClipPlane = base.GetComponent<Camera>().nearClipPlane;
		this.gbufferCamera.useOcclusionCulling = true;
		this.gbufferCamera.depth = base.GetComponent<Camera>().depth;
		this.gbufferCamera.fieldOfView = base.GetComponent<Camera>().fieldOfView;
		this.gbufferCamera.orthographic = base.GetComponent<Camera>().orthographic;
		this.gbufferCamera.enabled = false;
		this.gbufferCamera.renderingPath = RenderingPath.Forward;
		this.gbufferCamera.backgroundColor = Color.black;
	}

	private void SetupTransparentCamera()
	{
		GameObject gameObject = new GameObject("TRANSPARENT CAMERA");
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		gameObject.AddComponent<Camera>();
		this.transparentCamera = gameObject.GetComponent<Camera>();
		this.transparentCamera.enabled = false;
	}

	private void Init()
	{
		if (this.initChecker == null)
		{
			this.gbufferTextures = new RenderTexture[3];
			this.gbufferBuffers = new RenderBuffer[3];
			this.c = base.GetComponent<Camera>();
			this.SetupGbufferTextures();
			this.SetupTransparentCamera();
			this.c.renderingPath = RenderingPath.Forward;
			if (DeferredShadingCamera.sceneLights == null)
			{
				DeferredShadingCamera.sceneLights = new List<DeferredLight>();
			}
			else
			{
				foreach (DeferredLight current in DeferredShadingCamera.sceneLights)
				{
					if (current == null)
					{
						DeferredShadingCamera.sceneLights.Remove(current);
					}
				}
			}
			if (!this.compositeMaterial)
			{
				this.compositeMaterial = new Material(this.compositeShader);
			}
			if (!this.ssaoMaterial)
			{
				this.ssaoMaterial = new Material(this.ssaoShader);
			}
			if (!this.postBlurMaterial)
			{
				this.postBlurMaterial = new Material(this.postBlurShader);
			}
			if (this.sunshinePostProcess == null)
			{
				this.sunshinePostProcess = base.GetComponent<SunshinePostprocess>();
			}
			this.initChecker = new object();
		}
	}

	private void RenderGbuffers()
	{
		base.GetComponent<Camera>().cullingMask = 0;
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
		RenderTexture active = RenderTexture.active;
		this.gbufferCamera.cullingMask = this.gbufferCullingMask;
		this.gbufferCamera.clearFlags = CameraClearFlags.Color;
		this.gbufferCamera.backgroundColor = Color.black;
		this.gbufferCamera.SetTargetBuffers(this.gbufferBuffers, this.depthBuffer);
		this.gbufferCamera.Render();
		RenderTexture.active = active;
		for (int i = 0; i < 3; i++)
		{
			string propertyName = "gbuffer" + i.ToString();
			Shader.SetGlobalTexture(propertyName, this.gbufferTextures[i]);
		}
		Shader.SetGlobalTexture("gdepth", this.depthTexture);
	}

	private void SetupCameraForGbufferRender()
	{
		this.lastActive = RenderTexture.active;
		this.c.cullingMask = this.gbufferCullingMask;
		this.c.clearFlags = CameraClearFlags.Color;
		this.c.depthTextureMode = DepthTextureMode.None;
		Graphics.SetRenderTarget(this.gbufferBuffers, this.depthBuffer);
		for (int i = 0; i < 3; i++)
		{
			string propertyName = "gbuffer" + i.ToString();
			Shader.SetGlobalTexture(propertyName, this.gbufferTextures[i]);
		}
		Shader.SetGlobalTexture("gdepth", this.depthTexture);
	}

	private void OnPreRender()
	{
		this.SetupCameraForGbufferRender();
	}

	private void SynchronizeGbufferCamera()
	{
		this.gbufferCamera.fieldOfView = base.GetComponent<Camera>().fieldOfView;
		this.gbufferCamera.gameObject.transform.position = base.transform.position;
		this.gbufferCamera.gameObject.transform.rotation = base.transform.rotation;
		this.gbufferCamera.renderingPath = RenderingPath.Forward;
		if ((float)base.GetComponent<Camera>().pixelWidth != this.previousRenderSize.x || (float)base.GetComponent<Camera>().pixelHeight != this.previousRenderSize.y)
		{
			this.SetupGbufferTextures();
		}
	}

	private void LateUpdate()
	{
		this.Init();
		Shader.SetGlobalVector("_DirectionalLightVector", new Vector4(DeferredShadingCamera.directionalLightVector.x, DeferredShadingCamera.directionalLightVector.y, DeferredShadingCamera.directionalLightVector.z, 1f));
		Shader.SetGlobalVector("_DirectionalLightColor", new Color(Mathf.Pow(DeferredShadingCamera.directionalLightColor.r, 2.2f), Mathf.Pow(DeferredShadingCamera.directionalLightColor.g, 2.2f), Mathf.Pow(DeferredShadingCamera.directionalLightColor.b, 2.2f), DeferredShadingCamera.directionalLightColor.a));
		Shader.SetGlobalTexture("_UniformNoise", this.uniformNoise);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture.active = this.lastActive;
		if (this.displayMode == DeferredShadingCamera.DebugDisplay.gbuffer0)
		{
			Graphics.Blit(this.gbufferTextures[0], destination);
		}
		else if (this.displayMode == DeferredShadingCamera.DebugDisplay.gbuffer1)
		{
			Graphics.Blit(this.gbufferTextures[1], destination);
		}
		else if (this.displayMode == DeferredShadingCamera.DebugDisplay.gbuffer2)
		{
			Graphics.Blit(this.gbufferTextures[2], destination);
		}
		else if (this.displayMode == DeferredShadingCamera.DebugDisplay.composite)
		{
			RenderTexture renderTexture = null;
			if (this.renderSSAO)
			{
				RenderTexture temporary = RenderTexture.GetTemporary(this.depthTexture.width / 2, this.depthTexture.height / 2, 0, RenderTextureFormat.RFloat);
				temporary.filterMode = FilterMode.Point;
				Graphics.Blit(this.depthTexture, temporary, this.compositeMaterial, 3);
				Shader.SetGlobalTexture("gdepthhalf", temporary);
				renderTexture = RenderTexture.GetTemporary(this.gbufferTextures[0].width, this.gbufferTextures[0].height, 0, RenderTextureFormat.R8);
				RenderTexture temporary2 = RenderTexture.GetTemporary(this.gbufferTextures[0].width, this.gbufferTextures[0].height, 0, RenderTextureFormat.R8);
				Graphics.Blit(source, renderTexture, this.ssaoMaterial, 0);
				this.postBlurMaterial.SetFloat("BlurDepthTollerance", 0.1f);
				this.postBlurMaterial.SetVector("BlurXY", new Vector2(1f, 0f));
				Graphics.Blit(renderTexture, temporary2, this.postBlurMaterial, 0);
				this.postBlurMaterial.SetVector("BlurXY", new Vector2(0f, 1f));
				Graphics.Blit(temporary2, renderTexture, this.postBlurMaterial, 0);
				RenderTexture.ReleaseTemporary(temporary2);
				this.compositeMaterial.SetTexture("ssaotex", renderTexture);
				RenderTexture.ReleaseTemporary(temporary);
			}
			RenderTexture renderTexture2 = null;
			if (this.renderSSSSS)
			{
				renderTexture2 = RenderTexture.GetTemporary(this.gbufferTextures[0].width, this.gbufferTextures[0].height, 0, RenderTextureFormat.ARGBHalf);
				this.MultiBlit(this.gbufferTextures[0], new RenderTexture[]
				{
					destination,
					renderTexture2
				}, this.compositeMaterial, 0);
			}
			else
			{
				Graphics.Blit(this.gbufferTextures[0], destination, this.compositeMaterial, 0);
			}
			if (renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			if (this.renderPointLights)
			{
				int num = 0;
				int num2 = DeferredShadingCamera.sceneLights.Count / 20;
				for (int i = 0; i <= num2; i++)
				{
					for (int j = 1; j <= 20; j++)
					{
						this.compositeMaterial.SetColor("_LightColor" + j.ToString(), Color.black);
						this.compositeMaterial.SetVector("_LightPosition" + j.ToString(), new Vector4(0f, 0f, 0f, 0f));
						this.compositeMaterial.SetInt("_LightType" + j.ToString(), 2);
						this.compositeMaterial.SetFloat("_LightFakeReflection" + j.ToString(), 0f);
						this.compositeMaterial.SetVector("_LightSpotDirection" + j.ToString(), Vector4.zero);
					}
					for (int k = 1; k <= Mathf.Min(DeferredShadingCamera.sceneLights.Count, 20) - i * 20; k++)
					{
						DeferredLight deferredLight = DeferredShadingCamera.sceneLights[k - 1 + i * 20];
						if (deferredLight == null)
						{
							DeferredShadingCamera.sceneLights.Clear();
							break;
						}
						this.compositeMaterial.SetColor("_LightColor" + k.ToString(), new Color(deferredLight.color.r * deferredLight.color.r, deferredLight.color.g * deferredLight.color.g, deferredLight.color.b * deferredLight.color.b, deferredLight.intensity));
						this.compositeMaterial.SetVector("_LightPosition" + k.ToString(), new Vector4(deferredLight.position.x, deferredLight.position.y, deferredLight.position.z, deferredLight.range));
						this.compositeMaterial.SetInt("_LightType" + k.ToString(), (deferredLight.type != LightType.Point) ? ((deferredLight.type != LightType.Spot) ? 2 : 1) : 0);
						this.compositeMaterial.SetFloat("_LightFakeReflection" + k.ToString(), deferredLight.fakerefl);
						this.compositeMaterial.SetVector("_LightSpotDirection" + k.ToString(), new Vector4(deferredLight.spotDirection.x, deferredLight.spotDirection.y, deferredLight.spotDirection.z, deferredLight.spotAngle));
						this.compositeMaterial.SetTexture("_LightProjectTexture" + k.ToString(), deferredLight.spotlightProjectTexture);
						num++;
					}
					if (this.renderSSSSS)
					{
						this.MultiBlit(this.gbufferTextures[0], new RenderTexture[]
						{
							destination,
							renderTexture2
						}, this.compositeMaterial, 1);
					}
					else
					{
						Graphics.Blit(this.gbufferTextures[0], destination, this.compositeMaterial, 1);
					}
				}
			}
			if (this.renderSSSSS)
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(this.gbufferTextures[0].width, this.gbufferTextures[0].height, 0, RenderTextureFormat.ARGBHalf);
				this.compositeMaterial.SetVector("sssssKernel", new Vector3(1f, 0f, 1f));
				Graphics.Blit(renderTexture2, temporary3, this.compositeMaterial, 4);
				this.compositeMaterial.SetVector("sssssKernel", new Vector3(0f, 1f, 1f));
				Graphics.Blit(temporary3, renderTexture2, this.compositeMaterial, 4);
				this.compositeMaterial.SetVector("sssssKernel", new Vector3(3f, 0f, 0.5f));
				Graphics.Blit(renderTexture2, temporary3, this.compositeMaterial, 4);
				this.compositeMaterial.SetVector("sssssKernel", new Vector3(0f, 3f, 0.5f));
				Graphics.Blit(temporary3, renderTexture2, this.compositeMaterial, 4);
				RenderTexture.ReleaseTemporary(temporary3);
				Graphics.Blit(renderTexture2, destination, this.compositeMaterial, 5);
				RenderTexture.ReleaseTemporary(renderTexture2);
			}
			RenderTexture temporary4 = RenderTexture.GetTemporary(this.gbufferTextures[0].width, this.gbufferTextures[0].height, 0, this.gbufferTextures[0].format);
			Graphics.Blit(destination, temporary4);
			Shader.SetGlobalTexture("composite", temporary4);
			this.transparentCamera.CopyFrom(this.c);
			this.transparentCamera.enabled = false;
			this.transparentCamera.cullingMask = this.waterRenderingMask;
			this.transparentCamera.clearFlags = CameraClearFlags.Nothing;
			this.transparentCamera.SetTargetBuffers(destination.colorBuffer, destination.depthBuffer);
			this.transparentCamera.Render();
			this.transparentCamera.cullingMask = this.particleRenderingMask;
			this.transparentCamera.SetTargetBuffers(destination.colorBuffer, destination.depthBuffer);
			this.transparentCamera.Render();
			RenderTexture.ReleaseTemporary(temporary4);
		}
	}

	private void MultiBlit(RenderTexture source, RenderTexture[] targets, Material material, int pass)
	{
		RenderTexture active = RenderTexture.active;
		RenderBuffer[] array = new RenderBuffer[targets.Length];
		for (int i = 0; i < targets.Length; i++)
		{
			array[i] = targets[i].colorBuffer;
		}
		Graphics.SetRenderTarget(array, targets[0].depthBuffer);
		Graphics.Blit(source, material, pass);
		RenderTexture.active = active;
	}

	public static void AddLight(DeferredLight light)
	{
		if (DeferredShadingCamera.sceneLights == null)
		{
			DeferredShadingCamera.sceneLights = new List<DeferredLight>();
		}
		DeferredShadingCamera.sceneLights.Add(light);
		Debug.Log("Added a light. Total lights: " + DeferredShadingCamera.sceneLights.Count);
	}

	public static void RemoveLight(DeferredLight light)
	{
		DeferredShadingCamera.sceneLights.Remove(light);
		Debug.Log("Removed a light. Total lights: " + DeferredShadingCamera.sceneLights.Count);
	}
}
