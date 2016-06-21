using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

[DontStore, AddComponentMenu("Storage/Store Information"), ExecuteInEditMode]
public class StoreInformation : UniqueIdentifier
{
	public bool StoreAllComponents = true;

	[HideInInspector]
	public List<string> Components = new List<string>();

	protected override void Awake()
	{
		base.Awake();
		foreach (UniqueIdentifier current in from t in base.GetComponents<UniqueIdentifier>()
		where t.GetType() == typeof(UniqueIdentifier) || (t.GetType() == typeof(StoreInformation) && t != this)
		select t)
		{
			UnityEngine.Object.DestroyImmediate(current);
		}
	}
}
