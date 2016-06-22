using System;
using System.Collections.Generic;

[Serializable]
public class color_output_class
{
	public bool active;

	public bool foldout;

	public float strength;

	public string[] precolor_range_enum;

	public List<precolor_range_class> precolor_range;

	public string color_text;

	public color_output_class()
	{
		this.foldout = true;
		this.strength = (float)1;
		this.precolor_range_enum = new string[1];
		this.precolor_range = new List<precolor_range_class>();
		this.color_text = "Color Outputs:";
		this.precolor_range.Add(new precolor_range_class(1, true));
		this.set_precolor_range_enum();
	}

	public override void set_precolor_range_enum()
	{
		this.precolor_range_enum = new string[this.precolor_range.Count];
		for (int i = 0; i < this.precolor_range.Count; i++)
		{
			this.precolor_range_enum[i] = "Color Range" + i;
		}
	}

	public override void set_precolor_range_length(int length_new)
	{
		if (length_new != this.precolor_range.Count)
		{
			if (length_new > this.precolor_range.Count)
			{
				this.precolor_range.Add(new precolor_range_class(1, true));
				this.precolor_range[this.precolor_range.Count - 1].index = length_new - 1;
				this.precolor_range[this.precolor_range.Count - 1].set_color_range_text();
			}
			else
			{
				this.precolor_range.RemoveAt(this.precolor_range.Count - 1);
			}
			this.set_precolor_range_enum();
		}
	}

	public override void add_precolor_range(int precolor_range_number)
	{
		this.precolor_range.Insert(precolor_range_number, new precolor_range_class(1, true));
		this.precolor_range[precolor_range_number].index = precolor_range_number;
		this.precolor_range[precolor_range_number].set_color_range_text();
	}

	public override void erase_precolor_range(int precolor_range_number)
	{
		this.precolor_range.RemoveAt(precolor_range_number);
		this.set_precolor_range_enum();
	}
}
