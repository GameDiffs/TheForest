using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Sends an Event based on the value of an Integer Variable.")]
	public class IntSwitch : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		[CompoundArray("Int Switches", "Compare Int", "Send Event")]
		public FsmInt[] compareTo;

		public FsmEvent[] sendEvent;

		public bool everyFrame;

		public override void Reset()
		{
			this.intVariable = null;
			this.compareTo = new FsmInt[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoIntSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoIntSwitch();
		}

		private void DoIntSwitch()
		{
			if (this.intVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (this.intVariable.Value == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}
	}
}
