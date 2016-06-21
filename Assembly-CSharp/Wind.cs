using System;
using TheForest.Utils;
using UnityEngine;

public class Wind : MonoBehaviour
{
	public Vector4 MyWind;

	private float WindFrequency = 0.75f;

	private float WaveSizeFoliageShader = 10f;

	private float RainAmount;

	private float SpecPower;

	public static Vector3 WindFacing;

	private void Awake()
	{
		Shader.SetGlobalVector("_Wind", this.MyWind);
		Shader.SetGlobalFloat("_AfsWaveSize", this.WaveSizeFoliageShader);
		Shader.SetGlobalFloat("_AfsRainamount", this.RainAmount);
		Shader.SetGlobalFloat("_AfsSpecPower", this.SpecPower);
		base.InvokeRepeating("ChangeWindAmount", 1f, 60f);
	}

	private void Update()
	{
		if (Scene.WeatherSystem && Scene.WeatherSystem.Raining && (double)this.RainAmount < 1.0)
		{
			this.RainAmount += 0.1f * Time.deltaTime;
			this.SpecPower += 0.1f * Time.deltaTime;
			this.MyWind = new Vector4(2.1f, -3.1f, 1.1f, 0.5f);
		}
		else if (Scene.WeatherSystem && !Scene.WeatherSystem.Raining && (double)this.RainAmount > 0.0)
		{
			this.RainAmount -= 0.1f * Time.deltaTime;
			this.SpecPower -= 0.1f * Time.deltaTime;
		}
		Vector4 myWind = this.MyWind;
		float w = this.MyWind.w;
		myWind.x *= (1.25f + Mathf.Sin(Time.time * this.WindFrequency) * Mathf.Sin(Time.time * 0.375f)) * 0.5f;
		myWind.z *= (1.25f + Mathf.Sin(Time.time * this.WindFrequency) * Mathf.Sin(Time.time * 0.193f)) * 0.5f;
		myWind.w = w;
		Wind.WindFacing = new Vector3(myWind.x, 0f, myWind.z);
		Shader.SetGlobalVector("_Wind", myWind);
		Shader.SetGlobalFloat("_AfsWaveSize", 0.5f / this.WaveSizeFoliageShader);
		Shader.SetGlobalFloat("_AfsRainamount", this.RainAmount);
		Shader.SetGlobalFloat("_AfsSpecPower", this.SpecPower);
	}

	private void ChangeWindAmount()
	{
		if (Scene.WeatherSystem && !Scene.WeatherSystem.Raining)
		{
			this.MyWind = new Vector4(UnityEngine.Random.Range(0f, 1.1f), UnityEngine.Random.Range(0f, 1.1f), UnityEngine.Random.Range(0f, 1.1f), 0.5f);
		}
	}
}
