using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class save_template : MonoBehaviour
{
	public terrain_class preterrain;

	public Terrain terrain;

	public prelayer_class prelayer;

	public layer_class layer;

	public filter_class filter;

	public subfilter_class subfilter;

	public precolor_range_class precolor_range;

	public color_range_class color_range;

	public tree_class tree;

	public object_class @object;

	public rotation_map_class rotation_map;

	public animation_curve_class animation_curve;

	public List<filter_class> filters;

	public List<subfilter_class> subfilters;

	public List<prelayer_class> prelayers;

	public override void Main()
	{
	}
}
