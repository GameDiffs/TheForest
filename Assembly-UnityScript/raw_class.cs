using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class raw_class
{
	public bool foldout;

	public bool settings_foldout;

	public int raw_number;

	public List<int> file_index;

	public List<bool> file_foldout;

	public string path;

	public bool tile_offset;

	public bool flip_x;

	public bool flip_y;

	public bool clamp;

	public int list_length;

	public int list_row;

	public bool display_short_list;

	public list_condition_enum raw_list_mode;

	public image_mode_enum raw_mode;

	public object @object;

	public auto_search_class auto_search;

	public bool raw_auto_scale;

	public Vector2 conversion_step;

	public float tile_x;

	public float tile_y;

	public bool tile_link;

	public float tile_offset_x;

	public float tile_offset_y;

	public bool rgb;

	public bool rotation;

	public float rotation_value;

	public bool output;

	public float output_pos;

	public raw_class()
	{
		this.foldout = true;
		this.file_index = new List<int>();
		this.file_foldout = new List<bool>();
		this.path = string.Empty;
		this.list_length = 1;
		this.list_row = 4;
		this.auto_search = new auto_search_class();
		this.raw_auto_scale = true;
		this.conversion_step = new Vector2((float)1, (float)1);
		this.tile_x = (float)1;
		this.tile_y = (float)1;
		this.rgb = true;
		this.file_index.Add(-1);
		this.file_foldout.Add(true);
	}

	public override void adjust_list()
	{
		int num = this.list_length - this.file_index.Count;
		int i = 0;
		if (num > 0)
		{
			for (i = 0; i < num; i++)
			{
				this.file_index.Add(-1);
				this.file_foldout.Add(false);
			}
		}
		if (num < 0)
		{
			num *= -1;
			for (i = 0; i < num; i++)
			{
				this.file_index.RemoveAt(this.file_index.Count - 1);
				this.file_foldout.RemoveAt(this.file_foldout.Count - 1);
			}
		}
	}

	public override void set_raw_auto_scale(terrain_class preterrain1, Rect area, List<raw_file_class> raw_files, int raw_number)
	{
		if (raw_number < this.file_index.Count && raw_files[this.file_index[raw_number]].assigned && preterrain1 != null)
		{
			if (this.raw_mode == image_mode_enum.Area)
			{
				this.conversion_step.x = area.width / (raw_files[this.file_index[raw_number]].resolution.x - (float)1);
				this.conversion_step.y = area.height / (raw_files[this.file_index[raw_number]].resolution.y - (float)1);
			}
			else if (this.raw_mode == image_mode_enum.Terrain)
			{
				if (preterrain1.terrain)
				{
					this.conversion_step.x = preterrain1.terrain.terrainData.size.x / (raw_files[this.file_index[raw_number]].resolution.x - (float)1);
					this.conversion_step.y = preterrain1.terrain.terrainData.size.z / (raw_files[this.file_index[raw_number]].resolution.y - (float)1);
				}
			}
			else if (this.raw_mode == image_mode_enum.MultiTerrain)
			{
				this.conversion_step.x = preterrain1.terrain.terrainData.size.x * preterrain1.tiles.x / (raw_files[this.file_index[raw_number]].resolution.x - (float)1);
				this.conversion_step.y = preterrain1.terrain.terrainData.size.z * preterrain1.tiles.y / (raw_files[this.file_index[raw_number]].resolution.y - (float)1);
			}
		}
	}
}
