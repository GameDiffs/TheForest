using System;
using UnityEngine;

[Serializable]
public class texture_tool_class
{
	public bool active;

	public image_class preimage;

	public Vector2 resolution_display;

	public float scale;

	public Rect rect;

	public precolor_range_class precolor_range;

	public texture_tool_class()
	{
		this.active = true;
		this.preimage = new image_class();
		this.resolution_display = new Vector2((float)512, (float)512);
		this.scale = (float)1;
		this.precolor_range = new precolor_range_class(0, false);
	}
}
