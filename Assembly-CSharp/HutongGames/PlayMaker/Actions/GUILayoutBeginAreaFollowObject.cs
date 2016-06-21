using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout), HutongGames.PlayMaker.Tooltip("Begin a GUILayout area that follows the specified game object. Useful for overlays (e.g., playerName). NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	public class GUILayoutBeginAreaFollowObject : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to follow.")]
		public FsmGameObject gameObject;

		[RequiredField]
		public FsmFloat offsetLeft;

		[RequiredField]
		public FsmFloat offsetTop;

		[RequiredField]
		public FsmFloat width;

		[RequiredField]
		public FsmFloat height;

		[HutongGames.PlayMaker.Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		[HutongGames.PlayMaker.Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;

		public override void Reset()
		{
			this.gameObject = null;
			this.offsetLeft = 0f;
			this.offsetTop = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.style = string.Empty;
		}

		public override void OnGUI()
		{
			GameObject value = this.gameObject.Value;
			if (value == null || Camera.main == null)
			{
				GUILayoutBeginAreaFollowObject.DummyBeginArea();
				return;
			}
			Vector3 position = value.transform.position;
			if (Camera.main.transform.InverseTransformPoint(position).z < 0f)
			{
				GUILayoutBeginAreaFollowObject.DummyBeginArea();
				return;
			}
			Vector2 vector = Camera.main.WorldToScreenPoint(position);
			float left = vector.x + ((!this.normalized.Value) ? this.offsetLeft.Value : (this.offsetLeft.Value * (float)Screen.width));
			float top = vector.y + ((!this.normalized.Value) ? this.offsetTop.Value : (this.offsetTop.Value * (float)Screen.width));
			Rect screenRect = new Rect(left, top, this.width.Value, this.height.Value);
			if (this.normalized.Value)
			{
				screenRect.width *= (float)Screen.width;
				screenRect.height *= (float)Screen.height;
			}
			screenRect.y = (float)Screen.height - screenRect.y;
			GUILayout.BeginArea(screenRect, this.style.Value);
		}

		private static void DummyBeginArea()
		{
			GUILayout.BeginArea(default(Rect));
		}
	}
}
