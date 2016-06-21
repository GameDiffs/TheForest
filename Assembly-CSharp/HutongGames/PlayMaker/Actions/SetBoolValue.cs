using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), Tooltip("Sets the value of a Bool Variable.")]
	public class SetBoolValue : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmBool boolVariable;

		[RequiredField]
		public FsmBool boolValue;

		public bool everyFrame;

		public override void Reset()
		{
			this.boolVariable = null;
			this.boolValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.boolVariable.Value = this.boolValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.boolVariable.Value = this.boolValue.Value;
		}
	}
}
