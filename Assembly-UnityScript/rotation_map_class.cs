using System;
using UnityEngine;

[Serializable]
public class rotation_map_class
{
	public bool active;

	public image_class preimage;

	public rotation_map_class()
	{
		this.preimage = new image_class();
	}

	public override float calc_rotation(Color color)
	{
		return (!(color == new Color((float)1, (float)1, (float)1))) ? (-color.r * (float)255 + -color.g * (float)255 + -color.b) : ((float)0);
	}
}
