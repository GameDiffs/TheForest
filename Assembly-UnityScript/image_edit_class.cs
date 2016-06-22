using System;
using UnityEngine;

[Serializable]
public class image_edit_class
{
	public Color color1_start;

	public Color color1_end;

	public AnimationCurve curve1;

	public Color color2_start;

	public Color color2_end;

	public AnimationCurve curve2;

	public float strength;

	public image_output_enum output;

	public bool active;

	public bool solid_color;

	public float radius;

	public int repeat;

	public image_edit_class()
	{
		this.color1_start = new Color((float)0, (float)0, (float)0, (float)1);
		this.color1_end = new Color(0.3f, 0.3f, 0.3f, (float)1);
		this.curve1 = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.color2_start = new Color((float)1, (float)1, (float)1, (float)1);
		this.color2_end = new Color((float)1, (float)1, (float)1, (float)1);
		this.curve2 = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.strength = (float)1;
		this.active = true;
		this.radius = (float)300;
		this.repeat = 4;
	}
}
