using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Sends an Event based on the value of a String Variable.")]
	public class StringSwitch : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		[CompoundArray("String Switches", "Compare String", "Send Event")]
		public FsmString[] compareTo;

		public FsmEvent[] sendEvent;

		public bool everyFrame;

		public override void Reset()
		{
			this.stringVariable = null;
			this.compareTo = new FsmString[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoStringSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoStringSwitch();
		}

		private void DoStringSwitch()
		{
			if (this.stringVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (this.stringVariable.Value == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}
	}
}
