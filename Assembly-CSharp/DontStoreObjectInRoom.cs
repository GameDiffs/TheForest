using System;
using UnityEngine;

[AddComponentMenu("Storage/Rooms/Dont Store Object In Room")]
public class DontStoreObjectInRoom : MonoBehaviour, IControlSerialization, IControlSerializationEx
{
	public bool preserveThisObjectWhenLoading = true;

	private void Awake()
	{
		LevelLoader.OnDestroyObject += new LevelLoader.SerializedObjectDelegate(this.HandleLevelLoaderOnDestroyObject);
	}

	private void HandleLevelLoaderOnDestroyObject(GameObject toBeDestroyed, ref bool cancel)
	{
		if (toBeDestroyed == base.gameObject)
		{
			cancel = this.preserveThisObjectWhenLoading;
		}
	}

	private void OnDestroy()
	{
		LevelLoader.OnDestroyObject -= new LevelLoader.SerializedObjectDelegate(this.HandleLevelLoaderOnDestroyObject);
	}

	public bool ShouldSaveWholeObject()
	{
		return !RoomManager.savingRoom;
	}

	public bool ShouldSave()
	{
		return !RoomManager.savingRoom;
	}
}
