using System;
using UnityEngine;

public class Vertical : IDisposable
{
	public Vertical(params GUILayoutOption[] options)
	{
		GUILayout.BeginVertical(options);
	}

	public void Dispose()
	{
		GUILayout.EndVertical();
	}
}
