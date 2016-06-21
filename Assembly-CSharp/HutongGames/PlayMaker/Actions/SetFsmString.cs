using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Set the value of a String Variable in another FSM.")]
	public class SetFsmString : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object."), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The name of the FSM variable."), UIHint(UIHint.FsmString)]
		public FsmString variableName;

		[HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
		public FsmString setValue;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		private GameObject goLastFrame;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
		}

		public override void OnEnter()
		{
			this.DoSetFsmString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		private void DoSetFsmString()
		{
			if (this.setValue == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.goLastFrame)
			{
				this.goLastFrame = ownerDefaultTarget;
				this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
			}
			if (this.fsm == null)
			{
				this.LogWarning("Could not find FSM: " + this.fsmName.Value);
				return;
			}
			FsmString fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
			if (fsmString != null)
			{
				fsmString.Value = this.setValue.Value;
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		public override void OnUpdate()
		{
			this.DoSetFsmString();
		}
	}
}
