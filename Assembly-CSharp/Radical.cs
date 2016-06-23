using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UniLinq;
using UnityEngine;

public static class Radical
{
	public class PreferenceAccess
	{
		public bool this[string name]
		{
			get
			{
				name = "Pref " + name;
				return PlayerPrefs.HasKey(name) && PlayerPrefs.GetInt(name) == 1;
			}
			set
			{
				name = "Pref " + name;
				PlayerPrefs.SetInt(name, (!value) ? 0 : 1);
			}
		}
	}

	public class Logging : IDisposable
	{
		public Logging()
		{
			Radical._deferredLoggingEnabled++;
		}

		public void Dispose()
		{
			Radical._deferredLoggingEnabled--;
			if (Radical._deferredLoggingEnabled == 0)
			{
				Radical.CommitLog();
			}
		}
	}

	public static Radical.PreferenceAccess Preferences;

	private static global::Lookup<string, GameObject> _gameObjects;

	private static global::Lookup<string, GameObject> _fullPaths;

	public static bool AllowDeferredLogging;

	private static int _indent;

	public static readonly bool DebugBuild;

	public static int _deferredLoggingEnabled;

	public static Vector3 mergeMix;

	private static List<string> logEntries;

	public static bool DeferredLoggingEnabled
	{
		get
		{
			return Radical._deferredLoggingEnabled > 0;
		}
	}

	static Radical()
	{
		Radical.Preferences = new Radical.PreferenceAccess();
		Radical.AllowDeferredLogging = false;
		Radical._indent = 0;
		Radical._deferredLoggingEnabled = 0;
		Radical.mergeMix = new Vector3(0f, 1f, 0f);
		Radical.logEntries = new List<string>();
		Radical.DebugBuild = UnityEngine.Debug.isDebugBuild;
	}

	public static void ActivateChildren(this Component co)
	{
		co.gameObject.SetActiveRecursively(true);
	}

	public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) where TSource : class
	{
		return source.MaxBy(selector, Comparer<TKey>.Default);
	}

	public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer) where TSource : class
	{
		TSource result;
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				result = (TSource)((object)null);
			}
			else
			{
				TSource tSource = enumerator.Current;
				TKey y = selector(tSource);
				while (enumerator.MoveNext())
				{
					TSource current = enumerator.Current;
					TKey tKey = selector(current);
					if (comparer.Compare(tKey, y) > 0)
					{
						tSource = current;
						y = tKey;
					}
				}
				result = tSource;
			}
		}
		return result;
	}

	public static IEnumerable<TResult> Zip<T1, T2, TResult>(this IEnumerable<T1> seq1, IEnumerable<T2> seq2, Func<T1, T2, TResult> resultSelector)
	{
		List<TResult> list = new List<TResult>();
		IEnumerator<T1> enumerator = seq1.GetEnumerator();
		IEnumerator<T2> enumerator2 = seq2.GetEnumerator();
		while (enumerator.MoveNext() && enumerator2.MoveNext())
		{
			list.Add(resultSelector(enumerator.Current, enumerator2.Current));
		}
		return list;
	}

	public static bool CalledFrom(string name)
	{
		StackTrace stackTrace = new StackTrace();
		StackFrame[] frames = stackTrace.GetFrames();
		for (int i = 0; i < frames.Length; i++)
		{
			StackFrame stackFrame = frames[i];
			if (stackFrame.GetMethod().Name.Contains(name))
			{
				return true;
			}
		}
		return false;
	}

	public static int MaskLayers(params int[] layers)
	{
		int num = 0;
		for (int i = 0; i < layers.Length; i++)
		{
			int num2 = layers[i];
			num |= 1 << num2;
		}
		return num;
	}

	public static int MaskLayers(params string[] layers)
	{
		int num = 0;
		for (int i = 0; i < layers.Length; i++)
		{
			string layerName = layers[i];
			num |= 1 << LayerMask.NameToLayer(layerName);
		}
		return num;
	}

	public static void PlayOneShot(this GameObject gameObject, AudioClip clip)
	{
		if (clip == null)
		{
			return;
		}
		if (!gameObject.GetComponent<AudioSource>())
		{
			gameObject.AddComponent<AudioSource>();
			gameObject.GetComponent<AudioSource>().playOnAwake = false;
		}
		gameObject.GetComponent<AudioSource>().PlayOneShot(clip);
	}

	public static void PlayAudio(this GameObject gameObject, AudioClip clip)
	{
		if (clip == null)
		{
			return;
		}
		if (!gameObject.GetComponent<AudioSource>())
		{
			gameObject.AddComponent<AudioSource>();
			gameObject.GetComponent<AudioSource>().playOnAwake = false;
		}
		gameObject.GetComponent<AudioSource>().clip = clip;
		gameObject.GetComponent<AudioSource>().loop = true;
		gameObject.GetComponent<AudioSource>().Play();
	}

	public static void FadeVolume(this GameObject component, float toLevel = 1f, float time = 1f, float? fromLevel = null)
	{
		component.gameObject.StartExtendedCoroutine(Radical.VolumeFader(component.GetComponent<AudioSource>(), toLevel, time, fromLevel));
	}

	[DebuggerHidden]
	private static IEnumerator VolumeFader(AudioSource source, float level, float time, float? fromLevel)
	{
		Radical.<VolumeFader>c__Iterator1CD <VolumeFader>c__Iterator1CD = new Radical.<VolumeFader>c__Iterator1CD();
		<VolumeFader>c__Iterator1CD.fromLevel = fromLevel;
		<VolumeFader>c__Iterator1CD.source = source;
		<VolumeFader>c__Iterator1CD.time = time;
		<VolumeFader>c__Iterator1CD.level = level;
		<VolumeFader>c__Iterator1CD.<$>fromLevel = fromLevel;
		<VolumeFader>c__Iterator1CD.<$>source = source;
		<VolumeFader>c__Iterator1CD.<$>time = time;
		<VolumeFader>c__Iterator1CD.<$>level = level;
		return <VolumeFader>c__Iterator1CD;
	}

	public static void DeactivateChildren(this Component co)
	{
		foreach (Transform current in co.transform.GetComponentsInChildren<Transform>().Except(new Transform[]
		{
			co.transform
		}))
		{
			current.gameObject.active = false;
		}
	}

	public static void DestroyChildren(this Transform t)
	{
		foreach (Transform current in t.Cast<Transform>())
		{
			UnityEngine.Object.Destroy(current.gameObject);
		}
	}

	public static Transform FindChildIncludingDeactivated(this Transform t, string name)
	{
		Transform[] componentsInChildren = t.GetComponentsInChildren<Transform>(true);
		return componentsInChildren.FirstOrDefault((Transform c) => c.name == name);
	}

	public static string GetId(this GameObject go)
	{
		UniqueIdentifier component = go.GetComponent<UniqueIdentifier>();
		return (!(component == null)) ? component.Id : go.GetFullName();
	}

	public static GameObject FindGameObject(string name)
	{
		Radical.IndexScene();
		return (!name.Contains("/")) ? Radical._gameObjects[name] : Radical._fullPaths[name];
	}

	public static string GetFullName(this GameObject gameObject)
	{
		Stack<string> stack = new Stack<string>();
		Transform transform = gameObject.transform;
		while (transform != null)
		{
			stack.Push(transform.name);
			transform = transform.parent;
		}
		StringBuilder stringBuilder = new StringBuilder();
		while (stack.Count > 0)
		{
			stringBuilder.AppendFormat("/{0}", stack.Pop());
		}
		return stringBuilder.ToString();
	}

	private static void IndexScene()
	{
		if (GameObject.Find("_SceneIndex") != null)
		{
			return;
		}
		Radical._gameObjects = new global::Lookup<string, GameObject>();
		Radical._fullPaths = new global::Lookup<string, GameObject>();
		foreach (GameObject current in (from GameObject g in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))
		where g.transform.parent == null
		select g).SelectMany((GameObject g) => from Transform t in g.GetComponentsInChildren(typeof(Transform), true)
		select t.gameObject))
		{
			Radical._gameObjects[current.name] = current;
			Radical._fullPaths[current.GetFullName().Substring(1)] = current;
		}
		new GameObject("_SceneIndex");
	}

	public static void ReIndexScene()
	{
		GameObject gameObject = GameObject.Find("_SceneIndex");
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	public static T Find<T>(string name) where T : Component
	{
		Radical.IndexScene();
		GameObject gameObject = (!name.Contains("/")) ? Radical._gameObjects[name] : Radical._fullPaths[name];
		if (gameObject == null)
		{
			return (T)((object)null);
		}
		return gameObject.GetComponent<T>();
	}

	public static T Find<T>(this GameObject go, string name) where T : Component
	{
		go = go.transform.FindChild(name).gameObject;
		return go.GetComponentInChildren<T>();
	}

	public static Index<T, List<TR>> ToIndex<TSource, T, TR>(this IEnumerable<TSource> source, Func<TSource, T> keySelector, Func<TSource, TR> elementSelector) where T : class where TR : class
	{
		Index<T, List<TR>> index = new Index<T, List<TR>>();
		foreach (TSource current in source)
		{
			index[keySelector(current)].Add((elementSelector != null) ? elementSelector(current) : (current as TR));
		}
		return index;
	}

	public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
	{
		return source.MinBy(selector, Comparer<TKey>.Default);
	}

	public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
	{
		TSource result;
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				throw new InvalidOperationException("Sequence was empty");
			}
			TSource tSource = enumerator.Current;
			TKey y = selector(tSource);
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				TKey tKey = selector(current);
				if (comparer.Compare(tKey, y) < 0)
				{
					tSource = current;
					y = tKey;
				}
			}
			result = tSource;
		}
		return result;
	}

	public static IEnumerable<TResult> Discrete<TResult, T1>(this IEnumerable<TResult> seq, Func<TResult, T1> func)
	{
		return from g in seq.GroupBy(func)
		select g.First<TResult>();
	}

	public static Index<T, List<TSource>> ToIndex<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> keySelector) where T : class
	{
		Index<T, List<TSource>> index = new Index<T, List<TSource>>();
		foreach (TSource current in source)
		{
			index[keySelector(current)].Add(current);
		}
		return index;
	}

	public static T FindInterface<T>(this GameObject go) where T : class
	{
		return go.GetComponents<Component>().OfType<T>().FirstOrDefault<T>();
	}

	public static T FindImplementor<T>(this GameObject go) where T : class
	{
		return Radical.RecurseFind<T>(go);
	}

	public static T[] FindImplementors<T>(this GameObject go) where T : class
	{
		return go.GetComponentsInChildren<Component>().OfType<T>().ToArray<T>();
	}

	private static T RecurseFind<T>(GameObject go) where T : class
	{
		Component component = go.GetComponents<Component>().FirstOrDefault((Component c) => c is T);
		if (component != null)
		{
			return component as T;
		}
		if (go.transform.parent != null)
		{
			return Radical.RecurseFind<T>(go.transform.parent.gameObject);
		}
		return (T)((object)null);
	}

	public static int IndexOf<T>(this IEnumerable<T> items, T item)
	{
		return items.FindIndex((T i) => EqualityComparer<T>.Default.Equals(item, i));
	}

	public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
	{
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int num = 0;
		foreach (T current in items)
		{
			if (predicate(current))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static Color RGBA(int r, int g, int b, int a)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, (float)a / 255f);
	}

	public static Quaternion Merge(this Quaternion first, Vector3 second)
	{
		return Quaternion.Euler(first.eulerAngles.Merge(second));
	}

	public static Vector3 Merge(this Vector3 first, Vector3 second)
	{
		return new Vector3(first.x * (1f - Radical.mergeMix.x) + second.x * Radical.mergeMix.x, first.y * (1f - Radical.mergeMix.y) + second.y * Radical.mergeMix.y, first.z * (1f - Radical.mergeMix.z) + second.z * Radical.mergeMix.z);
	}

	public static T GetInterface<T>(this Transform tra) where T : class
	{
		return tra.gameObject.GetInterface<T>();
	}

	public static T GetInterface<T>(this GameObject go) where T : class
	{
		MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
		for (int i = 0; i < components.Length; i++)
		{
			MonoBehaviour monoBehaviour = components[i];
			if (monoBehaviour is T)
			{
				return monoBehaviour as T;
			}
		}
		return (T)((object)null);
	}

	public static IList<T> GetInterfaces<T>(this GameObject go) where T : class
	{
		List<T> list = new List<T>();
		MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
		for (int i = 0; i < components.Length; i++)
		{
			MonoBehaviour monoBehaviour = components[i];
			if (monoBehaviour is T)
			{
				list.Add(monoBehaviour as T);
			}
		}
		return list;
	}

	public static void IndentLog()
	{
		Radical._indent++;
	}

	public static void OutdentLog()
	{
		Radical._indent--;
	}

	public static void LogNode(object message)
	{
		Radical.LogNow(message.ToString(), new object[0]);
	}

	public static void LogNow(string message, params object[] parms)
	{
		if (!Radical.DebugBuild)
		{
			return;
		}
		UnityEngine.Debug.Log(string.Format(message, parms));
	}

	public static void LogWarning(string message)
	{
		Radical.LogWarning(message, null);
	}

	public static void LogWarning(string message, UnityEngine.Object context)
	{
		if (!Radical.DebugBuild)
		{
			return;
		}
		if (context != null)
		{
			UnityEngine.Debug.LogWarning(message, context);
		}
		else
		{
			UnityEngine.Debug.LogWarning(message);
		}
	}

	public static void LogError(string message)
	{
		Radical.LogError(message, null);
	}

	public static void LogError(string message, UnityEngine.Object context)
	{
		if (!Radical.DebugBuild)
		{
			return;
		}
		if (context != null)
		{
			UnityEngine.Debug.LogError(message, context);
		}
		else
		{
			UnityEngine.Debug.LogError(message);
		}
	}

	public static bool IsLogging()
	{
		return Radical.DebugBuild && Radical.DeferredLoggingEnabled;
	}

	public static void Log(string message, params object[] parms)
	{
		if (!Radical.DebugBuild || !Radical.DeferredLoggingEnabled || !Radical.AllowDeferredLogging)
		{
			return;
		}
		Radical.logEntries.Add(new string(' ', 4 * Radical._indent) + string.Format(message, parms));
		if (Radical.logEntries.Count > 50000)
		{
			Radical.logEntries.RemoveAt(0);
		}
	}

	public static void ClearLog()
	{
		Radical.logEntries.Clear();
	}

	public static void CommitLog()
	{
		if (Radical.logEntries.Count == 0)
		{
			return;
		}
		string message = Radical.logEntries.Aggregate((string current, string next) => current + "\n" + next);
		UnityEngine.Debug.Log(message);
		Radical.logEntries.Clear();
	}

	public static GameObject Instantiate(Transform template)
	{
		return Radical.Instantiate(template, null);
	}

	public static GameObject Instantiate(Transform template, GameObject parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<Transform>(template).gameObject;
		if (parent != null)
		{
			gameObject.transform.SetParent(parent.transform);
		}
		return gameObject;
	}

	public static GameObject SetParent(this GameObject child, GameObject parent)
	{
		return child.SetParent(parent, false);
	}

	public static GameObject SetParent(this GameObject child, GameObject parent, bool setScale)
	{
		child.transform.SetParent(parent.transform, setScale);
		return child;
	}

	public static Transform SetParent(this Transform child, GameObject parent)
	{
		return child.SetParent(parent, false);
	}

	public static Transform SetParent(this Transform child, GameObject parent, bool setScale)
	{
		child.SetParent(parent.transform, setScale);
		return child;
	}

	public static Transform SetParent(this Transform child, Transform parent)
	{
		return child.SetParent(parent, false);
	}

	public static Transform SetParent(this Transform child, Transform parent, bool setScale)
	{
		try
		{
			Vector3 localPosition = child.localPosition;
			Quaternion localRotation = child.localRotation;
			Vector3 localScale = child.localScale;
			child.parent = parent;
			child.localPosition = localPosition;
			child.localRotation = localRotation;
			if (setScale)
			{
				child.localScale = localScale;
			}
		}
		catch
		{
		}
		return child;
	}

	public static Quaternion SmoothDamp(this Vector3 current, Vector3 target, ref Vector3 velocity, float time)
	{
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, time);
		zero.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, time);
		zero.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, time);
		return Quaternion.Euler(zero);
	}

	public static GameObject AddChild(this GameObject parent, Transform template)
	{
		return Radical.Instantiate(template, parent);
	}

	public static void EnsureComponent(this GameObject obj, Type t)
	{
		if (obj.GetComponent(t) == null)
		{
			obj.AddComponent(t);
		}
	}

	public static void RemoveComponent(this GameObject obj, Type t)
	{
		Component[] components = obj.GetComponents(t);
		for (int i = 0; i < components.Length; i++)
		{
			Component obj2 = components[i];
			UnityEngine.Object.DestroyImmediate(obj2);
		}
	}

	public static void RemoveComponentNormal(this GameObject obj, Type t)
	{
		Component[] components = obj.GetComponents(t);
		for (int i = 0; i < components.Length; i++)
		{
			Component obj2 = components[i];
			UnityEngine.Object.Destroy(obj2);
		}
	}
}
