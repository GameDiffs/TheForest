using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Sonic Ether/SESSAO"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class SESSAO : MonoBehaviour
{
	private Material material;

	public bool visualizeSSAO;

	private Texture2D ditherTexture;

	private Texture2D ditherTextureSmall;

	private bool skipThisFrame;

	[Range(0.02f, 5f)]
	public float radius = 1f;

	[Range(-0.2f, 0.5f)]
	public float bias = 0.1f;

	[Range(0.1f, 3f)]
	public float bilateralDepthTolerance = 0.2f;

	[Range(1f, 5f)]
	public float zThickness = 2.35f;

	[Range(0.5f, 5f)]
	public float occlusionIntensity = 1.3f;

	[Range(1f, 6f)]
	public float sampleDistributionCurve = 1.15f;

	[Range(0f, 1f)]
	public float colorBleedAmount = 1f;

	[Range(0.1f, 3f)]
	public float brightnessThreshold;

	public float drawDistance = 500f;

	public float drawDistanceFadeSize = 1f;

	public bool reduceSelfBleeding = true;

	public bool useDownsampling;

	public bool halfSampling;

	public bool preserveDetails;

	[HideInInspector]
	public Camera attachedCamera;

	private object initChecker;

	private void CheckInit()
	{
		if (this.initChecker == null)
		{
			this.Init();
		}
	}

	private void Init()
	{
		this.skipThisFrame = false;
		Shader shader = Shader.Find("Hidden/SESSAO");
		if (!shader)
		{
			this.skipThisFrame = true;
			return;
		}
		this.material = new Material(shader);
		this.attachedCamera = base.GetComponent<Camera>();
		this.attachedCamera.depthTextureMode |= DepthTextureMode.Depth;
		this.attachedCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		this.SetupDitherTexture();
		this.SetupDitherTextureSmall();
		this.initChecker = new object();
	}

	private void Cleanup()
	{
		UnityEngine.Object.DestroyImmediate(this.material);
		this.initChecker = null;
	}

	private void SetupDitherTextureSmall()
	{
		this.ditherTextureSmall = new Texture2D(3, 3, TextureFormat.Alpha8, false);
		this.ditherTextureSmall.filterMode = FilterMode.Point;
		float[] array = new float[]
		{
			8f,
			1f,
			6f,
			3f,
			0f,
			4f,
			7f,
			2f,
			5f
		};
		for (int i = 0; i < 9; i++)
		{
			Color color = new Color(0f, 0f, 0f, array[i] / 9f);
			int x = i % 3;
			int y = Mathf.FloorToInt((float)i / 3f);
			this.ditherTextureSmall.SetPixel(x, y, color);
		}
		this.ditherTextureSmall.Apply();
		this.ditherTextureSmall.hideFlags = HideFlags.HideAndDontSave;
	}

	private void SetupDitherTexture()
	{
		this.ditherTexture = new Texture2D(5, 5, TextureFormat.Alpha8, false);
		this.ditherTexture.filterMode = FilterMode.Point;
		float[] array = new float[]
		{
			12f,
			1f,
			10f,
			3f,
			20f,
			5f,
			18f,
			7f,
			16f,
			9f,
			24f,
			2f,
			11f,
			6f,
			22f,
			15f,
			8f,
			0f,
			13f,
			19f,
			4f,
			21f,
			14f,
			23f,
			17f
		};
		for (int i = 0; i < 25; i++)
		{
			Color color = new Color(0f, 0f, 0f, array[i] / 25f);
			int x = i % 5;
			int y = Mathf.FloorToInt((float)i / 5f);
			this.ditherTexture.SetPixel(x, y, color);
		}
		this.ditherTexture.Apply();
		this.ditherTexture.hideFlags = HideFlags.HideAndDontSave;
	}

	private void Start()
	{
		this.CheckInit();
	}

	private void OnEnable()
	{
		this.CheckInit();
	}

	private void OnDisable()
	{
		this.Cleanup();
	}

	private void Update()
	{
		this.drawDistance = Mathf.Max(0f, this.drawDistance);
		this.drawDistanceFadeSize = Mathf.Max(0.001f, this.drawDistanceFadeSize);
		this.bilateralDepthTolerance = Mathf.Max(1E-06f, this.bilateralDepthTolerance);
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CheckInit();
		if (this.skipThisFrame)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.material.hideFlags = HideFlags.HideAndDontSave;
		this.material.SetTexture("_DitherTexture", (!this.preserveDetails) ? this.ditherTexture : this.ditherTextureSmall);
		this.material.SetInt("PreserveDetails", (!this.preserveDetails) ? 0 : 1);
		this.material.SetMatrix("ProjectionMatrixInverse", base.GetComponent<Camera>().projectionMatrix.inverse);
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBHalf);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBHalf);
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, source.format);
		temporary3.wrapMode = TextureWrapMode.Clamp;
		temporary3.filterMode = FilterMode.Bilinear;
		Graphics.Blit(source, temporary3);
		this.material.SetTexture("_ColorDownsampled", temporary3);
		RenderTexture renderTexture = null;
		this.material.SetFloat("Radius", this.radius);
		this.material.SetFloat("Bias", this.bias);
		this.material.SetFloat("DepthTolerance", this.bilateralDepthTolerance);
		this.material.SetFloat("ZThickness", this.zThickness);
		this.material.SetFloat("Intensity", this.occlusionIntensity);
		this.material.SetFloat("SampleDistributionCurve", this.sampleDistributionCurve);
		this.material.SetFloat("ColorBleedAmount", this.colorBleedAmount);
		this.material.SetFloat("DrawDistance", this.drawDistance);
		this.material.SetFloat("DrawDistanceFadeSize", this.drawDistanceFadeSize);
		this.material.SetFloat("SelfBleedReduction", (!this.reduceSelfBleeding) ? 0f : 1f);
		this.material.SetFloat("BrightnessThreshold", this.brightnessThreshold);
		this.material.SetInt("HalfSampling", (!this.halfSampling) ? 0 : 1);
		this.material.SetInt("Orthographic", (!this.attachedCamera.orthographic) ? 0 : 1);
		if (this.useDownsampling)
		{
			renderTexture = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, RenderTextureFormat.ARGBHalf);
			renderTexture.filterMode = FilterMode.Bilinear;
			this.material.SetInt("Downsamp", 1);
			Graphics.Blit(source, renderTexture, this.material, (this.colorBleedAmount > 0.0001f) ? 0 : 1);
		}
		else
		{
			this.material.SetInt("Downsamp", 0);
			Graphics.Blit(source, temporary, this.material, (this.colorBleedAmount > 0.0001f) ? 0 : 1);
		}
		RenderTexture.ReleaseTemporary(temporary3);
		this.material.SetFloat("BlurDepthTolerance", 0.1f);
		int pass = (!this.attachedCamera.orthographic) ? 2 : 6;
		if (this.attachedCamera.orthographic)
		{
			this.material.SetFloat("Near", this.attachedCamera.nearClipPlane);
			this.material.SetFloat("Far", this.attachedCamera.farClipPlane);
		}
		if (this.useDownsampling)
		{
			this.material.SetVector("Kernel", new Vector2(2f, 0f));
			Graphics.Blit(renderTexture, temporary2, this.material, pass);
			RenderTexture.ReleaseTemporary(renderTexture);
			this.material.SetVector("Kernel", new Vector2(0f, 2f));
			Graphics.Blit(temporary2, temporary, this.material, pass);
			this.material.SetVector("Kernel", new Vector2(2f, 0f));
			Graphics.Blit(temporary, temporary2, this.material, pass);
			this.material.SetVector("Kernel", new Vector2(0f, 2f));
			Graphics.Blit(temporary2, temporary, this.material, pass);
		}
		else
		{
			this.material.SetVector("Kernel", new Vector2(1f, 0f));
			Graphics.Blit(temporary, temporary2, this.material, pass);
			this.material.SetVector("Kernel", new Vector2(0f, 1f));
			Graphics.Blit(temporary2, temporary, this.material, pass);
			this.material.SetVector("Kernel", new Vector2(1f, 0f));
			Graphics.Blit(temporary, temporary2, this.material, pass);
			this.material.SetVector("Kernel", new Vector2(0f, 1f));
			Graphics.Blit(temporary2, temporary, this.material, pass);
		}
		RenderTexture.ReleaseTemporary(temporary2);
		this.material.SetTexture("_SSAO", temporary);
		if (!this.visualizeSSAO)
		{
			Graphics.Blit(source, destination, this.material, 3);
		}
		else
		{
			Graphics.Blit(source, destination, this.material, 5);
		}
		RenderTexture.ReleaseTemporary(temporary);
	}
}
