using Pathfinding.RVO.Sampled;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Pathfinding.RVO
{
	public class Simulator
	{
		public enum SamplingAlgorithm
		{
			AdaptiveSampling,
			GradientDecent
		}

		internal class WorkerContext
		{
			public const int KeepCount = 3;

			public Agent.VO[] vos = new Agent.VO[20];

			public Vector2[] bestPos = new Vector2[3];

			public float[] bestSizes = new float[3];

			public float[] bestScores = new float[4];

			public Vector2[] samplePos = new Vector2[50];

			public float[] sampleSize = new float[50];
		}

		private class Worker
		{
			[NonSerialized]
			public Thread thread;

			public int start;

			public int end;

			public int task;

			public AutoResetEvent runFlag = new AutoResetEvent(false);

			public ManualResetEvent waitFlag = new ManualResetEvent(true);

			public Simulator simulator;

			private bool terminate;

			private Simulator.WorkerContext context = new Simulator.WorkerContext();

			public Worker(Simulator sim)
			{
				this.simulator = sim;
				this.thread = new Thread(new ThreadStart(this.Run));
				this.thread.IsBackground = true;
				this.thread.Name = "RVO Simulator Thread";
				this.thread.Start();
			}

			public void Execute(int task)
			{
				this.task = task;
				this.waitFlag.Reset();
				this.runFlag.Set();
			}

			public void WaitOne()
			{
				this.waitFlag.WaitOne();
			}

			public void Terminate()
			{
				this.terminate = true;
			}

			public void Run()
			{
				this.runFlag.WaitOne();
				while (!this.terminate)
				{
					try
					{
						List<Agent> agents = this.simulator.GetAgents();
						if (this.task == 0)
						{
							for (int i = this.start; i < this.end; i++)
							{
								agents[i].CalculateNeighbours();
								agents[i].CalculateVelocity(this.context);
							}
						}
						else if (this.task == 1)
						{
							for (int j = this.start; j < this.end; j++)
							{
								agents[j].Update();
								agents[j].BufferSwitch();
							}
						}
						else
						{
							if (this.task != 2)
							{
								Debug.LogError("Invalid Task Number: " + this.task);
								throw new Exception("Invalid Task Number: " + this.task);
							}
							this.simulator.BuildQuadtree();
						}
					}
					catch (Exception message)
					{
						Debug.LogError(message);
					}
					this.waitFlag.Set();
					this.runFlag.WaitOne();
				}
			}
		}

		private bool doubleBuffering = true;

		private float desiredDeltaTime = 0.05f;

		private bool interpolation = true;

		private Simulator.Worker[] workers;

		private List<Agent> agents;

		public List<ObstacleVertex> obstacles;

		public Simulator.SamplingAlgorithm algorithm;

		private RVOQuadtree quadtree = new RVOQuadtree();

		public float qualityCutoff = 0.05f;

		public float stepScale = 1.5f;

		private float deltaTime;

		private float prevDeltaTime;

		private float lastStep = -99999f;

		private float lastStepInterpolationReference = -9999f;

		private bool doUpdateObstacles;

		private bool doCleanObstacles;

		private bool oversampling;

		private float wallThickness = 1f;

		private Simulator.WorkerContext coroutineWorkerContext = new Simulator.WorkerContext();

		public RVOQuadtree Quadtree
		{
			get
			{
				return this.quadtree;
			}
		}

		public float DeltaTime
		{
			get
			{
				return this.deltaTime;
			}
		}

		public float PrevDeltaTime
		{
			get
			{
				return this.prevDeltaTime;
			}
		}

		public bool Multithreading
		{
			get
			{
				return this.workers != null && this.workers.Length > 0;
			}
		}

		public float DesiredDeltaTime
		{
			get
			{
				return this.desiredDeltaTime;
			}
			set
			{
				this.desiredDeltaTime = Math.Max(value, 0f);
			}
		}

		public float WallThickness
		{
			get
			{
				return this.wallThickness;
			}
			set
			{
				this.wallThickness = Math.Max(value, 0f);
			}
		}

		public bool Interpolation
		{
			get
			{
				return this.interpolation;
			}
			set
			{
				this.interpolation = value;
			}
		}

		public bool Oversampling
		{
			get
			{
				return this.oversampling;
			}
			set
			{
				this.oversampling = value;
			}
		}

		public Simulator(int workers, bool doubleBuffering)
		{
			this.workers = new Simulator.Worker[workers];
			this.doubleBuffering = doubleBuffering;
			for (int i = 0; i < workers; i++)
			{
				this.workers[i] = new Simulator.Worker(this);
			}
			this.agents = new List<Agent>();
			this.obstacles = new List<ObstacleVertex>();
		}

		public List<Agent> GetAgents()
		{
			return this.agents;
		}

		public List<ObstacleVertex> GetObstacles()
		{
			return this.obstacles;
		}

		public void ClearAgents()
		{
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			for (int j = 0; j < this.agents.Count; j++)
			{
				this.agents[j].simulator = null;
			}
			this.agents.Clear();
		}

		public void OnDestroy()
		{
			if (this.workers != null)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].Terminate();
				}
			}
		}

		~Simulator()
		{
			this.OnDestroy();
		}

		public IAgent AddAgent(IAgent agent)
		{
			if (agent == null)
			{
				throw new ArgumentNullException("Agent must not be null");
			}
			Agent agent2 = agent as Agent;
			if (agent2 == null)
			{
				throw new ArgumentException("The agent must be of type Agent. Agent was of type " + agent.GetType());
			}
			if (agent2.simulator != null && agent2.simulator == this)
			{
				throw new ArgumentException("The agent is already in the simulation");
			}
			if (agent2.simulator != null)
			{
				throw new ArgumentException("The agent is already added to another simulation");
			}
			agent2.simulator = this;
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.agents.Add(agent2);
			return agent;
		}

		public IAgent AddAgent(Vector3 position)
		{
			Agent agent = new Agent(position);
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.agents.Add(agent);
			agent.simulator = this;
			return agent;
		}

		public void RemoveAgent(IAgent agent)
		{
			if (agent == null)
			{
				throw new ArgumentNullException("Agent must not be null");
			}
			Agent agent2 = agent as Agent;
			if (agent2 == null)
			{
				throw new ArgumentException("The agent must be of type Agent. Agent was of type " + agent.GetType());
			}
			if (agent2.simulator != this)
			{
				throw new ArgumentException("The agent is not added to this simulation");
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			agent2.simulator = null;
			if (!this.agents.Remove(agent2))
			{
				throw new ArgumentException("Critical Bug! This should not happen. Please report this.");
			}
		}

		public ObstacleVertex AddObstacle(ObstacleVertex v)
		{
			if (v == null)
			{
				throw new ArgumentNullException("Obstacle must not be null");
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.obstacles.Add(v);
			this.UpdateObstacles();
			return v;
		}

		public ObstacleVertex AddObstacle(Vector3[] vertices, float height)
		{
			return this.AddObstacle(vertices, height, Matrix4x4.identity, RVOLayer.DefaultObstacle);
		}

		public ObstacleVertex AddObstacle(Vector3[] vertices, float height, Matrix4x4 matrix, RVOLayer layer = RVOLayer.DefaultObstacle)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("Vertices must not be null");
			}
			if (vertices.Length < 2)
			{
				throw new ArgumentException("Less than 2 vertices in an obstacle");
			}
			ObstacleVertex obstacleVertex = null;
			ObstacleVertex obstacleVertex2 = null;
			bool flag = matrix == Matrix4x4.identity;
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			for (int j = 0; j < vertices.Length; j++)
			{
				ObstacleVertex obstacleVertex3 = new ObstacleVertex();
				if (obstacleVertex == null)
				{
					obstacleVertex = obstacleVertex3;
				}
				else
				{
					obstacleVertex2.next = obstacleVertex3;
				}
				obstacleVertex3.prev = obstacleVertex2;
				obstacleVertex3.layer = layer;
				obstacleVertex3.position = ((!flag) ? matrix.MultiplyPoint3x4(vertices[j]) : vertices[j]);
				obstacleVertex3.height = height;
				obstacleVertex2 = obstacleVertex3;
			}
			obstacleVertex2.next = obstacleVertex;
			obstacleVertex.prev = obstacleVertex2;
			ObstacleVertex obstacleVertex4 = obstacleVertex;
			do
			{
				Vector3 vector = obstacleVertex4.next.position - obstacleVertex4.position;
				ObstacleVertex arg_13E_0 = obstacleVertex4;
				Vector2 vector2 = new Vector2(vector.x, vector.z);
				arg_13E_0.dir = vector2.normalized;
				obstacleVertex4 = obstacleVertex4.next;
			}
			while (obstacleVertex4 != obstacleVertex);
			this.obstacles.Add(obstacleVertex);
			this.UpdateObstacles();
			return obstacleVertex;
		}

		public ObstacleVertex AddObstacle(Vector3 a, Vector3 b, float height)
		{
			ObstacleVertex obstacleVertex = new ObstacleVertex();
			ObstacleVertex obstacleVertex2 = new ObstacleVertex();
			obstacleVertex.layer = RVOLayer.DefaultObstacle;
			obstacleVertex2.layer = RVOLayer.DefaultObstacle;
			obstacleVertex.prev = obstacleVertex2;
			obstacleVertex2.prev = obstacleVertex;
			obstacleVertex.next = obstacleVertex2;
			obstacleVertex2.next = obstacleVertex;
			obstacleVertex.position = a;
			obstacleVertex2.position = b;
			obstacleVertex.height = height;
			obstacleVertex2.height = height;
			obstacleVertex2.ignore = true;
			ObstacleVertex arg_86_0 = obstacleVertex;
			Vector2 vector = new Vector2(b.x - a.x, b.z - a.z);
			arg_86_0.dir = vector.normalized;
			obstacleVertex2.dir = -obstacleVertex.dir;
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.obstacles.Add(obstacleVertex);
			this.UpdateObstacles();
			return obstacleVertex;
		}

		public void UpdateObstacle(ObstacleVertex obstacle, Vector3[] vertices, Matrix4x4 matrix)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("Vertices must not be null");
			}
			if (obstacle == null)
			{
				throw new ArgumentNullException("Obstacle must not be null");
			}
			if (vertices.Length < 2)
			{
				throw new ArgumentException("Less than 2 vertices in an obstacle");
			}
			if (obstacle.split)
			{
				throw new ArgumentException("Obstacle is not a start vertex. You should only pass those ObstacleVertices got from AddObstacle method calls");
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			int num = 0;
			ObstacleVertex obstacleVertex = obstacle;
			while (true)
			{
				while (obstacleVertex.next.split)
				{
					obstacleVertex.next = obstacleVertex.next.next;
					obstacleVertex.next.prev = obstacleVertex;
				}
				if (num >= vertices.Length)
				{
					break;
				}
				obstacleVertex.position = matrix.MultiplyPoint3x4(vertices[num]);
				num++;
				obstacleVertex = obstacleVertex.next;
				if (obstacleVertex == obstacle)
				{
					goto Block_9;
				}
			}
			Debug.DrawLine(obstacleVertex.prev.position, obstacleVertex.position, Color.red);
			throw new ArgumentException("Obstacle has more vertices than supplied for updating (" + vertices.Length + " supplied)");
			Block_9:
			obstacleVertex = obstacle;
			do
			{
				Vector3 vector = obstacleVertex.next.position - obstacleVertex.position;
				ObstacleVertex arg_160_0 = obstacleVertex;
				Vector2 vector2 = new Vector2(vector.x, vector.z);
				arg_160_0.dir = vector2.normalized;
				obstacleVertex = obstacleVertex.next;
			}
			while (obstacleVertex != obstacle);
			this.ScheduleCleanObstacles();
			this.UpdateObstacles();
		}

		private void ScheduleCleanObstacles()
		{
			this.doCleanObstacles = true;
		}

		private void CleanObstacles()
		{
			for (int i = 0; i < this.obstacles.Count; i++)
			{
				ObstacleVertex obstacleVertex = this.obstacles[i];
				ObstacleVertex obstacleVertex2 = obstacleVertex;
				do
				{
					while (obstacleVertex2.next.split)
					{
						obstacleVertex2.next = obstacleVertex2.next.next;
						obstacleVertex2.next.prev = obstacleVertex2;
					}
					obstacleVertex2 = obstacleVertex2.next;
				}
				while (obstacleVertex2 != obstacleVertex);
			}
		}

		public void RemoveObstacle(ObstacleVertex v)
		{
			if (v == null)
			{
				throw new ArgumentNullException("Vertex must not be null");
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.obstacles.Remove(v);
			this.UpdateObstacles();
		}

		public void UpdateObstacles()
		{
			this.doUpdateObstacles = true;
		}

		private void BuildQuadtree()
		{
			this.quadtree.Clear();
			if (this.agents.Count > 0)
			{
				Rect bounds = Rect.MinMaxRect(this.agents[0].position.x, this.agents[0].position.y, this.agents[0].position.x, this.agents[0].position.y);
				for (int i = 1; i < this.agents.Count; i++)
				{
					Vector3 position = this.agents[i].position;
					bounds = Rect.MinMaxRect(Mathf.Min(bounds.xMin, position.x), Mathf.Min(bounds.yMin, position.z), Mathf.Max(bounds.xMax, position.x), Mathf.Max(bounds.yMax, position.z));
				}
				this.quadtree.SetBounds(bounds);
				for (int j = 0; j < this.agents.Count; j++)
				{
					this.quadtree.Insert(this.agents[j]);
				}
			}
		}

		public void Update()
		{
			if (this.lastStep < 0f)
			{
				this.lastStep = Time.time;
				this.deltaTime = this.DesiredDeltaTime;
				this.prevDeltaTime = this.deltaTime;
				this.lastStepInterpolationReference = this.lastStep;
			}
			if (Time.time - this.lastStep >= this.DesiredDeltaTime)
			{
				for (int i = 0; i < this.agents.Count; i++)
				{
					this.agents[i].Interpolate((Time.time - this.lastStepInterpolationReference) / this.DeltaTime);
				}
				this.lastStepInterpolationReference = Time.time;
				this.prevDeltaTime = this.DeltaTime;
				this.deltaTime = Time.time - this.lastStep;
				this.lastStep = Time.time;
				this.deltaTime = Math.Max(this.deltaTime, 0.0005f);
				if (this.Multithreading)
				{
					if (this.doubleBuffering)
					{
						for (int j = 0; j < this.workers.Length; j++)
						{
							this.workers[j].WaitOne();
						}
						if (!this.Interpolation)
						{
							for (int k = 0; k < this.agents.Count; k++)
							{
								this.agents[k].Interpolate(1f);
							}
						}
					}
					if (this.doCleanObstacles)
					{
						this.CleanObstacles();
						this.doCleanObstacles = false;
						this.doUpdateObstacles = true;
					}
					if (this.doUpdateObstacles)
					{
						this.doUpdateObstacles = false;
					}
					this.BuildQuadtree();
					for (int l = 0; l < this.workers.Length; l++)
					{
						this.workers[l].start = l * this.agents.Count / this.workers.Length;
						this.workers[l].end = (l + 1) * this.agents.Count / this.workers.Length;
					}
					for (int m = 0; m < this.workers.Length; m++)
					{
						this.workers[m].Execute(1);
					}
					for (int n = 0; n < this.workers.Length; n++)
					{
						this.workers[n].WaitOne();
					}
					for (int num = 0; num < this.workers.Length; num++)
					{
						this.workers[num].Execute(0);
					}
					if (!this.doubleBuffering)
					{
						for (int num2 = 0; num2 < this.workers.Length; num2++)
						{
							this.workers[num2].WaitOne();
						}
						if (!this.Interpolation)
						{
							for (int num3 = 0; num3 < this.agents.Count; num3++)
							{
								this.agents[num3].Interpolate(1f);
							}
						}
					}
				}
				else
				{
					if (this.doCleanObstacles)
					{
						this.CleanObstacles();
						this.doCleanObstacles = false;
						this.doUpdateObstacles = true;
					}
					if (this.doUpdateObstacles)
					{
						this.doUpdateObstacles = false;
					}
					this.BuildQuadtree();
					for (int num4 = 0; num4 < this.agents.Count; num4++)
					{
						this.agents[num4].Update();
						this.agents[num4].BufferSwitch();
					}
					for (int num5 = 0; num5 < this.agents.Count; num5++)
					{
						this.agents[num5].CalculateNeighbours();
						this.agents[num5].CalculateVelocity(this.coroutineWorkerContext);
					}
					if (this.oversampling)
					{
						for (int num6 = 0; num6 < this.agents.Count; num6++)
						{
							this.agents[num6].Velocity = this.agents[num6].newVelocity;
						}
						for (int num7 = 0; num7 < this.agents.Count; num7++)
						{
							Vector3 newVelocity = this.agents[num7].newVelocity;
							this.agents[num7].CalculateVelocity(this.coroutineWorkerContext);
							this.agents[num7].newVelocity = (newVelocity + this.agents[num7].newVelocity) * 0.5f;
						}
					}
					if (!this.Interpolation)
					{
						for (int num8 = 0; num8 < this.agents.Count; num8++)
						{
							this.agents[num8].Interpolate(1f);
						}
					}
				}
			}
			if (this.Interpolation)
			{
				for (int num9 = 0; num9 < this.agents.Count; num9++)
				{
					this.agents[num9].Interpolate((Time.time - this.lastStepInterpolationReference) / this.DeltaTime);
				}
			}
		}
	}
}
