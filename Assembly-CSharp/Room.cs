using System;
using UnityEngine;

[DontStore, AddComponentMenu("Storage/Rooms/Room")]
public class Room : MonoBehaviour
{
	public static Room Current;

	private void Awake()
	{
		Room.Current = this;
	}

	public void Save()
	{
		RoomManager.SaveCurrentRoom();
	}
}
