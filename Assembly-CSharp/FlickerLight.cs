using System;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
	public float minIntensity = 0.45f;

	public float maxIntensity = 1.25f;

	public float timeMult = 1f;

	private float random;

	private void Start()
	{
		this.random = UnityEngine.Random.Range(0f, 65535f);
	}

	private void Update()
	{
		float t = Mathf.PerlinNoise(this.random, Time.time * this.timeMult);
		base.GetComponent<Light>().intensity = Mathf.Lerp(this.minIntensity, this.maxIntensity, t);
	}
}
