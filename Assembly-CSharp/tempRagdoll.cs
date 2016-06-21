using System;
using UnityEngine;

public class tempRagdoll : MonoBehaviour
{
	private Rigidbody[] rb;

	private Animator animator;

	public void blockRagdoll()
	{
		this.rb = base.transform.GetComponentsInChildren<Rigidbody>();
		this.animator = base.GetComponent<Animator>();
		Rigidbody[] array = this.rb;
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody rigidbody = array[i];
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			rigidbody.Sleep();
		}
		base.Invoke("startRagdoll", 0.1f);
	}

	private void startRagdoll()
	{
		this.animator.enabled = false;
		Rigidbody[] array = this.rb;
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody rigidbody = array[i];
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
			rigidbody.WakeUp();
		}
	}
}
