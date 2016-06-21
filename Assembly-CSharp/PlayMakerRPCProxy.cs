using System;
using UnityEngine;

public class PlayMakerRPCProxy : MonoBehaviour
{
	public PlayMakerFSM[] fsms;

	public void Reset()
	{
		this.fsms = base.GetComponents<PlayMakerFSM>();
	}

	[RPC]
	public void ForwardEvent(string eventName)
	{
		PlayMakerFSM[] array = this.fsms;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			playMakerFSM.SendEvent(eventName);
		}
	}
}
