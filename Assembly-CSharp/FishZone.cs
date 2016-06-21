using System;
using UnityEngine;

public class FishZone : MonoBehaviour
{
	public static Vector3 ScarePos;

	private bool Delay;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && !this.Delay)
		{
			this.Delay = true;
			FishZone.ScarePos = new Vector3(0f, other.transform.position.y, 0f);
			base.BroadcastMessage("Retreat", SendMessageOptions.DontRequireReceiver);
			base.Invoke("ResetDelay", 2f);
		}
		else if (other.gameObject.CompareTag("Rock") && !this.Delay)
		{
			this.Delay = true;
			FishZone.ScarePos = new Vector3(0f, other.transform.position.y, 0f);
			base.BroadcastMessage("Retreat", SendMessageOptions.DontRequireReceiver);
			base.Invoke("ResetDelay", 2f);
		}
	}

	private void ResetDelay()
	{
		this.Delay = false;
	}
}
