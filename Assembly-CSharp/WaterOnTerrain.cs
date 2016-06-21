using System;
using TheForest.Utils;
using UnityEngine;

public class WaterOnTerrain : MonoBehaviour
{
	[Space(5f)]
	public float RainIntensity;

	[Space(5f)]
	public float CracksDeltaRain = 0.1f;

	public float CracksDeltaDry = 0.05f;

	private float waterLevelCracks;

	[Space(5f)]
	public float PuddlesMinLevel = 0.5f;

	public float PuddlesDeltaRain = 0.05f;

	public float PuddlesDeltaDry = 0.025f;

	private float waterLevelPuddles;

	private Vector2 CurrentLevel = new Vector2(0f, 0f);

	[Space(10f)]
	private static WaterOnTerrain instance;

	public static WaterOnTerrain Instance
	{
		get
		{
			if (WaterOnTerrain.instance == null)
			{
				WaterOnTerrain.instance = Scene.Clock.gameObject.AddComponent<WaterOnTerrain>();
			}
			if (TerrainWetness.instance)
			{
				UnityEngine.Object.Destroy(TerrainWetness.instance);
			}
			return WaterOnTerrain.instance;
		}
		set
		{
			WaterOnTerrain.instance = value;
		}
	}

	private void Awake()
	{
		WaterOnTerrain.Instance = this;
		Shader.SetGlobalVector("_WaterFloodlevel", new Vector4(0f, this.PuddlesMinLevel, 0f, 0f));
		Shader.SetGlobalFloat("_RainAmount", 0f);
	}

	private void OnDestroy()
	{
		Shader.SetGlobalVector("_WaterFloodlevel", new Vector4(0f, this.PuddlesMinLevel, 0f, 0f));
		Shader.SetGlobalFloat("_RainAmount", 0f);
		if (WaterOnTerrain.instance == this)
		{
			WaterOnTerrain.instance = null;
		}
	}

	private void Update()
	{
		if (Scene.WeatherSystem.Raining)
		{
			if (this.RainIntensity < 1f)
			{
				this.RainIntensity += 0.1f * Time.deltaTime;
				this.RainIntensity = Mathf.Clamp01(this.RainIntensity);
			}
			if (this.waterLevelCracks < 1f)
			{
				this.waterLevelCracks += this.CracksDeltaRain * Time.deltaTime;
				this.waterLevelCracks = Mathf.Clamp01(this.waterLevelCracks);
			}
			if (this.waterLevelPuddles < 1f)
			{
				this.waterLevelPuddles += this.PuddlesDeltaRain * Time.deltaTime;
				this.waterLevelPuddles = Mathf.Clamp01(this.waterLevelPuddles);
			}
		}
		else
		{
			if (this.RainIntensity > 0f)
			{
				this.RainIntensity -= 0.1f * Time.deltaTime;
				this.RainIntensity = Mathf.Clamp01(this.RainIntensity);
			}
			if (this.waterLevelCracks > 0f)
			{
				this.waterLevelCracks -= this.CracksDeltaDry * Time.deltaTime;
				this.waterLevelCracks = Mathf.Clamp01(this.waterLevelCracks);
			}
			if (this.waterLevelPuddles > this.PuddlesMinLevel)
			{
				this.waterLevelPuddles -= this.PuddlesDeltaDry * Time.deltaTime;
				this.waterLevelPuddles = Mathf.Clamp(this.waterLevelPuddles, this.PuddlesMinLevel, 1f);
			}
		}
		if (this.waterLevelPuddles < this.PuddlesMinLevel)
		{
			this.waterLevelPuddles = this.PuddlesMinLevel;
		}
		if (this.CurrentLevel != new Vector2(this.waterLevelCracks, this.waterLevelPuddles))
		{
			this.CurrentLevel = new Vector2(this.waterLevelCracks, this.waterLevelPuddles);
			Shader.SetGlobalVector("_WaterFloodlevel", new Vector4(this.waterLevelCracks, this.waterLevelPuddles, 0f, 0f));
			Shader.SetGlobalFloat("_RainAmount", this.RainIntensity);
		}
	}
}
