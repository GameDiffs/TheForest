using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Data
{
	public class UserDataStore_PlayerPrefs : UserDataStore
	{
		[SerializeField]
		private bool isEnabled = true;

		[SerializeField]
		private bool loadDataOnStart = true;

		[SerializeField]
		private string playerPrefsKeyPrefix = "RewiredSaveData";

		public override void Save()
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SaveAll();
		}

		public override void SaveControllerData(int playerId, ControllerType controllerType, int controllerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SaveControllerDataNow(playerId, controllerType, controllerId);
		}

		public override void SaveControllerData(ControllerType controllerType, int controllerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SaveControllerDataNow(controllerType, controllerId);
		}

		public override void SavePlayerData(int playerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SavePlayerDataNow(playerId);
		}

		public override void SaveInputBehavior(int playerId, int behaviorId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
				return;
			}
			this.SaveInputBehaviorNow(playerId, behaviorId);
		}

		public override void Load()
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadAll();
		}

		public override void LoadControllerData(int playerId, ControllerType controllerType, int controllerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadControllerDataNow(playerId, controllerType, controllerId);
		}

		public override void LoadControllerData(ControllerType controllerType, int controllerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadControllerDataNow(controllerType, controllerId);
		}

		public override void LoadPlayerData(int playerId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadPlayerDataNow(playerId);
		}

		public override void LoadInputBehavior(int playerId, int behaviorId)
		{
			if (!this.isEnabled)
			{
				Debug.LogWarning("UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
				return;
			}
			this.LoadInputBehaviorNow(playerId, behaviorId);
		}

		protected override void OnInitialize()
		{
			if (this.loadDataOnStart)
			{
				this.Load();
			}
		}

		protected override void OnControllerConnected(ControllerStatusChangedEventArgs args)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (args.controllerType == ControllerType.Joystick)
			{
				this.LoadJoystickData(args.controllerId);
			}
		}

		protected override void OnControllerPreDiscconnect(ControllerStatusChangedEventArgs args)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (args.controllerType == ControllerType.Joystick)
			{
				this.SaveJoystickData(args.controllerId);
			}
		}

		protected override void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (!this.isEnabled)
			{
				return;
			}
		}

		private void LoadAll()
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				this.LoadPlayerDataNow(allPlayers[i]);
			}
			this.LoadAllJoystickCalibrationData();
		}

		private void LoadPlayerDataNow(int playerId)
		{
			this.LoadPlayerDataNow(ReInput.players.GetPlayer(playerId));
		}

		private void LoadPlayerDataNow(Player player)
		{
			if (player == null)
			{
				return;
			}
			this.LoadInputBehaviors(player.id);
			this.LoadControllerMaps(player.id, ControllerType.Keyboard, 0);
			this.LoadControllerMaps(player.id, ControllerType.Mouse, 0);
			foreach (Rewired.Joystick current in player.controllers.Joysticks)
			{
				this.LoadControllerMaps(player.id, ControllerType.Joystick, current.id);
			}
		}

		private void LoadAllJoystickCalibrationData()
		{
			IList<Rewired.Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				this.LoadJoystickCalibrationData(joysticks[i]);
			}
		}

		private void LoadJoystickCalibrationData(Rewired.Joystick joystick)
		{
			if (joystick == null)
			{
				return;
			}
			joystick.ImportCalibrationMapFromXmlString(this.GetJoystickCalibrationMapXml(joystick));
		}

		private void LoadJoystickCalibrationData(int joystickId)
		{
			this.LoadJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
		}

		private void LoadJoystickData(int joystickId)
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.controllers.ContainsController(ControllerType.Joystick, joystickId))
				{
					this.LoadControllerMaps(player.id, ControllerType.Joystick, joystickId);
				}
			}
			this.LoadJoystickCalibrationData(joystickId);
		}

		private void LoadControllerDataNow(int playerId, ControllerType controllerType, int controllerId)
		{
			this.LoadControllerMaps(playerId, controllerType, controllerId);
			this.LoadControllerDataNow(controllerType, controllerId);
		}

		private void LoadControllerDataNow(ControllerType controllerType, int controllerId)
		{
			if (controllerType == ControllerType.Joystick)
			{
				this.LoadJoystickCalibrationData(controllerId);
			}
		}

		private void LoadControllerMaps(int playerId, ControllerType controllerType, int controllerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			Controller controller = ReInput.controllers.GetController(controllerType, controllerId);
			if (controller == null)
			{
				return;
			}
			List<string> allControllerMapsXml = this.GetAllControllerMapsXml(player, true, controllerType, controller);
			if (allControllerMapsXml.Count == 0)
			{
				return;
			}
			player.controllers.maps.AddMapsFromXml(controllerType, controllerId, allControllerMapsXml);
		}

		private void LoadInputBehaviors(int playerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			IList<InputBehavior> inputBehaviors = ReInput.mapping.GetInputBehaviors(player.id);
			for (int i = 0; i < inputBehaviors.Count; i++)
			{
				this.LoadInputBehaviorNow(player, inputBehaviors[i]);
			}
		}

		private void LoadInputBehaviorNow(int playerId, int behaviorId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			InputBehavior inputBehavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
			if (inputBehavior == null)
			{
				return;
			}
			this.LoadInputBehaviorNow(player, inputBehavior);
		}

		private void LoadInputBehaviorNow(Player player, InputBehavior inputBehavior)
		{
			if (player == null || inputBehavior == null)
			{
				return;
			}
			string inputBehaviorXml = this.GetInputBehaviorXml(player, inputBehavior.id);
			if (inputBehaviorXml == null || inputBehaviorXml == string.Empty)
			{
				return;
			}
			inputBehavior.ImportXmlString(inputBehaviorXml);
		}

		private void SaveAll()
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				this.SavePlayerDataNow(allPlayers[i]);
			}
			this.SaveAllJoystickCalibrationData();
			PlayerPrefs.Save();
		}

		private void SavePlayerDataNow(int playerId)
		{
			this.SavePlayerDataNow(ReInput.players.GetPlayer(playerId));
		}

		private void SavePlayerDataNow(Player player)
		{
			if (player == null)
			{
				return;
			}
			PlayerSaveData saveData = player.GetSaveData(true);
			this.SaveInputBehaviors(player, saveData);
			this.SaveControllerMaps(player, saveData);
		}

		private void SaveAllJoystickCalibrationData()
		{
			IList<Rewired.Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int i = 0; i < joysticks.Count; i++)
			{
				this.SaveJoystickCalibrationData(joysticks[i]);
			}
		}

		private void SaveJoystickCalibrationData(int joystickId)
		{
			this.SaveJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
		}

		private void SaveJoystickCalibrationData(Rewired.Joystick joystick)
		{
			if (joystick == null)
			{
				return;
			}
			JoystickCalibrationMapSaveData calibrationMapSaveData = joystick.GetCalibrationMapSaveData();
			string joystickCalibrationMapPlayerPrefsKey = this.GetJoystickCalibrationMapPlayerPrefsKey(calibrationMapSaveData);
			PlayerPrefs.SetString(joystickCalibrationMapPlayerPrefsKey, calibrationMapSaveData.map.ToXmlString());
		}

		private void SaveJoystickData(int joystickId)
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.controllers.ContainsController(ControllerType.Joystick, joystickId))
				{
					this.SaveControllerMaps(player.id, ControllerType.Joystick, joystickId);
				}
			}
			this.SaveJoystickCalibrationData(joystickId);
		}

		private void SaveControllerDataNow(int playerId, ControllerType controllerType, int controllerId)
		{
			this.SaveControllerMaps(playerId, controllerType, controllerId);
			this.SaveControllerDataNow(controllerType, controllerId);
		}

		private void SaveControllerDataNow(ControllerType controllerType, int controllerId)
		{
			if (controllerType == ControllerType.Joystick)
			{
				this.SaveJoystickCalibrationData(controllerId);
			}
		}

		private void SaveControllerMaps(Player player, PlayerSaveData playerSaveData)
		{
			foreach (ControllerMapSaveData current in playerSaveData.AllControllerMapSaveData)
			{
				string controllerMapPlayerPrefsKey = this.GetControllerMapPlayerPrefsKey(player, current);
				PlayerPrefs.SetString(controllerMapPlayerPrefsKey, current.map.ToXmlString());
			}
		}

		private void SaveControllerMaps(int playerId, ControllerType controllerType, int controllerId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			if (!player.controllers.ContainsController(controllerType, controllerId))
			{
				return;
			}
			ControllerMapSaveData[] mapSaveData = player.controllers.maps.GetMapSaveData(controllerType, controllerId, true);
			if (mapSaveData == null)
			{
				return;
			}
			for (int i = 0; i < mapSaveData.Length; i++)
			{
				string controllerMapPlayerPrefsKey = this.GetControllerMapPlayerPrefsKey(player, mapSaveData[i]);
				PlayerPrefs.SetString(controllerMapPlayerPrefsKey, mapSaveData[i].map.ToXmlString());
			}
		}

		private void SaveInputBehaviors(Player player, PlayerSaveData playerSaveData)
		{
			if (player == null)
			{
				return;
			}
			InputBehavior[] inputBehaviors = playerSaveData.inputBehaviors;
			for (int i = 0; i < inputBehaviors.Length; i++)
			{
				this.SaveInputBehaviorNow(player, inputBehaviors[i]);
			}
		}

		private void SaveInputBehaviorNow(int playerId, int behaviorId)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null)
			{
				return;
			}
			InputBehavior inputBehavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
			if (inputBehavior == null)
			{
				return;
			}
			this.SaveInputBehaviorNow(player, inputBehavior);
		}

		private void SaveInputBehaviorNow(Player player, InputBehavior inputBehavior)
		{
			if (player == null || inputBehavior == null)
			{
				return;
			}
			string inputBehaviorPlayerPrefsKey = this.GetInputBehaviorPlayerPrefsKey(player, inputBehavior);
			PlayerPrefs.SetString(inputBehaviorPlayerPrefsKey, inputBehavior.ToXmlString());
		}

		private string GetBasePlayerPrefsKey(Player player)
		{
			string str = this.playerPrefsKeyPrefix;
			return str + "|playerName=" + player.name;
		}

		private string GetControllerMapPlayerPrefsKey(Player player, ControllerMapSaveData saveData)
		{
			string text = this.GetBasePlayerPrefsKey(player);
			text += "|dataType=ControllerMap";
			text = text + "|controllerMapType=" + saveData.mapTypeString;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"|categoryId=",
				saveData.map.categoryId,
				"|layoutId=",
				saveData.map.layoutId
			});
			text = text + "|hardwareIdentifier=" + saveData.controllerHardwareIdentifier;
			if (saveData.mapType == typeof(JoystickMap))
			{
				text = text + "|hardwareGuid=" + ((JoystickMapSaveData)saveData).joystickHardwareTypeGuid.ToString();
			}
			return text;
		}

		private string GetControllerMapXml(Player player, ControllerType controllerType, int categoryId, int layoutId, Controller controller)
		{
			string text = this.GetBasePlayerPrefsKey(player);
			text += "|dataType=ControllerMap";
			text = text + "|controllerMapType=" + controller.mapTypeString;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"|categoryId=",
				categoryId,
				"|layoutId=",
				layoutId
			});
			text = text + "|hardwareIdentifier=" + controller.hardwareIdentifier;
			if (controllerType == ControllerType.Joystick)
			{
				Rewired.Joystick joystick = (Rewired.Joystick)controller;
				text = text + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
			}
			if (!PlayerPrefs.HasKey(text))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(text);
		}

		private List<string> GetAllControllerMapsXml(Player player, bool userAssignableMapsOnly, ControllerType controllerType, Controller controller)
		{
			List<string> list = new List<string>();
			IList<InputMapCategory> mapCategories = ReInput.mapping.MapCategories;
			for (int i = 0; i < mapCategories.Count; i++)
			{
				InputMapCategory inputMapCategory = mapCategories[i];
				if (!userAssignableMapsOnly || inputMapCategory.userAssignable)
				{
					IList<InputLayout> list2 = ReInput.mapping.MapLayouts(controllerType);
					for (int j = 0; j < list2.Count; j++)
					{
						InputLayout inputLayout = list2[j];
						string controllerMapXml = this.GetControllerMapXml(player, controllerType, inputMapCategory.id, inputLayout.id, controller);
						if (!(controllerMapXml == string.Empty))
						{
							list.Add(controllerMapXml);
						}
					}
				}
			}
			return list;
		}

		private string GetJoystickCalibrationMapPlayerPrefsKey(JoystickCalibrationMapSaveData saveData)
		{
			string str = this.playerPrefsKeyPrefix;
			str += "|dataType=CalibrationMap";
			str = str + "|controllerType=" + saveData.controllerType.ToString();
			str = str + "|hardwareIdentifier=" + saveData.hardwareIdentifier;
			return str + "|hardwareGuid=" + saveData.joystickHardwareTypeGuid.ToString();
		}

		private string GetJoystickCalibrationMapXml(Rewired.Joystick joystick)
		{
			string text = this.playerPrefsKeyPrefix;
			text += "|dataType=CalibrationMap";
			text = text + "|controllerType=" + joystick.type.ToString();
			text = text + "|hardwareIdentifier=" + joystick.hardwareIdentifier;
			text = text + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
			if (!PlayerPrefs.HasKey(text))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(text);
		}

		private string GetInputBehaviorPlayerPrefsKey(Player player, InputBehavior saveData)
		{
			string text = this.GetBasePlayerPrefsKey(player);
			text += "|dataType=InputBehavior";
			return text + "|id=" + saveData.id;
		}

		private string GetInputBehaviorXml(Player player, int id)
		{
			string text = this.GetBasePlayerPrefsKey(player);
			text += "|dataType=InputBehavior";
			text = text + "|id=" + id;
			if (!PlayerPrefs.HasKey(text))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(text);
		}
	}
}
