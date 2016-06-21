using System;
using UnityEngine;

public class GUIArea : IDisposable
{
	public class Rotated : IDisposable
	{
		public Rotated()
		{
			GUIArea.rotated++;
		}

		public void Dispose()
		{
			GUIArea.rotated--;
		}
	}

	private static int rotated;

	public GUIArea() : this(null)
	{
	}

	public GUIArea(Rect? area)
	{
		Rect screenRect = (!area.HasValue) ? GUIArea.GetStandardArea() : area.Value;
		if (GUIArea.rotated > 0)
		{
			screenRect.y += screenRect.height;
			float width = screenRect.width;
			screenRect.width = screenRect.height;
			screenRect.height = width;
		}
		GUILayout.BeginArea(screenRect);
		if (GUIArea.rotated > 0)
		{
			GUIUtility.RotateAroundPivot(-90f, Vector2.zero);
		}
	}

	public static Rect GetStandardArea()
	{
		return new Rect(10f, 10f, 940f, 620f);
	}

	public void Dispose()
	{
		GUILayout.EndArea();
	}
}
