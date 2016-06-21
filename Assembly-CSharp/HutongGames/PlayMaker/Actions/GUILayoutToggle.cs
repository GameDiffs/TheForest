using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Makes an on/off Toggle Button and stores the button state in a Bool Variable.")]
	public class GUILayoutToggle : GUILayoutAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmBool storeButtonState;

		public FsmTexture image;

		public FsmString text;

		public FsmString tooltip;

		public FsmString style;

		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			this.storeButtonState = null;
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = "Toggle";
			this.changedEvent = null;
		}

		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			this.storeButtonState.Value = GUILayout.Toggle(this.storeButtonState.Value, new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				base.Fsm.Event(this.changedEvent);
				GUIUtility.ExitGUI();
			}
			else
			{
				GUI.changed = changed;
			}
		}
	}
}
