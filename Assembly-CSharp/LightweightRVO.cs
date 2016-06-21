using Pathfinding.RVO;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LightweightRVO : MonoBehaviour
{
	public enum RVOExampleType
	{
		Circle,
		Line,
		Point,
		RandomStreams
	}

	public int agentCount = 100;

	public float exampleScale = 100f;

	public LightweightRVO.RVOExampleType type;

	public float radius = 3f;

	public float maxSpeed = 2f;

	public float agentTimeHorizon = 10f;

	[HideInInspector]
	public float obstacleTimeHorizon = 10f;

	public int maxNeighbours = 10;

	public float neighbourDist = 15f;

	public Vector3 renderingOffset = Vector3.up * 0.1f;

	public bool debug;

	private Mesh mesh;

	private Simulator sim;

	private List<IAgent> agents;

	private List<Vector3> goals;

	private List<Color> colors;

	private Vector3[] verts;

	private Vector2[] uv;

	private int[] tris;

	private Color[] meshColors;

	private Vector3[] interpolatedVelocities;

	public void Start()
	{
		this.mesh = new Mesh();
		RVOSimulator rVOSimulator = UnityEngine.Object.FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
		if (rVOSimulator == null)
		{
			Debug.LogError("No RVOSimulator could be found in the scene. Please add a RVOSimulator component to any GameObject");
			return;
		}
		this.sim = rVOSimulator.GetSimulator();
		base.GetComponent<MeshFilter>().mesh = this.mesh;
		this.CreateAgents(this.agentCount);
	}

	public void OnGUI()
	{
		if (GUILayout.Button("2", new GUILayoutOption[0]))
		{
			this.CreateAgents(2);
		}
		if (GUILayout.Button("10", new GUILayoutOption[0]))
		{
			this.CreateAgents(10);
		}
		if (GUILayout.Button("100", new GUILayoutOption[0]))
		{
			this.CreateAgents(100);
		}
		if (GUILayout.Button("500", new GUILayoutOption[0]))
		{
			this.CreateAgents(500);
		}
		if (GUILayout.Button("1000", new GUILayoutOption[0]))
		{
			this.CreateAgents(1000);
		}
		if (GUILayout.Button("5000", new GUILayoutOption[0]))
		{
			this.CreateAgents(5000);
		}
		GUILayout.Space(5f);
		if (GUILayout.Button("Random Streams", new GUILayoutOption[0]))
		{
			this.type = LightweightRVO.RVOExampleType.RandomStreams;
			this.CreateAgents((this.agents == null) ? 100 : this.agents.Count);
		}
		if (GUILayout.Button("Line", new GUILayoutOption[0]))
		{
			this.type = LightweightRVO.RVOExampleType.Line;
			this.CreateAgents((this.agents == null) ? 10 : Mathf.Min(this.agents.Count, 100));
		}
		if (GUILayout.Button("Circle", new GUILayoutOption[0]))
		{
			this.type = LightweightRVO.RVOExampleType.Circle;
			this.CreateAgents((this.agents == null) ? 100 : this.agents.Count);
		}
		if (GUILayout.Button("Point", new GUILayoutOption[0]))
		{
			this.type = LightweightRVO.RVOExampleType.Point;
			this.CreateAgents((this.agents == null) ? 100 : this.agents.Count);
		}
	}

	private float uniformDistance(float radius)
	{
		float num = UnityEngine.Random.value + UnityEngine.Random.value;
		if (num > 1f)
		{
			return radius * (2f - num);
		}
		return radius * num;
	}

	public void CreateAgents(int num)
	{
		this.agentCount = num;
		this.agents = new List<IAgent>(this.agentCount);
		this.goals = new List<Vector3>(this.agentCount);
		this.colors = new List<Color>(this.agentCount);
		this.sim.ClearAgents();
		if (this.type == LightweightRVO.RVOExampleType.Circle)
		{
			float d = Mathf.Sqrt((float)this.agentCount * this.radius * this.radius * 4f / 3.14159274f) * this.exampleScale * 0.05f;
			for (int i = 0; i < this.agentCount; i++)
			{
				Vector3 vector = new Vector3(Mathf.Cos((float)i * 3.14159274f * 2f / (float)this.agentCount), 0f, Mathf.Sin((float)i * 3.14159274f * 2f / (float)this.agentCount)) * d;
				IAgent item = this.sim.AddAgent(vector);
				this.agents.Add(item);
				this.goals.Add(-vector);
				this.colors.Add(LightweightRVO.HSVToRGB((float)i * 360f / (float)this.agentCount, 0.8f, 0.6f));
			}
		}
		else if (this.type == LightweightRVO.RVOExampleType.Line)
		{
			for (int j = 0; j < this.agentCount; j++)
			{
				Vector3 position = new Vector3((float)((j % 2 != 0) ? -1 : 1) * this.exampleScale, 0f, (float)(j / 2) * this.radius * 2.5f);
				IAgent item2 = this.sim.AddAgent(position);
				this.agents.Add(item2);
				this.goals.Add(new Vector3(-position.x, position.y, position.z));
				this.colors.Add((j % 2 != 0) ? Color.blue : Color.red);
			}
		}
		else if (this.type == LightweightRVO.RVOExampleType.Point)
		{
			for (int k = 0; k < this.agentCount; k++)
			{
				Vector3 position2 = new Vector3(Mathf.Cos((float)k * 3.14159274f * 2f / (float)this.agentCount), 0f, Mathf.Sin((float)k * 3.14159274f * 2f / (float)this.agentCount)) * this.exampleScale;
				IAgent item3 = this.sim.AddAgent(position2);
				this.agents.Add(item3);
				this.goals.Add(new Vector3(0f, position2.y, 0f));
				this.colors.Add(LightweightRVO.HSVToRGB((float)k * 360f / (float)this.agentCount, 0.8f, 0.6f));
			}
		}
		else if (this.type == LightweightRVO.RVOExampleType.RandomStreams)
		{
			float num2 = Mathf.Sqrt((float)this.agentCount * this.radius * this.radius * 4f / 3.14159274f) * this.exampleScale * 0.05f;
			for (int l = 0; l < this.agentCount; l++)
			{
				float f = UnityEngine.Random.value * 3.14159274f * 2f;
				float num3 = UnityEngine.Random.value * 3.14159274f * 2f;
				Vector3 position3 = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * this.uniformDistance(num2);
				IAgent item4 = this.sim.AddAgent(position3);
				this.agents.Add(item4);
				this.goals.Add(new Vector3(Mathf.Cos(num3), 0f, Mathf.Sin(num3)) * this.uniformDistance(num2));
				this.colors.Add(LightweightRVO.HSVToRGB(num3 * 57.29578f, 0.8f, 0.6f));
			}
		}
		for (int m = 0; m < this.agents.Count; m++)
		{
			IAgent agent = this.agents[m];
			agent.Radius = this.radius;
			agent.MaxSpeed = this.maxSpeed;
			agent.AgentTimeHorizon = this.agentTimeHorizon;
			agent.ObstacleTimeHorizon = this.obstacleTimeHorizon;
			agent.MaxNeighbours = this.maxNeighbours;
			agent.NeighbourDist = this.neighbourDist;
			agent.DebugDraw = (m == 0 && this.debug);
		}
		this.verts = new Vector3[4 * this.agents.Count];
		this.uv = new Vector2[this.verts.Length];
		this.tris = new int[this.agents.Count * 2 * 3];
		this.meshColors = new Color[this.verts.Length];
	}

	public void Update()
	{
		if (this.agents == null || this.mesh == null)
		{
			return;
		}
		if (this.agents.Count != this.goals.Count)
		{
			Debug.LogError("Agent count does not match goal count");
			return;
		}
		for (int i = 0; i < this.agents.Count; i++)
		{
			Vector3 interpolatedPosition = this.agents[i].InterpolatedPosition;
			Vector3 vector = this.goals[i] - interpolatedPosition;
			vector = Vector3.ClampMagnitude(vector, 1f);
			this.agents[i].DesiredVelocity = vector * this.agents[i].MaxSpeed;
		}
		if (this.interpolatedVelocities == null || this.interpolatedVelocities.Length < this.agents.Count)
		{
			Vector3[] array = new Vector3[this.agents.Count];
			if (this.interpolatedVelocities != null)
			{
				for (int j = 0; j < this.interpolatedVelocities.Length; j++)
				{
					array[j] = this.interpolatedVelocities[j];
				}
			}
			this.interpolatedVelocities = array;
		}
		for (int k = 0; k < this.agents.Count; k++)
		{
			IAgent agent = this.agents[k];
			this.interpolatedVelocities[k] = Vector3.Lerp(this.interpolatedVelocities[k], agent.Velocity, agent.Velocity.magnitude * Time.deltaTime * 4f);
			Vector3 vector2 = this.interpolatedVelocities[k].normalized * agent.Radius;
			if (vector2 == Vector3.zero)
			{
				vector2 = new Vector3(0f, 0f, agent.Radius);
			}
			Vector3 b = Vector3.Cross(Vector3.up, vector2);
			Vector3 a = agent.InterpolatedPosition + this.renderingOffset;
			int num = 4 * k;
			int num2 = 6 * k;
			this.verts[num] = a + vector2 - b;
			this.verts[num + 1] = a + vector2 + b;
			this.verts[num + 2] = a - vector2 + b;
			this.verts[num + 3] = a - vector2 - b;
			this.uv[num] = new Vector2(0f, 1f);
			this.uv[num + 1] = new Vector2(1f, 1f);
			this.uv[num + 2] = new Vector2(1f, 0f);
			this.uv[num + 3] = new Vector2(0f, 0f);
			this.meshColors[num] = this.colors[k];
			this.meshColors[num + 1] = this.colors[k];
			this.meshColors[num + 2] = this.colors[k];
			this.meshColors[num + 3] = this.colors[k];
			this.tris[num2] = num;
			this.tris[num2 + 1] = num + 1;
			this.tris[num2 + 2] = num + 2;
			this.tris[num2 + 3] = num;
			this.tris[num2 + 4] = num + 2;
			this.tris[num2 + 5] = num + 3;
		}
		this.mesh.Clear();
		this.mesh.vertices = this.verts;
		this.mesh.uv = this.uv;
		this.mesh.colors = this.meshColors;
		this.mesh.triangles = this.tris;
		this.mesh.RecalculateNormals();
	}

	private static Color HSVToRGB(float h, float s, float v)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = s * v;
		float num5 = h / 60f;
		float num6 = num4 * (1f - Math.Abs(num5 % 2f - 1f));
		if (num5 < 1f)
		{
			num = num4;
			num2 = num6;
		}
		else if (num5 < 2f)
		{
			num = num6;
			num2 = num4;
		}
		else if (num5 < 3f)
		{
			num2 = num4;
			num3 = num6;
		}
		else if (num5 < 4f)
		{
			num2 = num6;
			num3 = num4;
		}
		else if (num5 < 5f)
		{
			num = num6;
			num3 = num4;
		}
		else if (num5 < 6f)
		{
			num = num4;
			num3 = num6;
		}
		float num7 = v - num4;
		num += num7;
		num2 += num7;
		num3 += num7;
		return new Color(num, num2, num3);
	}
}
