using System;
using UnityEngine;

public class DynamicShadowSettings : MonoBehaviour
{
	public Light sunLight;

	public float minHeight = 10f;

	public float minShadowDistance = 80f;

	public float minShadowBias = 1f;

	public float maxHeight = 1000f;

	public float maxShadowDistance = 10000f;

	public float maxShadowBias = 0.1f;

	public float adaptTime = 1f;

	private float smoothHeight;

	private float changeSpeed;

	private float originalStrength = 1f;

	private void Start()
	{
		this.originalStrength = this.sunLight.shadowStrength;
	}

	private void Update()
	{
		Ray ray = new Ray(Camera.main.transform.position, -Vector3.up);
		float num = base.transform.position.y;
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit))
		{
			num = raycastHit.distance;
		}
		if (Mathf.Abs(num - this.smoothHeight) > 1f)
		{
			this.smoothHeight = Mathf.SmoothDamp(this.smoothHeight, num, ref this.changeSpeed, this.adaptTime);
		}
		float num2 = Mathf.InverseLerp(this.minHeight, this.maxHeight, this.smoothHeight);
		QualitySettings.shadowDistance = Mathf.Lerp(this.minShadowDistance, this.maxShadowDistance, num2);
		this.sunLight.shadowBias = Mathf.Lerp(this.minShadowBias, this.maxShadowBias, 1f - (1f - num2) * (1f - num2));
		this.sunLight.shadowStrength = Mathf.Lerp(this.originalStrength, 0f, num2);
	}
}
