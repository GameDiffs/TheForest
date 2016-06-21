using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Set the value of a Game Object Variable in another All FSM. Accept null reference")]
	public class SetAllFsmGameObject : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public bool everyFrame;

		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		private void DoSetFsmGameObject()
		{
		}
	}
}
