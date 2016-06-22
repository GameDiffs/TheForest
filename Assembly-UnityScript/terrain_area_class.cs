using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class terrain_area_class
{
	public List<terrain_class2> terrains;

	public int index;

	public tile_class tiles;

	public tile_class tiles_select;

	public int tiles_total;

	public int tiles_select_total;

	public int tiles_assigned_total;

	public bool tiles_select_link;

	public Vector3 size;

	public Vector3 center;

	public bool edit;

	public bool disable_edit;

	public bool area_foldout;

	public bool tiles_foldout;

	public bool settings_foldout;

	public bool center_synchronous;

	public bool tile_synchronous;

	public bool tile_position_synchronous;

	public Rect rect;

	public Rect rect1;

	public string text;

	public string text_edit;

	public bool display_short;

	public remarks_class remarks;

	public bool copy_settings;

	public int copy_terrain;

	public bool foldout;

	public bool terrains_active;

	public bool terrains_scene_active;

	public bool terrains_foldout;

	public auto_search_class auto_search;

	public auto_search_class auto_name;

	public string path;

	public Transform parent;

	public string scene_name;

	public string asset_name;

	public bool resize;

	public bool resize_left;

	public bool resize_right;

	public bool resize_top;

	public bool resize_bottom;

	public bool resize_topLeft;

	public bool resize_topRight;

	public bool resize_bottomLeft;

	public bool resize_bottomRight;

	public bool resize_center;

	public terrain_area_class()
	{
		this.terrains = new List<terrain_class2>();
		this.tiles = new tile_class();
		this.tiles_select = new tile_class();
		this.tiles_select_link = true;
		this.center = new Vector3((float)0, (float)0, (float)0);
		this.center_synchronous = true;
		this.tile_synchronous = true;
		this.tile_position_synchronous = true;
		this.text_edit = string.Empty;
		this.remarks = new remarks_class();
		this.copy_settings = true;
		this.foldout = true;
		this.terrains_active = true;
		this.terrains_scene_active = true;
		this.terrains_foldout = true;
		this.auto_search = new auto_search_class();
		this.auto_name = new auto_search_class();
		this.scene_name = "Terrain";
		this.asset_name = "New Terrain";
		this.set_terrain_text();
	}

	public override void clear()
	{
		this.terrains.Clear();
		this.set_terrain_text();
	}

	public override void clear_to_one()
	{
		int count = this.terrains.Count;
		for (int i = 1; i < count; i++)
		{
			this.terrains.RemoveAt(1);
		}
		this.set_terrain_text();
	}

	public override void set_terrain_text()
	{
		if (this.text_edit.Length == 0)
		{
			if (this.terrains.Count > 1)
			{
				this.text = "Terrains";
			}
			else
			{
				this.text = "Terrain";
			}
		}
		else
		{
			this.text = this.text_edit;
		}
		this.text += " (" + this.terrains.Count.ToString() + ")";
	}
}
