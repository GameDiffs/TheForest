using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class image_class
{
	public precolor_range_class precolor_range;

	public bool settings_foldout;

	public int image_number;

	public List<Texture2D> image;

	public Texture2D texture;

	public bool tile_offset;

	public Color image_color;

	public AnimationCurve image_curve;

	public bool splatmap;

	public bool flip_x;

	public bool flip_y;

	public bool clamp;

	public int list_length;

	public int list_row;

	public list_condition_enum image_list_mode;

	public image_mode_enum image_mode;

	public select_mode_enum select_mode;

	public int import_max_size;

	public int import_max_size_list;

	public bool short_list;

	public bool image_auto_scale;

	public Vector2 conversion_step;

	public float tile_x;

	public float tile_y;

	public float tile_offset_x;

	public float tile_offset_y;

	public bool rgb;

	public bool rotation;

	public float rotation_value;

	public bool output;

	public float output_pos;

	public float output_alpha;

	public bool edge_blur;

	public float edge_blur_radius;

	public float alpha;

	public auto_search_class auto_search;

	public image_class()
	{
		this.precolor_range = new precolor_range_class(0, false);
		this.image = new List<Texture2D>();
		this.image_color = Color.white;
		this.list_length = 1;
		this.list_row = 4;
		this.image_auto_scale = true;
		this.conversion_step = new Vector2((float)1, (float)1);
		this.tile_x = (float)1;
		this.tile_y = (float)1;
		this.rgb = true;
		this.edge_blur_radius = (float)1;
		this.auto_search = new auto_search_class();
		this.image.Add(this.texture);
		this.auto_search.extension = ".png";
	}

	public override void set_image_auto_tile(terrain_class preterrain)
	{
	}

	public override void adjust_list()
	{
		int num = this.list_length - this.image.Count;
		int i = 0;
		if (num > 0)
		{
			for (i = 0; i < num; i++)
			{
				this.image.Add(new Texture2D(1, 1));
			}
		}
		if (num < 0)
		{
			num *= -1;
			for (i = 0; i < num; i++)
			{
				this.image.RemoveAt(this.image.Count - 1);
			}
		}
	}

	public override void set_image_auto_scale(terrain_class preterrain1, Rect area, int image_number)
	{
		if (image_number < this.image.Count && this.image[image_number] && preterrain1 != null)
		{
			if (this.image_mode == image_mode_enum.Area)
			{
				this.conversion_step.x = area.width / (float)(this.image[image_number].width - 1);
				this.conversion_step.y = area.height / (float)(this.image[image_number].height - 1);
			}
			else if (this.image_mode == image_mode_enum.Terrain)
			{
				if (preterrain1.terrain)
				{
					this.conversion_step.x = preterrain1.terrain.terrainData.size.x / (float)(this.image[image_number].width - 1);
					this.conversion_step.y = preterrain1.terrain.terrainData.size.z / (float)(this.image[image_number].height - 1);
				}
			}
			else if (this.image_mode == image_mode_enum.MultiTerrain)
			{
				this.conversion_step.x = preterrain1.terrain.terrainData.size.x * preterrain1.tiles.x / (float)(this.image[image_number].width - 1);
				this.conversion_step.y = preterrain1.terrain.terrainData.size.z * preterrain1.tiles.y / (float)(this.image[image_number].height - 1);
			}
		}
	}
}
