using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("Storage/Internal/Level Loader (Internal use only, do not add this to your objects!)")]
public class JSONLevelLoader : MonoBehaviour
{
	public delegate void CreateObjectDelegate(GameObject prefab, ref bool cancel);

	public delegate void SerializedComponentDelegate(GameObject gameObject, string componentName, ref bool cancel);

	public delegate void SerializedObjectDelegate(GameObject gameObject, ref bool cancel);

	public static JSONLevelLoader Current;

	private static Texture2D pixel;

	public JSONLevelSerializer.LevelData Data;

	public bool DontDelete;

	public GameObject Last;

	public bool showGUI;

	public float timeScaleAfterLoading = 1f;

	public Action<GameObject, List<GameObject>> whenCompleted = delegate
	{
	};

	public GameObject rootObject;

	private readonly Dictionary<string, int> _indexDictionary = new Dictionary<string, int>();

	private bool wasLoaded;

	private static int loadingCount = 0;

	public static event JSONLevelLoader.CreateObjectDelegate CreateGameObject
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelLoader.CreateGameObject = (JSONLevelLoader.CreateObjectDelegate)Delegate.Combine(JSONLevelLoader.CreateGameObject, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelLoader.CreateGameObject = (JSONLevelLoader.CreateObjectDelegate)Delegate.Remove(JSONLevelLoader.CreateGameObject, value);
		}
	}

	public static event JSONLevelLoader.SerializedObjectDelegate OnDestroyObject
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelLoader.OnDestroyObject = (JSONLevelLoader.SerializedObjectDelegate)Delegate.Combine(JSONLevelLoader.OnDestroyObject, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelLoader.OnDestroyObject = (JSONLevelLoader.SerializedObjectDelegate)Delegate.Remove(JSONLevelLoader.OnDestroyObject, value);
		}
	}

	public static event JSONLevelLoader.SerializedObjectDelegate LoadData
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelLoader.LoadData = (JSONLevelLoader.SerializedObjectDelegate)Delegate.Combine(JSONLevelLoader.LoadData, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelLoader.LoadData = (JSONLevelLoader.SerializedObjectDelegate)Delegate.Remove(JSONLevelLoader.LoadData, value);
		}
	}

	public static event JSONLevelLoader.SerializedComponentDelegate LoadComponent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelLoader.LoadComponent = (JSONLevelLoader.SerializedComponentDelegate)Delegate.Combine(JSONLevelLoader.LoadComponent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelLoader.LoadComponent = (JSONLevelLoader.SerializedComponentDelegate)Delegate.Remove(JSONLevelLoader.LoadComponent, value);
		}
	}

	public static event Action<Component> LoadedComponent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			JSONLevelLoader.LoadedComponent = (Action<Component>)Delegate.Combine(JSONLevelLoader.LoadedComponent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			JSONLevelLoader.LoadedComponent = (Action<Component>)Delegate.Remove(JSONLevelLoader.LoadedComponent, value);
		}
	}

	static JSONLevelLoader()
	{
		// Note: this type is marked as 'beforefieldinit'.
		JSONLevelLoader.CreateGameObject = delegate
		{
		};
		JSONLevelLoader.OnDestroyObject = delegate
		{
		};
		JSONLevelLoader.LoadData = delegate
		{
		};
		JSONLevelLoader.LoadComponent = delegate
		{
		};
		JSONLevelLoader.LoadedComponent = delegate
		{
		};
	}

	private void Awake()
	{
		this.timeScaleAfterLoading = Time.timeScale;
		JSONLevelLoader.Current = this;
		if (JSONLevelLoader.pixel == null)
		{
			JSONLevelLoader.pixel = new Texture2D(1, 1);
		}
	}

	private void OnGUI()
	{
	}

	private void OnLevelWasLoaded(int level)
	{
		if (this.wasLoaded)
		{
			return;
		}
		this.wasLoaded = true;
		this.timeScaleAfterLoading = Time.timeScale;
		base.StartCoroutine(this.Load());
	}

	private static void SetActive(GameObject go, bool activate)
	{
		go.SetActive(activate);
	}

	[DebuggerHidden]
	public IEnumerator Load()
	{
		JSONLevelLoader.<Load>c__Iterator1CF <Load>c__Iterator1CF = new JSONLevelLoader.<Load>c__Iterator1CF();
		<Load>c__Iterator1CF.<>f__this = this;
		return <Load>c__Iterator1CF;
	}

	[DebuggerHidden]
	public IEnumerator Load(int numberOfFrames, float timeScale = 0f)
	{
		JSONLevelLoader.<Load>c__Iterator1D0 <Load>c__Iterator1D = new JSONLevelLoader.<Load>c__Iterator1D0();
		<Load>c__Iterator1D.numberOfFrames = numberOfFrames;
		<Load>c__Iterator1D.timeScale = timeScale;
		<Load>c__Iterator1D.<$>numberOfFrames = numberOfFrames;
		<Load>c__Iterator1D.<$>timeScale = timeScale;
		<Load>c__Iterator1D.<>f__this = this;
		return <Load>c__Iterator1D;
	}
}
