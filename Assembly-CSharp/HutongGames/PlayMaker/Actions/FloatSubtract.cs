using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Subtracts a value from a Float Variable.")]
	public class FloatSubtract : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The float variable to subtract from."), UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Value to subtract from the float variable.")]
		public FsmFloat subtract;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		[HutongGames.PlayMaker.Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
		public bool perSecond;

		public override void Reset()
		{
			this.floatVariable = null;
			this.subtract = null;
			this.everyFrame = false;
			this.perSecond = false;
		}

		public override void OnEnter()
		{
			this.DoFloatSubtract();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoFloatSubtract();
		}

		private void DoFloatSubtract()
		{
			if (!this.perSecond)
			{
				this.floatVariable.Value -= this.subtract.Value;
			}
			else
			{
				this.floatVariable.Value -= this.subtract.Value * Time.deltaTime;
			}
		}
	}
}
