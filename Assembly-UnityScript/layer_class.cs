using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class layer_class
{
	public float software_id;

	public bool active;

	public bool foldout;

	public Color color_layer;

	public layer_output_enum output;

	public float strength;

	public bool nonOverlap;

	public float zoom;

	public Vector2 offset;

	public Vector2 offset_range;

	public Vector2 offset_middle;

	public bool drawn;

	public string text;

	public string text_placed;

	public Rect rect;

	public bool edit;

	public bool disable_edit;

	public bool smooth;

	public remarks_class remarks;

	public height_output_class height_output;

	public color_output_class color_output;

	public splat_output_class splat_output;

	public tree_output_class tree_output;

	public grass_output_class grass_output;

	public object_output_class object_output;

	public Rect menu_rect;

	public string swap_text;

	public bool swap_select;

	public bool copy_select;

	public prefilter_class prefilter;

	public List<distance_class> objects_placed;

	public layer_class()
	{
		this.active = true;
		this.color_layer = new Color(1.5f, 1.5f, 1.5f, (float)1);
		this.output = layer_output_enum.color;
		this.strength = (float)1;
		this.zoom = (float)1;
		this.offset_range = new Vector2((float)5, (float)5);
		this.text = string.Empty;
		this.text_placed = string.Empty;
		this.remarks = new remarks_class();
		this.height_output = new height_output_class();
		this.color_output = new color_output_class();
		this.splat_output = new splat_output_class();
		this.tree_output = new tree_output_class();
		this.grass_output = new grass_output_class();
		this.object_output = new object_output_class();
		this.swap_text = "S";
		this.prefilter = new prefilter_class();
		this.objects_placed = new List<distance_class>();
		this.object_output = new object_output_class();
	}
}
