using System;
using UnityEngine;

[Serializable]
public class heightmap_tool_class
{
	public bool active;

	public Vector2 resolution_display;

	public float scale;

	public bool clear;

	public bool first;

	public int place_total;

	public float pow_strength;

	public float scroll_offset;

	public perlin_class perlin;

	public Texture2D output_texture;

	public Texture2D preview_texture;

	public float output_resolution;

	public float preview_resolution;

	public int preview_resolution_slider;

	public string export_file;

	public string export_path;

	public export_mode_enum export_mode;

	public raw_file_class raw_save_file;

	public heightmap_tool_class()
	{
		this.resolution_display = new Vector2((float)512, (float)512);
		this.scale = (float)1;
		this.clear = true;
		this.scroll_offset = 0.1f;
		this.perlin = new perlin_class();
		this.output_resolution = (float)2049;
		this.preview_resolution = (float)128;
		this.preview_resolution_slider = 2;
		this.export_file = string.Empty;
		this.raw_save_file = new raw_file_class();
	}
}
