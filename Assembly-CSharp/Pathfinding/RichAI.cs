using Pathfinding.RVO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/AI/RichAI (for navmesh)"), RequireComponent(typeof(Seeker))]
	public class RichAI : MonoBehaviour
	{
		public Transform target;

		public bool drawGizmos = true;

		public bool repeatedlySearchPaths;

		public float repathRate = 0.5f;

		public float maxSpeed = 1f;

		public float acceleration = 5f;

		public float slowdownTime = 0.5f;

		public float rotationSpeed = 360f;

		public float endReachedDistance = 0.01f;

		public float wallForce = 3f;

		public float wallDist = 1f;

		public Vector3 gravity = new Vector3(0f, -9.82f, 0f);

		public bool raycastingForGroundPlacement = true;

		public LayerMask groundMask = -1;

		public float centerOffset = 1f;

		public RichFunnel.FunnelSimplification funnelSimplification;

		public Animation anim;

		public bool preciseSlowdown = true;

		public bool slowWhenNotFacingTarget = true;

		private Vector3 velocity;

		protected RichPath rp;

		protected Seeker seeker;

		protected Transform tr;

		private CharacterController controller;

		private RVOController rvoController;

		private Vector3 lastTargetPoint;

		private Vector3 currentTargetDirection;

		protected bool waitingForPathCalc;

		protected bool canSearchPath;

		protected bool delayUpdatePath;

		protected bool traversingSpecialPath;

		protected bool lastCorner;

		private float distanceToWaypoint = 999f;

		protected List<Vector3> buffer = new List<Vector3>();

		protected List<Vector3> wallBuffer = new List<Vector3>();

		private bool startHasRun;

		protected float lastRepath = -9999f;

		private static float deltaTime = 0f;

		public static readonly Color GizmoColorRaycast = new Color(0.4627451f, 0.807843149f, 0.4392157f);

		public static readonly Color GizmoColorPath = new Color(0.03137255f, 0.305882365f, 0.7607843f);

		public Vector3 Velocity
		{
			get
			{
				return this.velocity;
			}
		}

		public bool TraversingSpecial
		{
			get
			{
				return this.traversingSpecialPath;
			}
		}

		public Vector3 TargetPoint
		{
			get
			{
				return this.lastTargetPoint;
			}
		}

		public bool ApproachingPartEndpoint
		{
			get
			{
				return this.lastCorner;
			}
		}

		public bool ApproachingPathEndpoint
		{
			get
			{
				return this.rp != null && this.ApproachingPartEndpoint && !this.rp.PartsLeft();
			}
		}

		public float DistanceToNextWaypoint
		{
			get
			{
				return this.distanceToWaypoint;
			}
		}

		private void Awake()
		{
			this.seeker = base.GetComponent<Seeker>();
			this.controller = base.GetComponent<CharacterController>();
			this.rvoController = base.GetComponent<RVOController>();
			if (this.rvoController != null)
			{
				this.rvoController.enableRotation = false;
			}
			this.tr = base.transform;
		}

		protected virtual void Start()
		{
			this.startHasRun = true;
			this.OnEnable();
		}

		protected virtual void OnEnable()
		{
			this.lastRepath = -9999f;
			this.waitingForPathCalc = false;
			this.canSearchPath = true;
			if (this.startHasRun)
			{
				Seeker expr_2A = this.seeker;
				expr_2A.pathCallback = (OnPathDelegate)Delegate.Combine(expr_2A.pathCallback, new OnPathDelegate(this.OnPathComplete));
				base.StartCoroutine(this.SearchPaths());
			}
		}

		public void OnDisable()
		{
			if (this.seeker != null && !this.seeker.IsDone())
			{
				this.seeker.GetCurrentPath().Error();
			}
			Seeker expr_37 = this.seeker;
			expr_37.pathCallback = (OnPathDelegate)Delegate.Remove(expr_37.pathCallback, new OnPathDelegate(this.OnPathComplete));
		}

		public virtual void UpdatePath()
		{
			this.canSearchPath = true;
			this.waitingForPathCalc = false;
			Path currentPath = this.seeker.GetCurrentPath();
			if (currentPath != null && !this.seeker.IsDone())
			{
				currentPath.Error();
				currentPath.Claim(this);
				currentPath.Release(this);
			}
			this.waitingForPathCalc = true;
			this.lastRepath = Time.time;
			this.seeker.StartPath(this.tr.position, this.target.position);
		}

		[DebuggerHidden]
		private IEnumerator SearchPaths()
		{
			RichAI.<SearchPaths>c__Iterator1 <SearchPaths>c__Iterator = new RichAI.<SearchPaths>c__Iterator1();
			<SearchPaths>c__Iterator.<>f__this = this;
			return <SearchPaths>c__Iterator;
		}

		private void OnPathComplete(Path p)
		{
			this.waitingForPathCalc = false;
			p.Claim(this);
			if (p.error)
			{
				p.Release(this);
				return;
			}
			if (this.traversingSpecialPath)
			{
				this.delayUpdatePath = true;
			}
			else
			{
				if (this.rp == null)
				{
					this.rp = new RichPath();
				}
				this.rp.Initialize(this.seeker, p, true, this.funnelSimplification);
			}
			p.Release(this);
		}

		private void NextPart()
		{
			this.rp.NextPart();
			this.lastCorner = false;
			if (!this.rp.PartsLeft())
			{
				this.OnTargetReached();
			}
		}

		protected virtual void OnTargetReached()
		{
		}

		protected virtual Vector3 UpdateTarget(RichFunnel fn)
		{
			this.buffer.Clear();
			Vector3 vector = this.tr.position;
			bool flag;
			vector = fn.Update(vector, this.buffer, 2, out this.lastCorner, out flag);
			if (flag && !this.waitingForPathCalc)
			{
				this.UpdatePath();
			}
			return vector;
		}

		protected virtual void Update()
		{
			RichAI.deltaTime = Mathf.Min(Time.smoothDeltaTime * 2f, Time.deltaTime);
			if (this.rp != null)
			{
				RichPathPart currentPart = this.rp.GetCurrentPart();
				RichFunnel richFunnel = currentPart as RichFunnel;
				if (richFunnel != null)
				{
					Vector3 vector = this.UpdateTarget(richFunnel);
					if (Time.frameCount % 5 == 0)
					{
						this.wallBuffer.Clear();
						richFunnel.FindWalls(this.wallBuffer, this.wallDist);
					}
					int num = 0;
					Vector3 vector2 = this.buffer[num];
					Vector3 vector3 = vector2 - vector;
					vector3.y = 0f;
					bool flag = Vector3.Dot(vector3, this.currentTargetDirection) < 0f;
					if (flag && this.buffer.Count - num > 1)
					{
						num++;
						vector2 = this.buffer[num];
					}
					if (vector2 != this.lastTargetPoint)
					{
						this.currentTargetDirection = vector2 - vector;
						this.currentTargetDirection.y = 0f;
						this.currentTargetDirection.Normalize();
						this.lastTargetPoint = vector2;
					}
					vector3 = vector2 - vector;
					vector3.y = 0f;
					float magnitude = vector3.magnitude;
					this.distanceToWaypoint = magnitude;
					vector3 = ((magnitude != 0f) ? (vector3 / magnitude) : Vector3.zero);
					Vector3 lhs = vector3;
					Vector3 a = Vector3.zero;
					if (this.wallForce > 0f && this.wallDist > 0f)
					{
						float num2 = 0f;
						float num3 = 0f;
						for (int i = 0; i < this.wallBuffer.Count; i += 2)
						{
							Vector3 a2 = AstarMath.NearestPointStrict(this.wallBuffer[i], this.wallBuffer[i + 1], this.tr.position);
							float sqrMagnitude = (a2 - vector).sqrMagnitude;
							if (sqrMagnitude <= this.wallDist * this.wallDist)
							{
								Vector3 normalized = (this.wallBuffer[i + 1] - this.wallBuffer[i]).normalized;
								float num4 = Vector3.Dot(vector3, normalized) * (1f - Math.Max(0f, 2f * (sqrMagnitude / (this.wallDist * this.wallDist)) - 1f));
								if (num4 > 0f)
								{
									num3 = Math.Max(num3, num4);
								}
								else
								{
									num2 = Math.Max(num2, -num4);
								}
							}
						}
						Vector3 a3 = Vector3.Cross(Vector3.up, vector3);
						a = a3 * (num3 - num2);
					}
					bool flag2 = this.lastCorner && this.buffer.Count - num == 1;
					if (flag2)
					{
						if (this.slowdownTime < 0.001f)
						{
							this.slowdownTime = 0.001f;
						}
						Vector3 a4 = vector2 - vector;
						a4.y = 0f;
						if (this.preciseSlowdown)
						{
							vector3 = (6f * a4 - 4f * this.slowdownTime * this.velocity) / (this.slowdownTime * this.slowdownTime);
						}
						else
						{
							vector3 = 2f * (a4 - this.slowdownTime * this.velocity) / (this.slowdownTime * this.slowdownTime);
						}
						vector3 = Vector3.ClampMagnitude(vector3, this.acceleration);
						a *= Math.Min(magnitude / 0.5f, 1f);
						if (magnitude < this.endReachedDistance)
						{
							this.NextPart();
						}
					}
					else
					{
						vector3 *= this.acceleration;
					}
					this.velocity += (vector3 + a * this.wallForce) * RichAI.deltaTime;
					if (this.slowWhenNotFacingTarget)
					{
						float a5 = (Vector3.Dot(lhs, this.tr.forward) + 0.5f) * 0.6666667f;
						float a6 = Mathf.Sqrt(this.velocity.x * this.velocity.x + this.velocity.z * this.velocity.z);
						float y = this.velocity.y;
						this.velocity.y = 0f;
						float d = Mathf.Min(a6, this.maxSpeed * Mathf.Max(a5, 0.2f));
						this.velocity = Vector3.Lerp(this.tr.forward * d, this.velocity.normalized * d, Mathf.Clamp((!flag2) ? 0f : (magnitude * 2f), 0.5f, 1f));
						this.velocity.y = y;
					}
					else
					{
						float num5 = Mathf.Sqrt(this.velocity.x * this.velocity.x + this.velocity.z * this.velocity.z);
						num5 = this.maxSpeed / num5;
						if (num5 < 1f)
						{
							this.velocity.x = this.velocity.x * num5;
							this.velocity.z = this.velocity.z * num5;
						}
					}
					if (flag2)
					{
						Vector3 trotdir = Vector3.Lerp(this.velocity, this.currentTargetDirection, Math.Max(1f - magnitude * 2f, 0f));
						this.RotateTowards(trotdir);
					}
					else
					{
						this.RotateTowards(this.velocity);
					}
					this.velocity += RichAI.deltaTime * this.gravity;
					if (this.rvoController != null && this.rvoController.enabled)
					{
						this.tr.position = vector;
						this.rvoController.Move(this.velocity);
					}
					else if (this.controller != null && this.controller.enabled)
					{
						this.tr.position = vector;
						this.controller.Move(this.velocity * RichAI.deltaTime);
					}
					else
					{
						float y2 = vector.y;
						vector += this.velocity * RichAI.deltaTime;
						vector = this.RaycastPosition(vector, y2);
						this.tr.position = vector;
					}
				}
				else if (this.rvoController != null && this.rvoController.enabled)
				{
					this.rvoController.Move(Vector3.zero);
				}
				if (currentPart is RichSpecial)
				{
					RichSpecial rs = currentPart as RichSpecial;
					if (!this.traversingSpecialPath)
					{
						base.StartCoroutine(this.TraverseSpecial(rs));
					}
				}
			}
			else if (this.rvoController != null && this.rvoController.enabled)
			{
				this.rvoController.Move(Vector3.zero);
			}
			else if (!(this.controller != null) || !this.controller.enabled)
			{
				this.tr.position = this.RaycastPosition(this.tr.position, this.tr.position.y);
			}
		}

		private Vector3 RaycastPosition(Vector3 position, float lasty)
		{
			if (this.raycastingForGroundPlacement)
			{
				float num = Mathf.Max(this.centerOffset, lasty - position.y + this.centerOffset);
				RaycastHit raycastHit;
				if (Physics.Raycast(position + Vector3.up * num, Vector3.down, out raycastHit, num, this.groundMask))
				{
					if (raycastHit.distance < num)
					{
						position = raycastHit.point;
						this.velocity.y = 0f;
					}
				}
			}
			return position;
		}

		private bool RotateTowards(Vector3 trotdir)
		{
			Quaternion rotation = this.tr.rotation;
			trotdir.y = 0f;
			if (trotdir != Vector3.zero)
			{
				Vector3 eulerAngles = Quaternion.LookRotation(trotdir).eulerAngles;
				Vector3 eulerAngles2 = rotation.eulerAngles;
				eulerAngles2.y = Mathf.MoveTowardsAngle(eulerAngles2.y, eulerAngles.y, this.rotationSpeed * RichAI.deltaTime);
				this.tr.rotation = Quaternion.Euler(eulerAngles2);
				return Mathf.Abs(eulerAngles2.y - eulerAngles.y) < 5f;
			}
			return false;
		}

		public void OnDrawGizmos()
		{
			if (this.drawGizmos)
			{
				if (this.raycastingForGroundPlacement)
				{
					Gizmos.color = RichAI.GizmoColorRaycast;
					Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * this.centerOffset);
					Gizmos.DrawLine(base.transform.position + Vector3.left * 0.1f, base.transform.position + Vector3.right * 0.1f);
					Gizmos.DrawLine(base.transform.position + Vector3.back * 0.1f, base.transform.position + Vector3.forward * 0.1f);
				}
				if (this.tr != null && this.buffer != null)
				{
					Gizmos.color = RichAI.GizmoColorPath;
					Vector3 from = this.tr.position;
					for (int i = 0; i < this.buffer.Count; i++)
					{
						Gizmos.DrawLine(from, this.buffer[i]);
						from = this.buffer[i];
					}
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator TraverseSpecial(RichSpecial rs)
		{
			RichAI.<TraverseSpecial>c__Iterator2 <TraverseSpecial>c__Iterator = new RichAI.<TraverseSpecial>c__Iterator2();
			<TraverseSpecial>c__Iterator.rs = rs;
			<TraverseSpecial>c__Iterator.<$>rs = rs;
			<TraverseSpecial>c__Iterator.<>f__this = this;
			return <TraverseSpecial>c__Iterator;
		}
	}
}
