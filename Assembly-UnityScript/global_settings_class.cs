using System;
using UnityEngine;

[Serializable]
public class global_settings_class
{
	public color_settings_class color;

	public bool color_scheme_display;

	public bool color_scheme;

	public bool toggle_text_no;

	public bool toggle_text_short;

	public bool toggle_text_long;

	public bool tooltip_text_no;

	public bool tooltip_text_short;

	public bool tooltip_text_long;

	public bool mac_mode;

	public int tooltip_mode;

	public WWW myExt;

	public WWW myExt2;

	public WWW myExt3;

	public WWW myExt4;

	public bool restrict_resolutions;

	public bool load_terrain_data;

	public bool rtp;

	public bool video_help;

	public bool view_only_output;

	public float save_global_timer;

	public WWW download;

	public int downloading;

	public bool download_foldout;

	public bool download_display;

	public WWW download2;

	public int downloading2;

	public bool download_foldout2;

	public bool download_display2;

	public WWW wc_contents;

	public int wc_loading;

	public float old_version;

	public float new_version;

	public bool update_display;

	public bool update_display2;

	public bool update_version;

	public bool update_version2;

	public string[] update;

	public float time_out;

	public bool button_export;

	public bool button_measure;

	public bool button_capture;

	public bool button_tools;

	public bool button_tiles;

	public bool button_node;

	public bool button_world;

	public bool example_display;

	public int example_resolution;

	public int example_tiles;

	public int example_terrain;

	public int example_terrain_old1;

	public bool example_tree_active;

	public bool example_grass_active;

	public bool example_object_active;

	public int example_buttons;

	public global_settings_class()
	{
		this.color = new color_settings_class();
		this.color_scheme = true;
		this.toggle_text_short = true;
		this.tooltip_text_long = true;
		this.tooltip_mode = 2;
		this.restrict_resolutions = true;
		this.video_help = true;
		this.view_only_output = true;
		this.save_global_timer = (float)5;
		this.download_foldout = true;
		this.download_display = true;
		this.download_foldout2 = true;
		this.download_display2 = true;
		this.update = new string[]
		{
			"Don't check",
			"Notify",
			"Download and notify",
			"Download,import and notify",
			"Download and import automatically"
		};
		this.button_export = true;
		this.button_measure = true;
		this.button_capture = true;
		this.button_tools = true;
		this.button_tiles = true;
		this.button_node = true;
		this.button_world = true;
		this.example_display = true;
		this.example_resolution = 3;
		this.example_tiles = 2;
		this.example_terrain_old1 = -1;
		this.example_tree_active = true;
		this.example_grass_active = true;
		this.example_object_active = true;
		this.example_buttons = 1;
	}
}
