using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), Tooltip("Sets the value of a Float Variable.")]
	public class SetFloatValue : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat floatValue;

		public bool everyFrame;

		public override void Reset()
		{
			this.floatVariable = null;
			this.floatValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.floatVariable.Value = this.floatValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.floatVariable.Value = this.floatValue.Value;
		}
	}
}
