using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class pattern_tool_class
{
	public bool active;

	public Vector2 resolution_display;

	public float scale;

	public bool clear;

	public bool first;

	public int place_total;

	public Texture2D output_texture;

	public Vector2 output_resolution;

	public List<pattern_class> patterns;

	public pattern_class current_pattern;

	public string export_file;

	public string export_path;

	public pattern_tool_class()
	{
		this.resolution_display = new Vector2((float)512, (float)512);
		this.scale = (float)1;
		this.clear = true;
		this.output_resolution = new Vector2((float)512, (float)512);
		this.patterns = new List<pattern_class>();
		this.current_pattern = new pattern_class();
		this.export_file = string.Empty;
	}
}
