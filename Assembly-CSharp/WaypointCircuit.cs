using System;
using UnityEngine;

public class WaypointCircuit : MonoBehaviour
{
	[Serializable]
	public class WaypointList
	{
		public WaypointCircuit circuit;

		public Transform[] items = new Transform[0];
	}

	public struct RoutePoint
	{
		public Vector3 position;

		public Vector3 direction;

		public RoutePoint(Vector3 position, Vector3 direction)
		{
			this.position = position;
			this.direction = direction;
		}
	}

	public WaypointCircuit.WaypointList waypointList = new WaypointCircuit.WaypointList();

	[SerializeField]
	private bool smoothRoute = true;

	private int numPoints;

	private Vector3[] points;

	private float[] distances;

	public float editorVisualisationSubsteps = 100f;

	private int p0n;

	private int p1n;

	private int p2n;

	private int p3n;

	private float i;

	private Vector3 P0;

	private Vector3 P1;

	private Vector3 P2;

	private Vector3 P3;

	public float Length
	{
		get;
		private set;
	}

	public Transform[] Waypoints
	{
		get
		{
			return this.waypointList.items;
		}
	}

	private void Awake()
	{
		if (this.Waypoints.Length > 1)
		{
			this.CachePositionsAndDistances();
		}
		this.numPoints = this.Waypoints.Length;
	}

	public WaypointCircuit.RoutePoint GetRoutePoint(float dist)
	{
		Vector3 routePosition = this.GetRoutePosition(dist);
		Vector3 routePosition2 = this.GetRoutePosition(dist + 0.1f);
		return new WaypointCircuit.RoutePoint(routePosition, (routePosition2 - routePosition).normalized);
	}

	public Vector3 GetRoutePosition(float dist)
	{
		int num = 0;
		if (this.Length == 0f)
		{
			this.Length = this.distances[this.distances.Length - 1];
		}
		dist = Mathf.Repeat(dist, this.Length);
		while (this.distances[num] < dist)
		{
			num++;
		}
		this.p1n = (num - 1 + this.numPoints) % this.numPoints;
		this.p2n = num;
		this.i = Mathf.InverseLerp(this.distances[this.p1n], this.distances[this.p2n], dist);
		if (this.smoothRoute)
		{
			this.p0n = (num - 2 + this.numPoints) % this.numPoints;
			this.p3n = (num + 1) % this.numPoints;
			this.p2n %= this.numPoints;
			this.P0 = this.points[this.p0n];
			this.P1 = this.points[this.p1n];
			this.P2 = this.points[this.p2n];
			this.P3 = this.points[this.p3n];
			return this.CatmullRom(this.P0, this.P1, this.P2, this.P3, this.i);
		}
		this.p1n = (num - 1 + this.numPoints) % this.numPoints;
		this.p2n = num;
		return Vector3.Lerp(this.points[this.p1n], this.points[this.p2n], this.i);
	}

	private Vector3 CatmullRom(Vector3 _P0, Vector3 _P1, Vector3 _P2, Vector3 _P3, float _i)
	{
		return 0.5f * (2f * _P1 + (-_P0 + _P2) * _i + (2f * _P0 - 5f * _P1 + 4f * _P2 - _P3) * _i * _i + (-_P0 + 3f * _P1 - 3f * _P2 + _P3) * _i * _i * _i);
	}

	private void CachePositionsAndDistances()
	{
		this.points = new Vector3[this.Waypoints.Length + 1];
		this.distances = new float[this.Waypoints.Length + 1];
		float num = 0f;
		for (int i = 0; i < this.points.Length; i++)
		{
			Transform transform = this.Waypoints[i % this.Waypoints.Length];
			Transform transform2 = this.Waypoints[(i + 1) % this.Waypoints.Length];
			if (transform != null && transform2 != null)
			{
				Vector3 position = transform.position;
				Vector3 position2 = transform2.position;
				this.points[i] = this.Waypoints[i % this.Waypoints.Length].position;
				this.distances[i] = num;
				num += (position - position2).magnitude;
			}
		}
	}

	private void OnDrawGizmos()
	{
		this.DrawGizmos(false);
	}

	private void OnDrawGizmosSelected()
	{
		this.DrawGizmos(true);
	}

	private void DrawGizmos(bool selected)
	{
		this.waypointList.circuit = this;
		if (this.Waypoints.Length > 1)
		{
			this.numPoints = this.Waypoints.Length;
			this.CachePositionsAndDistances();
			this.Length = this.distances[this.distances.Length - 1];
			Gizmos.color = ((!selected) ? new Color(1f, 1f, 0f, 0.5f) : Color.yellow);
			Vector3 from = this.Waypoints[0].position;
			if (this.smoothRoute)
			{
				for (float num = 0f; num < this.Length; num += this.Length / this.editorVisualisationSubsteps)
				{
					Vector3 routePosition = this.GetRoutePosition(num + 1f);
					Gizmos.DrawLine(from, routePosition);
					from = routePosition;
				}
				Gizmos.DrawLine(from, this.Waypoints[0].position);
			}
			else
			{
				for (int i = 0; i < this.Waypoints.Length; i++)
				{
					Vector3 position = this.Waypoints[(i + 1) % this.Waypoints.Length].position;
					Gizmos.DrawLine(from, position);
					from = position;
				}
			}
		}
	}
}
