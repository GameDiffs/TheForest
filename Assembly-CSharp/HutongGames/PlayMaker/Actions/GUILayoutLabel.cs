using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout Label.")]
	public class GUILayoutLabel : GUILayoutAction
	{
		public FsmTexture image;

		public FsmString text;

		public FsmString tooltip;

		public FsmString style;

		public override void Reset()
		{
			base.Reset();
			this.text = string.Empty;
			this.image = null;
			this.tooltip = string.Empty;
			this.style = string.Empty;
		}

		public override void OnGUI()
		{
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUILayout.Label(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), base.LayoutOptions);
			}
			else
			{
				GUILayout.Label(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
			}
		}
	}
}
