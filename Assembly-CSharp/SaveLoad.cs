using System;
using TheForest.Utils;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private static void HandleLevelSerializerProgress(string section, float complete)
	{
		Debug.Log(string.Format("Progress on {0} = {1:0.00%}", section, complete));
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Save", new GUILayoutOption[0]))
		{
			DateTime now = DateTime.Now;
			LevelSerializer.SaveGame("Game");
			Radical.CommitLog();
			Debug.Log(string.Format("{0:0.000}", (DateTime.Now - now).TotalSeconds));
		}
		if (LevelSerializer.CanResume && GUILayout.Button("Resume", new GUILayoutOption[0]))
		{
			LevelSerializer.Resume();
		}
		if (LevelSerializer.SavedGames.Count > 0)
		{
			GUILayout.Label("Available saved games", new GUILayoutOption[0]);
			foreach (LevelSerializer.SaveEntry current in LevelSerializer.SavedGames[LevelSerializer.PlayerName])
			{
				if (GUILayout.Button(current.Caption, new GUILayoutOption[0]))
				{
					current.Load();
				}
			}
		}
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonDown("Save"))
		{
			DateTime now = DateTime.Now;
			LevelSerializer.SaveGame("Game");
			Radical.CommitLog();
			Debug.Log(string.Format("{0:0.000}", (DateTime.Now - now).TotalSeconds));
		}
		if (LevelSerializer.CanResume && GUILayout.Button("Resume", new GUILayoutOption[0]))
		{
			LevelSerializer.Resume();
		}
		if (LevelSerializer.SavedGames.Count > 0)
		{
			GUILayout.Label("Available saved games", new GUILayoutOption[0]);
			foreach (LevelSerializer.SaveEntry current in LevelSerializer.SavedGames[LevelSerializer.PlayerName])
			{
				if (GUILayout.Button(current.Caption, new GUILayoutOption[0]))
				{
					current.Load();
				}
			}
		}
	}
}
