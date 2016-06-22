using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class tree_map_class
{
	public Rect rect;

	public bool foldout;

	public bool foldouts;

	public GameObject map;

	public bool load;

	public List<tree_parameter_class> tree_param;

	public List<bool> tree_foldout;

	public int treeTypes;

	public tree_map_class()
	{
		this.load = true;
		this.tree_param = new List<tree_parameter_class>();
		this.tree_foldout = new List<bool>();
	}
}
