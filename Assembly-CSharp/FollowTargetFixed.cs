using System;
using UnityEngine;

public class FollowTargetFixed : MonoBehaviour
{
	public Transform target;

	public Vector3 offset = new Vector3(0f, 7.5f, 0f);

	public bool keepLocalRotation;

	private Quaternion initialLocalRotation;

	private void Awake()
	{
		this.initialLocalRotation = base.transform.localRotation;
	}

	private void Update()
	{
		base.transform.position = this.target.position + this.offset;
		if (this.keepLocalRotation)
		{
			base.transform.localRotation = this.initialLocalRotation;
		}
	}

	private void FixedUpdate()
	{
		base.transform.position = this.target.position + this.offset;
		if (this.keepLocalRotation)
		{
			base.transform.localRotation = this.initialLocalRotation;
		}
	}
}
