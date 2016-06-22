using System;
using UnityEngine;

[Serializable]
public class select_window_class
{
	public bool active;

	public bool button_colormap;

	public bool button_node;

	public bool button_terrain;

	public bool button_heightmap;

	public float terrain_zoom;

	public float terrain_zoom2;

	public Vector2 terrain_pos;

	public float node_zoom;

	public float node_zoom2;

	public Vector2 node_pos;

	public bool node_grid;

	public bool node_grid_center;

	public int mode;

	public Vector2 terrain_offset;

	public Vector2 node_offset;

	public select_window_class()
	{
		this.button_heightmap = true;
		this.terrain_zoom = (float)40;
		this.terrain_zoom2 = (float)40;
		this.terrain_pos = new Vector2((float)0, (float)0);
		this.node_zoom = (float)40;
		this.node_zoom2 = (float)40;
		this.node_pos = new Vector2((float)0, (float)0);
		this.node_grid = true;
		this.node_grid_center = true;
		this.terrain_offset = new Vector2((float)0, (float)0);
		this.node_offset = new Vector2((float)0, (float)0);
	}

	public override void select_colormap()
	{
		this.button_node = false;
		this.button_colormap = true;
		this.button_terrain = false;
	}

	public override void select_terrain()
	{
		this.button_node = false;
		this.button_colormap = false;
		this.button_terrain = true;
	}

	public override void select_node()
	{
		this.button_node = true;
		this.button_colormap = false;
		this.button_terrain = false;
	}
}
