using System;
using UnityEngine;

[RequireComponent(typeof(AeroplaneController))]
public class AeroplaneAIControl : MonoBehaviour
{
	[SerializeField]
	private float rollSensitivity = 0.2f;

	[SerializeField]
	private float pitchSensitivity = 0.5f;

	[SerializeField]
	private float lateralWanderDistance = 5f;

	[SerializeField]
	private float lateralWanderSpeed = 0.11f;

	[SerializeField]
	private float maxClimbAngle = 45f;

	[SerializeField]
	private float maxRollAngle = 45f;

	[SerializeField]
	private float speedEffect = 0.01f;

	[SerializeField]
	private float takeoffHeight = 20f;

	[SerializeField]
	private Transform target;

	private AeroplaneController aeroplaneController;

	private float randomPerlin;

	private bool takenOff;

	private void Awake()
	{
		this.aeroplaneController = base.GetComponent<AeroplaneController>();
		this.randomPerlin = UnityEngine.Random.Range(0f, 100f);
	}

	public void Reset()
	{
		this.takenOff = false;
	}

	private void FixedUpdate()
	{
		if (this.target != null)
		{
			Vector3 position = this.target.position + base.transform.right * (Mathf.PerlinNoise(Time.time * this.lateralWanderSpeed, this.randomPerlin) * 2f - 1f) * this.lateralWanderDistance;
			Vector3 vector = base.transform.InverseTransformPoint(position);
			float num = Mathf.Atan2(vector.x, vector.z);
			float num2 = -Mathf.Atan2(vector.y, vector.z);
			num2 = Mathf.Clamp(num2, -this.maxClimbAngle * 0.0174532924f, this.maxClimbAngle * 0.0174532924f);
			float num3 = num2 - this.aeroplaneController.PitchAngle;
			float throttleInput = 0.5f;
			float num4 = num3 * this.pitchSensitivity;
			float num5 = Mathf.Clamp(num, -this.maxRollAngle * 0.0174532924f, this.maxRollAngle * 0.0174532924f);
			float num6 = 0f;
			float num7 = 0f;
			if (!this.takenOff)
			{
				if (this.aeroplaneController.Altitude > this.takeoffHeight)
				{
					this.takenOff = true;
				}
			}
			else
			{
				num6 = num;
				num7 = -(this.aeroplaneController.RollAngle - num5) * this.rollSensitivity;
			}
			float num8 = 1f + this.aeroplaneController.ForwardSpeed * this.speedEffect;
			num7 *= num8;
			num4 *= num8;
			num6 *= num8;
			this.aeroplaneController.Move(num7, num4, num6, throttleInput, false);
		}
		else
		{
			this.aeroplaneController.Move(0f, 0f, 0f, 0f, false);
		}
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
	}
}
