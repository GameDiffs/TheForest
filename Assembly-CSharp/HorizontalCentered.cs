using System;
using UnityEngine;

public class HorizontalCentered : IDisposable
{
	public HorizontalCentered()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
	}

	public HorizontalCentered(GUIStyle style)
	{
		GUILayout.BeginHorizontal(style, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
	}

	public void Dispose()
	{
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
