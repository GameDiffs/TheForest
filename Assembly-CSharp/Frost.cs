using BlackfireStudio;
using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Blackfire Studio/Frost"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class Frost : MonoBehaviour
{
	private bool isPro;

	public Shader shader;

	public Color color;

	public Texture2D diffuseTex;

	public Texture2D bumpTex;

	public Texture2D coverageTex;

	public float transparency;

	public float refraction;

	public float coverage;

	public float smooth;

	private Material frostMaterial;

	protected Material material
	{
		get
		{
			if (this.frostMaterial == null)
			{
				this.frostMaterial = new Material(this.shader);
				this.frostMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this.frostMaterial;
		}
	}

	private void Start()
	{
		this.isPro = ImageEffects.IsPro<Frost>(base.gameObject, typeof(Frost), this.shader);
	}

	private void Update()
	{
		if (!this.isPro && this.shader != null)
		{
			this.material.SetTexture("_MainTex", ImageEffects.RenderTexture);
			this.material.SetColor("_Color", this.color);
			this.material.SetFloat("_Transparency", this.transparency);
			this.material.SetFloat("_Refraction", this.refraction);
			this.material.SetFloat("_Coverage", this.coverage);
			this.material.SetFloat("_Smooth", this.smooth);
			if (this.diffuseTex != null)
			{
				this.material.SetTexture("_DiffuseTex", this.diffuseTex);
			}
			else
			{
				this.material.SetTexture("_DiffuseTex", null);
			}
			if (this.bumpTex != null)
			{
				this.material.SetTexture("_BumpTex", this.bumpTex);
			}
			else
			{
				this.material.SetTexture("_BumpTex", null);
			}
			if (this.coverageTex != null)
			{
				this.material.SetTexture("_CoverageTex", this.coverageTex);
			}
			else
			{
				this.material.SetTexture("_CoverageTex", null);
			}
		}
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (this.shader != null)
		{
			this.material.SetColor("_Color", this.color);
			this.material.SetFloat("_Transparency", this.transparency);
			this.material.SetFloat("_Refraction", this.refraction);
			this.material.SetFloat("_Coverage", this.coverage);
			this.material.SetFloat("_Smooth", this.smooth);
			if (this.diffuseTex != null)
			{
				this.material.SetTexture("_DiffuseTex", this.diffuseTex);
			}
			else
			{
				this.material.SetTexture("_DiffuseTex", null);
			}
			if (this.bumpTex != null)
			{
				this.material.SetTexture("_BumpTex", this.bumpTex);
			}
			else
			{
				this.material.SetTexture("_BumpTex", null);
			}
			if (this.coverageTex != null)
			{
				this.material.SetTexture("_CoverageTex", this.coverageTex);
			}
			else
			{
				this.material.SetTexture("_CoverageTex", null);
			}
			Graphics.Blit(sourceTexture, destTexture, this.material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	public void OnPostRender()
	{
		if (!this.isPro)
		{
			ImageEffects.RenderImageEffect(this.material);
		}
	}

	private void OnEnable()
	{
		this.isPro = ImageEffects.IsPro<Frost>(base.gameObject, typeof(Frost), this.shader);
	}

	private void OnDisable()
	{
		if (this.frostMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(this.frostMaterial);
		}
	}
}
