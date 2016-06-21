using HutongGames.PlayMaker;
using System;
using UnityEngine;

public class playerHashIDs : MonoBehaviour
{
	private PlayMakerFSM pmControl;

	private int idleTag;

	private FsmInt idleTagFSM;

	private FsmInt combo1Hash;

	private FsmInt combo2Hash;

	private FsmInt heldHash;

	private void Start()
	{
		PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
		PlayMakerFSM[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			if (playMakerFSM.FsmName == "controlFSM")
			{
				this.pmControl = playMakerFSM;
			}
		}
		this.idleTagFSM = this.pmControl.FsmVariables.GetFsmInt("idlingHash");
		this.combo1Hash = this.pmControl.FsmVariables.GetFsmInt("combo1Hash");
		this.combo2Hash = this.pmControl.FsmVariables.GetFsmInt("combo2Hash");
		this.heldHash = this.pmControl.FsmVariables.GetFsmInt("heldHash");
		this.idleTagFSM.Value = Animator.StringToHash("idling");
		this.combo1Hash.Value = Animator.StringToHash("combo1");
		this.combo2Hash.Value = Animator.StringToHash("combo2");
		this.pmControl.FsmVariables.GetFsmInt("combo3Hash").Value = Animator.StringToHash("combo3");
		this.pmControl.FsmVariables.GetFsmInt("axeCombo1Hash").Value = Animator.StringToHash("axeCombo1");
		this.heldHash.Value = Animator.StringToHash("held");
	}
}
