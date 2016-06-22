using System;

namespace Steamworks
{
	public struct CGameID : IEquatable<CGameID>, IComparable<CGameID>
	{
		public enum EGameIDType
		{
			k_EGameIDTypeApp,
			k_EGameIDTypeGameMod,
			k_EGameIDTypeShortcut,
			k_EGameIDTypeP2P
		}

		public ulong m_GameID;

		public CGameID(ulong GameID)
		{
			this.m_GameID = GameID;
		}

		public CGameID(AppId_t nAppID)
		{
			this.m_GameID = 0uL;
			this.SetAppID(nAppID);
		}

		public CGameID(AppId_t nAppID, uint nModID)
		{
			this.m_GameID = 0uL;
			this.SetAppID(nAppID);
			this.SetType(CGameID.EGameIDType.k_EGameIDTypeGameMod);
			this.SetModID(nModID);
		}

		public bool IsSteamApp()
		{
			return this.Type() == CGameID.EGameIDType.k_EGameIDTypeApp;
		}

		public bool IsMod()
		{
			return this.Type() == CGameID.EGameIDType.k_EGameIDTypeGameMod;
		}

		public bool IsShortcut()
		{
			return this.Type() == CGameID.EGameIDType.k_EGameIDTypeShortcut;
		}

		public bool IsP2PFile()
		{
			return this.Type() == CGameID.EGameIDType.k_EGameIDTypeP2P;
		}

		public AppId_t AppID()
		{
			return new AppId_t((uint)(this.m_GameID & 16777215uL));
		}

		public CGameID.EGameIDType Type()
		{
			return (CGameID.EGameIDType)(this.m_GameID >> 24 & 255uL);
		}

		public uint ModID()
		{
			return (uint)(this.m_GameID >> 32 & (ulong)-1);
		}

		public bool IsValid()
		{
			switch (this.Type())
			{
			case CGameID.EGameIDType.k_EGameIDTypeApp:
				return this.AppID() != AppId_t.Invalid;
			case CGameID.EGameIDType.k_EGameIDTypeGameMod:
				return this.AppID() != AppId_t.Invalid && (this.ModID() & 2147483648u) != 0u;
			case CGameID.EGameIDType.k_EGameIDTypeShortcut:
				return (this.ModID() & 2147483648u) != 0u;
			case CGameID.EGameIDType.k_EGameIDTypeP2P:
				return this.AppID() == AppId_t.Invalid && (this.ModID() & 2147483648u) != 0u;
			default:
				return false;
			}
		}

		public void Reset()
		{
			this.m_GameID = 0uL;
		}

		public void Set(ulong GameID)
		{
			this.m_GameID = GameID;
		}

		private void SetAppID(AppId_t other)
		{
			this.m_GameID = ((this.m_GameID & 18446744073692774400uL) | ((ulong)((uint)other) & 16777215uL));
		}

		private void SetType(CGameID.EGameIDType other)
		{
			this.m_GameID = ((this.m_GameID & 18446744069431361535uL) | (ulong)((ulong)((long)other & 255L) << 24));
		}

		private void SetModID(uint other)
		{
			this.m_GameID = ((this.m_GameID & (ulong)-1) | ((ulong)other & (ulong)-1) << 32);
		}

		public override string ToString()
		{
			return this.m_GameID.ToString();
		}

		public override bool Equals(object other)
		{
			return other is CGameID && this == (CGameID)other;
		}

		public override int GetHashCode()
		{
			return this.m_GameID.GetHashCode();
		}

		public bool Equals(CGameID other)
		{
			return this.m_GameID == other.m_GameID;
		}

		public int CompareTo(CGameID other)
		{
			return this.m_GameID.CompareTo(other.m_GameID);
		}

		public static bool operator ==(CGameID x, CGameID y)
		{
			return x.m_GameID == y.m_GameID;
		}

		public static bool operator !=(CGameID x, CGameID y)
		{
			return !(x == y);
		}

		public static explicit operator CGameID(ulong value)
		{
			return new CGameID(value);
		}

		public static explicit operator ulong(CGameID that)
		{
			return that.m_GameID;
		}
	}
}
