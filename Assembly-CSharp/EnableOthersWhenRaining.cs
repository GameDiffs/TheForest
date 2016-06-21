using System;
using TheForest.Utils;
using UnityEngine;

public class EnableOthersWhenRaining : MonoBehaviour
{
	private bool wasRaining;

	private void Start()
	{
		this.UpdateBehaviours();
	}

	private void Update()
	{
		if (Scene.WeatherSystem.Raining != this.wasRaining)
		{
			this.UpdateBehaviours();
			this.wasRaining = Scene.WeatherSystem.Raining;
		}
	}

	private void UpdateBehaviours()
	{
		Behaviour[] components = base.GetComponents<Behaviour>();
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] != this)
			{
				components[i].enabled = Scene.WeatherSystem.Raining;
			}
		}
	}
}
