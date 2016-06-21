using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Compares 2 Game Objects and sends Events based on the result.")]
	public class GameObjectCompare : FsmStateAction
	{
		[RequiredField, Title("Game Object"), Tooltip("A Game Object variable to compare."), UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObjectVariable;

		[RequiredField, Tooltip("Compare the variable with this Game Object")]
		public FsmGameObject compareTo;

		[Tooltip("Send this event if Game Objects are equal")]
		public FsmEvent equalEvent;

		[Tooltip("Send this event if Game Objects are not equal")]
		public FsmEvent notEqualEvent;

		[Tooltip("Store the result of the check in a Bool Variable. (True if equal, false if not equal)."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame. Useful if you're waiting for a true or false result.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.gameObjectVariable = null;
			this.compareTo = null;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGameObjectCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGameObjectCompare();
		}

		private void DoGameObjectCompare()
		{
			bool flag = base.Fsm.GetOwnerDefaultTarget(this.gameObjectVariable) == this.compareTo.Value;
			this.storeResult.Value = flag;
			if (flag && this.equalEvent != null)
			{
				base.Fsm.Event(this.equalEvent);
			}
			else if (!flag && this.notEqualEvent != null)
			{
				base.Fsm.Event(this.notEqualEvent);
			}
		}
	}
}
