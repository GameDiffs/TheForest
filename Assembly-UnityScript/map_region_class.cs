using System;
using System.Collections.Generic;

[Serializable]
public class map_region_class
{
	public string name;

	public List<map_area_class> area;

	public string[] area_popup;

	public int area_select;

	public latlong_class center;

	public map_region_class(int index)
	{
		this.name = "Untitled";
		this.area = new List<map_area_class>();
		this.center = new latlong_class();
		this.name += index.ToString();
	}

	public override void make_area_popup()
	{
		this.area_popup = new string[this.area.Count];
		for (int i = 0; i < this.area.Count; i++)
		{
			this.area_popup[i] = this.area[i].name;
		}
	}
}
