using System;
using UnityEngine;

public class lizardAnimEvents : MonoBehaviour
{
	private PlayMakerFSM pmBase;

	private void Start()
	{
		PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
		PlayMakerFSM[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			if (playMakerFSM.FsmName == "aiBaseFSM")
			{
				this.pmBase = playMakerFSM;
			}
		}
	}

	private void toMatchPos()
	{
		this.pmBase.SendEvent("toMatchPos");
	}
}
