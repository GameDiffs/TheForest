using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public static class ExtendedFind
	{
		public static T GetInterface<T>(GameObject obj) where T : class
		{
			Component[] components = obj.GetComponents<Component>();
			Component[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				Component component = array[i];
				if (component is T)
				{
					return component as T;
				}
			}
			return (T)((object)null);
		}

		public static T GetInterfaceInChildren<T>(GameObject obj) where T : class
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren<Component>();
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Component component = array[i];
				if (component is T)
				{
					return component as T;
				}
			}
			return (T)((object)null);
		}

		public static T GetInterfaceImmediateChildren<T>(GameObject obj) where T : class
		{
			foreach (Transform transform in obj.transform)
			{
				Component[] components = transform.GetComponents<Component>();
				Component[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					Component component = array[i];
					if (component is T)
					{
						return component as T;
					}
				}
			}
			return (T)((object)null);
		}

		public static T[] GetInterfaces<T>(GameObject obj) where T : class
		{
			Component[] components = obj.GetComponents<Component>();
			List<T> list = new List<T>();
			Component[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				Component component = array[i];
				if (component is T)
				{
					list.Add(component as T);
				}
			}
			return list.ToArray();
		}

		public static T[] GetInterfacesInChildren<T>(GameObject obj) where T : class
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren<Component>();
			List<T> list = new List<T>();
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Component component = array[i];
				if (component is T)
				{
					list.Add(component as T);
				}
			}
			return list.ToArray();
		}

		public static T[] GetInterfacesImmediateChildren<T>(GameObject obj) where T : class
		{
			List<T> list = new List<T>();
			foreach (Transform transform in obj.transform)
			{
				Component[] components = transform.GetComponents<Component>();
				Component[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					Component component = array[i];
					if (component is T)
					{
						list.Add(component as T);
					}
				}
			}
			return list.ToArray();
		}

		public static T GetComponetInImmediateParent<T>(GameObject obj) where T : Component
		{
			if (obj.transform.parent == null)
			{
				return (T)((object)null);
			}
			return obj.transform.parent.GetComponent<T>();
		}

		public static T[] GetComponentsInImmediateParent<T>(GameObject obj) where T : Component
		{
			if (obj.transform.parent == null)
			{
				return new T[0];
			}
			return obj.transform.parent.GetComponents<T>();
		}

		public static T GetComponetInImmediateChildren<T>(GameObject obj) where T : Component
		{
			foreach (Transform transform in obj.transform)
			{
				T component = transform.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
			}
			return (T)((object)null);
		}

		public static T[] GetComponetsInImmediateChildren<T>(GameObject obj) where T : Component
		{
			List<T> list = new List<T>();
			foreach (Transform transform in obj.transform)
			{
				T[] components = transform.GetComponents<T>();
				T[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					T item = array[i];
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public static T FindComponentOnGameObject<T>(string name) where T : Component
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				return (T)((object)null);
			}
			return gameObject.GetComponent<T>();
		}

		public static T[] FindComponentsOnGameObject<T>(string name) where T : Component
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				return new T[0];
			}
			return gameObject.GetComponents<T>();
		}

		public static T FindInterfaceOnGameObject<T>(string name) where T : class
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				return (T)((object)null);
			}
			return ExtendedFind.GetInterface<T>(gameObject);
		}

		public static T[] FindInterfacesOnGameObject<T>(string name) where T : class
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				return new T[0];
			}
			return ExtendedFind.GetInterfaces<T>(gameObject);
		}
	}
}
