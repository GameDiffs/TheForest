using System;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			base.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.forward * 10f, base.transform.position + new Vector3(0.3f, 0f, 0f), ForceMode.Impulse);
		}
	}
}
