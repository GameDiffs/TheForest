using System;
using UnityEngine;

public class LoadLevel_Smoke_02 : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void NextLevelButton(string levelName)
	{
		Application.LoadLevel("Flamethrower");
	}

	public void OnGUI()
	{
		if (GUI.Button(new Rect(70f, 70f, 100f, 30f), "Next Effects"))
		{
			Application.LoadLevel("Flamethrower");
		}
	}
}
