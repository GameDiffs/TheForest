using System;
using UnityEngine;

[Serializable]
public class perlin_class
{
	public float frequency;

	public float amplitude;

	public int octaves;

	public animation_curve_class precurve;

	public Vector2 offset;

	public Vector2 offset_begin;

	public Rect curve_menu_rect;

	public perlin_class()
	{
		this.frequency = (float)512;
		this.amplitude = (float)1;
		this.octaves = 4;
		this.precurve = new animation_curve_class();
		this.offset = new Vector2((float)0, (float)0);
		this.offset_begin = new Vector2((float)0, (float)0);
		this.precurve.curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.precurve.default_curve = new AnimationCurve(this.precurve.curve.keys);
	}
}
