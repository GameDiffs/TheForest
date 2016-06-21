using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AeroplaneController : MonoBehaviour
{
	[SerializeField]
	private float maxEnginePower = 40f;

	[SerializeField]
	private float lift = 0.002f;

	[SerializeField]
	private float zeroLiftSpeed = 300f;

	[SerializeField]
	private float rollEffect = 1f;

	[SerializeField]
	private float pitchEffect = 1f;

	[SerializeField]
	private float yawEffect = 0.2f;

	[SerializeField]
	private float bankedTurnEffect = 0.5f;

	[SerializeField]
	private float aerodynamicEffect = 0.02f;

	[SerializeField]
	private float autoTurnPitch = 0.5f;

	[SerializeField]
	private float autoRollLevel = 0.2f;

	[SerializeField]
	private float autoPitchLevel = 0.2f;

	[SerializeField]
	private float airBrakesEffect = 3f;

	[SerializeField]
	private float throttleChangeSpeed = 0.3f;

	[SerializeField]
	private float dragIncreaseFactor = 0.001f;

	private float originalDrag;

	private float originalAngularDrag;

	private float aeroFactor;

	private bool immobilized;

	private float bankedTurnAmount;

	public float Altitude
	{
		get;
		private set;
	}

	public float Throttle
	{
		get;
		private set;
	}

	public bool AirBrakes
	{
		get;
		private set;
	}

	public float ForwardSpeed
	{
		get;
		private set;
	}

	public float EnginePower
	{
		get;
		private set;
	}

	public float MaxEnginePower
	{
		get
		{
			return this.maxEnginePower;
		}
	}

	public float RollAngle
	{
		get;
		private set;
	}

	public float PitchAngle
	{
		get;
		private set;
	}

	public float RollInput
	{
		get;
		private set;
	}

	public float PitchInput
	{
		get;
		private set;
	}

	public float YawInput
	{
		get;
		private set;
	}

	public float ThrottleInput
	{
		get;
		private set;
	}

	private void Start()
	{
		this.originalDrag = base.GetComponent<Rigidbody>().drag;
		this.originalAngularDrag = base.GetComponent<Rigidbody>().angularDrag;
	}

	public void Move(float rollInput, float pitchInput, float yawInput, float throttleInput, bool airBrakes)
	{
		this.RollInput = rollInput;
		this.PitchInput = pitchInput;
		this.YawInput = yawInput;
		this.ThrottleInput = throttleInput;
		this.AirBrakes = airBrakes;
		this.ClampInputs();
		this.CalculateRollAndPitchAngles();
		this.AutoLevel();
		this.CalculateForwardSpeed();
		this.ControlThrottle();
		this.CalculateDrag();
		this.CaluclateAerodynamicEffect();
		this.CalculateLinearForces();
		this.CalculateTorque();
		this.CalculateAltitude();
	}

	private void ClampInputs()
	{
		this.RollInput = Mathf.Clamp(this.RollInput, -1f, 1f);
		this.PitchInput = Mathf.Clamp(this.PitchInput, -1f, 1f);
		this.YawInput = Mathf.Clamp(this.YawInput, -1f, 1f);
		this.ThrottleInput = Mathf.Clamp(this.ThrottleInput, -1f, 1f);
	}

	private void CalculateRollAndPitchAngles()
	{
		Vector3 forward = base.transform.forward;
		forward.y = 0f;
		if (forward.sqrMagnitude > 0f)
		{
			forward.Normalize();
			Vector3 vector = base.transform.InverseTransformDirection(forward);
			this.PitchAngle = Mathf.Atan2(vector.y, vector.z);
			Vector3 direction = Vector3.Cross(Vector3.up, forward);
			Vector3 vector2 = base.transform.InverseTransformDirection(direction);
			this.RollAngle = Mathf.Atan2(vector2.y, vector2.x);
		}
	}

	private void AutoLevel()
	{
		this.bankedTurnAmount = Mathf.Sin(this.RollAngle);
		if (this.RollInput == 0f)
		{
			this.RollInput = -this.RollAngle * this.autoRollLevel;
		}
		if (this.PitchInput == 0f)
		{
			this.PitchInput = -this.PitchAngle * this.autoPitchLevel;
			this.PitchInput -= Mathf.Abs(this.bankedTurnAmount * this.bankedTurnAmount * this.autoTurnPitch);
		}
	}

	private void CalculateForwardSpeed()
	{
		this.ForwardSpeed = Mathf.Max(0f, base.transform.InverseTransformDirection(base.GetComponent<Rigidbody>().velocity).z);
	}

	private void ControlThrottle()
	{
		if (this.immobilized)
		{
			this.ThrottleInput = -0.5f;
		}
		this.Throttle = Mathf.Clamp01(this.Throttle + this.ThrottleInput * Time.deltaTime * this.throttleChangeSpeed);
		this.EnginePower = this.Throttle * this.maxEnginePower;
	}

	private void CalculateDrag()
	{
		float num = base.GetComponent<Rigidbody>().velocity.magnitude * this.dragIncreaseFactor;
		base.GetComponent<Rigidbody>().drag = ((!this.AirBrakes) ? (this.originalDrag + num) : ((this.originalDrag + num) * this.airBrakesEffect));
		base.GetComponent<Rigidbody>().angularDrag = this.originalAngularDrag * this.ForwardSpeed;
	}

	private void CaluclateAerodynamicEffect()
	{
		if (base.GetComponent<Rigidbody>().velocity.magnitude > 0f)
		{
			this.aeroFactor = Vector3.Dot(base.transform.forward, base.GetComponent<Rigidbody>().velocity.normalized);
			this.aeroFactor *= this.aeroFactor;
			Vector3 velocity = Vector3.Lerp(base.GetComponent<Rigidbody>().velocity, base.transform.forward * this.ForwardSpeed, this.aeroFactor * this.ForwardSpeed * this.aerodynamicEffect * Time.deltaTime);
			base.GetComponent<Rigidbody>().velocity = velocity;
		}
	}

	private void CalculateLinearForces()
	{
		Vector3 vector = Vector3.zero;
		vector += this.EnginePower * base.transform.forward;
		Vector3 normalized = Vector3.Cross(base.GetComponent<Rigidbody>().velocity, base.transform.right).normalized;
		float num = Mathf.InverseLerp(this.zeroLiftSpeed, 0f, this.ForwardSpeed);
		float d = this.ForwardSpeed * this.ForwardSpeed * this.lift * num * this.aeroFactor;
		vector += d * normalized;
		base.GetComponent<Rigidbody>().AddForce(vector);
	}

	private void CalculateTorque()
	{
		Vector3 a = Vector3.zero;
		a += this.PitchInput * this.pitchEffect * base.transform.right;
		a += this.YawInput * this.yawEffect * base.transform.up;
		a += -this.RollInput * this.rollEffect * base.transform.forward;
		a += this.bankedTurnAmount * this.bankedTurnEffect * base.transform.up;
		base.GetComponent<Rigidbody>().AddTorque(a * this.ForwardSpeed * this.aeroFactor);
	}

	private void CalculateAltitude()
	{
		Ray ray = new Ray(base.transform.position - Vector3.up * 10f, -Vector3.up);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit))
		{
			this.Altitude = raycastHit.distance + 10f;
		}
		else
		{
			this.Altitude = base.transform.position.y;
		}
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
