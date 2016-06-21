using System;
using System.Collections;
using UnityEngine;

public class ProtectCameraFromWallClip : MonoBehaviour
{
	public class RayHitComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
		}
	}

	public float clipMoveTime = 0.05f;

	public float returnTime = 0.4f;

	public float sphereCastRadius = 0.1f;

	public bool visualiseInEditor;

	public float closestDistance = 0.5f;

	public string dontClipTag = "Player";

	private Transform cam;

	private Transform pivot;

	private float originalDist;

	private float moveVelocity;

	private float currentDist;

	private Ray ray;

	private RaycastHit[] hits;

	private ProtectCameraFromWallClip.RayHitComparer rayHitComparer;

	public bool protecting
	{
		get;
		private set;
	}

	private void Start()
	{
		this.cam = base.GetComponentInChildren<Camera>().transform;
		this.pivot = this.cam.parent;
		this.originalDist = this.cam.localPosition.magnitude;
		this.currentDist = this.originalDist;
		this.rayHitComparer = new ProtectCameraFromWallClip.RayHitComparer();
	}

	private void LateUpdate()
	{
		float num = this.originalDist;
		this.ray.origin = this.pivot.position + this.pivot.forward * this.sphereCastRadius;
		this.ray.direction = -this.pivot.forward;
		Collider[] array = Physics.OverlapSphere(this.ray.origin, this.sphereCastRadius);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].isTrigger && (!(array[i].attachedRigidbody != null) || !array[i].attachedRigidbody.CompareTag(this.dontClipTag)))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.ray.origin = this.ray.origin + this.pivot.forward * this.sphereCastRadius;
			this.hits = Physics.RaycastAll(this.ray, this.originalDist - this.sphereCastRadius);
		}
		else
		{
			this.hits = Physics.SphereCastAll(this.ray, this.sphereCastRadius, this.originalDist + this.sphereCastRadius);
		}
		Array.Sort(this.hits, this.rayHitComparer);
		float num2 = float.PositiveInfinity;
		for (int j = 0; j < this.hits.Length; j++)
		{
			if (this.hits[j].distance < num2 && !this.hits[j].collider.isTrigger && (!(this.hits[j].collider.attachedRigidbody != null) || !this.hits[j].collider.attachedRigidbody.CompareTag(this.dontClipTag)))
			{
				num2 = this.hits[j].distance;
				num = -this.pivot.InverseTransformPoint(this.hits[j].point).z;
				flag2 = true;
			}
		}
		Debug.DrawRay(this.ray.origin, -this.pivot.forward * (num + this.sphereCastRadius), (!flag2) ? Color.green : Color.red);
		this.protecting = flag2;
		this.currentDist = Mathf.SmoothDamp(this.currentDist, num, ref this.moveVelocity, (this.currentDist <= num) ? this.returnTime : this.clipMoveTime);
		this.currentDist = Mathf.Clamp(this.currentDist, this.closestDistance, this.originalDist);
		this.cam.localPosition = -Vector3.forward * this.currentDist;
	}
}
