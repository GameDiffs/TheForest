using System;
using UnityEngine;

namespace Ceto
{
	public class FreeCam : MonoBehaviour
	{
		public enum RotationAxes
		{
			MouseXAndY,
			MouseX,
			MouseY
		}

		public float m_speed = 50f;

		public FreeCam.RotationAxes axes;

		public float sensitivityX = 15f;

		public float sensitivityY = 15f;

		public float minimumX = -360f;

		public float maximumX = 360f;

		public float minimumY = -60f;

		public float maximumY = 60f;

		public float rotationY;

		private bool m_takeMouseInput = true;

		private void Start()
		{
			base.transform.localEulerAngles = new Vector3(-this.rotationY, base.transform.localEulerAngles.y, 0f);
		}

		private void OnGUI()
		{
			if (Event.current == null)
			{
				return;
			}
			if (Event.current.isMouse)
			{
				this.m_takeMouseInput = true;
			}
			else
			{
				this.m_takeMouseInput = false;
			}
		}

		private void Update()
		{
			float num = this.m_speed;
			if (Input.GetKey(KeyCode.Space))
			{
				num *= 10f;
			}
			Vector3 translation = new Vector3(0f, 0f, 0f);
			if (Input.GetKey(KeyCode.A))
			{
				translation = new Vector3(-1f, 0f, 0f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.D))
			{
				translation = new Vector3(1f, 0f, 0f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.W))
			{
				translation = new Vector3(0f, 0f, 1f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.S))
			{
				translation = new Vector3(0f, 0f, -1f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.E))
			{
				translation = new Vector3(0f, -1f, 0f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.Q))
			{
				translation = new Vector3(0f, 1f, 0f) * Time.deltaTime * num;
			}
			base.transform.Translate(translation);
			if (this.m_takeMouseInput)
			{
				if (this.axes == FreeCam.RotationAxes.MouseXAndY)
				{
					float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.sensitivityX;
					this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
					this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
					base.transform.localEulerAngles = new Vector3(-this.rotationY, y, 0f);
				}
				else if (this.axes == FreeCam.RotationAxes.MouseX)
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
	}
}
