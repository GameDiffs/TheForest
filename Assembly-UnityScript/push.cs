using System;
using UnityEngine;

[Serializable]
public class push : MonoBehaviour
{
	public float pushPower;

	public push()
	{
		this.pushPower = 1f;
	}

	public override void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (!(attachedRigidbody == null) && !attachedRigidbody.isKinematic)
		{
			if (hit.moveDirection.y >= -0.3f)
			{
				Vector3 a = new Vector3(hit.moveDirection.x, (float)0, hit.moveDirection.z);
				attachedRigidbody.velocity = a * this.pushPower;
			}
		}
	}

	public override void Main()
	{
	}
}
