using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for all text rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIContentColor : FsmStateAction
	{
		[RequiredField]
		public FsmColor contentColor;

		public FsmBool applyGlobally;

		public override void Reset()
		{
			this.contentColor = Color.white;
		}

		public override void OnGUI()
		{
			GUI.contentColor = this.contentColor.Value;
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIContentColor = GUI.contentColor;
			}
		}
	}
}
