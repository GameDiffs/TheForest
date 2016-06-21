using System;
using UnityEngine;

public class simpleFollow : MonoBehaviour
{
	public Transform targetTr;

	private Transform tr;

	public GameObject velocityGo;

	private Rigidbody rb;

	public float mag;

	public float minMag;

	public float smoothTime;

	public float magLerpSpeed;

	private void Start()
	{
		this.rb = this.velocityGo.GetComponent<Rigidbody>();
		this.tr = base.transform;
	}

	private void FixedUpdate()
	{
		if (this.rb)
		{
			float num = this.rb.angularVelocity.magnitude;
			num = Mathf.Clamp(num, this.minMag, 1f);
			this.mag = Mathf.Lerp(this.mag, num, Time.deltaTime * this.magLerpSpeed);
			Vector3 zero = Vector3.zero;
			this.tr.position = Vector3.SmoothDamp(this.tr.position, this.targetTr.position, ref zero, this.smoothTime * this.mag);
		}
	}
}
