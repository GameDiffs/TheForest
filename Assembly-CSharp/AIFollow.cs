using Pathfinding;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Pathfinding/AI/AIFollow (deprecated)"), RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Seeker))]
public class AIFollow : MonoBehaviour
{
	public Transform target;

	public float repathRate = 0.1f;

	public float pickNextWaypointDistance = 1f;

	public float targetReached = 0.2f;

	public float speed = 5f;

	public float rotationSpeed = 1f;

	public bool drawGizmos;

	public bool canSearch = true;

	public bool canMove = true;

	protected Seeker seeker;

	protected CharacterController controller;

	protected NavmeshController navmeshController;

	protected Transform tr;

	protected float lastPathSearch = -9999f;

	protected int pathIndex;

	protected Vector3[] path;

	public void Start()
	{
		this.seeker = base.GetComponent<Seeker>();
		this.controller = base.GetComponent<CharacterController>();
		this.navmeshController = base.GetComponent<NavmeshController>();
		this.tr = base.transform;
		this.Repath();
	}

	public void Reset()
	{
		this.path = null;
	}

	public void OnPathComplete(Path p)
	{
		base.StartCoroutine(this.WaitToRepath());
		if (p.error)
		{
			return;
		}
		this.path = p.vectorPath.ToArray();
		float num = float.PositiveInfinity;
		int num2 = 0;
		for (int i = 0; i < this.path.Length - 1; i++)
		{
			float num3 = AstarMath.DistancePointSegmentStrict(this.path[i], this.path[i + 1], this.tr.position);
			if (num3 < num)
			{
				num2 = 0;
				num = num3;
				this.pathIndex = i + 1;
			}
			else if (num2 > 6)
			{
				break;
			}
		}
	}

	[DebuggerHidden]
	public IEnumerator WaitToRepath()
	{
		AIFollow.<WaitToRepath>c__Iterator10 <WaitToRepath>c__Iterator = new AIFollow.<WaitToRepath>c__Iterator10();
		<WaitToRepath>c__Iterator.<>f__this = this;
		return <WaitToRepath>c__Iterator;
	}

	public void Stop()
	{
		this.canMove = false;
		this.canSearch = false;
	}

	public void Resume()
	{
		this.canMove = true;
		this.canSearch = true;
	}

	public virtual void Repath()
	{
		this.lastPathSearch = Time.time;
		if (this.seeker == null || this.target == null || !this.canSearch || !this.seeker.IsDone())
		{
			base.StartCoroutine(this.WaitToRepath());
			return;
		}
		Path p = ABPath.Construct(base.transform.position, this.target.position, null);
		this.seeker.StartPath(p, new OnPathDelegate(this.OnPathComplete), -1);
	}

	public void PathToTarget(Vector3 targetPoint)
	{
		this.lastPathSearch = Time.time;
		if (this.seeker == null)
		{
			return;
		}
		this.seeker.StartPath(base.transform.position, targetPoint, new OnPathDelegate(this.OnPathComplete));
	}

	public virtual void ReachedEndOfPath()
	{
	}

	public void Update()
	{
		if (this.path == null || this.pathIndex >= this.path.Length || this.pathIndex < 0 || !this.canMove)
		{
			return;
		}
		Vector3 a = this.path[this.pathIndex];
		a.y = this.tr.position.y;
		while ((a - this.tr.position).sqrMagnitude < this.pickNextWaypointDistance * this.pickNextWaypointDistance)
		{
			this.pathIndex++;
			if (this.pathIndex >= this.path.Length)
			{
				if ((a - this.tr.position).sqrMagnitude < this.pickNextWaypointDistance * this.targetReached * (this.pickNextWaypointDistance * this.targetReached))
				{
					this.ReachedEndOfPath();
					return;
				}
				this.pathIndex--;
				break;
			}
			else
			{
				a = this.path[this.pathIndex];
				a.y = this.tr.position.y;
			}
		}
		Vector3 forward = a - this.tr.position;
		this.tr.rotation = Quaternion.Slerp(this.tr.rotation, Quaternion.LookRotation(forward), this.rotationSpeed * Time.deltaTime);
		this.tr.eulerAngles = new Vector3(0f, this.tr.eulerAngles.y, 0f);
		Vector3 a2 = base.transform.forward;
		a2 *= this.speed;
		a2 *= Mathf.Clamp01(Vector3.Dot(forward.normalized, this.tr.forward));
		if (!(this.navmeshController != null))
		{
			if (this.controller != null)
			{
				this.controller.SimpleMove(a2);
			}
			else
			{
				base.transform.Translate(a2 * Time.deltaTime, Space.World);
			}
		}
	}

	public void OnDrawGizmos()
	{
		if (!this.drawGizmos || this.path == null || this.pathIndex >= this.path.Length || this.pathIndex < 0)
		{
			return;
		}
		Vector3 vector = this.path[this.pathIndex];
		vector.y = this.tr.position.y;
		UnityEngine.Debug.DrawLine(base.transform.position, vector, Color.blue);
		float num = this.pickNextWaypointDistance;
		if (this.pathIndex == this.path.Length - 1)
		{
			num *= this.targetReached;
		}
		Vector3 start = vector + num * new Vector3(1f, 0f, 0f);
		float num2 = 0f;
		while ((double)num2 < 6.2831853071795862)
		{
			Vector3 vector2 = vector + new Vector3((float)Math.Cos((double)num2) * num, 0f, (float)Math.Sin((double)num2) * num);
			UnityEngine.Debug.DrawLine(start, vector2, Color.yellow);
			start = vector2;
			num2 += 0.1f;
		}
		UnityEngine.Debug.DrawLine(start, vector + num * new Vector3(1f, 0f, 0f), Color.yellow);
	}
}
