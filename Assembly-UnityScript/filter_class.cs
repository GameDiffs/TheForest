using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class filter_class
{
	public bool active;

	public bool foldout;

	public bool curve_foldout;

	public Color color_filter;

	public float strength;

	public int length;

	public bool linked;

	public filter_devices_enum device;

	public condition_type_enum type;

	public device2_type_enum type2;

	public change_mode_enum change_mode;

	public raw_class raw;

	public bool combine;

	public float[] last_value_x;

	public float[] last_value_y;

	public float last_pos_x;

	public bool smooth_y;

	public bool last_value_declared;

	public image_class preimage;

	public Color lerp_color_old;

	public animation_curve_class precurve_x_left;

	public animation_curve_class precurve_x_right;

	public animation_curve_class precurve_y;

	public animation_curve_class precurve_z_left;

	public animation_curve_class precurve_z_right;

	public Rect curve_x_left_menu_rect;

	public Rect curve_x_right_menu_rect;

	public Rect curve_y_menu_rect;

	public Rect curve_z_left_menu_rect;

	public Rect curve_z_right_menu_rect;

	public List<animation_curve_class> precurve_list;

	public animation_curve_class precurve;

	public float curve_position;

	public animation_curve_class prerandom_curve;

	public condition_output_enum output;

	public float range_start;

	public float range_end;

	public string swap_text;

	public bool swap_select;

	public bool copy_select;

	public int color_output_index;

	public int splat_range_length;

	public bool splat_range_foldout;

	public splat_range_class[] splat_range;

	public presubfilter_class presubfilter;

	public bool sub_strength_set;

	public Texture2D preview_texture;

	public int splatmap;

	public int splat_index;

	public int layerMask;

	public float cast_height;

	public float ray_length;

	public float ray_radius;

	public Vector3 ray_direction;

	public raycast_mode_enum raycast_mode;

	public filter_class()
	{
		this.active = true;
		this.curve_foldout = true;
		this.color_filter = new Color(1.5f, 1.5f, 1.5f, (float)1);
		this.strength = (float)1;
		this.length = 1;
		this.linked = true;
		this.last_pos_x = (float)4097;
		this.preimage = new image_class();
		this.precurve_x_left = new animation_curve_class();
		this.precurve_x_right = new animation_curve_class();
		this.precurve_y = new animation_curve_class();
		this.precurve_z_left = new animation_curve_class();
		this.precurve_z_right = new animation_curve_class();
		this.precurve_list = new List<animation_curve_class>();
		this.precurve = new animation_curve_class();
		this.prerandom_curve = new animation_curve_class();
		this.swap_text = "S";
		this.splat_range = new splat_range_class[0];
		this.presubfilter = new presubfilter_class();
		this.cast_height = (float)20;
		this.ray_length = (float)20;
		this.ray_radius = (float)1;
		this.ray_direction = new Vector3((float)0, (float)-1, (float)0);
		this.precurve_list.Add(new animation_curve_class());
		this.precurve_list[0].curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.precurve_list.Add(new animation_curve_class());
		this.precurve_list[0].default_curve = new AnimationCurve(this.precurve_list[0].curve.keys);
		this.precurve_list[1].curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)0);
		this.precurve_list[1].default_curve = new AnimationCurve(this.precurve_list[1].curve.keys);
		this.precurve_list[1].active = false;
		this.precurve_list[1].type = curve_type_enum.Random;
		this.precurve_x_left.curve = AnimationCurve.Linear((float)-1, (float)1, (float)0, (float)0);
		this.precurve_x_left.default_curve = new AnimationCurve(this.precurve_x_left.curve.keys);
		this.precurve_x_right.curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.precurve_x_right.default_curve = new AnimationCurve(this.precurve_x_right.curve.keys);
		this.precurve_y.curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)0);
		this.precurve_y.default_curve = new AnimationCurve(this.precurve_y.curve.keys);
		this.precurve_z_left.curve = AnimationCurve.Linear((float)-1, (float)1, (float)0, (float)0);
		this.precurve_z_left.default_curve = new AnimationCurve(this.precurve_z_left.curve.keys);
		this.precurve_z_right.curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.precurve_z_right.default_curve = new AnimationCurve(this.precurve_z_right.curve.keys);
	}
}
