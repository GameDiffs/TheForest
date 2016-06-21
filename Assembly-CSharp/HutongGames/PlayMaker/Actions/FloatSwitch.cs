using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Sends an Event based on the value of a Float Variable. The float could represent distance, angle to a target, health left... The array sets up float ranges that correspond to Events.")]
	public class FloatSwitch : FsmStateAction
	{
		[RequiredField, Tooltip("The float variable to test."), UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[CompoundArray("Float Switches", "Less Than", "Send Event")]
		public FsmFloat[] lessThan;

		public FsmEvent[] sendEvent;

		[Tooltip("Repeat every frame. Useful if the variable is changing.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.floatVariable = null;
			this.lessThan = new FsmFloat[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoFloatSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoFloatSwitch();
		}

		private void DoFloatSwitch()
		{
			if (this.floatVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.lessThan.Length; i++)
			{
				if (this.floatVariable.Value < this.lessThan[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}
	}
}
