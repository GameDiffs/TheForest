using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Get the values of multiple variables in another FSM and store in variables of the same name in this FSM.")]
	public class GetFsmVariables : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object"), UIHint(UIHint.FsmName)]
		public FsmString fsmName;

		[HideTypeFilter, RequiredField, UIHint(UIHint.Variable)]
		public FsmVar[] getVariables;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject cachedGO;

		private PlayMakerFSM sourceFsm;

		private INamedVariable[] sourceVariables;

		private NamedVariable[] targetVariables;

		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.getVariables = null;
		}

		private void InitFsmVars()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.cachedGO)
			{
				this.sourceVariables = new INamedVariable[this.getVariables.Length];
				this.targetVariables = new NamedVariable[this.getVariables.Length];
				for (int i = 0; i < this.getVariables.Length; i++)
				{
					string variableName = this.getVariables[i].variableName;
					this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
					this.sourceVariables[i] = this.sourceFsm.FsmVariables.GetVariable(variableName);
					this.targetVariables[i] = base.Fsm.Variables.GetVariable(variableName);
					this.getVariables[i].Type = FsmUtility.GetVariableType(this.targetVariables[i]);
					if (!string.IsNullOrEmpty(variableName) && this.sourceVariables[i] == null)
					{
						this.LogWarning("Missing Variable: " + variableName);
					}
					this.cachedGO = ownerDefaultTarget;
				}
			}
		}

		public override void OnEnter()
		{
			this.InitFsmVars();
			this.DoGetFsmVariables();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetFsmVariables();
		}

		private void DoGetFsmVariables()
		{
			this.InitFsmVars();
			for (int i = 0; i < this.getVariables.Length; i++)
			{
				this.getVariables[i].GetValueFrom(this.sourceVariables[i]);
				this.getVariables[i].ApplyValueTo(this.targetVariables[i]);
			}
		}
	}
}
