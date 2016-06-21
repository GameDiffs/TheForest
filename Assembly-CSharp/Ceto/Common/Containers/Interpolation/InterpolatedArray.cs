using System;

namespace Ceto.Common.Containers.Interpolation
{
	public abstract class InterpolatedArray
	{
		private bool m_wrap;

		public bool Wrap
		{
			get
			{
				return this.m_wrap;
			}
			set
			{
				this.m_wrap = value;
			}
		}

		public bool HalfPixelOffset
		{
			get;
			set;
		}

		public InterpolatedArray(bool wrap)
		{
			this.m_wrap = wrap;
			this.HalfPixelOffset = true;
		}

		public void Index(ref int x, int sx)
		{
			if (this.m_wrap)
			{
				if (x >= sx || x <= -sx)
				{
					x %= sx;
				}
				if (x < 0)
				{
					x = sx - -x;
				}
			}
			else if (x < 0)
			{
				x = 0;
			}
			else if (x >= sx)
			{
				x = sx - 1;
			}
		}

		public void Index(double x, int sx, out int ix0, out int ix1)
		{
			ix0 = (int)x;
			ix1 = (int)x + Math.Sign(x);
			if (this.m_wrap)
			{
				if (ix0 >= sx || ix0 <= -sx)
				{
					ix0 %= sx;
				}
				if (ix0 < 0)
				{
					ix0 = sx - -ix0;
				}
				if (ix1 >= sx || ix1 <= -sx)
				{
					ix1 %= sx;
				}
				if (ix1 < 0)
				{
					ix1 = sx - -ix1;
				}
			}
			else
			{
				if (ix0 < 0)
				{
					ix0 = 0;
				}
				else if (ix0 >= sx)
				{
					ix0 = sx - 1;
				}
				if (ix1 < 0)
				{
					ix1 = 0;
				}
				else if (ix1 >= sx)
				{
					ix1 = sx - 1;
				}
			}
		}
	}
}
