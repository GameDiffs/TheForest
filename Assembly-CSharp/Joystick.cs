using System;
using UnityEngine;

public class Joystick : JoystickAbstract
{
	protected override void TypeSpecificOnEnable()
	{
		this.guiTouchOffset.x = this.defaultRect.width * 0.5f;
		this.guiTouchOffset.y = this.defaultRect.height * 0.5f;
		this.guiCenter.x = this.defaultRect.x + this.guiTouchOffset.x;
		this.guiCenter.y = this.defaultRect.y + this.guiTouchOffset.y;
		this.guiBoundary.xMin = this.defaultRect.x - this.guiTouchOffset.x;
		this.guiBoundary.xMax = this.defaultRect.x + this.guiTouchOffset.x;
		this.guiBoundary.yMin = this.defaultRect.y - this.guiTouchOffset.y;
		this.guiBoundary.yMax = this.defaultRect.y + this.guiTouchOffset.y;
		this.moveStick = true;
	}

	protected override void ZeroWhenUnused()
	{
		if (this.lastFingerId == -1)
		{
			Rect pixelInset = this.gui.pixelInset;
			if (this.autoReturnStyle == JoystickAbstract.ReturnStyleOption.Curved)
			{
				pixelInset.x = Mathf.Lerp(pixelInset.x, this.defaultRect.x, Time.deltaTime * this.autoReturnSpeed.x * this.guiTouchOffset.x);
				pixelInset.y = Mathf.Lerp(pixelInset.y, this.defaultRect.y, Time.deltaTime * this.autoReturnSpeed.y * this.guiTouchOffset.y);
			}
			else
			{
				pixelInset.x = Mathf.MoveTowards(pixelInset.x, this.defaultRect.x, Time.deltaTime * this.autoReturnSpeed.x * this.guiTouchOffset.x);
				pixelInset.y = Mathf.MoveTowards(pixelInset.y, this.defaultRect.y, Time.deltaTime * this.autoReturnSpeed.y * this.guiTouchOffset.y);
			}
			this.gui.pixelInset = pixelInset;
		}
	}

	protected override void ForEachTouch(Touch touch, Vector2 guiTouchPos)
	{
		bool flag = this.gui.HitTest(touch.position);
		if (flag && (this.lastFingerId == -1 || this.lastFingerId != touch.fingerId))
		{
			this.lastFingerId = touch.fingerId;
			for (int i = 0; i < JoystickAbstract.joysticks.Length; i++)
			{
				if (JoystickAbstract.joysticks[i] != this)
				{
					JoystickAbstract.joysticks[i].LatchedFinger(touch.fingerId);
				}
			}
		}
		if (this.lastFingerId == touch.fingerId)
		{
			this.gui.pixelInset = new Rect(Mathf.Clamp(guiTouchPos.x, this.guiBoundary.xMin, this.guiBoundary.xMax), Mathf.Clamp(guiTouchPos.y, this.guiBoundary.yMin, this.guiBoundary.yMax), this.gui.pixelInset.width, this.gui.pixelInset.height);
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				base.ResetJoystick();
			}
		}
	}

	protected override void MoveJoystickGraphic()
	{
		if (this.useX)
		{
			this.position.x = (this.gui.pixelInset.x + this.guiTouchOffset.x - this.guiCenter.x) / this.guiTouchOffset.x;
		}
		if (this.useY)
		{
			this.position.y = (this.gui.pixelInset.y + this.guiTouchOffset.y - this.guiCenter.y) / this.guiTouchOffset.y;
		}
	}
}
