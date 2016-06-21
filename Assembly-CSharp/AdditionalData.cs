using System;
using UnityEngine;

[AddComponentMenu("Storage/Tests/AdditionalData")]
public class AdditionalData : ScriptableObject
{
	public float value = UnityEngine.Random.value;
}
