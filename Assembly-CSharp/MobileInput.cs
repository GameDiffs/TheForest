using System;
using UnityEngine;

public class MobileInput : VirtualInput
{
	public override float GetAxis(string name, bool raw)
	{
		return (!this.virtualAxes.ContainsKey(name)) ? 0f : this.virtualAxes[name].GetValue;
	}

	public override bool GetButton(string name, CrossPlatformInput.ButtonAction action)
	{
		if (!this.virtualButtons.ContainsKey(name))
		{
			throw new Exception(" Button " + name + " does not exist");
		}
		switch (action)
		{
		case CrossPlatformInput.ButtonAction.GetButtonDown:
			return this.virtualButtons[name].GetButtonDown;
		case CrossPlatformInput.ButtonAction.GetButtonUp:
			return this.virtualButtons[name].GetButtonUp;
		case CrossPlatformInput.ButtonAction.GetButton:
			return this.virtualButtons[name].GetButton;
		default:
			throw new Exception("Invalid button action.");
		}
	}

	public override Vector3 MousePosition()
	{
		return base.virtualMousePosition;
	}
}
