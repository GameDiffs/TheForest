using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert), Tooltip("Converts an Integer value to a String value with an optional format.")]
	public class ConvertIntToString : FsmStateAction
	{
		[RequiredField, Tooltip("The Int variable to convert."), UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		[RequiredField, Tooltip("A String variable to store the converted value."), UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		[Tooltip("Optional Format, allows for leading zeroes. E.g., 0000")]
		public FsmString format;

		[Tooltip("Repeat every frame. Useful if the Int variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.intVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
			this.format = null;
		}

		public override void OnEnter()
		{
			this.DoConvertIntToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoConvertIntToString();
		}

		private void DoConvertIntToString()
		{
			if (this.format.IsNone || string.IsNullOrEmpty(this.format.Value))
			{
				this.stringVariable.Value = this.intVariable.Value.ToString();
			}
			else
			{
				this.stringVariable.Value = this.intVariable.Value.ToString(this.format.Value);
			}
		}
	}
}
