using System;
using UnityEngine;

[Serializable]
public class map_area_class
{
	public string name;

	public latlong_class upper_left;

	public latlong_class lower_right;

	public latlong_class center;

	public int center_height;

	public map_pixel_class size;

	public bool created;

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

	public bool manual_area;

	public Vector2 heightmap_offset;

	public Vector2 heightmap_offset_e;

	public Vector2 image_offset_e;

	public bool image_stop_one;

	public int select;

	public float smooth_strength;

	public float width;

	public float height;

	public Vector2 heightmap_resolution;

	public double heightmap_scale;

	public int heightmap_zoom;

	public int elevation_zoom;

	public double area_resolution;

	public int resolution;

	public int image_zoom;

	public bool image_changed;

	public bool start_tile_enabled;

	public tile_class start_tile;

	public tile_class tiles;

	public bool export_heightmap_active;

	public bool export_heightmap_call;

	public string export_heightmap_path;

	public string export_heightmap_filename;

	public bool export_heightmap_changed;

	public bool export_heightmap_not_fit;

	public Vector2 export_heightmap_bres;

	public bool export_image_active;

	public bool export_image_call;

	public string export_image_path;

	public string export_image_filename;

	public bool export_image_changed;

	public bool export_image_import_settings;

	public bool export_image_world_file;

	public string export_terrain_path;

	public bool export_terrain_changed;

	public bool export_to_terraincomposer;

	public string import_heightmap_path_full;

	public bool import_heightmap;

	public bool filter_perlin;

	public string converter_source_path_full;

	public string converter_destination_path_full;

	public Vector2 converter_resolution;

	public float converter_height;

	public bool converter_import_heightmap;

	public string terrain_asset_name;

	public string terrain_scene_name;

	public bool terrain_name_changed;

	public float terrain_height;

	public float terrain_scale;

	public AnimationCurve terrain_curve;

	public bool do_heightmap;

	public bool do_image;

	public int terrain_heightmap_resolution_select;

	public int terrain_heightmap_resolution;

	public bool terrain_heightmap_resolution_changed;

	public bool mipmapEnabled;

	public bool terrain_done;

	public int anisoLevel;

	public int maxTextureSize;

	public int maxTextureSize_select;

	public bool maxTextureSize_changed;

	public bool auto_import_settings_apply;

	public bool preimage_export_active;

	public bool preimage_apply;

	public bool preimage_save_new;

	public string preimage_path;

	public bool preimage_path_changed;

	public string preimage_filename;

	public int preimage_count;

	public map_area_class(string name1, int index)
	{
		this.name = "Untitled";
		this.upper_left = new latlong_class();
		this.lower_right = new latlong_class();
		this.center = new latlong_class();
		this.size = new map_pixel_class();
		this.heightmap_offset = new Vector2((float)0, (float)0);
		this.smooth_strength = (float)1;
		this.resolution = 2048;
		this.image_zoom = 18;
		this.start_tile = new tile_class();
		this.tiles = new tile_class();
		this.export_heightmap_path = string.Empty;
		this.export_heightmap_filename = string.Empty;
		this.export_image_path = string.Empty;
		this.export_image_filename = string.Empty;
		this.export_terrain_path = string.Empty;
		this.export_to_terraincomposer = true;
		this.converter_source_path_full = string.Empty;
		this.converter_destination_path_full = string.Empty;
		this.converter_height = (float)9000;
		this.terrain_asset_name = string.Empty;
		this.terrain_scene_name = string.Empty;
		this.terrain_height = (float)9000;
		this.terrain_scale = (float)1;
		this.do_heightmap = true;
		this.do_image = true;
		this.mipmapEnabled = true;
		this.anisoLevel = 9;
		this.maxTextureSize_select = 6;
		this.auto_import_settings_apply = true;
		this.preimage_path = string.Empty;
		this.name = name1 + index.ToString();
		this.terrain_curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.terrain_curve.AddKey((float)1, (float)0);
		this.terrain_curve = this.set_curve_linear(this.terrain_curve);
	}

	public override void reset()
	{
		this.upper_left.reset();
		this.lower_right.reset();
		this.center.reset();
		this.size.reset();
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
			Vector2 b = default(Vector2);
			Vector2 a = default(Vector2);
			Vector2 vector = default(Vector2);
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
				b.x = curve.keys[i - 1].time;
				b.y = curve.keys[i - 1].value;
				a.x = curve.keys[i].time;
				a.y = curve.keys[i].value;
				vector = a - b;
				inTangent = vector.y / vector.x;
			}
			if (!flag2)
			{
				b.x = curve.keys[i].time;
				b.y = curve.keys[i].value;
				a.x = curve.keys[i + 1].time;
				a.y = curve.keys[i + 1].value;
				vector = a - b;
				outTangent = vector.y / vector.x;
			}
			key.inTangent = inTangent;
			key.outTangent = outTangent;
			animationCurve.AddKey(key);
		}
		return animationCurve;
	}
}
