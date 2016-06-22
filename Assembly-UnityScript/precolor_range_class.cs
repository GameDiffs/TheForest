using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class precolor_range_class
{
	public bool foldout;

	public bool color_ranges_active;

	public bool color_ranges_foldout;

	public bool palette;

	public AnimationCurve curve_red;

	public AnimationCurve curve_green;

	public AnimationCurve curve_blue;

	public animation_curve_math_class animation_curve_math;

	public bool interface_display;

	public int index;

	public bool one_color;

	public Rect menu_rect;

	public string text;

	public Rect rect;

	public int detect_max;

	public List<color_range_class> color_range;

	public value_class color_range_value;

	public precolor_range_class(int length, bool one_color1)
	{
		this.foldout = true;
		this.color_ranges_active = true;
		this.color_ranges_foldout = true;
		this.curve_red = new AnimationCurve();
		this.curve_green = new AnimationCurve();
		this.curve_blue = new AnimationCurve();
		this.animation_curve_math = new animation_curve_math_class();
		this.interface_display = true;
		this.text = "Color Range:";
		this.detect_max = 8;
		this.color_range = new List<color_range_class>();
		this.color_range_value = new value_class();
		for (int i = 0; i < length; i++)
		{
			this.add_color_range(i, one_color1);
		}
		this.one_color = one_color1;
		this.set_color_range_text();
		this.set_precolor_range_curve();
	}

	public override void add_color_range(int color_range_number, bool one_color)
	{
		this.color_range.Insert(color_range_number, new color_range_class());
		this.color_range[color_range_number].one_color = one_color;
		this.color_range[color_range_number].select_output = color_range_number;
		this.color_range_value.add_value(color_range_number, (float)50);
		this.set_precolor_range_curve();
		this.set_color_range_text();
	}

	public override void erase_color_range(int color_range_number)
	{
		if (this.color_range.Count > 0)
		{
			this.color_range.RemoveAt(color_range_number);
			this.color_range_value.erase_value(color_range_number);
			this.set_precolor_range_curve();
			this.set_color_range_text();
		}
	}

	public override void clear_color_range()
	{
		this.color_range.Clear();
		this.color_range_value.clear_value();
		this.set_precolor_range_curve();
		this.set_color_range_text();
	}

	public override void invert_color_range(int color_range_number)
	{
		float num = 0.003921569f;
		this.color_range_value.active[color_range_number] = false;
		this.add_color_range(color_range_number + 1, false);
		this.add_color_range(color_range_number + 1, false);
		if (this.color_range[color_range_number].color_start != new Color((float)0, (float)0, (float)0))
		{
			if (color_range_number < 2)
			{
				this.color_range[color_range_number + 1].color_start = new Color((float)0, (float)0, (float)0);
			}
			else
			{
				this.color_range[color_range_number + 1].color_start = this.color_range[color_range_number - 1].color_end + new Color(num, num, num);
			}
			this.color_range[color_range_number + 1].color_end = this.color_range[color_range_number].color_start + new Color(-num, -num, -num);
		}
		if (this.color_range[color_range_number].color_end != new Color((float)1, (float)1, (float)1))
		{
			this.color_range[color_range_number + 2].color_start = this.color_range[color_range_number].color_end + new Color(num, num, num);
			if (this.color_range.Count - 1 == color_range_number + 2)
			{
				this.color_range[color_range_number + 2].color_end = new Color((float)1, (float)1, (float)1);
			}
			else
			{
				this.color_range[color_range_number + 2].color_end = this.color_range[color_range_number + 3].color_start + new Color(-num, -num, -num);
			}
		}
	}

	public override void set_color_range_text()
	{
		this.text = "Color Range" + this.index + " (" + this.color_range.Count + ")";
	}

	public override void swap_color(int color_range_number1, int color_range_number2)
	{
		float num = 0f;
		if (color_range_number2 > -1 && color_range_number2 < this.color_range.Count)
		{
			color_range_class value = this.color_range[color_range_number1];
			this.color_range[color_range_number1] = this.color_range[color_range_number2];
			this.color_range[color_range_number2] = value;
			if (this.color_range[color_range_number1].color_color_range[0] < 1.5f)
			{
				this.color_range[color_range_number1].color_color_range = this.color_range[color_range_number1].color_color_range + new Color((float)1, (float)1, (float)1, (float)1);
			}
			if (this.color_range[color_range_number2].color_color_range[0] < 1.5f)
			{
				this.color_range[color_range_number2].color_color_range = this.color_range[color_range_number2].color_color_range + new Color((float)1, (float)1, (float)1, (float)1);
			}
			this.color_range_value.swap_value(color_range_number1, color_range_number2);
			this.set_precolor_range_curve();
		}
	}

	public override void detect_colors_image(Texture2D texture)
	{
		int num = 0;
		Color color = default(Color);
		int i = 0;
		for (int j = 0; j < texture.height; j++)
		{
			for (int k = 0; k < texture.width; k++)
			{
				color = texture.GetPixel(k, j);
				bool flag = false;
				for (i = 0; i < this.color_range.Count; i++)
				{
					if (color == this.color_range[i].color_start)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					this.add_color_range(this.color_range.Count, true);
					this.color_range[this.color_range.Count - 1].color_start = color;
					num++;
					if (num > this.detect_max - 1)
					{
						return;
					}
				}
			}
		}
	}

	public override void set_precolor_range_curve()
	{
		this.curve_red = new AnimationCurve();
		this.curve_green = new AnimationCurve();
		this.curve_blue = new AnimationCurve();
		float num = (float)this.color_range.Count;
		if (num > (float)1)
		{
			float num2 = (float)1 / (num - (float)1);
			int num3 = 0;
			while ((float)num3 < num)
			{
				int num4 = 0;
				num4 = this.curve_red.AddKey((float)num3 * num2, this.color_range[num3].color_start.r);
				num4 = this.curve_green.AddKey((float)num3 * num2, this.color_range[num3].color_start.g);
				num4 = this.curve_blue.AddKey((float)num3 * num2, this.color_range[num3].color_start.b);
				num3++;
			}
		}
		else if (num == (float)1)
		{
			this.curve_red.AddKey((float)0, this.color_range[0].color_start.r);
			this.curve_red.AddKey((float)1, this.color_range[0].color_start.r);
			this.curve_green.AddKey((float)0, this.color_range[0].color_start.g);
			this.curve_green.AddKey((float)1, this.color_range[0].color_start.g);
			this.curve_blue.AddKey((float)0, this.color_range[0].color_start.b);
			this.curve_blue.AddKey((float)1, this.color_range[0].color_start.b);
		}
		this.curve_red = this.animation_curve_math.set_curve_linear(this.curve_red);
		this.curve_green = this.animation_curve_math.set_curve_linear(this.curve_green);
		this.curve_blue = this.animation_curve_math.set_curve_linear(this.curve_blue);
	}
}
