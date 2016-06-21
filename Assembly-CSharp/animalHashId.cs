using System;
using UnityEngine;

public class animalHashId : MonoBehaviour
{
	private PlayMakerFSM pm;

	public int onTree;

	private void Start()
	{
		PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
		PlayMakerFSM[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			if (playMakerFSM.FsmName == "aiBaseFSM")
			{
				this.pm = playMakerFSM;
			}
		}
		this.pm.FsmVariables.GetFsmInt("HashOnTree").Value = Animator.StringToHash("onTree");
	}
}
