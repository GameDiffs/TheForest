using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player.Actions
{
	public class PlayerClimbRopeAction : MonoBehaviour
	{
		public PlayerInventory _player;

		public float onRopeOffset;

		public float ropeAttachOffset;

		public float ropeAttachTopOffsetZ;

		public float ropeAttachTopOffsetY;

		public float ropeAttachTopOffsetX;

		public bool doingClimb;

		private int climbHash;

		private int climbIdleHash;

		private void Start()
		{
			this.climbHash = Animator.StringToHash("climbing");
			this.climbIdleHash = Animator.StringToHash("climbIdle");
		}

		private void enterClimbRope(Transform trn)
		{
			FsmBool fsmBool = this._player.PM.FsmVariables.GetFsmBool("climbBool");
			if (!fsmBool.Value && !this.doingClimb && !LocalPlayer.FpCharacter.Sitting)
			{
				LocalPlayer.CamFollowHead.stopAllCameraShake();
				LocalPlayer.Animator.SetBoolReflected("resetClimbBool", false);
				LocalPlayer.Animator.SetIntegerReflected("climbTypeInt", 0);
				LocalPlayer.FpCharacter.enabled = false;
				LocalPlayer.MainRotator.enabled = false;
				LocalPlayer.CamRotator.enabled = true;
				LocalPlayer.CamRotator.rotationRange = new Vector2(78f, 105f);
				LocalPlayer.AnimControl.lockGravity = true;
				Vector3 vector = trn.position - trn.forward * this.ropeAttachOffset;
				this._player.PM.FsmVariables.GetFsmVector3("attachPos").Value = vector;
				this._player.transform.position = vector;
				this._player.transform.rotation = trn.rotation;
				LocalPlayer.AnimControl.enterClimbMode();
				this._player.HideRightHand(false);
				this._player.PM.FsmVariables.GetFsmGameObject("climbGo").Value = trn.gameObject;
				LocalPlayer.Animator.SetBoolReflected("exitClimbTopBool", false);
				this._player.PM.SendEvent("toClimb");
				fsmBool.Value = true;
				this.doingClimb = true;
				base.StartCoroutine("stickToRope", trn);
			}
		}

		private void enterClimbRopeTop(Transform trn)
		{
			FsmBool fsmBool = this._player.PM.FsmVariables.GetFsmBool("climbBool");
			if (!fsmBool.Value && !this.doingClimb && !LocalPlayer.FpCharacter.Sitting)
			{
				LocalPlayer.CamFollowHead.stopAllCameraShake();
				LocalPlayer.Animator.SetBoolReflected("resetClimbBool", false);
				fsmBool.Value = true;
				LocalPlayer.FpCharacter.enabled = false;
				LocalPlayer.MainRotator.enabled = false;
				LocalPlayer.AnimControl.lockGravity = true;
				LocalPlayer.MainCamTr.localEulerAngles = new Vector3(0f, 0f, 0f);
				LocalPlayer.CamRotator.enabled = true;
				LocalPlayer.CamRotator.rotationRange = new Vector2(78f, 105f);
				Vector3 vector = trn.position + trn.forward * this.ropeAttachTopOffsetZ;
				vector += trn.right * this.ropeAttachTopOffsetX;
				if (LocalPlayer.FpCharacter.jumping)
				{
					vector.y = trn.position.y;
				}
				else
				{
					vector.y = this._player.transform.position.y - this.ropeAttachTopOffsetY;
				}
				this._player.PM.FsmVariables.GetFsmVector3("attachPos").Value = vector;
				this._player.transform.position = vector;
				this._player.transform.rotation = trn.rotation;
				LocalPlayer.AnimControl.enterClimbMode();
				this._player.PM.FsmVariables.GetFsmGameObject("climbGo").Value = trn.gameObject;
				LocalPlayer.Animator.SetIntegerReflected("climbTypeInt", 0);
				this._player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = true;
				LocalPlayer.Animator.SetBoolReflected("exitClimbTopBool", false);
				this._player.MemorizeItem(Item.EquipmentSlot.RightHand);
				this._player.StashEquipedWeapon(false);
				this._player.PM.SendEvent("toClimb");
				LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
				base.StartCoroutine("stickToRope", trn);
				base.Invoke("enableDoingClimb", 2.6f);
			}
		}

		private void setAttachPos()
		{
		}

		private void enableDoingClimb()
		{
			if (this._player.PM.FsmVariables.GetFsmBool("climbBool").Value)
			{
				this.doingClimb = true;
			}
		}

		private void exitClimbRopeTop(Transform trn)
		{
			if (this.doingClimb)
			{
				LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
				LocalPlayer.Animator.SetBoolReflected("setClimbBool", false);
				LocalPlayer.Animator.SetBoolReflected("exitClimbTopBool", true);
				this._player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = false;
				this._player.PM.FsmVariables.GetFsmBool("climbBool").Value = false;
				this._player.PM.SendEvent("toExitClimb");
				base.CancelInvoke("enableDoingClimb");
				base.StopCoroutine("stickToRope");
			}
		}

		private void exitClimbRopeGround()
		{
			if (this.doingClimb)
			{
				LocalPlayer.AnimControl.lockGravity = false;
				LocalPlayer.GameObject.GetComponent<Rigidbody>().useGravity = true;
				LocalPlayer.GameObject.GetComponent<Rigidbody>().isKinematic = false;
				LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
				LocalPlayer.Animator.SetIntegerReflected("climbDirInt", -1);
				LocalPlayer.Animator.SetBoolReflected("setClimbBool", false);
				this._player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = false;
				this._player.PM.FsmVariables.GetFsmBool("climbBool").Value = false;
				this._player.PM.SendEvent("toExitClimb");
				base.CancelInvoke("enableDoingClimb");
				base.StopCoroutine("stickToRope");
			}
		}

		private void resetClimbRope()
		{
			this.doingClimb = false;
			LocalPlayer.MainRotator.enabled = true;
			LocalPlayer.AnimControl.lockGravity = false;
			LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
			LocalPlayer.MainRotator.rotationRange = new Vector2(0f, 999f);
			LocalPlayer.FpCharacter.enabled = true;
			LocalPlayer.Inventory.ShowRightHand();
			base.StopCoroutine("stickToRope");
		}

		[DebuggerHidden]
		private IEnumerator stickToRope(Transform trn)
		{
			PlayerClimbRopeAction.<stickToRope>c__Iterator18C <stickToRope>c__Iterator18C = new PlayerClimbRopeAction.<stickToRope>c__Iterator18C();
			<stickToRope>c__Iterator18C.trn = trn;
			<stickToRope>c__Iterator18C.<$>trn = trn;
			<stickToRope>c__Iterator18C.<>f__this = this;
			return <stickToRope>c__Iterator18C;
		}
	}
}
