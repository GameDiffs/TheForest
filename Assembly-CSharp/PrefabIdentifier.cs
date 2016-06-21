using System;
using UniLinq;
using UnityEngine;

[DontStore, AddComponentMenu("Storage/Prefab Identifier"), ExecuteInEditMode]
public class PrefabIdentifier : StoreInformation
{
	private bool inScenePrefab;

	public bool IsInScene()
	{
		return this.inScenePrefab;
	}

	protected override void Awake()
	{
		this.inScenePrefab = true;
		base.Awake();
		foreach (UniqueIdentifier current in from t in base.GetComponents<UniqueIdentifier>()
		where t.GetType() == typeof(UniqueIdentifier) || (t.GetType() == typeof(PrefabIdentifier) && t != this) || t.GetType() == typeof(StoreInformation)
		select t)
		{
			UnityEngine.Object.DestroyImmediate(current);
		}
	}
}
