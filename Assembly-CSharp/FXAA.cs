using System;
using UnityEngine;

[AddComponentMenu("Image Effects/FXAA"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class FXAA : FXAAPostEffectsBase
{
	public Shader shader;

	private Material mat;

	private void CreateMaterials()
	{
		if (this.mat == null)
		{
			this.mat = base.CheckShaderAndCreateMaterial(this.shader, this.mat);
		}
	}

	private void Start()
	{
		this.shader = Shader.Find("Hidden/FXAA3");
		this.CreateMaterials();
		base.CheckSupport(false);
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 1f / (float)Screen.width;
		float num2 = 1f / (float)Screen.height;
		this.mat.SetVector("_rcpFrame", new Vector4(num, num2, 0f, 0f));
		this.mat.SetVector("_rcpFrameOpt", new Vector4(num * 2f, num2 * 2f, num * 0.5f, num2 * 0.5f));
		Graphics.Blit(source, destination, this.mat);
	}
}
