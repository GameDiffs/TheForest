using System;

namespace Steamworks
{
	public struct ScreenshotHandle : IEquatable<ScreenshotHandle>, IComparable<ScreenshotHandle>
	{
		public static readonly ScreenshotHandle Invalid = new ScreenshotHandle(0u);

		public uint m_ScreenshotHandle;

		public ScreenshotHandle(uint value)
		{
			this.m_ScreenshotHandle = value;
		}

		public override string ToString()
		{
			return this.m_ScreenshotHandle.ToString();
		}

		public override bool Equals(object other)
		{
			return other is ScreenshotHandle && this == (ScreenshotHandle)other;
		}

		public override int GetHashCode()
		{
			return this.m_ScreenshotHandle.GetHashCode();
		}

		public bool Equals(ScreenshotHandle other)
		{
			return this.m_ScreenshotHandle == other.m_ScreenshotHandle;
		}

		public int CompareTo(ScreenshotHandle other)
		{
			return this.m_ScreenshotHandle.CompareTo(other.m_ScreenshotHandle);
		}

		public static bool operator ==(ScreenshotHandle x, ScreenshotHandle y)
		{
			return x.m_ScreenshotHandle == y.m_ScreenshotHandle;
		}

		public static bool operator !=(ScreenshotHandle x, ScreenshotHandle y)
		{
			return !(x == y);
		}

		public static explicit operator ScreenshotHandle(uint value)
		{
			return new ScreenshotHandle(value);
		}

		public static explicit operator uint(ScreenshotHandle that)
		{
			return that.m_ScreenshotHandle;
		}
	}
}
