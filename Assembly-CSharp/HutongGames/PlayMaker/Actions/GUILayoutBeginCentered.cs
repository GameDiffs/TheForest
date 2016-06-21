using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Begin a centered GUILayout block. The block is centered inside a parent GUILayout Area. So to place the block in the center of the screen, first use a GULayout Area the size of the whole screen (the default setting). NOTE: Block must end with a corresponding GUILayoutEndCentered.")]
	public class GUILayoutBeginCentered : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnGUI()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
		}
	}
}
