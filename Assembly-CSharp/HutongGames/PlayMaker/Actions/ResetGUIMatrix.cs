using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Resets the GUI matrix. Useful if you've rotated or scaled the GUI and now want to reset it.")]
	public class ResetGUIMatrix : FsmStateAction
	{
		public override void OnGUI()
		{
			Matrix4x4 identity = Matrix4x4.identity;
			GUI.matrix = identity;
			PlayMakerGUI.GUIMatrix = identity;
		}
	}
}
