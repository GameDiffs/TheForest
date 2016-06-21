using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Close a GUILayout group started with BeginArea.")]
	public class GUILayoutEndArea : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnGUI()
		{
			GUILayout.EndArea();
		}
	}
}
