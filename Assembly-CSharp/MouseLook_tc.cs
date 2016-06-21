using System;
using UnityEngine;

public class MouseLook_tc : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY,
		MouseX,
		MouseY
	}

	public MouseLook_tc.RotationAxes axes;

	public float sensitivityX = 15f;

	public float sensitivityY = 15f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	private bool mouse = true;

	private float rotationY;

	private void Update()
	{
		if (Input.GetKey(KeyCode.F))
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			this.mouse = !this.mouse;
		}
		if (this.mouse)
		{
			if (this.axes == MouseLook_tc.RotationAxes.MouseXAndY)
			{
				float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.sensitivityX;
				this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
				this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
				base.transform.localEulerAngles = new Vector3(-this.rotationY, y, 0f);
			}
			else if (this.axes == MouseLook_tc.RotationAxes.MouseX)
			{
				base.transform.Rotate(0f, Input.GetAxis("Mouse X") * this.sensitivityX, 0f);
			}
			else
			{
				this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
				this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
				base.transform.localEulerAngles = new Vector3(-this.rotationY, base.transform.localEulerAngles.y, 0f);
			}
		}
	}

	private void Start()
	{
		if (base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}
}
