using System;
using UnityEngine;

public class CoopSuitcasePusher : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (BoltNetwork.isRunning && collision != null && collision.rigidbody && collision.rigidbody.gameObject.CompareTag("Float"))
		{
			CoopSuitcase componentInChildren = collision.gameObject.GetComponentInChildren<CoopSuitcase>();
			if (componentInChildren)
			{
				componentInChildren.PushedByClient(-collision.contacts[0].normal);
			}
		}
	}
}
