using Serialization;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Save
{
	public class PlayerSpawn : MonoBehaviour
	{
		public GameObject _playerEditor;

		public GameObject _playerPrefab;

		private GameObject _player;

		public static string MpCharacterSaveFileName = "MPCharacterSave";

		public static bool LoadSavedCharacter;

		private void Awake()
		{
			UnityEngine.Object.DestroyImmediate(this._playerEditor);
			if (!CoopPeerStarter.DedicatedHost)
			{
				if ((!BoltNetwork.isRunning || !BoltNetwork.isClient || !PlayerSpawn.LoadSavedCharacter || !PlayerSpawn.LoadMpCharacter()) && !LevelSerializer.IsDeserializing)
				{
					this._player = (GameObject)UnityEngine.Object.Instantiate(this._playerPrefab, base.transform.position, base.transform.rotation);
					this._player.name = "player";
					this._player.GetComponent<playerAiInfo>().enabled = false;
					base.Invoke("InitPlayer", 0.1f);
					base.StartCoroutine(Scene.PlaneCrash.InitPlaneCrashSequence());
				}
			}
			else
			{
				GameObject gameObject = new GameObject("DummyLocalPlayer");
				LocalPlayer localPlayer = gameObject.AddComponent<LocalPlayer>();
				LocalPlayer.Transform = gameObject.transform;
				LocalPlayer.GameObject = gameObject;
			}
			PlayerSpawn.LoadSavedCharacter = false;
		}

		public static string GetClientSaveFileName()
		{
			return (CoopLobby.Instance == null) ? string.Empty : CoopLobby.Instance.Info.Guid;
		}

		public static void SaveMpCharacter(GameObject playerGO)
		{
			byte[] data = playerGO.SaveObjectTree();
			data.WriteToFile(SaveSlotUtils.GetMpClientLocalPath() + PlayerSpawn.GetClientSaveFileName());
		}

		public static void DeleteMpCharacter()
		{
			bool flag = PlayerSpawn.HasMPCharacterSave();
			if (flag)
			{
				File.Delete(SaveSlotUtils.GetMpClientLocalPath() + PlayerSpawn.GetClientSaveFileName());
				UnityEngine.Debug.Log("Deleted MP client local save");
			}
		}

		public static bool HasMPCharacterSave()
		{
			string mpClientLocalPath = SaveSlotUtils.GetMpClientLocalPath();
			string clientSaveFileName = PlayerSpawn.GetClientSaveFileName();
			return File.Exists(mpClientLocalPath + clientSaveFileName);
		}

		private static bool LoadMpCharacter()
		{
			bool flag = PlayerSpawn.HasMPCharacterSave();
			if (flag)
			{
				Scene.ActiveMB.StartCoroutine(PlayerSpawn.LoadMpCharacterDelayed());
				Scene.PlaneCrash.SetupCrashedPlane_MP();
				return true;
			}
			return false;
		}

		private void InitPlayer()
		{
			this._player.GetComponent<playerAiInfo>().enabled = true;
		}

		[DebuggerHidden]
		private static IEnumerator LoadMpCharacterDelayed()
		{
			return new PlayerSpawn.<LoadMpCharacterDelayed>c__Iterator1B0();
		}
	}
}
