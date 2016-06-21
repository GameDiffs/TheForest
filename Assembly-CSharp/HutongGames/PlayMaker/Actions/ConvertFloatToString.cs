using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert), Tooltip("Converts a Float value to a String value with optional format.")]
	public class ConvertFloatToString : FsmStateAction
	{
		[RequiredField, Tooltip("The float variable to convert."), UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField, Tooltip("A string variable to store the converted value."), UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		[Tooltip("Optional Format, allows for leading zeroes. E.g., 0000")]
		public FsmString format;

		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.floatVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
			this.format = null;
		}

		public override void OnEnter()
		{
			this.DoConvertFloatToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoConvertFloatToString();
		}

		private void DoConvertFloatToString()
		{
			if (this.format.IsNone || string.IsNullOrEmpty(this.format.Value))
			{
				this.stringVariable.Value = this.floatVariable.Value.ToString();
			}
			else
			{
				this.stringVariable.Value = this.floatVariable.Value.ToString(this.format.Value);
			}
		}
	}
}
