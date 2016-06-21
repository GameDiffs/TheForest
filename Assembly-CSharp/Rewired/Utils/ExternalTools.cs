using Rewired.Utils.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rewired.Utils
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ExternalTools : IExternalTools
	{
		public event Action<uint, bool> XboxOneInput_OnGamepadStateChange
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.XboxOneInput_OnGamepadStateChange = (Action<uint, bool>)Delegate.Combine(this.XboxOneInput_OnGamepadStateChange, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.XboxOneInput_OnGamepadStateChange = (Action<uint, bool>)Delegate.Remove(this.XboxOneInput_OnGamepadStateChange, value);
			}
		}

		public bool LinuxInput_IsJoystickPreconfigured(string name)
		{
			return false;
		}

		public int XboxOneInput_GetUserIdForGamepad(uint id)
		{
			return 0;
		}

		public ulong XboxOneInput_GetControllerId(uint unityJoystickId)
		{
			return 0uL;
		}

		public bool XboxOneInput_IsGamepadActive(uint unityJoystickId)
		{
			return false;
		}

		public string XboxOneInput_GetControllerType(ulong xboxControllerId)
		{
			return string.Empty;
		}

		public uint XboxOneInput_GetJoystickId(ulong xboxControllerId)
		{
			return 0u;
		}

		public void XboxOne_Gamepad_UpdatePlugin()
		{
		}

		public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel)
		{
			return false;
		}

		public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS)
		{
		}
	}
}
