using System;
using UnityEngine;

[Serializable]
public class BitmapData
{
	public int height;

	public int width;

	private Color[] pixels;

	public BitmapData(Texture2D texture)
	{
		this.height = texture.height;
		this.width = texture.width;
		this.pixels = texture.GetPixels();
	}

	public override Color getPixelColor(int x, int y)
	{
		if (x >= this.width)
		{
			x = this.width - 1;
		}
		if (y >= this.height)
		{
			y = this.height - 1;
		}
		if (x < 0)
		{
			x = 0;
		}
		if (y < 0)
		{
			y = 0;
		}
		return this.pixels[y * this.width + x];
	}
}
