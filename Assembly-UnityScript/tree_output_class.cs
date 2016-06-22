using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class tree_output_class
{
	public bool active;

	public Color color_tree;

	public bool foldout;

	public float strength;

	public bool interface_display;

	public bool icon_display;

	public bool trees_active;

	public bool trees_foldout;

	public int tree_terrain;

	public bool terrain_tree_assigned;

	public string tree_text;

	public float scale;

	public List<tree_class> tree;

	public value_class tree_value;

	public int placed;

	[NonSerialized]
	public tree_output_class placed_reference;

	public tree_output_class()
	{
		this.color_tree = new Color((float)2, (float)2, (float)2, (float)1);
		this.foldout = true;
		this.strength = (float)1;
		this.interface_display = true;
		this.icon_display = true;
		this.trees_active = true;
		this.trees_foldout = true;
		this.tree_text = "Tree:";
		this.scale = (float)1;
		this.tree = new List<tree_class>();
		this.tree_value = new value_class();
	}

	public override void set_scale(tree_class tree1, int tree_number, bool all)
	{
		for (int i = 0; i < this.tree.Count; i++)
		{
			if (this.tree_value.active[i] || all)
			{
				if (i != tree_number)
				{
					this.tree[i].link_start = tree1.link_start;
					this.tree[i].link_end = tree1.link_end;
					this.tree[i].width_start = tree1.width_start;
					this.tree[i].width_end = tree1.width_end;
					this.tree[i].height_start = tree1.height_start;
					this.tree[i].height_end = tree1.height_end;
					this.tree[i].unlink = tree1.unlink;
					this.tree[i].random_position = tree1.random_position;
					this.tree[i].height = tree1.height;
					this.tree[i].raycast = tree1.raycast;
					this.tree[i].layerMask = tree1.layerMask;
					this.tree[i].ray_length = tree1.ray_length;
					this.tree[i].cast_height = tree1.cast_height;
					this.tree[i].ray_radius = tree1.ray_radius;
					this.tree[i].ray_direction = tree1.ray_direction;
					this.tree[i].raycast_mode = tree1.raycast_mode;
				}
				if (this.tree[i].color_tree[0] < 1.5f)
				{
					this.tree[i].color_tree = this.tree[i].color_tree + new Color(0.5f, 0.5f, 0.5f, (float)0);
				}
			}
		}
	}

	public override void set_distance(tree_class tree1, int tree_number, bool all)
	{
		for (int i = 0; i < this.tree.Count; i++)
		{
			if (this.tree_value.active[i] || all)
			{
				if (i != tree_number)
				{
					this.tree[i].min_distance = tree1.min_distance;
					this.tree[i].distance_level = tree1.distance_level;
					this.tree[i].distance_mode = tree1.distance_mode;
					this.tree[i].distance_rotation_mode = tree1.distance_rotation_mode;
					this.tree[i].distance_include_scale = tree1.distance_include_scale;
					this.tree[i].distance_include_scale_group = tree1.distance_include_scale_group;
				}
				if (this.tree[i].color_tree[0] < 1.5f)
				{
					this.tree[i].color_tree = this.tree[i].color_tree + new Color(0.5f, 0.5f, 0.5f, (float)0);
				}
			}
		}
	}

	public override void add_tree(int tree_number, terraincomposer_save script, bool new_filter)
	{
		this.tree.Insert(tree_number, new tree_class(script, new_filter));
		this.tree_value.add_value(tree_number, (float)50);
		this.set_tree_text();
	}

	public override void erase_tree(int tree_number, terraincomposer_save script)
	{
		if (this.tree.Count > 0)
		{
			script.erase_filters(this.tree[tree_number].prefilter);
			this.tree.RemoveAt(tree_number);
			this.tree_value.erase_value(tree_number);
			this.set_tree_text();
		}
	}

	public override void clear_tree(terraincomposer_save script)
	{
		int count = this.tree.Count;
		for (int i = 0; i < count; i++)
		{
			this.erase_tree(this.tree.Count - 1, script);
		}
	}

	public override void swap_tree(int tree_number, int tree_number2)
	{
		if (tree_number2 > -1 && tree_number2 < this.tree.Count)
		{
			tree_class value = this.tree[tree_number];
			float num = this.tree_value.value[tree_number];
			this.tree[tree_number] = this.tree[tree_number2];
			this.tree[tree_number2] = value;
			if (this.tree[tree_number].color_tree[0] < 1.5f)
			{
				this.tree[tree_number].color_tree = this.tree[tree_number].color_tree + new Color(0.5f, 0.5f, 0.5f, (float)0);
			}
			if (this.tree[tree_number2].color_tree[0] < 1.5f)
			{
				this.tree[tree_number2].color_tree = this.tree[tree_number2].color_tree + new Color(0.5f, 0.5f, 0.5f, (float)0);
			}
			this.tree_value.swap_value(tree_number, tree_number2);
		}
	}

	public override void set_tree_text()
	{
		if (this.tree.Count > 1)
		{
			this.tree_text = "Trees(" + this.tree.Count + ")";
		}
		else
		{
			this.tree_text = "Tree(" + this.tree.Count + ")";
		}
	}
}
