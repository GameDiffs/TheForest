using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Modifiers/Advanced Smooth")]
	[Serializable]
	public class AdvancedSmooth : MonoModifier
	{
		[Serializable]
		public class MaxTurn : AdvancedSmooth.TurnConstructor
		{
			private Vector3 preRightCircleCenter = Vector3.zero;

			private Vector3 preLeftCircleCenter = Vector3.zero;

			private Vector3 rightCircleCenter;

			private Vector3 leftCircleCenter;

			private double vaRight;

			private double vaLeft;

			private double preVaLeft;

			private double preVaRight;

			private double gammaLeft;

			private double gammaRight;

			private double betaRightRight;

			private double betaRightLeft;

			private double betaLeftRight;

			private double betaLeftLeft;

			private double deltaRightLeft;

			private double deltaLeftRight;

			private double alfaRightRight;

			private double alfaLeftLeft;

			private double alfaRightLeft;

			private double alfaLeftRight;

			public override void OnTangentUpdate()
			{
				this.rightCircleCenter = AdvancedSmooth.TurnConstructor.current + AdvancedSmooth.TurnConstructor.normal * AdvancedSmooth.TurnConstructor.turningRadius;
				this.leftCircleCenter = AdvancedSmooth.TurnConstructor.current - AdvancedSmooth.TurnConstructor.normal * AdvancedSmooth.TurnConstructor.turningRadius;
				this.vaRight = base.Atan2(AdvancedSmooth.TurnConstructor.current - this.rightCircleCenter);
				this.vaLeft = this.vaRight + 3.1415926535897931;
			}

			public override void Prepare(int i, Vector3[] vectorPath)
			{
				this.preRightCircleCenter = this.rightCircleCenter;
				this.preLeftCircleCenter = this.leftCircleCenter;
				this.rightCircleCenter = AdvancedSmooth.TurnConstructor.current + AdvancedSmooth.TurnConstructor.normal * AdvancedSmooth.TurnConstructor.turningRadius;
				this.leftCircleCenter = AdvancedSmooth.TurnConstructor.current - AdvancedSmooth.TurnConstructor.normal * AdvancedSmooth.TurnConstructor.turningRadius;
				this.preVaRight = this.vaRight;
				this.preVaLeft = this.vaLeft;
				this.vaRight = base.Atan2(AdvancedSmooth.TurnConstructor.current - this.rightCircleCenter);
				this.vaLeft = this.vaRight + 3.1415926535897931;
			}

			public override void TangentToTangent(List<AdvancedSmooth.Turn> turnList)
			{
				this.alfaRightRight = base.Atan2(this.rightCircleCenter - this.preRightCircleCenter);
				this.alfaLeftLeft = base.Atan2(this.leftCircleCenter - this.preLeftCircleCenter);
				this.alfaRightLeft = base.Atan2(this.leftCircleCenter - this.preRightCircleCenter);
				this.alfaLeftRight = base.Atan2(this.rightCircleCenter - this.preLeftCircleCenter);
				double num = (double)(this.leftCircleCenter - this.preRightCircleCenter).magnitude;
				double num2 = (double)(this.rightCircleCenter - this.preLeftCircleCenter).magnitude;
				bool flag = false;
				bool flag2 = false;
				if (num < (double)(AdvancedSmooth.TurnConstructor.turningRadius * 2f))
				{
					num = (double)(AdvancedSmooth.TurnConstructor.turningRadius * 2f);
					flag = true;
				}
				if (num2 < (double)(AdvancedSmooth.TurnConstructor.turningRadius * 2f))
				{
					num2 = (double)(AdvancedSmooth.TurnConstructor.turningRadius * 2f);
					flag2 = true;
				}
				this.deltaRightLeft = ((!flag) ? (1.5707963267948966 - Math.Asin((double)(AdvancedSmooth.TurnConstructor.turningRadius * 2f) / num)) : 0.0);
				this.deltaLeftRight = ((!flag2) ? (1.5707963267948966 - Math.Asin((double)(AdvancedSmooth.TurnConstructor.turningRadius * 2f) / num2)) : 0.0);
				this.betaRightRight = base.ClockwiseAngle(this.preVaRight, this.alfaRightRight - 1.5707963267948966);
				this.betaRightLeft = base.ClockwiseAngle(this.preVaRight, this.alfaRightLeft - this.deltaRightLeft);
				this.betaLeftRight = base.CounterClockwiseAngle(this.preVaLeft, this.alfaLeftRight + this.deltaLeftRight);
				this.betaLeftLeft = base.CounterClockwiseAngle(this.preVaLeft, this.alfaLeftLeft + 1.5707963267948966);
				this.betaRightRight += base.ClockwiseAngle(this.alfaRightRight - 1.5707963267948966, this.vaRight);
				this.betaRightLeft += base.CounterClockwiseAngle(this.alfaRightLeft + this.deltaRightLeft, this.vaLeft);
				this.betaLeftRight += base.ClockwiseAngle(this.alfaLeftRight - this.deltaLeftRight, this.vaRight);
				this.betaLeftLeft += base.CounterClockwiseAngle(this.alfaLeftLeft + 1.5707963267948966, this.vaLeft);
				this.betaRightRight = base.GetLengthFromAngle(this.betaRightRight, (double)AdvancedSmooth.TurnConstructor.turningRadius);
				this.betaRightLeft = base.GetLengthFromAngle(this.betaRightLeft, (double)AdvancedSmooth.TurnConstructor.turningRadius);
				this.betaLeftRight = base.GetLengthFromAngle(this.betaLeftRight, (double)AdvancedSmooth.TurnConstructor.turningRadius);
				this.betaLeftLeft = base.GetLengthFromAngle(this.betaLeftLeft, (double)AdvancedSmooth.TurnConstructor.turningRadius);
				Vector3 a = base.AngleToVector(this.alfaRightRight - 1.5707963267948966) * AdvancedSmooth.TurnConstructor.turningRadius + this.preRightCircleCenter;
				Vector3 a2 = base.AngleToVector(this.alfaRightLeft - this.deltaRightLeft) * AdvancedSmooth.TurnConstructor.turningRadius + this.preRightCircleCenter;
				Vector3 a3 = base.AngleToVector(this.alfaLeftRight + this.deltaLeftRight) * AdvancedSmooth.TurnConstructor.turningRadius + this.preLeftCircleCenter;
				Vector3 a4 = base.AngleToVector(this.alfaLeftLeft + 1.5707963267948966) * AdvancedSmooth.TurnConstructor.turningRadius + this.preLeftCircleCenter;
				Vector3 b = base.AngleToVector(this.alfaRightRight - 1.5707963267948966) * AdvancedSmooth.TurnConstructor.turningRadius + this.rightCircleCenter;
				Vector3 b2 = base.AngleToVector(this.alfaRightLeft - this.deltaRightLeft + 3.1415926535897931) * AdvancedSmooth.TurnConstructor.turningRadius + this.leftCircleCenter;
				Vector3 b3 = base.AngleToVector(this.alfaLeftRight + this.deltaLeftRight + 3.1415926535897931) * AdvancedSmooth.TurnConstructor.turningRadius + this.rightCircleCenter;
				Vector3 b4 = base.AngleToVector(this.alfaLeftLeft + 1.5707963267948966) * AdvancedSmooth.TurnConstructor.turningRadius + this.leftCircleCenter;
				this.betaRightRight += (double)(a - b).magnitude;
				this.betaRightLeft += (double)(a2 - b2).magnitude;
				this.betaLeftRight += (double)(a3 - b3).magnitude;
				this.betaLeftLeft += (double)(a4 - b4).magnitude;
				if (flag)
				{
					this.betaRightLeft += 10000000.0;
				}
				if (flag2)
				{
					this.betaLeftRight += 10000000.0;
				}
				turnList.Add(new AdvancedSmooth.Turn((float)this.betaRightRight, this, 2));
				turnList.Add(new AdvancedSmooth.Turn((float)this.betaRightLeft, this, 3));
				turnList.Add(new AdvancedSmooth.Turn((float)this.betaLeftRight, this, 4));
				turnList.Add(new AdvancedSmooth.Turn((float)this.betaLeftLeft, this, 5));
			}

			public override void PointToTangent(List<AdvancedSmooth.Turn> turnList)
			{
				bool flag = false;
				bool flag2 = false;
				float magnitude = (AdvancedSmooth.TurnConstructor.prev - this.rightCircleCenter).magnitude;
				float magnitude2 = (AdvancedSmooth.TurnConstructor.prev - this.leftCircleCenter).magnitude;
				if (magnitude < AdvancedSmooth.TurnConstructor.turningRadius)
				{
					flag = true;
				}
				if (magnitude2 < AdvancedSmooth.TurnConstructor.turningRadius)
				{
					flag2 = true;
				}
				double num = (!flag) ? base.Atan2(AdvancedSmooth.TurnConstructor.prev - this.rightCircleCenter) : 0.0;
				double num2 = (!flag) ? (1.5707963267948966 - Math.Asin((double)(AdvancedSmooth.TurnConstructor.turningRadius / (AdvancedSmooth.TurnConstructor.prev - this.rightCircleCenter).magnitude))) : 0.0;
				this.gammaRight = num + num2;
				double num3 = (!flag) ? base.ClockwiseAngle(this.gammaRight, this.vaRight) : 0.0;
				double num4 = (!flag2) ? base.Atan2(AdvancedSmooth.TurnConstructor.prev - this.leftCircleCenter) : 0.0;
				double num5 = (!flag2) ? (1.5707963267948966 - Math.Asin((double)(AdvancedSmooth.TurnConstructor.turningRadius / (AdvancedSmooth.TurnConstructor.prev - this.leftCircleCenter).magnitude))) : 0.0;
				this.gammaLeft = num4 - num5;
				double num6 = (!flag2) ? base.CounterClockwiseAngle(this.gammaLeft, this.vaLeft) : 0.0;
				if (!flag)
				{
					turnList.Add(new AdvancedSmooth.Turn((float)num3, this, 0));
				}
				if (!flag2)
				{
					turnList.Add(new AdvancedSmooth.Turn((float)num6, this, 1));
				}
			}

			public override void TangentToPoint(List<AdvancedSmooth.Turn> turnList)
			{
				bool flag = false;
				bool flag2 = false;
				float magnitude = (AdvancedSmooth.TurnConstructor.next - this.rightCircleCenter).magnitude;
				float magnitude2 = (AdvancedSmooth.TurnConstructor.next - this.leftCircleCenter).magnitude;
				if (magnitude < AdvancedSmooth.TurnConstructor.turningRadius)
				{
					flag = true;
				}
				if (magnitude2 < AdvancedSmooth.TurnConstructor.turningRadius)
				{
					flag2 = true;
				}
				if (!flag)
				{
					double num = base.Atan2(AdvancedSmooth.TurnConstructor.next - this.rightCircleCenter);
					double num2 = 1.5707963267948966 - Math.Asin((double)(AdvancedSmooth.TurnConstructor.turningRadius / magnitude));
					this.gammaRight = num - num2;
					double num3 = base.ClockwiseAngle(this.vaRight, this.gammaRight);
					turnList.Add(new AdvancedSmooth.Turn((float)num3, this, 6));
				}
				if (!flag2)
				{
					double num4 = base.Atan2(AdvancedSmooth.TurnConstructor.next - this.leftCircleCenter);
					double num5 = 1.5707963267948966 - Math.Asin((double)(AdvancedSmooth.TurnConstructor.turningRadius / magnitude2));
					this.gammaLeft = num4 + num5;
					double num6 = base.CounterClockwiseAngle(this.vaLeft, this.gammaLeft);
					turnList.Add(new AdvancedSmooth.Turn((float)num6, this, 7));
				}
			}

			public override void GetPath(AdvancedSmooth.Turn turn, List<Vector3> output)
			{
				switch (turn.id)
				{
				case 0:
					base.AddCircleSegment(this.gammaRight, this.vaRight, true, this.rightCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					break;
				case 1:
					base.AddCircleSegment(this.gammaLeft, this.vaLeft, false, this.leftCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					break;
				case 2:
					base.AddCircleSegment(this.preVaRight, this.alfaRightRight - 1.5707963267948966, true, this.preRightCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					base.AddCircleSegment(this.alfaRightRight - 1.5707963267948966, this.vaRight, true, this.rightCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					break;
				case 3:
					base.AddCircleSegment(this.preVaRight, this.alfaRightLeft - this.deltaRightLeft, true, this.preRightCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					base.AddCircleSegment(this.alfaRightLeft - this.deltaRightLeft + 3.1415926535897931, this.vaLeft, false, this.leftCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					break;
				case 4:
					base.AddCircleSegment(this.preVaLeft, this.alfaLeftRight + this.deltaLeftRight, false, this.preLeftCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					base.AddCircleSegment(this.alfaLeftRight + this.deltaLeftRight + 3.1415926535897931, this.vaRight, true, this.rightCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					break;
				case 5:
					base.AddCircleSegment(this.preVaLeft, this.alfaLeftLeft + 1.5707963267948966, false, this.preLeftCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					base.AddCircleSegment(this.alfaLeftLeft + 1.5707963267948966, this.vaLeft, false, this.leftCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					break;
				case 6:
					base.AddCircleSegment(this.vaRight, this.gammaRight, true, this.rightCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					break;
				case 7:
					base.AddCircleSegment(this.vaLeft, this.gammaLeft, false, this.leftCircleCenter, output, AdvancedSmooth.TurnConstructor.turningRadius);
					break;
				}
			}
		}

		[Serializable]
		public class ConstantTurn : AdvancedSmooth.TurnConstructor
		{
			private Vector3 circleCenter;

			private double gamma1;

			private double gamma2;

			private bool clockwise;

			public override void Prepare(int i, Vector3[] vectorPath)
			{
			}

			public override void TangentToTangent(List<AdvancedSmooth.Turn> turnList)
			{
				Vector3 dir = Vector3.Cross(AdvancedSmooth.TurnConstructor.t1, Vector3.up);
				Vector3 vector = AdvancedSmooth.TurnConstructor.current - AdvancedSmooth.TurnConstructor.prev;
				Vector3 start = vector * 0.5f + AdvancedSmooth.TurnConstructor.prev;
				vector = Vector3.Cross(vector, Vector3.up);
				bool flag;
				this.circleCenter = Polygon.IntersectionPointOptimized(AdvancedSmooth.TurnConstructor.prev, dir, start, vector, out flag);
				if (!flag)
				{
					return;
				}
				this.gamma1 = base.Atan2(AdvancedSmooth.TurnConstructor.prev - this.circleCenter);
				this.gamma2 = base.Atan2(AdvancedSmooth.TurnConstructor.current - this.circleCenter);
				this.clockwise = !Polygon.Left(this.circleCenter, AdvancedSmooth.TurnConstructor.prev, AdvancedSmooth.TurnConstructor.prev + AdvancedSmooth.TurnConstructor.t1);
				double num = (!this.clockwise) ? base.CounterClockwiseAngle(this.gamma1, this.gamma2) : base.ClockwiseAngle(this.gamma1, this.gamma2);
				num = base.GetLengthFromAngle(num, (double)(this.circleCenter - AdvancedSmooth.TurnConstructor.current).magnitude);
				turnList.Add(new AdvancedSmooth.Turn((float)num, this, 0));
			}

			public override void GetPath(AdvancedSmooth.Turn turn, List<Vector3> output)
			{
				base.AddCircleSegment(this.gamma1, this.gamma2, this.clockwise, this.circleCenter, output, (this.circleCenter - AdvancedSmooth.TurnConstructor.current).magnitude);
				AdvancedSmooth.TurnConstructor.normal = (AdvancedSmooth.TurnConstructor.current - this.circleCenter).normalized;
				AdvancedSmooth.TurnConstructor.t2 = Vector3.Cross(AdvancedSmooth.TurnConstructor.normal, Vector3.up).normalized;
				AdvancedSmooth.TurnConstructor.normal = -AdvancedSmooth.TurnConstructor.normal;
				if (!this.clockwise)
				{
					AdvancedSmooth.TurnConstructor.t2 = -AdvancedSmooth.TurnConstructor.t2;
					AdvancedSmooth.TurnConstructor.normal = -AdvancedSmooth.TurnConstructor.normal;
				}
				AdvancedSmooth.TurnConstructor.changedPreviousTangent = true;
			}
		}

		public abstract class TurnConstructor
		{
			public const double ThreeSixtyRadians = 6.2831853071795862;

			public float constantBias;

			public float factorBias = 1f;

			public static float turningRadius = 1f;

			public static Vector3 prev;

			public static Vector3 current;

			public static Vector3 next;

			public static Vector3 t1;

			public static Vector3 t2;

			public static Vector3 normal;

			public static Vector3 prevNormal;

			public static bool changedPreviousTangent;

			public abstract void Prepare(int i, Vector3[] vectorPath);

			public virtual void OnTangentUpdate()
			{
			}

			public virtual void PointToTangent(List<AdvancedSmooth.Turn> turnList)
			{
			}

			public virtual void TangentToPoint(List<AdvancedSmooth.Turn> turnList)
			{
			}

			public virtual void TangentToTangent(List<AdvancedSmooth.Turn> turnList)
			{
			}

			public abstract void GetPath(AdvancedSmooth.Turn turn, List<Vector3> output);

			public static void Setup(int i, Vector3[] vectorPath)
			{
				AdvancedSmooth.TurnConstructor.current = vectorPath[i];
				AdvancedSmooth.TurnConstructor.prev = vectorPath[i - 1];
				AdvancedSmooth.TurnConstructor.next = vectorPath[i + 1];
				AdvancedSmooth.TurnConstructor.prev.y = AdvancedSmooth.TurnConstructor.current.y;
				AdvancedSmooth.TurnConstructor.next.y = AdvancedSmooth.TurnConstructor.current.y;
				AdvancedSmooth.TurnConstructor.t1 = AdvancedSmooth.TurnConstructor.t2;
				AdvancedSmooth.TurnConstructor.t2 = (AdvancedSmooth.TurnConstructor.next - AdvancedSmooth.TurnConstructor.current).normalized - (AdvancedSmooth.TurnConstructor.prev - AdvancedSmooth.TurnConstructor.current).normalized;
				AdvancedSmooth.TurnConstructor.t2 = AdvancedSmooth.TurnConstructor.t2.normalized;
				AdvancedSmooth.TurnConstructor.prevNormal = AdvancedSmooth.TurnConstructor.normal;
				AdvancedSmooth.TurnConstructor.normal = Vector3.Cross(AdvancedSmooth.TurnConstructor.t2, Vector3.up);
				AdvancedSmooth.TurnConstructor.normal = AdvancedSmooth.TurnConstructor.normal.normalized;
			}

			public static void PostPrepare()
			{
				AdvancedSmooth.TurnConstructor.changedPreviousTangent = false;
			}

			public void AddCircleSegment(double startAngle, double endAngle, bool clockwise, Vector3 center, List<Vector3> output, float radius)
			{
				double num = 0.062831853071795868;
				if (clockwise)
				{
					while (endAngle > startAngle + 6.2831853071795862)
					{
						endAngle -= 6.2831853071795862;
					}
					while (endAngle < startAngle)
					{
						endAngle += 6.2831853071795862;
					}
				}
				else
				{
					while (endAngle < startAngle - 6.2831853071795862)
					{
						endAngle += 6.2831853071795862;
					}
					while (endAngle > startAngle)
					{
						endAngle -= 6.2831853071795862;
					}
				}
				if (clockwise)
				{
					for (double num2 = startAngle; num2 < endAngle; num2 += num)
					{
						output.Add(this.AngleToVector(num2) * radius + center);
					}
				}
				else
				{
					for (double num3 = startAngle; num3 > endAngle; num3 -= num)
					{
						output.Add(this.AngleToVector(num3) * radius + center);
					}
				}
				output.Add(this.AngleToVector(endAngle) * radius + center);
			}

			public void DebugCircleSegment(Vector3 center, double startAngle, double endAngle, double radius, Color color)
			{
				double num = 0.062831853071795868;
				while (endAngle < startAngle)
				{
					endAngle += 6.2831853071795862;
				}
				Vector3 start = this.AngleToVector(startAngle) * (float)radius + center;
				for (double num2 = startAngle + num; num2 < endAngle; num2 += num)
				{
					Debug.DrawLine(start, this.AngleToVector(num2) * (float)radius + center);
				}
				Debug.DrawLine(start, this.AngleToVector(endAngle) * (float)radius + center);
			}

			public void DebugCircle(Vector3 center, double radius, Color color)
			{
				double num = 0.062831853071795868;
				Vector3 start = this.AngleToVector(-num) * (float)radius + center;
				for (double num2 = 0.0; num2 < 6.2831853071795862; num2 += num)
				{
					Vector3 vector = this.AngleToVector(num2) * (float)radius + center;
					Debug.DrawLine(start, vector, color);
					start = vector;
				}
			}

			public double GetLengthFromAngle(double angle, double radius)
			{
				return radius * angle;
			}

			public double ClockwiseAngle(double from, double to)
			{
				return this.ClampAngle(to - from);
			}

			public double CounterClockwiseAngle(double from, double to)
			{
				return this.ClampAngle(from - to);
			}

			public Vector3 AngleToVector(double a)
			{
				return new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a));
			}

			public double ToDegrees(double rad)
			{
				return rad * 57.295780181884766;
			}

			public double ClampAngle(double a)
			{
				while (a < 0.0)
				{
					a += 6.2831853071795862;
				}
				while (a > 6.2831853071795862)
				{
					a -= 6.2831853071795862;
				}
				return a;
			}

			public double Atan2(Vector3 v)
			{
				return Math.Atan2((double)v.z, (double)v.x);
			}
		}

		public struct Turn : IComparable<AdvancedSmooth.Turn>
		{
			public float length;

			public int id;

			public AdvancedSmooth.TurnConstructor constructor;

			public float score
			{
				get
				{
					return this.length * this.constructor.factorBias + this.constructor.constantBias;
				}
			}

			public Turn(float length, AdvancedSmooth.TurnConstructor constructor, int id = 0)
			{
				this.length = length;
				this.id = id;
				this.constructor = constructor;
			}

			public void GetPath(List<Vector3> output)
			{
				this.constructor.GetPath(this, output);
			}

			public int CompareTo(AdvancedSmooth.Turn t)
			{
				return (t.score <= this.score) ? ((t.score >= this.score) ? 0 : 1) : -1;
			}

			public static bool operator <(AdvancedSmooth.Turn lhs, AdvancedSmooth.Turn rhs)
			{
				return lhs.score < rhs.score;
			}

			public static bool operator >(AdvancedSmooth.Turn lhs, AdvancedSmooth.Turn rhs)
			{
				return lhs.score > rhs.score;
			}
		}

		public float turningRadius = 1f;

		public AdvancedSmooth.MaxTurn turnConstruct1 = new AdvancedSmooth.MaxTurn();

		public AdvancedSmooth.ConstantTurn turnConstruct2 = new AdvancedSmooth.ConstantTurn();

		public override ModifierData input
		{
			get
			{
				return ModifierData.VectorPath;
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
			Vector3[] array = p.vectorPath.ToArray();
			if (array == null || array.Length <= 2)
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			list.Add(array[0]);
			AdvancedSmooth.TurnConstructor.turningRadius = this.turningRadius;
			for (int i = 1; i < array.Length - 1; i++)
			{
				List<AdvancedSmooth.Turn> turnList = new List<AdvancedSmooth.Turn>();
				AdvancedSmooth.TurnConstructor.Setup(i, array);
				this.turnConstruct1.Prepare(i, array);
				this.turnConstruct2.Prepare(i, array);
				AdvancedSmooth.TurnConstructor.PostPrepare();
				if (i == 1)
				{
					this.turnConstruct1.PointToTangent(turnList);
					this.turnConstruct2.PointToTangent(turnList);
				}
				else
				{
					this.turnConstruct1.TangentToTangent(turnList);
					this.turnConstruct2.TangentToTangent(turnList);
				}
				this.EvaluatePaths(turnList, list);
				if (i == array.Length - 2)
				{
					this.turnConstruct1.TangentToPoint(turnList);
					this.turnConstruct2.TangentToPoint(turnList);
				}
				this.EvaluatePaths(turnList, list);
			}
			list.Add(array[array.Length - 1]);
			p.vectorPath = list;
		}

		private void EvaluatePaths(List<AdvancedSmooth.Turn> turnList, List<Vector3> output)
		{
			turnList.Sort();
			for (int i = 0; i < turnList.Count; i++)
			{
				if (i == 0)
				{
					turnList[i].GetPath(output);
				}
			}
			turnList.Clear();
			if (AdvancedSmooth.TurnConstructor.changedPreviousTangent)
			{
				this.turnConstruct1.OnTangentUpdate();
				this.turnConstruct2.OnTangentUpdate();
			}
		}
	}
}
