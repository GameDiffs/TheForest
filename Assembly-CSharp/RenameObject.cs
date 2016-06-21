using System;
using UnityEngine;

public class RenameObject : MonoBehaviour
{
	private void Start()
	{
		if (!LevelSerializer.IsDeserializing)
		{
			base.name = UnityEngine.Random.Range(0, 100000).ToString();
		}
	}
}
