using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Begin a GUILayout block of GUI controls in a fixed screen area. NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	public class GUILayoutBeginArea : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmRect screenRect;

		public FsmFloat left;

		public FsmFloat top;

		public FsmFloat width;

		public FsmFloat height;

		public FsmBool normalized;

		public FsmString style;

		private Rect rect;

		public override void Reset()
		{
			this.screenRect = null;
			this.left = 0f;
			this.top = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.style = string.Empty;
		}

		public override void OnGUI()
		{
			this.rect = (this.screenRect.IsNone ? default(Rect) : this.screenRect.Value);
			if (!this.left.IsNone)
			{
				this.rect.x = this.left.Value;
			}
			if (!this.top.IsNone)
			{
				this.rect.y = this.top.Value;
			}
			if (!this.width.IsNone)
			{
				this.rect.width = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				this.rect.height = this.height.Value;
			}
			if (this.normalized.Value)
			{
				this.rect.x = this.rect.x * (float)Screen.width;
				this.rect.width = this.rect.width * (float)Screen.width;
				this.rect.y = this.rect.y * (float)Screen.height;
				this.rect.height = this.rect.height * (float)Screen.height;
			}
			GUILayout.BeginArea(this.rect, GUIContent.none, this.style.Value);
		}
	}
}
