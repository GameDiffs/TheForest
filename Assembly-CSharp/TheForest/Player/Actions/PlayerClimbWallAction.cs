using HutongGames.PlayMaker;
using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player.Actions
{
	public class PlayerClimbWallAction : MonoBehaviour
	{
		public PlayerInventory _player;

		public float ropeAttachOffset;

		public float ropeAttachTopOffsetZ;

		public float ropeAttachTopOffsetX;

		public bool doingClimb;

		private void enterClimbWall(Transform trn)
		{
			FsmBool fsmBool = this._player.PM.FsmVariables.GetFsmBool("climbBool");
			if (!fsmBool.Value && !this.doingClimb)
			{
				LocalPlayer.CamFollowHead.stopAllCameraShake();
				LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
				LocalPlayer.Animator.SetIntegerReflected("climbTypeInt", 1);
				LocalPlayer.FpCharacter.enabled = false;
				LocalPlayer.MainRotator.enabled = false;
				LocalPlayer.MainCamTr.localEulerAngles = new Vector3(-35f, 0f, 0f);
				LocalPlayer.CamRotator.enabled = true;
				LocalPlayer.AnimControl.lockGravity = true;
				LocalPlayer.CamRotator.rotationRange = new Vector2(150f, 150f);
				Vector3 vector = trn.position - trn.forward * this.ropeAttachOffset;
				this._player.PM.FsmVariables.GetFsmVector3("attachPos").Value = vector;
				this._player.transform.position = vector;
				this._player.transform.rotation = trn.rotation;
				LocalPlayer.AnimControl.enterClimbMode();
				this._player.PM.FsmVariables.GetFsmGameObject("climbGo").Value = trn.gameObject;
				LocalPlayer.Animator.SetBoolReflected("exitClimbTopBool", false);
				this._player.PM.SendEvent("toClimb");
				fsmBool.Value = true;
				this.doingClimb = true;
			}
		}

		private void enterClimbWallTop(Transform trn)
		{
			FsmBool fsmBool = this._player.PM.FsmVariables.GetFsmBool("climbBool");
			if (!fsmBool.Value && !this.doingClimb)
			{
				LocalPlayer.CamFollowHead.stopAllCameraShake();
				LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
				LocalPlayer.FpCharacter.enabled = false;
				LocalPlayer.MainRotator.enabled = false;
				LocalPlayer.MainCamTr.localEulerAngles = new Vector3(0f, 0f, 0f);
				LocalPlayer.CamRotator.enabled = true;
				LocalPlayer.CamRotator.rotationRange = new Vector2(150f, 150f);
				LocalPlayer.AnimControl.lockGravity = true;
				Vector3 vector = trn.position + trn.forward * this.ropeAttachTopOffsetZ;
				vector += trn.right * this.ropeAttachTopOffsetX;
				vector.y = this._player.transform.position.y - 1.2f;
				this._player.PM.FsmVariables.GetFsmVector3("attachPos").Value = vector;
				this._player.transform.position = vector;
				this._player.transform.rotation = trn.rotation;
				LocalPlayer.AnimControl.enterClimbMode();
				this._player.PM.FsmVariables.GetFsmGameObject("climbGo").Value = trn.gameObject;
				LocalPlayer.Animator.SetIntegerReflected("climbTypeInt", 1);
				this._player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = true;
				LocalPlayer.Animator.SetBoolReflected("exitClimbTopBool", false);
				this._player.PM.SendEvent("toClimb");
				fsmBool.Value = true;
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

		private void exitClimbWallTop(Transform trn)
		{
			if (this.doingClimb)
			{
				LocalPlayer.AnimControl.lockGravity = false;
				LocalPlayer.Animator.SetBoolReflected("setClimbBool", false);
				LocalPlayer.Animator.SetBoolReflected("exitClimbTopBool", true);
				this._player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = false;
				this._player.PM.FsmVariables.GetFsmBool("climbBool").Value = false;
				this._player.PM.SendEvent("toExitClimb");
				base.CancelInvoke("enableDoingClimb");
				base.enabled = false;
			}
		}

		private void exitClimbWallGround()
		{
			if (this.doingClimb)
			{
				LocalPlayer.AnimControl.lockGravity = false;
				LocalPlayer.Animator.SetIntegerReflected("climbDirInt", -1);
				LocalPlayer.Animator.SetBoolReflected("setClimbBool", false);
				this._player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = false;
				this._player.PM.FsmVariables.GetFsmBool("climbBool").Value = false;
				this._player.PM.SendEvent("toExitClimb");
				base.CancelInvoke("enableDoingClimb");
				base.enabled = false;
			}
		}

		private void resetClimbWall()
		{
			this.doingClimb = false;
			LocalPlayer.AnimControl.lockGravity = false;
			LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
			LocalPlayer.CamRotator.transform.localEulerAngles = Vector3.Scale(LocalPlayer.CamRotator.transform.localEulerAngles, Vector3.right);
			LocalPlayer.FpCharacter.enabled = true;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().freezeRotation = true;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().useGravity = true;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
