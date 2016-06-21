using Serialization;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

[AddComponentMenu("Storage/Save Game Manager"), ExecuteInEditMode]
public class SaveGameManager : MonoBehaviour
{
	[Serializable]
	public class StoredEntry
	{
		public GameObject gameObject;

		public string Id = Guid.NewGuid().ToString();

		public bool lockId;
	}

	public class AssetReference
	{
		public string name;

		public string type;

		public int index;
	}

	public UnityEngine.Object[] requiredObjects;

	private static SaveGameManager instance;

	public static bool hasRun;

	public StoredReferences Reference;

	private static StoredReferences _cached;

	private static List<Action> _initActions = new List<Action>();

	private bool hasWoken;

	private Dictionary<Type, Index<string, List<UnityEngine.Object>>> assetReferences = new Dictionary<Type, Index<string, List<UnityEngine.Object>>>();

	public static SaveGameManager Instance
	{
		get
		{
			if (SaveGameManager.instance == null)
			{
				SaveGameManager.instance = (from GameObject g in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))
				where g.GetComponent<SaveGameManager>() != null
				select g.GetComponent<SaveGameManager>()).FirstOrDefault<SaveGameManager>();
				if (SaveGameManager.instance == null)
				{
					GameObject gameObject = new GameObject("Save Game Manager");
					SaveGameManager.instance = gameObject.AddComponent<SaveGameManager>();
					Debug.LogWarning("Creating a save game manager dynamically, consider adding one to the scene");
				}
			}
			return SaveGameManager.instance;
		}
		set
		{
			SaveGameManager.instance = value;
		}
	}

	public static void Loaded()
	{
		SaveGameManager._cached = null;
	}

	public GameObject GetById(string id)
	{
		SaveGameManager.StoredEntry storedEntry = SaveGameManager.Instance.Reference[id];
		return (storedEntry == null) ? null : storedEntry.gameObject;
	}

	public void SetId(GameObject gameObject, string id)
	{
		SaveGameManager.StoredEntry storedEntry = SaveGameManager.Instance.Reference[gameObject] ?? SaveGameManager.Instance.Reference[id];
		if (storedEntry != null)
		{
			SaveGameManager.Instance.Reference.Remove(storedEntry.gameObject);
			storedEntry.gameObject = gameObject;
			if (!storedEntry.lockId)
			{
				storedEntry.Id = id;
			}
		}
		else
		{
			storedEntry = new SaveGameManager.StoredEntry
			{
				gameObject = gameObject,
				Id = id
			};
		}
		SaveGameManager.Instance.Reference[storedEntry.Id] = storedEntry;
	}

	public static string GetId(GameObject gameObject)
	{
		if (SaveGameManager.Instance == null || gameObject == null)
		{
			return string.Empty;
		}
		SaveGameManager.StoredEntry storedEntry = SaveGameManager.Instance.Reference[gameObject];
		if (storedEntry != null)
		{
			return storedEntry.Id;
		}
		if (Application.isLoadingLevel && !Application.isPlaying)
		{
			return null;
		}
		storedEntry = new SaveGameManager.StoredEntry
		{
			gameObject = gameObject
		};
		SaveGameManager.Instance.Reference[storedEntry.Id] = storedEntry;
		return storedEntry.Id;
	}

	public static void Initialize(Action a)
	{
		if (SaveGameManager.Instance != null && SaveGameManager.Instance.hasWoken)
		{
			a();
		}
		else
		{
			SaveGameManager._initActions.Add(a);
		}
	}

	public SaveGameManager.AssetReference GetAssetId(UnityEngine.Object referencedObject)
	{
		if (referencedObject == null)
		{
			return new SaveGameManager.AssetReference
			{
				index = -1
			};
		}
		Index<string, List<UnityEngine.Object>> index = null;
		Type type = referencedObject.GetType();
		if (!this.assetReferences.TryGetValue(type, out index))
		{
			index = (this.assetReferences[type] = new Index<string, List<UnityEngine.Object>>());
			IEnumerable<UnityEngine.Object> enumerable = Resources.FindObjectsOfTypeAll(type).Except(UnityEngine.Object.FindObjectsOfType(type));
			foreach (UnityEngine.Object current in enumerable)
			{
				index[current.name].Add(current);
			}
		}
		List<UnityEngine.Object> list = null;
		if (!index.TryGetValue(referencedObject.name, out list))
		{
			return new SaveGameManager.AssetReference
			{
				index = -1
			};
		}
		return new SaveGameManager.AssetReference
		{
			index = list.IndexOf(referencedObject),
			name = referencedObject.name,
			type = type.FullName
		};
	}

	public object GetAsset(SaveGameManager.AssetReference id)
	{
		if (id.index == -1)
		{
			return null;
		}
		object result;
		try
		{
			Type typeEx = UnitySerializer.GetTypeEx(id.type);
			Index<string, List<UnityEngine.Object>> index;
			if (!this.assetReferences.TryGetValue(typeEx, out index))
			{
				index = (this.assetReferences[typeEx] = new Index<string, List<UnityEngine.Object>>());
				IEnumerable<UnityEngine.Object> enumerable = Resources.FindObjectsOfTypeAll(typeEx).Except(UnityEngine.Object.FindObjectsOfType(typeEx));
				foreach (UnityEngine.Object current in enumerable)
				{
					index[current.name].Add(current);
				}
			}
			List<UnityEngine.Object> list;
			if (!index.TryGetValue(id.name, out list))
			{
				result = null;
			}
			else if (id.index >= list.Count)
			{
				result = null;
			}
			else
			{
				result = list[id.index];
			}
		}
		catch
		{
			result = null;
		}
		return result;
	}

	private void OnDestroy()
	{
		UnityEngine.Object.DestroyImmediate(this.Reference);
	}

	private void GetAllInactiveGameObjects()
	{
		IEnumerable<Transform> items = from g in this.Reference.AllReferences
		select g.transform;
		this.RecurseAddInactive(items);
	}

	private void RecurseAddInactive(IEnumerable<Transform> items)
	{
		foreach (Transform current in items)
		{
			if (current.GetComponent<UniqueIdentifier>() != null && !current.gameObject.active)
			{
				SaveGameManager.GetId(current.gameObject);
			}
			this.RecurseAddInactive(current.Cast<Transform>());
		}
	}

	private void Awake()
	{
		if (this.Reference == null)
		{
			this.Reference = ScriptableObject.CreateInstance<StoredReferences>();
		}
		if (Application.isEditor)
		{
			this.GetAllInactiveGameObjects();
		}
		if (SaveGameManager.Instance != null && SaveGameManager.Instance != this)
		{
			UnityEngine.Object.Destroy(SaveGameManager.Instance.gameObject);
		}
		SaveGameManager.Instance = this;
		this.hasWoken = true;
		if (Application.isPlaying && !SaveGameManager.hasRun)
		{
			SaveGameManager._cached = this.Reference;
			SaveGameManager.hasRun = true;
		}
		else if (!Application.isPlaying)
		{
			SaveGameManager.hasRun = false;
			if (SaveGameManager._cached != null && SaveGameManager._cached.Count > 0)
			{
				this.Reference = SaveGameManager._cached.Alive();
			}
		}
		if (SaveGameManager._initActions.Count > 0)
		{
			foreach (Action current in SaveGameManager._initActions)
			{
				current();
			}
			SaveGameManager._initActions.Clear();
		}
	}
}
