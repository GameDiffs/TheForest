using Bolt;
using System;
using UnityEngine;

public class CoopSharedRigidbody : EntityBehaviour<IRigidbodyState>
{
	public float ClientMass = 15f;

	public float ClientDrag = 0.5f;

	public Rigidbody Body;

	[HideInInspector]
	public Transform ClientTransform;

	private void Awake()
	{
		base.enabled = false;
	}

	public override void Attached()
	{
		if (BoltNetwork.isClient)
		{
			this.ClientTransform = new GameObject(base.gameObject.name + "_CLIENTTRANSFORM").GetComponent<Transform>();
			base.state.Transform.SetTransforms(this.ClientTransform);
			if (this.Body)
			{
				this.Body.mass = this.ClientMass;
				this.Body.drag = this.ClientDrag;
			}
			base.enabled = true;
		}
		else
		{
			base.enabled = false;
			base.state.Transform.SetTransforms(base.transform);
		}
	}

	public override void Detached()
	{
		if (this.ClientTransform)
		{
			UnityEngine.Object.Destroy(this.ClientTransform.gameObject);
		}
	}

	private void Update()
	{
		if (BoltNetwork.isClient)
		{
			base.transform.position = Vector3.Slerp(base.transform.position, this.ClientTransform.position, Time.deltaTime);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.ClientTransform.rotation, Time.deltaTime);
		}
	}
}
