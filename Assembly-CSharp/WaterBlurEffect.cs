using System;
using TheForest.Graphics;
using TheForest.Utils;
using UnityEngine;

[AddComponentMenu("Image Effects/Blur/WaterBlur"), ExecuteInEditMode, RequireComponent(typeof(WaterViz))]
public class WaterBlurEffect : MonoBehaviour
{
	[Range(0f, 0.05f)]
	public float TransitionHeight = 0.03f;

	public Color WaterTint = new Color(0.4f, 0.58f, 0.75f);

	public int iterations = 3;

	public float blurSpread = 0.6f;

	public Shader blurShader;

	private Camera cam;

	private WaterViz waterViz;

	private Shader heightBlitShader;

	private Material heightBlitMaterial;

	private static Material m_Material;

	protected Material material
	{
		get
		{
			if (WaterBlurEffect.m_Material == null)
			{
				WaterBlurEffect.m_Material = new Material(this.blurShader);
				WaterBlurEffect.m_Material.hideFlags = HideFlags.DontSave;
			}
			return WaterBlurEffect.m_Material;
		}
	}

	protected void OnDisable()
	{
		if (WaterBlurEffect.m_Material)
		{
			UnityEngine.Object.DestroyImmediate(WaterBlurEffect.m_Material);
		}
	}

	protected void Start()
	{
		this.cam = base.GetComponent<Camera>();
		this.waterViz = base.GetComponent<WaterViz>();
		this.heightBlitShader = Shader.Find("Hidden/HeightBlit");
		if (this.heightBlitShader)
		{
			this.heightBlitMaterial = new Material(this.heightBlitShader);
		}
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
			return;
		}
		if (!this.blurShader || !this.material.shader.isSupported)
		{
			base.enabled = false;
			return;
		}
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
	{
		float num = 0.5f + (float)iteration * this.blurSpread;
		Graphics.BlitMultiTap(source, dest, this.material, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, this.material, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (WaterEngine.Ocean == null && LocalPlayer.Buoyancy.IsOcean)
		{
			Graphics.Blit(source, destination);
			return;
		}
		int width = source.width / 4;
		int height = source.height / 4;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0);
		this.DownSample4x(source, renderTexture);
		for (int i = 0; i < this.iterations; i++)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
			this.FourTapCone(renderTexture, temporary, i);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		if (this.waterViz && this.waterViz.Buoyancy && this.waterViz.InWater)
		{
			if (!this.cam)
			{
				this.cam = base.GetComponent<Camera>();
			}
			float waterLevel = this.waterViz.WaterLevel;
			Vector3 vector = this.cam.ViewportToWorldPoint(new Vector3(0f, 0f, this.cam.nearClipPlane));
			Vector3 vector2 = this.cam.ViewportToWorldPoint(new Vector3(0f, 1f, this.cam.nearClipPlane));
			Vector3 vector3 = this.cam.ViewportToWorldPoint(new Vector3(1f, 0f, this.cam.nearClipPlane));
			float num = vector.y - waterLevel;
			float num2 = vector2.y - vector.y;
			this.waterViz.ScreenCoverage = -num / num2;
			this.heightBlitMaterial.SetFloat("transitionHeight", this.TransitionHeight);
			this.heightBlitMaterial.SetColor("underColor", this.WaterTint);
			this.heightBlitMaterial.SetFloat("height", num);
			this.heightBlitMaterial.SetFloat("heightRayU", vector3.y - vector.y);
			this.heightBlitMaterial.SetFloat("heightRayV", num2);
			this.heightBlitMaterial.SetTexture("_LowerTex", renderTexture);
			Graphics.Blit(source, destination, this.heightBlitMaterial, 0);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}
}
