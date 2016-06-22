using System;

namespace Steamworks
{
	public struct HHTMLBrowser : IEquatable<HHTMLBrowser>, IComparable<HHTMLBrowser>
	{
		public static readonly HHTMLBrowser Invalid = new HHTMLBrowser(0u);

		public uint m_HHTMLBrowser;

		public HHTMLBrowser(uint value)
		{
			this.m_HHTMLBrowser = value;
		}

		public override string ToString()
		{
			return this.m_HHTMLBrowser.ToString();
		}

		public override bool Equals(object other)
		{
			return other is HHTMLBrowser && this == (HHTMLBrowser)other;
		}

		public override int GetHashCode()
		{
			return this.m_HHTMLBrowser.GetHashCode();
		}

		public bool Equals(HHTMLBrowser other)
		{
			return this.m_HHTMLBrowser == other.m_HHTMLBrowser;
		}

		public int CompareTo(HHTMLBrowser other)
		{
			return this.m_HHTMLBrowser.CompareTo(other.m_HHTMLBrowser);
		}

		public static bool operator ==(HHTMLBrowser x, HHTMLBrowser y)
		{
			return x.m_HHTMLBrowser == y.m_HHTMLBrowser;
		}

		public static bool operator !=(HHTMLBrowser x, HHTMLBrowser y)
		{
			return !(x == y);
		}

		public static explicit operator HHTMLBrowser(uint value)
		{
			return new HHTMLBrowser(value);
		}

		public static explicit operator uint(HHTMLBrowser that)
		{
			return that.m_HHTMLBrowser;
		}
	}
}
