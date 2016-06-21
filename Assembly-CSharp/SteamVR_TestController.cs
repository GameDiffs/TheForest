using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SteamVR_TestController : MonoBehaviour
{
	private List<int> controllerIndices = new List<int>();

	private EVRButtonId[] buttonIds = new EVRButtonId[]
	{
		EVRButtonId.k_EButton_ApplicationMenu,
		EVRButtonId.k_EButton_Grip,
		EVRButtonId.k_EButton_Axis0,
		EVRButtonId.k_EButton_Axis1
	};

	private EVRButtonId[] axisIds = new EVRButtonId[]
	{
		EVRButtonId.k_EButton_Axis0,
		EVRButtonId.k_EButton_Axis1
	};

	public Transform point;

	public Transform pointer;

	private void OnDeviceConnected(params object[] args)
	{
		int num = (int)args[0];
		CVRSystem system = OpenVR.System;
		if (system == null || system.GetTrackedDeviceClass((uint)num) != ETrackedDeviceClass.Controller)
		{
			return;
		}
		bool flag = (bool)args[1];
		if (flag)
		{
			Debug.Log(string.Format("Controller {0} connected.", num));
			this.PrintControllerStatus(num);
			this.controllerIndices.Add(num);
		}
		else
		{
			Debug.Log(string.Format("Controller {0} disconnected.", num));
			this.PrintControllerStatus(num);
			this.controllerIndices.Remove(num);
		}
	}

	private void OnEnable()
	{
		SteamVR_Utils.Event.Listen("device_connected", new SteamVR_Utils.Event.Handler(this.OnDeviceConnected));
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("device_connected", new SteamVR_Utils.Event.Handler(this.OnDeviceConnected));
	}

	private void PrintControllerStatus(int index)
	{
		SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
		Debug.Log("index: " + device.index);
		Debug.Log("connected: " + device.connected);
		Debug.Log("hasTracking: " + device.hasTracking);
		Debug.Log("outOfRange: " + device.outOfRange);
		Debug.Log("calibrating: " + device.calibrating);
		Debug.Log("uninitialized: " + device.uninitialized);
		Debug.Log("pos: " + device.transform.pos);
		Debug.Log("rot: " + device.transform.rot.eulerAngles);
		Debug.Log("velocity: " + device.velocity);
		Debug.Log("angularVelocity: " + device.angularVelocity);
		int deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost, ETrackedDeviceClass.Controller, 0);
		int deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost, ETrackedDeviceClass.Controller, 0);
		Debug.Log((deviceIndex != deviceIndex2) ? ((deviceIndex != index) ? "right" : "left") : "first");
	}

	private void Update()
	{
		foreach (int current in this.controllerIndices)
		{
			SteamVR_Overlay instance = SteamVR_Overlay.instance;
			if (instance && this.point && this.pointer)
			{
				SteamVR_Utils.RigidTransform transform = SteamVR_Controller.Input(current).transform;
				this.pointer.transform.localPosition = transform.pos;
				this.pointer.transform.localRotation = transform.rot;
				SteamVR_Overlay.IntersectionResults intersectionResults = default(SteamVR_Overlay.IntersectionResults);
				bool flag = instance.ComputeIntersection(transform.pos, transform.rot * Vector3.forward, ref intersectionResults);
				if (flag)
				{
					this.point.transform.localPosition = intersectionResults.point;
					this.point.transform.localRotation = Quaternion.LookRotation(intersectionResults.normal);
				}
			}
			else
			{
				EVRButtonId[] array = this.buttonIds;
				for (int i = 0; i < array.Length; i++)
				{
					EVRButtonId eVRButtonId = array[i];
					if (SteamVR_Controller.Input(current).GetPressDown(eVRButtonId))
					{
						Debug.Log(eVRButtonId + " press down");
					}
					if (SteamVR_Controller.Input(current).GetPressUp(eVRButtonId))
					{
						Debug.Log(eVRButtonId + " press up");
						if (eVRButtonId == EVRButtonId.k_EButton_Axis1)
						{
							SteamVR_Controller.Input(current).TriggerHapticPulse(500, EVRButtonId.k_EButton_Axis0);
							this.PrintControllerStatus(current);
						}
					}
					if (SteamVR_Controller.Input(current).GetPress(eVRButtonId))
					{
						Debug.Log(eVRButtonId);
					}
				}
				EVRButtonId[] array2 = this.axisIds;
				for (int j = 0; j < array2.Length; j++)
				{
					EVRButtonId eVRButtonId2 = array2[j];
					if (SteamVR_Controller.Input(current).GetTouchDown(eVRButtonId2))
					{
						Debug.Log(eVRButtonId2 + " touch down");
					}
					if (SteamVR_Controller.Input(current).GetTouchUp(eVRButtonId2))
					{
						Debug.Log(eVRButtonId2 + " touch up");
					}
					if (SteamVR_Controller.Input(current).GetTouch(eVRButtonId2))
					{
						Vector2 axis = SteamVR_Controller.Input(current).GetAxis(eVRButtonId2);
						Debug.Log("axis: " + axis);
					}
				}
			}
		}
	}
}
