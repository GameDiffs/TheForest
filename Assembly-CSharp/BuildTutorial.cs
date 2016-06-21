using System;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class BuildTutorial : MonoBehaviour
{
	public static bool Step1Shown;

	public static bool Step2Shown;

	public static bool Step3Shown;

	public bool Step1;

	public bool Step2;

	public bool Step3;

	private bool Show = true;

	private void OnDeserialized()
	{
	}

	private void Start()
	{
		if (this.Show)
		{
			if (this.Step1 && !BuildTutorial.Step1Shown)
			{
				LocalPlayer.Tuts.ShowStep1Tut();
			}
			if (this.Step2 && !BuildTutorial.Step2Shown)
			{
				LocalPlayer.Tuts.ShowStep2Tut();
			}
			if (this.Step3 && !BuildTutorial.Step3Shown)
			{
				LocalPlayer.Tuts.ShowStep3Tut();
			}
		}
	}
}
