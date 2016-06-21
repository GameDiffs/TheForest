using System;
using UnityEngine;

public class playerOnRaftSetup : MonoBehaviour
{
	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			FirstPersonCharacter component = other.gameObject.GetComponent<FirstPersonCharacter>();
			if (component && component.Grounded)
			{
				component.rb.mass = 0.1f;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			FirstPersonCharacter component = other.gameObject.GetComponent<FirstPersonCharacter>();
			if (component)
			{
				component.rb.mass = component.defaultMass;
			}
		}
	}
}
