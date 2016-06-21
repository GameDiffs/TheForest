using System;

public struct int2
{
	public int x;

	public int y;

	public int2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static int2 operator +(int2 a, int2 b)
	{
		return new int2(a.x + b.x, a.y + b.y);
	}

	public static int2 operator -(int2 a, int2 b)
	{
		return new int2(a.x - b.x, a.y - b.y);
	}

	public static int2 operator *(int2 a, int2 b)
	{
		return new int2(a.x * b.x, a.y * b.y);
	}

	public static int2 operator /(int2 a, int2 b)
	{
		return new int2(a.x / b.x, a.y / b.y);
	}

	public static int2 operator +(int2 a, int b)
	{
		return new int2(a.x + b, a.y + b);
	}

	public static int2 operator -(int2 a, int b)
	{
		return new int2(a.x - b, a.y - b);
	}

	public static int2 operator *(int2 a, int b)
	{
		return new int2(a.x * b, a.y * b);
	}

	public static int2 operator /(int2 a, int b)
	{
		return new int2(a.x / b, a.y / b);
	}
}
