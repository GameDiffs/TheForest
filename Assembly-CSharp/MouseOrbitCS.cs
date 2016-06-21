using System;
using UnityEngine;

public class MouseOrbitCS : MonoBehaviour
{
	public Transform target;

	public float distance = 10f;

	public float xSpeed = 250f;

	public float ySpeed = 120f;

	public float yMinLimit = -20f;

	public float yMaxLimit = 80f;

	private float x;

	private float y;

	private float normal_angle;

	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.x = eulerAngles.y;
		this.y = eulerAngles.x;
	}

	private void LateUpdate()
	{
		if (this.target && !Input.GetKey(KeyCode.F))
		{
			if (!Input.GetMouseButton(0))
			{
				this.x += Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
				this.y -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
			}
			this.y = MouseOrbitCS.ClampAngle(this.y, this.yMinLimit + this.normal_angle, this.yMaxLimit + this.normal_angle);
			Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
			Vector3 position = rotation * new Vector3(0f, 0f, -this.distance) + this.target.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
		}
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public void set_normal_angle(float a)
	{
		this.normal_angle = a;
	}
}
