using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert), Tooltip("Converts a Bool value to an Integer value.")]
	public class ConvertBoolToInt : FsmStateAction
	{
		[RequiredField, Tooltip("The Bool variable to test."), UIHint(UIHint.Variable)]
		public FsmBool boolVariable;

		[RequiredField, Tooltip("The Integer variable to set based on the Bool variable value."), UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		[Tooltip("Integer value if Bool variable is false.")]
		public FsmInt falseValue;

		[Tooltip("Integer value if Bool variable is false.")]
		public FsmInt trueValue;

		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.boolVariable = null;
			this.intVariable = null;
			this.falseValue = 0;
			this.trueValue = 1;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoConvertBoolToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoConvertBoolToInt();
		}

		private void DoConvertBoolToInt()
		{
			this.intVariable.Value = ((!this.boolVariable.Value) ? this.falseValue.Value : this.trueValue.Value);
		}
	}
}
