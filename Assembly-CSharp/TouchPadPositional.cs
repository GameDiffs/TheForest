using System;
using UnityEngine;

public class TouchPadPositional : TouchPad
{
	protected override void ForEachTouch(Touch touch, Vector2 guiTouchPos)
	{
		base.ForEachTouch(touch, guiTouchPos);
		if (this.lastFingerId == touch.fingerId)
		{
			Vector2 a = new Vector2((touch.position.x - this.touchZoneRect.center.x) / this.sensitivityRelativeX, (touch.position.y - this.touchZoneRect.center.y) / this.sensitivityRelativeY) * 2f;
			Vector2 vector = Vector2.Lerp(this.position, a * this.sensitivity, Time.deltaTime * this.interpolateTime);
			if (this.useX)
			{
				this.position.x = Mathf.Clamp(vector.x, -1f, 1f);
			}
			if (this.useY)
			{
				this.position.y = Mathf.Clamp(vector.y, -1f, 1f);
			}
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				base.ResetJoystick();
			}
		}
	}
}
