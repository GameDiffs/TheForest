using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class SceneColorControl_Anim : MonoBehaviour
{
	public Material skyMaterial;

	public Color skyColor = new Color(0.15f, 0.3f, 0.5f, 1f);

	public Color horizonColor = new Color(0.7f, 0.85f, 1f, 1f);

	public Color groundColor = new Color(0.4f, 0.35f, 0.3f, 1f);

	public float skyIntensity = 1.1f;

	public float skyFocus = 0.2f;

	public float horizonColorBanding = 0.25f;

	public bool customFogColor;

	public Color fogColor = new Color(0.7f, 0.85f, 1f, 1f);

	private void Start()
	{
		this.skyMaterial = RenderSettings.skybox;
	}

	private void Update()
	{
		this.UpdateColors();
	}

	private void OnValidate()
	{
		this.UpdateColors();
	}

	private void UpdateColors()
	{
		if (this.skyMaterial == null)
		{
			this.skyMaterial = RenderSettings.skybox;
		}
		this.skyMaterial.SetColor("_SkyColor", this.skyColor);
		this.skyMaterial.SetColor("_HorizonColor", this.horizonColor);
		this.skyMaterial.SetColor("_GroundColor", this.groundColor);
		this.skyMaterial.SetFloat("_SkyIntensity", this.skyIntensity);
		this.skyMaterial.SetFloat("_SunSkyFocus", this.skyFocus);
		this.skyMaterial.SetFloat("_HorizonBand", this.horizonColorBanding);
		Color color = (this.skyColor * 1.4f + this.horizonColor * 0.8f + this.groundColor * 0.8f) * 0.33f;
		if (RenderSettings.ambientMode == AmbientMode.Flat)
		{
			RenderSettings.ambientSkyColor = color * this.skyIntensity;
		}
		else
		{
			RenderSettings.ambientSkyColor = (this.skyColor * 1.5f + this.horizonColor * 0.5f + color) * 0.33f * this.skyIntensity;
			RenderSettings.ambientEquatorColor = (this.horizonColor + this.skyColor + this.groundColor + color) * 0.25f * this.skyIntensity;
			RenderSettings.ambientGroundColor = (this.groundColor + this.horizonColor + color) * 0.33f * this.skyIntensity;
		}
		if (this.customFogColor)
		{
			RenderSettings.fogColor = this.fogColor;
		}
		else
		{
			this.fogColor = (color + this.horizonColor + this.groundColor) * 0.33f * this.skyIntensity;
			RenderSettings.fogColor = this.fogColor;
		}
	}
}
