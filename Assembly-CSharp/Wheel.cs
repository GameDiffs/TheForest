using System;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour
{
	public Transform wheelModel;

	public Transform skidTrailPrefab;

	public static Transform skidTrailsDetachedParent;

	public float loQualDist = 100f;

	public bool steerable;

	public bool powered;

	[SerializeField]
	private float particleRate = 3f;

	[SerializeField]
	private float slideThreshold = 10f;

	private float spinAngle;

	private float particleEmit;

	private float sidewaysStiffness;

	private float forwardStiffness;

	private float spinoutFactor;

	private float sideSlideFactor;

	private float springCompression;

	private ParticleSystem skidSmokeSystem;

	private Rigidbody rb;

	private WheelFrictionCurve sidewaysFriction;

	private WheelFrictionCurve forwardFriction;

	private Transform skidTrail;

	private bool leavingSkidTrail;

	private RaycastHit hit;

	private Vector3 relativeVelocity;

	private float sideSlideFactorTarget;

	private float spinoutFactorTarget;

	private float accelAmount;

	private float burnoutFactor;

	private float burnoutGrip;

	private float spinoutGrip;

	private float sideSlideGrip;

	private float minGrip;

	private float springCompressionGripModifier;

	private float burnoutRpm;

	private float skidFactorTarget;

	private bool ignore;

	private Vector3 originalWheelModelPosition;

	private float dummyAngle;

	public float Rpm
	{
		get;
		private set;
	}

	public float MaxRpm
	{
		get;
		private set;
	}

	public float SkidFactor
	{
		get;
		private set;
	}

	public bool OnGround
	{
		get;
		private set;
	}

	public Transform Hub
	{
		get;
		set;
	}

	public WheelCollider wheelCollider
	{
		get;
		private set;
	}

	public CarController car
	{
		get;
		private set;
	}

	public float suspensionSpringPos
	{
		get;
		private set;
	}

	private void Start()
	{
		this.car = base.transform.parent.GetComponent<CarController>();
		this.wheelCollider = (base.GetComponent<Collider>() as WheelCollider);
		if (this.wheelModel != null)
		{
			this.originalWheelModelPosition = this.wheelModel.localPosition;
			base.transform.position = this.wheelModel.position;
		}
		this.sidewaysFriction = this.wheelCollider.sidewaysFriction;
		this.forwardFriction = this.wheelCollider.forwardFriction;
		this.sidewaysStiffness = this.wheelCollider.sidewaysFriction.stiffness;
		this.forwardStiffness = this.wheelCollider.forwardFriction.stiffness;
		this.MaxRpm = this.car.MaxSpeed / (3.14159274f * this.wheelCollider.radius * 2f) * 60f;
		this.skidSmokeSystem = this.car.GetComponentInChildren<ParticleSystem>();
		this.rb = this.wheelCollider.attachedRigidbody;
		if (Wheel.skidTrailsDetachedParent == null)
		{
			Wheel.skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
		}
	}

	private void FixedUpdate()
	{
		this.relativeVelocity = base.transform.InverseTransformDirection(this.rb.velocity);
		this.sideSlideFactorTarget = Mathf.Clamp01(Mathf.Abs(this.relativeVelocity.x * this.slideThreshold / this.car.MaxSpeed) * (this.car.SpeedFactor * 0.5f + 0.5f));
		this.sideSlideFactor = ((this.sideSlideFactorTarget <= this.sideSlideFactor) ? Mathf.Lerp(this.sideSlideFactor, this.sideSlideFactorTarget, Time.deltaTime) : this.sideSlideFactorTarget);
		this.spinoutFactorTarget = Mathf.Clamp01(this.rb.angularVelocity.magnitude * 57.29578f * 0.01f * ((1f - this.car.SpeedFactor) * 0.5f + 0.5f));
		this.spinoutFactorTarget = Mathf.Lerp(0f, this.spinoutFactorTarget, this.car.SpeedFactor + ((!this.powered) ? 0f : this.car.AccelInput));
		this.spinoutFactor = ((this.spinoutFactorTarget <= this.spinoutFactor) ? Mathf.Lerp(this.spinoutFactor, this.spinoutFactorTarget, Time.deltaTime) : this.spinoutFactorTarget);
		this.accelAmount = this.wheelCollider.motorTorque / this.car.MaxTorque;
		this.burnoutFactor = 0f;
		if (this.powered)
		{
			this.burnoutFactor = (this.accelAmount - (1f - this.car.BurnoutTendency)) / (1f - this.car.BurnoutTendency);
		}
		this.burnoutGrip = Mathf.Lerp(1f, 1f - this.car.BurnoutSlipEffect, this.burnoutFactor);
		this.spinoutGrip = Mathf.Lerp(1f, 1f - this.car.SpinoutSlipEffect, this.spinoutFactor);
		this.sideSlideGrip = Mathf.Lerp(1f, 1f - this.car.SideSlideEffect, this.sideSlideFactor);
		this.minGrip = Mathf.Min(this.burnoutGrip, this.spinoutGrip);
		this.minGrip = Mathf.Min(this.sideSlideGrip, this.minGrip);
		this.springCompressionGripModifier = this.springCompression + 0.6f;
		this.springCompressionGripModifier *= this.springCompressionGripModifier;
		this.sidewaysFriction.stiffness = this.sidewaysStiffness * this.minGrip * this.springCompressionGripModifier;
		this.forwardFriction.stiffness = this.forwardStiffness * this.burnoutGrip * this.springCompressionGripModifier;
		this.wheelCollider.sidewaysFriction = this.sidewaysFriction;
		this.wheelCollider.forwardFriction = this.forwardFriction;
		this.burnoutRpm = this.car.MaxSpeed * this.car.BurnoutTendency / (3.14159274f * this.wheelCollider.radius * 2f) * 60f;
		this.Rpm = ((this.burnoutRpm <= this.wheelCollider.rpm) ? this.wheelCollider.rpm : Mathf.Lerp(this.wheelCollider.rpm, this.burnoutRpm, this.burnoutFactor));
		if (this.OnGround)
		{
			this.skidFactorTarget = Mathf.Max(this.burnoutFactor * 2f, this.sideSlideFactor * this.rb.velocity.magnitude * 0.05f);
			this.skidFactorTarget = Mathf.Max(this.skidFactorTarget, this.spinoutFactor * this.rb.velocity.magnitude * 0.05f);
			this.skidFactorTarget = Mathf.Clamp01(-0.1f + this.skidFactorTarget * 1.1f);
			this.SkidFactor = Mathf.MoveTowards(this.SkidFactor, this.skidFactorTarget, Time.deltaTime * 2f);
			if (this.skidSmokeSystem != null)
			{
				this.particleEmit += this.SkidFactor * Time.deltaTime;
				if (this.particleEmit > this.particleRate)
				{
					this.particleEmit = 0f;
					this.skidSmokeSystem.transform.position = base.transform.position - base.transform.up * this.wheelCollider.radius;
					this.skidSmokeSystem.Emit(1);
				}
			}
		}
		if (!(this.skidTrailPrefab != null) || this.SkidFactor <= 0.5f || this.OnGround)
		{
		}
		this.spinAngle += this.Rpm * 6f * Time.deltaTime;
		float sqrMagnitude = (Camera.main.transform.position - base.transform.position).sqrMagnitude;
		bool flag = true;
		if (sqrMagnitude > this.loQualDist * this.loQualDist)
		{
			float num = Mathf.Lerp(1f, 0.2f, Mathf.InverseLerp(this.loQualDist * this.loQualDist, this.loQualDist * this.loQualDist * 4f, sqrMagnitude));
			flag = (UnityEngine.Random.value < num);
		}
		if (!flag)
		{
			return;
		}
		if (Physics.Raycast(base.transform.position, -base.transform.up, out this.hit, this.wheelCollider.suspensionDistance + this.wheelCollider.radius))
		{
			this.suspensionSpringPos = -(this.hit.distance - this.wheelCollider.radius);
			this.springCompression = Mathf.InverseLerp(-this.wheelCollider.suspensionDistance, this.wheelCollider.suspensionDistance, this.suspensionSpringPos);
			this.OnGround = true;
		}
		else
		{
			this.suspensionSpringPos = -this.wheelCollider.suspensionDistance;
			this.OnGround = false;
			this.springCompression = 0f;
			this.SkidFactor = 0f;
		}
		if (this.wheelModel != null)
		{
			this.wheelModel.localPosition = this.originalWheelModelPosition + Vector3.up * this.suspensionSpringPos;
			this.dummyAngle += this.car.CurrentSpeed * 4f;
			if (this.steerable)
			{
				this.wheelModel.localRotation = Quaternion.AngleAxis(this.car.CurrentSteerAngle, Vector3.up) * Quaternion.Euler(this.dummyAngle, 0f, 0f);
			}
			else
			{
				this.wheelModel.localRotation = Quaternion.Euler(this.dummyAngle, 0f, 0f);
			}
		}
	}
}
