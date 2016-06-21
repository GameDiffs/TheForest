using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
	[Serializable]
	public class Advanced
	{
		[Range(0f, 1f)]
		public float burnoutSlipEffect = 0.4f;

		[Range(0f, 1f)]
		public float burnoutTendency = 0.2f;

		[Range(0f, 1f)]
		public float spinoutSlipEffect = 0.5f;

		[Range(0f, 1f)]
		public float sideSlideEffect = 0.5f;

		public float downForce = 30f;

		public int numGears = 5;

		[Range(0f, 1f)]
		public float gearDistributionBias = 0.2f;

		public float steeringCorrection = 2f;

		public float oppositeLockSteeringCorrection = 4f;

		public float reversingSpeedFactor = 0.3f;

		public float skidGearLockFactor = 0.1f;

		public float accelChangeSmoothing = 2f;

		public float gearFactorSmoothing = 5f;

		[Range(0f, 1f)]
		public float revRangeBoundary = 0.8f;
	}

	[SerializeField]
	private float maxSteerAngle = 28f;

	[SerializeField]
	private float steeringResponseSpeed = 200f;

	[Range(0f, 1f), SerializeField]
	private float maxSpeedSteerAngle = 0.23f;

	[Range(0f, 0.5f), SerializeField]
	private float maxSpeedSteerResponse = 0.5f;

	[SerializeField]
	private float maxSpeed = 60f;

	[SerializeField]
	private float maxTorque = 35f;

	[SerializeField]
	private float minTorque = 10f;

	[SerializeField]
	private float brakePower = 40f;

	[SerializeField]
	private float adjustCentreOfMass = 0.25f;

	[SerializeField]
	private CarController.Advanced advanced;

	[SerializeField]
	private bool preserveDirectionWhileInAir;

	private float[] gearDistribution;

	private Wheel[] wheels;

	private float accelBrake;

	private float smallSpeed;

	private float maxReversingSpeed;

	private bool immobilized;

	private bool anyOnGround;

	private float curvedSpeedFactor;

	private bool reversing;

	private float targetAccelInput;

	public int GearNum
	{
		get;
		set;
	}

	public float CurrentSpeed
	{
		get;
		set;
	}

	public float CurrentSteerAngle
	{
		get;
		set;
	}

	public float AccelInput
	{
		get;
		set;
	}

	public float BrakeInput
	{
		get;
		set;
	}

	public float GearFactor
	{
		get;
		set;
	}

	public float AvgPowerWheelRpmFactor
	{
		get;
		set;
	}

	public float AvgSkid
	{
		get;
		set;
	}

	public float RevsFactor
	{
		get;
		set;
	}

	public float SpeedFactor
	{
		get;
		set;
	}

	public int NumGears
	{
		get
		{
			return this.advanced.numGears;
		}
	}

	public float MaxSpeed
	{
		get
		{
			return this.maxSpeed;
		}
	}

	public float MaxTorque
	{
		get
		{
			return this.maxTorque;
		}
	}

	public float BurnoutSlipEffect
	{
		get
		{
			return this.advanced.burnoutSlipEffect;
		}
	}

	public float BurnoutTendency
	{
		get
		{
			return this.advanced.burnoutTendency;
		}
	}

	public float SpinoutSlipEffect
	{
		get
		{
			return this.advanced.spinoutSlipEffect;
		}
	}

	public float SideSlideEffect
	{
		get
		{
			return this.advanced.sideSlideEffect;
		}
	}

	public float MaxSteerAngle
	{
		get
		{
			return this.maxSteerAngle;
		}
	}

	private void Awake()
	{
		if (BoltNetwork.isServer)
		{
			this.wheels = base.GetComponentsInChildren<Wheel>();
			this.SetUpGears();
			base.gameObject.SetActive(false);
			base.gameObject.SetActive(true);
			this.smallSpeed = this.maxSpeed * 0.05f;
			this.maxReversingSpeed = this.maxSpeed * this.advanced.reversingSpeedFactor;
		}
	}

	private void OnEnable()
	{
		base.GetComponent<Rigidbody>().centerOfMass = Vector3.up * this.adjustCentreOfMass;
	}

	public void Move(float steerInput, float accelBrakeInput)
	{
		if (this.immobilized)
		{
			accelBrakeInput = 0f;
		}
		this.ConvertInputToAccelerationAndBraking(accelBrakeInput);
		this.CalculateSpeedValues();
		this.HandleGearChanging();
		this.CalculateGearFactor();
		this.ProcessWheels(steerInput);
		this.ApplyDownforce();
		this.CalculateRevs();
		this.PreserveDirectionInAir();
	}

	private void ConvertInputToAccelerationAndBraking(float accelBrakeInput)
	{
		this.reversing = false;
		if (accelBrakeInput > 0f)
		{
			if (this.CurrentSpeed > -this.smallSpeed)
			{
				this.targetAccelInput = accelBrakeInput;
				this.BrakeInput = 0f;
			}
			else
			{
				this.BrakeInput = accelBrakeInput;
				this.targetAccelInput = 0f;
			}
		}
		else if (this.CurrentSpeed > this.smallSpeed)
		{
			this.BrakeInput = -accelBrakeInput;
			this.targetAccelInput = 0f;
		}
		else
		{
			this.BrakeInput = 0f;
			this.targetAccelInput = accelBrakeInput;
			this.reversing = true;
		}
		this.AccelInput = Mathf.MoveTowards(this.AccelInput, this.targetAccelInput, Time.deltaTime * this.advanced.accelChangeSmoothing);
	}

	private void CalculateSpeedValues()
	{
		this.CurrentSpeed = base.transform.InverseTransformDirection(base.GetComponent<Rigidbody>().velocity).z;
		this.SpeedFactor = Mathf.InverseLerp(0f, (!this.reversing) ? this.maxSpeed : this.maxReversingSpeed, Mathf.Abs(this.CurrentSpeed));
		this.curvedSpeedFactor = ((!this.reversing) ? this.CurveFactor(this.SpeedFactor) : 0f);
	}

	private void HandleGearChanging()
	{
		if (!this.reversing)
		{
			if (this.SpeedFactor < this.gearDistribution[this.GearNum] && this.GearNum > 0)
			{
				this.GearNum--;
			}
			if (this.SpeedFactor > this.gearDistribution[this.GearNum + 1] && this.AvgSkid < this.advanced.skidGearLockFactor && this.GearNum < this.advanced.numGears - 1)
			{
				this.GearNum++;
			}
		}
	}

	private void CalculateGearFactor()
	{
		float to = Mathf.InverseLerp(this.gearDistribution[this.GearNum], this.gearDistribution[this.GearNum + 1], Mathf.Abs(this.AvgPowerWheelRpmFactor));
		this.GearFactor = Mathf.Lerp(this.GearFactor, to, Time.deltaTime * this.advanced.gearFactorSmoothing);
	}

	private void ProcessWheels(float steerInput)
	{
		this.AvgPowerWheelRpmFactor = 0f;
		this.AvgSkid = 0f;
		int num = 0;
		this.anyOnGround = false;
		Wheel[] array = this.wheels;
		for (int i = 0; i < array.Length; i++)
		{
			Wheel wheel = array[i];
			WheelCollider wheelCollider = wheel.wheelCollider;
			if (wheel.steerable)
			{
				float num2 = Mathf.Lerp(this.steeringResponseSpeed, this.steeringResponseSpeed * this.maxSpeedSteerResponse, this.curvedSpeedFactor);
				float num3 = Mathf.Lerp(this.maxSteerAngle, this.maxSteerAngle * this.maxSpeedSteerAngle, this.curvedSpeedFactor);
				if (steerInput == 0f)
				{
					num2 *= this.advanced.steeringCorrection;
				}
				if (Mathf.Sign(steerInput) != Mathf.Sign(this.CurrentSteerAngle))
				{
					num2 *= this.advanced.oppositeLockSteeringCorrection;
				}
				this.CurrentSteerAngle = Mathf.MoveTowards(this.CurrentSteerAngle, steerInput * num3, Time.deltaTime * num2);
				wheelCollider.steerAngle = this.CurrentSteerAngle;
			}
			this.AvgSkid += wheel.SkidFactor;
			if (wheel.powered)
			{
				float num4 = Mathf.Lerp(this.maxTorque, (this.SpeedFactor >= 1f) ? 0f : this.minTorque, (!this.reversing) ? this.curvedSpeedFactor : this.SpeedFactor);
				wheelCollider.motorTorque = this.AccelInput * num4;
				this.AvgPowerWheelRpmFactor += wheel.Rpm / wheel.MaxRpm;
				num++;
			}
			wheelCollider.brakeTorque = this.BrakeInput * this.brakePower;
			if (wheel.OnGround)
			{
				this.anyOnGround = true;
			}
		}
		this.AvgPowerWheelRpmFactor /= (float)num;
		this.AvgSkid /= (float)this.wheels.Length;
	}

	private void ApplyDownforce()
	{
		if (this.anyOnGround)
		{
			base.GetComponent<Rigidbody>().AddForce(-base.transform.up * this.curvedSpeedFactor * this.advanced.downForce);
		}
	}

	private void CalculateRevs()
	{
		float num = (float)this.GearNum / (float)this.NumGears;
		float from = this.ULerp(0f, this.advanced.revRangeBoundary, this.CurveFactor(num));
		float to = this.ULerp(this.advanced.revRangeBoundary, 1f, num);
		this.RevsFactor = this.ULerp(from, to, this.GearFactor);
	}

	private void PreserveDirectionInAir()
	{
		if (!this.anyOnGround && this.preserveDirectionWhileInAir && base.GetComponent<Rigidbody>().velocity.magnitude > this.smallSpeed)
		{
			base.GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(base.GetComponent<Rigidbody>().rotation, Quaternion.LookRotation(base.GetComponent<Rigidbody>().velocity), Time.deltaTime));
			base.GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(base.GetComponent<Rigidbody>().angularVelocity, Vector3.zero, Time.deltaTime);
		}
	}

	private float CurveFactor(float factor)
	{
		return 1f - (1f - factor) * (1f - factor);
	}

	private float ULerp(float from, float to, float value)
	{
		return (1f - value) * from + value * to;
	}

	private void SetUpGears()
	{
		this.gearDistribution = new float[this.advanced.numGears + 1];
		for (int i = 0; i <= this.advanced.numGears; i++)
		{
			float num = (float)i / (float)this.advanced.numGears;
			float to = num * num * num;
			float to2 = 1f - (1f - num) * (1f - num) * (1f - num);
			if (this.advanced.gearDistributionBias < 0.5f)
			{
				num = Mathf.Lerp(num, to, 1f - this.advanced.gearDistributionBias * 2f);
			}
			else
			{
				num = Mathf.Lerp(num, to2, (this.advanced.gearDistributionBias - 0.5f) * 2f);
			}
			this.gearDistribution[i] = num;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.GetComponent<Rigidbody>().position + Vector3.up * this.adjustCentreOfMass, 0.2f);
	}

	public void Immobilize()
	{
		this.immobilized = true;
	}

	public void Reset()
	{
		this.immobilized = false;
	}
}
