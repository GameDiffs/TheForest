using System;
using UnityEngine;

public class animalAvoidance : MonoBehaviour
{
	public GameObject ControllerGo;

	private Rigidbody rb;

	private Collider currCollider;

	public Vector3 currNormal;

	public Vector3 currPoint;

	private bool locked;

	private Vector3 localPos;

	private Vector3 localRot;

	public int axisDir;

	private void Start()
	{
		this.localPos = base.transform.localPosition;
		this.localRot = base.transform.localEulerAngles;
		this.rb = base.GetComponent<Rigidbody>();
	}

	private void OnEnable()
	{
		this.ControllerGo.SendMessage("disableBlocked", SendMessageOptions.DontRequireReceiver);
		this.locked = false;
	}

	private void OnCollisionEnter(Collision other)
	{
		this.doCollisionCheck(other);
	}

	private void OnCollisionStay(Collision other)
	{
		this.doCollisionCheck(other);
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.layer == 11 || other.gameObject.layer == 13 || other.gameObject.layer == 17 || other.gameObject.layer == 20 || other.gameObject.layer == 21 || other.gameObject.layer == 25)
		{
			this.ControllerGo.SendMessage("disableBlocked", SendMessageOptions.DontRequireReceiver);
			this.locked = false;
		}
	}

	private void doCollisionCheck(Collision other)
	{
		if (other.gameObject.layer == 11 || other.gameObject.layer == 13 || other.gameObject.layer == 17 || other.gameObject.layer == 20 || other.gameObject.layer == 21 || other.gameObject.layer == 25)
		{
			if (other.collider == null)
			{
				return;
			}
			ContactPoint contactPoint = other.contacts[0];
			Vector3 vector = base.transform.InverseTransformPoint(contactPoint.point);
			if (this.axisDir == 1)
			{
				if (vector.z < 0f)
				{
					this.locked = false;
					this.ControllerGo.SendMessage("disableBlocked", SendMessageOptions.DontRequireReceiver);
					return;
				}
			}
			else if (this.axisDir == 0)
			{
				if (vector.x > 0f)
				{
					this.locked = false;
					this.ControllerGo.SendMessage("disableBlocked", SendMessageOptions.DontRequireReceiver);
					return;
				}
			}
			else if (this.axisDir == 2 && vector.y < 0f)
			{
				this.locked = false;
				this.ControllerGo.SendMessage("disableBlocked", SendMessageOptions.DontRequireReceiver);
				return;
			}
			this.currCollider = other.collider;
			this.currNormal = contactPoint.normal;
			this.currPoint = contactPoint.point;
			this.ControllerGo.SendMessage("enableBlocked", SendMessageOptions.DontRequireReceiver);
			this.locked = true;
		}
	}

	private void Update()
	{
		base.transform.localPosition = this.localPos;
		base.transform.localEulerAngles = this.localRot;
		if ((this.currCollider == null || !this.currCollider.enabled) && this.locked)
		{
			this.locked = false;
			this.ControllerGo.SendMessage("disableBlocked", SendMessageOptions.DontRequireReceiver);
		}
	}
}
