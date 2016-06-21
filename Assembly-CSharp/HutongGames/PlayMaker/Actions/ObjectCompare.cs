using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Compare 2 Object Variables and send events based on the result.")]
	public class ObjectCompare : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmObject objectVariable;

		[RequiredField]
		public FsmObject compareTo;

		[Tooltip("Event to send if the 2 object values are equal.")]
		public FsmEvent equalEvent;

		[Tooltip("Event to send if the 2 object values are not equal.")]
		public FsmEvent notEqualEvent;

		[Tooltip("Store the result in a variable."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.objectVariable = null;
			this.compareTo = null;
			this.storeResult = null;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoObjectCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoObjectCompare();
		}

		private void DoObjectCompare()
		{
			bool flag = this.objectVariable.Value == this.compareTo.Value;
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.notEqualEvent : this.equalEvent);
		}
	}
}
