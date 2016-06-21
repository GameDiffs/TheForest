using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AfterburnerPhysicsForce : MonoBehaviour
{
	public float effectAngle = 15f;

	public float effectWidth = 1f;

	public float effectDistance = 10f;

	public float force = 10f;

	private Collider[] cols;

	private float r;

	private SphereCollider sphere;

	private void Start()
	{
		this.sphere = (base.GetComponent<Collider>() as SphereCollider);
	}

	private void FixedUpdate()
	{
		this.cols = Physics.OverlapSphere(base.transform.position + this.sphere.center, this.sphere.radius);
		for (int i = 0; i < this.cols.Length; i++)
		{
			if (this.cols[i].attachedRigidbody != null)
			{
				Vector3 current = base.transform.InverseTransformPoint(this.cols[i].transform.position);
				current = Vector3.MoveTowards(current, new Vector3(0f, 0f, current.z), this.effectWidth * 0.5f);
				float value = Mathf.Abs(Mathf.Atan2(current.x, current.z) * 57.29578f);
				float num = Mathf.InverseLerp(this.effectDistance, 0f, current.magnitude);
				num *= Mathf.InverseLerp(this.effectAngle, 0f, value);
				Vector3 vector = this.cols[i].transform.position - base.transform.position;
				this.cols[i].attachedRigidbody.AddForceAtPosition(vector.normalized * this.force * num, Vector3.Lerp(this.cols[i].transform.position, base.transform.TransformPoint(0f, 0f, current.z), 0.1f));
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		(base.GetComponent<Collider>() as SphereCollider).radius = this.effectDistance * 0.5f;
		(base.GetComponent<Collider>() as SphereCollider).center = new Vector3(0f, 0f, this.effectDistance * 0.5f);
		Vector3[] array = new Vector3[]
		{
			Vector3.up,
			-Vector3.up,
			Vector3.right,
			-Vector3.right
		};
		Vector3[] array2 = new Vector3[]
		{
			-Vector3.right,
			Vector3.right,
			Vector3.up,
			-Vector3.up
		};
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		for (int i = 0; i < 4; i++)
		{
			Vector3 vector = base.transform.position + base.transform.rotation * array[i] * this.effectWidth * 0.5f;
			Vector3 a = base.transform.TransformDirection(Quaternion.AngleAxis(this.effectAngle, array2[i]) * Vector3.forward);
			Gizmos.DrawLine(vector, vector + a * (base.GetComponent<Collider>() as SphereCollider).radius * 2f);
		}
	}
}
