using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.World;
using TheForest.Player;
using TheForest.Utils;
using UnityEngine;

public class playerEnterRockThrowerAction : MonoBehaviour
{
	private MultiThrowerItemHolder ih;

	private rockThrowerAimingReticle ar;

	private rockThrowerAnimEvents am;

	public GameObject currentTriggerGo;

	public GameObject currentThrowerGo;

	public GameObject currentLever;

	public GameObject aimReticleGo;

	private AnimatorStateInfo layer0;

	private AnimatorStateInfo nextlayer0;

	private int throwerHash = Animator.StringToHash("thrower");

	private int enterThrowerHash = Animator.StringToHash("enterThrower");

	private int loadHash = Animator.StringToHash("load");

	private bool fixLeverRotation;

	public string sfxString;

	private bool loadCheck;

	private void Start()
	{
	}

	private void setCurrentTrigger(GameObject go)
	{
		this.currentTriggerGo = go;
	}

	private void setCurrentThrower(GameObject go)
	{
		this.currentThrowerGo = go;
		this.am = go.GetComponentInChildren<rockThrowerAnimEvents>();
	}

	private void setCurrentLever(GameObject go)
	{
		this.currentLever = go;
	}

	private void enterRockThrower(GameObject go)
	{
		base.StartCoroutine(this.doThrower(go));
	}

	private void Update()
	{
		if (LocalPlayer.AnimControl.onRockThrower && BoltNetwork.isRunning && this.currentLever)
		{
			this.ih.state.leverRotate = this.currentLever.transform.localEulerAngles.y;
		}
	}

	private void LateUpdate()
	{
		if (this.currentThrowerGo == null)
		{
			return;
		}
		this.layer0 = LocalPlayer.Animator.GetCurrentAnimatorStateInfo(0);
		if (this.layer0.tagHash == this.enterThrowerHash)
		{
			if (!this.loadCheck)
			{
				if (BoltNetwork.isRunning)
				{
					this.ih.sendAnimVars(0, true);
				}
				else
				{
					this.currentThrowerGo.GetComponent<Animator>().SetBoolReflected("load", true);
				}
				this.loadCheck = true;
			}
			Animator component = this.currentThrowerGo.GetComponent<Animator>();
			AnimatorStateInfo currentAnimatorStateInfo = component.GetCurrentAnimatorStateInfo(0);
			component.Play(this.loadHash, 0, this.layer0.normalizedTime);
		}
		else
		{
			this.loadCheck = false;
		}
	}

	[DebuggerHidden]
	public IEnumerator doThrower(GameObject posGo)
	{
		playerEnterRockThrowerAction.<doThrower>c__Iterator197 <doThrower>c__Iterator = new playerEnterRockThrowerAction.<doThrower>c__Iterator197();
		<doThrower>c__Iterator.posGo = posGo;
		<doThrower>c__Iterator.<$>posGo = posGo;
		<doThrower>c__Iterator.<>f__this = this;
		return <doThrower>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator offsetLeverRotation()
	{
		playerEnterRockThrowerAction.<offsetLeverRotation>c__Iterator198 <offsetLeverRotation>c__Iterator = new playerEnterRockThrowerAction.<offsetLeverRotation>c__Iterator198();
		<offsetLeverRotation>c__Iterator.<>f__this = this;
		return <offsetLeverRotation>c__Iterator;
	}

	private void exitThrower()
	{
		base.StopCoroutine("offsetLeverRotation");
		this.fixLeverRotation = false;
		this.currentLever.transform.localEulerAngles = Vector3.zero;
		this.am.throwPos.gameObject.SetActive(false);
		this.currentLever.transform.localRotation = Quaternion.identity;
		if (BoltNetwork.isRunning)
		{
			this.ih.sendAnimVars(0, false);
		}
		else
		{
			this.currentThrowerGo.GetComponent<Animator>().SetBoolReflected("load", false);
		}
		LocalPlayer.Animator.SetBoolReflected("blockColdBool", false);
		FMODCommon.PlayOneshotNetworked("event:/traps/catapult_unload", base.transform, FMODCommon.NetworkRole.Any);
		if (BoltNetwork.isRunning)
		{
			this.ih.Invoke("enableTriggerMP", 1f);
		}
		else
		{
			base.Invoke("enableTrigger", 1f);
		}
		this.currentThrowerGo = null;
		this.currentLever = null;
		LocalPlayer.ScriptSetup.pmControl.enabled = true;
		LocalPlayer.ScriptSetup.pmControl.SendEvent("toReset2");
		LocalPlayer.Transform.parent = null;
		LocalPlayer.Transform.localScale = new Vector3(1f, 1f, 1f);
		LocalPlayer.CamFollowHead.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		LocalPlayer.Create.Grabber.gameObject.SetActive(true);
		LocalPlayer.CamFollowHead.followAnim = false;
		LocalPlayer.FpCharacter.enabled = true;
		LocalPlayer.MainRotator.enabled = true;
		LocalPlayer.CamRotator.enabled = true;
		LocalPlayer.AnimControl.onRope = false;
		LocalPlayer.AnimControl.lockGravity = false;
		LocalPlayer.AnimControl.controller.useGravity = true;
		LocalPlayer.AnimControl.controller.isKinematic = false;
		LocalPlayer.Inventory.ShowAllEquiped();
		LocalPlayer.AnimControl.useRootMotion = false;
		LocalPlayer.AnimControl.onRockThrower = false;
		LocalPlayer.Ridigbody.isKinematic = false;
		LocalPlayer.Ridigbody.useGravity = true;
		LocalPlayer.Animator.SetBoolReflected("setThrowerBool", false);
		LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
		LocalPlayer.MainRotator.rotationRange = new Vector2(0f, 999f);
		LocalPlayer.Animator.SetLayerWeightReflected(1, 1f);
		LocalPlayer.Animator.SetLayerWeightReflected(4, 1f);
	}

	private void enableTrigger()
	{
		this.currentTriggerGo.SetActive(true);
	}

	private void resetThrowerParams()
	{
	}

	private void resetRelease()
	{
		LocalPlayer.Animator.SetBoolReflected("releaseThrowerBool", false);
		if (BoltNetwork.isRunning)
		{
			this.ih.sendAnimVars(1, false);
		}
		else
		{
			this.currentThrowerGo.GetComponent<Animator>().SetBoolReflected("release", false);
		}
	}

	private void loadAmmo()
	{
		this.ih.forceRemoveItem();
	}
}
