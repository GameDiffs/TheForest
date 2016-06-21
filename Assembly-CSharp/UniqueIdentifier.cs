using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

[DontStore, AddComponentMenu("Storage/Unique Identifier"), ExecuteInEditMode]
public class UniqueIdentifier : MonoBehaviour
{
	[HideInInspector]
	public bool IsDeserializing;

	public string _id = string.Empty;

	private static List<UniqueIdentifier> allIdentifiers = new List<UniqueIdentifier>();

	[HideInInspector]
	public string classId = Guid.NewGuid().ToString();

	public string Id
	{
		get
		{
			if (base.gameObject == null)
			{
				return this._id;
			}
			if (!string.IsNullOrEmpty(this._id))
			{
				return this._id;
			}
			return this._id = SaveGameManager.GetId(base.gameObject);
		}
		set
		{
			this._id = value;
			SaveGameManager.Instance.SetId(base.gameObject, value);
		}
	}

	public static List<UniqueIdentifier> AllIdentifiers
	{
		get
		{
			UniqueIdentifier.allIdentifiers = (from a in UniqueIdentifier.allIdentifiers
			where a != null
			select a).ToList<UniqueIdentifier>();
			return UniqueIdentifier.allIdentifiers;
		}
		set
		{
			UniqueIdentifier.allIdentifiers = value;
		}
	}

	public string ClassId
	{
		get
		{
			return this.classId;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				value = Guid.NewGuid().ToString();
			}
			this.classId = value;
		}
	}

	public static GameObject GetByName(string id)
	{
		GameObject byId = SaveGameManager.Instance.GetById(id);
		return byId ?? GameObject.Find(id);
	}

	public void FullConfigure()
	{
		this.ConfigureId();
		foreach (UniqueIdentifier current in from c in base.GetComponentsInChildren<UniqueIdentifier>(true)
		where !c.gameObject.activeInHierarchy
		select c)
		{
			current.ConfigureId();
		}
	}

	protected virtual void Awake()
	{
		foreach (UniqueIdentifier current in from t in base.GetComponents<UniqueIdentifier>()
		where t.GetType() == typeof(UniqueIdentifier) && t != this
		select t)
		{
			UnityEngine.Object.DestroyImmediate(current);
		}
		SaveGameManager.Initialize(delegate
		{
			if (base.gameObject == null)
			{
				return;
			}
			this.FullConfigure();
		});
	}

	private void ConfigureId()
	{
		this._id = SaveGameManager.GetId(base.gameObject);
		UniqueIdentifier.AllIdentifiers.Add(this);
	}

	private void OnDestroy()
	{
		if (UniqueIdentifier.AllIdentifiers.Count > 0)
		{
			UniqueIdentifier.AllIdentifiers.Remove(this);
		}
	}
}
