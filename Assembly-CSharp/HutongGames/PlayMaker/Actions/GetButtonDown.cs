using System;
using TheForest.Utils;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), Tooltip("Sends an Event when a Button is pressed.")]
	public class GetButtonDown : FsmStateAction
	{
		[RequiredField, Tooltip("The name of the button. Set in the Unity Input Manager.")]
		public FsmString buttonName;

		[Tooltip("Event to send if the button is pressed.")]
		public FsmEvent sendEvent;

		[Tooltip("Set to True if the button is pressed."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public override void Reset()
		{
			this.buttonName = "Fire1";
			this.sendEvent = null;
			this.storeResult = null;
		}

		public override void OnUpdate()
		{
			bool buttonDown = Input.GetButtonDown(this.buttonName.Value);
			if (buttonDown)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = buttonDown;
		}
	}
}
