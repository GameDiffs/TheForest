using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Sends an Event when a Key is released.")]
	public class GetKeyUp : FsmStateAction
	{
		[RequiredField]
		public KeyCode key;

		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public override void Reset()
		{
			this.sendEvent = null;
			this.key = KeyCode.None;
			this.storeResult = null;
		}

		public override void OnUpdate()
		{
			bool keyUp = Input.GetKeyUp(this.key);
			if (keyUp)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = keyUp;
		}
	}
}
