using System;
using UnityEngine;

[Serializable]
public class ext_class
{
	public WWW pull;

	public bool loaded;

	public bool converted;

	public bool error;

	public tile_class tile;

	public tile_class subtile;

	public latlong_area_class latlong_area;

	public latlong_class latlong_center;

	public string url;

	public Vector2 bres;

	public int zero_error;

	public ext_class()
	{
		this.tile = new tile_class();
		this.subtile = new tile_class();
		this.latlong_area = new latlong_area_class();
		this.latlong_center = new latlong_class();
	}
}
