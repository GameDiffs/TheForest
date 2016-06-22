using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class prelayer_class
{
	public bool foldout;

	public bool linked;

	public remarks_class remarks;

	public bool interface_display_layergroup;

	public bool interface_display_layer;

	public bool layers_foldout;

	public bool layers_active;

	public int index;

	public int level;

	public List<layer_class> layer;

	public predescription_class predescription;

	public string layer_text;

	public layer_output_enum layer_output;

	public Rect view_menu_rect;

	public bool view_heightmap_layer;

	public bool view_color_layer;

	public bool view_splat_layer;

	public bool view_tree_layer;

	public bool view_grass_layer;

	public bool view_object_layer;

	public bool view_only_selected;

	public float x;

	public float y;

	public float counter_y;

	public int count_terrain;

	public float break_x_value;

	public area_class prearea;

	public bool area;

	public List<distance_class> objects_placed;

	public prelayer_class(int length, int index2)
	{
		this.foldout = true;
		this.linked = true;
		this.remarks = new remarks_class();
		this.interface_display_layer = true;
		this.layers_foldout = true;
		this.layers_active = true;
		this.layer = new List<layer_class>();
		this.predescription = new predescription_class();
		this.layer_text = "Layer(1):";
		this.view_heightmap_layer = true;
		this.view_color_layer = true;
		this.view_splat_layer = true;
		this.view_tree_layer = true;
		this.view_grass_layer = true;
		this.view_object_layer = true;
		this.prearea = new area_class();
		this.objects_placed = new List<distance_class>();
		this.index = index2;
		if (length > 0)
		{
			for (int i = 0; i < length; i++)
			{
				this.layer.Add(new layer_class());
				this.layer[0].output = layer_output_enum.splat;
				this.predescription.add_layer_index(i, i, 0);
			}
		}
	}

	public override void set_prelayer_text()
	{
		this.layer_text = "Layer Level" + this.index + "(" + this.layer.Count + ")";
	}

	public override void new_layer(int layer_number, List<filter_class> filter)
	{
		this.layer[layer_number] = new layer_class();
	}

	public override void change_layers_active_from_description(int description_number, bool invert)
	{
		for (int i = 0; i < this.predescription.description[description_number].layer_index.Count; i++)
		{
			if (!invert)
			{
				this.layer[this.predescription.description[description_number].layer_index[i]].active = this.predescription.description[description_number].layers_active;
			}
			else
			{
				this.layer[this.predescription.description[description_number].layer_index[i]].active = !this.layer[this.predescription.description[description_number].layer_index[i]].active;
			}
		}
	}

	public override void change_layers_foldout_from_description(int description_number, bool invert)
	{
		for (int i = 0; i < this.predescription.description[description_number].layer_index.Count; i++)
		{
			if (!invert)
			{
				this.layer[this.predescription.description[description_number].layer_index[i]].foldout = this.predescription.description[description_number].layers_foldout;
			}
			else
			{
				this.layer[this.predescription.description[description_number].layer_index[i]].foldout = !this.layer[this.predescription.description[description_number].layer_index[i]].foldout;
			}
		}
	}

	public override void change_layers_active(bool invert)
	{
		for (int i = 0; i < this.layer.Count; i++)
		{
			if (!invert)
			{
				this.layer[i].active = this.layers_active;
			}
			else
			{
				this.layer[i].active = !this.layer[i].active;
			}
		}
	}

	public override void change_foldout_layers(bool invert)
	{
		for (int i = 0; i < this.layer.Count; i++)
		{
			if (!invert)
			{
				this.layer[i].foldout = this.layers_foldout;
			}
			else
			{
				this.layer[i].foldout = !this.layer[i].foldout;
			}
		}
	}

	public override void swap_layer2(int number1, int number2)
	{
		layer_class value = this.layer[number1];
		this.layer[number1] = this.layer[number2];
		this.layer[number2] = value;
		if (this.layer[number1].color_layer[0] < 1.5f)
		{
			this.layer[number1].color_layer = this.layer[number1].color_layer + new Color((float)1, (float)1, (float)1, (float)1);
		}
		if (this.layer[number2].color_layer[0] < 1.5f)
		{
			this.layer[number2].color_layer = this.layer[number2].color_layer + new Color((float)1, (float)1, (float)1, (float)1);
		}
	}
}
