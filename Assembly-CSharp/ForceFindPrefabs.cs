using System;
using UnityEngine;

public class ForceFindPrefabs : MonoBehaviour
{
	private void Awake()
	{
		JSONLevelSerializer.AddPrefabPath("Resources");
		LevelSerializer.AddPrefabPath("Resources");
	}

	private void Update()
	{
	}
}
