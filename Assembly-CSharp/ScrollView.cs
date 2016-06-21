using System;
using UnityEngine;

public class ScrollView : IDisposable
{
	public ScrollView(ref Vector2 scroll)
	{
		scroll = GUILayout.BeginScrollView(scroll, new GUILayoutOption[0]);
	}

	public virtual void Dispose()
	{
		GUILayout.EndScrollView();
	}
}
