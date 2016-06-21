using System;
using UnityEngine;

public class trailEffectTestObject : MonoBehaviour
{
	public float speed = 0.5f;

	public float width = 5f;

	public float height = 1f;

	public float wiggleFreq = 4f;

	public Vector3 centerPosition;

	public float localTime;

	private void Start()
	{
		this.centerPosition = base.transform.localPosition;
	}

	private void Update()
	{
		this.localTime += Time.deltaTime * this.speed;
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Sin(this.localTime) * this.width;
		zero.z = Mathf.Cos(this.localTime) * this.width;
		zero.y = Mathf.Sin(this.localTime * this.wiggleFreq) * this.height;
		base.transform.localPosition = zero + this.centerPosition;
	}
}
