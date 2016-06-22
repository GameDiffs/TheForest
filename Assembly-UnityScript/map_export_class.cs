using System;

[Serializable]
public class map_export_class
{
	public bool last_tile;

	public tile_class tiles;

	public tile_class tile;

	public tile_class subtiles;

	public tile_class subtile;

	public int subtiles_total;

	public int subtile_total;

	public int subtile2_total;

	public float progress;

	public map_export_class()
	{
		this.tiles = new tile_class();
		this.tile = new tile_class();
		this.subtiles = new tile_class();
		this.subtile = new tile_class();
	}
}
