using System;
using System.IO;
using System.Text;
using TheForest.Utils;
using UnityEngine;

public static class PlayerPrefsFile
{
	private static string GetPath(string name)
	{
		return string.Format("{0}/PlayerPrefsFile_{1}.dat", Application.persistentDataPath, name);
	}

	public static bool KeyExist(string name)
	{
		return CoopSteamCloud.CloudFileExist(name + ((!BoltNetwork.isRunning) ? string.Empty : "MP")) || !string.IsNullOrEmpty(PlayerPrefsFile.GetString(name, string.Empty, true));
	}

	public static string GetString(string name, string defaultValue = "", bool useSlots = true)
	{
		string path = ((!useSlots) ? SaveSlotUtils.GetUserPath() : SaveSlotUtils.GetLocalSlotPath()) + name;
		if (!File.Exists(path))
		{
			return defaultValue;
		}
		return File.ReadAllText(path);
	}

	public static byte[] GetBytes(string name, byte[] defaultValue, bool useSlots = true)
	{
		string path = ((!useSlots) ? SaveSlotUtils.GetUserPath() : SaveSlotUtils.GetLocalSlotPath()) + name;
		if (!File.Exists(path))
		{
			return defaultValue;
		}
		return File.ReadAllBytes(path);
	}

	public static void SetString(string name, string data, bool useSlots = true)
	{
		string text = (!useSlots) ? SaveSlotUtils.GetUserPath() : SaveSlotUtils.GetLocalSlotPath();
		string text2 = text + name;
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (File.Exists(text2))
		{
			string text3 = text2 + "prev";
			if (File.Exists(text3))
			{
				File.Delete(text3);
			}
			File.Move(text2, text3);
		}
		File.WriteAllText(text2, data);
		if (CoopSteamCloud.ShouldUseCloud())
		{
			CoopSteamCloud.CloudSave(((!useSlots) ? string.Empty : SaveSlotUtils.GetCloudSlotPath()) + name, Encoding.ASCII.GetBytes(data));
		}
	}

	public static void SetBytes(string name, byte[] data, bool useSlots = true)
	{
		string text = (!useSlots) ? SaveSlotUtils.GetUserPath() : SaveSlotUtils.GetLocalSlotPath();
		string text2 = text + name;
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (File.Exists(text2))
		{
			string text3 = text2 + "prev";
			if (File.Exists(text3))
			{
				File.Delete(text3);
			}
			File.Move(text2, text3);
		}
		File.WriteAllBytes(text2, data);
		if (CoopSteamCloud.ShouldUseCloud())
		{
			CoopSteamCloud.CloudSave(((!useSlots) ? string.Empty : SaveSlotUtils.GetCloudSlotPath()) + name, data);
		}
	}

	public static void Save()
	{
	}

	public static void DeleteKey(string name, bool useSlots = true)
	{
		string path = ((!useSlots) ? SaveSlotUtils.GetUserPath() : SaveSlotUtils.GetLocalSlotPath()) + name;
		string filename = ((!useSlots) ? string.Empty : SaveSlotUtils.GetCloudSlotPath()) + name;
		bool flag = File.Exists(path);
		bool flag2 = CoopSteamCloud.CloudFileExist(filename);
		if (flag)
		{
			File.Delete(path);
		}
		if (flag2 && CoopSteamCloud.ShouldUseCloud())
		{
			CoopSteamCloud.CloudDelete(filename);
		}
	}

	public static void ConvertToSlotSystem(string name, TitleScreen.GameSetup.PlayerModes mode)
	{
		string text = PlayerPrefsFile.GetPath(name);
		if (mode == TitleScreen.GameSetup.PlayerModes.Multiplayer)
		{
			text += "MP";
		}
		if (File.Exists(text))
		{
			string localSlotPath = SaveSlotUtils.GetLocalSlotPath(mode, TitleScreen.GameSetup.Slots.Slot1);
			if (!Directory.Exists(localSlotPath))
			{
				Directory.CreateDirectory(localSlotPath);
			}
			File.Move(text, localSlotPath + name);
		}
		if (CoopSteamCloud.ShouldUseCloud() && CoopSteamCloud.CloudFileExist(name))
		{
			Debug.Log("Converting cloud file: '" + name + "' to slot system");
			byte[] buffer = CoopSteamCloud.CloudLoad(name);
			CoopSteamCloud.CloudDelete(name);
			if (CoopSteamCloud.CloudSave(SaveSlotUtils.GetCloudSlotPath() + name, buffer))
			{
				Debug.Log(name + " converted successfully");
			}
			else
			{
				Debug.Log(name + " update failed");
			}
		}
	}

	public static void SyncWithCloud(string name, TitleScreen.GameSetup.PlayerModes mode, TitleScreen.GameSetup.Slots slot)
	{
		string localSlotPath = SaveSlotUtils.GetLocalSlotPath(mode, slot);
		string path = localSlotPath + name;
		string filename = SaveSlotUtils.GetCloudSlotPath(mode, slot) + name;
		bool flag = File.Exists(path);
		bool flag2 = CoopSteamCloud.CloudFileExist(filename);
		long num = 0L;
		long num2 = 0L;
		if (flag2 && flag)
		{
			num = CoopSteamCloud.CloudTimestamp(filename);
			num2 = File.GetCreationTime(path).ToUnixTimestamp();
			flag2 = (num > num2);
			flag = (num < num2);
		}
		if (flag2)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Syncing ",
				mode,
				"/",
				slot,
				"/",
				name,
				" from cloud(",
				num,
				") to local(",
				num2,
				")"
			}));
			if (!Directory.Exists(localSlotPath))
			{
				Directory.CreateDirectory(localSlotPath);
			}
			File.WriteAllBytes(path, CoopSteamCloud.CloudLoad(filename));
			File.SetCreationTime(path, DateEx.UnixTimeStampToDateTime(num));
			Debug.Log(string.Concat(new object[]
			{
				"Local file (",
				name,
				") Creation Time: ",
				File.GetCreationTime(path).ToUnixTimestamp(),
				" - ",
				num,
				" = ",
				File.GetCreationTime(path).ToUnixTimestamp() - num,
				"?"
			}));
		}
		else if (flag)
		{
			byte[] buffer = File.ReadAllBytes(path);
			bool flag3 = CoopSteamCloud.CloudSave(filename, buffer);
			Debug.Log(string.Concat(new object[]
			{
				"Cloud file (",
				name,
				") Creation time: ",
				CoopSteamCloud.CloudTimestamp(filename),
				" - ",
				File.GetCreationTime(path).ToUnixTimestamp(),
				" = ",
				CoopSteamCloud.CloudTimestamp(filename) - File.GetCreationTime(path).ToUnixTimestamp(),
				"?"
			}));
		}
	}
}
