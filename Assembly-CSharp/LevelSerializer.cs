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

public static class LevelSerializer
{
	public enum SerializationModes
	{
		SerializeWhenFree,
		CacheSerialization
	}

	private class CompareGameObjects : IEqualityComparer<GameObject>
	{
		public static readonly LevelSerializer.CompareGameObjects Instance = new LevelSerializer.CompareGameObjects();

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

		public List<LevelSerializer.StoredData> StoredItems;

		public List<LevelSerializer.StoredItem> StoredObjectNames;

		public string rootObject;
	}

	private class ProgressHelper
	{
		public void SetProgress(long inSize, long outSize)
		{
			LevelSerializer.RaiseProgress("Compression", 5f);
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
			UnitySerializer.DeserializeInto(Convert.FromBase64String(contents), this);
		}

		public SaveEntry()
		{
		}

		public void Load()
		{
			LevelSerializer.LoadSavedLevel(this.Data);
		}

		public void Delete()
		{
			KeyValuePair<string, List<LevelSerializer.SaveEntry>> keyValuePair = LevelSerializer.SavedGames.FirstOrDefault((KeyValuePair<string, List<LevelSerializer.SaveEntry>> p) => p.Value.Contains(this));
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.Remove(this);
				LevelSerializer.SaveDataToPlayerPrefs();
			}
		}

		public override string ToString()
		{
			return Convert.ToBase64String(UnitySerializer.Serialize(this));
		}
	}

	public class SerializationHelper : MonoBehaviour
	{
		public string gameName;

		public Action<string, bool> perform;

		private void Update()
		{
			if (!LevelSerializer.IsSuspended)
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
		public SerializationSuspendedException() : base("Serialization was suspended: " + LevelSerializer._suspensionCount + " times")
		{
		}
	}

	public class StoredData
	{
		public string ClassId;

		public byte[] Data;

		public string Name;

		public string Type;
	}

	public class StoredItem
	{
		public bool Active;

		public int layer;

		public string tag;

		public bool setExtraData;

		public readonly List<string> ChildIds = new List<string>();

		public Dictionary<string, List<string>> Children = new Dictionary<string, List<string>>();

		public string ClassId;

		public Dictionary<string, bool> Components;

		[DoNotSerialize]
		public GameObject GameObject;

		public string GameObjectName;

		public string Name;

		public string ParentName;

		public bool createEmptyObject;

		public override string ToString()
		{
			return string.Format("{0}  child of {2} - ({1})", this.Name, this.ClassId, this.ParentName);
		}
	}

	public delegate void StoreQuery(GameObject go, ref bool store);

	public delegate void StoreComponentQuery(Component component, ref bool store);

	private static Dictionary<string, GameObject> allPrefabs;

	public static HashSet<string> IgnoreTypes;

	internal static Dictionary<Type, IComponentSerializer> CustomSerializers;

	internal static int lastFrame;

	public static string PlayerName;

	public static bool SaveResumeInformation;

	private static int _suspensionCount;

	private static LevelSerializer.SaveEntry _cachedState;

	public static LevelSerializer.SerializationModes SerializationMode;

	public static int MaxGames;

	public static global::Lookup<string, List<LevelSerializer.SaveEntry>> SavedGames;

	private static readonly List<Type> _stopCases;

	public static bool IsDeserializing;

	private static readonly List<object> createdPlugins;

	public static bool useCompression;

	private static WebClient webClient;

	private static readonly object Guard;

	private static int uploadCount;

	private static int _collectionCount;

	public static AsyncOperation LevelLoadingOperation;

	public static event Action Deserialized
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelSerializer.Deserialized = (Action)Delegate.Combine(LevelSerializer.Deserialized, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelSerializer.Deserialized = (Action)Delegate.Remove(LevelSerializer.Deserialized, value);
		}
	}

	public static event Action GameSaved
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelSerializer.GameSaved = (Action)Delegate.Combine(LevelSerializer.GameSaved, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelSerializer.GameSaved = (Action)Delegate.Remove(LevelSerializer.GameSaved, value);
		}
	}

	public static event Action SuspendingSerialization
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelSerializer.SuspendingSerialization = (Action)Delegate.Combine(LevelSerializer.SuspendingSerialization, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelSerializer.SuspendingSerialization = (Action)Delegate.Remove(LevelSerializer.SuspendingSerialization, value);
		}
	}

	public static event Action ResumingSerialization
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelSerializer.ResumingSerialization = (Action)Delegate.Combine(LevelSerializer.ResumingSerialization, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelSerializer.ResumingSerialization = (Action)Delegate.Remove(LevelSerializer.ResumingSerialization, value);
		}
	}

	public static event LevelSerializer.StoreQuery Store
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelSerializer.Store = (LevelSerializer.StoreQuery)Delegate.Combine(LevelSerializer.Store, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelSerializer.Store = (LevelSerializer.StoreQuery)Delegate.Remove(LevelSerializer.Store, value);
		}
	}

	public static event LevelSerializer.StoreComponentQuery StoreComponent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelSerializer.StoreComponent = (LevelSerializer.StoreComponentQuery)Delegate.Combine(LevelSerializer.StoreComponent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelSerializer.StoreComponent = (LevelSerializer.StoreComponentQuery)Delegate.Remove(LevelSerializer.StoreComponent, value);
		}
	}

	public static event Action<string, float> Progress
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelSerializer.Progress = (Action<string, float>)Delegate.Combine(LevelSerializer.Progress, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelSerializer.Progress = (Action<string, float>)Delegate.Remove(LevelSerializer.Progress, value);
		}
	}

	internal static Dictionary<string, GameObject> AllPrefabs
	{
		get
		{
			if (Time.frameCount != LevelSerializer.lastFrame)
			{
				LevelSerializer.allPrefabs = (from p in LevelSerializer.allPrefabs
				where p.Value
				select p).ToDictionary((KeyValuePair<string, GameObject> p) => p.Key, (KeyValuePair<string, GameObject> p) => p.Value);
				LevelSerializer.lastFrame = Time.frameCount;
			}
			return LevelSerializer.allPrefabs;
		}
		set
		{
			LevelSerializer.allPrefabs = value;
		}
	}

	public static bool CanResume
	{
		get
		{
			return PlayerPrefsFile.KeyExist(LevelSerializer.PlayerName + "__RESUME__");
		}
	}

	public static bool IsSuspended
	{
		get
		{
			return LevelSerializer._suspensionCount > 0;
		}
	}

	public static int SuspensionCount
	{
		get
		{
			return LevelSerializer._suspensionCount;
		}
	}

	public static bool ShouldCollect
	{
		get
		{
			return LevelSerializer._collectionCount <= 0;
		}
	}

	static LevelSerializer()
	{
		LevelSerializer.allPrefabs = new Dictionary<string, GameObject>();
		LevelSerializer.IgnoreTypes = new HashSet<string>();
		LevelSerializer.CustomSerializers = new Dictionary<Type, IComponentSerializer>();
		LevelSerializer.PlayerName = string.Empty;
		LevelSerializer.SaveResumeInformation = true;
		LevelSerializer.SerializationMode = LevelSerializer.SerializationModes.CacheSerialization;
		LevelSerializer.MaxGames = 20;
		LevelSerializer.SavedGames = new Index<string, List<LevelSerializer.SaveEntry>>();
		LevelSerializer._stopCases = new List<Type>();
		LevelSerializer.createdPlugins = new List<object>();
		LevelSerializer.useCompression = false;
		LevelSerializer.webClient = new WebClient();
		LevelSerializer.Guard = new object();
		LevelSerializer._collectionCount = 0;
		LevelSerializer.Deserialized = delegate
		{
		};
		LevelSerializer.GameSaved = delegate
		{
		};
		LevelSerializer.SuspendingSerialization = delegate
		{
		};
		LevelSerializer.ResumingSerialization = delegate
		{
		};
		LevelSerializer.StoreComponent = delegate
		{
		};
		LevelSerializer.Progress = delegate
		{
		};
		LevelSerializer.webClient.UploadDataCompleted += new UploadDataCompletedEventHandler(LevelSerializer.HandleWebClientUploadDataCompleted);
		LevelSerializer.webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(LevelSerializer.HandleWebClientUploadStringCompleted);
		LevelSerializer._stopCases.Add(typeof(PrefabIdentifier));
		UnitySerializer.AddPrivateType(typeof(AnimationClip));
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Assembly assembly = assemblies[i];
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				LevelSerializer.createdPlugins.Add(Activator.CreateInstance(tp));
			}, assembly, typeof(SerializerPlugIn));
			UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
			{
				LevelSerializer.CustomSerializers[((ComponentSerializerFor)attr).SerializesType] = (Activator.CreateInstance(tp) as IComponentSerializer);
			}, assembly, typeof(ComponentSerializerFor));
		}
		LevelSerializer.AllPrefabs = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(delegate(GameObject go)
		{
			PrefabIdentifier component = go.GetComponent<PrefabIdentifier>();
			return component != null && !component.IsInScene();
		}).Distinct(LevelSerializer.CompareGameObjects.Instance).ToDictionary((GameObject go) => go.GetComponent<PrefabIdentifier>().ClassId, (GameObject go) => go);
		try
		{
			string @string = PlayerPrefsFile.GetString("_Save_Game_Data_", string.Empty, true);
			if (!string.IsNullOrEmpty(@string))
			{
				try
				{
					LevelSerializer.SavedGames = UnitySerializer.Deserialize<global::Lookup<string, List<LevelSerializer.SaveEntry>>>(Convert.FromBase64String(@string));
				}
				catch
				{
					LevelSerializer.SavedGames = null;
				}
			}
			if (LevelSerializer.SavedGames == null)
			{
				LevelSerializer.SavedGames = new Index<string, List<LevelSerializer.SaveEntry>>();
				LevelSerializer.SaveDataToPlayerPrefs();
			}
		}
		catch
		{
			LevelSerializer.SavedGames = new Index<string, List<LevelSerializer.SaveEntry>>();
		}
	}

	private static void HandleWebClientUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
	{
		object guard = LevelSerializer.Guard;
		lock (guard)
		{
			LevelSerializer.uploadCount--;
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
		object guard = LevelSerializer.Guard;
		lock (guard)
		{
			LevelSerializer.uploadCount--;
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
		byte[] data = rootOfTree.SaveObjectTree();
		data.WriteToFile(Application.persistentDataPath + "/" + filename);
	}

	public static void LoadObjectTreeFromFile(string filename, Action<LevelLoader> onComplete = null)
	{
		FileStream fileStream = File.Open(Application.persistentDataPath + "/" + filename, FileMode.Open);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, (int)fileStream.Length);
		fileStream.Close();
		array.LoadObjectTree(onComplete);
	}

	public static void SerializeLevelToFile(string filename)
	{
		string str = LevelSerializer.SerializeLevel();
		str.WriteToFile(Application.persistentDataPath + "/" + filename);
	}

	public static void LoadSavedLevelFromFile(string filename)
	{
		StreamReader streamReader = File.OpenText(Application.persistentDataPath + "/" + filename);
		string data = streamReader.ReadToEnd();
		streamReader.Close();
		LevelSerializer.LoadSavedLevel(data);
	}

	public static void SaveObjectTreeToServer(string uri, GameObject rootOfTree, string userName = "", string password = "", Action<Exception> onComplete = null)
	{
		LevelSerializer.<SaveObjectTreeToServer>c__AnonStorey2C2 <SaveObjectTreeToServer>c__AnonStorey2C = new LevelSerializer.<SaveObjectTreeToServer>c__AnonStorey2C2();
		<SaveObjectTreeToServer>c__AnonStorey2C.rootOfTree = rootOfTree;
		<SaveObjectTreeToServer>c__AnonStorey2C.userName = userName;
		<SaveObjectTreeToServer>c__AnonStorey2C.password = password;
		<SaveObjectTreeToServer>c__AnonStorey2C.uri = uri;
		<SaveObjectTreeToServer>c__AnonStorey2C.onComplete = onComplete;
		LevelSerializer.<SaveObjectTreeToServer>c__AnonStorey2C2 arg_55_0 = <SaveObjectTreeToServer>c__AnonStorey2C;
		Action<Exception> arg_55_1;
		if ((arg_55_1 = <SaveObjectTreeToServer>c__AnonStorey2C.onComplete) == null)
		{
			arg_55_1 = delegate
			{
			};
		}
		arg_55_0.onComplete = arg_55_1;
		Action action = delegate
		{
			byte[] data = <SaveObjectTreeToServer>c__AnonStorey2C.rootOfTree.SaveObjectTree();
			Action upload = delegate
			{
				LevelSerializer.uploadCount++;
				LevelSerializer.webClient.Credentials = new NetworkCredential(<SaveObjectTreeToServer>c__AnonStorey2C.userName, <SaveObjectTreeToServer>c__AnonStorey2C.password);
				LevelSerializer.webClient.UploadDataAsync(new Uri(<SaveObjectTreeToServer>c__AnonStorey2C.uri), null, data, <SaveObjectTreeToServer>c__AnonStorey2C.onComplete);
			};
			LevelSerializer.DoWhenReady(upload);
		};
		action();
	}

	private static void DoWhenReady(Action upload)
	{
		object guard = LevelSerializer.Guard;
		lock (guard)
		{
			if (LevelSerializer.uploadCount > 0)
			{
				Loom.QueueOnMainThread(delegate
				{
					LevelSerializer.DoWhenReady(upload);
				}, 0.4f);
			}
			else
			{
				upload();
			}
		}
	}

	public static void LoadObjectTreeFromServer(string uri, Action<LevelLoader> onComplete = null)
	{
		Action<LevelLoader> arg_25_0;
		if ((arg_25_0 = onComplete) == null)
		{
			arg_25_0 = delegate
			{
			};
		}
		onComplete = arg_25_0;
		RadicalRoutineHelper.Current.StartCoroutine(LevelSerializer.DownloadFromServer(uri, onComplete));
	}

	public static void SerializeLevelToServer(string uri, string userName = "", string password = "", Action<Exception> onComplete = null)
	{
		LevelSerializer.<SerializeLevelToServer>c__AnonStorey2C5 <SerializeLevelToServer>c__AnonStorey2C = new LevelSerializer.<SerializeLevelToServer>c__AnonStorey2C5();
		<SerializeLevelToServer>c__AnonStorey2C.uri = uri;
		<SerializeLevelToServer>c__AnonStorey2C.userName = userName;
		<SerializeLevelToServer>c__AnonStorey2C.password = password;
		<SerializeLevelToServer>c__AnonStorey2C.onComplete = onComplete;
		object guard = LevelSerializer.Guard;
		lock (guard)
		{
			if (LevelSerializer.uploadCount > 0)
			{
				Loom.QueueOnMainThread(delegate
				{
					LevelSerializer.SerializeLevelToServer(<SerializeLevelToServer>c__AnonStorey2C.uri, <SerializeLevelToServer>c__AnonStorey2C.userName, <SerializeLevelToServer>c__AnonStorey2C.password, <SerializeLevelToServer>c__AnonStorey2C.onComplete);
				}, 0.5f);
			}
			else
			{
				LevelSerializer.uploadCount++;
				LevelSerializer.<SerializeLevelToServer>c__AnonStorey2C5 arg_8B_0 = <SerializeLevelToServer>c__AnonStorey2C;
				Action<Exception> arg_8B_1;
				if ((arg_8B_1 = <SerializeLevelToServer>c__AnonStorey2C.onComplete) == null)
				{
					arg_8B_1 = delegate
					{
					};
				}
				arg_8B_0.onComplete = arg_8B_1;
				string data = LevelSerializer.SerializeLevel();
				LevelSerializer.webClient.Credentials = new NetworkCredential(<SerializeLevelToServer>c__AnonStorey2C.userName, <SerializeLevelToServer>c__AnonStorey2C.password);
				LevelSerializer.webClient.UploadStringAsync(new Uri(<SerializeLevelToServer>c__AnonStorey2C.uri), null, data, <SerializeLevelToServer>c__AnonStorey2C.onComplete);
			}
		}
	}

	public static void LoadSavedLevelFromServer(string uri)
	{
		RadicalRoutineHelper.Current.StartCoroutine(LevelSerializer.DownloadLevelFromServer(uri));
	}

	[DebuggerHidden]
	private static IEnumerator DownloadFromServer(string uri, Action<LevelLoader> onComplete)
	{
		LevelSerializer.<DownloadFromServer>c__Iterator1D6 <DownloadFromServer>c__Iterator1D = new LevelSerializer.<DownloadFromServer>c__Iterator1D6();
		<DownloadFromServer>c__Iterator1D.uri = uri;
		<DownloadFromServer>c__Iterator1D.onComplete = onComplete;
		<DownloadFromServer>c__Iterator1D.<$>uri = uri;
		<DownloadFromServer>c__Iterator1D.<$>onComplete = onComplete;
		return <DownloadFromServer>c__Iterator1D;
	}

	[DebuggerHidden]
	private static IEnumerator DownloadLevelFromServer(string uri)
	{
		LevelSerializer.<DownloadLevelFromServer>c__Iterator1D7 <DownloadLevelFromServer>c__Iterator1D = new LevelSerializer.<DownloadLevelFromServer>c__Iterator1D7();
		<DownloadLevelFromServer>c__Iterator1D.uri = uri;
		<DownloadLevelFromServer>c__Iterator1D.<$>uri = uri;
		return <DownloadLevelFromServer>c__Iterator1D;
	}

	internal static void InvokeDeserialized()
	{
		LevelSerializer._suspensionCount = 0;
		if (LevelSerializer.Deserialized != null)
		{
			LevelSerializer.Deserialized();
		}
		foreach (GameObject current in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)).Cast<GameObject>())
		{
			current.SendMessage("OnDeserialized", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void Resume()
	{
		string @string = PlayerPrefsFile.GetString(LevelSerializer.PlayerName + "__RESUME__", string.Empty, true);
		if (!string.IsNullOrEmpty(@string))
		{
			LevelSerializer.SaveEntry saveEntry = UnitySerializer.Deserialize<LevelSerializer.SaveEntry>(Convert.FromBase64String(@string));
			saveEntry.Load();
		}
	}

	public static void Checkpoint()
	{
		LevelSerializer.SaveGame("Resume", false, new Action<string, bool>(LevelSerializer.PerformSaveCheckPoint));
	}

	private static void PerformSaveCheckPoint(string name, bool urgent)
	{
		LevelSerializer.SaveEntry item = LevelSerializer.CreateSaveEntry(name, urgent);
		PlayerPrefsFile.SetString(LevelSerializer.PlayerName + "__RESUME__", Convert.ToBase64String(UnitySerializer.Serialize(item)), true);
		PlayerPrefsFile.Save();
	}

	public static void SuspendSerialization()
	{
		if (LevelSerializer._suspensionCount == 0)
		{
			LevelSerializer.SuspendingSerialization();
			if (LevelSerializer.SerializationMode == LevelSerializer.SerializationModes.CacheSerialization)
			{
				LevelSerializer._cachedState = LevelSerializer.CreateSaveEntry("resume", true);
				if (LevelSerializer.SaveResumeInformation)
				{
					PlayerPrefsFile.SetString(LevelSerializer.PlayerName + "__RESUME__", Convert.ToBase64String(UnitySerializer.Serialize(LevelSerializer._cachedState)), true);
					PlayerPrefsFile.Save();
				}
			}
		}
		LevelSerializer._suspensionCount++;
	}

	public static void ResumeSerialization()
	{
		LevelSerializer._suspensionCount--;
		if (LevelSerializer._suspensionCount == 0)
		{
			LevelSerializer.ResumingSerialization();
		}
	}

	public static void IgnoreType(string typename)
	{
		LevelSerializer.IgnoreTypes.Add(typename);
	}

	public static void UnIgnoreType(string typename)
	{
		LevelSerializer.IgnoreTypes.Remove(typename);
	}

	public static void IgnoreType(Type tp)
	{
		if (tp.FullName != null)
		{
			LevelSerializer.IgnoreTypes.Add(tp.FullName);
		}
	}

	public static LevelSerializer.SaveEntry CreateSaveEntry(string name, bool urgent)
	{
		return new LevelSerializer.SaveEntry
		{
			Name = name,
			When = DateTime.Now,
			Level = Application.loadedLevelName,
			Data = LevelSerializer.SerializeLevel(urgent)
		};
	}

	public static void SaveGame(string name)
	{
		LevelSerializer.SaveGame(name, false, null);
	}

	public static void SaveGame(string name, bool urgent, Action<string, bool> perform)
	{
		perform = (perform ?? new Action<string, bool>(LevelSerializer.PerformSave));
		if (urgent || !LevelSerializer.IsSuspended || LevelSerializer.SerializationMode != LevelSerializer.SerializationModes.SerializeWhenFree)
		{
			perform(name, urgent);
			return;
		}
		if (GameObject.Find("/SerializationHelper") != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("SerializationHelper");
		LevelSerializer.SerializationHelper serializationHelper = gameObject.AddComponent(typeof(LevelSerializer.SerializationHelper)) as LevelSerializer.SerializationHelper;
		serializationHelper.gameName = name;
		serializationHelper.perform = perform;
	}

	private static void PerformSave(string name, bool urgent)
	{
		LevelSerializer.SaveEntry item = LevelSerializer.CreateSaveEntry(name, urgent);
		LevelSerializer.SavedGames[LevelSerializer.PlayerName].Insert(0, item);
		while (LevelSerializer.SavedGames[LevelSerializer.PlayerName].Count > LevelSerializer.MaxGames)
		{
			LevelSerializer.SavedGames[LevelSerializer.PlayerName].RemoveAt(LevelSerializer.SavedGames.Count - 1);
		}
		LevelSerializer.SaveDataToPlayerPrefs();
		PlayerPrefsFile.SetString(LevelSerializer.PlayerName + "__RESUME__", Convert.ToBase64String(UnitySerializer.Serialize(item)), true);
		PlayerPrefsFile.Save();
		LevelSerializer.GameSaved();
	}

	public static void SaveDataToPlayerPrefs()
	{
		PlayerPrefsFile.SetString("_Save_Game_Data_", Convert.ToBase64String(UnitySerializer.Serialize(LevelSerializer.SavedGames)), true);
		PlayerPrefsFile.Save();
	}

	public static void RegisterAssembly()
	{
		UnitySerializer.ScanAllTypesForAttribute(delegate(Type tp, Attribute attr)
		{
			LevelSerializer.CustomSerializers[((ComponentSerializerFor)attr).SerializesType] = (Activator.CreateInstance(tp) as IComponentSerializer);
		}, Assembly.GetCallingAssembly(), typeof(ComponentSerializerFor));
	}

	public static void AddPrefabPath(string path)
	{
		foreach (KeyValuePair<string, GameObject> current in from pair in (from GameObject go in Resources.LoadAll(path, typeof(GameObject))
		where go.GetComponent<UniqueIdentifier>() != null
		select go).ToDictionary((GameObject go) => go.GetComponent<UniqueIdentifier>().ClassId, (GameObject go) => go)
		where !LevelSerializer.AllPrefabs.ContainsKey(pair.Key)
		select pair)
		{
			LevelSerializer.AllPrefabs.Add(current.Key, current.Value);
		}
	}

	public static void DontCollect()
	{
		LevelSerializer._collectionCount++;
	}

	public static void Collect()
	{
		LevelSerializer._collectionCount--;
	}

	public static string SerializeLevel()
	{
		return LevelSerializer.SerializeLevel(false);
	}

	public static string SerializeLevel(bool urgent)
	{
		if (LevelSerializer.IsSuspended && !urgent)
		{
			if (LevelSerializer.SerializationMode == LevelSerializer.SerializationModes.CacheSerialization)
			{
				return LevelSerializer._cachedState.Data;
			}
			throw new LevelSerializer.SerializationSuspendedException();
		}
		else
		{
			Resources.UnloadUnusedAssets();
			if (LevelSerializer.ShouldCollect)
			{
				GC.Collect();
			}
			byte[] array = LevelSerializer.SerializeLevel(false, null);
			if (LevelSerializer.ShouldCollect)
			{
				GC.Collect();
			}
			if (LevelSerializer.useCompression)
			{
				return CompressionHelper.Compress(array);
			}
			return "NOCOMPRESSION" + Convert.ToBase64String(array);
		}
	}

	internal static void RaiseProgress(string section, float complete)
	{
		LevelSerializer.Progress(section, complete);
	}

	internal static bool HasParent(UniqueIdentifier i, string id)
	{
		if (i)
		{
			GameObject byName = UniqueIdentifier.GetByName(i.Id);
			if (byName)
			{
				Transform transform = byName.transform;
				while (transform != null)
				{
					UniqueIdentifier component;
					if ((component = transform.GetComponent<UniqueIdentifier>()) != null && id == component.Id)
					{
						return true;
					}
					transform = transform.parent;
				}
			}
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
					LevelSerializer.GetComponentsInChildrenWithClause(current, components);
				}
			}
			else
			{
				LevelSerializer.GetComponentsInChildrenWithClause(current, components);
			}
		}
	}

	public static List<StoreInformation> GetComponentsInChildrenWithClause(GameObject go)
	{
		List<StoreInformation> list = new List<StoreInformation>();
		LevelSerializer.GetComponentsInChildrenWithClause(go.transform, list);
		return list;
	}

	public static byte[] SaveObjectTree(this GameObject rootOfTree)
	{
		if (!rootOfTree.GetComponent<UniqueIdentifier>())
		{
			EmptyObjectIdentifier.FlagAll(rootOfTree);
		}
		return LevelSerializer.SerializeLevel(false, rootOfTree.GetComponent<UniqueIdentifier>().Id);
	}

	public static byte[] SerializeLevel(bool urgent, string id)
	{
		LevelSerializer.LevelData levelData;
		using (new Radical.Logging())
		{
			using (new UnitySerializer.SerializationScope())
			{
				levelData = new LevelSerializer.LevelData
				{
					Name = Application.loadedLevelName,
					rootObject = (!string.IsNullOrEmpty(id)) ? id : null
				};
				levelData.StoredObjectNames = (from si in (from i in UniqueIdentifier.AllIdentifiers
				where string.IsNullOrEmpty(id) || i.Id == id || LevelSerializer.HasParent(i, id)
				select i.gameObject into go
				where go != null
				select go).Where(delegate(GameObject go)
				{
					IControlSerializationEx controlSerializationEx = go.FindInterface<IControlSerializationEx>();
					return controlSerializationEx == null || controlSerializationEx.ShouldSaveWholeObject();
				}).Where(delegate(GameObject go)
				{
					if (LevelSerializer.Store == null)
					{
						return true;
					}
					bool result = true;
					LevelSerializer.Store(go, ref result);
					return result;
				}).Select(delegate(GameObject n)
				{
					LevelSerializer.StoredItem result;
					try
					{
						LevelSerializer.StoredItem storedItem = new LevelSerializer.StoredItem();
						storedItem.createEmptyObject = (n.GetComponent<EmptyObjectIdentifier>() != null);
						storedItem.layer = n.layer;
						storedItem.tag = n.tag;
						storedItem.setExtraData = true;
						storedItem.Active = n.activeSelf;
						storedItem.Components = (from c in n.GetComponents<Component>()
						where c != null
						select c.GetType().FullName).Distinct<string>().ToDictionary((string v) => v, (string v) => true);
						storedItem.Name = n.GetComponent<UniqueIdentifier>().Id;
						storedItem.GameObjectName = n.name;
						storedItem.ParentName = ((!(n.transform.parent == null) && !(n.transform.parent.GetComponent<UniqueIdentifier>() == null)) ? n.transform.parent.GetComponent<UniqueIdentifier>().Id : null);
						storedItem.ClassId = ((!(n.GetComponent<PrefabIdentifier>() != null)) ? string.Empty : n.GetComponent<PrefabIdentifier>().ClassId);
						LevelSerializer.StoredItem storedItem2 = storedItem;
						if (n.GetComponent<StoreInformation>())
						{
							n.SendMessage("OnSerializing", SendMessageOptions.DontRequireReceiver);
						}
						PrefabIdentifier component = n.GetComponent<PrefabIdentifier>();
						if (component != null)
						{
							List<StoreInformation> componentsInChildrenWithClause = LevelSerializer.GetComponentsInChildrenWithClause(n);
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
				select si).ToList<LevelSerializer.StoredItem>();
				List<<>__AnonType4<StoreInformation, Component>> toBeProcessed = (from c in (from o in UniqueIdentifier.AllIdentifiers
				where o.GetComponent<StoreInformation>() != null || o.GetComponent<PrefabIdentifier>() != null
				select o into i
				where string.IsNullOrEmpty(id) || i.Id == id || LevelSerializer.HasParent(i, id)
				where i != null
				select i.gameObject into i
				where i != null
				select i).Where(delegate(GameObject go)
				{
					IControlSerializationEx controlSerializationEx = go.FindInterface<IControlSerializationEx>();
					return controlSerializationEx == null || controlSerializationEx.ShouldSaveWholeObject();
				}).Distinct<GameObject>().Where(delegate(GameObject go)
				{
					if (LevelSerializer.Store == null)
					{
						return true;
					}
					bool result = true;
					LevelSerializer.Store(go, ref result);
					return result;
				}).SelectMany((GameObject o) => o.GetComponents<Component>()).Where(delegate(Component c)
				{
					if (c == null)
					{
						return false;
					}
					Type type = c.GetType();
					bool flag = true;
					LevelSerializer.StoreComponent(c, ref flag);
					return flag && (!(c is IControlSerialization) || (c as IControlSerialization).ShouldSave()) && !type.IsDefined(typeof(DontStoreAttribute), true) && !LevelSerializer.IgnoreTypes.Contains(type.FullName);
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
					LevelSerializer.StoredData result;
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
						LevelSerializer.StoredData storedData = new LevelSerializer.StoredData
						{
							Type = cp.Component.GetType().FullName,
							ClassId = cp.Identifier.ClassId,
							Name = cp.Component.GetComponent<UniqueIdentifier>().Id
						};
						if (LevelSerializer.CustomSerializers.ContainsKey(cp.Component.GetType()))
						{
							storedData.Data = LevelSerializer.CustomSerializers[cp.Component.GetType()].Serialize(cp.Component);
						}
						else
						{
							storedData.Data = UnitySerializer.SerializeForDeserializeInto(cp.Component);
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
						LevelSerializer.Progress("Storing", (float)processed / (float)toBeProcessed.Count);
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
				select s).ToList<LevelSerializer.StoredData>();
			}
		}
		return UnitySerializer.Serialize(levelData);
	}

	public static void LoadObjectTree(this byte[] data, Action<LevelLoader> onComplete = null)
	{
		Action<LevelLoader> arg_25_0;
		if ((arg_25_0 = onComplete) == null)
		{
			arg_25_0 = delegate
			{
			};
		}
		onComplete = arg_25_0;
		LevelSerializer.LoadNow(data, true, false, onComplete);
	}

	public static void LoadNow(object data)
	{
		LevelSerializer.LoadNow(data, false, true, null);
	}

	public static void LoadNow(object data, bool dontDeleteExistingItems)
	{
		LevelSerializer.LoadNow(data, dontDeleteExistingItems, true, null);
	}

	public static void LoadNow(object data, bool dontDeleteExistingItems, bool showLoadingGUI)
	{
		LevelSerializer.LoadNow(data, dontDeleteExistingItems, showLoadingGUI, null);
	}

	public static void LoadNow(object data, bool dontDeleteExistingItems, bool showLoadingGUI, Action<LevelLoader> complete)
	{
		byte[] array = null;
		if (data is byte[])
		{
			array = (byte[])data;
		}
		if (data is string)
		{
			string text = (string)data;
			if (text.StartsWith("NOCOMPRESSION"))
			{
				array = Convert.FromBase64String(text.Substring(13));
			}
			else
			{
				array = CompressionHelper.Decompress(text);
			}
		}
		if (array == null)
		{
			throw new ArgumentException("data parameter must be either a byte[] or a base64 encoded string");
		}
		GameObject gameObject = new GameObject();
		LevelLoader levelLoader = gameObject.AddComponent<LevelLoader>();
		levelLoader.showGUI = showLoadingGUI;
		LevelSerializer.LevelData data2 = UnitySerializer.Deserialize<LevelSerializer.LevelData>(array);
		levelLoader.Data = data2;
		levelLoader.DontDelete = dontDeleteExistingItems;
		levelLoader.StartCoroutine(LevelSerializer.PerformLoad(levelLoader, complete));
	}

	[DebuggerHidden]
	private static IEnumerator PerformLoad(LevelLoader loader, Action<LevelLoader> complete)
	{
		LevelSerializer.<PerformLoad>c__Iterator1D8 <PerformLoad>c__Iterator1D = new LevelSerializer.<PerformLoad>c__Iterator1D8();
		<PerformLoad>c__Iterator1D.loader = loader;
		<PerformLoad>c__Iterator1D.complete = complete;
		<PerformLoad>c__Iterator1D.<$>loader = loader;
		<PerformLoad>c__Iterator1D.<$>complete = complete;
		return <PerformLoad>c__Iterator1D;
	}

	public static LevelLoader LoadSavedLevel(string data)
	{
		LevelSerializer.IsDeserializing = true;
		LevelSerializer.LevelData levelData;
		if (data.StartsWith("NOCOMPRESSION"))
		{
			levelData = UnitySerializer.Deserialize<LevelSerializer.LevelData>(Convert.FromBase64String(data.Substring(13)));
		}
		else
		{
			levelData = UnitySerializer.Deserialize<LevelSerializer.LevelData>(CompressionHelper.Decompress(data));
		}
		SaveGameManager.Loaded();
		GameObject gameObject = new GameObject();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		LevelLoader levelLoader = gameObject.AddComponent<LevelLoader>();
		levelLoader.Data = levelData;
		LevelSerializer.LevelLoadingOperation = Application.LoadLevelAsync(levelData.Name);
		return levelLoader;
	}
}
