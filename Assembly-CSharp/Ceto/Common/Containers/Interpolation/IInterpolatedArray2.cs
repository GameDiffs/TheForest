using System;

namespace Ceto.Common.Containers.Interpolation
{
	public interface IInterpolatedArray2
	{
		int SX
		{
			get;
		}

		int SY
		{
			get;
		}

		int Channels
		{
			get;
		}

		bool HalfPixelOffset
		{
			get;
			set;
		}

		bool Wrap
		{
			get;
			set;
		}

		float this[int x, int y, int c]
		{
			get;
			set;
		}

		void Clear();

		void Copy(Array data);

		void Set(int x, int y, int c, float v);

		void Set(int x, int y, float[] v);

		float Get(int x, int y, int c);

		void Get(int x, int y, float[] v);

		void Get(float x, float y, float[] v);

		float Get(float x, float y, int c);
	}
}
