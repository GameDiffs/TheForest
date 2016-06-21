using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Sends Events based on a float being inside a window of values.")]
	public class FloatWindowCompare : FsmStateAction
	{
		[RequiredField, Tooltip("Float to Check")]
		public FsmFloat floatTest;

		[RequiredField, Tooltip("Lower limit for window")]
		public FsmFloat floatLower;

		[RequiredField, Tooltip("Upper limit for window")]
		public FsmFloat floatUpper;

		[Tooltip("Event sent if FloatTest is inside the window")]
		public FsmEvent inWindow;

		[Tooltip("Event sent if FloatTest is outside the window")]
		public FsmEvent outOfWindow;

		public bool everyFrame;

		public override void Reset()
		{
			this.floatTest = 0f;
			this.floatLower = 0f;
			this.floatUpper = 0f;
			this.inWindow = null;
			this.outOfWindow = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoCompare();
		}

		private void DoCompare()
		{
			if (this.floatTest.Value > this.floatLower.Value && this.floatTest.Value < this.floatUpper.Value)
			{
				base.Fsm.Event(this.inWindow);
				return;
			}
			if (this.floatTest.Value < this.floatLower.Value || this.floatTest.Value > this.floatUpper.Value)
			{
				base.Fsm.Event(this.outOfWindow);
				return;
			}
		}

		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.inWindow) && FsmEvent.IsNullOrEmpty(this.outOfWindow))
			{
				return "Action sends no events!";
			}
			return string.Empty;
		}
	}
}
