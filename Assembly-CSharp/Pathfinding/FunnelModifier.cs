using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Modifiers/Funnel")]
	[Serializable]
	public class FunnelModifier : MonoModifier
	{
		public override ModifierData input
		{
			get
			{
				return ModifierData.StrictVectorPath;
			}
		}

		public override ModifierData output
		{
			get
			{
				return ModifierData.VectorPath;
			}
		}

		public override void Apply(Path p, ModifierData source)
		{
			List<GraphNode> path = p.path;
			List<Vector3> vectorPath = p.vectorPath;
			if (path == null || path.Count == 0 || vectorPath == null || vectorPath.Count != path.Count)
			{
				return;
			}
			List<Vector3> list = ListPool<Vector3>.Claim();
			List<Vector3> list2 = ListPool<Vector3>.Claim(path.Count + 1);
			List<Vector3> list3 = ListPool<Vector3>.Claim(path.Count + 1);
			list2.Add(vectorPath[0]);
			list3.Add(vectorPath[0]);
			for (int i = 0; i < path.Count - 1; i++)
			{
				if (!path[i].GetPortal(path[i + 1], list2, list3, false))
				{
					list2.Add((Vector3)path[i].position);
					list3.Add((Vector3)path[i].position);
					list2.Add((Vector3)path[i + 1].position);
					list3.Add((Vector3)path[i + 1].position);
				}
			}
			list2.Add(vectorPath[vectorPath.Count - 1]);
			list3.Add(vectorPath[vectorPath.Count - 1]);
			if (!FunnelModifier.RunFunnel(list2, list3, list))
			{
				list.Add(vectorPath[0]);
				list.Add(vectorPath[vectorPath.Count - 1]);
			}
			ListPool<Vector3>.Release(p.vectorPath);
			p.vectorPath = list;
			ListPool<Vector3>.Release(list2);
			ListPool<Vector3>.Release(list3);
		}

		public static bool RunFunnel(List<Vector3> left, List<Vector3> right, List<Vector3> funnelPath)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (funnelPath == null)
			{
				throw new ArgumentNullException("funnelPath");
			}
			if (left.Count != right.Count)
			{
				throw new ArgumentException("left and right lists must have equal length");
			}
			if (left.Count <= 3)
			{
				return false;
			}
			while (left[1] == left[2] && right[1] == right[2])
			{
				left.RemoveAt(1);
				right.RemoveAt(1);
				if (left.Count <= 3)
				{
					return false;
				}
			}
			Vector3 vector = left[2];
			if (vector == left[1])
			{
				vector = right[2];
			}
			while (Polygon.IsColinear(left[0], left[1], right[1]) || Polygon.Left(left[1], right[1], vector) == Polygon.Left(left[1], right[1], left[0]))
			{
				left.RemoveAt(1);
				right.RemoveAt(1);
				if (left.Count <= 3)
				{
					return false;
				}
				vector = left[2];
				if (vector == left[1])
				{
					vector = right[2];
				}
			}
			if (!Polygon.IsClockwise(left[0], left[1], right[1]) && !Polygon.IsColinear(left[0], left[1], right[1]))
			{
				List<Vector3> list = left;
				left = right;
				right = list;
			}
			funnelPath.Add(left[0]);
			Vector3 vector2 = left[0];
			Vector3 vector3 = left[1];
			Vector3 vector4 = right[1];
			int num = 1;
			int num2 = 1;
			int i = 2;
			while (i < left.Count)
			{
				if (funnelPath.Count > 2000)
				{
					Debug.LogWarning("Avoiding infinite loop. Remove this check if you have this long paths.");
					break;
				}
				Vector3 vector5 = left[i];
				Vector3 vector6 = right[i];
				if (Polygon.TriangleArea2(vector2, vector4, vector6) < 0f)
				{
					goto IL_279;
				}
				if (vector2 == vector4 || Polygon.TriangleArea2(vector2, vector3, vector6) <= 0f)
				{
					vector4 = vector6;
					num = i;
					goto IL_279;
				}
				funnelPath.Add(vector3);
				vector2 = vector3;
				int num3 = num2;
				vector3 = vector2;
				vector4 = vector2;
				num2 = num3;
				num = num3;
				i = num3;
				IL_2DD:
				i++;
				continue;
				IL_279:
				if (Polygon.TriangleArea2(vector2, vector3, vector5) > 0f)
				{
					goto IL_2DD;
				}
				if (vector2 == vector3 || Polygon.TriangleArea2(vector2, vector4, vector5) >= 0f)
				{
					vector3 = vector5;
					num2 = i;
					goto IL_2DD;
				}
				funnelPath.Add(vector4);
				vector2 = vector4;
				num3 = num;
				vector3 = vector2;
				vector4 = vector2;
				num2 = num3;
				num = num3;
				i = num3;
				goto IL_2DD;
			}
			funnelPath.Add(left[left.Count - 1]);
			return true;
		}
	}
}
