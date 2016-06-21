using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), Tooltip("Sets the value of an integer variable using a float value.")]
	public class SetIntFromFloat : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		public FsmFloat floatValue;

		public bool everyFrame;

		public override void Reset()
		{
			this.intVariable = null;
			this.floatValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.intVariable.Value = (int)this.floatValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.intVariable.Value = (int)this.floatValue.Value;
		}
	}
}
