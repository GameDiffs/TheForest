using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Sends an Event when the specified Mouse Button is pressed. Optionally store the button state in a bool variable.")]
	public class GetMouseButtonDown : FsmStateAction
	{
		[RequiredField]
		public MouseButton button;

		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public override void Reset()
		{
			this.button = MouseButton.Left;
			this.sendEvent = null;
			this.storeResult = null;
		}

		public override void OnUpdate()
		{
			bool mouseButtonDown = Input.GetMouseButtonDown((int)this.button);
			if (mouseButtonDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = mouseButtonDown;
		}
	}
}
