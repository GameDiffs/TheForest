using System;
using UnityEngine;

public class GUIScale : IDisposable
{
	private static int count;

	private static Matrix4x4 cached;

	public GUIScale()
	{
		if (GUIScale.count++ == 0)
		{
			GUIScale.cached = GUI.matrix;
			if (Screen.width < 500)
			{
				GUIUtility.ScaleAroundPivot(new Vector2(0.5f, 0.5f), Vector2.zero);
			}
			if (Screen.width > 1100)
			{
				GUIUtility.ScaleAroundPivot(new Vector2(2f, 2f), Vector2.zero);
			}
		}
	}

	public void Dispose()
	{
		if (--GUIScale.count == 0)
		{
			GUI.matrix = GUIScale.cached;
		}
	}
}
