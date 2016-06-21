using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheForest.Utils
{
	public static class SaveSlotUtils
	{
		public static string GetUserPath()
		{
			string text = "0";
			try
			{
				text = SteamUser.GetSteamID().ToString();
			}
			catch (Exception var_1_19)
			{
			}
			finally
			{
				if (string.IsNullOrEmpty(text))
				{
					text = "0";
				}
			}
			return Application.persistentDataPath + "/" + text + "/";
		}

		public static string GetLocalSlotPath()
		{
			return SaveSlotUtils.GetLocalSlotPath(TitleScreen.StartGameSetup.Slot);
		}

		public static string GetLocalSlotPath(TitleScreen.GameSetup.Slots slot)
		{
			return SaveSlotUtils.GetLocalSlotPath(TitleScreen.StartGameSetup.Mode, slot);
		}

		public static string GetLocalSlotPath(TitleScreen.GameSetup.PlayerModes mode, TitleScreen.GameSetup.Slots slot)
		{
			return string.Concat(new object[]
			{
				SaveSlotUtils.GetUserPath(),
				mode,
				"/",
				slot,
				"/"
			});
		}

		public static string GetMpClientLocalPath()
		{
			return SaveSlotUtils.GetUserPath() + TitleScreen.GameSetup.PlayerModes.Multiplayer + "/cs/";
		}

		public static string GetCloudSlotPath()
		{
			return SaveSlotUtils.GetCloudSlotPath(TitleScreen.StartGameSetup.Slot);
		}

		public static string GetCloudSlotPath(TitleScreen.GameSetup.Slots slot)
		{
			return SaveSlotUtils.GetCloudSlotPath(TitleScreen.StartGameSetup.Mode, slot);
		}

		public static string GetCloudSlotPath(TitleScreen.GameSetup.PlayerModes mode, TitleScreen.GameSetup.Slots slot)
		{
			return string.Concat(new object[]
			{
				mode,
				"_",
				slot,
				"_"
			});
		}

		private static void DeleteDirectory(string target_dir)
		{
			string[] files = Directory.GetFiles(target_dir);
			string[] directories = Directory.GetDirectories(target_dir);
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string path = array[i];
				File.SetAttributes(path, FileAttributes.Normal);
				File.Delete(path);
			}
			string[] array2 = directories;
			for (int j = 0; j < array2.Length; j++)
			{
				string target_dir2 = array2[j];
				SaveSlotUtils.DeleteDirectory(target_dir2);
			}
			Directory.Delete(target_dir, false);
		}

		public static void DeleteSlot(TitleScreen.GameSetup.PlayerModes mode, TitleScreen.GameSetup.Slots slot)
		{
			string localSlotPath = SaveSlotUtils.GetLocalSlotPath(mode, slot);
			if (Directory.Exists(localSlotPath))
			{
				SaveSlotUtils.DeleteDirectory(localSlotPath);
			}
			string[] array = CoopSteamCloud.ListFiles();
			string cloudSlotPath = SaveSlotUtils.GetCloudSlotPath(mode, slot);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].StartsWith(cloudSlotPath))
				{
					CoopSteamCloud.CloudDelete(array[i]);
				}
			}
		}

		public static void CreateThumbnail()
		{
			string localSlotPath = SaveSlotUtils.GetLocalSlotPath();
			if (!Directory.Exists(localSlotPath))
			{
				Directory.CreateDirectory(localSlotPath);
			}
			string path = localSlotPath + "thumb.png";
			RenderTexture targetTexture = LocalPlayer.MainCam.targetTexture;
			RenderTexture active = RenderTexture.active;
			int num = 250;
			int num2 = 175;
			RenderTexture renderTexture = new RenderTexture(num, num2, 24);
			LocalPlayer.MainCam.targetTexture = renderTexture;
			Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGB24, false);
			Vector3 b = LocalPlayer.MainCamTr.forward * 3f / 4f;
			LocalPlayer.MainCamTr.position += b;
			LocalPlayer.MainCam.Render();
			LocalPlayer.MainCamTr.position -= b;
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
			LocalPlayer.MainCam.targetTexture = targetTexture;
			RenderTexture.active = active;
			UnityEngine.Object.Destroy(renderTexture);
			byte[] array = texture2D.EncodeToPNG();
			File.WriteAllBytes(path, array);
			CoopSteamCloud.CloudSave(SaveSlotUtils.GetCloudSlotPath() + "thumb.png", array);
		}

		public static void SaveHostGameGUID()
		{
			PlayerPrefsFile.SetString("guid", CoopLobby.Instance.Info.Guid, true);
		}

		public static void LoadHostGameGUID()
		{
			string text = PlayerPrefsFile.GetString("guid", null, true);
			if (string.IsNullOrEmpty(text))
			{
				text = Guid.NewGuid().ToString();
			}
			CoopLobby.Instance.SetGuid(text);
		}

		public static HashSet<string> GetPreviouslyPlayedServers()
		{
			HashSet<string> hashSet = new HashSet<string>();
			string mpClientLocalPath = SaveSlotUtils.GetMpClientLocalPath();
			if (!Directory.Exists(mpClientLocalPath))
			{
				Directory.CreateDirectory(mpClientLocalPath);
			}
			string[] files = Directory.GetFiles(mpClientLocalPath);
			for (int i = 0; i < files.Length; i++)
			{
				string path = files[i];
				hashSet.Add(Path.GetFileName(path));
			}
			return hashSet;
		}
	}
}
