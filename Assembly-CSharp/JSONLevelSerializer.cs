using Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using UniLinq;
using UnityEngine;

public static class JSONLevelSerializer
{
	public enum SerializationModes
	{
		SerializeWhenFree,
		CacheSerialization
	}

	private class CompareGameObjects : IEqualityComparer<GameObject>
	{
		public static readonly JSONLevelSerializer.CompareGameObjects Instance = new JSONLevelSerializer.CompareGameObjects();

		public bool Equals(GameObject x, GameObject y)
		{
			return string.Compare(x.GetComponent<PrefabIdentifier>().ClassId, y.GetComponent<PrefabIdentifier>().ClassId, StringComparison.Ordinal) == 0;
		}

		public int GetHashCode(GameObject obj)
		{
			return obj.GetComponent<PrefabIdentifier>().ClassId.GetHashCode();
		}
	}

	public class LevelData
	{
		public string Name;

		public List<JSONLevelSerializer.StoredData> StoredItems;

		public List<JSONLevelSerializer.StoredItem> StoredObjectNames;

		public string rootObject;
	}

	private class ProgressHelper
	{
		public void SetProgress(long inSize, long outSize)
		{
			JSONLevelSerializer.RaiseProgress("Compression", 0.5f);
		}
	}

	public class SaveEntry
	{
		public string Data;

		public string Level;

		public string Name;

		public DateTime When;

		public string Caption
		{
			get
			{
				return string.Format("{0} - {1} - {2:g}", this.Name, this.Level, this.When);
			}
		}

		public SaveEntry(string contents)
		{
			UnitySerializer.JSONDeserializeInto(contents, this);
		}

		public SaveEntry()
		{
		}

		public void Load()
		{
			JSONLevelSerializer.LoadSavedLevel(this.Data);
		}

		public void Delete()
		{
			KeyValuePair<string, List<JSONLevelSerializer.SaveEntry>> keyValuePair = JSONLevelSerializer.SavedGames.FirstOrDefault((KeyValuePair<string, List<JSONLevelSerializer.SaveEntry>> p) => p.Value.Contains(this));
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.Remove(this);
				JSONLevelSerializer.SaveDataToPlayerPrefs();
			}
		}

		public override string ToString()
		{
			return UnitySerializer.JSONSerialize(this);
		}
	}

	public class SerializationHelper : MonoBehaviour
	{
		public string gameName;

		public Action<string, bool> perform;

		private void Update()
		{
			if (!JSONLevelSerializer.IsSuspended)
			{
				if (this.perform != null)
				{
					this.perform(this.gameName, false);
				}
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}
	}

	public class SerializationSuspendedException : Exception
	{
		public SerializationSuspendedException() : base("Serialization was suspended: " + JSONLevelSerializer._suspensionCount + " times")
		{
		}
	}

	public class StoredData
	{
		public string ClassId;

		public string Data;

		public string Name;

		public string Type;
	}

	public class StoredItem
	{
		public bool Active;

		public bool createEmptyObject;

		public int layer;

		public string tag;

		public bool setExtraData;

		public List<string> ChildIds = new List<string>();

		public Dictionary<string, List<string>> Children = new Dictionary<string, List<string>>();

		public string ClassId;

		public Dictionary<string, bool> Components;

		[DoNotSerialize]
		public GameObject GameObject;

		public string GameObjectName;

		public string Name;

		public string ParentName;

		public override string ToString()
		{
			return string.Format("{0}  child of {2} - ({1})", this.Name, this.ClassId, this.ParentName);
		}
	}

	public delegate void StoreQuery(GameObject go, ref bool store);

	public delegate void StoreComponentQuery(Component component, ref bool store);

	private static Dictionary<string, GameObject> allPrefabs;

	public static HashSet<string> IgnoreTypes;

	public static Dictionary<Type, IComponentSerializer> CustomSerializers;

	private static int lastFrame;

	public static string PlayerName;

	public static bool SaveResumeInformation;

	private static int _suspensionCount;

	private static JSONLevelSerializer.SaveEntry _cachedState;

	public static JSONLevelSerializer.SerializationModes SerializationMode;

	public static int MaxGames;

	public static global::Lookup<string, List<JSONLevelSerializer.SaveEntry>> SavedGames;

	private static readonly List<Type> _stopCases;

	private static readonly List<object> createdPlugins;

	private static readonly object Guard;

	private static WebClient webClient;

	private static int uploadCount;

	private static int _collectionCount;

	public static event Action Deserialized
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelSerializer.Deserialized = (Action)Delegate.Combine(JSONLevelSerializer.Deserialized, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelSerializer.Deserialized = (Action)Delegate.Remove(JSONLevelSerializer.Deserialized, value);
		}
	}

	public static event Action GameSaved
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelSerializer.GameSaved = (Action)Delegate.Combine(JSONLevelSerializer.GameSaved, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelSerializer.GameSaved = (Action)Delegate.Remove(JSONLevelSerializer.GameSaved, value);
		}
	}

	public static event Action SuspendingSerialization
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelSerializer.SuspendingSerialization = (Action)Delegate.Combine(JSONLevelSerializer.SuspendingSerialization, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelSerializer.SuspendingSerialization = (Action)Delegate.Remove(JSONLevelSerializer.SuspendingSerialization, value);
		}
	}

	public static event Action ResumingSerialization
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelSerializer.ResumingSerialization = (Action)Delegate.Combine(JSONLevelSerializer.ResumingSerialization, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelSerializer.ResumingSerialization = (Action)Delegate.Remove(JSONLevelSerializer.ResumingSerialization, value);
		}
	}

	public static event JSONLevelSerializer.StoreQuery Store
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelSerializer.Store = (JSONLevelSerializer.StoreQuery)Delegate.Combine(JSONLevelSerializer.Store, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelSerializer.Store = (JSONLevelSerializer.StoreQuery)Delegate.Remove(JSONLevelSerializer.Store, value);
		}
	}

	public static event JSONLevelSerializer.StoreComponentQuery StoreComponent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelSerializer.StoreComponent = (JSONLevelSerializer.StoreComponentQuery)Delegate.Combine(JSONLevelSerializer.StoreComponent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelSerializer.StoreComponent = (JSONLevelSerializer.StoreComponentQuery)Delegate.Remove(JSONLevelSerializer.StoreComponent, value);
		}
	}

	public static event Action<string, float> Progress
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelSerializer.Progress = (Action<string, float>)Delegate.Combine(JSONLevelSerializer.Progress, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelSerializer.Progress = (Action<string, float>)Delegate.Remove(JSONLevelSerializer.Progress, value);
		}
	}

	public static bool IsDeserializing
	{
		get
		{
			return LevelSerializer.IsDeserializing;
		}
		set
		{
			LevelSerializer.IsDeserializing = value;
		}
	}

	public static Dictionary<string, GameObject> AllPrefabs
	{
		get
		{
			if (Time.frameCount != JSONLevelSerializer.lastFrame)
			{
				JSONLevelSerializer.allPrefabs = (from p in JSONLevelSerializer.allPrefabs
				where p.Value
				select p).ToDictionary((KeyValuePair<string, GameObject> p) => p.Key, (KeyValuePair<string, GameObject> p) => p.Value);
				JSONLevelSerializer.lastFrame = Time.frameCount;
			}
			return JSONLevelSerializer.allPrefabs;
		}
		set
		{
			JSONLevelSerializer.allPrefabs = value;
		}
	}

	public static bool CanResume
	{
		get
		{
			return !string.IsNullOrEmpty(PlayerPrefs.GetString(JSONLevelSerializer.PlayerName + "JSON__RESUME__"));
		}
	}

	public static bool IsSuspended
	{
		get
		{
			return JSONLevelSerializer._suspensionCount > 0;
		}
	}

	public static int SuspensionCount
	{
		get
		{
			return JSONLevelSerializer._suspensionCount;
		}
	}

	public static bool ShouldCollect
	{
		get
		{
			return JSONLevelSerializer._collectionCount <= 0;
		}
	}

	static JSONLevelSerializer()
	{
		JSONLevelSerializer.allPrefabs = new Dictionary<string, GameObject>();
		JSONLevelSerializer.IgnoreTypes = new HashSet<string>();
		JSONLevelSerializer.CustomSerializers = new Dictionary<Type, IComponentSerializer>();
		JSONLevelSerializer.PlayerName = string.Empty;
		JSONLevelSerializer.SaveResumeInformation = true;
		JSONLevelSerializer.SerializationMode = JSONLevelSerializer.SerializationModes.CacheSerialization;
		JSONLevelSerializer.MaxGames = 20;
		JSONLevelSerializer.SavedGames = new Index<string, List<JSONLevelSerializer.SaveEntry>>();
		JSONLevelSerializer._stopCases = new List<Type>();
		JSONLevelSerializer.createdPlugins = new List<object>();
		JSONLevelSerializer.Guard = new object();
		JSONLevelSerializer.webClient = new WebClient();
		JSONLevelSerializer._collectionCount = 0;
		JSONLevelSerializer.Deserialized = delegate
		{
		};
		JSONLevelSerializer.GameSaved = delegate
		{
		};
		JSONLevelSerializer.SuspendingSerialization = delegate
		{
		};
		JSONLevelSerializer.ResumingSerialization = delegate
		{
		};
		JSONLevelSerializer.StoreComponent = delegate
		{
		};
		JSONLevelSerializer.Progress = delegate
		{
		};
		JSONLevelSerializer.webClient.UploadDataCompleted += new UploadDataCompletedEventHandler(JSONLevelSerializer.HandleWebClientUploadDataCompleted);
		JSONLevelSerializer.webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(JSONLevelSerializer.HandleWebClientUploadStringCompleted);
		JSONLevelSerializer._stopCases.Add(typeof(PrefabIdentifier));
		UnitySerializer.AddPrivateType(typeof(AnimationClip));
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Assembly assembly = assemblies[i];
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				JSONLevelSerializer.createdPlugins.Add(Activator.CreateInstance(tp));
			}, assembly, typeof(SerializerPlugIn));
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				JSONLevelSerializer.CustomSerializers[((ComponentSerializerFor)attr).SerializesType] = (Activator.CreateInstance(tp) as IComponentSerializer);
			}, assembly, typeof(ComponentSerializerFor));
		}
		JSONLevelSerializer.AllPrefabs = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(delegate(GameObject go)
		{
			PrefabIdentifier component = go.GetComponent<PrefabIdentifier>();
			return component != null && !component.IsInScene();
		}).Distinct(JSONLevelSerializer.CompareGameObjects.Instance).ToDictionary((GameObject go) => go.GetComponent<PrefabIdentifier>().ClassId, (GameObject go) => go);
		try
		{
			string @string = PlayerPrefs.GetString("JSON_Save_Game_Data_");
			if (!string.IsNullOrEmpty(@string))
			{
				JSONLevelSerializer.SavedGames = UnitySerializer.JSONDeserialize<global::Lookup<string, List<JSONLevelSerializer.SaveEntry>>>(@string);
			}
			if (JSONLevelSerializer.SavedGames == null)
			{
				JSONLevelSerializer.SavedGames = new Index<string, List<JSONLevelSerializer.SaveEntry>>();
			}
		}
		catch
		{
			JSONLevelSerializer.SavedGames = new Index<string, List<JSONLevelSerializer.SaveEntry>>();
		}
	}

	private static void HandleWebClientUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
	{
		object guard = JSONLevelSerializer.Guard;
		lock (guard)
		{
			JSONLevelSerializer.uploadCount--;
		}
		Loom.QueueOnMainThread(delegate
		{
			if (e.UserState is Action<Exception>)
			{
				(e.UserState as Action<Exception>)(e.Error);
			}
		});
	}

	private static void HandleWebClientUploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
	{
		object guard = JSONLevelSerializer.Guard;
		lock (guard)
		{
			JSONLevelSerializer.uploadCount--;
		}
		Loom.QueueOnMainThread(delegate
		{
			if (e.UserState is Action<Exception>)
			{
				(e.UserState as Action<Exception>)(e.Error);
			}
		});
	}

	public static void SaveObjectTreeToFile(string filename, GameObject rootOfTree)
	{
		string str = rootOfTree.SaveObjectTree();
		str.WriteToFile(Application.persistentDataPath + "/" + filename);
	}

	public static void LoadObjectTreeFromFile(string filename, Action<JSONLevelLoader> onComplete = null)
	{
		StreamReader streamReader = File.OpenText(Application.persistentDataPath + "/" + filename);
		string data = streamReader.ReadToEnd();
		streamReader.Close();
		JSONLevelSerializer.LoadObjectTree(data, onComplete);
	}

	public static void SerializeLevelToFile(string filename)
	{
		string str = JSONLevelSerializer.SerializeLevel();
		str.WriteToFile(Application.persistentDataPath + "/" + filename);
	}

	public static void LoadSavedLevelFromFile(string filename)
	{
		StreamReader streamReader = File.OpenText(Application.persistentDataPath + "/" + filename);
		string data = streamReader.ReadToEnd();
		streamReader.Close();
		JSONLevelSerializer.LoadSavedLevel(data);
	}

	public static void SaveObjectTreeToServer(string uri, GameObject rootOfTree, string userName = "", string password = "", Action<Exception> onComplete = null)
	{
		JSONLevelSerializer.<SaveObjectTreeToServer>c__AnonStorey2AF <SaveObjectTreeToServer>c__AnonStorey2AF = new JSONLevelSerializer.<SaveObjectTreeToServer>c__AnonStorey2AF();
		<SaveObjectTreeToServer>c__AnonStorey2AF.rootOfTree = rootOfTree;
		<SaveObjectTreeToServer>c__AnonStorey2AF.userName = userName;
		<SaveObjectTreeToServer>c__AnonStorey2AF.password = password;
		<SaveObjectTreeToServer>c__AnonStorey2AF.uri = uri;
		<SaveObjectTreeToServer>c__AnonStorey2AF.onComplete = onComplete;
		JSONLevelSerializer.<SaveObjectTreeToServer>c__AnonStorey2AF arg_55_0 = <SaveObjectTreeToServer>c__AnonStorey2AF;
		Action<Exception> arg_55_1;
		if ((arg_55_1 = <SaveObjectTreeToServer>c__AnonStorey2AF.onComplete) == null)
		{
			arg_55_1 = delegate
			{
			};
		}
		arg_55_0.onComplete = arg_55_1;
		Action action = delegate
		{
			string data = <SaveObjectTreeToServer>c__AnonStorey2AF.rootOfTree.SaveObjectTree();
			Action upload = delegate
			{
				JSONLevelSerializer.uploadCount++;
				JSONLevelSerializer.webClient.Credentials = new NetworkCredential(<SaveObjectTreeToServer>c__AnonStorey2AF.userName, <SaveObjectTreeToServer>c__AnonStorey2AF.password);
				JSONLevelSerializer.webClient.UploadStringAsync(new Uri(<SaveObjectTreeToServer>c__AnonStorey2AF.uri), null, data, <SaveObjectTreeToServer>c__AnonStorey2AF.onComplete);
			};
			JSONLevelSerializer.DoWhenReady(upload);
		};
		action();
	}

	private static void DoWhenReady(Action upload)
	{
		object guard = JSONLevelSerializer.Guard;
		lock (guard)
		{
			if (JSONLevelSerializer.uploadCount > 0)
			{
				Loom.QueueOnMainThread(delegate
				{
					JSONLevelSerializer.DoWhenReady(upload);
				}, 0.4f);
			}
			else
			{
				upload();
			}
		}
	}

	public static void LoadObjectTreeFromServer(string uri, Action<JSONLevelLoader> onComplete = null, Action<string> onError = null)
	{
		Action<JSONLevelLoader> arg_25_0;
		if ((arg_25_0 = onComplete) == null)
		{
			arg_25_0 = delegate
			{
			};
		}
		onComplete = arg_25_0;
		Action<string> arg_4C_0;
		if ((arg_4C_0 = onError) == null)
		{
			arg_4C_0 = delegate
			{
			};
		}
		onError = arg_4C_0;
		RadicalRoutineHelper.Current.StartCoroutine(JSONLevelSerializer.DownloadFromServer(uri, onComplete, onError));
	}

	public static void SerializeLevelToServer(string uri, string userName = "", string password = "", Action<Exception> onComplete = null)
	{
		JSONLevelSerializer.<SerializeLevelToServer>c__AnonStorey2B2 <SerializeLevelToServer>c__AnonStorey2B = new JSONLevelSerializer.<SerializeLevelToServer>c__AnonStorey2B2();
		<SerializeLevelToServer>c__AnonStorey2B.uri = uri;
		<SerializeLevelToServer>c__AnonStorey2B.userName = userName;
		<SerializeLevelToServer>c__AnonStorey2B.password = password;
		<SerializeLevelToServer>c__AnonStorey2B.onComplete = onComplete;
		object guard = JSONLevelSerializer.Guard;
		lock (guard)
		{
			if (JSONLevelSerializer.uploadCount > 0)
			{
				Loom.QueueOnMainThread(delegate
				{
					JSONLevelSerializer.SerializeLevelToServer(<SerializeLevelToServer>c__AnonStorey2B.uri, <SerializeLevelToServer>c__AnonStorey2B.userName, <SerializeLevelToServer>c__AnonStorey2B.password, <SerializeLevelToServer>c__AnonStorey2B.onComplete);
				}, 0.5f);
			}
			else
			{
				JSONLevelSerializer.uploadCount++;
				JSONLevelSerializer.<SerializeLevelToServer>c__AnonStorey2B2 arg_8B_0 = <SerializeLevelToServer>c__AnonStorey2B;
				Action<Exception> arg_8B_1;
				if ((arg_8B_1 = <SerializeLevelToServer>c__AnonStorey2B.onComplete) == null)
				{
					arg_8B_1 = delegate
					{
					};
				}
				arg_8B_0.onComplete = arg_8B_1;
				string data = JSONLevelSerializer.SerializeLevel();
				JSONLevelSerializer.webClient.Credentials = new NetworkCredential(<SerializeLevelToServer>c__AnonStorey2B.userName, <SerializeLevelToServer>c__AnonStorey2B.password);
				JSONLevelSerializer.webClient.UploadStringAsync(new Uri(<SerializeLevelToServer>c__AnonStorey2B.uri), null, data, <SerializeLevelToServer>c__AnonStorey2B.onComplete);
			}
		}
	}

	public static void LoadSavedLevelFromServer(string uri, Action<string> onError = null)
	{
		Action<string> arg_25_0;
		if ((arg_25_0 = onError) == null)
		{
			arg_25_0 = delegate
			{
			};
		}
		onError = arg_25_0;
		RadicalRoutineHelper.Current.StartCoroutine(JSONLevelSerializer.DownloadLevelFromServer(uri, onError));
	}

	[DebuggerHidden]
	private static IEnumerator DownloadFromServer(string uri, Action<JSONLevelLoader> onComplete, Action<string> onError)
	{
		JSONLevelSerializer.<DownloadFromServer>c__Iterator1C8 <DownloadFromServer>c__Iterator1C = new JSONLevelSerializer.<DownloadFromServer>c__Iterator1C8();
		<DownloadFromServer>c__Iterator1C.uri = uri;
		<DownloadFromServer>c__Iterator1C.onError = onError;
		<DownloadFromServer>c__Iterator1C.onComplete = onComplete;
		<DownloadFromServer>c__Iterator1C.<$>uri = uri;
		<DownloadFromServer>c__Iterator1C.<$>onError = onError;
		<DownloadFromServer>c__Iterator1C.<$>onComplete = onComplete;
		return <DownloadFromServer>c__Iterator1C;
	}

	[DebuggerHidden]
	private static IEnumerator DownloadLevelFromServer(string uri, Action<string> onError)
	{
		JSONLevelSerializer.<DownloadLevelFromServer>c__Iterator1C9 <DownloadLevelFromServer>c__Iterator1C = new JSONLevelSerializer.<DownloadLevelFromServer>c__Iterator1C9();
		<DownloadLevelFromServer>c__Iterator1C.uri = uri;
		<DownloadLevelFromServer>c__Iterator1C.onError = onError;
		<DownloadLevelFromServer>c__Iterator1C.<$>uri = uri;
		<DownloadLevelFromServer>c__Iterator1C.<$>onError = onError;
		return <DownloadLevelFromServer>c__Iterator1C;
	}

	internal static void InvokeDeserialized()
	{
		JSONLevelSerializer._suspensionCount = 0;
		if (JSONLevelSerializer.Deserialized != null)
		{
			JSONLevelSerializer.Deserialized();
		}
		foreach (GameObject current in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)).Cast<GameObject>())
		{
			current.SendMessage("OnDeserialized", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void Resume()
	{
		string @string = PlayerPrefs.GetString(JSONLevelSerializer.PlayerName + "JSON__RESUME__");
		if (!string.IsNullOrEmpty(@string))
		{
			JSONLevelSerializer.SaveEntry saveEntry = UnitySerializer.JSONDeserialize<JSONLevelSerializer.SaveEntry>(@string);
			saveEntry.Load();
		}
	}

	public static void Checkpoint()
	{
		JSONLevelSerializer.SaveGame("Resume", false, new Action<string, bool>(JSONLevelSerializer.PerformSaveCheckPoint));
	}

	private static void PerformSaveCheckPoint(string name, bool urgent)
	{
		JSONLevelSerializer.SaveEntry item = JSONLevelSerializer.CreateSaveEntry(name, urgent);
		PlayerPrefs.SetString(JSONLevelSerializer.PlayerName + "JSON__RESUME__", UnitySerializer.JSONSerialize(item));
		PlayerPrefs.Save();
	}

	public static void SuspendSerialization()
	{
		if (JSONLevelSerializer._suspensionCount == 0)
		{
			JSONLevelSerializer.SuspendingSerialization();
			if (JSONLevelSerializer.SerializationMode == JSONLevelSerializer.SerializationModes.CacheSerialization)
			{
				JSONLevelSerializer._cachedState = JSONLevelSerializer.CreateSaveEntry("resume", true);
				if (JSONLevelSerializer.SaveResumeInformation)
				{
					PlayerPrefs.SetString(JSONLevelSerializer.PlayerName + "JSON__RESUME__", UnitySerializer.JSONSerialize(JSONLevelSerializer._cachedState));
					PlayerPrefs.Save();
				}
			}
		}
		JSONLevelSerializer._suspensionCount++;
	}

	public static void ResumeSerialization()
	{
		JSONLevelSerializer._suspensionCount--;
		if (JSONLevelSerializer._suspensionCount == 0)
		{
			JSONLevelSerializer.ResumingSerialization();
		}
	}

	public static void IgnoreType(string typename)
	{
		JSONLevelSerializer.IgnoreTypes.Add(typename);
	}

	public static void UnIgnoreType(string typename)
	{
		JSONLevelSerializer.IgnoreTypes.Remove(typename);
	}

	public static void IgnoreType(Type tp)
	{
		if (tp.FullName != null)
		{
			JSONLevelSerializer.IgnoreTypes.Add(tp.FullName);
		}
	}

	public static JSONLevelSerializer.SaveEntry CreateSaveEntry(string name, bool urgent)
	{
		return new JSONLevelSerializer.SaveEntry
		{
			Name = name,
			When = DateTime.Now,
			Level = Application.loadedLevelName,
			Data = JSONLevelSerializer.SerializeLevel(urgent)
		};
	}

	public static void SaveGame(string name)
	{
		JSONLevelSerializer.SaveGame(name, false, null);
	}

	public static void SaveGame(string name, bool urgent, Action<string, bool> perform)
	{
		perform = (perform ?? new Action<string, bool>(JSONLevelSerializer.PerformSave));
		if (urgent || !JSONLevelSerializer.IsSuspended || JSONLevelSerializer.SerializationMode != JSONLevelSerializer.SerializationModes.SerializeWhenFree)
		{
			perform(name, urgent);
			return;
		}
		if (GameObject.Find("/SerializationHelper") != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("SerializationHelper");
		JSONLevelSerializer.SerializationHelper serializationHelper = gameObject.AddComponent(typeof(JSONLevelSerializer.SerializationHelper)) as JSONLevelSerializer.SerializationHelper;
		serializationHelper.gameName = name;
		serializationHelper.perform = perform;
	}

	private static void PerformSave(string name, bool urgent)
	{
		JSONLevelSerializer.SaveEntry item = JSONLevelSerializer.CreateSaveEntry(name, urgent);
		JSONLevelSerializer.SavedGames[JSONLevelSerializer.PlayerName].Insert(0, item);
		while (JSONLevelSerializer.SavedGames[JSONLevelSerializer.PlayerName].Count > JSONLevelSerializer.MaxGames)
		{
			JSONLevelSerializer.SavedGames[JSONLevelSerializer.PlayerName].RemoveAt(JSONLevelSerializer.SavedGames.Count - 1);
		}
		JSONLevelSerializer.SaveDataToPlayerPrefs();
		PlayerPrefs.SetString(JSONLevelSerializer.PlayerName + "JSON__RESUME__", UnitySerializer.JSONSerialize(item));
		PlayerPrefs.Save();
		JSONLevelSerializer.GameSaved();
	}

	public static void SaveDataToPlayerPrefs()
	{
		PlayerPrefs.SetString("JSON_Save_Game_Data_", UnitySerializer.JSONSerialize(JSONLevelSerializer.SavedGames));
		PlayerPrefs.Save();
	}

	public static void RegisterAssembly()
	{
		UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
		{
			JSONLevelSerializer.CustomSerializers[((ComponentSerializerFor)attr).SerializesType] = (Activator.CreateInstance(tp) as IComponentSerializer);
		}, Assembly.GetCallingAssembly(), typeof(ComponentSerializerFor));
	}

	public static void AddPrefabPath(string path)
	{
		foreach (KeyValuePair<string, GameObject> current in from pair in (from GameObject go in Resources.LoadAll(path, typeof(GameObject))
		where go.GetComponent<UniqueIdentifier>() != null
		select go).ToDictionary((GameObject go) => go.GetComponent<UniqueIdentifier>().ClassId, (GameObject go) => go)
		where !JSONLevelSerializer.AllPrefabs.ContainsKey(pair.Key)
		select pair)
		{
			JSONLevelSerializer.AllPrefabs.Add(current.Key, current.Value);
		}
	}

	public static void DontCollect()
	{
		JSONLevelSerializer._collectionCount++;
	}

	public static void Collect()
	{
		JSONLevelSerializer._collectionCount--;
	}

	public static string SerializeLevel()
	{
		return JSONLevelSerializer.SerializeLevel(false);
	}

	public static string SerializeLevel(bool urgent)
	{
		if (!JSONLevelSerializer.IsSuspended || urgent)
		{
			Resources.UnloadUnusedAssets();
			if (JSONLevelSerializer.ShouldCollect)
			{
				GC.Collect();
			}
			string result = JSONLevelSerializer.SerializeLevel(false, null);
			if (JSONLevelSerializer.ShouldCollect)
			{
				GC.Collect();
			}
			return result;
		}
		if (JSONLevelSerializer.SerializationMode == JSONLevelSerializer.SerializationModes.CacheSerialization)
		{
			return JSONLevelSerializer._cachedState.Data;
		}
		throw new JSONLevelSerializer.SerializationSuspendedException();
	}

	public static void RaiseProgress(string section, float complete)
	{
		JSONLevelSerializer.Progress(section, complete);
	}

	public static bool HasParent(UniqueIdentifier i, string id)
	{
		Transform transform = UniqueIdentifier.GetByName(i.Id).transform;
		while (transform != null)
		{
			UniqueIdentifier component;
			if ((component = transform.GetComponent<UniqueIdentifier>()) != null && id == component.Id)
			{
				return true;
			}
			transform = transform.parent;
		}
		return false;
	}

	private static void GetComponentsInChildrenWithClause(Transform t, List<StoreInformation> components)
	{
		foreach (Transform current in t.Cast<Transform>())
		{
			StoreInformation component = current.GetComponent<StoreInformation>();
			if (component != null)
			{
				if (!(component is PrefabIdentifier))
				{
					components.Add(component);
					JSONLevelSerializer.GetComponentsInChildrenWithClause(current, components);
				}
			}
			else
			{
				JSONLevelSerializer.GetComponentsInChildrenWithClause(current, components);
			}
		}
	}

	public static string SaveObjectTree(this GameObject rootOfTree)
	{
		if (!rootOfTree.GetComponent<UniqueIdentifier>())
		{
			EmptyObjectIdentifier.FlagAll(rootOfTree);
		}
		return JSONLevelSerializer.SerializeLevel(false, rootOfTree.GetComponent<UniqueIdentifier>().Id);
	}

	public static void LoadObjectTree(string data, Action<JSONLevelLoader> onComplete = null)
	{
		Action<JSONLevelLoader> arg_25_0;
		if ((arg_25_0 = onComplete) == null)
		{
			arg_25_0 = delegate
			{
			};
		}
		onComplete = arg_25_0;
		JSONLevelSerializer.LoadNow(data, true, false, onComplete);
	}

	public static List<StoreInformation> GetComponentsInChildrenWithClause(GameObject go)
	{
		List<StoreInformation> list = new List<StoreInformation>();
		JSONLevelSerializer.GetComponentsInChildrenWithClause(go.transform, list);
		return list;
	}

	public static string SerializeLevel(bool urgent, string id)
	{
		JSONLevelSerializer.LevelData levelData;
		using (new Radical.Logging())
		{
			using (new UnitySerializer.ForceJSON())
			{
				using (new UnitySerializer.SerializationScope())
				{
					levelData = new JSONLevelSerializer.LevelData
					{
						Name = Application.loadedLevelName
					};
					levelData.StoredObjectNames = (from si in (from i in UniqueIdentifier.AllIdentifiers
					where string.IsNullOrEmpty(id) || i.Id == id || JSONLevelSerializer.HasParent(i, id)
					select i.gameObject into go
					where go != null
					select go).Where(delegate(GameObject go)
					{
						IControlSerializationEx controlSerializationEx = go.FindInterface<IControlSerializationEx>();
						return controlSerializationEx == null || controlSerializationEx.ShouldSaveWholeObject();
					}).Where(delegate(GameObject go)
					{
						if (JSONLevelSerializer.Store == null)
						{
							return true;
						}
						bool result = true;
						JSONLevelSerializer.Store(go, ref result);
						return result;
					}).Select(delegate(GameObject n)
					{
						JSONLevelSerializer.StoredItem result;
						try
						{
							JSONLevelSerializer.StoredItem storedItem = new JSONLevelSerializer.StoredItem();
							storedItem.createEmptyObject = (n.GetComponent<EmptyObjectIdentifier>() != null);
							storedItem.Active = n.active;
							storedItem.layer = n.layer;
							storedItem.tag = n.tag;
							storedItem.setExtraData = true;
							storedItem.Components = (from c in n.GetComponents<Component>()
							where c != null
							select c.GetType().FullName).Distinct<string>().ToDictionary((string v) => v, (string v) => true);
							storedItem.Name = n.GetComponent<UniqueIdentifier>().Id;
							storedItem.GameObjectName = n.name;
							storedItem.ParentName = ((!(n.transform.parent == null) && !(n.transform.parent.GetComponent<UniqueIdentifier>() == null)) ? n.transform.parent.GetComponent<UniqueIdentifier>().Id : null);
							storedItem.ClassId = ((!(n.GetComponent<PrefabIdentifier>() != null)) ? string.Empty : n.GetComponent<PrefabIdentifier>().ClassId);
							JSONLevelSerializer.StoredItem storedItem2 = storedItem;
							if (n.GetComponent<StoreInformation>())
							{
								n.SendMessage("OnSerializing", SendMessageOptions.DontRequireReceiver);
							}
							PrefabIdentifier component = n.GetComponent<PrefabIdentifier>();
							if (component != null)
							{
								List<StoreInformation> componentsInChildrenWithClause = JSONLevelSerializer.GetComponentsInChildrenWithClause(n);
								storedItem2.Children = (from c in componentsInChildrenWithClause
								group c by c.ClassId).ToDictionary((IGrouping<string, StoreInformation> c) => c.Key, (IGrouping<string, StoreInformation> c) => (from i in c
								select i.Id).ToList<string>());
							}
							result = storedItem2;
						}
						catch (Exception ex)
						{
							UnityEngine.Debug.LogWarning("Failed to serialize status of " + n.name + " with error " + ex.ToString());
							result = null;
						}
						return result;
					})
					where si != null
					select si).ToList<JSONLevelSerializer.StoredItem>();
					List<<>__AnonType4<StoreInformation, Component>> toBeProcessed = (from c in (from o in UniqueIdentifier.AllIdentifiers
					where o.GetComponent<StoreInformation>() != null || o.GetComponent<PrefabIdentifier>() != null
					select o into i
					where string.IsNullOrEmpty(id) || i.Id == id || JSONLevelSerializer.HasParent(i, id)
					where i != null
					select i.gameObject into i
					where i != null
					select i).Where(delegate(GameObject go)
					{
						IControlSerializationEx controlSerializationEx = go.FindInterface<IControlSerializationEx>();
						return controlSerializationEx == null || controlSerializationEx.ShouldSaveWholeObject();
					}).Distinct<GameObject>().Where(delegate(GameObject go)
					{
						if (JSONLevelSerializer.Store == null)
						{
							return true;
						}
						bool result = true;
						JSONLevelSerializer.Store(go, ref result);
						return result;
					}).SelectMany((GameObject o) => o.GetComponents<Component>()).Where(delegate(Component c)
					{
						if (c == null)
						{
							return false;
						}
						Type type = c.GetType();
						bool flag = true;
						JSONLevelSerializer.StoreComponent(c, ref flag);
						return flag && (!(c is IControlSerialization) || (c as IControlSerialization).ShouldSave()) && !type.IsDefined(typeof(DontStoreAttribute), true) && !JSONLevelSerializer.IgnoreTypes.Contains(type.FullName);
					})
					select new
					{
						Identifier = (StoreInformation)c.gameObject.GetComponent(typeof(StoreInformation)),
						Component = c
					} into cp
					where cp.Identifier.StoreAllComponents || cp.Identifier.Components.Contains(cp.Component.GetType().FullName)
					orderby cp.Identifier.Id, cp.Component.GetType().FullName
					select cp).ToList();
					int processed = 0;
					levelData.StoredItems = (from s in toBeProcessed.Select(delegate(cp)
					{
						JSONLevelSerializer.StoredData result;
						try
						{
							if (Radical.IsLogging())
							{
								Radical.Log("<{0} : {1} - {2}>", new object[]
								{
									cp.Component.gameObject.GetFullName(),
									cp.Component.GetType().Name,
									cp.Component.GetComponent<UniqueIdentifier>().Id
								});
								Radical.IndentLog();
							}
							JSONLevelSerializer.StoredData storedData = new JSONLevelSerializer.StoredData
							{
								Type = cp.Component.GetType().FullName,
								ClassId = cp.Identifier.ClassId,
								Name = cp.Component.GetComponent<UniqueIdentifier>().Id
							};
							if (JSONLevelSerializer.CustomSerializers.ContainsKey(cp.Component.GetType()))
							{
								storedData.Data = UnitySerializer.TextEncoding.GetString(JSONLevelSerializer.CustomSerializers[cp.Component.GetType()].Serialize(cp.Component));
							}
							else
							{
								storedData.Data = UnitySerializer.JSONSerializeForDeserializeInto(cp.Component);
							}
							if (Radical.IsLogging())
							{
								Radical.OutdentLog();
								Radical.Log("</{0} : {1}>", new object[]
								{
									cp.Component.gameObject.GetFullName(),
									cp.Component.GetType().Name
								});
							}
							processed++;
							JSONLevelSerializer.Progress("Storing", (float)processed / (float)toBeProcessed.Count);
							result = storedData;
						}
						catch (Exception ex)
						{
							processed++;
							UnityEngine.Debug.LogWarning(string.Concat(new string[]
							{
								"Failed to serialize data (",
								cp.Component.GetType().AssemblyQualifiedName,
								") of ",
								cp.Component.name,
								" with error ",
								ex.ToString()
							}));
							result = null;
						}
						return result;
					})
					where s != null
					select s).ToList<JSONLevelSerializer.StoredData>();
				}
			}
		}
		return UnitySerializer.JSONSerialize(levelData);
	}

	public static void LoadNow(string data)
	{
		JSONLevelSerializer.LoadNow(data, false, true, null);
	}

	public static void LoadNow(string data, bool dontDeleteExistingItems)
	{
		JSONLevelSerializer.LoadNow(data, dontDeleteExistingItems, true, null);
	}

	public static void LoadNow(string data, bool dontDeleteExistingItems, bool showLoadingGUI)
	{
		JSONLevelSerializer.LoadNow(data, dontDeleteExistingItems, showLoadingGUI, null);
	}

	public static void LoadNow(string data, bool dontDeleteExistingItems, bool showLoadingGUI, Action<JSONLevelLoader> complete)
	{
		if (data == null)
		{
			throw new ArgumentException("data parameter must be provided");
		}
		GameObject gameObject = new GameObject();
		JSONLevelLoader jSONLevelLoader = gameObject.AddComponent<JSONLevelLoader>();
		jSONLevelLoader.showGUI = showLoadingGUI;
		JSONLevelSerializer.LevelData data2 = UnitySerializer.JSONDeserialize<JSONLevelSerializer.LevelData>(data);
		jSONLevelLoader.Data = data2;
		jSONLevelLoader.DontDelete = dontDeleteExistingItems;
		jSONLevelLoader.StartCoroutine(JSONLevelSerializer.PerformLoad(jSONLevelLoader, complete));
	}

	[DebuggerHidden]
	private static IEnumerator PerformLoad(JSONLevelLoader loader, Action<JSONLevelLoader> complete)
	{
		JSONLevelSerializer.<PerformLoad>c__Iterator1CA <PerformLoad>c__Iterator1CA = new JSONLevelSerializer.<PerformLoad>c__Iterator1CA();
		<PerformLoad>c__Iterator1CA.loader = loader;
		<PerformLoad>c__Iterator1CA.complete = complete;
		<PerformLoad>c__Iterator1CA.<$>loader = loader;
		<PerformLoad>c__Iterator1CA.<$>complete = complete;
		return <PerformLoad>c__Iterator1CA;
	}

	public static JSONLevelLoader LoadSavedLevel(string data)
	{
		JSONLevelSerializer.IsDeserializing = true;
		LevelSerializer.IsDeserializing = true;
		SaveGameManager.Loaded();
		GameObject gameObject = new GameObject();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		JSONLevelLoader jSONLevelLoader = gameObject.AddComponent<JSONLevelLoader>();
		jSONLevelLoader.Data = UnitySerializer.JSONDeserialize<JSONLevelSerializer.LevelData>(UnitySerializer.UnEscape(data));
		Application.LoadLevel(jSONLevelLoader.Data.Name);
		return jSONLevelLoader;
	}
}
