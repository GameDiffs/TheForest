using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Valve.VR;

public class SteamVR_TrackedController : MonoBehaviour
{
	public uint controllerIndex;

	public VRControllerState_t controllerState;

	public bool triggerPressed;

	public bool steamPressed;

	public bool menuPressed;

	public bool padPressed;

	public bool padTouched;

	public bool gripped;

	public event ClickedEventHandler MenuButtonClicked
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.MenuButtonClicked = (ClickedEventHandler)Delegate.Combine(this.MenuButtonClicked, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.MenuButtonClicked = (ClickedEventHandler)Delegate.Remove(this.MenuButtonClicked, value);
		}
	}

	public event ClickedEventHandler MenuButtonUnclicked
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.MenuButtonUnclicked = (ClickedEventHandler)Delegate.Combine(this.MenuButtonUnclicked, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.MenuButtonUnclicked = (ClickedEventHandler)Delegate.Remove(this.MenuButtonUnclicked, value);
		}
	}

	public event ClickedEventHandler TriggerClicked
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.TriggerClicked = (ClickedEventHandler)Delegate.Combine(this.TriggerClicked, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.TriggerClicked = (ClickedEventHandler)Delegate.Remove(this.TriggerClicked, value);
		}
	}

	public event ClickedEventHandler TriggerUnclicked
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.TriggerUnclicked = (ClickedEventHandler)Delegate.Combine(this.TriggerUnclicked, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.TriggerUnclicked = (ClickedEventHandler)Delegate.Remove(this.TriggerUnclicked, value);
		}
	}

	public event ClickedEventHandler SteamClicked
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.SteamClicked = (ClickedEventHandler)Delegate.Combine(this.SteamClicked, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.SteamClicked = (ClickedEventHandler)Delegate.Remove(this.SteamClicked, value);
		}
	}

	public event ClickedEventHandler PadClicked
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.PadClicked = (ClickedEventHandler)Delegate.Combine(this.PadClicked, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.PadClicked = (ClickedEventHandler)Delegate.Remove(this.PadClicked, value);
		}
	}

	public event ClickedEventHandler PadUnclicked
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.PadUnclicked = (ClickedEventHandler)Delegate.Combine(this.PadUnclicked, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.PadUnclicked = (ClickedEventHandler)Delegate.Remove(this.PadUnclicked, value);
		}
	}

	public event ClickedEventHandler PadTouched
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.PadTouched = (ClickedEventHandler)Delegate.Combine(this.PadTouched, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.PadTouched = (ClickedEventHandler)Delegate.Remove(this.PadTouched, value);
		}
	}

	public event ClickedEventHandler PadUntouched
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.PadUntouched = (ClickedEventHandler)Delegate.Combine(this.PadUntouched, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.PadUntouched = (ClickedEventHandler)Delegate.Remove(this.PadUntouched, value);
		}
	}

	public event ClickedEventHandler Gripped
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Gripped = (ClickedEventHandler)Delegate.Combine(this.Gripped, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Gripped = (ClickedEventHandler)Delegate.Remove(this.Gripped, value);
		}
	}

	public event ClickedEventHandler Ungripped
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.Ungripped = (ClickedEventHandler)Delegate.Combine(this.Ungripped, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.Ungripped = (ClickedEventHandler)Delegate.Remove(this.Ungripped, value);
		}
	}

	private void Start()
	{
		if (base.GetComponent<SteamVR_TrackedObject>() == null)
		{
			base.gameObject.AddComponent<SteamVR_TrackedObject>();
		}
		base.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)this.controllerIndex;
		if (base.GetComponent<SteamVR_RenderModel>() != null)
		{
			base.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)this.controllerIndex;
		}
	}

	public virtual void OnTriggerClicked(ClickedEventArgs e)
	{
		if (this.TriggerClicked != null)
		{
			this.TriggerClicked(this, e);
		}
	}

	public virtual void OnTriggerUnclicked(ClickedEventArgs e)
	{
		if (this.TriggerUnclicked != null)
		{
			this.TriggerUnclicked(this, e);
		}
	}

	public virtual void OnMenuClicked(ClickedEventArgs e)
	{
		if (this.MenuButtonClicked != null)
		{
			this.MenuButtonClicked(this, e);
		}
	}

	public virtual void OnMenuUnclicked(ClickedEventArgs e)
	{
		if (this.MenuButtonUnclicked != null)
		{
			this.MenuButtonUnclicked(this, e);
		}
	}

	public virtual void OnSteamClicked(ClickedEventArgs e)
	{
		if (this.SteamClicked != null)
		{
			this.SteamClicked(this, e);
		}
	}

	public virtual void OnPadClicked(ClickedEventArgs e)
	{
		if (this.PadClicked != null)
		{
			this.PadClicked(this, e);
		}
	}

	public virtual void OnPadUnclicked(ClickedEventArgs e)
	{
		if (this.PadUnclicked != null)
		{
			this.PadUnclicked(this, e);
		}
	}

	public virtual void OnPadTouched(ClickedEventArgs e)
	{
		if (this.PadTouched != null)
		{
			this.PadTouched(this, e);
		}
	}

	public virtual void OnPadUntouched(ClickedEventArgs e)
	{
		if (this.PadUntouched != null)
		{
			this.PadUntouched(this, e);
		}
	}

	public virtual void OnGripped(ClickedEventArgs e)
	{
		if (this.Gripped != null)
		{
			this.Gripped(this, e);
		}
	}

	public virtual void OnUngripped(ClickedEventArgs e)
	{
		if (this.Ungripped != null)
		{
			this.Ungripped(this, e);
		}
	}

	private void Update()
	{
		CVRSystem system = OpenVR.System;
		if (system != null && system.GetControllerState(this.controllerIndex, ref this.controllerState))
		{
			ulong num = this.controllerState.ulButtonPressed & 8589934592uL;
			if (num > 0uL && !this.triggerPressed)
			{
				this.triggerPressed = true;
				ClickedEventArgs e;
				e.controllerIndex = this.controllerIndex;
				e.flags = (uint)this.controllerState.ulButtonPressed;
				e.padX = this.controllerState.rAxis0.x;
				e.padY = this.controllerState.rAxis0.y;
				this.OnTriggerClicked(e);
			}
			else if (num == 0uL && this.triggerPressed)
			{
				this.triggerPressed = false;
				ClickedEventArgs e2;
				e2.controllerIndex = this.controllerIndex;
				e2.flags = (uint)this.controllerState.ulButtonPressed;
				e2.padX = this.controllerState.rAxis0.x;
				e2.padY = this.controllerState.rAxis0.y;
				this.OnTriggerUnclicked(e2);
			}
			ulong num2 = this.controllerState.ulButtonPressed & 4uL;
			if (num2 > 0uL && !this.gripped)
			{
				this.gripped = true;
				ClickedEventArgs e3;
				e3.controllerIndex = this.controllerIndex;
				e3.flags = (uint)this.controllerState.ulButtonPressed;
				e3.padX = this.controllerState.rAxis0.x;
				e3.padY = this.controllerState.rAxis0.y;
				this.OnGripped(e3);
			}
			else if (num2 == 0uL && this.gripped)
			{
				this.gripped = false;
				ClickedEventArgs e4;
				e4.controllerIndex = this.controllerIndex;
				e4.flags = (uint)this.controllerState.ulButtonPressed;
				e4.padX = this.controllerState.rAxis0.x;
				e4.padY = this.controllerState.rAxis0.y;
				this.OnUngripped(e4);
			}
			ulong num3 = this.controllerState.ulButtonPressed & 4294967296uL;
			if (num3 > 0uL && !this.padPressed)
			{
				this.padPressed = true;
				ClickedEventArgs e5;
				e5.controllerIndex = this.controllerIndex;
				e5.flags = (uint)this.controllerState.ulButtonPressed;
				e5.padX = this.controllerState.rAxis0.x;
				e5.padY = this.controllerState.rAxis0.y;
				this.OnPadClicked(e5);
			}
			else if (num3 == 0uL && this.padPressed)
			{
				this.padPressed = false;
				ClickedEventArgs e6;
				e6.controllerIndex = this.controllerIndex;
				e6.flags = (uint)this.controllerState.ulButtonPressed;
				e6.padX = this.controllerState.rAxis0.x;
				e6.padY = this.controllerState.rAxis0.y;
				this.OnPadUnclicked(e6);
			}
			ulong num4 = this.controllerState.ulButtonPressed & 2uL;
			if (num4 > 0uL && !this.menuPressed)
			{
				this.menuPressed = true;
				ClickedEventArgs e7;
				e7.controllerIndex = this.controllerIndex;
				e7.flags = (uint)this.controllerState.ulButtonPressed;
				e7.padX = this.controllerState.rAxis0.x;
				e7.padY = this.controllerState.rAxis0.y;
				this.OnMenuClicked(e7);
			}
			else if (num4 == 0uL && this.menuPressed)
			{
				this.menuPressed = false;
				ClickedEventArgs e8;
				e8.controllerIndex = this.controllerIndex;
				e8.flags = (uint)this.controllerState.ulButtonPressed;
				e8.padX = this.controllerState.rAxis0.x;
				e8.padY = this.controllerState.rAxis0.y;
				this.OnMenuUnclicked(e8);
			}
			num3 = (this.controllerState.ulButtonTouched & 4294967296uL);
			if (num3 > 0uL && !this.padTouched)
			{
				this.padTouched = true;
				ClickedEventArgs e9;
				e9.controllerIndex = this.controllerIndex;
				e9.flags = (uint)this.controllerState.ulButtonPressed;
				e9.padX = this.controllerState.rAxis0.x;
				e9.padY = this.controllerState.rAxis0.y;
				this.OnPadTouched(e9);
			}
			else if (num3 == 0uL && this.padTouched)
			{
				this.padTouched = false;
				ClickedEventArgs e10;
				e10.controllerIndex = this.controllerIndex;
				e10.flags = (uint)this.controllerState.ulButtonPressed;
				e10.padX = this.controllerState.rAxis0.x;
				e10.padY = this.controllerState.rAxis0.y;
				this.OnPadUntouched(e10);
			}
		}
	}
}
