using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class line_list_class
{
	public Color color;

	public bool foldout;

	public bool point_foldout;

	public int point_length;

	public List<Vector3> points;

	public line_list_class()
	{
		this.color = new Color((float)1, (float)0, (float)0);
		this.points = new List<Vector3>();
	}
}
