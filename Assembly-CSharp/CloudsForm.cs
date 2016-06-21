using System;
using TheForest.Utils;
using UnityEngine;

public class CloudsForm : MonoBehaviour
{
	private float A1;

	private float A2;

	private float A3;

	private float A4;

	private float C1;

	private float C2;

	private float C3;

	private float C4;

	public TheForestAtmosphere Atmos;

	private float SunAmount;

	private float Overcast = 1f;

	private float Sunny = 2f;

	private Material MyClouds;

	private void Start()
	{
		base.InvokeRepeating("ChangeClouds", 0f, 60f);
	}

	private void Update()
	{
		if (this.C1 < this.A1)
		{
			this.C1 += 0.02f * Time.deltaTime;
		}
		else if (this.C1 > this.A1)
		{
			this.C1 -= 0.02f * Time.deltaTime;
		}
		if (this.C2 < this.A2)
		{
			this.C1 += 0.02f * Time.deltaTime;
		}
		else if (this.C2 > this.A2)
		{
			this.C2 -= 0.02f * Time.deltaTime;
		}
		if (this.C3 < this.A3)
		{
			this.C3 += 0.02f * Time.deltaTime;
		}
		else if (this.C3 > this.A3)
		{
			this.C3 -= 0.02f * Time.deltaTime;
		}
		if (this.C4 < this.A4)
		{
			this.C4 += 0.02f * Time.deltaTime;
		}
		else if (this.C4 > this.A4)
		{
			this.C4 -= 0.02f * Time.deltaTime;
		}
		if (this.Atmos.SunLightIntensity < this.SunAmount)
		{
			this.Atmos.SunLightIntensity += 0.05f * Time.deltaTime;
		}
		else if (this.Atmos.SunLightIntensity > this.SunAmount)
		{
			this.Atmos.SunLightIntensity -= 0.05f * Time.deltaTime;
		}
	}

	public void StartRain()
	{
		this.A1 = 1f;
		this.A2 = 1f;
		this.A3 = 1f;
		this.A4 = 1f;
	}

	private void ChangeClouds()
	{
		if (!Scene.WeatherSystem.Raining)
		{
			this.A1 = UnityEngine.Random.Range(0f, 1f);
			this.A2 = UnityEngine.Random.Range(0f, 1f);
			this.A3 = UnityEngine.Random.Range(0f, 1f);
			this.A4 = UnityEngine.Random.Range(0f, 1f);
			if (this.A1 < 0.5f)
			{
				this.SunAmount = UnityEngine.Random.Range(1f, 1.5f);
			}
			else
			{
				this.SunAmount = this.Sunny;
			}
		}
	}
}
