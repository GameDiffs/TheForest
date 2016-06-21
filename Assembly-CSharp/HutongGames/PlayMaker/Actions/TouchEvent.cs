using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Sends events based on Touch Phases. Optionally filter by a fingerID.")]
	public class TouchEvent : FsmStateAction
	{
		public FsmInt fingerId;

		public TouchPhase touchPhase;

		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmInt storeFingerId;

		public override void Reset()
		{
			this.fingerId = new FsmInt
			{
				UseVariable = true
			};
			this.storeFingerId = null;
		}

		public override void OnUpdate()
		{
			if (Input.touchCount > 0)
			{
				Touch[] touches = Input.touches;
				for (int i = 0; i < touches.Length; i++)
				{
					Touch touch = touches[i];
					if ((this.fingerId.IsNone || touch.fingerId == this.fingerId.Value) && touch.phase == this.touchPhase)
					{
						this.storeFingerId.Value = touch.fingerId;
						base.Fsm.Event(this.sendEvent);
					}
				}
			}
		}
	}
}
