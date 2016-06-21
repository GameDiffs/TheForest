using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Gets the name of the previously active state and stores it in a String Variable.")]
	public class GetPreviousStateName : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmString storeName;

		public override void Reset()
		{
			this.storeName = null;
		}

		public override void OnEnter()
		{
			this.storeName.Value = ((base.Fsm.PreviousActiveState != null) ? base.Fsm.PreviousActiveState.Name : null);
			base.Finish();
		}
	}
}
