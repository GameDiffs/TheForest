using System;
using System.Collections.Generic;

namespace Pathfinding.Util
{
	public static class ObjectPool<T> where T : class, IAstarPooledObject, new()
	{
		private static List<T> pool;

		static ObjectPool()
		{
			ObjectPool<T>.pool = new List<T>();
		}

		public static T Claim()
		{
			if (ObjectPool<T>.pool.Count > 0)
			{
				T result = ObjectPool<T>.pool[ObjectPool<T>.pool.Count - 1];
				ObjectPool<T>.pool.RemoveAt(ObjectPool<T>.pool.Count - 1);
				return result;
			}
			return Activator.CreateInstance<T>();
		}

		public static void Warmup(int count)
		{
			T[] array = new T[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = ObjectPool<T>.Claim();
			}
			for (int j = 0; j < count; j++)
			{
				ObjectPool<T>.Release(array[j]);
			}
		}

		public static void Release(T obj)
		{
			for (int i = 0; i < ObjectPool<T>.pool.Count; i++)
			{
				if (ObjectPool<T>.pool[i] == obj)
				{
					throw new InvalidOperationException("The object is released even though it is in the pool. Are you releasing it twice?");
				}
			}
			obj.OnEnterPool();
			ObjectPool<T>.pool.Add(obj);
		}

		public static void Clear()
		{
			ObjectPool<T>.pool.Clear();
		}

		public static int GetSize()
		{
			return ObjectPool<T>.pool.Count;
		}
	}
}
