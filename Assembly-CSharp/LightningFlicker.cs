using System;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightningFlicker : MonoBehaviour
{
	[Range(0.1f, 0.2f)]
	public float PulseAmount = 0.1f;

	[Range(1f, 20f)]
	public float PulseSpeed = 10f;

	private float startingIntensity;

	private void Start()
	{
		this.startingIntensity = base.GetComponent<Light>().intensity;
	}

	private void Update()
	{
		base.GetComponent<Light>().intensity = (Mathf.Sin(Time.timeSinceLevelLoad * 3.14159274f * this.PulseSpeed) * this.PulseAmount + (1f - this.PulseAmount)) * this.startingIntensity;
	}
}
