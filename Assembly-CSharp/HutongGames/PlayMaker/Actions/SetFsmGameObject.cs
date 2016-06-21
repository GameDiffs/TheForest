using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Set the value of a Game Object Variable in another FSM. Accept null reference")]
	public class SetFsmGameObject : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object"), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The name of the FSM variable."), UIHint(UIHint.FsmGameObject)]
		public FsmString variableName;

		[HutongGames.PlayMaker.Tooltip("Set the value of the variable.")]
		public FsmGameObject setValue;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		private GameObject goLastFrame;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.setValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetFsmGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		private void DoSetFsmGameObject()
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
			if (this.fsm == null)
			{
				return;
			}
			FsmGameObject fsmGameObject = this.fsm.FsmVariables.FindFsmGameObject(this.variableName.Value);
			if (fsmGameObject != null)
			{
				fsmGameObject.Value = ((this.setValue != null) ? this.setValue.Value : null);
			}
			else
			{
				this.LogWarning("Could not find variable: " + this.variableName.Value);
			}
		}

		public override void OnUpdate()
		{
			this.DoSetFsmGameObject();
		}
	}
}
