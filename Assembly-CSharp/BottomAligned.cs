using System;
using UnityEngine;

public class BottomAligned : IDisposable
{
	public BottomAligned()
	{
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
	}

	public void Dispose()
	{
		GUILayout.EndVertical();
	}
}
