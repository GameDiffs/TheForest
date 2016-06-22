using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class map_class
{
	public map_type_enum type;

	public bool active;

	public bool button_parameters;

	public bool button_image_editor;

	public bool button_region;

	public bool button_image_export;

	public bool button_heightmap_export;

	public bool button_converter;

	public bool button_settings;

	public bool button_create_terrain;

	public bool button_help;

	public bool button_update;

	public float alpha;

	public Color backgroundColor;

	public Color titleColor;

	public bool region_popup_edit;

	public bool area_popup_edit;

	public bool disable_region_popup_edit;

	public bool disable_area_popup_edit;

	public List<map_region_class> region;

	public string[] region_popup;

	public int region_select;

	public bool manual_edit;

	public Rect region_rect;

	public Rect area_rect;

	public preimage_edit_class preimage_edit;

	public Color color_fault;

	public Texture2D tex1;

	public Texture2D tex2;

	public Texture2D tex3;

	public FileStream file_tex2;

	public FileStream file_tex3;

	public bool tex_swapped;

	public tile_class tex2_tile;

	public tile_class tex3_tile;

	public WWW elExt_check;

	public bool elExt_check_loaded;

	public bool elExt_check_assign;

	public List<ext_class> elExt;

	public List<ext_class> texExt;

	public List<float> time_start_elExt;

	public List<float> time_start_texExt;

	public int export_texExt;

	public int export_elExt;

	public int mode;

	public bool export_tex3;

	public bool export_tex2;

	public latlong_area_class export_heightmap_area;

	public latlong_area_class export_image_area;

	public tile_class export_pullIndex;

	public int export_pulled;

	public bool export_image_active;

	public bool export_heightmap_active;

	public int export_heightmap_zoom;

	public float export_heightmap_timeStart;

	public float export_heightmap_timeEnd;

	public float export_heightmap_timePause;

	public bool export_heightmap_continue;

	public map_export_class export_heightmap;

	public map_export_class export_image;

	public int export_image_zoom;

	public float export_image_timeStart;

	public float export_image_timeEnd;

	public float export_image_timePause;

	public bool export_image_continue;

	public int export_jpg_quality;

	public bool export_jpg;

	public bool export_png;

	public bool export_raw;

	public Color color;

	public bool key_edit;

	public List<map_key_class> bingKey;

	public int bingKey_selected;

	public float mouse_sensivity;

	public bool path_display;

	public bool warnings;

	public bool track_tile;

	public map_class()
	{
		this.active = true;
		this.button_parameters = true;
		this.button_image_editor = true;
		this.button_region = true;
		this.button_image_export = true;
		this.button_heightmap_export = true;
		this.button_settings = true;
		this.alpha = 0.65f;
		this.region = new List<map_region_class>();
		this.preimage_edit = new preimage_edit_class();
		this.tex2_tile = new tile_class();
		this.tex3_tile = new tile_class();
		this.elExt = new List<ext_class>();
		this.texExt = new List<ext_class>();
		this.time_start_elExt = new List<float>();
		this.time_start_texExt = new List<float>();
		this.export_texExt = 8;
		this.export_elExt = 16;
		this.export_heightmap_area = new latlong_area_class();
		this.export_image_area = new latlong_area_class();
		this.export_pullIndex = new tile_class();
		this.export_heightmap_continue = true;
		this.export_heightmap = new map_export_class();
		this.export_image = new map_export_class();
		this.export_image_continue = true;
		this.export_jpg_quality = 100;
		this.export_jpg = true;
		this.color = Color.red;
		this.bingKey = new List<map_key_class>();
		this.mouse_sensivity = (float)2;
		this.warnings = true;
		this.track_tile = true;
		this.make_region_popup();
	}

	public override void make_region_popup()
	{
		this.region_popup = new string[this.region.Count];
		for (int i = 0; i < this.region.Count; i++)
		{
			this.region_popup[i] = this.region[i].name;
		}
	}
}
