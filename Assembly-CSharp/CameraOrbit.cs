using System;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
	public Transform target;

	public float targetHeight = 1.7f;

	public float distance = 10f;

	public float maxDistance = 20f;

	public float minDistance = 0.6f;

	public float xSpeed = 200f;

	public float ySpeed = 200f;

	public int yMinLimit = -80;

	public int yMaxLimit = 80;

	public int zoomRate = 40;

	public float rotationDampening = 3f;

	public float zoomDampening = 5f;

	public float LeftBorder;

	public float TopBorder;

	private float xDeg;

	private float yDeg;

	private float currentDistance;

	private float desiredDistance;

	private float correctedDistance;

	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.xDeg = eulerAngles.y;
		this.yDeg = eulerAngles.x;
		this.currentDistance = this.distance;
		this.desiredDistance = this.distance;
		this.correctedDistance = this.distance;
	}

	private void LateUpdate()
	{
		if (!this.target)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			if (!Input.GetKey("mouse 0") || (Input.mousePosition.x < this.LeftBorder && Input.mousePosition.y > (float)Screen.height - this.TopBorder))
			{
				return;
			}
			this.xDeg += Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
			this.yDeg -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
		}
		else if (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f)
		{
			float y = this.target.eulerAngles.y;
			float y2 = base.transform.eulerAngles.y;
			this.xDeg = Mathf.LerpAngle(y2, y, this.rotationDampening * Time.deltaTime);
		}
		this.yDeg = CameraOrbit.ClampAngle(this.yDeg, (float)this.yMinLimit, (float)this.yMaxLimit);
		Quaternion rotation = Quaternion.Euler(this.yDeg, this.xDeg, 0f);
		this.desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * (float)this.zoomRate * Mathf.Abs(this.desiredDistance);
		this.desiredDistance = Mathf.Clamp(this.desiredDistance, this.minDistance, this.maxDistance);
		this.correctedDistance = this.desiredDistance;
		Vector3 b = new Vector3(0f, -this.targetHeight, 0f);
		Vector3 position = this.target.position - (rotation * Vector3.forward * this.desiredDistance + b);
		bool flag = false;
		this.currentDistance = ((flag && this.correctedDistance <= this.currentDistance) ? this.correctedDistance : Mathf.Lerp(this.currentDistance, this.correctedDistance, Time.deltaTime * this.zoomDampening));
		this.currentDistance = Mathf.Clamp(this.currentDistance, this.minDistance, this.maxDistance);
		position = this.target.position - (rotation * Vector3.forward * this.currentDistance + b);
		base.transform.rotation = rotation;
		base.transform.position = position;
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
}
