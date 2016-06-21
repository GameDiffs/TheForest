using Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Deferred, AddComponentMenu("Storage/Resumable Coroutine Support")]
public class RadicalRoutineHelper : MonoBehaviour, IDeserialized
{
	private static RadicalRoutineHelper _current;

	public List<RadicalRoutine> Running = new List<RadicalRoutine>();

	public static RadicalRoutineHelper Current
	{
		get
		{
			if (RadicalRoutineHelper._current == null)
			{
				GameObject gameObject = new GameObject("Radical Routine Helper (AUTO)");
				RadicalRoutineHelper._current = gameObject.AddComponent<RadicalRoutineHelper>();
			}
			return RadicalRoutineHelper._current;
		}
	}

	static RadicalRoutineHelper()
	{
		DelegateSupport.RegisterFunctionType<RadicalRoutineHelper, string>();
		DelegateSupport.RegisterFunctionType<RadicalRoutineHelper, bool>();
		DelegateSupport.RegisterFunctionType<RadicalRoutineHelper, Transform>();
	}

	void IDeserialized.Deserialized()
	{
		try
		{
			Loom.QueueOnMainThread(delegate
			{
				foreach (RadicalRoutine current in this.Running)
				{
					try
					{
						if (current.trackedObject)
						{
							current.trackedObject.StartCoroutine(current.enumerator);
						}
						else
						{
							base.StartCoroutine(current.enumerator);
						}
					}
					catch (Exception ex2)
					{
						Radical.LogError("Problem starting radical coroutine " + ex2.ToString());
					}
				}
			}, 0.02f);
		}
		catch (Exception ex)
		{
			Radical.LogError("Problem queing restart for radical routines " + ex.ToString());
		}
	}

	private void Awake()
	{
		if (!base.GetComponent<StoreInformation>())
		{
			UniqueIdentifier component;
			if (component = base.GetComponent<UniqueIdentifier>())
			{
				string id = component.Id;
				UnityEngine.Object.DestroyImmediate(component);
				EmptyObjectIdentifier emptyObjectIdentifier = base.gameObject.AddComponent<EmptyObjectIdentifier>();
				emptyObjectIdentifier.Id = id;
			}
			else
			{
				base.gameObject.AddComponent<EmptyObjectIdentifier>();
			}
		}
	}

	private void OnDestroy()
	{
		if (RadicalRoutineHelper._current == this)
		{
			RadicalRoutineHelper._current = null;
		}
	}

	public void Run(RadicalRoutine routine)
	{
		this.Running.Add(routine);
		if (routine.trackedObject)
		{
			routine.trackedObject.StartCoroutine(routine.enumerator);
		}
		else
		{
			base.StartCoroutine(routine.enumerator);
		}
	}

	public void Finished(RadicalRoutine routine)
	{
		this.Running.Remove(routine);
		if (!string.IsNullOrEmpty(routine.Method) && routine.Target != null)
		{
			try
			{
				MethodInfo method = routine.Target.GetType().GetMethod(routine.Method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (method != null)
				{
					method.Invoke(routine.Target, new object[0]);
				}
			}
			catch
			{
			}
		}
	}
}
