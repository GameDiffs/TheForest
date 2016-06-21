using System;
using UnityEngine;

[AddComponentMenu("Storage/Internal/Cleanup Maintenance (Immediately removes itself)"), ExecuteInEditMode]
public class RemoveEditors : MonoBehaviour
{
	private void Awake()
	{
		LevelSerializer.SavedGames.Clear();
		LevelSerializer.SaveDataToPlayerPrefs();
		UnityEngine.Object.DestroyImmediate(this);
	}
}
