using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Scales the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class ScaleGUI : FsmStateAction
	{
		[RequiredField]
		public FsmFloat scaleX;

		[RequiredField]
		public FsmFloat scaleY;

		[RequiredField]
		public FsmFloat pivotX;

		[RequiredField]
		public FsmFloat pivotY;

		[HutongGames.PlayMaker.Tooltip("Pivot point uses normalized coordinates. E.g. 0.5 is the center of the screen.")]
		public bool normalized;

		public bool applyGlobally;

		private bool applied;

		public override void Reset()
		{
			this.scaleX = 1f;
			this.scaleY = 1f;
			this.pivotX = 0.5f;
			this.pivotY = 0.5f;
			this.normalized = true;
			this.applyGlobally = false;
		}

		public override void OnGUI()
		{
			if (this.applied)
			{
				return;
			}
			Vector2 scale = new Vector2(this.scaleX.Value, this.scaleY.Value);
			if (object.Equals(scale.x, 0))
			{
				scale.x = 0.0001f;
			}
			if (object.Equals(scale.y, 0))
			{
				scale.x = 0.0001f;
			}
			Vector2 pivotPoint = new Vector2(this.pivotX.Value, this.pivotY.Value);
			if (this.normalized)
			{
				pivotPoint.x *= (float)Screen.width;
				pivotPoint.y *= (float)Screen.height;
			}
			GUIUtility.ScaleAroundPivot(scale, pivotPoint);
			if (this.applyGlobally)
			{
				PlayMakerGUI.GUIMatrix = GUI.matrix;
				this.applied = true;
			}
		}

		public override void OnUpdate()
		{
			this.applied = false;
		}
	}
}
