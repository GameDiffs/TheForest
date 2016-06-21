using System;
using UnityEngine;

public class simpleFPSCamera : MonoBehaviour
{
	public Vector2 sensitivity = new Vector2(1f, 1f);

	public Vector2 speed = new Vector2(1f, 1f);

	public float minimumY = 60f;

	public float maximumY = 60f;

	private float rotationY;

	private float rotationX;

	private void Update()
	{
		this.rotationX = base.transform.parent.localEulerAngles.y + Input.GetAxis("Mouse X") * this.sensitivity.x;
		this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivity.y;
		this.rotationY = Mathf.Clamp(this.rotationY, -this.minimumY, this.maximumY);
		base.transform.localEulerAngles = new Vector3(-this.rotationY, 0f, 0f);
		base.transform.parent.localEulerAngles = new Vector3(0f, this.rotationX, 0f);
		base.transform.parent.position += base.transform.parent.forward * Input.GetAxis("Vertical") * this.speed.y * 0.1f + base.transform.parent.right * Input.GetAxis("Horizontal") * this.speed.x * 0.1f;
	}
}
