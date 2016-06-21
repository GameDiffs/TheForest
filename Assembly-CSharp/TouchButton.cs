using System;
using UnityEngine;

internal class TouchButton : AbstractButton
{
	private bool m_Pressed;

	public override void Update()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (this.m_Rect.Contains(touch.position))
			{
				if (this.m_Pressed)
				{
					return;
				}
				if (touch.phase == TouchPhase.Began)
				{
					this.m_Button.Pressed();
					this.m_Pressed = true;
					return;
				}
			}
		}
		if (this.m_Pressed)
		{
			this.m_Button.Released();
			this.m_Pressed = false;
		}
	}
}
