using System;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
	public Transform targetTransform;

	public float smoothTime = 0.15f;

	private Vector3 velocity = Vector3.zero;

	private void Update()
	{
		Vector3 position = this.targetTransform.position;
		base.transform.position = Vector3.SmoothDamp(base.transform.position, position, ref this.velocity, this.smoothTime);
	}
}
