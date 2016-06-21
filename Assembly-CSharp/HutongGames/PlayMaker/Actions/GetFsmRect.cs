using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Get the value of a Rect Variable from another FSM.")]
	public class GetFsmRect : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object"), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[RequiredField, UIHint(UIHint.FsmRect)]
		public FsmString variableName;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmRect storeValue;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject goLastFrame;

		protected PlayMakerFSM fsm;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.variableName = string.Empty;
			this.storeValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		private void DoGetFsmVariable()
		{
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
			if (this.fsm == null || this.storeValue == null)
			{
				return;
			}
			FsmRect fsmRect = this.fsm.FsmVariables.GetFsmRect(this.variableName.Value);
			if (fsmRect != null)
			{
				this.storeValue.Value = fsmRect.Value;
			}
		}
	}
}
