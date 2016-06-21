using System;
using TheForest.Buildings.Creation;
using UnityEngine;

public class ClearStaticVars : MonoBehaviour
{
	public bool MainScene = true;

	private void Awake()
	{
		BuildMission.ActiveMissions.Clear();
		Clock.Day = 0;
		if (!this.MainScene)
		{
			RainEffigy.RainAdd = 0;
		}
		Time.timeScale = 1f;
	}
}
