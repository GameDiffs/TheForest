using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Tests if the value of a Float variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class FloatChanged : FsmStateAction
	{
		[RequiredField, Tooltip("The Float variable to watch for a change."), UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[Tooltip("Event to send if the float variable changes.")]
		public FsmEvent changedEvent;

		[Tooltip("Set to True if the float variable changes."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		private float previousValue;

		public override void Reset()
		{
			this.floatVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			if (this.floatVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.floatVariable.Value;
		}

		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.floatVariable.Value != this.previousValue)
			{
				this.previousValue = this.floatVariable.Value;
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}
	}
}
