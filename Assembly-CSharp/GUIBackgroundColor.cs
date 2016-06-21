using System;
using UnityEngine;

public class GUIBackgroundColor : IDisposable
{
	private Color old;

	public GUIBackgroundColor(Color color)
	{
		this.old = GUI.backgroundColor;
		GUI.backgroundColor = color;
	}

	public void Dispose()
	{
		GUI.backgroundColor = this.old;
	}
}
