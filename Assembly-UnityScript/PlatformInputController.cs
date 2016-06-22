using System;
using UnityEngine;

[AddComponentMenu("Character/Platform Input Controller"), RequireComponent(typeof(CharacterMotor))]
[Serializable]
public class PlatformInputController : MonoBehaviour
{
	public bool autoRotate;

	public float maxRotationSpeed;

	private CharacterMotor motor;

	public PlatformInputController()
	{
		this.autoRotate = true;
		this.maxRotationSpeed = (float)360;
	}

	public override void Awake()
	{
		this.motor = (CharacterMotor)this.GetComponent(typeof(CharacterMotor));
	}

	public override void Update()
	{
		Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), (float)0);
		if (vector != Vector3.zero)
		{
			float num = vector.magnitude;
			vector /= num;
			num = Mathf.Min((float)1, num);
			num *= num;
			vector *= num;
		}
		vector = Camera.main.transform.rotation * vector;
		Quaternion rotation = Quaternion.FromToRotation(-Camera.main.transform.forward, this.transform.up);
		vector = rotation * vector;
		this.motor.inputMoveDirection = vector;
		this.motor.inputJump = Input.GetButton("Jump");
		if (this.autoRotate && vector.sqrMagnitude > 0.01f)
		{
			Vector3 vector2 = this.ConstantSlerp(this.transform.forward, vector, this.maxRotationSpeed * Time.deltaTime);
			vector2 = this.ProjectOntoPlane(vector2, this.transform.up);
			this.transform.rotation = Quaternion.LookRotation(vector2, this.transform.up);
		}
	}

	public override Vector3 ProjectOntoPlane(Vector3 v, Vector3 normal)
	{
		return v - Vector3.Project(v, normal);
	}

	public override Vector3 ConstantSlerp(Vector3 from, Vector3 to, float angle)
	{
		float t = Mathf.Min((float)1, angle / Vector3.Angle(from, to));
		return Vector3.Slerp(from, to, t);
	}

	public override void Main()
	{
	}
}
