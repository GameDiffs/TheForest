using HutongGames.PlayMaker;
using System;
using TheForest.Utils;
using UnityEngine;

public class smashHitTrigger : MonoBehaviour
{
	private PlayMakerFSM pmControl;

	private PlayMakerFSM pmRotate;

	private FsmBool fsmSmashBool;

	private FsmGameObject fsmSmashGo;

	private bool enable;

	private GameObject activeGo;

	private void Start()
	{
		this.fsmSmashBool = LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("smashBool");
		this.fsmSmashGo = LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmGameObject("smashGo");
	}

	private void Update()
	{
		if (this.enable && this.activeGo && !this.activeGo.activeSelf)
		{
			this.forceExit();
		}
	}

	private void OnTriggerStay(Collider source)
	{
		if (source.gameObject.CompareTag("smash") && !this.enable)
		{
			this.activeGo = source.gameObject;
			this.fsmSmashBool.Value = true;
			this.fsmSmashGo.Value = base.gameObject;
			this.enable = true;
		}
	}

	private void forceExit()
	{
		this.fsmSmashBool.Value = false;
		this.enable = false;
	}

	private void OnTriggerExit(Collider source)
	{
		if (source.gameObject.CompareTag("smash"))
		{
			this.fsmSmashBool.Value = false;
			this.enable = false;
		}
	}
}
