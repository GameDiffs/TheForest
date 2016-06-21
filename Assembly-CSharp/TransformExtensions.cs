using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

public static class TransformExtensions
{
	public static T FirstAncestorOfType<T>(this GameObject gameObject) where T : Component
	{
		Transform parent = gameObject.transform.parent;
		T result = (T)((object)null);
		while (parent != null && (result = parent.GetComponent<T>()) == null)
		{
			parent = parent.parent;
		}
		return result;
	}

	public static T LastAncestorOfType<T>(this GameObject gameObject) where T : class
	{
		Transform parent = gameObject.transform.parent;
		T result = (T)((object)null);
		while (parent != null)
		{
			T t = parent.gameObject.FindImplementor<T>();
			if (t != null)
			{
				result = t;
			}
			parent = parent.parent;
		}
		return result;
	}

	public static T[] GetAllComponentsInChildren<T>(this Transform parent) where T : Component
	{
		List<T> list = new List<T>();
		T component = parent.GetComponent<T>();
		if (component)
		{
			list.Add(component);
		}
		foreach (Transform current in parent.Cast<Transform>())
		{
			T component2 = current.GetComponent<T>();
			if (component2)
			{
				list.Add(component2);
			}
			list.AddRange(current.GetAllComponentsInChildren<T>());
		}
		return list.ToArray();
	}

	public static T[] GetAllComponentsInChildren<T>(this Component comp) where T : Component
	{
		return comp.transform.GetAllComponentsInChildren<T>();
	}

	public static T[] GetAllComponentsInChildren<T>(this GameObject go) where T : Component
	{
		return go.transform.GetAllComponentsInChildren<T>();
	}
}
