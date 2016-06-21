using System;
using UnityEngine;

public class WaypointProgressTracker : MonoBehaviour
{
	public enum ProgressStyle
	{
		SmoothAlongRoute,
		PointToPoint
	}

	[SerializeField]
	private WaypointCircuit circuit;

	[SerializeField]
	private float lookAheadForTargetOffset = 5f;

	[SerializeField]
	private float lookAheadForTargetFactor = 0.1f;

	[SerializeField]
	private float lookAheadForSpeedOffset = 10f;

	[SerializeField]
	private float lookAheadForSpeedFactor = 0.2f;

	[SerializeField]
	private WaypointProgressTracker.ProgressStyle progressStyle;

	[SerializeField]
	private float pointToPointThreshold = 4f;

	public Transform target;

	private float progressDistance;

	private int progressNum;

	private Vector3 lastPosition;

	private float speed;

	public WaypointCircuit.RoutePoint targetPoint
	{
		get;
		private set;
	}

	public WaypointCircuit.RoutePoint speedPoint
	{
		get;
		private set;
	}

	public WaypointCircuit.RoutePoint progressPoint
	{
		get;
		private set;
	}

	private void Start()
	{
		if (this.target == null)
		{
			this.target = new GameObject(base.name + " Waypoint Target").transform;
		}
		this.Reset();
	}

	public void Reset()
	{
		this.progressDistance = 0f;
		this.progressNum = 0;
		if (this.progressStyle == WaypointProgressTracker.ProgressStyle.PointToPoint)
		{
			this.target.position = this.circuit.Waypoints[this.progressNum].position;
			this.target.rotation = this.circuit.Waypoints[this.progressNum].rotation;
		}
	}

	private void Update()
	{
		if (this.progressStyle == WaypointProgressTracker.ProgressStyle.SmoothAlongRoute)
		{
			if (Time.deltaTime > 0f)
			{
				this.speed = Mathf.Lerp(this.speed, (this.lastPosition - base.transform.position).magnitude / Time.deltaTime, Time.deltaTime);
			}
			this.target.position = this.circuit.GetRoutePoint(this.progressDistance + this.lookAheadForTargetOffset + this.lookAheadForTargetFactor * this.speed).position;
			this.target.rotation = Quaternion.LookRotation(this.circuit.GetRoutePoint(this.progressDistance + this.lookAheadForSpeedOffset + this.lookAheadForSpeedFactor * this.speed).direction);
			this.progressPoint = this.circuit.GetRoutePoint(this.progressDistance);
			Vector3 lhs = this.progressPoint.position - base.transform.position;
			if (Vector3.Dot(lhs, this.progressPoint.direction) < 0f)
			{
				this.progressDistance += lhs.magnitude * 0.5f;
			}
			this.lastPosition = base.transform.position;
		}
		else
		{
			if ((this.target.position - base.transform.position).magnitude < this.pointToPointThreshold)
			{
				this.progressNum = (this.progressNum + 1) % this.circuit.Waypoints.Length;
			}
			this.target.position = this.circuit.Waypoints[this.progressNum].position;
			this.target.rotation = this.circuit.Waypoints[this.progressNum].rotation;
			this.progressPoint = this.circuit.GetRoutePoint(this.progressDistance);
			Vector3 lhs2 = this.progressPoint.position - base.transform.position;
			if (Vector3.Dot(lhs2, this.progressPoint.direction) < 0f)
			{
				this.progressDistance += lhs2.magnitude;
			}
			this.lastPosition = base.transform.position;
		}
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, this.target.position);
			Gizmos.DrawWireSphere(this.circuit.GetRoutePosition(this.progressDistance), 1f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.target.position, this.target.position + this.target.forward);
		}
	}
}
