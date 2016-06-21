using System;
using UnityEngine;

public class Horizontal : IDisposable
{
	public Horizontal()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
	}

	public Horizontal(GUIStyle style)
	{
		GUILayout.BeginHorizontal(style, new GUILayoutOption[0]);
	}

	public Horizontal(params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal(options);
	}

	public void Dispose()
	{
		GUILayout.EndHorizontal();
	}
}
