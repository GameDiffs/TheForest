using System;
using UnityEngine;

public class FeatherPhysics : MonoBehaviour
{
	private Vector3 torque;

	private void Start()
	{
		this.torque.x = (float)UnityEngine.Random.Range(-200, 200);
		this.torque.y = (float)UnityEngine.Random.Range(-200, 200);
		this.torque.z = (float)UnityEngine.Random.Range(-200, 200);
		base.GetComponent<ConstantForce>().torque = this.torque;
		base.GetComponent<Rigidbody>().AddForce(Vector3.up * 2.5f, ForceMode.Impulse);
		base.Invoke("FloatDown", 2f);
		base.Invoke("CleanUp", 30f);
	}

	private void FloatDown()
	{
		this.torque = new Vector3(0f, 0f, 0f);
		base.GetComponent<ConstantForce>().torque = this.torque;
		base.GetComponent<Rigidbody>().AddForce(Vector3.down * 3f, ForceMode.Impulse);
	}

	private void CleanUp()
	{
		Renderer component = base.GetComponent<Renderer>();
		if (!component || !component.IsVisibleFrom(Camera.main))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			base.Invoke("CleanUp", 4f);
		}
	}
}
