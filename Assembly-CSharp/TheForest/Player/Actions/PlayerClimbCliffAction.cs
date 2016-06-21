using HutongGames.PlayMaker;
using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player.Actions
{
	public class PlayerClimbCliffAction : MonoBehaviour
	{
		public PlayerInventory _player;

		public float ropeAttachOffset;

		private Vector3 enterPos;

		public bool doingClimb;

		private void enterClimbCliff(Transform trn)
		{
			FsmBool fsmBool = this._player.PM.FsmVariables.GetFsmBool("climbBool");
			if (!fsmBool.Value && !this.doingClimb)
			{
				LocalPlayer.CamFollowHead.stopAllCameraShake();
				LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
				LocalPlayer.Animator.SetIntegerReflected("climbTypeInt", 1);
				LocalPlayer.FpCharacter.enabled = false;
				LocalPlayer.MainRotator.enabled = false;
				LocalPlayer.CamRotator.enabled = true;
				LocalPlayer.CamRotator.rotationRange = new Vector2(90f, 105f);
				LocalPlayer.AnimControl.enterClimbMode();
				LocalPlayer.AnimControl.cliffClimb = true;
				LocalPlayer.AnimControl.lockGravity = true;
				LocalPlayer.Ridigbody.velocity = Vector3.zero;
				this._player.PM.FsmVariables.GetFsmGameObject("climbGo").Value = trn.gameObject;
				LocalPlayer.Animator.SetBoolReflected("exitClimbTopBool", false);
				this._player.PM.SendEvent("toClimb");
				fsmBool.Value = true;
				this.doingClimb = true;
				Vector3 vector;
				LocalPlayer.Transform.position.y = vector.y + 1f;
				LocalPlayer.Transform.position = this.enterPos;
				LocalPlayer.Transform.position -= LocalPlayer.Transform.forward;
			}
		}

		private void setEnterClimbPos(Vector3 pos)
		{
			this.enterPos = pos;
		}

		private void enableDoingClimbCliff()
		{
			if (this._player.PM.FsmVariables.GetFsmBool("climbBool").Value)
			{
				this.doingClimb = true;
			}
		}

		private void exitClimbCliffTop(Transform trn)
		{
			if (this.doingClimb)
			{
				LocalPlayer.Animator.SetBoolReflected("setClimbBool", false);
				LocalPlayer.Animator.SetBoolReflected("exitClimbTopBool", true);
				this._player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = false;
				this._player.PM.FsmVariables.GetFsmBool("climbBool").Value = false;
				this._player.PM.SendEvent("toExitClimb");
				base.CancelInvoke("enableDoingClimb");
				base.enabled = false;
			}
		}

		public void exitClimbCliffGround()
		{
			if (this.doingClimb)
			{
				LocalPlayer.Animator.SetIntegerReflected("climbDirInt", -1);
				LocalPlayer.Animator.SetBoolReflected("setClimbBool", false);
				this._player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = false;
				this._player.PM.FsmVariables.GetFsmBool("climbBool").Value = false;
				this._player.PM.SendEvent("toExitClimb");
				base.CancelInvoke("enableDoingClimb");
				LocalPlayer.AnimControl.cliffClimb = false;
				LocalPlayer.AnimControl.lockGravity = false;
				LocalPlayer.AnimControl.allowCliffReset = false;
				LocalPlayer.AnimControl.CancelInvoke("enableCliffReset");
				LocalPlayer.AnimControl.playerHeadCollider.enabled = true;
				base.enabled = false;
			}
		}

		private void resetClimbCliff()
		{
			LocalPlayer.AnimControl.playerHeadCollider.enabled = true;
			this.doingClimb = false;
			LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
			LocalPlayer.CamRotator.transform.localEulerAngles = Vector3.Scale(LocalPlayer.CamRotator.transform.localEulerAngles, Vector3.right);
			LocalPlayer.FpCharacter.enabled = true;
			LocalPlayer.AnimControl.lockGravity = false;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().freezeRotation = true;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().useGravity = true;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().isKinematic = false;
			LocalPlayer.AnimControl.CancelInvoke("enableCliffReset");
			LocalPlayer.AnimControl.allowCliffReset = false;
		}
	}
}
