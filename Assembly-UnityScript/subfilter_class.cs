using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class subfilter_class
{
	public bool active;

	public bool foldout;

	public Color color_subfilter;

	public bool linked;

	public condition_type_enum type;

	public subfilter_output_enum output;

	public int output_max;

	public int output_count;

	public float output_count_min;

	public float strength;

	public float range_start;

	public float range_end;

	public int range_count;

	public image_class preimage;

	public precolor_range_class precolor_range;

	public List<animation_curve_class> precurve_list;

	public animation_curve_class precurve;

	public Rect curve_menu_rect;

	public bool curve_inv;

	public float curve_position;

	public animation_curve_class prerandom_curve;

	public Rect random_curve_menu_rect;

	public bool random_curve_inv;

	public string swap_text;

	public bool swap_select;

	public bool copy_select;

	public bool from_tree;

	public raw_class raw;

	public subfilter_mode_enum mode;

	public bool smooth;

	public smooth_method_enum smooth_method;

	public int splatmap;

	public int splat_index;

	public int layerMask;

	public float cast_height;

	public float ray_length;

	public float ray_radius;

	public Vector3 ray_direction;

	public raycast_mode_enum raycast_mode;

	public subfilter_class()
	{
		this.active = true;
		this.color_subfilter = new Color(1.5f, 1.5f, 1.5f, (float)1);
		this.linked = true;
		this.output_max = 1;
		this.output_count_min = 0.5f;
		this.strength = (float)1;
		this.preimage = new image_class();
		this.precolor_range = new precolor_range_class(0, false);
		this.precurve_list = new List<animation_curve_class>();
		this.precurve = new animation_curve_class();
		this.prerandom_curve = new animation_curve_class();
		this.swap_text = "S";
		this.mode = subfilter_mode_enum.strength;
		this.cast_height = (float)20;
		this.ray_length = (float)20;
		this.ray_radius = (float)1;
		this.ray_direction = new Vector3((float)0, (float)-1, (float)0);
		this.precurve_list.Add(new animation_curve_class());
		this.precurve_list[0].curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.precurve_list[0].default_curve = new AnimationCurve(this.precurve_list[0].curve.keys);
		this.precurve_list.Add(new animation_curve_class());
		this.precurve_list[1].curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)0);
		this.precurve_list[1].default_curve = new AnimationCurve(this.precurve_list[1].curve.keys);
		this.precurve_list[1].active = false;
		this.precurve_list[1].type = curve_type_enum.Random;
	}
}
