using System;
using UnityEngine;

public class SaveAndReload : MonoBehaviour
{
	private static int _id;

	public int id;

	private void Awake()
	{
		this.id = SaveAndReload._id++;
	}

	private void OnMouseDown()
	{
		JSONLevelSerializer.SaveObjectTreeToServer("ftp://whydoidoit.net/SavedData" + this.id.ToString() + ".json", base.gameObject, "testserializer", "T3sts3rializer", delegate(Exception error)
		{
			Debug.Log("Uploaded!" + error);
		});
		UnityEngine.Object.Destroy(base.gameObject);
		Loom.QueueOnMainThread(delegate
		{
			Debug.Log("Downloading");
			JSONLevelSerializer.LoadObjectTreeFromServer("http://whydoidoit.net/testserializer/SavedData" + this.id.ToString() + ".json", null, null);
		}, 6f);
	}
}
