using System;
using UnityEngine;

public class RightAligned : IDisposable
{
	public RightAligned()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
	}

	public void Dispose()
	{
		GUILayout.EndHorizontal();
	}
}
