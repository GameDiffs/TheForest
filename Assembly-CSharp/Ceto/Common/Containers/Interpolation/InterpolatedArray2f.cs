using System;

namespace Ceto.Common.Containers.Interpolation
{
	public class InterpolatedArray2f : InterpolatedArray, IInterpolatedArray2
	{
		private float[] m_data;

		private int m_sx;

		private int m_sy;

		private int m_c;

		public float[] Data
		{
			get
			{
				return this.m_data;
			}
		}

		public int SX
		{
			get
			{
				return this.m_sx;
			}
		}

		public int SY
		{
			get
			{
				return this.m_sy;
			}
		}

		public int Channels
		{
			get
			{
				return this.m_c;
			}
		}

		public float this[int x, int y, int c]
		{
			get
			{
				return this.m_data[(x + y * this.m_sx) * this.m_c + c];
			}
			set
			{
				this.m_data[(x + y * this.m_sx) * this.m_c + c] = value;
			}
		}

		public InterpolatedArray2f(int sx, int sy, int c, bool wrap) : base(wrap)
		{
			this.m_sx = sx;
			this.m_sy = sy;
			this.m_c = c;
			this.m_data = new float[this.m_sx * this.m_sy * this.m_c];
		}

		public InterpolatedArray2f(float[] data, int sx, int sy, int c, bool wrap) : base(wrap)
		{
			this.m_sx = sx;
			this.m_sy = sy;
			this.m_c = c;
			this.m_data = new float[this.m_sx * this.m_sy * this.m_c];
			this.Copy(data);
		}

		public InterpolatedArray2f(float[,,] data, bool wrap) : base(wrap)
		{
			this.m_sx = data.GetLength(0);
			this.m_sy = data.GetLength(1);
			this.m_c = data.GetLength(2);
			this.m_data = new float[this.m_sx * this.m_sy * this.m_c];
			this.Copy(data);
		}

		public void Clear()
		{
			Array.Clear(this.m_data, 0, this.m_data.Length);
		}

		public void Copy(Array data)
		{
			Array.Copy(data, this.m_data, this.m_data.Length);
		}

		public float Get(int x, int y, int c)
		{
			return this.m_data[(x + y * this.m_sx) * this.m_c + c];
		}

		public void Set(int x, int y, int c, float v)
		{
			this.m_data[(x + y * this.m_sx) * this.m_c + c] = v;
		}

		public void Set(int x, int y, float[] v)
		{
			for (int i = 0; i < this.m_c; i++)
			{
				this.m_data[(x + y * this.m_sx) * this.m_c + i] = v[i];
			}
		}

		public void Get(int x, int y, float[] v)
		{
			for (int i = 0; i < this.m_c; i++)
			{
				v[i] = this.m_data[(x + y * this.m_sx) * this.m_c + i];
			}
		}

		public void Get(float x, float y, float[] v)
		{
			if (base.HalfPixelOffset)
			{
				x *= (float)this.m_sx;
				y *= (float)this.m_sy;
				x -= 0.5f;
				y -= 0.5f;
			}
			else
			{
				x *= (float)(this.m_sx - 1);
				y *= (float)(this.m_sy - 1);
			}
			float num = Math.Abs(x - (float)((int)x));
			int num2;
			int num3;
			base.Index((double)x, this.m_sx, out num2, out num3);
			float num4 = Math.Abs(y - (float)((int)y));
			int num5;
			int num6;
			base.Index((double)y, this.m_sy, out num5, out num6);
			for (int i = 0; i < this.m_c; i++)
			{
				float num7 = this.m_data[(num2 + num5 * this.m_sx) * this.m_c + i] * (1f - num) + this.m_data[(num3 + num5 * this.m_sx) * this.m_c + i] * num;
				float num8 = this.m_data[(num2 + num6 * this.m_sx) * this.m_c + i] * (1f - num) + this.m_data[(num3 + num6 * this.m_sx) * this.m_c + i] * num;
				v[i] = num7 * (1f - num4) + num8 * num4;
			}
		}

		public float Get(float x, float y, int c)
		{
			if (base.HalfPixelOffset)
			{
				x *= (float)this.m_sx;
				y *= (float)this.m_sy;
				x -= 0.5f;
				y -= 0.5f;
			}
			else
			{
				x *= (float)(this.m_sx - 1);
				y *= (float)(this.m_sy - 1);
			}
			float num = Math.Abs(x - (float)((int)x));
			int num2;
			int num3;
			base.Index((double)x, this.m_sx, out num2, out num3);
			float num4 = Math.Abs(y - (float)((int)y));
			int num5;
			int num6;
			base.Index((double)y, this.m_sy, out num5, out num6);
			float num7 = this.m_data[(num2 + num5 * this.m_sx) * this.m_c + c] * (1f - num) + this.m_data[(num3 + num5 * this.m_sx) * this.m_c + c] * num;
			float num8 = this.m_data[(num2 + num6 * this.m_sx) * this.m_c + c] * (1f - num) + this.m_data[(num3 + num6 * this.m_sx) * this.m_c + c] * num;
			return num7 * (1f - num4) + num8 * num4;
		}

		virtual bool get_HalfPixelOffset()
		{
			return base.HalfPixelOffset;
		}

		virtual void set_HalfPixelOffset(bool value)
		{
			base.HalfPixelOffset = value;
		}

		virtual bool get_Wrap()
		{
			return base.Wrap;
		}

		virtual void set_Wrap(bool value)
		{
			base.Wrap = value;
		}
	}
}
