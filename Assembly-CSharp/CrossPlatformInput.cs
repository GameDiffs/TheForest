using System;
using UnityEngine;

public static class CrossPlatformInput
{
	public enum ButtonAction
	{
		GetButtonDown,
		GetButtonUp,
		GetButton
	}

	public class VirtualAxis
	{
		private float m_Value;

		public string name
		{
			get;
			private set;
		}

		public bool matchWithInputManager
		{
			get;
			private set;
		}

		public float GetValue
		{
			get
			{
				return this.m_Value;
			}
		}

		public float GetValueRaw
		{
			get
			{
				return this.m_Value;
			}
		}

		public VirtualAxis(string name) : this(name, true)
		{
		}

		public VirtualAxis(string name, bool matchToInputSettings)
		{
			this.name = name;
			this.matchWithInputManager = matchToInputSettings;
			CrossPlatformInput.RegisterVirtualAxis(this);
		}

		public void Remove()
		{
			CrossPlatformInput.UnRegisterVirtualAxis(this.name);
		}

		public void Update(float value)
		{
			this.m_Value = value;
		}
	}

	public class VirtualButton
	{
		private int lastPressedFrame = -5;

		private int releasedFrame = -5;

		private bool pressed;

		public string name
		{
			get;
			private set;
		}

		public bool matchWithInputManager
		{
			get;
			private set;
		}

		public bool GetButton
		{
			get
			{
				return this.pressed;
			}
		}

		public bool GetButtonDown
		{
			get
			{
				return this.lastPressedFrame - Time.frameCount == 0;
			}
		}

		public bool GetButtonUp
		{
			get
			{
				return this.releasedFrame == Time.frameCount - 1;
			}
		}

		public VirtualButton(string name) : this(name, true)
		{
		}

		public VirtualButton(string name, bool matchToInputSettings)
		{
			this.name = name;
			this.matchWithInputManager = matchToInputSettings;
			CrossPlatformInput.RegisterVirtualButton(this);
		}

		public void Pressed()
		{
			if (!this.pressed)
			{
				this.pressed = true;
				this.lastPressedFrame = Time.frameCount;
			}
		}

		public void Released()
		{
			this.pressed = false;
			this.releasedFrame = Time.frameCount;
		}

		public void Remove()
		{
			CrossPlatformInput.UnRegisterVirtualButton(this.name);
		}
	}

	private static VirtualInput virtualInput;

	public static Vector3 mousePosition
	{
		get
		{
			return CrossPlatformInput.virtualInput.MousePosition();
		}
	}

	static CrossPlatformInput()
	{
		CrossPlatformInput.virtualInput = new StandaloneInput();
	}

	private static void RegisterVirtualAxis(CrossPlatformInput.VirtualAxis axis)
	{
		CrossPlatformInput.virtualInput.RegisterVirtualAxis(axis);
	}

	private static void RegisterVirtualButton(CrossPlatformInput.VirtualButton button)
	{
		CrossPlatformInput.virtualInput.RegisterVirtualButton(button);
	}

	private static void UnRegisterVirtualAxis(string name)
	{
		CrossPlatformInput.virtualInput.UnRegisterVirtualAxis(name);
	}

	private static void UnRegisterVirtualButton(string name)
	{
		CrossPlatformInput.virtualInput.UnRegisterVirtualButton(name);
	}

	public static CrossPlatformInput.VirtualAxis VirtualAxisReference(string name)
	{
		return CrossPlatformInput.virtualInput.VirtualAxisReference(name);
	}

	public static float GetAxis(string name)
	{
		return CrossPlatformInput.GetAxis(name, false);
	}

	public static float GetAxisRaw(string name)
	{
		return CrossPlatformInput.GetAxis(name, true);
	}

	private static float GetAxis(string name, bool raw)
	{
		return CrossPlatformInput.virtualInput.GetAxis(name, raw);
	}

	public static bool GetButton(string name)
	{
		return CrossPlatformInput.GetButton(name, CrossPlatformInput.ButtonAction.GetButton);
	}

	public static bool GetButtonDown(string name)
	{
		return CrossPlatformInput.GetButton(name, CrossPlatformInput.ButtonAction.GetButtonDown);
	}

	public static bool GetButtonUp(string name)
	{
		return CrossPlatformInput.GetButton(name, CrossPlatformInput.ButtonAction.GetButtonUp);
	}

	private static bool GetButton(string name, CrossPlatformInput.ButtonAction action)
	{
		return CrossPlatformInput.virtualInput.GetButton(name, action);
	}

	public static void SetVirtualMousePositionX(float f)
	{
		CrossPlatformInput.virtualInput.SetVirtualMousePositionX(f);
	}

	public static void SetVirtualMousePositionY(float f)
	{
		CrossPlatformInput.virtualInput.SetVirtualMousePositionY(f);
	}

	public static void SetVirtualMousePositionZ(float f)
	{
		CrossPlatformInput.virtualInput.SetVirtualMousePositionZ(f);
	}
}
