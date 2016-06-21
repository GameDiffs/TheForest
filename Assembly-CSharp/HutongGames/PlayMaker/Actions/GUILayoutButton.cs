using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
	public class GUILayoutButton : GUILayoutAction
	{
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeButtonState;

		public FsmTexture image;

		public FsmString text;

		public FsmString tooltip;

		public FsmString style;

		public override void Reset()
		{
			base.Reset();
			this.sendEvent = null;
			this.storeButtonState = null;
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = string.Empty;
		}

		public override void OnGUI()
		{
			bool flag;
			if (string.IsNullOrEmpty(this.style.Value))
			{
				flag = GUILayout.Button(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
			}
			else
			{
				flag = GUILayout.Button(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			if (this.storeButtonState != null)
			{
				this.storeButtonState.Value = flag;
			}
		}
	}
}
