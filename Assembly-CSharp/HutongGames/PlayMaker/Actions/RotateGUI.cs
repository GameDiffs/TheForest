using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Rotates the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class RotateGUI : FsmStateAction
	{
		[RequiredField]
		public FsmFloat angle;

		[RequiredField]
		public FsmFloat pivotX;

		[RequiredField]
		public FsmFloat pivotY;

		public bool normalized;

		public bool applyGlobally;

		private bool applied;

		public override void Reset()
		{
			this.angle = 0f;
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
			Vector2 pivotPoint = new Vector2(this.pivotX.Value, this.pivotY.Value);
			if (this.normalized)
			{
				pivotPoint.x *= (float)Screen.width;
				pivotPoint.y *= (float)Screen.height;
			}
			GUIUtility.RotateAroundPivot(this.angle.Value, pivotPoint);
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
