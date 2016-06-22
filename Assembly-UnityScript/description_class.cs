using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class description_class
{
	public bool foldout;

	public string text;

	public remarks_class remarks;

	public bool edit;

	public bool disable_edit;

	public Rect menu_rect;

	public Rect rect;

	public List<int> layer_index;

	public bool layers_active;

	public bool layers_foldout;

	public string swap_text;

	public bool swap_select;

	public bool copy_select;

	public description_class()
	{
		this.foldout = true;
		this.text = "Root";
		this.remarks = new remarks_class();
		this.layer_index = new List<int>();
		this.layers_active = true;
		this.layers_foldout = true;
		this.swap_text = "S";
	}
}
