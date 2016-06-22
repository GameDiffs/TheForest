using System;
using UnityEngine;

[AddComponentMenu("Character/FPS Input Controller"), RequireComponent(typeof(CharacterMotor))]
[Serializable]
public class FPSInputController : MonoBehaviour
{
	private CharacterMotor motor;

	public override void Awake()
	{
		this.motor = (CharacterMotor)this.GetComponent(typeof(CharacterMotor));
	}

	public override void Update()
	{
		Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), (float)0, Input.GetAxis("Vertical"));
		if (vector != Vector3.zero)
		{
			float num = vector.magnitude;
			vector /= num;
			num = Mathf.Min((float)1, num);
			num *= num;
			vector *= num;
		}
		this.motor.inputMoveDirection = this.transform.rotation * vector;
		this.motor.inputJump = Input.GetButton("Jump");
	}

	public override void Main()
	{
	}
}
