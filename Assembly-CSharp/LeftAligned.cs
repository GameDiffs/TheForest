using System;
using UnityEngine;

public class LeftAligned : IDisposable
{
	public LeftAligned()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
	}

	public void Dispose()
	{
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
