using System;
using System.IO;
using UnityEngine;

[Serializable]
public class buffer_class
{
	public FileStream file;

	public Vector2 resolution;

	public byte[] bytes;

	public ulong length;

	public Vector2 size;

	public tile_class tiles;

	public ulong pos;

	public ulong row;

	public Rect innerRect;

	public Rect outerRect;

	public Vector2 offset;

	public int radius;

	public buffer_class()
	{
		this.tiles = new tile_class();
	}

	public override void init()
	{
		this.tiles.x = (int)Mathf.Ceil(this.resolution.x / this.size.x);
		this.tiles.y = (int)Mathf.Ceil(this.resolution.x / this.size.y);
		this.row = (ulong)(this.resolution.x * (float)3);
	}

	public override void getRects(tile_class tile)
	{
		int num = this.radius + 20;
		this.innerRect.x = (float)tile.x * this.size.x - (float)5;
		this.innerRect.y = (float)tile.y * this.size.y - (float)5;
		this.innerRect.width = this.size.x + (float)10;
		this.innerRect.height = this.size.y + (float)10;
		if (this.innerRect.xMin < (float)0)
		{
			this.innerRect.xMin = (float)0;
		}
		if (this.innerRect.yMin < (float)0)
		{
			this.innerRect.yMin = (float)0;
		}
		if (this.innerRect.xMax > this.resolution.x)
		{
			this.innerRect.xMax = this.resolution.x;
		}
		if (this.innerRect.yMax > this.resolution.y)
		{
			this.innerRect.yMax = this.resolution.y;
		}
		this.outerRect.xMin = this.innerRect.xMin - (float)num;
		this.outerRect.yMin = this.innerRect.yMin - (float)num;
		this.outerRect.xMax = this.innerRect.xMax + (float)num;
		this.outerRect.yMax = this.innerRect.yMax + (float)num;
		if (this.outerRect.xMin < (float)0)
		{
			this.outerRect.xMin = (float)0;
		}
		else if (this.outerRect.xMax > this.resolution.x)
		{
			this.outerRect.xMax = this.resolution.x;
		}
		if (this.outerRect.yMin < (float)0)
		{
			this.outerRect.yMin = (float)0;
		}
		else if (this.outerRect.yMax > this.resolution.y)
		{
			this.outerRect.yMax = this.resolution.y;
		}
		this.length = (ulong)(this.outerRect.width * this.outerRect.height * (float)3);
		this.offset.x = this.innerRect.x - this.outerRect.x;
		this.offset.y = this.innerRect.y - this.outerRect.y;
		if (this.bytes == null)
		{
			this.bytes = new byte[(int)this.length];
		}
		if ((long)this.bytes.Length != (long)this.length)
		{
			this.bytes = new byte[(int)this.length];
		}
	}

	public override void read()
	{
		int num = 0;
		while ((float)num < this.outerRect.height)
		{
			this.pos = (ulong)((float)this.row * this.outerRect.y + (float)((long)this.row * (long)num) + this.outerRect.x * (float)3);
			this.file.Seek((long)this.pos, SeekOrigin.Begin);
			this.file.Read(this.bytes, (int)(this.outerRect.width * (float)num * (float)3), (int)(this.outerRect.width * (float)3));
			num++;
		}
	}

	public override void write()
	{
		int num = 0;
		while ((float)num < this.innerRect.height)
		{
			this.pos = (ulong)((float)this.row * this.innerRect.y + (float)((long)this.row * (long)num) + this.innerRect.x * (float)3);
			this.file.Seek((long)this.pos, SeekOrigin.Begin);
			this.file.Write(this.bytes, (int)(this.outerRect.width * (float)num * (float)3 + this.outerRect.width * (float)3 * this.offset.y + this.offset.x * (float)3), (int)(this.innerRect.width * (float)3));
			num++;
		}
	}

	public override void copy_bytes(byte[] bytes1, byte[] bytes2)
	{
		ulong num = (ulong)0;
		while ((long)num < (long)bytes1.Length)
		{
			bytes2[(int)num] = bytes1[(int)num];
			num = (ulong)((long)num + (long)((ulong)1));
		}
	}

	public override void clear_bytes()
	{
		ulong num = (ulong)0;
		while ((long)num < (long)this.bytes.Length)
		{
			this.bytes[(int)num] = 0;
			num = (ulong)((long)num + (long)((ulong)1));
		}
	}
}
