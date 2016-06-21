using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Label for simple text.")]
	public class GUILayoutTextLabel : GUILayoutAction
	{
		[HutongGames.PlayMaker.Tooltip("Text to display.")]
		public FsmString text;

		[HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISkin.")]
		public FsmString style;

		public override void Reset()
		{
			base.Reset();
			this.text = string.Empty;
			this.style = string.Empty;
		}

		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.text.Value), base.LayoutOptions);
			}
			else
			{
				GUILayout.Label(new GUIContent(this.text.Value), this.style.Value, base.LayoutOptions);
			}
		}
	}
}
