using System;

[Serializable]
public class map_pixel_class
{
	public double x;

	public double y;

	public override void reset()
	{
		this.x = (double)0;
		this.y = (double)0;
	}
}
