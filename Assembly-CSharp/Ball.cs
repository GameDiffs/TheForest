using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private const float GroundRayLength = 1f;

	[SerializeField]
	private float movePower = 5f;

	[SerializeField]
	private bool useTorque = true;

	[SerializeField]
	private float maxAngularVelocity = 25f;

	[SerializeField]
	private float jumpPower = 2f;

	private void Start()
	{
		base.GetComponent<Rigidbody>().maxAngularVelocity = this.maxAngularVelocity;
	}

	public void Move(Vector3 move, bool jump)
	{
		Vector3 normalized = Camera.main.transform.TransformDirection(move).normalized;
		if (this.useTorque)
		{
			base.GetComponent<Rigidbody>().AddTorque(new Vector3(normalized.z, 0f, -normalized.x) * this.movePower);
		}
		else
		{
			base.GetComponent<Rigidbody>().AddForce(normalized * this.movePower);
		}
		if (Physics.Raycast(base.transform.position, -Vector3.up, 1f) && jump)
		{
			base.GetComponent<Rigidbody>().AddForce(Vector3.up * this.jumpPower, ForceMode.Impulse);
		}
	}
}
