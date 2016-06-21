using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Inserts a space in the current layout group.")]
	public class GUILayoutSpace : FsmStateAction
	{
		public FsmFloat space;

		public override void Reset()
		{
			this.space = 10f;
		}

		public override void OnGUI()
		{
			GUILayout.Space(this.space.Value);
		}
	}
}
