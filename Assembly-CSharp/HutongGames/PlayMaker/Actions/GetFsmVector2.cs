using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Get the value of a Vector2 Variable from another FSM.")]
	public class GetFsmVector2 : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object"), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[RequiredField, UIHint(UIHint.FsmVector2)]
		public FsmString variableName;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector2 storeValue;

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
			this.DoGetFsmVector2();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetFsmVector2();
		}

		private void DoGetFsmVector2()
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
			FsmVector2 fsmVector = this.fsm.FsmVariables.GetFsmVector2(this.variableName.Value);
			if (fsmVector == null)
			{
				return;
			}
			this.storeValue.Value = fsmVector.Value;
		}
	}
}
