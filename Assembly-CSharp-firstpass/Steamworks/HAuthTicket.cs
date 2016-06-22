using System;

namespace Steamworks
{
	public struct HAuthTicket : IEquatable<HAuthTicket>, IComparable<HAuthTicket>
	{
		public static readonly HAuthTicket Invalid = new HAuthTicket(0u);

		public uint m_HAuthTicket;

		public HAuthTicket(uint value)
		{
			this.m_HAuthTicket = value;
		}

		public override string ToString()
		{
			return this.m_HAuthTicket.ToString();
		}

		public override bool Equals(object other)
		{
			return other is HAuthTicket && this == (HAuthTicket)other;
		}

		public override int GetHashCode()
		{
			return this.m_HAuthTicket.GetHashCode();
		}

		public bool Equals(HAuthTicket other)
		{
			return this.m_HAuthTicket == other.m_HAuthTicket;
		}

		public int CompareTo(HAuthTicket other)
		{
			return this.m_HAuthTicket.CompareTo(other.m_HAuthTicket);
		}

		public static bool operator ==(HAuthTicket x, HAuthTicket y)
		{
			return x.m_HAuthTicket == y.m_HAuthTicket;
		}

		public static bool operator !=(HAuthTicket x, HAuthTicket y)
		{
			return !(x == y);
		}

		public static explicit operator HAuthTicket(uint value)
		{
			return new HAuthTicket(value);
		}

		public static explicit operator uint(HAuthTicket that)
		{
			return that.m_HAuthTicket;
		}
	}
}
