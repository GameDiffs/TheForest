using System;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarAIControl : MonoBehaviour
{
	public enum BrakeCondition
	{
		NeverBrake,
		TargetDirectionDifference,
		TargetDistance
	}

	[Range(0f, 1f), SerializeField]
	private float cautiousSpeedFactor = 0.05f;

	[Range(0f, 180f), SerializeField]
	private float cautiousMaxAngle = 50f;

	[SerializeField]
	private float cautiousMaxDistance = 100f;

	[SerializeField]
	private float cautiousAngularVelocityFactor = 30f;

	[SerializeField]
	private float steerSensitivity = 0.05f;

	[SerializeField]
	private float accelSensitivity = 0.04f;

	[SerializeField]
	private float brakeSensitivity = 1f;

	[SerializeField]
	private float lateralWanderDistance = 3f;

	[SerializeField]
	private float lateralWanderSpeed = 0.1f;

	[Range(0f, 1f), SerializeField]
	public float accelWanderAmount = 0.1f;

	[SerializeField]
	private float accelWanderSpeed = 0.1f;

	[SerializeField]
	private CarAIControl.BrakeCondition brakeCondition = CarAIControl.BrakeCondition.TargetDistance;

	[SerializeField]
	private bool driving;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private bool stopWhenTargetReached;

	[SerializeField]
	private float reachTargetThreshold = 2f;

	private float randomPerlin;

	private CarController carController;

	private float avoidOtherCarTime;

	private float avoidOtherCarSlowdown;

	private float avoidPathOffset;

	private void Awake()
	{
		this.carController = base.GetComponent<CarController>();
		this.randomPerlin = UnityEngine.Random.value * 100f;
	}

	private void FixedUpdate()
	{
		if (this.target == null || !this.driving)
		{
			float accelBrakeInput = Mathf.Clamp(-this.carController.CurrentSpeed, -1f, 1f);
			this.carController.Move(0f, accelBrakeInput);
		}
		else
		{
			Vector3 to = base.transform.forward;
			if (base.GetComponent<Rigidbody>().velocity.magnitude > this.carController.MaxSpeed * 0.1f)
			{
				to = base.GetComponent<Rigidbody>().velocity;
			}
			float num = this.carController.MaxSpeed;
			switch (this.brakeCondition)
			{
			case CarAIControl.BrakeCondition.TargetDirectionDifference:
			{
				float b = Vector3.Angle(this.target.forward, to);
				float a = base.GetComponent<Rigidbody>().angularVelocity.magnitude * this.cautiousAngularVelocityFactor;
				float t = Mathf.InverseLerp(0f, this.cautiousMaxAngle, Mathf.Max(a, b));
				num = Mathf.Lerp(this.carController.MaxSpeed, this.carController.MaxSpeed * this.cautiousSpeedFactor, t);
				break;
			}
			case CarAIControl.BrakeCondition.TargetDistance:
			{
				Vector3 vector = this.target.position - base.transform.position;
				float b2 = Mathf.InverseLerp(this.cautiousMaxDistance, 0f, vector.magnitude);
				float value = base.GetComponent<Rigidbody>().angularVelocity.magnitude * this.cautiousAngularVelocityFactor;
				float t2 = Mathf.Max(Mathf.InverseLerp(0f, this.cautiousMaxAngle, value), b2);
				num = Mathf.Lerp(this.carController.MaxSpeed, this.carController.MaxSpeed * this.cautiousSpeedFactor, t2);
				break;
			}
			}
			Vector3 vector2 = this.target.position;
			if (Time.time < this.avoidOtherCarTime)
			{
				num *= this.avoidOtherCarSlowdown;
				vector2 += this.target.right * this.avoidPathOffset;
			}
			else
			{
				vector2 += this.target.right * (Mathf.PerlinNoise(Time.time * this.lateralWanderSpeed, this.randomPerlin) * 2f - 1f) * this.lateralWanderDistance;
			}
			float num2 = (num >= this.carController.CurrentSpeed) ? this.accelSensitivity : this.brakeSensitivity;
			float num3 = Mathf.Clamp((num - this.carController.CurrentSpeed) * num2, -1f, 1f);
			num3 *= 1f - this.accelWanderAmount + Mathf.PerlinNoise(Time.time * this.accelWanderSpeed, this.randomPerlin) * this.accelWanderAmount;
			Vector3 vector3 = base.transform.InverseTransformPoint(vector2);
			float num4 = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
			float steerInput = Mathf.Clamp(num4 * this.steerSensitivity, -1f, 1f) * Mathf.Sign(this.carController.CurrentSpeed);
			this.carController.Move(steerInput, num3);
			if (this.stopWhenTargetReached && vector3.magnitude < this.reachTargetThreshold)
			{
				this.driving = false;
			}
		}
	}

	private void OnCollisionStay(Collision col)
	{
		if (col.rigidbody != null)
		{
			CarAIControl component = col.rigidbody.GetComponent<CarAIControl>();
			if (component != null)
			{
				this.avoidOtherCarTime = Time.time + 1f;
				if (Vector3.Angle(base.transform.forward, component.transform.position - base.transform.position) < 90f)
				{
					this.avoidOtherCarSlowdown = 0.5f;
				}
				else
				{
					this.avoidOtherCarSlowdown = 1f;
				}
				Vector3 vector = base.transform.InverseTransformPoint(component.transform.position);
				float f = Mathf.Atan2(vector.x, vector.z);
				this.avoidPathOffset = this.lateralWanderDistance * -Mathf.Sign(f);
			}
		}
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
		this.driving = true;
	}
}
