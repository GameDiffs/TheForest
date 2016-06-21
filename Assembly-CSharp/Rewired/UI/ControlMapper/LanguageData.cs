using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	public class LanguageData : ScriptableObject
	{
		[Serializable]
		private class CustomEntry
		{
			public string key;

			public string value;

			public CustomEntry()
			{
			}

			public CustomEntry(string key, string value)
			{
				this.key = key;
				this.value = value;
			}

			public static Dictionary<string, string> ToDictionary(LanguageData.CustomEntry[] array)
			{
				if (array == null)
				{
					return new Dictionary<string, string>();
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						if (!string.IsNullOrEmpty(array[i].key) && !string.IsNullOrEmpty(array[i].value))
						{
							if (dictionary.ContainsKey(array[i].key))
							{
								Debug.LogError("Key \"" + array[i].key + "\" is already in dictionary!");
							}
							else
							{
								dictionary.Add(array[i].key, array[i].value);
							}
						}
					}
				}
				return dictionary;
			}
		}

		[SerializeField]
		private string _yes = "Yes";

		[SerializeField]
		private string _no = "No";

		[SerializeField]
		private string _add = "Add";

		[SerializeField]
		private string _replace = "Replace";

		[SerializeField]
		private string _remove = "Remove";

		[SerializeField]
		private string _cancel = "Cancel";

		[SerializeField]
		private string _none = "None";

		[SerializeField]
		private string _okay = "Okay";

		[SerializeField]
		private string _done = "Done";

		[SerializeField]
		private string _default = "Default";

		[SerializeField]
		private string _assignControllerWindowTitle = "Choose Controller";

		[SerializeField]
		private string _assignControllerWindowMessage = "Press any button or move an axis on the controller you would like to use.";

		[SerializeField]
		private string _controllerAssignmentConflictWindowTitle = "Controller Assignment";

		[SerializeField, Tooltip("{0} = Joystick Name\n{1} = Other Player Name\n{2} = This Player Name")]
		private string _controllerAssignmentConflictWindowMessage = "{0} is already assigned to {1}. Do you want to assign this controller to {2} instead?";

		[SerializeField]
		private string _elementAssignmentPrePollingWindowMessage = "First center or zero all sticks and axes and press any button or wait for the timer to finish.";

		[SerializeField, Tooltip("{0} = Action Name")]
		private string _joystickElementAssignmentPollingWindowMessage = "Now press a button or move an axis to assign it to {0}.";

		[SerializeField, Tooltip("{0} = Action Name")]
		private string _keyboardElementAssignmentPollingWindowMessage = "Press a key to assign it to {0}. Modifier keys may also be used. To assign a modifier key alone, hold it down for 1 second.";

		[SerializeField, Tooltip("{0} = Action Name")]
		private string _mouseElementAssignmentPollingWindowMessage = "Press a mouse button or move an axis to assign it to {0}.";

		[SerializeField]
		private string _elementAssignmentConflictWindowMessage = "Assignment Conflict";

		[SerializeField, Tooltip("{0} = Element Name")]
		private string _elementAlreadyInUseBlocked = "{0} is already in use cannot be replaced.";

		[SerializeField, Tooltip("{0} = Element Name")]
		private string _elementAlreadyInUseCanReplace = "{0} is already in use. Do you want to replace it?";

		[SerializeField, Tooltip("{0} = Element Name")]
		private string _elementAlreadyInUseCanReplace_conflictAllowed = "{0} is already in use. Do you want to replace it? You may also choose to add the assignment anyway.";

		[SerializeField]
		private string _mouseAssignmentConflictWindowTitle = "Mouse Assignment";

		[SerializeField, Tooltip("{0} = Other Player Name\n{1} = This Player Name")]
		private string _mouseAssignmentConflictWindowMessage = "The mouse is already assigned to {0}. Do you want to assign the mouse to {1} instead?";

		[SerializeField]
		private string _calibrateControllerWindowTitle = "Calibrate Controller";

		[SerializeField]
		private string _calibrateAxisStep1WindowTitle = "Calibrate Zero";

		[SerializeField, Tooltip("{0} = Axis Name")]
		private string _calibrateAxisStep1WindowMessage = "Center or zero {0} and press any button or wait for the timer to finish.";

		[SerializeField]
		private string _calibrateAxisStep2WindowTitle = "Calibrate Range";

		[SerializeField, Tooltip("{0} = Axis Name")]
		private string _calibrateAxisStep2WindowMessage = "Move {0} through its entire range then press any button or wait for the timer to finish.";

		[SerializeField]
		private string _inputBehaviorSettingsWindowTitle = "Sensitivity Settings";

		[SerializeField]
		private string _restoreDefaultsWindowTitle = "Restore Defaults";

		[SerializeField, Tooltip("Message for a single player game.")]
		private string _restoreDefaultsWindowMessage_onePlayer = "This will restore the default input configuration. Are you sure you want to do this?";

		[SerializeField, Tooltip("Message for a multi-player game.")]
		private string _restoreDefaultsWindowMessage_multiPlayer = "This will restore the default input configuration for all players. Are you sure you want to do this?";

		[SerializeField]
		private string _actionColumnLabel = "Actions";

		[SerializeField]
		private string _keyboardColumnLabel = "Keyboard";

		[SerializeField]
		private string _mouseColumnLabel = "Mouse";

		[SerializeField]
		private string _controllerColumnLabel = "Controller";

		[SerializeField]
		private string _removeControllerButtonLabel = "Remove";

		[SerializeField]
		private string _calibrateControllerButtonLabel = "Calibrate";

		[SerializeField]
		private string _assignControllerButtonLabel = "Assign Controller";

		[SerializeField]
		private string _inputBehaviorSettingsButtonLabel = "Sensitivity";

		[SerializeField]
		private string _doneButtonLabel = "Done";

		[SerializeField]
		private string _restoreDefaultsButtonLabel = "Restore Defaults";

		[SerializeField]
		private string _playersGroupLabel = "Players:";

		[SerializeField]
		private string _controllerSettingsGroupLabel = "Controller:";

		[SerializeField]
		private string _assignedControllersGroupLabel = "Assigned Controllers:";

		[SerializeField]
		private string _settingsGroupLabel = "Settings:";

		[SerializeField]
		private string _mapCategoriesGroupLabel = "Categories:";

		[SerializeField]
		private string _calibrateWindow_deadZoneSliderLabel = "Dead Zone:";

		[SerializeField]
		private string _calibrateWindow_zeroSliderLabel = "Zero:";

		[SerializeField]
		private string _calibrateWindow_sensitivitySliderLabel = "Sensitivity:";

		[SerializeField]
		private string _calibrateWindow_invertToggleLabel = "Invert";

		[SerializeField]
		private string _calibrateWindow_calibrateButtonLabel = "Calibrate";

		[SerializeField]
		private LanguageData.CustomEntry[] _customEntries;

		private bool _initialized;

		private Dictionary<string, string> customDict;

		public string yes
		{
			get
			{
				return this._yes;
			}
		}

		public string no
		{
			get
			{
				return this._no;
			}
		}

		public string add
		{
			get
			{
				return this._add;
			}
		}

		public string replace
		{
			get
			{
				return this._replace;
			}
		}

		public string remove
		{
			get
			{
				return this._remove;
			}
		}

		public string cancel
		{
			get
			{
				return this._cancel;
			}
		}

		public string none
		{
			get
			{
				return this._none;
			}
		}

		public string okay
		{
			get
			{
				return this._okay;
			}
		}

		public string done
		{
			get
			{
				return this._done;
			}
		}

		public string default_
		{
			get
			{
				return this._default;
			}
		}

		public string assignControllerWindowTitle
		{
			get
			{
				return this._assignControllerWindowTitle;
			}
		}

		public string assignControllerWindowMessage
		{
			get
			{
				return this._assignControllerWindowMessage;
			}
		}

		public string controllerAssignmentConflictWindowTitle
		{
			get
			{
				return this._controllerAssignmentConflictWindowTitle;
			}
		}

		public string elementAssignmentPrePollingWindowMessage
		{
			get
			{
				return this._elementAssignmentPrePollingWindowMessage;
			}
		}

		public string elementAssignmentConflictWindowMessage
		{
			get
			{
				return this._elementAssignmentConflictWindowMessage;
			}
		}

		public string mouseAssignmentConflictWindowTitle
		{
			get
			{
				return this._mouseAssignmentConflictWindowTitle;
			}
		}

		public string calibrateControllerWindowTitle
		{
			get
			{
				return this._calibrateControllerWindowTitle;
			}
		}

		public string calibrateAxisStep1WindowTitle
		{
			get
			{
				return this._calibrateAxisStep1WindowTitle;
			}
		}

		public string calibrateAxisStep2WindowTitle
		{
			get
			{
				return this._calibrateAxisStep2WindowTitle;
			}
		}

		public string inputBehaviorSettingsWindowTitle
		{
			get
			{
				return this._inputBehaviorSettingsWindowTitle;
			}
		}

		public string restoreDefaultsWindowTitle
		{
			get
			{
				return this._restoreDefaultsWindowTitle;
			}
		}

		public string actionColumnLabel
		{
			get
			{
				return this._actionColumnLabel;
			}
		}

		public string keyboardColumnLabel
		{
			get
			{
				return this._keyboardColumnLabel;
			}
		}

		public string mouseColumnLabel
		{
			get
			{
				return this._mouseColumnLabel;
			}
		}

		public string controllerColumnLabel
		{
			get
			{
				return this._controllerColumnLabel;
			}
		}

		public string removeControllerButtonLabel
		{
			get
			{
				return this._removeControllerButtonLabel;
			}
		}

		public string calibrateControllerButtonLabel
		{
			get
			{
				return this._calibrateControllerButtonLabel;
			}
		}

		public string assignControllerButtonLabel
		{
			get
			{
				return this._assignControllerButtonLabel;
			}
		}

		public string inputBehaviorSettingsButtonLabel
		{
			get
			{
				return this._inputBehaviorSettingsButtonLabel;
			}
		}

		public string doneButtonLabel
		{
			get
			{
				return this._doneButtonLabel;
			}
		}

		public string restoreDefaultsButtonLabel
		{
			get
			{
				return this._restoreDefaultsButtonLabel;
			}
		}

		public string controllerSettingsGroupLabel
		{
			get
			{
				return this._controllerSettingsGroupLabel;
			}
		}

		public string playersGroupLabel
		{
			get
			{
				return this._playersGroupLabel;
			}
		}

		public string assignedControllersGroupLabel
		{
			get
			{
				return this._assignedControllersGroupLabel;
			}
		}

		public string settingsGroupLabel
		{
			get
			{
				return this._settingsGroupLabel;
			}
		}

		public string mapCategoriesGroupLabel
		{
			get
			{
				return this._mapCategoriesGroupLabel;
			}
		}

		public string restoreDefaultsWindowMessage
		{
			get
			{
				if (ReInput.players.playerCount > 1)
				{
					return this._restoreDefaultsWindowMessage_multiPlayer;
				}
				return this._restoreDefaultsWindowMessage_onePlayer;
			}
		}

		public string calibrateWindow_deadZoneSliderLabel
		{
			get
			{
				return this._calibrateWindow_deadZoneSliderLabel;
			}
		}

		public string calibrateWindow_zeroSliderLabel
		{
			get
			{
				return this._calibrateWindow_zeroSliderLabel;
			}
		}

		public string calibrateWindow_sensitivitySliderLabel
		{
			get
			{
				return this._calibrateWindow_sensitivitySliderLabel;
			}
		}

		public string calibrateWindow_invertToggleLabel
		{
			get
			{
				return this._calibrateWindow_invertToggleLabel;
			}
		}

		public string calibrateWindow_calibrateButtonLabel
		{
			get
			{
				return this._calibrateWindow_calibrateButtonLabel;
			}
		}

		public void Initialize()
		{
			if (!this._initialized)
			{
				return;
			}
			this.customDict = LanguageData.CustomEntry.ToDictionary(this._customEntries);
			this._initialized = true;
		}

		public string GetCustomEntry(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return string.Empty;
			}
			string result;
			if (!this.customDict.TryGetValue(key, out result))
			{
				return string.Empty;
			}
			return result;
		}

		public bool ContainsCustomEntryKey(string key)
		{
			return !string.IsNullOrEmpty(key) && this.customDict.ContainsKey(key);
		}

		public string GetControllerAssignmentConflictWindowMessage(string joystickName, string otherPlayerName, string currentPlayerName)
		{
			return string.Format(this._controllerAssignmentConflictWindowMessage, joystickName, otherPlayerName, currentPlayerName);
		}

		public string GetJoystickElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(this._joystickElementAssignmentPollingWindowMessage, actionName);
		}

		public string GetKeyboardElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(this._keyboardElementAssignmentPollingWindowMessage, actionName);
		}

		public string GetMouseElementAssignmentPollingWindowMessage(string actionName)
		{
			return string.Format(this._mouseElementAssignmentPollingWindowMessage, actionName);
		}

		public string GetElementAlreadyInUseBlocked(string elementName)
		{
			return string.Format(this._elementAlreadyInUseBlocked, elementName);
		}

		public string GetElementAlreadyInUseCanReplace(string elementName, bool allowConflicts)
		{
			if (!allowConflicts)
			{
				return string.Format(this._elementAlreadyInUseCanReplace, elementName);
			}
			return string.Format(this._elementAlreadyInUseCanReplace_conflictAllowed, elementName);
		}

		public string GetMouseAssignmentConflictWindowMessage(string otherPlayerName, string thisPlayerName)
		{
			return string.Format(this._mouseAssignmentConflictWindowMessage, otherPlayerName, thisPlayerName);
		}

		public string GetCalibrateAxisStep1WindowMessage(string axisName)
		{
			return string.Format(this._calibrateAxisStep1WindowMessage, axisName);
		}

		public string GetCalibrateAxisStep2WindowMessage(string axisName)
		{
			return string.Format(this._calibrateAxisStep2WindowMessage, axisName);
		}
	}
}
