using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Storage/Rooms/Room Data Save Game Storage")]
public class RoomDataSaveGameStorage : DontStoreObjectInRoom
{
	public Dictionary<string, string> roomData
	{
		get
		{
			return RoomManager.rooms;
		}
		set
		{
			RoomManager.rooms = value;
		}
	}
}
