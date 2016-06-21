using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look2")]
public class MouseLook2 : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY,
		MouseX,
		MouseY
	}

	public MouseLook2.RotationAxes axes;

	public float sensitivityX = 5f;

	public float sensitivityY = 5f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	private float rotationY;

	public float LeftBorder;

	public float TopBorder;

	private void Update()
	{
		if (!Input.GetKey("mouse 0") || (Input.mousePosition.x < this.LeftBorder && Input.mousePosition.y > (float)Screen.height - this.TopBorder))
		{
			return;
		}
		if (this.axes == MouseLook2.RotationAxes.MouseXAndY)
		{
			float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.sensitivityX;
			this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
			this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
			base.transform.localEulerAngles = new Vector3(-this.rotationY, y, 0f);
		}
		else if (this.axes == MouseLook2.RotationAxes.MouseX)
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

	private void Start()
	{
		if (base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}
}
