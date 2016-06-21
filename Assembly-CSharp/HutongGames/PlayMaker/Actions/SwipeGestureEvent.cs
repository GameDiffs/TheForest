using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Sends an event when a swipe is detected.")]
	public class SwipeGestureEvent : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("How far a touch has to travel to be considered a swipe. Uses normalized distance (e.g. 1 = 1 screen diagonal distance). Should generally be a very small number.")]
		public FsmFloat minSwipeDistance;

		[HutongGames.PlayMaker.Tooltip("Event to send when swipe left detected.")]
		public FsmEvent swipeLeftEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send when swipe right detected.")]
		public FsmEvent swipeRightEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send when swipe up detected.")]
		public FsmEvent swipeUpEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send when swipe down detected.")]
		public FsmEvent swipeDownEvent;

		private float screenDiagonalSize;

		private float minSwipeDistancePixels;

		private bool touchStarted;

		private Vector2 touchStartPos;

		public override void Reset()
		{
			this.minSwipeDistance = 0.1f;
			this.swipeLeftEvent = null;
			this.swipeRightEvent = null;
			this.swipeUpEvent = null;
			this.swipeDownEvent = null;
		}

		public override void OnEnter()
		{
			this.screenDiagonalSize = Mathf.Sqrt((float)(Screen.width * Screen.width + Screen.height * Screen.height));
			this.minSwipeDistancePixels = this.minSwipeDistance.Value * this.screenDiagonalSize;
		}

		public override void OnUpdate()
		{
			if (Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
				switch (touch.phase)
				{
				case TouchPhase.Began:
					this.touchStarted = true;
					this.touchStartPos = touch.position;
					break;
				case TouchPhase.Ended:
					if (this.touchStarted)
					{
						this.TestForSwipeGesture(touch);
						this.touchStarted = false;
					}
					break;
				case TouchPhase.Canceled:
					this.touchStarted = false;
					break;
				}
			}
		}

		private void TestForSwipeGesture(Touch touch)
		{
			Vector2 position = touch.position;
			float num = Vector2.Distance(position, this.touchStartPos);
			if (num > this.minSwipeDistancePixels)
			{
				float x = position.y - this.touchStartPos.y;
				float y = position.x - this.touchStartPos.x;
				float num2 = 57.29578f * Mathf.Atan2(y, x);
				num2 = (360f + num2 - 45f) % 360f;
				Debug.Log(num2);
				if (num2 < 90f)
				{
					base.Fsm.Event(this.swipeRightEvent);
				}
				else if (num2 < 180f)
				{
					base.Fsm.Event(this.swipeDownEvent);
				}
				else if (num2 < 270f)
				{
					base.Fsm.Event(this.swipeLeftEvent);
				}
				else
				{
					base.Fsm.Event(this.swipeUpEvent);
				}
			}
		}
	}
}
