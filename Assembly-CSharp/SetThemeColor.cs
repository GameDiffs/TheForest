using System;
using UnityEngine;

public class SetThemeColor : MonoBehaviour
{
	public void SetR()
	{
		GetThemeColor.COLOR_R = (byte)Mathf.RoundToInt(255f * UIProgressBar.current.value);
	}

	public void SetG()
	{
		GetThemeColor.COLOR_G = (byte)Mathf.RoundToInt(255f * UIProgressBar.current.value);
	}

	public void SetB()
	{
		GetThemeColor.COLOR_B = (byte)Mathf.RoundToInt(255f * UIProgressBar.current.value);
	}
}
