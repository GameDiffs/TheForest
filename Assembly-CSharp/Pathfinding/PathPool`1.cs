using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public static class PathPool<T> where T : Path, new()
	{
		private static readonly Stack<T> pool;

		private static int totalCreated;

		static PathPool()
		{
			PathPool<T>.pool = new Stack<T>();
		}

		public static void Recycle(T path)
		{
			Stack<T> obj = PathPool<T>.pool;
			lock (obj)
			{
				path.recycled = true;
				path.OnEnterPool();
				PathPool<T>.pool.Push(path);
			}
		}

		public static void Warmup(int count, int length)
		{
			ListPool<GraphNode>.Warmup(count, length);
			ListPool<Vector3>.Warmup(count, length);
			Path[] array = new Path[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = PathPool<T>.GetPath();
				array[i].Claim(array);
			}
			for (int j = 0; j < count; j++)
			{
				array[j].Release(array);
			}
		}

		public static int GetTotalCreated()
		{
			return PathPool<T>.totalCreated;
		}

		public static int GetSize()
		{
			return PathPool<T>.pool.Count;
		}

		public static T GetPath()
		{
			Stack<T> obj = PathPool<T>.pool;
			T result;
			lock (obj)
			{
				T t;
				if (PathPool<T>.pool.Count > 0)
				{
					t = PathPool<T>.pool.Pop();
				}
				else
				{
					t = Activator.CreateInstance<T>();
					PathPool<T>.totalCreated++;
				}
				t.recycled = false;
				t.Reset();
				result = t;
			}
			return result;
		}
	}
}
