using System;
using UnityEngine;

public class WindControl : MonoBehaviour
{
	public enum WindD
	{
		NoWind,
		North,
		West,
		East,
		South
	}

	public WindControl.WindD windDirection;

	public float windSpeed = 0.5f;

	public static int windV;

	public static float windS = 0.5f;

	public void Start()
	{
		switch (this.windDirection)
		{
		case WindControl.WindD.NoWind:
			WindControl.windV = 0;
			break;
		case WindControl.WindD.North:
			WindControl.windV = 1;
			break;
		case WindControl.WindD.West:
			WindControl.windV = 2;
			break;
		case WindControl.WindD.East:
			WindControl.windV = 3;
			break;
		case WindControl.WindD.South:
			WindControl.windV = 4;
			break;
		}
		WindControl.windS = this.windSpeed;
	}
}
