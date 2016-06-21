using System;
using UnityEngine;

namespace Pathfinding
{
	public struct Int3
	{
		public const int Precision = 1000;

		public const float FloatPrecision = 1000f;

		public const float PrecisionFactor = 0.001f;

		public int x;

		public int y;

		public int z;

		private static Int3 _zero = new Int3(0, 0, 0);

		public static Int3 zero
		{
			get
			{
				return Int3._zero;
			}
		}

		public int this[int i]
		{
			get
			{
				return (i != 0) ? ((i != 1) ? this.z : this.y) : this.x;
			}
			set
			{
				if (i == 0)
				{
					this.x = value;
				}
				else if (i == 1)
				{
					this.y = value;
				}
				else
				{
					this.z = value;
				}
			}
		}

		public float magnitude
		{
			get
			{
				double num = (double)this.x;
				double num2 = (double)this.y;
				double num3 = (double)this.z;
				return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
			}
		}

		public int costMagnitude
		{
			get
			{
				return (int)Math.Round((double)this.magnitude);
			}
		}

		public float worldMagnitude
		{
			get
			{
				double num = (double)this.x;
				double num2 = (double)this.y;
				double num3 = (double)this.z;
				return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3) * 0.001f;
			}
		}

		public float sqrMagnitude
		{
			get
			{
				double num = (double)this.x;
				double num2 = (double)this.y;
				double num3 = (double)this.z;
				return (float)(num * num + num2 * num2 + num3 * num3);
			}
		}

		public long sqrMagnitudeLong
		{
			get
			{
				long num = (long)this.x;
				long num2 = (long)this.y;
				long num3 = (long)this.z;
				return num * num + num2 * num2 + num3 * num3;
			}
		}

		public int unsafeSqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		public Int3(Vector3 position)
		{
			this.x = (int)Math.Round((double)(position.x * 1000f));
			this.y = (int)Math.Round((double)(position.y * 1000f));
			this.z = (int)Math.Round((double)(position.z * 1000f));
		}

		public Int3(int _x, int _y, int _z)
		{
			this.x = _x;
			this.y = _y;
			this.z = _z;
		}

		public Int3 DivBy2()
		{
			this.x >>= 1;
			this.y >>= 1;
			this.z >>= 1;
			return this;
		}

		public static float Angle(Int3 lhs, Int3 rhs)
		{
			double num = (double)Int3.Dot(lhs, rhs) / ((double)lhs.magnitude * (double)rhs.magnitude);
			num = ((num >= -1.0) ? ((num <= 1.0) ? num : 1.0) : -1.0);
			return (float)Math.Acos(num);
		}

		public static int Dot(Int3 lhs, Int3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		public static long DotLong(Int3 lhs, Int3 rhs)
		{
			return (long)lhs.x * (long)rhs.x + (long)lhs.y * (long)rhs.y + (long)lhs.z * (long)rhs.z;
		}

		public Int3 Normal2D()
		{
			return new Int3(this.z, this.y, -this.x);
		}

		public Int3 NormalizeTo(int newMagn)
		{
			float magnitude = this.magnitude;
			if (magnitude == 0f)
			{
				return this;
			}
			this.x *= newMagn;
			this.y *= newMagn;
			this.z *= newMagn;
			this.x = (int)Math.Round((double)((float)this.x / magnitude));
			this.y = (int)Math.Round((double)((float)this.y / magnitude));
			this.z = (int)Math.Round((double)((float)this.z / magnitude));
			return this;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"( ",
				this.x,
				", ",
				this.y,
				", ",
				this.z,
				")"
			});
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			Int3 @int = (Int3)o;
			return this.x == @int.x && this.y == @int.y && this.z == @int.z;
		}

		public override int GetHashCode()
		{
			return this.x * 73856093 ^ this.y * 19349663 ^ this.z * 83492791;
		}

		public static bool operator ==(Int3 lhs, Int3 rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}

		public static bool operator !=(Int3 lhs, Int3 rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
		}

		public static explicit operator Int3(Vector3 ob)
		{
			return new Int3((int)Math.Round((double)(ob.x * 1000f)), (int)Math.Round((double)(ob.y * 1000f)), (int)Math.Round((double)(ob.z * 1000f)));
		}

		public static explicit operator Vector3(Int3 ob)
		{
			return new Vector3((float)ob.x * 0.001f, (float)ob.y * 0.001f, (float)ob.z * 0.001f);
		}

		public static Int3 operator -(Int3 lhs, Int3 rhs)
		{
			lhs.x -= rhs.x;
			lhs.y -= rhs.y;
			lhs.z -= rhs.z;
			return lhs;
		}

		public static Int3 operator -(Int3 lhs)
		{
			lhs.x = -lhs.x;
			lhs.y = -lhs.y;
			lhs.z = -lhs.z;
			return lhs;
		}

		public static Int3 operator +(Int3 lhs, Int3 rhs)
		{
			lhs.x += rhs.x;
			lhs.y += rhs.y;
			lhs.z += rhs.z;
			return lhs;
		}

		public static Int3 operator *(Int3 lhs, int rhs)
		{
			lhs.x *= rhs;
			lhs.y *= rhs;
			lhs.z *= rhs;
			return lhs;
		}

		public static Int3 operator *(Int3 lhs, float rhs)
		{
			lhs.x = (int)Math.Round((double)((float)lhs.x * rhs));
			lhs.y = (int)Math.Round((double)((float)lhs.y * rhs));
			lhs.z = (int)Math.Round((double)((float)lhs.z * rhs));
			return lhs;
		}

		public static Int3 operator *(Int3 lhs, double rhs)
		{
			lhs.x = (int)Math.Round((double)lhs.x * rhs);
			lhs.y = (int)Math.Round((double)lhs.y * rhs);
			lhs.z = (int)Math.Round((double)lhs.z * rhs);
			return lhs;
		}

		public static Int3 operator *(Int3 lhs, Vector3 rhs)
		{
			lhs.x = (int)Math.Round((double)((float)lhs.x * rhs.x));
			lhs.y = (int)Math.Round((double)((float)lhs.y * rhs.y));
			lhs.z = (int)Math.Round((double)((float)lhs.z * rhs.z));
			return lhs;
		}

		public static Int3 operator /(Int3 lhs, float rhs)
		{
			lhs.x = (int)Math.Round((double)((float)lhs.x / rhs));
			lhs.y = (int)Math.Round((double)((float)lhs.y / rhs));
			lhs.z = (int)Math.Round((double)((float)lhs.z / rhs));
			return lhs;
		}

		public static implicit operator string(Int3 ob)
		{
			return ob.ToString();
		}
	}
}
