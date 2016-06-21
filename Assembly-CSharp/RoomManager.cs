using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager
{
	public static bool savingRoom;

	public static bool loadingRoom;

	public static Dictionary<string, string> rooms = new Dictionary<string, string>();

	public static void SaveCurrentRoom()
	{
		RoomManager.savingRoom = true;
		RoomManager.rooms[Application.loadedLevelName] = LevelSerializer.SerializeLevel();
		RoomManager.savingRoom = false;
	}

	public static void LoadRoom(string name)
	{
		RoomManager.LoadRoom(name, true);
	}

	public static void LoadRoom(string name, bool showGUI)
	{
		if (Room.Current)
		{
			Room.Current.Save();
		}
		if (RoomManager.rooms.ContainsKey(name))
		{
			RoomManager.loadingRoom = true;
			LevelLoader levelLoader = LevelSerializer.LoadSavedLevel(RoomManager.rooms[name]);
			levelLoader.whenCompleted = delegate(GameObject obj, List<GameObject> list)
			{
				foreach (GameObject current in list)
				{
					current.SendMessage("OnRoomWasLoaded", SendMessageOptions.DontRequireReceiver);
				}
			};
		}
		else
		{
			GameObject gameObject = new GameObject("RoomLoader");
			gameObject.AddComponent<RoomLoader>();
			Application.LoadLevel(name);
		}
	}
}
