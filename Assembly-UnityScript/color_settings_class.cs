using System;
using UnityEngine;

[Serializable]
public class color_settings_class
{
	public Color backgroundColor;

	public bool backgroundActive;

	public Color color_description;

	public Color color_layer;

	public Color color_filter;

	public Color color_subfilter;

	public Color color_colormap;

	public Color color_splat;

	public Color color_tree;

	public Color color_tree_precolor_range;

	public Color color_tree_filter;

	public Color color_tree_subfilter;

	public Color color_grass;

	public Color color_object;

	public Color color_terrain;

	public color_settings_class()
	{
		this.backgroundColor = new Color((float)0, (float)0, (float)0, 0.5f);
		this.color_description = new Color((float)1, 0.45f, (float)0);
		this.color_layer = Color.yellow;
		this.color_filter = Color.cyan;
		this.color_subfilter = Color.green;
		this.color_colormap = Color.white;
		this.color_splat = Color.white;
		this.color_tree = new Color((float)1, 0.7f, 0.4f);
		this.color_tree_precolor_range = new Color((float)1, 0.84f, 0.64f);
		this.color_tree_filter = new Color(0.5f, (float)1, (float)1);
		this.color_tree_subfilter = new Color(0.5f, (float)1, 0.5f);
		this.color_grass = Color.white;
		this.color_object = Color.white;
		this.color_terrain = Color.white;
	}
}
