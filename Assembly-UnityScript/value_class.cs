using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class value_class
{
	public List<float> value;

	public List<float> select_value;

	public List<bool> active;

	public List<string> text;

	public float value_total;

	public int active_total;

	public AnimationCurve curve;

	public animation_curve_math_class animation_curve_math;

	public value_class()
	{
		this.value = new List<float>();
		this.select_value = new List<float>();
		this.active = new List<bool>();
		this.text = new List<string>();
		this.curve = new AnimationCurve();
		this.animation_curve_math = new animation_curve_math_class();
	}

	public override void calc_value()
	{
		if (this.select_value.Count < this.value.Count)
		{
			this.select_value_init();
		}
		this.value_total = (float)0;
		this.active_total = 0;
		float num = (float)0;
		for (int i = 0; i < this.value.Count; i++)
		{
			if (this.active[i])
			{
				this.value_total += this.value[i];
				this.active_total++;
			}
		}
		Keyframe[] array = new Keyframe[this.active_total + 1];
		int num2 = 0;
		for (int i = 0; i < this.value.Count; i++)
		{
			if (this.active[i])
			{
				array[num2].value = 1f / ((float)this.active_total * 1f) * (float)(num2 + 1);
				array[num2].time = num + this.value[i] / this.value_total;
				this.select_value[i] = (num * (float)2 + this.value[i] / this.value_total) / (float)2;
				this.text[i] = "(V " + num.ToString("F2") + "-" + array[num2].time.ToString("F2") + ")";
				num = array[num2].time;
				num2++;
			}
			else
			{
				this.text[i] = "(V - )";
			}
		}
		this.curve = this.animation_curve_math.set_curve_linear(new AnimationCurve(array));
	}

	public override void add_value(int value_index, float number)
	{
		if (this.select_value.Count < this.value.Count)
		{
			int num = this.value.Count - this.select_value.Count;
			for (int i = 0; i < num; i++)
			{
				this.select_value.Add((float)0);
			}
		}
		this.value.Insert(value_index, number);
		this.select_value.Insert(value_index, (float)0);
		this.text.Insert(value_index, string.Empty);
		this.active.Insert(value_index, true);
		this.calc_value();
	}

	public override void erase_value(int value_index)
	{
		this.value.RemoveAt(value_index);
		if (value_index < this.select_value.Count)
		{
			this.select_value.RemoveAt(value_index);
		}
		this.text.RemoveAt(value_index);
		this.active.RemoveAt(value_index);
		this.calc_value();
	}

	public override void clear_value()
	{
		this.value.Clear();
		this.select_value.Clear();
		this.text.Clear();
		this.active.Clear();
	}

	public override void swap_value(int value_number1, int value_number2)
	{
		if (this.select_value.Count < this.value.Count)
		{
			this.select_value_init();
		}
		float num = this.value[value_number1];
		bool flag = this.active[value_number1];
		float num2 = this.select_value[value_number1];
		this.value[value_number1] = this.value[value_number2];
		this.value[value_number2] = num;
		this.active[value_number1] = this.active[value_number2];
		this.active[value_number2] = flag;
		this.select_value[value_number1] = this.select_value[value_number2];
		this.select_value[value_number2] = num2;
		this.calc_value();
	}

	public override void select_value_init()
	{
		int num = this.value.Count - this.select_value.Count;
		for (int i = 0; i < num; i++)
		{
			this.select_value.Add((float)0);
		}
	}
}
