using System;

namespace Steamworks
{
	public struct SteamInventoryResult_t : IEquatable<SteamInventoryResult_t>, IComparable<SteamInventoryResult_t>
	{
		public static readonly SteamInventoryResult_t Invalid = new SteamInventoryResult_t(-1);

		public int m_SteamInventoryResult_t;

		public SteamInventoryResult_t(int value)
		{
			this.m_SteamInventoryResult_t = value;
		}

		public override string ToString()
		{
			return this.m_SteamInventoryResult_t.ToString();
		}

		public override bool Equals(object other)
		{
			return other is SteamInventoryResult_t && this == (SteamInventoryResult_t)other;
		}

		public override int GetHashCode()
		{
			return this.m_SteamInventoryResult_t.GetHashCode();
		}

		public bool Equals(SteamInventoryResult_t other)
		{
			return this.m_SteamInventoryResult_t == other.m_SteamInventoryResult_t;
		}

		public int CompareTo(SteamInventoryResult_t other)
		{
			return this.m_SteamInventoryResult_t.CompareTo(other.m_SteamInventoryResult_t);
		}

		public static bool operator ==(SteamInventoryResult_t x, SteamInventoryResult_t y)
		{
			return x.m_SteamInventoryResult_t == y.m_SteamInventoryResult_t;
		}

		public static bool operator !=(SteamInventoryResult_t x, SteamInventoryResult_t y)
		{
			return !(x == y);
		}

		public static explicit operator SteamInventoryResult_t(int value)
		{
			return new SteamInventoryResult_t(value);
		}

		public static explicit operator int(SteamInventoryResult_t that)
		{
			return that.m_SteamInventoryResult_t;
		}
	}
}
