using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class VirtualInput
{
	protected Dictionary<string, CrossPlatformInput.VirtualAxis> virtualAxes = new Dictionary<string, CrossPlatformInput.VirtualAxis>();

	protected Dictionary<string, CrossPlatformInput.VirtualButton> virtualButtons = new Dictionary<string, CrossPlatformInput.VirtualButton>();

	protected List<string> alwaysUseVirtual = new List<string>();

	public Vector3 virtualMousePosition
	{
		get;
		private set;
	}

	public void RegisterVirtualAxis(CrossPlatformInput.VirtualAxis axis)
	{
		if (this.virtualAxes.ContainsKey(axis.name))
		{
			Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
		}
		else
		{
			this.virtualAxes.Add(axis.name, axis);
			if (!axis.matchWithInputManager)
			{
				this.alwaysUseVirtual.Add(axis.name);
			}
		}
	}

	public void RegisterVirtualButton(CrossPlatformInput.VirtualButton button)
	{
		if (this.virtualButtons.ContainsKey(button.name))
		{
			Debug.LogError("There is already a virtual button named " + button.name + " registered.");
		}
		else
		{
			this.virtualButtons.Add(button.name, button);
			if (!button.matchWithInputManager)
			{
				this.alwaysUseVirtual.Add(button.name);
			}
		}
	}

	public void UnRegisterVirtualAxis(string name)
	{
		if (this.virtualAxes.ContainsKey(name))
		{
			this.virtualAxes.Remove(name);
		}
	}

	public void UnRegisterVirtualButton(string name)
	{
		if (this.virtualButtons.ContainsKey(name))
		{
			this.virtualButtons.Remove(name);
		}
	}

	public CrossPlatformInput.VirtualAxis VirtualAxisReference(string name)
	{
		return (!this.virtualAxes.ContainsKey(name)) ? null : this.virtualAxes[name];
	}

	public void SetVirtualMousePositionX(float f)
	{
		this.virtualMousePosition = new Vector3(f, this.virtualMousePosition.y, this.virtualMousePosition.z);
	}

	public void SetVirtualMousePositionY(float f)
	{
		this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, f, this.virtualMousePosition.z);
	}

	public void SetVirtualMousePositionZ(float f)
	{
		this.virtualMousePosition = new Vector3(this.virtualMousePosition.x, this.virtualMousePosition.y, f);
	}

	public abstract float GetAxis(string name, bool raw);

	public abstract bool GetButton(string name, CrossPlatformInput.ButtonAction action);

	public abstract Vector3 MousePosition();
}
