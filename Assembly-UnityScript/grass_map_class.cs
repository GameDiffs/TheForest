using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class grass_map_class
{
	public Rect rect;

	public bool foldout;

	public bool foldouts;

	public GameObject map;

	public bool load;

	public List<grass_parameter_class> grass_param;

	public int grassTypes;

	public List<bool> grass_foldout;

	public grass_map_class()
	{
		this.load = true;
		this.grass_param = new List<grass_parameter_class>();
		this.grass_foldout = new List<bool>();
	}
}
