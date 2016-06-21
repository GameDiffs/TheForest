using System;
using UnityEngine;

public abstract class AbstractButton
{
	protected CrossPlatformInput.VirtualButton m_Button;

	protected Rect m_Rect;

	public void Enable(string name, bool pairwithinputmanager, Rect rect)
	{
		this.m_Button = new CrossPlatformInput.VirtualButton(name, pairwithinputmanager);
		this.m_Rect = rect;
	}

	public void Disable()
	{
		this.m_Button.Remove();
	}

	public abstract void Update();
}
