using System;
using UnityEngine;

namespace Ceto
{
	public class ShipCamera : MonoBehaviour
	{
		private struct Position
		{
			public Vector3 forwardAmount;

			public float turnAmount;

			public Vector2 camRotation;

			public float camDistance;
		}

		private const float MAX_ACCELERATION = 1f;

		private const float ACCELERATION_RATE = 1f;

		private const float DECELERATION_RATE = 0.25f;

		public bool disableInterpolation;

		public GameObject m_ship;

		public Vector3 m_forward = new Vector3(0f, 0f, 1f);

		public float m_shipMoveSpeed = 20f;

		[Range(0.01f, 1f)]
		public float shipSmoothness = 0.5f;

		public float m_camRotationSpeed = 10f;

		public float m_camStartRotationX = 180f;

		public float m_camStartRotationY = 60f;

		public float m_camStartDistance = 100f;

		[Range(0.01f, 1f)]
		public float camSmoothness = 0.5f;

		private ShipCamera.Position m_position;

		private ShipCamera.Position m_target;

		private float m_acceleration;

		private Vector3 m_previousPos;

		private Vector3 m_velocity;

		private GameObject m_dummy;

		private GameObject Ship
		{
			get
			{
				if (this.m_ship == null)
				{
					this.m_dummy = new GameObject();
					this.m_ship = this.m_dummy;
				}
				return this.m_ship;
			}
		}

		private void Start()
		{
			this.m_position.camRotation.x = this.m_camStartRotationX;
			this.m_position.camRotation.y = this.m_camStartRotationY;
			this.m_position.camDistance = this.m_camStartDistance;
			this.m_position.forwardAmount = Vector3.zero;
			this.m_target = this.m_position;
			this.m_previousPos = this.Ship.transform.position;
		}

		private void LateUpdate()
		{
			this.ProcessInput();
			this.InterpolateToTarget();
			this.MoveShip();
		}

		private void OnDestroy()
		{
			if (this.m_dummy != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_dummy);
			}
		}

		private void MoveShip()
		{
			this.Ship.transform.position += this.m_position.forwardAmount;
			Vector3 eulerAngles = this.Ship.transform.eulerAngles;
			eulerAngles.y += this.m_position.turnAmount;
			this.Ship.transform.eulerAngles = eulerAngles;
			float y = Mathf.Cos(this.m_position.camRotation.y * 0.0174532924f);
			float num = Mathf.Sin(this.m_position.camRotation.y * 0.0174532924f);
			float num2 = Mathf.Cos(this.m_position.camRotation.x * 0.0174532924f);
			float num3 = Mathf.Sin(this.m_position.camRotation.x * 0.0174532924f);
			Vector3 position = this.Ship.transform.position;
			Vector3 position2 = position + new Vector3(num3 * num, y, num2 * num) * this.m_position.camDistance;
			base.transform.position = position2;
			base.transform.LookAt(position);
			this.m_velocity = this.Ship.transform.position - this.m_previousPos;
			this.m_previousPos = this.Ship.transform.position;
		}

		private void InterpolateToTarget()
		{
			if (this.disableInterpolation || Time.timeScale == 0f)
			{
				this.m_position = this.m_target;
				return;
			}
			float num = 1f / Mathf.Clamp(this.camSmoothness, 0.01f, 1f);
			float t = Mathf.Clamp01(Time.deltaTime * num);
			num = 1f / Mathf.Clamp(this.shipSmoothness, 0.01f, 1f);
			float t2 = Mathf.Clamp01(Time.deltaTime * num);
			this.m_position.camDistance = Mathf.Lerp(this.m_position.camDistance, this.m_target.camDistance, t);
			this.m_position.camRotation = Vector2.Lerp(this.m_position.camRotation, this.m_target.camRotation, t);
			this.m_position.forwardAmount = Vector3.Lerp(this.m_position.forwardAmount, this.m_target.forwardAmount, t2);
			this.m_position.turnAmount = Mathf.Lerp(this.m_position.turnAmount, this.m_target.turnAmount, t2);
		}

		private void ProcessInput()
		{
			float shipMoveSpeed = this.m_shipMoveSpeed;
			float magnitude = this.m_velocity.magnitude;
			this.m_target.forwardAmount = Vector3.zero;
			this.m_target.turnAmount = 0f;
			if (Input.GetKey(KeyCode.A))
			{
				float num = shipMoveSpeed * magnitude;
				this.m_target.turnAmount = this.m_target.turnAmount - num * Time.deltaTime;
				this.m_target.camRotation.x = this.m_target.camRotation.x - num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.D))
			{
				float num2 = shipMoveSpeed * magnitude;
				this.m_target.turnAmount = this.m_target.turnAmount + num2 * Time.deltaTime;
				this.m_target.camRotation.x = this.m_target.camRotation.x + num2 * Time.deltaTime;
			}
			Vector3 a = this.Ship.transform.localToWorldMatrix * this.m_forward;
			a.Normalize();
			if (Input.GetKey(KeyCode.W))
			{
				this.m_acceleration += Time.deltaTime * 1f;
			}
			else
			{
				this.m_acceleration -= Time.deltaTime * 0.25f;
			}
			this.m_acceleration = Mathf.Clamp(this.m_acceleration, 0f, 1f);
			this.m_target.forwardAmount = this.m_target.forwardAmount + a * shipMoveSpeed * this.m_acceleration * Time.deltaTime;
			float a2 = Time.deltaTime * 1000f;
			float num3 = Mathf.Pow(1.02f, Mathf.Min(a2, 1f));
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				this.m_target.camDistance = this.m_target.camDistance * num3;
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				this.m_target.camDistance = this.m_target.camDistance / num3;
			}
			this.m_target.camDistance = Mathf.Max(1f, this.m_target.camDistance);
			this.m_target.camRotation.y = Mathf.Clamp(this.m_target.camRotation.y, 20f, 160f);
			if (Input.GetMouseButton(0))
			{
				this.m_target.camRotation.y = this.m_target.camRotation.y + Input.GetAxis("Mouse Y") * this.m_camRotationSpeed;
				this.m_target.camRotation.x = this.m_target.camRotation.x + Input.GetAxis("Mouse X") * this.m_camRotationSpeed;
			}
		}
	}
}
