using System;
using UnityEngine;

public class StandaloneInput : VirtualInput
{
	public override float GetAxis(string name, bool raw)
	{
		return (!this.alwaysUseVirtual.Contains(name)) ? Input.GetAxis(name) : this.virtualAxes[name].GetValue;
	}

	public override bool GetButton(string name, CrossPlatformInput.ButtonAction action)
	{
		switch (action)
		{
		case CrossPlatformInput.ButtonAction.GetButtonDown:
			return (!this.alwaysUseVirtual.Contains(name)) ? Input.GetButtonDown(name) : this.virtualButtons[name].GetButtonDown;
		case CrossPlatformInput.ButtonAction.GetButtonUp:
			return (!this.alwaysUseVirtual.Contains(name)) ? Input.GetButtonUp(name) : this.virtualButtons[name].GetButtonUp;
		case CrossPlatformInput.ButtonAction.GetButton:
			return (!this.alwaysUseVirtual.Contains(name)) ? Input.GetButton(name) : this.virtualButtons[name].GetButton;
		default:
			throw new Exception("Invalid button action.");
		}
	}

	public override Vector3 MousePosition()
	{
		return Input.mousePosition;
	}
}
