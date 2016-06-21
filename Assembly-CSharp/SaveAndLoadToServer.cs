using System;
using UnityEngine;

public class SaveAndLoadToServer : MonoBehaviour
{
	public GameObject targetGameObject;

	private void OnGUI()
	{
		using (new VerticalCentered())
		{
			if (this.targetGameObject && GUILayout.Button("Save to server JSON", new GUILayoutOption[0]))
			{
				JSONLevelSerializer.SaveObjectTreeToServer("ftp://whydoidoit.net/testme.json", this.targetGameObject, "testserializer", "T3sts3rializer", new Action<Exception>(this.Completed));
				UnityEngine.Object.Destroy(this.targetGameObject);
			}
			if (!this.targetGameObject && GUILayout.Button("Load from server JSON", new GUILayoutOption[0]))
			{
				JSONLevelSerializer.LoadObjectTreeFromServer("http://whydoidoit.net/testserializer/testme.json", new Action<JSONLevelLoader>(this.CompletedJSONLoad), null);
			}
			if (this.targetGameObject && GUILayout.Button("Save to server Binary", new GUILayoutOption[0]))
			{
				LevelSerializer.SaveObjectTreeToServer("ftp://whydoidoit.net/testme.dat", this.targetGameObject, "testserializer", "T3sts3rializer", new Action<Exception>(this.Completed));
				UnityEngine.Object.Destroy(this.targetGameObject);
			}
			if (!this.targetGameObject && GUILayout.Button("Load from server Binary", new GUILayoutOption[0]))
			{
				LevelSerializer.LoadObjectTreeFromServer("http://whydoidoit.net/testserializer/testme.dat", new Action<LevelLoader>(this.CompletedLoad));
			}
			if (GUILayout.Button("Save scene to server JSON", new GUILayoutOption[0]))
			{
				JSONLevelSerializer.SerializeLevelToServer("ftp://whydoidoit.net/testscene.json", "testserializer", "T3sts3rializer", new Action<Exception>(this.Completed));
			}
			if (GUILayout.Button("Load scene from server JSON", new GUILayoutOption[0]))
			{
				JSONLevelSerializer.LoadSavedLevelFromServer("http://whydoidoit.net/testserializer/testscene.json", null);
			}
			if (GUILayout.Button("Save scene to server Binary", new GUILayoutOption[0]))
			{
				JSONLevelSerializer.SerializeLevelToServer("ftp://whydoidoit.net/testscene.data", "testserializer", "T3sts3rializer", new Action<Exception>(this.Completed));
			}
			if (GUILayout.Button("Load scene from server Binary", new GUILayoutOption[0]))
			{
				JSONLevelSerializer.LoadSavedLevelFromServer("http://whydoidoit.net/testserializer/testscene.data", null);
			}
		}
	}

	private void CompletedLoad(LevelLoader loader)
	{
		this.targetGameObject = loader.Last;
	}

	private void CompletedJSONLoad(JSONLevelLoader loader)
	{
		this.targetGameObject = loader.Last;
	}

	private void Completed(Exception e)
	{
		if (e != null)
		{
			Debug.Log("Error");
			Debug.Log(e.ToString());
			Debug.Log(base.transform.position.ToString());
		}
		else
		{
			Debug.Log("Succeeded");
		}
	}
}
