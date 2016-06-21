using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIColor : FsmStateAction
	{
		[RequiredField]
		public FsmColor color;

		public FsmBool applyGlobally;

		public override void Reset()
		{
			this.color = Color.white;
		}

		public override void OnGUI()
		{
			GUI.color = this.color.Value;
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUIColor = GUI.color;
			}
		}
	}
}
