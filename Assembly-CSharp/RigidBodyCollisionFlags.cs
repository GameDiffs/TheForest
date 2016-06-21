using System;
using UnityEngine;

[AddComponentMenu("Physics/Rigidbody Collision Flags"), RequireComponent(typeof(Collider))]
public class RigidBodyCollisionFlags : MonoBehaviour
{
	private Transform _trans;

	public int collisionCount;

	public float groundAngleVal = 45f;

	public float clampVal;

	public CapsuleCollider cColl;

	private SphereCollider sColl;

	private BoxCollider bColl;

	public Collider coll;

	private CollisionFlags collFlags;

	public int collType = 1;

	public CollisionFlags collisionFlags
	{
		get
		{
			return this.collFlags;
		}
	}

	private void Awake()
	{
		this._trans = base.transform;
		this.cColl = base.GetComponent<CapsuleCollider>();
		this.collisionCount = 0;
	}

	private void OnCollisionEnter(Collision other)
	{
		this.collisionCount++;
	}

	private void OnCollisionExit(Collision other)
	{
		this.collisionCount--;
	}

	private void Update()
	{
		if (this.collisionCount == 0)
		{
			this.collFlags = CollisionFlags.None;
			this.groundAngleVal = 0f;
		}
	}

	private void OnCollisionStay(Collision collInfo)
	{
		if (!this.coll)
		{
			this.coll = base.GetComponent<Collider>();
			this.GetCollType();
		}
		this.collFlags = CollisionFlags.None;
		Vector3 center = this.coll.bounds.center;
		switch (this.collType)
		{
		case 1:
		{
			Vector3 vector = this._trans.up * ((this.cColl.height - this.cColl.radius * 2f) * 0.5f);
			vector = Vector3.Scale(vector, this._trans.localScale);
			Vector3 vector2 = center - vector;
			Vector3 vector3 = center + vector;
			Debug.DrawLine(this.coll.bounds.center, vector2, Color.green);
			Debug.DrawLine(this.coll.bounds.center, vector3, Color.blue);
			ContactPoint[] contacts = collInfo.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				int num = 0;
				float num2 = UEx.SqrLineDistance(vector2, vector3, contactPoint.point, out num);
				if (contactPoint.otherCollider.GetType() == typeof(MeshCollider))
				{
					if ((double)contactPoint.point.y < (double)vector2.y + 0.25)
					{
						Vector3 vector4 = contactPoint.point;
						Vector3 normal = contactPoint.normal;
						vector4 += normal;
						RaycastHit raycastHit;
						if (contactPoint.otherCollider.Raycast(new Ray(vector4, -normal), out raycastHit, 2f))
						{
							this.groundAngleVal = Mathf.Abs(Vector3.Angle(raycastHit.normal, Vector3.up));
						}
						else
						{
							this.groundAngleVal = 0f;
						}
						this.collFlags |= CollisionFlags.Below;
					}
				}
				else if (num == 1)
				{
					this.collFlags |= CollisionFlags.Below;
					this.groundAngleVal = 0f;
				}
				else if (num == 2)
				{
					this.collFlags |= CollisionFlags.Above;
					this.groundAngleVal = 0f;
				}
				else
				{
					this.collFlags |= CollisionFlags.Sides;
					this.groundAngleVal = 0f;
				}
			}
			break;
		}
		case 2:
		{
			ContactPoint[] contacts2 = collInfo.contacts;
			for (int j = 0; j < contacts2.Length; j++)
			{
				ContactPoint contactPoint2 = contacts2[j];
				Vector3 rhs = contactPoint2.point - center;
				rhs.Normalize();
				float num3 = Vector3.Dot(this._trans.up, rhs);
				if (num3 < 0.333f)
				{
					this.collFlags |= CollisionFlags.Below;
				}
				else if (num3 > 0.333f)
				{
					this.collFlags |= CollisionFlags.Above;
				}
				else
				{
					this.collFlags |= CollisionFlags.Sides;
				}
			}
			break;
		}
		case 3:
		{
			ContactPoint[] contacts3 = collInfo.contacts;
			for (int k = 0; k < contacts3.Length; k++)
			{
				ContactPoint contactPoint3 = contacts3[k];
				Vector3 rhs2 = contactPoint3.point - center;
				rhs2.Normalize();
				float num4 = Vector3.Dot(this._trans.up, rhs2);
				if (num4 < 0.5f)
				{
					this.collFlags |= CollisionFlags.Below;
				}
				else if (num4 > 0.5f)
				{
					this.collFlags |= CollisionFlags.Above;
				}
				else
				{
					this.collFlags |= CollisionFlags.Sides;
				}
			}
			break;
		}
		}
	}

	private void GetCollType()
	{
		this.cColl = null;
		this.sColl = null;
		this.bColl = null;
		Type type = this.coll.GetType();
		if (type == typeof(CapsuleCollider))
		{
			this.collType = 1;
			this.cColl = (CapsuleCollider)this.coll;
		}
		else if (type == typeof(SphereCollider))
		{
			this.sColl = (SphereCollider)this.coll;
			this.collType = 2;
		}
		else if (type == typeof(BoxCollider))
		{
			this.bColl = (BoxCollider)this.coll;
			this.collType = 3;
		}
		else
		{
			this.collType = 0;
		}
	}
}
