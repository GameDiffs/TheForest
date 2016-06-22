using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PauseMenu : MonoBehaviour
{
	public bool paused;

	public GUITexture pausedGUI;

	public string gameName;

	public List<Transform> myList;

	public PauseMenu()
	{
		this.gameName = "Your Game";
		this.myList = new List<Transform>();
	}

	public override void Start()
	{
		if (this.pausedGUI)
		{
			this.pausedGUI.enabled = false;
		}
	}

	public override void Update()
	{
		if (Input.GetKeyUp(KeyCode.P))
		{
			this.paused = !this.paused;
			if (this.paused)
			{
				Time.timeScale = (float)0;
				if (this.pausedGUI)
				{
					this.pausedGUI.enabled = true;
				}
			}
			else
			{
				Time.timeScale = 1f;
				if (this.pausedGUI)
				{
					this.pausedGUI.enabled = false;
				}
			}
		}
	}

	public override void Main()
	{
	}
}
