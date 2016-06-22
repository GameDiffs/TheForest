using System;
using UnityEngine;

[Serializable]
public class splatPrototype_class
{
	public bool foldout;

	public Texture2D texture;

	public Vector2 tileSize;

	public bool tileSize_link;

	public Vector2 tileSize_old;

	public Vector2 tileOffset;

	public Vector2 normal_tileSize;

	public float strength;

	public float strength_splat;

	public Texture2D normal_texture;

	public Texture2D normalMap;

	public Texture2D height_texture;

	public Texture2D specular_texture;

	public int import_max_size_list;

	public splatPrototype_class()
	{
		this.tileSize = new Vector2((float)10, (float)10);
		this.tileSize_link = true;
		this.tileOffset = new Vector2((float)0, (float)0);
		this.normal_tileSize = new Vector2((float)10, (float)10);
		this.strength = (float)1;
		this.strength_splat = (float)1;
	}
}
