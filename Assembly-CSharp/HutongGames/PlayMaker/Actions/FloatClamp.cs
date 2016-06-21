using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Clamps the value of Float Variable to a Min/Max range.")]
	public class FloatClamp : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Float variable to clamp."), UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The minimum value.")]
		public FsmFloat minValue;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The maximum value.")]
		public FsmFloat maxValue;

		[HutongGames.PlayMaker.Tooltip("Repeate every frame. Useful if the float variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.floatVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoClamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoClamp();
		}

		private void DoClamp()
		{
			this.floatVariable.Value = Mathf.Clamp(this.floatVariable.Value, this.minValue.Value, this.maxValue.Value);
		}
	}
}
