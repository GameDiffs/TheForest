using System;
using UnityEngine;

[Serializable]
public class ThrowMolotov : MonoBehaviour
{
	public Transform Molotov;

	public override void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Transform transform = (Transform)UnityEngine.Object.Instantiate(this.Molotov, this.transform.position, Quaternion.identity);
			transform.GetComponent<Rigidbody>().AddForce((float)600 * this.transform.forward);
			Physics.IgnoreCollision(this.transform.root.GetComponent<Collider>(), transform.GetComponent<Collider>());
		}
	}

	public override void Main()
	{
	}
}
