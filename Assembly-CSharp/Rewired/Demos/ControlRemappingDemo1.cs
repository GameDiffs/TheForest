using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class ControlRemappingDemo1 : MonoBehaviour
	{
		private class ControllerSelection
		{
			private int _id;

			private int _idPrev;

			private ControllerType _type;

			private ControllerType _typePrev;

			public int id
			{
				get
				{
					return this._id;
				}
				set
				{
					this._idPrev = this._id;
					this._id = value;
				}
			}

			public ControllerType type
			{
				get
				{
					return this._type;
				}
				set
				{
					this._typePrev = this._type;
					this._type = value;
				}
			}

			public int idPrev
			{
				get
				{
					return this._idPrev;
				}
			}

			public ControllerType typePrev
			{
				get
				{
					return this._typePrev;
				}
			}

			public bool hasSelection
			{
				get
				{
					return this._id >= 0;
				}
			}

			public ControllerSelection()
			{
				this.Clear();
			}

			public void Set(int id, ControllerType type)
			{
				this.id = id;
				this.type = type;
			}

			public void Clear()
			{
				this._id = -1;
				this._idPrev = -1;
				this._type = ControllerType.Joystick;
				this._typePrev = ControllerType.Joystick;
			}
		}

		private class DialogHelper
		{
			public enum DialogType
			{
				None,
				JoystickConflict,
				ElementConflict,
				KeyConflict,
				DeleteAssignmentConfirmation = 10,
				AssignElement
			}

			private const float openBusyDelay = 0.25f;

			private const float closeBusyDelay = 0.1f;

			private ControlRemappingDemo1.DialogHelper.DialogType _type;

			private bool _enabled;

			private float _closeTime;

			private bool _closeTimerRunning;

			private float _busyTime;

			private bool _busyTimerRunning;

			private Action<int> drawWindowDelegate;

			private GUI.WindowFunction drawWindowFunction;

			private ControlRemappingDemo1.WindowProperties windowProperties;

			private int currentActionId;

			private Action<int, ControlRemappingDemo1.UserResponse> resultCallback;

			private float busyTimer
			{
				get
				{
					if (!this._busyTimerRunning)
					{
						return 0f;
					}
					return this._busyTime - Time.realtimeSinceStartup;
				}
			}

			public bool enabled
			{
				get
				{
					return this._enabled;
				}
				set
				{
					if (value)
					{
						if (this._type == ControlRemappingDemo1.DialogHelper.DialogType.None)
						{
							return;
						}
						this.StateChanged(0.25f);
					}
					else
					{
						this._enabled = value;
						this._type = ControlRemappingDemo1.DialogHelper.DialogType.None;
						this.StateChanged(0.1f);
					}
				}
			}

			public ControlRemappingDemo1.DialogHelper.DialogType type
			{
				get
				{
					if (!this._enabled)
					{
						return ControlRemappingDemo1.DialogHelper.DialogType.None;
					}
					return this._type;
				}
				set
				{
					if (value == ControlRemappingDemo1.DialogHelper.DialogType.None)
					{
						this._enabled = false;
						this.StateChanged(0.1f);
					}
					else
					{
						this._enabled = true;
						this.StateChanged(0.25f);
					}
					this._type = value;
				}
			}

			public float closeTimer
			{
				get
				{
					if (!this._closeTimerRunning)
					{
						return 0f;
					}
					return this._closeTime - Time.realtimeSinceStartup;
				}
			}

			public bool busy
			{
				get
				{
					return this._busyTimerRunning;
				}
			}

			public DialogHelper()
			{
				this.drawWindowDelegate = new Action<int>(this.DrawWindow);
				this.drawWindowFunction = new GUI.WindowFunction(this.drawWindowDelegate.Invoke);
			}

			public void StartModal(int queueActionId, ControlRemappingDemo1.DialogHelper.DialogType type, ControlRemappingDemo1.WindowProperties windowProperties, Action<int, ControlRemappingDemo1.UserResponse> resultCallback)
			{
				this.StartModal(queueActionId, type, windowProperties, resultCallback, 0f, -1f);
			}

			public void StartModal(int queueActionId, ControlRemappingDemo1.DialogHelper.DialogType type, ControlRemappingDemo1.WindowProperties windowProperties, Action<int, ControlRemappingDemo1.UserResponse> resultCallback, float closeTimer)
			{
				this.StartModal(queueActionId, type, windowProperties, resultCallback, closeTimer, -1f);
			}

			public void StartModal(int queueActionId, ControlRemappingDemo1.DialogHelper.DialogType type, ControlRemappingDemo1.WindowProperties windowProperties, Action<int, ControlRemappingDemo1.UserResponse> resultCallback, float closeTimer, float openBusyDelay)
			{
				this.currentActionId = queueActionId;
				this.windowProperties = windowProperties;
				this.type = type;
				this.resultCallback = resultCallback;
				if (closeTimer > 0f)
				{
					this.StartCloseTimer(closeTimer);
				}
				if (openBusyDelay >= 0f)
				{
					this.StateChanged(openBusyDelay);
				}
			}

			public void Update()
			{
				this.Draw();
				this.UpdateTimers();
			}

			public void Draw()
			{
				if (!this._enabled)
				{
					return;
				}
				bool enabled = GUI.enabled;
				GUI.enabled = true;
				GUILayout.Window(this.windowProperties.windowId, this.windowProperties.rect, this.drawWindowFunction, this.windowProperties.title, new GUILayoutOption[0]);
				GUI.FocusWindow(this.windowProperties.windowId);
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}

			public void DrawConfirmButton()
			{
				this.DrawConfirmButton("Confirm");
			}

			public void DrawConfirmButton(string title)
			{
				bool enabled = GUI.enabled;
				if (this.busy)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button(title, new GUILayoutOption[0]))
				{
					this.Confirm(ControlRemappingDemo1.UserResponse.Confirm);
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}

			public void DrawConfirmButton(ControlRemappingDemo1.UserResponse response)
			{
				this.DrawConfirmButton(response, "Confirm");
			}

			public void DrawConfirmButton(ControlRemappingDemo1.UserResponse response, string title)
			{
				bool enabled = GUI.enabled;
				if (this.busy)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button(title, new GUILayoutOption[0]))
				{
					this.Confirm(response);
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}

			public void DrawCancelButton()
			{
				this.DrawCancelButton("Cancel");
			}

			public void DrawCancelButton(string title)
			{
				bool enabled = GUI.enabled;
				if (this.busy)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button(title, new GUILayoutOption[0]))
				{
					this.Cancel();
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}

			public void Confirm()
			{
				this.Confirm(ControlRemappingDemo1.UserResponse.Confirm);
			}

			public void Confirm(ControlRemappingDemo1.UserResponse response)
			{
				this.resultCallback(this.currentActionId, response);
				this.Close();
			}

			public void Cancel()
			{
				this.resultCallback(this.currentActionId, ControlRemappingDemo1.UserResponse.Cancel);
				this.Close();
			}

			private void DrawWindow(int windowId)
			{
				this.windowProperties.windowDrawDelegate(this.windowProperties.title, this.windowProperties.message);
			}

			private void UpdateTimers()
			{
				if (this._closeTimerRunning && this.closeTimer <= 0f)
				{
					this.Cancel();
				}
				if (this._busyTimerRunning && this.busyTimer <= 0f)
				{
					this._busyTimerRunning = false;
				}
			}

			public void StartCloseTimer(float time)
			{
				this._closeTime = time + Time.realtimeSinceStartup;
				this._closeTimerRunning = true;
			}

			private void StartBusyTimer(float time)
			{
				this._busyTime = time + Time.realtimeSinceStartup;
				this._busyTimerRunning = true;
			}

			private void Close()
			{
				this.Reset();
				this.StateChanged(0.1f);
			}

			private void StateChanged(float delay)
			{
				this.StartBusyTimer(delay);
			}

			private void Reset()
			{
				this._enabled = false;
				this._type = ControlRemappingDemo1.DialogHelper.DialogType.None;
				this.currentActionId = -1;
				this.resultCallback = null;
				this._closeTimerRunning = false;
				this._closeTime = 0f;
			}

			private void ResetTimers()
			{
				this._busyTimerRunning = false;
				this._closeTimerRunning = false;
			}

			public void FullReset()
			{
				this.Reset();
				this.ResetTimers();
			}
		}

		private abstract class QueueEntry
		{
			public enum State
			{
				Waiting,
				Confirmed,
				Canceled
			}

			private static int uidCounter;

			public int id
			{
				get;
				protected set;
			}

			public ControlRemappingDemo1.QueueActionType queueActionType
			{
				get;
				protected set;
			}

			public ControlRemappingDemo1.QueueEntry.State state
			{
				get;
				protected set;
			}

			public ControlRemappingDemo1.UserResponse response
			{
				get;
				protected set;
			}

			protected static int nextId
			{
				get
				{
					int result = ControlRemappingDemo1.QueueEntry.uidCounter;
					ControlRemappingDemo1.QueueEntry.uidCounter++;
					return result;
				}
			}

			public QueueEntry(ControlRemappingDemo1.QueueActionType queueActionType)
			{
				this.id = ControlRemappingDemo1.QueueEntry.nextId;
				this.queueActionType = queueActionType;
			}

			public void Confirm(ControlRemappingDemo1.UserResponse response)
			{
				this.state = ControlRemappingDemo1.QueueEntry.State.Confirmed;
				this.response = response;
			}

			public void Cancel()
			{
				this.state = ControlRemappingDemo1.QueueEntry.State.Canceled;
			}
		}

		private class JoystickAssignmentChange : ControlRemappingDemo1.QueueEntry
		{
			public int playerId
			{
				get;
				private set;
			}

			public int joystickId
			{
				get;
				private set;
			}

			public bool assign
			{
				get;
				private set;
			}

			public JoystickAssignmentChange(int newPlayerId, int joystickId, bool assign) : base(ControlRemappingDemo1.QueueActionType.JoystickAssignment)
			{
				this.playerId = newPlayerId;
				this.joystickId = joystickId;
				this.assign = assign;
			}
		}

		private class ElementAssignmentChange : ControlRemappingDemo1.QueueEntry
		{
			public int playerId
			{
				get;
				private set;
			}

			public int controllerId
			{
				get;
				private set;
			}

			public ControllerType controllerType
			{
				get;
				private set;
			}

			public ControllerMap controllerMap
			{
				get;
				private set;
			}

			public int actionElementMapId
			{
				get;
				private set;
			}

			public int actionId
			{
				get;
				private set;
			}

			public Pole actionAxisContribution
			{
				get;
				private set;
			}

			public InputActionType actionType
			{
				get;
				private set;
			}

			public bool assignFullAxis
			{
				get;
				private set;
			}

			public bool invert
			{
				get;
				private set;
			}

			public ControlRemappingDemo1.ElementAssignmentChangeType changeType
			{
				get;
				set;
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

			public AxisRange AssignedAxisRange
			{
				get
				{
					if (!this.pollingInfo.success)
					{
						return AxisRange.Positive;
					}
					ControllerElementType elementType = this.pollingInfo.elementType;
					Pole axisPole = this.pollingInfo.axisPole;
					AxisRange result = AxisRange.Positive;
					if (elementType == ControllerElementType.Axis)
					{
						if (this.actionType == InputActionType.Axis)
						{
							if (this.assignFullAxis)
							{
								result = AxisRange.Full;
							}
							else
							{
								result = ((axisPole != Pole.Positive) ? AxisRange.Negative : AxisRange.Positive);
							}
						}
						else
						{
							result = ((axisPole != Pole.Positive) ? AxisRange.Negative : AxisRange.Positive);
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
					return this.pollingInfo.elementIdentifierName;
				}
			}

			public ElementAssignmentChange(int playerId, int controllerId, ControllerType controllerType, ControllerMap controllerMap, ControlRemappingDemo1.ElementAssignmentChangeType changeType, int actionElementMapId, int actionId, Pole actionAxisContribution, InputActionType actionType, bool assignFullAxis, bool invert) : base(ControlRemappingDemo1.QueueActionType.ElementAssignment)
			{
				this.playerId = playerId;
				this.controllerId = controllerId;
				this.controllerType = controllerType;
				this.controllerMap = controllerMap;
				this.changeType = changeType;
				this.actionElementMapId = actionElementMapId;
				this.actionId = actionId;
				this.actionAxisContribution = actionAxisContribution;
				this.actionType = actionType;
				this.assignFullAxis = assignFullAxis;
				this.invert = invert;
			}

			public ElementAssignmentChange(ControlRemappingDemo1.ElementAssignmentChange source) : base(ControlRemappingDemo1.QueueActionType.ElementAssignment)
			{
				this.playerId = source.playerId;
				this.controllerId = source.controllerId;
				this.controllerType = source.controllerType;
				this.controllerMap = source.controllerMap;
				this.changeType = source.changeType;
				this.actionElementMapId = source.actionElementMapId;
				this.actionId = source.actionId;
				this.actionAxisContribution = source.actionAxisContribution;
				this.actionType = source.actionType;
				this.assignFullAxis = source.assignFullAxis;
				this.invert = source.invert;
				this.pollingInfo = source.pollingInfo;
				this.modifierKeyFlags = source.modifierKeyFlags;
			}

			public void ReplaceOrCreateActionElementMap()
			{
				this.controllerMap.ReplaceOrCreateElementMap(this.ToElementAssignment());
			}

			public ElementAssignmentConflictCheck ToElementAssignmentConflictCheck()
			{
				return new ElementAssignmentConflictCheck(this.playerId, this.controllerType, this.controllerId, this.controllerMap.id, this.pollingInfo.elementType, this.pollingInfo.elementIdentifierId, this.AssignedAxisRange, this.pollingInfo.keyboardKey, this.modifierKeyFlags, this.actionId, this.actionAxisContribution, this.invert, this.actionElementMapId);
			}

			public ElementAssignment ToElementAssignment()
			{
				return new ElementAssignment(this.controllerType, this.pollingInfo.elementType, this.pollingInfo.elementIdentifierId, this.AssignedAxisRange, this.pollingInfo.keyboardKey, this.modifierKeyFlags, this.actionId, this.actionAxisContribution, this.invert, this.actionElementMapId);
			}
		}

		private class FallbackJoystickIdentification : ControlRemappingDemo1.QueueEntry
		{
			public int joystickId
			{
				get;
				private set;
			}

			public string joystickName
			{
				get;
				private set;
			}

			public FallbackJoystickIdentification(int joystickId, string joystickName) : base(ControlRemappingDemo1.QueueActionType.FallbackJoystickIdentification)
			{
				this.joystickId = joystickId;
				this.joystickName = joystickName;
			}
		}

		private class Calibration : ControlRemappingDemo1.QueueEntry
		{
			public int selectedElementIdentifierId;

			public bool recording;

			public Player player
			{
				get;
				private set;
			}

			public ControllerType controllerType
			{
				get;
				private set;
			}

			public Rewired.Joystick joystick
			{
				get;
				private set;
			}

			public CalibrationMap calibrationMap
			{
				get;
				private set;
			}

			public Calibration(Player player, Rewired.Joystick joystick, CalibrationMap calibrationMap) : base(ControlRemappingDemo1.QueueActionType.Calibrate)
			{
				this.player = player;
				this.joystick = joystick;
				this.calibrationMap = calibrationMap;
				this.selectedElementIdentifierId = -1;
			}
		}

		private struct WindowProperties
		{
			public int windowId;

			public Rect rect;

			public Action<string, string> windowDrawDelegate;

			public string title;

			public string message;
		}

		private enum QueueActionType
		{
			None,
			JoystickAssignment,
			ElementAssignment,
			FallbackJoystickIdentification,
			Calibrate
		}

		private enum ElementAssignmentChangeType
		{
			Add,
			Replace,
			Remove,
			ReassignOrRemove,
			ConflictCheck
		}

		public enum UserResponse
		{
			Confirm,
			Cancel,
			Custom1,
			Custom2
		}

		private const string playerPrefsBaseKey = "UserRemappingDemo";

		private const float defaultModalWidth = 250f;

		private const float defaultModalHeight = 200f;

		private const float assignmentTimeout = 5f;

		private ControlRemappingDemo1.DialogHelper dialog;

		private bool guiState;

		private bool busy;

		private bool pageGUIState;

		private Player selectedPlayer;

		private int selectedMapCategoryId;

		private ControlRemappingDemo1.ControllerSelection selectedController;

		private ControllerMap selectedMap;

		private bool showMenu;

		private Vector2 actionScrollPos;

		private Vector2 calibrateScrollPos;

		private Queue<ControlRemappingDemo1.QueueEntry> actionQueue;

		private bool setupFinished;

		[NonSerialized]
		private bool initialized;

		private bool isCompiling;

		private GUIStyle style_wordWrap;

		private GUIStyle style_centeredBox;

		private void Awake()
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.dialog = new ControlRemappingDemo1.DialogHelper();
			this.actionQueue = new Queue<ControlRemappingDemo1.QueueEntry>();
			this.selectedController = new ControlRemappingDemo1.ControllerSelection();
			ReInput.ControllerConnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickConnected);
			ReInput.ControllerPreDisconnectEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickPreDisconnect);
			ReInput.ControllerDisconnectedEvent += new Action<ControllerStatusChangedEventArgs>(this.JoystickDisconnected);
			this.Reset();
			this.initialized = true;
			this.LoadAllMaps();
			if (ReInput.unityJoystickIdentificationRequired)
			{
				this.IdentifyAllJoysticks();
			}
		}

		private void Setup()
		{
			if (this.setupFinished)
			{
				return;
			}
			this.style_wordWrap = new GUIStyle(GUI.skin.label);
			this.style_wordWrap.wordWrap = true;
			this.style_centeredBox = new GUIStyle(GUI.skin.box);
			this.style_centeredBox.alignment = TextAnchor.MiddleCenter;
			this.setupFinished = true;
		}

		public void OnGUI()
		{
			if (!this.initialized)
			{
				return;
			}
			this.Setup();
			this.HandleMenuControl();
			if (!this.showMenu)
			{
				this.DrawInitialScreen();
				return;
			}
			this.SetGUIStateStart();
			this.ProcessQueue();
			this.DrawPage();
			this.ShowDialog();
			this.SetGUIStateEnd();
			this.busy = false;
		}

		private void HandleMenuControl()
		{
			if (this.dialog.enabled)
			{
				return;
			}
			if (ReInput.players.GetSystemPlayer().GetButtonDown("Menu"))
			{
				if (this.showMenu)
				{
					this.SaveAllMaps();
					this.Close();
				}
				else
				{
					this.Open();
				}
			}
		}

		private void Close()
		{
			this.ClearWorkingVars();
			this.showMenu = false;
		}

		private void Open()
		{
			this.showMenu = true;
		}

		private void DrawInitialScreen()
		{
			ActionElementMap firstElementMapWithAction = ReInput.players.GetSystemPlayer().controllers.maps.GetFirstElementMapWithAction("Menu", true);
			GUIContent content;
			if (firstElementMapWithAction != null)
			{
				content = new GUIContent("Press " + firstElementMapWithAction.elementIdentifierName + " to open the menu.");
			}
			else
			{
				content = new GUIContent("There is no element assigned to open the menu!");
			}
			GUILayout.BeginArea(this.GetScreenCenteredRect(300f, 50f));
			GUILayout.Box(content, this.style_centeredBox, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			GUILayout.EndArea();
		}

		private void DrawPage()
		{
			if (GUI.enabled != this.pageGUIState)
			{
				GUI.enabled = this.pageGUIState;
			}
			Rect screenRect = new Rect(((float)Screen.width - (float)Screen.width * 0.9f) * 0.5f, ((float)Screen.height - (float)Screen.height * 0.9f) * 0.5f, (float)Screen.width * 0.9f, (float)Screen.height * 0.9f);
			GUILayout.BeginArea(screenRect);
			this.DrawPlayerSelector();
			this.DrawJoystickSelector();
			this.DrawMouseAssignment();
			this.DrawControllerSelector();
			this.DrawCalibrateButton();
			this.DrawMapCategories();
			this.actionScrollPos = GUILayout.BeginScrollView(this.actionScrollPos, new GUILayoutOption[0]);
			this.DrawCategoryActions();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		private void DrawPlayerSelector()
		{
			if (ReInput.players.allPlayerCount == 0)
			{
				GUILayout.Label("There are no players.", new GUILayoutOption[0]);
				return;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Players:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			foreach (Player current in ReInput.players.GetPlayers(true))
			{
				if (this.selectedPlayer == null)
				{
					this.selectedPlayer = current;
				}
				bool flag = current == this.selectedPlayer;
				bool flag2 = GUILayout.Toggle(flag, (!(current.descriptiveName != string.Empty)) ? current.name : current.descriptiveName, "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag2 != flag && flag2)
				{
					this.selectedPlayer = current;
					this.selectedController.Clear();
					this.selectedMapCategoryId = -1;
				}
			}
			GUILayout.EndHorizontal();
		}

		private void DrawMouseAssignment()
		{
			bool enabled = GUI.enabled;
			if (this.selectedPlayer == null)
			{
				GUI.enabled = false;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Assign Mouse:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool flag = this.selectedPlayer != null && this.selectedPlayer.controllers.hasMouse;
			bool flag2 = GUILayout.Toggle(flag, "Assign Mouse", "Button", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag2 != flag)
			{
				if (flag2)
				{
					this.selectedPlayer.controllers.hasMouse = true;
					foreach (Player current in ReInput.players.Players)
					{
						if (current != this.selectedPlayer)
						{
							current.controllers.hasMouse = false;
						}
					}
				}
				else
				{
					this.selectedPlayer.controllers.hasMouse = false;
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		private void DrawJoystickSelector()
		{
			bool enabled = GUI.enabled;
			if (this.selectedPlayer == null)
			{
				GUI.enabled = false;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Assign Joysticks:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool flag = this.selectedPlayer == null || this.selectedPlayer.controllers.joystickCount == 0;
			bool flag2 = GUILayout.Toggle(flag, "None", "Button", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag2 != flag)
			{
				this.selectedPlayer.controllers.ClearControllersOfType(ControllerType.Joystick);
				this.ControllerSelectionChanged();
			}
			if (this.selectedPlayer != null)
			{
				foreach (Rewired.Joystick current in ReInput.controllers.Joysticks)
				{
					flag = this.selectedPlayer.controllers.ContainsController(current);
					flag2 = GUILayout.Toggle(flag, current.name, "Button", new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					});
					if (flag2 != flag)
					{
						this.EnqueueAction(new ControlRemappingDemo1.JoystickAssignmentChange(this.selectedPlayer.id, current.id, flag2));
					}
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		private void DrawControllerSelector()
		{
			if (this.selectedPlayer == null)
			{
				return;
			}
			bool enabled = GUI.enabled;
			GUILayout.Space(15f);
			GUILayout.Label("Controller to Map:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (!this.selectedController.hasSelection)
			{
				this.selectedController.Set(0, ControllerType.Keyboard);
				this.ControllerSelectionChanged();
			}
			bool flag = this.selectedController.type == ControllerType.Keyboard;
			bool flag2 = GUILayout.Toggle(flag, "Keyboard", "Button", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag2 != flag)
			{
				this.selectedController.Set(0, ControllerType.Keyboard);
				this.ControllerSelectionChanged();
			}
			if (!this.selectedPlayer.controllers.hasMouse)
			{
				GUI.enabled = false;
			}
			flag = (this.selectedController.type == ControllerType.Mouse);
			flag2 = GUILayout.Toggle(flag, "Mouse", "Button", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag2 != flag)
			{
				this.selectedController.Set(0, ControllerType.Mouse);
				this.ControllerSelectionChanged();
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
			foreach (Rewired.Joystick current in this.selectedPlayer.controllers.Joysticks)
			{
				flag = (this.selectedController.type == ControllerType.Joystick && this.selectedController.id == current.id);
				flag2 = GUILayout.Toggle(flag, current.name, "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag2 != flag)
				{
					this.selectedController.Set(current.id, ControllerType.Joystick);
					this.ControllerSelectionChanged();
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		private void DrawCalibrateButton()
		{
			if (this.selectedPlayer == null)
			{
				return;
			}
			bool enabled = GUI.enabled;
			GUILayout.Space(10f);
			Controller controller = (!this.selectedController.hasSelection) ? null : this.selectedPlayer.controllers.GetController(this.selectedController.type, this.selectedController.id);
			if (controller == null || this.selectedController.type != ControllerType.Joystick)
			{
				GUI.enabled = false;
				GUILayout.Button("Select a controller to calibrate", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}
			else if (GUILayout.Button("Calibrate " + controller.name, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				Rewired.Joystick joystick = controller as Rewired.Joystick;
				if (joystick != null)
				{
					CalibrationMap calibrationMap = joystick.calibrationMap;
					if (calibrationMap != null)
					{
						this.EnqueueAction(new ControlRemappingDemo1.Calibration(this.selectedPlayer, joystick, calibrationMap));
					}
				}
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		private void DrawMapCategories()
		{
			if (this.selectedPlayer == null)
			{
				return;
			}
			if (!this.selectedController.hasSelection)
			{
				return;
			}
			bool enabled = GUI.enabled;
			GUILayout.Space(15f);
			GUILayout.Label("Categories:", new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			foreach (InputMapCategory current in ReInput.mapping.UserAssignableMapCategories)
			{
				if (!this.selectedPlayer.controllers.maps.ContainsMapInCategory(this.selectedController.type, current.id))
				{
					GUI.enabled = false;
				}
				else if (this.selectedMapCategoryId < 0)
				{
					this.selectedMapCategoryId = current.id;
					this.selectedMap = this.selectedPlayer.controllers.maps.GetFirstMapInCategory(this.selectedController.type, this.selectedController.id, current.id);
				}
				bool flag = current.id == this.selectedMapCategoryId;
				bool flag2 = GUILayout.Toggle(flag, (!(current.descriptiveName != string.Empty)) ? current.name : current.descriptiveName, "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag2 != flag)
				{
					this.selectedMapCategoryId = current.id;
					this.selectedMap = this.selectedPlayer.controllers.maps.GetFirstMapInCategory(this.selectedController.type, this.selectedController.id, current.id);
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		private void DrawCategoryActions()
		{
			if (this.selectedPlayer == null)
			{
				return;
			}
			if (this.selectedMapCategoryId < 0)
			{
				return;
			}
			bool enabled = GUI.enabled;
			if (this.selectedMap == null)
			{
				return;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Actions:", new GUILayoutOption[0]);
			InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(this.selectedMapCategoryId);
			if (mapCategory == null)
			{
				return;
			}
			InputCategory actionCategory = ReInput.mapping.GetActionCategory(mapCategory.name);
			if (actionCategory == null)
			{
				return;
			}
			float width = 150f;
			foreach (InputAction current in ReInput.mapping.ActionsInCategory(actionCategory.id))
			{
				string text = (!(current.descriptiveName != string.Empty)) ? current.name : current.descriptiveName;
				if (current.type == InputActionType.Button)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(text, new GUILayoutOption[]
					{
						GUILayout.Width(width)
					});
					this.DrawAddActionMapButton(this.selectedPlayer.id, current, Pole.Positive, this.selectedController, this.selectedMap, true);
					foreach (ActionElementMap current2 in this.selectedMap.AllMaps)
					{
						if (current2.actionId == current.id)
						{
							this.DrawActionAssignmentButton(this.selectedPlayer.id, current, Pole.Positive, this.selectedController, this.selectedMap, true, current2);
						}
					}
					GUILayout.EndHorizontal();
				}
				else if (current.type == InputActionType.Axis)
				{
					if (this.selectedController.type != ControllerType.Keyboard)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label(text, new GUILayoutOption[]
						{
							GUILayout.Width(width)
						});
						this.DrawAddActionMapButton(this.selectedPlayer.id, current, Pole.Positive, this.selectedController, this.selectedMap, true);
						foreach (ActionElementMap current3 in this.selectedMap.AllMaps)
						{
							if (current3.actionId == current.id)
							{
								if (current3.elementType != ControllerElementType.Button)
								{
									if (current3.axisType != AxisType.Split)
									{
										this.DrawActionAssignmentButton(this.selectedPlayer.id, current, Pole.Positive, this.selectedController, this.selectedMap, true, current3);
										this.DrawInvertButton(this.selectedPlayer.id, current, Pole.Positive, this.selectedController, this.selectedMap, current3);
									}
								}
							}
						}
						GUILayout.EndHorizontal();
					}
					string text2 = (!(current.positiveDescriptiveName != string.Empty)) ? (current.descriptiveName + " +") : current.positiveDescriptiveName;
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(text2, new GUILayoutOption[]
					{
						GUILayout.Width(width)
					});
					this.DrawAddActionMapButton(this.selectedPlayer.id, current, Pole.Positive, this.selectedController, this.selectedMap, false);
					foreach (ActionElementMap current4 in this.selectedMap.AllMaps)
					{
						if (current4.actionId == current.id)
						{
							if (current4.axisContribution == Pole.Positive)
							{
								if (current4.axisType != AxisType.Normal)
								{
									this.DrawActionAssignmentButton(this.selectedPlayer.id, current, Pole.Positive, this.selectedController, this.selectedMap, false, current4);
								}
							}
						}
					}
					GUILayout.EndHorizontal();
					string text3 = (!(current.negativeDescriptiveName != string.Empty)) ? (current.descriptiveName + " -") : current.negativeDescriptiveName;
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(text3, new GUILayoutOption[]
					{
						GUILayout.Width(width)
					});
					this.DrawAddActionMapButton(this.selectedPlayer.id, current, Pole.Negative, this.selectedController, this.selectedMap, false);
					foreach (ActionElementMap current5 in this.selectedMap.AllMaps)
					{
						if (current5.actionId == current.id)
						{
							if (current5.axisContribution == Pole.Negative)
							{
								if (current5.axisType != AxisType.Normal)
								{
									this.DrawActionAssignmentButton(this.selectedPlayer.id, current, Pole.Negative, this.selectedController, this.selectedMap, false, current5);
								}
							}
						}
					}
					GUILayout.EndHorizontal();
				}
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		private void DrawActionAssignmentButton(int playerId, InputAction action, Pole actionAxisContribution, ControlRemappingDemo1.ControllerSelection controller, ControllerMap controllerMap, bool assignFullAxis, ActionElementMap elementMap)
		{
			if (GUILayout.Button(elementMap.elementIdentifierName, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false),
				GUILayout.MinWidth(30f)
			}))
			{
				this.EnqueueAction(new ControlRemappingDemo1.ElementAssignmentChange(playerId, controller.id, controller.type, controllerMap, ControlRemappingDemo1.ElementAssignmentChangeType.ReassignOrRemove, elementMap.id, action.id, actionAxisContribution, action.type, assignFullAxis, elementMap.invert));
			}
			GUILayout.Space(4f);
		}

		private void DrawInvertButton(int playerId, InputAction action, Pole actionAxisContribution, ControlRemappingDemo1.ControllerSelection controller, ControllerMap controllerMap, ActionElementMap elementMap)
		{
			bool invert = elementMap.invert;
			bool flag = GUILayout.Toggle(invert, "Invert", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag != invert)
			{
				elementMap.invert = flag;
			}
			GUILayout.Space(10f);
		}

		private void DrawAddActionMapButton(int playerId, InputAction action, Pole actionAxisContribution, ControlRemappingDemo1.ControllerSelection controller, ControllerMap controllerMap, bool assignFullAxis)
		{
			if (GUILayout.Button("Add...", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				this.EnqueueAction(new ControlRemappingDemo1.ElementAssignmentChange(playerId, controller.id, controller.type, controllerMap, ControlRemappingDemo1.ElementAssignmentChangeType.Add, -1, action.id, actionAxisContribution, action.type, assignFullAxis, false));
			}
			GUILayout.Space(10f);
		}

		private void ShowDialog()
		{
			this.dialog.Update();
		}

		private void DrawModalWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.dialog.DrawConfirmButton("Okay");
			GUILayout.FlexibleSpace();
			this.dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}

		private void DrawModalWindow_OkayOnly(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.dialog.DrawConfirmButton("Okay");
			GUILayout.EndHorizontal();
		}

		private void DrawElementAssignmentWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			ControlRemappingDemo1.ElementAssignmentChange elementAssignmentChange = this.actionQueue.Peek() as ControlRemappingDemo1.ElementAssignmentChange;
			if (elementAssignmentChange == null)
			{
				this.dialog.Cancel();
				return;
			}
			this.PollControllerForAssignment(elementAssignmentChange);
			GUILayout.Label("Assignment will be canceled in " + ((int)Mathf.Ceil(this.dialog.closeTimer)).ToString() + "...", this.style_wordWrap, new GUILayoutOption[0]);
		}

		private void DrawElementAssignmentProtectedConflictWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (!(this.actionQueue.Peek() is ControlRemappingDemo1.ElementAssignmentChange))
			{
				this.dialog.Cancel();
				return;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.dialog.DrawConfirmButton(ControlRemappingDemo1.UserResponse.Custom1, "Add");
			GUILayout.FlexibleSpace();
			this.dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}

		private void DrawElementAssignmentNormalConflictWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (!(this.actionQueue.Peek() is ControlRemappingDemo1.ElementAssignmentChange))
			{
				this.dialog.Cancel();
				return;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.dialog.DrawConfirmButton(ControlRemappingDemo1.UserResponse.Confirm, "Replace");
			GUILayout.FlexibleSpace();
			this.dialog.DrawConfirmButton(ControlRemappingDemo1.UserResponse.Custom1, "Add");
			GUILayout.FlexibleSpace();
			this.dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}

		private void DrawReassignOrRemoveElementAssignmentWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.dialog.DrawConfirmButton("Reassign");
			GUILayout.FlexibleSpace();
			this.dialog.DrawCancelButton("Remove");
			GUILayout.EndHorizontal();
		}

		private void DrawFallbackJoystickIdentificationWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			ControlRemappingDemo1.FallbackJoystickIdentification fallbackJoystickIdentification = this.actionQueue.Peek() as ControlRemappingDemo1.FallbackJoystickIdentification;
			if (fallbackJoystickIdentification == null)
			{
				this.dialog.Cancel();
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.Label("Press any button or axis on \"" + fallbackJoystickIdentification.joystickName + "\" now.", this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Skip", new GUILayoutOption[0]))
			{
				this.dialog.Cancel();
				return;
			}
			if (this.dialog.busy)
			{
				return;
			}
			if (!ReInput.controllers.SetUnityJoystickIdFromAnyButtonOrAxisPress(fallbackJoystickIdentification.joystickId, 0.8f, false))
			{
				return;
			}
			this.dialog.Confirm();
		}

		private void DrawCalibrationWindow(string title, string message)
		{
			if (!this.dialog.enabled)
			{
				return;
			}
			ControlRemappingDemo1.Calibration calibration = this.actionQueue.Peek() as ControlRemappingDemo1.Calibration;
			if (calibration == null)
			{
				this.dialog.Cancel();
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, this.style_wordWrap, new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool enabled = GUI.enabled;
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(200f)
			});
			this.calibrateScrollPos = GUILayout.BeginScrollView(this.calibrateScrollPos, new GUILayoutOption[0]);
			if (calibration.recording)
			{
				GUI.enabled = false;
			}
			IList<ControllerElementIdentifier> axisElementIdentifiers = calibration.joystick.AxisElementIdentifiers;
			for (int i = 0; i < axisElementIdentifiers.Count; i++)
			{
				ControllerElementIdentifier controllerElementIdentifier = axisElementIdentifiers[i];
				bool flag = calibration.selectedElementIdentifierId == controllerElementIdentifier.id;
				bool flag2 = GUILayout.Toggle(flag, controllerElementIdentifier.name, "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag != flag2)
				{
					calibration.selectedElementIdentifierId = controllerElementIdentifier.id;
				}
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(200f)
			});
			if (calibration.selectedElementIdentifierId >= 0)
			{
				float axisRawById = calibration.joystick.GetAxisRawById(calibration.selectedElementIdentifierId);
				GUILayout.Label("Raw Value: " + axisRawById.ToString(), new GUILayoutOption[0]);
				int axisIndexById = calibration.joystick.GetAxisIndexById(calibration.selectedElementIdentifierId);
				AxisCalibration axis = calibration.calibrationMap.GetAxis(axisIndexById);
				GUILayout.Label("Calibrated Value: " + calibration.joystick.GetAxisById(calibration.selectedElementIdentifierId), new GUILayoutOption[0]);
				GUILayout.Label("Zero: " + axis.calibratedZero, new GUILayoutOption[0]);
				GUILayout.Label("Min: " + axis.calibratedMin, new GUILayoutOption[0]);
				GUILayout.Label("Max: " + axis.calibratedMax, new GUILayoutOption[0]);
				GUILayout.Label("Dead Zone: " + axis.deadZone, new GUILayoutOption[0]);
				GUILayout.Space(15f);
				bool flag3 = GUILayout.Toggle(axis.enabled, "Enabled", "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (axis.enabled != flag3)
				{
					axis.enabled = flag3;
				}
				GUILayout.Space(10f);
				bool flag4 = GUILayout.Toggle(calibration.recording, "Record Min/Max", "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag4 != calibration.recording)
				{
					if (flag4)
					{
						axis.calibratedMax = 0f;
						axis.calibratedMin = 0f;
					}
					calibration.recording = flag4;
				}
				if (calibration.recording)
				{
					axis.calibratedMin = Mathf.Min(new float[]
					{
						axis.calibratedMin,
						axisRawById,
						axis.calibratedMin
					});
					axis.calibratedMax = Mathf.Max(new float[]
					{
						axis.calibratedMax,
						axisRawById,
						axis.calibratedMax
					});
					GUI.enabled = false;
				}
				if (GUILayout.Button("Set Zero", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.calibratedZero = axisRawById;
				}
				if (GUILayout.Button("Set Dead Zone", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.deadZone = axisRawById;
				}
				bool flag5 = GUILayout.Toggle(axis.invert, "Invert", "Button", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
				if (axis.invert != flag5)
				{
					axis.invert = flag5;
				}
				GUILayout.Space(10f);
				if (GUILayout.Button("Reset", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.Reset();
				}
				if (GUI.enabled != enabled)
				{
					GUI.enabled = enabled;
				}
			}
			else
			{
				GUILayout.Label("Select an axis to begin.", new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			if (calibration.recording)
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button("Close", new GUILayoutOption[0]))
			{
				this.calibrateScrollPos = default(Vector2);
				this.dialog.Confirm();
			}
			if (GUI.enabled != enabled)
			{
				GUI.enabled = enabled;
			}
		}

		private void DialogResultCallback(int queueActionId, ControlRemappingDemo1.UserResponse response)
		{
			foreach (ControlRemappingDemo1.QueueEntry current in this.actionQueue)
			{
				if (current.id == queueActionId)
				{
					if (response != ControlRemappingDemo1.UserResponse.Cancel)
					{
						current.Confirm(response);
					}
					else
					{
						current.Cancel();
					}
					break;
				}
			}
		}

		private void PollControllerForAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (this.dialog.busy)
			{
				return;
			}
			switch (entry.controllerType)
			{
			case ControllerType.Keyboard:
				this.PollKeyboardForAssignment(entry);
				break;
			case ControllerType.Mouse:
				this.PollMouseForAssignment(entry);
				break;
			case ControllerType.Joystick:
				this.PollJoystickForAssignment(entry);
				break;
			}
		}

		private void PollKeyboardForAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			int num = 0;
			ControllerPollingInfo pollingInfo = default(ControllerPollingInfo);
			ControllerPollingInfo pollingInfo2 = default(ControllerPollingInfo);
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
							pollingInfo2 = current;
						}
						modifierKeyFlags |= Keyboard.KeyCodeToModifierKeyFlags(keyboardKey);
						num++;
					}
					else if (pollingInfo.keyboardKey == KeyCode.None)
					{
						pollingInfo = current;
					}
				}
			}
			if (pollingInfo.keyboardKey == KeyCode.None)
			{
				if (num > 0)
				{
					this.dialog.StartCloseTimer(5f);
					if (num == 1)
					{
						if (ReInput.controllers.Keyboard.GetKeyTimePressed(pollingInfo2.keyboardKey) > 1f)
						{
							entry.pollingInfo = pollingInfo2;
							this.dialog.Confirm();
							return;
						}
						GUILayout.Label(Keyboard.GetKeyName(pollingInfo2.keyboardKey), new GUILayoutOption[0]);
					}
					else
					{
						GUILayout.Label(Keyboard.ModifierKeyFlagsToString(modifierKeyFlags), new GUILayoutOption[0]);
					}
				}
				return;
			}
			if (num == 0)
			{
				entry.pollingInfo = pollingInfo;
				this.dialog.Confirm();
				return;
			}
			entry.pollingInfo = pollingInfo;
			entry.modifierKeyFlags = modifierKeyFlags;
			this.dialog.Confirm();
		}

		private void PollJoystickForAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			Player player = ReInput.players.GetPlayer(entry.playerId);
			if (player == null)
			{
				this.dialog.Cancel();
				return;
			}
			entry.pollingInfo = player.controllers.polling.PollControllerForFirstElementDown(entry.controllerType, entry.controllerId);
			if (entry.pollingInfo.success)
			{
				this.dialog.Confirm();
			}
		}

		private void PollMouseForAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			Player player = ReInput.players.GetPlayer(entry.playerId);
			if (player == null)
			{
				this.dialog.Cancel();
				return;
			}
			entry.pollingInfo = player.controllers.polling.PollControllerForFirstElementDown(entry.controllerType, entry.controllerId);
			if (entry.pollingInfo.success)
			{
				this.dialog.Confirm();
			}
		}

		private Rect GetScreenCenteredRect(float width, float height)
		{
			return new Rect((float)Screen.width * 0.5f - width * 0.5f, (float)((double)Screen.height * 0.5 - (double)(height * 0.5f)), width, height);
		}

		private void EnqueueAction(ControlRemappingDemo1.QueueEntry entry)
		{
			if (entry == null)
			{
				return;
			}
			this.busy = true;
			GUI.enabled = false;
			this.actionQueue.Enqueue(entry);
		}

		private void ProcessQueue()
		{
			if (this.dialog.enabled)
			{
				return;
			}
			if (this.busy || this.actionQueue.Count == 0)
			{
				return;
			}
			while (this.actionQueue.Count > 0)
			{
				ControlRemappingDemo1.QueueEntry queueEntry = this.actionQueue.Peek();
				bool flag = false;
				switch (queueEntry.queueActionType)
				{
				case ControlRemappingDemo1.QueueActionType.JoystickAssignment:
					flag = this.ProcessJoystickAssignmentChange((ControlRemappingDemo1.JoystickAssignmentChange)queueEntry);
					break;
				case ControlRemappingDemo1.QueueActionType.ElementAssignment:
					flag = this.ProcessElementAssignmentChange((ControlRemappingDemo1.ElementAssignmentChange)queueEntry);
					break;
				case ControlRemappingDemo1.QueueActionType.FallbackJoystickIdentification:
					flag = this.ProcessFallbackJoystickIdentification((ControlRemappingDemo1.FallbackJoystickIdentification)queueEntry);
					break;
				case ControlRemappingDemo1.QueueActionType.Calibrate:
					flag = this.ProcessCalibration((ControlRemappingDemo1.Calibration)queueEntry);
					break;
				}
				if (!flag)
				{
					break;
				}
				this.actionQueue.Dequeue();
			}
		}

		private bool ProcessJoystickAssignmentChange(ControlRemappingDemo1.JoystickAssignmentChange entry)
		{
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			Player player = ReInput.players.GetPlayer(entry.playerId);
			if (player == null)
			{
				return true;
			}
			if (!entry.assign)
			{
				player.controllers.RemoveController(ControllerType.Joystick, entry.joystickId);
				this.ControllerSelectionChanged();
				return true;
			}
			if (player.controllers.ContainsController(ControllerType.Joystick, entry.joystickId))
			{
				return true;
			}
			bool flag = ReInput.controllers.IsJoystickAssigned(entry.joystickId);
			if (!flag || entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				player.controllers.AddController(ControllerType.Joystick, entry.joystickId, true);
				this.ControllerSelectionChanged();
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.JoystickConflict, new ControlRemappingDemo1.WindowProperties
			{
				title = "Joystick Reassignment",
				message = "This joystick is already assigned to another player. Do you want to reassign this joystick to " + player.descriptiveName + "?",
				rect = this.GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = new Action<string, string>(this.DrawModalWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			return false;
		}

		private bool ProcessElementAssignmentChange(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			switch (entry.changeType)
			{
			case ControlRemappingDemo1.ElementAssignmentChangeType.Add:
			case ControlRemappingDemo1.ElementAssignmentChangeType.Replace:
				return this.ProcessAddOrReplaceElementAssignment(entry);
			case ControlRemappingDemo1.ElementAssignmentChangeType.Remove:
				return this.ProcessRemoveElementAssignment(entry);
			case ControlRemappingDemo1.ElementAssignmentChangeType.ReassignOrRemove:
				return this.ProcessRemoveOrReassignElementAssignment(entry);
			case ControlRemappingDemo1.ElementAssignmentChangeType.ConflictCheck:
				return this.ProcessElementAssignmentConflictCheck(entry);
			default:
				throw new NotImplementedException();
			}
		}

		private bool ProcessRemoveOrReassignElementAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (entry.controllerMap == null)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				ControlRemappingDemo1.ElementAssignmentChange elementAssignmentChange = new ControlRemappingDemo1.ElementAssignmentChange(entry);
				elementAssignmentChange.changeType = ControlRemappingDemo1.ElementAssignmentChangeType.Remove;
				this.actionQueue.Enqueue(elementAssignmentChange);
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				ControlRemappingDemo1.ElementAssignmentChange elementAssignmentChange2 = new ControlRemappingDemo1.ElementAssignmentChange(entry);
				elementAssignmentChange2.changeType = ControlRemappingDemo1.ElementAssignmentChangeType.Replace;
				this.actionQueue.Enqueue(elementAssignmentChange2);
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.AssignElement, new ControlRemappingDemo1.WindowProperties
			{
				title = "Reassign or Remove",
				message = "Do you want to reassign or remove this assignment?",
				rect = this.GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = new Action<string, string>(this.DrawReassignOrRemoveElementAssignmentWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			return false;
		}

		private bool ProcessRemoveElementAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (entry.controllerMap == null)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				entry.controllerMap.DeleteElementMap(entry.actionElementMapId);
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.DeleteAssignmentConfirmation, new ControlRemappingDemo1.WindowProperties
			{
				title = "Remove Assignment",
				message = "Are you sure you want to remove this assignment?",
				rect = this.GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = new Action<string, string>(this.DrawModalWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			return false;
		}

		private bool ProcessAddOrReplaceElementAssignment(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (ReInput.players.GetPlayer(entry.playerId) == null)
			{
				return true;
			}
			if (entry.controllerMap == null)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state != ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				string text;
				if (entry.controllerType == ControllerType.Keyboard)
				{
					if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
					{
						text = "Press any key to assign it to this action. You may also use the modifier keys Command, Control, Alt, and Shift. If you wish to assign a modifier key ifselt this action, press and hold the key for 1 second.";
					}
					else
					{
						text = "Press any key to assign it to this action. You may also use the modifier keys Control, Alt, and Shift. If you wish to assign a modifier key itself to this action, press and hold the key for 1 second.";
					}
					if (Application.isEditor)
					{
						text += "\n\nNOTE: Some modifier key combinations will not work in the Unity Editor, but they will work in a game build.";
					}
				}
				else if (entry.controllerType == ControllerType.Mouse)
				{
					text = "Press any mouse button or axis to assign it to this action.\n\nTo assign mouse movement axes, move the mouse quickly in the direction you want mapped to the action. Slow movements will be ignored.";
				}
				else
				{
					text = "Press any button or axis to assign it to this action.";
				}
				this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.AssignElement, new ControlRemappingDemo1.WindowProperties
				{
					title = "Assign",
					message = text,
					rect = this.GetScreenCenteredRect(250f, 200f),
					windowDrawDelegate = new Action<string, string>(this.DrawElementAssignmentWindow)
				}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback), 5f);
				return false;
			}
			if (Event.current.type != EventType.Layout)
			{
				return false;
			}
			if (!ReInput.controllers.conflictChecking.DoesElementAssignmentConflict(entry.ToElementAssignmentConflictCheck()))
			{
				entry.ReplaceOrCreateActionElementMap();
			}
			else
			{
				ControlRemappingDemo1.ElementAssignmentChange elementAssignmentChange = new ControlRemappingDemo1.ElementAssignmentChange(entry);
				elementAssignmentChange.changeType = ControlRemappingDemo1.ElementAssignmentChangeType.ConflictCheck;
				this.actionQueue.Enqueue(elementAssignmentChange);
			}
			return true;
		}

		private bool ProcessElementAssignmentConflictCheck(ControlRemappingDemo1.ElementAssignmentChange entry)
		{
			if (ReInput.players.GetPlayer(entry.playerId) == null)
			{
				return true;
			}
			if (entry.controllerMap == null)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				entry.changeType = ControlRemappingDemo1.ElementAssignmentChangeType.Add;
				if (entry.response == ControlRemappingDemo1.UserResponse.Confirm)
				{
					ReInput.controllers.conflictChecking.RemoveElementAssignmentConflicts(entry.ToElementAssignmentConflictCheck());
					entry.ReplaceOrCreateActionElementMap();
				}
				else
				{
					if (entry.response != ControlRemappingDemo1.UserResponse.Custom1)
					{
						throw new NotImplementedException();
					}
					entry.ReplaceOrCreateActionElementMap();
				}
				return true;
			}
			bool flag = false;
			foreach (ElementAssignmentConflictInfo current in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(entry.ToElementAssignmentConflictCheck()))
			{
				if (!current.isUserAssignable)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				string message = entry.elementName + " is already in use and is protected from reassignment. You cannot remove the protected assignment, but you can still assign the action to this element. If you do so, the element will trigger multiple actions when activated.";
				this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.AssignElement, new ControlRemappingDemo1.WindowProperties
				{
					title = "Assignment Conflict",
					message = message,
					rect = this.GetScreenCenteredRect(250f, 200f),
					windowDrawDelegate = new Action<string, string>(this.DrawElementAssignmentProtectedConflictWindow)
				}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			}
			else
			{
				string message2 = entry.elementName + " is already in use. You may replace the other conflicting assignments, add this assignment anyway which will leave multiple actions assigned to this element, or cancel this assignment.";
				this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.AssignElement, new ControlRemappingDemo1.WindowProperties
				{
					title = "Assignment Conflict",
					message = message2,
					rect = this.GetScreenCenteredRect(250f, 200f),
					windowDrawDelegate = new Action<string, string>(this.DrawElementAssignmentNormalConflictWindow)
				}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			}
			return false;
		}

		private bool ProcessFallbackJoystickIdentification(ControlRemappingDemo1.FallbackJoystickIdentification entry)
		{
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.JoystickConflict, new ControlRemappingDemo1.WindowProperties
			{
				title = "Joystick Identification Required",
				message = "A joystick has been attached or removed. You will need to identify each joystick by pressing a button on the controller listed below:",
				rect = this.GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = new Action<string, string>(this.DrawFallbackJoystickIdentificationWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback), 0f, 1f);
			return false;
		}

		private bool ProcessCalibration(ControlRemappingDemo1.Calibration entry)
		{
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == ControlRemappingDemo1.QueueEntry.State.Confirmed)
			{
				return true;
			}
			this.dialog.StartModal(entry.id, ControlRemappingDemo1.DialogHelper.DialogType.JoystickConflict, new ControlRemappingDemo1.WindowProperties
			{
				title = "Calibrate Controller",
				message = "Select an axis to calibrate on the " + entry.joystick.name + ".",
				rect = this.GetScreenCenteredRect(450f, 480f),
				windowDrawDelegate = new Action<string, string>(this.DrawCalibrationWindow)
			}, new Action<int, ControlRemappingDemo1.UserResponse>(this.DialogResultCallback));
			return false;
		}

		private void PlayerSelectionChanged()
		{
			this.ClearControllerSelection();
		}

		private void ControllerSelectionChanged()
		{
			this.ClearMapSelection();
		}

		private void ClearControllerSelection()
		{
			this.selectedController.Clear();
			this.ClearMapSelection();
		}

		private void ClearMapSelection()
		{
			this.selectedMapCategoryId = -1;
			this.selectedMap = null;
		}

		private void Reset()
		{
			this.ClearWorkingVars();
			this.initialized = false;
			this.showMenu = false;
		}

		private void ClearWorkingVars()
		{
			this.selectedPlayer = null;
			this.ClearMapSelection();
			this.selectedController.Clear();
			this.actionScrollPos = default(Vector2);
			this.dialog.FullReset();
			this.actionQueue.Clear();
			this.busy = false;
		}

		private void SetGUIStateStart()
		{
			this.guiState = true;
			if (this.busy)
			{
				this.guiState = false;
			}
			this.pageGUIState = (this.guiState && !this.busy && !this.dialog.enabled && !this.dialog.busy);
			if (GUI.enabled != this.guiState)
			{
				GUI.enabled = this.guiState;
			}
		}

		private void SetGUIStateEnd()
		{
			this.guiState = true;
			if (!GUI.enabled)
			{
				GUI.enabled = this.guiState;
			}
		}

		private void JoystickConnected(ControllerStatusChangedEventArgs args)
		{
			this.LoadJoystickMaps(args.controllerId);
			if (ReInput.unityJoystickIdentificationRequired)
			{
				this.IdentifyAllJoysticks();
			}
		}

		private void JoystickPreDisconnect(ControllerStatusChangedEventArgs args)
		{
			if (this.selectedController.hasSelection && args.controllerType == this.selectedController.type && args.controllerId == this.selectedController.id)
			{
				this.ClearControllerSelection();
			}
			if (this.showMenu)
			{
				this.SaveJoystickMaps(args.controllerId);
			}
		}

		private void JoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (this.showMenu)
			{
				this.ClearWorkingVars();
			}
			if (ReInput.unityJoystickIdentificationRequired)
			{
				this.IdentifyAllJoysticks();
			}
		}

		private void LoadAllMaps()
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				IList<InputBehavior> inputBehaviors = ReInput.mapping.GetInputBehaviors(player.id);
				for (int j = 0; j < inputBehaviors.Count; j++)
				{
					string inputBehaviorXml = this.GetInputBehaviorXml(player, inputBehaviors[j].id);
					if (inputBehaviorXml != null && !(inputBehaviorXml == string.Empty))
					{
						inputBehaviors[j].ImportXmlString(inputBehaviorXml);
					}
				}
				List<string> allControllerMapsXml = this.GetAllControllerMapsXml(player, true, ControllerType.Keyboard, ReInput.controllers.Keyboard);
				List<string> allControllerMapsXml2 = this.GetAllControllerMapsXml(player, true, ControllerType.Mouse, ReInput.controllers.Mouse);
				bool flag = false;
				List<List<string>> list = new List<List<string>>();
				foreach (Rewired.Joystick current in player.controllers.Joysticks)
				{
					List<string> allControllerMapsXml3 = this.GetAllControllerMapsXml(player, true, ControllerType.Joystick, current);
					list.Add(allControllerMapsXml3);
					if (allControllerMapsXml3.Count > 0)
					{
						flag = true;
					}
				}
				if (allControllerMapsXml.Count > 0)
				{
					player.controllers.maps.ClearMaps(ControllerType.Keyboard, true);
				}
				player.controllers.maps.AddMapsFromXml(ControllerType.Keyboard, 0, allControllerMapsXml);
				if (flag)
				{
					player.controllers.maps.ClearMaps(ControllerType.Joystick, true);
				}
				int num = 0;
				foreach (Rewired.Joystick current2 in player.controllers.Joysticks)
				{
					player.controllers.maps.AddMapsFromXml(ControllerType.Joystick, current2.id, list[num]);
					num++;
				}
				if (allControllerMapsXml2.Count > 0)
				{
					player.controllers.maps.ClearMaps(ControllerType.Mouse, true);
				}
				player.controllers.maps.AddMapsFromXml(ControllerType.Mouse, 0, allControllerMapsXml2);
			}
			foreach (Rewired.Joystick current3 in ReInput.controllers.Joysticks)
			{
				current3.ImportCalibrationMapFromXmlString(this.GetJoystickCalibrationMapXml(current3));
			}
		}

		private void SaveAllMaps()
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				PlayerSaveData saveData = player.GetSaveData(true);
				InputBehavior[] inputBehaviors = saveData.inputBehaviors;
				for (int j = 0; j < inputBehaviors.Length; j++)
				{
					InputBehavior inputBehavior = inputBehaviors[j];
					string inputBehaviorPlayerPrefsKey = this.GetInputBehaviorPlayerPrefsKey(player, inputBehavior);
					PlayerPrefs.SetString(inputBehaviorPlayerPrefsKey, inputBehavior.ToXmlString());
				}
				foreach (ControllerMapSaveData current in saveData.AllControllerMapSaveData)
				{
					string controllerMapPlayerPrefsKey = this.GetControllerMapPlayerPrefsKey(player, current);
					PlayerPrefs.SetString(controllerMapPlayerPrefsKey, current.map.ToXmlString());
				}
			}
			foreach (Rewired.Joystick current2 in ReInput.controllers.Joysticks)
			{
				JoystickCalibrationMapSaveData calibrationMapSaveData = current2.GetCalibrationMapSaveData();
				string joystickCalibrationMapPlayerPrefsKey = this.GetJoystickCalibrationMapPlayerPrefsKey(calibrationMapSaveData);
				PlayerPrefs.SetString(joystickCalibrationMapPlayerPrefsKey, calibrationMapSaveData.map.ToXmlString());
			}
			PlayerPrefs.Save();
		}

		private void LoadJoystickMaps(int joystickId)
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.controllers.ContainsController(ControllerType.Joystick, joystickId))
				{
					Rewired.Joystick controller = player.controllers.GetController<Rewired.Joystick>(joystickId);
					if (controller != null)
					{
						List<string> allControllerMapsXml = this.GetAllControllerMapsXml(player, true, ControllerType.Joystick, controller);
						if (allControllerMapsXml.Count != 0)
						{
							player.controllers.maps.ClearMaps(ControllerType.Joystick, true);
							player.controllers.maps.AddMapsFromXml(ControllerType.Joystick, joystickId, allControllerMapsXml);
							controller.ImportCalibrationMapFromXmlString(this.GetJoystickCalibrationMapXml(controller));
						}
					}
				}
			}
		}

		private void SaveJoystickMaps(int joystickId)
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				if (player.controllers.ContainsController(ControllerType.Joystick, joystickId))
				{
					JoystickMapSaveData[] mapSaveData = player.controllers.maps.GetMapSaveData<JoystickMapSaveData>(joystickId, true);
					string key;
					if (mapSaveData != null)
					{
						for (int j = 0; j < mapSaveData.Length; j++)
						{
							key = this.GetControllerMapPlayerPrefsKey(player, mapSaveData[j]);
							PlayerPrefs.SetString(key, mapSaveData[j].map.ToXmlString());
						}
					}
					Rewired.Joystick controller = player.controllers.GetController<Rewired.Joystick>(joystickId);
					JoystickCalibrationMapSaveData calibrationMapSaveData = controller.GetCalibrationMapSaveData();
					key = this.GetJoystickCalibrationMapPlayerPrefsKey(calibrationMapSaveData);
					PlayerPrefs.SetString(key, calibrationMapSaveData.map.ToXmlString());
				}
			}
			IList<Rewired.Joystick> joysticks = ReInput.controllers.Joysticks;
			for (int k = 0; k < joysticks.Count; k++)
			{
				JoystickCalibrationMapSaveData calibrationMapSaveData2 = joysticks[k].GetCalibrationMapSaveData();
				string joystickCalibrationMapPlayerPrefsKey = this.GetJoystickCalibrationMapPlayerPrefsKey(calibrationMapSaveData2);
				PlayerPrefs.SetString(joystickCalibrationMapPlayerPrefsKey, calibrationMapSaveData2.map.ToXmlString());
			}
		}

		private string GetBasePlayerPrefsKey(Player player)
		{
			string str = "UserRemappingDemo";
			return str + "|playerName=" + player.name;
		}

		private string GetControllerMapPlayerPrefsKey(Player player, ControllerMapSaveData saveData)
		{
			string text = this.GetBasePlayerPrefsKey(player);
			text += "|dataType=ControllerMap";
			text = text + "|controllerMapType=" + saveData.mapTypeString;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"|categoryId=",
				saveData.map.categoryId,
				"|layoutId=",
				saveData.map.layoutId
			});
			text = text + "|hardwareIdentifier=" + saveData.controllerHardwareIdentifier;
			if (saveData.mapType == typeof(JoystickMap))
			{
				text = text + "|hardwareGuid=" + ((JoystickMapSaveData)saveData).joystickHardwareTypeGuid.ToString();
			}
			return text;
		}

		private string GetControllerMapXml(Player player, ControllerType controllerType, int categoryId, int layoutId, Controller controller)
		{
			string text = this.GetBasePlayerPrefsKey(player);
			text += "|dataType=ControllerMap";
			text = text + "|controllerMapType=" + controller.mapTypeString;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"|categoryId=",
				categoryId,
				"|layoutId=",
				layoutId
			});
			text = text + "|hardwareIdentifier=" + controller.hardwareIdentifier;
			if (controllerType == ControllerType.Joystick)
			{
				Rewired.Joystick joystick = (Rewired.Joystick)controller;
				text = text + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
			}
			if (!PlayerPrefs.HasKey(text))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(text);
		}

		private List<string> GetAllControllerMapsXml(Player player, bool userAssignableMapsOnly, ControllerType controllerType, Controller controller)
		{
			List<string> list = new List<string>();
			IList<InputMapCategory> mapCategories = ReInput.mapping.MapCategories;
			for (int i = 0; i < mapCategories.Count; i++)
			{
				InputMapCategory inputMapCategory = mapCategories[i];
				if (!userAssignableMapsOnly || inputMapCategory.userAssignable)
				{
					IList<InputLayout> list2 = ReInput.mapping.MapLayouts(controllerType);
					for (int j = 0; j < list2.Count; j++)
					{
						InputLayout inputLayout = list2[j];
						string controllerMapXml = this.GetControllerMapXml(player, controllerType, inputMapCategory.id, inputLayout.id, controller);
						if (!(controllerMapXml == string.Empty))
						{
							list.Add(controllerMapXml);
						}
					}
				}
			}
			return list;
		}

		private string GetJoystickCalibrationMapPlayerPrefsKey(JoystickCalibrationMapSaveData saveData)
		{
			string str = "UserRemappingDemo";
			str += "|dataType=CalibrationMap";
			str = str + "|controllerType=" + saveData.controllerType.ToString();
			str = str + "|hardwareIdentifier=" + saveData.hardwareIdentifier;
			return str + "|hardwareGuid=" + saveData.joystickHardwareTypeGuid.ToString();
		}

		private string GetJoystickCalibrationMapXml(Rewired.Joystick joystick)
		{
			string text = "UserRemappingDemo";
			text += "|dataType=CalibrationMap";
			text = text + "|controllerType=" + joystick.type.ToString();
			text = text + "|hardwareIdentifier=" + joystick.hardwareIdentifier;
			text = text + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
			if (!PlayerPrefs.HasKey(text))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(text);
		}

		private string GetInputBehaviorPlayerPrefsKey(Player player, InputBehavior saveData)
		{
			string text = this.GetBasePlayerPrefsKey(player);
			text += "|dataType=InputBehavior";
			return text + "|id=" + saveData.id;
		}

		private string GetInputBehaviorXml(Player player, int id)
		{
			string text = this.GetBasePlayerPrefsKey(player);
			text += "|dataType=InputBehavior";
			text = text + "|id=" + id;
			if (!PlayerPrefs.HasKey(text))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(text);
		}

		public void IdentifyAllJoysticks()
		{
			if (ReInput.controllers.joystickCount == 0)
			{
				return;
			}
			this.ClearWorkingVars();
			this.Open();
			foreach (Rewired.Joystick current in ReInput.controllers.Joysticks)
			{
				this.actionQueue.Enqueue(new ControlRemappingDemo1.FallbackJoystickIdentification(current.id, current.name));
			}
		}

		protected void CheckRecompile()
		{
		}

		private void RecompileWindow(int windowId)
		{
		}
	}
}
