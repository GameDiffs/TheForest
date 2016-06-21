using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Sets the GUISkin used by GUI elements.")]
	public class SetGUISkin : FsmStateAction
	{
		[RequiredField]
		public GUISkin skin;

		public FsmBool applyGlobally;

		public override void Reset()
		{
			this.skin = null;
			this.applyGlobally = true;
		}

		public override void OnGUI()
		{
			if (this.skin != null)
			{
				GUI.skin = this.skin;
			}
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUISkin = this.skin;
				base.Finish();
			}
		}
	}
}
