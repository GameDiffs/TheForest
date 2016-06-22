using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class gui_class
{
	public List<float> column;

	public float y;

	public float x;

	public gui_class(int columns)
	{
		this.column = new List<float>();
		for (int i = 0; i < columns; i++)
		{
			this.column.Add((float)0);
		}
	}

	public override Rect getRect(int column_index, float width, float y1, bool add_width, bool add_height)
	{
		float num = this.x;
		float top = this.y;
		if (add_width)
		{
			this.x += width;
		}
		if (add_height)
		{
			this.y += y1;
		}
		return new Rect(this.column[column_index] + num, top, width, y1);
	}

	public override Rect getRect(int column_index, float x1, float width, float y1, bool add_width, bool add_height)
	{
		float num = this.x;
		float top = this.y;
		if (add_width)
		{
			this.x += width + x1;
		}
		if (add_height)
		{
			this.y += y1;
		}
		return new Rect(this.column[column_index] + num + x1, top, width, y1);
	}
}
