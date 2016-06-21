using Steamworks;
using System;

public static class CoopSteamCloud
{
	public static bool ShouldUseCloud()
	{
		return SteamManager.Initialized && SteamRemoteStorage.IsCloudEnabledForApp() && SteamRemoteStorage.IsCloudEnabledForAccount();
	}

	public static string[] ListFiles()
	{
		if (SteamManager.Initialized)
		{
			int fileCount = SteamRemoteStorage.GetFileCount();
			string[] array = new string[fileCount];
			for (int i = 0; i < fileCount; i++)
			{
				int num;
				array[i] = SteamRemoteStorage.GetFileNameAndSize(i, out num);
			}
			return array;
		}
		return new string[0];
	}

	public static bool CloudSave(string filename, byte[] buffer)
	{
		return SteamManager.Initialized && SteamRemoteStorage.FileWrite(filename, buffer, buffer.Length);
	}

	public static bool CloudFileExist(string filename)
	{
		return SteamManager.Initialized && SteamRemoteStorage.FileExists(filename);
	}

	public static long CloudTimestamp(string filename)
	{
		if (SteamManager.Initialized)
		{
			return SteamRemoteStorage.GetFileTimestamp(filename);
		}
		return 0L;
	}

	public static byte[] CloudLoad(string filename)
	{
		if (SteamManager.Initialized)
		{
			int fileSize = SteamRemoteStorage.GetFileSize(filename);
			byte[] array = new byte[fileSize];
			int num = SteamRemoteStorage.FileRead(filename, array, fileSize);
			if (num == fileSize)
			{
				return array;
			}
		}
		return new byte[0];
	}

	public static bool CloudDelete(string filename)
	{
		return SteamManager.Initialized && SteamRemoteStorage.FileDelete(filename);
	}
}
