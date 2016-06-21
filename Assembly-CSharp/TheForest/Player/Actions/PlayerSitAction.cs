using HutongGames.PlayMaker;
using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player.Actions
{
	public class PlayerSitAction : MonoBehaviour
	{
		public float _zOffset = 1f;

		public float _seatOffset = 10f;

		private void SitOnBench(Transform trn)
		{
			FsmBool fsmBool = LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("seatedBool");
			if (!fsmBool.Value)
			{
				LocalPlayer.ScriptSetup.pmBlock.SendEvent("toReset");
				LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmGameObject("seatedGo").Value = trn.gameObject;
				LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmFloat("seatOffsetY").Value = trn.position.y;
				Vector3 vector = trn.InverseTransformPoint(LocalPlayer.Transform.position);
				float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				LocalPlayer.Transform.position = trn.position;
				if (num > -90f && num < 90f)
				{
					LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmFloat("seatOffset").Value = this._seatOffset * 20f;
					Vector3 vector2 = trn.position + trn.forward * -this._zOffset;
					LocalPlayer.Transform.position = new Vector3(vector2.x, LocalPlayer.Transform.position.y, vector2.z);
				}
				else
				{
					LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmFloat("seatOffset").Value = -this._seatOffset * 20f;
					Vector3 vector3 = trn.position + trn.forward * this._zOffset;
					LocalPlayer.Transform.position = new Vector3(vector3.x, LocalPlayer.Transform.position.y, vector3.z);
				}
				Debug.Log("2 LocalPlayer.Transform.position = " + LocalPlayer.Transform.position);
				LocalPlayer.MainRotator.enabled = false;
				LocalPlayer.CamRotator.rotationRange = new Vector2(80f, 140f);
				LocalPlayer.FpCharacter.CanJump = false;
				LocalPlayer.FpCharacter.Sitting = true;
				LocalPlayer.Stats.SitDown();
				fsmBool.Value = true;
			}
		}

		public void UpFromBench()
		{
			if (LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("seatedBool").Value)
			{
				LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("seatedBool").Value = false;
				LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
				LocalPlayer.MainRotator.resetOriginalRotation = true;
				LocalPlayer.MainRotator.enabled = true;
				LocalPlayer.MainRotator.rotationRange = new Vector2(0f, 999f);
				LocalPlayer.CamRotator.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				LocalPlayer.FpCharacter.CanJump = true;
				LocalPlayer.FpCharacter.Sitting = false;
				LocalPlayer.Stats.StandUp();
			}
		}

		public void forceDisableBench()
		{
			LocalPlayer.FpCharacter.CanJump = true;
			LocalPlayer.FpCharacter.Sitting = false;
			LocalPlayer.Animator.SetBoolReflected("sittingBool", false);
			LocalPlayer.Stats.StandUp();
		}
	}
}
