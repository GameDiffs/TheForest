using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("GUILayout BeginHorizontal.")]
	public class GUILayoutBeginHorizontal : GUILayoutAction
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
			GUILayout.BeginHorizontal(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.style.Value, base.LayoutOptions);
		}
	}
}
