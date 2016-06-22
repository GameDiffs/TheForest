using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CoopLobby
{
	public static CoopLobby Instance
	{
		get;
		private set;
	}

	public static bool IsInLobby
	{
		get
		{
			return CoopLobby.Instance != null && CoopLobby.Instance.Info.LobbyId.IsValid();
		}
	}

	public CoopLobbyInfo Info
	{
		get;
		private set;
	}

	public int MemberCount
	{
		get
		{
			return SteamMatchmaking.GetNumLobbyMembers(this.Info.LobbyId);
		}
	}

	public IEnumerable<CSteamID> AllMembers
	{
		get
		{
			CoopLobby.<>c__Iterator24 <>c__Iterator = new CoopLobby.<>c__Iterator24();
			<>c__Iterator.<>f__this = this;
			CoopLobby.<>c__Iterator24 expr_0E = <>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}

	private CoopLobby()
	{
	}

	[DebuggerHidden]
	public IEnumerable<string> GetMembersData(string key)
	{
		CoopLobby.<GetMembersData>c__Iterator25 <GetMembersData>c__Iterator = new CoopLobby.<GetMembersData>c__Iterator25();
		<GetMembersData>c__Iterator.key = key;
		<GetMembersData>c__Iterator.<$>key = key;
		<GetMembersData>c__Iterator.<>f__this = this;
		CoopLobby.<GetMembersData>c__Iterator25 expr_1C = <GetMembersData>c__Iterator;
		expr_1C.$PC = -2;
		return expr_1C;
	}

	public void Destroy()
	{
		this.Info.Destroyed = true;
		SteamMatchmaking.SetLobbyJoinable(this.Info.LobbyId, false);
		SteamMatchmaking.SetLobbyData(this.Info.LobbyId, "destroyed", "YES");
	}

	public void SetName(string name)
	{
		this.Info.Name = name;
		SteamMatchmaking.SetLobbyData(this.Info.LobbyId, "name", name);
	}

	public void SetGuid(string guid)
	{
		this.Info.Guid = guid;
		SteamMatchmaking.SetLobbyData(this.Info.LobbyId, "guid", guid);
	}

	public void SetCurrentMembers(int value)
	{
		if (this.Info.CurrentMembers != value)
		{
			this.Info.CurrentMembers = value;
			SteamMatchmaking.SetLobbyData(this.Info.LobbyId, "currentmembers", this.Info.CurrentMembers.ToString());
		}
	}

	public void SetServer(CSteamID server)
	{
		this.Info.ServerId = server;
		SteamMatchmaking.SetLobbyGameServer(this.Info.LobbyId, 0u, 0, server);
	}

	public void SetMemberLimit(int limit)
	{
		this.Info.MemberLimit = limit;
		SteamMatchmaking.SetLobbyMemberLimit(this.Info.LobbyId, limit);
	}

	public void SetJoinable(bool joinable)
	{
		this.Info.Joinable = joinable;
		SteamMatchmaking.SetLobbyJoinable(this.Info.LobbyId, joinable);
	}

	public static void LeaveActive()
	{
		UnityEngine.Debug.Log("CoopLobby.LeaveActive instance=" + CoopLobby.Instance);
		if (CoopLobby.Instance != null)
		{
			try
			{
				if (CoopLobby.Instance.Info.LobbyId.IsValid())
				{
					SteamMatchmaking.LeaveLobby(CoopLobby.Instance.Info.LobbyId);
				}
			}
			finally
			{
				CoopLobby.Instance = null;
			}
		}
	}

	public static void SetActive(CoopLobbyInfo info)
	{
		if (CoopLobby.IsInLobby && CoopLobby.Instance.Info.LobbyId == info.LobbyId)
		{
			return;
		}
		CoopLobby.LeaveActive();
		CoopLobby.Instance = new CoopLobby();
		CoopLobby.Instance.Info = info;
		if (info.IsOwner)
		{
			CoopLobby.Instance.SetName(CoopLobby.Instance.Info.Name);
			CoopLobby.Instance.SetMemberLimit(CoopLobby.Instance.Info.MemberLimit);
			CoopLobby.Instance.SetJoinable(true);
		}
	}
}
