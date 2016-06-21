using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
public class Loom : MonoBehaviour
{
	public class DelayedQueueItem
	{
		public float time;

		public Action action;
	}

	private static Loom _current;

	private int _count;

	private static bool _initialized;

	private static int _threadId = -1;

	private List<Action> _actions = new List<Action>();

	private List<Loom.DelayedQueueItem> _delayed = new List<Loom.DelayedQueueItem>();

	public static Loom Current
	{
		get
		{
			if (!Loom._initialized)
			{
				Loom.Initialize();
			}
			return Loom._current;
		}
	}

	public static void Initialize()
	{
		bool flag = !Loom._initialized;
		if (flag && Loom._threadId != -1 && Loom._threadId != Thread.CurrentThread.ManagedThreadId)
		{
			return;
		}
		if (flag)
		{
			GameObject gameObject = new GameObject("Loom");
			gameObject.hideFlags = HideFlags.DontSave;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			if (Loom._current)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(Loom._current.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(Loom._current.gameObject);
				}
			}
			Loom._current = gameObject.AddComponent<Loom>();
			UnityEngine.Object.DontDestroyOnLoad(Loom._current);
			Loom._initialized = true;
			Loom._threadId = Thread.CurrentThread.ManagedThreadId;
		}
	}

	private void OnDestroy()
	{
		Loom._initialized = false;
	}

	public static void QueueOnMainThread(Action action)
	{
		Loom.QueueOnMainThread(action, 0f);
	}

	public static void QueueOnMainThread(Action action, float time)
	{
		if (time != 0f)
		{
			List<Loom.DelayedQueueItem> delayed = Loom.Current._delayed;
			lock (delayed)
			{
				Loom.Current._delayed.Add(new Loom.DelayedQueueItem
				{
					time = Time.time + time,
					action = action
				});
			}
		}
		else
		{
			List<Action> actions = Loom.Current._actions;
			lock (actions)
			{
				Loom.Current._actions.Add(action);
			}
		}
	}

	public static void RunAsync(Action action)
	{
		new Thread(new ParameterizedThreadStart(Loom.RunAction))
		{
			Priority = System.Threading.ThreadPriority.Normal
		}.Start(action);
	}

	private static void RunAction(object action)
	{
		((Action)action)();
	}

	private void Start()
	{
	}

	private void Update()
	{
		List<Action> actions = this._actions;
		lock (actions)
		{
			for (int i = 0; i < this._actions.Count; i++)
			{
				this._actions[i]();
			}
			this._actions.Clear();
		}
		List<Loom.DelayedQueueItem> delayed = this._delayed;
		lock (delayed)
		{
			for (int j = 0; j < this._delayed.Count; j++)
			{
				Loom.DelayedQueueItem delayedQueueItem = this._delayed[j];
				if (delayedQueueItem.time <= Time.time)
				{
					delayedQueueItem.action();
					this._delayed.RemoveAt(j);
					j--;
				}
			}
		}
	}
}
