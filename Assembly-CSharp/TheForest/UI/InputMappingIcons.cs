using Rewired;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.UI
{
	public class InputMappingIcons : MonoBehaviour
	{
		public enum Actions
		{
			None = -1,
			Inventory,
			SurvivalBook,
			Horizontal,
			Right,
			Left,
			Vertical,
			Up,
			Down,
			Jump,
			Mouse_X,
			LookRight,
			LookLeft,
			Mouse_Y,
			LookUp,
			LookDown,
			Run,
			Crouch,
			Fire1,
			AltFire,
			Take,
			Drop,
			RestKey,
			Save,
			Lighter,
			Craft,
			Utility,
			WalkyTalky,
			Rotate,
			Batch,
			OpenChat,
			Submit,
			Esc,
			Build,
			Combine,
			Equip,
			PreviousChapter,
			NextChapter,
			PreviousPage,
			NextPage,
			Back,
			Map,
			Scroll
		}

		public Texture2D[] _textures;

		public Texture2D _textIconBacking;

		private bool _usingGamePadVersion;

		public static int Version;

		public static Texture2D TextIconBacking;

		private static Dictionary<string, Texture2D> TexturesByName;

		private static string[] KeyboardMappings = new string[Enum.GetNames(typeof(InputMappingIcons.Actions)).Length];

		private static string[] GamepadMappings = new string[Enum.GetNames(typeof(InputMappingIcons.Actions)).Length];

		private static Controller LastController;

		private void Start()
		{
			InputMappingIcons.TextIconBacking = this._textIconBacking;
			InputMappingIcons.TexturesByName = this._textures.ToDictionary((Texture2D t) => t.name);
			InputMappingIcons.Version = 1;
			InputMappingIcons.RefreshMappings();
		}

		private void Update()
		{
			if (TheForest.Utils.Input.IsGamePad)
			{
				Controller lastActiveController = TheForest.Utils.Input.player.controllers.GetLastActiveController();
				if (lastActiveController != InputMappingIcons.LastController)
				{
					InputMappingIcons.LastController = lastActiveController;
					InputMappingIcons.RefreshMappings();
					InputMappingIcons.Version++;
				}
			}
			if (this._usingGamePadVersion != TheForest.Utils.Input.IsGamePad)
			{
				this._usingGamePadVersion = TheForest.Utils.Input.IsGamePad;
				InputMappingIcons.Version++;
			}
		}

		public static bool UsesText(InputMappingIcons.Actions action)
		{
			if (TheForest.Utils.Input.IsGamePad)
			{
				return false;
			}
			string text = InputMappingIcons.KeyboardMappings[(int)action];
			if (text != null)
			{
				if (InputMappingIcons.<>f__switch$map18 == null)
				{
					InputMappingIcons.<>f__switch$map18 = new Dictionary<string, int>(4)
					{
						{
							"Right_Mouse_Button",
							0
						},
						{
							"Left_Mouse_Button",
							0
						},
						{
							"Mouse_Horizontal",
							0
						},
						{
							"Mouse_Vertical",
							0
						}
					};
				}
				int num;
				if (InputMappingIcons.<>f__switch$map18.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static Texture GetTextureFor(InputMappingIcons.Actions action)
		{
			if (TheForest.Utils.Input.IsGamePad)
			{
				return InputMappingIcons.TexturesByName[InputMappingIcons.GetMappingFor(action)];
			}
			string text = InputMappingIcons.KeyboardMappings[(int)action];
			if (text != null)
			{
				if (InputMappingIcons.<>f__switch$map19 == null)
				{
					InputMappingIcons.<>f__switch$map19 = new Dictionary<string, int>(4)
					{
						{
							"Right_Mouse_Button",
							0
						},
						{
							"Left_Mouse_Button",
							0
						},
						{
							"Mouse_Horizontal",
							0
						},
						{
							"Mouse_Vertical",
							0
						}
					};
				}
				int num;
				if (InputMappingIcons.<>f__switch$map19.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						return InputMappingIcons.TexturesByName[InputMappingIcons.KeyboardMappings[(int)action]];
					}
				}
			}
			return InputMappingIcons.TextIconBacking;
		}

		public static string GetMappingFor(InputMappingIcons.Actions action)
		{
			if (TheForest.Utils.Input.IsGamePad)
			{
				return InputMappingIcons.GamepadMappings[(int)action];
			}
			return InputMappingIcons.KeyboardMappings[(int)action];
		}

		public static void RefreshMappings()
		{
			Debug.Log("Refreshing Input Mapping Icons");
			foreach (ControllerMap current in ReInput.players.GetPlayer(0).controllers.maps.GetAllMaps(ControllerType.Mouse))
			{
				foreach (ActionElementMap current2 in current.AllMaps)
				{
					if (!string.IsNullOrEmpty(current2.elementIdentifierName))
					{
						try
						{
							string text = current2.elementIdentifierName.Replace(' ', '_');
							InputAction action = ReInput.mapping.GetAction(current2.actionId);
							if (action.type == InputActionType.Axis)
							{
								InputMappingIcons.KeyboardMappings[(int)Enum.Parse(typeof(InputMappingIcons.Actions), ((current2.axisContribution != Pole.Positive) ? action.negativeDescriptiveName : action.positiveDescriptiveName).Replace(' ', '_'))] = text;
							}
							InputMappingIcons.KeyboardMappings[(int)Enum.Parse(typeof(InputMappingIcons.Actions), action.name.Replace(' ', '_'))] = text;
						}
						catch
						{
						}
					}
				}
			}
			foreach (ControllerMap current3 in ReInput.players.GetPlayer(0).controllers.maps.GetAllMaps(ControllerType.Keyboard))
			{
				foreach (ActionElementMap current4 in current3.AllMaps)
				{
					if (!string.IsNullOrEmpty(current4.elementIdentifierName))
					{
						try
						{
							string elementIdentifierName = current4.elementIdentifierName;
							string text2;
							if (elementIdentifierName != null)
							{
								if (InputMappingIcons.<>f__switch$map1A == null)
								{
									InputMappingIcons.<>f__switch$map1A = new Dictionary<string, int>(2)
									{
										{
											"Left Control",
											0
										},
										{
											"Right Control",
											1
										}
									};
								}
								int num;
								if (InputMappingIcons.<>f__switch$map1A.TryGetValue(elementIdentifierName, out num))
								{
									if (num == 0)
									{
										text2 = "LCtrl";
										goto IL_22B;
									}
									if (num == 1)
									{
										text2 = "RCtrl";
										goto IL_22B;
									}
								}
							}
							text2 = current4.elementIdentifierName;
							IL_22B:
							InputAction action2 = ReInput.mapping.GetAction(current4.actionId);
							if (action2.type == InputActionType.Axis)
							{
								InputMappingIcons.KeyboardMappings[(int)Enum.Parse(typeof(InputMappingIcons.Actions), ((current4.axisContribution != Pole.Positive) ? action2.negativeDescriptiveName : action2.positiveDescriptiveName).Replace(' ', '_'))] = text2;
							}
							InputMappingIcons.KeyboardMappings[(int)Enum.Parse(typeof(InputMappingIcons.Actions), action2.name.Replace(' ', '_'))] = text2;
						}
						catch
						{
						}
					}
				}
			}
			if (InputMappingIcons.LastController != null)
			{
				bool flag = InputMappingIcons.LastController.name.Contains("DualShock");
				foreach (ControllerMap current5 in ReInput.players.GetPlayer(0).controllers.maps.GetAllMaps(ControllerType.Joystick))
				{
					if (current5.controllerId == InputMappingIcons.LastController.id)
					{
						foreach (ActionElementMap current6 in current5.AllMaps)
						{
							if (!string.IsNullOrEmpty(current6.elementIdentifierName))
							{
								try
								{
									string elementIdentifierName = current6.elementIdentifierName;
									string text3;
									if (elementIdentifierName != null)
									{
										if (InputMappingIcons.<>f__switch$map1B == null)
										{
											InputMappingIcons.<>f__switch$map1B = new Dictionary<string, int>(4)
											{
												{
													"Right Stick X",
													0
												},
												{
													"Right Stick Y",
													0
												},
												{
													"Left Stick X",
													1
												},
												{
													"Left Stick Y",
													1
												}
											};
										}
										int num;
										if (InputMappingIcons.<>f__switch$map1B.TryGetValue(elementIdentifierName, out num))
										{
											if (num == 0)
											{
												text3 = "Right_Stick_Button";
												goto IL_47F;
											}
											if (num == 1)
											{
												text3 = "Left_Stick_Button";
												goto IL_47F;
											}
										}
									}
									text3 = current6.elementIdentifierName.Replace(" X", string.Empty).Replace(" Y", string.Empty).TrimEnd(new char[]
									{
										' ',
										'+',
										'-'
									}).Replace(' ', '_');
									IL_47F:
									InputAction action3 = ReInput.mapping.GetAction(current6.actionId);
									if (flag)
									{
										text3 = "PS4_" + text3;
										if (action3.type == InputActionType.Axis)
										{
											InputMappingIcons.GamepadMappings[(int)Enum.Parse(typeof(InputMappingIcons.Actions), ((current6.axisContribution != Pole.Positive) ? action3.negativeDescriptiveName : action3.positiveDescriptiveName).Replace(' ', '_'))] = text3;
										}
										InputMappingIcons.GamepadMappings[(int)Enum.Parse(typeof(InputMappingIcons.Actions), action3.name.Replace(' ', '_'))] = text3;
									}
									else
									{
										text3 = "360_" + text3;
										if (action3.type == InputActionType.Axis)
										{
											InputMappingIcons.GamepadMappings[(int)Enum.Parse(typeof(InputMappingIcons.Actions), ((current6.axisContribution != Pole.Positive) ? action3.negativeDescriptiveName : action3.positiveDescriptiveName).Replace(' ', '_'))] = text3;
										}
										InputMappingIcons.GamepadMappings[(int)Enum.Parse(typeof(InputMappingIcons.Actions), action3.name.Replace(' ', '_'))] = text3;
										Debug.Log(Enum.Parse(typeof(InputMappingIcons.Actions), ReInput.mapping.GetAction(current6.actionId).name.Replace(' ', '_')) + " -> " + text3);
									}
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			InputMappingIcons.Version++;
		}
	}
}
