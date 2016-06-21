using Pathfinding.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public class GraphCollision
	{
		public const float RaycastErrorMargin = 0.005f;

		public ColliderType type = ColliderType.Capsule;

		public float diameter = 1f;

		public float height = 2f;

		public float collisionOffset;

		public RayDirection rayDirection = RayDirection.Both;

		public LayerMask mask;

		public LayerMask heightMask = -1;

		public float fromHeight = 100f;

		public bool thickRaycast;

		public float thickRaycastDiameter = 1f;

		public bool unwalkableWhenNoGround = true;

		public bool use2D;

		public bool collisionCheck = true;

		public bool heightCheck = true;

		public Vector3 up;

		private Vector3 upheight;

		private float finalRadius;

		private float finalRaycastRadius;

		public void Initialize(Matrix4x4 matrix, float scale)
		{
			this.up = matrix.MultiplyVector(Vector3.up);
			this.upheight = this.up * this.height;
			this.finalRadius = this.diameter * scale * 0.5f;
			this.finalRaycastRadius = this.thickRaycastDiameter * scale * 0.5f;
		}

		public bool Check(Vector3 position)
		{
			if (!this.collisionCheck)
			{
				return true;
			}
			if (this.use2D)
			{
				ColliderType colliderType = this.type;
				if (colliderType == ColliderType.Sphere)
				{
					return Physics2D.OverlapCircle(position, this.finalRadius, this.mask) == null;
				}
				if (colliderType != ColliderType.Capsule)
				{
					return Physics2D.OverlapPoint(position, this.mask) == null;
				}
				throw new Exception("Capsule mode cannot be used with 2D since capsules don't exist in 2D. Please change the Physics Testing -> Collider Type setting.");
			}
			else
			{
				position += this.up * this.collisionOffset;
				ColliderType colliderType = this.type;
				if (colliderType == ColliderType.Sphere)
				{
					return !Physics.CheckSphere(position, this.finalRadius, this.mask);
				}
				if (colliderType != ColliderType.Capsule)
				{
					switch (this.rayDirection)
					{
					case RayDirection.Up:
						return !Physics.Raycast(position, this.up, this.height, this.mask);
					case RayDirection.Both:
						return !Physics.Raycast(position, this.up, this.height, this.mask) && !Physics.Raycast(position + this.upheight, -this.up, this.height, this.mask);
					}
					return !Physics.Raycast(position + this.upheight, -this.up, this.height, this.mask);
				}
				return !Physics.CheckCapsule(position, position + this.upheight, this.finalRadius, this.mask);
			}
		}

		public Vector3 CheckHeight(Vector3 position)
		{
			RaycastHit raycastHit;
			bool flag;
			return this.CheckHeight(position, out raycastHit, out flag);
		}

		public Vector3 CheckHeight(Vector3 position, out RaycastHit hit, out bool walkable)
		{
			walkable = true;
			if (!this.heightCheck || this.use2D)
			{
				hit = default(RaycastHit);
				return position;
			}
			if (this.thickRaycast)
			{
				Ray ray = new Ray(position + this.up * this.fromHeight, -this.up);
				if (Physics.SphereCast(ray, this.finalRaycastRadius, out hit, this.fromHeight + 0.005f, this.heightMask))
				{
					return AstarMath.NearestPoint(ray.origin, ray.origin + ray.direction, hit.point);
				}
				walkable &= !this.unwalkableWhenNoGround;
			}
			else
			{
				if (Physics.Raycast(position + this.up * this.fromHeight, -this.up, out hit, this.fromHeight + 0.005f, this.heightMask))
				{
					return hit.point;
				}
				walkable &= !this.unwalkableWhenNoGround;
			}
			return position;
		}

		public Vector3 Raycast(Vector3 origin, out RaycastHit hit, out bool walkable)
		{
			walkable = true;
			if (!this.heightCheck || this.use2D)
			{
				hit = default(RaycastHit);
				return origin - this.up * this.fromHeight;
			}
			if (this.thickRaycast)
			{
				Ray ray = new Ray(origin, -this.up);
				if (Physics.SphereCast(ray, this.finalRaycastRadius, out hit, this.fromHeight + 0.005f, this.heightMask))
				{
					return AstarMath.NearestPoint(ray.origin, ray.origin + ray.direction, hit.point);
				}
				walkable &= !this.unwalkableWhenNoGround;
			}
			else
			{
				if (Physics.Raycast(origin, -this.up, out hit, this.fromHeight + 0.005f, this.heightMask))
				{
					return hit.point;
				}
				walkable &= !this.unwalkableWhenNoGround;
			}
			return origin - this.up * this.fromHeight;
		}

		public RaycastHit[] CheckHeightAll(Vector3 position)
		{
			if (!this.heightCheck || this.use2D)
			{
				return new RaycastHit[]
				{
					new RaycastHit
					{
						point = position,
						distance = 0f
					}
				};
			}
			if (this.thickRaycast)
			{
				return new RaycastHit[0];
			}
			List<RaycastHit> list = new List<RaycastHit>();
			Vector3 vector = position + this.up * this.fromHeight;
			Vector3 vector2 = Vector3.zero;
			int num = 0;
			while (true)
			{
				RaycastHit item;
				bool flag;
				this.Raycast(vector, out item, out flag);
				if (item.transform == null)
				{
					break;
				}
				if (item.point != vector2 || list.Count == 0)
				{
					vector = item.point - this.up * 0.005f;
					vector2 = item.point;
					num = 0;
					list.Add(item);
				}
				else
				{
					vector -= this.up * 0.001f;
					num++;
					if (num > 10)
					{
						goto Block_5;
					}
				}
			}
			goto IL_15A;
			Block_5:
			Debug.LogError(string.Concat(new object[]
			{
				"Infinite Loop when raycasting. Please report this error (arongranberg.com)\n",
				vector,
				" : ",
				vector2
			}));
			IL_15A:
			return list.ToArray();
		}

		public void SerializeSettings(GraphSerializationContext ctx)
		{
			ctx.writer.Write((int)this.type);
			ctx.writer.Write(this.diameter);
			ctx.writer.Write(this.height);
			ctx.writer.Write(this.collisionOffset);
			ctx.writer.Write((int)this.rayDirection);
			ctx.writer.Write(this.mask);
			ctx.writer.Write(this.heightMask);
			ctx.writer.Write(this.fromHeight);
			ctx.writer.Write(this.thickRaycast);
			ctx.writer.Write(this.thickRaycastDiameter);
			ctx.writer.Write(this.unwalkableWhenNoGround);
			ctx.writer.Write(this.use2D);
			ctx.writer.Write(this.collisionCheck);
			ctx.writer.Write(this.heightCheck);
		}

		public void DeserializeSettings(GraphSerializationContext ctx)
		{
			this.type = (ColliderType)ctx.reader.ReadInt32();
			this.diameter = ctx.reader.ReadSingle();
			this.height = ctx.reader.ReadSingle();
			this.collisionOffset = ctx.reader.ReadSingle();
			this.rayDirection = (RayDirection)ctx.reader.ReadInt32();
			this.mask = ctx.reader.ReadInt32();
			this.heightMask = ctx.reader.ReadInt32();
			this.fromHeight = ctx.reader.ReadSingle();
			this.thickRaycast = ctx.reader.ReadBoolean();
			this.thickRaycastDiameter = ctx.reader.ReadSingle();
			this.unwalkableWhenNoGround = ctx.reader.ReadBoolean();
			this.use2D = ctx.reader.ReadBoolean();
			this.collisionCheck = ctx.reader.ReadBoolean();
			this.heightCheck = ctx.reader.ReadBoolean();
		}
	}
}
