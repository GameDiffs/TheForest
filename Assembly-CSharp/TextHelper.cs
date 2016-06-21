using System;
using UnityEngine;

public static class TextHelper
{
	public static string FixTo(this string str, float width)
	{
		return str.FixTo(width, "label");
	}

	public static string FixTo(this string str, float width, string type)
	{
		float x = GUI.skin.GetStyle(type).CalcSize(new GUIContent("\t")).x;
		float x2 = GUI.skin.GetStyle(type).CalcSize(new GUIContent(".")).x;
		float num = Mathf.Max(1f, GUI.skin.GetStyle(type).CalcSize(new GUIContent(". .")).x - 2f * x2);
		float x3 = GUI.skin.GetStyle(type).CalcSize(new GUIContent(str)).x;
		return str + new string(' ', (int)((width - x - x3) / num) + 1) + "\t";
	}
}
