using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class pattern_class
{
	public bool active;

	public bool foldout;

	public List<Vector2> pattern_placed;

	public condition_output_enum output;

	public float current_x;

	public float current_y;

	public float start_x;

	public float start_y;

	public float width;

	public float height;

	public float width2;

	public float height2;

	public int count_x;

	public int count_y;

	public Vector2 scale;

	public Vector2 scale_start;

	public Vector2 scale_end;

	public bool scale_link;

	public bool scale_link_start_y;

	public bool scale_link_end_y;

	public bool scale_link_start_z;

	public bool scale_link_end_z;

	public float link_scale;

	public Color color;

	public float strength;

	public bool rotate;

	public float rotation;

	public float rotation_start;

	public float rotation_end;

	public Vector2 current;

	public Vector2 pivot;

	public precolor_range_class precolor_range;

	public bool break_x;

	public Texture2D input_texture;

	public float min_distance_x;

	public float min_distance_y;

	public bool min_distance;

	public bool distance_global;

	public int place_max;

	public bool placed_max;

	public pattern_class()
	{
		this.active = true;
		this.pattern_placed = new List<Vector2>();
		this.count_x = 1;
		this.count_y = 1;
		this.scale_start = new Vector2((float)1, (float)1);
		this.scale_end = new Vector2((float)1, (float)1);
		this.scale_link = true;
		this.scale_link_start_y = true;
		this.scale_link_end_y = true;
		this.scale_link_start_z = true;
		this.scale_link_end_z = true;
		this.color = Color.white;
		this.strength = (float)1;
		this.rotation_start = (float)-180;
		this.rotation_end = (float)180;
		this.precolor_range = new precolor_range_class(0, false);
		this.place_max = 100;
	}
}
