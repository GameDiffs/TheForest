using System;
using UnityEngine;

[Serializable]
public class area_class
{
	public bool active;

	public bool foldout;

	public Rect area;

	public Rect area_old;

	public Rect area_max;

	public Vector2 center;

	public Vector2 image_offset;

	public Vector3 rotation;

	public bool rotation_active;

	public bool link_start;

	public bool link_end;

	public float resolution;

	public float custom_resolution;

	public Vector2 step;

	public Vector2 step_old;

	public Vector2 conversion_step;

	public resolution_mode_enum resolution_mode;

	public string resolution_mode_text;

	public string resolution_tooltip_text;

	public int tree_resolution;

	public int object_resolution;

	public int colormap_resolution;

	public bool tree_resolution_active;

	public bool object_resolution_active;

	public area_class()
	{
		this.active = true;
		this.link_start = true;
		this.link_end = true;
		this.resolution_mode = resolution_mode_enum.Automatic;
		this.tree_resolution = 512;
		this.object_resolution = 512;
		this.colormap_resolution = 2048;
	}

	public override void max()
	{
		this.area = this.area_max;
	}

	public override Rect round_area_to_step(Rect area1)
	{
		area1.xMin = Mathf.Round(area1.xMin / this.step.x) * this.step.x;
		area1.xMax = Mathf.Round(area1.xMax / this.step.x) * this.step.x;
		area1.yMin = Mathf.Round(area1.yMin / this.step.y) * this.step.y;
		area1.yMax = Mathf.Round(area1.yMax / this.step.y) * this.step.y;
		return area1;
	}

	public override void set_resolution_mode_text()
	{
		if (this.area == this.area_max)
		{
			this.resolution_mode_text = "M";
			this.resolution_tooltip_text = "Maximum Area Selected";
		}
		else
		{
			this.resolution_mode_text = "C";
			this.resolution_tooltip_text = "Custum Area Selected";
		}
		if (this.resolution_mode == resolution_mode_enum.Automatic)
		{
			this.resolution_mode_text += "-> A";
			this.resolution_tooltip_text += "\n\nStep Mode is on Automatic";
		}
		else if (this.resolution_mode == resolution_mode_enum.Heightmap)
		{
			this.resolution_mode_text += "-> H";
			this.resolution_tooltip_text += "\n\nStep Mode is on Heightmap";
		}
		else if (this.resolution_mode == resolution_mode_enum.Splatmap)
		{
			this.resolution_mode_text += "-> S";
			this.resolution_tooltip_text += "\n\nStep Mode is on Splatmap";
		}
		else if (this.resolution_mode == resolution_mode_enum.Detailmap)
		{
			this.resolution_mode_text += "-> D";
			this.resolution_tooltip_text += "\n\nStep Mode is on Detailmap";
		}
		else if (this.resolution_mode == resolution_mode_enum.Tree)
		{
			this.resolution_mode_text += "-> T";
			this.resolution_tooltip_text += "\n\nStep Mode is on Tree";
		}
		else if (this.resolution_mode == resolution_mode_enum.Object)
		{
			this.resolution_mode_text += "-> O";
			this.resolution_tooltip_text += "\n\nStep Mode is on Object";
		}
		else if (this.resolution_mode == resolution_mode_enum.Units)
		{
			this.resolution_mode_text += "-> U";
			this.resolution_tooltip_text += "\n\nStep Mode is on Units";
		}
		else if (this.resolution_mode == resolution_mode_enum.Custom)
		{
			this.resolution_mode_text += "-> C";
			this.resolution_tooltip_text += "\n\nStep Mode is on Custom";
		}
	}
}
