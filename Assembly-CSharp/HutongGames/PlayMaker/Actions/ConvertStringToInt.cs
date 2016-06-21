using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert), Tooltip("Converts an String value to an Int value.")]
	public class ConvertStringToInt : FsmStateAction
	{
		[RequiredField, Tooltip("The String variable to convert to an integer."), UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		[RequiredField, Tooltip("Store the result in an Int variable."), UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		[Tooltip("Repeat every frame. Useful if the String variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.intVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoConvertStringToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoConvertStringToInt();
		}

		private void DoConvertStringToInt()
		{
			this.intVariable.Value = int.Parse(this.stringVariable.Value);
		}
	}
}
