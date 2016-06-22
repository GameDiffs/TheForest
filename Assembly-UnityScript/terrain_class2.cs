using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class terrain_class2
{
	public bool active;

	public bool foldout;

	public int index;

	public int index_old;

	public bool on_row;

	public Color color_terrain;

	public Component rtp_script;

	public Texture2D[] splat_alpha;

	public Terrain terrain;

	public Transform parent;

	public string name;

	public area_class prearea;

	public float[,,] map;

	public List<splatPrototype_class> splatPrototypes;

	public splatPrototype_class colormap;

	public bool splats_foldout;

	public List<treePrototype_class> treePrototypes;

	public bool trees_foldout;

	public List<detailPrototype_class> detailPrototypes;

	public bool details_foldout;

	public List<TreeInstance> tree_instances;

	public float[] splat;

	public float[] splat_calc;

	public float[] color;

	public float[] splat_layer;

	public float[] color_layer;

	public float[] grass;

	public int heightmap_resolution_list;

	public int splatmap_resolution_list;

	public int basemap_resolution_list;

	public int detailmap_resolution_list;

	public int detail_resolution_per_patch_list;

	public Vector3 size;

	public bool size_xz_link;

	public int tile_x;

	public int tile_z;

	public Vector2 tiles;

	public Rect rect;

	public bool data_foldout;

	public Vector3 scale;

	public bool maps_foldout;

	public bool settings_foldout;

	public bool resolution_foldout;

	public bool scripts_foldout;

	public bool reset_foldout;

	public bool size_foldout;

	public int raw_file_index;

	public raw_file_class raw_save_file;

	public int heightmap_resolution;

	public int splatmap_resolution;

	public int detail_resolution;

	public int detail_resolution_per_patch;

	public int basemap_resolution;

	public bool size_synchronous;

	public bool resolutions_synchronous;

	public bool splat_synchronous;

	public bool tree_synchronous;

	public bool detail_synchronous;

	public Vector2 splatmap_conversion;

	public Vector2 heightmap_conversion;

	public Vector2 detailmap_conversion;

	public bool splat_foldout;

	public int splat_length;

	public int color_length;

	public bool tree_foldout;

	public int tree_length;

	public bool detail_foldout;

	public float detail_scale;

	public bool base_terrain_foldout;

	public bool tree_detail_objects_foldout;

	public bool wind_settings_foldout;

	public bool settings_all_terrain;

	public float heightmapPixelError;

	public int heightmapMaximumLOD;

	public bool castShadows;

	public float basemapDistance;

	public float treeDistance;

	public float detailObjectDistance;

	public float detailObjectDensity;

	public int treeMaximumFullLODCount;

	public float treeBillboardDistance;

	public float treeCrossFadeLength;

	public bool draw;

	public bool editor_draw;

	public TerrainDetail script_terrainDetail;

	public bool settings_runtime;

	public bool settings_editor;

	public float wavingGrassSpeed;

	public float wavingGrassAmount;

	public float wavingGrassStrength;

	public Color wavingGrassTint;

	public neighbor_class neighbor;

	public terrain_class2()
	{
		this.active = true;
		this.color_terrain = new Color((float)2, (float)2, (float)2, (float)1);
		this.prearea = new area_class();
		this.splatPrototypes = new List<splatPrototype_class>();
		this.colormap = new splatPrototype_class();
		this.treePrototypes = new List<treePrototype_class>();
		this.detailPrototypes = new List<detailPrototype_class>();
		this.tree_instances = new List<TreeInstance>();
		this.heightmap_resolution_list = 5;
		this.splatmap_resolution_list = 4;
		this.basemap_resolution_list = 4;
		this.size = new Vector3((float)1000, (float)250, (float)1000);
		this.size_xz_link = true;
		this.tiles = new Vector2((float)1, (float)1);
		this.data_foldout = true;
		this.resolution_foldout = true;
		this.raw_file_index = -1;
		this.raw_save_file = new raw_file_class();
		this.heightmap_resolution = 129;
		this.splatmap_resolution = 128;
		this.detail_resolution = 128;
		this.detail_resolution_per_patch = 8;
		this.basemap_resolution = 128;
		this.size_synchronous = true;
		this.resolutions_synchronous = true;
		this.splat_synchronous = true;
		this.tree_synchronous = true;
		this.detail_synchronous = true;
		this.detail_scale = (float)1;
		this.base_terrain_foldout = true;
		this.tree_detail_objects_foldout = true;
		this.wind_settings_foldout = true;
		this.settings_all_terrain = true;
		this.heightmapPixelError = (float)5;
		this.basemapDistance = (float)20000;
		this.treeDistance = (float)20000;
		this.detailObjectDistance = (float)250;
		this.detailObjectDensity = (float)1;
		this.treeMaximumFullLODCount = 50;
		this.treeBillboardDistance = (float)250;
		this.treeCrossFadeLength = (float)200;
		this.draw = true;
		this.editor_draw = true;
		this.settings_editor = true;
		this.wavingGrassSpeed = 0.5f;
		this.wavingGrassAmount = 0.5f;
		this.wavingGrassStrength = 0.5f;
		this.wavingGrassTint = new Color(0.698f, 0.6f, 0.5f);
		this.neighbor = new neighbor_class();
	}

	public override void add_splatprototype(int splat_number)
	{
		this.splatPrototypes.Insert(splat_number, new splatPrototype_class());
	}

	public override void erase_splatprototype(int splat_number)
	{
		if (this.splatPrototypes.Count > 0)
		{
			this.splatPrototypes.RemoveAt(splat_number);
		}
	}

	public override void clear_splatprototype()
	{
		this.splatPrototypes.Clear();
	}

	public override void add_treeprototype(int tree_number)
	{
		this.treePrototypes.Insert(tree_number, new treePrototype_class());
	}

	public override void erase_treeprototype(int tree_number)
	{
		if (this.treePrototypes.Count > 0)
		{
			this.treePrototypes.RemoveAt(tree_number);
		}
	}

	public override void clear_treeprototype()
	{
		this.treePrototypes.Clear();
	}

	public override void add_detailprototype(int detail_number)
	{
		this.detailPrototypes.Insert(detail_number, new detailPrototype_class());
	}

	public override void erase_detailprototype(int detail_number)
	{
		if (this.detailPrototypes.Count > 0)
		{
			this.detailPrototypes.RemoveAt(detail_number);
		}
	}

	public override void clear_detailprototype()
	{
		this.detailPrototypes.Clear();
	}
}
