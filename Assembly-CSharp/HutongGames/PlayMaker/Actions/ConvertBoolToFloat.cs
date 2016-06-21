using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert), Tooltip("Converts a Bool value to a Float value.")]
	public class ConvertBoolToFloat : FsmStateAction
	{
		[RequiredField, Tooltip("The Bool variable to test."), UIHint(UIHint.Variable)]
		public FsmBool boolVariable;

		[RequiredField, Tooltip("The Float variable to set based on the Bool variable value."), UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[Tooltip("Float value if Bool variable is false.")]
		public FsmFloat falseValue;

		[Tooltip("Float value if Bool variable is true.")]
		public FsmFloat trueValue;

		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.boolVariable = null;
			this.floatVariable = null;
			this.falseValue = 0f;
			this.trueValue = 1f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoConvertBoolToFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoConvertBoolToFloat();
		}

		private void DoConvertBoolToFloat()
		{
			this.floatVariable.Value = ((!this.boolVariable.Value) ? this.falseValue.Value : this.trueValue.Value);
		}
	}
}
