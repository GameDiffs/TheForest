using System;
using UnityEngine;

[AddComponentMenu("Relief Terrain/Helpers/Use baked gloss texture"), ExecuteInEditMode]
public class GlossBakedTextureReplacement : MonoBehaviour
{
	public RTPGlossBaked glossBakedData;

	public bool RTPStandAloneShader;

	public int layerNumber = 1;

	public Material CustomMaterial;

	public Texture2D originalTexture;

	public bool resetGlossMultAndShaping;

	[NonSerialized]
	public Texture2D bakedTexture;

	private Renderer _renderer;

	public GlossBakedTextureReplacement()
	{
		this.bakedTexture = (this.originalTexture = null);
	}

	private void Start()
	{
		this.Refresh();
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			this.Refresh();
			if (this.resetGlossMultAndShaping)
			{
				this.resetGlossMultAndShaping = false;
				this.resetGlossMultAndShapingFun();
			}
		}
	}

	public void resetGlossMultAndShapingFun()
	{
		if (this.glossBakedData == null)
		{
			return;
		}
		Material material;
		if (this.CustomMaterial != null)
		{
			material = this.CustomMaterial;
		}
		else
		{
			if (!this._renderer)
			{
				this._renderer = base.GetComponent<Renderer>();
				if (!this._renderer)
				{
					return;
				}
			}
			material = this._renderer.sharedMaterial;
		}
		if (!material)
		{
			return;
		}
		if (this.RTPStandAloneShader)
		{
			Vector4 vector = new Vector4(1f, 1f, 1f, 1f);
			Vector4 vector2 = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
			if (material.HasProperty("RTP_gloss_mult0123"))
			{
				vector = material.GetVector("RTP_gloss_mult0123");
				if (this.layerNumber >= 1 && this.layerNumber <= 4)
				{
					vector[this.layerNumber - 1] = 1f;
				}
				material.SetVector("RTP_gloss_mult0123", vector);
			}
			if (material.HasProperty("RTP_gloss_shaping0123"))
			{
				vector2 = material.GetVector("RTP_gloss_shaping0123");
				if (this.layerNumber >= 1 && this.layerNumber <= 4)
				{
					vector2[this.layerNumber - 1] = 0.5f;
				}
				material.SetVector("RTP_gloss_shaping0123", vector2);
			}
		}
		else
		{
			string propertyName = "RTP_gloss_mult0";
			string propertyName2 = "RTP_gloss_shaping0";
			if (this.layerNumber == 2)
			{
				propertyName = "RTP_gloss_mult1";
				propertyName2 = "RTP_gloss_shaping1";
			}
			if (material.HasProperty(propertyName))
			{
				material.SetFloat(propertyName, 1f);
			}
			if (material.HasProperty(propertyName2))
			{
				material.SetFloat(propertyName2, 0.5f);
			}
		}
	}

	public void Refresh()
	{
		if (this.glossBakedData == null)
		{
			return;
		}
		string propertyName = "_MainTex";
		if (this.RTPStandAloneShader)
		{
			propertyName = "_SplatA0";
			if (this.layerNumber == 2)
			{
				propertyName = "_SplatA1";
			}
			else if (this.layerNumber == 3)
			{
				propertyName = "_SplatA2";
			}
			else if (this.layerNumber == 4)
			{
				propertyName = "_SplatA3";
			}
		}
		else if (this.layerNumber == 2)
		{
			propertyName = "_MainTex2";
		}
		Material material;
		if (this.CustomMaterial != null)
		{
			material = this.CustomMaterial;
		}
		else
		{
			if (!this._renderer)
			{
				this._renderer = base.GetComponent<Renderer>();
				if (!this._renderer)
				{
					return;
				}
			}
			material = this._renderer.sharedMaterial;
		}
		if (!material)
		{
			return;
		}
		if (material.HasProperty(propertyName))
		{
			if (this.bakedTexture)
			{
				material.SetTexture(propertyName, this.bakedTexture);
			}
			else
			{
				if (this.originalTexture == null)
				{
					this.originalTexture = (Texture2D)material.GetTexture(propertyName);
				}
				if (this.originalTexture != null && this.glossBakedData != null && !this.glossBakedData.used_in_atlas && this.glossBakedData.CheckSize(this.originalTexture))
				{
					this.bakedTexture = this.glossBakedData.MakeTexture(this.originalTexture);
					if (this.bakedTexture)
					{
						material.SetTexture(propertyName, this.bakedTexture);
					}
				}
			}
		}
	}
}
