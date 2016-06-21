using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("Storage/Internal/Level Loader (Internal use only, do not add this to your objects!)")]
public class LevelLoader : MonoBehaviour
{
	public delegate void CreateObjectDelegate(GameObject prefab, ref bool cancel);

	public delegate void SerializedComponentDelegate(GameObject gameObject, string componentName, ref bool cancel);

	public delegate void SerializedObjectDelegate(GameObject gameObject, ref bool cancel);

	public static LevelLoader Current;

	private static Texture2D _pixel;

	public GameObject rootObject;

	private readonly Dictionary<string, int> _indexDictionary = new Dictionary<string, int>();

	public LevelSerializer.LevelData Data;

	public bool DontDelete;

	public GameObject Last;

	private float _alpha = 1f;

	private bool _loading = true;

	public bool showGUI;

	public float timeScaleAfterLoading = 1f;

	public bool useJSON;

	public Action<GameObject, List<GameObject>> whenCompleted = delegate
	{
	};

	private bool wasLoaded;

	private static int loadingCount = 0;

	public static event LevelLoader.CreateObjectDelegate CreateGameObject
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelLoader.CreateGameObject = (LevelLoader.CreateObjectDelegate)Delegate.Combine(LevelLoader.CreateGameObject, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelLoader.CreateGameObject = (LevelLoader.CreateObjectDelegate)Delegate.Remove(LevelLoader.CreateGameObject, value);
		}
	}

	public static event LevelLoader.SerializedObjectDelegate OnDestroyObject
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelLoader.OnDestroyObject = (LevelLoader.SerializedObjectDelegate)Delegate.Combine(LevelLoader.OnDestroyObject, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelLoader.OnDestroyObject = (LevelLoader.SerializedObjectDelegate)Delegate.Remove(LevelLoader.OnDestroyObject, value);
		}
	}

	public static event LevelLoader.SerializedObjectDelegate LoadData
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelLoader.LoadData = (LevelLoader.SerializedObjectDelegate)Delegate.Combine(LevelLoader.LoadData, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelLoader.LoadData = (LevelLoader.SerializedObjectDelegate)Delegate.Remove(LevelLoader.LoadData, value);
		}
	}

	public static event LevelLoader.SerializedComponentDelegate LoadComponent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelLoader.LoadComponent = (LevelLoader.SerializedComponentDelegate)Delegate.Combine(LevelLoader.LoadComponent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelLoader.LoadComponent = (LevelLoader.SerializedComponentDelegate)Delegate.Remove(LevelLoader.LoadComponent, value);
		}
	}

	public static event Action<Component> LoadedComponent
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			LevelLoader.LoadedComponent = (Action<Component>)Delegate.Combine(LevelLoader.LoadedComponent, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			LevelLoader.LoadedComponent = (Action<Component>)Delegate.Remove(LevelLoader.LoadedComponent, value);
		}
	}

	static LevelLoader()
	{
		// Note: this type is marked as 'beforefieldinit'.
		LevelLoader.CreateGameObject = delegate
		{
		};
		LevelLoader.OnDestroyObject = delegate
		{
		};
		LevelLoader.LoadData = delegate
		{
		};
		LevelLoader.LoadComponent = delegate
		{
		};
		LevelLoader.LoadedComponent = delegate
		{
		};
	}

	private void Awake()
	{
		this.timeScaleAfterLoading = Time.timeScale;
		LevelLoader.Current = this;
		if (LevelLoader._pixel == null)
		{
			LevelLoader._pixel = new Texture2D(1, 1);
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
		LevelLoader.<Load>c__Iterator1CB <Load>c__Iterator1CB = new LevelLoader.<Load>c__Iterator1CB();
		<Load>c__Iterator1CB.<>f__this = this;
		return <Load>c__Iterator1CB;
	}

	[DebuggerHidden]
	public IEnumerator Load(int numberOfFrames, float timeScale = 0f)
	{
		LevelLoader.<Load>c__Iterator1CC <Load>c__Iterator1CC = new LevelLoader.<Load>c__Iterator1CC();
		<Load>c__Iterator1CC.numberOfFrames = numberOfFrames;
		<Load>c__Iterator1CC.timeScale = timeScale;
		<Load>c__Iterator1CC.<$>numberOfFrames = numberOfFrames;
		<Load>c__Iterator1CC.<$>timeScale = timeScale;
		<Load>c__Iterator1CC.<>f__this = this;
		return <Load>c__Iterator1CC;
	}
}
