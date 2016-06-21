using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String), Tooltip("Replaces each format item in a specified string with the text equivalent of variable's value. Stores the result in a string variable.")]
	public class FormatString : FsmStateAction
	{
		[RequiredField, Tooltip("E.g. Hello {0} and {1}\nWith 2 variables that replace {0} and {1}\nSee C# string.Format docs.")]
		public FsmString format;

		[Tooltip("Variables to use for each formatting item.")]
		public FsmVar[] variables;

		[RequiredField, Tooltip("Store the formatted result in a string variable."), UIHint(UIHint.Variable)]
		public FsmString storeResult;

		[Tooltip("Repeat every frame. This is useful if the variables are changing.")]
		public bool everyFrame;

		private object[] objectArray;

		public override void Reset()
		{
			this.format = null;
			this.variables = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.objectArray = new object[this.variables.Length];
			this.DoFormatString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoFormatString();
		}

		private void DoFormatString()
		{
			for (int i = 0; i < this.variables.Length; i++)
			{
				this.variables[i].UpdateValue();
				this.objectArray[i] = this.variables[i].GetValue();
			}
			try
			{
				this.storeResult.Value = string.Format(this.format.Value, this.objectArray);
			}
			catch (FormatException ex)
			{
				this.LogError(ex.Message);
				base.Finish();
			}
		}
	}
}
