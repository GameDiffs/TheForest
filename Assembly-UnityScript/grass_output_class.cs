using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class grass_output_class
{
	public bool active;

	public bool foldout;

	public Color color_grass;

	public float strength;

	public int grass_terrain;

	public string grass_text;

	public List<float> mix;

	public mix_mode_enum mix_mode;

	public animation_curve_math_class animation_curve_math;

	public List<animation_curve_class> curves;

	public List<grass_class> grass;

	public value_class grass_value;

	public List<float> grass_calc;

	public grass_output_class()
	{
		this.active = true;
		this.foldout = true;
		this.color_grass = new Color((float)2, (float)2, (float)2, (float)1);
		this.strength = (float)1;
		this.grass_text = "Grass:";
		this.mix = new List<float>();
		this.animation_curve_math = new animation_curve_math_class();
		this.curves = new List<animation_curve_class>();
		this.grass = new List<grass_class>();
		this.grass_value = new value_class();
		this.grass_calc = new List<float>();
		this.add_grass(0);
	}

	public override void add_grass(int grass_number)
	{
		this.grass.Insert(grass_number, new grass_class());
		this.grass_calc.Insert(grass_number, (float)0);
		this.curves.Insert(grass_number, new animation_curve_class());
		this.mix.Insert(grass_number, 0.5f);
		this.grass_value.add_value(grass_number, (float)50);
		this.set_grass_curve();
		this.set_grass_text();
	}

	public override void erase_grass(int grass_number)
	{
		if (this.grass.Count > 0)
		{
			this.grass.RemoveAt(grass_number);
			this.grass_calc.RemoveAt(grass_number);
			this.grass_value.erase_value(grass_number);
			this.mix.RemoveAt(grass_number);
			this.curves.RemoveAt(grass_number);
			this.set_grass_curve();
			this.set_grass_text();
		}
	}

	public override void clear_grass()
	{
		this.grass.Clear();
		this.grass_calc.Clear();
		this.grass_value.clear_value();
		this.mix.Clear();
		this.curves.Clear();
		this.set_grass_curve();
		this.set_grass_text();
	}

	public override bool swap_grass(int grass_number, int grass_number2)
	{
		bool arg_E3_0;
		if (grass_number2 > -1 && grass_number2 < this.grass.Count)
		{
			grass_class value = this.grass[grass_number];
			float value2 = this.grass_value.value[grass_number];
			bool value3 = this.grass_value.active[grass_number];
			this.grass[grass_number] = this.grass[grass_number2];
			this.grass[grass_number2] = value;
			this.grass_value.value[grass_number] = this.grass_value.value[grass_number2];
			this.grass_value.value[grass_number2] = value2;
			this.grass_value.active[grass_number] = this.grass_value.active[grass_number2];
			this.grass_value.active[grass_number2] = value3;
			this.set_grass_curve();
			arg_E3_0 = true;
		}
		else
		{
			arg_E3_0 = false;
		}
		return arg_E3_0;
	}

	public override void set_grass_curve()
	{
		float num = (float)this.curves.Count;
		int num2 = 0;
		int num3 = 0;
		Keyframe[] array = null;
		for (int i = 0; i < this.curves.Count; i++)
		{
			if (!this.grass_value.active[i])
			{
				this.curves[i].curve = AnimationCurve.Linear((float)0, (float)0, (float)0, (float)0);
				num -= (float)1;
			}
		}
		if (num == (float)1)
		{
			this.curves[0].curve = AnimationCurve.Linear((float)0, (float)1, (float)1, (float)1);
		}
		else
		{
			float num4 = 0f;
			num4 = (float)1 / num;
			float num5 = 0f;
			float num6 = 0f;
			for (int i = 0; i < this.curves.Count; i++)
			{
				if (!this.grass_value.active[i])
				{
					num2++;
				}
				else
				{
					num3 = i - num2;
					this.curves[i].curve = new AnimationCurve();
					if (this.mix_mode == mix_mode_enum.Single)
					{
						if (num3 == 0)
						{
							num5 = ((float)1 - this.mix[1]) * (num4 / (float)2);
						}
						if (num3 > 0 && (float)num3 < num - (float)1)
						{
							num5 = ((float)1 - this.mix[i]) * (num4 / (float)2);
							num6 = ((float)1 - this.mix[i + 1]) * (num4 / (float)2);
						}
						if ((float)num3 == num - (float)1)
						{
							num6 = ((float)1 - this.mix[i]) * (num4 / (float)2);
						}
					}
					else
					{
						num5 = ((float)1 - this.mix[0]) * (num4 / (float)2);
						num6 = ((float)1 - this.mix[0]) * (num4 / (float)2);
					}
					if (num > (float)1)
					{
						if (num3 == 0)
						{
							array = new Keyframe[]
							{
								new Keyframe((float)0, (float)1),
								new Keyframe(num5 + num4 / (float)2, (float)1),
								new Keyframe(num4 * (float)(num3 + 1) - num5 + 1E-07f + num4 / (float)2, (float)0)
							};
						}
						if (num3 > 0 && (float)num3 < num - (float)1)
						{
							array = new Keyframe[4];
							array[0] = new Keyframe(num4 * (float)(num3 - 1) + num5 - 1E-07f + num4 / (float)2, (float)0);
							array[1] = new Keyframe(num4 * (float)num3 - num5 + num4 / (float)2, (float)1);
							if (!Mathf.Approximately(num4 * (float)num3 - num5 + num4 / (float)2, num4 * (float)num3 + num6 + num4 / (float)2))
							{
								array[2] = new Keyframe(num4 * (float)num3 + num6 + num4 / (float)2, (float)1);
							}
							array[3] = new Keyframe(num4 * (float)(num3 + 1) - num6 + 1E-07f + num4 / (float)2, (float)0);
						}
						if ((float)num3 == num - (float)1)
						{
							array = new Keyframe[]
							{
								new Keyframe(num4 * (float)(num3 - 1) + num6 - 1E-07f + num4 / (float)2, (float)0),
								new Keyframe((float)1 - num6 - num4 / (float)2, (float)1),
								new Keyframe((float)1, (float)1)
							};
						}
						this.curves[i].curve = this.animation_curve_math.set_curve_linear(new AnimationCurve(array));
					}
				}
			}
		}
	}

	public override void set_grass_text()
	{
		if (this.grass.Count > 1)
		{
			this.grass_text = "Grass(" + this.grass.Count + ")";
		}
		else
		{
			this.grass_text = "Grass";
		}
	}
}
