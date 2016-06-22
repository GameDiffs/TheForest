using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class object_output_class
{
	public object active;

	public bool foldout;

	public Color color_object;

	public line_placement_class line_placement;

	public object_mode_enum object_mode;

	public bool icon_display;

	public bool objects_active;

	public bool objects_foldout;

	public float strength;

	public bool interface_display;

	public bool object_set;

	public string object_text;

	public float scale;

	public float min_distance_x;

	public float min_distance_z;

	public float min_distance_x_rot;

	public float min_distance_z_rot;

	public bool group_rotation;

	public bool group_rotation_steps;

	public Vector3 group_rotation_step;

	public List<Vector3> objects_placed;

	public int placed;

	[NonSerialized]
	public object_output_class placed_reference;

	public List<Vector3> objects_placed_rot;

	public List<object_class> @object;

	public value_class object_value;

	[NonSerialized]
	public rotation_map_class rotation_map;

	public bool search_active;

	public bool search_erase_doubles;

	public Transform search_object;

	public object_output_class()
	{
		this.foldout = true;
		this.color_object = new Color((float)2, (float)2, (float)2, (float)1);
		this.icon_display = true;
		this.objects_active = true;
		this.objects_foldout = true;
		this.strength = (float)1;
		this.interface_display = true;
		this.object_text = "Object:";
		this.scale = (float)1;
		this.objects_placed = new List<Vector3>();
		this.objects_placed_rot = new List<Vector3>();
		this.@object = new List<object_class>();
		this.object_value = new value_class();
		this.rotation_map = new rotation_map_class();
	}

	public override void set_settings(object_class object1, int object_number, bool all)
	{
		for (int i = 0; i < this.@object.Count; i++)
		{
			if (this.object_value.active[i] || all)
			{
				if (i != object_number)
				{
					this.@object[i].parent = object1.parent;
					this.@object[i].parent_clear = object1.parent_clear;
					this.@object[i].combine = object1.combine;
					this.@object[i].combine_total = object1.combine_total;
					this.@object[i].place_max = object1.place_max;
					this.@object[i].place_maximum = object1.place_maximum;
					this.@object[i].place_maximum_total = object1.place_maximum_total;
				}
				if (this.@object[i].color_object[0] > 0.5f)
				{
					this.@object[i].color_object = this.@object[i].color_object + new Color(-0.5f, (float)0, -0.5f, (float)0);
				}
			}
		}
	}

	public override void set_transform(object_class object1, int object_number, bool all)
	{
		for (int i = 0; i < this.@object.Count; i++)
		{
			if (this.object_value.active[i] || all)
			{
				if (i != object_number)
				{
					this.@object[i].scale_start = object1.scale_start;
					this.@object[i].scale_end = object1.scale_end;
					this.@object[i].scale_link = object1.scale_link;
					this.@object[i].scale_link_start_y = object1.scale_link_start_y;
					this.@object[i].scale_link_end_y = object1.scale_link_end_y;
					this.@object[i].scale_link_start_z = object1.scale_link_start_z;
					this.@object[i].scale_link_end_z = object1.scale_link_end_z;
					this.@object[i].rotation_start = object1.rotation_start;
					this.@object[i].rotation_end = object1.rotation_end;
					this.@object[i].rotation_link = object1.rotation_link;
					this.@object[i].rotation_link_start_y = object1.rotation_link_start_y;
					this.@object[i].rotation_link_end_y = object1.rotation_link_end_y;
					this.@object[i].rotation_link_start_z = object1.rotation_link_start_z;
					this.@object[i].rotation_link_end_z = object1.rotation_link_end_z;
					this.@object[i].terrain_height = object1.terrain_height;
					this.@object[i].position_start = object1.position_start;
					this.@object[i].position_end = object1.position_end;
					this.@object[i].unlink_y = object1.unlink_y;
					this.@object[i].unlink_z = object1.unlink_z;
					this.@object[i].random_position = object1.random_position;
					this.@object[i].scaleCurve = new AnimationCurve(object1.scaleCurve.keys);
					this.@object[i].raycast = object1.raycast;
					this.@object[i].layerMask = object1.layerMask;
					this.@object[i].ray_length = object1.ray_length;
					this.@object[i].cast_height = object1.cast_height;
					this.@object[i].ray_radius = object1.ray_radius;
					this.@object[i].ray_direction = object1.ray_direction;
					this.@object[i].raycast_mode = object1.raycast_mode;
				}
				if (this.@object[i].color_object[0] > 0.5f)
				{
					this.@object[i].color_object = this.@object[i].color_object + new Color(-0.5f, (float)0, -0.5f, (float)0);
				}
			}
		}
	}

	public override void set_rotation(object_class object1, int object_number, bool all)
	{
		for (int i = 0; i < this.@object.Count; i++)
		{
			if (this.object_value.active[i] || all)
			{
				if (i != object_number)
				{
					this.@object[i].rotation_steps = object1.rotation_steps;
					this.@object[i].rotation_step = object1.rotation_step;
					this.@object[i].rotation_map = this.copy_rotation_map(object1.rotation_map);
				}
				if (this.@object[i].color_object[0] > 0.5f)
				{
					this.@object[i].color_object = this.@object[i].color_object + new Color(-0.5f, (float)0, -0.5f, (float)0);
				}
			}
		}
	}

	public override void set_distance(object_class object1, int object_number, bool all)
	{
		for (int i = 0; i < this.@object.Count; i++)
		{
			if (this.object_value.active[i] || all)
			{
				if (i != object_number)
				{
					this.@object[i].min_distance = object1.min_distance;
					this.@object[i].distance_level = object1.distance_level;
					this.@object[i].distance_mode = object1.distance_mode;
					this.@object[i].distance_rotation_mode = object1.distance_rotation_mode;
					this.@object[i].distance_include_scale = object1.distance_include_scale;
					this.@object[i].distance_include_scale_group = object1.distance_include_scale_group;
				}
				if (this.@object[i].color_object[0] > 0.5f)
				{
					this.@object[i].color_object = this.@object[i].color_object + new Color(-0.5f, (float)0, -0.5f, (float)0);
				}
			}
		}
	}

	public override void swap_object(int object_number, int object_number2)
	{
		if (object_number2 > -1 && object_number2 < this.@object.Count)
		{
			object_class value = this.@object[object_number];
			float value2 = this.object_value.value[object_number];
			bool value3 = this.object_value.active[object_number];
			this.@object[object_number] = this.@object[object_number2];
			this.@object[object_number2] = value;
			if (this.@object[object_number].color_object[0] > 0.5f)
			{
				this.@object[object_number].color_object = this.@object[object_number].color_object + new Color(-0.5f, (float)0, -0.5f, (float)0);
			}
			if (this.@object[object_number2].color_object[0] > 0.5f)
			{
				this.@object[object_number2].color_object = this.@object[object_number2].color_object + new Color(-0.5f, (float)0, -0.5f, (float)0);
			}
			this.object_value.value[object_number] = this.object_value.value[object_number2];
			this.object_value.value[object_number2] = value2;
			this.object_value.active[object_number] = this.object_value.active[object_number2];
			this.object_value.active[object_number2] = value3;
			this.object_value.calc_value();
		}
	}

	public override void set_object_text()
	{
		if (this.@object.Count > 1)
		{
			this.object_text = "Objects(" + this.@object.Count + ")";
		}
		else
		{
			this.object_text = "Object";
		}
	}

	public override rotation_map_class copy_rotation_map(rotation_map_class rotation_map)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.rotation_map = rotation_map;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		rotation_map_class result = save_template.rotation_map;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		return result;
	}
}
