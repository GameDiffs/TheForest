using System;
using UnityEngine;

[Serializable]
public class color_range_class : animation_curve_class
{
	public Color color_color_range;

	public Color color_start;

	public Color color_end;

	public bool one_color;

	public Rect curve_menu_rect;

	public bool invert;

	public string swap_text;

	public bool swap_select;

	public bool copy_select;

	public float output;

	public int select_output;

	public color_range_class()
	{
		this.color_color_range = new Color((float)2, (float)2, (float)2, (float)1);
		this.color_start = new Color((float)0, (float)0, (float)0, (float)1);
		this.color_end = new Color((float)0, (float)0, (float)0, (float)1);
		this.swap_text = "S";
		this.output = (float)1;
		this.curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.default_curve = new AnimationCurve(this.curve.keys);
	}
}
