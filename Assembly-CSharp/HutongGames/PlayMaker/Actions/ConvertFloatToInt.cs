using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Convert), HutongGames.PlayMaker.Tooltip("Converts a Float value to an Integer value.")]
	public class ConvertFloatToInt : FsmStateAction
	{
		public enum FloatRounding
		{
			RoundDown,
			RoundUp,
			Nearest
		}

		[RequiredField, HutongGames.PlayMaker.Tooltip("The Float variable to convert to an integer."), UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the result in an Integer variable."), UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		public ConvertFloatToInt.FloatRounding rounding;

		public bool everyFrame;

		public override void Reset()
		{
			this.floatVariable = null;
			this.intVariable = null;
			this.rounding = ConvertFloatToInt.FloatRounding.Nearest;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoConvertFloatToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoConvertFloatToInt();
		}

		private void DoConvertFloatToInt()
		{
			switch (this.rounding)
			{
			case ConvertFloatToInt.FloatRounding.RoundDown:
				this.intVariable.Value = Mathf.FloorToInt(this.floatVariable.Value);
				break;
			case ConvertFloatToInt.FloatRounding.RoundUp:
				this.intVariable.Value = Mathf.CeilToInt(this.floatVariable.Value);
				break;
			case ConvertFloatToInt.FloatRounding.Nearest:
				this.intVariable.Value = Mathf.RoundToInt(this.floatVariable.Value);
				break;
			}
		}
	}
}
