using System;
using UnityEngine;

[AddComponentMenu("Storage/Tests/Additional Data Storage")]
public class AdditionalDataStorage : MonoBehaviour
{
	public AdditionalData data;

	private void Start()
	{
		if (LevelSerializer.IsDeserializing)
		{
			return;
		}
		if (this.data == null)
		{
			this.data = ScriptableObject.CreateInstance<AdditionalData>();
		}
	}
}
