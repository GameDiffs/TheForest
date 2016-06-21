using System;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY,
		MouseX,
		MouseY
	}

	public CameraLook.RotationAxes axes;

	public float mousesensitivityX = 15f;

	public float mousesensitivityY = 15f;

	public float joysensitivityX = 3f;

	public float joysensitivityY = 3f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	private float rotationY;

	private void Update()
	{
		float num = Mathf.Abs(Input.GetAxis("Joy X"));
		float num2 = Mathf.Abs(Input.GetAxis("Joy Y"));
		if (this.axes == CameraLook.RotationAxes.MouseXAndY)
		{
			float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.mousesensitivityX;
			this.rotationY += Input.GetAxis("Mouse Y") * this.mousesensitivityY;
			this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
			base.transform.localEulerAngles = new Vector3(-this.rotationY, y, 0f);
		}
		else if (this.axes == CameraLook.RotationAxes.MouseX)
		{
			if ((double)num > 0.05)
			{
				base.transform.Rotate(0f, Input.GetAxis("Joy X") * this.joysensitivityX, 0f);
			}
			base.transform.Rotate(0f, Input.GetAxis("Mouse X") * this.mousesensitivityX, 0f);
		}
		else
		{
			if ((double)num2 > 0.05)
			{
				this.rotationY += Input.GetAxis("Joy Y") * this.joysensitivityY;
			}
			this.rotationY += Input.GetAxis("Mouse Y") * this.mousesensitivityY;
			this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
			base.transform.localEulerAngles = new Vector3(-this.rotationY, base.transform.localEulerAngles.y, 0f);
		}
	}
}
