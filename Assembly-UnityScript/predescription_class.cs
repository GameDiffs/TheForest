using System;
using System.Collections.Generic;

[Serializable]
public class predescription_class
{
	public List<description_class> description;

	public string[] description_enum;

	public int description_position;

	public int layer_index;

	public int description_index;

	public predescription_class()
	{
		this.description = new List<description_class>();
		this.description.Add(new description_class());
		this.set_description_enum();
	}

	public override void add_description(int description_number)
	{
		this.description.Insert(description_number, new description_class());
	}

	public override void erase_description(int description_number)
	{
		if (this.description.Count > 1)
		{
			this.description.RemoveAt(description_number);
			this.set_description_enum();
			if (this.description_position > this.description_enum.Length - 1)
			{
				this.description_position = this.description_enum.Length - 1;
			}
		}
	}

	public override void add_layer_index(int layer_number, int layer_index, int description_number)
	{
		this.move_layer_index(layer_number, 1);
		this.description[description_number].layer_index.Insert(layer_index, layer_number);
	}

	public override void erase_layer_index(int layer_number, int layer_index, int description_number)
	{
		this.move_layer_index(layer_number, -1);
		this.description[description_number].layer_index.RemoveAt(layer_index);
	}

	public override void move_layer_index(int layer_number, int direction)
	{
		for (int i = 0; i < this.description.Count; i++)
		{
			for (int j = 0; j < this.description[i].layer_index.Count; j++)
			{
				if (this.description[i].layer_index[j] >= layer_number)
				{
					this.description[i].layer_index[j] = this.description[i].layer_index[j] + direction;
				}
			}
		}
	}

	public override void search_layer(int layer_number)
	{
		for (int i = 0; i < this.description.Count; i++)
		{
			for (int j = 0; j < this.description[i].layer_index.Count; j++)
			{
				if (this.description[i].layer_index[j] == layer_number)
				{
					this.description_index = i;
					this.layer_index = j;
					return;
				}
			}
		}
		this.description_index = -1;
		this.layer_index = -1;
	}

	public override void set_description_enum()
	{
		this.description_enum = new string[this.description.Count];
		for (int i = 0; i < this.description.Count; i++)
		{
			this.description_enum[i] = this.description[i].text;
		}
	}
}
