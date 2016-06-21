using System;
using UnityEngine;

public class swingingRockSetup : MonoBehaviour
{
	public GameObject looseRope;

	public GameObject straightRope;

	private void Start()
	{
	}

	private void enableSwingingRock()
	{
		Rigidbody component = base.transform.GetComponent<Rigidbody>();
		component.useGravity = true;
		component.isKinematic = false;
		Rigidbody[] componentsInChildren = base.transform.GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Rigidbody rigidbody = componentsInChildren[i];
			rigidbody.useGravity = true;
			rigidbody.isKinematic = false;
			rigidbody.AddForce(Vector3.down * 20f, ForceMode.VelocityChange);
		}
		this.looseRope.SetActive(false);
		this.straightRope.SetActive(true);
	}
}
