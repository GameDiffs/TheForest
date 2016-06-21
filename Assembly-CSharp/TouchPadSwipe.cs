using System;
using UnityEngine;

public class TouchPadSwipe : TouchPad
{
	protected override void ZeroWhenUnused()
	{
		if (this.autoReturnStyle == JoystickAbstract.ReturnStyleOption.Curved)
		{
			this.position.x = Mathf.Lerp(this.position.x, 0f, Time.deltaTime * this.autoReturnSpeed.x);
			this.position.y = Mathf.Lerp(this.position.y, 0f, Time.deltaTime * this.autoReturnSpeed.y);
		}
		else
		{
			this.position.x = Mathf.MoveTowards(this.position.x, 0f, Time.deltaTime * this.autoReturnSpeed.x);
			this.position.y = Mathf.MoveTowards(this.position.y, 0f, Time.deltaTime * this.autoReturnSpeed.y);
		}
	}

	protected override void ForEachTouch(Touch touch, Vector2 guiTouchPos)
	{
		base.ForEachTouch(touch, guiTouchPos);
		if (this.lastFingerId != touch.fingerId)
		{
			return;
		}
		if (touch.phase == TouchPhase.Began)
		{
			this.lastTouchPos = touch.position;
			this.touchDelta = Vector2.zero;
		}
		this.touchDelta = Vector2.Lerp(this.touchDelta, (this.lastTouchPos - touch.position) / this.swipeScale, Time.deltaTime * this.interpolateTime);
		if (touch.deltaTime > 0f)
		{
			if (this.useX)
			{
				float x = this.touchDelta.x * this.sensitivity;
				this.position.x = x;
			}
			if (this.useY)
			{
				float y = this.touchDelta.y * this.sensitivity;
				this.position.y = y;
			}
		}
		this.lastTouchPos = touch.position;
		this.lastTouchPos = touch.position;
		if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
		{
			base.ResetJoystick();
		}
	}
}
