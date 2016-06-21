using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Sends Events based on the sign of a Float.")]
	public class FloatSignTest : FsmStateAction
	{
		[RequiredField, Tooltip("The float variable to test."), UIHint(UIHint.Variable)]
		public FsmFloat floatValue;

		[Tooltip("Event to send if the float variable is positive.")]
		public FsmEvent isPositive;

		[Tooltip("Event to send if the float variable is negative.")]
		public FsmEvent isNegative;

		[Tooltip("Repeat every frame. Useful if the variable is changing and you're waiting for a particular result.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.floatValue = 0f;
			this.isPositive = null;
			this.isNegative = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSignTest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSignTest();
		}

		private void DoSignTest()
		{
			if (this.floatValue == null)
			{
				return;
			}
			base.Fsm.Event((this.floatValue.Value >= 0f) ? this.isPositive : this.isNegative);
		}

		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.isPositive) && FsmEvent.IsNullOrEmpty(this.isNegative))
			{
				return "Action sends no events!";
			}
			return string.Empty;
		}
	}
}
