using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert), HutongGames.PlayMaker.Tooltip("Converts a Bool value to a Color.")]
	public class ConvertBoolToColor : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The Bool variable to test."), UIHint(UIHint.Variable)]
		public FsmBool boolVariable;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The Color variable to set based on the bool variable value."), UIHint(UIHint.Variable)]
		public FsmColor colorVariable;

		[HutongGames.PlayMaker.Tooltip("Color if Bool variable is false.")]
		public FsmColor falseColor;

		[HutongGames.PlayMaker.Tooltip("Color if Bool variable is true.")]
		public FsmColor trueColor;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.boolVariable = null;
			this.colorVariable = null;
			this.falseColor = Color.black;
			this.trueColor = Color.white;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoConvertBoolToColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoConvertBoolToColor();
		}

		private void DoConvertBoolToColor()
		{
			this.colorVariable.Value = ((!this.boolVariable.Value) ? this.falseColor.Value : this.trueColor.Value);
		}
	}
}
