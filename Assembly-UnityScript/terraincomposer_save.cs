using Boo.Lang.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class terraincomposer_save : MonoBehaviour
{
	public GameObject Combine_Children;

	public string software_version;

	public float software_id;

	public string filename;

	public Texture2D tex1;

	public int create_pass;

	public string[] heightmap_resolution_list;

	public string[] splatmap_resolution_list;

	public string[] detailmap_resolution_list;

	public string[] detail_resolution_per_patch_list;

	public string[] image_import_max_settings;

	public TreeInstance[] trees;

	public int tree_number;

	public List<subfilter_class> subfilter;

	public List<filter_class> filter;

	public List<precolor_range_class> precolor_range;

	[NonSerialized]
	public terraincomposer_save script;

	public int show_prelayer;

	public List<prelayer_class> prelayers;

	public prelayer_class prelayer;

	public List<int> prelayer_stack;

	public List<Rect> area_stack;

	public bool area_stack_enabled;

	public bool area_skip;

	public int count_area;

	public bool layer_count;

	public bool placed_count;

	public bool overlap;

	public int layer_heightmap;

	public int layer_color;

	public int layer_splat;

	public int layer_tree;

	public int layer_grass;

	public int layer_object;

	public bool layer_heightmap_foldout;

	public bool layer_color_foldout;

	public bool layer_splat_foldout;

	public bool layer_tree_foldout;

	public bool layer_grass_foldout;

	public bool layer_object_foldout;

	public layer_class current_layer;

	public filter_class current_filter;

	public subfilter_class current_subfilter;

	public string terrain_text;

	public List<terrain_class> terrains;

	public string raw_path;

	public string raw_save_path;

	public bool terrains_foldout;

	public bool terrains_active;

	public bool terrains_foldout2;

	public remarks_class remarks;

	public remarks_class mesh_remarks;

	public int terrain_instances;

	public bool terrain_asset_erase;

	public int terrain_tiles;

	public string terrain_path;

	public Transform terrain_parent;

	public string terrain_scene_name;

	public string terrain_asset_name;

	public float object_resolution;

	public Transform object_search;

	public List<mesh_class> meshes;

	public int meshes_layer;

	public bool meshes_active;

	public bool meshes_foldout;

	public bool meshes_foldout2;

	public float meshes_heightscale;

	public area_class meshes_area;

	public string mesh_text;

	public mesh_measure_class mesh_measure;

	public string terrain_slice_path;

	public Transform terrain_slice_parent;

	public bool swap_color_range_select;

	public int swap_color_range_number;

	public precolor_range_class swap_precolor_range;

	public bool copy_color_range_select;

	public bool swap_tree_select;

	public tree_class swap_tree1;

	public tree_output_class swap_tree_output;

	public int swap_tree_position;

	public bool copy_tree_select;

	public tree_class copy_tree1;

	public bool swap_object_select;

	public object_output_class swap_object_output;

	public int swap_object_number;

	public bool copy_object_select;

	public bool swap_description_select;

	public int swap_description_prelayer_index;

	public int swap_description_position;

	public bool copy_description_select;

	public int copy_description_prelayer_index;

	public int copy_description_position;

	public bool swap_layer_select;

	public int swap_prelayer_index;

	public int swap_layer_index;

	public bool copy_layer_select;

	public int copy_prelayer_index;

	public int copy_layer_index;

	public bool swap_filter_select;

	public bool copy_filter_select;

	public bool swap_subfilter_select;

	public presubfilter_class swap_presubfilter;

	public int swap_subfilter_index;

	public bool copy_subfilter_select;

	public subfilter_class copy_subfilter1;

	public terrain_class preterrain;

	public mesh_class premesh;

	public float mix;

	public float xx;

	public float zz;

	public float resolution;

	public int splat_plus;

	public float Rad2Deg;

	public detail_class[] grass_detail;

	public SplatPrototype splat1;

	public int count_value;

	public int count_color_range;

	public int count_prelayer;

	public int count_layer;

	public int count_tree;

	public int count_object;

	public int count_filter;

	public int count_subfilter;

	public int call_from;

	public float random_range;

	public float random_range2;

	public Color color1;

	public Color color2;

	public float color_r;

	public float color_g;

	public float color_b;

	public float color_a;

	public bool heightmap_output;

	public bool heightmap_output_layer;

	public bool color_output;

	public bool splat_output;

	public bool tree_output;

	public bool grass_output;

	public bool object_output;

	public bool world_output;

	public bool line_output;

	public bool button1;

	public bool button_export;

	public string button_generate_text;

	public Texture2D export_texture;

	public byte[] export_bytes;

	public string export_file;

	public string export_path;

	public string export_name;

	public bool export_color_advanced;

	public Color export_color;

	public bool export_color_curve_advanced;

	public AnimationCurve export_color_curve;

	public AnimationCurve export_color_curve_red;

	public AnimationCurve export_color_curve_green;

	public AnimationCurve export_color_curve_blue;

	public int seed;

	public bool generate;

	public bool generateDone;

	public bool generate_manual;

	public int generate_speed;

	public bool generate_speed_display;

	public bool generate_sub_break;

	public bool generate_pause;

	public float generate_call_time;

	public float generate_call_time2;

	public float generate_call_delay;

	public float generate_time;

	public float generate_time_start;

	public bool generate_on_top;

	public bool generate_world_mode;

	public bool generate_auto;

	public int generate_auto_mode;

	public float generate_auto_delay1;

	public float generate_auto_delay2;

	public bool generate_call;

	public bool generate_error;

	public int generate_export;

	public float[,] heights;

	public float[,,] alphamap;

	public List<TreeInstance> tree_instances;

	public int grass_resolution_old;

	public Color color;

	public int object_speed;

	public bool runtime;

	public bool auto_speed;

	public float auto_speed_time;

	public float auto_speed_object_time;

	public int min_speed;

	public float frames;

	public float object_frames;

	public int target_frame;

	public bool only_heightmap;

	public int terrain_index_old;

	public Color tree_color;

	public float layer_x;

	public float layer_y;

	public bool unload_textures;

	public bool clean_memory;

	public float splat_total;

	public List<distance_class> objects_placed;

	public distance_class object_info;

	public int heightmap_x;

	public int heightmap_y;

	public int heightmap_x_old;

	public int heightmap_y_old;

	public int detailmap_x;

	public int detailmap_y;

	public int h_local_x;

	public int h_local_y;

	public int map_x;

	public int map_y;

	public bool measure_normal;

	public List<erosion> erosion_list;

	public bool erosion_move;

	public bool place;

	public Vector3 position;

	public Vector3 scale;

	public float height;

	public float height_interpolated;

	public float degree;

	public Vector3 normal;

	public float local_x_rot;

	public float local_y_rot;

	public float local_x;

	public float local_y;

	public float a;

	public float b;

	public System.Random random;

	public bool measure_tool;

	public bool measure_tool_foldout;

	public bool measure_tool_active;

	public bool measure_tool_undock;

	public bool measure_tool_clicked;

	public float measure_tool_range;

	public bool measure_tool_inrange;

	public Vector2 measure_tool_terrain_point;

	public Vector2 measure_tool_terrain_point_interpolated;

	public bool measure_tool_converter_foldout;

	public float measure_tool_converter_height_input;

	public float measure_tool_converter_height;

	public float measure_tool_converter_angle_input;

	public float measure_tool_converter_angle;

	public bool stitch_tool;

	public bool stitch_tool_foldout;

	public float stitch_tool_border_influence;

	public AnimationCurve stitch_tool_curve;

	public float stitch_tool_strength;

	public bool stitch_command;

	public bool smooth_tool;

	public bool smooth_tool_foldout;

	public float smooth_tool_strength;

	public int smooth_tool_repeat;

	public bool smooth_tool_advanced;

	public float smooth_tool_layer_strength;

	public bool smooth_command;

	public string[] smooth_tool_terrain;

	public int smooth_tool_terrain_select;

	public animation_curve_class smooth_tool_height_curve;

	public animation_curve_class smooth_tool_angle_curve;

	public bool quick_tools;

	public bool quick_tools_foldout;

	public bool slice_tool;

	public bool slice_tool_active;

	public bool slice_tool_foldout;

	public bool slice_tool_heightmap_foldout;

	public bool slice_tool_all_foldout;

	public Rect slice_tool_rect;

	public Terrain slice_tool_terrain;

	public bool slice_tool_erase_terrain_scene;

	public bool slice_tool_erase_terrain_data;

	public Vector2 slice_tool_offset;

	public float slice_tool_min_height;

	public bool sphere_draw;

	public float sphere_radius;

	public Vector3 measure_tool_point_old;

	public bool image_tools;

	public texture_tool_class texture_tool;

	public pattern_tool_class pattern_tool;

	public heightmap_tool_class heightmap_tool;

	public bool description_display;

	public float description_space;

	public animation_curve_class curve_in_memory_old;

	public bool meshcapture_tool;

	public bool meshcapture_tool_foldout;

	public GameObject meshcapture_tool_object;

	public Texture2D meshcapture_tool_image;

	public Transform meshcapture_tool_pivot;

	public int meshcapture_tool_image_width;

	public int meshcapture_tool_image_height;

	public float meshcapture_tool_scale;

	public bool meshcapture_tool_save_scale;

	public bool meshcapture_tool_shadows;

	public Color meshcapture_tool_color;

	public Color meshcapture_background_color;

	public int row_object_count;

	public bool break_x;

	public float break_time;

	public float break_time_set;

	public bool generate_settings;

	public bool generate_settings_foldout;

	public int tile_resolution;

	public int trees_maximum;

	public terraincomposer_save script_base;

	public save_trees tree_script;

	public save_grass grass_script;

	public int tc_id;

	public settings_class settings;

	public RaycastHit hit;

	public int layerHit;

	public GameObject TerrainComposer_Parent;

	[HideInInspector]
	public float filter_value;

	[HideInInspector]
	public float filter_strength;

	[HideInInspector]
	public float filter_input;

	[HideInInspector]
	public float filter_combine;

	[HideInInspector]
	public float filter_combine_start;

	[HideInInspector]
	public float subfilter_value;

	[HideInInspector]
	public int byte_hi2;

	[HideInInspector]
	public int byte_hi;

	[HideInInspector]
	public int byte_lo;

	public List<raw_file_class> raw_files;

	public bool converted_resolutions;

	public float converted_version;

	public GameObject RTP_LODmanager1;

	public Component rtpLod_script;

	public List<object_point_class> pointsRange;

	public List<GameObject> placedObjects;

	public Vector2 imagePosition;

	public terraincomposer_save()
	{
		this.software_version = "Beta";
		this.create_pass = -1;
		this.heightmap_resolution_list = new string[]
		{
			"4097",
			"2049",
			"1025",
			"513",
			"257",
			"129",
			"65",
			"33"
		};
		this.splatmap_resolution_list = new string[]
		{
			"2048",
			"1024",
			"512",
			"256",
			"128",
			"64",
			"32",
			"16"
		};
		this.detailmap_resolution_list = new string[]
		{
			"2048",
			"1024",
			"512",
			"256",
			"128",
			"64",
			"32",
			"16",
			"8"
		};
		this.detail_resolution_per_patch_list = new string[]
		{
			"8",
			"16",
			"32",
			"64",
			"128"
		};
		this.image_import_max_settings = new string[]
		{
			"32",
			"64",
			"128",
			"256",
			"512",
			"1024",
			"2048",
			"4096"
		};
		this.subfilter = new List<subfilter_class>();
		this.filter = new List<filter_class>();
		this.precolor_range = new List<precolor_range_class>();
		this.prelayers = new List<prelayer_class>();
		this.prelayer_stack = new List<int>();
		this.area_stack = new List<Rect>();
		this.layer_count = true;
		this.placed_count = true;
		this.terrain_text = "Terrain:";
		this.terrains = new List<terrain_class>();
		this.raw_path = string.Empty;
		this.raw_save_path = string.Empty;
		this.terrains_foldout = true;
		this.terrains_active = true;
		this.terrains_foldout2 = true;
		this.remarks = new remarks_class();
		this.mesh_remarks = new remarks_class();
		this.terrain_instances = 1;
		this.terrain_tiles = 1;
		this.terrain_scene_name = "Terrain";
		this.terrain_asset_name = "New Terrain";
		this.object_resolution = (float)10;
		this.meshes = new List<mesh_class>();
		this.meshes_active = true;
		this.meshes_foldout = true;
		this.meshes_foldout2 = true;
		this.meshes_area = new area_class();
		this.mesh_text = "Meshes";
		this.mesh_measure = new mesh_measure_class();
		this.resolution = (float)2048;
		this.splat_plus = 1;
		this.Rad2Deg = 57.29578f;
		this.splat1 = new SplatPrototype();
		this.heightmap_output = true;
		this.color_output = true;
		this.splat_output = true;
		this.tree_output = true;
		this.grass_output = true;
		this.object_output = true;
		this.button_generate_text = "Generate";
		this.export_file = string.Empty;
		this.export_color_advanced = true;
		this.export_color = Color.white;
		this.export_color_curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.export_color_curve_red = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.export_color_curve_green = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.export_color_curve_blue = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.seed = 10;
		this.generate_speed = 10;
		this.generate_on_top = true;
		this.generate_auto_mode = 1;
		this.generate_auto_delay2 = 0.2f;
		this.tree_instances = new List<TreeInstance>();
		this.object_speed = 3;
		this.min_speed = 3;
		this.target_frame = 60;
		this.terrain_index_old = -1;
		this.unload_textures = true;
		this.clean_memory = true;
		this.objects_placed = new List<distance_class>();
		this.object_info = new distance_class();
		this.erosion_list = new List<erosion>();
		this.place = true;
		this.random = new System.Random();
		this.measure_tool_foldout = true;
		this.measure_tool_range = (float)10000;
		this.stitch_tool = true;
		this.stitch_tool_foldout = true;
		this.stitch_tool_border_influence = (float)20;
		this.stitch_tool_curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.stitch_tool_strength = (float)1;
		this.smooth_tool = true;
		this.smooth_tool_foldout = true;
		this.smooth_tool_strength = (float)1;
		this.smooth_tool_repeat = 1;
		this.smooth_tool_layer_strength = (float)1;
		this.smooth_tool_height_curve = new animation_curve_class();
		this.smooth_tool_angle_curve = new animation_curve_class();
		this.quick_tools = true;
		this.slice_tool = true;
		this.slice_tool_foldout = true;
		this.slice_tool_erase_terrain_scene = true;
		this.slice_tool_offset = default(Vector2);
		this.sphere_draw = true;
		this.sphere_radius = (float)10;
		this.texture_tool = new texture_tool_class();
		this.pattern_tool = new pattern_tool_class();
		this.heightmap_tool = new heightmap_tool_class();
		this.description_display = true;
		this.description_space = (float)15;
		this.meshcapture_tool = true;
		this.meshcapture_tool_foldout = true;
		this.meshcapture_tool_image_width = 128;
		this.meshcapture_tool_image_height = 128;
		this.meshcapture_tool_scale = (float)1;
		this.meshcapture_tool_save_scale = true;
		this.meshcapture_tool_color = Color.white;
		this.meshcapture_background_color = Color.black;
		this.break_time = 0.2f;
		this.generate_settings_foldout = true;
		this.tile_resolution = 1000;
		this.trees_maximum = 1000;
		this.settings = new settings_class();
		this.raw_files = new List<raw_file_class>();
		this.pointsRange = new List<object_point_class>();
		this.placedObjects = new List<GameObject>();
	}

	public override void add_prelayer(bool search_level)
	{
		this.prelayers.Add(new prelayer_class(0, this.prelayers.Count));
		this.prelayers[this.prelayers.Count - 1].index = this.prelayers.Count - 1;
		this.prelayers[this.prelayers.Count - 1].prearea.area_max = this.settings.area_max;
		this.prelayers[this.prelayers.Count - 1].prearea.max();
		this.prelayers[this.prelayers.Count - 1].set_prelayer_text();
		if (search_level)
		{
			this.search_level_prelayer(0, this.prelayers.Count - 1, 0);
		}
	}

	public override void erase_prelayer(int prelayer_index)
	{
		this.erase_layers(this.prelayers[prelayer_index]);
		if (prelayer_index < this.prelayers.Count - 1)
		{
			this.swap_prelayer1(prelayer_index, this.prelayers.Count - 1);
		}
		this.prelayers.RemoveAt(this.prelayers.Count - 1);
		if (prelayer_index < this.prelayers.Count)
		{
			this.search_prelayer(prelayer_index);
			this.prelayers[prelayer_index].index = prelayer_index;
			this.prelayers[prelayer_index].set_prelayer_text();
		}
	}

	public override void search_prelayer(int prelayer_index)
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				for (int k = 0; k < this.prelayers[i].layer[j].object_output.@object.Count; k++)
				{
					if (this.prelayers[i].layer[j].object_output.@object[k].prelayer_index == this.prelayers.Count)
					{
						this.prelayers[i].layer[j].object_output.@object[k].prelayer_index = prelayer_index;
						return;
					}
				}
			}
		}
	}

	public override void search_level_prelayer(int prelayer_index, int find_index, int level)
	{
		for (int i = 0; i < this.prelayers[prelayer_index].layer.Count; i++)
		{
			for (int j = 0; j < this.prelayers[prelayer_index].layer[i].object_output.@object.Count; j++)
			{
				if (this.prelayers[prelayer_index].layer[i].object_output.@object[j].prelayer_created)
				{
					level++;
					if (this.prelayers[prelayer_index].layer[i].object_output.@object[j].prelayer_index == find_index)
					{
						this.prelayers[this.prelayers[prelayer_index].layer[i].object_output.@object[j].prelayer_index].level = level;
						return;
					}
					this.search_level_prelayer(this.prelayers[prelayer_index].layer[i].object_output.@object[j].prelayer_index, find_index, level);
					level--;
				}
			}
		}
	}

	public override void swap_prelayer1(int prelayer_index1, int prelayer_index2)
	{
		prelayer_class prelayer_class = this.prelayers[prelayer_index1];
		this.prelayers[prelayer_index1] = this.prelayers[prelayer_index2];
		this.prelayers[prelayer_index2] = this.prelayers[prelayer_index1];
	}

	public override void new_layergroup(prelayer_class prelayer1, int description_number)
	{
		int count = prelayer1.predescription.description[description_number].layer_index.Count;
		for (int i = 0; i < count; i++)
		{
			this.erase_layer(prelayer1, prelayer1.predescription.description[description_number].layer_index[0], description_number, 0, true, true, false);
		}
	}

	public override void erase_description(prelayer_class prelayer1, int description_number)
	{
		if (prelayer1.predescription.description.Count > 1)
		{
			int count = prelayer1.predescription.description[description_number].layer_index.Count;
			for (int i = 0; i < count; i++)
			{
				this.erase_layer(prelayer1, prelayer1.predescription.description[description_number].layer_index[0], description_number, 0, true, true, false);
			}
			prelayer1.predescription.erase_description(description_number);
		}
		this.count_layers();
	}

	public override void swap_description(int description_number1, int description_number2, prelayer_class prelayer1)
	{
		if (description_number2 >= 0 && description_number2 <= prelayer1.predescription.description.Count - 1)
		{
			int count = prelayer1.predescription.description[description_number1].layer_index.Count;
			int count2 = prelayer1.predescription.description[description_number2].layer_index.Count;
			int i = 0;
			string text = prelayer1.predescription.description[description_number1].text;
			bool edit = prelayer1.predescription.description[description_number1].edit;
			prelayer1.predescription.description[description_number1].text = prelayer1.predescription.description[description_number2].text;
			prelayer1.predescription.description[description_number2].text = text;
			prelayer1.predescription.description[description_number1].edit = prelayer1.predescription.description[description_number2].edit;
			prelayer1.predescription.description[description_number2].edit = edit;
			bool foldout = prelayer1.predescription.description[description_number1].foldout;
			prelayer1.predescription.description[description_number1].foldout = prelayer1.predescription.description[description_number2].foldout;
			prelayer1.predescription.description[description_number2].foldout = foldout;
			for (i = 0; i < count; i++)
			{
				this.replace_layer(0, description_number1, description_number2, prelayer1);
			}
			for (i = 0; i < count2; i++)
			{
				this.replace_layer(0, description_number2, description_number1, prelayer1);
			}
			prelayer1.predescription.set_description_enum();
		}
	}

	public override void replace_layer(int source_layer_index, int source_description_number, int target_description_number, prelayer_class prelayer1)
	{
		int num = this.get_layer_position(prelayer1.predescription.description[target_description_number].layer_index.Count, target_description_number, prelayer1);
		this.add_layer(prelayer1, num, layer_output_enum.color, target_description_number, prelayer1.predescription.description[target_description_number].layer_index.Count, false, false, true);
		prelayer1.layer[num] = this.copy_layer(prelayer1.layer[prelayer1.predescription.description[source_description_number].layer_index[source_layer_index]], true, true);
		this.erase_layer(prelayer1, prelayer1.predescription.description[source_description_number].layer_index[source_layer_index], source_description_number, source_layer_index, true, true, true);
	}

	public override void count_layers()
	{
		if (this.layer_count)
		{
			this.layer_heightmap = (this.layer_color = (this.layer_splat = (this.layer_tree = (this.layer_grass = (this.layer_object = 0)))));
			for (int i = 0; i < this.prelayers.Count; i++)
			{
				for (int j = 0; j < this.prelayers[i].layer.Count; j++)
				{
					layer_output_enum output = this.prelayers[i].layer[j].output;
					if (output == layer_output_enum.heightmap)
					{
						this.layer_heightmap++;
					}
					else if (output == layer_output_enum.color)
					{
						this.layer_color++;
					}
					else if (output == layer_output_enum.splat)
					{
						this.layer_splat++;
					}
					else if (output == layer_output_enum.tree)
					{
						this.layer_tree++;
					}
					else if (output == layer_output_enum.grass)
					{
						this.layer_grass++;
					}
					else if (output == layer_output_enum.@object)
					{
						this.layer_object++;
					}
				}
			}
		}
	}

	public override void erase_layers(prelayer_class prelayer1)
	{
		int count = prelayer1.layer.Count;
		for (int i = 0; i < count; i++)
		{
			this.erase_layer(prelayer1, 0, 0, 0, false, true, false);
		}
	}

	public override void erase_layer(prelayer_class prelayer1, int layer_number, int description_number, int layer_index, bool description, bool loop_layer, bool count_layer)
	{
		if (prelayer1.layer.Count > 0)
		{
			if (loop_layer)
			{
				this.loop_layer(prelayer1.layer[layer_number], -1);
			}
			this.erase_filters(prelayer1.layer[layer_number].prefilter);
			for (int i = 0; i < prelayer1.layer[layer_number].tree_output.tree.Count; i++)
			{
				this.erase_filters(prelayer1.layer[layer_number].tree_output.tree[i].prefilter);
			}
			prelayer1.layer.RemoveAt(layer_number);
		}
		if (description)
		{
			prelayer1.predescription.erase_layer_index(layer_number, layer_index, description_number);
		}
		if (count_layer)
		{
			this.count_layers();
		}
		prelayer1.set_prelayer_text();
	}

	public override void strip_layer(prelayer_class prelayer1, int layer_number)
	{
		for (int i = 0; i < prelayer1.layer[layer_number].object_output.@object.Count; i++)
		{
			if (prelayer1.layer[layer_number].object_output.@object[i].prelayer_created)
			{
				this.erase_prelayer(prelayer1.layer[layer_number].object_output.@object[i].prelayer_index);
			}
		}
		this.erase_filters(prelayer1.layer[layer_number].prefilter);
	}

	public override void add_layer(prelayer_class prelayer1, int layer_number, layer_output_enum layer_output, int description_number, int layer_index, bool new_filter, bool count_layer, bool custom)
	{
		prelayer1.layer.Insert(layer_number, new layer_class());
		if (new_filter)
		{
			this.add_filter(0, prelayer1.layer[layer_number].prefilter);
		}
		prelayer1.layer[layer_number].output = layer_output;
		prelayer1.predescription.add_layer_index(layer_number, layer_index, description_number);
		if (count_layer)
		{
			this.count_layers();
		}
		prelayer1.set_prelayer_text();
		if (layer_output == layer_output_enum.heightmap && custom && new_filter)
		{
			this.filter[this.filter.Count - 1].type = condition_type_enum.Always;
		}
	}

	public override void swap_layer(prelayer_class prelayer1, int layer_number1, prelayer_class prelayer2, int layer_number2)
	{
		if (layer_number2 >= 0 && layer_number2 <= prelayer2.layer.Count - 1)
		{
			layer_class value = prelayer1.layer[layer_number1];
			prelayer1.layer[layer_number1] = prelayer2.layer[layer_number2];
			prelayer2.layer[layer_number2] = value;
			if (prelayer1.layer[layer_number1].color_layer[0] < 1.5f)
			{
				prelayer1.layer[layer_number1].color_layer = prelayer1.layer[layer_number1].color_layer + new Color((float)1, (float)1, (float)1, (float)1);
			}
			if (prelayer2.layer[layer_number2].color_layer[0] < 1.5f)
			{
				prelayer2.layer[layer_number2].color_layer = prelayer2.layer[layer_number2].color_layer + new Color((float)1, (float)1, (float)1, (float)1);
			}
		}
	}

	public override int get_layer_position(int layer_index, int description_number, prelayer_class prelayer1)
	{
		int num = 0;
		int arg_51_0;
		for (int i = 0; i < prelayer1.predescription.description.Count; i++)
		{
			if (i == description_number)
			{
				arg_51_0 = num + layer_index;
				return arg_51_0;
			}
			num += prelayer1.predescription.description[i].layer_index.Count;
		}
		arg_51_0 = -1;
		return arg_51_0;
	}

	public override int get_layer_description(prelayer_class prelayer1, int layer_index)
	{
		int arg_76_0;
		for (int i = 0; i < prelayer1.predescription.description.Count; i++)
		{
			for (int j = 0; j < prelayer1.predescription.description[i].layer_index.Count; j++)
			{
				if (prelayer1.predescription.description[i].layer_index[j] == layer_index)
				{
					arg_76_0 = i;
					return arg_76_0;
				}
			}
		}
		arg_76_0 = -1;
		return arg_76_0;
	}

	public override void layer_sort(prelayer_class prelayer, int description_number)
	{
		int num = 0;
		int index = 0;
		int index2 = 0;
		int index3 = 0;
		int index4 = 0;
		int index5 = 0;
		int index6 = 0;
		for (int i = 0; i < prelayer.predescription.description[description_number].layer_index.Count; i++)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			for (int j = i; j < prelayer.predescription.description[description_number].layer_index.Count; j++)
			{
				if (!flag && prelayer.layer[prelayer.predescription.description[description_number].layer_index[j]].output == layer_output_enum.heightmap)
				{
					index = j;
					flag = true;
				}
				if (!flag2 && prelayer.layer[prelayer.predescription.description[description_number].layer_index[j]].output == layer_output_enum.color)
				{
					index2 = j;
					flag2 = true;
				}
				if (!flag3 && prelayer.layer[prelayer.predescription.description[description_number].layer_index[j]].output == layer_output_enum.splat)
				{
					index3 = j;
					flag3 = true;
				}
				if (!flag4 && prelayer.layer[prelayer.predescription.description[description_number].layer_index[j]].output == layer_output_enum.tree)
				{
					index4 = j;
					flag4 = true;
				}
				if (!flag5 && prelayer.layer[prelayer.predescription.description[description_number].layer_index[j]].output == layer_output_enum.grass)
				{
					index5 = j;
					flag5 = true;
				}
				if (!flag6 && prelayer.layer[prelayer.predescription.description[description_number].layer_index[j]].output == layer_output_enum.@object)
				{
					index6 = j;
					flag6 = true;
				}
			}
			if (flag)
			{
				this.swap_layer(prelayer, prelayer.predescription.description[description_number].layer_index[i], prelayer, prelayer.predescription.description[description_number].layer_index[index]);
			}
			else if (flag2)
			{
				this.swap_layer(prelayer, prelayer.predescription.description[description_number].layer_index[i], prelayer, prelayer.predescription.description[description_number].layer_index[index2]);
			}
			else if (flag3)
			{
				this.swap_layer(prelayer, prelayer.predescription.description[description_number].layer_index[i], prelayer, prelayer.predescription.description[description_number].layer_index[index3]);
			}
			else if (flag4)
			{
				this.swap_layer(prelayer, prelayer.predescription.description[description_number].layer_index[i], prelayer, prelayer.predescription.description[description_number].layer_index[index4]);
			}
			else if (flag5)
			{
				this.swap_layer(prelayer, prelayer.predescription.description[description_number].layer_index[i], prelayer, prelayer.predescription.description[description_number].layer_index[index5]);
			}
			else if (flag6)
			{
				this.swap_layer(prelayer, prelayer.predescription.description[description_number].layer_index[i], prelayer, prelayer.predescription.description[description_number].layer_index[index6]);
			}
		}
	}

	public override void layers_sort(prelayer_class prelayer)
	{
		for (int i = 0; i < prelayer.predescription.description.Count; i++)
		{
			this.layer_sort(prelayer, i);
		}
	}

	public override void erase_filters(prefilter_class prefilter)
	{
		int count = prefilter.filter_index.Count;
		for (int i = 0; i < count; i++)
		{
			this.erase_filter(0, prefilter);
		}
	}

	public override void add_filter(int filter_number, prefilter_class prefilter)
	{
		this.filter.Add(new filter_class());
		prefilter.filter_index.Insert(filter_number, this.filter.Count - 1);
		if (this.terrains.Count > 1)
		{
			this.filter[this.filter.Count - 1].preimage.image_mode = image_mode_enum.MultiTerrain;
		}
		prefilter.set_filter_text();
	}

	public override void add_animation_curve(List<animation_curve_class> precurve_list, int curve_number, bool copy)
	{
		if (!copy)
		{
			precurve_list.Insert(curve_number, new animation_curve_class());
			precurve_list[curve_number].curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)0);
			precurve_list[curve_number].default_curve = new AnimationCurve(precurve_list[curve_number].curve.keys);
		}
		else
		{
			precurve_list.Insert(curve_number, this.copy_animation_curve(precurve_list[curve_number - 1]));
		}
	}

	public override void erase_animation_curve(List<animation_curve_class> precurve_list, int curve_number)
	{
		if (precurve_list.Count > 0)
		{
			precurve_list.RemoveAt(curve_number);
		}
	}

	public override void swap_animation_curve(List<animation_curve_class> curve_list, int curve_number1, int curve_number2)
	{
		animation_curve_class value = curve_list[curve_number1];
		curve_list[curve_number1] = curve_list[curve_number2];
		curve_list[curve_number2] = value;
	}

	public override void erase_filter(int filter_number, prefilter_class prefilter)
	{
		if (prefilter.filter_index.Count > 0)
		{
			this.erase_subfilters(this.filter[prefilter.filter_index[filter_number]]);
			int num = prefilter.filter_index[filter_number];
			this.swap_filter2(num, this.filter.Count - 1, false);
			this.filter.RemoveAt(this.filter.Count - 1);
			prefilter.filter_index.RemoveAt(filter_number);
			this.relink_filter_index(num);
			prefilter.set_filter_text();
		}
	}

	public override void erase_filter_reference(prefilter_class prefilter, int filter_index)
	{
		prefilter.filter_index.RemoveAt(filter_index);
		prefilter.set_filter_text();
	}

	public override void erase_filter_unlinked(int filter_number)
	{
		this.swap_filter2(filter_number, this.filter.Count - 1, false);
		this.filter.RemoveAt(this.filter.Count - 1);
		this.relink_filter_index(filter_number);
	}

	public override void swap_filter(prefilter_class prefilter1, int filter_index1, prefilter_class prefilter2, int filter_index2)
	{
		if (filter_index2 >= 0 && filter_index2 <= prefilter2.filter_index.Count - 1)
		{
			filter_class value = this.filter[prefilter1.filter_index[filter_index1]];
			this.filter[prefilter1.filter_index[filter_index1]] = this.filter[prefilter2.filter_index[filter_index2]];
			this.filter[prefilter2.filter_index[filter_index2]] = value;
			if (this.filter[prefilter1.filter_index[filter_index1]].color_filter[0] < 1.5f)
			{
				this.filter[prefilter1.filter_index[filter_index1]].color_filter = this.filter[prefilter1.filter_index[filter_index1]].color_filter + new Color((float)1, (float)1, (float)1, (float)1);
			}
			if (this.filter[prefilter2.filter_index[filter_index2]].color_filter[0] < 1.5f)
			{
				this.filter[prefilter2.filter_index[filter_index2]].color_filter = this.filter[prefilter2.filter_index[filter_index2]].color_filter + new Color((float)1, (float)1, (float)1, (float)1);
			}
		}
	}

	public override void swap_filter2(int filter_number1, int filter_number2, bool blink)
	{
		filter_class value = this.filter[filter_number1];
		this.filter[filter_number1] = this.filter[filter_number2];
		this.filter[filter_number2] = value;
		if (blink)
		{
			if (this.filter[filter_number1].color_filter[0] < 1.5f)
			{
				this.filter[filter_number1].color_filter = this.filter[filter_number1].color_filter + new Color((float)1, (float)1, (float)1, (float)1);
			}
			if (this.filter[filter_number2].color_filter[0] < 1.5f)
			{
				this.filter[filter_number2].color_filter = this.filter[filter_number2].color_filter + new Color((float)1, (float)1, (float)1, (float)1);
			}
		}
	}

	public override void swap_object(object_output_class object_output1, int object_number1, object_output_class object_output2, int object_number2)
	{
		object_class value = object_output1.@object[object_number1];
		float value2 = object_output1.object_value.value[object_number1];
		bool value3 = object_output1.object_value.active[object_number1];
		object_output1.@object[object_number1] = object_output2.@object[object_number2];
		object_output2.@object[object_number2] = value;
		if (object_output1.@object[object_number1].color_object[0] > 0.5f)
		{
			object_output1.@object[object_number1].color_object = object_output1.@object[object_number1].color_object + new Color(-0.5f, (float)0, -0.5f, (float)0);
		}
		if (object_output2.@object[object_number2].color_object[0] > 0.5f)
		{
			object_output2.@object[object_number2].color_object = object_output2.@object[object_number2].color_object + new Color(-0.5f, (float)0, -0.5f, (float)0);
		}
		object_output1.object_value.value[object_number1] = object_output2.object_value.value[object_number2];
		object_output2.object_value.value[object_number2] = value2;
		object_output1.object_value.active[object_number1] = object_output2.object_value.active[object_number2];
		object_output2.object_value.active[object_number2] = value3;
		object_output1.object_value.calc_value();
		if (!RuntimeServices.EqualityOperator(object_output1, object_output2))
		{
			object_output2.object_value.calc_value();
		}
	}

	public override void swap_tree(tree_output_class tree_output1, int tree_number1, tree_output_class tree_output2, int tree_number2)
	{
		tree_class value = tree_output1.tree[tree_number1];
		float value2 = tree_output1.tree_value.value[tree_number1];
		bool value3 = tree_output1.tree_value.active[tree_number1];
		tree_output1.tree[tree_number1] = tree_output2.tree[tree_number2];
		tree_output2.tree[tree_number2] = value;
		if (tree_output1.tree[tree_number1].color_tree[0] < 1.5f)
		{
			tree_output1.tree[tree_number1].color_tree = tree_output1.tree[tree_number1].color_tree + new Color(0.5f, 0.5f, 0.5f, (float)0);
		}
		if (tree_output2.tree[tree_number2].color_tree[0] < 1.5f)
		{
			tree_output2.tree[tree_number2].color_tree = tree_output2.tree[tree_number2].color_tree + new Color(0.5f, 0.5f, 0.5f, (float)0);
		}
		tree_output1.tree_value.value[tree_number1] = tree_output2.tree_value.value[tree_number2];
		tree_output2.tree_value.value[tree_number2] = value2;
		tree_output1.tree_value.active[tree_number1] = tree_output2.tree_value.active[tree_number2];
		tree_output2.tree_value.active[tree_number2] = value3;
		tree_output1.tree_value.calc_value();
		if (!RuntimeServices.EqualityOperator(tree_output1, tree_output2))
		{
			tree_output2.tree_value.calc_value();
		}
	}

	public override void add_object(object_output_class object_output, int object_number)
	{
		object_output.@object.Insert(object_number, new object_class());
		object_output.object_value.add_value(object_number, (float)50);
		object_output.set_object_text();
	}

	public override void erase_object(object_output_class object_output, int object_number)
	{
		if (object_output.@object.Count > 0)
		{
			if (object_output.@object[object_number].prelayer_created)
			{
				this.erase_prelayer(object_output.@object[object_number].prelayer_index);
			}
			object_output.@object.RemoveAt(object_number);
			object_output.object_value.erase_value(object_number);
			object_output.set_object_text();
		}
	}

	public override void clear_object(object_output_class object_output)
	{
		int count = object_output.@object.Count;
		for (int i = 0; i < count; i++)
		{
			this.erase_object(object_output, object_output.@object.Count - 1);
		}
	}

	public override void swap_color_range(precolor_range_class precolor_range1, int color_range_number1, precolor_range_class precolor_range2, int color_range_number2)
	{
		color_range_class value = precolor_range1.color_range[color_range_number1];
		float value2 = precolor_range1.color_range_value.value[color_range_number1];
		bool value3 = precolor_range1.color_range_value.active[color_range_number1];
		precolor_range1.color_range[color_range_number1] = precolor_range2.color_range[color_range_number2];
		precolor_range2.color_range[color_range_number2] = value;
		if (precolor_range1.color_range[color_range_number1].color_color_range[0] < 1.5f)
		{
			precolor_range1.color_range[color_range_number1].color_color_range = precolor_range1.color_range[color_range_number1].color_color_range + new Color((float)1, (float)1, (float)1, (float)1);
		}
		if (precolor_range2.color_range[color_range_number2].color_color_range[0] < 1.5f)
		{
			precolor_range2.color_range[color_range_number2].color_color_range = precolor_range2.color_range[color_range_number2].color_color_range + new Color((float)1, (float)1, (float)1, (float)1);
		}
		precolor_range1.color_range_value.value[color_range_number1] = precolor_range2.color_range_value.value[color_range_number2];
		precolor_range2.color_range_value.value[color_range_number2] = value2;
		precolor_range1.color_range_value.active[color_range_number1] = precolor_range2.color_range_value.active[color_range_number2];
		precolor_range2.color_range_value.active[color_range_number2] = value3;
		precolor_range1.color_range_value.calc_value();
		precolor_range1.set_precolor_range_curve();
		if (!RuntimeServices.EqualityOperator(precolor_range1, precolor_range2))
		{
			precolor_range2.color_range_value.calc_value();
			precolor_range2.set_precolor_range_curve();
		}
		if (precolor_range1.one_color)
		{
			precolor_range1.color_range[color_range_number1].one_color = true;
		}
		if (precolor_range2.one_color)
		{
			precolor_range2.color_range[color_range_number2].one_color = true;
		}
	}

	public override void change_filters_active(prefilter_class prefilter, bool invert)
	{
		for (int i = 0; i < prefilter.filter_index.Count; i++)
		{
			if (!invert)
			{
				this.filter[prefilter.filter_index[i]].active = prefilter.filters_active;
			}
			else
			{
				this.filter[prefilter.filter_index[i]].active = !this.filter[prefilter.filter_index[i]].active;
			}
		}
	}

	public override void change_filters_foldout(prefilter_class prefilter, bool invert)
	{
		for (int i = 0; i < prefilter.filter_index.Count; i++)
		{
			if (!invert)
			{
				this.filter[prefilter.filter_index[i]].foldout = prefilter.filters_foldout;
			}
			else
			{
				this.filter[prefilter.filter_index[i]].foldout = !this.filter[prefilter.filter_index[i]].foldout;
			}
		}
	}

	public override void change_subfilters_active(presubfilter_class presubfilter, bool invert)
	{
		for (int i = 0; i < presubfilter.subfilter_index.Count; i++)
		{
			if (!invert)
			{
				this.subfilter[presubfilter.subfilter_index[i]].active = presubfilter.subfilters_active;
			}
			else
			{
				this.subfilter[presubfilter.subfilter_index[i]].active = !this.subfilter[presubfilter.subfilter_index[i]].active;
			}
		}
	}

	public override void change_subfilters_foldout(presubfilter_class presubfilter, bool invert)
	{
		for (int i = 0; i < presubfilter.subfilter_index.Count; i++)
		{
			if (!invert)
			{
				this.subfilter[presubfilter.subfilter_index[i]].foldout = presubfilter.subfilters_foldout;
			}
			else
			{
				this.subfilter[presubfilter.subfilter_index[i]].foldout = !this.subfilter[presubfilter.subfilter_index[i]].foldout;
			}
		}
	}

	public override void change_terrains_active(bool invert)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (!invert)
			{
				this.terrains[i].active = this.terrains_active;
			}
			else
			{
				this.terrains[i].active = !this.terrains[i].active;
			}
		}
	}

	public override void change_meshes_active(bool invert)
	{
		for (int i = 0; i < this.meshes.Count; i++)
		{
			if (!invert)
			{
				this.meshes[i].active = this.meshes_active;
			}
			else
			{
				this.meshes[i].active = !this.meshes[i].active;
			}
		}
	}

	public override void change_terrains_foldout(bool invert)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (!invert)
			{
				this.terrains[i].foldout = this.terrains_foldout2;
			}
			else
			{
				this.terrains[i].foldout = !this.terrains[i].foldout;
			}
		}
	}

	public override void change_meshes_foldout(bool invert)
	{
		for (int i = 0; i < this.meshes.Count; i++)
		{
			if (!invert)
			{
				this.meshes[i].foldout = this.meshes_foldout2;
			}
			else
			{
				this.meshes[i].foldout = !this.meshes[i].foldout;
			}
		}
	}

	public override void change_trees_active(tree_output_class tree_output, bool invert)
	{
		for (int i = 0; i < tree_output.tree.Count; i++)
		{
			if (!invert)
			{
				tree_output.tree_value.active[i] = tree_output.trees_active;
			}
			else
			{
				tree_output.tree_value.active[i] = !tree_output.tree_value.active[i];
			}
		}
		tree_output.tree_value.calc_value();
	}

	public override void change_trees_foldout(tree_output_class tree_output, bool invert)
	{
		for (int i = 0; i < tree_output.tree.Count; i++)
		{
			if (!invert)
			{
				tree_output.tree[i].foldout = tree_output.trees_foldout;
			}
			else
			{
				tree_output.tree[i].foldout = !tree_output.tree[i].foldout;
			}
		}
	}

	public override void change_trees_settings_foldout(terrain_class preterrain1, bool invert)
	{
		for (int i = 0; i < preterrain1.treePrototypes.Count; i++)
		{
			if (!invert)
			{
				preterrain1.treePrototypes[i].foldout = preterrain1.trees_foldout;
			}
			else
			{
				preterrain1.treePrototypes[i].foldout = !preterrain1.treePrototypes[i].foldout;
			}
		}
	}

	public override void change_splats_settings_foldout(terrain_class preterrain1, bool invert)
	{
		for (int i = 0; i < preterrain1.splatPrototypes.Count; i++)
		{
			if (!invert)
			{
				preterrain1.splatPrototypes[i].foldout = preterrain1.splats_foldout;
			}
			else
			{
				preterrain1.splatPrototypes[i].foldout = !preterrain1.splatPrototypes[i].foldout;
			}
		}
	}

	public override void change_color_splats_settings_foldout(terrain_class preterrain1, bool invert)
	{
		for (int i = 0; i < this.settings.color_splatPrototypes.Length; i++)
		{
			if (!invert)
			{
				this.settings.color_splatPrototypes[i].foldout = preterrain1.splats_foldout;
			}
			else
			{
				this.settings.color_splatPrototypes[i].foldout = !this.settings.color_splatPrototypes[i].foldout;
			}
		}
	}

	public override void change_details_settings_foldout(terrain_class preterrain1, bool invert)
	{
		for (int i = 0; i < preterrain1.detailPrototypes.Count; i++)
		{
			if (!invert)
			{
				preterrain1.detailPrototypes[i].foldout = preterrain1.details_foldout;
			}
			else
			{
				preterrain1.detailPrototypes[i].foldout = !preterrain1.detailPrototypes[i].foldout;
			}
		}
	}

	public override void change_objects_active(object_output_class object_output, bool invert)
	{
		for (int i = 0; i < object_output.@object.Count; i++)
		{
			if (!invert)
			{
				object_output.object_value.active[i] = object_output.objects_active;
			}
			else
			{
				object_output.object_value.active[i] = !object_output.object_value.active[i];
			}
		}
		object_output.object_value.calc_value();
	}

	public override void change_objects_foldout(object_output_class object_output, bool invert)
	{
		for (int i = 0; i < object_output.@object.Count; i++)
		{
			if (!invert)
			{
				object_output.@object[i].foldout = object_output.objects_foldout;
			}
			else
			{
				object_output.@object[i].foldout = !object_output.@object[i].foldout;
			}
		}
	}

	public override void change_color_ranges_active(precolor_range_class precolor_range, bool invert)
	{
		for (int i = 0; i < precolor_range.color_range.Count; i++)
		{
			if (!invert)
			{
				precolor_range.color_range_value.active[i] = precolor_range.color_ranges_active;
			}
			else
			{
				precolor_range.color_range_value.active[i] = !precolor_range.color_range_value.active[i];
			}
		}
	}

	public override void erase_subfilters(filter_class filter)
	{
		int count = filter.presubfilter.subfilter_index.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			this.erase_subfilter(0, filter.presubfilter);
		}
	}

	public override void add_subfilter(int subfilter_number, presubfilter_class presubfilter)
	{
		this.subfilter.Add(new subfilter_class());
		presubfilter.subfilter_index.Insert(subfilter_number, this.subfilter.Count - 1);
		if (this.terrains.Count > 1)
		{
			this.subfilter[this.subfilter.Count - 1].preimage.image_mode = image_mode_enum.MultiTerrain;
		}
		presubfilter.set_subfilter_text(presubfilter.subfilter_index.Count);
	}

	public override void add_line_point(List<line_list_class> line_list, int line_point_number)
	{
		line_list.Insert(line_point_number, new line_list_class());
	}

	public override void erase_line_point(List<line_list_class> line_list, int line_point_number)
	{
		line_list.RemoveAt(line_point_number);
	}

	public override void erase_subfilter(int subfilter_number, presubfilter_class presubfilter)
	{
		if (presubfilter.subfilter_index.Count > 0)
		{
			int num = presubfilter.subfilter_index[subfilter_number];
			this.swap_subfilter2(num, this.subfilter.Count - 1, false);
			this.subfilter.RemoveAt(this.subfilter.Count - 1);
			presubfilter.subfilter_index.RemoveAt(subfilter_number);
			this.relink_subfilter_index(num);
			presubfilter.set_subfilter_text(presubfilter.subfilter_index.Count);
		}
	}

	public override void erase_subfilter_reference(presubfilter_class presubfilter, int subfilter_index)
	{
		presubfilter.subfilter_index.RemoveAt(subfilter_index);
		presubfilter.set_subfilter_text(presubfilter.subfilter_index.Count);
	}

	public override void erase_subfilter_unlinked(int subfilter_number)
	{
		this.swap_subfilter2(subfilter_number, this.subfilter.Count - 1, false);
		this.subfilter.RemoveAt(this.subfilter.Count - 1);
		this.relink_subfilter_index(subfilter_number);
	}

	public override void swap_subfilter(presubfilter_class presubfilter1, int subfilter_index1, presubfilter_class presubfilter2, int subfilter_index2)
	{
		if (subfilter_index2 >= 0 && subfilter_index2 <= presubfilter2.subfilter_index.Count - 1)
		{
			subfilter_class value = this.subfilter[presubfilter1.subfilter_index[subfilter_index1]];
			this.subfilter[presubfilter1.subfilter_index[subfilter_index1]] = this.subfilter[presubfilter2.subfilter_index[subfilter_index2]];
			this.subfilter[presubfilter2.subfilter_index[subfilter_index2]] = value;
			if (this.subfilter[presubfilter1.subfilter_index[subfilter_index1]].color_subfilter[0] < 1.5f)
			{
				this.subfilter[presubfilter1.subfilter_index[subfilter_index1]].color_subfilter = this.subfilter[presubfilter1.subfilter_index[subfilter_index1]].color_subfilter + new Color((float)1, (float)1, (float)1, (float)1);
			}
			if (this.subfilter[presubfilter2.subfilter_index[subfilter_index2]].color_subfilter[0] < 1.5f)
			{
				this.subfilter[presubfilter2.subfilter_index[subfilter_index2]].color_subfilter = this.subfilter[presubfilter2.subfilter_index[subfilter_index2]].color_subfilter + new Color((float)1, (float)1, (float)1, (float)1);
			}
		}
	}

	public override void swap_subfilter2(int subfilter_number1, int subfilter_number2, bool blink)
	{
		subfilter_class value = this.subfilter[subfilter_number1];
		this.subfilter[subfilter_number1] = this.subfilter[subfilter_number2];
		this.subfilter[subfilter_number2] = value;
		if (blink)
		{
			if (this.subfilter[subfilter_number1].color_subfilter[0] < 1.5f)
			{
				this.subfilter[subfilter_number1].color_subfilter = this.subfilter[subfilter_number1].color_subfilter + new Color((float)1, (float)1, (float)1, (float)1);
			}
			if (this.subfilter[subfilter_number2].color_subfilter[0] < 1.5f)
			{
				this.subfilter[subfilter_number2].color_subfilter = this.subfilter[subfilter_number2].color_subfilter + new Color((float)1, (float)1, (float)1, (float)1);
			}
		}
	}

	public override void new_layers()
	{
		this.filter.Clear();
		this.subfilter.Clear();
		this.prelayers.Clear();
		this.reset_swapcopy();
		this.disable_outputs();
		this.prelayers.Add(new prelayer_class(0, 0));
		this.prelayer = this.prelayers[0];
		this.prelayer.prearea.active = false;
		this.filename = string.Empty;
		this.set_area_resolution(this.terrains[0], this.prelayer.prearea);
		this.settings.colormap = false;
		this.count_layers();
	}

	public override void reset_swapcopy()
	{
		this.swap_layer_select = false;
		this.swap_filter_select = false;
		this.swap_subfilter_select = false;
		this.swap_object_select = false;
		this.swap_color_range_select = false;
		this.swap_description_select = false;
		this.copy_layer_select = false;
		this.copy_filter_select = false;
		this.copy_subfilter_select = false;
		this.copy_object_select = false;
		this.copy_color_range_select = false;
		this.copy_description_select = false;
		for (int i = 0; i < this.filter.Count; i++)
		{
			for (int j = 0; j < this.filter[i].precurve_list.Count; j++)
			{
				this.filter[i].precurve_list[j].curve_text = "Curve";
			}
		}
		for (int k = 0; k < this.subfilter.Count; k++)
		{
			for (int j = 0; j < this.subfilter[k].precurve_list.Count; j++)
			{
				this.subfilter[k].precurve_list[j].curve_text = "Curve";
			}
		}
	}

	public override void reset_software_version()
	{
		this.software_id = (float)0;
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				this.prelayers[i].layer[j].software_id = (float)0;
			}
		}
	}

	public override int get_output_length(layer_class layer)
	{
		return (layer.output != layer_output_enum.heightmap) ? ((layer.output != layer_output_enum.color) ? ((layer.output != layer_output_enum.splat) ? ((layer.output != layer_output_enum.tree) ? ((layer.output != layer_output_enum.grass) ? ((layer.output != layer_output_enum.@object) ? -1 : layer.object_output.@object.Count) : layer.grass_output.grass.Count) : layer.tree_output.tree.Count) : layer.splat_output.splat.Count) : 0) : 0;
	}

	public override void set_view_only_selected(prelayer_class prelayer, layer_output_enum selected, bool disable_view)
	{
		if (disable_view)
		{
			prelayer.view_heightmap_layer = (prelayer.view_color_layer = (prelayer.view_splat_layer = (prelayer.view_tree_layer = (prelayer.view_grass_layer = (prelayer.view_object_layer = false)))));
		}
		if (selected == layer_output_enum.heightmap)
		{
			prelayer.view_heightmap_layer = true;
		}
		if (selected == layer_output_enum.color)
		{
			prelayer.view_color_layer = true;
		}
		if (selected == layer_output_enum.splat)
		{
			prelayer.view_splat_layer = true;
		}
		if (selected == layer_output_enum.tree)
		{
			prelayer.view_tree_layer = true;
		}
		if (selected == layer_output_enum.grass)
		{
			prelayer.view_grass_layer = true;
		}
		if (selected == layer_output_enum.@object)
		{
			prelayer.view_object_layer = true;
		}
	}

	public override void set_output(layer_output_enum selected)
	{
		if (selected == layer_output_enum.heightmap)
		{
			this.disable_outputs();
			this.heightmap_output = true;
		}
		if (selected == layer_output_enum.color)
		{
			this.disable_outputs();
			this.color_output = true;
		}
		if (selected == layer_output_enum.splat)
		{
			this.disable_outputs();
			this.splat_output = true;
		}
		if (selected == layer_output_enum.tree)
		{
			this.disable_outputs();
			this.tree_output = true;
		}
		if (selected == layer_output_enum.grass)
		{
			this.disable_outputs();
			this.grass_output = true;
		}
		if (selected == layer_output_enum.@object)
		{
			this.disable_outputs();
			this.object_output = true;
		}
	}

	public override bool swap_search_layer(prelayer_class prelayer1, prelayer_class prelayer2, layer_class layer, string text1, string text2)
	{
		int arg_1EB_0;
		if (prelayer2.index > 0)
		{
			for (int i = 0; i < prelayer1.layer.Count; i++)
			{
				for (int j = 0; j < prelayer1.layer[i].object_output.@object.Count; j++)
				{
					if (prelayer1.layer[i].object_output.@object[j].prelayer_created)
					{
						for (int k = 0; k < this.prelayers[prelayer1.layer[i].object_output.@object[j].prelayer_index].layer.Count; k++)
						{
							if (RuntimeServices.EqualityOperator(this.prelayers[prelayer1.layer[i].object_output.@object[j].prelayer_index].layer[k], layer))
							{
								prelayer1.layer[i].swap_text = prelayer1.layer[i].swap_text.Replace(text1, text2);
							}
							if (this.prelayers[prelayer1.layer[i].object_output.@object[j].prelayer_index].layer.Count > 0 && this.swap_search_layer(this.prelayers[prelayer1.layer[i].object_output.@object[j].prelayer_index], prelayer2, layer, text1, text2))
							{
								prelayer1.layer[i].swap_text = prelayer1.layer[i].swap_text.Replace(text1, text2);
								arg_1EB_0 = 1;
								return arg_1EB_0 != 0;
							}
						}
					}
				}
			}
		}
		arg_1EB_0 = 0;
		return arg_1EB_0 != 0;
	}

	public override void set_area_cube(int terrain_number)
	{
		terrain_class terrain_class = this.terrains[terrain_number];
		Vector3 localScale = default(Vector3);
		float num = terrain_class.terrain.terrainData.size.x / this.resolution;
		float num2 = terrain_class.terrain.terrainData.size.z / this.resolution;
		Vector3 vector = terrain_class.terrain.transform.position;
		localScale.x = (terrain_class.prearea.area.xMax - terrain_class.prearea.area.x) * num;
		localScale.z = (terrain_class.prearea.area.yMax - terrain_class.prearea.area.y) * num2;
		localScale.y = (float)50;
		vector.x += terrain_class.prearea.area.x * num + localScale.x / (float)2;
		vector.z += terrain_class.prearea.area.y * num2 + localScale.z / (float)2;
		vector.y = this.transform.position.y;
		this.transform.position = vector;
		this.transform.localScale = localScale;
	}

	public override void calc_area_max(area_class prearea)
	{
		Rect area_max = default(Rect);
		area_max.width = this.terrains[0].tiles.x * this.terrains[0].size.x;
		area_max.height = this.terrains[0].tiles.y * this.terrains[0].size.z;
		int index = 0;
		int i = 0;
		while (i < this.terrains.Count)
		{
			if (this.terrains[i].tile_z == (float)0 && this.terrains[i].tile_x == (float)0)
			{
				index = i;
				if (!this.terrains[i].terrain)
				{
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
		area_max.x = this.terrains[index].terrain.transform.position.x;
		area_max.y = this.terrains[index].terrain.transform.position.z;
		prearea.area_max = area_max;
		prearea.round_area_to_step(prearea.area_max);
		this.correct_area_max(prearea);
	}

	public override void correct_area_max(area_class prearea)
	{
		if (prearea.area.xMin < prearea.area_max.xMin)
		{
			prearea.area.xMin = prearea.area_max.xMin;
		}
		if (prearea.area.xMax > prearea.area_max.xMax)
		{
			prearea.area.xMax = prearea.area_max.xMax;
		}
		if (prearea.area.yMin < prearea.area_max.yMin)
		{
			prearea.area.yMin = prearea.area_max.yMin;
		}
		if (prearea.area.yMax > prearea.area_max.yMax)
		{
			prearea.area.yMax = prearea.area_max.yMax;
		}
	}

	public override void set_terrain_length(int length_new)
	{
		if (length_new != this.terrains.Count)
		{
			if (length_new > this.terrains.Count)
			{
				this.terrains.Add(new terrain_class());
				if (this.terrains.Count > 1)
				{
					this.terrains[this.terrains.Count - 1].size = this.terrains[this.terrains.Count - 2].size;
					this.terrains[this.terrains.Count - 1].heightmap_resolution_list = this.terrains[this.terrains.Count - 2].heightmap_resolution_list;
					this.terrains[this.terrains.Count - 1].splatmap_resolution_list = this.terrains[this.terrains.Count - 2].splatmap_resolution_list;
					this.terrains[this.terrains.Count - 1].basemap_resolution_list = this.terrains[this.terrains.Count - 2].basemap_resolution_list;
					this.terrains[this.terrains.Count - 1].detailmap_resolution_list = this.terrains[this.terrains.Count - 2].detailmap_resolution_list;
					this.terrains[this.terrains.Count - 1].detail_resolution_per_patch_list = this.terrains[this.terrains.Count - 2].detail_resolution_per_patch_list;
					this.set_terrain_resolution_from_list(this.terrains[this.terrains.Count - 1]);
				}
				this.terrains[this.terrains.Count - 1].prearea.area.xMax = this.resolution;
				this.terrains[this.terrains.Count - 1].prearea.area.yMax = this.resolution;
				this.terrains[this.terrains.Count - 1].index = this.terrains.Count - 1;
			}
			else
			{
				this.terrains.RemoveAt(this.terrains.Count - 1);
			}
		}
		if (this.terrains.Count == 1)
		{
			this.terrains[0].tile_x = (float)0;
			this.terrains[0].tile_z = (float)0;
			this.terrains[0].tiles = new Vector2((float)1, (float)1);
		}
		this.calc_terrain_one_more_tile();
		this.set_smooth_tool_terrain_popup();
		this.set_terrain_text();
	}

	public override void add_terrain(int terrain_number)
	{
		this.terrains.Insert(terrain_number, new terrain_class());
		this.terrains[this.terrains.Count - 1].prearea.area.xMax = this.resolution;
		this.terrains[this.terrains.Count - 1].prearea.area.yMax = this.resolution;
		this.terrains[this.terrains.Count - 1].index = terrain_number;
		this.set_smooth_tool_terrain_popup();
		this.set_terrain_text();
	}

	public override void clear_terrains()
	{
		this.terrains.Clear();
		this.set_terrain_length(1);
	}

	public override void clear_meshes()
	{
		this.meshes.Clear();
		this.meshes.Add(new mesh_class());
	}

	public override void clear_terrain_list()
	{
		for (int i = 1; i < this.terrains.Count; i++)
		{
			this.terrains.RemoveAt(i);
			i--;
		}
	}

	public override void set_terrain_text()
	{
		if (this.terrains.Count > 1)
		{
			this.terrain_text = "Terrains(" + this.terrains.Count + ")";
		}
		else
		{
			this.terrain_text = "Terrain(" + this.terrains.Count + ")";
		}
	}

	public override bool erase_raw_file(int raw_file_index)
	{
		bool arg_5F_0;
		if (raw_file_index < this.raw_files.Count)
		{
			this.raw_files[raw_file_index] = this.raw_files[this.raw_files.Count - 1];
			this.raw_files.RemoveAt(this.raw_files.Count - 1);
			this.loop_raw_file(raw_file_index, false, true, false);
			arg_5F_0 = true;
		}
		else
		{
			arg_5F_0 = false;
		}
		return arg_5F_0;
	}

	public override int check_raw_file_in_list(string file)
	{
		int arg_49_0;
		for (int i = 0; i < this.raw_files.Count; i++)
		{
			if (file.ToLower() == this.raw_files[i].file.ToLower())
			{
				arg_49_0 = i;
				return arg_49_0;
			}
		}
		arg_49_0 = -1;
		return arg_49_0;
	}

	public override void strip_auto_search_file(auto_search_class auto_search)
	{
		string format = new string("0"[0], auto_search.digits);
		string text = auto_search.format.Replace("%x", auto_search.start_x.ToString(format));
		text = text.Replace("%y", auto_search.start_y.ToString(format));
		text = text.Replace("%n", auto_search.start_n.ToString(format));
		auto_search.filename = Path.GetFileNameWithoutExtension(auto_search.path_full).Replace(text, string.Empty);
		auto_search.extension = Path.GetExtension(auto_search.path_full);
		if (auto_search.extension.Length != 0)
		{
			auto_search.filename = auto_search.filename.Replace(auto_search.extension, string.Empty);
		}
	}

	public override void clean_raw_file_list()
	{
		this.loop_raw_file(0, false, false, true);
		for (int i = 0; i < this.raw_files.Count; i++)
		{
			if (!this.raw_files[i].linked)
			{
				this.erase_raw_file(i);
			}
		}
	}

	public override bool loop_raw_file(int raw_file_index, bool check_double, bool relink, bool mark_linked)
	{
		int num = 0;
		if (mark_linked)
		{
			for (int i = 0; i < this.raw_files.Count; i++)
			{
				if (!this.raw_files[i].created)
				{
					this.raw_files[i].linked = false;
				}
			}
		}
		int arg_3AD_0;
		for (int j = 0; j < this.terrains.Count; j++)
		{
			if (this.terrains[j].raw_file_index > -1)
			{
				if (mark_linked)
				{
					this.raw_files[this.terrains[j].raw_file_index].linked = true;
				}
				if (check_double)
				{
					if (this.terrains[j].raw_file_index == raw_file_index)
					{
						num++;
					}
					if (num > 1)
					{
						arg_3AD_0 = 1;
						return arg_3AD_0 != 0;
					}
				}
				if (relink && this.terrains[j].raw_file_index == this.raw_files.Count)
				{
					this.terrains[j].raw_file_index = raw_file_index;
				}
			}
		}
		for (int k = 0; k < this.filter.Count; k++)
		{
			if (this.filter[k].raw != null)
			{
				for (int l = 0; l < this.filter[k].raw.file_index.Count; l++)
				{
					if (this.filter[k].raw.file_index[l] > -1)
					{
						if (mark_linked)
						{
							this.raw_files[this.filter[k].raw.file_index[l]].linked = true;
						}
						if (check_double)
						{
							if (this.filter[k].raw.file_index[l] == raw_file_index)
							{
								num++;
							}
							if (num > 1)
							{
								arg_3AD_0 = 1;
								return arg_3AD_0 != 0;
							}
						}
						if (relink && this.filter[k].raw.file_index[l] == this.raw_files.Count)
						{
							this.filter[k].raw.file_index[l] = raw_file_index;
						}
					}
				}
			}
		}
		for (int m = 0; m < this.subfilter.Count; m++)
		{
			if (this.subfilter[m].raw != null)
			{
				for (int l = 0; l < this.subfilter[m].raw.file_index.Count; l++)
				{
					if (this.subfilter[m].raw.file_index[l] > -1)
					{
						if (mark_linked)
						{
							this.raw_files[this.subfilter[m].raw.file_index[l]].linked = true;
						}
						if (check_double)
						{
							if (this.subfilter[m].raw.file_index[l] == raw_file_index)
							{
								num++;
							}
							if (num > 1)
							{
								arg_3AD_0 = 1;
								return arg_3AD_0 != 0;
							}
						}
						if (relink && this.subfilter[m].raw.file_index[l] == this.raw_files.Count)
						{
							this.subfilter[m].raw.file_index[l] = raw_file_index;
						}
					}
				}
			}
		}
		arg_3AD_0 = 0;
		return arg_3AD_0 != 0;
	}

	public override void reset_all_outputs()
	{
		this.terrain_reset_heightmap(this.terrains[0], true);
		this.terrain_all_reset_splat();
		this.terrain_reset_trees(this.terrains[0], true);
		this.terrain_reset_grass(this.terrains[0], true);
		this.terrain_reset_heightmap(this.terrains[0], true);
		this.loop_prelayer("(cpo)", 0, true);
	}

	public override void terrain_reset_heightmap(terrain_class preterrain1, bool all)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				float[,] array = (float[,])System.Array.CreateInstance(typeof(float), new int[]
				{
					preterrain1.terrain.terrainData.heightmapResolution,
					preterrain1.terrain.terrainData.heightmapResolution
				});
				if (!all)
				{
					if (preterrain1.terrain)
					{
						preterrain1.terrain.terrainData.SetHeights(0, 0, array);
						preterrain1.color_terrain = new Color(0.5f, 0.5f, (float)1, (float)1);
					}
				}
				else
				{
					for (int i = 0; i < this.terrains.Count; i++)
					{
						if (this.terrains[i].terrain)
						{
							if ((float)this.terrains[i].terrain.terrainData.heightmapResolution != Mathf.Sqrt((float)array.Length))
							{
								array = (float[,])System.Array.CreateInstance(typeof(float), new int[]
								{
									this.terrains[i].terrain.terrainData.heightmapResolution,
									this.terrains[i].terrain.terrainData.heightmapResolution
								});
							}
							this.terrains[i].terrain.terrainData.SetHeights(0, 0, array);
							this.terrains[i].color_terrain = new Color(0.5f, 0.5f, (float)1, (float)1);
						}
					}
				}
			}
		}
	}

	public override void terrain_reset_splat(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				if (preterrain1.splat_alpha != null)
				{
					if (preterrain1.splat_alpha.Length > 0)
					{
						this.texture_fill(preterrain1.splat_alpha[0], new Color((float)1, (float)0, (float)0, (float)0), true);
					}
					if (preterrain1.splat_alpha.Length > 1)
					{
						for (int i = 1; i < preterrain1.splat_alpha.Length; i++)
						{
							this.texture_fill(preterrain1.splat_alpha[i], new Color((float)0, (float)0, (float)0, (float)0), true);
						}
					}
					preterrain1.terrain.terrainData.SetAlphamaps(0, 0, preterrain1.terrain.terrainData.GetAlphamaps(0, 0, 1, 1));
					preterrain1.color_terrain = new Color(0.5f, 0.5f, (float)1, (float)1);
				}
			}
		}
	}

	public override void terrain_all_reset_splat()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.terrain_reset_splat(this.terrains[i]);
		}
	}

	public override void texture_fill(Texture2D texture, Color color, bool apply)
	{
		int width = texture.width;
		int num = texture.height;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < width; j++)
			{
				texture.SetPixel(j, i, color);
			}
		}
		if (apply)
		{
			texture.Apply();
		}
	}

	public override void terrain_reset_trees(terrain_class preterrain1, bool all)
	{
		if (!all)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.terrain.terrainData)
				{
					preterrain1.terrain.terrainData.treeInstances = new TreeInstance[0];
					preterrain1.color_terrain = new Color(0.5f, 0.5f, (float)1, (float)1);
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					if (this.terrains[i].terrain.terrainData)
					{
						this.terrains[i].terrain.terrainData.treeInstances = new TreeInstance[0];
						this.terrains[i].terrain.Flush();
						this.terrains[i].color_terrain = new Color(0.5f, 0.5f, (float)1, (float)1);
					}
				}
			}
		}
	}

	public override void terrain_reset_grass(terrain_class preterrain1, bool all)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				int[,] array = (int[,])System.Array.CreateInstance(typeof(int), new int[]
				{
					preterrain1.terrain.terrainData.detailResolution,
					preterrain1.terrain.terrainData.detailResolution
				});
				int i = 0;
				if (!all)
				{
					if (preterrain1.terrain)
					{
						for (i = 0; i < preterrain1.terrain.terrainData.detailPrototypes.Length; i++)
						{
							preterrain1.terrain.terrainData.SetDetailLayer(0, 0, i, array);
						}
						preterrain1.color_terrain = new Color(0.5f, 0.5f, (float)1, (float)1);
					}
				}
				else
				{
					for (int j = 0; j < this.terrains.Count; j++)
					{
						if (this.terrains[j].terrain)
						{
							if ((float)this.terrains[j].terrain.terrainData.detailResolution != Mathf.Sqrt((float)array.Length))
							{
								array = (int[,])System.Array.CreateInstance(typeof(int), new int[]
								{
									this.terrains[j].terrain.terrainData.detailResolution,
									this.terrains[j].terrain.terrainData.detailResolution
								});
							}
							for (i = 0; i < this.terrains[j].terrain.terrainData.detailPrototypes.Length; i++)
							{
								this.terrains[j].terrain.terrainData.SetDetailLayer(0, 0, i, array);
							}
							this.terrains[j].color_terrain = new Color(0.5f, 0.5f, (float)1, (float)1);
						}
					}
				}
			}
		}
	}

	public override bool terrains_check_double(terrain_class preterrain)
	{
		bool result = false;
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain == preterrain.terrain && preterrain.terrain != null && !RuntimeServices.EqualityOperator(this.terrains[i], preterrain))
			{
				preterrain.terrain = null;
				result = true;
			}
		}
		return result;
	}

	public override void erase_terrain(int number)
	{
		if (this.terrains.Count > 1)
		{
			this.terrains.RemoveAt(number);
		}
		else
		{
			this.terrains[number] = null;
		}
		if (this.terrains.Count == 1)
		{
			this.terrains[0].tile_x = (float)0;
			this.terrains[0].tile_z = (float)0;
			this.terrains[0].tiles = new Vector2((float)1, (float)1);
		}
		this.set_smooth_tool_terrain_popup();
		this.set_terrain_text();
	}

	public override void restore_references()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].name != string.Empty)
			{
				GameObject gameObject = GameObject.Find(this.terrains[i].name);
				if (gameObject)
				{
					this.terrains[i].terrain = (Terrain)gameObject.GetComponent(typeof(Terrain));
				}
			}
		}
		for (int j = 0; j < this.prelayers.Count; j++)
		{
			for (int k = 0; k < this.prelayers[j].layer.Count; k++)
			{
				for (int l = 0; l < this.prelayers[j].layer[k].object_output.@object.Count; l++)
				{
					object_class object_class = this.prelayers[j].layer[k].object_output.@object[l];
					if (object_class.name != string.Empty && !object_class.object1)
					{
						object_class.object1 = GameObject.Find(object_class.name);
					}
					if (object_class.parent_name != string.Empty && !object_class.parent)
					{
						GameObject gameObject2 = GameObject.Find(object_class.parent_name);
						if (gameObject2)
						{
							object_class.parent = gameObject2.transform;
						}
					}
				}
			}
		}
	}

	public override void set_references()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				this.terrains[i].name = this.terrains[i].terrain.name;
			}
		}
		for (int j = 0; j < this.prelayers.Count; j++)
		{
			for (int k = 0; k < this.prelayers[j].layer.Count; k++)
			{
				for (int l = 0; l < this.prelayers[j].layer[k].object_output.@object.Count; l++)
				{
					object_class object_class = this.prelayers[j].layer[k].object_output.@object[l];
					if (object_class.object1)
					{
						object_class.name = object_class.object1.name;
					}
					if (object_class.parent)
					{
						object_class.parent_name = object_class.parent.name;
					}
				}
			}
		}
	}

	public override float calc_color_pos(Color color, Color color_start, Color color_end)
	{
		Color color2 = color_start;
		Color color3 = default(Color);
		float num = 0f;
		float num2 = 0f;
		if (color_start.r > color_end.r)
		{
			color_start.r = color_end.r;
			color_end.r = color2.r;
		}
		if (color_start.g > color_end.g)
		{
			color_start.g = color_end.g;
			color_end.g = color2.g;
		}
		if (color_start.b > color_end.b)
		{
			color_start.b = color_end.b;
			color_end.b = color2.b;
		}
		color3 = color_end - color_start;
		color -= color_start;
		float arg_1D8_0;
		if (color.r < (float)0 || color.g < (float)0 || color.b < (float)0)
		{
			arg_1D8_0 = (float)0;
		}
		else if (color.r > color3.r || color.g > color3.g || color.b > color3.b)
		{
			arg_1D8_0 = (float)0;
		}
		else
		{
			num += color3.r + color3.g + color3.b;
			num2 += color.r + color.g + color.b;
			arg_1D8_0 = ((num == (float)0) ? ((float)1) : (num2 / num));
		}
		return arg_1D8_0;
	}

	public override bool color_in_range(Color color, Color color_start, Color color_end)
	{
		Color color2 = color_start;
		if (color_start.r > color_end.r)
		{
			color_start.r = color_end.r;
			color_end.r = color2.r;
		}
		if (color_start.g > color_end.g)
		{
			color_start.g = color_end.g;
			color_end.g = color2.g;
		}
		if (color_start.b > color_end.b)
		{
			color_start.b = color_end.b;
			color_end.b = color2.b;
		}
		return color.r >= color_start.r && color.r <= color_end.r && color.g >= color_start.g && color.g <= color_end.g && color.b >= color_start.b && color.b <= color_end.b;
	}

	public override Color random_color_from_range(Color color_start, Color color_end)
	{
		Color result = default(Color);
		AnimationCurve animationCurve = AnimationCurve.Linear((float)0, color_start.r, (float)1, color_end.r);
		AnimationCurve animationCurve2 = AnimationCurve.Linear((float)0, color_start.g, (float)1, color_end.g);
		AnimationCurve animationCurve3 = AnimationCurve.Linear((float)0, color_start.b, (float)1, color_end.b);
		float time = UnityEngine.Random.Range((float)0, 1f);
		result.r = animationCurve.Evaluate(time);
		result.g = animationCurve2.Evaluate(time);
		result.b = animationCurve3.Evaluate(time);
		return result;
	}

	public override void convert_16to8_bit(float value)
	{
		value *= (float)65535;
		ushort num = (ushort)value;
		this.byte_hi = (int)((uint)num >> 8);
		this.byte_lo = (int)num - (this.byte_hi << 8);
	}

	public override void convert_24to8_bit(float value)
	{
		value *= (float)8388608;
		uint num = (uint)value;
		this.byte_hi2 = (int)(num >> 16);
		this.byte_hi = (int)((long)num - (long)((long)this.byte_hi2 << 16) >> (int)8L);
		this.byte_lo = (int)((long)num - (long)((long)this.byte_hi2 << 16) - (long)((long)this.byte_hi << 8));
	}

	public override int export_meshcapture()
	{
		int arg_772_0;
		if (!this.meshcapture_tool_object)
		{
			arg_772_0 = -1;
		}
		else
		{
			MeshFilter[] componentsInChildren = this.meshcapture_tool_object.GetComponentsInChildren<MeshFilter>();
			this.meshcapture_tool_image = new Texture2D(this.meshcapture_tool_image_width, this.meshcapture_tool_image_height);
			Vector3[] array = new Vector3[3];
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			this.meshcapture_tool_image.wrapMode = TextureWrapMode.Clamp;
			if (this.meshcapture_tool_pivot)
			{
				vector = this.meshcapture_tool_pivot.position;
			}
			else
			{
				vector = this.meshcapture_tool_object.transform.position;
			}
			if (componentsInChildren != null)
			{
				Color[] array2 = new Color[this.meshcapture_tool_image_width * this.meshcapture_tool_image_height];
				int i = 0;
				Color[] array3 = array2;
				int length = array3.Length;
				while (i < length)
				{
					array3[i] = this.meshcapture_background_color;
					i++;
				}
				this.meshcapture_tool_image.SetPixels(array2);
				int j = 0;
				MeshFilter[] array4 = componentsInChildren;
				int length2 = array4.Length;
				while (j < length2)
				{
					Mesh sharedMesh = ((MeshFilter)array4[j].GetComponent(typeof(MeshFilter))).sharedMesh;
					if (sharedMesh)
					{
						Vector3[] vertices = sharedMesh.vertices;
						int[] triangles = sharedMesh.triangles;
						Transform transform = array4[j].gameObject.transform;
						Vector3[] normals = sharedMesh.normals;
						for (int k = 0; k < triangles.Length / 3; k++)
						{
							int num = triangles[k * 3];
							int num2 = triangles[k * 3 + 1];
							int num3 = triangles[k * 3 + 2];
							array[0] = transform.TransformPoint(vertices[num]) - vector;
							array[1] = transform.TransformPoint(vertices[num2]) - vector;
							array[2] = transform.TransformPoint(vertices[num3]) - vector;
							Vector3 vector3 = array[0];
							Vector3 vector4 = normals[num];
							vector4.Normalize();
							vector3.Normalize();
							this.color1 = this.meshcapture_tool_color;
							if (this.meshcapture_tool_shadows)
							{
								this.color1.r = Mathf.Abs((vector4.x + vector4.y + vector4.z) / (float)3);
								this.color1.g = Mathf.Abs((vector4.x + vector4.y + vector4.z) / (float)3);
								this.color1.b = Mathf.Abs((vector4.x + vector4.y + vector4.z) / (float)3);
								this.color1.r = this.color1.r * this.meshcapture_tool_color.r;
								this.color1.g = this.color1.g * this.meshcapture_tool_color.g;
								this.color1.b = this.color1.b * this.meshcapture_tool_color.b;
							}
							array[0].x = array[0].x * this.meshcapture_tool_scale;
							array[0].z = array[0].z * this.meshcapture_tool_scale;
							array[0].y = array[0].y * this.meshcapture_tool_scale;
							array[0].x = array[0].x + (float)(this.meshcapture_tool_image_width / 2);
							array[0].z = array[0].z + (float)(this.meshcapture_tool_image_height / 2);
							array[0].y = array[0].y + (float)(this.meshcapture_tool_image_height / 2);
							array[1].x = array[1].x * this.meshcapture_tool_scale;
							array[1].z = array[1].z * this.meshcapture_tool_scale;
							array[1].y = array[1].y * this.meshcapture_tool_scale;
							array[1].x = array[1].x + (float)(this.meshcapture_tool_image_width / 2);
							array[1].z = array[1].z + (float)(this.meshcapture_tool_image_height / 2);
							array[1].y = array[1].y + (float)(this.meshcapture_tool_image_height / 2);
							array[2].x = array[2].x * this.meshcapture_tool_scale;
							array[2].z = array[2].z * this.meshcapture_tool_scale;
							array[2].y = array[2].y * this.meshcapture_tool_scale;
							array[2].x = array[2].x + (float)(this.meshcapture_tool_image_width / 2);
							array[2].z = array[2].z + (float)(this.meshcapture_tool_image_height / 2);
							array[2].y = array[2].y + (float)(this.meshcapture_tool_image_height / 2);
							float num4 = (float)0;
							float num5 = (float)700;
							float num6 = (float)900;
							this.Line(this.meshcapture_tool_image, (int)(array[0].x - num4), (int)array[0].z, (int)(array[1].x - num4), (int)array[1].z, this.color1);
							this.Line(this.meshcapture_tool_image, (int)(array[0].x - num4), (int)array[0].z, (int)(array[2].x - num4), (int)array[2].z, this.color1);
							this.Line(this.meshcapture_tool_image, (int)(array[1].x - num4), (int)array[1].z, (int)(array[2].x - num4), (int)array[2].z, this.color1);
						}
					}
					j++;
				}
				if (this.meshcapture_tool_save_scale)
				{
					Color color = this.convert_float_to_color(this.meshcapture_tool_scale);
					Color color2 = default(Color);
					color2 = this.meshcapture_tool_image.GetPixel(0, 0);
					color2[3] = color[0];
					this.meshcapture_tool_image.SetPixel(0, 0, color2);
					color2 = this.meshcapture_tool_image.GetPixel(1, 0);
					color2[3] = color[1];
					this.meshcapture_tool_image.SetPixel(1, 0, color2);
					color2 = this.meshcapture_tool_image.GetPixel(2, 0);
					color2[3] = color[2];
					this.meshcapture_tool_image.SetPixel(2, 0, color2);
					color2 = this.meshcapture_tool_image.GetPixel(3, 0);
					color2[3] = color[3];
					this.meshcapture_tool_image.SetPixel(3, 0, color2);
				}
				this.meshcapture_tool_image.Apply();
			}
			arg_772_0 = 1;
		}
		return arg_772_0;
	}

	public override void Line(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
	{
		int num = y1 - y0;
		int num2 = x1 - x0;
		int num3 = 0;
		int num4 = 0;
		if (num < 0)
		{
			num = -num;
			num4 = -1;
		}
		else
		{
			num4 = 1;
		}
		if (num2 < 0)
		{
			num2 = -num2;
			num3 = -1;
		}
		else
		{
			num3 = 1;
		}
		num <<= 1;
		num2 <<= 1;
		tex.SetPixel(x0, y0, col);
		if (num2 > num)
		{
			int num5 = num - (num2 >> 1);
			while (x0 != x1)
			{
				if (num5 >= 0)
				{
					y0 += num4;
					num5 -= num2;
				}
				x0 += num3;
				num5 += num;
				tex.SetPixel(x0, y0, col);
			}
		}
		else
		{
			int num5 = num2 - (num >> 1);
			while (y0 != y1)
			{
				if (num5 >= 0)
				{
					x0 += num3;
					num5 -= num;
				}
				y0 += num4;
				num5 += num2;
				tex.SetPixel(x0, y0, col);
			}
		}
	}

	public override void texture_fill_color(Texture2D texture1, Color color)
	{
		float num = (float)(texture1.width * texture1.height);
		Color[] array = new Color[(int)num];
		int i = 0;
		Color[] array2 = array;
		int length = array2.Length;
		while (i < length)
		{
			array2[i] = color;
			i++;
		}
		texture1.SetPixels(0, 0, texture1.width, texture1.height, array);
	}

	public override void tree_placed_reset()
	{
		for (int i = 0; i < this.prelayer.layer.Count; i++)
		{
			if (this.prelayer.layer[i].output == layer_output_enum.tree)
			{
				for (int j = 0; j < this.prelayer.layer[i].tree_output.tree.Count; j++)
				{
					this.prelayer.layer[i].tree_output.tree[j].placed = 0;
				}
			}
		}
	}

	public override void disable_outputs()
	{
		this.heightmap_output = false;
		this.color_output = false;
		this.splat_output = false;
		this.tree_output = false;
		this.grass_output = false;
		this.object_output = false;
	}

	public override bool check_treemap()
	{
		int arg_63_0;
		for (int i = 0; i < this.settings.treemap.Count; i++)
		{
			if (this.settings.treemap[i].map && this.settings.treemap[i].load)
			{
				arg_63_0 = 1;
				return arg_63_0 != 0;
			}
		}
		arg_63_0 = 0;
		return arg_63_0 != 0;
	}

	public override bool check_grassmap()
	{
		int arg_63_0;
		for (int i = 0; i < this.settings.grassmap.Count; i++)
		{
			if (this.settings.grassmap[i].map && this.settings.grassmap[i].load)
			{
				arg_63_0 = 1;
				return arg_63_0 != 0;
			}
		}
		arg_63_0 = 0;
		return arg_63_0 != 0;
	}

	public override int generate_begin_mesh()
	{
		this.prelayer = this.prelayers[0];
		UnityEngine.Random.seed = this.seed;
		this.prelayer.count_terrain = 0;
		for (int i = 0; i < this.meshes.Count; i++)
		{
			if (!this.meshes[i].gameObject || !this.meshes[i].meshFilter || !this.meshes[i].mesh || !this.meshes[i].active)
			{
				this.meshes.RemoveAt(i);
				i--;
			}
		}
		this.get_meshes_areas();
		this.calc_total_mesh_area();
		this.get_meshes_minmax_height();
		this.generate_pause = false;
		this.prelayer_stack.Add(0);
		this.link_placed_reference();
		this.loop_prelayer("(gfc)(slv)(ocr)(asr)(cpo)(ias)(eho)(st)(cmn)(ed)", 0, false);
		int arg_16B_0;
		if (!this.find_mesh())
		{
			arg_16B_0 = -1;
		}
		else if (this.prelayer.layer.Count == 0)
		{
			arg_16B_0 = -2;
		}
		else
		{
			if (this.prelayers.Count > 1)
			{
				this.area_stack_enabled = true;
			}
			this.load_raw_heightmaps();
			this.generate_time_start = Time.realtimeSinceStartup;
			this.generate_time = (float)0;
			this.tree_number = 0;
			this.auto_speed_time = Time.realtimeSinceStartup;
			this.generate_mesh_start();
			arg_16B_0 = 1;
		}
		return arg_16B_0;
	}

	public override int generate_begin()
	{
		if (this.heightmap_output && !this.color_output && !this.splat_output && !this.tree_output && !this.grass_output && !this.object_output)
		{
			this.only_heightmap = true;
		}
		this.prelayer = this.prelayers[0];
		UnityEngine.Random.seed = this.seed;
		this.prelayer.count_terrain = 0;
		this.generate_world_mode = this.prelayers[0].prearea.active;
		int num = 0;
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.terrains[i].index = i;
			this.terrains[i].index_old = num;
			num++;
			if (!this.terrains[i].terrain)
			{
				this.terrains.RemoveAt(i);
				i--;
			}
			else if (!this.terrains[i].terrain.terrainData)
			{
				this.terrains.RemoveAt(i);
				i--;
			}
		}
		int arg_8F2_0;
		if (this.terrains.Count == 0)
		{
			arg_8F2_0 = -2;
		}
		else
		{
			this.get_terrains_position();
			for (int i = 0; i < this.terrains.Count; i++)
			{
				this.terrains[i].heightmap_resolution = (float)this.terrains[i].terrain.terrainData.heightmapResolution;
				this.terrains[i].splatmap_resolution = (float)this.terrains[i].terrain.terrainData.alphamapResolution;
				this.terrains[i].detail_resolution = (float)this.terrains[i].terrain.terrainData.detailResolution;
				if (this.terrains[i].prearea.resolution_mode == resolution_mode_enum.Automatic)
				{
					this.select_automatic_step_resolution(this.terrains[i], this.terrains[i].prearea);
				}
				this.set_area_resolution(this.terrains[i], this.terrains[i].prearea);
				this.terrains[i].prearea.round_area_to_step(this.terrains[i].prearea.area);
				this.terrains[i].prearea.area_old = this.terrains[i].prearea.area;
				if (!this.world_output)
				{
					this.terrains[i].prearea.area.x = this.terrains[i].prearea.area.x + this.terrains[i].terrain.transform.position.x;
					this.terrains[i].prearea.area.y = this.terrains[i].prearea.area.y + this.terrains[i].terrain.transform.position.z;
				}
				if (!this.object_output)
				{
					this.terrains[i].prearea.area.xMax = this.terrains[i].prearea.area.xMax + this.terrains[i].prearea.step.x / (float)2;
					this.terrains[i].prearea.area.yMin = this.terrains[i].prearea.area.yMin - this.terrains[i].prearea.step.y / (float)2;
				}
				if (this.color_output)
				{
					this.terrains[i].color_length = 3;
					this.terrains[i].color = new float[3];
					this.terrains[i].color_layer = new float[3];
				}
				if (this.splat_output)
				{
					this.terrains[i].splat_length = this.terrains[i].terrain.terrainData.splatPrototypes.Length;
					if (this.terrains[i].splat_length == 0 && (this.color_output || this.splat_output))
					{
						this.preterrain = this.terrains[i];
						arg_8F2_0 = -3;
						return arg_8F2_0;
					}
					if (this.terrains[i].splat_length == 1 && this.splat_output)
					{
						this.preterrain = this.terrains[i];
						arg_8F2_0 = -5;
						return arg_8F2_0;
					}
					if (this.terrains[i].splat_length < this.terrains[0].splat_length)
					{
						this.preterrain = this.terrains[i];
						arg_8F2_0 = -7;
						return arg_8F2_0;
					}
					this.terrains[i].splat = new float[this.terrains[i].splat_length];
					this.terrains[i].splat_layer = new float[(int)(Mathf.Ceil((float)this.terrains[i].splat_length / 4f) * 4f)];
				}
				if (this.button_export && !this.export_texture)
				{
					this.export_texture = new Texture2D((int)this.terrains[0].prearea.resolution, (int)this.terrains[0].prearea.resolution, TextureFormat.RGB24, false);
				}
				if (this.grass_output)
				{
					if (this.terrains[i].terrain.terrainData.detailPrototypes.Length < this.terrains[0].terrain.terrainData.detailPrototypes.Length)
					{
						this.preterrain = this.terrains[i];
						arg_8F2_0 = -8;
						return arg_8F2_0;
					}
					this.terrains[i].grass = new float[this.terrains[i].terrain.terrainData.detailPrototypes.Length];
				}
			}
			if (this.heightmap_output)
			{
				this.heights = (float[,])System.Array.CreateInstance(typeof(float), new int[]
				{
					this.terrains[0].terrain.terrainData.heightmapResolution,
					this.terrains[0].terrain.terrainData.heightmapResolution
				});
			}
			if (this.grass_output)
			{
				this.grass_detail = new detail_class[this.terrains[0].terrain.terrainData.detailPrototypes.Length];
				for (int j = 0; j < this.grass_detail.Length; j++)
				{
					this.grass_detail[j] = new detail_class();
					this.grass_detail[j].detail = (int[,])System.Array.CreateInstance(typeof(int), new int[]
					{
						(int)this.terrains[0].detail_resolution,
						(int)this.terrains[0].detail_resolution
					});
				}
				this.grass_resolution_old = (int)this.preterrain.detail_resolution;
			}
			this.generate_pause = false;
			this.prelayer_stack.Add(0);
			this.link_placed_reference();
			this.loop_prelayer("(gfc)(slv)(ocr)(asr)(cpo)(ias)(eho)(st)(cmn)(ed)", 0, false);
			if (!this.find_terrain(true))
			{
				arg_8F2_0 = -1;
			}
			else
			{
				this.preterrain = this.terrains[this.prelayer.count_terrain];
				if (this.generate_world_mode)
				{
				}
				this.load_raw_heightmaps();
				if (!this.heightmap_output_layer)
				{
					this.heightmap_output = false;
				}
				if (this.prelayer.layer.Count == 0)
				{
					if ((!this.tree_output || !this.check_treemap()) && (!this.grass_output || !this.check_grassmap()))
					{
						arg_8F2_0 = -2;
						return arg_8F2_0;
					}
				}
				if (this.prelayers.Count > 1)
				{
					this.area_stack_enabled = true;
				}
				this.generate_time_start = Time.realtimeSinceStartup;
				this.generate_time = (float)0;
				this.tree_number = 0;
				this.auto_speed_time = Time.realtimeSinceStartup;
				this.generate_terrain_start();
				arg_8F2_0 = 1;
			}
		}
		return arg_8F2_0;
	}

	public override void select_automatic_step_resolution(terrain_class preterrain1, area_class prearea)
	{
		int num = 0;
		if (preterrain1.terrain.terrainData.heightmapResolution > num && this.heightmap_output)
		{
			num = preterrain1.terrain.terrainData.heightmapResolution;
			prearea.resolution_mode = resolution_mode_enum.Heightmap;
		}
		if (preterrain1.terrain.terrainData.alphamapResolution > num && this.color_output)
		{
			num = preterrain1.prearea.colormap_resolution;
			prearea.resolution_mode = resolution_mode_enum.Colormap;
		}
		if (preterrain1.terrain.terrainData.alphamapResolution > num && this.splat_output)
		{
			num = preterrain1.terrain.terrainData.alphamapResolution;
			prearea.resolution_mode = resolution_mode_enum.Splatmap;
		}
		if (preterrain1.terrain.terrainData.detailResolution > num && this.grass_output)
		{
			num = preterrain1.terrain.terrainData.detailResolution;
			prearea.resolution_mode = resolution_mode_enum.Detailmap;
		}
		if (prearea.tree_resolution > num && this.tree_output)
		{
			num = prearea.tree_resolution;
			prearea.resolution_mode = resolution_mode_enum.Tree;
		}
		if (prearea.object_resolution > num && this.object_output)
		{
			num = prearea.object_resolution;
			prearea.resolution_mode = resolution_mode_enum.Object;
		}
		if (num == 0)
		{
			prearea.resolution_mode = resolution_mode_enum.Detailmap;
		}
	}

	public override void select_automatic_step_resolution_mesh(area_class prearea)
	{
		this.resolution = this.object_resolution;
		prearea.resolution_mode = resolution_mode_enum.Object;
	}

	public override void generate_mesh_start()
	{
		this.premesh = this.meshes[this.prelayer.count_terrain];
		Rect rect = this.get_mesh_area(this.prelayer.count_terrain);
		rect.xMin = Mathf.Ceil(rect.xMin / this.object_resolution) * this.object_resolution;
		rect.yMin = Mathf.Ceil(rect.yMin / this.object_resolution) * this.object_resolution;
		rect.xMax = Mathf.Floor(rect.xMax / this.object_resolution) * this.object_resolution;
		rect.yMax = Mathf.Floor(rect.yMax / this.object_resolution) * this.object_resolution;
		this.prelayer.prearea.area.xMin = rect.xMin;
		this.prelayer.prearea.area.yMin = rect.yMin;
		this.prelayer.prearea.area.width = rect.width;
		this.prelayer.prearea.area.height = rect.height;
		this.prelayer.y = this.prelayer.prearea.area.yMax;
	}

	public override Rect get_mesh_area(int count_terrain)
	{
		return new Rect
		{
			xMin = this.meshes[count_terrain].mesh.bounds.center.x - this.meshes[count_terrain].mesh.bounds.extents.x + this.meshes[count_terrain].gameObject.transform.position.x,
			yMin = this.meshes[count_terrain].mesh.bounds.center.z - this.meshes[count_terrain].mesh.bounds.extents.z + this.meshes[count_terrain].gameObject.transform.position.z,
			width = this.meshes[count_terrain].mesh.bounds.size.x,
			height = this.meshes[count_terrain].mesh.bounds.size.z
		};
	}

	public override void get_meshes_areas()
	{
		for (int i = 0; i < this.meshes.Count; i++)
		{
			this.meshes[i].area = this.get_mesh_area(i);
		}
	}

	public override void calc_total_mesh_area()
	{
		Rect rect = default(Rect);
		this.meshes_area.area = this.get_mesh_area(0);
		for (int i = 1; i < this.meshes.Count; i++)
		{
			rect = this.get_mesh_area(i);
			if (rect.xMin < this.meshes_area.area.xMin)
			{
				this.meshes_area.area.xMin = rect.xMin;
			}
			if (rect.xMax > this.meshes_area.area.xMax)
			{
				this.meshes_area.area.xMax = rect.xMax;
			}
			if (rect.yMin < this.meshes_area.area.yMin)
			{
				this.meshes_area.area.yMin = rect.yMin;
			}
			if (rect.yMax > this.meshes_area.area.yMax)
			{
				this.meshes_area.area.yMax = rect.yMax;
			}
		}
	}

	public override void GetMeshHeightSlope(mesh_class premesh1, Vector2 position)
	{
		Ray ray = default(Ray);
		ray.direction = new Vector3((float)0, (float)-1, (float)0);
		ray.origin = new Vector3(position.x, premesh1.transform.position.y + (float)1000, position.y);
		RaycastHit raycastHit = default(RaycastHit);
		RaycastHit raycastHit2 = default(RaycastHit);
		if (Physics.Raycast(ray, out raycastHit, (float)2000) && (raycastHit.collider.gameObject.layer & this.meshes_layer) == this.meshes_layer)
		{
			this.mesh_measure.hit = true;
			this.mesh_measure.height = raycastHit.point.y;
			this.mesh_measure.degree = (float)1 - (raycastHit.normal.y - 0.5f) * (float)2;
			this.mesh_measure.normal = raycastHit.normal;
			this.mesh_measure.transform = raycastHit.transform;
		}
		else
		{
			this.mesh_measure.hit = false;
		}
	}

	public override void generate_terrain_start()
	{
		if (!this.generate_world_mode)
		{
			this.prelayer.prearea = this.preterrain.prearea;
			for (int i = 0; i < this.terrains.Count; i++)
			{
				this.terrains[i].on_row = false;
			}
			this.preterrain.on_row = true;
		}
		this.prelayer.y = this.prelayer.prearea.area.yMax;
		this.select_image_prelayer();
		if (this.button_export && this.heightmap_output)
		{
			this.export_bytes = new byte[(int)(Mathf.Pow((float)this.preterrain.terrain.terrainData.heightmapResolution, (float)2) * (float)2)];
		}
	}

	public override int generate_output(prelayer_class prelayer3)
	{
		this.generate_error = true;
		int arg_3351_0;
		if (prelayer3.prearea.step.x == (float)0 || prelayer3.prearea.step.y == (float)0)
		{
			this.generate = false;
			Debug.Log("Area size is 0...");
			arg_3351_0 = -1;
		}
		else
		{
			this.frames = (float)1 / (Time.realtimeSinceStartup - this.auto_speed_time);
			this.auto_speed_time = Time.realtimeSinceStartup;
			this.break_x = false;
			this.row_object_count = 0;
			prelayer3.counter_y = prelayer3.y;
			while (prelayer3.counter_y >= prelayer3.y - (float)this.generate_speed * this.prelayer.prearea.step.y)
			{
				this.generate_call_time = Time.realtimeSinceStartup;
				float y = prelayer3.y;
				int i = 0;
				if (prelayer3.counter_y < prelayer3.prearea.area.yMin)
				{
					if (this.prelayer_stack.Count > 1)
					{
						if (this.line_output)
						{
							this.line_generate(prelayer3.index);
						}
						this.prelayer_stack.RemoveAt(this.prelayer_stack.Count - 1);
						this.prelayer = this.prelayers[this.prelayer_stack[this.prelayer_stack.Count - 1]];
						this.generate_error = false;
						arg_3351_0 = 2;
						return arg_3351_0;
					}
					if (this.generate_world_mode)
					{
						this.generate = false;
						for (i = 0; i < this.terrains.Count; i++)
						{
							this.terrain_apply(this.terrains[i]);
						}
					}
					else if (this.settings.showTerrains)
					{
						if (prelayer3.count_terrain >= this.terrains.Count - 1)
						{
							this.generate = false;
						}
						this.terrain_apply(this.terrains[prelayer3.count_terrain]);
					}
					else if (prelayer3.count_terrain >= this.meshes.Count - 1)
					{
						this.generate = false;
					}
					if (this.button_export)
					{
						this.export_name = this.export_file;
						if (this.terrains.Count > 1)
						{
							this.export_name += "_" + this.prelayer.count_terrain;
						}
						if ((this.settings.colormap || this.preterrain.rtp_script) && (this.settings.colormap_auto_assign || this.settings.normalmap_auto_assign))
						{
							this.script_base.preterrain = this.script_base.terrains[prelayer3.count_terrain];
						}
						this.generate_export = 1;
					}
					this.generate_time = Time.realtimeSinceStartup - this.generate_time_start;
					if (this.generate)
					{
						prelayer3.count_terrain++;
						if (this.settings.showTerrains)
						{
							if (this.find_terrain(false))
							{
								this.preterrain = this.terrains[prelayer3.count_terrain];
								this.generate_terrain_start();
							}
							else
							{
								this.generate = false;
							}
						}
						else if (this.find_mesh())
						{
							this.generate_mesh_start();
						}
						else
						{
							this.generate = false;
						}
					}
					else
					{
						if (this.settings.showTerrains)
						{
							this.set_neighbor(1);
						}
						this.object_apply();
					}
					if (!this.generate && this.line_output)
					{
						this.line_generate(0);
					}
					this.generate_error = false;
					arg_3351_0 = 2;
					return arg_3351_0;
				}
				else
				{
					if (this.generate_world_mode || prelayer3.index > 0)
					{
						for (i = 0; i < this.terrains.Count; i++)
						{
							if (this.terrains[i].rect.Contains(new Vector2(this.terrains[i].prearea.area.x, y)))
							{
								this.terrains[i].on_row = true;
							}
							else
							{
								this.terrains[i].on_row = false;
							}
						}
					}
					prelayer3.x = prelayer3.prearea.area.x + prelayer3.break_x_value;
					while (prelayer3.x <= prelayer3.prearea.area.xMax)
					{
						float x = prelayer3.x;
						float num = 0f;
						float num2 = 0f;
						float num3 = 0f;
						float num4 = prelayer3.counter_y;
						if (!this.generate_world_mode && prelayer3.index <= 0)
						{
							goto IL_4E3;
						}
						bool flag = true;
						for (i = 0; i < this.terrains.Count; i++)
						{
							if (this.terrains[i].rect.Contains(new Vector2(x, num4)))
							{
								flag = false;
								this.preterrain = this.terrains[i];
								break;
							}
						}
						if (!flag)
						{
							goto IL_4E3;
						}
						IL_327A:
						prelayer3.x += prelayer3.prearea.step.x;
						continue;
						IL_4E3:
						this.local_x = x - this.preterrain.rect.x;
						this.local_y = num4 - this.preterrain.rect.y;
						if (prelayer3.prearea.rotation_active)
						{
							Vector2 vector = this.calc_rotation_pixel(x, num4, prelayer3.prearea.center.x, prelayer3.prearea.center.y, prelayer3.prearea.rotation.y);
							x = vector.x;
							num4 = vector.y;
						}
						this.local_x_rot = x - this.preterrain.rect.x;
						this.local_y_rot = num4 - this.preterrain.rect.y;
						if (!this.only_heightmap)
						{
							if (this.settings.showTerrains)
							{
								this.degree = this.calc_terrain_angle(this.preterrain, this.local_x_rot, this.local_y_rot, this.settings.smooth_angle) * this.settings.global_degree_strength + this.settings.global_degree_level;
								this.height = this.preterrain.terrain.terrainData.GetHeight((int)(this.local_x_rot / this.preterrain.heightmap_conversion.x), (int)(this.local_y_rot / this.preterrain.heightmap_conversion.y)) / this.preterrain.size.y * this.settings.global_height_strength + this.settings.global_height_level;
								if (this.measure_normal)
								{
									this.normal = this.preterrain.terrain.terrainData.GetInterpolatedNormal(this.local_x_rot / this.preterrain.size.x, this.local_y_rot / this.preterrain.size.z);
								}
							}
							else
							{
								this.GetMeshHeightSlope(this.premesh, new Vector2(x, num4));
								if (!this.mesh_measure.hit)
								{
									goto IL_327A;
								}
								this.degree = this.mesh_measure.degree * (float)90;
								this.height = (this.mesh_measure.height - this.mesh_measure.transform.position.y) / this.meshes_heightscale;
								this.normal = this.mesh_measure.normal;
							}
						}
						this.random_range = UnityEngine.Random.Range((float)0, 1000f);
						if (!this.heightmap_output)
						{
							this.map_x = (int)Mathf.Round(this.local_x_rot / this.preterrain.splatmap_conversion.x);
							this.map_y = (int)Mathf.Round(this.local_y_rot / this.preterrain.splatmap_conversion.y);
							if ((float)this.map_y > this.preterrain.splatmap_resolution - (float)1)
							{
								this.map_y = (int)(this.preterrain.splatmap_resolution - (float)1);
							}
							else if (this.map_y < 0)
							{
								this.map_y = 0;
							}
							if ((float)this.map_x > this.preterrain.splatmap_resolution - (float)1)
							{
								this.map_x = (int)(this.preterrain.splatmap_resolution - (float)1);
							}
							else if (this.map_x < 0)
							{
								this.map_x = 0;
							}
						}
						if (this.grass_output)
						{
							this.detailmap_x = (int)Mathf.Floor(this.local_x_rot / this.preterrain.detailmap_conversion.x);
							this.detailmap_y = (int)Mathf.Floor(this.local_y_rot / this.preterrain.detailmap_conversion.y);
							if ((float)this.detailmap_x > this.preterrain.detail_resolution - (float)1)
							{
								this.detailmap_x = (int)(this.preterrain.detail_resolution - (float)1);
							}
							else if (this.detailmap_x < 0)
							{
								this.detailmap_x = 0;
							}
							if ((float)this.detailmap_y > this.preterrain.detail_resolution - (float)1)
							{
								this.detailmap_y = (int)(this.preterrain.detail_resolution - (float)1);
							}
							else if (this.detailmap_y < 0)
							{
								this.detailmap_y = 0;
							}
						}
						else if (this.heightmap_output)
						{
							this.heightmap_x = (int)Mathf.Round(this.local_x_rot / this.preterrain.heightmap_conversion.x);
							this.heightmap_y = (int)Mathf.Round(this.local_y_rot / this.preterrain.heightmap_conversion.y);
							this.heightmap_x_old = this.heightmap_x;
							this.heightmap_y_old = this.heightmap_y;
							if ((float)this.heightmap_y > this.preterrain.heightmap_resolution - (float)1)
							{
								this.heightmap_y = (int)(this.preterrain.heightmap_resolution - (float)1);
							}
							else if (this.heightmap_y < 0)
							{
								this.heightmap_y = 0;
							}
							if ((float)this.heightmap_x > this.preterrain.heightmap_resolution - (float)1)
							{
								this.heightmap_x = (int)(this.preterrain.heightmap_resolution - (float)1);
							}
							else if (this.heightmap_x < 0)
							{
								this.heightmap_x = 0;
							}
							this.heights[this.heightmap_y, this.heightmap_x] = (float)0;
						}
						this.overlap = false;
						int j = 0;
						while (j < prelayer3.layer.Count)
						{
							this.current_layer = prelayer3.layer[j];
							this.filter_value = (float)0;
							this.filter_strength = (float)1;
							if (this.current_layer.output == layer_output_enum.heightmap)
							{
								this.layer_x = (float)this.heightmap_x * this.preterrain.heightmap_conversion.x;
								this.layer_y = (float)this.heightmap_y * this.preterrain.heightmap_conversion.y;
							}
							else if (this.current_layer.output == layer_output_enum.color || this.current_layer.output == layer_output_enum.splat)
							{
								this.layer_x = (float)this.map_x * this.preterrain.splatmap_conversion.x;
								this.layer_y = (float)this.map_y * this.preterrain.splatmap_conversion.y;
							}
							else if (this.current_layer.output == layer_output_enum.grass)
							{
								this.layer_x = (float)this.detailmap_x * this.preterrain.detailmap_conversion.x;
								this.layer_y = (float)this.detailmap_y * this.preterrain.detailmap_conversion.y;
							}
							else
							{
								this.layer_x = this.local_x_rot;
								this.layer_y = this.local_y_rot;
							}
							for (int k = 0; k < this.current_layer.prefilter.filter_index.Count; k++)
							{
								this.calc_filter_value(this.filter[this.current_layer.prefilter.filter_index[k]], num4, x);
							}
							layer_output_enum output = this.current_layer.output;
							if (output == layer_output_enum.tree)
							{
								if (this.overlap)
								{
									goto IL_2BB8;
								}
								if (this.subfilter_value * this.current_layer.strength > (float)0)
								{
									if (this.current_layer.tree_output.tree.Count == 0)
									{
										goto IL_2D35;
									}
									TreeInstance item = default(TreeInstance);
									float num5 = (float)0;
									float num6 = 0f;
									float num7 = 0f;
									bool flag2 = true;
									int num8 = Mathf.FloorToInt(this.current_layer.tree_output.tree_value.curve.Evaluate(this.filter_value) * (float)this.current_layer.tree_output.tree.Count);
									if (num8 > this.current_layer.tree_output.tree.Count - 1)
									{
										num8 = this.current_layer.tree_output.tree.Count - 1;
									}
									if (num8 < 0)
									{
										num8 = 0;
									}
									tree_class tree_class = this.current_layer.tree_output.tree[num8];
									item.prototypeIndex = tree_class.prototypeindex;
									if (num4 == (float)0 || x == (float)0)
									{
										UnityEngine.Random.seed = (int)((num4 + (float)1) * (x + (float)1) + (float)10000000 + (float)j);
									}
									else
									{
										UnityEngine.Random.seed = (int)(num4 * x + (float)j);
									}
									this.random_range2 = UnityEngine.Random.value;
									if (this.random_range2 > this.subfilter_value * this.current_layer.strength)
									{
										goto IL_2D35;
									}
									this.filter_value = (float)0;
									for (int l = 0; l < tree_class.prefilter.filter_index.Count; l++)
									{
										this.calc_filter_value(this.filter[tree_class.prefilter.filter_index[l]], num4, x);
									}
									float num9 = tree_class.height_end - tree_class.height_start;
									float num10 = num9 * this.filter_value;
									float y2 = num10 + tree_class.height_start;
									float num11 = num10 / num9;
									float num12 = tree_class.width_end - tree_class.width_start;
									float num13 = num12 * num11 - tree_class.unlink * num10 + tree_class.width_start;
									if (num13 < tree_class.width_start)
									{
										num13 = tree_class.width_start;
									}
									float num14 = num12 * num11 + tree_class.unlink * num10 + tree_class.width_start;
									if (num14 > tree_class.width_end)
									{
										num14 = tree_class.width_end;
									}
									float num15 = UnityEngine.Random.Range(num13, num14);
									this.scale = new Vector3(num15, y2, num15);
									float num16 = (float)0;
									float num17 = (float)0;
									if (tree_class.random_position)
									{
										num16 = UnityEngine.Random.Range(-prelayer3.prearea.step.x / (float)2, this.prelayer.prearea.step.x / (float)2);
										num17 = UnityEngine.Random.Range(-prelayer3.prearea.step.y / (float)2, this.prelayer.prearea.step.y / (float)2);
									}
									float interpolatedHeight = this.preterrain.terrain.terrainData.GetInterpolatedHeight((this.local_x_rot + num16) / this.preterrain.size.x, (this.local_y_rot + num17) / this.preterrain.size.z);
									this.position = new Vector3(this.local_x_rot + num16, interpolatedHeight + tree_class.height, this.local_y_rot + num17);
									if (tree_class.distance_level != distance_level_enum.This || tree_class.min_distance.x != (float)0 || tree_class.min_distance.z != (float)0 || tree_class.min_distance.y != (float)0)
									{
										this.object_info.position = this.position + new Vector3(this.preterrain.rect.x, (float)0, this.preterrain.rect.y);
										this.object_info.min_distance = tree_class.min_distance;
										if (tree_class.distance_include_scale)
										{
											this.object_info.min_distance = new Vector3(this.object_info.min_distance.x * this.scale.x, this.object_info.min_distance.y * this.scale.y, this.object_info.min_distance.z * this.scale.z);
										}
										if (tree_class.distance_include_scale_group)
										{
											this.object_info.min_distance = this.object_info.min_distance * this.current_layer.tree_output.scale;
										}
										this.object_info.distance_rotation = tree_class.distance_rotation_mode;
										this.object_info.distance_mode = tree_class.distance_mode;
										distance_level_enum distance_level = tree_class.distance_level;
										if (distance_level == distance_level_enum.This)
										{
											flag2 = this.check_object_distance(tree_class.objects_placed);
										}
										else if (distance_level == distance_level_enum.Layer)
										{
											flag2 = this.check_object_distance(this.current_layer.objects_placed);
										}
										else if (distance_level == distance_level_enum.LayerLevel)
										{
											flag2 = this.check_object_distance(prelayer3.objects_placed);
										}
										else if (distance_level == distance_level_enum.Global)
										{
											flag2 = this.check_object_distance(this.objects_placed);
										}
									}
									if (tree_class.raycast)
									{
										if (tree_class.raycast_mode == raycast_mode_enum.Hit)
										{
											if (Physics.SphereCast(new Vector3(x + num16 + this.prelayer.prearea.step.x / (float)2, this.height * this.preterrain.size.y + tree_class.cast_height, this.prelayer.counter_y + num17), tree_class.ray_radius, tree_class.ray_direction, out this.hit, tree_class.ray_length, tree_class.layerMask))
											{
												this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
												if ((this.layerHit & tree_class.layerMask) != 0)
												{
													goto IL_2D35;
												}
											}
										}
										else if (Physics.Raycast(new Vector3(x + num16 + this.prelayer.prearea.step.x / (float)2, tree_class.cast_height, this.prelayer.counter_y + num17), tree_class.ray_direction, out this.hit, tree_class.ray_length, tree_class.layerMask))
										{
											this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
											if ((this.layerHit & tree_class.layerMask) != 0)
											{
												this.position.y = this.hit.point.y;
											}
										}
									}
									if (flag2)
									{
										item.position = new Vector3(this.position.x / this.preterrain.size.x, this.position.y / this.preterrain.size.y, this.position.z / this.preterrain.size.z);
										if (tree_class.precolor_range.color_range.Count != 0)
										{
											int num18 = Mathf.FloorToInt(tree_class.precolor_range.color_range_value.curve.Evaluate(this.subfilter_value) * (float)tree_class.precolor_range.color_range.Count);
											if (num18 > tree_class.precolor_range.color_range.Count - 1)
											{
												num18 = tree_class.precolor_range.color_range.Count - 1;
											}
											color_range_class color_range_class = tree_class.precolor_range.color_range[num18];
											this.tree_color = this.random_color_from_range(color_range_class.color_start, color_range_class.color_end);
										}
										item.color = this.tree_color;
										item.lightmapColor = this.tree_color;
										item.heightScale = this.scale.y * this.current_layer.tree_output.scale;
										item.widthScale = this.scale.x * this.current_layer.tree_output.scale;
										this.tree_instances.Add(item);
										tree_class.placed++;
										this.prelayer.layer[j].tree_output.placed = this.prelayer.layer[j].tree_output.placed + 1;
										if (this.current_layer.nonOverlap)
										{
											this.overlap = true;
										}
									}
								}
								goto IL_2BB8;
							}
							else
							{
								if (output == layer_output_enum.grass)
								{
									if (this.subfilter_value * this.current_layer.strength > (float)0)
									{
										int m = 0;
										int num19 = 0;
										for (m = 0; m < this.preterrain.grass.Length; m++)
										{
											num19 = (int)((float)num19 + this.preterrain.grass[m]);
											this.grass_detail[m].detail[this.detailmap_y, this.detailmap_x] = (int)((float)this.grass_detail[m].detail[this.detailmap_y, this.detailmap_x] + this.preterrain.grass[m] * this.settings.grass_density);
											if (this.grass_detail[m].detail[this.detailmap_y, this.detailmap_x] < 0)
											{
												this.grass_detail[m].detail[this.detailmap_y, this.detailmap_x] = 0;
											}
										}
										if (this.current_layer.nonOverlap && num19 > 0)
										{
											this.overlap = true;
										}
									}
									this.count_value = 0;
									while (this.count_value < this.preterrain.grass.Length)
									{
										this.preterrain.grass[this.count_value] = (float)0;
										this.count_value++;
									}
									goto IL_2BB8;
								}
								if (output == layer_output_enum.@object)
								{
									if (this.overlap)
									{
										goto IL_2BB8;
									}
									if (this.subfilter_value * this.current_layer.strength * this.filter_strength > (float)0)
									{
										if (this.current_layer.object_output.@object.Count == 0)
										{
											goto IL_2D35;
										}
										int num20 = (int)(this.current_layer.object_output.object_value.curve.Evaluate(this.filter_value) * (float)this.current_layer.object_output.@object.Count);
										if (num20 > this.current_layer.object_output.@object.Count - 1)
										{
											num20 = this.current_layer.object_output.@object.Count - 1;
										}
										if (num20 < 0)
										{
											num20 = 0;
										}
										object_class object_class = this.current_layer.object_output.@object[num20];
										if (object_class.place_maximum && object_class.placed_prelayer >= object_class.place_max)
										{
											goto IL_2D35;
										}
										if (num4 == (float)0 || x == (float)0)
										{
											UnityEngine.Random.seed = (int)((num4 + (float)1) * (x + (float)1) + (float)10000000 + (float)j);
										}
										else
										{
											UnityEngine.Random.seed = (int)(num4 * x + (float)j);
										}
										this.random_range2 = UnityEngine.Random.Range((float)0, 1f);
										if (this.random_range2 > this.subfilter_value * this.current_layer.strength * this.filter_strength)
										{
											goto IL_2D35;
										}
										bool flag2 = true;
										Quaternion quaternion = Quaternion.identity;
										int num21 = 0;
										this.position = new Vector3(x, (float)0, num4);
										Vector3 position_start = object_class.position_start;
										Vector3 position_end = object_class.position_end;
										Vector3 vector2 = default(Vector3);
										vector2.x = UnityEngine.Random.Range(position_start.x, position_end.x);
										vector2.y = UnityEngine.Random.Range(position_start.y, position_end.y);
										vector2.z = UnityEngine.Random.Range(position_start.z, position_end.z);
										if (object_class.random_position)
										{
											vector2.x += UnityEngine.Random.Range(-prelayer3.prearea.step.x, prelayer3.prearea.step.x);
											vector2.z += UnityEngine.Random.Range(-prelayer3.prearea.step.y, prelayer3.prearea.step.y);
										}
										this.position += vector2;
										if (object_class.terrain_rotate)
										{
											Vector3 interpolatedNormal = this.preterrain.terrain.terrainData.GetInterpolatedNormal((this.local_x_rot + vector2.x) / this.preterrain.size.x, (this.local_y_rot + vector2.y) / this.preterrain.size.z);
											interpolatedNormal.x = interpolatedNormal.x / (float)3 * (float)2;
											interpolatedNormal.z = interpolatedNormal.z / (float)3 * (float)2;
											quaternion = Quaternion.FromToRotation(Vector3.up, interpolatedNormal);
										}
										if (!object_class.rotation_map.active)
										{
											quaternion *= Quaternion.AngleAxis(UnityEngine.Random.Range(object_class.rotation_start.x, object_class.rotation_end.x), Vector3.right);
											quaternion *= Quaternion.AngleAxis(UnityEngine.Random.Range(object_class.rotation_start.y, object_class.rotation_end.y), Vector3.up);
											quaternion *= Quaternion.AngleAxis(UnityEngine.Random.Range(object_class.rotation_start.z, object_class.rotation_end.z), Vector3.forward);
											quaternion *= Quaternion.Euler(object_class.parent_rotation);
										}
										else
										{
											quaternion *= Quaternion.AngleAxis(object_class.rotation_map.calc_rotation(this.get_image_pixel(object_class.rotation_map.preimage, this.local_x, this.local_y)), Vector3.up);
										}
										if (object_class.look_at_parent)
										{
											quaternion.y = Quaternion.LookRotation(new Vector3(prelayer3.prearea.area.center.x, (float)0, prelayer3.prearea.area.center.y) - this.position).eulerAngles.y;
										}
										if (object_class.rotation_steps)
										{
											if (object_class.rotation_step.x != (float)0)
											{
												quaternion.x = Mathf.Round(quaternion.x / object_class.rotation_step.x) * object_class.rotation_step.x;
											}
											if (object_class.rotation_step.y != (float)0)
											{
												quaternion.y = Mathf.Round(quaternion.y / object_class.rotation_step.y) * object_class.rotation_step.y;
											}
											if (object_class.rotation_step.z != (float)0)
											{
												quaternion.z = Mathf.Round(quaternion.z / object_class.rotation_step.z) * object_class.rotation_step.z;
											}
										}
										if (this.current_layer.object_output.group_rotation)
										{
											int num22 = this.check_object_rotate(this.current_layer.object_output.objects_placed, this.current_layer.object_output.objects_placed_rot, this.position, (int)this.current_layer.object_output.min_distance_x_rot, (int)this.current_layer.object_output.min_distance_z_rot);
											if (num22 != -1)
											{
												quaternion.eulerAngles = this.current_layer.object_output.objects_placed_rot[num22];
												if (this.current_layer.object_output.group_rotation_steps)
												{
													if (this.current_layer.object_output.group_rotation_step.x != (float)0)
													{
														quaternion.x += Mathf.Round(UnityEngine.Random.Range((float)0, (float)360 / this.current_layer.object_output.group_rotation_step.x)) * this.current_layer.object_output.group_rotation_step.x;
													}
													if (this.current_layer.object_output.group_rotation_step.y != (float)0)
													{
														quaternion.y += Mathf.Round(UnityEngine.Random.Range((float)0, (float)360 / this.current_layer.object_output.group_rotation_step.y)) * this.current_layer.object_output.group_rotation_step.y;
													}
													if (this.current_layer.object_output.group_rotation_step.z != (float)0)
													{
														quaternion.z += Mathf.Round(UnityEngine.Random.Range((float)0, (float)360 / this.current_layer.object_output.group_rotation_step.z)) * this.current_layer.object_output.group_rotation_step.z;
													}
												}
											}
										}
										if (object_class.terrain_height)
										{
											if (this.settings.showTerrains)
											{
												this.height_interpolated = this.preterrain.terrain.terrainData.GetInterpolatedHeight((this.local_x_rot + vector2.x) / this.preterrain.size.x, (this.local_y_rot + vector2.z) / this.preterrain.size.z);
												this.position.y = this.height_interpolated + this.preterrain.terrain.transform.position.y + vector2.y;
											}
											else
											{
												this.GetMeshHeightSlope(this.premesh, new Vector2(x + vector2.x, num4 + vector2.z));
												if (!this.mesh_measure.hit)
												{
													goto IL_2D35;
												}
												this.height_interpolated = this.mesh_measure.height;
												this.degree = this.mesh_measure.degree;
												this.position.y = this.height_interpolated + vector2.y;
											}
										}
										if (object_class.sphereOverlapRadius > (float)0 && Physics.OverlapSphere(new Vector3(this.position.x, this.position.y + object_class.sphereOverlapHeight, this.position.z), object_class.sphereOverlapRadius).Length != 0)
										{
											goto IL_2D35;
										}
										this.position.y = this.position.y - this.degree * object_class.slopeY;
										float num23 = object_class.scale_end.x - object_class.scale_start.x;
										this.scale.x = UnityEngine.Random.Range(object_class.scale_start.x, object_class.scale_end.x);
										float num24 = this.scale.x - object_class.scale_start.x;
										float num25 = num24 / num23;
										float num26 = object_class.scale_end.y - object_class.scale_start.y;
										float num27 = num26 * num25 - object_class.unlink_y * num24 + object_class.scale_start.y;
										if (num27 < object_class.scale_start.y)
										{
											num27 = object_class.scale_start.y;
										}
										float num28 = num26 * num25 + object_class.unlink_y * num24 + object_class.scale_start.y;
										if (num28 > object_class.scale_end.y)
										{
											num28 = object_class.scale_end.y;
										}
										this.scale.y = UnityEngine.Random.Range(num27, num28);
										float num29 = object_class.scale_end.z - object_class.scale_start.z;
										float num30 = num29 * num25 - object_class.unlink_z * num24 + object_class.scale_start.z;
										if (num30 < object_class.scale_start.z)
										{
											num30 = object_class.scale_start.z;
										}
										float num31 = num29 * num25 + object_class.unlink_z * num24 + object_class.scale_start.z;
										if (num31 > object_class.scale_end.z)
										{
											num31 = object_class.scale_end.z;
										}
										this.scale.z = UnityEngine.Random.Range(num30, num31);
										if (object_class.raycast)
										{
											if (object_class.raycast_mode == raycast_mode_enum.Hit)
											{
												if (Physics.SphereCast(new Vector3(x + this.prelayer.prearea.step.x / (float)2, this.height * this.preterrain.size.y + object_class.cast_height, this.prelayer.counter_y), object_class.ray_radius, object_class.ray_direction, out this.hit, object_class.ray_length, object_class.layerMask))
												{
													this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
													if ((this.layerHit & object_class.layerMask) != 0)
													{
														goto IL_2D35;
													}
												}
											}
											else if (Physics.Raycast(new Vector3(x + this.prelayer.prearea.step.x / (float)2, object_class.cast_height, this.prelayer.counter_y), object_class.ray_direction, out this.hit, object_class.ray_length, object_class.layerMask))
											{
												this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
												if ((this.layerHit & object_class.layerMask) != 0)
												{
													this.position.y = this.hit.point.y;
												}
											}
										}
										if (object_class.pivot_center)
										{
											this.position.y = this.position.y + this.scale.y / (float)2;
										}
										bool flag3 = false;
										if (object_class.distance_level != distance_level_enum.This || object_class.min_distance.x != (float)0 || object_class.min_distance.z != (float)0 || object_class.min_distance.y != (float)0)
										{
											flag3 = true;
											this.object_info.position = this.position;
											this.object_info.rotation = quaternion.eulerAngles;
											this.object_info.min_distance = object_class.min_distance;
											if (object_class.distance_include_scale)
											{
												this.object_info.min_distance = new Vector3(this.object_info.min_distance.x * this.scale.x, this.object_info.min_distance.y * this.scale.y, this.object_info.min_distance.z * this.scale.z);
											}
											if (object_class.distance_include_scale_group)
											{
												this.object_info.min_distance = this.object_info.min_distance * this.current_layer.object_output.scale;
											}
											this.object_info.distance_rotation = object_class.distance_rotation_mode;
											this.object_info.distance_mode = object_class.distance_mode;
											distance_level_enum distance_level2 = object_class.distance_level;
											if (distance_level2 == distance_level_enum.This)
											{
												flag2 = this.check_object_distance(object_class.objects_placed);
											}
											else if (distance_level2 == distance_level_enum.Layer)
											{
												flag2 = this.check_object_distance(this.current_layer.objects_placed);
											}
											else if (distance_level2 == distance_level_enum.LayerLevel)
											{
												flag2 = this.check_object_distance(prelayer3.objects_placed);
											}
											else if (distance_level2 == distance_level_enum.Global)
											{
												flag2 = this.check_object_distance(this.objects_placed);
											}
										}
										this.scale *= this.current_layer.object_output.scale;
										if (flag2)
										{
											float num32 = this.preterrain.size.z;
											num32 /= this.preterrain.heightmap_resolution;
											if (this.current_layer.nonOverlap)
											{
												this.overlap = true;
											}
											GameObject gameObject;
											if (!object_class.objectStream)
											{
												gameObject = null;
												gameObject = (GameObject)UnityEngine.Object.Instantiate(object_class.object1, this.position, Quaternion.identity);
												this.pointsRange.Add(new object_point_class(new Vector2(this.position.x, this.position.z), gameObject));
												gameObject.transform.rotation = quaternion;
												gameObject.transform.localScale = this.scale;
												gameObject.SetActive(false);
												this.placedObjects.Add(gameObject);
												int num33 = 0;
												if (object_class.object_material.active)
												{
													num33 = object_class.object_material.set_material(gameObject, 0);
												}
												if (object_class.combine)
												{
													if (object_class.object_material.combine_count[num33] >= object_class.mesh_combine || (!object_class.combine_total && object_class.placed_prelayer == 0))
													{
														object_class.object_material.combine_count[num33] = 0;
													}
													if (object_class.object_material.combine_count[num33] == 0)
													{
														string rhs = null;
														if (object_class.object_material.material.Count > 1)
														{
															rhs = "_mat" + num33;
														}
														object_class.object_material.combine_parent[num33] = UnityEngine.Object.Instantiate<GameObject>(this.Combine_Children);
														if (this.settings.parentObjectsTerrain)
														{
															object_class.object_material.combine_parent[num33].transform.parent = this.preterrain.objectParent.transform;
														}
														else
														{
															object_class.object_material.combine_parent[num33].transform.parent = object_class.parent;
														}
														if (object_class.combine_parent_name == string.Empty)
														{
															object_class.object_material.combine_parent[num33].name = object_class.object1.name + rhs;
														}
														else
														{
															object_class.object_material.combine_parent[num33].name = object_class.combine_parent_name + rhs;
														}
													}
													gameObject.transform.parent = object_class.object_material.combine_parent[num33].transform;
												}
												else if (this.settings.parentObjectsTerrain)
												{
													gameObject.transform.parent = this.preterrain.objectParent.transform;
												}
												else
												{
													gameObject.transform.parent = object_class.parent;
												}
												object_class.object_material.combine_count[num33] = object_class.object_material.combine_count[num33] + 1;
												gameObject.name = object_class.object1.name + "_" + object_class.placed;
											}
											if (flag3)
											{
												this.current_layer.object_output.objects_placed.Add(this.position);
												if (this.current_layer.object_output.group_rotation)
												{
													this.current_layer.object_output.objects_placed_rot.Add(quaternion.eulerAngles);
												}
											}
											object_class.placed++;
											object_class.placed_prelayer++;
											this.current_layer.object_output.placed = this.current_layer.object_output.placed + 1;
											this.row_object_count++;
											object_class.object2 = gameObject;
											if (object_class.prelayer_created && this.prelayers[object_class.prelayer_index].prearea.active)
											{
												this.set_object_child(object_class, quaternion.eulerAngles);
												prelayer3.x += prelayer3.prearea.step.x;
												prelayer3.y = prelayer3.counter_y;
												if (prelayer3.x <= this.prelayer.prearea.area.xMax)
												{
													prelayer3.break_x_value = prelayer3.x - prelayer3.prearea.area.x;
												}
												else
												{
													prelayer3.y -= prelayer3.prearea.step.y;
													prelayer3.break_x_value = (float)0;
												}
												this.prelayer_stack.Add(object_class.prelayer_index);
												this.prelayer = this.prelayers[object_class.prelayer_index];
												this.prelayer.prearea.area.x = this.position.x + this.prelayer.prearea.area_old.x * this.scale.x;
												this.prelayer.prearea.area.y = this.position.z + this.prelayer.prearea.area_old.y * this.scale.z;
												this.prelayer.prearea.area.width = this.prelayer.prearea.area_old.width * this.scale.x;
												this.prelayer.prearea.area.height = this.prelayer.prearea.area_old.height * this.scale.z;
												if (quaternion.y != (float)0)
												{
													this.prelayer.prearea.rotation = quaternion.eulerAngles;
													this.prelayer.prearea.rotation_active = true;
												}
												this.prelayer.prearea.step.y = Mathf.Sqrt(Mathf.Pow(this.prelayer.prearea.step_old.x, (float)2) + Mathf.Pow(this.prelayer.prearea.step_old.y, (float)2)) / (float)2;
												this.prelayer.prearea.step.x = this.prelayer.prearea.step.y;
												this.prelayer.prearea.center = new Vector2(this.position.x, this.position.z);
												this.prelayer.y = this.prelayer.prearea.area.yMax;
												this.generate_error = false;
												arg_3351_0 = 3;
												return arg_3351_0;
											}
										}
									}
									goto IL_2BB8;
								}
								else
								{
									if (output == layer_output_enum.heightmap)
									{
										if (!this.button_export)
										{
											this.heights[this.heightmap_y, this.heightmap_x] = this.heights[this.heightmap_y, this.heightmap_x] + this.filter_value * this.current_layer.strength;
										}
										goto IL_2BB8;
									}
									goto IL_2BB8;
								}
							}
							IL_2D35:
							j++;
							continue;
							IL_2BB8:
							if (this.current_layer.output == layer_output_enum.splat)
							{
								this.count_value = 0;
								while (this.count_value < this.preterrain.splat_length)
								{
									this.preterrain.splat_layer[this.count_value] = this.preterrain.splat_layer[this.count_value] + this.preterrain.splat[this.count_value];
									this.count_value++;
								}
								this.count_value = 0;
								while (this.count_value < Extensions.get_length(this.preterrain.splat))
								{
									this.preterrain.splat[this.count_value] = (float)0;
									this.count_value++;
								}
							}
							if (this.current_layer.output == layer_output_enum.color)
							{
								this.count_value = 0;
								while (this.count_value < this.preterrain.color_length)
								{
									this.preterrain.color_layer[this.count_value] = this.preterrain.color_layer[this.count_value] + this.preterrain.color[this.count_value];
									this.count_value++;
								}
								this.count_value = 0;
								while (this.count_value < this.preterrain.color_length)
								{
									this.preterrain.color[this.count_value] = (float)0;
									this.count_value++;
								}
								goto IL_2D35;
							}
							goto IL_2D35;
						}
						if (this.button_export)
						{
							Color color = default(Color);
							if (this.color_output)
							{
								this.count_value = 0;
								while (this.count_value < 3)
								{
									color[this.count_value] = this.preterrain.color_layer[this.count_value];
									this.preterrain.color_layer[this.count_value] = (float)0;
									this.count_value++;
								}
							}
							if (this.splat_output)
							{
								this.count_value = 0;
								while (this.count_value < 3)
								{
									color[this.count_value] = this.preterrain.splat_layer[this.count_value];
									this.preterrain.splat_layer[this.count_value] = (float)0;
									this.count_value++;
								}
							}
							if (this.heightmap_output)
							{
								this.convert_16to8_bit(this.heights[this.heightmap_y, this.heightmap_x]);
								color[0] = (float)0;
								color[1] = (float)this.byte_hi * 1f / (float)255;
								color[2] = (float)this.byte_lo * 1f / (float)255;
								color[3] = (float)0;
								this.export_bytes[this.heightmap_x * 2 + this.heightmap_y * 2049 * 2] = (byte)this.byte_hi;
								this.export_bytes[this.heightmap_x * 2 + 1 + this.heightmap_y * 2049 * 2] = (byte)this.byte_lo;
							}
							this.export_texture.SetPixel(this.map_x, this.map_y, color);
						}
						else
						{
							Color color;
							int n;
							if (this.splat_output)
							{
								float num34 = 0f;
								int num35 = this.preterrain.splat_layer.Length / 4;
								num34 = (float)0;
								this.count_value = 0;
								while (this.count_value < this.preterrain.splat_layer.Length)
								{
									num34 += this.preterrain.splat_layer[this.count_value];
									this.count_value++;
								}
								if (num34 < (float)1)
								{
									num34 = (float)1;
								}
								for (n = 0; n < num35; n++)
								{
									this.count_value = 0;
									while (this.count_value < 4)
									{
										color[this.count_value] = this.preterrain.splat_layer[n * 4 + this.count_value] / num34;
										this.preterrain.splat_layer[n * 4 + this.count_value] = (float)0;
										this.count_value++;
									}
									this.preterrain.splat_alpha[n].SetPixel(this.map_x, this.map_y, color);
								}
							}
							if (this.color_output)
							{
								if (!this.settings.direct_colormap)
								{
									color[3] = (float)0;
									this.count_value = 0;
									while (this.count_value < 3)
									{
										color[this.count_value] = this.preterrain.color_layer[this.count_value];
										this.preterrain.color_layer[this.count_value] = (float)0;
										this.count_value++;
									}
									this.preterrain.splat_alpha[n].SetPixel(this.map_x, this.map_y, color);
								}
								else
								{
									color[3] = (float)1;
									this.count_value = 0;
									while (this.count_value < 3)
									{
										color[this.count_value] = this.preterrain.color_layer[this.count_value];
										this.preterrain.color_layer[this.count_value] = (float)0;
										this.count_value++;
									}
									this.preterrain.ColorGlobal.SetPixel(this.map_x, this.map_y, color);
								}
							}
						}
						if (this.auto_speed)
						{
							if (Time.realtimeSinceStartup - this.auto_speed_time > 1f / (float)this.target_frame)
							{
								prelayer3.break_x_value = prelayer3.x - prelayer3.prearea.area.x + prelayer3.prearea.step.x;
								this.row_object_count = 0;
								this.break_x = true;
								prelayer3.y = prelayer3.counter_y;
								this.generate_time = Time.realtimeSinceStartup - this.generate_time_start;
								this.generate_error = false;
								arg_3351_0 = 4;
								return arg_3351_0;
							}
							goto IL_327A;
						}
						else
						{
							if (this.row_object_count > this.object_speed)
							{
								prelayer3.break_x_value = prelayer3.x - prelayer3.prearea.area.x + prelayer3.prearea.step.x;
								this.row_object_count = 0;
								this.break_x = true;
								prelayer3.y = prelayer3.counter_y;
								this.generate_time = Time.realtimeSinceStartup - this.generate_time_start;
								this.object_frames = (float)1 / (Time.realtimeSinceStartup - this.auto_speed_object_time);
								this.auto_speed_object_time = Time.realtimeSinceStartup;
								this.generate_error = false;
								arg_3351_0 = 4;
								return arg_3351_0;
							}
							goto IL_327A;
						}
					}
					prelayer3.break_x_value = (float)0;
					prelayer3.counter_y -= this.prelayer.prearea.step.y;
				}
			}
			prelayer3.y -= (float)(this.generate_speed + 1) * this.prelayer.prearea.step.y;
			this.generate_time = Time.realtimeSinceStartup - this.generate_time_start;
			this.generate_error = false;
			arg_3351_0 = 1;
		}
		return arg_3351_0;
	}

	public override void set_object_child(object_class @object, Vector3 rotation)
	{
		for (int i = 0; i < @object.object_child.Count; i++)
		{
			@object.object_child[i].parent_rotation = rotation;
			if (!@object.object_child[i].place_maximum_total)
			{
				@object.object_child[i].placed_prelayer = 0;
			}
			if (!@object.object_child[i].parent || @object.object_child[i].parent_set)
			{
				@object.object_child[i].parent = @object.object2.transform;
				@object.object_child[i].parent_set = true;
			}
		}
	}

	public override void create_object_child_list(object_class @object)
	{
		if (@object.prelayer_created)
		{
			for (int i = 0; i < this.prelayers[@object.prelayer_index].layer.Count; i++)
			{
				if (this.prelayers[@object.prelayer_index].layer[i].output == layer_output_enum.@object)
				{
					for (int j = 0; j < this.prelayers[@object.prelayer_index].layer[i].object_output.@object.Count; j++)
					{
						@object.object_child.Add(this.prelayers[@object.prelayer_index].layer[i].object_output.@object[j]);
					}
				}
			}
		}
	}

	public override void calc_filter_value(filter_class filter, float counter_y, float x)
	{
		float num = 0f;
		float num2 = 0f;
		Color color = default(Color);
		Color color2 = default(Color);
		this.filter_input = (float)0;
		this.filter_strength = filter.strength;
		bool flag = false;
		if (filter.device == filter_devices_enum.Standard)
		{
			condition_type_enum type = filter.type;
			if (type == condition_type_enum.Height)
			{
				this.filter_input = this.height;
			}
			else if (type == condition_type_enum.Current)
			{
				if (filter.change_mode == change_mode_enum.filter)
				{
					this.filter_input = this.filter_value;
				}
				else if (this.current_layer.output == layer_output_enum.heightmap)
				{
					this.filter_input = this.filter_value + this.heights[this.heightmap_y, this.heightmap_x];
				}
			}
			else if (type == condition_type_enum.Always)
			{
				this.filter_input = filter.curve_position;
			}
			else if (type == condition_type_enum.Steepness)
			{
				this.filter_input = this.degree / (float)90;
			}
			else if (type == condition_type_enum.Direction)
			{
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				if (this.normal.x >= (float)0)
				{
					num3 = filter.precurve_x_right.curve.Evaluate(this.normal.x);
				}
				else
				{
					num3 = filter.precurve_x_left.curve.Evaluate(this.normal.x);
				}
				if (this.normal.z >= (float)0)
				{
					num5 = filter.precurve_z_right.curve.Evaluate(this.normal.z);
				}
				else
				{
					num5 = filter.precurve_z_left.curve.Evaluate(this.normal.z);
				}
				num4 = filter.precurve_y.curve.Evaluate(this.normal.y);
				float num6 = num3 + num4 + num5;
				this.filter_input = num6;
			}
			else if (type == condition_type_enum.Image)
			{
				color2 = this.calc_image_value(filter.preimage, this.layer_x, this.layer_y);
				if (filter.preimage.splatmap)
				{
					flag = true;
				}
				if (filter.preimage.output)
				{
					this.filter_input = filter.preimage.output_pos;
				}
			}
			else if (type == condition_type_enum.RawHeightmap)
			{
				this.calc_raw_value(filter.raw, this.layer_x, this.layer_y);
				this.filter_input = filter.raw.output_pos;
			}
			else if (type == condition_type_enum.Random)
			{
				this.filter_input = UnityEngine.Random.Range((float)0, 1f);
			}
			else if (type == condition_type_enum.RandomRange)
			{
				if (this.random_range > filter.range_start && this.random_range < filter.range_end)
				{
					this.filter_input = UnityEngine.Random.Range((float)0, 1f);
				}
			}
			else if (type == condition_type_enum.Splatmap)
			{
				if (filter.splatmap < this.preterrain.splat_alpha.Length - 1)
				{
					this.color = this.preterrain.splat_alpha[filter.splatmap].GetPixel(this.map_x, this.map_y);
					this.filter_input = this.color[filter.splat_index - filter.splatmap * 4];
				}
				else
				{
					this.filter_input = (float)1;
				}
			}
			else if (type == condition_type_enum.RayCast)
			{
				if (filter.raycast_mode == raycast_mode_enum.Hit)
				{
					if (Physics.SphereCast(new Vector3(x + this.prelayer.prearea.step.x / (float)2, this.height * this.preterrain.size.y + filter.cast_height, this.prelayer.counter_y), this.prelayer.prearea.step.x / (float)2 * filter.ray_radius, filter.ray_direction, out this.hit, filter.ray_length, filter.layerMask))
					{
						this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
						if ((this.layerHit & filter.layerMask) != 0)
						{
							this.filter_input = (float)0;
						}
						else
						{
							this.filter_input = (float)1;
						}
					}
					else
					{
						this.filter_input = (float)1;
					}
				}
				else if (filter.raycast_mode == raycast_mode_enum.Height)
				{
					if (Physics.Raycast(new Vector3(x + this.prelayer.prearea.step.x / (float)2, filter.cast_height, this.prelayer.counter_y), filter.ray_direction, out this.hit, filter.ray_length, filter.layerMask))
					{
						this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
						if ((this.layerHit & filter.layerMask) != 0)
						{
							this.filter_input = this.hit.point.y / this.preterrain.terrain.terrainData.size.y;
						}
						else
						{
							this.filter_strength = (float)0;
						}
					}
					else
					{
						this.filter_strength = (float)0;
					}
				}
			}
		}
		else if (filter.device == filter_devices_enum.Math)
		{
			device2_type_enum type2 = filter.type2;
			if (type2 == device2_type_enum.Sin)
			{
				this.filter_input = Mathf.Sin(x * 1f / (float)20);
			}
			else if (type2 == device2_type_enum.Sin)
			{
				this.filter_input = Mathf.Cos(x * 1f / (float)20);
			}
		}
		for (int i = 0; i < filter.precurve_list.Count; i++)
		{
			curve_type_enum type3 = filter.precurve_list[i].type;
			if (type3 == curve_type_enum.Normal)
			{
				this.filter_input = filter.precurve_list[i].curve.Evaluate(this.filter_input);
			}
			else if (type3 == curve_type_enum.Random)
			{
				num = filter.precurve_list[i].curve.Evaluate(this.filter_input);
				if (!filter.precurve_list[i].abs)
				{
					this.filter_input += UnityEngine.Random.Range(-num, num);
				}
				else
				{
					this.filter_input += UnityEngine.Random.Range((float)0, num);
				}
			}
			else if (type3 == curve_type_enum.Perlin)
			{
				num2 = filter.precurve_list[i].curve.Evaluate(this.filter_input);
				num = this.perlin_noise(this.prelayer.x, this.prelayer.counter_y, filter.precurve_list[i].offset.x, filter.precurve_list[i].offset.y, filter.precurve_list[i].frequency, (float)filter.precurve_list[i].detail, filter.precurve_list[i].detail_strength) * num2 * filter.precurve_list[i].strength;
				if (!filter.precurve_list[i].abs)
				{
					this.filter_input += num * (float)2 - num2;
				}
				else
				{
					this.filter_input += num;
				}
			}
		}
		if (filter.presubfilter.subfilter_index.Count > 0)
		{
			if (filter.sub_strength_set)
			{
				this.subfilter_value = (float)0;
			}
			else
			{
				this.subfilter_value = (float)1;
			}
			for (int j = 0; j < filter.presubfilter.subfilter_index.Count; j++)
			{
				this.current_subfilter = this.subfilter[filter.presubfilter.subfilter_index[j]];
				this.calc_subfilter_value(filter, this.current_subfilter, counter_y, x);
			}
			if (filter.last_value_declared)
			{
				int num7 = (int)((x - this.prelayer.prearea.area.xMin) / this.prelayer.prearea.step.x);
				filter.last_value_y[num7] = this.filter_input;
				filter.last_pos_x = (float)num7;
				filter.last_value_x[0] = this.filter_input;
			}
		}
		else
		{
			this.subfilter_value = (float)1;
		}
		float num8 = (float)0;
		if (!filter.combine)
		{
			this.filter_input += this.filter_combine;
			this.filter_combine = (float)0;
			this.filter_combine_start = (float)0;
		}
		else if (this.filter_combine_start != (float)0)
		{
			this.filter_combine_start = this.filter_value;
		}
		if ((filter.type != condition_type_enum.Image || !filter.preimage.edge_blur) && this.current_layer.output == layer_output_enum.splat)
		{
			this.count_value = 0;
			while (this.count_value < this.current_layer.splat_output.splat.Count)
			{
				this.current_layer.splat_output.splat_calc[this.count_value] = this.current_layer.splat_output.curves[this.count_value].curve.Evaluate(this.current_layer.splat_output.splat_value.curve.Evaluate(this.filter_input));
				this.count_value++;
			}
		}
		if (this.current_layer.output == layer_output_enum.color)
		{
			if (filter.type == condition_type_enum.Image && filter.preimage.rgb)
			{
				color = color2;
			}
			else
			{
				color[0] = this.current_layer.color_output.precolor_range[filter.color_output_index].curve_red.Evaluate(this.current_layer.color_output.precolor_range[filter.color_output_index].color_range_value.curve.Evaluate(this.filter_input));
				color[1] = this.current_layer.color_output.precolor_range[filter.color_output_index].curve_green.Evaluate(this.current_layer.color_output.precolor_range[filter.color_output_index].color_range_value.curve.Evaluate(this.filter_input));
				color[2] = this.current_layer.color_output.precolor_range[filter.color_output_index].curve_blue.Evaluate(this.current_layer.color_output.precolor_range[filter.color_output_index].color_range_value.curve.Evaluate(this.filter_input));
				color[3] = (float)0;
			}
			if (this.export_color_advanced)
			{
				color *= this.export_color;
				if (this.export_color_curve_advanced)
				{
					color[0] = this.export_color_curve_red.Evaluate(color[0]);
					color[1] = this.export_color_curve_green.Evaluate(color[1]);
					color[2] = this.export_color_curve_blue.Evaluate(color[2]);
				}
				else
				{
					color[0] = this.export_color_curve.Evaluate(color[0]);
					color[1] = this.export_color_curve.Evaluate(color[1]);
					color[2] = this.export_color_curve.Evaluate(color[2]);
				}
			}
		}
		else if (this.current_layer.output == layer_output_enum.grass)
		{
			this.count_value = 0;
			while (this.count_value < this.current_layer.grass_output.grass_calc.Count)
			{
				this.current_layer.grass_output.grass_calc[this.count_value] = this.current_layer.grass_output.curves[this.count_value].curve.Evaluate(this.current_layer.grass_output.grass_value.curve.Evaluate(this.filter_input));
				this.count_value++;
			}
		}
		condition_output_enum output = filter.output;
		if (output == condition_output_enum.add)
		{
			if (this.current_layer.output == layer_output_enum.heightmap)
			{
				this.filter_value += this.filter_input * this.filter_strength * this.subfilter_value;
			}
			else
			{
				this.filter_value += this.filter_input * this.filter_strength * this.subfilter_value;
			}
			if (filter.combine)
			{
				this.filter_combine = this.filter_value - this.filter_combine_start;
			}
			else
			{
				if (this.current_layer.output == layer_output_enum.color)
				{
					this.count_value = 0;
					while (this.count_value < 3)
					{
						this.preterrain.color[this.count_value] = this.preterrain.color[this.count_value] + color[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
						this.count_value++;
					}
				}
				if (this.current_layer.output == layer_output_enum.splat)
				{
					if (!flag)
					{
						this.count_value = 0;
						while (this.count_value < this.current_layer.splat_output.splat.Count)
						{
							int num9 = this.current_layer.splat_output.splat[this.count_value];
							float num10 = this.preterrain.splat[num9] = this.preterrain.splat[num9] + this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
							this.count_value++;
						}
					}
					else
					{
						this.count_value = 0;
						while (this.count_value < 4)
						{
							if (this.count_value > this.preterrain.splat.Length - 1)
							{
								break;
							}
							int num11 = this.current_layer.splat_output.splat[this.count_value];
							float num12 = this.preterrain.splat[num11] = this.preterrain.splat[num11] + color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
							this.count_value++;
						}
					}
				}
				else if (this.current_layer.output == layer_output_enum.grass)
				{
					this.count_value = 0;
					while (this.count_value < this.current_layer.grass_output.grass.Count)
					{
						this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] = this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] + this.current_layer.grass_output.grass_calc[this.count_value] * (this.current_layer.strength * this.subfilter_value);
						this.count_value++;
					}
				}
			}
		}
		else if (output == condition_output_enum.subtract)
		{
			if (this.current_layer.output == layer_output_enum.heightmap)
			{
				this.filter_value -= this.filter_input * this.filter_strength * this.subfilter_value;
			}
			else
			{
				this.filter_value -= this.filter_input;
			}
			if (filter.combine)
			{
				this.filter_combine = this.filter_value - this.filter_combine_start;
			}
			else
			{
				if (this.current_layer.output == layer_output_enum.color)
				{
					this.count_value = 0;
					while (this.count_value < 3)
					{
						this.preterrain.color[this.count_value] = this.preterrain.color[this.count_value] - color[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
						this.count_value++;
					}
				}
				if (this.current_layer.output == layer_output_enum.splat)
				{
					if (!flag)
					{
						this.count_value = 0;
						while (this.count_value < this.current_layer.splat_output.splat.Count)
						{
							int num13 = this.current_layer.splat_output.splat[this.count_value];
							float num14 = this.preterrain.splat[num13] = this.preterrain.splat[num13] - this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
							this.count_value++;
						}
					}
					else
					{
						this.count_value = 0;
						while (this.count_value < 4)
						{
							if (this.count_value > this.preterrain.splat.Length - 1)
							{
								break;
							}
							int num15 = this.current_layer.splat_output.splat[this.count_value];
							float num16 = this.preterrain.splat[num15] = this.preterrain.splat[num15] - color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
							this.count_value++;
						}
					}
				}
				else if (this.current_layer.output == layer_output_enum.grass)
				{
					this.count_value = 0;
					while (this.count_value < this.current_layer.grass_output.grass.Count)
					{
						this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] = this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] - this.current_layer.grass_output.grass_calc[this.count_value] * (this.current_layer.strength * this.subfilter_value);
						this.count_value++;
					}
				}
			}
		}
		else if (output == condition_output_enum.change)
		{
			if (this.current_layer.output == layer_output_enum.heightmap)
			{
				if (filter.change_mode == change_mode_enum.filter)
				{
					this.filter_value = this.filter_value * ((float)1 - this.subfilter_value * this.filter_strength) + this.filter_input * (this.subfilter_value * this.filter_strength);
				}
				else
				{
					if (filter.combine)
					{
						this.filter_combine = this.filter_value - this.filter_combine_start;
						return;
					}
					this.heights[this.heightmap_y, this.heightmap_x] = (this.heights[this.heightmap_y, this.heightmap_x] + this.filter_value) * ((float)1 - this.subfilter_value * this.filter_strength) + this.filter_input * (this.subfilter_value * this.filter_strength);
				}
			}
			else
			{
				this.filter_value = this.filter_value * ((float)1 - this.subfilter_value) + this.filter_input * this.subfilter_value;
			}
			if (filter.combine)
			{
				this.filter_combine = this.filter_value - this.filter_combine_start;
			}
			else
			{
				if (this.current_layer.output == layer_output_enum.color)
				{
					if (filter.type == condition_type_enum.Current)
					{
						if (filter.change_mode == change_mode_enum.filter)
						{
							this.count_value = 0;
							while (this.count_value < 3)
							{
								this.preterrain.color[this.count_value] = this.preterrain.color[this.count_value] - this.preterrain.color[this.count_value] * ((float)1 - this.subfilter_value) * this.current_layer.strength * this.filter_strength;
								this.count_value++;
							}
						}
						else
						{
							this.count_value = 0;
							while (this.count_value < 3)
							{
								this.preterrain.color_layer[this.count_value] = this.preterrain.color_layer[this.count_value] - this.preterrain.color_layer[this.count_value] * ((float)1 - this.subfilter_value) * this.current_layer.strength * this.filter_strength;
								this.count_value++;
							}
						}
					}
					else
					{
						this.count_value = 0;
						while (this.count_value < 3)
						{
							this.preterrain.color_layer[this.count_value] = this.preterrain.color_layer[this.count_value] * ((float)1 - this.subfilter_value * this.filter_strength * this.current_layer.strength) + color[this.count_value] * this.subfilter_value * this.filter_strength * this.current_layer.strength;
							this.count_value++;
						}
					}
				}
				if (this.current_layer.output == layer_output_enum.splat)
				{
					if (filter.type == condition_type_enum.Current)
					{
						if (filter.change_mode == change_mode_enum.filter)
						{
							this.count_value = 0;
							while (this.count_value < this.preterrain.splat.Length)
							{
								this.preterrain.splat[this.count_value] = this.preterrain.splat[this.count_value] - this.preterrain.splat[this.count_value] * ((float)1 - this.subfilter_value) * this.current_layer.strength * this.filter_strength;
								this.count_value++;
							}
						}
						else
						{
							this.count_value = 0;
							while (this.count_value < this.preterrain.splat.Length)
							{
								this.preterrain.splat_layer[this.count_value] = this.preterrain.splat_layer[this.count_value] - this.preterrain.splat_layer[this.count_value] * ((float)1 - this.subfilter_value) * this.current_layer.strength * this.filter_strength;
								this.count_value++;
							}
						}
					}
					else if (filter.change_mode == change_mode_enum.filter)
					{
						if (!flag)
						{
							this.count_value = 0;
							while (this.count_value < this.current_layer.splat_output.splat.Count)
							{
								this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] = this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] * ((float)1 - this.filter_strength * this.current_layer.strength * this.subfilter_value) + this.current_layer.splat_output.splat_calc[this.count_value] * this.subfilter_value * this.filter_strength * this.current_layer.strength;
								this.count_value++;
							}
						}
						else
						{
							this.count_value = 0;
							while (this.count_value < 4)
							{
								if (this.count_value > this.preterrain.splat.Length - 1)
								{
									break;
								}
								this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] = this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] * ((float)1 - this.filter_strength * this.current_layer.strength * this.subfilter_value) + color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
								this.count_value++;
							}
						}
					}
					else
					{
						this.count_value = 0;
						while (this.count_value < this.preterrain.splat.Length)
						{
							this.preterrain.splat_layer[this.count_value] = this.preterrain.splat_layer[this.count_value] * ((float)1 - this.filter_strength * this.current_layer.strength * this.subfilter_value);
							this.count_value++;
						}
						if (!flag)
						{
							this.count_value = 0;
							while (this.count_value < this.current_layer.splat_output.splat.Count)
							{
								int num17 = this.current_layer.splat_output.splat[this.count_value];
								float num18 = this.preterrain.splat[num17] = this.preterrain.splat[num17] + this.current_layer.splat_output.splat_calc[this.count_value] * this.subfilter_value * this.filter_strength * this.current_layer.strength;
								this.count_value++;
							}
						}
						else
						{
							this.count_value = 0;
							while (this.count_value < 4)
							{
								if (this.count_value > this.preterrain.splat.Length - 1)
								{
									break;
								}
								int num19 = this.current_layer.splat_output.splat[this.count_value];
								float num20 = this.preterrain.splat[num19] = this.preterrain.splat[num19] + color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
								this.count_value++;
							}
						}
					}
				}
			}
		}
		else if (output == condition_output_enum.multiply)
		{
			if (this.current_layer.output == layer_output_enum.heightmap)
			{
				this.filter_value *= this.filter_input * this.filter_strength * this.subfilter_value;
			}
			else
			{
				this.filter_value *= this.filter_input;
			}
			if (filter.combine)
			{
				this.filter_combine = this.filter_value - this.filter_combine_start;
			}
			else
			{
				if (this.current_layer.output == layer_output_enum.color)
				{
					this.count_value = 0;
					while (this.count_value < this.current_layer.splat_output.splat.Count)
					{
						this.preterrain.color[this.count_value] = this.preterrain.color[this.count_value] * (color[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
						this.count_value++;
					}
				}
				if (this.current_layer.output == layer_output_enum.splat)
				{
					if (!flag)
					{
						this.count_value = 0;
						while (this.count_value < this.current_layer.splat_output.splat.Count)
						{
							int num21 = this.current_layer.splat_output.splat[this.count_value];
							float num22 = this.preterrain.splat[num21] = this.preterrain.splat[num21] * (this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
							this.count_value++;
						}
					}
					else
					{
						this.count_value = 0;
						while (this.count_value < 4)
						{
							if (this.count_value > this.preterrain.splat.Length - 1)
							{
								break;
							}
							int num23 = this.current_layer.splat_output.splat[this.count_value];
							float num24 = this.preterrain.splat[num23] = this.preterrain.splat[num23] * (color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
							this.count_value++;
						}
					}
				}
				else if (this.current_layer.output == layer_output_enum.grass)
				{
					this.count_value = 0;
					while (this.count_value < this.preterrain.grass.Length)
					{
						this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] = this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] * (this.current_layer.grass_output.grass_calc[this.count_value] * (this.current_layer.strength * this.subfilter_value));
						this.count_value++;
					}
				}
			}
		}
		else if (output == condition_output_enum.divide)
		{
			if (this.current_layer.output == layer_output_enum.heightmap)
			{
				if (this.filter_input * this.filter_strength * this.subfilter_value != (float)0)
				{
					this.filter_value /= this.filter_input * this.filter_strength * this.subfilter_value;
				}
			}
			else if (this.filter_input != (float)0)
			{
				this.filter_value /= this.filter_input;
			}
			if (filter.combine)
			{
				this.filter_combine = this.filter_value - this.filter_combine_start;
			}
			else
			{
				if (this.current_layer.output == layer_output_enum.color)
				{
					this.count_value = 0;
					while (this.count_value < 3)
					{
						if (color[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value) != (float)0)
						{
							this.preterrain.color[this.count_value] = this.preterrain.color[this.count_value] / (color[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
						}
						this.count_value++;
					}
				}
				if (this.current_layer.output == layer_output_enum.splat)
				{
					if (!flag)
					{
						this.count_value = 0;
						while (this.count_value < this.current_layer.splat_output.splat.Count)
						{
							if (this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value) != (float)0)
							{
								this.preterrain.splat[this.count_value] = this.preterrain.splat[this.count_value] / (this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
							}
							this.count_value++;
						}
					}
					else
					{
						this.count_value = 0;
						while (this.count_value < 4)
						{
							if (this.count_value > this.preterrain.splat.Length - 1)
							{
								break;
							}
							if (color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value) != (float)0)
							{
								int num25 = this.current_layer.splat_output.splat[this.count_value];
								float num26 = this.preterrain.splat[num25] = this.preterrain.splat[num25] / (color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
							}
							this.count_value++;
						}
					}
				}
				else if (this.current_layer.output == layer_output_enum.grass)
				{
					this.count_value = 0;
					while (this.count_value < this.preterrain.grass.Length)
					{
						if (this.current_layer.grass_output.grass_calc[this.current_layer.grass_output.grass[this.count_value].prototypeindex] * (this.current_layer.strength * this.subfilter_value) != (float)0)
						{
							this.preterrain.grass[this.count_value] = this.preterrain.grass[this.count_value] / this.current_layer.grass_output.grass_calc[this.count_value] * (this.current_layer.strength * this.subfilter_value);
						}
						this.count_value++;
					}
				}
			}
		}
		else if (output == condition_output_enum.average)
		{
			if (this.current_layer.output == layer_output_enum.heightmap)
			{
				this.filter_value += this.filter_input * this.filter_strength * this.subfilter_value / (float)this.current_layer.prefilter.filter_index.Count;
			}
			else
			{
				this.filter_value += this.filter_input / (float)this.current_layer.prefilter.filter_index.Count;
			}
			if (filter.combine)
			{
				this.filter_combine = this.filter_value - this.filter_combine_start;
			}
			else
			{
				if (this.current_layer.output == layer_output_enum.color)
				{
					this.count_value = 0;
					while (this.count_value < 3)
					{
						this.preterrain.color[this.count_value] = this.preterrain.color[this.count_value] + color[0] * (this.current_layer.strength * this.filter_strength * this.subfilter_value) / (float)this.current_layer.prefilter.filter_index.Count;
						this.count_value++;
					}
				}
				if (this.current_layer.output == layer_output_enum.splat)
				{
					if (!flag)
					{
						this.count_value = 0;
						while (this.count_value < this.current_layer.splat_output.splat.Count)
						{
							int num27 = this.current_layer.splat_output.splat[this.count_value];
							float num28 = this.preterrain.splat[num27] = this.preterrain.splat[num27] + this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value) / (float)this.current_layer.prefilter.filter_index.Count;
							this.count_value++;
						}
					}
					else
					{
						this.count_value = 0;
						while (this.count_value < 4)
						{
							if (this.count_value > this.preterrain.splat.Length - 1)
							{
								break;
							}
							int num29 = this.current_layer.splat_output.splat[this.count_value];
							float num30 = this.preterrain.splat[num29] = this.preterrain.splat[num29] + color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value) / (float)this.current_layer.prefilter.filter_index.Count;
							this.count_value++;
						}
					}
				}
				else if (this.current_layer.output == layer_output_enum.grass)
				{
					this.count_value = 0;
					while (this.count_value < this.preterrain.grass.Length)
					{
						this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] = this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] + this.current_layer.grass_output.grass_calc[this.count_value] * (this.current_layer.strength * this.subfilter_value) / (float)this.current_layer.prefilter.filter_index.Count;
						this.count_value++;
					}
				}
			}
		}
		else if (output == condition_output_enum.difference)
		{
			if (this.current_layer.output == layer_output_enum.heightmap)
			{
				this.filter_value = Mathf.Abs(this.filter_value - this.filter_input * this.filter_strength * this.subfilter_value);
			}
			else
			{
				this.filter_value = Mathf.Abs(this.filter_value - this.filter_input);
			}
			if (filter.combine)
			{
				this.filter_combine = this.filter_value - this.filter_combine_start;
			}
			else
			{
				if (this.current_layer.output == layer_output_enum.color)
				{
					this.count_value = 0;
					while (this.count_value < 3)
					{
						this.preterrain.color[this.count_value] = Mathf.Abs(this.preterrain.color[this.count_value] - color[0] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
						this.count_value++;
					}
				}
				if (this.current_layer.output == layer_output_enum.splat)
				{
					if (!flag)
					{
						this.count_value = 0;
						while (this.count_value < this.current_layer.splat_output.splat.Count)
						{
							this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] = Mathf.Abs(this.preterrain.splat[this.count_value] - this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
							this.count_value++;
						}
					}
					else
					{
						this.count_value = 0;
						while (this.count_value < 4)
						{
							if (this.count_value > this.preterrain.splat.Length - 1)
							{
								break;
							}
							int num31 = this.current_layer.splat_output.splat[this.count_value];
							float num32 = this.preterrain.splat[num31] = this.preterrain.splat[num31] + Mathf.Abs(this.preterrain.splat[this.count_value] - color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value));
							this.count_value++;
						}
					}
				}
				else if (this.current_layer.output == layer_output_enum.grass)
				{
					this.count_value = 0;
					while (this.count_value < this.preterrain.grass.Length)
					{
						this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] = Mathf.Abs(this.preterrain.grass[this.count_value] - this.current_layer.grass_output.grass_calc[this.count_value] * (this.current_layer.strength * this.subfilter_value));
						this.count_value++;
					}
				}
			}
		}
		else if (output == condition_output_enum.max)
		{
			if (this.filter_input * this.filter_strength > this.filter_value)
			{
				if (this.current_layer.output == layer_output_enum.heightmap)
				{
					this.filter_value = this.filter_input * this.filter_strength * this.subfilter_value;
				}
				else
				{
					this.filter_value = this.filter_input;
				}
				if (filter.combine)
				{
					this.filter_combine = this.filter_value - this.filter_combine_start;
				}
				else
				{
					if (this.current_layer.output == layer_output_enum.color)
					{
						this.count_value = 0;
						while (this.count_value < 3)
						{
							this.preterrain.color[this.count_value] = color[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
							this.count_value++;
						}
					}
					if (this.current_layer.output == layer_output_enum.splat)
					{
						if (!flag)
						{
							this.count_value = 0;
							while (this.count_value < this.current_layer.splat_output.splat.Count)
							{
								this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] = this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
								this.count_value++;
							}
						}
						else
						{
							this.count_value = 0;
							while (this.count_value < 4)
							{
								if (this.count_value > this.preterrain.splat.Length - 1)
								{
									break;
								}
								this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] = color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
								this.count_value++;
							}
						}
					}
					else if (this.current_layer.output == layer_output_enum.grass)
					{
						this.count_value = 0;
						while (this.count_value < this.preterrain.grass.Length)
						{
							this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] = this.current_layer.grass_output.grass_calc[this.count_value] * (this.current_layer.strength * this.subfilter_value);
							this.count_value++;
						}
					}
				}
			}
		}
		else if (output == condition_output_enum.min)
		{
			if (this.filter_input * this.filter_strength < this.filter_value)
			{
				if (this.current_layer.output == layer_output_enum.heightmap)
				{
					this.filter_value = this.filter_input * this.filter_strength * this.subfilter_value;
				}
				else
				{
					this.filter_value = this.filter_input;
				}
				if (filter.combine)
				{
					this.filter_combine = this.filter_value - this.filter_combine_start;
				}
				else
				{
					if (this.current_layer.output == layer_output_enum.color)
					{
						this.count_value = 0;
						while (this.count_value < 3)
						{
							this.preterrain.color[this.count_value] = color[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
							this.count_value++;
						}
					}
					if (this.current_layer.output == layer_output_enum.splat)
					{
						if (!flag)
						{
							this.count_value = 0;
							while (this.count_value < this.current_layer.splat_output.splat.Count)
							{
								this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] = this.current_layer.splat_output.splat_calc[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
								this.count_value++;
							}
						}
						else
						{
							this.count_value = 0;
							while (this.count_value < 4)
							{
								if (this.count_value > this.preterrain.splat.Length - 1)
								{
									break;
								}
								this.preterrain.splat[this.current_layer.splat_output.splat[this.count_value]] = color2[this.count_value] * (this.current_layer.strength * this.filter_strength * this.subfilter_value);
								this.count_value++;
							}
						}
					}
					else if (this.current_layer.output == layer_output_enum.grass)
					{
						this.count_value = 0;
						while (this.count_value < this.preterrain.grass.Length)
						{
							this.preterrain.grass[this.current_layer.grass_output.grass[this.count_value].prototypeindex] = this.current_layer.grass_output.grass_calc[this.count_value] * (this.current_layer.strength * this.subfilter_value);
							this.count_value++;
						}
					}
				}
			}
		}
	}

	public override void calc_subfilter_value(filter_class filter, subfilter_class subfilter, float counter_y, float x)
	{
		float num = (float)0;
		float num2 = UnityEngine.Random.Range((float)0, 1f);
		float num3 = 0f;
		float num4 = 0f;
		int i = 0;
		condition_type_enum type = subfilter.type;
		if (type == condition_type_enum.RandomRange)
		{
			if (this.random_range >= subfilter.range_start && this.random_range <= subfilter.range_end)
			{
				num = num2;
				subfilter.range_count++;
			}
		}
		else if (type == condition_type_enum.Random)
		{
			num = num2;
		}
		else if (type == condition_type_enum.Height)
		{
			num = this.height;
		}
		else if (type == condition_type_enum.Steepness)
		{
			num = this.degree / (float)90;
		}
		else if (type == condition_type_enum.Always)
		{
			num = subfilter.curve_position;
		}
		else if (type == condition_type_enum.Image)
		{
			this.color = this.calc_image_value(subfilter.preimage, this.layer_x, this.layer_y);
			if (subfilter.preimage.output)
			{
				num = subfilter.preimage.output_pos;
				if (this.current_layer.output == layer_output_enum.tree && subfilter.from_tree)
				{
					this.tree_color[0] = this.color[0] * subfilter.strength;
					this.tree_color[1] = this.color[1] * subfilter.strength;
					this.tree_color[2] = this.color[2] * subfilter.strength;
					for (i = 0; i < subfilter.precurve_list.Count; i++)
					{
						curve_type_enum type2 = subfilter.precurve_list[i].type;
						if (type2 == curve_type_enum.Normal)
						{
							this.tree_color[0] = subfilter.precurve_list[i].curve.Evaluate(this.tree_color[0]);
							this.tree_color[1] = subfilter.precurve_list[i].curve.Evaluate(this.tree_color[1]);
							this.tree_color[2] = subfilter.precurve_list[i].curve.Evaluate(this.tree_color[2]);
						}
					}
				}
			}
		}
		else if (type == condition_type_enum.RawHeightmap)
		{
			this.calc_raw_value(subfilter.raw, this.layer_x, this.layer_y);
			num = subfilter.raw.output_pos;
		}
		else if (type == condition_type_enum.MaxCount)
		{
			if (subfilter.output_count >= subfilter.output_max)
			{
				this.subfilter_value = (float)0;
				return;
			}
			if (this.subfilter_value >= subfilter.output_count_min)
			{
				subfilter.output_count++;
			}
		}
		else if (type == condition_type_enum.Splatmap)
		{
			if (subfilter.splatmap < this.preterrain.splat_alpha.Length)
			{
				this.color = this.preterrain.splat_alpha[subfilter.splatmap].GetPixel(this.map_x, this.map_y);
				num = this.color[subfilter.splat_index - subfilter.splatmap * 4];
			}
			else
			{
				num = (float)1;
			}
		}
		else if (type == condition_type_enum.RayCast)
		{
			if (subfilter.raycast_mode == raycast_mode_enum.Hit)
			{
				if (Physics.SphereCast(new Vector3(x + this.prelayer.prearea.step.x / (float)2, this.height * this.preterrain.size.y + subfilter.cast_height, this.prelayer.counter_y), this.prelayer.prearea.step.x / (float)2 * subfilter.ray_radius, subfilter.ray_direction, out this.hit, subfilter.ray_length, subfilter.layerMask))
				{
					this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
					if ((this.layerHit & subfilter.layerMask) != 0)
					{
						num = (float)0;
					}
					else
					{
						num = (float)1;
					}
				}
				else
				{
					num = (float)1;
				}
			}
			else if (subfilter.raycast_mode == raycast_mode_enum.Height)
			{
				if (Physics.Raycast(new Vector3(x + this.prelayer.prearea.step.x / (float)2, subfilter.cast_height, this.prelayer.counter_y), subfilter.ray_direction, out this.hit, subfilter.ray_length, subfilter.layerMask))
				{
					this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
					if ((this.layerHit & subfilter.layerMask) != 0)
					{
						num = this.hit.point.y / this.preterrain.terrain.terrainData.size.y;
					}
					else
					{
						num = (float)0;
					}
				}
				else
				{
					num = (float)0;
				}
			}
		}
		for (i = 0; i < subfilter.precurve_list.Count; i++)
		{
			curve_type_enum type3 = subfilter.precurve_list[i].type;
			if (type3 == curve_type_enum.Normal)
			{
				num = subfilter.precurve_list[i].curve.Evaluate(num);
			}
			else if (type3 == curve_type_enum.Random)
			{
				num3 = subfilter.precurve_list[i].curve.Evaluate(num);
				if (!subfilter.precurve_list[i].abs)
				{
					num += UnityEngine.Random.Range(-num3, num3);
				}
				else
				{
					num += UnityEngine.Random.Range((float)0, num3);
				}
			}
			else if (type3 == curve_type_enum.Perlin)
			{
				num4 = subfilter.precurve_list[i].curve.Evaluate(num);
				num3 = this.perlin_noise(this.prelayer.x, this.prelayer.counter_y, subfilter.precurve_list[i].offset.x, subfilter.precurve_list[i].offset.y, subfilter.precurve_list[i].frequency, (float)subfilter.precurve_list[i].detail, subfilter.precurve_list[i].detail_strength) * num4 * subfilter.precurve_list[i].strength;
				if (!subfilter.precurve_list[i].abs)
				{
					num += num3 * (float)2 - num4;
				}
				else
				{
					num += num3;
				}
			}
		}
		if (subfilter.mode != subfilter_mode_enum.strength)
		{
			float num5 = this.filter_input;
			float num6 = this.filter_input;
			int num7 = (int)((x - this.prelayer.prearea.area.xMin) / this.prelayer.prearea.step.x);
			if (Mathf.Abs((float)num7 - filter.last_pos_x) <= this.prelayer.prearea.step.x)
			{
				if (subfilter.mode == subfilter_mode_enum.smooth)
				{
					num5 = Mathf.SmoothStep(filter.last_value_x[0], num5, (float)1 - num) * subfilter.strength;
				}
				if (subfilter.mode == subfilter_mode_enum.lerp)
				{
					num5 = Mathf.Lerp(filter.last_value_x[0], num5, (float)1 - num) * subfilter.strength;
				}
			}
			for (int j = 0; j < this.terrains.Count; j++)
			{
				if (this.terrains[j].rect.Contains(new Vector2(Mathf.Round(x / this.preterrain.heightmap_conversion.x) * this.preterrain.heightmap_conversion.x, counter_y + this.preterrain.heightmap_conversion.y)) && j == this.preterrain.index)
				{
					if (subfilter.mode == subfilter_mode_enum.smooth)
					{
						num6 = Mathf.SmoothStep(filter.last_value_y[num7], num6, (float)1 - num) * subfilter.strength;
					}
					else if (subfilter.mode == subfilter_mode_enum.lerp)
					{
						num6 = Mathf.Lerp(filter.last_value_y[num7], num6, (float)1 - num) * subfilter.strength;
					}
				}
			}
			this.filter_input = (num5 + num6) / (float)2;
		}
		else
		{
			subfilter_output_enum output = subfilter.output;
			if (output == subfilter_output_enum.add)
			{
				this.subfilter_value += num * subfilter.strength;
			}
			else if (output == subfilter_output_enum.subtract)
			{
				this.subfilter_value -= num * subfilter.strength;
			}
			else if (output == subfilter_output_enum.min)
			{
				if (num * subfilter.strength < this.subfilter_value)
				{
					this.subfilter_value = num * subfilter.strength + this.subfilter_value * ((float)1 - subfilter.strength);
				}
			}
			else if (output == subfilter_output_enum.max)
			{
				if (num * subfilter.strength > this.subfilter_value)
				{
					this.subfilter_value = num * subfilter.strength + this.subfilter_value * ((float)1 - subfilter.strength);
				}
			}
			else if (output == subfilter_output_enum.average)
			{
				this.subfilter_value += num * subfilter.strength / (float)filter.presubfilter.subfilter_index.Count;
			}
		}
	}

	public override void calc_raw_value(raw_class raw, float local_x, float local_y)
	{
		if (raw.raw_list_mode == list_condition_enum.Terrain)
		{
			if (this.preterrain.index > raw.file_index.Count - 1)
			{
				raw.raw_number = 0;
			}
			else
			{
				raw.raw_number = this.preterrain.index_old;
			}
		}
		if (raw.raw_number > raw.file_index.Count - 1)
		{
			raw.raw_number = 0;
		}
		Vector2 vector = default(Vector2);
		Vector2 vector2 = default(Vector2);
		Vector2 vector3 = default(Vector2);
		float num = (float)0;
		float num2 = (float)0;
		float num3 = (float)1;
		float num4 = (float)1;
		if (raw.flip_x)
		{
			num3 = (float)-1;
			num = this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1;
		}
		if (raw.flip_y)
		{
			num4 = (float)-1;
			num2 = this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1;
		}
		if (raw.raw_mode == image_mode_enum.MultiTerrain)
		{
			if (this.settings.showMeshes)
			{
				vector.x = Mathf.Round(((this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1) / this.meshes_area.area.width * (this.prelayer.x - this.meshes_area.area.xMin) * num3 - raw.tile_offset_x + num) / raw.tile_x);
				vector.y = Mathf.Round(((this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1) / this.meshes_area.area.height * (this.prelayer.counter_y - this.meshes_area.area.yMin) * num4 - raw.tile_offset_y + num2) / raw.tile_y);
			}
			else
			{
				float num5 = 0f;
				float num6 = 0f;
				num5 = (this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1) / this.preterrain.tiles.x * this.preterrain.tile_x;
				num6 = (this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1) / this.preterrain.tiles.y * this.preterrain.tile_z;
				vector.x = Mathf.Round(((local_x / raw.conversion_step.x + num5) * num3 - raw.tile_offset_x + num) / raw.tile_x);
				vector.y = Mathf.Round(((local_y / raw.conversion_step.y + num6) * num4 - raw.tile_offset_y + num2) / raw.tile_y);
			}
		}
		else if (raw.raw_mode == image_mode_enum.Terrain)
		{
			if (this.settings.showMeshes)
			{
				vector.x = Mathf.Round(((this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1) / this.premesh.area.width * (this.prelayer.x - this.premesh.area.xMin) * num3 - raw.tile_offset_x + num) / raw.tile_x);
				vector.y = Mathf.Round(((this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1) / this.premesh.area.height * (this.prelayer.counter_y - this.premesh.area.yMin) * num4 - raw.tile_offset_y + num2) / raw.tile_y);
			}
			else
			{
				vector.x = (local_x / raw.conversion_step.x * num3 - raw.tile_offset_x + num) / raw.tile_x;
				vector.y = (local_y / raw.conversion_step.y * num4 - raw.tile_offset_y + num2) / raw.tile_y;
			}
		}
		else if (this.settings.showMeshes)
		{
			vector.x = Mathf.Round(((this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1) / this.premesh.area.width * (this.prelayer.x - this.premesh.area.xMin) * num3 - raw.tile_offset_x + num) / raw.tile_x);
			vector.y = Mathf.Round(((this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1) / this.premesh.area.height * (this.prelayer.counter_y - this.premesh.area.yMin) * num4 - raw.tile_offset_y + num2) / raw.tile_y);
		}
		else
		{
			vector.x = ((local_x - this.prelayer.prearea.area_old.x) / raw.conversion_step.x * num3 - raw.tile_offset_x + num) / raw.tile_x;
			vector.y = ((local_y - this.prelayer.prearea.area_old.y) / raw.conversion_step.y * num4 - raw.tile_offset_y + num2) / raw.tile_y;
		}
		if (raw.rotation)
		{
			vector = this.calc_rotation_pixel(vector.x, vector.y, this.raw_files[raw.file_index[raw.raw_number]].resolution.x / (float)2 / raw.conversion_step.x, this.raw_files[raw.file_index[raw.raw_number]].resolution.y / (float)2 / raw.conversion_step.y, raw.rotation_value);
		}
		vector2.x = Mathf.Floor(vector.x);
		vector2.y = Mathf.Floor(vector.y);
		vector3.x = Mathf.Ceil(vector.x);
		vector3.y = Mathf.Ceil(vector.y);
		float num7 = vector.x - vector2.x;
		float num8 = vector.y - vector2.y;
		if (vector2.x < (float)0)
		{
			vector2.x = (float)0;
		}
		if (vector2.x > this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1)
		{
			vector2.x = this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1;
		}
		if (vector2.y < (float)0)
		{
			vector2.y = (float)0;
		}
		if (vector2.y > this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1)
		{
			vector2.y = this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1;
		}
		if (vector3.x < (float)0)
		{
			vector3.x = (float)0;
		}
		if (vector3.x > this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1)
		{
			vector3.x = this.raw_files[raw.file_index[raw.raw_number]].resolution.x - (float)1;
		}
		if (vector3.y < (float)0)
		{
			vector3.y = (float)0;
		}
		if (vector3.y > this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1)
		{
			vector3.y = this.raw_files[raw.file_index[raw.raw_number]].resolution.y - (float)1;
		}
		int num9 = (int)(vector2.y * this.raw_files[raw.file_index[raw.raw_number]].resolution.x * (float)2 + vector2.x * (float)2);
		int num10 = (int)(vector2.y * this.raw_files[raw.file_index[raw.raw_number]].resolution.x * (float)2 + vector3.x * (float)2);
		int num11 = (int)(vector3.y * this.raw_files[raw.file_index[raw.raw_number]].resolution.x * (float)2 + vector2.x * (float)2);
		int num12 = (int)(vector3.y * this.raw_files[raw.file_index[raw.raw_number]].resolution.x * (float)2 + vector3.x * (float)2);
		if (num9 > this.raw_files[raw.file_index[raw.raw_number]].bytes.Length - 1)
		{
			Debug.Log("The Raw Heightmap file '" + this.raw_files[raw.file_index[raw.raw_number]].file + "' has a lower resolution than selected. Please check the File size. It should be X*Y*2 = " + this.raw_files[raw.file_index[raw.raw_number]].resolution.x + "*" + this.raw_files[raw.file_index[raw.raw_number]].resolution.y + "*2 = " + this.raw_files[raw.file_index[raw.raw_number]].resolution.x * this.raw_files[raw.file_index[raw.raw_number]].resolution.y * (float)2 + " Bytes (" + this.raw_files[raw.file_index[raw.raw_number]].resolution.x + "*" + this.raw_files[raw.file_index[raw.raw_number]].resolution.y + " resolution). But the File size is " + this.raw_files[raw.file_index[raw.raw_number]].bytes.Length + " Bytes (" + Mathf.Round(Mathf.Sqrt((float)(this.raw_files[raw.file_index[raw.raw_number]].bytes.Length / 2))) + "x" + Mathf.Round(Mathf.Sqrt((float)(this.raw_files[raw.file_index[raw.raw_number]].bytes.Length / 2))) + " resolution).");
			this.prelayer.x = (float)999999999999L;
			this.prelayer.counter_y = (float)-99999999999L;
			this.generate = false;
		}
		else
		{
			float num13 = 0f;
			float num14 = 0f;
			float num15 = 0f;
			float num16 = 0f;
			if (this.raw_files[raw.file_index[raw.raw_number]].mode == raw_mode_enum.Mac)
			{
				num13 = ((float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num9] * 256f + (float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num9 + 1]) / 65535f;
				num14 = ((float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num10] * 256f + (float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num10 + 1]) / 65535f;
				num15 = ((float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num11] * 256f + (float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num11 + 1]) / 65535f;
				num16 = ((float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num12] * 256f + (float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num12 + 1]) / 65535f;
			}
			else if (this.raw_files[raw.file_index[raw.raw_number]].mode == raw_mode_enum.Windows)
			{
				num13 = ((float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num9] + (float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num9 + 1] * 256f) / 65535f;
				num14 = ((float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num10] + (float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num10 + 1] * 256f) / 65535f;
				num15 = ((float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num11] + (float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num11 + 1] * 256f) / 65535f;
				num16 = ((float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num12] + (float)this.raw_files[raw.file_index[raw.raw_number]].bytes[num12 + 1] * 256f) / 65535f;
			}
			float num17 = num13 + (num14 - num13) * num7;
			float num18 = num15 + (num16 - num15) * num7;
			raw.output_pos = num17 + (num18 - num17) * num8;
		}
	}

	public override void generate_filter(filter_class filter)
	{
		layer_class layer_class = new layer_class();
		Rect area = this.terrains[0].prearea.area;
		Vector2 vector = new Vector2(area.width / (float)128, area.height / (float)128);
		Color color = new Color((float)1, (float)1, (float)1, (float)1);
		this.current_layer = layer_class;
		this.current_layer.output = layer_output_enum.heightmap;
		for (float num = area.yMin; num < area.yMax; num += vector.y)
		{
			for (float num2 = area.xMin; num2 < area.xMax; num2 += vector.x)
			{
				this.filter_value = (float)0;
				this.calc_filter_value(filter, num, num2);
				color[0] = this.filter_value;
				color[1] = this.filter_value;
				color[2] = this.filter_value;
				filter.preview_texture.SetPixel((int)(num2 / vector.x), (int)(num / vector.y), color);
			}
		}
		filter.preview_texture.Apply();
	}

	public override void set_image_terrain_mode(int terrain_index)
	{
		for (int i = 0; i < this.filter.Count; i++)
		{
			if (this.filter[i].type == condition_type_enum.Image && this.filter[i].preimage.image_list_mode == list_condition_enum.Terrain)
			{
				if (!this.filter[i].preimage.short_list)
				{
					this.filter[i].preimage.image_number = this.terrains[terrain_index].index_old;
					if (this.filter[i].preimage.image_number > this.filter[i].preimage.image.Count - 1)
					{
						this.filter[i].preimage.image_number = this.filter[i].preimage.image.Count - 1;
					}
				}
			}
		}
		for (int j = 0; j < this.subfilter.Count; j++)
		{
			if (this.subfilter[j].type == condition_type_enum.Image && this.subfilter[j].preimage.image_list_mode == list_condition_enum.Terrain)
			{
				if (!this.subfilter[j].preimage.short_list)
				{
					this.subfilter[j].preimage.image_number = this.terrains[terrain_index].index_old;
					if (this.subfilter[j].preimage.image_number > this.subfilter[j].preimage.image.Count - 1)
					{
						this.subfilter[j].preimage.image_number = this.subfilter[j].preimage.image.Count - 1;
					}
				}
			}
		}
	}

	public override Color get_image_pixel(image_class preimage, float local_x, float local_y)
	{
		Color arg_6D1_0;
		if (!preimage.image[preimage.image_number])
		{
			arg_6D1_0 = new Color((float)0, (float)0, (float)0);
		}
		else
		{
			float num = (float)0;
			float num2 = (float)0;
			float num3 = (float)1;
			float num4 = (float)1;
			Color color = default(Color);
			local_x -= this.prelayer.prearea.image_offset.x;
			local_y -= this.prelayer.prearea.image_offset.y;
			if (preimage.flip_x)
			{
				num3 = (float)-1;
				num = (float)(preimage.image[preimage.image_number].width - 1);
			}
			if (preimage.flip_y)
			{
				num4 = (float)-1;
				num2 = (float)(preimage.image[preimage.image_number].height - 1);
			}
			if (preimage.image_mode == image_mode_enum.Terrain)
			{
				if (this.settings.showMeshes)
				{
					this.imagePosition.x = Mathf.Round(((float)preimage.image[preimage.image_number].width / this.premesh.area.width * (this.prelayer.x - this.premesh.area.xMin) * num3 - preimage.tile_offset_x + num) / preimage.tile_x);
					this.imagePosition.y = Mathf.Round(((float)preimage.image[preimage.image_number].height / this.premesh.area.height * (this.prelayer.counter_y - this.premesh.area.yMin) * num4 - preimage.tile_offset_y + num2) / preimage.tile_y);
				}
				else
				{
					this.imagePosition.x = Mathf.Round((local_x / preimage.conversion_step.x * num3 - preimage.tile_offset_x + num) / preimage.tile_x);
					this.imagePosition.y = Mathf.Round((local_y / preimage.conversion_step.y * num4 - preimage.tile_offset_y + num2) / preimage.tile_y);
				}
			}
			else if (preimage.image_mode == image_mode_enum.MultiTerrain)
			{
				if (this.settings.showMeshes)
				{
					this.imagePosition.x = Mathf.Round(((float)preimage.image[preimage.image_number].width / this.meshes_area.area.width * (this.prelayer.x - this.meshes_area.area.xMin) * num3 - preimage.tile_offset_x + num) / preimage.tile_x);
					this.imagePosition.y = Mathf.Round(((float)preimage.image[preimage.image_number].height / this.meshes_area.area.height * (this.prelayer.counter_y - this.meshes_area.area.yMin) * num4 - preimage.tile_offset_y + num2) / preimage.tile_y);
				}
				else
				{
					float num5 = 0f;
					float num6 = 0f;
					num5 = (float)preimage.image[preimage.image_number].width / this.preterrain.tiles.x * this.preterrain.tile_x;
					num6 = (float)preimage.image[preimage.image_number].height / this.preterrain.tiles.y * this.preterrain.tile_z;
					this.imagePosition.x = Mathf.Round(((local_x / preimage.conversion_step.x + num5) * num3 - preimage.tile_offset_x + num) / preimage.tile_x);
					this.imagePosition.y = Mathf.Round(((local_y / preimage.conversion_step.y + num6) * num4 - preimage.tile_offset_y + num2) / preimage.tile_y);
				}
			}
			else if (this.settings.showMeshes)
			{
				this.imagePosition.x = Mathf.Round(((float)preimage.image[preimage.image_number].width / this.premesh.area.width * (this.prelayer.x - this.premesh.area.xMin) * num3 - preimage.tile_offset_x + num) / preimage.tile_x);
				this.imagePosition.y = Mathf.Round(((float)preimage.image[preimage.image_number].height / this.premesh.area.height * (this.prelayer.counter_y - this.premesh.area.yMin) * num4 - preimage.tile_offset_y + num2) / preimage.tile_y);
			}
			else
			{
				this.imagePosition.x = ((local_x - this.prelayer.prearea.area_old.x) / preimage.conversion_step.x * num3 - preimage.tile_offset_x + num) / preimage.tile_x;
				this.imagePosition.y = ((local_y - this.prelayer.prearea.area_old.y) / preimage.conversion_step.y * num4 - preimage.tile_offset_y + num2) / preimage.tile_y;
			}
			if (preimage.rotation)
			{
				this.imagePosition = this.calc_rotation_pixel(this.imagePosition.x, this.imagePosition.y, (float)(preimage.image[preimage.image_number].width / 2) / preimage.conversion_step.x, (float)(preimage.image[preimage.image_number].height / 2) / preimage.conversion_step.y, preimage.rotation_value);
			}
			if (preimage.clamp)
			{
				if (this.imagePosition.x > (float)preimage.image[preimage.image_number].width || this.imagePosition.x < (float)0 || this.imagePosition.y > (float)preimage.image[preimage.image_number].height || this.imagePosition.y < (float)0)
				{
					color = Color.black;
				}
				else
				{
					color = preimage.image[preimage.image_number].GetPixel((int)this.imagePosition.x, (int)this.imagePosition.y);
				}
			}
			else
			{
				color = preimage.image[preimage.image_number].GetPixel((int)this.imagePosition.x, (int)this.imagePosition.y);
			}
			color *= preimage.image_color;
			arg_6D1_0 = color;
		}
		return arg_6D1_0;
	}

	public override Color calc_image_value(image_class preimage, float local_x, float local_y)
	{
		Color color = this.get_image_pixel(preimage, local_x, local_y);
		float num = 0f;
		preimage.output = true;
		preimage.output_pos = color[0];
		if (!preimage.edge_blur || this.current_layer.output != layer_output_enum.splat)
		{
			if (preimage.precolor_range.color_range.Count > 0)
			{
				preimage.output = false;
				this.count_color_range = 0;
				while (this.count_color_range < preimage.precolor_range.color_range.Count)
				{
					Color color_start = preimage.precolor_range.color_range[this.count_color_range].color_start;
					Color color_end = preimage.precolor_range.color_range[this.count_color_range].color_end;
					if (preimage.select_mode == select_mode_enum.free)
					{
						num = preimage.precolor_range.color_range[this.count_color_range].output;
					}
					else
					{
						layer_output_enum output = this.current_layer.output;
						if (output == layer_output_enum.color)
						{
							num = this.current_layer.color_output.precolor_range[0].color_range_value.select_value[preimage.precolor_range.color_range[this.count_color_range].select_output];
						}
						else if (output == layer_output_enum.splat)
						{
							if (this.current_layer.splat_output.splat.Count > 0)
							{
								num = this.current_layer.splat_output.splat_value.select_value[preimage.precolor_range.color_range[this.count_color_range].select_output];
							}
						}
						else if (output == layer_output_enum.tree)
						{
							num = this.current_layer.tree_output.tree_value.select_value[preimage.precolor_range.color_range[this.count_color_range].select_output];
						}
						else if (output == layer_output_enum.grass)
						{
							num = this.current_layer.grass_output.grass_value.select_value[preimage.precolor_range.color_range[this.count_color_range].select_output];
						}
						else if (output == layer_output_enum.@object)
						{
							num = this.current_layer.object_output.object_value.select_value[preimage.precolor_range.color_range[this.count_color_range].select_output];
						}
					}
					if (preimage.precolor_range.color_range[this.count_color_range].one_color)
					{
						if (preimage.precolor_range.color_range[this.count_color_range].color_start == color && !preimage.precolor_range.color_range[this.count_color_range].invert)
						{
							preimage.output_pos = num;
							preimage.output = true;
						}
						else if (preimage.precolor_range.color_range[this.count_color_range].invert)
						{
							preimage.output_pos = num;
							preimage.output = true;
						}
					}
					else if (this.color_in_range(color, color_start, color_end))
					{
						if (!preimage.precolor_range.color_range[this.count_color_range].invert)
						{
							if (preimage.select_mode == select_mode_enum.free)
							{
								preimage.output_pos = preimage.precolor_range.color_range[this.count_color_range].curve.Evaluate(this.calc_color_pos(color, color_start, color_end));
							}
							else
							{
								preimage.output_pos = num;
								preimage.output_alpha = this.calc_color_pos(color, color_start, color_end);
								this.filter_strength = this.calc_color_pos(color, color_start, color_end);
							}
							preimage.output = true;
						}
					}
					else if (preimage.precolor_range.color_range[this.count_color_range].invert)
					{
						if (preimage.select_mode == select_mode_enum.free)
						{
							preimage.output_pos = (float)1 - preimage.precolor_range.color_range[this.count_color_range].curve.Evaluate(this.calc_color_pos(color, color_start, color_end));
							preimage.output = true;
						}
						else
						{
							preimage.output_pos = num;
							preimage.output_alpha = this.calc_color_pos(color, color_start, color_end);
						}
					}
					this.count_color_range++;
				}
			}
		}
		else
		{
			float num2 = 0f;
			float edge_blur_radius = preimage.edge_blur_radius;
			float num3 = edge_blur_radius * (float)2 + (float)1;
			this.imagePosition.x = this.imagePosition.x - edge_blur_radius;
			this.imagePosition.y = this.imagePosition.y - edge_blur_radius;
			if (this.imagePosition.x < (float)0)
			{
				this.imagePosition.x = (float)0;
				num3 -= edge_blur_radius;
			}
			if (this.imagePosition.y < (float)0)
			{
				this.imagePosition.y = (float)0;
				num3 -= edge_blur_radius;
			}
			if (this.imagePosition.x > (float)preimage.image[preimage.image_number].width - num3 - (float)1)
			{
				this.imagePosition.x = (float)preimage.image[preimage.image_number].width - num3 - (float)1;
				num3 -= edge_blur_radius;
			}
			if (this.imagePosition.y > (float)preimage.image[preimage.image_number].height - num3 - (float)1)
			{
				this.imagePosition.y = (float)preimage.image[preimage.image_number].height - num3 - (float)1;
				num3 -= edge_blur_radius;
			}
			Color[] pixels = preimage.image[preimage.image_number].GetPixels((int)this.imagePosition.x, (int)this.imagePosition.y, (int)num3, (int)num3);
			for (int i = 0; i < this.current_layer.splat_output.splat_calc.Count; i++)
			{
				this.current_layer.splat_output.splat_calc[i] = (float)0;
			}
			for (int j = 0; j < pixels.Length; j++)
			{
				if (this.current_layer.splat_output.splat.Count > 0)
				{
					this.count_color_range = 0;
					while (this.count_color_range < preimage.precolor_range.color_range.Count)
					{
						if (pixels[j] == preimage.precolor_range.color_range[this.count_color_range].color_start)
						{
							if (preimage.select_mode == select_mode_enum.free)
							{
								preimage.output_pos += preimage.precolor_range.color_range[this.count_color_range].output / ((float)pixels.Length * 1f);
							}
							else
							{
								layer_output_enum output2 = this.current_layer.output;
								if (output2 == layer_output_enum.splat)
								{
									num = this.current_layer.splat_output.splat_value.select_value[this.current_layer.splat_output.splat[preimage.precolor_range.color_range[this.count_color_range].select_output]];
									this.current_layer.splat_output.splat_calc[this.current_layer.splat_output.splat[(int)(num * (float)this.current_layer.splat_output.splat.Count)]] = this.current_layer.splat_output.splat_calc[this.current_layer.splat_output.splat[(int)(num * (float)this.current_layer.splat_output.splat.Count)]] + 1f / ((float)pixels.Length * 1f);
								}
							}
						}
						this.count_color_range++;
					}
				}
			}
		}
		return color;
	}

	public override void loop_prelayer(string command, int index, bool loop_inactive)
	{
		bool flag = false;
		bool image_auto_scale = false;
		bool texture_resize_null = false;
		bool flag2 = false;
		bool store_last_values = false;
		bool unload_texture = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		bool flag9 = false;
		bool check_measure_normal = false;
		bool flag10 = false;
		bool foldout_filter = false;
		bool foldout_subfilter = false;
		bool set_as_default = false;
		layer_output_enum layer_output_enum = layer_output_enum.heightmap;
		bool flag11 = default(bool);
		int i = 0;
		int j = 0;
		int k = 0;
		if (command.IndexOf("(gfc)") != -1)
		{
		}
		if (command.IndexOf("(cmn)") != -1)
		{
			check_measure_normal = true;
		}
		if (command.IndexOf("(rsc") != -1)
		{
			flag9 = true;
		}
		if (command.IndexOf("(ias)") != -1)
		{
			image_auto_scale = true;
		}
		if (command.IndexOf("(trn)") != -1)
		{
			texture_resize_null = true;
		}
		if (command.IndexOf("(ed)") != -1)
		{
			flag2 = true;
		}
		if (command.IndexOf("(slv)") != -1)
		{
			store_last_values = true;
		}
		if (command.IndexOf("(ut)") != -1)
		{
			unload_texture = true;
		}
		if (command.IndexOf("(ocr)") != -1)
		{
			flag4 = true;
		}
		if (command.IndexOf("(asr)") != -1)
		{
			flag5 = true;
		}
		if (command.IndexOf("(cpo)") != -1)
		{
			flag = true;
		}
		if (command.IndexOf("(inf)") != -1)
		{
			flag7 = true;
		}
		if (command.IndexOf("(eho)") != -1)
		{
			flag8 = true;
		}
		if (command.IndexOf("(caf)") != -1)
		{
			flag10 = true;
		}
		if (command.IndexOf("(ff)") != -1)
		{
			foldout_filter = true;
		}
		if (command.IndexOf("(fs)") != -1)
		{
			foldout_subfilter = true;
		}
		if (command.IndexOf("(sad)") != -1)
		{
			set_as_default = true;
		}
		if (command.IndexOf("(fix)") != -1)
		{
			flag6 = true;
		}
		if (flag6 || flag7)
		{
			this.reset_link_filter();
			this.reset_link_subfilter();
			this.settings.prelayers_linked = 0;
			this.settings.filters_linked = 0;
			this.settings.subfilters_linked = 0;
		}
		if (command.IndexOf("(fl)") != -1)
		{
			if (command.IndexOf("(heightmap)") != -1)
			{
				layer_output_enum = layer_output_enum.heightmap;
			}
			else if (command.IndexOf("(color)") != -1)
			{
				layer_output_enum = layer_output_enum.color;
			}
			else if (command.IndexOf("(splat)") != -1)
			{
				layer_output_enum = layer_output_enum.splat;
			}
			else if (command.IndexOf("(tree)") != -1)
			{
				layer_output_enum = layer_output_enum.tree;
			}
			else if (command.IndexOf("(grass)") != -1)
			{
				layer_output_enum = layer_output_enum.grass;
			}
			else if (command.IndexOf("(object)") != -1)
			{
				layer_output_enum = layer_output_enum.@object;
			}
			if (command.IndexOf("(true)") != -1)
			{
				flag11 = true;
			}
			else if (command.IndexOf("(false)") != -1)
			{
				flag11 = false;
			}
			flag3 = true;
		}
		this.count_prelayer = 0;
		while (this.count_prelayer < this.prelayers.Count)
		{
			prelayer_class prelayer_class = this.prelayers[this.count_prelayer];
			if (flag5)
			{
				if (this.settings.showTerrains)
				{
					if (this.prelayers[this.count_prelayer].prearea.resolution_mode == resolution_mode_enum.Automatic)
					{
						this.select_automatic_step_resolution(this.terrains[0], this.prelayers[this.count_prelayer].prearea);
					}
					this.set_area_resolution(this.terrains[0], this.prelayers[this.count_prelayer].prearea);
					this.prelayers[this.count_prelayer].prearea.area_old = this.prelayers[this.count_prelayer].prearea.area;
					this.prelayers[this.count_prelayer].prearea.step_old = this.prelayers[this.count_prelayer].prearea.step;
				}
				else
				{
					this.select_automatic_step_resolution_mesh(this.prelayers[this.count_prelayer].prearea);
					this.set_area_resolution(this.prelayers[this.count_prelayer].prearea);
					this.prelayers[this.count_prelayer].prearea.area_old = this.prelayers[this.count_prelayer].prearea.area;
					this.prelayers[this.count_prelayer].prearea.step_old = this.prelayers[this.count_prelayer].prearea.step;
				}
			}
			if (flag9 || flag10)
			{
				for (int l = 0; l < this.prelayers[this.count_prelayer].predescription.description.Count; l++)
				{
					if (flag9)
					{
						this.prelayers[this.count_prelayer].predescription.description[l].swap_text = "S";
						this.prelayers[this.count_prelayer].predescription.description[l].swap_select = false;
						this.prelayers[this.count_prelayer].predescription.description[l].copy_select = false;
					}
					if (flag10)
					{
						this.prelayers[this.count_prelayer].predescription.description[l].foldout = false;
					}
				}
			}
			if (flag10)
			{
				this.prelayers[this.count_prelayer].prearea.foldout = false;
			}
			this.count_layer = 0;
			while (this.count_layer < prelayer_class.layer.Count)
			{
				layer_class layer_class = prelayer_class.layer[this.count_layer];
				if (!flag2)
				{
					goto IL_8D7;
				}
				bool flag12 = false;
				if (!layer_class.active)
				{
					flag12 = true;
				}
				else if ((layer_class.output != layer_output_enum.color || !this.color_output) && (layer_class.output != layer_output_enum.splat || !this.splat_output) && (layer_class.output != layer_output_enum.tree || !this.tree_output) && (layer_class.output != layer_output_enum.grass || !this.grass_output) && (layer_class.output != layer_output_enum.@object || !this.object_output) && (layer_class.output != layer_output_enum.heightmap || !this.heightmap_output))
				{
					flag12 = true;
				}
				if (flag12)
				{
					this.erase_layer(prelayer_class, this.count_layer, 0, 0, false, true, false);
					this.count_layer--;
				}
				else
				{
					if (layer_class.output == layer_output_enum.color)
					{
						for (int m = 0; m < layer_class.color_output.precolor_range.Count; m++)
						{
							for (int n = 0; n < layer_class.color_output.precolor_range[m].color_range.Count; n++)
							{
								if (!layer_class.color_output.precolor_range[m].color_range_value.active[n])
								{
									layer_class.color_output.precolor_range[m].erase_color_range(n);
									this.loop_prefilter_index(layer_class.prefilter, n);
									n--;
								}
							}
						}
						goto IL_8D7;
					}
					if (layer_class.output == layer_output_enum.splat)
					{
						for (i = 0; i < layer_class.splat_output.splat.Count; i++)
						{
							if (!layer_class.splat_output.splat_value.active[i] || layer_class.splat_output.splat[i] > this.terrains[0].terrain.terrainData.splatPrototypes.Length - 1)
							{
								layer_class.splat_output.erase_splat(i);
								this.loop_prefilter_index(layer_class.prefilter, i);
								i--;
							}
						}
						goto IL_8D7;
					}
					if (layer_class.output == layer_output_enum.grass)
					{
						for (int num = 0; num < layer_class.grass_output.grass_value.active.Count; num++)
						{
							if (!layer_class.grass_output.grass_value.active[num] || layer_class.grass_output.grass[num].prototypeindex > this.terrains[0].terrain.terrainData.detailPrototypes.Length - 1)
							{
								layer_class.grass_output.erase_grass(num);
								this.loop_prefilter_index(layer_class.prefilter, num);
								num--;
							}
						}
						goto IL_8D7;
					}
					goto IL_8D7;
				}
				IL_F2D:
				this.count_layer++;
				continue;
				IL_8D7:
				if (flag9)
				{
					layer_class.swap_text = "S";
					layer_class.swap_select = false;
					layer_class.copy_select = false;
					layer_class.tree_output.placed = 0;
					layer_class.object_output.placed = 0;
					layer_class.text_placed = string.Empty;
					for (j = 0; j < layer_class.tree_output.tree.Count; j++)
					{
						layer_class.tree_output.tree[j].placed = 0;
					}
					for (k = 0; k < layer_class.object_output.@object.Count; k++)
					{
						layer_class.object_output.@object[k].placed = 0;
					}
				}
				if (flag10)
				{
					layer_class.foldout = false;
					layer_class.tree_output.foldout = false;
					for (j = 0; j < layer_class.tree_output.tree.Count; j++)
					{
						layer_class.tree_output.tree[j].foldout = false;
						layer_class.tree_output.tree[j].scale_foldout = false;
						layer_class.tree_output.tree[j].distance_foldout = false;
						layer_class.tree_output.tree[j].data_foldout = false;
						layer_class.tree_output.tree[j].precolor_range.foldout = false;
					}
					layer_class.object_output.foldout = false;
					for (k = 0; k < layer_class.object_output.@object.Count; k++)
					{
						layer_class.object_output.@object[k].foldout = false;
						layer_class.object_output.@object[k].data_foldout = false;
						layer_class.object_output.@object[k].transform_foldout = false;
						layer_class.object_output.@object[k].settings_foldout = false;
						layer_class.object_output.@object[k].distance_foldout = false;
						layer_class.object_output.@object[k].rotation_foldout = false;
						layer_class.object_output.@object[k].rotation_map_foldout = false;
					}
				}
				if (layer_class.active || loop_inactive)
				{
					if (flag8 && layer_class.output == layer_output_enum.heightmap)
					{
						if (layer_class.smooth)
						{
							this.smooth_command = true;
						}
						this.heightmap_output_layer = true;
					}
					if (flag3)
					{
						if (layer_class.output == layer_output_enum)
						{
							if (flag11)
							{
								layer_class.foldout = false;
							}
							else
							{
								layer_class.foldout = true;
							}
						}
						else
						{
							layer_class.foldout = false;
						}
					}
					j = 0;
					while (j < layer_class.tree_output.tree.Count)
					{
						if (!flag2)
						{
							goto IL_C78;
						}
						if (layer_class.tree_output.tree_value.active[j] && layer_class.tree_output.tree[j].prototypeindex <= this.terrains[0].terrain.terrainData.treePrototypes.Length - 1)
						{
							this.erase_deactive_color_range(layer_class.tree_output.tree[j].precolor_range);
							goto IL_C78;
						}
						layer_class.tree_output.erase_tree(j, this);
						this.loop_prefilter_index(layer_class.prefilter, j);
						j--;
						IL_CB6:
						j++;
						continue;
						IL_C78:
						this.call_from = 1;
						this.loop_prefilter(layer_class.tree_output.tree[j].prefilter, index, flag6, flag7, loop_inactive, image_auto_scale, texture_resize_null, unload_texture, flag2, store_last_values, flag9, check_measure_normal, flag10, foldout_filter, foldout_subfilter, set_as_default);
						goto IL_CB6;
					}
					if (layer_class.output == layer_output_enum.@object)
					{
						for (k = 0; k < layer_class.object_output.@object.Count; k++)
						{
							if (flag2 && (!layer_class.object_output.object_value.active[k] || !layer_class.object_output.@object[k].object1))
							{
								this.erase_object(this.prelayers[this.count_prelayer].layer[this.count_layer].object_output, k);
								this.loop_prefilter_index(layer_class.prefilter, k);
								k--;
							}
							else
							{
								object_class object_class = layer_class.object_output.@object[k];
								if ((flag6 || flag7) && object_class.prelayer_created)
								{
									if (object_class.prelayer_index > this.prelayers.Count - 1)
									{
										if (!flag7)
										{
											Debug.Log("Prelayer reference -> " + object_class.prelayer_index + " not found, erasing reference entry...");
											object_class.prelayer_created = false;
											object_class.prelayer_index = -1;
										}
									}
									else if (!flag7)
									{
										this.prelayers[object_class.prelayer_index].linked = true;
									}
									else
									{
										this.settings.prelayers_linked = this.settings.prelayers_linked + 1;
									}
								}
								if (((layer_class.object_output.object_value.active[k] && this.object_output) || loop_inactive) && flag)
								{
									if (flag4)
									{
										this.create_object_child_list(object_class);
									}
									if (object_class.parent_clear || loop_inactive)
									{
										this.clear_parent_object(object_class);
									}
								}
								if (object_class.rotation_map.active)
								{
									object_class.rotation_map.preimage.set_image_auto_scale(this.terrains[0], this.prelayers[this.count_prelayer].prearea.area_old, 0);
								}
							}
						}
					}
					this.call_from = 0;
					this.loop_prefilter(layer_class.prefilter, index, flag6, flag7, loop_inactive, image_auto_scale, texture_resize_null, unload_texture, flag2, store_last_values, flag9, check_measure_normal, flag10, foldout_filter, foldout_subfilter, set_as_default);
					goto IL_F2D;
				}
				goto IL_F2D;
			}
			this.count_prelayer++;
		}
		if (flag7 || flag6)
		{
			this.erase_unlinked_prelayer(flag6);
			this.erase_unlinked_filter(flag6);
			this.erase_unlinked_subfilter(flag6);
		}
	}

	public override void loop_prefilter(prefilter_class prefilter1, int index, bool fix_database, bool info_database, bool loop_inactive, bool image_auto_scale, bool texture_resize_null, bool unload_texture, bool erase_deactive, bool store_last_values, bool reset_swap_copy, bool check_measure_normal, bool close_all_foldout, bool foldout_filter, bool foldout_subfilter, bool set_as_default)
	{
		if (close_all_foldout)
		{
			prefilter1.foldout = false;
		}
		this.count_filter = 0;
		while (this.count_filter < prefilter1.filter_index.Count)
		{
			filter_class filter_class = this.filter[prefilter1.filter_index[this.count_filter]];
			if (!erase_deactive)
			{
				goto IL_91;
			}
			if (filter_class.active)
			{
				this.erase_deactive_color_range(filter_class.preimage.precolor_range);
				this.erase_deactive_animation_curve(filter_class.precurve_list);
				goto IL_91;
			}
			this.erase_filter(this.count_filter, prefilter1);
			this.count_filter--;
			IL_C2F:
			this.count_filter++;
			continue;
			IL_91:
			if (fix_database || info_database)
			{
				if (prefilter1.filter_index[this.count_filter] > this.filter.Count - 1)
				{
					Debug.Log("Filter reference -> " + prefilter1.filter_index[this.count_filter] + " not found, erasing reference entry...");
					if (!info_database)
					{
						this.erase_filter_reference(prefilter1, this.count_filter);
						this.count_filter--;
						goto IL_C2F;
					}
				}
				else if (this.filter[prefilter1.filter_index[this.count_filter]].linked)
				{
					Debug.Log("Filter double linked -> " + prefilter1.filter_index[this.count_filter]);
					if (fix_database)
					{
						this.filter.Add(new filter_class());
						this.filter[this.filter.Count - 1] = this.copy_filter(this.filter[prefilter1.filter_index[this.count_filter]], true);
						prefilter1.filter_index[this.count_filter] = this.filter.Count - 1;
						goto IL_C2F;
					}
				}
				else
				{
					this.filter[prefilter1.filter_index[this.count_filter]].linked = true;
					if (this.filter[prefilter1.filter_index[this.count_filter]].linked)
					{
						this.settings.filters_linked = this.settings.filters_linked + 1;
					}
				}
			}
			if (foldout_filter && prefilter1.filter_index[this.count_filter] == index)
			{
				int num = this.get_layer_description(this.prelayers[this.count_prelayer], this.count_layer);
				if (num != -1)
				{
					this.prelayers[this.count_prelayer].predescription.description[num].foldout = true;
				}
				this.prelayers[this.count_prelayer].foldout = true;
				this.prelayers[this.count_prelayer].layer[this.count_layer].foldout = true;
				if (this.call_from == 1)
				{
					this.prelayers[this.count_prelayer].layer[this.count_layer].tree_output.foldout = true;
					this.prelayers[this.count_prelayer].layer[this.count_layer].tree_output.tree[this.count_tree].foldout = true;
				}
				prefilter1.foldout = true;
				filter_class.foldout = true;
			}
			if (set_as_default)
			{
				for (int i = 0; i < filter_class.precurve_list.Count; i++)
				{
					filter_class.precurve_list[i].set_as_default();
				}
			}
			if (close_all_foldout)
			{
				filter_class.foldout = false;
				filter_class.presubfilter.foldout = false;
			}
			if (reset_swap_copy)
			{
				this.filter[prefilter1.filter_index[this.count_filter]].swap_text = "S";
				this.filter[prefilter1.filter_index[this.count_filter]].swap_select = false;
				this.filter[prefilter1.filter_index[this.count_filter]].copy_select = false;
			}
			if (check_measure_normal && filter_class.active && !this.measure_normal && filter_class.type == condition_type_enum.Direction)
			{
				this.measure_normal = true;
			}
			if (filter_class.active || loop_inactive)
			{
				if (image_auto_scale && filter_class.preimage.image_auto_scale && this.settings.showTerrains)
				{
					if (!this.prelayers[this.count_prelayer].prearea.active)
					{
						filter_class.preimage.set_image_auto_scale(this.terrains[0], this.terrains[0].prearea.area_old, 0);
					}
					else
					{
						filter_class.preimage.set_image_auto_scale(this.terrains[0], this.prelayers[this.count_prelayer].prearea.area_old, 0);
					}
				}
				if (unload_texture && filter_class.preimage.image.Count > 0)
				{
					for (int j = 0; j < filter_class.preimage.image.Count; j++)
					{
						if (this.current_filter.preimage.image[j])
						{
							Resources.UnloadAsset(filter_class.preimage.image[j]);
						}
					}
				}
				filter_class.sub_strength_set = false;
				this.count_subfilter = 0;
				while (this.count_subfilter < filter_class.presubfilter.subfilter_index.Count)
				{
					subfilter_class subfilter_class = this.subfilter[filter_class.presubfilter.subfilter_index[this.count_subfilter]];
					if (!erase_deactive)
					{
						goto IL_63F;
					}
					if (subfilter_class.active)
					{
						this.erase_deactive_animation_curve(subfilter_class.precurve_list);
						this.erase_deactive_color_range(subfilter_class.preimage.precolor_range);
						goto IL_63F;
					}
					this.erase_subfilter(this.count_subfilter, filter_class.presubfilter);
					this.count_subfilter--;
					IL_C06:
					this.count_subfilter++;
					continue;
					IL_63F:
					if (fix_database || info_database)
					{
						if (filter_class.presubfilter.subfilter_index[this.count_subfilter] > this.subfilter.Count - 1)
						{
							Debug.Log("Subfilter reference -> " + filter_class.presubfilter.subfilter_index[this.count_subfilter] + " not found, erasing reference entry...");
							if (!info_database)
							{
								this.erase_subfilter_reference(filter_class.presubfilter, this.count_subfilter);
								this.count_subfilter--;
								goto IL_C06;
							}
						}
						if (this.subfilter[filter_class.presubfilter.subfilter_index[this.count_subfilter]].linked)
						{
							Debug.Log("Subfilter double linked -> " + filter_class.presubfilter.subfilter_index[this.count_subfilter]);
							if (fix_database)
							{
								this.subfilter.Add(new subfilter_class());
								this.subfilter[this.subfilter.Count - 1] = this.copy_subfilter(this.subfilter[filter_class.presubfilter.subfilter_index[this.count_subfilter]]);
								filter_class.presubfilter.subfilter_index[this.count_subfilter] = this.subfilter.Count - 1;
								goto IL_C06;
							}
						}
						else
						{
							this.subfilter[filter_class.presubfilter.subfilter_index[this.count_subfilter]].linked = true;
							if (this.filter[prefilter1.filter_index[this.count_filter]].linked)
							{
								this.settings.subfilters_linked = this.settings.subfilters_linked + 1;
							}
						}
					}
					if (foldout_subfilter && filter_class.presubfilter.subfilter_index[this.count_subfilter] == index)
					{
						int num2 = this.get_layer_description(this.prelayers[this.count_prelayer], this.count_layer);
						if (num2 != -1)
						{
							this.prelayers[this.count_prelayer].predescription.description[num2].foldout = true;
						}
						this.prelayers[this.count_prelayer].foldout = true;
						this.prelayers[this.count_prelayer].layer[this.count_layer].foldout = true;
						if (this.call_from == 1)
						{
							this.prelayers[this.count_prelayer].layer[this.count_layer].tree_output.foldout = true;
							this.prelayers[this.count_prelayer].layer[this.count_layer].tree_output.tree[this.count_tree].foldout = true;
						}
						prefilter1.foldout = true;
						subfilter_class.foldout = true;
						filter_class.foldout = true;
						filter_class.presubfilter.foldout = true;
					}
					if (set_as_default)
					{
						subfilter_class.precurve.set_as_default();
						subfilter_class.prerandom_curve.set_as_default();
					}
					if (close_all_foldout)
					{
						subfilter_class.foldout = false;
					}
					if (check_measure_normal && subfilter_class.active && !this.measure_normal && subfilter_class.type == condition_type_enum.Direction)
					{
						this.measure_normal = true;
					}
					if (reset_swap_copy)
					{
						subfilter_class.swap_text = "S";
						subfilter_class.swap_select = false;
						subfilter_class.copy_select = false;
					}
					if (!subfilter_class.active && !loop_inactive)
					{
						goto IL_C06;
					}
					if (image_auto_scale && subfilter_class.preimage.image_auto_scale && this.settings.showTerrains)
					{
						if (!this.prelayers[this.count_prelayer].prearea.active)
						{
							subfilter_class.preimage.set_image_auto_scale(this.terrains[0], this.terrains[0].prearea.area_old, 0);
						}
						else
						{
							subfilter_class.preimage.set_image_auto_scale(this.terrains[0], this.prelayers[this.count_prelayer].prearea.area_old, 0);
						}
					}
					if (subfilter_class.mode == subfilter_mode_enum.strength)
					{
						filter_class.sub_strength_set = true;
					}
					if (store_last_values && subfilter_class.mode != subfilter_mode_enum.strength && !this.current_filter.last_value_declared)
					{
						filter_class.last_value_x = new float[1];
						if (this.generate_world_mode)
						{
							filter_class.last_value_y = new float[(int)(this.prelayers[this.count_prelayer].prearea.area.width / this.prelayers[this.count_prelayer].prearea.step.x + (float)1)];
						}
						else
						{
							filter_class.last_value_y = new float[(int)(this.terrains[0].size.x / this.terrains[0].prearea.step.x + (float)2)];
						}
						filter_class.last_pos_x = (float)4097;
						filter_class.last_value_declared = true;
					}
					if (unload_texture)
					{
						for (int j = 0; j < subfilter_class.preimage.image.Count; j++)
						{
							if (subfilter_class.preimage.image != null)
							{
								Resources.UnloadAsset(subfilter_class.preimage.image[j]);
							}
						}
						goto IL_C06;
					}
					goto IL_C06;
				}
				goto IL_C2F;
			}
			goto IL_C2F;
		}
	}

	public override Rect get_terrain_size(int terrain_index)
	{
		Rect result = default(Rect);
		if (this.terrains[terrain_index].terrain && this.terrains[terrain_index].terrain.terrainData)
		{
			result.width = this.terrains[terrain_index].terrain.terrainData.size.x;
			result.height = this.terrains[terrain_index].terrain.terrainData.size.z;
		}
		return result;
	}

	public override Rect get_total_terrain_size()
	{
		Rect result = default(Rect);
		if (this.terrains[0].terrain && this.terrains[0].terrain.terrainData)
		{
			result.width = this.terrains[0].terrain.terrainData.size.x * this.terrains[0].tiles.x;
			result.height = this.terrains[0].terrain.terrainData.size.z * this.terrains[0].tiles.y;
		}
		return result;
	}

	public override void loop_prefilter_index(prefilter_class prefilter1, int index)
	{
		for (int i = 0; i < prefilter1.filter_index.Count; i++)
		{
			if (this.filter[prefilter1.filter_index[i]].type == condition_type_enum.Image && this.filter[prefilter1.filter_index[i]].preimage.select_mode == select_mode_enum.select)
			{
				for (int j = 0; j < this.filter[prefilter1.filter_index[i]].preimage.precolor_range.color_range.Count; j++)
				{
					if (this.filter[prefilter1.filter_index[i]].preimage.precolor_range.color_range[j].select_output == index)
					{
						this.filter[prefilter1.filter_index[i]].preimage.precolor_range.erase_color_range(j);
						j--;
					}
					else if (this.filter[prefilter1.filter_index[i]].preimage.precolor_range.color_range[j].select_output > index)
					{
						this.filter[prefilter1.filter_index[i]].preimage.precolor_range.color_range[j].select_output = index;
					}
				}
			}
		}
	}

	public override void disable_prefilter_select_mode(prefilter_class prefilter1)
	{
		for (int i = 0; i < prefilter1.filter_index.Count; i++)
		{
			this.filter[prefilter1.filter_index[i]].preimage.select_mode = select_mode_enum.free;
		}
	}

	public override void link_placed_reference()
	{
		if (this.script_base)
		{
			int i = 0;
			int j = 0;
			int k = 0;
			for (int l = 0; l < this.prelayers.Count; l++)
			{
				for (i = 0; i < this.prelayers[l].layer.Count; i++)
				{
					if (this.prelayers[l].layer[i].active)
					{
						if (this.prelayers[l].layer[i].output == layer_output_enum.tree)
						{
							this.prelayers[l].layer[i].tree_output.placed = 0;
							this.script_base.prelayers[l].layer[i].tree_output.placed_reference = this.prelayers[l].layer[i].tree_output;
							for (j = 0; j < this.prelayers[l].layer[i].tree_output.tree.Count; j++)
							{
								this.prelayers[l].layer[i].tree_output.tree[j].placed = 0;
								this.script_base.prelayers[l].layer[i].tree_output.tree[j].placed_reference = this.prelayers[l].layer[i].tree_output.tree[j];
							}
						}
						if (this.prelayers[l].layer[i].output == layer_output_enum.@object)
						{
							this.prelayers[l].layer[i].object_output.placed = 0;
							this.script_base.prelayers[l].layer[i].object_output.placed_reference = this.prelayers[l].layer[i].object_output;
							this.prelayers[l].layer[i].object_output.placed_reference = this.script_base.prelayers[l].layer[i].object_output;
							if (this.prelayers[l].layer[i].object_output.object_mode == object_mode_enum.LinePlacement)
							{
								this.script_base.prelayers[l].layer[i].object_output.line_placement.line_list[0].points.Clear();
								for (int m = 0; m < this.prelayers[l].layer[i].object_output.line_placement.line_list[0].point_length; m++)
								{
									this.script_base.prelayers[l].layer[i].object_output.line_placement.line_list[0].points.Add(new Vector3((float)0, (float)0, (float)0));
								}
								if (this.prelayers[l].layer[i].object_output.line_placement.preimage.image_auto_scale && this.settings.showTerrains)
								{
									if (!this.generate_world_mode && l < 1)
									{
										this.prelayers[l].layer[i].object_output.line_placement.preimage.set_image_auto_scale(this.terrains[0], this.terrains[0].prearea.area_old, 0);
									}
									else
									{
										this.prelayers[l].layer[i].object_output.line_placement.preimage.set_image_auto_scale(this.terrains[0], this.prelayers[l].prearea.area_old, 0);
									}
								}
								this.line_output = true;
							}
							for (k = 0; k < this.prelayers[l].layer[i].object_output.@object.Count; k++)
							{
								this.prelayers[l].layer[i].object_output.@object[k].placed = 0;
								this.script_base.prelayers[l].layer[i].object_output.@object[k].placed_reference = this.prelayers[l].layer[i].object_output.@object[k];
							}
						}
					}
				}
			}
		}
	}

	public override void line_generate(int count_prelayer)
	{
		int i = 0;
		int num = 0;
		int num2 = 0;
		for (i = 0; i < this.prelayers[count_prelayer].layer.Count; i++)
		{
			if (this.prelayers[count_prelayer].layer[i].output == layer_output_enum.@object && this.prelayers[count_prelayer].layer[i].object_output.object_mode == object_mode_enum.LinePlacement)
			{
				this.create_object_line(this.prelayers[count_prelayer].layer[i].object_output);
			}
		}
	}

	public override void erase_deactive_color_range(precolor_range_class precolor_range)
	{
		for (int i = 0; i < precolor_range.color_range.Count; i++)
		{
			if (!precolor_range.color_range_value.active[i])
			{
				precolor_range.erase_color_range(i);
				i--;
			}
		}
	}

	public override void erase_deactive_animation_curve(List<animation_curve_class> precurve_list)
	{
		for (int i = 0; i < precurve_list.Count; i++)
		{
			if (!precurve_list[i].active)
			{
				this.erase_animation_curve(precurve_list, i);
				i--;
			}
		}
	}

	public override void clear_parent_object(object_class current_object1)
	{
		if (this.settings.parentObjectsTerrain)
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				for (int j = 0; j < this.terrains[i].terrain.transform.childCount; j++)
				{
					Transform transform = this.terrains[i].terrain.transform.GetChild(j);
					if (transform.name == "Objects")
					{
						UnityEngine.Object.DestroyImmediate(transform.gameObject);
						j--;
					}
				}
			}
		}
		else
		{
			Transform parent = current_object1.parent;
			if (parent && current_object1.parent_clear)
			{
				int instanceID = parent.gameObject.GetInstanceID();
				Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>(true);
				if (componentsInChildren != null)
				{
					IEnumerator enumerator = componentsInChildren.GetEnumerator();
					while (enumerator.MoveNext())
					{
						object arg_109_0;
						object expr_EF = arg_109_0 = enumerator.Current;
						if (!(expr_EF is Transform))
						{
							arg_109_0 = RuntimeServices.Coerce(expr_EF, typeof(Transform));
						}
						Transform transform = (Transform)arg_109_0;
						if (transform && transform.gameObject.GetInstanceID() != instanceID)
						{
							UnityEngine.Object.DestroyImmediate(transform.gameObject);
						}
					}
				}
			}
		}
	}

	public override void unload_textures1()
	{
		int i = 0;
		for (int j = 0; j < this.filter.Count; j++)
		{
			for (i = 0; i < this.filter[j].preimage.image.Count; i++)
			{
				if (this.filter[j].preimage.image[i])
				{
					Resources.UnloadAsset(this.filter[j].preimage.image[i]);
				}
			}
		}
		for (int k = 0; k < this.subfilter.Count; k++)
		{
			for (i = 0; i < this.subfilter[k].preimage.image.Count; i++)
			{
				if (this.subfilter[k].preimage.image[i])
				{
					Resources.UnloadAsset(this.subfilter[k].preimage.image[i]);
				}
			}
		}
		if (this.settings.showTerrains)
		{
			for (int l = 0; l < this.terrains.Count; l++)
			{
				if (this.terrains[l].terrain.terrainData.splatPrototypes.Length > 0)
				{
					for (int m = 0; m < this.terrains[l].splat_alpha.Length; m++)
					{
						if (this.terrains[l].splat_alpha[m])
						{
							this.terrains[l].splat_alpha[m] = null;
						}
					}
				}
			}
		}
	}

	public override void loop_layer(layer_class layer, int command)
	{
		for (int i = 0; i < layer.object_output.@object.Count; i++)
		{
			if (layer.object_output.@object[i].prelayer_created)
			{
				if (command == 1)
				{
					this.add_prelayer(false);
					int num = this.prelayers.Count - 1;
					this.prelayers[num] = this.copy_prelayer(this.prelayers[layer.object_output.@object[i].prelayer_index], true);
					layer.object_output.@object[i].prelayer_index = num;
					this.prelayers[num].index = num;
					this.prelayers[num].set_prelayer_text();
					for (int j = 0; j < this.prelayers[num].layer.Count; j++)
					{
						this.loop_layer(this.prelayers[num].layer[j], 1);
					}
				}
				else if (command == -1)
				{
					this.erase_prelayer(layer.object_output.@object[i].prelayer_index);
				}
			}
		}
	}

	public override void loop_object_copy(object_class @object)
	{
		if (@object.prelayer_created)
		{
			for (int i = 0; i < this.prelayers[@object.prelayer_index].layer.Count; i++)
			{
				this.loop_layer_copy(this.prelayers[@object.prelayer_index].layer[i]);
			}
		}
	}

	public override void loop_layer_copy(layer_class layer)
	{
		for (int i = 0; i < layer.object_output.@object.Count; i++)
		{
			layer.object_output.@object[i].swap_select = false;
			layer.object_output.@object[i].copy_select = false;
			layer.object_output.@object[i].swap_text = "S";
			layer.object_output.@object[i].placed = 0;
			layer.object_output.placed = 0;
			if (layer.object_output.@object[i].prelayer_created)
			{
				int prelayer_index = layer.object_output.@object[i].prelayer_index;
				for (int j = 0; j < this.prelayers[prelayer_index].layer.Count; j++)
				{
					this.loop_layer_copy(this.prelayers[prelayer_index].layer[j]);
				}
			}
		}
		int k = 0;
		for (int l = 0; l < layer.color_output.precolor_range.Count; l++)
		{
			for (k = 0; k < layer.color_output.precolor_range[l].color_range.Count; k++)
			{
				layer.color_output.precolor_range[l].color_range[k].swap_select = false;
				layer.color_output.precolor_range[l].color_range[k].copy_select = false;
				layer.color_output.precolor_range[l].color_range[k].swap_text = "S";
			}
		}
		for (int m = 0; m < layer.tree_output.tree.Count; m++)
		{
			layer.tree_output.tree[m].swap_select = false;
			layer.tree_output.tree[m].copy_select = false;
			layer.tree_output.tree[m].swap_text = "S";
			layer.tree_output.placed = 0;
			layer.tree_output.tree[m].placed = 0;
			layer.tree_output.tree[m].placed = 0;
		}
		layer.swap_select = false;
		layer.copy_select = false;
		layer.swap_text = "S";
	}

	public override int check_object_rotate(List<Vector3> objects_placed, List<Vector3> objects_placed_rot, Vector3 position, int min_distance_rot_x, int min_distance_rot_z)
	{
		Vector3 vector = default(Vector3);
		int arg_62_0;
		for (int i = 0; i < objects_placed.Count; i++)
		{
			vector = position - objects_placed[i];
			if (Mathf.Abs(vector.x) <= (float)min_distance_rot_x && Mathf.Abs(vector.z) <= (float)min_distance_rot_z)
			{
				arg_62_0 = i;
				return arg_62_0;
			}
		}
		arg_62_0 = -1;
		return arg_62_0;
	}

	public override bool check_object_distance(List<distance_class> object_placed_list)
	{
		float num = 0f;
		float num2 = 0f;
		int arg_17A_0;
		for (int i = object_placed_list.Count - 1; i >= 0; i--)
		{
			num = Vector3.Distance(this.object_info.position, object_placed_list[i].position);
			num2 = object_placed_list[i].position.z - this.object_info.position.z;
			if (num < this.object_info.min_distance.x && num < object_placed_list[i].min_distance.x)
			{
				arg_17A_0 = 0;
				return arg_17A_0 != 0;
			}
		}
		object_placed_list.Add(new distance_class());
		object_placed_list[object_placed_list.Count - 1].position = this.object_info.position;
		object_placed_list[object_placed_list.Count - 1].rotation = this.object_info.rotation;
		object_placed_list[object_placed_list.Count - 1].min_distance = this.object_info.min_distance;
		object_placed_list[object_placed_list.Count - 1].min_distance_rotation_group = this.object_info.min_distance_rotation_group;
		object_placed_list[object_placed_list.Count - 1].distance_rotation = this.object_info.distance_rotation;
		object_placed_list[object_placed_list.Count - 1].distance_mode = this.object_info.distance_mode;
		object_placed_list[object_placed_list.Count - 1].rotation_group = this.object_info.rotation_group;
		arg_17A_0 = 1;
		return arg_17A_0 != 0;
	}

	public override void relink_subfilter_index(int subfilter_index)
	{
		for (int i = 0; i < this.filter.Count; i++)
		{
			for (int j = 0; j < this.filter[i].presubfilter.subfilter_index.Count; j++)
			{
				if (this.filter[i].presubfilter.subfilter_index[j] == this.subfilter.Count)
				{
					this.filter[i].presubfilter.subfilter_index[j] = subfilter_index;
					return;
				}
			}
		}
	}

	public override void relink_filter_index(int filter_index)
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				for (int k = 0; k < this.prelayers[i].layer[j].prefilter.filter_index.Count; k++)
				{
					if (this.prelayers[i].layer[j].prefilter.filter_index[k] == this.filter.Count)
					{
						this.prelayers[i].layer[j].prefilter.filter_index[k] = filter_index;
						return;
					}
				}
				for (int l = 0; l < this.prelayers[i].layer[j].tree_output.tree.Count; l++)
				{
					for (int k = 0; k < this.prelayers[i].layer[j].tree_output.tree[l].prefilter.filter_index.Count; k++)
					{
						if (this.prelayers[i].layer[j].tree_output.tree[l].prefilter.filter_index[k] == this.filter.Count)
						{
							this.prelayers[i].layer[j].tree_output.tree[l].prefilter.filter_index[k] = filter_index;
							return;
						}
					}
				}
			}
		}
	}

	public override bool search_filter_index(int filter_index)
	{
		int arg_193_0;
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				for (int k = 0; k < this.prelayers[i].layer[j].prefilter.filter_index.Count; k++)
				{
					if (this.prelayers[i].layer[j].prefilter.filter_index[k] == filter_index)
					{
						this.filter[filter_index].linked = true;
						arg_193_0 = 1;
						return arg_193_0 != 0;
					}
				}
				for (int l = 0; l < this.prelayers[i].layer[j].tree_output.tree.Count; l++)
				{
					for (int k = 0; k < this.prelayers[i].layer[j].tree_output.tree[l].prefilter.filter_index.Count; k++)
					{
						if (this.prelayers[i].layer[j].tree_output.tree[l].prefilter.filter_index[k] == filter_index)
						{
							this.filter[filter_index].linked = true;
							arg_193_0 = 1;
							return arg_193_0 != 0;
						}
					}
				}
			}
		}
		arg_193_0 = 0;
		return arg_193_0 != 0;
	}

	public override bool search_subfilter_index(int subfilter_index)
	{
		int arg_83_0;
		for (int i = 0; i < this.filter.Count; i++)
		{
			for (int j = 0; j < this.filter[i].presubfilter.subfilter_index.Count; j++)
			{
				if (this.filter[i].presubfilter.subfilter_index[j] == subfilter_index)
				{
					this.subfilter[subfilter_index].linked = true;
					arg_83_0 = 1;
					return arg_83_0 != 0;
				}
			}
		}
		arg_83_0 = 0;
		return arg_83_0 != 0;
	}

	public override void reset_link_prelayer()
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			this.prelayers[i].linked = false;
		}
	}

	public override void reset_link_filter()
	{
		for (int i = 0; i < this.filter.Count; i++)
		{
			this.filter[i].linked = false;
		}
	}

	public override void reset_link_subfilter()
	{
		for (int i = 0; i < this.subfilter.Count; i++)
		{
			this.subfilter[i].linked = false;
		}
	}

	public override void erase_unlinked_prelayer(bool erase)
	{
		for (int i = 1; i < this.prelayers.Count; i++)
		{
			if (!this.prelayers[i].linked)
			{
				if (erase)
				{
					Debug.Log("Erasing unlinked Prelayer -> " + i);
					this.erase_prelayer(i);
					i--;
				}
				else
				{
					Debug.Log("Unlinked Prelayer -> " + i);
				}
			}
		}
	}

	public override void erase_unlinked_filter(bool erase)
	{
		for (int i = 0; i < this.filter.Count; i++)
		{
			if (!this.filter[i].linked)
			{
				if (erase)
				{
					Debug.Log("Erasing unlinked Filter -> " + i);
					this.erase_filter_unlinked(i);
					i--;
				}
				else
				{
					Debug.Log("Unlinked Filter -> " + i);
				}
			}
		}
	}

	public override void erase_unlinked_subfilter(bool erase)
	{
		for (int i = 0; i < this.subfilter.Count; i++)
		{
			if (!this.subfilter[i].linked)
			{
				if (erase)
				{
					Debug.Log("Erasing unlinked Subfilter -> " + i);
					this.erase_subfilter_unlinked(i);
					i--;
				}
				else
				{
					Debug.Log("Unlinked subfilter -> " + i);
				}
			}
		}
	}

	public override void select_image_prelayer()
	{
		for (int i = 0; i < this.prelayer.layer.Count; i++)
		{
			for (int j = 0; j < this.prelayer.layer[i].prefilter.filter_index.Count; j++)
			{
				filter_class current_filter = this.filter[this.prelayer.layer[i].prefilter.filter_index[j]];
				this.select_image_filter(current_filter);
				this.select_image_subfilter(current_filter);
			}
			if (this.prelayer.layer[i].output == layer_output_enum.tree)
			{
				for (int k = 0; k < this.prelayer.layer[i].tree_output.tree.Count; k++)
				{
					for (int j = 0; j < this.prelayer.layer[i].tree_output.tree[k].prefilter.filter_index.Count; j++)
					{
						filter_class current_filter = this.filter[this.prelayer.layer[i].tree_output.tree[k].prefilter.filter_index[j]];
						this.select_image_filter(current_filter);
						this.select_image_subfilter(current_filter);
					}
				}
			}
		}
	}

	public override void select_image_filter(filter_class current_filter1)
	{
		if (current_filter1.type == condition_type_enum.Image && current_filter1.preimage.image_list_mode == list_condition_enum.Random)
		{
			current_filter1.preimage.image_number = UnityEngine.Random.Range(0, current_filter1.preimage.image.Count - 1);
		}
	}

	public override void select_image_subfilter(filter_class current_filter1)
	{
		for (int i = 0; i < current_filter1.presubfilter.subfilter_index.Count; i++)
		{
			subfilter_class subfilter_class = this.subfilter[current_filter1.presubfilter.subfilter_index[i]];
			if (subfilter_class.type == condition_type_enum.Image && subfilter_class.preimage.image_list_mode == list_condition_enum.Random)
			{
				subfilter_class.preimage.image_number = UnityEngine.Random.Range(0, subfilter_class.preimage.image.Count - 1);
			}
		}
	}

	public override int search_filter_swap()
	{
		int arg_3F_0;
		for (int i = 0; i < this.filter.Count; i++)
		{
			if (this.filter[i].swap_select)
			{
				arg_3F_0 = i;
				return arg_3F_0;
			}
		}
		arg_3F_0 = -1;
		return arg_3F_0;
	}

	public override int search_filter_copy()
	{
		int arg_3F_0;
		for (int i = 0; i < this.filter.Count; i++)
		{
			if (this.filter[i].copy_select)
			{
				arg_3F_0 = i;
				return arg_3F_0;
			}
		}
		arg_3F_0 = -1;
		return arg_3F_0;
	}

	public override int search_subfilter_swap()
	{
		int arg_3F_0;
		for (int i = 0; i < this.subfilter.Count; i++)
		{
			if (this.subfilter[i].swap_select)
			{
				arg_3F_0 = i;
				return arg_3F_0;
			}
		}
		arg_3F_0 = -1;
		return arg_3F_0;
	}

	public override int search_subfilter_copy()
	{
		int arg_46_0;
		for (int i = 0; i < this.subfilter.Count; i++)
		{
			if (this.subfilter[i].copy_select)
			{
				arg_46_0 = i;
				return arg_46_0;
			}
		}
		this.copy_subfilter_select = false;
		arg_46_0 = -1;
		return arg_46_0;
	}

	public override void search_layer_swap()
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				if (this.prelayers[i].layer[j].swap_select)
				{
					this.swap_prelayer_index = i;
					this.swap_layer_index = j;
					return;
				}
			}
		}
	}

	public override void search_layer_copy()
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				if (this.prelayers[i].layer[j].copy_select)
				{
					this.copy_prelayer_index = i;
					this.copy_layer_index = j;
					return;
				}
			}
		}
	}

	public override void search_description_swap()
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].predescription.description.Count; j++)
			{
				if (this.prelayers[i].predescription.description[j].swap_select)
				{
					this.swap_description_prelayer_index = i;
					this.swap_description_position = j;
					return;
				}
			}
		}
	}

	public override void search_description_copy()
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].predescription.description.Count; j++)
			{
				if (this.prelayers[i].predescription.description[j].copy_select)
				{
					this.copy_description_prelayer_index = i;
					this.copy_description_position = j;
					return;
				}
			}
		}
	}

	public override void search_object_swap()
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				for (int k = 0; k < this.prelayers[i].layer[j].object_output.@object.Count; k++)
				{
					if (this.prelayers[i].layer[j].object_output.@object[k].swap_select)
					{
						this.swap_object_output = this.prelayers[i].layer[j].object_output;
						this.swap_object_number = k;
						return;
					}
				}
			}
		}
	}

	public override object_class search_object_copy()
	{
		object_class arg_DC_0;
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				for (int k = 0; k < this.prelayers[i].layer[j].object_output.@object.Count; k++)
				{
					if (this.prelayers[i].layer[j].object_output.@object[k].copy_select)
					{
						arg_DC_0 = this.prelayers[i].layer[j].object_output.@object[k];
						return arg_DC_0;
					}
				}
			}
		}
		arg_DC_0 = new object_class();
		return arg_DC_0;
	}

	public override void search_tree_swap()
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				for (int k = 0; k < this.prelayers[i].layer[j].tree_output.tree.Count; k++)
				{
					if (this.prelayers[i].layer[j].tree_output.tree[k].swap_select)
					{
						this.swap_tree_output = this.prelayers[i].layer[j].tree_output;
						this.swap_tree_position = k;
						return;
					}
				}
			}
		}
	}

	public override tree_class search_tree_copy()
	{
		tree_class arg_E3_0;
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				for (int k = 0; k < this.prelayers[i].layer[j].tree_output.tree.Count; k++)
				{
					if (this.prelayers[i].layer[j].tree_output.tree[k].copy_select)
					{
						arg_E3_0 = this.prelayers[i].layer[j].tree_output.tree[k];
						return arg_E3_0;
					}
				}
			}
		}
		arg_E3_0 = new tree_class(this.script, false);
		return arg_E3_0;
	}

	public override void search_color_range_swap()
	{
		int i = 0;
		for (int j = 0; j < this.prelayers.Count; j++)
		{
			for (int k = 0; k < this.prelayers[j].layer.Count; k++)
			{
				for (int l = 0; l < this.prelayers[j].layer[k].color_output.precolor_range.Count; l++)
				{
					for (i = 0; i < this.prelayers[j].layer[k].color_output.precolor_range[l].color_range.Count; i++)
					{
						if (this.prelayers[j].layer[k].color_output.precolor_range[l].color_range[i].swap_select)
						{
							this.swap_precolor_range = this.prelayers[j].layer[k].color_output.precolor_range[l];
							this.swap_color_range_number = i;
							return;
						}
					}
				}
				for (int m = 0; m < this.prelayers[j].layer[k].tree_output.tree.Count; m++)
				{
					for (i = 0; i < this.prelayers[j].layer[k].tree_output.tree[m].precolor_range.color_range.Count; i++)
					{
						if (this.prelayers[j].layer[k].tree_output.tree[m].precolor_range.color_range[i].swap_select)
						{
							this.swap_precolor_range = this.prelayers[j].layer[k].tree_output.tree[m].precolor_range;
							this.swap_color_range_number = i;
							return;
						}
					}
				}
			}
		}
		for (int n = 0; n < this.filter.Count; n++)
		{
			for (i = 0; i < this.filter[n].preimage.precolor_range.color_range.Count; i++)
			{
				if (this.filter[n].preimage.precolor_range.color_range[i].swap_select)
				{
					this.swap_precolor_range = this.filter[n].preimage.precolor_range;
					this.swap_color_range_number = i;
					return;
				}
			}
		}
		for (int num = 0; num < this.subfilter.Count; num++)
		{
			for (i = 0; i < this.subfilter[num].preimage.precolor_range.color_range.Count; i++)
			{
				if (this.subfilter[num].preimage.precolor_range.color_range[i].swap_select)
				{
					this.swap_precolor_range = this.subfilter[num].preimage.precolor_range;
					this.swap_color_range_number = i;
					return;
				}
			}
		}
		for (int num2 = 0; num2 < this.pattern_tool.patterns.Count; num2++)
		{
			for (i = 0; i < this.pattern_tool.patterns[num2].precolor_range.color_range.Count; i++)
			{
				if (this.pattern_tool.patterns[num2].precolor_range.color_range[i].swap_select)
				{
					this.swap_precolor_range = this.pattern_tool.patterns[num2].precolor_range;
					this.swap_color_range_number = i;
					return;
				}
			}
		}
		for (i = 0; i < this.texture_tool.precolor_range.color_range.Count; i++)
		{
			if (this.texture_tool.precolor_range.color_range[i].swap_select)
			{
				this.swap_precolor_range = this.texture_tool.precolor_range;
				this.swap_color_range_number = i;
				break;
			}
		}
	}

	public override color_range_class search_color_range_copy()
	{
		int i = 0;
		color_range_class arg_493_0;
		for (int j = 0; j < this.prelayers.Count; j++)
		{
			for (int k = 0; k < this.prelayers[j].layer.Count; k++)
			{
				for (int l = 0; l < this.prelayers[j].layer[k].color_output.precolor_range.Count; l++)
				{
					for (i = 0; i < this.prelayers[j].layer[k].color_output.precolor_range[l].color_range.Count; i++)
					{
						if (this.prelayers[j].layer[k].color_output.precolor_range[l].color_range[i].copy_select)
						{
							arg_493_0 = this.prelayers[j].layer[k].color_output.precolor_range[l].color_range[i];
							return arg_493_0;
						}
					}
				}
				for (int m = 0; m < this.prelayers[j].layer[k].tree_output.tree.Count; m++)
				{
					for (i = 0; i < this.prelayers[j].layer[k].tree_output.tree[m].precolor_range.color_range.Count; i++)
					{
						if (this.prelayers[j].layer[k].tree_output.tree[m].precolor_range.color_range[i].copy_select)
						{
							arg_493_0 = this.prelayers[j].layer[k].tree_output.tree[m].precolor_range.color_range[i];
							return arg_493_0;
						}
					}
				}
			}
		}
		for (int n = 0; n < this.filter.Count; n++)
		{
			for (i = 0; i < this.filter[n].preimage.precolor_range.color_range.Count; i++)
			{
				if (this.filter[n].preimage.precolor_range.color_range[i].copy_select)
				{
					arg_493_0 = this.filter[n].preimage.precolor_range.color_range[i];
					return arg_493_0;
				}
			}
		}
		for (int num = 0; num < this.subfilter.Count; num++)
		{
			for (i = 0; i < this.subfilter[num].preimage.precolor_range.color_range.Count; i++)
			{
				if (this.subfilter[num].preimage.precolor_range.color_range[i].copy_select)
				{
					arg_493_0 = this.subfilter[num].preimage.precolor_range.color_range[i];
					return arg_493_0;
				}
			}
		}
		for (int num2 = 0; num2 < this.pattern_tool.patterns.Count; num2++)
		{
			for (i = 0; i < this.pattern_tool.patterns[num2].precolor_range.color_range.Count; i++)
			{
				if (this.pattern_tool.patterns[num2].precolor_range.color_range[i].copy_select)
				{
					arg_493_0 = this.pattern_tool.patterns[num2].precolor_range.color_range[i];
					return arg_493_0;
				}
			}
		}
		for (i = 0; i < this.texture_tool.precolor_range.color_range.Count; i++)
		{
			if (this.texture_tool.precolor_range.color_range[i].copy_select)
			{
				arg_493_0 = this.texture_tool.precolor_range.color_range[i];
				return arg_493_0;
			}
		}
		arg_493_0 = new color_range_class();
		return arg_493_0;
	}

	public override int get_import_resolution_to_list(int resolution)
	{
		int result = -1;
		if (resolution == 32)
		{
			result = 0;
		}
		else if (resolution == 64)
		{
			result = 1;
		}
		else if (resolution == 128)
		{
			result = 2;
		}
		else if (resolution == 256)
		{
			result = 3;
		}
		else if (resolution == 512)
		{
			result = 4;
		}
		else if (resolution == 1024)
		{
			result = 5;
		}
		else if (resolution == 2048)
		{
			result = 6;
		}
		else if (resolution == 4096)
		{
			result = 7;
		}
		return result;
	}

	public override int set_import_resolution_from_list(int resolution_index)
	{
		int result = -1;
		if (resolution_index == 0)
		{
			result = 32;
		}
		else if (resolution_index == 1)
		{
			result = 64;
		}
		else if (resolution_index == 2)
		{
			result = 128;
		}
		else if (resolution_index == 3)
		{
			result = 256;
		}
		else if (resolution_index == 4)
		{
			result = 512;
		}
		else if (resolution_index == 5)
		{
			result = 1024;
		}
		else if (resolution_index == 6)
		{
			result = 2048;
		}
		else if (resolution_index == 7)
		{
			result = 4096;
		}
		return result;
	}

	public override void get_terrain_resolution_to_list(terrain_class preterrain1)
	{
		if (preterrain1.heightmap_resolution == (float)4097)
		{
			preterrain1.heightmap_resolution_list = 0;
		}
		else if (preterrain1.heightmap_resolution == (float)2049)
		{
			preterrain1.heightmap_resolution_list = 1;
		}
		else if (preterrain1.heightmap_resolution == (float)1025)
		{
			preterrain1.heightmap_resolution_list = 2;
		}
		else if (preterrain1.heightmap_resolution == (float)513)
		{
			preterrain1.heightmap_resolution_list = 3;
		}
		else if (preterrain1.heightmap_resolution == (float)257)
		{
			preterrain1.heightmap_resolution_list = 4;
		}
		else if (preterrain1.heightmap_resolution == (float)129)
		{
			preterrain1.heightmap_resolution_list = 5;
		}
		else if (preterrain1.heightmap_resolution == (float)65)
		{
			preterrain1.heightmap_resolution_list = 6;
		}
		else if (preterrain1.heightmap_resolution == (float)33)
		{
			preterrain1.heightmap_resolution_list = 7;
		}
		if (preterrain1.splatmap_resolution == (float)2048)
		{
			preterrain1.splatmap_resolution_list = 0;
		}
		else if (preterrain1.splatmap_resolution == (float)1024)
		{
			preterrain1.splatmap_resolution_list = 1;
		}
		else if (preterrain1.splatmap_resolution == (float)512)
		{
			preterrain1.splatmap_resolution_list = 2;
		}
		else if (preterrain1.splatmap_resolution == (float)256)
		{
			preterrain1.splatmap_resolution_list = 3;
		}
		else if (preterrain1.splatmap_resolution == (float)128)
		{
			preterrain1.splatmap_resolution_list = 4;
		}
		else if (preterrain1.splatmap_resolution == (float)64)
		{
			preterrain1.splatmap_resolution_list = 5;
		}
		else if (preterrain1.splatmap_resolution == (float)32)
		{
			preterrain1.splatmap_resolution_list = 6;
		}
		else if (preterrain1.splatmap_resolution == (float)16)
		{
			preterrain1.splatmap_resolution_list = 7;
		}
		if (preterrain1.basemap_resolution == (float)2048)
		{
			preterrain1.basemap_resolution_list = 0;
		}
		else if (preterrain1.basemap_resolution == (float)1024)
		{
			preterrain1.basemap_resolution_list = 1;
		}
		else if (preterrain1.basemap_resolution == (float)512)
		{
			preterrain1.basemap_resolution_list = 2;
		}
		else if (preterrain1.basemap_resolution == (float)256)
		{
			preterrain1.basemap_resolution_list = 3;
		}
		else if (preterrain1.basemap_resolution == (float)128)
		{
			preterrain1.basemap_resolution_list = 4;
		}
		else if (preterrain1.basemap_resolution == (float)64)
		{
			preterrain1.basemap_resolution_list = 5;
		}
		else if (preterrain1.basemap_resolution == (float)32)
		{
			preterrain1.basemap_resolution_list = 6;
		}
		else if (preterrain1.basemap_resolution == (float)16)
		{
			preterrain1.basemap_resolution_list = 7;
		}
		if (preterrain1.detail_resolution_per_patch == (float)128)
		{
			preterrain1.detail_resolution_per_patch_list = 4;
		}
		else if (preterrain1.detail_resolution_per_patch == (float)64)
		{
			preterrain1.detail_resolution_per_patch_list = 3;
		}
		else if (preterrain1.detail_resolution_per_patch == (float)32)
		{
			preterrain1.detail_resolution_per_patch_list = 2;
		}
		else if (preterrain1.detail_resolution_per_patch == (float)16)
		{
			preterrain1.detail_resolution_per_patch_list = 1;
		}
		else if (preterrain1.detail_resolution_per_patch == (float)8)
		{
			preterrain1.detail_resolution_per_patch_list = 0;
		}
	}

	public override void set_terrain_resolution_from_list(terrain_class preterrain1)
	{
		if (preterrain1.heightmap_resolution_list == 0)
		{
			preterrain1.heightmap_resolution = (float)4097;
		}
		else if (preterrain1.heightmap_resolution_list == 1)
		{
			preterrain1.heightmap_resolution = (float)2049;
		}
		else if (preterrain1.heightmap_resolution_list == 2)
		{
			preterrain1.heightmap_resolution = (float)1025;
		}
		else if (preterrain1.heightmap_resolution_list == 3)
		{
			preterrain1.heightmap_resolution = (float)513;
		}
		else if (preterrain1.heightmap_resolution_list == 4)
		{
			preterrain1.heightmap_resolution = (float)257;
		}
		else if (preterrain1.heightmap_resolution_list == 5)
		{
			preterrain1.heightmap_resolution = (float)129;
		}
		else if (preterrain1.heightmap_resolution_list == 6)
		{
			preterrain1.heightmap_resolution = (float)65;
		}
		else if (preterrain1.heightmap_resolution_list == 7)
		{
			preterrain1.heightmap_resolution = (float)33;
		}
		if (preterrain1.splatmap_resolution_list == 0)
		{
			preterrain1.splatmap_resolution = (float)2048;
		}
		else if (preterrain1.splatmap_resolution_list == 1)
		{
			preterrain1.splatmap_resolution = (float)1024;
		}
		else if (preterrain1.splatmap_resolution_list == 2)
		{
			preterrain1.splatmap_resolution = (float)512;
		}
		else if (preterrain1.splatmap_resolution_list == 3)
		{
			preterrain1.splatmap_resolution = (float)256;
		}
		else if (preterrain1.splatmap_resolution_list == 4)
		{
			preterrain1.splatmap_resolution = (float)128;
		}
		else if (preterrain1.splatmap_resolution_list == 5)
		{
			preterrain1.splatmap_resolution = (float)64;
		}
		else if (preterrain1.splatmap_resolution_list == 6)
		{
			preterrain1.splatmap_resolution = (float)32;
		}
		else if (preterrain1.splatmap_resolution_list == 7)
		{
			preterrain1.splatmap_resolution = (float)16;
		}
		if (preterrain1.basemap_resolution_list == 0)
		{
			preterrain1.basemap_resolution = (float)2048;
		}
		else if (preterrain1.basemap_resolution_list == 1)
		{
			preterrain1.basemap_resolution = (float)1024;
		}
		else if (preterrain1.basemap_resolution_list == 2)
		{
			preterrain1.basemap_resolution = (float)512;
		}
		else if (preterrain1.basemap_resolution_list == 3)
		{
			preterrain1.basemap_resolution = (float)256;
		}
		else if (preterrain1.basemap_resolution_list == 4)
		{
			preterrain1.basemap_resolution = (float)128;
		}
		else if (preterrain1.basemap_resolution_list == 5)
		{
			preterrain1.basemap_resolution = (float)64;
		}
		else if (preterrain1.basemap_resolution_list == 6)
		{
			preterrain1.basemap_resolution = (float)32;
		}
		else if (preterrain1.basemap_resolution_list == 7)
		{
			preterrain1.basemap_resolution = (float)16;
		}
		if (preterrain1.detail_resolution_per_patch_list == 0)
		{
			preterrain1.detail_resolution_per_patch = (float)8;
		}
		else if (preterrain1.detail_resolution_per_patch_list == 1)
		{
			preterrain1.detail_resolution_per_patch = (float)16;
		}
		else if (preterrain1.detail_resolution_per_patch_list == 2)
		{
			preterrain1.detail_resolution_per_patch = (float)32;
		}
		else if (preterrain1.detail_resolution_per_patch_list == 3)
		{
			preterrain1.detail_resolution_per_patch = (float)64;
		}
		else if (preterrain1.detail_resolution_per_patch_list == 4)
		{
			preterrain1.detail_resolution_per_patch = (float)128;
		}
	}

	public override void get_terrains_position()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.terrains[i].rect.x = this.terrains[i].terrain.transform.position.x;
			this.terrains[i].rect.y = this.terrains[i].terrain.transform.position.z;
			this.terrains[i].rect.width = this.terrains[i].terrain.terrainData.size.x;
			this.terrains[i].rect.height = this.terrains[i].terrain.terrainData.size.z;
		}
		if (this.slice_tool && this.slice_tool_terrain)
		{
			this.slice_tool_rect.x = this.slice_tool_terrain.transform.position.x;
			this.slice_tool_rect.y = this.slice_tool_terrain.transform.position.z;
			this.slice_tool_rect.width = this.slice_tool_terrain.terrainData.size.x;
			this.slice_tool_rect.height = this.slice_tool_terrain.terrainData.size.z;
		}
	}

	public override void set_basemap_max(bool editor)
	{
		if (this.terrains.Count > 1)
		{
			if (editor)
			{
				this.settings.editor_basemap_distance_max = (int)(this.terrains[0].tiles.x * this.terrains[0].size.x);
			}
			else
			{
				this.settings.runtime_basemap_distance_max = (int)(this.terrains[0].tiles.x * this.terrains[0].size.x);
			}
		}
		else if (editor)
		{
			this.settings.editor_basemap_distance_max = (int)this.terrains[0].size.x;
		}
		else
		{
			this.settings.runtime_basemap_distance_max = (int)this.terrains[0].size.x;
		}
	}

	public override void get_all_terrain_settings(string command)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.get_terrain_settings(this.terrains[i], command);
			this.check_synchronous_terrain_size(this.terrains[i]);
		}
	}

	public override void get_terrain_settings(terrain_class preterrain1, string command)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				bool flag8 = false;
				if (command.IndexOf("(siz)") != -1)
				{
					flag = true;
				}
				if (command.IndexOf("(res)") != -1)
				{
					flag2 = true;
				}
				if (command.IndexOf("(con)") != -1)
				{
					flag3 = true;
				}
				if (command.IndexOf("(all)") != -1)
				{
					flag4 = true;
				}
				if (command.IndexOf("(fir)") != -1)
				{
					flag5 = true;
				}
				if (command.IndexOf("(spl)") != -1)
				{
					flag6 = true;
				}
				if (command.IndexOf("(tre)") != -1)
				{
					flag7 = true;
				}
				if (command.IndexOf("(gra)") != -1)
				{
					flag8 = true;
				}
				preterrain1.splat_length = preterrain1.terrain.terrainData.splatPrototypes.Length;
				preterrain1.name = preterrain1.terrain.name;
				if (flag || flag4)
				{
					preterrain1.size = preterrain1.terrain.terrainData.size;
					this.check_synchronous_terrain_size(preterrain1);
					preterrain1.scale.x = preterrain1.size.x / (float)preterrain1.terrain.terrainData.heightmapResolution;
					preterrain1.scale.y = preterrain1.size.y / (float)preterrain1.terrain.terrainData.heightmapResolution;
					preterrain1.scale.z = preterrain1.size.z / (float)preterrain1.terrain.terrainData.heightmapResolution;
				}
				if (flag2 || flag4)
				{
					preterrain1.heightmap_resolution = (float)preterrain1.terrain.terrainData.heightmapResolution;
					preterrain1.splatmap_resolution = (float)preterrain1.terrain.terrainData.alphamapResolution;
					preterrain1.detail_resolution = (float)preterrain1.terrain.terrainData.detailResolution;
					preterrain1.basemap_resolution = (float)preterrain1.terrain.terrainData.baseMapResolution;
					this.get_terrain_resolution_to_list(preterrain1);
					this.check_synchronous_terrain_resolutions(preterrain1);
				}
				if (flag3 || flag4)
				{
					preterrain1.heightmap_conversion.x = preterrain1.terrain.terrainData.size.x / (float)(preterrain1.terrain.terrainData.heightmapResolution - 1);
					preterrain1.heightmap_conversion.y = preterrain1.terrain.terrainData.size.z / (float)(preterrain1.terrain.terrainData.heightmapResolution - 1);
					preterrain1.splatmap_conversion.x = preterrain1.terrain.terrainData.size.x / (float)(preterrain1.terrain.terrainData.alphamapResolution - 1);
					preterrain1.splatmap_conversion.y = preterrain1.terrain.terrainData.size.z / (float)(preterrain1.terrain.terrainData.alphamapResolution - 1);
					preterrain1.detailmap_conversion.x = preterrain1.terrain.terrainData.size.x / (float)(preterrain1.terrain.terrainData.detailResolution - 1);
					preterrain1.detailmap_conversion.y = preterrain1.terrain.terrainData.size.z / (float)(preterrain1.terrain.terrainData.detailResolution - 1);
					this.set_area_resolution(preterrain1, preterrain1.prearea);
				}
				if (flag5)
				{
					preterrain1.prearea.area_max = new Rect((float)0, (float)0, preterrain1.terrain.terrainData.size.x, preterrain1.terrain.terrainData.size.z);
					preterrain1.prearea.area = preterrain1.prearea.area_max;
					preterrain1.prearea.set_resolution_mode_text();
					this.get_terrain_parameter_settings(preterrain1);
				}
				if (flag || flag4 || flag3 || flag2 || flag5)
				{
					this.set_area_resolution(preterrain1, preterrain1.prearea);
					this.set_area_resolution_prelayers(preterrain1);
				}
				if (preterrain1.prearea.area.xMax > preterrain1.terrain.terrainData.size.x)
				{
					preterrain1.prearea.area.xMax = preterrain1.terrain.terrainData.size.x;
				}
				if (preterrain1.prearea.area.yMax > preterrain1.terrain.terrainData.size.y)
				{
					preterrain1.prearea.area.yMax = preterrain1.terrain.terrainData.size.z;
				}
				if (flag6 || flag4)
				{
					this.get_terrain_splat_textures(preterrain1);
					this.check_synchronous_terrain_splat_textures(preterrain1);
				}
				if (flag7 || flag4)
				{
					this.get_terrain_trees(preterrain1);
					this.check_synchronous_terrain_trees(preterrain1);
				}
				if (flag8 || flag4)
				{
					this.get_terrain_details(preterrain1);
					this.check_synchronous_terrain_detail(preterrain1);
				}
			}
		}
	}

	public override void set_area_resolution_prelayers(terrain_class preterrain1)
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			this.set_area_resolution(preterrain1, this.prelayers[i].prearea);
		}
	}

	public override void set_area_resolution(area_class prearea)
	{
		prearea.step.x = this.object_resolution;
		prearea.step.y = this.object_resolution;
		prearea.conversion_step = prearea.step;
		prearea.resolution = this.object_resolution;
	}

	public override void set_area_resolution(terrain_class preterrain1, area_class prearea)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				if (prearea.resolution_mode == resolution_mode_enum.Heightmap)
				{
					prearea.step.x = preterrain1.terrain.terrainData.size.x / (float)(preterrain1.terrain.terrainData.heightmapResolution - 1);
					prearea.step.y = preterrain1.terrain.terrainData.size.z / (float)(preterrain1.terrain.terrainData.heightmapResolution - 1);
					prearea.conversion_step = prearea.step;
					prearea.resolution = preterrain1.heightmap_resolution;
				}
				else if (prearea.resolution_mode == resolution_mode_enum.Colormap)
				{
					prearea.step.x = preterrain1.terrain.terrainData.size.x / (float)preterrain1.prearea.colormap_resolution;
					prearea.step.y = preterrain1.terrain.terrainData.size.z / (float)preterrain1.prearea.colormap_resolution;
					preterrain1.splatmap_conversion.x = preterrain1.terrain.terrainData.size.x / (float)(preterrain1.prearea.colormap_resolution - 1);
					preterrain1.splatmap_conversion.y = preterrain1.terrain.terrainData.size.z / (float)(preterrain1.prearea.colormap_resolution - 1);
					preterrain1.splatmap_resolution = (float)preterrain1.prearea.colormap_resolution;
					prearea.conversion_step = prearea.step;
					prearea.resolution = (float)preterrain1.prearea.colormap_resolution;
				}
				else if (prearea.resolution_mode == resolution_mode_enum.Splatmap)
				{
					prearea.step.x = preterrain1.terrain.terrainData.size.x / (float)preterrain1.terrain.terrainData.alphamapResolution;
					prearea.step.y = preterrain1.terrain.terrainData.size.z / (float)preterrain1.terrain.terrainData.alphamapResolution;
					prearea.conversion_step = prearea.step;
					prearea.resolution = preterrain1.splatmap_resolution;
				}
				else if (prearea.resolution_mode == resolution_mode_enum.Detailmap)
				{
					prearea.step.x = preterrain1.terrain.terrainData.size.x / (float)preterrain1.terrain.terrainData.detailResolution;
					prearea.step.y = preterrain1.terrain.terrainData.size.z / (float)preterrain1.terrain.terrainData.detailResolution;
					prearea.conversion_step = prearea.step;
					prearea.resolution = preterrain1.detail_resolution;
				}
				else if (prearea.resolution_mode == resolution_mode_enum.Tree)
				{
					prearea.step.x = preterrain1.terrain.terrainData.size.x / (float)prearea.tree_resolution;
					prearea.step.y = preterrain1.terrain.terrainData.size.z / (float)prearea.tree_resolution;
					prearea.conversion_step = prearea.step;
					prearea.resolution = (float)prearea.tree_resolution;
				}
				else if (prearea.resolution_mode == resolution_mode_enum.Object)
				{
					prearea.step.x = preterrain1.terrain.terrainData.size.x / (float)prearea.object_resolution;
					prearea.step.y = preterrain1.terrain.terrainData.size.z / (float)prearea.object_resolution;
					prearea.conversion_step = prearea.step;
					prearea.resolution = (float)prearea.object_resolution;
				}
				else if (prearea.resolution_mode == resolution_mode_enum.Units)
				{
					prearea.step.x = (float)1;
					prearea.step.y = (float)1;
					prearea.conversion_step = prearea.step;
					prearea.resolution = preterrain1.terrain.terrainData.size.x;
				}
				else if (prearea.resolution_mode == resolution_mode_enum.Custom)
				{
					prearea.resolution = preterrain1.terrain.terrainData.size.x / prearea.step.x;
					prearea.conversion_step = prearea.step;
				}
			}
		}
	}

	public override void get_terrain_parameter_settings(terrain_class preterrain1)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.terrains[i].settings_editor = preterrain1.settings_editor;
			this.terrains[i].settings_runtime = preterrain1.settings_runtime;
			if (this.terrains[i].terrain)
			{
				if (this.terrains[i].terrain.terrainData)
				{
					if (this.terrains[i].settings_editor)
					{
						this.terrains[i].heightmapPixelError = this.terrains[i].terrain.heightmapPixelError;
						this.terrains[i].heightmapMaximumLOD = this.terrains[i].terrain.heightmapMaximumLOD;
						this.terrains[i].basemapDistance = this.terrains[i].terrain.basemapDistance;
						this.terrains[i].castShadows = this.terrains[i].terrain.castShadows;
						this.terrains[i].draw = this.terrains[i].editor_draw;
						this.terrains[i].treeDistance = this.terrains[i].terrain.treeDistance;
						this.terrains[i].detailObjectDistance = this.terrains[i].terrain.detailObjectDistance;
						this.terrains[i].detailObjectDensity = this.terrains[i].terrain.detailObjectDensity;
						this.terrains[i].treeBillboardDistance = this.terrains[i].terrain.treeBillboardDistance;
						this.terrains[i].treeCrossFadeLength = this.terrains[i].terrain.treeCrossFadeLength;
						this.terrains[i].treeMaximumFullLODCount = this.terrains[i].terrain.treeMaximumFullLODCount;
					}
					else
					{
						this.terrains[i].script_terrainDetail = (TerrainDetail)this.terrains[i].terrain.gameObject.GetComponent(typeof(TerrainDetail));
						if (!this.terrains[i].script_terrainDetail)
						{
							this.terrains[i].script_terrainDetail = (TerrainDetail)this.terrains[i].terrain.gameObject.AddComponent(typeof(TerrainDetail));
						}
						this.terrains[i].heightmapPixelError = this.terrains[i].script_terrainDetail.heightmapPixelError;
						this.terrains[i].heightmapMaximumLOD = this.terrains[i].script_terrainDetail.heightmapMaximumLOD;
						this.terrains[i].basemapDistance = this.terrains[i].script_terrainDetail.basemapDistance;
						this.terrains[i].castShadows = this.terrains[i].script_terrainDetail.castShadows;
						this.terrains[i].draw = this.terrains[i].script_terrainDetail.draw;
						this.terrains[i].treeDistance = this.terrains[i].script_terrainDetail.treeDistance;
						this.terrains[i].detailObjectDistance = this.terrains[i].script_terrainDetail.detailObjectDistance;
						this.terrains[i].detailObjectDensity = this.terrains[i].script_terrainDetail.detailObjectDensity;
						this.terrains[i].treeBillboardDistance = this.terrains[i].script_terrainDetail.treeBillboardDistance;
						this.terrains[i].treeCrossFadeLength = this.terrains[i].script_terrainDetail.treeCrossFadeLength;
						this.terrains[i].treeMaximumFullLODCount = this.terrains[i].script_terrainDetail.treeMaximumFullLODCount;
					}
				}
			}
		}
	}

	public override void set_terrain_parameters(terrain_class preterrain1, terrain_class preterrain2)
	{
		preterrain1.terrain.heightmapPixelError = preterrain2.heightmapPixelError;
		preterrain1.terrain.heightmapMaximumLOD = preterrain2.heightmapMaximumLOD;
		preterrain1.terrain.basemapDistance = preterrain2.basemapDistance;
		preterrain1.terrain.castShadows = preterrain2.castShadows;
		preterrain1.terrain.treeDistance = preterrain2.treeDistance;
		preterrain1.terrain.detailObjectDistance = preterrain2.detailObjectDistance;
		preterrain1.terrain.detailObjectDensity = preterrain2.detailObjectDensity;
		preterrain1.terrain.treeBillboardDistance = preterrain2.treeBillboardDistance;
		preterrain1.terrain.treeCrossFadeLength = preterrain2.treeCrossFadeLength;
		preterrain1.terrain.treeMaximumFullLODCount = preterrain2.treeMaximumFullLODCount;
	}

	public override void set_terrain_pixelerror(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.heightmapPixelError = preterrain1.heightmapPixelError;
				}
				else
				{
					preterrain1.script_terrainDetail.heightmapPixelError = preterrain1.heightmapPixelError;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].heightmapPixelError = preterrain1.heightmapPixelError;
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.heightmapPixelError = preterrain1.heightmapPixelError;
					}
					else
					{
						this.terrains[i].script_terrainDetail.heightmapPixelError = preterrain1.heightmapPixelError;
					}
				}
			}
		}
	}

	public override void set_terrain_heightmap_lod(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.heightmapMaximumLOD = preterrain1.heightmapMaximumLOD;
				}
				else
				{
					preterrain1.script_terrainDetail.heightmapMaximumLOD = preterrain1.heightmapMaximumLOD;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].heightmapMaximumLOD = preterrain1.heightmapMaximumLOD;
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.heightmapMaximumLOD = preterrain1.heightmapMaximumLOD;
					}
					else
					{
						this.terrains[i].script_terrainDetail.heightmapMaximumLOD = preterrain1.heightmapMaximumLOD;
					}
				}
			}
		}
	}

	public override void set_terrain_draw(terrain_class preterrain1, bool all_terrain, bool draw)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (draw)
				{
					if (preterrain1.settings_editor)
					{
						preterrain1.terrain.detailObjectDistance = preterrain1.detailObjectDistance;
						preterrain1.terrain.treeDistance = preterrain1.treeDistance;
						preterrain1.editor_draw = true;
					}
					else
					{
						preterrain1.script_terrainDetail.draw = true;
					}
				}
				else if (preterrain1.settings_editor)
				{
					preterrain1.terrain.detailObjectDistance = (float)0;
					preterrain1.terrain.treeDistance = (float)0;
					preterrain1.editor_draw = false;
				}
				else
				{
					preterrain1.script_terrainDetail.draw = false;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					if (draw)
					{
						if (preterrain1.settings_editor)
						{
							this.terrains[i].terrain.detailObjectDistance = preterrain1.detailObjectDistance;
							this.terrains[i].terrain.treeDistance = preterrain1.treeDistance;
							this.terrains[i].editor_draw = true;
						}
						else
						{
							this.terrains[i].script_terrainDetail.draw = true;
						}
					}
					else if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.detailObjectDistance = (float)0;
						this.terrains[i].terrain.treeDistance = (float)0;
						this.terrains[i].editor_draw = false;
					}
					else
					{
						this.terrains[i].script_terrainDetail.draw = false;
					}
				}
			}
		}
	}

	public override void set_terrain_basemap_distance(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.basemapDistance = preterrain1.basemapDistance;
				}
				else
				{
					preterrain1.script_terrainDetail.basemapDistance = preterrain1.basemapDistance;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].basemapDistance = preterrain1.basemapDistance;
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.basemapDistance = preterrain1.basemapDistance;
					}
					else
					{
						this.terrains[i].script_terrainDetail.basemapDistance = preterrain1.basemapDistance;
					}
				}
			}
		}
	}

	public override void set_terrain_detail_distance(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.detailObjectDistance = preterrain1.detailObjectDistance;
				}
				else
				{
					preterrain1.script_terrainDetail.detailObjectDistance = preterrain1.detailObjectDistance;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				this.terrains[i].detailObjectDistance = preterrain1.detailObjectDistance;
				if (this.terrains[i].terrain)
				{
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.detailObjectDistance = preterrain1.detailObjectDistance;
					}
					else
					{
						this.terrains[i].script_terrainDetail.detailObjectDistance = preterrain1.detailObjectDistance;
					}
				}
			}
		}
	}

	public override void set_terrain_detail_density(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.detailObjectDensity = preterrain1.detailObjectDensity;
				}
				else
				{
					preterrain1.script_terrainDetail.detailObjectDensity = preterrain1.detailObjectDensity;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].detailObjectDensity = preterrain1.detailObjectDensity;
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.detailObjectDensity = preterrain1.detailObjectDensity;
					}
					else
					{
						this.terrains[i].script_terrainDetail.detailObjectDensity = preterrain1.detailObjectDensity;
					}
				}
			}
		}
	}

	public override void set_terrain_tree_distance(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.treeDistance = preterrain1.treeDistance;
				}
				else
				{
					preterrain1.script_terrainDetail.treeDistance = preterrain1.treeDistance;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				this.terrains[i].treeDistance = preterrain1.treeDistance;
				if (this.terrains[i].terrain)
				{
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.treeDistance = preterrain1.treeDistance;
					}
					else
					{
						this.terrains[i].script_terrainDetail.treeDistance = preterrain1.treeDistance;
					}
				}
			}
		}
	}

	public override void set_terrain_tree_billboard_distance(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.treeBillboardDistance = preterrain1.treeBillboardDistance;
				}
				else
				{
					preterrain1.script_terrainDetail.treeBillboardDistance = preterrain1.treeBillboardDistance;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				this.terrains[i].treeBillboardDistance = preterrain1.treeBillboardDistance;
				if (this.terrains[i].terrain)
				{
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.treeBillboardDistance = preterrain1.treeBillboardDistance;
					}
					else
					{
						this.terrains[i].script_terrainDetail.treeBillboardDistance = preterrain1.treeBillboardDistance;
					}
				}
			}
		}
	}

	public override void set_terrain_tree_billboard_fade_length(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.treeCrossFadeLength = preterrain1.treeCrossFadeLength;
				}
				else
				{
					preterrain1.script_terrainDetail.treeCrossFadeLength = preterrain1.treeCrossFadeLength;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].treeCrossFadeLength = preterrain1.treeCrossFadeLength;
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.treeCrossFadeLength = preterrain1.treeCrossFadeLength;
					}
					else
					{
						this.terrains[i].script_terrainDetail.treeCrossFadeLength = preterrain1.treeCrossFadeLength;
					}
				}
			}
		}
	}

	public override void set_terrain_tree_max_mesh(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.treeMaximumFullLODCount = preterrain1.treeMaximumFullLODCount;
				}
				else
				{
					preterrain1.script_terrainDetail.treeMaximumFullLODCount = preterrain1.treeMaximumFullLODCount;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				this.terrains[i].treeMaximumFullLODCount = preterrain1.treeMaximumFullLODCount;
				if (this.terrains[i].terrain)
				{
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.treeMaximumFullLODCount = preterrain1.treeMaximumFullLODCount;
					}
					else
					{
						this.terrains[i].script_terrainDetail.treeMaximumFullLODCount = preterrain1.treeMaximumFullLODCount;
					}
				}
			}
		}
	}

	public override void set_terrain_shadow(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				if (preterrain1.settings_editor)
				{
					preterrain1.terrain.castShadows = preterrain1.castShadows;
				}
				else
				{
					preterrain1.script_terrainDetail.castShadows = preterrain1.castShadows;
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				this.terrains[i].castShadows = preterrain1.castShadows;
				if (this.terrains[i].terrain)
				{
					if (preterrain1.settings_editor)
					{
						this.terrains[i].terrain.castShadows = preterrain1.castShadows;
					}
					else
					{
						this.terrains[i].script_terrainDetail.castShadows = preterrain1.castShadows;
					}
				}
			}
		}
	}

	public override void set_terrain_material(terrain_class preterrain1, bool all_terrain)
	{
		if (all_terrain)
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].terrain.materialTemplate = preterrain1.terrain.materialTemplate;
				}
			}
		}
	}

	public override void set_terrain_wind_speed(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				preterrain1.terrain.terrainData.wavingGrassSpeed = preterrain1.wavingGrassSpeed;
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].terrain.terrainData.wavingGrassSpeed = preterrain1.wavingGrassSpeed;
					this.terrains[i].wavingGrassSpeed = preterrain1.wavingGrassSpeed;
				}
			}
		}
	}

	public override void set_terrain_wind_amount(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				preterrain1.terrain.terrainData.wavingGrassAmount = preterrain1.wavingGrassAmount;
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].terrain.terrainData.wavingGrassAmount = preterrain1.wavingGrassAmount;
					this.terrains[i].wavingGrassAmount = preterrain1.wavingGrassAmount;
				}
			}
		}
	}

	public override void set_terrain_wind_bending(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				preterrain1.terrain.terrainData.wavingGrassStrength = preterrain1.wavingGrassStrength;
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].terrain.terrainData.wavingGrassStrength = preterrain1.wavingGrassStrength;
					this.terrains[i].wavingGrassStrength = preterrain1.wavingGrassStrength;
				}
			}
		}
	}

	public override void set_terrain_grass_tint(terrain_class preterrain1, bool all_terrain)
	{
		if (!all_terrain)
		{
			if (preterrain1.terrain)
			{
				preterrain1.terrain.terrainData.wavingGrassTint = preterrain1.wavingGrassTint;
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].terrain)
				{
					this.terrains[i].terrain.terrainData.wavingGrassTint = preterrain1.wavingGrassTint;
					this.terrains[i].wavingGrassTint = preterrain1.wavingGrassTint;
				}
			}
		}
	}

	public override void set_terrain_settings(terrain_class preterrain1, string command)
	{
		if (preterrain1.terrain)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (command.IndexOf("(siz)") != -1)
			{
				flag = true;
			}
			if (command.IndexOf("(res)") != -1)
			{
				flag3 = true;
			}
			if (command.IndexOf("(all)") != -1)
			{
				flag2 = true;
			}
			if (flag || flag2)
			{
				Vector3 size = preterrain1.terrain.terrainData.size;
				if (preterrain1.terrain.terrainData.size != preterrain1.size)
				{
					preterrain1.terrain.terrainData.size = preterrain1.size;
					Vector2 vector = default(Vector2);
					vector.x = preterrain1.size.x / size.x;
					vector.y = preterrain1.size.z / size.z;
					preterrain1.prearea.area_max.xMin = (float)0;
					preterrain1.prearea.area_max.yMin = (float)0;
					preterrain1.prearea.area_max.xMax = preterrain1.terrain.terrainData.size.x;
					preterrain1.prearea.area_max.yMax = preterrain1.terrain.terrainData.size.z;
					preterrain1.prearea.area.xMin = preterrain1.prearea.area.xMin * vector.x;
					preterrain1.prearea.area.xMax = preterrain1.prearea.area.xMax * vector.x;
					preterrain1.prearea.area.yMin = preterrain1.prearea.area.yMin * vector.y;
					preterrain1.prearea.area.yMax = preterrain1.prearea.area.yMax * vector.y;
				}
			}
			if (flag3 || flag2)
			{
				if ((float)preterrain1.terrain.terrainData.heightmapResolution != preterrain1.heightmap_resolution)
				{
					preterrain1.terrain.terrainData.heightmapResolution = (int)preterrain1.heightmap_resolution;
					preterrain1.terrain.terrainData.size = preterrain1.size;
				}
				if ((float)preterrain1.terrain.terrainData.alphamapResolution != preterrain1.splatmap_resolution)
				{
					preterrain1.terrain.terrainData.alphamapResolution = (int)preterrain1.splatmap_resolution;
				}
				if ((float)preterrain1.terrain.terrainData.baseMapResolution != preterrain1.basemap_resolution)
				{
					preterrain1.terrain.terrainData.baseMapResolution = (int)preterrain1.basemap_resolution;
				}
				preterrain1.terrain.terrainData.SetDetailResolution((int)preterrain1.detail_resolution, (int)preterrain1.detail_resolution_per_patch);
			}
			this.get_terrain_settings(preterrain1, "(con)" + command);
		}
	}

	public override void set_all_terrain_area(terrain_class preterrain1)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.terrains[i].prearea.resolution_mode = preterrain1.prearea.resolution_mode;
			this.terrains[i].prearea.area = preterrain1.prearea.area;
			this.terrains[i].prearea.step = preterrain1.prearea.step;
			this.terrains[i].prearea.tree_resolution = preterrain1.prearea.tree_resolution;
			this.terrains[i].prearea.object_resolution = preterrain1.prearea.object_resolution;
			this.terrains[i].prearea.colormap_resolution = preterrain1.prearea.colormap_resolution;
			this.terrains[i].prearea.set_resolution_mode_text();
			this.terrains[i].color_terrain = new Color(0.5f, (float)1, 0.5f);
		}
	}

	public override void set_all_terrain_settings(terrain_class preterrain1, string command)
	{
		bool flag = false;
		bool flag2 = false;
		if (command.IndexOf("(siz)") != -1)
		{
			flag = true;
		}
		if (command.IndexOf("(res)") != -1)
		{
			flag2 = true;
		}
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (flag)
			{
				this.terrains[i].size = preterrain1.size;
			}
			if (flag2)
			{
				this.terrains[i].heightmap_resolution = preterrain1.heightmap_resolution;
				this.terrains[i].splatmap_resolution = preterrain1.splatmap_resolution;
				this.terrains[i].detail_resolution = preterrain1.detail_resolution;
				this.terrains[i].basemap_resolution = preterrain1.basemap_resolution;
				this.terrains[i].detail_resolution_per_patch = preterrain1.detail_resolution_per_patch;
			}
			this.terrains[i].color_terrain = new Color(0.5f, (float)1, 0.5f);
			this.set_terrain_settings(this.terrains[i], command);
			this.get_terrain_settings(this.terrains[i], command);
		}
	}

	public override void object_apply()
	{
		if (this.object_output)
		{
			if (this.placedObjects.Count > 0)
			{
				for (int i = 0; i < this.placedObjects.Count; i++)
				{
					this.placedObjects[i].SetActive(true);
				}
				this.placedObjects.Clear();
			}
		}
	}

	public override void terrain_apply(terrain_class preterrain1)
	{
		if ((this.runtime || this.settings.direct_colormap) && this.color_output)
		{
			preterrain1.ColorGlobal.Apply();
		}
		if (this.splat_output || (this.color_output && !this.button_export && !this.settings.direct_colormap))
		{
			for (int i = 0; i < preterrain1.splat_alpha.Length; i++)
			{
				preterrain1.splat_alpha[i].Apply();
			}
			preterrain1.terrain.terrainData.SetAlphamaps(0, 0, preterrain1.terrain.terrainData.GetAlphamaps(0, 0, 1, 1));
		}
		if (this.grass_output)
		{
			for (int j = 0; j < preterrain1.grass.Length; j++)
			{
				preterrain1.terrain.terrainData.SetDetailLayer(0, 0, j, this.grass_detail[j].detail);
				int num = 0;
				while ((float)num < preterrain1.detail_resolution)
				{
					int num2 = 0;
					while ((float)num2 < preterrain1.detail_resolution)
					{
						this.grass_detail[j].detail[num2, num] = 0;
						num2++;
					}
					num++;
				}
			}
		}
		if (this.heightmap_output)
		{
			preterrain1.terrain.terrainData.SetHeights(0, 0, this.heights);
			if (this.smooth_command)
			{
				this.smooth_terrain(preterrain1, this.smooth_tool_layer_strength);
			}
		}
		if (this.tree_output)
		{
			preterrain1.terrain.terrainData.treeInstances = new TreeInstance[this.tree_instances.Count];
			preterrain1.terrain.terrainData.treeInstances = this.tree_instances.ToArray();
		}
		preterrain1.terrain.Flush();
	}

	public override float get_terrain_alpha(terrain_class preterrain1, int local_x, int local_y, int alpha_index)
	{
		int num = alpha_index / 4;
		return preterrain1.splat_alpha[num].GetPixel(local_x, local_y)[alpha_index - num * 4];
	}

	public override void set_all_tree_filters(tree_output_class tree_output, int tree_number, bool all)
	{
		for (int i = 0; i < tree_output.tree.Count; i++)
		{
			if (tree_output.tree_value.active[i] || all)
			{
				if (i != tree_number)
				{
					this.erase_filters(tree_output.tree[i].prefilter);
					tree_output.tree[i].prefilter = this.copy_prefilter(tree_output.tree[tree_number].prefilter);
				}
				if (tree_output.tree[i].color_tree[0] < 1.5f)
				{
					tree_output.tree[i].color_tree = tree_output.tree[i].color_tree + new Color(0.5f, 0.5f, 0.5f, 0.5f);
				}
			}
		}
	}

	public override void set_all_tree_precolor_range(tree_output_class tree_output, int tree_number, bool all)
	{
		for (int i = 0; i < tree_output.tree.Count; i++)
		{
			if (tree_output.tree_value.active[i] || all)
			{
				if (i != tree_number)
				{
					tree_output.tree[i].precolor_range = this.copy_precolor_range(tree_output.tree[tree_number].precolor_range);
				}
				if (tree_output.tree[i].color_tree[0] < 1.5f)
				{
					tree_output.tree[i].color_tree = tree_output.tree[i].color_tree + new Color(0.5f, 0.5f, 0.5f, 0.5f);
				}
			}
		}
	}

	public override bool set_auto_object(object_output_class object_output)
	{
		bool arg_EC_0;
		if (!object_output.search_object)
		{
			arg_EC_0 = false;
		}
		else
		{
			Transform[] componentsInChildren = object_output.search_object.GetComponentsInChildren<Transform>();
			this.add_object(object_output, object_output.@object.Count);
			object_output.@object[object_output.@object.Count - 1].object1 = componentsInChildren[1].gameObject;
			string name = componentsInChildren[1].name;
			for (int i = 2; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].name != name)
				{
					this.add_object(object_output, object_output.@object.Count);
					object_output.@object[object_output.@object.Count - 1].object1 = componentsInChildren[i].gameObject;
					name = componentsInChildren[i].name;
				}
				else if (object_output.search_erase_doubles)
				{
					UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
				}
			}
			arg_EC_0 = true;
		}
		return arg_EC_0;
	}

	public override void create_terrain(terrain_class preterrain1, int length, int name_number)
	{
		for (int i = 0; i < length; i++)
		{
			TerrainData terrainData = new TerrainData();
			terrainData.heightmapResolution = (int)preterrain1.heightmap_resolution;
			terrainData.baseMapResolution = (int)preterrain1.basemap_resolution;
			terrainData.alphamapResolution = (int)preterrain1.splatmap_resolution;
			terrainData.SetDetailResolution((int)preterrain1.detail_resolution, (int)preterrain1.detail_resolution_per_patch);
			if (preterrain1.size.x == (float)0)
			{
				preterrain1.size.x = (float)1000;
			}
			if (preterrain1.size.y == (float)0)
			{
				preterrain1.size.y = (float)500;
			}
			if (preterrain1.size.z == (float)0)
			{
				preterrain1.size.z = (float)1000;
			}
			terrainData.size = preterrain1.size;
			GameObject gameObject = Terrain.CreateTerrainGameObject(terrainData);
			if (preterrain1.parent)
			{
				gameObject.transform.parent = preterrain1.parent;
			}
			Terrain terrain = (Terrain)gameObject.GetComponent(typeof(Terrain));
			terrain.name = this.terrain_scene_name + (i + name_number);
			if (this.terrains.Count < i + name_number)
			{
				this.set_terrain_length(this.terrains.Count + 1);
			}
			this.terrains[i + name_number - 1].terrain = terrain;
			if (i != 0)
			{
				this.set_terrain_parameters(this.terrains[i + name_number - 1], this.terrains[name_number - 1]);
			}
			else
			{
				this.set_terrain_parameters(this.terrains[i + name_number - 1], this.terrains[i + name_number - 1]);
			}
			this.get_terrain_settings(this.terrains[i + name_number - 1], "(res)(con)(fir)");
			this.terrains[i + name_number - 1].tile_x = (float)0;
			this.terrains[i + name_number - 1].tile_z = (float)0;
			this.terrains[i + name_number - 1].tiles = new Vector2((float)1, (float)1);
			this.terrains[i + name_number - 1].terrain.transform.position = new Vector3(-preterrain1.size.x / (float)2, (float)0, -preterrain1.size.z / (float)2);
			this.terrains[i + name_number - 1].prearea.max();
		}
		this.set_all_terrain_area(preterrain1);
		this.set_all_terrain_splat_textures(preterrain1, true, true);
		this.assign_all_terrain_splat_alpha();
		this.set_all_terrain_trees(preterrain1);
		this.set_all_terrain_details(preterrain1);
	}

	public override void assign_all_terrain_splat_alpha()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.assign_terrain_splat_alpha(this.terrains[i]);
		}
	}

	public override void assign_terrain_splat_alpha(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				if (preterrain1.terrain.terrainData.splatPrototypes.Length >= 1)
				{
					Type type = preterrain1.terrain.terrainData.GetType();
					PropertyInfo property = type.GetProperty("alphamapTextures", BindingFlags.Instance | BindingFlags.NonPublic);
					if (!RuntimeServices.EqualityOperator(property, null))
					{
						preterrain1.splat_alpha = (property.GetValue(preterrain1.terrain.terrainData, null) as Texture2D[]);
					}
					else
					{
						Debug.LogError("Can't access alphamapTexture directly...");
					}
				}
			}
		}
	}

	public override void randomize_layer_offset(layer_output_enum layer_output, Vector2 offset, int seed)
	{
		UnityEngine.Random.seed = seed;
		for (int i = 0; i < this.prelayers[0].layer.Count; i++)
		{
			if (this.prelayers[0].layer[i].output == layer_output)
			{
				this.prelayers[0].layer[i].offset = new Vector2(UnityEngine.Random.Range(offset.x, offset.y), UnityEngine.Random.Range(offset.x, offset.y));
				this.prelayers[0].layer[i].offset_middle = this.prelayers[0].layer[i].offset;
			}
		}
	}

	public override void set_auto_terrain()
	{
		Terrain[] array = (Terrain[])UnityEngine.Object.FindObjectsOfType(typeof(Terrain));
		if (array.Length > 0)
		{
			this.terrains.Clear();
			int i = 0;
			for (i = 0; i < array.Length; i++)
			{
				string name = array[i].name;
				string s = Regex.Replace(name, "[^0-9]", string.Empty);
				float num = (float)0;
				if (float.TryParse(s, out num))
				{
					this.terrains.Add(new terrain_class());
					this.terrains[this.terrains.Count - 1].terrain = array[i];
					this.terrains[this.terrains.Count - 1].name = this.terrains[this.terrains.Count - 1].terrain.name;
					this.terrains[this.terrains.Count - 1].index = this.terrains.Count - 1;
					this.get_terrain_settings(this.terrains[this.terrains.Count - 1], "(all)(fir)(spl)(tre)");
				}
			}
		}
		this.set_smooth_tool_terrain_popup();
		this.set_terrain_text();
	}

	public override void set_auto_mesh()
	{
		MeshFilter[] array;
		if (this.object_search == null)
		{
			array = (MeshFilter[])UnityEngine.Object.FindObjectsOfType(typeof(MeshFilter));
		}
		else
		{
			array = this.object_search.GetComponentsInChildren<MeshFilter>();
		}
		if (!RuntimeServices.EqualityOperator(array, null) && array.Length > 0)
		{
			this.meshes.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i].gameObject.layer & this.meshes_layer) == this.meshes_layer)
				{
					this.meshes.Add(new mesh_class());
					this.meshes[this.meshes.Count - 1].gameObject = array[i].gameObject;
					this.meshes[this.meshes.Count - 1].transform = array[i].transform;
					this.meshes[this.meshes.Count - 1].collider = (MeshCollider)array[i].GetComponent(typeof(MeshCollider));
					this.meshes[this.meshes.Count - 1].meshFilter = (MeshFilter)array[i].GetComponent(typeof(MeshFilter));
					this.meshes[this.meshes.Count - 1].mesh = this.meshes[this.meshes.Count - 1].meshFilter.sharedMesh;
				}
			}
		}
	}

	public override int get_rank_in_list(List<int> list, int number)
	{
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[number] > list[i])
			{
				num++;
			}
		}
		return num;
	}

	public override bool check_terrains_assigned()
	{
		int i = 0;
		int arg_64_0;
		while (i < this.terrains.Count)
		{
			if (!this.terrains[i].terrain)
			{
				arg_64_0 = 0;
			}
			else
			{
				if (this.terrains[i].terrain.terrainData)
				{
					i++;
					continue;
				}
				arg_64_0 = 0;
			}
			return arg_64_0 != 0;
		}
		arg_64_0 = 1;
		return arg_64_0 != 0;
	}

	public override bool find_mesh()
	{
		return this.prelayer.count_terrain < this.meshes.Count;
	}

	public override bool find_terrain(bool first)
	{
		ulong num = 0uL;
		int arg_4A8_0;
		for (int i = this.prelayer.count_terrain; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].active)
			{
				this.tree_instances.Clear();
				for (int j = 0; j < this.settings.treemap.Count; j++)
				{
					if (this.tree_output && this.settings.treemap[j].load && this.settings.treemap[j].map)
					{
						this.load_tree(j, i);
					}
				}
				if ((this.runtime || this.settings.direct_colormap) && this.color_output && this.terrains[i].rtp_script)
				{
					Type type = this.terrains[i].rtp_script.GetType();
					FieldInfo field = type.GetField("ColorGlobal");
					this.terrains[i].ColorGlobal = (field.GetValue(this.terrains[i].rtp_script) as Texture2D);
				}
				if (this.object_output && this.settings.parentObjectsTerrain)
				{
					this.terrains[i].objectParent = new GameObject();
					this.terrains[i].objectParent.transform.parent = this.terrains[i].terrain.transform;
					this.terrains[i].objectParent.name = "Objects";
				}
				if (!first)
				{
					if (this.heightmap_output)
					{
						num = (ulong)(this.terrains[i].heightmap_resolution * this.terrains[i].heightmap_resolution);
						if ((long)num != (long)this.heights.Length)
						{
							this.heights = (float[,])System.Array.CreateInstance(typeof(float), new int[]
							{
								(int)this.terrains[i].heightmap_resolution,
								(int)this.terrains[i].heightmap_resolution
							});
						}
					}
					if (this.grass_output)
					{
						if (this.terrains[i].terrain.terrainData.detailPrototypes.Length > this.grass_detail.Length)
						{
							this.grass_detail = new detail_class[this.terrains[i].terrain.terrainData.detailPrototypes.Length];
						}
						for (int k = 0; k < this.grass_detail.Length; k++)
						{
							if (this.grass_detail[k] == null)
							{
								this.grass_detail[k] = new detail_class();
							}
							if (this.grass_detail[k].detail != null)
							{
								if (this.terrains[i].detail_resolution * this.terrains[i].detail_resolution != (float)this.grass_detail[k].detail.Length)
								{
									this.grass_detail[k].detail = (int[,])System.Array.CreateInstance(typeof(int), new int[]
									{
										(int)this.terrains[i].detail_resolution,
										(int)this.terrains[i].detail_resolution
									});
								}
							}
							else
							{
								this.grass_detail[k].detail = (int[,])System.Array.CreateInstance(typeof(int), new int[]
								{
									(int)this.terrains[i].detail_resolution,
									(int)this.terrains[i].detail_resolution
								});
							}
						}
					}
				}
				for (int l = 0; l < this.settings.grassmap.Count; l++)
				{
					if (this.grass_output && this.settings.grassmap[l].load && this.settings.grassmap[l].map)
					{
						this.load_grass(l, i);
					}
				}
				if (this.prelayer.count_terrain > 0)
				{
					this.set_image_terrain_mode(i);
				}
				arg_4A8_0 = 1;
				return arg_4A8_0 != 0;
			}
			this.prelayer.count_terrain = this.prelayer.count_terrain + 1;
		}
		arg_4A8_0 = 0;
		return arg_4A8_0 != 0;
	}

	public override void load_tree(int treemap_index, int terrain_index)
	{
		this.tree_script = (this.settings.treemap[treemap_index].map.GetComponent("save_trees") as save_trees);
		if (this.tree_script.tree_save.Count - 1 >= terrain_index)
		{
			int count = this.tree_script.tree_save[terrain_index].treeInstances.Count;
			TreeInstance item = default(TreeInstance);
			int index = 0;
			Vector3 size = this.terrains[terrain_index].terrain.terrainData.size;
			for (int i = 0; i < count; i++)
			{
				index = this.tree_script.tree_save[terrain_index].treeInstances[i].prototypeIndex;
				if (UnityEngine.Random.Range((float)0, 1f) <= this.settings.treemap[treemap_index].tree_param[index].density)
				{
					item.position = this.tree_script.tree_save[terrain_index].treeInstances[i].position;
					item.position.y = this.terrains[terrain_index].terrain.terrainData.GetInterpolatedHeight(item.position.x, item.position.z) / size.y;
					item.widthScale = this.tree_script.tree_save[terrain_index].treeInstances[i].widthScale * this.settings.treemap[treemap_index].tree_param[index].scale;
					item.heightScale = this.tree_script.tree_save[terrain_index].treeInstances[i].heightScale * this.settings.treemap[treemap_index].tree_param[index].scale;
					item.color = this.tree_script.tree_save[terrain_index].treeInstances[i].color;
					item.lightmapColor = this.tree_script.tree_save[terrain_index].treeInstances[i].lightmapColor;
					item.prototypeIndex = this.settings.treemap[treemap_index].tree_param[index].prototype;
					this.tree_instances.Add(item);
				}
			}
		}
	}

	public override void load_grass(int grassmap_index, int terrain_index)
	{
		this.grass_script = (this.settings.grassmap[grassmap_index].map.GetComponent("save_grass") as save_grass);
		if (this.grass_script.grass_save.Count - 1 >= terrain_index)
		{
			int num = this.grass_script.grass_save[terrain_index].resolution;
			int num2 = 0;
			int count = this.grass_script.grass_save[terrain_index].details.Count;
			float num3 = (float)num / this.terrains[terrain_index].detail_resolution * ((float)num / this.terrains[terrain_index].detail_resolution);
			int num4 = 0;
			for (int i = 0; i < count; i++)
			{
				if (i > this.terrains[terrain_index].terrain.terrainData.detailPrototypes.Length - 1)
				{
					break;
				}
				num4 = this.grass_detail[this.settings.grassmap[grassmap_index].grass_param[i].prototype].detail.Length;
				num2 = (int)this.terrains[terrain_index].detail_resolution;
				for (int j = 0; j < num4; j++)
				{
					this.grass_detail[this.settings.grassmap[grassmap_index].grass_param[i].prototype].detail[j - j / num2 * num2, j / num2] = (int)((float)this.grass_detail[this.settings.grassmap[grassmap_index].grass_param[i].prototype].detail[j - j / num2 * num2, j / num2] + (float)this.grass_script.grass_save[terrain_index].details[i].detail[(int)((float)j * num3)] * this.settings.grassmap[grassmap_index].grass_param[i].density);
				}
			}
		}
	}

	public override void assign_rtp(bool active, bool open_link)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				if (!active)
				{
					this.terrains[i].rtp_script = null;
				}
			}
		}
	}

	public override void assign_rtp_single(terrain_class terrain1)
	{
	}

	public override void center_terrain_position(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				preterrain1.terrain.transform.position = new Vector3(-preterrain1.terrain.terrainData.size.x / (float)2, (float)0, -preterrain1.terrain.terrainData.size.z / (float)2);
			}
		}
	}

	public override bool check_terrains_square()
	{
		float f = (float)this.terrains.Count;
		float num = Mathf.Round(Mathf.Sqrt(f));
		return num == Mathf.Sqrt(f);
	}

	public override int fit_terrain_tiles(terrain_class preterrain1, bool refit)
	{
		int arg_255_0;
		if (this.terrains.Count < 2)
		{
			this.center_terrain_position(this.terrains[0]);
			arg_255_0 = 1;
		}
		else if (!this.check_terrains_assigned())
		{
			arg_255_0 = -2;
		}
		else
		{
			Vector3 size = preterrain1.size;
			float f = (float)this.terrains.Count;
			float num = Mathf.Round(Mathf.Sqrt(f));
			if (num != Mathf.Sqrt(f))
			{
				this.reset_terrains_tiles(this);
				arg_255_0 = -3;
			}
			else
			{
				this.set_all_terrain_settings(preterrain1, "(siz)");
				for (float num2 = (float)0; num2 < num; num2 += (float)1)
				{
					for (float num3 = (float)0; num3 < num; num3 += (float)1)
					{
						float num4 = num2 * num + num3;
						if (num4 < (float)this.terrains.Count)
						{
							Vector3 vector = default(Vector3);
							Vector3 vector2 = default(Vector3);
							if (num == (float)2)
							{
								vector2.z = num - num3 - (float)2;
								vector.z = vector2.z * size.z;
								vector2.x = -num + num2 + (float)1;
								vector.x = vector2.x * size.x;
								vector.y = (float)0;
							}
							else
							{
								vector2.z = (num - num3 * (float)2 - (float)2) / (float)2;
								vector.z = vector2.z * size.z;
								vector2.x = (-num + num2 * (float)2) / (float)2;
								vector.x = vector2.x * size.x;
								vector.y = (float)0;
							}
							if (refit)
							{
								this.terrains[(int)num4].terrain.transform.position = vector;
							}
							this.terrains[(int)num4].tile_x = num2;
							this.terrains[(int)num4].tile_z = num - num3 - (float)1;
							this.terrains[(int)num4].tiles.x = num;
							this.terrains[(int)num4].tiles.y = num;
							this.terrains[(int)num4].color_terrain = new Color(0.5f, (float)1, 0.5f);
						}
					}
				}
				this.tile_resolution = (int)(num * preterrain1.size.x);
				this.set_neighbor(1);
				arg_255_0 = 1;
			}
		}
		return arg_255_0;
	}

	public override void set_neighbor(int mode)
	{
		int num = 0;
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				TerrainNeighbors terrainNeighbors = (TerrainNeighbors)this.terrains[i].terrain.GetComponent(typeof(TerrainNeighbors));
				if (mode == 1)
				{
					if (!terrainNeighbors)
					{
						terrainNeighbors = (TerrainNeighbors)this.terrains[i].terrain.gameObject.AddComponent(typeof(TerrainNeighbors));
					}
					terrainNeighbors.left = null;
					terrainNeighbors.top = null;
					terrainNeighbors.right = null;
					terrainNeighbors.bottom = null;
					num = this.search_tile((int)(this.terrains[i].tile_x - (float)1), (int)this.terrains[i].tile_z);
					if (num != -1)
					{
						terrainNeighbors.left = this.terrains[num].terrain;
					}
					num = this.search_tile((int)this.terrains[i].tile_x, (int)(this.terrains[i].tile_z + (float)1));
					if (num != -1)
					{
						terrainNeighbors.top = this.terrains[num].terrain;
					}
					num = this.search_tile((int)(this.terrains[i].tile_x + (float)1), (int)this.terrains[i].tile_z);
					if (num != -1)
					{
						terrainNeighbors.right = this.terrains[num].terrain;
					}
					num = this.search_tile((int)this.terrains[i].tile_x, (int)(this.terrains[i].tile_z - (float)1));
					if (num != -1)
					{
						terrainNeighbors.bottom = this.terrains[num].terrain;
					}
				}
				if (mode == -1 && terrainNeighbors)
				{
					UnityEngine.Object.DestroyImmediate(terrainNeighbors);
				}
			}
		}
	}

	public override void set_detail_script(int mode)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				TerrainDetail terrainDetail = (TerrainDetail)this.terrains[i].terrain.GetComponent(typeof(TerrainDetail));
				if (mode == 1 && !terrainDetail)
				{
					terrainDetail = (TerrainDetail)this.terrains[i].terrain.gameObject.AddComponent(typeof(TerrainDetail));
				}
				if (mode == -1 && terrainDetail)
				{
					UnityEngine.Object.DestroyImmediate(terrainDetail);
				}
			}
		}
	}

	public override int search_tile(int tile_x, int tile_z)
	{
		int arg_AD_0;
		if ((float)tile_x > this.terrains[0].tiles.x - (float)1 || tile_x < 0)
		{
			arg_AD_0 = -1;
		}
		else if ((float)tile_z > this.terrains[0].tiles.y - (float)1 || tile_z < 0)
		{
			arg_AD_0 = -1;
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].tile_x == (float)tile_x && this.terrains[i].tile_z == (float)tile_z)
				{
					arg_AD_0 = i;
					return arg_AD_0;
				}
			}
			arg_AD_0 = -1;
		}
		return arg_AD_0;
	}

	public override void set_all_trees_settings_terrain(terrain_class preterrain1, int tree_number)
	{
		for (int i = 0; i < preterrain1.treePrototypes.Count; i++)
		{
			preterrain1.treePrototypes[i].bendFactor = preterrain1.treePrototypes[tree_number].bendFactor;
		}
		if (preterrain1.color_terrain[0] < 1.5f)
		{
			preterrain1.color_terrain += new Color(0.5f, (float)1, 0.5f, 0.5f);
		}
	}

	public override void set_all_trees_settings_terrains(terrain_class preterrain1, int tree_number)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			for (int j = 0; j < preterrain1.treePrototypes.Count; j++)
			{
				if (this.terrains[i].treePrototypes.Count - 1 >= j)
				{
					this.terrains[i].treePrototypes[j].bendFactor = preterrain1.treePrototypes[tree_number].bendFactor;
				}
			}
			this.check_synchronous_terrain_trees(this.terrains[i]);
			if (this.terrains[i].color_terrain[0] < 1.5f)
			{
				this.terrains[i].color_terrain = this.terrains[i].color_terrain + new Color(0.5f, (float)1, 0.5f, 0.5f);
			}
		}
	}

	public override void set_all_terrain_splat_textures(terrain_class preterrain1, bool copy, bool flash)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				if (copy)
				{
					this.set_terrain_splat_textures(preterrain1, this.terrains[i]);
				}
				else
				{
					this.set_terrain_splat_textures(this.terrains[i], this.terrains[i]);
				}
				this.get_terrain_splat_textures(this.terrains[i]);
				if (flash && this.terrains[i].color_terrain[0] < 1.5f)
				{
					this.terrains[i].color_terrain = new Color(0.5f, (float)1, 0.5f);
				}
			}
		}
	}

	public override void set_all_terrain_color_textures(bool flash)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.set_terrain_color_textures(this.terrains[i]);
			if (flash && this.terrains[i].color_terrain[0] < 1.5f)
			{
				this.terrains[i].color_terrain = new Color(0.5f, (float)1, 0.5f);
			}
		}
	}

	public override void set_terrain_color_textures(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			object value;
			FieldInfo field2;
			FieldInfo field3;
			FieldInfo field4;
			Texture2D[] array;
			Texture2D[] array2;
			Texture2D[] array3;
			if (preterrain1.rtp_script)
			{
				Type type = preterrain1.rtp_script.GetType();
				FieldInfo field = type.GetField("globalSettingsHolder");
				value = field.GetValue(preterrain1.rtp_script);
				Type type2 = value.GetType();
				field2 = type2.GetField("splats");
				field3 = type2.GetField("Bumps");
				field4 = type2.GetField("Heights");
				array = new Texture2D[preterrain1.splatPrototypes.Count];
				array2 = new Texture2D[preterrain1.splatPrototypes.Count];
				array3 = new Texture2D[preterrain1.splatPrototypes.Count];
			}
			List<SplatPrototype> list = new List<SplatPrototype>();
			for (int i = 0; i < this.settings.color_splatPrototypes.Length; i++)
			{
				list.Add(new SplatPrototype());
				if (preterrain1.rtp_script && this.settings.color_splatPrototypes[i].texture)
				{
					array[i] = this.settings.color_splatPrototypes[i].texture;
					array2[i] = null;
					array3[i] = null;
				}
				if (this.settings.color_splatPrototypes[i].texture)
				{
					list[i].texture = this.settings.color_splatPrototypes[i].texture;
					list[i].tileSize = this.settings.color_splatPrototypes[i].tileSize;
					list[i].tileOffset = this.settings.color_splatPrototypes[i].tileOffset;
				}
			}
			preterrain1.terrain.terrainData.splatPrototypes = list.ToArray();
			if (preterrain1.rtp_script)
			{
				field2.SetValue(value, array);
				field3.SetValue(value, array2);
				field4.SetValue(value, array3);
			}
		}
	}

	public override void set_terrain_splat_textures(terrain_class preterrain1, terrain_class preterrain2)
	{
		if (preterrain1.terrain)
		{
			object value;
			FieldInfo field2;
			FieldInfo field3;
			FieldInfo field4;
			Texture2D[] array;
			Texture2D[] array2;
			Texture2D[] array3;
			if (preterrain1.rtp_script)
			{
				Type type = preterrain1.rtp_script.GetType();
				FieldInfo field = type.GetField("globalSettingsHolder");
				value = field.GetValue(preterrain1.rtp_script);
				Type type2 = value.GetType();
				field2 = type2.GetField("splats");
				field3 = type2.GetField("Bumps");
				field4 = type2.GetField("Heights");
				array = new Texture2D[preterrain1.splatPrototypes.Count];
				array2 = new Texture2D[preterrain1.splatPrototypes.Count];
				array3 = new Texture2D[preterrain1.splatPrototypes.Count];
			}
			List<SplatPrototype> list = new List<SplatPrototype>();
			for (int i = 0; i < preterrain1.splatPrototypes.Count; i++)
			{
				if (preterrain1.splatPrototypes[i].texture)
				{
					list.Add(new SplatPrototype());
					if (this.settings.colormap && i == 0)
					{
						list[i].texture = preterrain2.splatPrototypes[i].texture;
						list[i].normalMap = preterrain2.splatPrototypes[i].normalMap;
						list[i].tileSize = preterrain2.splatPrototypes[i].tileSize;
						list[i].tileOffset = preterrain2.splatPrototypes[i].tileOffset;
					}
					else
					{
						list[i].texture = preterrain1.splatPrototypes[i].texture;
						list[i].normalMap = preterrain1.splatPrototypes[i].normalMap;
						list[i].tileSize = preterrain1.splatPrototypes[i].tileSize;
						list[i].tileOffset = preterrain1.splatPrototypes[i].tileOffset;
					}
				}
				else
				{
					preterrain1.splatPrototypes.RemoveAt(i);
					i--;
				}
				if (preterrain1.rtp_script)
				{
					array[i] = preterrain1.splatPrototypes[i].texture;
					array2[i] = preterrain1.splatPrototypes[i].normal_texture;
					array3[i] = preterrain1.splatPrototypes[i].height_texture;
				}
			}
			preterrain2.terrain.terrainData.splatPrototypes = list.ToArray();
			if (preterrain1.rtp_script)
			{
				field2.SetValue(value, array);
				field3.SetValue(value, array2);
				field4.SetValue(value, array3);
			}
		}
	}

	public override void set_colormap(bool active, bool all_parameters)
	{
		if (active)
		{
			float num = 0f;
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (all_parameters)
				{
					this.terrains[i].add_splatprototype(0);
				}
				if (this.terrains[i].terrain)
				{
					num = this.terrains[i].terrain.terrainData.size.x;
				}
				else
				{
					num = this.terrains[i].size.x;
				}
				if (this.terrains[i].splatPrototypes.Count > 0)
				{
					if (all_parameters)
					{
						this.terrains[i].splatPrototypes[0].texture = this.terrains[i].colormap.texture;
					}
					this.terrains[i].colormap.tileSize = new Vector2(num, num);
					this.terrains[i].splatPrototypes[0].tileSize = this.terrains[i].colormap.tileSize;
					this.terrains[i].splatPrototypes[0].tileOffset = this.terrains[i].colormap.tileOffset;
					if (this.terrains[i].splatPrototypes[0].texture)
					{
						this.set_terrain_splat_textures(this.terrains[i], this.terrains[i]);
					}
				}
			}
		}
		else
		{
			for (int i = 0; i < this.terrains.Count; i++)
			{
				if (this.terrains[i].splatPrototypes.Count > 0)
				{
					this.terrains[i].colormap.texture = this.terrains[i].splatPrototypes[0].texture;
					this.terrains[i].erase_splatprototype(0);
					this.set_terrain_splat_textures(this.terrains[i], this.terrains[i]);
				}
			}
		}
		this.loop_colormap(active);
	}

	public override void loop_colormap(bool active)
	{
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				if (this.prelayers[i].layer[j].output == layer_output_enum.splat)
				{
					for (int k = 0; k < this.prelayers[i].layer[j].splat_output.splat.Count; k++)
					{
						if (this.prelayers[i].layer[j].splat_output.splat[k] == 0)
						{
							if (!active)
							{
								this.prelayers[i].layer[j].splat_output.splat_value.active[k] = false;
							}
							else
							{
								this.prelayers[i].layer[j].splat_output.splat[k] = this.prelayers[i].layer[j].splat_output.splat[k] + 1;
								this.prelayers[i].layer[j].splat_output.splat_value.active[k] = true;
							}
						}
						else if (!active)
						{
							this.prelayers[i].layer[j].splat_output.splat[k] = this.prelayers[i].layer[j].splat_output.splat[k] - 1;
						}
						else
						{
							this.prelayers[i].layer[j].splat_output.splat[k] = this.prelayers[i].layer[j].splat_output.splat[k] + 1;
						}
					}
				}
			}
		}
	}

	public override void get_all_terrain_splat_textures()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.get_terrain_splat_textures(this.terrains[i]);
			this.check_synchronous_terrain_textures(this.terrains[i]);
		}
	}

	public override void get_terrain_splat_textures(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			Texture2D[] array;
			Texture2D[] array2;
			Texture2D[] array3;
			if (preterrain1.rtp_script)
			{
				Type type = preterrain1.rtp_script.GetType();
				FieldInfo field = type.GetField("globalSettingsHolder");
				object value = field.GetValue(preterrain1.rtp_script);
				Type type2 = value.GetType();
				FieldInfo field2 = type2.GetField("splats");
				array = (field2.GetValue(value) as Texture2D[]);
				field2 = type2.GetField("Bumps");
				array2 = (field2.GetValue(value) as Texture2D[]);
				field2 = type2.GetField("Heights");
				array3 = (field2.GetValue(value) as Texture2D[]);
			}
			for (int i = 0; i < preterrain1.terrain.terrainData.splatPrototypes.Length; i++)
			{
				if (preterrain1.splatPrototypes.Count - 1 < i)
				{
					preterrain1.splatPrototypes.Add(new splatPrototype_class());
				}
				preterrain1.splatPrototypes[i].tileSize = preterrain1.terrain.terrainData.splatPrototypes[i].tileSize;
				preterrain1.splatPrototypes[i].tileOffset = preterrain1.terrain.terrainData.splatPrototypes[i].tileOffset;
				if (!preterrain1.rtp_script)
				{
					preterrain1.splatPrototypes[i].texture = preterrain1.terrain.terrainData.splatPrototypes[i].texture;
					preterrain1.splatPrototypes[i].normalMap = preterrain1.terrain.terrainData.splatPrototypes[i].normalMap;
				}
				else
				{
					preterrain1.splatPrototypes[i].texture = array[i];
					preterrain1.splatPrototypes[i].normal_texture = array2[i];
					preterrain1.splatPrototypes[i].height_texture = array3[i];
				}
			}
			int num = preterrain1.splatPrototypes.Count - preterrain1.terrain.terrainData.splatPrototypes.Length;
			for (int i = 0; i < num; i++)
			{
				preterrain1.splatPrototypes.RemoveAt(preterrain1.splatPrototypes.Count - 1);
			}
		}
	}

	public override void check_synchronous_terrains_textures()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.check_synchronous_terrain_textures(this.terrains[i]);
		}
	}

	public override void check_synchronous_terrain_textures(terrain_class preterrain1)
	{
		this.check_synchronous_terrain_splat_textures(preterrain1);
	}

	public override void check_synchronous_terrains_splat_textures()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.check_synchronous_terrain_splat_textures(this.terrains[i]);
		}
	}

	public override void check_synchronous_terrain_splat_textures(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				Texture2D[] array;
				Texture2D[] array2;
				Texture2D[] array3;
				if (preterrain1.rtp_script)
				{
					Type type = preterrain1.rtp_script.GetType();
					FieldInfo field = type.GetField("globalSettingsHolder");
					object value = field.GetValue(preterrain1.rtp_script);
					Type type2 = value.GetType();
					FieldInfo field2 = type2.GetField("splats");
					array = (field2.GetValue(value) as Texture2D[]);
					field2 = type2.GetField("Bumps");
					array2 = (field2.GetValue(value) as Texture2D[]);
					field2 = type2.GetField("Heights");
					array3 = (field2.GetValue(value) as Texture2D[]);
				}
				bool splat_synchronous = true;
				if (preterrain1.splatPrototypes.Count != preterrain1.terrain.terrainData.splatPrototypes.Length)
				{
					splat_synchronous = false;
				}
				else
				{
					for (int i = 0; i < preterrain1.splatPrototypes.Count; i++)
					{
						if (!preterrain1.rtp_script)
						{
							if (preterrain1.splatPrototypes[i].texture != preterrain1.terrain.terrainData.splatPrototypes[i].texture)
							{
								splat_synchronous = false;
								break;
							}
							if (preterrain1.splatPrototypes[i].normalMap != preterrain1.terrain.terrainData.splatPrototypes[i].normalMap)
							{
								splat_synchronous = false;
								break;
							}
							if (preterrain1.splatPrototypes[i].tileOffset != preterrain1.terrain.terrainData.splatPrototypes[i].tileOffset)
							{
								splat_synchronous = false;
								break;
							}
						}
						else
						{
							if (preterrain1.splatPrototypes[i].texture != array[i])
							{
								splat_synchronous = false;
								break;
							}
							if (preterrain1.splatPrototypes[i].normal_texture != array2[i])
							{
								splat_synchronous = false;
								break;
							}
							if (preterrain1.splatPrototypes[i].height_texture != array3[i])
							{
								splat_synchronous = false;
								break;
							}
						}
					}
				}
				preterrain1.splat_synchronous = splat_synchronous;
			}
		}
	}

	public override void check_synchronous_terrains_color_textures()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.check_synchronous_terrain_color_textures(this.terrains[i]);
		}
	}

	public override void check_synchronous_terrain_color_textures(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				Texture2D[] array;
				if (preterrain1.rtp_script)
				{
					Type type = preterrain1.rtp_script.GetType();
					FieldInfo field = type.GetField("globalSettingsHolder");
					object value = field.GetValue(preterrain1.rtp_script);
					Type type2 = value.GetType();
					FieldInfo field2 = type2.GetField("splats");
					array = (field2.GetValue(value) as Texture2D[]);
					field2 = type2.GetField("Bumps");
					Texture2D[] array2 = field2.GetValue(value) as Texture2D[];
					field2 = type2.GetField("Heights");
					Texture2D[] array3 = field2.GetValue(value) as Texture2D[];
				}
				bool splat_synchronous = true;
				if (this.settings.color_splatPrototypes.Length != preterrain1.terrain.terrainData.splatPrototypes.Length)
				{
					splat_synchronous = false;
				}
				else
				{
					for (int i = 0; i < this.settings.color_splatPrototypes.Length; i++)
					{
						if (!preterrain1.rtp_script)
						{
							if (this.settings.color_splatPrototypes[i].texture != preterrain1.terrain.terrainData.splatPrototypes[i].texture)
							{
								splat_synchronous = false;
								break;
							}
							if (this.settings.color_splatPrototypes[i].tileOffset != preterrain1.terrain.terrainData.splatPrototypes[i].tileOffset)
							{
								splat_synchronous = false;
								break;
							}
						}
						else if (this.settings.color_splatPrototypes[i].texture != array[i])
						{
							splat_synchronous = false;
							break;
						}
					}
				}
				preterrain1.splat_synchronous = splat_synchronous;
			}
		}
	}

	public override void check_synchronous_terrain_size(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				bool size_synchronous = true;
				if (preterrain1.size.x != preterrain1.terrain.terrainData.size.x || preterrain1.size.y != preterrain1.terrain.terrainData.size.y || preterrain1.size.z != preterrain1.terrain.terrainData.size.z)
				{
					size_synchronous = false;
				}
				preterrain1.size_synchronous = size_synchronous;
			}
		}
	}

	public override void check_synchronous_terrain_resolutions(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				bool resolutions_synchronous = true;
				if (preterrain1.heightmap_resolution != (float)preterrain1.terrain.terrainData.heightmapResolution || preterrain1.splatmap_resolution != (float)preterrain1.terrain.terrainData.alphamapResolution || preterrain1.detail_resolution != (float)preterrain1.terrain.terrainData.detailResolution || preterrain1.basemap_resolution != (float)preterrain1.terrain.terrainData.baseMapResolution)
				{
					resolutions_synchronous = false;
				}
				preterrain1.resolutions_synchronous = resolutions_synchronous;
			}
		}
	}

	public override void copy_terrain_splat(splatPrototype_class splatPrototype1, splatPrototype_class splatPrototype2)
	{
		splatPrototype2.texture = splatPrototype1.texture;
		splatPrototype2.tileSize_old = splatPrototype1.tileSize_old;
		splatPrototype2.tileOffset = splatPrototype1.tileOffset;
	}

	public override void copy_terrain_splats(terrain_class preterrain1, terrain_class preterrain2)
	{
		for (int i = 0; i < preterrain1.splatPrototypes.Count; i++)
		{
			if (preterrain2.splatPrototypes.Count < preterrain1.splatPrototypes.Count)
			{
				preterrain2.splatPrototypes.Add(new splatPrototype_class());
			}
			this.copy_terrain_splat(preterrain1.splatPrototypes[i], preterrain2.splatPrototypes[i]);
		}
	}

	public override void swap_terrain_splat(terrain_class preterrain1, int splat_number1, int splat_number2)
	{
		if (splat_number2 > -1 && splat_number2 < preterrain1.splatPrototypes.Count)
		{
			splatPrototype_class value = preterrain1.splatPrototypes[splat_number1];
			preterrain1.splatPrototypes[splat_number1] = preterrain1.splatPrototypes[splat_number2];
			preterrain1.splatPrototypes[splat_number2] = value;
		}
	}

	public override void set_all_terrain_trees(terrain_class preterrain1)
	{
		this.set_terrain_trees(preterrain1);
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				this.terrains[i].terrain.terrainData.treePrototypes = preterrain1.terrain.terrainData.treePrototypes;
				this.get_terrain_trees(this.terrains[i]);
				this.terrains[i].color_terrain = new Color(0.5f, (float)1, 0.5f);
				this.check_synchronous_terrain_trees(this.terrains[i]);
			}
		}
	}

	public override void set_terrain_trees(terrain_class preterrain1)
	{
		List<TreePrototype> list = new List<TreePrototype>();
		for (int i = 0; i < preterrain1.treePrototypes.Count; i++)
		{
			if (preterrain1.treePrototypes[i].prefab)
			{
				list.Add(new TreePrototype());
				list[i].prefab = preterrain1.treePrototypes[i].prefab;
				list[i].bendFactor = preterrain1.treePrototypes[i].bendFactor;
			}
		}
		preterrain1.terrain.terrainData.treePrototypes = list.ToArray();
	}

	public override void get_all_terrain_trees()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.get_terrain_trees(this.terrains[i]);
			this.check_synchronous_terrain_trees(this.terrains[i]);
		}
	}

	public override void get_terrain_trees(terrain_class preterrain1)
	{
		preterrain1.treePrototypes.Clear();
		for (int i = 0; i < preterrain1.terrain.terrainData.treePrototypes.Length; i++)
		{
			preterrain1.treePrototypes.Add(new treePrototype_class());
			preterrain1.treePrototypes[i].prefab = preterrain1.terrain.terrainData.treePrototypes[i].prefab;
			preterrain1.treePrototypes[i].bendFactor = preterrain1.terrain.terrainData.treePrototypes[i].bendFactor;
		}
	}

	public override void check_synchronous_terrain_trees(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				bool tree_synchronous = true;
				if (preterrain1.treePrototypes.Count != preterrain1.terrain.terrainData.treePrototypes.Length)
				{
					tree_synchronous = false;
				}
				else
				{
					for (int i = 0; i < preterrain1.treePrototypes.Count; i++)
					{
						if (preterrain1.treePrototypes[i].prefab != preterrain1.terrain.terrainData.treePrototypes[i].prefab)
						{
							tree_synchronous = false;
							break;
						}
						if (preterrain1.treePrototypes[i].bendFactor != preterrain1.terrain.terrainData.treePrototypes[i].bendFactor)
						{
							tree_synchronous = false;
							break;
						}
					}
				}
				preterrain1.tree_synchronous = tree_synchronous;
			}
		}
	}

	public override void copy_terrain_tree(treePrototype_class treePrototype1, treePrototype_class treePrototype2)
	{
		treePrototype2.prefab = treePrototype1.prefab;
		treePrototype2.bendFactor = treePrototype1.bendFactor;
	}

	public override void copy_terrain_trees(terrain_class preterrain1, terrain_class preterrain2)
	{
		for (int i = 0; i < preterrain1.treePrototypes.Count; i++)
		{
			if (preterrain2.treePrototypes.Count < preterrain1.treePrototypes.Count)
			{
				preterrain2.treePrototypes.Add(new treePrototype_class());
			}
			this.copy_terrain_tree(preterrain1.treePrototypes[i], preterrain2.treePrototypes[i]);
		}
	}

	public override void swap_terrain_tree(terrain_class preterrain1, int tree_number1, int tree_number2)
	{
		if (tree_number2 > -1 && tree_number2 < preterrain1.treePrototypes.Count)
		{
			treePrototype_class value = preterrain1.treePrototypes[tree_number1];
			preterrain1.treePrototypes[tree_number1] = preterrain1.treePrototypes[tree_number2];
			preterrain1.treePrototypes[tree_number2] = value;
		}
	}

	public override void set_terrain_details(terrain_class preterrain1)
	{
		List<DetailPrototype> list = new List<DetailPrototype>();
		for (int i = 0; i < preterrain1.detailPrototypes.Count; i++)
		{
			if (preterrain1.detailPrototypes[i].prototype || preterrain1.detailPrototypes[i].prototypeTexture)
			{
				list.Add(new DetailPrototype());
				list[i].renderMode = preterrain1.detailPrototypes[i].renderMode;
				if (preterrain1.detailPrototypes[i].usePrototypeMesh)
				{
					list[i].usePrototypeMesh = true;
					list[i].prototype = preterrain1.detailPrototypes[i].prototype;
					list[i].minWidth = (float)-1;
					list[i].maxWidth = preterrain1.detailPrototypes[i].maxWidth + (float)1;
					list[i].minHeight = (float)-1;
					list[i].maxHeight = preterrain1.detailPrototypes[i].maxHeight + (float)1;
				}
				else
				{
					list[i].prototypeTexture = preterrain1.detailPrototypes[i].prototypeTexture;
					list[i].minWidth = preterrain1.detailPrototypes[i].minWidth;
					list[i].maxWidth = preterrain1.detailPrototypes[i].maxWidth;
					list[i].minHeight = preterrain1.detailPrototypes[i].minHeight;
					list[i].maxHeight = preterrain1.detailPrototypes[i].maxHeight;
				}
				list[i].noiseSpread = preterrain1.detailPrototypes[i].noiseSpread;
				list[i].healthyColor = preterrain1.detailPrototypes[i].healthyColor;
				list[i].dryColor = preterrain1.detailPrototypes[i].dryColor;
				list[i].bendFactor = preterrain1.detailPrototypes[i].bendFactor;
			}
		}
		preterrain1.terrain.terrainData.detailPrototypes = list.ToArray();
		preterrain1.detail_scale = (float)1;
	}

	public override void copy_terrain_detail(detailPrototype_class detailPrototype1, detailPrototype_class detailPrototype2)
	{
		detailPrototype2.prototype = detailPrototype1.prototype;
		detailPrototype2.prototypeTexture = detailPrototype1.prototypeTexture;
		detailPrototype2.minWidth = detailPrototype1.minWidth;
		detailPrototype2.maxWidth = detailPrototype1.maxWidth;
		detailPrototype2.minHeight = detailPrototype1.minHeight;
		detailPrototype2.maxHeight = detailPrototype1.maxHeight;
		detailPrototype2.noiseSpread = detailPrototype1.noiseSpread;
		detailPrototype2.healthyColor = detailPrototype1.healthyColor;
		detailPrototype2.dryColor = detailPrototype1.dryColor;
		detailPrototype2.renderMode = detailPrototype1.renderMode;
		detailPrototype2.bendFactor = detailPrototype1.bendFactor;
	}

	public override void copy_terrain_details(terrain_class preterrain1, terrain_class preterrain2)
	{
		for (int i = 0; i < preterrain1.detailPrototypes.Count; i++)
		{
			if (preterrain2.detailPrototypes.Count < preterrain1.detailPrototypes.Count)
			{
				preterrain2.detailPrototypes.Add(new detailPrototype_class());
			}
			this.copy_terrain_detail(preterrain1.detailPrototypes[i], preterrain2.detailPrototypes[i]);
		}
	}

	public override void swap_terrain_detail(terrain_class preterrain1, int detail_number1, int detail_number2)
	{
		if (detail_number2 > -1 && detail_number2 < preterrain1.detailPrototypes.Count)
		{
			detailPrototype_class value = preterrain1.detailPrototypes[detail_number1];
			preterrain1.detailPrototypes[detail_number1] = preterrain1.detailPrototypes[detail_number2];
			preterrain1.detailPrototypes[detail_number2] = value;
		}
	}

	public override void get_all_terrain_details()
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.get_terrain_details(this.terrains[i]);
			this.check_synchronous_terrain_detail(this.terrains[i]);
		}
	}

	public override void get_terrain_details(terrain_class preterrain1)
	{
		for (int i = 0; i < preterrain1.terrain.terrainData.detailPrototypes.Length; i++)
		{
			if (preterrain1.detailPrototypes.Count < preterrain1.terrain.terrainData.detailPrototypes.Length)
			{
				preterrain1.detailPrototypes.Add(new detailPrototype_class());
			}
			else if (preterrain1.detailPrototypes.Count > preterrain1.terrain.terrainData.detailPrototypes.Length)
			{
				preterrain1.detailPrototypes.RemoveAt(i);
			}
			if (preterrain1.detailPrototypes[i].usePrototypeMesh)
			{
				preterrain1.detailPrototypes[i].prototype = preterrain1.terrain.terrainData.detailPrototypes[i].prototype;
				preterrain1.detailPrototypes[i].minWidth = preterrain1.terrain.terrainData.detailPrototypes[i].minWidth;
				preterrain1.detailPrototypes[i].maxWidth = preterrain1.terrain.terrainData.detailPrototypes[i].maxWidth - (float)1;
				preterrain1.detailPrototypes[i].minHeight = preterrain1.terrain.terrainData.detailPrototypes[i].minHeight;
				preterrain1.detailPrototypes[i].maxHeight = preterrain1.terrain.terrainData.detailPrototypes[i].maxHeight - (float)1;
			}
			else
			{
				preterrain1.detailPrototypes[i].prototypeTexture = preterrain1.terrain.terrainData.detailPrototypes[i].prototypeTexture;
				preterrain1.detailPrototypes[i].minWidth = preterrain1.terrain.terrainData.detailPrototypes[i].minWidth;
				preterrain1.detailPrototypes[i].maxWidth = preterrain1.terrain.terrainData.detailPrototypes[i].maxWidth;
				preterrain1.detailPrototypes[i].minHeight = preterrain1.terrain.terrainData.detailPrototypes[i].minHeight;
				preterrain1.detailPrototypes[i].maxHeight = preterrain1.terrain.terrainData.detailPrototypes[i].maxHeight;
			}
			preterrain1.detailPrototypes[i].noiseSpread = preterrain1.terrain.terrainData.detailPrototypes[i].noiseSpread;
			preterrain1.detailPrototypes[i].healthyColor = preterrain1.terrain.terrainData.detailPrototypes[i].healthyColor;
			preterrain1.detailPrototypes[i].dryColor = preterrain1.terrain.terrainData.detailPrototypes[i].dryColor;
			preterrain1.detailPrototypes[i].renderMode = preterrain1.terrain.terrainData.detailPrototypes[i].renderMode;
			preterrain1.detailPrototypes[i].bendFactor = preterrain1.terrain.terrainData.detailPrototypes[i].bendFactor;
		}
		if (preterrain1.terrain.terrainData.detailPrototypes.Length == 0)
		{
			preterrain1.detailPrototypes.Clear();
		}
	}

	public override void check_synchronous_terrain_detail(terrain_class preterrain1)
	{
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				bool detail_synchronous = true;
				if (preterrain1.detailPrototypes.Count != preterrain1.terrain.terrainData.detailPrototypes.Length)
				{
					detail_synchronous = false;
				}
				else
				{
					for (int i = 0; i < preterrain1.detailPrototypes.Count; i++)
					{
						if (preterrain1.detailPrototypes[i].usePrototypeMesh)
						{
							if (preterrain1.detailPrototypes[i].prototype != preterrain1.terrain.terrainData.detailPrototypes[i].prototype)
							{
								detail_synchronous = false;
								break;
							}
							if (preterrain1.detailPrototypes[i].maxWidth != preterrain1.terrain.terrainData.detailPrototypes[i].maxWidth - (float)1)
							{
								detail_synchronous = false;
								break;
							}
							if (preterrain1.detailPrototypes[i].maxHeight != preterrain1.terrain.terrainData.detailPrototypes[i].maxHeight - (float)1)
							{
								detail_synchronous = false;
								break;
							}
						}
						else
						{
							if (preterrain1.detailPrototypes[i].prototypeTexture != preterrain1.terrain.terrainData.detailPrototypes[i].prototypeTexture)
							{
								detail_synchronous = false;
								break;
							}
							if (preterrain1.detailPrototypes[i].minWidth != preterrain1.terrain.terrainData.detailPrototypes[i].minWidth)
							{
								detail_synchronous = false;
								break;
							}
							if (preterrain1.detailPrototypes[i].maxWidth != preterrain1.terrain.terrainData.detailPrototypes[i].maxWidth)
							{
								detail_synchronous = false;
								break;
							}
							if (preterrain1.detailPrototypes[i].minHeight != preterrain1.terrain.terrainData.detailPrototypes[i].minHeight)
							{
								detail_synchronous = false;
								break;
							}
							if (preterrain1.detailPrototypes[i].maxHeight != preterrain1.terrain.terrainData.detailPrototypes[i].maxHeight)
							{
								detail_synchronous = false;
								break;
							}
						}
						if (preterrain1.detailPrototypes[i].noiseSpread != preterrain1.terrain.terrainData.detailPrototypes[i].noiseSpread)
						{
							detail_synchronous = false;
							break;
						}
						if (preterrain1.detailPrototypes[i].healthyColor != preterrain1.terrain.terrainData.detailPrototypes[i].healthyColor)
						{
							detail_synchronous = false;
							break;
						}
						if (preterrain1.detailPrototypes[i].dryColor != preterrain1.terrain.terrainData.detailPrototypes[i].dryColor)
						{
							detail_synchronous = false;
							break;
						}
						if (preterrain1.detailPrototypes[i].renderMode != preterrain1.terrain.terrainData.detailPrototypes[i].renderMode)
						{
							detail_synchronous = false;
							break;
						}
						if (preterrain1.detailPrototypes[i].bendFactor != preterrain1.terrain.terrainData.detailPrototypes[i].bendFactor)
						{
							detail_synchronous = false;
							break;
						}
					}
				}
				preterrain1.detail_synchronous = detail_synchronous;
			}
		}
	}

	public override void change_terrain_detail_scale(terrain_class preterrain1)
	{
		if (preterrain1.terrain.terrainData.detailPrototypes.Length >= preterrain1.detailPrototypes.Count)
		{
			for (int i = 0; i < preterrain1.detailPrototypes.Count; i++)
			{
				preterrain1.detailPrototypes[i].minWidth = preterrain1.terrain.terrainData.detailPrototypes[i].minWidth * preterrain1.detail_scale;
				preterrain1.detailPrototypes[i].maxWidth = preterrain1.terrain.terrainData.detailPrototypes[i].maxWidth * preterrain1.detail_scale;
				preterrain1.detailPrototypes[i].minHeight = preterrain1.terrain.terrainData.detailPrototypes[i].minHeight * preterrain1.detail_scale;
				preterrain1.detailPrototypes[i].maxHeight = preterrain1.terrain.terrainData.detailPrototypes[i].maxHeight * preterrain1.detail_scale;
			}
		}
	}

	public override void set_all_terrain_details(terrain_class preterrain1)
	{
		this.set_terrain_details(preterrain1);
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				this.terrains[i].terrain.terrainData.detailPrototypes = preterrain1.terrain.terrainData.detailPrototypes;
				this.get_terrain_details(this.terrains[i]);
				this.terrains[i].color_terrain = new Color(0.5f, (float)1, 0.5f);
				this.check_synchronous_terrain_detail(this.terrains[i]);
			}
		}
	}

	public override Color convert_float_to_color(float value_float)
	{
		byte[] array = new byte[4];
		Color result = default(Color);
		float num = 0f;
		array = BitConverter.GetBytes(value_float);
		num = (float)array[0];
		result[0] = num / (float)255;
		num = (float)array[1];
		result[1] = num / (float)255;
		num = (float)array[2];
		result[2] = num / (float)255;
		num = (float)array[3];
		result[3] = num / (float)255;
		return result;
	}

	public override float convert_color_to_float(Color color)
	{
		return BitConverter.ToSingle(new byte[]
		{
			(byte)(color[0] * (float)255),
			(byte)(color[1] * (float)255),
			(byte)(color[2] * (float)255),
			(byte)(color[3] * (float)255)
		}, 0);
	}

	public override float get_scale_from_image(Texture2D image)
	{
		Color color = default(Color);
		Color color2 = default(Color);
		float num = 0f;
		color[0] = image.GetPixel(0, 0)[3];
		color[1] = image.GetPixel(1, 0)[3];
		color[2] = image.GetPixel(2, 0)[3];
		color[3] = image.GetPixel(3, 0)[3];
		return this.convert_color_to_float(color);
	}

	public override Vector2 calc_rotation_pixel(float x, float y, float xx, float yy, float rotation)
	{
		float num = x - xx;
		float num2 = y - yy;
		float num3 = Mathf.Sqrt(num * num + num2 * num2);
		if (num3 != (float)0)
		{
			num /= num3;
			num2 /= num3;
		}
		float num4 = Mathf.Acos(num);
		if (num2 < (float)0)
		{
			num4 = 6.28318548f - num4;
		}
		num4 -= rotation * 0.0174532924f;
		num = Mathf.Cos(num4) * num3;
		num2 = Mathf.Sin(num4) * num3;
		return new Vector2
		{
			x = num + xx,
			y = num2 + yy
		};
	}

	public override prelayer_class copy_prelayer(prelayer_class prelayer1, object copy_filter)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.prelayer = prelayer1;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		prelayer_class prelayer_class = save_template.prelayer;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		if (RuntimeServices.ToBool(copy_filter))
		{
			for (int i = 0; i < prelayer1.layer.Count; i++)
			{
				prelayer_class.layer[i].prefilter = this.copy_prefilter(prelayer1.layer[i].prefilter);
			}
		}
		return prelayer_class;
	}

	public override prelayer_class copy_layergroup(prelayer_class prelayer1, int description_number, bool copy_filter)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.prelayer = new prelayer_class(0, 0);
		for (int i = 0; i < prelayer1.predescription.description[description_number].layer_index.Count; i++)
		{
			save_template.prelayer.layer.Insert(i, new layer_class());
			save_template.prelayer.layer[i] = this.copy_layer(prelayer1.layer[prelayer1.predescription.description[description_number].layer_index[i]], false, false);
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		prelayer_class prelayer_class = save_template.prelayer;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		if (copy_filter)
		{
			for (int i = 0; i < prelayer1.layer.Count; i++)
			{
				prelayer_class.layer[i].prefilter = this.copy_prefilter(prelayer1.layer[i].prefilter);
				for (int j = 0; j < prelayer1.layer[i].tree_output.tree.Count; j++)
				{
					prelayer_class.layer[i].tree_output.tree[j].prefilter = this.copy_prefilter(prelayer1.layer[i].tree_output.tree[j].prefilter);
				}
			}
		}
		return prelayer_class;
	}

	public override layer_class copy_layer(layer_class layer, bool copy_filter, bool loop)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.layer = layer;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		layer = save_template.layer;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		if (copy_filter)
		{
			save_template.layer.prefilter = this.copy_prefilter(layer.prefilter);
			for (int i = 0; i < layer.tree_output.tree.Count; i++)
			{
				save_template.layer.tree_output.tree[i].prefilter = this.copy_prefilter(layer.tree_output.tree[i].prefilter);
			}
		}
		layer.text_placed = string.Empty;
		if (loop)
		{
			this.loop_layer(layer, 1);
		}
		layer.swap_text = "S";
		layer.swap_select = false;
		layer.copy_select = false;
		layer.color_layer = new Color((float)2, (float)2, (float)2, (float)1);
		return layer;
	}

	public override void save_loop_layer(int prelayer_number, int layer_number, int prelayer_number_save, int layer_number_save, save_template script3)
	{
		script3.prelayers[prelayer_number_save].layer[layer_number_save].swap_text = "S";
		script3.prelayers[prelayer_number_save].layer[layer_number_save].swap_select = false;
		script3.prelayers[prelayer_number_save].layer[layer_number_save].copy_select = false;
		for (int i = 0; i < this.prelayers[prelayer_number].layer[layer_number].prefilter.filter_index.Count; i++)
		{
			script3.filters.Add(this.copy_filter(this.filter[this.prelayers[prelayer_number].layer[layer_number].prefilter.filter_index[i]], false));
			script3.prelayers[prelayer_number_save].layer[layer_number_save].prefilter.filter_index[i] = script3.filters.Count - 1;
			for (int j = 0; j < this.filter[this.prelayers[prelayer_number].layer[layer_number].prefilter.filter_index[i]].presubfilter.subfilter_index.Count; j++)
			{
				script3.subfilters.Add(this.copy_subfilter(this.subfilter[this.filter[this.prelayers[prelayer_number].layer[layer_number].prefilter.filter_index[i]].presubfilter.subfilter_index[j]]));
				script3.filters[script3.filters.Count - 1].presubfilter.subfilter_index[j] = script3.subfilters.Count - 1;
			}
		}
		for (int k = 0; k < this.prelayers[prelayer_number].layer[layer_number].tree_output.tree.Count; k++)
		{
			for (int i = 0; i < this.prelayers[prelayer_number].layer[layer_number].tree_output.tree[k].prefilter.filter_index.Count; i++)
			{
				script3.filters.Add(this.copy_filter(this.filter[this.prelayers[prelayer_number].layer[layer_number].tree_output.tree[k].prefilter.filter_index[i]], false));
				script3.prelayers[prelayer_number_save].layer[layer_number_save].tree_output.tree[k].prefilter.filter_index[i] = script3.filters.Count - 1;
				for (int j = 0; j < this.filter[this.prelayers[prelayer_number].layer[layer_number].tree_output.tree[k].prefilter.filter_index[i]].presubfilter.subfilter_index.Count; j++)
				{
					script3.subfilters.Add(this.copy_subfilter(this.subfilter[this.filter[this.prelayers[prelayer_number].layer[layer_number].tree_output.tree[k].prefilter.filter_index[i]].presubfilter.subfilter_index[j]]));
					script3.filters[script3.filters.Count - 1].presubfilter.subfilter_index[j] = script3.subfilters.Count - 1;
				}
			}
		}
		for (int l = 0; l < this.prelayers[prelayer_number].layer[layer_number].object_output.@object.Count; l++)
		{
			if (this.prelayers[prelayer_number].layer[layer_number].object_output.@object[l].prelayer_created)
			{
				script3.prelayers.Add(this.copy_prelayer(this.prelayers[this.prelayers[prelayer_number].layer[layer_number].object_output.@object[l].prelayer_index], false));
				script3.prelayers[prelayer_number_save].layer[layer_number_save].object_output.@object[l].prelayer_index = script3.prelayers.Count - 1;
				for (int m = 0; m < this.prelayers[this.prelayers[prelayer_number].layer[layer_number].object_output.@object[l].prelayer_index].layer.Count; m++)
				{
					this.save_loop_layer(this.prelayers[prelayer_number].layer[layer_number].object_output.@object[l].prelayer_index, m, prelayer_number_save + 1, m, script3);
				}
			}
		}
	}

	public override void load_loop_layer(int prelayer_number, int layer_number, int prelayer_number_load, int layer_number_load, save_template script3)
	{
		for (int i = 0; i < script3.prelayers[prelayer_number_load].layer[layer_number_load].prefilter.filter_index.Count; i++)
		{
			this.filter.Add(this.copy_filter(script3.filters[script3.prelayers[prelayer_number_load].layer[layer_number_load].prefilter.filter_index[i]], false));
			this.prelayers[prelayer_number].layer[layer_number].prefilter.filter_index[i] = this.filter.Count - 1;
			for (int j = 0; j < this.filter[this.filter.Count - 1].presubfilter.subfilter_index.Count; j++)
			{
				this.subfilter.Add(this.copy_subfilter(script3.subfilters[this.filter[this.filter.Count - 1].presubfilter.subfilter_index[j]]));
				this.filter[this.filter.Count - 1].presubfilter.subfilter_index[j] = this.subfilter.Count - 1;
			}
		}
		for (int k = 0; k < script3.prelayers[prelayer_number_load].layer[layer_number_load].tree_output.tree.Count; k++)
		{
			for (int i = 0; i < script3.prelayers[prelayer_number_load].layer[layer_number_load].tree_output.tree[k].prefilter.filter_index.Count; i++)
			{
				this.filter.Add(this.copy_filter(script3.filters[script3.prelayers[prelayer_number_load].layer[layer_number_load].tree_output.tree[k].prefilter.filter_index[i]], false));
				this.prelayers[prelayer_number].layer[layer_number].tree_output.tree[k].prefilter.filter_index[i] = this.filter.Count - 1;
				for (int j = 0; j < this.filter[this.filter.Count - 1].presubfilter.subfilter_index.Count; j++)
				{
					this.subfilter.Add(this.copy_subfilter(script3.subfilters[this.filter[this.filter.Count - 1].presubfilter.subfilter_index[j]]));
					this.filter[this.filter.Count - 1].presubfilter.subfilter_index[j] = this.subfilter.Count - 1;
				}
			}
		}
		for (int l = 0; l < script3.prelayers[prelayer_number_load].layer[layer_number_load].object_output.@object.Count; l++)
		{
			if (script3.prelayers[prelayer_number_load].layer[layer_number_load].object_output.@object[l].prelayer_created)
			{
				this.prelayers.Add(this.copy_prelayer(script3.prelayers[script3.prelayers[prelayer_number_load].layer[layer_number_load].object_output.@object[l].prelayer_index], false));
				this.prelayers[prelayer_number].layer[layer_number].object_output.@object[l].prelayer_index = this.prelayers.Count - 1;
				for (int m = 0; m < script3.prelayers[script3.prelayers[prelayer_number_load].layer[layer_number_load].object_output.@object[l].prelayer_index].layer.Count; m++)
				{
					this.load_loop_layer(this.prelayers.Count - 1, m, script3.prelayers[prelayer_number_load].layer[layer_number_load].object_output.@object[l].prelayer_index, m, script3);
				}
			}
		}
	}

	public override void copy_description(prelayer_class prelayer1, int description_number, prelayer_class target_prelayer, int target_description_number)
	{
		target_prelayer.predescription.description[target_description_number].text = prelayer1.predescription.description[description_number].text + "#";
		target_prelayer.predescription.description[target_description_number].edit = prelayer1.predescription.description[description_number].edit;
		target_prelayer.predescription.description[target_description_number].layers_active = prelayer1.predescription.description[description_number].layers_active;
		int num = this.get_layer_position(0, target_description_number, target_prelayer);
		int count = prelayer1.predescription.description[description_number].layer_index.Count;
		for (int i = 0; i < count; i++)
		{
			this.add_layer(target_prelayer, num, layer_output_enum.color, target_description_number, 0, false, false, false);
			target_prelayer.layer[num] = this.copy_layer(prelayer1.layer[prelayer1.predescription.description[description_number].layer_index[count - 1 - i]], true, true);
		}
		this.count_layers();
	}

	public override prefilter_class copy_prefilter(prefilter_class prefilter)
	{
		prefilter_class prefilter_class = new prefilter_class();
		for (int i = 0; i < prefilter.filter_index.Count; i++)
		{
			this.filter.Add(this.copy_filter(this.filter[prefilter.filter_index[i]], true));
			prefilter_class.filter_index.Add(this.filter.Count - 1);
		}
		prefilter_class.set_filter_text();
		return prefilter_class;
	}

	public override filter_class copy_filter(filter_class filter, bool copy_subfilter)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.filter = filter;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		filter = save_template.filter;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		if (copy_subfilter)
		{
			for (int i = 0; i < filter.presubfilter.subfilter_index.Count; i++)
			{
				this.subfilter.Add(this.copy_subfilter(this.subfilter[filter.presubfilter.subfilter_index[i]]));
				filter.presubfilter.subfilter_index[i] = this.subfilter.Count - 1;
			}
		}
		for (int j = 0; j < filter.preimage.precolor_range.color_range.Count; j++)
		{
			filter.preimage.precolor_range.color_range[j].swap_text = "S";
			filter.preimage.precolor_range.color_range[j].swap_select = false;
			filter.preimage.precolor_range.color_range[j].copy_select = false;
		}
		filter.swap_text = "S";
		filter.swap_select = false;
		filter.copy_select = false;
		filter.prerandom_curve.curve_text = "Curve";
		filter.precurve_x_left.curve_text = "Curve";
		filter.precurve_x_right.curve_text = "Curve";
		filter.precurve_z_left.curve_text = "Curve";
		filter.precurve_z_right.curve_text = "Curve";
		filter.color_filter = new Color((float)2, (float)2, (float)2, (float)1);
		return filter;
	}

	public override subfilter_class copy_subfilter(subfilter_class subfilter)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.subfilter = subfilter;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		subfilter = save_template.subfilter;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		subfilter.swap_text = "S";
		subfilter.swap_select = false;
		subfilter.copy_select = false;
		for (int i = 0; i < subfilter.preimage.precolor_range.color_range.Count; i++)
		{
			subfilter.preimage.precolor_range.color_range[i].swap_text = "S";
			subfilter.preimage.precolor_range.color_range[i].swap_select = false;
			subfilter.preimage.precolor_range.color_range[i].copy_select = false;
		}
		subfilter.color_subfilter = new Color((float)2, (float)2, (float)2, (float)1);
		return subfilter;
	}

	public override terrain_class copy_terrain(terrain_class preterrain1)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.preterrain = preterrain1;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		preterrain1 = save_template.preterrain;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		return preterrain1;
	}

	public override Terrain copy_terrain2(Terrain terrain1)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.terrain = terrain1;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		terrain1 = save_template.terrain;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		return terrain1;
	}

	public override animation_curve_class copy_animation_curve(animation_curve_class animation_curve)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.animation_curve = animation_curve;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		animation_curve = save_template.animation_curve;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		return animation_curve;
	}

	public override color_range_class copy_color_range(color_range_class color_range)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.color_range = color_range;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		color_range = save_template.color_range;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		color_range.swap_text = "S";
		color_range.swap_select = false;
		color_range.copy_select = false;
		return color_range;
	}

	public override precolor_range_class copy_precolor_range(precolor_range_class precolor_range)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.precolor_range = precolor_range;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		precolor_range = save_template.precolor_range;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		return precolor_range;
	}

	public override tree_class copy_tree(tree_class tree)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.tree = tree;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		tree = save_template.tree;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		for (int i = 0; i < tree.prefilter.filter_index.Count; i++)
		{
			this.filter.Add(this.copy_filter(this.filter[tree.prefilter.filter_index[i]], true));
			tree.prefilter.filter_index[i] = this.filter.Count - 1;
		}
		tree.placed = 0;
		tree.swap_select = false;
		tree.copy_select = false;
		tree.swap_text = "S";
		return tree;
	}

	public override object_class copy_object(object_class object1)
	{
		GameObject gameObject = new GameObject();
		save_template save_template = (save_template)gameObject.AddComponent(typeof(save_template));
		save_template.@object = object1;
		GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
		UnityEngine.Object.DestroyImmediate(gameObject);
		save_template = (save_template)gameObject2.GetComponent(typeof(save_template));
		object1 = save_template.@object;
		UnityEngine.Object.DestroyImmediate(gameObject2);
		object1.color_object = new Color((float)2, (float)2, (float)2, (float)1);
		object1.swap_text = "S";
		object1.swap_select = false;
		object1.copy_select = false;
		object1.placed = 0;
		return object1;
	}

	public override int check_terrains_same_resolution()
	{
		int i = 0;
		int arg_A7_0;
		while (i < this.terrains.Count)
		{
			if (!this.terrains[i].terrain)
			{
				arg_A7_0 = -2;
			}
			else if (!this.terrains[i].terrain.terrainData)
			{
				arg_A7_0 = -2;
			}
			else
			{
				if (this.terrains[i].terrain.terrainData.heightmapResolution == this.terrains[0].terrain.terrainData.heightmapResolution)
				{
					i++;
					continue;
				}
				arg_A7_0 = -1;
			}
			return arg_A7_0;
		}
		arg_A7_0 = 1;
		return arg_A7_0;
	}

	public override bool stitch_terrains(float border_influence)
	{
		float num = Mathf.Round(border_influence / this.terrains[0].heightmap_conversion.x);
		float num2 = Mathf.Round(border_influence / this.terrains[0].heightmap_conversion.y);
		bool arg_646_0;
		if (border_influence < this.terrains[0].heightmap_conversion.x * 1.5f)
		{
			arg_646_0 = false;
		}
		else
		{
			float num3 = this.stitch_tool_strength;
			int i = 0;
			int num4 = 0;
			int num5 = 0;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			float num11 = 0f;
			float num12 = 0f;
			float num13 = 0f;
			float num14 = 0f;
			float num15 = 0f;
			float num16 = 0f;
			float num17 = 0f;
			float num18 = 0f;
			for (int j = 0; j < this.terrains.Count; j++)
			{
				num4 = -1;
				num5 = -1;
				for (i = 0; i < this.terrains.Count; i++)
				{
					if (i != j)
					{
						if (this.terrains[i].rect.Contains(new Vector2(this.terrains[j].rect.center.x, this.terrains[j].rect.yMax + this.terrains[j].heightmap_conversion.y)) && num4 == -1)
						{
							num4 = i;
						}
						if (this.terrains[i].rect.Contains(new Vector2(this.terrains[j].rect.xMin - this.terrains[j].heightmap_conversion.x, this.terrains[j].rect.center.y)) && num5 == -1)
						{
							num5 = i;
						}
					}
				}
				if (num4 != -1)
				{
					float[,] array = this.terrains[j].terrain.terrainData.GetHeights(0, (int)(this.terrains[j].heightmap_resolution - num2), (int)this.terrains[j].heightmap_resolution, (int)num2);
					float[,] array2 = this.terrains[num4].terrain.terrainData.GetHeights(0, 0, (int)this.terrains[j].heightmap_resolution, (int)num2);
					int num19 = 0;
					while ((float)num19 < this.terrains[j].heightmap_resolution)
					{
						num11 = array[0, num19];
						num12 = array2[(int)(num2 - (float)1), num19];
						for (num7 = (float)0; num7 < num2 - (float)1; num7 += (float)1)
						{
							if (num7 == (float)0)
							{
								num9 = array[(int)(num2 - num7 - (float)1), num19];
								num10 = array2[(int)num7, num19];
								num8 = (num11 + num12) / (float)2;
								num13 = (num11 - num8) / (num2 - (float)1);
								num14 = (num12 - num8) / (num2 - (float)1);
								array[(int)(num2 - num7 - (float)1), num19] = num8;
								array2[(int)num7, num19] = num8;
							}
							else
							{
								array[(int)(num2 - num7 - (float)1), num19] = num8 + num13 * num7;
								array2[(int)num7, num19] = num8 + num14 * num7;
							}
						}
						num19++;
					}
					this.terrains[j].terrain.terrainData.SetHeights(0, (int)(this.terrains[j].heightmap_resolution - num2), array);
					this.terrains[num4].terrain.terrainData.SetHeights(0, 0, array2);
				}
				if (num5 != -1)
				{
					float[,] array = this.terrains[j].terrain.terrainData.GetHeights(0, 0, (int)num, (int)this.terrains[j].heightmap_resolution);
					float[,] array2 = this.terrains[num5].terrain.terrainData.GetHeights((int)(this.terrains[j].heightmap_resolution - num), 0, (int)num, (int)this.terrains[j].heightmap_resolution);
					int num20 = 0;
					while ((float)num20 < this.terrains[j].heightmap_resolution)
					{
						num11 = array[num20, (int)(num - (float)1)];
						num12 = array2[num20, 0];
						for (num6 = (float)0; num6 < num - (float)1; num6 += (float)1)
						{
							if (num6 == (float)0)
							{
								num9 = array[num20, (int)num6];
								num10 = array2[num20, (int)(num - num6 - (float)1)];
								num8 = (num11 + num12) / (float)2;
								num13 = (num11 - num8) / (num - (float)1);
								num14 = (num12 - num8) / (num - (float)1);
								array[num20, (int)num6] = num8;
								array2[num20, (int)(num - num6 - (float)1)] = num8;
							}
							else
							{
								array[num20, (int)num6] = num8 + num13 * num6;
								array2[num20, (int)(num - num6 - (float)1)] = num8 + num14 * num6;
							}
						}
						num20++;
					}
					this.terrains[j].terrain.terrainData.SetHeights(0, 0, array);
					this.terrains[num5].terrain.terrainData.SetHeights((int)(this.terrains[j].heightmap_resolution - num), 0, array2);
				}
				if (this.terrains[j].color_terrain[0] < 1.5f)
				{
					this.terrains[j].color_terrain = this.terrains[j].color_terrain + new Color(0.5f, 0.5f, (float)1, 0.5f);
				}
			}
			arg_646_0 = true;
		}
		return arg_646_0;
	}

	public override void stitch_splatmap()
	{
		if (this.terrains.Count >= 2)
		{
			int num = 0;
			int index = 0;
			int num2 = 0;
			int num3 = 0;
			while ((float)num3 < this.terrains[0].tiles.x)
			{
				int num4 = 0;
				while ((float)num4 < this.terrains[0].tiles.y)
				{
					num = (int)((float)num4 + (float)num3 * this.terrains[0].tiles.y);
					index = (int)((float)num4 + (float)(num3 - 1) * this.terrains[0].tiles.y);
					if (num4 == 0)
					{
						goto IL_15B;
					}
					num2 = this.terrains[num - 1].terrain.terrainData.alphamapResolution;
					float[,,] alphamaps;
					if (this.terrains[num].terrain.terrainData.alphamapResolution == num2)
					{
						if (this.terrains[num].terrain.terrainData.splatPrototypes.Length == this.terrains[num - 1].terrain.terrainData.splatPrototypes.Length)
						{
							alphamaps = this.terrains[num - 1].terrain.terrainData.GetAlphamaps(0, 0, num2, 1);
							this.terrains[num].terrain.terrainData.SetAlphamaps(0, num2 - 1, alphamaps);
							goto IL_15B;
						}
					}
					IL_22E:
					num4++;
					continue;
					IL_15B:
					if (num3 == 0)
					{
						goto IL_22E;
					}
					num2 = this.terrains[index].terrain.terrainData.alphamapResolution;
					if (this.terrains[num].terrain.terrainData.alphamapResolution != num2)
					{
						goto IL_22E;
					}
					if (this.terrains[num].terrain.terrainData.splatPrototypes.Length != this.terrains[index].terrain.terrainData.splatPrototypes.Length)
					{
						goto IL_22E;
					}
					alphamaps = this.terrains[index].terrain.terrainData.GetAlphamaps(num2 - 1, 0, 1, num2);
					this.terrains[num].terrain.terrainData.SetAlphamaps(0, 0, alphamaps);
					goto IL_22E;
				}
				num3++;
			}
		}
	}

	public override void smooth_terrain(terrain_class preterrain1, float strength)
	{
		if (preterrain1.terrain)
		{
			int heightmapResolution = preterrain1.terrain.terrainData.heightmapResolution;
			float num = 0f;
			float num2 = 0f;
			float num3 = (float)1;
			float num4 = (float)1;
			float num5 = (float)0;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			this.heights = preterrain1.terrain.terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
			for (int i = 0; i < this.smooth_tool_repeat; i++)
			{
				for (int j = 0; j < heightmapResolution; j++)
				{
					for (int k = 1; k < heightmapResolution - 1; k++)
					{
						num2 = this.heights[k - 1, j];
						num6 = this.heights[k, j];
						num7 = this.heights[k + 1, j];
						num = num6 - (num2 + num7) / (float)2;
						if (this.smooth_tool_advanced)
						{
							num3 = this.smooth_tool_height_curve.curve.Evaluate(num6);
							num4 = this.smooth_tool_angle_curve.curve.Evaluate(this.calc_terrain_angle(preterrain1, (float)k, (float)j, this.settings.smooth_angle) / (float)90);
						}
						num *= (float)1 - strength * num3 * num4;
						num8 = num + (num2 + num7) / (float)2;
						this.heights[k, j] = num8;
					}
				}
				for (int j = 1; j < heightmapResolution - 1; j++)
				{
					for (int k = 0; k < heightmapResolution; k++)
					{
						num2 = this.heights[k, j - 1];
						num6 = this.heights[k, j];
						num7 = this.heights[k, j + 1];
						num = num6 - (num2 + num7) / (float)2;
						if (this.smooth_tool_advanced)
						{
							num3 = this.smooth_tool_height_curve.curve.Evaluate(num6);
							num4 = this.smooth_tool_angle_curve.curve.Evaluate(this.calc_terrain_angle(preterrain1, (float)k, (float)j, this.settings.smooth_angle) / (float)90);
						}
						num *= (float)1 - strength * num3 * num4;
						num8 = num + (num2 + num7) / (float)2;
						this.heights[k, j] = num8;
					}
				}
			}
			preterrain1.terrain.terrainData.SetHeights(0, 0, this.heights);
			if (preterrain1.color_terrain[0] < 1.5f)
			{
				preterrain1.color_terrain += new Color(0.5f, 0.5f, (float)1, 0.5f);
			}
		}
	}

	public override void get_terrains_minmax()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		Vector3 vector = default(Vector3);
		this.settings.terrainMinHeight = (float)100000000;
		this.settings.terrainMaxHeight = (float)0;
		this.settings.terrainMinDegree = (float)100;
		this.settings.terrainMaxDegree = (float)0;
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				if (this.terrains[i].terrain.terrainData)
				{
					num = (float)this.terrains[i].terrain.terrainData.heightmapResolution;
					vector = this.terrains[i].terrain.terrainData.size;
					int num4 = 0;
					while ((float)num4 < num)
					{
						int num5 = 0;
						while ((float)num5 < num)
						{
							num2 = this.terrains[i].terrain.terrainData.GetHeight(num5, num4);
							if (num2 < this.settings.terrainMinHeight)
							{
								this.settings.terrainMinHeight = num2;
							}
							if (num2 > this.settings.terrainMaxHeight)
							{
								this.settings.terrainMaxHeight = num2;
							}
							num3 = this.calc_terrain_angle(this.terrains[i], (float)num5 / num * vector.x, (float)num4 / num * vector.z, this.settings.smooth_angle);
							if (num3 < this.settings.terrainMinDegree)
							{
								this.settings.terrainMinDegree = num3;
							}
							if (num3 > this.settings.terrainMaxDegree)
							{
								this.settings.terrainMaxDegree = num3;
							}
							num5++;
						}
						num4++;
					}
				}
			}
		}
	}

	public override void get_meshes_minmax_height()
	{
		this.settings.terrainMinHeight = (float)100000000;
		this.settings.terrainMaxHeight = (float)0;
		this.settings.terrainMinDegree = (float)0;
		this.settings.terrainMaxDegree = (float)0;
		for (int i = 0; i < this.meshes.Count; i++)
		{
			if (this.meshes[i].mesh)
			{
				if (this.meshes[i].mesh.bounds.min.y < this.settings.terrainMinHeight)
				{
					this.settings.terrainMinHeight = this.meshes[i].mesh.bounds.min.y;
				}
				if (this.meshes[i].mesh.bounds.max.y > this.settings.terrainMaxHeight)
				{
					this.settings.terrainMaxHeight = this.meshes[i].mesh.bounds.max.y;
				}
			}
		}
		this.meshes_heightscale = this.settings.terrainMaxHeight;
	}

	public override void smooth_all_terrain(float strength)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.smooth_terrain(this.terrains[i], strength);
		}
	}

	public override void set_smooth_tool_terrain_popup()
	{
		if (this.terrains.Count > 1)
		{
			this.smooth_tool_terrain = new string[this.terrains.Count + 1];
			this.smooth_tool_terrain[this.terrains.Count] = "All";
			this.smooth_tool_terrain_select = this.terrains.Count;
		}
		else
		{
			this.smooth_tool_terrain = new string[1];
			this.smooth_tool_terrain_select = 0;
		}
		for (int i = 0; i < this.terrains.Count; i++)
		{
			this.smooth_tool_terrain[i] = this.terrains[i].name;
		}
	}

	public override void convert_software_version()
	{
		if (this.converted_version < 1.04f)
		{
			for (int i = 0; i < this.prelayers.Count; i++)
			{
				for (int j = 0; j < this.prelayers[i].layer.Count; j++)
				{
					for (int k = 0; k < this.prelayers[i].layer[j].color_output.precolor_range.Count; k++)
					{
						this.convert_precolor_range(this.prelayers[i].layer[j].color_output.precolor_range[k]);
					}
				}
			}
			for (int l = 0; l < this.filter.Count; l++)
			{
				this.convert_precolor_range(this.filter[l].preimage.precolor_range);
			}
			for (int m = 0; m < this.subfilter.Count; m++)
			{
				this.convert_precolor_range(this.subfilter[m].preimage.precolor_range);
			}
			this.converted_version = 1.04f;
		}
	}

	public override void convert_precolor_range(precolor_range_class precolor_range)
	{
		precolor_range.color_range_value.calc_value();
	}

	public override void filter_texture(int previewMode)
	{
		if (this.texture_tool.preimage.image[0])
		{
			int width = this.texture_tool.preimage.image[0].width;
			int num = this.texture_tool.preimage.image[0].height;
			if (this.texture_tool.preimage.image.Count == 1)
			{
				this.texture_tool.preimage.image.Add(new Texture2D(1, 1));
			}
			if (!this.texture_tool.preimage.image[1])
			{
				this.texture_tool.preimage.image[1] = new Texture2D(1, 1);
			}
			if ((float)this.texture_tool.preimage.image[1].width != this.texture_tool.resolution_display.x || (float)this.texture_tool.preimage.image[1].height != this.texture_tool.resolution_display.x)
			{
				this.texture_tool.preimage.image[1].Resize((int)this.texture_tool.resolution_display.x, (int)this.texture_tool.resolution_display.y);
			}
			int width2 = this.texture_tool.preimage.image[1].width;
			int num2 = this.texture_tool.preimage.image[1].height;
			Vector2 vector = new Vector2((float)(width / width2), (float)(num / num2));
			int count = this.texture_tool.precolor_range.color_range.Count;
			Color color = default(Color);
			Color color_end = default(Color);
			Color lhs = default(Color);
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < width2; j++)
				{
					bool flag = false;
					lhs = this.texture_tool.preimage.image[0].GetPixel((int)((float)j * vector.x), (int)((float)i * vector.y));
					for (int k = 0; k < count; k++)
					{
						if (this.texture_tool.precolor_range.color_range_value.active[k])
						{
							color = this.texture_tool.precolor_range.color_range[k].color_start;
							color_end = this.texture_tool.precolor_range.color_range[k].color_end;
							if (this.texture_tool.precolor_range.color_range[k].one_color)
							{
								if (lhs == color)
								{
									flag = true;
									if ((previewMode & 1) != 0)
									{
										lhs = this.choose_color(k, (float)1);
									}
								}
							}
							else if (this.color_in_range(lhs, color, color_end))
							{
								if (!this.texture_tool.precolor_range.color_range[k].invert)
								{
									if ((previewMode & 1) != 0)
									{
										lhs = this.choose_color(k, this.texture_tool.precolor_range.color_range[k].curve.Evaluate(this.calc_color_pos(lhs, color, color_end)));
									}
									flag = true;
								}
							}
							else if (this.texture_tool.precolor_range.color_range[k].invert)
							{
								flag = true;
								if ((previewMode & 1) != 0)
								{
									lhs = this.choose_color(k, (float)1 - this.texture_tool.precolor_range.color_range[k].curve.Evaluate(this.calc_color_pos(lhs, color, color_end)));
								}
							}
						}
					}
					if (!flag && (previewMode & 2) == 0)
					{
						lhs = new Color((float)0, (float)0, (float)0);
					}
					this.texture_tool.preimage.image[1].SetPixel(j, i, lhs);
				}
			}
			this.texture_tool.preimage.image[1].Apply();
		}
	}

	public override Color choose_color(int index, float falloff)
	{
		Color result = Color.red;
		if (index == 0)
		{
			result = Color.red * falloff;
		}
		else if (index == 1)
		{
			result = Color.green * falloff;
		}
		else if (index == 2)
		{
			result = Color.blue * falloff;
		}
		else if (index == 3)
		{
			result = Color.yellow * falloff;
		}
		else if (index == 4)
		{
			result = Color.white * falloff;
		}
		else if (index == 5)
		{
			result = Color.cyan * falloff;
		}
		else if (index == 6)
		{
			result = Color.magenta * falloff;
		}
		else if (index == 7)
		{
			result = Color.grey * falloff;
		}
		return result;
	}

	public override AnimationCurve set_curve_linear(AnimationCurve curve)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		for (int i = 0; i < curve.keys.Length; i++)
		{
			float inTangent = (float)0;
			float outTangent = (float)0;
			bool flag = false;
			bool flag2 = false;
			Vector2 vector = default(Vector2);
			Vector2 vector2 = default(Vector2);
			Vector2 vector3 = default(Vector2);
			Keyframe key = curve[i];
			if (i == 0)
			{
				inTangent = (float)0;
				flag = true;
			}
			if (i == curve.keys.Length - 1)
			{
				outTangent = (float)0;
				flag2 = true;
			}
			if (!flag)
			{
				vector.x = curve.keys[i - 1].time;
				vector.y = curve.keys[i - 1].value;
				vector2.x = curve.keys[i].time;
				vector2.y = curve.keys[i].value;
				vector3 = vector2 - vector;
				inTangent = vector3.y / vector3.x;
			}
			if (!flag2)
			{
				vector.x = curve.keys[i].time;
				vector.y = curve.keys[i].value;
				vector2.x = curve.keys[i + 1].time;
				vector2.y = curve.keys[i + 1].value;
				vector3 = vector2 - vector;
				outTangent = vector3.y / vector3.x;
			}
			key.inTangent = inTangent;
			key.outTangent = outTangent;
			animationCurve.AddKey(key);
		}
		return animationCurve;
	}

	public override float perlin_noise(float x, float y, float offset_x, float offset_y, float frequency, float octaves, float detail_strength)
	{
		frequency *= this.current_layer.zoom;
		offset_x += this.current_layer.offset.x;
		offset_y += this.current_layer.offset.y;
		float num = Mathf.PerlinNoise((x + frequency * (offset_x + (float)50)) / frequency, (y + frequency * (offset_y + (float)50)) / frequency);
		float num2 = (float)2;
		for (float num3 = (float)1; num3 < octaves; num3 += (float)1)
		{
			num += (Mathf.PerlinNoise((x + frequency * (offset_x + (float)50)) / (frequency / num2), (y + frequency * (offset_y + (float)50)) / (frequency / num2)) - 0.5f) / num2;
			num2 *= detail_strength;
		}
		return num;
	}

	public override float clamp_range(float number, float start, float end)
	{
		return (end - start) * number + start;
	}

	public override void create_perlin(int preview_resolution, int resolution, export_mode_enum mode, bool save)
	{
		float num = (float)resolution * 1f / ((float)preview_resolution * 1f);
		int i = 0;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		int num5 = preview_resolution * preview_resolution;
		int num6 = 0;
		float frequency = this.heightmap_tool.perlin.frequency;
		if (mode == export_mode_enum.Image)
		{
			Color[] array;
			if (mode == export_mode_enum.Image)
			{
				array = new Color[num5];
			}
			Color color = default(Color);
			for (num2 = (float)0; num2 < (float)resolution; num2 += num)
			{
				for (num3 = (float)0; num3 < (float)resolution; num3 += num)
				{
					color[0] = Mathf.PerlinNoise(((float)(-(float)(resolution / 2)) + num3 + frequency * (this.heightmap_tool.perlin.offset.x + (float)5000)) / frequency, ((float)(-(float)(resolution / 2)) + num2 + frequency * (this.heightmap_tool.perlin.offset.y + (float)5000)) / frequency);
					num4 = (float)2;
					for (i = 1; i < this.heightmap_tool.perlin.octaves; i++)
					{
						color[0] = color[0] + (Mathf.PerlinNoise(((float)(-(float)(resolution / 2)) + num3 + frequency * (this.heightmap_tool.perlin.offset.x + (float)5000)) / (frequency / num4), ((float)(-(float)(resolution / 2)) + num2 + frequency * (this.heightmap_tool.perlin.offset.y + (float)5000)) / (frequency / num4)) - 0.5f) / num4;
						num4 *= (float)2;
					}
					color[0] = color[0];
					color[1] = color[0];
					color[2] = color[0];
					num6 = (int)(num3 / num + num2 / num * (float)preview_resolution);
					if (num6 > num5 - 1)
					{
						num6 = num5 - 1;
					}
					array[num6] = color;
				}
			}
			if (save)
			{
				if (preview_resolution != this.heightmap_tool.output_texture.width)
				{
					this.heightmap_tool.output_texture.Resize(resolution, resolution);
				}
				this.heightmap_tool.output_texture.SetPixels(array);
				this.heightmap_tool.output_texture.Apply();
			}
			else
			{
				this.heightmap_tool.preview_texture.SetPixels(array);
				this.heightmap_tool.preview_texture.Apply();
			}
		}
		else if (mode == export_mode_enum.Raw && save)
		{
			this.heightmap_tool.raw_save_file.bytes = new byte[resolution * resolution * 2];
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			ushort num10 = 0;
			float num11 = 0f;
			if (this.heightmap_tool.raw_save_file.mode == raw_mode_enum.Mac)
			{
				for (num3 = (float)0; num3 < (float)resolution; num3 += (float)1)
				{
					for (num2 = (float)0; num2 < (float)resolution; num2 += (float)1)
					{
						num11 = Mathf.PerlinNoise((num3 + this.heightmap_tool.perlin.offset.x + (float)1000000) / this.heightmap_tool.perlin.frequency, (num2 + this.heightmap_tool.perlin.offset.y + (float)1000000) / this.heightmap_tool.perlin.frequency);
						num4 = (float)2;
						for (i = 1; i < this.heightmap_tool.perlin.octaves; i++)
						{
							num11 += (Mathf.PerlinNoise((num3 + this.heightmap_tool.perlin.offset.x + (float)1000000) / (this.heightmap_tool.perlin.frequency / num4), (num2 + this.heightmap_tool.perlin.offset.y + (float)1000000) / (this.heightmap_tool.perlin.frequency / num4)) - 0.5f) / num4;
							num4 *= (float)2;
						}
						num11 = this.heightmap_tool.perlin.precurve.curve.Evaluate(num11) * (float)65535;
						if (num11 < (float)0)
						{
							num11 = (float)0;
						}
						if (num11 > (float)65535)
						{
							num11 = (float)65535;
						}
						num10 = (ushort)num11;
						num7 = (int)((uint)num10 >> 8);
						num8 = (int)num10 - (num7 << 8);
						byte[] arg_47D_0 = this.heightmap_tool.raw_save_file.bytes;
						int num12;
						num9 = (num12 = num9) + 1;
						arg_47D_0[num12] = (byte)num7;
						byte[] arg_49E_0 = this.heightmap_tool.raw_save_file.bytes;
						int num13;
						num9 = (num13 = num9) + 1;
						arg_49E_0[num13] = (byte)num8;
					}
				}
			}
			else if (this.heightmap_tool.raw_save_file.mode == raw_mode_enum.Windows)
			{
				for (num3 = (float)0; num3 < (float)resolution; num3 += (float)1)
				{
					for (num2 = (float)0; num2 < (float)resolution; num2 += (float)1)
					{
						num11 = Mathf.PerlinNoise((num3 + this.heightmap_tool.perlin.offset.x + (float)1000000) / this.heightmap_tool.perlin.frequency, (num2 + this.heightmap_tool.perlin.offset.y + (float)1000000) / this.heightmap_tool.perlin.frequency);
						num4 = (float)2;
						for (i = 1; i < this.heightmap_tool.perlin.octaves; i++)
						{
							num11 += (Mathf.PerlinNoise((num3 + this.heightmap_tool.perlin.offset.x + (float)1000000) / (this.heightmap_tool.perlin.frequency / num4), (num2 + this.heightmap_tool.perlin.offset.y + (float)1000000) / (this.heightmap_tool.perlin.frequency / num4)) - 0.5f) / num4;
							num4 *= (float)2;
						}
						num11 = this.heightmap_tool.perlin.precurve.curve.Evaluate(num11) * (float)65535;
						if (num11 < (float)0)
						{
							num11 = (float)0;
						}
						if (num11 > (float)65535)
						{
							num11 = (float)65535;
						}
						num10 = (ushort)num11;
						num7 = (int)((uint)num10 >> 8);
						num8 = (int)num10 - (num7 << 8);
						byte[] arg_66A_0 = this.heightmap_tool.raw_save_file.bytes;
						int num14;
						num9 = (num14 = num9) + 1;
						arg_66A_0[num14] = (byte)num8;
						byte[] arg_68B_0 = this.heightmap_tool.raw_save_file.bytes;
						int num15;
						num9 = (num15 = num9) + 1;
						arg_68B_0[num15] = (byte)num7;
					}
				}
			}
		}
	}

	public override bool generate_pattern_start()
	{
		if (this.pattern_tool.clear)
		{
			for (int i = 0; i < this.pattern_tool.output_texture.height; i++)
			{
				for (int j = 0; j < this.pattern_tool.output_texture.width; j++)
				{
					this.pattern_tool.output_texture.SetPixel(j, i, new Color((float)0, (float)0, (float)0));
				}
			}
		}
		this.pattern_tool.place_total = 0;
		int arg_1B4_0;
		for (int k = 0; k < this.pattern_tool.patterns.Count; k++)
		{
			if (!this.pattern_tool.patterns[k].input_texture)
			{
				arg_1B4_0 = 0;
				return arg_1B4_0 != 0;
			}
			this.pattern_tool.patterns[k].pattern_placed.Clear();
			this.pattern_tool.patterns[k].placed_max = false;
			this.pattern_tool.place_total = this.pattern_tool.place_total + this.pattern_tool.patterns[k].place_max;
			this.pattern_tool.patterns[k].width = (float)(this.pattern_tool.patterns[k].input_texture.width / this.pattern_tool.patterns[k].count_x);
			this.pattern_tool.patterns[k].height = (float)(this.pattern_tool.patterns[k].input_texture.height / this.pattern_tool.patterns[k].count_y);
		}
		arg_1B4_0 = 1;
		return arg_1B4_0 != 0;
	}

	public override bool generate_pattern()
	{
		bool result = true;
		this.pick_pattern();
		this.draw_pattern();
		for (int i = 0; i < this.pattern_tool.patterns.Count; i++)
		{
			if (!this.pattern_tool.patterns[i].placed_max && this.pattern_tool.patterns[i].active)
			{
				result = false;
			}
		}
		return result;
	}

	public override void pick_pattern()
	{
		int index = 0;
		do
		{
			index = UnityEngine.Random.Range(0, this.pattern_tool.patterns.Count);
		}
		while (this.pattern_tool.patterns[index].placed_max || !this.pattern_tool.patterns[index].active);
		this.pattern_tool.current_pattern = this.pattern_tool.patterns[index];
		this.pattern_tool.current_pattern.current_x = (float)UnityEngine.Random.Range(0, this.pattern_tool.current_pattern.count_x);
		this.pattern_tool.current_pattern.current_y = (float)UnityEngine.Random.Range(0, this.pattern_tool.current_pattern.count_y);
		this.pattern_tool.current_pattern.rotation = UnityEngine.Random.Range(this.pattern_tool.current_pattern.rotation_start, this.pattern_tool.current_pattern.rotation_end);
		this.pattern_tool.current_pattern.width2 = this.pattern_tool.current_pattern.width / (float)2;
		this.pattern_tool.current_pattern.height2 = this.pattern_tool.current_pattern.height / (float)2;
		this.pattern_tool.current_pattern.start_x = this.pattern_tool.current_pattern.current_x * this.pattern_tool.current_pattern.width;
		this.pattern_tool.current_pattern.start_y = this.pattern_tool.current_pattern.current_y * this.pattern_tool.current_pattern.height;
		this.pattern_tool.current_pattern.scale.x = (float)1 / UnityEngine.Random.Range(this.pattern_tool.current_pattern.scale_start.x, this.pattern_tool.current_pattern.scale_end.x);
		float num = this.pattern_tool.current_pattern.scale_end.x - this.pattern_tool.current_pattern.scale_start.x;
		float num2 = this.pattern_tool.current_pattern.scale.x - this.pattern_tool.current_pattern.scale_start.x;
		float num3 = num2 / num * (float)100;
		this.pattern_tool.current_pattern.scale.y = this.pattern_tool.current_pattern.scale.x;
		float num4 = this.pattern_tool.current_pattern.scale_end.y - this.pattern_tool.current_pattern.scale_start.y;
		float num5 = this.pattern_tool.current_pattern.scale.y - this.pattern_tool.current_pattern.scale_start.y;
		float num6 = num5 / num4 * (float)100;
		float num7 = Mathf.Abs(num3 - num6);
	}

	public override void draw_pattern()
	{
		if (this.pattern_tool.current_pattern.pattern_placed.Count >= this.pattern_tool.current_pattern.place_max)
		{
			this.pattern_tool.current_pattern.placed_max = true;
		}
		else
		{
			Vector2 item = default(Vector2);
			Color color = default(Color);
			Color color2 = default(Color);
			Vector2 vector = default(Vector2);
			Vector2 vector2 = default(Vector2);
			item.x = UnityEngine.Random.Range((float)0 - this.pattern_tool.current_pattern.width, (float)this.pattern_tool.output_texture.width);
			item.y = UnityEngine.Random.Range((float)0 - this.pattern_tool.current_pattern.height, (float)this.pattern_tool.output_texture.height);
			float rotation = this.pattern_tool.current_pattern.rotation;
			for (float num = (float)0; num < this.pattern_tool.current_pattern.height + this.pattern_tool.current_pattern.height2; num += this.pattern_tool.current_pattern.scale.y)
			{
				for (float num2 = (float)0; num2 < this.pattern_tool.current_pattern.width + this.pattern_tool.current_pattern.width2; num2 += this.pattern_tool.current_pattern.scale.x)
				{
					vector2.x = num2 / this.pattern_tool.current_pattern.scale.x + item.x;
					vector2.y = num / this.pattern_tool.current_pattern.scale.y + item.y;
					if (vector2.x < this.pattern_tool.output_resolution.x && vector2.y < this.pattern_tool.output_resolution.y && vector2.x >= (float)0 && vector2.y >= (float)0)
					{
						vector.x = num2 + this.pattern_tool.current_pattern.start_x - this.pattern_tool.current_pattern.width2 / (float)2;
						vector.y = num + this.pattern_tool.current_pattern.start_y - this.pattern_tool.current_pattern.height2 / (float)2;
						vector = this.calc_rotation_pixel(vector.x, vector.y, this.pattern_tool.current_pattern.start_x + this.pattern_tool.current_pattern.width / (float)2, this.pattern_tool.current_pattern.start_y + this.pattern_tool.current_pattern.height / (float)2, this.pattern_tool.current_pattern.rotation);
						if (vector.x - this.pattern_tool.current_pattern.start_x >= (float)0 && vector.x - this.pattern_tool.current_pattern.start_x <= this.pattern_tool.current_pattern.width)
						{
							if (vector.y - this.pattern_tool.current_pattern.start_y >= (float)0 && vector.y - this.pattern_tool.current_pattern.start_y <= this.pattern_tool.current_pattern.height)
							{
								color = this.pattern_tool.current_pattern.input_texture.GetPixel((int)vector.x, (int)vector.y) * this.pattern_tool.current_pattern.color;
								color[0] = color[0] * this.pattern_tool.current_pattern.strength;
								color[1] = color[1] * this.pattern_tool.current_pattern.strength;
								color[2] = color[2] * this.pattern_tool.current_pattern.strength;
								color2 = this.pattern_tool.output_texture.GetPixel((int)vector2.x, (int)vector2.y);
								bool flag = false;
								for (int i = 0; i < this.pattern_tool.current_pattern.precolor_range.color_range.Count; i++)
								{
									if (this.color_in_range(color, this.pattern_tool.current_pattern.precolor_range.color_range[i].color_start, this.pattern_tool.current_pattern.precolor_range.color_range[i].color_end))
									{
										if (!this.pattern_tool.current_pattern.precolor_range.color_range[i].invert)
										{
											flag = true;
										}
									}
									else if (this.pattern_tool.current_pattern.precolor_range.color_range[i].invert)
									{
										flag = true;
									}
								}
								condition_output_enum output = this.pattern_tool.current_pattern.output;
								if (output == condition_output_enum.add)
								{
									color += color2;
								}
								else if (output == condition_output_enum.subtract)
								{
									color = color2 - color;
								}
								else if (output != condition_output_enum.change)
								{
									if (output == condition_output_enum.multiply)
									{
										color = color2 * color;
									}
									else if (output == condition_output_enum.divide)
									{
										color[0] = color2[0] / color[0];
										color[1] = color2[1] / color[1];
										color[2] = color2[2] / color[2];
									}
									else if (output == condition_output_enum.difference)
									{
										color[0] = Mathf.Abs(color2[0] - color[0]);
										color[1] = Mathf.Abs(color2[1] - color[1]);
										color[2] = Mathf.Abs(color2[2] - color[2]);
									}
									else if (output == condition_output_enum.average)
									{
										color = (color + color2) / (float)2;
									}
									else if (output == condition_output_enum.max)
									{
										if (color[0] < color2[0] && color[1] < color2[1] && color[2] < color2[2])
										{
											flag = false;
										}
									}
									else if (output == condition_output_enum.max)
									{
										if (color[0] > color2[0] && color[1] > color2[1] && color[2] > color2[2])
										{
											flag = false;
										}
									}
								}
								if (flag)
								{
									this.pattern_tool.output_texture.SetPixel((int)vector2.x, (int)vector2.y, color);
								}
							}
						}
					}
				}
			}
			this.pattern_tool.current_pattern.pattern_placed.Add(item);
		}
	}

	public override float calc_floor(float number)
	{
		int num = (int)number;
		return (float)num;
	}

	public override void create_object_line(object_output_class object_output)
	{
		Transform transform = new GameObject().transform;
		float num = 0f;
		int num2 = 0;
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		for (int i = 0; i < object_output.placed_reference.line_placement.line_list[0].points.Count - 1; i++)
		{
			if (i == object_output.placed_reference.line_placement.line_list[0].points.Count - 1)
			{
				vector2 = object_output.placed_reference.line_placement.line_list[0].points[0];
			}
			else
			{
				vector2 = object_output.placed_reference.line_placement.line_list[0].points[i + 1];
			}
			if (i == 0)
			{
				transform.position = object_output.placed_reference.line_placement.line_list[0].points[0];
			}
			num = Vector3.Distance(transform.position, vector2);
			num2 = (int)(num / (object_output.placed_reference.@object[0].mesh_size.x * (float)2));
			transform.rotation = Quaternion.LookRotation(vector2 - transform.position);
			for (int j = 0; j < num2; j++)
			{
				float y = this.terrains[0].terrain.SampleHeight(transform.position);
				Vector3 vector3 = transform.position;
				float num3 = vector3.y = y;
				Vector3 vector4 = transform.position = vector3;
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(object_output.placed_reference.@object[0].object1, transform.position, transform.rotation);
				gameObject.transform.position = gameObject.transform.position + gameObject.transform.forward * object_output.placed_reference.@object[0].mesh_size.x;
				gameObject.transform.Rotate((float)0, (float)90, (float)0);
				gameObject.transform.parent = object_output.placed_reference.@object[0].parent;
				transform.position += transform.forward * (object_output.placed_reference.@object[0].mesh_size.x * (float)2);
			}
		}
		UnityEngine.Object.DestroyImmediate(transform.gameObject);
	}

	public override void erosion_alive()
	{
		for (int i = 0; i < this.erosion_list.Count; i++)
		{
			if (this.erosion_list[i])
			{
				this.erosion_list[i].erosion();
			}
			else
			{
				this.erosion_list.RemoveAt(i);
				i--;
			}
		}
	}

	public override void normalize_splat(terrain_class preterrain1)
	{
		int length = preterrain1.terrain.terrainData.splatPrototypes.Length;
		preterrain1.map = preterrain1.terrain.terrainData.GetAlphamaps(0, 0, preterrain1.terrain.terrainData.alphamapResolution, preterrain1.terrain.terrainData.alphamapResolution);
		float num = 0f;
		int num2 = 0;
		while ((float)num2 < this.preterrain.splatmap_resolution)
		{
			int num3 = 0;
			while ((float)num3 < this.preterrain.splatmap_resolution)
			{
				num = (float)0;
				for (int i = 0; i < length; i++)
				{
					num += preterrain1.map[num3, num2, i];
				}
				for (int i = 0; i < length; i++)
				{
					preterrain1.map[num3, num2, i] = preterrain1.map[num3, num2, i] / num;
				}
				num3++;
			}
			num2++;
		}
		preterrain1.terrain.terrainData.SetAlphamaps(0, 0, preterrain1.map);
	}

	public override float calc_terrain_angle(terrain_class preterrain1, float x, float y, int smooth)
	{
		Vector3 size = preterrain1.terrain.terrainData.size;
		float num = (float)preterrain1.terrain.terrainData.heightmapResolution;
		float num2 = size.x / (num - (float)1);
		short num3 = (short)(x / num2);
		short num4 = (short)(y / num2);
		bool flag = false;
		short num5 = (short)((int)num3 - smooth);
		short num6 = (short)((int)num4 + smooth);
		short num7 = (short)preterrain1.tile_x;
		short num8 = (short)preterrain1.tile_z;
		short num9 = (short)((int)num3 + smooth);
		short num10 = (short)((int)num4 + smooth);
		short num11 = num7;
		short num12 = num8;
		short num13 = (short)((int)num3 - smooth);
		short num14 = (short)((int)num4 - smooth);
		short num15 = num7;
		short num16 = num8;
		short num17 = (short)((int)num3 + smooth);
		short num18 = (short)((int)num4 - smooth);
		short num19 = num7;
		short num20 = num8;
		if (num5 < 0)
		{
			if (preterrain1.tile_x > (float)0)
			{
				num7 = (short)((int)num7 - 1);
				num15 = (short)((int)num15 - 1);
				num5 = (short)(num - (float)1 + (float)num5);
				num13 = num5;
				flag = true;
			}
			else
			{
				num9 = (short)((int)num9 - (int)num5);
				num17 = num9;
				num5 = 0;
				num13 = num5;
			}
		}
		else if ((float)num9 > num - (float)1)
		{
			if (preterrain1.tile_x < preterrain1.tiles.x - (float)1)
			{
				num11 = (short)((int)num11 + 1);
				num19 = (short)((int)num19 + 1);
				num9 = (short)((float)num9 - (num - (float)1));
				num17 = num9;
				flag = true;
			}
			else
			{
				num5 = (short)((float)num5 - ((float)num9 - (num - (float)1)));
				num13 = num5;
				num9 = (short)(num - (float)1);
				num17 = num9;
			}
		}
		if (num14 < 0)
		{
			if (preterrain1.tile_z > (float)0)
			{
				num16 = (short)((int)num16 - 1);
				num20 = (short)((int)num20 - 1);
				num14 = (short)(num - (float)1 + (float)num14);
				num18 = num14;
				flag = true;
			}
			else
			{
				num6 = (short)((int)num6 - (int)num14);
				num10 = num6;
				num14 = 0;
				num18 = num14;
			}
		}
		else if ((float)num6 > num - (float)1)
		{
			if (preterrain1.tile_z < preterrain1.tiles.y - (float)1)
			{
				num8 = (short)((int)num8 + 1);
				num12 = (short)((int)num12 + 1);
				num6 = (short)((float)num6 - (num - (float)1));
				num10 = num6;
				flag = true;
			}
			else
			{
				num14 = (short)((float)num14 - ((float)num6 - (num - (float)1)));
				num18 = num14;
				num6 = (short)(num - (float)1);
				num10 = num6;
			}
		}
		float num21 = 0f;
		float num22 = 0f;
		float num23 = 0f;
		float num24 = 0f;
		if (flag)
		{
			num21 = this.terrains[this.find_terrain((int)num7, (int)num8)].terrain.terrainData.GetHeight((int)num5, (int)num6);
			num22 = this.terrains[this.find_terrain((int)num11, (int)num12)].terrain.terrainData.GetHeight((int)num9, (int)num10);
			num23 = this.terrains[this.find_terrain((int)num15, (int)num16)].terrain.terrainData.GetHeight((int)num13, (int)num14);
			num24 = this.terrains[this.find_terrain((int)num19, (int)num20)].terrain.terrainData.GetHeight((int)num17, (int)num18);
		}
		else
		{
			num21 = preterrain1.terrain.terrainData.GetHeight((int)num5, (int)num6);
			num22 = preterrain1.terrain.terrainData.GetHeight((int)num9, (int)num10);
			num23 = preterrain1.terrain.terrainData.GetHeight((int)num13, (int)num14);
			num24 = preterrain1.terrain.terrainData.GetHeight((int)num17, (int)num18);
		}
		float num25 = 0f;
		float num26 = 0f;
		if (num21 > num22)
		{
			num26 = num21;
			num25 = num22;
		}
		else
		{
			num26 = num22;
			num25 = num21;
		}
		if (num23 > num26)
		{
			num26 = num23;
		}
		if (num24 > num26)
		{
			num26 = num24;
		}
		if (num23 < num25)
		{
			num25 = num23;
		}
		if (num24 < num25)
		{
			num25 = num24;
		}
		float num27 = Mathf.Round((num26 - num25) * (float)(101 - this.settings.round_angle)) / (float)(101 - this.settings.round_angle);
		float num28 = size.x / num * ((float)smooth * 2f + (float)1);
		return Mathf.Atan(num27 / num28) * this.Rad2Deg;
	}

	public override int find_terrain_by_position(Vector3 position)
	{
		int arg_59_0;
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].rect.Contains(new Vector2(position.x, position.z)))
			{
				arg_59_0 = i;
				return arg_59_0;
			}
		}
		arg_59_0 = -1;
		return arg_59_0;
	}

	public override int find_terrain(int tile_x, int tile_y)
	{
		int arg_7B_0;
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].tile_x == (float)tile_x && this.terrains[i].tile_z == (float)tile_y)
			{
				arg_7B_0 = i;
				return arg_7B_0;
			}
		}
		if (!this.generate_error)
		{
			Debug.Log("The order of the terrains has been changed! If you have more terrains please shift click <Fit All> in Terrain List -> Data -> Size.");
			this.generate_error = true;
			this.reset_terrains_tiles(this.script_base);
		}
		arg_7B_0 = 0;
		return arg_7B_0;
	}

	public override void calc_terrain_needed_tiles()
	{
		this.terrain_instances = (int)(Mathf.Pow((float)this.terrain_tiles, (float)2) - (float)(this.terrains.Count - 1));
	}

	public override void calc_terrain_one_more_tile()
	{
		this.terrain_tiles = (int)(this.terrains[0].tiles.x + (float)1);
		this.calc_terrain_needed_tiles();
	}

	public override tile_class calc_terrain_tile(int terrain_index, tile_class tiles)
	{
		tile_class tile_class = new tile_class();
		tile_class.y = terrain_index / tiles.x;
		tile_class.x = terrain_index - tile_class.y * tiles.x;
		return tile_class;
	}

	public override int calc_terrain_index2(tile_class tile, tile_class tiles)
	{
		return tile.x + tile.y * tiles.x;
	}

	public override int find_terrain_by_name(Terrain terrain)
	{
		string name = terrain.name;
		int arg_66_0;
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain && this.terrains[i].terrain.name == name)
			{
				arg_66_0 = i;
				return arg_66_0;
			}
		}
		arg_66_0 = -1;
		return arg_66_0;
	}

	public override void reset_terrains_tiles(terraincomposer_save script_save)
	{
		for (int i = 0; i < this.terrains.Count; i++)
		{
			script_save.terrains[i].tiles = new Vector2((float)1, (float)1);
			script_save.terrains[i].tile_x = (float)0;
			script_save.terrains[i].tile_z = (float)0;
		}
	}

	public override string sec_to_timeMin(float seconds, bool display_seconds)
	{
		int num = (int)(seconds / (float)60);
		seconds -= (float)(num * 60);
		string arg_B2_0;
		if (num == 0)
		{
			arg_B2_0 = seconds.ToString("F2");
		}
		else
		{
			int num2 = (int)seconds;
			seconds -= (float)num2;
			int num3 = (int)(seconds * (float)100);
			arg_B2_0 = ((!display_seconds) ? (num.ToString() + ":" + num2.ToString("D2")) : (num.ToString() + ":" + num2.ToString("D2") + "." + num3.ToString("D2")));
		}
		return arg_B2_0;
	}

	public override void get_rtp_lodmanager()
	{
		this.RTP_LODmanager1 = GameObject.Find("_RTP_LODmanager");
		if (this.RTP_LODmanager1)
		{
			this.rtpLod_script = this.RTP_LODmanager1.GetComponent("RTP_LODmanager");
		}
	}

	public override int generate_object(prelayer_class prelayer3)
	{
		this.frames = (float)1 / (Time.realtimeSinceStartup - this.auto_speed_time);
		this.auto_speed_time = Time.realtimeSinceStartup;
		this.break_x = false;
		this.row_object_count = 0;
		prelayer3.counter_y = prelayer3.y;
		int arg_FFF_0;
		while (prelayer3.counter_y >= prelayer3.y - (float)this.generate_speed * this.prelayer.prearea.step.y)
		{
			this.generate_call_time = Time.realtimeSinceStartup;
			float y = prelayer3.y;
			int i = 0;
			if (prelayer3.counter_y >= prelayer3.prearea.area.yMin)
			{
				if (this.generate_world_mode || prelayer3.index > 0)
				{
					for (i = 0; i < this.terrains.Count; i++)
					{
						if (this.terrains[i].rect.Contains(new Vector2(this.terrains[i].prearea.area.x, y)))
						{
							this.terrains[i].on_row = true;
						}
						else
						{
							this.terrains[i].on_row = false;
						}
					}
				}
				prelayer3.x = prelayer3.prearea.area.x + prelayer3.break_x_value;
				while (prelayer3.x <= prelayer3.prearea.area.xMax)
				{
					float x = prelayer3.x;
					float num = 0f;
					float num2 = 0f;
					float num3 = 0f;
					float num4 = prelayer3.counter_y;
					if (!this.generate_world_mode && prelayer3.index <= 0)
					{
						goto IL_348;
					}
					bool flag = true;
					for (i = 0; i < this.terrains.Count; i++)
					{
						if (this.terrains[i].rect.Contains(new Vector2(x, num4)))
						{
							flag = false;
							this.preterrain = this.terrains[i];
							break;
						}
					}
					if (!flag)
					{
						if (prelayer3.prearea.rotation_active)
						{
							Vector2 vector = this.calc_rotation_pixel(x, num4, prelayer3.prearea.center.x, prelayer3.prearea.center.y, prelayer3.prearea.rotation.y);
							x = vector.x;
							num4 = vector.y;
							goto IL_348;
						}
						goto IL_348;
					}
					IL_F37:
					prelayer3.x += prelayer3.prearea.step.x;
					continue;
					IL_348:
					this.local_x = x - this.preterrain.rect.x;
					this.local_y = num4 - this.preterrain.rect.y;
					this.local_x_rot = x - this.preterrain.rect.x;
					this.local_y_rot = num4 - this.preterrain.rect.y;
					if (this.settings.showTerrains)
					{
						this.degree = this.calc_terrain_angle(this.preterrain, this.local_x_rot, this.local_y_rot, this.settings.smooth_angle) * this.settings.global_degree_strength + this.settings.global_degree_level;
						this.height = this.preterrain.terrain.terrainData.GetHeight((int)(this.local_x_rot / this.preterrain.heightmap_conversion.x), (int)(this.local_y_rot / this.preterrain.heightmap_conversion.y)) / this.preterrain.size.y * this.settings.global_height_strength + this.settings.global_height_level;
					}
					this.random_range = UnityEngine.Random.Range((float)0, 1000f);
					int j;
					for (j = 0; j < prelayer3.layer.Count; j++)
					{
						this.current_layer = prelayer3.layer[j];
						this.filter_value = (float)0;
						this.filter_strength = (float)1;
						this.layer_x = this.local_x_rot;
						this.layer_y = this.local_y_rot;
					}
					for (int k = 0; k < this.current_layer.prefilter.filter_index.Count; k++)
					{
						this.calc_filter_value(this.filter[this.current_layer.prefilter.filter_index[k]], num4, x);
					}
					if (this.subfilter_value * this.current_layer.strength * this.filter_strength <= (float)0)
					{
						goto IL_F37;
					}
					int index = (int)(this.current_layer.object_output.object_value.curve.Evaluate(this.filter_value) * (float)this.current_layer.object_output.@object.Count);
					object_class object_class = this.current_layer.object_output.@object[index];
					if (num4 == (float)0 || x == (float)0)
					{
						UnityEngine.Random.seed = (int)((num4 + (float)1) * (x + (float)1) + (float)(10000000 * (j + 1)));
					}
					else
					{
						UnityEngine.Random.seed = (int)(num4 * x * (float)(j + 1));
					}
					this.random_range2 = UnityEngine.Random.Range((float)0, 1f);
					if (this.random_range2 > this.subfilter_value * this.current_layer.strength * this.filter_strength)
					{
						goto IL_F37;
					}
					this.place = true;
					Quaternion lhs = Quaternion.identity;
					int num5 = 0;
					this.position = new Vector3(x, (float)0, num4);
					Vector3 position_start = object_class.position_start;
					Vector3 position_end = object_class.position_end;
					Vector3 vector2 = default(Vector3);
					vector2.x = UnityEngine.Random.Range(position_start.x, position_end.x);
					vector2.y = UnityEngine.Random.Range(position_start.y, position_end.y);
					vector2.z = UnityEngine.Random.Range(position_start.z, position_end.z);
					if (object_class.random_position)
					{
						vector2.x += UnityEngine.Random.Range(-prelayer3.prearea.step.x, prelayer3.prearea.step.x);
						vector2.z += UnityEngine.Random.Range(-prelayer3.prearea.step.y, prelayer3.prearea.step.y);
					}
					this.position += vector2;
					if (object_class.terrain_rotate)
					{
						Vector3 interpolatedNormal = this.preterrain.terrain.terrainData.GetInterpolatedNormal((this.local_x_rot + vector2.x) / this.preterrain.size.x, (this.local_y_rot + vector2.y) / this.preterrain.size.z);
						interpolatedNormal.x = interpolatedNormal.x / (float)3 * (float)2;
						interpolatedNormal.z = interpolatedNormal.z / (float)3 * (float)2;
						lhs = Quaternion.FromToRotation(Vector3.up, interpolatedNormal);
					}
					lhs *= Quaternion.AngleAxis(UnityEngine.Random.Range(object_class.rotation_start.x, object_class.rotation_end.x), Vector3.right);
					lhs *= Quaternion.AngleAxis(UnityEngine.Random.Range(object_class.rotation_start.y, object_class.rotation_end.y), Vector3.up);
					lhs *= Quaternion.AngleAxis(UnityEngine.Random.Range(object_class.rotation_start.z, object_class.rotation_end.z), Vector3.forward);
					lhs *= Quaternion.Euler(object_class.parent_rotation);
					if (object_class.terrain_height)
					{
						this.height_interpolated = this.preterrain.terrain.terrainData.GetInterpolatedHeight((this.local_x_rot + vector2.x) / this.preterrain.size.x, (this.local_y_rot + vector2.z) / this.preterrain.size.z);
						this.position.y = this.height_interpolated + this.preterrain.terrain.transform.position.y + vector2.y;
					}
					float num6 = object_class.scale_end.x - object_class.scale_start.x;
					this.scale.x = UnityEngine.Random.Range(object_class.scale_start.x, object_class.scale_end.x);
					float num7 = this.scale.x - object_class.scale_start.x;
					float num8 = num7 / num6;
					float num9 = object_class.scale_end.y - object_class.scale_start.y;
					float num10 = num9 * num8 - object_class.unlink_y * num7 + object_class.scale_start.y;
					if (num10 < object_class.scale_start.y)
					{
						num10 = object_class.scale_start.y;
					}
					float num11 = num9 * num8 + object_class.unlink_y * num7 + object_class.scale_start.y;
					if (num11 > object_class.scale_end.y)
					{
						num11 = object_class.scale_end.y;
					}
					this.scale.y = UnityEngine.Random.Range(num10, num11);
					float num12 = object_class.scale_end.z - object_class.scale_start.z;
					float num13 = num12 * num8 - object_class.unlink_z * num7 + object_class.scale_start.z;
					if (num13 < object_class.scale_start.z)
					{
						num13 = object_class.scale_start.z;
					}
					float num14 = num12 * num8 + object_class.unlink_z * num7 + object_class.scale_start.z;
					if (num14 > object_class.scale_end.z)
					{
						num14 = object_class.scale_end.z;
					}
					this.scale.z = UnityEngine.Random.Range(num13, num14);
					if (object_class.raycast && Physics.SphereCast(this.position + new Vector3((float)0, object_class.cast_height, (float)0), object_class.ray_radius, object_class.ray_direction, out this.hit, object_class.ray_length))
					{
						this.layerHit = (int)Mathf.Pow((float)2, (float)this.hit.transform.gameObject.layer);
						if ((this.layerHit & object_class.layerMask) != 0)
						{
							this.position.y = this.hit.point.y;
						}
					}
					if (object_class.pivot_center)
					{
						this.position.y = this.position.y + this.scale.y / (float)2;
					}
					this.scale *= this.current_layer.object_output.scale;
					if (!this.place)
					{
						goto IL_F37;
					}
					float num15 = this.preterrain.size.z;
					num15 /= this.preterrain.heightmap_resolution;
					object_class.placed++;
					object_class.placed_prelayer++;
					this.current_layer.object_output.placed = this.current_layer.object_output.placed + 1;
					this.row_object_count++;
					if (object_class.prelayer_created && this.prelayers[object_class.prelayer_index].prearea.active)
					{
						this.set_object_child(object_class, lhs.eulerAngles);
						prelayer3.x += prelayer3.prearea.step.x;
						prelayer3.y = prelayer3.counter_y;
						if (prelayer3.x <= this.prelayer.prearea.area.xMax)
						{
							prelayer3.break_x_value = prelayer3.x - prelayer3.prearea.area.x;
						}
						else
						{
							prelayer3.y -= prelayer3.prearea.step.y;
							prelayer3.break_x_value = (float)0;
						}
						this.prelayer_stack.Add(object_class.prelayer_index);
						this.prelayer = this.prelayers[object_class.prelayer_index];
						this.prelayer.prearea.area.x = this.position.x + this.prelayer.prearea.area_old.x * this.scale.x;
						this.prelayer.prearea.area.y = this.position.z + this.prelayer.prearea.area_old.y * this.scale.z;
						this.prelayer.prearea.area.width = this.prelayer.prearea.area_old.width * this.scale.x;
						this.prelayer.prearea.area.height = this.prelayer.prearea.area_old.height * this.scale.z;
						if (lhs.y != (float)0)
						{
							this.prelayer.prearea.rotation = lhs.eulerAngles;
							this.prelayer.prearea.rotation_active = true;
						}
						this.prelayer.prearea.step.y = Mathf.Sqrt(Mathf.Pow(this.prelayer.prearea.step_old.x, (float)2) + Mathf.Pow(this.prelayer.prearea.step_old.y, (float)2)) / (float)2;
						this.prelayer.prearea.step.x = this.prelayer.prearea.step.y;
						this.prelayer.prearea.center = new Vector2(this.position.x, this.position.z);
						this.prelayer.y = this.prelayer.prearea.area.yMax;
						arg_FFF_0 = 3;
						return arg_FFF_0;
					}
					goto IL_F37;
				}
				prelayer3.counter_y -= this.prelayer.prearea.step.y;
				continue;
			}
			if (this.prelayer_stack.Count > 1)
			{
				this.prelayer_stack.RemoveAt(this.prelayer_stack.Count - 1);
				this.prelayer = this.prelayers[this.prelayer_stack[this.prelayer_stack.Count - 1]];
				this.generate_error = false;
				arg_FFF_0 = 2;
			}
			else
			{
				if (this.generate_world_mode)
				{
					this.generate = false;
					for (i = 0; i < this.terrains.Count; i++)
					{
						this.terrain_apply(this.terrains[i]);
					}
				}
				this.generate_time = Time.realtimeSinceStartup - this.generate_time_start;
				if (this.generate)
				{
					prelayer3.count_terrain++;
					if (this.find_terrain(false))
					{
						this.preterrain = this.terrains[prelayer3.count_terrain];
						this.generate_terrain_start();
					}
					else
					{
						this.generate = false;
						this.generateDone = true;
					}
				}
				this.generate_error = false;
				arg_FFF_0 = 2;
			}
			return arg_FFF_0;
		}
		prelayer3.y -= (float)(this.generate_speed + 1) * this.prelayer.prearea.step.y;
		this.generate_time = Time.realtimeSinceStartup - this.generate_time_start;
		arg_FFF_0 = 1;
		return arg_FFF_0;
	}

	public override string convert_terrains_to_mesh()
	{
		List<vertex_class> list = new List<vertex_class>();
		int num = 0;
		int num2 = 0;
		int i = 0;
		List<Mesh> list2 = new List<Mesh>();
		int j = 0;
		int k = 0;
		terrain_class terrain_class = this.terrains[0];
		Vector3 vector = default(Vector3);
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		i = 0;
		string arg_514_0;
		while (i < this.terrains.Count)
		{
			if (!this.terrains[i].terrain)
			{
				arg_514_0 = "All terrains must be assigned";
			}
			else if (!this.terrains[i].terrain.terrainData)
			{
				arg_514_0 = "All terrainData's must be assigned";
			}
			else
			{
				if (this.terrains[i].terrain.terrainData.heightmapResolution <= 129)
				{
					i++;
					continue;
				}
				arg_514_0 = "Can only convert 33, 65 and 129 heightmap resolution at the moment";
			}
			return arg_514_0;
		}
		GameObject gameObject = new GameObject();
		gameObject.name = "Terrains Mesh";
		for (i = 0; i < this.terrains.Count; i++)
		{
			num2 = this.terrains[i].terrain.terrainData.heightmapResolution;
			vector.x = terrain_class.terrain.terrainData.size.x / (float)(num2 - 1);
			vector.z = terrain_class.terrain.terrainData.size.z / (float)(num2 - 1);
			vector.y = terrain_class.terrain.terrainData.size.y;
			this.terrains[i].index = i;
			list2.Add(this.convert_terrain_to_mesh(this.terrains[i], gameObject));
			list.Add(new vertex_class());
			list[i].vertices = list2[i].vertices;
		}
		Vector3[] array = new Vector3[list2[0].vertices.Length];
		Vector4[] array2 = new Vector4[list2[0].vertices.Length];
		Vector3 size = default(Vector3);
		for (i = 0; i < this.terrains.Count; i++)
		{
			num2 = this.terrains[i].terrain.terrainData.heightmapResolution;
			size = this.terrains[i].terrain.terrainData.size;
			for (k = 1; k < num2 - 1; k++)
			{
				for (j = 1; j < num2 - 1; j++)
				{
					num = k * num2 + j;
					normal_class normal_class = this.calc_normal(vector.x, list[i], num, num2);
					array[num] = normal_class.normal;
					array2[num] = normal_class.tangent;
				}
			}
			for (k = 0; k < num2; k++)
			{
				num = k * num2;
				normal_class normal_class;
				if (k > 0 && k < num2 - 1)
				{
					normal_class = this.calc_normal_border_left(this.terrains[i], vector.x, list, k, num2, size);
					array[num] = normal_class.normal;
					array2[num] = normal_class.tangent;
				}
				if (k > 0 && k < num2 - 1)
				{
					normal_class = this.calc_normal_border_right(this.terrains[i], vector.x, list, k, num2, size);
					array[num + num2 - 1] = normal_class.normal;
					array2[num + num2 - 1] = normal_class.tangent;
				}
				normal_class = this.calc_normal_border_top(this.terrains[i], vector.x, list, k, num2, size);
				array[k + num2 * (num2 - 1)] = normal_class.normal;
				array2[k + num2 * (num2 - 1)] = normal_class.tangent;
				normal_class = this.calc_normal_border_bottom(this.terrains[i], vector.x, list, k, num2, size);
				array[k] = normal_class.normal;
				array2[k] = normal_class.tangent;
			}
			list2[i].normals = array;
			list2[i].tangents = array2;
		}
		arg_514_0 = "Converting " + this.terrains.Count.ToString() + " terrains to meshes done";
		return arg_514_0;
	}

	public override Mesh convert_terrain_to_mesh(terrain_class preterrain1, GameObject parent)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		Mesh arg_325_0;
		if (preterrain1.terrain)
		{
			if (preterrain1.terrain.terrainData)
			{
				int heightmapResolution = preterrain1.terrain.terrainData.heightmapResolution;
				int num = heightmapResolution * heightmapResolution;
				Vector3[] array = new Vector3[num];
				int num2 = 0;
				Vector2[] array2 = new Vector2[array.Length];
				float num3 = preterrain1.terrain.terrainData.size.x / (float)(heightmapResolution - 1);
				float num4 = preterrain1.terrain.terrainData.size.z / (float)(heightmapResolution - 1);
				float y = preterrain1.terrain.terrainData.size.y;
				int i = 0;
				int j = 0;
				float y2 = preterrain1.terrain.terrainData.size.y;
				Vector3 vector = preterrain1.terrain.transform.position;
				for (i = 0; i < heightmapResolution; i++)
				{
					for (j = 0; j < heightmapResolution; j++)
					{
						array[num2] = new Vector3((float)j * num3, preterrain1.terrain.terrainData.GetHeight(j, i), (float)i * num4);
						Vector2[] arg_14A_0 = array2;
						int num5;
						num2 = (num5 = num2) + 1;
						arg_14A_0[num5] = new Vector2((float)j * 1f / (float)heightmapResolution, (float)i * 1f / (float)heightmapResolution);
					}
				}
				int[] array3 = new int[array.Length * 6];
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				for (i = 0; i < heightmapResolution - 1; i++)
				{
					for (j = 0; j < heightmapResolution - 1; j++)
					{
						num7 = i * heightmapResolution + j;
						int[] arg_1DB_0 = array3;
						int num9;
						num6 = (num9 = num6) + 1;
						arg_1DB_0[num9] = num7 + 1;
						int[] arg_1ED_0 = array3;
						int num10;
						num6 = (num10 = num6) + 1;
						arg_1ED_0[num10] = num7;
						int[] arg_201_0 = array3;
						int num11;
						num6 = (num11 = num6) + 1;
						arg_201_0[num11] = num7 + heightmapResolution;
						int[] arg_215_0 = array3;
						int num12;
						num6 = (num12 = num6) + 1;
						arg_215_0[num12] = num7 + 1;
						int[] arg_229_0 = array3;
						int num13;
						num6 = (num13 = num6) + 1;
						arg_229_0[num13] = num7 + heightmapResolution;
						int[] arg_23F_0 = array3;
						int num14;
						num6 = (num14 = num6) + 1;
						arg_23F_0[num14] = num7 + heightmapResolution + 1;
					}
				}
				GameObject gameObject = new GameObject();
				gameObject.transform.position = preterrain1.terrain.transform.position;
				gameObject.transform.parent = parent.transform;
				gameObject.name = preterrain1.name;
				Mesh mesh = new Mesh();
				MeshFilter meshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
				MeshRenderer meshRenderer = (MeshRenderer)gameObject.AddComponent(typeof(MeshRenderer));
				meshRenderer.material = this.settings.mesh_material;
				meshFilter.mesh = mesh;
				mesh.vertices = array;
				mesh.triangles = array3;
				mesh.uv = array2;
				float num15 = Time.realtimeSinceStartup - realtimeSinceStartup;
				float num16 = Time.realtimeSinceStartup - realtimeSinceStartup;
				arg_325_0 = mesh;
				return arg_325_0;
			}
		}
		arg_325_0 = null;
		return arg_325_0;
	}

	public override normal_class calc_normal(float space, vertex_class vertex, int pos, int resolution)
	{
		normal_class normal_class = new normal_class();
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		Vector3 vector5 = default(Vector3);
		Vector3 vector6 = default(Vector3);
		Vector3 vector7 = default(Vector3);
		Vector3 vector8 = default(Vector3);
		Vector3 vector9 = default(Vector3);
		Vector3 vector10 = default(Vector3);
		Vector4 vector11 = default(Vector4);
		float num = 0f;
		float num2 = 0f;
		vector = vertex.vertices[pos];
		vector2 = vertex.vertices[pos - resolution - 1];
		vector3 = vertex.vertices[pos - resolution];
		vector4 = vertex.vertices[pos - resolution + 1];
		vector5 = vertex.vertices[pos - 1];
		vector6 = vertex.vertices[pos + 1];
		vector7 = vertex.vertices[pos + resolution - 1];
		vector8 = vertex.vertices[pos + resolution];
		vector9 = vertex.vertices[pos + resolution + 1];
		vector10 = Vector3.Cross(vector2 - vector, vector3 - vector);
		vector10 += Vector3.Cross(vector3 - vector, vector4 - vector);
		vector10 += Vector3.Cross(vector5 - vector, vector2 - vector);
		vector10 += Vector3.Cross(vector4 - vector, vector6 - vector);
		vector10 += Vector3.Cross(vector7 - vector, vector5 - vector);
		vector10 += Vector3.Cross(vector6 - vector, vector9 - vector);
		vector10 += Vector3.Cross(vector8 - vector, vector7 - vector);
		vector10 += Vector3.Cross(vector9 - vector, vector8 - vector);
		vector10 = (vector10 / 8f).normalized;
		vector10.y *= (float)-1;
		normal_class.normal = vector10;
		num = vector5.y;
		num2 = vector6.y;
		normal_class.tangent = new Vector4(-space, num2 - num, (float)0, -space).normalized;
		return normal_class;
	}

	public override normal_class calc_normal_border_left(terrain_class preterrain1, float space, List<vertex_class> vertex, int y, int resolution, Vector3 size)
	{
		normal_class normal_class = new normal_class();
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		Vector3 vector5 = default(Vector3);
		Vector3 vector6 = default(Vector3);
		Vector3 vector7 = default(Vector3);
		Vector3 vector8 = default(Vector3);
		Vector3 vector9 = default(Vector3);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = true;
		bool flag6 = true;
		bool flag7 = true;
		Vector3 vector10 = Vector3.zero;
		Vector4 vector11 = default(Vector4);
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)0;
		vector = vertex[preterrain1.index].vertices[y * resolution];
		if (preterrain1.neighbor.left > -1)
		{
			vector2 = vertex[preterrain1.neighbor.left].vertices[resolution - 2 + (y - 1) * resolution] - new Vector3(size.x, (float)0, (float)0);
			vector5 = vertex[preterrain1.neighbor.left].vertices[resolution - 2 + y * resolution] - new Vector3(size.x, (float)0, (float)0);
			vector7 = vertex[preterrain1.neighbor.left].vertices[resolution - 2 + (y + 1) * resolution] - new Vector3(size.x, (float)0, (float)0);
		}
		else
		{
			flag = false;
			flag4 = false;
			flag5 = false;
		}
		vector3 = vertex[preterrain1.index].vertices[(y - 1) * resolution];
		vector4 = vertex[preterrain1.index].vertices[1 + (y - 1) * resolution];
		vector8 = vertex[preterrain1.index].vertices[(y + 1) * resolution];
		vector9 = vertex[preterrain1.index].vertices[1 + (y + 1) * resolution];
		vector6 = vertex[preterrain1.index].vertices[1 + y * resolution];
		if (flag && flag2)
		{
			vector10 = Vector3.Cross(vector2 - vector, vector3 - vector);
			num3 += (float)1;
		}
		if (flag2 && flag3)
		{
			vector10 += Vector3.Cross(vector3 - vector, vector4 - vector);
			num3 += (float)1;
		}
		if (flag4 && flag)
		{
			vector10 += Vector3.Cross(vector5 - vector, vector2 - vector);
			num3 += (float)1;
		}
		if (flag3)
		{
			vector10 += Vector3.Cross(vector4 - vector, vector6 - vector);
			num3 += (float)1;
		}
		if (flag5 && flag4)
		{
			vector10 += Vector3.Cross(vector7 - vector, vector5 - vector);
			num3 += (float)1;
		}
		if (flag7)
		{
			vector10 += Vector3.Cross(vector6 - vector, vector9 - vector);
			num3 += (float)1;
		}
		if (flag5 && flag6)
		{
			vector10 += Vector3.Cross(vector8 - vector, vector7 - vector);
			num3 += (float)1;
		}
		if (flag6 && flag7)
		{
			vector10 += Vector3.Cross(vector9 - vector, vector8 - vector);
			num3 += (float)1;
		}
		vector10 = (vector10 / num3).normalized;
		vector10.y *= (float)-1;
		normal_class.normal = vector10;
		if (flag4)
		{
			num = vector5.y;
		}
		else
		{
			num = vector.y;
		}
		num2 = vector6.y;
		normal_class.tangent = new Vector4(-space, num2 - num, (float)0, -space).normalized;
		return normal_class;
	}

	public override normal_class calc_normal_border_right(terrain_class preterrain1, float space, List<vertex_class> vertex, int y, int resolution, Vector3 size)
	{
		normal_class normal_class = new normal_class();
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		Vector3 vector5 = default(Vector3);
		Vector3 vector6 = default(Vector3);
		Vector3 vector7 = default(Vector3);
		Vector3 vector8 = default(Vector3);
		Vector3 vector9 = default(Vector3);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = true;
		bool flag6 = true;
		bool flag7 = true;
		Vector3 vector10 = Vector3.zero;
		Vector4 vector11 = default(Vector4);
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)0;
		if (preterrain1.neighbor.right > -1)
		{
			vector4 = vertex[preterrain1.neighbor.right].vertices[1 + (y - 1) * resolution] + new Vector3(size.x, (float)0, (float)0);
			vector6 = vertex[preterrain1.neighbor.right].vertices[1 + y * resolution] + new Vector3(size.x, (float)0, (float)0);
			vector9 = vertex[preterrain1.neighbor.right].vertices[1 + (y + 1) * resolution] + new Vector3(size.x, (float)0, (float)0);
		}
		else
		{
			flag3 = false;
			flag4 = false;
			flag7 = false;
		}
		vector2 = vertex[preterrain1.index].vertices[resolution - 2 + (y - 1) * resolution];
		vector3 = vertex[preterrain1.index].vertices[resolution - 1 + (y - 1) * resolution];
		vector7 = vertex[preterrain1.index].vertices[resolution - 2 + (y + 1) * resolution];
		vector8 = vertex[preterrain1.index].vertices[resolution - 1 + (y + 1) * resolution];
		vector = vertex[preterrain1.index].vertices[resolution - 1 + y * resolution];
		vector5 = vertex[preterrain1.index].vertices[resolution - 2 + y * resolution];
		if (flag && flag2)
		{
			vector10 = Vector3.Cross(vector2 - vector, vector3 - vector);
			num3 += (float)1;
		}
		if (flag2 && flag3)
		{
			vector10 += Vector3.Cross(vector3 - vector, vector4 - vector);
			num3 += (float)1;
		}
		if (flag)
		{
			vector10 += Vector3.Cross(vector5 - vector, vector2 - vector);
			num3 += (float)1;
		}
		if (flag3 && flag4)
		{
			vector10 += Vector3.Cross(vector4 - vector, vector6 - vector);
			num3 += (float)1;
		}
		if (flag5)
		{
			vector10 += Vector3.Cross(vector7 - vector, vector5 - vector);
			num3 += (float)1;
		}
		if (flag4 && flag7)
		{
			vector10 += Vector3.Cross(vector6 - vector, vector9 - vector);
			num3 += (float)1;
		}
		if (flag5 && flag6)
		{
			vector10 += Vector3.Cross(vector8 - vector, vector7 - vector);
			num3 += (float)1;
		}
		if (flag7 && flag6)
		{
			vector10 += Vector3.Cross(vector9 - vector, vector8 - vector);
			num3 += (float)1;
		}
		vector10 = (vector10 / num3).normalized;
		vector10.y *= (float)-1;
		normal_class.normal = vector10;
		num = vector5.y;
		if (flag4)
		{
			num2 = vector6.y;
		}
		else
		{
			num2 = vector.y;
		}
		normal_class.tangent = new Vector4(-space, num2 - num, (float)0, -space).normalized;
		return normal_class;
	}

	public override normal_class calc_normal_border_top(terrain_class preterrain1, float space, List<vertex_class> vertex, int x, int resolution, Vector3 size)
	{
		normal_class normal_class = new normal_class();
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		Vector3 vector5 = default(Vector3);
		Vector3 vector6 = default(Vector3);
		Vector3 vector7 = default(Vector3);
		Vector3 vector8 = default(Vector3);
		Vector3 vector9 = default(Vector3);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = true;
		bool flag6 = true;
		bool flag7 = true;
		Vector3 vector10 = Vector3.zero;
		Vector4 vector11 = default(Vector4);
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)0;
		if (x + 1 > resolution - 1)
		{
			if (preterrain1.neighbor.right > -1)
			{
				vector6 = vertex[preterrain1.neighbor.right].vertices[1 + resolution * (resolution - 1)] + new Vector3(size.x, (float)0, (float)0);
				vector4 = vertex[preterrain1.neighbor.right].vertices[1 + resolution * (resolution - 2)] + new Vector3(size.x, (float)0, (float)0);
				if (preterrain1.neighbor.top_right > -1)
				{
					vector9 = vertex[preterrain1.neighbor.top_right].vertices[resolution + 1] + new Vector3(size.x, (float)0, size.z);
				}
				else
				{
					flag7 = false;
				}
			}
			else
			{
				flag2 = false;
				flag4 = false;
				flag7 = false;
			}
			if (preterrain1.neighbor.top > -1)
			{
				vector7 = vertex[preterrain1.neighbor.top].vertices[x - 1 + resolution] + new Vector3((float)0, (float)0, size.z);
				vector8 = vertex[preterrain1.neighbor.top].vertices[x + resolution] + new Vector3((float)0, (float)0, size.z);
			}
			else
			{
				flag5 = false;
				flag6 = false;
			}
			vector2 = vertex[preterrain1.index].vertices[x - 1 + resolution * (resolution - 2)];
			vector5 = vertex[preterrain1.index].vertices[x - 1 + resolution * (resolution - 1)];
		}
		else if (x - 1 < 0)
		{
			if (preterrain1.neighbor.left > -1)
			{
				vector2 = vertex[preterrain1.neighbor.left].vertices[resolution - 2 + resolution * (resolution - 2)] - new Vector3(size.x, (float)0, (float)0);
				vector5 = vertex[preterrain1.neighbor.left].vertices[resolution - 2 + resolution * (resolution - 1)] - new Vector3(size.x, (float)0, (float)0);
				if (preterrain1.neighbor.top_left > -1)
				{
					vector7 = vertex[preterrain1.neighbor.top_left].vertices[resolution - 2 + resolution] - new Vector3(size.x, (float)0, -size.z);
				}
				else
				{
					flag5 = false;
				}
			}
			else
			{
				flag = false;
				flag3 = false;
				flag5 = false;
			}
			if (preterrain1.neighbor.top > -1)
			{
				vector8 = vertex[preterrain1.neighbor.top].vertices[x + resolution] + new Vector3((float)0, (float)0, size.z);
				vector9 = vertex[preterrain1.neighbor.top].vertices[x + 1 + resolution] + new Vector3((float)0, (float)0, size.z);
			}
			else
			{
				flag6 = false;
				flag7 = false;
			}
			vector4 = vertex[preterrain1.index].vertices[x + 1 + resolution * (resolution - 2)];
			vector6 = vertex[preterrain1.index].vertices[x + 1 + resolution * (resolution - 1)];
		}
		else
		{
			if (preterrain1.neighbor.top > -1)
			{
				vector7 = vertex[preterrain1.neighbor.top].vertices[x - 1 + resolution] + new Vector3((float)0, (float)0, size.z);
				vector8 = vertex[preterrain1.neighbor.top].vertices[x + resolution] + new Vector3((float)0, (float)0, size.z);
				vector9 = vertex[preterrain1.neighbor.top].vertices[x + 1 + resolution] + new Vector3((float)0, (float)0, size.z);
			}
			else
			{
				flag5 = false;
				flag6 = false;
				flag7 = false;
			}
			vector2 = vertex[preterrain1.index].vertices[x - 1 + resolution * (resolution - 2)];
			vector4 = vertex[preterrain1.index].vertices[x + 1 + resolution * (resolution - 2)];
			vector5 = vertex[preterrain1.index].vertices[x - 1 + resolution * (resolution - 1)];
			vector6 = vertex[preterrain1.index].vertices[x + 1 + resolution * (resolution - 1)];
		}
		vector = vertex[preterrain1.index].vertices[x + resolution * (resolution - 1)];
		vector3 = vertex[preterrain1.index].vertices[x + resolution * (resolution - 2)];
		if (flag)
		{
			vector10 = Vector3.Cross(vector2 - vector, vector3 - vector);
			num3 += (float)1;
		}
		if (flag2)
		{
			vector10 += Vector3.Cross(vector3 - vector, vector4 - vector);
			num3 += (float)1;
		}
		if (flag && flag3)
		{
			vector10 += Vector3.Cross(vector5 - vector, vector2 - vector);
			num3 += (float)1;
		}
		if (flag2 && flag4)
		{
			vector10 += Vector3.Cross(vector4 - vector, vector6 - vector);
			num3 += (float)1;
		}
		if (flag5 && flag3)
		{
			vector10 += Vector3.Cross(vector7 - vector, vector5 - vector);
			num3 += (float)1;
		}
		if (flag4 && flag7)
		{
			vector10 += Vector3.Cross(vector6 - vector, vector9 - vector);
			num3 += (float)1;
		}
		if (flag5 && flag6)
		{
			vector10 += Vector3.Cross(vector8 - vector, vector7 - vector);
			num3 += (float)1;
		}
		if (flag7 && flag6)
		{
			vector10 += Vector3.Cross(vector9 - vector, vector8 - vector);
			num3 += (float)1;
		}
		vector10 = (vector10 / num3).normalized;
		vector10.y *= (float)-1;
		normal_class.normal = vector10;
		if (flag3)
		{
			num = vector5.y;
		}
		else
		{
			num = vector.y;
		}
		if (flag4)
		{
			num2 = vector6.y;
		}
		else
		{
			num2 = vector.y;
		}
		normal_class.tangent = new Vector4(-space, num2 - num, (float)0, -space).normalized;
		return normal_class;
	}

	public override normal_class calc_normal_border_bottom(terrain_class preterrain1, float space, List<vertex_class> vertex, int x, int resolution, Vector3 size)
	{
		normal_class normal_class = new normal_class();
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		Vector3 vector5 = default(Vector3);
		Vector3 vector6 = default(Vector3);
		Vector3 vector7 = default(Vector3);
		Vector3 vector8 = default(Vector3);
		Vector3 vector9 = default(Vector3);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = true;
		bool flag6 = true;
		bool flag7 = true;
		Vector3 vector10 = Vector3.zero;
		Vector4 vector11 = default(Vector4);
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)0;
		if (x + 1 > resolution - 1)
		{
			if (preterrain1.neighbor.right > -1)
			{
				vector6 = vertex[preterrain1.neighbor.right].vertices[1] + new Vector3(size.x, (float)0, (float)0);
				vector9 = vertex[preterrain1.neighbor.right].vertices[1 + resolution] + new Vector3(size.x, (float)0, (float)0);
				if (preterrain1.neighbor.bottom_right > -1)
				{
					vector4 = vertex[preterrain1.neighbor.bottom_right].vertices[1 + resolution * (resolution - 2)] + new Vector3(size.x, (float)0, -size.z);
				}
				else
				{
					flag3 = false;
				}
			}
			else
			{
				flag3 = false;
				flag5 = false;
				flag7 = false;
			}
			if (preterrain1.neighbor.bottom > -1)
			{
				vector2 = vertex[preterrain1.neighbor.bottom].vertices[x - 1 + resolution * (resolution - 2)] - new Vector3((float)0, (float)0, size.z);
				vector3 = vertex[preterrain1.neighbor.bottom].vertices[x + resolution * (resolution - 2)] - new Vector3((float)0, (float)0, size.z);
			}
			else
			{
				flag = false;
				flag2 = false;
			}
			vector7 = vertex[preterrain1.index].vertices[x - 1 + resolution];
			vector5 = vertex[preterrain1.index].vertices[x - 1];
		}
		else if (x - 1 < 0)
		{
			if (preterrain1.neighbor.left > -1)
			{
				vector7 = vertex[preterrain1.neighbor.left].vertices[resolution - 2 + resolution] - new Vector3(size.x, (float)0, (float)0);
				vector5 = vertex[preterrain1.neighbor.left].vertices[resolution - 2] - new Vector3(size.x, (float)0, (float)0);
				if (preterrain1.neighbor.bottom_left > -1)
				{
					vector2 = vertex[preterrain1.neighbor.bottom_left].vertices[resolution - 2 + resolution * (resolution - 2)] - new Vector3(size.x, (float)0, size.z);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
				flag4 = false;
				flag6 = false;
			}
			if (preterrain1.neighbor.bottom > -1)
			{
				vector3 = vertex[preterrain1.neighbor.bottom].vertices[x + resolution * (resolution - 2)] - new Vector3((float)0, (float)0, size.z);
				vector4 = vertex[preterrain1.neighbor.bottom].vertices[x + 1 + resolution * (resolution - 2)] - new Vector3((float)0, (float)0, size.z);
			}
			else
			{
				flag2 = false;
				flag3 = false;
			}
			vector9 = vertex[preterrain1.index].vertices[x + 1 + resolution];
			vector6 = vertex[preterrain1.index].vertices[x + 1];
		}
		else
		{
			if (preterrain1.neighbor.bottom > -1)
			{
				vector2 = vertex[preterrain1.neighbor.bottom].vertices[x - 1 + resolution * (resolution - 2)] - new Vector3((float)0, (float)0, size.z);
				vector3 = vertex[preterrain1.neighbor.bottom].vertices[x + resolution * (resolution - 2)] - new Vector3((float)0, (float)0, size.z);
				vector4 = vertex[preterrain1.neighbor.bottom].vertices[x + 1 + resolution * (resolution - 2)] - new Vector3((float)0, (float)0, size.z);
			}
			else
			{
				flag = false;
				flag2 = false;
				flag3 = false;
			}
			vector7 = vertex[preterrain1.index].vertices[x - 1 + resolution];
			vector9 = vertex[preterrain1.index].vertices[x + 1 + resolution];
			vector5 = vertex[preterrain1.index].vertices[x - 1];
			vector6 = vertex[preterrain1.index].vertices[x + 1];
		}
		vector = vertex[preterrain1.index].vertices[x];
		vector8 = vertex[preterrain1.index].vertices[x + resolution];
		if (flag && flag2)
		{
			vector10 = Vector3.Cross(vector2 - vector, vector3 - vector);
			num3 += (float)1;
		}
		if (flag3 && flag2)
		{
			vector10 += Vector3.Cross(vector3 - vector, vector4 - vector);
			num3 += (float)1;
		}
		if (flag && flag4)
		{
			vector10 += Vector3.Cross(vector5 - vector, vector2 - vector);
			num3 += (float)1;
		}
		if (flag3 && flag5)
		{
			vector10 += Vector3.Cross(vector4 - vector, vector6 - vector);
			num3 += (float)1;
		}
		if (flag6 && flag4)
		{
			vector10 += Vector3.Cross(vector7 - vector, vector5 - vector);
			num3 += (float)1;
		}
		if (flag5 && flag7)
		{
			vector10 += Vector3.Cross(vector6 - vector, vector9 - vector);
			num3 += (float)1;
		}
		if (flag6)
		{
			vector10 += Vector3.Cross(vector8 - vector, vector7 - vector);
			num3 += (float)1;
		}
		if (flag7)
		{
			vector10 += Vector3.Cross(vector9 - vector, vector8 - vector);
			num3 += (float)1;
		}
		vector10 = (vector10 / num3).normalized;
		vector10.y *= (float)-1;
		normal_class.normal = vector10;
		if (flag4)
		{
			num = vector5.y;
		}
		else
		{
			num = vector.y;
		}
		if (flag5)
		{
			num2 = vector6.y;
		}
		else
		{
			num2 = vector.y;
		}
		normal_class.tangent = new Vector4(-space, num2 - num, (float)0, -space).normalized;
		return normal_class;
	}

	public override void set_terrains_neighbor()
	{
		int num = 0;
		for (int i = 0; i < this.terrains.Count; i++)
		{
			if (this.terrains[i].terrain)
			{
				num = this.search_tile((int)(this.terrains[i].tile_x - (float)1), (int)this.terrains[i].tile_z);
				if (num != -1)
				{
					this.terrains[i].neighbor.left = num;
				}
				else
				{
					this.terrains[i].neighbor.left = -1;
				}
				num = this.search_tile((int)this.terrains[i].tile_x, (int)(this.terrains[i].tile_z - (float)1));
				if (num != -1)
				{
					this.terrains[i].neighbor.top = num;
				}
				else
				{
					this.terrains[i].neighbor.top = -1;
				}
				num = this.search_tile((int)(this.terrains[i].tile_x + (float)1), (int)this.terrains[i].tile_z);
				if (num != -1)
				{
					this.terrains[i].neighbor.right = num;
				}
				else
				{
					this.terrains[i].neighbor.right = -1;
				}
				num = this.search_tile((int)this.terrains[i].tile_x, (int)(this.terrains[i].tile_z + (float)1));
				if (num != -1)
				{
					this.terrains[i].neighbor.bottom = num;
				}
				else
				{
					this.terrains[i].neighbor.bottom = -1;
				}
				num = this.search_tile((int)(this.terrains[i].tile_x + (float)1), (int)(this.terrains[i].tile_z + (float)1));
				if (num != -1)
				{
					this.terrains[i].neighbor.bottom_right = num;
				}
				else
				{
					this.terrains[i].neighbor.bottom_right = -1;
				}
				num = this.search_tile((int)(this.terrains[i].tile_x - (float)1), (int)(this.terrains[i].tile_z + (float)1));
				if (num != -1)
				{
					this.terrains[i].neighbor.bottom_left = num;
				}
				else
				{
					this.terrains[i].neighbor.bottom_left = -1;
				}
				num = this.search_tile((int)(this.terrains[i].tile_x + (float)1), (int)(this.terrains[i].tile_z - (float)1));
				if (num != -1)
				{
					this.terrains[i].neighbor.top_right = num;
				}
				else
				{
					this.terrains[i].neighbor.top_right = -1;
				}
				num = this.search_tile((int)(this.terrains[i].tile_x - (float)1), (int)(this.terrains[i].tile_z - (float)1));
				if (num != -1)
				{
					this.terrains[i].neighbor.top_left = num;
				}
				else
				{
					this.terrains[i].neighbor.top_left = -1;
				}
				this.terrains[i].neighbor.self = i;
				this.terrains[i].index = i;
			}
		}
	}

	public override void load_raw_heightmaps()
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		for (int i = 0; i < this.prelayers.Count; i++)
		{
			for (int j = 0; j < this.prelayers[i].layer.Count; j++)
			{
				for (int k = 0; k < this.prelayers[i].layer[j].prefilter.filter_index.Count; k++)
				{
					filter_class filter_class = this.filter[this.prelayers[i].layer[j].prefilter.filter_index[k]];
					if (filter_class.type == condition_type_enum.RawHeightmap)
					{
						for (int l = 0; l < filter_class.raw.file_index.Count; l++)
						{
							if (filter_class.raw.file_index[l] > -1)
							{
								if (!this.raw_files[filter_class.raw.file_index[l]].loaded)
								{
									if (!this.raw_files[filter_class.raw.file_index[l]].exists())
									{
										if (this.script_base != null)
										{
											this.script_base.erase_raw_file(l);
										}
										this.erase_raw_file(l);
										filter_class.raw.file_index.RemoveAt(l);
										l--;
										if (filter_class.raw.file_index.Count == 0)
										{
											this.erase_filter(k, this.prelayers[i].layer[j].prefilter);
											k--;
										}
										goto IL_644;
									}
									this.raw_files[filter_class.raw.file_index[l]].bytes = File.ReadAllBytes(this.raw_files[filter_class.raw.file_index[l]].file);
									num = (ulong)this.raw_files[filter_class.raw.file_index[l]].bytes.Length;
									num2 = (ulong)(this.raw_files[filter_class.raw.file_index[l]].resolution.x * this.raw_files[filter_class.raw.file_index[l]].resolution.y * (float)2);
									if (num != num2)
									{
										Debug.Log("Prelayer" + i + " -> Layer" + j + " -> Filter" + l + "\nThe Raw Heightmap file '" + this.raw_files[filter_class.raw.file_index[l]].file + "' has a lower resolution than selected. Please check the File size. It should be X*Y*2 = " + this.raw_files[filter_class.raw.file_index[l]].resolution.x + "*" + this.raw_files[filter_class.raw.file_index[l]].resolution.y + "*2 = " + this.raw_files[filter_class.raw.file_index[l]].resolution.x * this.raw_files[filter_class.raw.file_index[l]].resolution.y * (float)2 + " Bytes (" + this.raw_files[filter_class.raw.file_index[l]].resolution.x + "*" + this.raw_files[filter_class.raw.file_index[l]].resolution.y + " resolution). But the File size is " + this.raw_files[filter_class.raw.file_index[l]].bytes.Length + " Bytes (" + Mathf.Round(Mathf.Sqrt((float)(this.raw_files[filter_class.raw.file_index[l]].bytes.Length / 2))) + "x" + Mathf.Round(Mathf.Sqrt((float)(this.raw_files[filter_class.raw.file_index[l]].bytes.Length / 2))) + " resolution).");
										this.erase_raw_file(l);
										filter_class.raw.file_index.RemoveAt(l);
										l--;
										if (filter_class.raw.file_index.Count == 0)
										{
											this.erase_filter(k, this.prelayers[i].layer[j].prefilter);
											k--;
										}
										goto IL_644;
									}
									this.raw_files[filter_class.raw.file_index[l]].loaded = true;
								}
								if (i == 0 && !this.prelayers[0].prearea.active)
								{
									filter_class.raw.set_raw_auto_scale(this.terrains[0], this.terrains[0].prearea.area_old, this.raw_files, l);
								}
								else
								{
									filter_class.raw.set_raw_auto_scale(this.terrains[0], this.prelayers[i].prearea.area_old, this.raw_files, l);
								}
							}
							else
							{
								filter_class.raw.file_index.RemoveAt(l);
								l--;
								if (filter_class.raw.file_index.Count == 0)
								{
									this.erase_filter(k, this.prelayers[i].layer[j].prefilter);
									k--;
								}
							}
							IL_644:;
						}
					}
					this.load_raw_subfilter(filter_class, i, j);
				}
			}
		}
	}

	public override void load_raw_subfilter(filter_class filter1, int count_prelayer1, int count_layer1)
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		for (int i = 0; i < filter1.presubfilter.subfilter_index.Count; i++)
		{
			subfilter_class subfilter_class = this.subfilter[filter1.presubfilter.subfilter_index[i]];
			if (subfilter_class.type == condition_type_enum.RawHeightmap)
			{
				for (int j = 0; j < subfilter_class.raw.file_index.Count; j++)
				{
					if (subfilter_class.raw.file_index[j] > -1)
					{
						if (!this.raw_files[subfilter_class.raw.file_index[j]].loaded)
						{
							if (!this.raw_files[subfilter_class.raw.file_index[j]].exists())
							{
								this.script.erase_raw_file(j);
								this.erase_raw_file(j);
								subfilter_class.raw.file_index.RemoveAt(j);
								j--;
								if (subfilter_class.raw.file_index.Count == 0)
								{
									this.erase_subfilter(i, filter1.presubfilter);
									i--;
								}
								goto IL_5BC;
							}
							this.raw_files[subfilter_class.raw.file_index[j]].bytes = File.ReadAllBytes(this.raw_files[subfilter_class.raw.file_index[j]].file);
							num = (ulong)this.raw_files[subfilter_class.raw.file_index[j]].bytes.Length;
							num2 = (ulong)(this.raw_files[subfilter_class.raw.file_index[j]].resolution.x * this.raw_files[subfilter_class.raw.file_index[j]].resolution.y * (float)2);
							if (num != num2)
							{
								Debug.Log("Prelayer" + count_prelayer1 + " -> Layer" + count_layer1 + " -> subfilter" + j + "\nThe Raw Heightmap file '" + this.raw_files[subfilter_class.raw.file_index[j]].file + "' has a lower resolution than selected. Please check the File size. It should be X*Y*2 = " + this.raw_files[subfilter_class.raw.file_index[j]].resolution.x + "*" + this.raw_files[subfilter_class.raw.file_index[j]].resolution.y + "*2 = " + this.raw_files[subfilter_class.raw.file_index[j]].resolution.x * this.raw_files[subfilter_class.raw.file_index[j]].resolution.y * (float)2 + " Bytes (" + this.raw_files[subfilter_class.raw.file_index[j]].resolution.x + "*" + this.raw_files[subfilter_class.raw.file_index[j]].resolution.y + " resolution). But the File size is " + this.raw_files[subfilter_class.raw.file_index[j]].bytes.Length + " Bytes (" + Mathf.Round(Mathf.Sqrt((float)(this.raw_files[subfilter_class.raw.file_index[j]].bytes.Length / 2))) + "x" + Mathf.Round(Mathf.Sqrt((float)(this.raw_files[subfilter_class.raw.file_index[j]].bytes.Length / 2))) + " resolution).");
								this.erase_raw_file(j);
								subfilter_class.raw.file_index.RemoveAt(j);
								j--;
								if (subfilter_class.raw.file_index.Count == 0)
								{
									this.erase_subfilter(i, filter1.presubfilter);
									i--;
								}
								goto IL_5BC;
							}
							this.raw_files[subfilter_class.raw.file_index[j]].loaded = true;
						}
						if (count_prelayer1 == 0 && !this.prelayers[0].prearea.active)
						{
							subfilter_class.raw.set_raw_auto_scale(this.terrains[0], this.terrains[0].prearea.area_old, this.raw_files, j);
						}
						else
						{
							subfilter_class.raw.set_raw_auto_scale(this.terrains[0], this.prelayers[count_prelayer1].prearea.area_old, this.raw_files, j);
						}
					}
					else
					{
						subfilter_class.raw.file_index.RemoveAt(j);
						j--;
						if (subfilter_class.raw.file_index.Count == 0)
						{
							this.erase_subfilter(i, filter1.presubfilter);
							i--;
						}
					}
					IL_5BC:;
				}
			}
		}
	}

	public override void Main()
	{
	}
}
