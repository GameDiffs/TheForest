using System;
using UnityEngine;

[AddComponentMenu(""), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(SphereCollider))]
public class AmplifyColorTriggerProxy : MonoBehaviour
{
	public Transform Reference;

	public AmplifyColorBase OwnerEffect;

	private SphereCollider sphereCollider;

	private Rigidbody rigidBody;

	private void Start()
	{
		this.sphereCollider = base.GetComponent<SphereCollider>();
		this.sphereCollider.radius = 0.01f;
		this.sphereCollider.isTrigger = true;
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.rigidBody.useGravity = false;
		this.rigidBody.isKinematic = true;
	}

	private void LateUpdate()
	{
		base.transform.position = this.Reference.position;
		base.transform.rotation = this.Reference.rotation;
	}
}
