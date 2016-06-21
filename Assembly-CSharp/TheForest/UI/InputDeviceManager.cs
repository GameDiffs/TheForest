using Rewired;
using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class InputDeviceManager : MonoBehaviour
	{
		public InputDeviceRow _row;

		public UITable _table;

		public UIToggle _allDevicesCheckbox;

		private void OnEnable()
		{
			this._allDevicesCheckbox.value = (PlayerPrefs.GetInt("alldevices", 1) == 1);
			this.ResetUI();
			ReInput.ControllerConnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerConnectedEvent);
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnControllerConnectedEvent);
			ReInput.ControllerDisconnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerConnectedEvent);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnControllerConnectedEvent);
		}

		private void OnDisable()
		{
			ReInput.ControllerConnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerConnectedEvent);
			ReInput.ControllerDisconnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnControllerConnectedEvent);
		}

		private void OnControllerConnectedEvent(ControllerStatusChangedEventArgs obj)
		{
			this.ResetUI();
		}

		public void OnToggleAllDevices()
		{
			PlayerPrefs.SetInt("alldevices", (!this._allDevicesCheckbox.value) ? 0 : 1);
			PlayerPrefs.Save();
			InputMapping.InitControllers();
			this.ResetUI();
		}

		public void ResetUI()
		{
			this.ClearUI();
			Rewired.Joystick[] joysticks = ReInput.controllers.GetJoysticks();
			foreach (Controller current in ReInput.controllers.Controllers)
			{
				this.AddDeviceRow(current);
			}
			this._table.repositionNow = true;
		}

		private void ClearUI()
		{
			int i = this._table.transform.childCount;
			while (i > 0)
			{
				UnityEngine.Object.Destroy(this._table.transform.GetChild(--i).gameObject);
			}
		}

		private void AddDeviceRow(Controller controller)
		{
			Rewired.Joystick joystick = controller as Rewired.Joystick;
			InputDeviceRow inputDeviceRow = UnityEngine.Object.Instantiate<InputDeviceRow>(this._row);
			inputDeviceRow.transform.parent = this._table.transform;
			inputDeviceRow.transform.localPosition = Vector3.zero;
			inputDeviceRow.transform.localScale = Vector3.one;
			inputDeviceRow._label.text = ((joystick == null) ? controller.name : joystick.name);
			if (!this._allDevicesCheckbox.value && controller.type == ControllerType.Joystick)
			{
				inputDeviceRow._checkbox.value = InputDeviceManager.UseDevice(controller.hardwareIdentifier);
				EventDelegate eventDelegate = new EventDelegate();
				eventDelegate.target = this;
				eventDelegate.methodName = "ToggleDevice";
				eventDelegate.parameters[0] = new EventDelegate.Parameter(inputDeviceRow._checkbox);
				eventDelegate.parameters[1] = new EventDelegate.Parameter(controller.id);
				eventDelegate.parameters[2] = new EventDelegate.Parameter(controller.hardwareIdentifier);
				inputDeviceRow._checkbox.onChange.Add(eventDelegate);
			}
			else
			{
				UnityEngine.Object.Destroy(inputDeviceRow._checkbox.gameObject);
			}
		}

		public void ToggleDevice(UIToggle checkbox, int id, string hardwareId)
		{
			if (checkbox.value != InputDeviceManager.UseDevice(hardwareId))
			{
				PlayerPrefs.SetInt("device_" + hardwareId, (!checkbox.value) ? 0 : 1);
				PlayerPrefs.Save();
				InputMapping.InitControllers();
			}
		}

		public static bool UseDevice(string hardwareId)
		{
			return PlayerPrefs.GetInt("alldevices", 1) == 1 || PlayerPrefs.GetInt("device_" + hardwareId, 1) == 1;
		}
	}
}
