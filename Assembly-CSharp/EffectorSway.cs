using System;
using UnityEngine;

public class EffectorSway : MonoBehaviour
{
	private float time;

	private Vector3 initialPosition;

	private float direction = 1f;

	public float travel;

	public float period = 5f;

	private void Start()
	{
		this.initialPosition = base.transform.position;
	}

	private void Update()
	{
		if (this.time >= this.period / 2f)
		{
			this.time = 0f;
			this.direction *= -1f;
		}
		float x = Mathf.Lerp(this.initialPosition.x - this.travel * this.direction, this.initialPosition.x + this.travel * this.direction, this.time / (this.period / 2f));
		base.transform.position = new Vector3(x, base.transform.position.y, base.transform.position.z);
		this.time += Time.deltaTime;
	}
}
