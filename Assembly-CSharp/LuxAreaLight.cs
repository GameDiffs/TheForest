using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Light))]
public class LuxAreaLight : MonoBehaviour
{
	[Header("Area Light Properties"), Range(0f, 40f)]
	public float lightLength = 1f;

	[Range(0f, 40f)]
	public float lightRadius = 0.5f;

	[Range(0f, 1f)]
	public float specularIntensity = 1f;

	[Header("Light Overwrites")]
	public Color lightColor = new Color(1f, 1f, 1f, 0f);

	[Range(0f, 8f)]
	public float lightIntensity = 1f;

	private Light light;

	private float range;

	private void OnEnable()
	{
	}

	private void OnValidate()
	{
		this.UpdateAreaLight();
	}

	public void UpdateAreaLight()
	{
		if (this.light == null)
		{
			this.light = base.GetComponent<Light>();
		}
		this.range = this.light.range;
		if (this.range < this.lightLength * 0.75f)
		{
			this.range = this.lightLength * 0.75f;
			this.light.range = this.range;
		}
		Color color = this.lightColor;
		color *= this.lightIntensity;
		float num = this.lightRadius / 80f;
		float num2 = this.lightLength / 80f;
		color.a = Mathf.Floor(num * 2047f) * 2048f + Mathf.Floor(num2 * 2047f) + this.specularIntensity * 0.5f;
		this.light.intensity = 1f;
		this.light.color = color;
	}
}
