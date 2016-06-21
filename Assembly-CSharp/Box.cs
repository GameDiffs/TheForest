using System;
using UnityEngine;

public class Box : IDisposable
{
	public Box(GUIStyle style)
	{
		GUILayout.BeginVertical(style, new GUILayoutOption[0]);
	}

	public Box(params GUILayoutOption[] options)
	{
		GUILayout.BeginVertical("box", options);
	}

	public Box()
	{
		GUILayout.BeginVertical("box", new GUILayoutOption[0]);
	}

	public virtual void Dispose()
	{
		GUILayout.EndVertical();
	}
}
