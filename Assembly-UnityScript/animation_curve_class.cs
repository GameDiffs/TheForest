using System;
using UnityEngine;

[Serializable]
public class animation_curve_class
{
	public bool curve_in_memory;

	public string curve_text;

	public Rect menu_rect;

	public AnimationCurve curve;

	public AnimationCurve default_curve;

	public bool abs;

	public curve_type_enum type;

	public bool active;

	public bool settings_foldout;

	public float frequency;

	public Vector2 offset;

	public Vector2 offset_range;

	public Vector2 offset_middle;

	public int detail;

	public float detail_strength;

	public Transform pivot;

	public float strength;

	public animation_curve_class()
	{
		this.curve_text = "Curve";
		this.curve = new AnimationCurve();
		this.default_curve = new AnimationCurve();
		this.active = true;
		this.settings_foldout = true;
		this.frequency = (float)256;
		this.offset_range = new Vector2((float)5, (float)5);
		this.detail = 1;
		this.detail_strength = (float)2;
		this.strength = (float)1;
	}

	public override void set_zero()
	{
		this.curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)0);
	}

	public override void set_invert()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		for (int i = 0; i < this.curve.keys.Length; i++)
		{
			Keyframe key = this.curve.keys[i];
			key.value = (float)1 - key.value;
			float inTangent = key.inTangent;
			key.inTangent *= (float)-1;
			key.outTangent *= (float)-1;
			animationCurve.AddKey(key);
		}
		this.curve = new AnimationCurve(animationCurve.keys);
	}

	public override void set_default()
	{
		this.curve = new AnimationCurve(this.default_curve.keys);
	}

	public override void set_as_default()
	{
		this.default_curve = new AnimationCurve(this.curve.keys);
	}

	public override void default_normal()
	{
		this.curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)1);
		this.type = curve_type_enum.Normal;
	}

	public override void default_random()
	{
		this.curve = AnimationCurve.Linear((float)0, (float)0, (float)1, (float)0);
		this.type = curve_type_enum.Random;
	}

	public override void default_perlin()
	{
		this.curve = AnimationCurve.Linear((float)0, (float)1, (float)1, (float)1);
		this.type = curve_type_enum.Perlin;
		this.frequency = (float)256;
		this.offset = new Vector2((float)0, (float)0);
		this.offset_middle = new Vector2((float)0, (float)0);
		this.detail = 7;
		this.detail_strength = (float)2;
		this.abs = false;
	}

	public override void change_key(float time, float value)
	{
		if (this.curve.AddKey(time, value) == -1)
		{
			Keyframe[] keys = this.curve.keys;
			for (int i = 0; i < this.curve.keys.Length; i++)
			{
				if (keys[i].time == time)
				{
					keys[i].value = value;
				}
			}
			this.curve = new AnimationCurve(keys);
		}
	}

	public override void set_curve_linear()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		for (int i = 0; i < this.curve.keys.Length; i++)
		{
			float inTangent = (float)0;
			float outTangent = (float)0;
			bool flag = false;
			bool flag2 = false;
			Vector2 b = default(Vector2);
			Vector2 a = default(Vector2);
			Vector2 vector = default(Vector2);
			Keyframe key = this.curve[i];
			if (i == 0)
			{
				inTangent = (float)0;
				flag = true;
			}
			if (i == this.curve.keys.Length - 1)
			{
				outTangent = (float)0;
				flag2 = true;
			}
			if (!flag)
			{
				b.x = this.curve.keys[i - 1].time;
				b.y = this.curve.keys[i - 1].value;
				a.x = this.curve.keys[i].time;
				a.y = this.curve.keys[i].value;
				vector = a - b;
				inTangent = vector.y / vector.x;
			}
			if (!flag2)
			{
				b.x = this.curve.keys[i].time;
				b.y = this.curve.keys[i].value;
				a.x = this.curve.keys[i + 1].time;
				a.y = this.curve.keys[i + 1].value;
				vector = a - b;
				outTangent = vector.y / vector.x;
			}
			key.inTangent = inTangent;
			key.outTangent = outTangent;
			animationCurve.AddKey(key);
		}
		this.curve = animationCurve;
	}
}
