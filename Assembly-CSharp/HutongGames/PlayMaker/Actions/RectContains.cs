using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Rect), HutongGames.PlayMaker.Tooltip("Tests if a point is inside a rectangle.")]
	public class RectContains : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Rectangle to test.")]
		public FsmRect rectangle;

		[HutongGames.PlayMaker.Tooltip("Point to test.")]
		public FsmVector3 point;

		[HutongGames.PlayMaker.Tooltip("Specify/override X value.")]
		public FsmFloat x;

		[HutongGames.PlayMaker.Tooltip("Specify/override Y value.")]
		public FsmFloat y;

		[HutongGames.PlayMaker.Tooltip("Event to send if the Point is inside the Rectangle.")]
		public FsmEvent trueEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if the Point is outside the Rectangle.")]
		public FsmEvent falseEvent;

		[HutongGames.PlayMaker.Tooltip("Store the result in a variable."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.rectangle = new FsmRect
			{
				UseVariable = true
			};
			this.point = new FsmVector3
			{
				UseVariable = true
			};
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.storeResult = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoRectContains();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoRectContains();
		}

		private void DoRectContains()
		{
			if (this.rectangle.IsNone)
			{
				return;
			}
			Vector3 value = this.point.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			bool flag = this.rectangle.Value.Contains(value);
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.falseEvent : this.trueEvent);
		}
	}
}
