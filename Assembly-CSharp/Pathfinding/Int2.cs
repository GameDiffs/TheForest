using System;

namespace Pathfinding
{
	public struct Int2
	{
		public int x;

		public int y;

		private static readonly int[] Rotations = new int[]
		{
			1,
			0,
			0,
			1,
			0,
			1,
			-1,
			0,
			-1,
			0,
			0,
			-1,
			0,
			-1,
			1,
			0
		};

		public int sqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		public long sqrMagnitudeLong
		{
			get
			{
				return (long)this.x * (long)this.x + (long)this.y * (long)this.y;
			}
		}

		public Int2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static int Dot(Int2 a, Int2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		public static long DotLong(Int2 a, Int2 b)
		{
			return (long)a.x * (long)b.x + (long)a.y * (long)b.y;
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			Int2 @int = (Int2)o;
			return this.x == @int.x && this.y == @int.y;
		}

		public override int GetHashCode()
		{
			return this.x * 49157 + this.y * 98317;
		}

		public static Int2 Rotate(Int2 v, int r)
		{
			r %= 4;
			return new Int2(v.x * Int2.Rotations[r * 4] + v.y * Int2.Rotations[r * 4 + 1], v.x * Int2.Rotations[r * 4 + 2] + v.y * Int2.Rotations[r * 4 + 3]);
		}

		public static Int2 Min(Int2 a, Int2 b)
		{
			return new Int2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
		}

		public static Int2 Max(Int2 a, Int2 b)
		{
			return new Int2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
		}

		public static Int2 FromInt3XZ(Int3 o)
		{
			return new Int2(o.x, o.z);
		}

		public static Int3 ToInt3XZ(Int2 o)
		{
			return new Int3(o.x, 0, o.y);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(",
				this.x,
				", ",
				this.y,
				")"
			});
		}

		public static Int2 operator +(Int2 a, Int2 b)
		{
			return new Int2(a.x + b.x, a.y + b.y);
		}

		public static Int2 operator -(Int2 a, Int2 b)
		{
			return new Int2(a.x - b.x, a.y - b.y);
		}

		public static bool operator ==(Int2 a, Int2 b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Int2 a, Int2 b)
		{
			return a.x != b.x || a.y != b.y;
		}
	}
}
