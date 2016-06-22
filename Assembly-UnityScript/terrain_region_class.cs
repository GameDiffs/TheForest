using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class terrain_region_class
{
	public bool active;

	public bool foldout;

	public string text;

	public List<terrain_area_class> area;

	public int area_select;

	public int mode;

	public Rect area_size;

	public terrain_region_class()
	{
		this.active = true;
		this.foldout = true;
		this.text = "Terrain Area";
		this.area = new List<terrain_area_class>();
		this.area.Add(new terrain_area_class());
	}

	public override void add_area(int index)
	{
		this.area.Insert(index, new terrain_area_class());
		this.set_area_index();
		this.set_area_text();
		this.area[index].set_terrain_text();
		this.area[index].path = Application.dataPath;
	}

	public override void erase_area(int index)
	{
		this.area.RemoveAt(index);
		this.set_area_index();
		this.set_area_text();
	}

	public override void set_area_index()
	{
		for (int i = 0; i < this.area.Count; i++)
		{
			this.area[i].index = i;
		}
	}

	public override void set_area_text()
	{
		if (this.area.Count > 1)
		{
			this.text = "Terrain Areas";
		}
		else
		{
			this.text = "Terrain Area";
		}
	}
}
