using System;
using UniLinq;
using UnityEngine;

[DontStore, AddComponentMenu("Storage/Empty Object Identifier"), ExecuteInEditMode]
public class EmptyObjectIdentifier : StoreInformation
{
	protected override void Awake()
	{
		base.Awake();
		if (!base.gameObject.GetComponent<StoreMaterials>())
		{
			base.gameObject.AddComponent<StoreMaterials>();
		}
	}

	public static void FlagAll(GameObject gameObject)
	{
		foreach (Transform current in from c in gameObject.GetComponentsInChildren<Transform>()
		where !c.GetComponent<UniqueIdentifier>()
		select c)
		{
			current.gameObject.AddComponent<EmptyObjectIdentifier>();
		}
	}
}
