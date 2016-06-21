using System;
using UnityEngine;

public class LooseFocus : MonoBehaviour
{
	private bool Started;

	private void Awake()
	{
		base.Invoke("StartMe", 1f);
	}

	private void OnApplicationFocus()
	{
		if (this.Started)
		{
			Screen.SetResolution(800, 600, false);
		}
	}

	private void StartMe()
	{
		this.Started = true;
	}
}
