using System;
using TheForest.Utils;
using UnityEngine;

public class FireWarmth : MonoBehaviour
{
	public GameObject player;

	private void OnTriggerStay(Collider otherObject)
	{
		if (LocalPlayer.Stats && (this.player == otherObject.gameObject || otherObject.gameObject.CompareTag("Player")))
		{
			LocalPlayer.Stats.Heat();
			this.player = otherObject.gameObject;
		}
	}

	private void OnTriggerExit(Collider otherObject)
	{
		if (LocalPlayer.Stats && (this.player == otherObject.gameObject || otherObject.gameObject.CompareTag("Player")))
		{
			LocalPlayer.Stats.LeaveHeat();
			this.player = null;
		}
	}

	private void OnDestroy()
	{
		if (this.player && LocalPlayer.Stats)
		{
			LocalPlayer.Stats.LeaveHeat();
		}
	}
}
