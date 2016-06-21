using System;
using UnityEngine;

public class TestSavingthen : MonoBehaviour
{
	private void Start()
	{
		LevelSerializer.Checkpoint();
	}

	private void Update()
	{
		if (LevelSerializer.CanResume)
		{
			MonoBehaviour.print("Cann");
		}
	}
}
