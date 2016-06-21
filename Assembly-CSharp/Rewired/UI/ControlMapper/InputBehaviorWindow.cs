using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class InputBehaviorWindow : Window
	{
		private class InputBehaviorInfo
		{
			private InputBehavior _inputBehavior;

			private UIControlSet _controlSet;

			private Dictionary<int, InputBehaviorWindow.PropertyType> idToProperty;

			private InputBehavior copyOfOriginal;

			public InputBehavior inputBehavior
			{
				get
				{
					return this._inputBehavior;
				}
			}

			public UIControlSet controlSet
			{
				get
				{
					return this._controlSet;
				}
			}

			public InputBehaviorInfo(InputBehavior inputBehavior, UIControlSet controlSet, Dictionary<int, InputBehaviorWindow.PropertyType> idToProperty)
			{
				this._inputBehavior = inputBehavior;
				this._controlSet = controlSet;
				this.idToProperty = idToProperty;
				this.copyOfOriginal = new InputBehavior(inputBehavior);
			}

			public void RestorePreviousData()
			{
				this._inputBehavior.ImportData(this.copyOfOriginal);
			}

			public void RestoreDefaultData()
			{
				this._inputBehavior.Reset();
				this.RefreshControls();
			}

			public void RestoreData(InputBehaviorWindow.PropertyType propertyType, int controlId)
			{
				if (propertyType != InputBehaviorWindow.PropertyType.JoystickAxisSensitivity)
				{
					if (propertyType == InputBehaviorWindow.PropertyType.MouseXYAxisSensitivity)
					{
						float mouseXYAxisSensitivity = this.copyOfOriginal.mouseXYAxisSensitivity;
						this._inputBehavior.mouseXYAxisSensitivity = mouseXYAxisSensitivity;
						UISliderControl control = this._controlSet.GetControl<UISliderControl>(controlId);
						if (control != null)
						{
							control.slider.value = mouseXYAxisSensitivity;
						}
					}
				}
				else
				{
					float joystickAxisSensitivity = this.copyOfOriginal.joystickAxisSensitivity;
					this._inputBehavior.joystickAxisSensitivity = joystickAxisSensitivity;
					UISliderControl control2 = this._controlSet.GetControl<UISliderControl>(controlId);
					if (control2 != null)
					{
						control2.slider.value = joystickAxisSensitivity;
					}
				}
			}

			public void RefreshControls()
			{
				if (this._controlSet == null)
				{
					return;
				}
				if (this.idToProperty == null)
				{
					return;
				}
				foreach (KeyValuePair<int, InputBehaviorWindow.PropertyType> current in this.idToProperty)
				{
					UISliderControl control = this._controlSet.GetControl<UISliderControl>(current.Key);
					if (!(control == null))
					{
						InputBehaviorWindow.PropertyType value = current.Value;
						if (value != InputBehaviorWindow.PropertyType.JoystickAxisSensitivity)
						{
							if (value == InputBehaviorWindow.PropertyType.MouseXYAxisSensitivity)
							{
								control.slider.value = this._inputBehavior.mouseXYAxisSensitivity;
							}
						}
						else
						{
							control.slider.value = this._inputBehavior.joystickAxisSensitivity;
						}
					}
				}
			}
		}

		public enum ButtonIdentifier
		{
			Done,
			Cancel,
			Default
		}

		private enum PropertyType
		{
			JoystickAxisSensitivity,
			MouseXYAxisSensitivity
		}

		private const float minSensitivity = 0.1f;

		[SerializeField]
		private RectTransform spawnTransform;

		[SerializeField]
		private UnityEngine.UI.Button doneButton;

		[SerializeField]
		private UnityEngine.UI.Button cancelButton;

		[SerializeField]
		private UnityEngine.UI.Button defaultButton;

		[SerializeField]
		private Text doneButtonLabel;

		[SerializeField]
		private Text cancelButtonLabel;

		[SerializeField]
		private Text defaultButtonLabel;

		[SerializeField]
		private GameObject uiControlSetPrefab;

		[SerializeField]
		private GameObject uiSliderControlPrefab;

		private List<InputBehaviorWindow.InputBehaviorInfo> inputBehaviorInfo;

		private Dictionary<int, Action<int>> buttonCallbacks;

		private int playerId;

		public override void Initialize(int id, Func<int, bool> isFocusedCallback)
		{
			if (this.spawnTransform == null || this.doneButton == null || this.cancelButton == null || this.defaultButton == null || this.uiControlSetPrefab == null || this.uiSliderControlPrefab == null || this.doneButtonLabel == null || this.cancelButtonLabel == null || this.defaultButtonLabel == null)
			{
				Debug.LogError("Rewired Control Mapper: All inspector values must be assigned!");
				return;
			}
			this.inputBehaviorInfo = new List<InputBehaviorWindow.InputBehaviorInfo>();
			this.buttonCallbacks = new Dictionary<int, Action<int>>();
			this.doneButtonLabel.text = ControlMapper.GetLanguage().done;
			this.cancelButtonLabel.text = ControlMapper.GetLanguage().cancel;
			this.defaultButtonLabel.text = ControlMapper.GetLanguage().default_;
			base.Initialize(id, isFocusedCallback);
		}

		public void SetData(int playerId, ControlMapper.InputBehaviorSettings[] data)
		{
			if (!base.initialized)
			{
				return;
			}
			this.playerId = playerId;
			for (int i = 0; i < data.Length; i++)
			{
				ControlMapper.InputBehaviorSettings inputBehaviorSettings = data[i];
				if (inputBehaviorSettings != null && inputBehaviorSettings.isValid)
				{
					InputBehavior inputBehavior = this.GetInputBehavior(inputBehaviorSettings.inputBehaviorId);
					if (inputBehavior != null)
					{
						UIControlSet uIControlSet = this.CreateControlSet();
						Dictionary<int, InputBehaviorWindow.PropertyType> dictionary = new Dictionary<int, InputBehaviorWindow.PropertyType>();
						string customEntry = ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.labelLanguageKey);
						if (!string.IsNullOrEmpty(customEntry))
						{
							uIControlSet.SetTitle(customEntry);
						}
						else
						{
							uIControlSet.SetTitle(inputBehavior.name);
						}
						if (inputBehaviorSettings.showJoystickAxisSensitivity)
						{
							UISliderControl uISliderControl = this.CreateSlider(uIControlSet, inputBehavior.id, null, ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.joystickAxisSensitivityLabelLanguageKey), inputBehaviorSettings.joystickAxisSensitivityIcon, inputBehaviorSettings.joystickAxisSensitivityMin, inputBehaviorSettings.joystickAxisSensitivityMax, new Action<int, int, float>(this.JoystickAxisSensitivityValueChanged), new Action<int, int>(this.JoystickAxisSensitivityCanceled));
							uISliderControl.slider.value = Mathf.Clamp(inputBehavior.joystickAxisSensitivity, inputBehaviorSettings.joystickAxisSensitivityMin, inputBehaviorSettings.joystickAxisSensitivityMax);
							dictionary.Add(uISliderControl.id, InputBehaviorWindow.PropertyType.JoystickAxisSensitivity);
						}
						if (inputBehaviorSettings.showMouseXYAxisSensitivity)
						{
							UISliderControl uISliderControl2 = this.CreateSlider(uIControlSet, inputBehavior.id, null, ControlMapper.GetLanguage().GetCustomEntry(inputBehaviorSettings.mouseXYAxisSensitivityLabelLanguageKey), inputBehaviorSettings.mouseXYAxisSensitivityIcon, inputBehaviorSettings.mouseXYAxisSensitivityMin, inputBehaviorSettings.mouseXYAxisSensitivityMax, new Action<int, int, float>(this.MouseXYAxisSensitivityValueChanged), new Action<int, int>(this.MouseXYAxisSensitivityCanceled));
							uISliderControl2.slider.value = Mathf.Clamp(inputBehavior.mouseXYAxisSensitivity, inputBehaviorSettings.mouseXYAxisSensitivityMin, inputBehaviorSettings.mouseXYAxisSensitivityMax);
							dictionary.Add(uISliderControl2.id, InputBehaviorWindow.PropertyType.MouseXYAxisSensitivity);
						}
						this.inputBehaviorInfo.Add(new InputBehaviorWindow.InputBehaviorInfo(inputBehavior, uIControlSet, dictionary));
					}
				}
			}
			base.defaultUIElement = this.doneButton.gameObject;
		}

		public void SetButtonCallback(InputBehaviorWindow.ButtonIdentifier buttonIdentifier, Action<int> callback)
		{
			if (!base.initialized)
			{
				return;
			}
			if (callback == null)
			{
				return;
			}
			if (this.buttonCallbacks.ContainsKey((int)buttonIdentifier))
			{
				this.buttonCallbacks[(int)buttonIdentifier] = callback;
			}
			else
			{
				this.buttonCallbacks.Add((int)buttonIdentifier, callback);
			}
		}

		public override void Cancel()
		{
			if (!base.initialized)
			{
				return;
			}
			foreach (InputBehaviorWindow.InputBehaviorInfo current in this.inputBehaviorInfo)
			{
				current.RestorePreviousData();
			}
			Action<int> action;
			if (!this.buttonCallbacks.TryGetValue(1, out action))
			{
				if (this.cancelCallback != null)
				{
					this.cancelCallback();
				}
				return;
			}
			action(base.id);
		}

		public void OnDone()
		{
			if (!base.initialized)
			{
				return;
			}
			Action<int> action;
			if (!this.buttonCallbacks.TryGetValue(0, out action))
			{
				return;
			}
			action(base.id);
		}

		public void OnCancel()
		{
			this.Cancel();
		}

		public void OnRestoreDefault()
		{
			if (!base.initialized)
			{
				return;
			}
			foreach (InputBehaviorWindow.InputBehaviorInfo current in this.inputBehaviorInfo)
			{
				current.RestoreDefaultData();
			}
		}

		private void JoystickAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value)
		{
			this.GetInputBehavior(inputBehaviorId).joystickAxisSensitivity = value;
		}

		private void MouseXYAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value)
		{
			this.GetInputBehavior(inputBehaviorId).mouseXYAxisSensitivity = value;
		}

		private void JoystickAxisSensitivityCanceled(int inputBehaviorId, int controlId)
		{
			InputBehaviorWindow.InputBehaviorInfo inputBehaviorInfo = this.GetInputBehaviorInfo(inputBehaviorId);
			if (inputBehaviorInfo == null)
			{
				return;
			}
			inputBehaviorInfo.RestoreData(InputBehaviorWindow.PropertyType.JoystickAxisSensitivity, controlId);
		}

		private void MouseXYAxisSensitivityCanceled(int inputBehaviorId, int controlId)
		{
			InputBehaviorWindow.InputBehaviorInfo inputBehaviorInfo = this.GetInputBehaviorInfo(inputBehaviorId);
			if (inputBehaviorInfo == null)
			{
				return;
			}
			inputBehaviorInfo.RestoreData(InputBehaviorWindow.PropertyType.MouseXYAxisSensitivity, controlId);
		}

		public override void TakeInputFocus()
		{
			base.TakeInputFocus();
		}

		private UIControlSet CreateControlSet()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.uiControlSetPrefab);
			gameObject.transform.SetParent(this.spawnTransform, false);
			return gameObject.GetComponent<UIControlSet>();
		}

		private UISliderControl CreateSlider(UIControlSet set, int inputBehaviorId, string defaultTitle, string overrideTitle, Sprite icon, float minValue, float maxValue, Action<int, int, float> valueChangedCallback, Action<int, int> cancelCallback)
		{
			UISliderControl uISliderControl = set.CreateSlider(this.uiSliderControlPrefab, icon, minValue, maxValue, delegate(int cId, float value)
			{
				valueChangedCallback(inputBehaviorId, cId, value);
			}, delegate(int cId)
			{
				cancelCallback(inputBehaviorId, cId);
			});
			string text = (!string.IsNullOrEmpty(overrideTitle)) ? overrideTitle : defaultTitle;
			if (!string.IsNullOrEmpty(text))
			{
				uISliderControl.showTitle = true;
				uISliderControl.title.text = text;
			}
			else
			{
				uISliderControl.showTitle = false;
			}
			uISliderControl.showIcon = (icon != null);
			return uISliderControl;
		}

		private InputBehavior GetInputBehavior(int id)
		{
			return ReInput.mapping.GetInputBehavior(this.playerId, id);
		}

		private InputBehaviorWindow.InputBehaviorInfo GetInputBehaviorInfo(int inputBehaviorId)
		{
			int count = this.inputBehaviorInfo.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.inputBehaviorInfo[i].inputBehavior.id == inputBehaviorId)
				{
					return this.inputBehaviorInfo[i];
				}
			}
			return null;
		}
	}
}
