using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class splat_output_class
{
	public bool active;

	public bool foldout;

	public Color color_splat;

	public float strength;

	public List<float> mix;

	public string splat_text;

	public int splat_terrain;

	public bool terrain_splat_assigned;

	public mix_mode_enum mix_mode;

	public List<animation_curve_class> curves;

	public animation_curve_math_class animation_curve_math;

	public List<int> splat;

	public value_class splat_value;

	public List<float> splat_calc;

	public List<string> swap_text;

	public splat_output_class()
	{
		this.foldout = true;
		this.color_splat = new Color((float)2, (float)2, (float)2, (float)1);
		this.strength = (float)1;
		this.mix = new List<float>();
		this.splat_text = "Splat:";
		this.curves = new List<animation_curve_class>();
		this.animation_curve_math = new animation_curve_math_class();
		this.splat = new List<int>();
		this.splat_value = new value_class();
		this.splat_calc = new List<float>();
		this.swap_text = new List<string>();
		for (int i = 0; i < 3; i++)
		{
			this.add_splat(i, i);
		}
	}

	public override void add_splat(int splat_number, int splat_index)
	{
		this.splat.Insert(splat_number, splat_index);
		this.splat_calc.Insert(splat_number, (float)0);
		this.curves.Insert(splat_number, new animation_curve_class());
		this.mix.Insert(splat_number, 0.5f);
		this.splat_value.add_value(splat_number, (float)50);
		this.swap_text.Insert(splat_number, "S");
		this.set_splat_curve();
		this.set_splat_text();
	}

	public override void erase_splat(int splat_number)
	{
		if (this.splat.Count > 0)
		{
			this.splat.RemoveAt(splat_number);
			this.splat_calc.RemoveAt(splat_number);
			this.mix.RemoveAt(splat_number);
			this.curves.RemoveAt(splat_number);
			this.splat_value.erase_value(splat_number);
			this.swap_text.RemoveAt(splat_number);
			this.set_splat_curve();
			this.set_splat_text();
		}
	}

	public override void clear_splat()
	{
		this.splat.Clear();
		this.splat_value.clear_value();
		this.mix.Clear();
		this.curves.Clear();
		this.swap_text.Clear();
		this.set_splat_curve();
		this.set_splat_text();
	}

	public override void swap_splat(int splat_number1, int splat_number2)
	{
		if (splat_number2 > -1 && splat_number2 < this.splat.Count)
		{
			float num = (float)this.splat[splat_number1];
			float num2 = this.splat_value.value[splat_number1];
			this.splat[splat_number1] = this.splat[splat_number2];
			this.splat[splat_number2] = (int)num;
			this.splat_value.swap_value(splat_number1, splat_number2);
			this.set_splat_curve();
		}
	}

	public override void set_splat_curve()
	{
		float num = (float)this.curves.Count;
		int num2 = 0;
		int num3 = 0;
		Keyframe[] array = null;
		for (int i = 0; i < this.curves.Count; i++)
		{
			if (!this.splat_value.active[i])
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
				if (!this.splat_value.active[i])
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

	public override void set_splat_text()
	{
		if (this.splat.Count > 1)
		{
			this.splat_text = "Splats(" + this.splat.Count + ")";
		}
		else
		{
			this.splat_text = "Splat";
		}
	}
}
