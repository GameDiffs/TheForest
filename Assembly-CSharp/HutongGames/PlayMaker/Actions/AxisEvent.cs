using System;
using TheForest.Utils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Sends events based on the direction of Input Axis (Left/Right/Up/Down...).")]
	public class AxisEvent : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Horizontal axis as defined in the Input Manager")]
		public FsmString horizontalAxis;

		[HutongGames.PlayMaker.Tooltip("Vertical axis as defined in the Input Manager")]
		public FsmString verticalAxis;

		[HutongGames.PlayMaker.Tooltip("Event to send if input is to the left.")]
		public FsmEvent leftEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if input is to the right.")]
		public FsmEvent rightEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if input is to the up.")]
		public FsmEvent upEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if input is to the down.")]
		public FsmEvent downEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if input is in any direction.")]
		public FsmEvent anyDirection;

		[HutongGames.PlayMaker.Tooltip("Event to send if no axis input (centered).")]
		public FsmEvent noDirection;

		public override void Reset()
		{
			this.horizontalAxis = "Horizontal";
			this.verticalAxis = "Vertical";
			this.leftEvent = null;
			this.rightEvent = null;
			this.upEvent = null;
			this.downEvent = null;
			this.anyDirection = null;
			this.noDirection = null;
		}

		public override void OnUpdate()
		{
			float num = (!(this.horizontalAxis.Value != string.Empty)) ? 0f : TheForest.Utils.Input.GetAxis(this.horizontalAxis.Value);
			float num2 = (!(this.verticalAxis.Value != string.Empty)) ? 0f : TheForest.Utils.Input.GetAxis(this.verticalAxis.Value);
			if ((num * num + num2 * num2).Equals(0f))
			{
				if (this.noDirection != null)
				{
					base.Fsm.Event(this.noDirection);
				}
				return;
			}
			float num3 = Mathf.Atan2(num2, num) * 57.29578f + 45f;
			if (num3 < 0f)
			{
				num3 += 360f;
			}
			int num4 = (int)(num3 / 90f);
			if (num4 == 0 && this.rightEvent != null)
			{
				base.Fsm.Event(this.rightEvent);
			}
			else if (num4 == 1 && this.upEvent != null)
			{
				base.Fsm.Event(this.upEvent);
			}
			else if (num4 == 2 && this.leftEvent != null)
			{
				base.Fsm.Event(this.leftEvent);
			}
			else if (num4 == 3 && this.downEvent != null)
			{
				base.Fsm.Event(this.downEvent);
			}
			else if (this.anyDirection != null)
			{
				base.Fsm.Event(this.anyDirection);
			}
		}
	}
}
