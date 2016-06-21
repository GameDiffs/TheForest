using System;
using UnityEngine;

public class prefs : MonoBehaviour
{
	private void Start()
	{
		Debug.LogError("PlayerPrefs.DeleteAll");
		PlayerPrefs.DeleteAll();
	}

	private void Update()
	{
	}
}
