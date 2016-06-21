using Pathfinding;
using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Pathfinding/Modifiers/Radius Offset")]
public class RadiusModifier : MonoModifier
{
	[Flags]
	private enum TangentType
	{
		OuterRight = 1,
		InnerRightLeft = 2,
		InnerLeftRight = 4,
		OuterLeft = 8,
		Outer = 9,
		Inner = 6
	}

	public float radius = 1f;

	public float detail = 10f;

	private float[] radi = new float[10];

	private float[] a1 = new float[10];

	private float[] a2 = new float[10];

	private bool[] dir = new bool[10];

	public override ModifierData input
	{
		get
		{
			return ModifierData.Vector;
		}
	}

	public override ModifierData output
	{
		get
		{
			return ModifierData.VectorPath;
		}
	}

	private bool CalculateCircleInner(Vector3 p1, Vector3 p2, float r1, float r2, out float a, out float sigma)
	{
		float magnitude = (p1 - p2).magnitude;
		if (r1 + r2 > magnitude)
		{
			a = 0f;
			sigma = 0f;
			return false;
		}
		a = (float)Math.Acos((double)((r1 + r2) / magnitude));
		sigma = (float)Math.Atan2((double)(p2.z - p1.z), (double)(p2.x - p1.x));
		return true;
	}

	private bool CalculateCircleOuter(Vector3 p1, Vector3 p2, float r1, float r2, out float a, out float sigma)
	{
		float magnitude = (p1 - p2).magnitude;
		if (Math.Abs(r1 - r2) > magnitude)
		{
			a = 0f;
			sigma = 0f;
			return false;
		}
		a = (float)Math.Acos((double)((r1 - r2) / magnitude));
		sigma = (float)Math.Atan2((double)(p2.z - p1.z), (double)(p2.x - p1.x));
		return true;
	}

	private RadiusModifier.TangentType CalculateTangentType(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
	{
		bool flag = Polygon.Left(p1, p2, p3);
		bool flag2 = Polygon.Left(p2, p3, p4);
		return (RadiusModifier.TangentType)(1 << (((!flag) ? 0 : 2) + ((!flag2) ? 0 : 1) & 31));
	}

	private RadiusModifier.TangentType CalculateTangentTypeSimple(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		bool flag = Polygon.Left(p1, p2, p3);
		bool flag2 = flag;
		return (RadiusModifier.TangentType)(1 << (((!flag2) ? 0 : 2) + ((!flag) ? 0 : 1) & 31));
	}

	private void DrawCircleSegment(Vector3 p1, float rad, Color col, float start = 0f, float end = 6.28318548f)
	{
		Vector3 start2 = new Vector3((float)Math.Cos((double)start), 0f, (float)Math.Sin((double)start)) * rad + p1;
		for (float num = start; num < end; num += 0.03141593f)
		{
			Vector3 vector = new Vector3((float)Math.Cos((double)num), 0f, (float)Math.Sin((double)num)) * rad + p1;
			Debug.DrawLine(start2, vector, col);
			start2 = vector;
		}
		if ((double)end == 6.2831853071795862)
		{
			Vector3 end2 = new Vector3((float)Math.Cos((double)start), 0f, (float)Math.Sin((double)start)) * rad + p1;
			Debug.DrawLine(start2, end2, col);
		}
	}

	public override void Apply(Path p, ModifierData source)
	{
		List<Vector3> vectorPath = p.vectorPath;
		List<Vector3> list = this.Apply(vectorPath);
		if (list != vectorPath)
		{
			ListPool<Vector3>.Release(p.vectorPath);
			p.vectorPath = list;
		}
	}

	public List<Vector3> Apply(List<Vector3> vs)
	{
		if (vs == null || vs.Count < 3)
		{
			return vs;
		}
		if (this.radi.Length < vs.Count)
		{
			this.radi = new float[vs.Count];
			this.a1 = new float[vs.Count];
			this.a2 = new float[vs.Count];
			this.dir = new bool[vs.Count];
		}
		for (int i = 0; i < vs.Count; i++)
		{
			this.radi[i] = this.radius;
		}
		this.radi[0] = 0f;
		this.radi[vs.Count - 1] = 0f;
		int num = 0;
		for (int j = 0; j < vs.Count - 1; j++)
		{
			num++;
			if (num > 2 * vs.Count)
			{
				Debug.LogWarning("Could not resolve radiuses, the path is too complex. Try reducing the base radius");
				break;
			}
			RadiusModifier.TangentType tangentType;
			if (j == 0)
			{
				tangentType = this.CalculateTangentTypeSimple(vs[j], vs[j + 1], vs[j + 2]);
			}
			else if (j == vs.Count - 2)
			{
				tangentType = this.CalculateTangentTypeSimple(vs[j - 1], vs[j], vs[j + 1]);
			}
			else
			{
				tangentType = this.CalculateTangentType(vs[j - 1], vs[j], vs[j + 1], vs[j + 2]);
			}
			float num4;
			float num5;
			if ((tangentType & RadiusModifier.TangentType.Inner) != (RadiusModifier.TangentType)0)
			{
				float num2;
				float num3;
				if (!this.CalculateCircleInner(vs[j], vs[j + 1], this.radi[j], this.radi[j + 1], out num2, out num3))
				{
					float magnitude = (vs[j + 1] - vs[j]).magnitude;
					this.radi[j] = magnitude * (this.radi[j] / (this.radi[j] + this.radi[j + 1]));
					this.radi[j + 1] = magnitude - this.radi[j];
					this.radi[j] *= 0.99f;
					this.radi[j + 1] *= 0.99f;
					j -= 2;
				}
				else if (tangentType == RadiusModifier.TangentType.InnerRightLeft)
				{
					this.a2[j] = num3 - num2;
					this.a1[j + 1] = num3 - num2 + 3.14159274f;
					this.dir[j] = true;
				}
				else
				{
					this.a2[j] = num3 + num2;
					this.a1[j + 1] = num3 + num2 + 3.14159274f;
					this.dir[j] = false;
				}
			}
			else if (!this.CalculateCircleOuter(vs[j], vs[j + 1], this.radi[j], this.radi[j + 1], out num4, out num5))
			{
				if (j == vs.Count - 2)
				{
					this.radi[j] = (vs[j + 1] - vs[j]).magnitude;
					this.radi[j] *= 0.99f;
					j--;
				}
				else
				{
					if (this.radi[j] > this.radi[j + 1])
					{
						this.radi[j + 1] = this.radi[j] - (vs[j + 1] - vs[j]).magnitude;
					}
					else
					{
						this.radi[j + 1] = this.radi[j] + (vs[j + 1] - vs[j]).magnitude;
					}
					this.radi[j + 1] *= 0.99f;
				}
				j--;
			}
			else if (tangentType == RadiusModifier.TangentType.OuterRight)
			{
				this.a2[j] = num5 - num4;
				this.a1[j + 1] = num5 - num4;
				this.dir[j] = true;
			}
			else
			{
				this.a2[j] = num5 + num4;
				this.a1[j + 1] = num5 + num4;
				this.dir[j] = false;
			}
		}
		List<Vector3> list = ListPool<Vector3>.Claim();
		list.Add(vs[0]);
		if (this.detail < 1f)
		{
			this.detail = 1f;
		}
		float num6 = 6.28318548f / this.detail;
		for (int k = 1; k < vs.Count - 1; k++)
		{
			float num7 = this.a1[k];
			float num8 = this.a2[k];
			float d = this.radi[k];
			if (this.dir[k])
			{
				if (num8 < num7)
				{
					num8 += 6.28318548f;
				}
				for (float num9 = num7; num9 < num8; num9 += num6)
				{
					list.Add(new Vector3((float)Math.Cos((double)num9), 0f, (float)Math.Sin((double)num9)) * d + vs[k]);
				}
			}
			else
			{
				if (num7 < num8)
				{
					num7 += 6.28318548f;
				}
				for (float num10 = num7; num10 > num8; num10 -= num6)
				{
					list.Add(new Vector3((float)Math.Cos((double)num10), 0f, (float)Math.Sin((double)num10)) * d + vs[k]);
				}
			}
		}
		list.Add(vs[vs.Count - 1]);
		return list;
	}
}
