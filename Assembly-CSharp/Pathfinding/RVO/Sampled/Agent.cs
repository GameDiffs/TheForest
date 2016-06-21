using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Pathfinding.RVO.Sampled
{
	public class Agent : IAgent
	{
		public struct VO
		{
			public Vector2 origin;

			public Vector2 center;

			private Vector2 line1;

			private Vector2 line2;

			private Vector2 dir1;

			private Vector2 dir2;

			private Vector2 cutoffLine;

			private Vector2 cutoffDir;

			private float sqrCutoffDistance;

			private bool leftSide;

			private bool colliding;

			private float radius;

			private float weightFactor;

			public VO(Vector2 offset, Vector2 p0, Vector2 dir, float weightFactor)
			{
				this.colliding = true;
				this.line1 = p0;
				this.dir1 = -dir;
				this.origin = Vector2.zero;
				this.center = Vector2.zero;
				this.line2 = Vector2.zero;
				this.dir2 = Vector2.zero;
				this.cutoffLine = Vector2.zero;
				this.cutoffDir = Vector2.zero;
				this.sqrCutoffDistance = 0f;
				this.leftSide = false;
				this.radius = 0f;
				this.weightFactor = weightFactor * 0.5f;
			}

			public VO(Vector2 offset, Vector2 p1, Vector2 p2, Vector2 tang1, Vector2 tang2, float weightFactor)
			{
				this.weightFactor = weightFactor * 0.5f;
				this.colliding = false;
				this.cutoffLine = p1;
				this.cutoffDir = (p2 - p1).normalized;
				this.line1 = p1;
				this.dir1 = tang1;
				this.line2 = p2;
				this.dir2 = tang2;
				this.dir2 = -this.dir2;
				this.cutoffDir = -this.cutoffDir;
				this.origin = Vector2.zero;
				this.center = Vector2.zero;
				this.sqrCutoffDistance = 0f;
				this.leftSide = false;
				this.radius = 0f;
				weightFactor = 5f;
			}

			public VO(Vector2 center, Vector2 offset, float radius, Vector2 sideChooser, float inverseDt, float weightFactor)
			{
				this.weightFactor = weightFactor * 0.5f;
				this.origin = offset;
				weightFactor = 0.5f;
				if (center.magnitude < radius)
				{
					this.colliding = true;
					this.leftSide = false;
					this.line1 = center.normalized * (center.magnitude - radius);
					Vector2 vector = new Vector2(this.line1.y, -this.line1.x);
					this.dir1 = vector.normalized;
					this.line1 += offset;
					this.cutoffDir = Vector2.zero;
					this.cutoffLine = Vector2.zero;
					this.sqrCutoffDistance = 0f;
					this.dir2 = Vector2.zero;
					this.line2 = Vector2.zero;
					this.center = Vector2.zero;
					this.radius = 0f;
				}
				else
				{
					this.colliding = false;
					center *= inverseDt;
					radius *= inverseDt;
					Vector2 b = center + offset;
					this.sqrCutoffDistance = center.magnitude - radius;
					this.center = center;
					this.cutoffLine = center.normalized * this.sqrCutoffDistance;
					Vector2 vector2 = new Vector2(-this.cutoffLine.y, this.cutoffLine.x);
					this.cutoffDir = vector2.normalized;
					this.cutoffLine += offset;
					this.sqrCutoffDistance *= this.sqrCutoffDistance;
					float num = Mathf.Atan2(-center.y, -center.x);
					float num2 = Mathf.Abs(Mathf.Acos(radius / center.magnitude));
					this.radius = radius;
					this.leftSide = Polygon.Left(Vector2.zero, center, sideChooser);
					this.line1 = new Vector2(Mathf.Cos(num + num2), Mathf.Sin(num + num2)) * radius;
					Vector2 vector3 = new Vector2(this.line1.y, -this.line1.x);
					this.dir1 = vector3.normalized;
					this.line2 = new Vector2(Mathf.Cos(num - num2), Mathf.Sin(num - num2)) * radius;
					Vector2 vector4 = new Vector2(this.line2.y, -this.line2.x);
					this.dir2 = vector4.normalized;
					this.line1 += b;
					this.line2 += b;
				}
			}

			public static bool Left(Vector2 a, Vector2 dir, Vector2 p)
			{
				return dir.x * (p.y - a.y) - (p.x - a.x) * dir.y <= 0f;
			}

			public static float Det(Vector2 a, Vector2 dir, Vector2 p)
			{
				return (p.x - a.x) * dir.y - dir.x * (p.y - a.y);
			}

			public Vector2 Sample(Vector2 p, out float weight)
			{
				if (this.colliding)
				{
					float num = Agent.VO.Det(this.line1, this.dir1, p);
					if (num >= 0f)
					{
						weight = num * this.weightFactor;
						return new Vector2(-this.dir1.y, this.dir1.x) * weight * Agent.GlobalIncompressibility;
					}
					weight = 0f;
					return new Vector2(0f, 0f);
				}
				else
				{
					float num2 = Agent.VO.Det(this.cutoffLine, this.cutoffDir, p);
					if (num2 <= 0f)
					{
						weight = 0f;
						return Vector2.zero;
					}
					float num3 = Agent.VO.Det(this.line1, this.dir1, p);
					float num4 = Agent.VO.Det(this.line2, this.dir2, p);
					if (num3 < 0f || num4 < 0f)
					{
						weight = 0f;
						return new Vector2(0f, 0f);
					}
					if (this.leftSide)
					{
						if (num2 < this.radius)
						{
							weight = num2 * this.weightFactor;
							return new Vector2(-this.cutoffDir.y, this.cutoffDir.x) * weight;
						}
						weight = num3;
						return new Vector2(-this.dir1.y, this.dir1.x) * weight;
					}
					else
					{
						if (num2 < this.radius)
						{
							weight = num2 * this.weightFactor;
							return new Vector2(-this.cutoffDir.y, this.cutoffDir.x) * weight;
						}
						weight = num4 * this.weightFactor;
						return new Vector2(-this.dir2.y, this.dir2.x) * weight;
					}
				}
			}

			public float ScalarSample(Vector2 p)
			{
				if (this.colliding)
				{
					float num = Agent.VO.Det(this.line1, this.dir1, p);
					if (num >= 0f)
					{
						return num * Agent.GlobalIncompressibility * this.weightFactor;
					}
					return 0f;
				}
				else
				{
					float num2 = Agent.VO.Det(this.cutoffLine, this.cutoffDir, p);
					if (num2 <= 0f)
					{
						return 0f;
					}
					float num3 = Agent.VO.Det(this.line1, this.dir1, p);
					float num4 = Agent.VO.Det(this.line2, this.dir2, p);
					if (num3 < 0f || num4 < 0f)
					{
						return 0f;
					}
					if (this.leftSide)
					{
						if (num2 < this.radius)
						{
							return num2 * this.weightFactor;
						}
						return num3 * this.weightFactor;
					}
					else
					{
						if (num2 < this.radius)
						{
							return num2 * this.weightFactor;
						}
						return num4 * this.weightFactor;
					}
				}
			}
		}

		private const float WallWeight = 5f;

		private Vector3 smoothPos;

		public float radius;

		public float height;

		public float maxSpeed;

		public float neighbourDist;

		public float agentTimeHorizon;

		public float obstacleTimeHorizon;

		public float weight;

		public bool locked;

		private RVOLayer layer;

		private RVOLayer collidesWith;

		public int maxNeighbours;

		public Vector3 position;

		public Vector3 desiredVelocity;

		public Vector3 prevSmoothPos;

		internal Agent next;

		private Vector3 velocity;

		internal Vector3 newVelocity;

		public Simulator simulator;

		public List<Agent> neighbours = new List<Agent>();

		public List<float> neighbourDists = new List<float>();

		private List<ObstacleVertex> obstaclesBuffered = new List<ObstacleVertex>();

		private List<ObstacleVertex> obstacles = new List<ObstacleVertex>();

		private List<float> obstacleDists = new List<float>();

		public static Stopwatch watch1 = new Stopwatch();

		public static Stopwatch watch2 = new Stopwatch();

		public static float DesiredVelocityWeight = 0.02f;

		public static float DesiredVelocityScale = 0.1f;

		public static float GlobalIncompressibility = 30f;

		public Vector3 Position
		{
			get;
			private set;
		}

		public Vector3 InterpolatedPosition
		{
			get
			{
				return this.smoothPos;
			}
		}

		public Vector3 DesiredVelocity
		{
			get;
			set;
		}

		public RVOLayer Layer
		{
			get;
			set;
		}

		public RVOLayer CollidesWith
		{
			get;
			set;
		}

		public bool Locked
		{
			get;
			set;
		}

		public float Radius
		{
			get;
			set;
		}

		public float Height
		{
			get;
			set;
		}

		public float MaxSpeed
		{
			get;
			set;
		}

		public float NeighbourDist
		{
			get;
			set;
		}

		public float AgentTimeHorizon
		{
			get;
			set;
		}

		public float ObstacleTimeHorizon
		{
			get;
			set;
		}

		public Vector3 Velocity
		{
			get;
			set;
		}

		public bool DebugDraw
		{
			get;
			set;
		}

		public int MaxNeighbours
		{
			get;
			set;
		}

		public List<ObstacleVertex> NeighbourObstacles
		{
			get
			{
				return null;
			}
		}

		public Agent(Vector3 pos)
		{
			this.MaxSpeed = 2f;
			this.NeighbourDist = 15f;
			this.AgentTimeHorizon = 2f;
			this.ObstacleTimeHorizon = 2f;
			this.Height = 5f;
			this.Radius = 5f;
			this.MaxNeighbours = 10;
			this.Locked = false;
			this.position = pos;
			this.Position = this.position;
			this.prevSmoothPos = this.position;
			this.smoothPos = this.position;
			this.Layer = RVOLayer.DefaultAgent;
			this.CollidesWith = (RVOLayer)(-1);
		}

		public void Teleport(Vector3 pos)
		{
			this.Position = pos;
			this.smoothPos = pos;
			this.prevSmoothPos = pos;
		}

		public void SetYPosition(float yCoordinate)
		{
			this.Position = new Vector3(this.Position.x, yCoordinate, this.Position.z);
			this.smoothPos.y = yCoordinate;
			this.prevSmoothPos.y = yCoordinate;
		}

		public void BufferSwitch()
		{
			this.radius = this.Radius;
			this.height = this.Height;
			this.maxSpeed = this.MaxSpeed;
			this.neighbourDist = this.NeighbourDist;
			this.agentTimeHorizon = this.AgentTimeHorizon;
			this.obstacleTimeHorizon = this.ObstacleTimeHorizon;
			this.maxNeighbours = this.MaxNeighbours;
			this.desiredVelocity = this.DesiredVelocity;
			this.locked = this.Locked;
			this.collidesWith = this.CollidesWith;
			this.layer = this.Layer;
			this.Velocity = this.velocity;
			List<ObstacleVertex> list = this.obstaclesBuffered;
			this.obstaclesBuffered = this.obstacles;
			this.obstacles = list;
		}

		public void Update()
		{
			this.velocity = this.newVelocity;
			this.prevSmoothPos = this.smoothPos;
			this.position = this.prevSmoothPos;
			this.position += this.velocity * this.simulator.DeltaTime;
			this.Position = this.position;
		}

		public void Interpolate(float t)
		{
			this.smoothPos = this.prevSmoothPos + (this.Position - this.prevSmoothPos) * t;
		}

		public void CalculateNeighbours()
		{
			this.neighbours.Clear();
			this.neighbourDists.Clear();
			if (this.locked)
			{
				return;
			}
			float num;
			if (this.MaxNeighbours > 0)
			{
				num = this.neighbourDist * this.neighbourDist;
				this.simulator.Quadtree.Query(new Vector2(this.position.x, this.position.z), this.neighbourDist, this);
			}
			this.obstacles.Clear();
			this.obstacleDists.Clear();
			num = this.obstacleTimeHorizon * this.maxSpeed + this.radius;
			num *= num;
		}

		private float Sqr(float x)
		{
			return x * x;
		}

		public float InsertAgentNeighbour(Agent agent, float rangeSq)
		{
			if (this == agent)
			{
				return rangeSq;
			}
			if ((agent.layer & this.collidesWith) == (RVOLayer)0)
			{
				return rangeSq;
			}
			float num = this.Sqr(agent.position.x - this.position.x) + this.Sqr(agent.position.z - this.position.z);
			if (num < rangeSq)
			{
				if (this.neighbours.Count < this.maxNeighbours)
				{
					this.neighbours.Add(agent);
					this.neighbourDists.Add(num);
				}
				int num2 = this.neighbours.Count - 1;
				if (num < this.neighbourDists[num2])
				{
					while (num2 != 0 && num < this.neighbourDists[num2 - 1])
					{
						this.neighbours[num2] = this.neighbours[num2 - 1];
						this.neighbourDists[num2] = this.neighbourDists[num2 - 1];
						num2--;
					}
					this.neighbours[num2] = agent;
					this.neighbourDists[num2] = num;
				}
				if (this.neighbours.Count == this.maxNeighbours)
				{
					rangeSq = this.neighbourDists[this.neighbourDists.Count - 1];
				}
			}
			return rangeSq;
		}

		public void InsertObstacleNeighbour(ObstacleVertex ob1, float rangeSq)
		{
			ObstacleVertex obstacleVertex = ob1.next;
			float num = AstarMath.DistancePointSegmentStrict(ob1.position, obstacleVertex.position, this.Position);
			if (num < rangeSq)
			{
				this.obstacles.Add(ob1);
				this.obstacleDists.Add(num);
				int num2 = this.obstacles.Count - 1;
				while (num2 != 0 && num < this.obstacleDists[num2 - 1])
				{
					this.obstacles[num2] = this.obstacles[num2 - 1];
					this.obstacleDists[num2] = this.obstacleDists[num2 - 1];
					num2--;
				}
				this.obstacles[num2] = ob1;
				this.obstacleDists[num2] = num;
			}
		}

		private static Vector3 To3D(Vector2 p)
		{
			return new Vector3(p.x, 0f, p.y);
		}

		private static void DrawCircle(Vector2 _p, float radius, Color col)
		{
			Agent.DrawCircle(_p, radius, 0f, 6.28318548f, col);
		}

		private static void DrawCircle(Vector2 _p, float radius, float a0, float a1, Color col)
		{
			Vector3 a2 = Agent.To3D(_p);
			while (a0 > a1)
			{
				a0 -= 6.28318548f;
			}
			Vector3 b = new Vector3(Mathf.Cos(a0) * radius, 0f, Mathf.Sin(a0) * radius);
			int num = 0;
			while ((float)num <= 40f)
			{
				Vector3 vector = new Vector3(Mathf.Cos(Mathf.Lerp(a0, a1, (float)num / 40f)) * radius, 0f, Mathf.Sin(Mathf.Lerp(a0, a1, (float)num / 40f)) * radius);
				UnityEngine.Debug.DrawLine(a2 + b, a2 + vector, col);
				b = vector;
				num++;
			}
		}

		private static void DrawVO(Vector2 circleCenter, float radius, Vector2 origin)
		{
			float num = Mathf.Atan2((origin - circleCenter).y, (origin - circleCenter).x);
			float num2 = radius / (origin - circleCenter).magnitude;
			float num3 = (num2 > 1f) ? 0f : Mathf.Abs(Mathf.Acos(num2));
			Agent.DrawCircle(circleCenter, radius, num - num3, num + num3, Color.black);
			Vector2 vector = new Vector2(Mathf.Cos(num - num3), Mathf.Sin(num - num3)) * radius;
			Vector2 vector2 = new Vector2(Mathf.Cos(num + num3), Mathf.Sin(num + num3)) * radius;
			Vector2 p = -new Vector2(-vector.y, vector.x);
			Vector2 p2 = new Vector2(-vector2.y, vector2.x);
			vector += circleCenter;
			vector2 += circleCenter;
			UnityEngine.Debug.DrawRay(Agent.To3D(vector), Agent.To3D(p).normalized * 100f, Color.black);
			UnityEngine.Debug.DrawRay(Agent.To3D(vector2), Agent.To3D(p2).normalized * 100f, Color.black);
		}

		private static void DrawCross(Vector2 p, float size = 1f)
		{
			Agent.DrawCross(p, Color.white, size);
		}

		private static void DrawCross(Vector2 p, Color col, float size = 1f)
		{
			size *= 0.5f;
			UnityEngine.Debug.DrawLine(new Vector3(p.x, 0f, p.y) - Vector3.right * size, new Vector3(p.x, 0f, p.y) + Vector3.right * size, col);
			UnityEngine.Debug.DrawLine(new Vector3(p.x, 0f, p.y) - Vector3.forward * size, new Vector3(p.x, 0f, p.y) + Vector3.forward * size, col);
		}

		internal void CalculateVelocity(Simulator.WorkerContext context)
		{
			if (this.locked)
			{
				this.newVelocity = Vector2.zero;
				return;
			}
			if (context.vos.Length < this.neighbours.Count + this.simulator.obstacles.Count)
			{
				context.vos = new Agent.VO[Mathf.Max(context.vos.Length * 2, this.neighbours.Count + this.simulator.obstacles.Count)];
			}
			Vector2 vector = new Vector2(this.position.x, this.position.z);
			Agent.VO[] vos = context.vos;
			int num = 0;
			Vector2 vector2 = new Vector2(this.velocity.x, this.velocity.z);
			float num2 = 1f / this.agentTimeHorizon;
			float wallThickness = this.simulator.WallThickness;
			float num3 = (this.simulator.algorithm != Simulator.SamplingAlgorithm.GradientDecent) ? 5f : 1f;
			for (int i = 0; i < this.simulator.obstacles.Count; i++)
			{
				ObstacleVertex obstacleVertex = this.simulator.obstacles[i];
				ObstacleVertex obstacleVertex2 = obstacleVertex;
				do
				{
					if (obstacleVertex2.ignore || this.position.y > obstacleVertex2.position.y + obstacleVertex2.height || this.position.y + this.height < obstacleVertex2.position.y || (obstacleVertex2.layer & this.collidesWith) == (RVOLayer)0)
					{
						obstacleVertex2 = obstacleVertex2.next;
					}
					else
					{
						float num4 = Agent.VO.Det(new Vector2(obstacleVertex2.position.x, obstacleVertex2.position.z), obstacleVertex2.dir, vector);
						float num5 = num4;
						float num6 = Vector2.Dot(obstacleVertex2.dir, vector - new Vector2(obstacleVertex2.position.x, obstacleVertex2.position.z));
						bool flag = num6 <= wallThickness * 0.05f || num6 >= (new Vector2(obstacleVertex2.position.x, obstacleVertex2.position.z) - new Vector2(obstacleVertex2.next.position.x, obstacleVertex2.next.position.z)).magnitude - wallThickness * 0.05f;
						if (Mathf.Abs(num5) < this.neighbourDist)
						{
							if (num5 <= 0f && !flag && num5 > -wallThickness)
							{
								vos[num] = new Agent.VO(vector, new Vector2(obstacleVertex2.position.x, obstacleVertex2.position.z) - vector, obstacleVertex2.dir, num3 * 2f);
								num++;
							}
							else if (num5 > 0f)
							{
								Vector2 p = new Vector2(obstacleVertex2.position.x, obstacleVertex2.position.z) - vector;
								Vector2 p2 = new Vector2(obstacleVertex2.next.position.x, obstacleVertex2.next.position.z) - vector;
								Vector2 normalized = p.normalized;
								Vector2 normalized2 = p2.normalized;
								vos[num] = new Agent.VO(vector, p, p2, normalized, normalized2, num3);
								num++;
							}
						}
						obstacleVertex2 = obstacleVertex2.next;
					}
				}
				while (obstacleVertex2 != obstacleVertex);
			}
			for (int j = 0; j < this.neighbours.Count; j++)
			{
				Agent agent = this.neighbours[j];
				if (agent != this)
				{
					float num7 = Math.Min(this.position.y + this.height, agent.position.y + agent.height);
					float num8 = Math.Max(this.position.y, agent.position.y);
					if (num7 - num8 >= 0f)
					{
						Vector2 vector3 = new Vector2(agent.Velocity.x, agent.velocity.z);
						float num9 = this.radius + agent.radius;
						Vector2 vector4 = new Vector2(agent.position.x, agent.position.z) - vector;
						Vector2 sideChooser = vector2 - vector3;
						Vector2 vector5;
						if (agent.locked)
						{
							vector5 = vector3;
						}
						else
						{
							vector5 = (vector2 + vector3) * 0.5f;
						}
						vos[num] = new Agent.VO(vector4, vector5, num9, sideChooser, num2, 1f);
						num++;
						if (this.DebugDraw)
						{
							Agent.DrawVO(vector + vector4 * num2 + vector5, num9 * num2, vector + vector5);
						}
					}
				}
			}
			Vector2 vector6 = Vector2.zero;
			if (this.simulator.algorithm == Simulator.SamplingAlgorithm.GradientDecent)
			{
				if (this.DebugDraw)
				{
					for (int k = 0; k < 40; k++)
					{
						for (int l = 0; l < 40; l++)
						{
							Vector2 vector7 = new Vector2((float)k * 15f / 40f, (float)l * 15f / 40f);
							Vector2 a = Vector2.zero;
							float num10 = 0f;
							for (int m = 0; m < num; m++)
							{
								float num11;
								a += vos[m].Sample(vector7 - vector, out num11);
								if (num11 > num10)
								{
									num10 = num11;
								}
							}
							Vector2 a2 = new Vector2(this.desiredVelocity.x, this.desiredVelocity.z) - (vector7 - vector);
							a += a2 * Agent.DesiredVelocityScale;
							if (a2.magnitude * Agent.DesiredVelocityWeight > num10)
							{
								num10 = a2.magnitude * Agent.DesiredVelocityWeight;
							}
							if (num10 > 0f)
							{
								a /= num10;
							}
							UnityEngine.Debug.DrawRay(Agent.To3D(vector7), Agent.To3D(a2 * 0f), Color.blue);
							float num12 = 0f;
							Vector2 vector8 = vector7 - Vector2.one * 15f * 0.5f;
							Vector2 vector9 = this.Trace(vos, num, vector8, 0.01f, out num12);
							if ((vector8 - vector9).sqrMagnitude < this.Sqr(0.375f) * 2.6f)
							{
								UnityEngine.Debug.DrawRay(Agent.To3D(vector9 + vector), Vector3.up * 1f, Color.red);
							}
						}
					}
				}
				float num13 = float.PositiveInfinity;
				Vector2 vector10 = new Vector2(this.velocity.x, this.velocity.z);
				float cutoff = vector10.magnitude * this.simulator.qualityCutoff;
				vector6 = this.Trace(vos, num, new Vector2(this.desiredVelocity.x, this.desiredVelocity.z), cutoff, out num13);
				if (this.DebugDraw)
				{
					Agent.DrawCross(vector6 + vector, Color.yellow, 0.5f);
				}
				Vector2 p3 = this.Velocity;
				float num14;
				Vector2 vector11 = this.Trace(vos, num, p3, cutoff, out num14);
				if (num14 < num13)
				{
					vector6 = vector11;
					num13 = num14;
				}
				if (this.DebugDraw)
				{
					Agent.DrawCross(vector11 + vector, Color.magenta, 0.5f);
				}
			}
			else
			{
				Vector2[] samplePos = context.samplePos;
				float[] sampleSize = context.sampleSize;
				int num15 = 0;
				Vector2 vector12 = new Vector2(this.desiredVelocity.x, this.desiredVelocity.z);
				float num16 = Mathf.Max(this.radius, Mathf.Max(vector12.magnitude, this.Velocity.magnitude));
				samplePos[num15] = vector12;
				sampleSize[num15] = num16 * 0.3f;
				num15++;
				samplePos[num15] = vector2;
				sampleSize[num15] = num16 * 0.3f;
				num15++;
				Vector2 a3 = vector2 * 0.5f;
				Vector2 a4 = new Vector2(a3.y, -a3.x);
				for (int n = 0; n < 8; n++)
				{
					samplePos[num15] = a4 * Mathf.Sin((float)n * 3.14159274f * 2f / 8f) + a3 * (1f + Mathf.Cos((float)n * 3.14159274f * 2f / 8f));
					sampleSize[num15] = (1f - Mathf.Abs((float)n - 4f) / 8f) * num16 * 0.5f;
					num15++;
				}
				a3 *= 0.6f;
				a4 *= 0.6f;
				for (int num17 = 0; num17 < 6; num17++)
				{
					samplePos[num15] = a4 * Mathf.Cos(((float)num17 + 0.5f) * 3.14159274f * 2f / 6f) + a3 * (1.66666663f + Mathf.Sin(((float)num17 + 0.5f) * 3.14159274f * 2f / 6f));
					sampleSize[num15] = num16 * 0.3f;
					num15++;
				}
				for (int num18 = 0; num18 < 6; num18++)
				{
					samplePos[num15] = vector2 + new Vector2(num16 * 0.2f * Mathf.Cos(((float)num18 + 0.5f) * 3.14159274f * 2f / 6f), num16 * 0.2f * Mathf.Sin(((float)num18 + 0.5f) * 3.14159274f * 2f / 6f));
					sampleSize[num15] = num16 * 0.2f * 2f;
					num15++;
				}
				samplePos[num15] = vector2 * 0.5f;
				sampleSize[num15] = num16 * 0.4f;
				num15++;
				Vector2[] bestPos = context.bestPos;
				float[] bestSizes = context.bestSizes;
				float[] bestScores = context.bestScores;
				for (int num19 = 0; num19 < 3; num19++)
				{
					bestScores[num19] = float.PositiveInfinity;
				}
				bestScores[3] = float.NegativeInfinity;
				Vector2 vector13 = vector2;
				float num20 = float.PositiveInfinity;
				for (int num21 = 0; num21 < 3; num21++)
				{
					for (int num22 = 0; num22 < num15; num22++)
					{
						float num23 = 0f;
						for (int num24 = 0; num24 < num; num24++)
						{
							num23 = Math.Max(num23, vos[num24].ScalarSample(samplePos[num22]));
						}
						float magnitude = (samplePos[num22] - vector12).magnitude;
						float num25 = num23 + magnitude * Agent.DesiredVelocityWeight;
						num23 += magnitude * 0.001f;
						if (this.DebugDraw)
						{
							Agent.DrawCross(vector + samplePos[num22], Agent.Rainbow(Mathf.Log(num23 + 1f) * 5f), sampleSize[num22] * 0.5f);
						}
						if (num25 < bestScores[0])
						{
							for (int num26 = 0; num26 < 3; num26++)
							{
								if (num25 >= bestScores[num26 + 1])
								{
									bestScores[num26] = num25;
									bestSizes[num26] = sampleSize[num22];
									bestPos[num26] = samplePos[num22];
									break;
								}
							}
						}
						if (num23 < num20)
						{
							vector13 = samplePos[num22];
							num20 = num23;
							if (num23 == 0f)
							{
								num21 = 100;
								break;
							}
						}
					}
					num15 = 0;
					for (int num27 = 0; num27 < 3; num27++)
					{
						Vector2 a5 = bestPos[num27];
						float num28 = bestSizes[num27];
						bestScores[num27] = float.PositiveInfinity;
						float num29 = num28 * 0.6f * 0.5f;
						samplePos[num15] = a5 + new Vector2(num29, num29);
						samplePos[num15 + 1] = a5 + new Vector2(-num29, num29);
						samplePos[num15 + 2] = a5 + new Vector2(-num29, -num29);
						samplePos[num15 + 3] = a5 + new Vector2(num29, -num29);
						num28 *= num28 * 0.6f;
						sampleSize[num15] = num28;
						sampleSize[num15 + 1] = num28;
						sampleSize[num15 + 2] = num28;
						sampleSize[num15 + 3] = num28;
						num15 += 4;
					}
				}
				vector6 = vector13;
			}
			if (this.DebugDraw)
			{
				Agent.DrawCross(vector6 + vector, 1f);
			}
			this.newVelocity = Agent.To3D(Vector2.ClampMagnitude(vector6, this.maxSpeed));
		}

		private static Color Rainbow(float v)
		{
			Color result = new Color(v, 0f, 0f);
			if (result.r > 1f)
			{
				result.g = result.r - 1f;
				result.r = 1f;
			}
			if (result.g > 1f)
			{
				result.b = result.g - 1f;
				result.g = 1f;
			}
			return result;
		}

		private Vector2 Trace(Agent.VO[] vos, int voCount, Vector2 p, float cutoff, out float score)
		{
			score = 0f;
			float stepScale = this.simulator.stepScale;
			float num = float.PositiveInfinity;
			Vector2 result = p;
			for (int i = 0; i < 50; i++)
			{
				float num2 = 1f - (float)i / 50f;
				num2 *= stepScale;
				Vector2 vector = Vector2.zero;
				float num3 = 0f;
				for (int j = 0; j < voCount; j++)
				{
					float num4;
					Vector2 b = vos[j].Sample(p, out num4);
					vector += b;
					if (num4 > num3)
					{
						num3 = num4;
					}
				}
				Vector2 a = new Vector2(this.desiredVelocity.x, this.desiredVelocity.z) - p;
				float val = a.magnitude * Agent.DesiredVelocityWeight;
				vector += a * Agent.DesiredVelocityScale;
				num3 = Math.Max(num3, val);
				score = num3;
				if (score < num)
				{
					num = score;
				}
				result = p;
				if (score <= cutoff && i > 10)
				{
					break;
				}
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude > 0f)
				{
					vector *= num3 / Mathf.Sqrt(sqrMagnitude);
				}
				vector *= num2;
				Vector2 p2 = p;
				p += vector;
				if (this.DebugDraw)
				{
					UnityEngine.Debug.DrawLine(Agent.To3D(p2) + this.position, Agent.To3D(p) + this.position, Agent.Rainbow(0.1f / score) * new Color(1f, 1f, 1f, 0.2f));
				}
			}
			score = num;
			return result;
		}

		public static bool IntersectionFactor(Vector2 start1, Vector2 dir1, Vector2 start2, Vector2 dir2, out float factor)
		{
			float num = dir2.y * dir1.x - dir2.x * dir1.y;
			if (num == 0f)
			{
				factor = 0f;
				return false;
			}
			float num2 = dir2.x * (start1.y - start2.y) - dir2.y * (start1.x - start2.x);
			factor = num2 / num;
			return true;
		}
	}
}
