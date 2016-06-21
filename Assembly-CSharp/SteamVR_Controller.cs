using System;
using UnityEngine;
using Valve.VR;

public class SteamVR_Controller
{
	public class ButtonMask
	{
		public const ulong System = 1uL;

		public const ulong ApplicationMenu = 2uL;

		public const ulong Grip = 4uL;

		public const ulong Axis0 = 4294967296uL;

		public const ulong Axis1 = 8589934592uL;

		public const ulong Axis2 = 17179869184uL;

		public const ulong Axis3 = 34359738368uL;

		public const ulong Axis4 = 68719476736uL;

		public const ulong Touchpad = 4294967296uL;

		public const ulong Trigger = 8589934592uL;
	}

	public class Device
	{
		private VRControllerState_t state;

		private VRControllerState_t prevState;

		private TrackedDevicePose_t pose;

		private int prevFrameCount = -1;

		public float hairTriggerDelta = 0.1f;

		private float hairTriggerLimit;

		private bool hairTriggerState;

		private bool hairTriggerPrevState;

		public uint index
		{
			get;
			private set;
		}

		public bool valid
		{
			get;
			private set;
		}

		public bool connected
		{
			get
			{
				this.Update();
				return this.pose.bDeviceIsConnected;
			}
		}

		public bool hasTracking
		{
			get
			{
				this.Update();
				return this.pose.bPoseIsValid;
			}
		}

		public bool outOfRange
		{
			get
			{
				this.Update();
				return this.pose.eTrackingResult == ETrackingResult.Running_OutOfRange || this.pose.eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			}
		}

		public bool calibrating
		{
			get
			{
				this.Update();
				return this.pose.eTrackingResult == ETrackingResult.Calibrating_InProgress || this.pose.eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			}
		}

		public bool uninitialized
		{
			get
			{
				this.Update();
				return this.pose.eTrackingResult == ETrackingResult.Uninitialized;
			}
		}

		public SteamVR_Utils.RigidTransform transform
		{
			get
			{
				this.Update();
				return new SteamVR_Utils.RigidTransform(this.pose.mDeviceToAbsoluteTracking);
			}
		}

		public Vector3 velocity
		{
			get
			{
				this.Update();
				return new Vector3(this.pose.vVelocity.v0, this.pose.vVelocity.v1, -this.pose.vVelocity.v2);
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				this.Update();
				return new Vector3(-this.pose.vAngularVelocity.v0, -this.pose.vAngularVelocity.v1, this.pose.vAngularVelocity.v2);
			}
		}

		public Device(uint i)
		{
			this.index = i;
		}

		public VRControllerState_t GetState()
		{
			this.Update();
			return this.state;
		}

		public VRControllerState_t GetPrevState()
		{
			this.Update();
			return this.prevState;
		}

		public TrackedDevicePose_t GetPose()
		{
			this.Update();
			return this.pose;
		}

		public void Update()
		{
			if (Time.frameCount != this.prevFrameCount)
			{
				this.prevFrameCount = Time.frameCount;
				this.prevState = this.state;
				CVRSystem system = OpenVR.System;
				if (system != null)
				{
					this.valid = system.GetControllerStateWithPose(SteamVR_Render.instance.trackingSpace, this.index, ref this.state, ref this.pose);
					this.UpdateHairTrigger();
				}
			}
		}

		public bool GetPress(ulong buttonMask)
		{
			this.Update();
			return (this.state.ulButtonPressed & buttonMask) != 0uL;
		}

		public bool GetPressDown(ulong buttonMask)
		{
			this.Update();
			return (this.state.ulButtonPressed & buttonMask) != 0uL && (this.prevState.ulButtonPressed & buttonMask) == 0uL;
		}

		public bool GetPressUp(ulong buttonMask)
		{
			this.Update();
			return (this.state.ulButtonPressed & buttonMask) == 0uL && (this.prevState.ulButtonPressed & buttonMask) != 0uL;
		}

		public bool GetPress(EVRButtonId buttonId)
		{
			return this.GetPress(1uL << (int)buttonId);
		}

		public bool GetPressDown(EVRButtonId buttonId)
		{
			return this.GetPressDown(1uL << (int)buttonId);
		}

		public bool GetPressUp(EVRButtonId buttonId)
		{
			return this.GetPressUp(1uL << (int)buttonId);
		}

		public bool GetTouch(ulong buttonMask)
		{
			this.Update();
			return (this.state.ulButtonTouched & buttonMask) != 0uL;
		}

		public bool GetTouchDown(ulong buttonMask)
		{
			this.Update();
			return (this.state.ulButtonTouched & buttonMask) != 0uL && (this.prevState.ulButtonTouched & buttonMask) == 0uL;
		}

		public bool GetTouchUp(ulong buttonMask)
		{
			this.Update();
			return (this.state.ulButtonTouched & buttonMask) == 0uL && (this.prevState.ulButtonTouched & buttonMask) != 0uL;
		}

		public bool GetTouch(EVRButtonId buttonId)
		{
			return this.GetTouch(1uL << (int)buttonId);
		}

		public bool GetTouchDown(EVRButtonId buttonId)
		{
			return this.GetTouchDown(1uL << (int)buttonId);
		}

		public bool GetTouchUp(EVRButtonId buttonId)
		{
			return this.GetTouchUp(1uL << (int)buttonId);
		}

		public Vector2 GetAxis(EVRButtonId buttonId = EVRButtonId.k_EButton_Axis0)
		{
			this.Update();
			switch (buttonId)
			{
			case EVRButtonId.k_EButton_Axis0:
				return new Vector2(this.state.rAxis0.x, this.state.rAxis0.y);
			case EVRButtonId.k_EButton_Axis1:
				return new Vector2(this.state.rAxis1.x, this.state.rAxis1.y);
			case EVRButtonId.k_EButton_Axis2:
				return new Vector2(this.state.rAxis2.x, this.state.rAxis2.y);
			case EVRButtonId.k_EButton_Axis3:
				return new Vector2(this.state.rAxis3.x, this.state.rAxis3.y);
			case EVRButtonId.k_EButton_Axis4:
				return new Vector2(this.state.rAxis4.x, this.state.rAxis4.y);
			default:
				return Vector2.zero;
			}
		}

		public void TriggerHapticPulse(ushort durationMicroSec = 500, EVRButtonId buttonId = EVRButtonId.k_EButton_Axis0)
		{
			CVRSystem system = OpenVR.System;
			if (system != null)
			{
				uint unAxisId = (uint)(buttonId - EVRButtonId.k_EButton_Axis0);
				system.TriggerHapticPulse(this.index, unAxisId, (char)durationMicroSec);
			}
		}

		private void UpdateHairTrigger()
		{
			this.hairTriggerPrevState = this.hairTriggerState;
			float x = this.state.rAxis1.x;
			if (this.hairTriggerState)
			{
				if (x < this.hairTriggerLimit - this.hairTriggerDelta || x <= 0f)
				{
					this.hairTriggerState = false;
				}
			}
			else if (x > this.hairTriggerLimit + this.hairTriggerDelta || x >= 1f)
			{
				this.hairTriggerState = true;
			}
			this.hairTriggerLimit = ((!this.hairTriggerState) ? Mathf.Min(this.hairTriggerLimit, x) : Mathf.Max(this.hairTriggerLimit, x));
		}

		public bool GetHairTrigger()
		{
			this.Update();
			return this.hairTriggerState;
		}

		public bool GetHairTriggerDown()
		{
			this.Update();
			return this.hairTriggerState && !this.hairTriggerPrevState;
		}

		public bool GetHairTriggerUp()
		{
			this.Update();
			return !this.hairTriggerState && this.hairTriggerPrevState;
		}
	}

	public enum DeviceRelation
	{
		First,
		Leftmost,
		Rightmost,
		FarthestLeft,
		FarthestRight
	}

	private static SteamVR_Controller.Device[] devices;

	public static SteamVR_Controller.Device Input(int deviceIndex)
	{
		if (SteamVR_Controller.devices == null)
		{
			SteamVR_Controller.devices = new SteamVR_Controller.Device[16];
			uint num = 0u;
			while ((ulong)num < (ulong)((long)SteamVR_Controller.devices.Length))
			{
				SteamVR_Controller.devices[(int)((UIntPtr)num)] = new SteamVR_Controller.Device(num);
				num += 1u;
			}
		}
		return SteamVR_Controller.devices[deviceIndex];
	}

	public static void Update()
	{
		int num = 0;
		while ((long)num < 16L)
		{
			SteamVR_Controller.Input(num).Update();
			num++;
		}
	}

	public static int GetDeviceIndex(SteamVR_Controller.DeviceRelation relation, ETrackedDeviceClass deviceClass = ETrackedDeviceClass.Controller, int relativeTo = 0)
	{
		int result = -1;
		SteamVR_Utils.RigidTransform t = (relativeTo >= 16) ? SteamVR_Utils.RigidTransform.identity : SteamVR_Controller.Input(relativeTo).transform.GetInverse();
		CVRSystem system = OpenVR.System;
		if (system == null)
		{
			return result;
		}
		float num = -3.40282347E+38f;
		int num2 = 0;
		while ((long)num2 < 16L)
		{
			if (num2 != relativeTo && system.GetTrackedDeviceClass((uint)num2) == deviceClass)
			{
				SteamVR_Controller.Device device = SteamVR_Controller.Input(num2);
				if (device.connected)
				{
					if (relation == SteamVR_Controller.DeviceRelation.First)
					{
						return num2;
					}
					Vector3 vector = t * device.transform.pos;
					float num3;
					if (relation == SteamVR_Controller.DeviceRelation.FarthestRight)
					{
						num3 = vector.x;
					}
					else if (relation == SteamVR_Controller.DeviceRelation.FarthestLeft)
					{
						num3 = -vector.x;
					}
					else
					{
						Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
						Vector3 normalized = vector2.normalized;
						float num4 = Vector3.Dot(normalized, Vector3.forward);
						Vector3 vector3 = Vector3.Cross(normalized, Vector3.forward);
						if (relation == SteamVR_Controller.DeviceRelation.Leftmost)
						{
							num3 = ((vector3.y <= 0f) ? num4 : (2f - num4));
						}
						else
						{
							num3 = ((vector3.y >= 0f) ? num4 : (2f - num4));
						}
					}
					if (num3 > num)
					{
						result = num2;
						num = num3;
					}
				}
			}
			num2++;
		}
		return result;
	}
}
