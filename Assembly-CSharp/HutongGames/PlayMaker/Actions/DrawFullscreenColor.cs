using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Fills the screen with a Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
	public class DrawFullscreenColor : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
		public FsmColor color;

		public override void Reset()
		{
			this.color = Color.white;
		}

		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.color.Value;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}
	}
}
