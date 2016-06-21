using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("GUI button. Sends an Event when pressed. Optionally store the button state in a Bool Variable.")]
	public class GUIButton : GUIContentAction
	{
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeButtonState;

		public override void Reset()
		{
			base.Reset();
			this.sendEvent = null;
			this.storeButtonState = null;
			this.style = "Button";
		}

		public override void OnGUI()
		{
			base.OnGUI();
			bool value = false;
			if (GUI.Button(this.rect, this.content, this.style.Value))
			{
				base.Fsm.Event(this.sendEvent);
				value = true;
			}
			if (this.storeButtonState != null)
			{
				this.storeButtonState.Value = value;
			}
		}
	}
}
