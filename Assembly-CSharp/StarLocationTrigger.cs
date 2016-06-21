using System;
using TheForest.Tools;
using UnityEngine;

public class StarLocationTrigger : MonoBehaviour
{
	private const string SEEN_EVENT = "event:/music/toy_pickup";

	public int MyInt;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			EventRegistry.Player.Publish(TfEvent.FoundStarLocation, this.MyInt);
			other.SendMessage("StarSeen" + this.MyInt, SendMessageOptions.DontRequireReceiver);
			FMOD_StudioSystem.instance.PlayOneShot("event:/music/toy_pickup", other.transform.position, null);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
