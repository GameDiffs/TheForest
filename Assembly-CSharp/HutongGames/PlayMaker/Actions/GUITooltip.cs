using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Gets the Tooltip of the control the mouse is currently over and store it in a String Variable.")]
	public class GUITooltip : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmString storeTooltip;

		public override void Reset()
		{
			this.storeTooltip = null;
		}

		public override void OnGUI()
		{
			this.storeTooltip.Value = GUI.tooltip;
		}
	}
}
