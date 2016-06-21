using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TheForest.Utils;
using UdpKit;
using UniLinq;
using UnityEngine;

public class CoopKick
{
	[Serializable]
	public class KickedPlayer
	{
		public string Name;

		public ulong SteamId;

		public long BanEndTime;
	}

	public static string Client_KickMessage = string.Empty;

	public static bool Client_Banned;

	private List<CoopKick.KickedPlayer> kickedSteamIds = new List<CoopKick.KickedPlayer>();

	private static CoopKick instance;

	public static CoopKick Instance
	{
		get
		{
			if (CoopKick.instance == null)
			{
				CoopKick.instance = new CoopKick();
			}
			return CoopKick.instance;
		}
	}

	public List<CoopKick.KickedPlayer> KickedPlayers
	{
		get
		{
			return this.kickedSteamIds;
		}
	}

	public CoopKick()
	{
		try
		{
			byte[] bytes = PlayerPrefsFile.GetBytes("BanList", null, false);
			if (bytes != null)
			{
				IFormatter formatter = new BinaryFormatter();
				using (MemoryStream memoryStream = new MemoryStream(bytes))
				{
					this.kickedSteamIds = (List<CoopKick.KickedPlayer>)formatter.Deserialize(memoryStream);
				}
			}
			else
			{
				this.kickedSteamIds = new List<CoopKick.KickedPlayer>();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			this.kickedSteamIds = new List<CoopKick.KickedPlayer>();
		}
	}

	public static void SaveList()
	{
		try
		{
			IFormatter formatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				formatter.Serialize(memoryStream, CoopKick.Instance.kickedSteamIds);
				PlayerPrefsFile.SetBytes("BanList", memoryStream.ToArray(), false);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void KickPlayer(BoltEntity playerEntity, int duration, string message = "Host kicked you from the game")
	{
		if (BoltNetwork.isServer)
		{
			if (duration >= 0 && !CoopKick.IsBanned(playerEntity.source.RemoteEndPoint.SteamId))
			{
				CoopKick.Instance.kickedSteamIds.Add(new CoopKick.KickedPlayer
				{
					Name = playerEntity.GetState<IPlayerState>().name,
					SteamId = playerEntity.source.RemoteEndPoint.SteamId.Id,
					BanEndTime = (duration <= 0) ? 0L : (DateTime.UtcNow.ToUnixTimestamp() + (long)duration)
				});
				CoopKick.SaveList();
			}
			playerEntity.source.Disconnect(new CoopKickToken
			{
				KickMessage = message,
				Banned = duration == 0
			});
		}
	}

	public static void BanPlayer(BoltEntity playerEntity)
	{
		CoopKick.KickPlayer(playerEntity, 0, "Host banned you permanently from his games");
	}

	public static void UnBanPlayer(ulong steamId)
	{
		CoopKick.KickedPlayer kickedPlayer = CoopKick.Instance.kickedSteamIds.First((CoopKick.KickedPlayer k) => k.SteamId == steamId);
		if (kickedPlayer != null)
		{
			CoopKick.Instance.kickedSteamIds.Remove(kickedPlayer);
			CoopKick.SaveList();
		}
	}

	public static void ClearKickList()
	{
		CoopKick.Instance.kickedSteamIds.Clear();
	}

	public static bool IsBanned(UdpSteamID steamid)
	{
		CoopKick.CheckBanEndTimes();
		return CoopKick.Instance.kickedSteamIds.Any((CoopKick.KickedPlayer k) => k.SteamId == steamid.Id);
	}

	public static void CheckBanEndTimes()
	{
		bool flag = false;
		long num = DateTime.UtcNow.ToUnixTimestamp();
		for (int i = CoopKick.Instance.kickedSteamIds.Count - 1; i >= 0; i--)
		{
			long banEndTime = CoopKick.Instance.kickedSteamIds[i].BanEndTime;
			if (banEndTime > 0L && banEndTime < num)
			{
				CoopKick.Instance.kickedSteamIds.RemoveAt(i);
				flag = true;
			}
		}
		if (flag)
		{
			CoopKick.SaveList();
		}
	}
}
