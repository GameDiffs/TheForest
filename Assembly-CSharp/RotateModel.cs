using System;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
	public float rotationY;

	private float rotationX;

	private float time;

	public float Speed = 20f;

	public bool rotate;

	private void Start()
	{
		this.rotationX = base.transform.localEulerAngles.x;
		this.rotationY = base.transform.localEulerAngles.y;
	}

	private void Update()
	{
		if (this.rotate)
		{
			this.time += Time.deltaTime;
			base.transform.localEulerAngles = new Vector3(this.rotationX, this.rotationY + this.time * this.Speed, 0f);
		}
	}
}
