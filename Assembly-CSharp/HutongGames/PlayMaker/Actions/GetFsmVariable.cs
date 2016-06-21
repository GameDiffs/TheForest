using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Get the value of a variable in another FSM and store it in a variable of the same name in this FSM.")]
	public class GetFsmVariable : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object"), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[HideTypeFilter, RequiredField, UIHint(UIHint.Variable)]
		public FsmVar storeValue;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject cachedGO;

		private PlayMakerFSM sourceFsm;

		private INamedVariable sourceVariable;

		private NamedVariable targetVariable;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = new FsmVar();
		}

		public override void OnEnter()
		{
			this.InitFsmVar();
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

		private void InitFsmVar()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.cachedGO)
			{
				this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
				this.sourceVariable = this.sourceFsm.FsmVariables.GetVariable(this.storeValue.variableName);
				this.targetVariable = base.Fsm.Variables.GetVariable(this.storeValue.variableName);
				this.storeValue.Type = FsmUtility.GetVariableType(this.targetVariable);
				if (!string.IsNullOrEmpty(this.storeValue.variableName) && this.sourceVariable == null)
				{
					this.LogWarning("Missing Variable: " + this.storeValue.variableName);
				}
				this.cachedGO = ownerDefaultTarget;
			}
		}

		private void DoGetFsmVariable()
		{
			if (this.storeValue.IsNone)
			{
				return;
			}
			this.InitFsmVar();
			this.storeValue.GetValueFrom(this.sourceVariable);
			this.storeValue.ApplyValueTo(this.targetVariable);
		}
	}
}
