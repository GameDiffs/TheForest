using System;
using TheForest.Utils;
using UnityEngine;

public class TerrainWetness : MonoBehaviour
{
	public ReliefTerrain rt1;

	public Terrain MainTerrain;

	private float CurrentLevel;

	public static TerrainWetness instance
	{
		get;
		private set;
	}

	public float WaterLevel
	{
		get;
		private set;
	}

	private void Awake()
	{
		TerrainWetness.instance = this;
	}

	private void Update()
	{
		if (Scene.WeatherSystem.Raining)
		{
			if (this.WaterLevel < 1f)
			{
				this.WaterLevel += 0.1f * Time.deltaTime;
			}
		}
		else if (this.WaterLevel > 0f)
		{
			this.WaterLevel -= 0.1f * Time.deltaTime;
		}
		if (this.CurrentLevel != this.WaterLevel)
		{
			this.CurrentLevel = this.WaterLevel;
			this.rt1.globalSettingsHolder.TERRAIN_GlobalWetness = this.WaterLevel;
			this.rt1.globalSettingsHolder.Refresh(this.MainTerrain.materialTemplate, null);
		}
	}
}
