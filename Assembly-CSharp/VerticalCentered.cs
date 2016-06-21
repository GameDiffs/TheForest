using System;
using UnityEngine;

public class VerticalCentered : IDisposable
{
	public VerticalCentered()
	{
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
	}

	public VerticalCentered(params GUILayoutOption[] options)
	{
		GUILayout.BeginVertical(options);
		GUILayout.FlexibleSpace();
	}

	public void Dispose()
	{
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
}
