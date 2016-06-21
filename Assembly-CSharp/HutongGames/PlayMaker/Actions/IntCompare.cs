using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Sends Events based on the comparison of 2 Integers.")]
	public class IntCompare : FsmStateAction
	{
		[RequiredField]
		public FsmInt integer1;

		[RequiredField]
		public FsmInt integer2;

		[Tooltip("Event sent if Int 1 equals Int 2")]
		public FsmEvent equal;

		[Tooltip("Event sent if Int 1 is less than Int 2")]
		public FsmEvent lessThan;

		[Tooltip("Event sent if Int 1 is greater than Int 2")]
		public FsmEvent greaterThan;

		public bool everyFrame;

		public override void Reset()
		{
			this.integer1 = 0;
			this.integer2 = 0;
			this.equal = null;
			this.lessThan = null;
			this.greaterThan = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoIntCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoIntCompare();
		}

		private void DoIntCompare()
		{
			if (this.integer1.Value == this.integer2.Value)
			{
				base.Fsm.Event(this.equal);
				return;
			}
			if (this.integer1.Value < this.integer2.Value)
			{
				base.Fsm.Event(this.lessThan);
				return;
			}
			if (this.integer1.Value > this.integer2.Value)
			{
				base.Fsm.Event(this.greaterThan);
			}
		}

		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan))
			{
				return "Action sends no events!";
			}
			return string.Empty;
		}
	}
}
