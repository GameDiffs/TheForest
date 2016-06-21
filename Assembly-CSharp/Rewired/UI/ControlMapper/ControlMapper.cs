using Rewired.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class ControlMapper : MonoBehaviour
	{
		private abstract class GUIElement
		{
			public readonly GameObject gameObject;

			protected readonly Text text;

			public readonly Selectable selectable;

			protected readonly UIElementInfo uiElementInfo;

			protected bool permanentStateSet;

			protected readonly List<ControlMapper.GUIElement> children;

			public RectTransform rectTransform
			{
				get;
				private set;
			}

			public GUIElement(GameObject gameObject)
			{
				if (gameObject == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: gameObject is null!");
					return;
				}
				this.selectable = gameObject.GetComponent<Selectable>();
				if (this.selectable == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: Selectable is null!");
					return;
				}
				this.gameObject = gameObject;
				this.rectTransform = gameObject.GetComponent<RectTransform>();
				this.text = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
				this.uiElementInfo = gameObject.GetComponent<UIElementInfo>();
				this.children = new List<ControlMapper.GUIElement>();
			}

			public GUIElement(Selectable selectable, Text label)
			{
				if (selectable == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: Selectable is null!");
					return;
				}
				this.selectable = selectable;
				this.gameObject = selectable.gameObject;
				this.rectTransform = this.gameObject.GetComponent<RectTransform>();
				this.text = label;
				this.uiElementInfo = this.gameObject.GetComponent<UIElementInfo>();
				this.children = new List<ControlMapper.GUIElement>();
			}

			public virtual void SetInteractible(bool state, bool playTransition)
			{
				this.SetInteractible(state, playTransition, false);
			}

			public virtual void SetInteractible(bool state, bool playTransition, bool permanent)
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i] != null)
					{
						this.children[i].SetInteractible(state, playTransition, permanent);
					}
				}
				if (this.permanentStateSet)
				{
					return;
				}
				if (this.selectable == null)
				{
					return;
				}
				if (permanent)
				{
					this.permanentStateSet = true;
				}
				if (this.selectable.interactable == state)
				{
					return;
				}
				UITools.SetInteractable(this.selectable, state, playTransition);
			}

			public virtual void SetTextWidth(int value)
			{
				if (this.text == null)
				{
					return;
				}
				LayoutElement layoutElement = this.text.GetComponent<LayoutElement>();
				if (layoutElement == null)
				{
					layoutElement = this.text.gameObject.AddComponent<LayoutElement>();
				}
				layoutElement.preferredWidth = (float)value;
			}

			public virtual void SetFirstChildObjectWidth(ControlMapper.LayoutElementSizeType type, int value)
			{
				if (this.rectTransform.childCount == 0)
				{
					return;
				}
				Transform child = this.rectTransform.GetChild(0);
				LayoutElement layoutElement = child.GetComponent<LayoutElement>();
				if (layoutElement == null)
				{
					layoutElement = child.gameObject.AddComponent<LayoutElement>();
				}
				if (type == ControlMapper.LayoutElementSizeType.MinSize)
				{
					layoutElement.minWidth = (float)value;
				}
				else
				{
					if (type != ControlMapper.LayoutElementSizeType.PreferredSize)
					{
						throw new NotImplementedException();
					}
					layoutElement.preferredWidth = (float)value;
				}
			}

			public virtual void SetLabel(string label)
			{
				if (this.text == null)
				{
					return;
				}
				this.text.text = label;
			}

			public virtual string GetLabel()
			{
				if (this.text == null)
				{
					return string.Empty;
				}
				return this.text.text;
			}

			public virtual void AddChild(ControlMapper.GUIElement child)
			{
				this.children.Add(child);
			}

			public void SetElementInfoData(string identifier, int intData)
			{
				if (this.uiElementInfo == null)
				{
					return;
				}
				this.uiElementInfo.identifier = identifier;
				this.uiElementInfo.intData = intData;
			}

			public virtual void SetActive(bool state)
			{
				if (this.gameObject == null)
				{
					return;
				}
				this.gameObject.SetActive(state);
			}

			protected virtual bool Init()
			{
				bool result = true;
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i] != null)
					{
						if (!this.children[i].Init())
						{
							result = false;
						}
					}
				}
				if (this.selectable == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: UI Element is missing Selectable component!");
					result = false;
				}
				if (this.rectTransform == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: UI Element is missing RectTransform component!");
					result = false;
				}
				if (this.uiElementInfo == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: UI Element is missing UIElementInfo component!");
					result = false;
				}
				return result;
			}
		}

		private class GUIButton : ControlMapper.GUIElement
		{
			protected UnityEngine.UI.Button button
			{
				get
				{
					return this.selectable as UnityEngine.UI.Button;
				}
			}

			public ButtonInfo buttonInfo
			{
				get
				{
					return this.uiElementInfo as ButtonInfo;
				}
			}

			public GUIButton(GameObject gameObject) : base(gameObject)
			{
				if (!this.Init())
				{
					return;
				}
			}

			public GUIButton(UnityEngine.UI.Button button, Text label) : base(button, label)
			{
				if (!this.Init())
				{
					return;
				}
			}

			public void SetButtonInfoData(string identifier, int intData)
			{
				base.SetElementInfoData(identifier, intData);
			}

			public void SetOnClickCallback(Action<ButtonInfo> callback)
			{
				if (this.button == null)
				{
					return;
				}
				this.button.onClick.AddListener(delegate
				{
					callback(this.buttonInfo);
				});
			}
		}

		private class GUIInputField : ControlMapper.GUIElement
		{
			protected UnityEngine.UI.Button button
			{
				get
				{
					return this.selectable as UnityEngine.UI.Button;
				}
			}

			public InputFieldInfo fieldInfo
			{
				get
				{
					return this.uiElementInfo as InputFieldInfo;
				}
			}

			public bool hasToggle
			{
				get
				{
					return this.toggle != null;
				}
			}

			public ControlMapper.GUIToggle toggle
			{
				get;
				private set;
			}

			public int actionElementMapId
			{
				get
				{
					if (this.fieldInfo == null)
					{
						return -1;
					}
					return this.fieldInfo.actionElementMapId;
				}
				set
				{
					if (this.fieldInfo == null)
					{
						return;
					}
					this.fieldInfo.actionElementMapId = value;
				}
			}

			public int controllerId
			{
				get
				{
					if (this.fieldInfo == null)
					{
						return -1;
					}
					return this.fieldInfo.controllerId;
				}
				set
				{
					if (this.fieldInfo == null)
					{
						return;
					}
					this.fieldInfo.controllerId = value;
				}
			}

			public GUIInputField(GameObject gameObject) : base(gameObject)
			{
				if (!this.Init())
				{
					return;
				}
			}

			public GUIInputField(UnityEngine.UI.Button button, Text label) : base(button, label)
			{
				if (!this.Init())
				{
					return;
				}
			}

			public void SetFieldInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
			{
				base.SetElementInfoData(string.Empty, intData);
				if (this.fieldInfo == null)
				{
					return;
				}
				this.fieldInfo.actionId = actionId;
				this.fieldInfo.axisRange = axisRange;
				this.fieldInfo.controllerType = controllerType;
			}

			public void SetOnClickCallback(Action<InputFieldInfo> callback)
			{
				if (this.button == null)
				{
					return;
				}
				this.button.onClick.AddListener(delegate
				{
					callback(this.fieldInfo);
				});
			}

			public virtual void SetInteractable(bool state, bool playTransition, bool permanent)
			{
				if (this.permanentStateSet)
				{
					return;
				}
				if (this.hasToggle && !state)
				{
					this.toggle.SetInteractible(state, playTransition, permanent);
				}
				base.SetInteractible(state, playTransition, permanent);
			}

			public void AddToggle(ControlMapper.GUIToggle toggle)
			{
				if (toggle == null)
				{
					return;
				}
				this.toggle = toggle;
			}
		}

		private class GUIToggle : ControlMapper.GUIElement
		{
			protected Toggle toggle
			{
				get
				{
					return this.selectable as Toggle;
				}
			}

			public ToggleInfo toggleInfo
			{
				get
				{
					return this.uiElementInfo as ToggleInfo;
				}
			}

			public int actionElementMapId
			{
				get
				{
					if (this.toggleInfo == null)
					{
						return -1;
					}
					return this.toggleInfo.actionElementMapId;
				}
				set
				{
					if (this.toggleInfo == null)
					{
						return;
					}
					this.toggleInfo.actionElementMapId = value;
				}
			}

			public GUIToggle(GameObject gameObject) : base(gameObject)
			{
				if (!this.Init())
				{
					return;
				}
			}

			public GUIToggle(Toggle toggle, Text label) : base(toggle, label)
			{
				if (!this.Init())
				{
					return;
				}
			}

			public void SetToggleInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
			{
				base.SetElementInfoData(string.Empty, intData);
				if (this.toggleInfo == null)
				{
					return;
				}
				this.toggleInfo.actionId = actionId;
				this.toggleInfo.axisRange = axisRange;
				this.toggleInfo.controllerType = controllerType;
			}

			public void SetOnSubmitCallback(Action<ToggleInfo, bool> callback)
			{
				if (this.toggle == null)
				{
					return;
				}
				EventTrigger eventTrigger = this.toggle.GetComponent<EventTrigger>();
				if (eventTrigger == null)
				{
					eventTrigger = this.toggle.gameObject.AddComponent<EventTrigger>();
				}
				EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
				triggerEvent.AddListener(delegate(BaseEventData data)
				{
					PointerEventData pointerEventData = data as PointerEventData;
					if (pointerEventData != null && pointerEventData.button != PointerEventData.InputButton.Left)
					{
						return;
					}
					callback(this.toggleInfo, this.toggle.isOn);
				});
				EventTrigger.Entry item = new EventTrigger.Entry
				{
					callback = triggerEvent,
					eventID = EventTriggerType.Submit
				};
				EventTrigger.Entry item2 = new EventTrigger.Entry
				{
					callback = triggerEvent,
					eventID = EventTriggerType.PointerClick
				};
				if (eventTrigger.triggers != null)
				{
					eventTrigger.triggers.Clear();
				}
				else
				{
					eventTrigger.triggers = new List<EventTrigger.Entry>();
				}
				eventTrigger.triggers.Add(item);
				eventTrigger.triggers.Add(item2);
			}

			public void SetToggleState(bool state)
			{
				if (this.toggle == null)
				{
					return;
				}
				this.toggle.isOn = state;
			}
		}

		private class GUILabel
		{
			public GameObject gameObject
			{
				get;
				private set;
			}

			private Text text
			{
				get;
				set;
			}

			public RectTransform rectTransform
			{
				get;
				private set;
			}

			public GUILabel(GameObject gameObject)
			{
				if (gameObject == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: gameObject is null!");
					return;
				}
				this.text = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
				this.Check();
			}

			public GUILabel(Text label)
			{
				this.text = label;
				if (!this.Check())
				{
					return;
				}
			}

			public void SetSize(int width, int height)
			{
				if (this.text == null)
				{
					return;
				}
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)width);
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)height);
			}

			public void SetWidth(int width)
			{
				if (this.text == null)
				{
					return;
				}
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)width);
			}

			public void SetHeight(int height)
			{
				if (this.text == null)
				{
					return;
				}
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)height);
			}

			public void SetLabel(string label)
			{
				if (this.text == null)
				{
					return;
				}
				this.text.text = label;
			}

			public void SetFontStyle(FontStyle style)
			{
				if (this.text == null)
				{
					return;
				}
				this.text.fontStyle = style;
			}

			public void SetTextAlignment(TextAnchor alignment)
			{
				if (this.text == null)
				{
					return;
				}
				this.text.alignment = alignment;
			}

			public void SetActive(bool state)
			{
				if (this.gameObject == null)
				{
					return;
				}
				this.gameObject.SetActive(state);
			}

			private bool Check()
			{
				bool result = true;
				if (this.text == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: Button is missing Text child component!");
					result = false;
				}
				this.gameObject = this.text.gameObject;
				this.rectTransform = this.text.GetComponent<RectTransform>();
				return result;
			}
		}

		[Serializable]
		public class MappingSet
		{
			public enum ActionListMode
			{
				ActionCategory,
				Action
			}

			[SerializeField, Tooltip("The Map Category that will be displayed to the user for remapping.")]
			private int _mapCategoryId;

			[SerializeField, Tooltip("Choose whether you want to list Actions to display for this Map Category by individual Action or by all the Actions in an Action Category.")]
			private ControlMapper.MappingSet.ActionListMode _actionListMode;

			[SerializeField]
			private int[] _actionCategoryIds;

			[SerializeField]
			private int[] _actionIds;

			private IList<int> _actionCategoryIdsReadOnly;

			private IList<int> _actionIdsReadOnly;

			public int mapCategoryId
			{
				get
				{
					return this._mapCategoryId;
				}
			}

			public ControlMapper.MappingSet.ActionListMode actionListMode
			{
				get
				{
					return this._actionListMode;
				}
			}

			public IList<int> actionCategoryIds
			{
				get
				{
					if (this._actionCategoryIds == null)
					{
						return null;
					}
					if (this._actionCategoryIdsReadOnly == null)
					{
						this._actionCategoryIdsReadOnly = new ReadOnlyCollection<int>(this._actionCategoryIds);
					}
					return this._actionCategoryIdsReadOnly;
				}
			}

			public IList<int> actionIds
			{
				get
				{
					if (this._actionIds == null)
					{
						return null;
					}
					if (this._actionIdsReadOnly == null)
					{
						this._actionIdsReadOnly = new ReadOnlyCollection<int>(this._actionIds);
					}
					return this._actionIds;
				}
			}

			public bool isValid
			{
				get
				{
					return this._mapCategoryId >= 0 && ReInput.mapping.GetMapCategory(this._mapCategoryId) != null;
				}
			}

			public static ControlMapper.MappingSet Default
			{
				get
				{
					return new ControlMapper.MappingSet(0, ControlMapper.MappingSet.ActionListMode.ActionCategory, new int[1], new int[0]);
				}
			}

			public MappingSet()
			{
				this._mapCategoryId = -1;
				this._actionCategoryIds = new int[0];
				this._actionIds = new int[0];
				this._actionListMode = ControlMapper.MappingSet.ActionListMode.ActionCategory;
			}

			private MappingSet(int mapCategoryId, ControlMapper.MappingSet.ActionListMode actionListMode, int[] actionCategoryIds, int[] actionIds)
			{
				this._mapCategoryId = mapCategoryId;
				this._actionListMode = actionListMode;
				this._actionCategoryIds = actionCategoryIds;
				this._actionIds = actionIds;
			}
		}

		[Serializable]
		public class InputBehaviorSettings
		{
			[SerializeField, Tooltip("The Input Behavior that will be displayed to the user for modification.")]
			private int _inputBehaviorId = -1;

			[SerializeField, Tooltip("If checked, a slider will be displayed so the user can change this value.")]
			private bool _showJoystickAxisSensitivity = true;

			[SerializeField, Tooltip("If checked, a slider will be displayed so the user can change this value.")]
			private bool _showMouseXYAxisSensitivity = true;

			[SerializeField, Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed as the title for the Input Behavior control set. Otherwise, the name field of the InputBehavior will be used.")]
			private string _labelLanguageKey = string.Empty;

			[SerializeField, Tooltip("If set to a non-blank value, this name will be displayed above the individual slider control. Otherwise, no name will be displayed.")]
			private string _joystickAxisSensitivityLabelLanguageKey = string.Empty;

			[SerializeField, Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed above the individual slider control. Otherwise, no name will be displayed.")]
			private string _mouseXYAxisSensitivityLabelLanguageKey = string.Empty;

			[SerializeField, Tooltip("The icon to display next to the slider. Set to none for no icon.")]
			private Sprite _joystickAxisSensitivityIcon;

			[SerializeField, Tooltip("The icon to display next to the slider. Set to none for no icon.")]
			private Sprite _mouseXYAxisSensitivityIcon;

			[SerializeField, Tooltip("Minimum value the user is allowed to set for this property.")]
			private float _joystickAxisSensitivityMin;

			[SerializeField, Tooltip("Maximum value the user is allowed to set for this property.")]
			private float _joystickAxisSensitivityMax = 2f;

			[SerializeField, Tooltip("Minimum value the user is allowed to set for this property.")]
			private float _mouseXYAxisSensitivityMin;

			[SerializeField, Tooltip("Maximum value the user is allowed to set for this property.")]
			private float _mouseXYAxisSensitivityMax = 2f;

			public int inputBehaviorId
			{
				get
				{
					return this._inputBehaviorId;
				}
			}

			public bool showJoystickAxisSensitivity
			{
				get
				{
					return this._showJoystickAxisSensitivity;
				}
			}

			public bool showMouseXYAxisSensitivity
			{
				get
				{
					return this._showMouseXYAxisSensitivity;
				}
			}

			public string labelLanguageKey
			{
				get
				{
					return this._labelLanguageKey;
				}
			}

			public string joystickAxisSensitivityLabelLanguageKey
			{
				get
				{
					return this._joystickAxisSensitivityLabelLanguageKey;
				}
			}

			public string mouseXYAxisSensitivityLabelLanguageKey
			{
				get
				{
					return this._mouseXYAxisSensitivityLabelLanguageKey;
				}
			}

			public Sprite joystickAxisSensitivityIcon
			{
				get
				{
					return this._joystickAxisSensitivityIcon;
				}
			}

			public Sprite mouseXYAxisSensitivityIcon
			{
				get
				{
					return this._mouseXYAxisSensitivityIcon;
				}
			}

			public float joystickAxisSensitivityMin
			{
				get
				{
					return this._joystickAxisSensitivityMin;
				}
			}

			public float joystickAxisSensitivityMax
			{
				get
				{
					return this._joystickAxisSensitivityMax;
				}
			}

			public float mouseXYAxisSensitivityMin
			{
				get
				{
					return this._mouseXYAxisSensitivityMin;
				}
			}

			public float mouseXYAxisSensitivityMax
			{
				get
				{
					return this._mouseXYAxisSensitivityMax;
				}
			}

			public bool isValid
			{
				get
				{
					return this._inputBehaviorId >= 0 && (this._showJoystickAxisSensitivity || this._showMouseXYAxisSensitivity);
				}
			}
		}

		[Serializable]
		private class Prefabs
		{
			[SerializeField]
			private GameObject _button;

			[SerializeField]
			private GameObject _fitButton;

			[SerializeField]
			private GameObject _inputGridLabel;

			[SerializeField]
			private GameObject _inputGridHeaderLabel;

			[SerializeField]
			private GameObject _inputGridFieldButton;

			[SerializeField]
			private GameObject _inputGridFieldInvertToggle;

			[SerializeField]
			private GameObject _window;

			[SerializeField]
			private GameObject _windowTitleText;

			[SerializeField]
			private GameObject _windowContentText;

			[SerializeField]
			private GameObject _fader;

			[SerializeField]
			private GameObject _calibrationWindow;

			[SerializeField]
			private GameObject _inputBehaviorsWindow;

			[SerializeField]
			private GameObject _centerStickGraphic;

			[SerializeField]
			private GameObject _moveStickGraphic;

			public GameObject button
			{
				get
				{
					return this._button;
				}
			}

			public GameObject fitButton
			{
				get
				{
					return this._fitButton;
				}
			}

			public GameObject inputGridLabel
			{
				get
				{
					return this._inputGridLabel;
				}
			}

			public GameObject inputGridHeaderLabel
			{
				get
				{
					return this._inputGridHeaderLabel;
				}
			}

			public GameObject inputGridFieldButton
			{
				get
				{
					return this._inputGridFieldButton;
				}
			}

			public GameObject inputGridFieldInvertToggle
			{
				get
				{
					return this._inputGridFieldInvertToggle;
				}
			}

			public GameObject window
			{
				get
				{
					return this._window;
				}
			}

			public GameObject windowTitleText
			{
				get
				{
					return this._windowTitleText;
				}
			}

			public GameObject windowContentText
			{
				get
				{
					return this._windowContentText;
				}
			}

			public GameObject fader
			{
				get
				{
					return this._fader;
				}
			}

			public GameObject calibrationWindow
			{
				get
				{
					return this._calibrationWindow;
				}
			}

			public GameObject inputBehaviorsWindow
			{
				get
				{
					return this._inputBehaviorsWindow;
				}
			}

			public GameObject centerStickGraphic
			{
				get
				{
					return this._centerStickGraphic;
				}
			}

			public GameObject moveStickGraphic
			{
				get
				{
					return this._moveStickGraphic;
				}
			}

			public bool Check()
			{
				return !(this._button == null) && !(this._fitButton == null) && !(this._inputGridLabel == null) && !(this._inputGridHeaderLabel == null) && !(this._inputGridFieldButton == null) && !(this._inputGridFieldInvertToggle == null) && !(this._window == null) && !(this._windowTitleText == null) && !(this._windowContentText == null) && !(this._fader == null) && !(this._calibrationWindow == null) && !(this._inputBehaviorsWindow == null);
			}
		}

		[Serializable]
		private class References
		{
			[SerializeField]
			private Canvas _canvas;

			[SerializeField]
			private CanvasGroup _mainCanvasGroup;

			[SerializeField]
			private Transform _mainContent;

			[SerializeField]
			private Transform _mainContentInner;

			[SerializeField]
			private UIGroup _playersGroup;

			[SerializeField]
			private Transform _controllerGroup;

			[SerializeField]
			private Transform _controllerGroupLabelGroup;

			[SerializeField]
			private UIGroup _controllerSettingsGroup;

			[SerializeField]
			private UIGroup _assignedControllersGroup;

			[SerializeField]
			private Transform _settingsAndMapCategoriesGroup;

			[SerializeField]
			private UIGroup _settingsGroup;

			[SerializeField]
			private UIGroup _mapCategoriesGroup;

			[SerializeField]
			private Transform _inputGridGroup;

			[SerializeField]
			private Transform _inputGridContainer;

			[SerializeField]
			private Transform _inputGridHeadersGroup;

			[SerializeField]
			private Scrollbar _inputGridVScrollbar;

			[SerializeField]
			private ScrollRect _inputGridScrollRect;

			[SerializeField]
			private Transform _inputGridInnerGroup;

			[SerializeField]
			private Text _controllerNameLabel;

			[SerializeField]
			private UnityEngine.UI.Button _removeControllerButton;

			[SerializeField]
			private UnityEngine.UI.Button _assignControllerButton;

			[SerializeField]
			private UnityEngine.UI.Button _calibrateControllerButton;

			[SerializeField]
			private UnityEngine.UI.Button _doneButton;

			[SerializeField]
			private UnityEngine.UI.Button _restoreDefaultsButton;

			[SerializeField]
			private Selectable _defaultSelection;

			[SerializeField]
			private GameObject[] _fixedSelectableUIElements;

			[SerializeField]
			private Image _mainBackgroundImage;

			public Canvas canvas
			{
				get
				{
					return this._canvas;
				}
			}

			public CanvasGroup mainCanvasGroup
			{
				get
				{
					return this._mainCanvasGroup;
				}
			}

			public Transform mainContent
			{
				get
				{
					return this._mainContent;
				}
			}

			public Transform mainContentInner
			{
				get
				{
					return this._mainContentInner;
				}
			}

			public UIGroup playersGroup
			{
				get
				{
					return this._playersGroup;
				}
			}

			public Transform controllerGroup
			{
				get
				{
					return this._controllerGroup;
				}
			}

			public Transform controllerGroupLabelGroup
			{
				get
				{
					return this._controllerGroupLabelGroup;
				}
			}

			public UIGroup controllerSettingsGroup
			{
				get
				{
					return this._controllerSettingsGroup;
				}
			}

			public UIGroup assignedControllersGroup
			{
				get
				{
					return this._assignedControllersGroup;
				}
			}

			public Transform settingsAndMapCategoriesGroup
			{
				get
				{
					return this._settingsAndMapCategoriesGroup;
				}
			}

			public UIGroup settingsGroup
			{
				get
				{
					return this._settingsGroup;
				}
			}

			public UIGroup mapCategoriesGroup
			{
				get
				{
					return this._mapCategoriesGroup;
				}
			}

			public Transform inputGridGroup
			{
				get
				{
					return this._inputGridGroup;
				}
			}

			public Transform inputGridContainer
			{
				get
				{
					return this._inputGridContainer;
				}
			}

			public Transform inputGridHeadersGroup
			{
				get
				{
					return this._inputGridHeadersGroup;
				}
			}

			public Scrollbar inputGridVScrollbar
			{
				get
				{
					return this._inputGridVScrollbar;
				}
			}

			public ScrollRect inputGridScrollRect
			{
				get
				{
					return this._inputGridScrollRect;
				}
			}

			public Transform inputGridInnerGroup
			{
				get
				{
					return this._inputGridInnerGroup;
				}
			}

			public Text controllerNameLabel
			{
				get
				{
					return this._controllerNameLabel;
				}
			}

			public UnityEngine.UI.Button removeControllerButton
			{
				get
				{
					return this._removeControllerButton;
				}
			}

			public UnityEngine.UI.Button assignControllerButton
			{
				get
				{
					return this._assignControllerButton;
				}
			}

			public UnityEngine.UI.Button calibrateControllerButton
			{
				get
				{
					return this._calibrateControllerButton;
				}
			}

			public UnityEngine.UI.Button doneButton
			{
				get
				{
					return this._doneButton;
				}
			}

			public UnityEngine.UI.Button restoreDefaultsButton
			{
				get
				{
					return this._restoreDefaultsButton;
				}
			}

			public Selectable defaultSelection
			{
				get
				{
					return this._defaultSelection;
				}
			}

			public GameObject[] fixedSelectableUIElements
			{
				get
				{
					return this._fixedSelectableUIElements;
				}
			}

			public Image mainBackgroundImage
			{
				get
				{
					return this._mainBackgroundImage;
				}
			}

			public LayoutElement inputGridLayoutElement
			{
				get;
				set;
			}

			public Transform inputGridActionColumn
			{
				get;
				set;
			}

			public Transform inputGridKeyboardColumn
			{
				get;
				set;
			}

			public Transform inputGridMouseColumn
			{
				get;
				set;
			}

			public Transform inputGridControllerColumn
			{
				get;
				set;
			}

			public Transform inputGridHeader1
			{
				get;
				set;
			}

			public Transform inputGridHeader2
			{
				get;
				set;
			}

			public Transform inputGridHeader3
			{
				get;
				set;
			}

			public Transform inputGridHeader4
			{
				get;
				set;
			}

			public bool Check()
			{
				return !(this._canvas == null) && !(this._mainCanvasGroup == null) && !(this._mainContent == null) && !(this._mainContentInner == null) && !(this._playersGroup == null) && !(this._controllerGroup == null) && !(this._controllerGroupLabelGroup == null) && !(this._controllerSettingsGroup == null) && !(this._assignedControllersGroup == null) && !(this._settingsAndMapCategoriesGroup == null) && !(this._settingsGroup == null) && !(this._mapCategoriesGroup == null) && !(this._inputGridGroup == null) && !(this._inputGridContainer == null) && !(this._inputGridHeadersGroup == null) && !(this._inputGridVScrollbar == null) && !(this._inputGridScrollRect == null) && !(this._inputGridInnerGroup == null) && !(this._controllerNameLabel == null) && !(this._removeControllerButton == null) && !(this._assignControllerButton == null) && !(this._calibrateControllerButton == null) && !(this._doneButton == null) && !(this._restoreDefaultsButton == null) && !(this._defaultSelection == null);
			}
		}

		private class InputActionSet
		{
			private int _actionId;

			private AxisRange _axisRange;

			public int actionId
			{
				get
				{
					return this._actionId;
				}
			}

			public AxisRange axisRange
			{
				get
				{
					return this._axisRange;
				}
			}

			public InputActionSet(int actionId, AxisRange axisRange)
			{
				this._actionId = actionId;
				this._axisRange = axisRange;
			}
		}

		private class InputMapping
		{
			public string actionName
			{
				get;
				private set;
			}

			public InputFieldInfo fieldInfo
			{
				get;
				private set;
			}

			public ControllerMap map
			{
				get;
				private set;
			}

			public ActionElementMap aem
			{
				get;
				private set;
			}

			public ControllerType controllerType
			{
				get;
				private set;
			}

			public int controllerId
			{
				get;
				private set;
			}

			public ControllerPollingInfo pollingInfo
			{
				get;
				set;
			}

			public ModifierKeyFlags modifierKeyFlags
			{
				get;
				set;
			}

			public AxisRange axisRange
			{
				get
				{
					AxisRange result = AxisRange.Positive;
					if (this.pollingInfo.elementType == ControllerElementType.Axis)
					{
						if (this.fieldInfo.axisRange == AxisRange.Full)
						{
							result = AxisRange.Full;
						}
						else
						{
							result = ((this.pollingInfo.axisPole != Pole.Positive) ? AxisRange.Negative : AxisRange.Positive);
						}
					}
					return result;
				}
			}

			public string elementName
			{
				get
				{
					if (this.controllerType == ControllerType.Keyboard && this.modifierKeyFlags != ModifierKeyFlags.None)
					{
						return string.Format("{0} + {1}", Keyboard.ModifierKeyFlagsToString(this.modifierKeyFlags), this.pollingInfo.elementIdentifierName);
					}
					string text = this.pollingInfo.elementIdentifierName;
					if (this.pollingInfo.elementType == ControllerElementType.Axis)
					{
						if (this.axisRange == AxisRange.Positive)
						{
							text += " +";
						}
						else if (this.axisRange == AxisRange.Negative)
						{
							text += " -";
						}
					}
					return text;
				}
			}

			public InputMapping(string actionName, InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, ControllerType controllerType, int controllerId)
			{
				this.actionName = actionName;
				this.fieldInfo = fieldInfo;
				this.map = map;
				this.aem = aem;
				this.controllerType = controllerType;
				this.controllerId = controllerId;
			}

			public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo)
			{
				this.pollingInfo = pollingInfo;
				return this.ToElementAssignment();
			}

			public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo, ModifierKeyFlags modifierKeyFlags)
			{
				this.pollingInfo = pollingInfo;
				this.modifierKeyFlags = modifierKeyFlags;
				return this.ToElementAssignment();
			}

			public ElementAssignment ToElementAssignment()
			{
				return new ElementAssignment(this.controllerType, this.pollingInfo.elementType, this.pollingInfo.elementIdentifierId, this.axisRange, this.pollingInfo.keyboardKey, this.modifierKeyFlags, this.fieldInfo.actionId, (this.fieldInfo.axisRange != AxisRange.Negative) ? Pole.Positive : Pole.Negative, false, (this.aem == null) ? -1 : this.aem.id);
			}
		}

		private class AxisCalibrator
		{
			public AxisCalibrationData data;

			public readonly Rewired.Joystick joystick;

			public readonly int axisIndex;

			private Controller.Axis axis;

			private bool firstRun;

			public bool isValid
			{
				get
				{
					return this.axis != null;
				}
			}

			public AxisCalibrator(Rewired.Joystick joystick, int axisIndex)
			{
				this.data = default(AxisCalibrationData);
				this.joystick = joystick;
				this.axisIndex = axisIndex;
				if (joystick != null && axisIndex >= 0 && joystick.axisCount > axisIndex)
				{
					this.axis = joystick.Axes[axisIndex];
					this.data = joystick.calibrationMap.GetAxis(axisIndex).GetData();
				}
				this.firstRun = true;
			}

			public void RecordMinMax()
			{
				if (this.axis == null)
				{
					return;
				}
				float valueRaw = this.axis.valueRaw;
				if (this.firstRun || valueRaw < this.data.min)
				{
					this.data.min = valueRaw;
				}
				if (this.firstRun || valueRaw > this.data.max)
				{
					this.data.max = valueRaw;
				}
				this.firstRun = false;
			}

			public void RecordZero()
			{
				if (this.axis == null)
				{
					return;
				}
				this.data.zero = this.axis.valueRaw;
			}

			public void Commit()
			{
				if (this.axis == null)
				{
					return;
				}
				AxisCalibration axisCalibration = this.joystick.calibrationMap.GetAxis(this.axisIndex);
				if (axisCalibration == null)
				{
					return;
				}
				if ((double)Mathf.Abs(this.data.max - this.data.min) < 0.1)
				{
					return;
				}
				axisCalibration.SetData(this.data);
			}
		}

		private class IndexedDictionary<TKey, TValue>
		{
			private class Entry
			{
				public TKey key;

				public TValue value;

				public Entry(TKey key, TValue value)
				{
					this.key = key;
					this.value = value;
				}
			}

			private List<ControlMapper.IndexedDictionary<TKey, TValue>.Entry> list;

			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}

			public TValue this[int index]
			{
				get
				{
					return this.list[index].value;
				}
			}

			public IndexedDictionary()
			{
				this.list = new List<ControlMapper.IndexedDictionary<TKey, TValue>.Entry>();
			}

			public TValue Get(TKey key)
			{
				int num = this.IndexOfKey(key);
				if (num < 0)
				{
					throw new Exception("Key does not exist!");
				}
				return this.list[num].value;
			}

			public bool TryGet(TKey key, out TValue value)
			{
				value = default(TValue);
				int num = this.IndexOfKey(key);
				if (num < 0)
				{
					return false;
				}
				value = this.list[num].value;
				return true;
			}

			public void Add(TKey key, TValue value)
			{
				if (this.ContainsKey(key))
				{
					throw new Exception("Key " + key.ToString() + " is already in use!");
				}
				this.list.Add(new ControlMapper.IndexedDictionary<TKey, TValue>.Entry(key, value));
			}

			public int IndexOfKey(TKey key)
			{
				int count = this.list.Count;
				for (int i = 0; i < count; i++)
				{
					if (EqualityComparer<TKey>.Default.Equals(this.list[i].key, key))
					{
						return i;
					}
				}
				return -1;
			}

			public bool ContainsKey(TKey key)
			{
				int count = this.list.Count;
				for (int i = 0; i < count; i++)
				{
					if (EqualityComparer<TKey>.Default.Equals(this.list[i].key, key))
					{
						return true;
					}
				}
				return false;
			}

			public void Clear()
			{
				this.list.Clear();
			}
		}

		private enum LayoutElementSizeType
		{
			MinSize,
			PreferredSize
		}

		private enum WindowType
		{
			None,
			ChooseJoystick,
			JoystickAssignmentConflict,
			ElementAssignment,
			ElementAssignmentPrePolling,
			ElementAssignmentPolling,
			ElementAssignmentResult,
			ElementAssignmentConflict,
			Calibration,
			CalibrateStep1,
			CalibrateStep2
		}

		private class InputGrid
		{
			private ControlMapper.InputGridEntryList list;

			private List<GameObject> groups;

			public InputGrid()
			{
				this.list = new ControlMapper.InputGridEntryList();
				this.groups = new List<GameObject>();
			}

			public void AddMapCategory(int mapCategoryId)
			{
				this.list.AddMapCategory(mapCategoryId);
			}

			public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				this.list.AddAction(mapCategoryId, action, axisRange);
			}

			public void AddActionCategory(int mapCategoryId, int actionCategoryId)
			{
				this.list.AddActionCategory(mapCategoryId, actionCategoryId);
			}

			public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
			{
				this.list.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, fieldSetContainer);
			}

			public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, ControlMapper.GUIInputField inputField)
			{
				this.list.AddInputField(mapCategoryId, action, axisRange, controllerType, fieldIndex, inputField);
			}

			public void AddGroup(GameObject group)
			{
				this.groups.Add(group);
			}

			public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControlMapper.GUILabel label)
			{
				this.list.AddActionLabel(mapCategoryId, actionId, axisRange, label);
			}

			public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, ControlMapper.GUILabel label)
			{
				this.list.AddActionCategoryLabel(mapCategoryId, actionCategoryId, label);
			}

			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				return this.list.Contains(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
			}

			public ControlMapper.GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				return this.list.GetGUIInputField(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
			}

			public IEnumerable<ControlMapper.InputActionSet> GetActionSets(int mapCategoryId)
			{
				return this.list.GetActionSets(mapCategoryId);
			}

			public void SetColumnHeight(int mapCategoryId, float height)
			{
				this.list.SetColumnHeight(mapCategoryId, height);
			}

			public float GetColumnHeight(int mapCategoryId)
			{
				return this.list.GetColumnHeight(mapCategoryId);
			}

			public void SetFieldsActive(int mapCategoryId, bool state)
			{
				this.list.SetFieldsActive(mapCategoryId, state);
			}

			public void SetFieldLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
			{
				this.list.SetLabel(mapCategoryId, actionId, axisRange, controllerType, index, label);
			}

			public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
			{
				this.list.PopulateField(mapCategoryId, actionId, axisRange, controllerType, controllerId, index, actionElementMapId, label, invert);
			}

			public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
			{
				this.list.SetFixedFieldData(mapCategoryId, actionId, axisRange, controllerType, controllerId);
			}

			public void InitializeFields(int mapCategoryId)
			{
				this.list.InitializeFields(mapCategoryId);
			}

			public void Show(int mapCategoryId)
			{
				this.list.Show(mapCategoryId);
			}

			public void HideAll()
			{
				this.list.HideAll();
			}

			public void ClearLabels(int mapCategoryId)
			{
				this.list.ClearLabels(mapCategoryId);
			}

			private void ClearGroups()
			{
				for (int i = 0; i < this.groups.Count; i++)
				{
					if (!(this.groups[i] == null))
					{
						UnityEngine.Object.Destroy(this.groups[i]);
					}
				}
			}

			public void ClearAll()
			{
				this.ClearGroups();
				this.list.Clear();
			}
		}

		private class InputGridEntryList
		{
			private class MapCategoryEntry
			{
				private List<ControlMapper.InputGridEntryList.ActionEntry> _actionList;

				private ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.ActionCategoryEntry> _actionCategoryList;

				private float _columnHeight;

				public List<ControlMapper.InputGridEntryList.ActionEntry> actionList
				{
					get
					{
						return this._actionList;
					}
				}

				public ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.ActionCategoryEntry> actionCategoryList
				{
					get
					{
						return this._actionCategoryList;
					}
				}

				public float columnHeight
				{
					get
					{
						return this._columnHeight;
					}
					set
					{
						this._columnHeight = value;
					}
				}

				public MapCategoryEntry()
				{
					this._actionList = new List<ControlMapper.InputGridEntryList.ActionEntry>();
					this._actionCategoryList = new ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.ActionCategoryEntry>();
				}

				public ControlMapper.InputGridEntryList.ActionEntry GetActionEntry(int actionId, AxisRange axisRange)
				{
					int num = this.IndexOfActionEntry(actionId, axisRange);
					if (num < 0)
					{
						return null;
					}
					return this._actionList[num];
				}

				public int IndexOfActionEntry(int actionId, AxisRange axisRange)
				{
					int count = this._actionList.Count;
					for (int i = 0; i < count; i++)
					{
						if (this._actionList[i].Matches(actionId, axisRange))
						{
							return i;
						}
					}
					return -1;
				}

				public bool ContainsActionEntry(int actionId, AxisRange axisRange)
				{
					return this.IndexOfActionEntry(actionId, axisRange) >= 0;
				}

				public ControlMapper.InputGridEntryList.ActionEntry AddAction(InputAction action, AxisRange axisRange)
				{
					if (action == null)
					{
						return null;
					}
					if (this.ContainsActionEntry(action.id, axisRange))
					{
						return null;
					}
					this._actionList.Add(new ControlMapper.InputGridEntryList.ActionEntry(action, axisRange));
					return this._actionList[this._actionList.Count - 1];
				}

				public ControlMapper.InputGridEntryList.ActionCategoryEntry GetActionCategoryEntry(int actionCategoryId)
				{
					if (!this._actionCategoryList.ContainsKey(actionCategoryId))
					{
						return null;
					}
					return this._actionCategoryList.Get(actionCategoryId);
				}

				public ControlMapper.InputGridEntryList.ActionCategoryEntry AddActionCategory(int actionCategoryId)
				{
					if (actionCategoryId < 0)
					{
						return null;
					}
					if (this._actionCategoryList.ContainsKey(actionCategoryId))
					{
						return null;
					}
					this._actionCategoryList.Add(actionCategoryId, new ControlMapper.InputGridEntryList.ActionCategoryEntry(actionCategoryId));
					return this._actionCategoryList.Get(actionCategoryId);
				}

				public void SetAllActive(bool state)
				{
					for (int i = 0; i < this._actionCategoryList.Count; i++)
					{
						this._actionCategoryList[i].SetActive(state);
					}
					for (int j = 0; j < this._actionList.Count; j++)
					{
						this._actionList[j].SetActive(state);
					}
				}
			}

			private class ActionEntry
			{
				private ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.FieldSet> fieldSets;

				public ControlMapper.GUILabel label;

				public readonly InputAction action;

				public readonly AxisRange axisRange;

				public readonly ControlMapper.InputActionSet actionSet;

				public ActionEntry(InputAction action, AxisRange axisRange)
				{
					this.action = action;
					this.axisRange = axisRange;
					this.actionSet = new ControlMapper.InputActionSet(action.id, axisRange);
					this.fieldSets = new ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.FieldSet>();
				}

				public void SetLabel(ControlMapper.GUILabel label)
				{
					this.label = label;
				}

				public bool Matches(int actionId, AxisRange axisRange)
				{
					return this.action.id == actionId && this.axisRange == axisRange;
				}

				public void AddInputFieldSet(ControllerType controllerType, GameObject fieldSetContainer)
				{
					if (this.fieldSets.ContainsKey((int)controllerType))
					{
						return;
					}
					this.fieldSets.Add((int)controllerType, new ControlMapper.InputGridEntryList.FieldSet(fieldSetContainer));
				}

				public void AddInputField(ControllerType controllerType, int fieldIndex, ControlMapper.GUIInputField inputField)
				{
					if (!this.fieldSets.ContainsKey((int)controllerType))
					{
						return;
					}
					ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets.Get((int)controllerType);
					if (fieldSet.fields.ContainsKey(fieldIndex))
					{
						return;
					}
					fieldSet.fields.Add(fieldIndex, inputField);
				}

				public ControlMapper.GUIInputField GetGUIInputField(ControllerType controllerType, int fieldIndex)
				{
					if (!this.fieldSets.ContainsKey((int)controllerType))
					{
						return null;
					}
					if (!this.fieldSets.Get((int)controllerType).fields.ContainsKey(fieldIndex))
					{
						return null;
					}
					return this.fieldSets.Get((int)controllerType).fields.Get(fieldIndex);
				}

				public bool Contains(ControllerType controllerType, int fieldId)
				{
					return this.fieldSets.ContainsKey((int)controllerType) && this.fieldSets.Get((int)controllerType).fields.ContainsKey(fieldId);
				}

				public void SetFieldLabel(ControllerType controllerType, int index, string label)
				{
					if (!this.fieldSets.ContainsKey((int)controllerType))
					{
						return;
					}
					if (!this.fieldSets.Get((int)controllerType).fields.ContainsKey(index))
					{
						return;
					}
					this.fieldSets.Get((int)controllerType).fields.Get(index).SetLabel(label);
				}

				public void PopulateField(ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
				{
					if (!this.fieldSets.ContainsKey((int)controllerType))
					{
						return;
					}
					if (!this.fieldSets.Get((int)controllerType).fields.ContainsKey(index))
					{
						return;
					}
					ControlMapper.GUIInputField gUIInputField = this.fieldSets.Get((int)controllerType).fields.Get(index);
					gUIInputField.SetLabel(label);
					gUIInputField.actionElementMapId = actionElementMapId;
					gUIInputField.controllerId = controllerId;
					if (gUIInputField.hasToggle)
					{
						gUIInputField.toggle.SetInteractible(true, false);
						gUIInputField.toggle.SetToggleState(invert);
						gUIInputField.toggle.actionElementMapId = actionElementMapId;
					}
				}

				public void SetFixedFieldData(ControllerType controllerType, int controllerId)
				{
					if (!this.fieldSets.ContainsKey((int)controllerType))
					{
						return;
					}
					ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets.Get((int)controllerType);
					int count = fieldSet.fields.Count;
					for (int i = 0; i < count; i++)
					{
						fieldSet.fields[i].controllerId = controllerId;
					}
				}

				public void Initialize()
				{
					for (int i = 0; i < this.fieldSets.Count; i++)
					{
						ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							ControlMapper.GUIInputField gUIInputField = fieldSet.fields[j];
							if (gUIInputField.hasToggle)
							{
								gUIInputField.toggle.SetInteractible(false, false);
								gUIInputField.toggle.SetToggleState(false);
								gUIInputField.toggle.actionElementMapId = -1;
							}
							gUIInputField.SetLabel(string.Empty);
							gUIInputField.actionElementMapId = -1;
							gUIInputField.controllerId = -1;
						}
					}
				}

				public void SetActive(bool state)
				{
					if (this.label != null)
					{
						this.label.SetActive(state);
					}
					int count = this.fieldSets.Count;
					for (int i = 0; i < count; i++)
					{
						this.fieldSets[i].groupContainer.SetActive(state);
					}
				}

				public void ClearLabels()
				{
					for (int i = 0; i < this.fieldSets.Count; i++)
					{
						ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							ControlMapper.GUIInputField gUIInputField = fieldSet.fields[j];
							gUIInputField.SetLabel(string.Empty);
						}
					}
				}

				public void SetFieldsActive(bool state)
				{
					for (int i = 0; i < this.fieldSets.Count; i++)
					{
						ControlMapper.InputGridEntryList.FieldSet fieldSet = this.fieldSets[i];
						int count = fieldSet.fields.Count;
						for (int j = 0; j < count; j++)
						{
							ControlMapper.GUIInputField gUIInputField = fieldSet.fields[j];
							gUIInputField.SetInteractible(state, false);
							if (gUIInputField.hasToggle && (!state || gUIInputField.toggle.actionElementMapId >= 0))
							{
								gUIInputField.toggle.SetInteractible(state, false);
							}
						}
					}
				}
			}

			private class FieldSet
			{
				public readonly GameObject groupContainer;

				public readonly ControlMapper.IndexedDictionary<int, ControlMapper.GUIInputField> fields;

				public FieldSet(GameObject groupContainer)
				{
					this.groupContainer = groupContainer;
					this.fields = new ControlMapper.IndexedDictionary<int, ControlMapper.GUIInputField>();
				}
			}

			private class ActionCategoryEntry
			{
				public readonly int actionCategoryId;

				public ControlMapper.GUILabel label;

				public ActionCategoryEntry(int actionCategoryId)
				{
					this.actionCategoryId = actionCategoryId;
				}

				public void SetLabel(ControlMapper.GUILabel label)
				{
					this.label = label;
				}

				public void SetActive(bool state)
				{
					if (this.label != null)
					{
						this.label.SetActive(state);
					}
				}
			}

			private ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.MapCategoryEntry> entries;

			public InputGridEntryList()
			{
				this.entries = new ControlMapper.IndexedDictionary<int, ControlMapper.InputGridEntryList.MapCategoryEntry>();
			}

			public void AddMapCategory(int mapCategoryId)
			{
				if (mapCategoryId < 0)
				{
					return;
				}
				if (this.entries.ContainsKey(mapCategoryId))
				{
					return;
				}
				this.entries.Add(mapCategoryId, new ControlMapper.InputGridEntryList.MapCategoryEntry());
			}

			public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				this.AddActionEntry(mapCategoryId, action, axisRange);
			}

			private ControlMapper.InputGridEntryList.ActionEntry AddActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				if (action == null)
				{
					return null;
				}
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return null;
				}
				return mapCategoryEntry.AddAction(action, axisRange);
			}

			public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControlMapper.GUILabel label)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = mapCategoryEntry.GetActionEntry(actionId, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.SetLabel(label);
			}

			public void AddActionCategory(int mapCategoryId, int actionCategoryId)
			{
				this.AddActionCategoryEntry(mapCategoryId, actionCategoryId);
			}

			private ControlMapper.InputGridEntryList.ActionCategoryEntry AddActionCategoryEntry(int mapCategoryId, int actionCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return null;
				}
				return mapCategoryEntry.AddActionCategory(actionCategoryId);
			}

			public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, ControlMapper.GUILabel label)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				ControlMapper.InputGridEntryList.ActionCategoryEntry actionCategoryEntry = mapCategoryEntry.GetActionCategoryEntry(actionCategoryId);
				if (actionCategoryEntry == null)
				{
					return;
				}
				actionCategoryEntry.SetLabel(label);
			}

			public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, action, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.AddInputFieldSet(controllerType, fieldSetContainer);
			}

			public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, ControlMapper.GUIInputField inputField)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, action, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.AddInputField(controllerType, fieldIndex, inputField);
			}

			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange)
			{
				return this.GetActionEntry(mapCategoryId, actionId, axisRange) != null;
			}

			public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				return actionEntry != null && actionEntry.Contains(controllerType, fieldIndex);
			}

			public void SetColumnHeight(int mapCategoryId, float height)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				mapCategoryEntry.columnHeight = height;
			}

			public float GetColumnHeight(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return 0f;
				}
				return mapCategoryEntry.columnHeight;
			}

			public ControlMapper.GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return null;
				}
				return actionEntry.GetGUIInputField(controllerType, fieldIndex);
			}

			private ControlMapper.InputGridEntryList.ActionEntry GetActionEntry(int mapCategoryId, int actionId, AxisRange axisRange)
			{
				if (actionId < 0)
				{
					return null;
				}
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return null;
				}
				return mapCategoryEntry.GetActionEntry(actionId, axisRange);
			}

			private ControlMapper.InputGridEntryList.ActionEntry GetActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
			{
				if (action == null)
				{
					return null;
				}
				return this.GetActionEntry(mapCategoryId, action.id, axisRange);
			}

			[DebuggerHidden]
			public IEnumerable<ControlMapper.InputActionSet> GetActionSets(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.<GetActionSets>c__Iterator110 <GetActionSets>c__Iterator = new ControlMapper.InputGridEntryList.<GetActionSets>c__Iterator110();
				<GetActionSets>c__Iterator.mapCategoryId = mapCategoryId;
				<GetActionSets>c__Iterator.<$>mapCategoryId = mapCategoryId;
				<GetActionSets>c__Iterator.<>f__this = this;
				ControlMapper.InputGridEntryList.<GetActionSets>c__Iterator110 expr_1C = <GetActionSets>c__Iterator;
				expr_1C.$PC = -2;
				return expr_1C;
			}

			public void SetFieldsActive(int mapCategoryId, bool state)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				List<ControlMapper.InputGridEntryList.ActionEntry> actionList = mapCategoryEntry.actionList;
				int num = (actionList == null) ? 0 : actionList.Count;
				for (int i = 0; i < num; i++)
				{
					actionList[i].SetFieldsActive(state);
				}
			}

			public void SetLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.SetFieldLabel(controllerType, index, label);
			}

			public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.PopulateField(controllerType, controllerId, index, actionElementMapId, label, invert);
			}

			public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
			{
				ControlMapper.InputGridEntryList.ActionEntry actionEntry = this.GetActionEntry(mapCategoryId, actionId, axisRange);
				if (actionEntry == null)
				{
					return;
				}
				actionEntry.SetFixedFieldData(controllerType, controllerId);
			}

			public void InitializeFields(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				List<ControlMapper.InputGridEntryList.ActionEntry> actionList = mapCategoryEntry.actionList;
				int num = (actionList == null) ? 0 : actionList.Count;
				for (int i = 0; i < num; i++)
				{
					actionList[i].Initialize();
				}
			}

			public void Show(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				mapCategoryEntry.SetAllActive(true);
			}

			public void HideAll()
			{
				for (int i = 0; i < this.entries.Count; i++)
				{
					this.entries[i].SetAllActive(false);
				}
			}

			public void ClearLabels(int mapCategoryId)
			{
				ControlMapper.InputGridEntryList.MapCategoryEntry mapCategoryEntry;
				if (!this.entries.TryGet(mapCategoryId, out mapCategoryEntry))
				{
					return;
				}
				List<ControlMapper.InputGridEntryList.ActionEntry> actionList = mapCategoryEntry.actionList;
				int num = (actionList == null) ? 0 : actionList.Count;
				for (int i = 0; i < num; i++)
				{
					actionList[i].ClearLabels();
				}
			}

			public void Clear()
			{
				this.entries.Clear();
			}
		}

		private class WindowManager
		{
			private List<Window> windows;

			private GameObject windowPrefab;

			private Transform parent;

			private GameObject fader;

			private int idCounter;

			public bool isWindowOpen
			{
				get
				{
					for (int i = this.windows.Count - 1; i >= 0; i--)
					{
						if (!(this.windows[i] == null))
						{
							return true;
						}
					}
					return false;
				}
			}

			public Window topWindow
			{
				get
				{
					for (int i = this.windows.Count - 1; i >= 0; i--)
					{
						if (!(this.windows[i] == null))
						{
							return this.windows[i];
						}
					}
					return null;
				}
			}

			public WindowManager(GameObject windowPrefab, GameObject faderPrefab, Transform parent)
			{
				this.windowPrefab = windowPrefab;
				this.parent = parent;
				this.windows = new List<Window>();
				this.fader = UnityEngine.Object.Instantiate<GameObject>(faderPrefab);
				this.fader.transform.SetParent(parent, false);
				this.fader.GetComponent<RectTransform>().localScale = Vector2.one;
				this.SetFaderActive(false);
			}

			public Window OpenWindow(string name, int width, int height)
			{
				Window result = this.InstantiateWindow(name, width, height);
				this.UpdateFader();
				return result;
			}

			public Window OpenWindow(GameObject windowPrefab, string name)
			{
				if (windowPrefab == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: Window Prefab is null!");
					return null;
				}
				Window result = this.InstantiateWindow(name, windowPrefab);
				this.UpdateFader();
				return result;
			}

			public void CloseTop()
			{
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (!(this.windows[i] == null))
					{
						this.DestroyWindow(this.windows[i]);
						this.windows.RemoveAt(i);
						break;
					}
					this.windows.RemoveAt(i);
				}
				this.UpdateFader();
			}

			public void CloseWindow(int windowId)
			{
				this.CloseWindow(this.GetWindow(windowId));
			}

			public void CloseWindow(Window window)
			{
				if (window == null)
				{
					return;
				}
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (this.windows[i] == null)
					{
						this.windows.RemoveAt(i);
					}
					else if (!(this.windows[i] != window))
					{
						this.DestroyWindow(this.windows[i]);
						this.windows.RemoveAt(i);
						break;
					}
				}
				this.UpdateFader();
				this.FocusTopWindow();
			}

			public void CloseAll()
			{
				this.SetFaderActive(false);
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (this.windows[i] == null)
					{
						this.windows.RemoveAt(i);
					}
					else
					{
						this.DestroyWindow(this.windows[i]);
						this.windows.RemoveAt(i);
					}
				}
				this.UpdateFader();
			}

			public void CancelAll()
			{
				if (!this.isWindowOpen)
				{
					return;
				}
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (!(this.windows[i] == null))
					{
						this.windows[i].Cancel();
					}
				}
				this.CloseAll();
			}

			public Window GetWindow(int windowId)
			{
				if (windowId < 0)
				{
					return null;
				}
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (!(this.windows[i] == null))
					{
						if (this.windows[i].id == windowId)
						{
							return this.windows[i];
						}
					}
				}
				return null;
			}

			public bool IsFocused(int windowId)
			{
				return windowId >= 0 && !(this.topWindow == null) && this.topWindow.id == windowId;
			}

			public void Focus(int windowId)
			{
				this.Focus(this.GetWindow(windowId));
			}

			public void Focus(Window window)
			{
				if (window == null)
				{
					return;
				}
				window.TakeInputFocus();
				this.DefocusOtherWindows(window.id);
			}

			private void DefocusOtherWindows(int focusedWindowId)
			{
				if (focusedWindowId < 0)
				{
					return;
				}
				for (int i = this.windows.Count - 1; i >= 0; i--)
				{
					if (!(this.windows[i] == null))
					{
						if (this.windows[i].id != focusedWindowId)
						{
							this.windows[i].Disable();
						}
					}
				}
			}

			private void UpdateFader()
			{
				if (!this.isWindowOpen)
				{
					this.SetFaderActive(false);
					return;
				}
				Transform x = this.topWindow.transform.parent;
				if (x == null)
				{
					return;
				}
				this.SetFaderActive(true);
				this.fader.transform.SetAsLastSibling();
				int siblingIndex = this.topWindow.transform.GetSiblingIndex();
				this.fader.transform.SetSiblingIndex(siblingIndex);
			}

			private void FocusTopWindow()
			{
				if (this.topWindow == null)
				{
					return;
				}
				this.topWindow.TakeInputFocus();
			}

			private void SetFaderActive(bool state)
			{
				this.fader.SetActive(state);
			}

			private Window InstantiateWindow(string name, int width, int height)
			{
				if (string.IsNullOrEmpty(name))
				{
					name = "Window";
				}
				GameObject gameObject = UITools.InstantiateGUIObject<Window>(this.windowPrefab, this.parent, name);
				if (gameObject == null)
				{
					return null;
				}
				Window component = gameObject.GetComponent<Window>();
				if (component != null)
				{
					component.Initialize(this.GetNewId(), new Func<int, bool>(this.IsFocused));
					this.windows.Add(component);
					component.SetSize(width, height);
				}
				return component;
			}

			private Window InstantiateWindow(string name, GameObject windowPrefab)
			{
				if (string.IsNullOrEmpty(name))
				{
					name = "Window";
				}
				if (windowPrefab == null)
				{
					return null;
				}
				GameObject gameObject = UITools.InstantiateGUIObject<Window>(windowPrefab, this.parent, name);
				if (gameObject == null)
				{
					return null;
				}
				Window component = gameObject.GetComponent<Window>();
				if (component != null)
				{
					component.Initialize(this.GetNewId(), new Func<int, bool>(this.IsFocused));
					this.windows.Add(component);
				}
				return component;
			}

			private void DestroyWindow(Window window)
			{
				if (window == null)
				{
					return;
				}
				UnityEngine.Object.Destroy(window.gameObject);
			}

			private int GetNewId()
			{
				int result = this.idCounter;
				this.idCounter++;
				return result;
			}

			public void ClearCompletely()
			{
				this.CloseAll();
				if (this.fader != null)
				{
					UnityEngine.Object.Destroy(this.fader);
				}
			}
		}

		private const float blockInputOnFocusTimeout = 0.1f;

		private const string buttonIdentifier_playerSelection = "PlayerSelection";

		private const string buttonIdentifier_removeController = "RemoveController";

		private const string buttonIdentifier_assignController = "AssignController";

		private const string buttonIdentifier_calibrateController = "CalibrateController";

		private const string buttonIdentifier_editInputBehaviors = "EditInputBehaviors";

		private const string buttonIdentifier_mapCategorySelection = "MapCategorySelection";

		private const string buttonIdentifier_assignedControllerSelection = "AssignedControllerSelection";

		private const string buttonIdentifier_done = "Done";

		private const string buttonIdentifier_restoreDefaults = "RestoreDefaults";

		[SerializeField, Tooltip("Must be assigned a Rewired Input Manager scene object or prefab.")]
		private InputManager _rewiredInputManager;

		[SerializeField, Tooltip("Open the control mapping screen immediately on start. Mainly used for testing.")]
		private bool _openOnStart;

		[SerializeField, Tooltip("The Layout of the Keyboard Maps to be displayed.")]
		private int _keyboardMapDefaultLayout;

		[SerializeField, Tooltip("The Layout of the Mouse Maps to be displayed.")]
		private int _mouseMapDefaultLayout;

		[SerializeField, Tooltip("The Layout of the Mouse Maps to be displayed.")]
		private int _joystickMapDefaultLayout;

		[SerializeField]
		private ControlMapper.MappingSet[] _mappingSets = new ControlMapper.MappingSet[]
		{
			ControlMapper.MappingSet.Default
		};

		[SerializeField, Tooltip("Display a selectable list of Players. If your game only supports 1 player, you can disable this.")]
		private bool _showPlayers = true;

		[SerializeField, Tooltip("Display the Controller column for input mapping.")]
		private bool _showControllers = true;

		[SerializeField, Tooltip("Display the Keyboard column for input mapping.")]
		private bool _showKeyboard = true;

		[SerializeField, Tooltip("Display the Mouse column for input mapping.")]
		private bool _showMouse = true;

		[SerializeField, Tooltip("The maximum number of controllers allowed to be assigned to a Player. If set to any value other than 1, a selectable list of currently-assigned controller will be displayed to the user. [0 = infinite]")]
		private int _maxControllersPerPlayer = 1;

		[SerializeField, Tooltip("Display section labels for each Action Category in the input field grid. Only applies if Action Categories are used to display the Action list.")]
		private bool _showActionCategoryLabels;

		[SerializeField, Tooltip("The number of input fields to display for the keyboard. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		private int _keyboardInputFieldCount = 2;

		[SerializeField, Tooltip("The number of input fields to display for the mouse. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		private int _mouseInputFieldCount = 1;

		[SerializeField, Tooltip("The number of input fields to display for joysticks. If you want to support alternate mappings on the same device, set this to 2 or more.")]
		private int _controllerInputFieldCount = 1;

		[SerializeField, Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to make the conflicting assignment anyway.")]
		private bool _allowElementAssignmentConflicts;

		[SerializeField, Tooltip("The width in relative pixels of the Action label column.")]
		private int _actionLabelWidth = 200;

		[SerializeField, Tooltip("The width in relative pixels of the Keyboard column.")]
		private int _keyboardColMaxWidth = 360;

		[SerializeField, Tooltip("The width in relative pixels of the Mouse column.")]
		private int _mouseColMaxWidth = 200;

		[SerializeField, Tooltip("The width in relative pixels of the Controller column.")]
		private int _controllerColMaxWidth = 200;

		[SerializeField, Tooltip("The height in relative pixels of the input grid button rows.")]
		private int _inputRowHeight = 40;

		[SerializeField, Tooltip("The width in relative pixels of spacing between columns.")]
		private int _inputColumnSpacing = 40;

		[SerializeField, Tooltip("The height in relative pixels of the space between Action Category sections. Only applicable if Show Action Category Labels is checked.")]
		private int _inputRowCategorySpacing = 20;

		[SerializeField, Tooltip("The width in relative pixels of the invert toggle buttons.")]
		private int _invertToggleWidth = 40;

		[SerializeField, Tooltip("The width in relative pixels of generated popup windows.")]
		private int _defaultWindowWidth = 500;

		[SerializeField, Tooltip("The height in relative pixels of generated popup windows.")]
		private int _defaultWindowHeight = 400;

		[SerializeField, Tooltip("The time in seconds the user has to press an element on a controller when assigning a controller to a Player. If this time elapses with no user input a controller, the assignment will be canceled.")]
		private float _controllerAssignmentTimeout = 5f;

		[SerializeField, Tooltip("The time in seconds the user has to press an element on a controller while waiting for axes to be centered before assigning input.")]
		private float _preInputAssignmentTimeout = 5f;

		[SerializeField, Tooltip("The time in seconds the user has to press an element on a controller when assigning input. If this time elapses with no user input on the target controller, the assignment will be canceled.")]
		private float _inputAssignmentTimeout = 5f;

		[SerializeField, Tooltip("The time in seconds the user has to press an element on a controller during calibration.")]
		private float _axisCalibrationTimeout = 5f;

		[SerializeField, Tooltip("If checked, mouse X-axis movement will always be ignored during input assignment. Check this if you don't want the horizontal mouse axis to be user-assignable to any Actions.")]
		private bool _ignoreMouseXAxisAssignment = true;

		[SerializeField, Tooltip("If checked, mouse Y-axis movement will always be ignored during input assignment. Check this if you don't want the vertical mouse axis to be user-assignable to any Actions.")]
		private bool _ignoreMouseYAxisAssignment = true;

		[SerializeField, Tooltip("An Action that when activated will alternately close or open the main screen as long as no popup windows are open.")]
		private int _screenToggleAction = -1;

		[SerializeField, Tooltip("An Action that when activated will open the main screen if it is closed.")]
		private int _screenOpenAction = -1;

		[SerializeField, Tooltip("An Action that when activated will close the main screen as long as no popup windows are open.")]
		private int _screenCloseAction = -1;

		[SerializeField, Tooltip("An Action that when activated will cancel and close any open popup window. Use with care because the element assigned to this Action can never be mapped by the user (because it would just cancel his assignment).")]
		private int _universalCancelAction = -1;

		[SerializeField, Tooltip("If enabled, Universal Cancel will also close the main screen if pressed when no windows are open.")]
		private bool _universalCancelClosesScreen = true;

		[SerializeField, Tooltip("If checked, controls will be displayed which will allow the user to customize certain Input Behavior settings.")]
		private bool _showInputBehaviorSettings;

		[SerializeField, Tooltip("Customizable settings for user-modifiable Input Behaviors. This can be used for settings like Mouse Look Sensitivity.")]
		private ControlMapper.InputBehaviorSettings[] _inputBehaviorSettings;

		[SerializeField, Tooltip("If enabled, UI elements will be themed based on the settings in Theme Settings.")]
		private bool _useThemeSettings = true;

		[SerializeField, Tooltip("Must be assigned a ThemeSettings object. Used to theme UI elements.")]
		private ThemeSettings _themeSettings;

		[SerializeField, Tooltip("Must be assigned a LanguageData object. Used to retrieve language entries for UI elements.")]
		private LanguageData _language;

		[SerializeField, Tooltip("A list of prefabs. You should not have to modify this.")]
		private ControlMapper.Prefabs prefabs;

		[SerializeField, Tooltip("A list of references to elements in the hierarchy. You should not have to modify this.")]
		private ControlMapper.References references;

		[SerializeField, Tooltip("Show the label for the Players button group?")]
		private bool _showPlayersGroupLabel = true;

		[SerializeField, Tooltip("Show the label for the Controller button group?")]
		private bool _showControllerGroupLabel = true;

		[SerializeField, Tooltip("Show the label for the Assigned Controllers button group?")]
		private bool _showAssignedControllersGroupLabel = true;

		[SerializeField, Tooltip("Show the label for the Settings button group?")]
		private bool _showSettingsGroupLabel = true;

		[SerializeField, Tooltip("Show the label for the Map Categories button group?")]
		private bool _showMapCategoriesGroupLabel = true;

		[SerializeField, Tooltip("Show the label for the current controller name?")]
		private bool _showControllerNameLabel = true;

		[SerializeField, Tooltip("Show the Assigned Controllers group? If joystick auto-assignment is enabled in the Rewired Input Manager and the max joysticks per player is set to any value other than 1, the Assigned Controllers group will always be displayed.")]
		private bool _showAssignedControllers = true;

		private static ControlMapper Instance;

		private bool inputEventsInitialized;

		private bool initialized;

		private int playerCount;

		private ControlMapper.InputGrid inputGrid;

		private ControlMapper.WindowManager windowManager;

		private int currentPlayerId;

		private int currentMapCategoryId;

		private List<ControlMapper.GUIButton> playerButtons;

		private List<ControlMapper.GUIButton> mapCategoryButtons;

		private List<ControlMapper.GUIButton> assignedControllerButtons;

		private ControlMapper.GUIButton assignedControllerButtonsPlaceholder;

		private GameObject canvas;

		private GameObject lastUISelection;

		private int currentJoystickId = -1;

		private float blockInputOnFocusEndTime;

		private ControlMapper.InputMapping pendingInputMapping;

		private ControlMapper.AxisCalibrator pendingAxisCalibration;

		private Action<InputFieldInfo> inputFieldActivatedDelegate;

		private Action<ToggleInfo, bool> inputFieldInvertToggleStateChangedDelegate;

		public event Action ScreenClosedEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ScreenClosedEvent = (Action)Delegate.Combine(this.ScreenClosedEvent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ScreenClosedEvent = (Action)Delegate.Remove(this.ScreenClosedEvent, value);
			}
		}

		public event Action ScreenOpenedEvent
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ScreenOpenedEvent = (Action)Delegate.Combine(this.ScreenOpenedEvent, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ScreenOpenedEvent = (Action)Delegate.Remove(this.ScreenOpenedEvent, value);
			}
		}

		public InputManager rewiredInputManager
		{
			get
			{
				return this._rewiredInputManager;
			}
			set
			{
				this._rewiredInputManager = value;
				this.InspectorPropertyChanged();
			}
		}

		public int keyboardMapDefaultLayout
		{
			get
			{
				return this._keyboardMapDefaultLayout;
			}
			set
			{
				this._keyboardMapDefaultLayout = value;
				this.InspectorPropertyChanged();
			}
		}

		public int mouseMapDefaultLayout
		{
			get
			{
				return this._mouseMapDefaultLayout;
			}
			set
			{
				this._mouseMapDefaultLayout = value;
				this.InspectorPropertyChanged();
			}
		}

		public int joystickMapDefaultLayout
		{
			get
			{
				return this._joystickMapDefaultLayout;
			}
			set
			{
				this._joystickMapDefaultLayout = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showPlayers
		{
			get
			{
				return this._showPlayers && ReInput.players.playerCount > 1;
			}
			set
			{
				this._showPlayers = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showControllers
		{
			get
			{
				return this._showControllers;
			}
			set
			{
				this._showControllers = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showKeyboard
		{
			get
			{
				return this._showKeyboard;
			}
			set
			{
				this._showKeyboard = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showMouse
		{
			get
			{
				return this._showMouse;
			}
			set
			{
				this._showMouse = value;
				this.InspectorPropertyChanged();
			}
		}

		public int maxControllersPerPlayer
		{
			get
			{
				return this._maxControllersPerPlayer;
			}
			set
			{
				this._maxControllersPerPlayer = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showActionCategoryLabels
		{
			get
			{
				return this._showActionCategoryLabels;
			}
			set
			{
				this._showActionCategoryLabels = value;
				this.InspectorPropertyChanged();
			}
		}

		public int keyboardInputFieldCount
		{
			get
			{
				return this._keyboardInputFieldCount;
			}
			set
			{
				this._keyboardInputFieldCount = value;
				this.InspectorPropertyChanged();
			}
		}

		public int mouseInputFieldCount
		{
			get
			{
				return this._mouseInputFieldCount;
			}
			set
			{
				this._mouseInputFieldCount = value;
				this.InspectorPropertyChanged();
			}
		}

		public int controllerInputFieldCount
		{
			get
			{
				return this._controllerInputFieldCount;
			}
			set
			{
				this._controllerInputFieldCount = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool allowElementAssignmentConflicts
		{
			get
			{
				return this._allowElementAssignmentConflicts;
			}
			set
			{
				this._allowElementAssignmentConflicts = value;
				this.InspectorPropertyChanged();
			}
		}

		public int actionLabelWidth
		{
			get
			{
				return this._actionLabelWidth;
			}
			set
			{
				this._actionLabelWidth = value;
				this.InspectorPropertyChanged();
			}
		}

		public int keyboardColMaxWidth
		{
			get
			{
				return this._keyboardColMaxWidth;
			}
			set
			{
				this._keyboardColMaxWidth = value;
				this.InspectorPropertyChanged();
			}
		}

		public int mouseColMaxWidth
		{
			get
			{
				return this._mouseColMaxWidth;
			}
			set
			{
				this._mouseColMaxWidth = value;
				this.InspectorPropertyChanged();
			}
		}

		public int controllerColMaxWidth
		{
			get
			{
				return this._controllerColMaxWidth;
			}
			set
			{
				this._controllerColMaxWidth = value;
				this.InspectorPropertyChanged();
			}
		}

		public int inputRowHeight
		{
			get
			{
				return this._inputRowHeight;
			}
			set
			{
				this._inputRowHeight = value;
				this.InspectorPropertyChanged();
			}
		}

		public int inputColumnSpacing
		{
			get
			{
				return this._inputColumnSpacing;
			}
			set
			{
				this._inputColumnSpacing = value;
				this.InspectorPropertyChanged();
			}
		}

		public int inputRowCategorySpacing
		{
			get
			{
				return this._inputRowCategorySpacing;
			}
			set
			{
				this._inputRowCategorySpacing = value;
				this.InspectorPropertyChanged();
			}
		}

		public int invertToggleWidth
		{
			get
			{
				return this._invertToggleWidth;
			}
			set
			{
				this._invertToggleWidth = value;
				this.InspectorPropertyChanged();
			}
		}

		public int defaultWindowWidth
		{
			get
			{
				return this._defaultWindowWidth;
			}
			set
			{
				this._defaultWindowWidth = value;
				this.InspectorPropertyChanged();
			}
		}

		public int defaultWindowHeight
		{
			get
			{
				return this._defaultWindowHeight;
			}
			set
			{
				this._defaultWindowHeight = value;
				this.InspectorPropertyChanged();
			}
		}

		public float controllerAssignmentTimeout
		{
			get
			{
				return this._controllerAssignmentTimeout;
			}
			set
			{
				this._controllerAssignmentTimeout = value;
				this.InspectorPropertyChanged();
			}
		}

		public float preInputAssignmentTimeout
		{
			get
			{
				return this._preInputAssignmentTimeout;
			}
			set
			{
				this._preInputAssignmentTimeout = value;
				this.InspectorPropertyChanged();
			}
		}

		public float inputAssignmentTimeout
		{
			get
			{
				return this._inputAssignmentTimeout;
			}
			set
			{
				this._inputAssignmentTimeout = value;
				this.InspectorPropertyChanged();
			}
		}

		public float axisCalibrationTimeout
		{
			get
			{
				return this._axisCalibrationTimeout;
			}
			set
			{
				this._axisCalibrationTimeout = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool ignoreMouseXAxisAssignment
		{
			get
			{
				return this._ignoreMouseXAxisAssignment;
			}
			set
			{
				this._ignoreMouseXAxisAssignment = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool ignoreMouseYAxisAssignment
		{
			get
			{
				return this._ignoreMouseYAxisAssignment;
			}
			set
			{
				this._ignoreMouseYAxisAssignment = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool universalCancelClosesScreen
		{
			get
			{
				return this._universalCancelClosesScreen;
			}
			set
			{
				this._universalCancelClosesScreen = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showInputBehaviorSettings
		{
			get
			{
				return this._showInputBehaviorSettings;
			}
			set
			{
				this._showInputBehaviorSettings = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool useThemeSettings
		{
			get
			{
				return this._useThemeSettings;
			}
			set
			{
				this._useThemeSettings = value;
				this.InspectorPropertyChanged();
			}
		}

		public LanguageData language
		{
			get
			{
				return this._language;
			}
			set
			{
				this._language = value;
				if (this._language != null)
				{
					this._language.Initialize();
				}
				this.InspectorPropertyChanged();
			}
		}

		public bool showPlayersGroupLabel
		{
			get
			{
				return this._showPlayersGroupLabel;
			}
			set
			{
				this._showPlayersGroupLabel = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showControllerGroupLabel
		{
			get
			{
				return this._showControllerGroupLabel;
			}
			set
			{
				this._showControllerGroupLabel = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showAssignedControllersGroupLabel
		{
			get
			{
				return this._showAssignedControllersGroupLabel;
			}
			set
			{
				this._showAssignedControllersGroupLabel = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showSettingsGroupLabel
		{
			get
			{
				return this._showSettingsGroupLabel;
			}
			set
			{
				this._showSettingsGroupLabel = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showMapCategoriesGroupLabel
		{
			get
			{
				return this._showMapCategoriesGroupLabel;
			}
			set
			{
				this._showMapCategoriesGroupLabel = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showControllerNameLabel
		{
			get
			{
				return this._showControllerNameLabel;
			}
			set
			{
				this._showControllerNameLabel = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool showAssignedControllers
		{
			get
			{
				return this._showAssignedControllers;
			}
			set
			{
				this._showAssignedControllers = value;
				this.InspectorPropertyChanged();
			}
		}

		public bool isOpen
		{
			get
			{
				if (!this.initialized)
				{
					return this.references.canvas != null && this.references.canvas.gameObject.activeInHierarchy;
				}
				return this.canvas.activeInHierarchy;
			}
		}

		private bool isFocused
		{
			get
			{
				return this.initialized && !this.windowManager.isWindowOpen;
			}
		}

		private bool inputAllowed
		{
			get
			{
				return this.blockInputOnFocusEndTime <= Time.unscaledTime;
			}
		}

		private int inputGridColumnCount
		{
			get
			{
				int num = 1;
				if (this._showKeyboard)
				{
					num++;
				}
				if (this._showMouse)
				{
					num++;
				}
				if (this._showControllers)
				{
					num++;
				}
				return num;
			}
		}

		private int inputGridWidth
		{
			get
			{
				return this._actionLabelWidth + ((!this._showKeyboard) ? 0 : this._keyboardColMaxWidth) + ((!this._showMouse) ? 0 : this._mouseColMaxWidth) + ((!this._showControllers) ? 0 : this._controllerColMaxWidth) + (this.inputGridColumnCount - 1) * this._inputColumnSpacing;
			}
		}

		private Player currentPlayer
		{
			get
			{
				return ReInput.players.GetPlayer(this.currentPlayerId);
			}
		}

		private InputCategory currentMapCategory
		{
			get
			{
				return ReInput.mapping.GetMapCategory(this.currentMapCategoryId);
			}
		}

		private ControlMapper.MappingSet currentMappingSet
		{
			get
			{
				if (this.currentMapCategoryId < 0)
				{
					return null;
				}
				for (int i = 0; i < this._mappingSets.Length; i++)
				{
					if (this._mappingSets[i].mapCategoryId == this.currentMapCategoryId)
					{
						return this._mappingSets[i];
					}
				}
				return null;
			}
		}

		private Rewired.Joystick currentJoystick
		{
			get
			{
				return ReInput.controllers.GetJoystick(this.currentJoystickId);
			}
		}

		private bool isJoystickSelected
		{
			get
			{
				return this.currentJoystickId >= 0;
			}
		}

		private GameObject currentUISelection
		{
			get
			{
				return (!(EventSystem.current != null)) ? null : EventSystem.current.currentSelectedGameObject;
			}
		}

		private bool showSettings
		{
			get
			{
				return this._showInputBehaviorSettings && this._inputBehaviorSettings.Length > 0;
			}
		}

		private bool showMapCategories
		{
			get
			{
				return this._mappingSets != null && this._mappingSets.Length > 1;
			}
		}

		private void Awake()
		{
			this.PreInitialize();
			if (this.isOpen)
			{
				this.Initialize();
				this.Open(true);
			}
		}

		private void Start()
		{
			if (this._openOnStart)
			{
				this.Open(false);
			}
		}

		private void Update()
		{
			if (!this.isOpen)
			{
				return;
			}
			if (!this.initialized)
			{
				return;
			}
			this.CheckUISelection();
		}

		private void OnDestroy()
		{
			ReInput.ControllerConnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnJoystickConnected);
			ReInput.ControllerDisconnectedEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnJoystickDisconnected);
			ReInput.ControllerPreDisconnectEvent -= new Action<ControllerStatusChangedEventArgs>(this.OnJoystickPreDisconnect);
			this.UnsubscribeMenuControlInputEvents();
		}

		private void PreInitialize()
		{
			if (!ReInput.isReady)
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: Rewired has not been initialized! Are you missing a Rewired Input Manager in your scene?");
				return;
			}
			this.SubscribeMenuControlInputEvents();
		}

		private void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			if (!ReInput.isReady)
			{
				return;
			}
			if (this._rewiredInputManager == null)
			{
				this._rewiredInputManager = UnityEngine.Object.FindObjectOfType<InputManager>();
				if (this._rewiredInputManager == null)
				{
					UnityEngine.Debug.LogError("Rewired Control Mapper: A Rewired Input Manager was not assigned in the inspector or found in the current scene! Control Mapper will not function.");
					return;
				}
			}
			if (ControlMapper.Instance != null)
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: Only one ControlMapper can exist at one time!");
				return;
			}
			ControlMapper.Instance = this;
			if (this.prefabs == null || !this.prefabs.Check())
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: All prefabs must be assigned in the inspector!");
				return;
			}
			if (this.references == null || !this.references.Check())
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: All references must be assigned in the inspector!");
				return;
			}
			this.references.inputGridLayoutElement = this.references.inputGridContainer.GetComponent<LayoutElement>();
			if (this.references.inputGridLayoutElement == null)
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: InputGridContainer is missing LayoutElement component!");
				return;
			}
			if (this._showKeyboard && this._keyboardInputFieldCount < 1)
			{
				UnityEngine.Debug.LogWarning("Rewired Control Mapper: Keyboard Input Fields must be at least 1!");
				this._keyboardInputFieldCount = 1;
			}
			if (this._showMouse && this._mouseInputFieldCount < 1)
			{
				UnityEngine.Debug.LogWarning("Rewired Control Mapper: Mouse Input Fields must be at least 1!");
				this._mouseInputFieldCount = 1;
			}
			if (this._showControllers && this._controllerInputFieldCount < 1)
			{
				UnityEngine.Debug.LogWarning("Rewired Control Mapper: Controller Input Fields must be at least 1!");
				this._controllerInputFieldCount = 1;
			}
			if (this._maxControllersPerPlayer < 0)
			{
				UnityEngine.Debug.LogWarning("Rewired Control Mapper: Max Controllers Per Player must be at least 0 (no limit)!");
				this._maxControllersPerPlayer = 0;
			}
			if (this._useThemeSettings && this._themeSettings == null)
			{
				UnityEngine.Debug.LogWarning("Rewired Control Mapper: To use theming, Theme Settings must be set in the inspector! Theming has been disabled.");
				this._useThemeSettings = false;
			}
			if (this._language == null)
			{
				UnityEngine.Debug.LogError("Rawired UI: Language must be set in the inspector!");
				return;
			}
			this._language.Initialize();
			this.inputFieldActivatedDelegate = new Action<InputFieldInfo>(this.OnInputFieldActivated);
			this.inputFieldInvertToggleStateChangedDelegate = new Action<ToggleInfo, bool>(this.OnInputFieldInvertToggleStateChanged);
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnJoystickConnected);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.OnJoystickDisconnected);
			ReInput.ControllerPreDisconnectEvent += new Action<ControllerStatusChangedEventArgs>(this.OnJoystickPreDisconnect);
			this.playerCount = ReInput.players.playerCount;
			this.canvas = this.references.canvas.gameObject;
			this.windowManager = new ControlMapper.WindowManager(this.prefabs.window, this.prefabs.fader, this.references.canvas.transform);
			this.playerButtons = new List<ControlMapper.GUIButton>();
			this.mapCategoryButtons = new List<ControlMapper.GUIButton>();
			this.assignedControllerButtons = new List<ControlMapper.GUIButton>();
			this.currentMapCategoryId = this._mappingSets[0].mapCategoryId;
			this.Draw();
			this.CreateInputGrid();
			this.CreateLayout();
			this.SubscribeFixedUISelectionEvents();
			this.initialized = true;
		}

		private void OnJoystickConnected(ControllerStatusChangedEventArgs args)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this._showControllers)
			{
				return;
			}
			this.ClearVarsOnJoystickChange();
			this.ForceRefresh();
		}

		private void OnJoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this._showControllers)
			{
				return;
			}
			this.ClearVarsOnJoystickChange();
			this.ForceRefresh();
		}

		private void OnJoystickPreDisconnect(ControllerStatusChangedEventArgs args)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this._showControllers)
			{
				return;
			}
		}

		public void OnButtonActivated(ButtonInfo buttonInfo)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.inputAllowed)
			{
				return;
			}
			string identifier = buttonInfo.identifier;
			switch (identifier)
			{
			case "PlayerSelection":
				this.OnPlayerSelected(buttonInfo.intData, true);
				break;
			case "AssignedControllerSelection":
				this.OnControllerSelected(buttonInfo.intData);
				break;
			case "RemoveController":
				this.OnRemoveCurrentController();
				break;
			case "AssignController":
				this.ShowAssignControllerWindow();
				break;
			case "CalibrateController":
				this.ShowCalibrateControllerWindow();
				break;
			case "EditInputBehaviors":
				this.ShowEditInputBehaviorsWindow();
				break;
			case "MapCategorySelection":
				this.OnMapCategorySelected(buttonInfo.intData, true);
				break;
			case "Done":
				this.Close(true);
				break;
			case "RestoreDefaults":
				this.OnRestoreDefaults();
				break;
			}
		}

		public void OnInputFieldActivated(InputFieldInfo fieldInfo)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.inputAllowed)
			{
				return;
			}
			if (this.currentPlayer == null)
			{
				return;
			}
			InputAction action = ReInput.mapping.GetAction(fieldInfo.actionId);
			if (action == null)
			{
				return;
			}
			string actionName;
			if (action.type == InputActionType.Button)
			{
				actionName = action.descriptiveName;
			}
			else
			{
				if (action.type != InputActionType.Axis)
				{
					throw new NotImplementedException();
				}
				if (fieldInfo.axisRange == AxisRange.Full)
				{
					actionName = action.descriptiveName;
				}
				else if (fieldInfo.axisRange == AxisRange.Positive)
				{
					if (string.IsNullOrEmpty(action.positiveDescriptiveName))
					{
						actionName = action.descriptiveName + " +";
					}
					else
					{
						actionName = action.positiveDescriptiveName;
					}
				}
				else
				{
					if (fieldInfo.axisRange != AxisRange.Negative)
					{
						throw new NotImplementedException();
					}
					if (string.IsNullOrEmpty(action.negativeDescriptiveName))
					{
						actionName = action.descriptiveName + " -";
					}
					else
					{
						actionName = action.negativeDescriptiveName;
					}
				}
			}
			ControllerMap controllerMap = this.GetControllerMap(fieldInfo.controllerType);
			if (controllerMap == null)
			{
				return;
			}
			ActionElementMap actionElementMap = (fieldInfo.actionElementMapId < 0) ? null : controllerMap.GetElementMap(fieldInfo.actionElementMapId);
			if (actionElementMap != null)
			{
				this.ShowBeginElementAssignmentReplacementWindow(fieldInfo, action, controllerMap, actionElementMap, actionName);
			}
			else
			{
				this.ShowCreateNewElementAssignmentWindow(fieldInfo, action, controllerMap, actionName);
			}
		}

		public void OnInputFieldInvertToggleStateChanged(ToggleInfo toggleInfo, bool newState)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.inputAllowed)
			{
				return;
			}
			this.SetActionAxisInverted(newState, toggleInfo.controllerType, toggleInfo.actionElementMapId);
		}

		private void OnPlayerSelected(int playerId, bool redraw)
		{
			if (!this.initialized)
			{
				return;
			}
			this.currentPlayerId = playerId;
			this.ClearVarsOnPlayerChange();
			if (redraw)
			{
				this.Redraw(true, true);
			}
		}

		private void OnControllerSelected(int joystickId)
		{
			if (!this.initialized)
			{
				return;
			}
			this.currentJoystickId = joystickId;
			this.Redraw(true, true);
		}

		private void OnRemoveCurrentController()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (this.currentJoystickId < 0)
			{
				return;
			}
			this.RemoveController(this.currentPlayer, this.currentJoystickId);
			this.ClearVarsOnJoystickChange();
			this.Redraw(false, false);
		}

		private void OnMapCategorySelected(int id, bool redraw)
		{
			if (!this.initialized)
			{
				return;
			}
			this.currentMapCategoryId = id;
			if (redraw)
			{
				this.Redraw(true, true);
			}
		}

		private void OnRestoreDefaults()
		{
			if (!this.initialized)
			{
				return;
			}
			this.ShowRestoreDefaultsWindow();
		}

		private void OnScreenToggleActionPressed(InputActionEventData data)
		{
			if (!this.isOpen)
			{
				this.Open();
				return;
			}
			if (!this.initialized)
			{
				return;
			}
			if (!this.isFocused)
			{
				return;
			}
			this.Close(true);
		}

		private void OnScreenOpenActionPressed(InputActionEventData data)
		{
			this.Open();
		}

		private void OnScreenCloseActionPressed(InputActionEventData data)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.isOpen)
			{
				return;
			}
			if (!this.isFocused)
			{
				return;
			}
			this.Close(true);
		}

		private void OnUniversalCancelActionPressed(InputActionEventData data)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.isOpen)
			{
				return;
			}
			if (this._universalCancelClosesScreen)
			{
				if (this.isFocused)
				{
					this.Close(true);
					return;
				}
			}
			else if (this.isFocused)
			{
				return;
			}
			this.CloseAllWindows();
		}

		private void OnWindowCancel(int windowId)
		{
			if (!this.initialized)
			{
				return;
			}
			if (windowId < 0)
			{
				return;
			}
			this.CloseWindow(windowId);
		}

		private void OnRemoveElementAssignment(int windowId, ControllerMap map, ActionElementMap aem)
		{
			if (map == null || aem == null)
			{
				return;
			}
			map.DeleteElementMap(aem.id);
			this.CloseWindow(windowId);
		}

		private void OnBeginElementAssignment(InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, string actionName)
		{
			if (fieldInfo == null || map == null)
			{
				return;
			}
			this.pendingInputMapping = new ControlMapper.InputMapping(actionName, fieldInfo, map, aem, fieldInfo.controllerType, fieldInfo.controllerId);
			switch (fieldInfo.controllerType)
			{
			case ControllerType.Keyboard:
				this.ShowElementAssignmentPollingWindow();
				break;
			case ControllerType.Mouse:
				this.ShowElementAssignmentPollingWindow();
				break;
			case ControllerType.Joystick:
				this.ShowElementAssignmentPrePollingWindow();
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private void OnControllerAssignmentConfirmed(int windowId, Player player, int controllerId)
		{
			if (windowId < 0 || player == null || controllerId < 0)
			{
				return;
			}
			this.AssignController(player, controllerId);
			this.CloseWindow(windowId);
		}

		private void OnMouseAssignmentConfirmed(int windowId, Player player)
		{
			if (windowId < 0 || player == null)
			{
				return;
			}
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != player)
				{
					players[i].controllers.hasMouse = false;
				}
			}
			player.controllers.hasMouse = true;
			this.CloseWindow(windowId);
		}

		private void OnElementAssignmentConflictReplaceConfirmed(int windowId, ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (this.currentPlayer == null || mapping == null)
			{
				return;
			}
			ElementAssignmentConflictCheck conflictCheck;
			if (!this.CreateConflictCheck(mapping, assignment, out conflictCheck))
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
				this.CloseWindow(windowId);
				return;
			}
			if (skipOtherPlayers)
			{
				ReInput.players.SystemPlayer.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
				this.currentPlayer.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
			}
			else
			{
				ReInput.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
			}
			mapping.map.ReplaceOrCreateElementMap(assignment);
			this.CloseWindow(windowId);
		}

		private void OnElementAssignmentAddConfirmed(int windowId, ControlMapper.InputMapping mapping, ElementAssignment assignment)
		{
			if (this.currentPlayer == null || mapping == null)
			{
				return;
			}
			mapping.map.ReplaceOrCreateElementMap(assignment);
			this.CloseWindow(windowId);
		}

		private void OnRestoreDefaultsConfirmed(int windowId)
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (this._showControllers)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
				}
				if (this._showKeyboard)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
				}
				if (this._showMouse)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
				}
			}
			this.CloseWindow(windowId);
		}

		private void OnAssignControllerWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (window.timer.finished)
			{
				this.CloseWindow(windowId);
				return;
			}
			ControllerPollingInfo controllerPollingInfo = ReInput.controllers.polling.PollAllControllersOfTypeForFirstElementDown(ControllerType.Joystick);
			if (!controllerPollingInfo.success)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				return;
			}
			if (ReInput.controllers.IsControllerAssigned(ControllerType.Joystick, controllerPollingInfo.controllerId) && !this.currentPlayer.controllers.ContainsController(ControllerType.Joystick, controllerPollingInfo.controllerId))
			{
				this.ShowControllerAssignmentConflictWindow(controllerPollingInfo.controllerId);
				return;
			}
			this.OnControllerAssignmentConfirmed(windowId, this.currentPlayer, controllerPollingInfo.controllerId);
		}

		private void OnElementAssignmentPrePollingWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingInputMapping == null)
			{
				return;
			}
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				ControllerPollingInfo controllerPollingInfo;
				switch (this.pendingInputMapping.controllerType)
				{
				case ControllerType.Keyboard:
				case ControllerType.Mouse:
					controllerPollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(this.pendingInputMapping.controllerType, 0);
					break;
				case ControllerType.Joystick:
					if (this.currentPlayer.controllers.joystickCount == 0)
					{
						return;
					}
					controllerPollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(this.pendingInputMapping.controllerType, this.currentJoystick.id);
					break;
				default:
					throw new NotImplementedException();
				}
				if (!controllerPollingInfo.success)
				{
					return;
				}
			}
			this.ShowElementAssignmentPollingWindow();
		}

		private void OnJoystickElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingInputMapping == null)
			{
				return;
			}
			if (window.timer.finished)
			{
				this.CloseWindow(windowId);
				return;
			}
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			if (this.currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, this.currentJoystick.id);
			if (!pollingInfo.success)
			{
				return;
			}
			ElementAssignment elementAssignment = this.pendingInputMapping.ToElementAssignment(pollingInfo);
			if (!this.HasElementAssignmentConflicts(this.currentPlayer, this.pendingInputMapping, elementAssignment, false))
			{
				this.pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
				this.CloseWindow(windowId);
			}
			else
			{
				this.ShowElementAssignmentConflictWindow(elementAssignment, false);
			}
		}

		private void OnKeyboardElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingInputMapping == null)
			{
				return;
			}
			if (window.timer.finished)
			{
				this.CloseWindow(windowId);
				return;
			}
			ControllerPollingInfo pollingInfo;
			bool flag;
			ModifierKeyFlags modifierKeyFlags;
			string text;
			this.PollKeyboardForAssignment(out pollingInfo, out flag, out modifierKeyFlags, out text);
			if (flag)
			{
				window.timer.Start(this._inputAssignmentTimeout);
			}
			window.SetContentText((!flag) ? Mathf.CeilToInt(window.timer.remaining).ToString() : string.Empty, 2);
			window.SetContentText(text, 1);
			if (!pollingInfo.success)
			{
				return;
			}
			ElementAssignment elementAssignment = this.pendingInputMapping.ToElementAssignment(pollingInfo, modifierKeyFlags);
			if (!this.HasElementAssignmentConflicts(this.currentPlayer, this.pendingInputMapping, elementAssignment, false))
			{
				this.pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
				this.CloseWindow(windowId);
			}
			else
			{
				this.ShowElementAssignmentConflictWindow(elementAssignment, false);
			}
		}

		private void OnMouseElementAssignmentPollingWindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingInputMapping == null)
			{
				return;
			}
			if (window.timer.finished)
			{
				this.CloseWindow(windowId);
				return;
			}
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			ControllerPollingInfo pollingInfo;
			if (this._ignoreMouseXAxisAssignment || this._ignoreMouseYAxisAssignment)
			{
				pollingInfo = default(ControllerPollingInfo);
				foreach (ControllerPollingInfo current in ReInput.controllers.polling.PollControllerForAllElementsDown(ControllerType.Mouse, 0))
				{
					if (current.elementType == ControllerElementType.Axis)
					{
						if (this._ignoreMouseXAxisAssignment && current.elementIndex == 0)
						{
							continue;
						}
						if (this._ignoreMouseYAxisAssignment && current.elementIndex == 1)
						{
							continue;
						}
					}
					pollingInfo = current;
					break;
				}
			}
			else
			{
				pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Mouse, 0);
			}
			if (!pollingInfo.success)
			{
				return;
			}
			ElementAssignment elementAssignment = this.pendingInputMapping.ToElementAssignment(pollingInfo);
			if (!this.HasElementAssignmentConflicts(this.currentPlayer, this.pendingInputMapping, elementAssignment, true))
			{
				this.pendingInputMapping.map.ReplaceOrCreateElementMap(elementAssignment);
				this.CloseWindow(windowId);
			}
			else
			{
				this.ShowElementAssignmentConflictWindow(elementAssignment, true);
			}
		}

		private void OnCalibrateAxisStep1WindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingAxisCalibration == null || !this.pendingAxisCalibration.isValid)
			{
				return;
			}
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				if (this.currentPlayer.controllers.joystickCount == 0)
				{
					return;
				}
				if (!this.pendingAxisCalibration.joystick.PollForFirstButtonDown().success)
				{
					return;
				}
			}
			this.pendingAxisCalibration.RecordZero();
			this.CloseWindow(windowId);
			this.ShowCalibrateAxisStep2Window();
		}

		private void OnCalibrateAxisStep2WindowUpdate(int windowId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.windowManager.GetWindow(windowId);
			if (windowId < 0)
			{
				return;
			}
			if (this.pendingAxisCalibration == null || !this.pendingAxisCalibration.isValid)
			{
				return;
			}
			if (!window.timer.finished)
			{
				window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
				this.pendingAxisCalibration.RecordMinMax();
				if (this.currentPlayer.controllers.joystickCount == 0)
				{
					return;
				}
				if (!this.pendingAxisCalibration.joystick.PollForFirstButtonDown().success)
				{
					return;
				}
			}
			this.EndAxisCalibration();
			this.CloseWindow(windowId);
		}

		private void ShowAssignControllerWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.SetUpdateCallback(new Action<int>(this.OnAssignControllerWindowUpdate));
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.assignControllerWindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.assignControllerWindowMessage);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
			window.timer.Start(this._controllerAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		private void ShowControllerAssignmentConflictWindow(int controllerId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			string otherPlayerName = string.Empty;
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != this.currentPlayer)
				{
					if (players[i].controllers.ContainsController(ControllerType.Joystick, controllerId))
					{
						otherPlayerName = players[i].descriptiveName;
						break;
					}
				}
			}
			Rewired.Joystick joystick = ReInput.controllers.GetJoystick(controllerId);
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.controllerAssignmentConflictWindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetControllerAssignmentConflictWindowMessage(joystick.name, otherPlayerName, this.currentPlayer.descriptiveName));
			UnityAction unityAction = delegate
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, Rewired.UI.UIAnchor.BottomLeft, Vector2.zero, this._language.yes, delegate
			{
				this.OnControllerAssignmentConfirmed(window.id, this.currentPlayer, controllerId);
			}, unityAction, true);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, Rewired.UI.UIAnchor.BottomRight, Vector2.zero, this._language.no, unityAction, unityAction, false);
			this.windowManager.Focus(window);
		}

		private void ShowBeginElementAssignmentReplacementWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, ActionElementMap aem, string actionName)
		{
			ControlMapper.GUIInputField gUIInputField = this.inputGrid.GetGUIInputField(this.currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
			if (gUIInputField == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), gUIInputField.GetLabel());
			UnityAction unityAction = delegate
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, Rewired.UI.UIAnchor.BottomLeft, Vector2.zero, this._language.replace, delegate
			{
				this.OnBeginElementAssignment(fieldInfo, map, aem, actionName);
			}, unityAction, true);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomCenter, Vector2.zero, this._language.remove, delegate
			{
				this.OnRemoveElementAssignment(window.id, map, aem);
			}, unityAction, false);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, Rewired.UI.UIAnchor.BottomRight, Vector2.zero, this._language.cancel, unityAction, unityAction, false);
			this.windowManager.Focus(window);
		}

		private void ShowCreateNewElementAssignmentWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, string actionName)
		{
			if (this.inputGrid.GetGUIInputField(this.currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData) == null)
			{
				return;
			}
			this.OnBeginElementAssignment(fieldInfo, map, null, actionName);
		}

		private void ShowElementAssignmentPrePollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this.pendingInputMapping.actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.elementAssignmentPrePollingWindowMessage);
			if (this.prefabs.centerStickGraphic != null)
			{
				window.AddContentImage(this.prefabs.centerStickGraphic, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
			window.SetUpdateCallback(new Action<int>(this.OnElementAssignmentPrePollingWindowUpdate));
			window.timer.Start(this._preInputAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		private void ShowElementAssignmentPollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			switch (this.pendingInputMapping.controllerType)
			{
			case ControllerType.Keyboard:
				this.ShowKeyboardElementAssignmentPollingWindow();
				break;
			case ControllerType.Mouse:
				if (this.currentPlayer.controllers.hasMouse)
				{
					this.ShowMouseElementAssignmentPollingWindow();
				}
				else
				{
					this.ShowMouseAssignmentConflictWindow();
				}
				break;
			case ControllerType.Joystick:
				this.ShowJoystickElementAssignmentPollingWindow();
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private void ShowJoystickElementAssignmentPollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this.pendingInputMapping.actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetJoystickElementAssignmentPollingWindowMessage(this.pendingInputMapping.actionName));
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
			window.SetUpdateCallback(new Action<int>(this.OnJoystickElementAssignmentPollingWindowUpdate));
			window.timer.Start(this._inputAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		private void ShowKeyboardElementAssignmentPollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this.pendingInputMapping.actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetKeyboardElementAssignmentPollingWindowMessage(this.pendingInputMapping.actionName));
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -(window.GetContentTextHeight(0) + 50f)), string.Empty);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
			window.SetUpdateCallback(new Action<int>(this.OnKeyboardElementAssignmentPollingWindowUpdate));
			window.timer.Start(this._inputAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		private void ShowMouseElementAssignmentPollingWindow()
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this.pendingInputMapping.actionName);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetMouseElementAssignmentPollingWindowMessage(this.pendingInputMapping.actionName));
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
			window.SetUpdateCallback(new Action<int>(this.OnMouseElementAssignmentPollingWindowUpdate));
			window.timer.Start(this._inputAssignmentTimeout);
			this.windowManager.Focus(window);
		}

		private void ShowElementAssignmentConflictWindow(ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (this.pendingInputMapping == null)
			{
				return;
			}
			bool flag = this.IsBlockingAssignmentConflict(this.pendingInputMapping, assignment, skipOtherPlayers);
			string text = (!flag) ? this._language.GetElementAlreadyInUseCanReplace(this.pendingInputMapping.elementName, this._allowElementAssignmentConflicts) : this._language.GetElementAlreadyInUseBlocked(this.pendingInputMapping.elementName);
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.elementAssignmentConflictWindowMessage);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), text);
			UnityAction unityAction = delegate
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			if (flag)
			{
				window.CreateButton(this.prefabs.fitButton, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomCenter, Vector2.zero, this._language.okay, unityAction, unityAction, true);
			}
			else
			{
				window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, Rewired.UI.UIAnchor.BottomLeft, Vector2.zero, this._language.replace, delegate
				{
					this.OnElementAssignmentConflictReplaceConfirmed(window.id, this.pendingInputMapping, assignment, skipOtherPlayers);
				}, unityAction, true);
				if (this._allowElementAssignmentConflicts)
				{
					window.CreateButton(this.prefabs.fitButton, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomCenter, Vector2.zero, this._language.add, delegate
					{
						this.OnElementAssignmentAddConfirmed(window.id, this.pendingInputMapping, assignment);
					}, unityAction, false);
				}
				window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, Rewired.UI.UIAnchor.BottomRight, Vector2.zero, this._language.cancel, unityAction, unityAction, false);
			}
			this.windowManager.Focus(window);
		}

		private void ShowMouseAssignmentConflictWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.OpenWindow(true);
			if (window == null)
			{
				return;
			}
			string otherPlayerName = string.Empty;
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i] != this.currentPlayer)
				{
					if (players[i].controllers.hasMouse)
					{
						otherPlayerName = players[i].descriptiveName;
						break;
					}
				}
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.mouseAssignmentConflictWindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetMouseAssignmentConflictWindowMessage(otherPlayerName, this.currentPlayer.descriptiveName));
			UnityAction unityAction = delegate
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, Rewired.UI.UIAnchor.BottomLeft, Vector2.zero, this._language.yes, delegate
			{
				this.OnMouseAssignmentConfirmed(window.id, this.currentPlayer);
			}, unityAction, true);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, Rewired.UI.UIAnchor.BottomRight, Vector2.zero, this._language.no, unityAction, unityAction, false);
			this.windowManager.Focus(window);
		}

		private void ShowCalibrateControllerWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (this.currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			CalibrationWindow calibrationWindow = this.OpenWindow(this.prefabs.calibrationWindow, "CalibrationWindow", true) as CalibrationWindow;
			if (calibrationWindow == null)
			{
				return;
			}
			Rewired.Joystick currentJoystick = this.currentJoystick;
			calibrationWindow.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.calibrateControllerWindowTitle);
			calibrationWindow.SetJoystick(this.currentPlayer.id, currentJoystick);
			calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Done, new Action<int>(this.CloseWindow));
			calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Calibrate, new Action<int>(this.StartAxisCalibration));
			calibrationWindow.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Cancel, new Action<int>(this.CloseWindow));
			this.windowManager.Focus(calibrationWindow);
		}

		private void ShowCalibrateAxisStep1Window()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.OpenWindow(false);
			if (window == null)
			{
				return;
			}
			if (this.pendingAxisCalibration == null)
			{
				return;
			}
			Rewired.Joystick joystick = this.pendingAxisCalibration.joystick;
			if (joystick.axisCount == 0)
			{
				return;
			}
			int axisIndex = this.pendingAxisCalibration.axisIndex;
			if (axisIndex < 0 || axisIndex >= joystick.axisCount)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.calibrateAxisStep1WindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetCalibrateAxisStep1WindowMessage(joystick.AxisElementIdentifiers[axisIndex].name));
			if (this.prefabs.centerStickGraphic != null)
			{
				window.AddContentImage(this.prefabs.centerStickGraphic, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
			window.SetUpdateCallback(new Action<int>(this.OnCalibrateAxisStep1WindowUpdate));
			window.timer.Start(this._axisCalibrationTimeout);
			this.windowManager.Focus(window);
		}

		private void ShowCalibrateAxisStep2Window()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			Window window = this.OpenWindow(false);
			if (window == null)
			{
				return;
			}
			if (this.pendingAxisCalibration == null)
			{
				return;
			}
			Rewired.Joystick joystick = this.pendingAxisCalibration.joystick;
			if (joystick.axisCount == 0)
			{
				return;
			}
			int axisIndex = this.pendingAxisCalibration.axisIndex;
			if (axisIndex < 0 || axisIndex >= joystick.axisCount)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.calibrateAxisStep2WindowTitle);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), this._language.GetCalibrateAxisStep2WindowMessage(joystick.AxisElementIdentifiers[axisIndex].name));
			if (this.prefabs.moveStickGraphic != null)
			{
				window.AddContentImage(this.prefabs.moveStickGraphic, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(this.prefabs.windowContentText, UIPivot.BottomCenter, Rewired.UI.UIAnchor.BottomHStretch, Vector2.zero, string.Empty);
			window.SetUpdateCallback(new Action<int>(this.OnCalibrateAxisStep2WindowUpdate));
			window.timer.Start(this._axisCalibrationTimeout);
			this.windowManager.Focus(window);
		}

		private void ShowEditInputBehaviorsWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (this._inputBehaviorSettings == null)
			{
				return;
			}
			InputBehaviorWindow inputBehaviorWindow = this.OpenWindow(this.prefabs.inputBehaviorsWindow, "EditInputBehaviorsWindow", true) as InputBehaviorWindow;
			if (inputBehaviorWindow == null)
			{
				return;
			}
			inputBehaviorWindow.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, this._language.inputBehaviorSettingsWindowTitle);
			inputBehaviorWindow.SetData(this.currentPlayer.id, this._inputBehaviorSettings);
			inputBehaviorWindow.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Done, new Action<int>(this.CloseWindow));
			inputBehaviorWindow.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Cancel, new Action<int>(this.CloseWindow));
			this.windowManager.Focus(inputBehaviorWindow);
		}

		private void ShowRestoreDefaultsWindow()
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			this.OpenModal(this._language.restoreDefaultsWindowTitle, this._language.restoreDefaultsWindowMessage, this._language.yes, new Action<int>(this.OnRestoreDefaultsConfirmed), this._language.no, new Action<int>(this.OnWindowCancel), true);
		}

		private void CreateInputGrid()
		{
			this.InitializeInputGrid();
			this.CreateHeaderLabels();
			this.CreateActionLabelColumn();
			this.CreateKeyboardInputFieldColumn();
			this.CreateMouseInputFieldColumn();
			this.CreateControllerInputFieldColumn();
			this.CreateInputActionLabels();
			this.CreateInputFields();
			this.inputGrid.HideAll();
			this.ResetInputGridScrollBar();
		}

		private void InitializeInputGrid()
		{
			if (this.inputGrid == null)
			{
				this.inputGrid = new ControlMapper.InputGrid();
			}
			else
			{
				this.inputGrid.ClearAll();
			}
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				ControlMapper.MappingSet mappingSet = this._mappingSets[i];
				if (mappingSet != null && mappingSet.isValid)
				{
					InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(mappingSet.mapCategoryId);
					if (mapCategory != null)
					{
						if (mapCategory.userAssignable)
						{
							this.inputGrid.AddMapCategory(mappingSet.mapCategoryId);
							if (mappingSet.actionListMode == ControlMapper.MappingSet.ActionListMode.ActionCategory)
							{
								IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
								for (int j = 0; j < actionCategoryIds.Count; j++)
								{
									int num = actionCategoryIds[j];
									InputCategory actionCategory = ReInput.mapping.GetActionCategory(num);
									if (actionCategory != null)
									{
										if (actionCategory.userAssignable)
										{
											this.inputGrid.AddActionCategory(mappingSet.mapCategoryId, num);
											foreach (InputAction current in ReInput.mapping.UserAssignableActionsInCategory(num))
											{
												if (current.type == InputActionType.Axis)
												{
													this.inputGrid.AddAction(mappingSet.mapCategoryId, current, AxisRange.Full);
													this.inputGrid.AddAction(mappingSet.mapCategoryId, current, AxisRange.Positive);
													this.inputGrid.AddAction(mappingSet.mapCategoryId, current, AxisRange.Negative);
												}
												else if (current.type == InputActionType.Button)
												{
													this.inputGrid.AddAction(mappingSet.mapCategoryId, current, AxisRange.Positive);
												}
											}
										}
									}
								}
							}
							else
							{
								IList<int> actionIds = mappingSet.actionIds;
								for (int k = 0; k < actionIds.Count; k++)
								{
									InputAction action = ReInput.mapping.GetAction(actionIds[k]);
									if (action != null)
									{
										if (action.type == InputActionType.Axis)
										{
											this.inputGrid.AddAction(mappingSet.mapCategoryId, action, AxisRange.Full);
											this.inputGrid.AddAction(mappingSet.mapCategoryId, action, AxisRange.Positive);
											this.inputGrid.AddAction(mappingSet.mapCategoryId, action, AxisRange.Negative);
										}
										else if (action.type == InputActionType.Button)
										{
											this.inputGrid.AddAction(mappingSet.mapCategoryId, action, AxisRange.Positive);
										}
									}
								}
							}
						}
					}
				}
			}
			this.references.inputGridInnerGroup.GetComponent<HorizontalLayoutGroup>().spacing = (float)this._inputColumnSpacing;
			this.references.inputGridLayoutElement.flexibleWidth = 0f;
			this.references.inputGridLayoutElement.preferredWidth = (float)this.inputGridWidth;
		}

		private void RefreshInputGridStructure()
		{
			if (this.currentMappingSet == null)
			{
				return;
			}
			this.inputGrid.HideAll();
			this.inputGrid.Show(this.currentMappingSet.mapCategoryId);
			this.references.inputGridInnerGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.inputGrid.GetColumnHeight(this.currentMappingSet.mapCategoryId));
		}

		private void CreateHeaderLabels()
		{
			this.references.inputGridHeader1 = this.CreateNewColumnGroup("ActionsHeader", this.references.inputGridHeadersGroup, this._actionLabelWidth).transform;
			this.CreateLabel(this.prefabs.inputGridHeaderLabel, this._language.actionColumnLabel, this.references.inputGridHeader1, Vector2.zero);
			if (this._showKeyboard)
			{
				this.references.inputGridHeader2 = this.CreateNewColumnGroup("KeybordHeader", this.references.inputGridHeadersGroup, this._keyboardColMaxWidth).transform;
				ControlMapper.GUILabel gUILabel = this.CreateLabel(this.prefabs.inputGridHeaderLabel, this._language.keyboardColumnLabel, this.references.inputGridHeader2, Vector2.zero);
				gUILabel.SetTextAlignment(TextAnchor.MiddleCenter);
			}
			if (this._showMouse)
			{
				this.references.inputGridHeader3 = this.CreateNewColumnGroup("MouseHeader", this.references.inputGridHeadersGroup, this._mouseColMaxWidth).transform;
				ControlMapper.GUILabel gUILabel = this.CreateLabel(this.prefabs.inputGridHeaderLabel, this._language.mouseColumnLabel, this.references.inputGridHeader3, Vector2.zero);
				gUILabel.SetTextAlignment(TextAnchor.MiddleCenter);
			}
			if (this._showControllers)
			{
				this.references.inputGridHeader4 = this.CreateNewColumnGroup("ControllerHeader", this.references.inputGridHeadersGroup, this._controllerColMaxWidth).transform;
				ControlMapper.GUILabel gUILabel = this.CreateLabel(this.prefabs.inputGridHeaderLabel, this._language.controllerColumnLabel, this.references.inputGridHeader4, Vector2.zero);
				gUILabel.SetTextAlignment(TextAnchor.MiddleCenter);
			}
		}

		private void CreateActionLabelColumn()
		{
			Transform transform = this.CreateNewColumnGroup("ActionLabelColumn", this.references.inputGridInnerGroup, this._actionLabelWidth).transform;
			this.references.inputGridActionColumn = transform;
		}

		private void CreateKeyboardInputFieldColumn()
		{
			if (!this._showKeyboard)
			{
				return;
			}
			this.CreateInputFieldColumn("KeyboardColumn", ControllerType.Keyboard, this._keyboardColMaxWidth, this._keyboardInputFieldCount, true);
		}

		private void CreateMouseInputFieldColumn()
		{
			if (!this._showMouse)
			{
				return;
			}
			this.CreateInputFieldColumn("MouseColumn", ControllerType.Mouse, this._mouseColMaxWidth, this._mouseInputFieldCount, false);
		}

		private void CreateControllerInputFieldColumn()
		{
			if (!this._showControllers)
			{
				return;
			}
			this.CreateInputFieldColumn("ControllerColumn", ControllerType.Joystick, this._controllerColMaxWidth, this._controllerInputFieldCount, false);
		}

		private void CreateInputFieldColumn(string name, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
		{
			Transform transform = this.CreateNewColumnGroup(name, this.references.inputGridInnerGroup, maxWidth).transform;
			switch (controllerType)
			{
			case ControllerType.Keyboard:
				this.references.inputGridKeyboardColumn = transform;
				break;
			case ControllerType.Mouse:
				this.references.inputGridMouseColumn = transform;
				break;
			case ControllerType.Joystick:
				this.references.inputGridControllerColumn = transform;
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private void CreateInputActionLabels()
		{
			Transform inputGridActionColumn = this.references.inputGridActionColumn;
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				ControlMapper.MappingSet mappingSet = this._mappingSets[i];
				if (mappingSet != null && mappingSet.isValid)
				{
					int num = 0;
					if (mappingSet.actionListMode == ControlMapper.MappingSet.ActionListMode.ActionCategory)
					{
						int num2 = 0;
						IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
						for (int j = 0; j < actionCategoryIds.Count; j++)
						{
							InputCategory actionCategory = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
							if (actionCategory != null)
							{
								if (actionCategory.userAssignable)
								{
									if (this.CountIEnumerable<InputAction>(ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id)) != 0)
									{
										if (this._showActionCategoryLabels)
										{
											if (num2 > 0)
											{
												num -= this._inputRowCategorySpacing;
											}
											ControlMapper.GUILabel gUILabel = this.CreateLabel(actionCategory.descriptiveName, inputGridActionColumn, new Vector2(0f, (float)num));
											gUILabel.SetFontStyle(FontStyle.Bold);
											gUILabel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
											this.inputGrid.AddActionCategoryLabel(mappingSet.mapCategoryId, actionCategory.id, gUILabel);
											num -= this._inputRowHeight;
										}
										foreach (InputAction current in ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id, true))
										{
											if (current.type == InputActionType.Axis)
											{
												ControlMapper.GUILabel gUILabel2 = this.CreateLabel(current.descriptiveName, inputGridActionColumn, new Vector2(0f, (float)num));
												gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
												this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, current.id, AxisRange.Full, gUILabel2);
												num -= this._inputRowHeight;
												string labelText = string.IsNullOrEmpty(current.positiveDescriptiveName) ? (current.descriptiveName + " +") : current.positiveDescriptiveName;
												gUILabel2 = this.CreateLabel(labelText, inputGridActionColumn, new Vector2(0f, (float)num));
												gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
												this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, current.id, AxisRange.Positive, gUILabel2);
												num -= this._inputRowHeight;
												string labelText2 = string.IsNullOrEmpty(current.negativeDescriptiveName) ? (current.descriptiveName + " -") : current.negativeDescriptiveName;
												gUILabel2 = this.CreateLabel(labelText2, inputGridActionColumn, new Vector2(0f, (float)num));
												gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
												this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, current.id, AxisRange.Negative, gUILabel2);
												num -= this._inputRowHeight;
											}
											else if (current.type == InputActionType.Button)
											{
												ControlMapper.GUILabel gUILabel2 = this.CreateLabel(current.descriptiveName, inputGridActionColumn, new Vector2(0f, (float)num));
												gUILabel2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
												this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, current.id, AxisRange.Positive, gUILabel2);
												num -= this._inputRowHeight;
											}
										}
										num2++;
									}
								}
							}
						}
					}
					else
					{
						IList<int> actionIds = mappingSet.actionIds;
						for (int k = 0; k < actionIds.Count; k++)
						{
							InputAction action = ReInput.mapping.GetAction(actionIds[k]);
							if (action != null)
							{
								if (action.userAssignable)
								{
									InputCategory actionCategory2 = ReInput.mapping.GetActionCategory(action.categoryId);
									if (actionCategory2 != null)
									{
										if (actionCategory2.userAssignable)
										{
											if (action.type == InputActionType.Axis)
											{
												ControlMapper.GUILabel gUILabel3 = this.CreateLabel(action.descriptiveName, inputGridActionColumn, new Vector2(0f, (float)num));
												gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
												this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, AxisRange.Full, gUILabel3);
												num -= this._inputRowHeight;
												gUILabel3 = this.CreateLabel(action.positiveDescriptiveName, inputGridActionColumn, new Vector2(0f, (float)num));
												gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
												this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, AxisRange.Positive, gUILabel3);
												num -= this._inputRowHeight;
												gUILabel3 = this.CreateLabel(action.negativeDescriptiveName, inputGridActionColumn, new Vector2(0f, (float)num));
												gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
												this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, AxisRange.Negative, gUILabel3);
												num -= this._inputRowHeight;
											}
											else if (action.type == InputActionType.Button)
											{
												ControlMapper.GUILabel gUILabel3 = this.CreateLabel(action.descriptiveName, inputGridActionColumn, new Vector2(0f, (float)num));
												gUILabel3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
												this.inputGrid.AddActionLabel(mappingSet.mapCategoryId, action.id, AxisRange.Positive, gUILabel3);
												num -= this._inputRowHeight;
											}
										}
									}
								}
							}
						}
					}
					this.inputGrid.SetColumnHeight(mappingSet.mapCategoryId, (float)(-(float)num));
				}
			}
		}

		private void CreateInputFields()
		{
			if (this._showControllers)
			{
				this.CreateInputFields(this.references.inputGridControllerColumn, ControllerType.Joystick, this._controllerColMaxWidth, this._controllerInputFieldCount, false);
			}
			if (this._showKeyboard)
			{
				this.CreateInputFields(this.references.inputGridKeyboardColumn, ControllerType.Keyboard, this._keyboardColMaxWidth, this._keyboardInputFieldCount, true);
			}
			if (this._showMouse)
			{
				this.CreateInputFields(this.references.inputGridMouseColumn, ControllerType.Mouse, this._mouseColMaxWidth, this._mouseInputFieldCount, false);
			}
		}

		private void CreateInputFields(Transform columnXform, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
		{
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				ControlMapper.MappingSet mappingSet = this._mappingSets[i];
				if (mappingSet != null && mappingSet.isValid)
				{
					int fieldWidth = maxWidth / cols;
					int num = 0;
					int num2 = 0;
					if (mappingSet.actionListMode == ControlMapper.MappingSet.ActionListMode.ActionCategory)
					{
						IList<int> actionCategoryIds = mappingSet.actionCategoryIds;
						for (int j = 0; j < actionCategoryIds.Count; j++)
						{
							InputCategory actionCategory = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
							if (actionCategory != null)
							{
								if (actionCategory.userAssignable)
								{
									if (this.CountIEnumerable<InputAction>(ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id)) != 0)
									{
										if (this._showActionCategoryLabels)
										{
											num -= ((num2 <= 0) ? this._inputRowHeight : (this._inputRowHeight + this._inputRowCategorySpacing));
										}
										foreach (InputAction current in ReInput.mapping.UserAssignableActionsInCategory(actionCategory.id, true))
										{
											if (current.type == InputActionType.Axis)
											{
												this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, current, AxisRange.Full, controllerType, cols, fieldWidth, ref num, disableFullAxis);
												this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, current, AxisRange.Positive, controllerType, cols, fieldWidth, ref num, false);
												this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, current, AxisRange.Negative, controllerType, cols, fieldWidth, ref num, false);
											}
											else if (current.type == InputActionType.Button)
											{
												this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, current, AxisRange.Positive, controllerType, cols, fieldWidth, ref num, false);
											}
											num2++;
										}
									}
								}
							}
						}
					}
					else
					{
						IList<int> actionIds = mappingSet.actionIds;
						for (int k = 0; k < actionIds.Count; k++)
						{
							InputAction action = ReInput.mapping.GetAction(actionIds[k]);
							if (action != null)
							{
								if (action.userAssignable)
								{
									InputCategory actionCategory2 = ReInput.mapping.GetActionCategory(action.categoryId);
									if (actionCategory2 != null)
									{
										if (actionCategory2.userAssignable)
										{
											if (action.type == InputActionType.Axis)
											{
												this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, AxisRange.Full, controllerType, cols, fieldWidth, ref num, disableFullAxis);
												this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref num, false);
												this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, AxisRange.Negative, controllerType, cols, fieldWidth, ref num, false);
											}
											else if (action.type == InputActionType.Button)
											{
												this.CreateInputFieldSet(columnXform, mappingSet.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref num, false);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void CreateInputFieldSet(Transform parent, int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int cols, int fieldWidth, ref int yPos, bool disableFullAxis)
		{
			GameObject gameObject = this.CreateNewGUIObject("FieldLayoutGroup", parent, new Vector2(0f, (float)yPos));
			HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(1f, 1f);
			component.pivot = new Vector2(0f, 1f);
			component.sizeDelta = Vector2.zero;
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)this._inputRowHeight);
			this.inputGrid.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, gameObject);
			for (int i = 0; i < cols; i++)
			{
				int num = (axisRange != AxisRange.Full) ? 0 : this._invertToggleWidth;
				ControlMapper.GUIInputField gUIInputField = this.CreateInputField(horizontalLayoutGroup.transform, Vector2.zero, string.Empty, action.id, axisRange, controllerType, i);
				gUIInputField.SetFirstChildObjectWidth(ControlMapper.LayoutElementSizeType.PreferredSize, fieldWidth - num);
				this.inputGrid.AddInputField(mapCategoryId, action, axisRange, controllerType, i, gUIInputField);
				if (axisRange == AxisRange.Full)
				{
					if (!disableFullAxis)
					{
						ControlMapper.GUIToggle gUIToggle = this.CreateToggle(this.prefabs.inputGridFieldInvertToggle, horizontalLayoutGroup.transform, Vector2.zero, string.Empty, action.id, axisRange, controllerType, i);
						gUIToggle.SetFirstChildObjectWidth(ControlMapper.LayoutElementSizeType.MinSize, num);
						gUIInputField.AddToggle(gUIToggle);
					}
					else
					{
						gUIInputField.SetInteractible(false, false, true);
					}
				}
			}
			yPos -= this._inputRowHeight;
		}

		private void PopulateInputFields()
		{
			this.inputGrid.InitializeFields(this.currentMapCategoryId);
			if (this.currentPlayer == null)
			{
				return;
			}
			this.inputGrid.SetFieldsActive(this.currentMapCategoryId, true);
			foreach (ControlMapper.InputActionSet current in this.inputGrid.GetActionSets(this.currentMapCategoryId))
			{
				if (this._showKeyboard)
				{
					ControllerType controllerType = ControllerType.Keyboard;
					int controllerId = 0;
					int layoutId = this._keyboardMapDefaultLayout;
					int maxFields = this._keyboardInputFieldCount;
					ControllerMap controllerMapOrCreateNew = this.GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
					this.PopulateInputFieldGroup(current, controllerMapOrCreateNew, controllerType, controllerId, maxFields);
				}
				if (this._showMouse)
				{
					ControllerType controllerType = ControllerType.Mouse;
					int controllerId = 0;
					int layoutId = this._mouseMapDefaultLayout;
					int maxFields = this._mouseInputFieldCount;
					ControllerMap controllerMapOrCreateNew2 = this.GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
					if (this.currentPlayer.controllers.hasMouse)
					{
						this.PopulateInputFieldGroup(current, controllerMapOrCreateNew2, controllerType, controllerId, maxFields);
					}
				}
				if (this.isJoystickSelected && this.currentPlayer.controllers.joystickCount > 0)
				{
					ControllerType controllerType = ControllerType.Joystick;
					int controllerId = this.currentJoystick.id;
					int layoutId = this._joystickMapDefaultLayout;
					int maxFields = this._controllerInputFieldCount;
					ControllerMap controllerMapOrCreateNew3 = this.GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
					this.PopulateInputFieldGroup(current, controllerMapOrCreateNew3, controllerType, controllerId, maxFields);
				}
				else
				{
					this.DisableInputFieldGroup(current, ControllerType.Joystick, this._controllerInputFieldCount);
				}
			}
		}

		private void PopulateInputFieldGroup(ControlMapper.InputActionSet actionSet, ControllerMap controllerMap, ControllerType controllerType, int controllerId, int maxFields)
		{
			if (controllerMap == null)
			{
				return;
			}
			int num = 0;
			this.inputGrid.SetFixedFieldData(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId);
			foreach (ActionElementMap current in controllerMap.ElementMapsWithAction(actionSet.actionId))
			{
				if (current.elementType == ControllerElementType.Button)
				{
					if (actionSet.axisRange == AxisRange.Full)
					{
						continue;
					}
					if (actionSet.axisRange == AxisRange.Positive)
					{
						if (current.axisContribution == Pole.Negative)
						{
							continue;
						}
					}
					else if (actionSet.axisRange == AxisRange.Negative && current.axisContribution == Pole.Positive)
					{
						continue;
					}
					this.inputGrid.PopulateField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, current.id, current.elementIdentifierName, false);
				}
				else if (current.elementType == ControllerElementType.Axis)
				{
					if (actionSet.axisRange == AxisRange.Full)
					{
						if (current.axisRange != AxisRange.Full)
						{
							continue;
						}
						this.inputGrid.PopulateField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, current.id, current.elementIdentifierName, current.invert);
					}
					else if (actionSet.axisRange == AxisRange.Positive)
					{
						if (current.axisRange == AxisRange.Full)
						{
							continue;
						}
						if (current.axisContribution == Pole.Negative)
						{
							continue;
						}
						this.inputGrid.PopulateField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, current.id, current.elementIdentifierName, false);
					}
					else if (actionSet.axisRange == AxisRange.Negative)
					{
						if (current.axisRange == AxisRange.Full)
						{
							continue;
						}
						if (current.axisContribution == Pole.Positive)
						{
							continue;
						}
						this.inputGrid.PopulateField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, num, current.id, current.elementIdentifierName, false);
					}
				}
				num++;
				if (num > maxFields)
				{
					break;
				}
			}
		}

		private void DisableInputFieldGroup(ControlMapper.InputActionSet actionSet, ControllerType controllerType, int fieldCount)
		{
			for (int i = 0; i < fieldCount; i++)
			{
				ControlMapper.GUIInputField gUIInputField = this.inputGrid.GetGUIInputField(this.currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, i);
				if (gUIInputField != null)
				{
					gUIInputField.SetInteractible(false, false);
				}
			}
		}

		private void ResetInputGridScrollBar()
		{
			this.references.inputGridInnerGroup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			this.references.inputGridVScrollbar.value = 1f;
		}

		private void CreateLayout()
		{
			this.references.playersGroup.gameObject.SetActive(this.showPlayers);
			this.references.controllerGroup.gameObject.SetActive(this._showControllers);
			this.references.assignedControllersGroup.gameObject.SetActive(this._showControllers && this.ShowAssignedControllers());
			this.references.settingsAndMapCategoriesGroup.gameObject.SetActive(this.showSettings || this.showMapCategories);
			this.references.settingsGroup.gameObject.SetActive(this.showSettings);
			this.references.mapCategoriesGroup.gameObject.SetActive(this.showMapCategories);
		}

		private void Draw()
		{
			this.DrawPlayersGroup();
			this.DrawControllersGroup();
			this.DrawSettingsGroup();
			this.DrawMapCategoriesGroup();
			this.DrawWindowButtonsGroup();
		}

		private void DrawPlayersGroup()
		{
			if (!this.showPlayers)
			{
				return;
			}
			this.references.playersGroup.labelText = this._language.playersGroupLabel;
			this.references.playersGroup.SetLabelActive(this._showPlayersGroupLabel);
			for (int i = 0; i < this.playerCount; i++)
			{
				Player player = ReInput.players.GetPlayer(i);
				if (player != null)
				{
					GameObject gameObject = UITools.InstantiateGUIObject<ButtonInfo>(this.prefabs.button, this.references.playersGroup.content, "Player" + i + "Button");
					ControlMapper.GUIButton gUIButton = new ControlMapper.GUIButton(gameObject);
					gUIButton.SetLabel(player.descriptiveName);
					gUIButton.SetButtonInfoData("PlayerSelection", player.id);
					gUIButton.SetOnClickCallback(new Action<ButtonInfo>(this.OnButtonActivated));
					gUIButton.buttonInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
					this.playerButtons.Add(gUIButton);
				}
			}
		}

		private void DrawControllersGroup()
		{
			if (!this._showControllers)
			{
				return;
			}
			this.references.controllerSettingsGroup.labelText = this._language.controllerSettingsGroupLabel;
			this.references.controllerSettingsGroup.SetLabelActive(this._showControllerGroupLabel);
			this.references.controllerNameLabel.gameObject.SetActive(this._showControllerNameLabel);
			this.references.controllerGroupLabelGroup.gameObject.SetActive(this._showControllerGroupLabel || this._showControllerNameLabel);
			if (this.ShowAssignedControllers())
			{
				this.references.assignedControllersGroup.labelText = this._language.assignedControllersGroupLabel;
				this.references.assignedControllersGroup.SetLabelActive(this._showAssignedControllersGroupLabel);
			}
			ButtonInfo component = this.references.removeControllerButton.GetComponent<ButtonInfo>();
			component.text.text = this._language.removeControllerButtonLabel;
			component = this.references.calibrateControllerButton.GetComponent<ButtonInfo>();
			component.text.text = this._language.calibrateControllerButtonLabel;
			component = this.references.assignControllerButton.GetComponent<ButtonInfo>();
			component.text.text = this._language.assignControllerButtonLabel;
			ControlMapper.GUIButton gUIButton = this.CreateButton(this._language.none, this.references.assignedControllersGroup.content, Vector2.zero);
			gUIButton.SetInteractible(false, false, true);
			this.assignedControllerButtonsPlaceholder = gUIButton;
		}

		private void DrawSettingsGroup()
		{
			if (!this.showSettings)
			{
				return;
			}
			this.references.settingsGroup.labelText = this._language.settingsGroupLabel;
			this.references.settingsGroup.SetLabelActive(this._showSettingsGroupLabel);
			ControlMapper.GUIButton gUIButton = this.CreateButton(this._language.inputBehaviorSettingsButtonLabel, this.references.settingsGroup.content, Vector2.zero);
			gUIButton.buttonInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
			gUIButton.SetButtonInfoData("EditInputBehaviors", 0);
			gUIButton.SetOnClickCallback(new Action<ButtonInfo>(this.OnButtonActivated));
		}

		private void DrawMapCategoriesGroup()
		{
			if (!this.showMapCategories)
			{
				return;
			}
			if (this._mappingSets == null)
			{
				return;
			}
			this.references.mapCategoriesGroup.labelText = this._language.mapCategoriesGroupLabel;
			this.references.mapCategoriesGroup.SetLabelActive(this._showMapCategoriesGroupLabel);
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				ControlMapper.MappingSet mappingSet = this._mappingSets[i];
				if (mappingSet != null)
				{
					InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(mappingSet.mapCategoryId);
					if (mapCategory != null)
					{
						GameObject gameObject = UITools.InstantiateGUIObject<ButtonInfo>(this.prefabs.button, this.references.mapCategoriesGroup.content, mapCategory.name + "Button");
						ControlMapper.GUIButton gUIButton = new ControlMapper.GUIButton(gameObject);
						gUIButton.SetLabel(mapCategory.descriptiveName);
						gUIButton.SetButtonInfoData("MapCategorySelection", mapCategory.id);
						gUIButton.SetOnClickCallback(new Action<ButtonInfo>(this.OnButtonActivated));
						gUIButton.buttonInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
						this.mapCategoryButtons.Add(gUIButton);
					}
				}
			}
		}

		private void DrawWindowButtonsGroup()
		{
			this.references.doneButton.GetComponent<ButtonInfo>().text.text = this._language.doneButtonLabel;
			this.references.restoreDefaultsButton.GetComponent<ButtonInfo>().text.text = this._language.restoreDefaultsButtonLabel;
		}

		private void Redraw(bool listsChanged, bool playTransitions)
		{
			this.RedrawPlayerGroup(playTransitions);
			this.RedrawControllerGroup();
			this.RedrawMapCategoriesGroup(playTransitions);
			this.RedrawInputGrid(listsChanged);
			if (this.currentUISelection == null || !this.currentUISelection.activeInHierarchy)
			{
				this.RestoreLastUISelection();
			}
		}

		private void RedrawPlayerGroup(bool playTransitions)
		{
			if (!this.showPlayers)
			{
				return;
			}
			for (int i = 0; i < this.playerButtons.Count; i++)
			{
				bool state = this.currentPlayerId != this.playerButtons[i].buttonInfo.intData;
				this.playerButtons[i].SetInteractible(state, playTransitions);
			}
		}

		private void RedrawControllerGroup()
		{
			int num = -1;
			this.references.controllerNameLabel.text = this._language.none;
			UITools.SetInteractable(this.references.removeControllerButton, false, false);
			UITools.SetInteractable(this.references.assignControllerButton, false, false);
			UITools.SetInteractable(this.references.calibrateControllerButton, false, false);
			if (this.ShowAssignedControllers())
			{
				foreach (ControlMapper.GUIButton current in this.assignedControllerButtons)
				{
					if (!(current.gameObject == null))
					{
						if (this.currentUISelection == current.gameObject)
						{
							num = current.buttonInfo.intData;
						}
						UnityEngine.Object.Destroy(current.gameObject);
					}
				}
				this.assignedControllerButtons.Clear();
				this.assignedControllerButtonsPlaceholder.SetActive(true);
			}
			Player player = ReInput.players.GetPlayer(this.currentPlayerId);
			if (player == null)
			{
				return;
			}
			if (this.ShowAssignedControllers())
			{
				if (player.controllers.joystickCount > 0)
				{
					this.assignedControllerButtonsPlaceholder.SetActive(false);
				}
				foreach (Rewired.Joystick current2 in player.controllers.Joysticks)
				{
					ControlMapper.GUIButton gUIButton = this.CreateButton(current2.name, this.references.assignedControllersGroup.content, Vector2.zero);
					gUIButton.SetButtonInfoData("AssignedControllerSelection", current2.id);
					gUIButton.SetOnClickCallback(new Action<ButtonInfo>(this.OnButtonActivated));
					gUIButton.buttonInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
					this.assignedControllerButtons.Add(gUIButton);
					if (current2.id == this.currentJoystickId)
					{
						gUIButton.SetInteractible(false, true);
					}
				}
				if (player.controllers.joystickCount > 0 && !this.isJoystickSelected)
				{
					this.currentJoystickId = player.controllers.Joysticks[0].id;
					this.assignedControllerButtons[0].SetInteractible(false, false);
				}
				if (num >= 0)
				{
					foreach (ControlMapper.GUIButton current3 in this.assignedControllerButtons)
					{
						if (current3.buttonInfo.intData == num)
						{
							this.SetUISelection(current3.gameObject);
							break;
						}
					}
				}
			}
			else if (player.controllers.joystickCount > 0 && !this.isJoystickSelected)
			{
				this.currentJoystickId = player.controllers.Joysticks[0].id;
			}
			if (this.isJoystickSelected && player.controllers.joystickCount > 0)
			{
				this.references.removeControllerButton.interactable = true;
				this.references.controllerNameLabel.text = this.currentJoystick.name;
				if (this.currentJoystick.axisCount > 0)
				{
					this.references.calibrateControllerButton.interactable = true;
				}
			}
			int joystickCount = player.controllers.joystickCount;
			int joystickCount2 = ReInput.controllers.joystickCount;
			int maxControllersPerPlayer = this.GetMaxControllersPerPlayer();
			bool flag = maxControllersPerPlayer == 0;
			if (joystickCount2 > 0 && joystickCount < joystickCount2 && (maxControllersPerPlayer == 1 || flag || joystickCount < maxControllersPerPlayer))
			{
				UITools.SetInteractable(this.references.assignControllerButton, true, false);
			}
		}

		private void RedrawMapCategoriesGroup(bool playTransitions)
		{
			if (!this.showMapCategories)
			{
				return;
			}
			for (int i = 0; i < this.mapCategoryButtons.Count; i++)
			{
				bool state = this.currentMapCategoryId != this.mapCategoryButtons[i].buttonInfo.intData;
				this.mapCategoryButtons[i].SetInteractible(state, playTransitions);
			}
		}

		private void RedrawInputGrid(bool listsChanged)
		{
			if (listsChanged)
			{
				this.RefreshInputGridStructure();
			}
			this.PopulateInputFields();
			if (listsChanged)
			{
				this.ResetInputGridScrollBar();
			}
		}

		private void ForceRefresh()
		{
			if (this.windowManager.isWindowOpen)
			{
				this.CloseAllWindows();
			}
			else
			{
				this.Redraw(false, false);
			}
		}

		private void CreateInputCategoryRow(ref int rowCount, InputCategory category)
		{
			this.CreateLabel(category.descriptiveName, this.references.inputGridActionColumn, new Vector2(0f, (float)(rowCount * this._inputRowHeight) * -1f));
			rowCount++;
		}

		private ControlMapper.GUILabel CreateLabel(string labelText, Transform parent, Vector2 offset)
		{
			return this.CreateLabel(this.prefabs.inputGridLabel, labelText, parent, offset);
		}

		private ControlMapper.GUILabel CreateLabel(GameObject prefab, string labelText, Transform parent, Vector2 offset)
		{
			GameObject gameObject = this.InstantiateGUIObject(prefab, parent, offset);
			Text componentInSelfOrChildren = UnityTools.GetComponentInSelfOrChildren<Text>(gameObject);
			if (componentInSelfOrChildren == null)
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: Label prefab is missing Text component!");
				return null;
			}
			componentInSelfOrChildren.text = labelText;
			return new ControlMapper.GUILabel(gameObject);
		}

		private ControlMapper.GUIButton CreateButton(string labelText, Transform parent, Vector2 offset)
		{
			ControlMapper.GUIButton gUIButton = new ControlMapper.GUIButton(this.InstantiateGUIObject(this.prefabs.button, parent, offset));
			gUIButton.SetLabel(labelText);
			return gUIButton;
		}

		private ControlMapper.GUIButton CreateFitButton(string labelText, Transform parent, Vector2 offset)
		{
			ControlMapper.GUIButton gUIButton = new ControlMapper.GUIButton(this.InstantiateGUIObject(this.prefabs.fitButton, parent, offset));
			gUIButton.SetLabel(labelText);
			return gUIButton;
		}

		private ControlMapper.GUIInputField CreateInputField(Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			ControlMapper.GUIInputField gUIInputField = this.CreateInputField(parent, offset);
			gUIInputField.SetLabel(string.Empty);
			gUIInputField.SetFieldInfoData(actionId, axisRange, controllerType, fieldIndex);
			gUIInputField.SetOnClickCallback(this.inputFieldActivatedDelegate);
			gUIInputField.fieldInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
			return gUIInputField;
		}

		private ControlMapper.GUIInputField CreateInputField(Transform parent, Vector2 offset)
		{
			return new ControlMapper.GUIInputField(this.InstantiateGUIObject(this.prefabs.inputGridFieldButton, parent, offset));
		}

		private ControlMapper.GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			ControlMapper.GUIToggle gUIToggle = this.CreateToggle(prefab, parent, offset);
			gUIToggle.SetToggleInfoData(actionId, axisRange, controllerType, fieldIndex);
			gUIToggle.SetOnSubmitCallback(this.inputFieldInvertToggleStateChangedDelegate);
			gUIToggle.toggleInfo.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
			return gUIToggle;
		}

		private ControlMapper.GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset)
		{
			return new ControlMapper.GUIToggle(this.InstantiateGUIObject(prefab, parent, offset));
		}

		private GameObject InstantiateGUIObject(GameObject prefab, Transform parent, Vector2 offset)
		{
			if (prefab == null)
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: Prefab is null!");
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			return this.InitializeNewGUIGameObject(gameObject, parent, offset);
		}

		private GameObject CreateNewGUIObject(string name, Transform parent, Vector2 offset)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = name;
			gameObject.AddComponent<RectTransform>();
			return this.InitializeNewGUIGameObject(gameObject, parent, offset);
		}

		private GameObject InitializeNewGUIGameObject(GameObject gameObject, Transform parent, Vector2 offset)
		{
			if (gameObject == null)
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: GameObject is null!");
				return null;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			if (component == null)
			{
				UnityEngine.Debug.LogError("Rewired Control Mapper: GameObject does not have a RectTransform component!");
				return gameObject;
			}
			if (parent != null)
			{
				component.SetParent(parent, false);
			}
			component.anchoredPosition = offset;
			return gameObject;
		}

		private GameObject CreateNewColumnGroup(string name, Transform parent, int maxWidth)
		{
			GameObject gameObject = this.CreateNewGUIObject(name, parent, Vector2.zero);
			this.inputGrid.AddGroup(gameObject);
			LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
			if (maxWidth >= 0)
			{
				layoutElement.preferredWidth = (float)maxWidth;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(1f, 0f);
			return gameObject;
		}

		private Window OpenWindow(bool closeOthers)
		{
			return this.OpenWindow(string.Empty, closeOthers);
		}

		private Window OpenWindow(string name, bool closeOthers)
		{
			if (closeOthers)
			{
				this.windowManager.CancelAll();
			}
			Window window = this.windowManager.OpenWindow(name, this._defaultWindowWidth, this._defaultWindowHeight);
			if (window == null)
			{
				return null;
			}
			this.ChildWindowOpened();
			return window;
		}

		private Window OpenWindow(GameObject windowPrefab, bool closeOthers)
		{
			return this.OpenWindow(windowPrefab, string.Empty, closeOthers);
		}

		private Window OpenWindow(GameObject windowPrefab, string name, bool closeOthers)
		{
			if (closeOthers)
			{
				this.windowManager.CancelAll();
			}
			Window window = this.windowManager.OpenWindow(windowPrefab, name);
			if (window == null)
			{
				return null;
			}
			this.ChildWindowOpened();
			return window;
		}

		private void OpenModal(string title, string message, string confirmText, Action<int> confirmAction, string cancelText, Action<int> cancelAction, bool closeOthers)
		{
			Window window = this.OpenWindow(closeOthers);
			if (window == null)
			{
				return;
			}
			window.CreateTitleText(this.prefabs.windowTitleText, Vector2.zero, title);
			window.AddContentText(this.prefabs.windowContentText, UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, new Vector2(0f, -100f), message);
			UnityAction unityAction = delegate
			{
				this.OnWindowCancel(window.id);
			};
			window.cancelCallback = unityAction;
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomLeft, Rewired.UI.UIAnchor.BottomLeft, Vector2.zero, confirmText, delegate
			{
				this.OnRestoreDefaultsConfirmed(window.id);
			}, unityAction, false);
			window.CreateButton(this.prefabs.fitButton, UIPivot.BottomRight, Rewired.UI.UIAnchor.BottomRight, Vector2.zero, cancelText, unityAction, unityAction, true);
			this.windowManager.Focus(window);
		}

		private void CloseWindow(int windowId)
		{
			if (!this.windowManager.isWindowOpen)
			{
				return;
			}
			this.windowManager.CloseWindow(windowId);
			this.ChildWindowClosed();
		}

		private void CloseTopWindow()
		{
			if (!this.windowManager.isWindowOpen)
			{
				return;
			}
			this.windowManager.CloseTop();
			this.ChildWindowClosed();
		}

		private void CloseAllWindows()
		{
			if (!this.windowManager.isWindowOpen)
			{
				return;
			}
			this.windowManager.CancelAll();
			this.ChildWindowClosed();
		}

		private void ChildWindowOpened()
		{
			if (!this.windowManager.isWindowOpen)
			{
				return;
			}
			this.SetIsFocused(false);
		}

		private void ChildWindowClosed()
		{
			if (this.windowManager.isWindowOpen)
			{
				return;
			}
			this.SetIsFocused(true);
		}

		private bool HasElementAssignmentConflicts(Player player, ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			if (player == null || mapping == null)
			{
				return false;
			}
			ElementAssignmentConflictCheck conflictCheck;
			if (!this.CreateConflictCheck(mapping, assignment, out conflictCheck))
			{
				return false;
			}
			if (skipOtherPlayers)
			{
				return ReInput.players.SystemPlayer.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck) || player.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck);
			}
			return ReInput.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck);
		}

		private bool IsBlockingAssignmentConflict(ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			ElementAssignmentConflictCheck conflictCheck;
			if (!this.CreateConflictCheck(mapping, assignment, out conflictCheck))
			{
				return false;
			}
			if (skipOtherPlayers)
			{
				foreach (ElementAssignmentConflictInfo current in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!current.isUserAssignable)
					{
						bool result = true;
						return result;
					}
				}
				foreach (ElementAssignmentConflictInfo current2 in this.currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!current2.isUserAssignable)
					{
						bool result = true;
						return result;
					}
				}
			}
			else
			{
				foreach (ElementAssignmentConflictInfo current3 in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
				{
					if (!current3.isUserAssignable)
					{
						bool result = true;
						return result;
					}
				}
			}
			return false;
		}

		[DebuggerHidden]
		private IEnumerable<ElementAssignmentConflictInfo> ElementAssignmentConflicts(Player player, ControlMapper.InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
		{
			ControlMapper.<ElementAssignmentConflicts>c__Iterator10F <ElementAssignmentConflicts>c__Iterator10F = new ControlMapper.<ElementAssignmentConflicts>c__Iterator10F();
			<ElementAssignmentConflicts>c__Iterator10F.player = player;
			<ElementAssignmentConflicts>c__Iterator10F.mapping = mapping;
			<ElementAssignmentConflicts>c__Iterator10F.assignment = assignment;
			<ElementAssignmentConflicts>c__Iterator10F.skipOtherPlayers = skipOtherPlayers;
			<ElementAssignmentConflicts>c__Iterator10F.<$>player = player;
			<ElementAssignmentConflicts>c__Iterator10F.<$>mapping = mapping;
			<ElementAssignmentConflicts>c__Iterator10F.<$>assignment = assignment;
			<ElementAssignmentConflicts>c__Iterator10F.<$>skipOtherPlayers = skipOtherPlayers;
			<ElementAssignmentConflicts>c__Iterator10F.<>f__this = this;
			ControlMapper.<ElementAssignmentConflicts>c__Iterator10F expr_48 = <ElementAssignmentConflicts>c__Iterator10F;
			expr_48.$PC = -2;
			return expr_48;
		}

		private bool CreateConflictCheck(ControlMapper.InputMapping mapping, ElementAssignment assignment, out ElementAssignmentConflictCheck conflictCheck)
		{
			if (mapping == null || this.currentPlayer == null)
			{
				conflictCheck = default(ElementAssignmentConflictCheck);
				return false;
			}
			conflictCheck = assignment.ToElementAssignmentConflictCheck();
			conflictCheck.playerId = this.currentPlayer.id;
			conflictCheck.controllerType = mapping.controllerType;
			conflictCheck.controllerId = mapping.controllerId;
			conflictCheck.controllerMapId = mapping.map.id;
			conflictCheck.controllerMapCategoryId = mapping.map.categoryId;
			if (mapping.aem != null)
			{
				conflictCheck.elementMapId = mapping.aem.id;
			}
			return true;
		}

		private void PollKeyboardForAssignment(out ControllerPollingInfo pollingInfo, out bool modifierKeyPressed, out ModifierKeyFlags modifierFlags, out string label)
		{
			pollingInfo = default(ControllerPollingInfo);
			label = string.Empty;
			modifierKeyPressed = false;
			modifierFlags = ModifierKeyFlags.None;
			int num = 0;
			ControllerPollingInfo controllerPollingInfo = default(ControllerPollingInfo);
			ControllerPollingInfo controllerPollingInfo2 = default(ControllerPollingInfo);
			ModifierKeyFlags modifierKeyFlags = ModifierKeyFlags.None;
			foreach (ControllerPollingInfo current in ReInput.controllers.Keyboard.PollForAllKeys())
			{
				KeyCode keyboardKey = current.keyboardKey;
				if (keyboardKey != KeyCode.AltGr)
				{
					if (Keyboard.IsModifierKey(current.keyboardKey))
					{
						if (num == 0)
						{
							controllerPollingInfo2 = current;
						}
						modifierKeyFlags |= Keyboard.KeyCodeToModifierKeyFlags(keyboardKey);
						num++;
					}
					else if (controllerPollingInfo.keyboardKey == KeyCode.None)
					{
						controllerPollingInfo = current;
					}
				}
			}
			if (controllerPollingInfo.keyboardKey == KeyCode.None)
			{
				if (num > 0)
				{
					modifierKeyPressed = true;
					if (num == 1)
					{
						if (ReInput.controllers.Keyboard.GetKeyTimePressed(controllerPollingInfo2.keyboardKey) > 1f)
						{
							pollingInfo = controllerPollingInfo2;
							return;
						}
						label = Keyboard.GetKeyName(controllerPollingInfo2.keyboardKey);
					}
					else
					{
						label = Keyboard.ModifierKeyFlagsToString(modifierKeyFlags);
					}
				}
				return;
			}
			if (!ReInput.controllers.Keyboard.GetKeyDown(controllerPollingInfo.keyboardKey))
			{
				return;
			}
			if (num == 0)
			{
				pollingInfo = controllerPollingInfo;
				return;
			}
			pollingInfo = controllerPollingInfo;
			modifierFlags = modifierKeyFlags;
		}

		private void StartAxisCalibration(int axisIndex)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			if (this.currentPlayer.controllers.joystickCount == 0)
			{
				return;
			}
			Rewired.Joystick currentJoystick = this.currentJoystick;
			if (axisIndex < 0 || axisIndex >= currentJoystick.axisCount)
			{
				return;
			}
			this.pendingAxisCalibration = new ControlMapper.AxisCalibrator(currentJoystick, axisIndex);
			this.ShowCalibrateAxisStep1Window();
		}

		private void EndAxisCalibration()
		{
			if (this.pendingAxisCalibration == null)
			{
				return;
			}
			this.pendingAxisCalibration.Commit();
			this.pendingAxisCalibration = null;
		}

		private void SetUISelection(GameObject selection)
		{
			if (EventSystem.current == null)
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(selection);
		}

		private void RestoreLastUISelection()
		{
			if (this.lastUISelection == null || !this.lastUISelection.activeInHierarchy)
			{
				this.SetDefaultUISelection();
				return;
			}
			this.SetUISelection(this.lastUISelection);
		}

		private void SetDefaultUISelection()
		{
			if (!this.isOpen)
			{
				return;
			}
			if (this.references.defaultSelection == null)
			{
				this.SetUISelection(null);
			}
			else
			{
				this.SetUISelection(this.references.defaultSelection.gameObject);
			}
		}

		private void SelectDefaultMapCategory(bool redraw)
		{
			this.currentMapCategoryId = this.GetDefaultMapCategoryId();
			this.OnMapCategorySelected(this.currentMapCategoryId, redraw);
			if (!this.showMapCategories)
			{
				return;
			}
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				if (ReInput.mapping.GetMapCategory(this._mappingSets[i].mapCategoryId) != null)
				{
					this.currentMapCategoryId = this._mappingSets[i].mapCategoryId;
					break;
				}
			}
			if (this.currentMapCategoryId < 0)
			{
				return;
			}
			for (int j = 0; j < this._mappingSets.Length; j++)
			{
				bool state = this._mappingSets[j].mapCategoryId != this.currentMapCategoryId;
				this.mapCategoryButtons[j].SetInteractible(state, false);
			}
		}

		private void CheckUISelection()
		{
			if (!this.isFocused)
			{
				return;
			}
			if (this.currentUISelection == null)
			{
				this.RestoreLastUISelection();
			}
		}

		private void OnUIElementSelected(GameObject selectedObject)
		{
			this.lastUISelection = selectedObject;
		}

		private void SetIsFocused(bool state)
		{
			this.references.mainCanvasGroup.interactable = state;
			if (state)
			{
				this.Redraw(false, false);
				this.RestoreLastUISelection();
				this.blockInputOnFocusEndTime = Time.unscaledTime + 0.1f;
			}
		}

		public void Toggle()
		{
			if (this.isOpen)
			{
				this.Close(true);
			}
			else
			{
				this.Open();
			}
		}

		public void Open()
		{
			this.Open(false);
		}

		private void Open(bool force)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}
			if (!this.initialized)
			{
				return;
			}
			if (!force && this.isOpen)
			{
				return;
			}
			this.Clear();
			this.canvas.SetActive(true);
			this.OnPlayerSelected(0, false);
			this.SelectDefaultMapCategory(false);
			this.SetDefaultUISelection();
			this.Redraw(true, false);
			if (this.ScreenOpenedEvent != null)
			{
				this.ScreenOpenedEvent();
			}
		}

		public void Close(bool save)
		{
			if (!this.initialized)
			{
				return;
			}
			if (!this.isOpen)
			{
				return;
			}
			if (save && ReInput.userDataStore != null)
			{
				ReInput.userDataStore.Save();
			}
			this.Clear();
			this.canvas.SetActive(false);
			this.SetUISelection(null);
			if (this.ScreenClosedEvent != null)
			{
				this.ScreenClosedEvent();
			}
		}

		private void Clear()
		{
			this.windowManager.CancelAll();
			this.lastUISelection = null;
			this.pendingInputMapping = null;
			this.pendingAxisCalibration = null;
		}

		private void ClearCompletely()
		{
			this.ClearSpawnedObjects();
			this.ClearAllVars();
		}

		private void ClearSpawnedObjects()
		{
			this.windowManager.ClearCompletely();
			this.inputGrid.ClearAll();
			foreach (ControlMapper.GUIButton current in this.playerButtons)
			{
				UnityEngine.Object.Destroy(current.gameObject);
			}
			this.playerButtons.Clear();
			foreach (ControlMapper.GUIButton current2 in this.mapCategoryButtons)
			{
				UnityEngine.Object.Destroy(current2.gameObject);
			}
			this.mapCategoryButtons.Clear();
			foreach (ControlMapper.GUIButton current3 in this.assignedControllerButtons)
			{
				UnityEngine.Object.Destroy(current3.gameObject);
			}
			this.assignedControllerButtons.Clear();
			if (this.assignedControllerButtonsPlaceholder != null)
			{
				UnityEngine.Object.Destroy(this.assignedControllerButtonsPlaceholder.gameObject);
				this.assignedControllerButtonsPlaceholder = null;
			}
		}

		private void ClearVarsOnPlayerChange()
		{
			this.currentJoystickId = -1;
		}

		private void ClearVarsOnJoystickChange()
		{
			this.currentJoystickId = -1;
		}

		private void ClearAllVars()
		{
			this.initialized = false;
			ControlMapper.Instance = null;
			this.playerCount = 0;
			this.inputGrid = null;
			this.windowManager = null;
			this.currentPlayerId = -1;
			this.currentMapCategoryId = -1;
			this.playerButtons = null;
			this.mapCategoryButtons = null;
			this.canvas = null;
			this.lastUISelection = null;
			this.currentJoystickId = -1;
			this.pendingInputMapping = null;
			this.pendingAxisCalibration = null;
			this.inputFieldActivatedDelegate = null;
			this.inputFieldInvertToggleStateChangedDelegate = null;
		}

		public void Reset()
		{
			if (!this.initialized)
			{
				return;
			}
			this.ClearCompletely();
			this.Initialize();
			if (this.isOpen)
			{
				this.Open(true);
			}
		}

		private void SetActionAxisInverted(bool state, ControllerType controllerType, int actionElementMapId)
		{
			if (this.currentPlayer == null)
			{
				return;
			}
			ControllerMapWithAxes controllerMapWithAxes = this.GetControllerMap(controllerType) as ControllerMapWithAxes;
			if (controllerMapWithAxes == null)
			{
				return;
			}
			ActionElementMap elementMap = controllerMapWithAxes.GetElementMap(actionElementMapId);
			if (elementMap == null)
			{
				return;
			}
			elementMap.invert = state;
		}

		private ControllerMap GetControllerMap(ControllerType type)
		{
			if (this.currentPlayer == null)
			{
				return null;
			}
			int controllerId = 0;
			switch (type)
			{
			case ControllerType.Keyboard:
				break;
			case ControllerType.Mouse:
				break;
			case ControllerType.Joystick:
				if (this.currentPlayer.controllers.joystickCount <= 0)
				{
					return null;
				}
				controllerId = this.currentJoystick.id;
				break;
			default:
				throw new NotImplementedException();
			}
			return this.currentPlayer.controllers.maps.GetFirstMapInCategory(type, controllerId, this.currentMapCategoryId);
		}

		private ControllerMap GetControllerMapOrCreateNew(ControllerType controllerType, int controllerId, int layoutId)
		{
			ControllerMap controllerMap = this.GetControllerMap(controllerType);
			if (controllerMap == null)
			{
				this.currentPlayer.controllers.maps.AddEmptyMap(controllerType, controllerId, this.currentMapCategoryId, layoutId);
				controllerMap = this.currentPlayer.controllers.maps.GetMap(controllerType, controllerId, this.currentMapCategoryId, layoutId);
			}
			return controllerMap;
		}

		private int CountIEnumerable<T>(IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				return 0;
			}
			IEnumerator<T> enumerator = enumerable.GetEnumerator();
			if (enumerator == null)
			{
				return 0;
			}
			int num = 0;
			while (enumerator.MoveNext())
			{
				num++;
			}
			return num;
		}

		private int GetDefaultMapCategoryId()
		{
			if (this._mappingSets.Length == 0)
			{
				return 0;
			}
			for (int i = 0; i < this._mappingSets.Length; i++)
			{
				if (ReInput.mapping.GetMapCategory(this._mappingSets[i].mapCategoryId) != null)
				{
					return this._mappingSets[i].mapCategoryId;
				}
			}
			return 0;
		}

		private void SubscribeFixedUISelectionEvents()
		{
			if (this.references.fixedSelectableUIElements == null)
			{
				return;
			}
			GameObject[] fixedSelectableUIElements = this.references.fixedSelectableUIElements;
			for (int i = 0; i < fixedSelectableUIElements.Length; i++)
			{
				GameObject gameObject = fixedSelectableUIElements[i];
				UIElementInfo component = UnityTools.GetComponent<UIElementInfo>(gameObject);
				if (!(component == null))
				{
					component.OnSelectedEvent += new Action<GameObject>(this.OnUIElementSelected);
				}
			}
		}

		private void SubscribeMenuControlInputEvents()
		{
			this.SubscribeRewiredInputEventAllPlayers(this._screenToggleAction, new Action<InputActionEventData>(this.OnScreenToggleActionPressed));
			this.SubscribeRewiredInputEventAllPlayers(this._screenOpenAction, new Action<InputActionEventData>(this.OnScreenOpenActionPressed));
			this.SubscribeRewiredInputEventAllPlayers(this._screenCloseAction, new Action<InputActionEventData>(this.OnScreenCloseActionPressed));
			this.SubscribeRewiredInputEventAllPlayers(this._universalCancelAction, new Action<InputActionEventData>(this.OnUniversalCancelActionPressed));
		}

		private void UnsubscribeMenuControlInputEvents()
		{
			this.UnsubscribeRewiredInputEventAllPlayers(this._screenToggleAction, new Action<InputActionEventData>(this.OnScreenToggleActionPressed));
			this.UnsubscribeRewiredInputEventAllPlayers(this._screenOpenAction, new Action<InputActionEventData>(this.OnScreenOpenActionPressed));
			this.UnsubscribeRewiredInputEventAllPlayers(this._screenCloseAction, new Action<InputActionEventData>(this.OnScreenCloseActionPressed));
			this.UnsubscribeRewiredInputEventAllPlayers(this._universalCancelAction, new Action<InputActionEventData>(this.OnUniversalCancelActionPressed));
		}

		private void SubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
		{
			if (actionId < 0 || callback == null)
			{
				return;
			}
			if (ReInput.mapping.GetAction(actionId) == null)
			{
				UnityEngine.Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
				return;
			}
			foreach (Player current in ReInput.players.AllPlayers)
			{
				current.AddInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionId);
			}
		}

		private void UnsubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
		{
			if (actionId < 0 || callback == null)
			{
				return;
			}
			if (!ReInput.isReady)
			{
				return;
			}
			if (ReInput.mapping.GetAction(actionId) == null)
			{
				UnityEngine.Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
				return;
			}
			foreach (Player current in ReInput.players.AllPlayers)
			{
				current.RemoveInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionId);
			}
		}

		private int GetMaxControllersPerPlayer()
		{
			if (this._rewiredInputManager.userData.ConfigVars.autoAssignJoysticks)
			{
				return this._rewiredInputManager.userData.ConfigVars.maxJoysticksPerPlayer;
			}
			return this._maxControllersPerPlayer;
		}

		private bool ShowAssignedControllers()
		{
			return this._showControllers && (this._showAssignedControllers || this.GetMaxControllersPerPlayer() != 1);
		}

		private void InspectorPropertyChanged()
		{
		}

		private void AssignController(Player player, int controllerId)
		{
			if (player == null)
			{
				return;
			}
			if (player.controllers.ContainsController(ControllerType.Joystick, controllerId))
			{
				return;
			}
			if (this.GetMaxControllersPerPlayer() == 1)
			{
				this.RemoveAllControllers(player);
				this.ClearVarsOnJoystickChange();
			}
			foreach (Player current in ReInput.players.Players)
			{
				if (current != player)
				{
					this.RemoveController(current, controllerId);
				}
			}
			player.controllers.AddController(ControllerType.Joystick, controllerId, false);
			if (ReInput.userDataStore != null)
			{
				ReInput.userDataStore.LoadControllerData(player.id, ControllerType.Joystick, controllerId);
			}
		}

		private void RemoveAllControllers(Player player)
		{
			if (player == null)
			{
				return;
			}
			IList<Rewired.Joystick> joysticks = player.controllers.Joysticks;
			for (int i = joysticks.Count - 1; i >= 0; i--)
			{
				this.RemoveController(player, joysticks[i].id);
			}
		}

		private void RemoveController(Player player, int controllerId)
		{
			if (player == null)
			{
				return;
			}
			if (!player.controllers.ContainsController(ControllerType.Joystick, controllerId))
			{
				return;
			}
			if (ReInput.userDataStore != null)
			{
				ReInput.userDataStore.SaveControllerData(player.id, ControllerType.Joystick, controllerId);
			}
			player.controllers.RemoveController(ControllerType.Joystick, controllerId);
		}

		public static void ApplyTheme(ThemedElement.ElementInfo[] elementInfo)
		{
			if (ControlMapper.Instance == null)
			{
				return;
			}
			if (ControlMapper.Instance._themeSettings == null)
			{
				return;
			}
			if (!ControlMapper.Instance._useThemeSettings)
			{
				return;
			}
			ControlMapper.Instance._themeSettings.Apply(elementInfo);
		}

		public static LanguageData GetLanguage()
		{
			if (ControlMapper.Instance == null)
			{
				return null;
			}
			return ControlMapper.Instance._language;
		}
	}
}
