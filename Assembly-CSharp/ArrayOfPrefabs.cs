using System;
using UnityEngine;

public class ArrayOfPrefabs : MonoBehaviour
{
	public GUIStyle style;

	public Transform[] prefabs;

	public TextAsset aTextAsset;

	private void Start()
	{
		if (LevelSerializer.IsDeserializing)
		{
			return;
		}
		this.style.normal.textColor = Color.red;
	}

	private void OnGUI()
	{
		GUILayout.FlexibleSpace();
		GUILayout.Label("Hello", this.style, new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
	}
}
