using System;
using UnityEngine;

[Serializable]
public class AnimalSpawnConfig
{
	public float Weight;

	public GameObject Prefab;

	public bool largeAnimalType;

	public bool nocturnal;

	public float WeightRunningTotal
	{
		get;
		set;
	}
}
