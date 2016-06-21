using System;
using UnityEngine;

[AddComponentMenu("Storage/Tests/Delete Component")]
public class DeleteComponent : MonoBehaviour
{
	private void Start()
	{
		if (!LevelSerializer.IsDeserializing)
		{
			UnityEngine.Object.Destroy(base.GetComponent<DeletedComponent>());
		}
	}
}
