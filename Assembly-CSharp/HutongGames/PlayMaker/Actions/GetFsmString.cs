using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Get the value of a String Variable from another FSM.")]
	public class GetFsmString : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object"), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[RequiredField, UIHint(UIHint.FsmString)]
		public FsmString variableName;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString storeValue;

		public bool everyFrame;

		private GameObject goLastFrame;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		public override void OnEnter()
		{
			this.DoGetFsmString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetFsmString();
		}

		private void DoGetFsmString()
		{
			if (this.storeValue == null)
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
				return;
			}
			FsmString fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
			if (fsmString == null)
			{
				return;
			}
			this.storeValue.Value = fsmString.Value;
		}
	}
}
