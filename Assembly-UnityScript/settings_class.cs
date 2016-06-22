using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class settings_class
{
	public bool example_display;

	public bool terrainDataDisplay;

	public bool cull_optimizer;

	public float terrainMinHeight;

	public float terrainMaxHeight;

	public float terrainMinDegree;

	public float terrainMaxDegree;

	public Rect area_max;

	public color_settings_class color;

	public bool color_scheme_display_foldout;

	public bool remarks;

	public bool tips;

	public bool tip_local_area_foldout;

	public string tip_local_area_text;

	public bool parentObjectsTerrain;

	public int prelayers_linked;

	public int filters_linked;

	public int subfilters_linked;

	public bool database_display;

	public int filter_foldout_index;

	public int subfilter_foldout_index;

	public bool update_display;

	public bool update_display2;

	public bool update_version;

	public bool update_version2;

	public float old_version;

	public float new_version;

	public string[] update;

	public float time_out;

	public bool project_prefab;

	public bool copy_terrain_material;

	public float grass_density;

	public int smooth_angle;

	public int round_angle;

	public bool resolution_density;

	public int resolution_density_min;

	public float resolution_density_conversion;

	public bool run_in_background;

	public bool display_bar_auto_generate;

	public float global_height_strength;

	public float global_height_level;

	public float global_degree_strength;

	public float global_degree_level;

	public bool global_parameters;

	public auto_search_class colormap_auto_search;

	public auto_search_class normalmap_auto_search;

	public bool colormap;

	public bool colormap_auto_assign;

	public bool colormap_assign;

	public bool normalmap;

	public bool normalmap_auto_assign;

	public float top_height;

	public Rect top_rect;

	public bool box_scheme;

	public bool display_color_curves;

	public bool display_mix_curves;

	public bool display_log;

	public bool display_filename;

	public bool filter_select_text;

	public bool display_short_terrain;

	public bool display_short_mesh;

	public int loading;

	public bool tabs;

	public WWW contents;

	public WWW myExt;

	public bool ipr;

	public bool button_globe;

	public bool showTerrains;

	public bool showMeshes;

	public bool help_splat_textures_foldout;

	public bool help_grass_foldout;

	public bool help_heightmap_layer_foldout;

	public bool terrain_settings;

	public bool terrain_settings_foldout;

	public int editor_basemap_distance_max;

	public int editor_detail_distance_max;

	public int editor_tree_distance_max;

	public int editor_fade_length_max;

	public int editor_mesh_trees_max;

	public int runtime_basemap_distance_max;

	public int runtime_detail_distance_max;

	public int runtime_tree_distance_max;

	public int runtime_fade_length_max;

	public int runtime_mesh_trees_max;

	public int terrain_tiles_max;

	public bool settings_editor;

	public bool settings_runtime;

	public bool auto_fit_terrains;

	public splatPrototype_class[] color_splatPrototypes;

	public bool splat_apply_all;

	public bool stitch_heightmap;

	public bool stitch_splatmap;

	public string raw_search_pattern;

	public string raw_search_filename;

	public string raw_search_extension;

	public bool tree_button;

	public List<tree_map_class> treemap;

	public bool tree_foldouts;

	public bool tree_actives;

	public bool grass_button;

	public List<grass_map_class> grassmap;

	public bool grass_foldouts;

	public bool grass_actives;

	public bool runtime_create_terrain;

	public bool direct_colormap;

	public bool mesh_button;

	public Material mesh_material;

	public string mesh_path;

	public bool light_button;

	public Light directional_light;

	public GameObject measure_distance_prefab;

	public GameObject measure_distance1;

	public GameObject measure_distance2;

	public int measure_distance_mode;

	public float measure_distance;

	public bool load_colormap;

	public bool load_normalmap;

	public bool load_treemap;

	public bool load_controlmap;

	public bool load_bumpglobal;

	public bool load_layers;

	public bool load_layers_settings;

	public bool export_heightmap_combined;

	public settings_class()
	{
		this.example_display = true;
		this.cull_optimizer = true;
		this.area_max = new Rect((float)-256, (float)-256, (float)512, (float)512);
		this.color = new color_settings_class();
		this.color_scheme_display_foldout = true;
		this.remarks = true;
		this.tips = true;
		this.tip_local_area_foldout = true;
		this.prelayers_linked = -1;
		this.filters_linked = -1;
		this.subfilters_linked = -1;
		this.update = new string[]
		{
			"Don't check for updates",
			"Notify updates",
			"Download updates and notify",
			"Download updates,import and notify",
			"Download updates and import automatically"
		};
		this.grass_density = (float)32;
		this.smooth_angle = 1;
		this.resolution_density = true;
		this.resolution_density_min = 128;
		this.run_in_background = true;
		this.global_height_strength = (float)1;
		this.global_degree_strength = (float)1;
		this.colormap_auto_search = new auto_search_class();
		this.normalmap_auto_search = new auto_search_class();
		this.colormap_assign = true;
		this.display_mix_curves = true;
		this.display_log = true;
		this.filter_select_text = true;
		this.tabs = true;
		this.showTerrains = true;
		this.terrain_settings_foldout = true;
		this.editor_basemap_distance_max = 1000000;
		this.editor_detail_distance_max = 2000;
		this.editor_tree_distance_max = 50000;
		this.editor_fade_length_max = 400;
		this.editor_mesh_trees_max = 1000;
		this.runtime_basemap_distance_max = 1000000;
		this.runtime_detail_distance_max = 2000;
		this.runtime_tree_distance_max = 50000;
		this.runtime_fade_length_max = 400;
		this.runtime_mesh_trees_max = 1000;
		this.terrain_tiles_max = 5;
		this.settings_editor = true;
		this.color_splatPrototypes = new splatPrototype_class[3];
		this.splat_apply_all = true;
		this.stitch_splatmap = true;
		this.raw_search_pattern = "_x%x_y%y";
		this.raw_search_filename = "tile";
		this.raw_search_extension = ".raw";
		this.treemap = new List<tree_map_class>();
		this.grassmap = new List<grass_map_class>();
		this.mesh_path = string.Empty;
		this.load_colormap = true;
		this.load_normalmap = true;
		this.load_treemap = true;
		this.load_controlmap = true;
		this.load_bumpglobal = true;
		this.load_layers = true;
		this.load_layers_settings = true;
	}
}
