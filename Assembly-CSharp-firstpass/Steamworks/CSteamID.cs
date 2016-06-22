using System;

namespace Steamworks
{
	public struct CSteamID : IEquatable<CSteamID>, IComparable<CSteamID>
	{
		public static readonly CSteamID Nil = default(CSteamID);

		public static readonly CSteamID OutofDateGS = new CSteamID(new AccountID_t(0u), 0u, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);

		public static readonly CSteamID LanModeGS = new CSteamID(new AccountID_t(0u), 0u, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeInvalid);

		public static readonly CSteamID NotInitYetGS = new CSteamID(new AccountID_t(1u), 0u, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);

		public static readonly CSteamID NonSteamGS = new CSteamID(new AccountID_t(2u), 0u, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);

		public ulong m_SteamID;

		public CSteamID(AccountID_t unAccountID, EUniverse eUniverse, EAccountType eAccountType)
		{
			this.m_SteamID = 0uL;
			this.Set(unAccountID, eUniverse, eAccountType);
		}

		public CSteamID(AccountID_t unAccountID, uint unAccountInstance, EUniverse eUniverse, EAccountType eAccountType)
		{
			this.m_SteamID = 0uL;
			this.InstancedSet(unAccountID, unAccountInstance, eUniverse, eAccountType);
		}

		public CSteamID(ulong ulSteamID)
		{
			this.m_SteamID = ulSteamID;
		}

		public void Set(AccountID_t unAccountID, EUniverse eUniverse, EAccountType eAccountType)
		{
			this.SetAccountID(unAccountID);
			this.SetEUniverse(eUniverse);
			this.SetEAccountType(eAccountType);
			if (eAccountType == EAccountType.k_EAccountTypeClan || eAccountType == EAccountType.k_EAccountTypeGameServer)
			{
				this.SetAccountInstance(0u);
			}
			else
			{
				this.SetAccountInstance(1u);
			}
		}

		public void InstancedSet(AccountID_t unAccountID, uint unInstance, EUniverse eUniverse, EAccountType eAccountType)
		{
			this.SetAccountID(unAccountID);
			this.SetEUniverse(eUniverse);
			this.SetEAccountType(eAccountType);
			this.SetAccountInstance(unInstance);
		}

		public void Clear()
		{
			this.m_SteamID = 0uL;
		}

		public void CreateBlankAnonLogon(EUniverse eUniverse)
		{
			this.SetAccountID(new AccountID_t(0u));
			this.SetEUniverse(eUniverse);
			this.SetEAccountType(EAccountType.k_EAccountTypeAnonGameServer);
			this.SetAccountInstance(0u);
		}

		public void CreateBlankAnonUserLogon(EUniverse eUniverse)
		{
			this.SetAccountID(new AccountID_t(0u));
			this.SetEUniverse(eUniverse);
			this.SetEAccountType(EAccountType.k_EAccountTypeAnonUser);
			this.SetAccountInstance(0u);
		}

		public bool BBlankAnonAccount()
		{
			return this.GetAccountID() == new AccountID_t(0u) && this.BAnonAccount() && this.GetUnAccountInstance() == 0u;
		}

		public bool BGameServerAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeGameServer || this.GetEAccountType() == EAccountType.k_EAccountTypeAnonGameServer;
		}

		public bool BPersistentGameServerAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeGameServer;
		}

		public bool BAnonGameServerAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeAnonGameServer;
		}

		public bool BContentServerAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeContentServer;
		}

		public bool BClanAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeClan;
		}

		public bool BChatAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeChat;
		}

		public bool IsLobby()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeChat && (this.GetUnAccountInstance() & 262144u) != 0u;
		}

		public bool BIndividualAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeIndividual || this.GetEAccountType() == EAccountType.k_EAccountTypeConsoleUser;
		}

		public bool BAnonAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeAnonUser || this.GetEAccountType() == EAccountType.k_EAccountTypeAnonGameServer;
		}

		public bool BAnonUserAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeAnonUser;
		}

		public bool BConsoleUserAccount()
		{
			return this.GetEAccountType() == EAccountType.k_EAccountTypeConsoleUser;
		}

		public void SetAccountID(AccountID_t other)
		{
			this.m_SteamID = ((this.m_SteamID & 18446744069414584320uL) | ((ulong)((uint)other) & (ulong)-1));
		}

		public void SetAccountInstance(uint other)
		{
			this.m_SteamID = ((this.m_SteamID & 18442240478377148415uL) | ((ulong)other & 1048575uL) << 32);
		}

		public void SetEAccountType(EAccountType other)
		{
			this.m_SteamID = ((this.m_SteamID & 18379190079298994175uL) | (ulong)((ulong)((long)other & 15L) << 52));
		}

		public void SetEUniverse(EUniverse other)
		{
			this.m_SteamID = ((this.m_SteamID & 72057594037927935uL) | (ulong)((ulong)((long)other & 255L) << 56));
		}

		public void ClearIndividualInstance()
		{
			if (this.BIndividualAccount())
			{
				this.SetAccountInstance(0u);
			}
		}

		public bool HasNoIndividualInstance()
		{
			return this.BIndividualAccount() && this.GetUnAccountInstance() == 0u;
		}

		public AccountID_t GetAccountID()
		{
			return new AccountID_t((uint)(this.m_SteamID & (ulong)-1));
		}

		public uint GetUnAccountInstance()
		{
			return (uint)(this.m_SteamID >> 32 & 1048575uL);
		}

		public EAccountType GetEAccountType()
		{
			return (EAccountType)(this.m_SteamID >> 52 & 15uL);
		}

		public EUniverse GetEUniverse()
		{
			return (EUniverse)(this.m_SteamID >> 56 & 255uL);
		}

		public bool IsValid()
		{
			return this.GetEAccountType() > EAccountType.k_EAccountTypeInvalid && this.GetEAccountType() < EAccountType.k_EAccountTypeMax && this.GetEUniverse() > EUniverse.k_EUniverseInvalid && this.GetEUniverse() < EUniverse.k_EUniverseMax && (this.GetEAccountType() != EAccountType.k_EAccountTypeIndividual || (!(this.GetAccountID() == new AccountID_t(0u)) && this.GetUnAccountInstance() <= 4u)) && (this.GetEAccountType() != EAccountType.k_EAccountTypeClan || (!(this.GetAccountID() == new AccountID_t(0u)) && this.GetUnAccountInstance() == 0u)) && (this.GetEAccountType() != EAccountType.k_EAccountTypeGameServer || !(this.GetAccountID() == new AccountID_t(0u)));
		}

		public override string ToString()
		{
			return this.m_SteamID.ToString();
		}

		public override bool Equals(object other)
		{
			return other is CSteamID && this == (CSteamID)other;
		}

		public override int GetHashCode()
		{
			return this.m_SteamID.GetHashCode();
		}

		public bool Equals(CSteamID other)
		{
			return this.m_SteamID == other.m_SteamID;
		}

		public int CompareTo(CSteamID other)
		{
			return this.m_SteamID.CompareTo(other.m_SteamID);
		}

		public static bool operator ==(CSteamID x, CSteamID y)
		{
			return x.m_SteamID == y.m_SteamID;
		}

		public static bool operator !=(CSteamID x, CSteamID y)
		{
			return !(x == y);
		}

		public static explicit operator CSteamID(ulong value)
		{
			return new CSteamID(value);
		}

		public static explicit operator ulong(CSteamID that)
		{
			return that.m_SteamID;
		}
	}
}
