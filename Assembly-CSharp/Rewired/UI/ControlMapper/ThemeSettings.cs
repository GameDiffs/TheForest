using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[Serializable]
	public class ThemeSettings : ScriptableObject
	{
		[Serializable]
		private abstract class SelectableSettings_Base
		{
			[SerializeField]
			protected Selectable.Transition _transition;

			[SerializeField]
			protected ThemeSettings.CustomColorBlock _colors;

			[SerializeField]
			protected ThemeSettings.CustomSpriteState _spriteState;

			[SerializeField]
			protected ThemeSettings.CustomAnimationTriggers _animationTriggers;

			public Selectable.Transition transition
			{
				get
				{
					return this._transition;
				}
			}

			public ThemeSettings.CustomColorBlock selectableColors
			{
				get
				{
					return this._colors;
				}
			}

			public ThemeSettings.CustomSpriteState spriteState
			{
				get
				{
					return this._spriteState;
				}
			}

			public ThemeSettings.CustomAnimationTriggers animationTriggers
			{
				get
				{
					return this._animationTriggers;
				}
			}

			public virtual void Apply(Selectable item)
			{
				Selectable.Transition transition = this._transition;
				bool flag = item.transition != transition;
				item.transition = transition;
				ICustomSelectable customSelectable = item as ICustomSelectable;
				if (transition == Selectable.Transition.ColorTint)
				{
					ThemeSettings.CustomColorBlock colors = this._colors;
					colors.fadeDuration = 0f;
					item.colors = colors;
					colors.fadeDuration = this._colors.fadeDuration;
					item.colors = colors;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedColor = colors.disabledHighlightedColor;
					}
				}
				else if (transition == Selectable.Transition.SpriteSwap)
				{
					item.spriteState = this._spriteState;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedSprite = this._spriteState.disabledHighlightedSprite;
					}
				}
				else if (transition == Selectable.Transition.Animation)
				{
					item.animationTriggers.disabledTrigger = this._animationTriggers.disabledTrigger;
					item.animationTriggers.highlightedTrigger = this._animationTriggers.highlightedTrigger;
					item.animationTriggers.normalTrigger = this._animationTriggers.normalTrigger;
					item.animationTriggers.pressedTrigger = this._animationTriggers.pressedTrigger;
					if (customSelectable != null)
					{
						customSelectable.disabledHighlightedTrigger = this._animationTriggers.disabledHighlightedTrigger;
					}
				}
				if (flag)
				{
					item.targetGraphic.CrossFadeColor(item.targetGraphic.color, 0f, true, true);
				}
			}
		}

		[Serializable]
		private class SelectableSettings : ThemeSettings.SelectableSettings_Base
		{
			[SerializeField]
			private ThemeSettings.ImageSettings _imageSettings;

			public ThemeSettings.ImageSettings imageSettings
			{
				get
				{
					return this._imageSettings;
				}
			}

			public override void Apply(Selectable item)
			{
				if (item == null)
				{
					return;
				}
				base.Apply(item);
				if (this._imageSettings != null)
				{
					this._imageSettings.CopyTo(item.targetGraphic as Image);
				}
			}
		}

		[Serializable]
		private class SliderSettings : ThemeSettings.SelectableSettings_Base
		{
			[SerializeField]
			private ThemeSettings.ImageSettings _handleImageSettings;

			[SerializeField]
			private ThemeSettings.ImageSettings _fillImageSettings;

			[SerializeField]
			private ThemeSettings.ImageSettings _backgroundImageSettings;

			public ThemeSettings.ImageSettings handleImageSettings
			{
				get
				{
					return this._handleImageSettings;
				}
			}

			public ThemeSettings.ImageSettings fillImageSettings
			{
				get
				{
					return this._fillImageSettings;
				}
			}

			public ThemeSettings.ImageSettings backgroundImageSettings
			{
				get
				{
					return this._backgroundImageSettings;
				}
			}

			private void Apply(Slider item)
			{
				if (item == null)
				{
					return;
				}
				if (this._handleImageSettings != null)
				{
					this._handleImageSettings.CopyTo(item.targetGraphic as Image);
				}
				if (this._fillImageSettings != null)
				{
					RectTransform fillRect = item.fillRect;
					if (fillRect != null)
					{
						this._fillImageSettings.CopyTo(fillRect.GetComponent<Image>());
					}
				}
				if (this._backgroundImageSettings != null)
				{
					Transform transform = item.transform.Find("Background");
					if (transform != null)
					{
						this._backgroundImageSettings.CopyTo(transform.GetComponent<Image>());
					}
				}
			}

			public override void Apply(Selectable item)
			{
				this.Apply(item as Slider);
			}
		}

		[Serializable]
		private class ScrollbarSettings : ThemeSettings.SelectableSettings_Base
		{
			[SerializeField]
			private ThemeSettings.ImageSettings _handleImageSettings;

			[SerializeField]
			private ThemeSettings.ImageSettings _backgroundImageSettings;

			public ThemeSettings.ImageSettings handle
			{
				get
				{
					return this._handleImageSettings;
				}
			}

			public ThemeSettings.ImageSettings background
			{
				get
				{
					return this._backgroundImageSettings;
				}
			}

			private void Apply(Scrollbar item)
			{
				if (item == null)
				{
					return;
				}
				if (this._handleImageSettings != null)
				{
					this._handleImageSettings.CopyTo(item.targetGraphic as Image);
				}
				if (this._backgroundImageSettings != null)
				{
					this._backgroundImageSettings.CopyTo(item.GetComponent<Image>());
				}
			}

			public override void Apply(Selectable item)
			{
				base.Apply(item);
				this.Apply(item as Scrollbar);
			}
		}

		[Serializable]
		private class ImageSettings
		{
			[SerializeField]
			private Color _color = Color.white;

			[SerializeField]
			private Sprite _sprite;

			[SerializeField]
			private Material _materal;

			[SerializeField]
			private Image.Type _type;

			[SerializeField]
			private bool _preserveAspect;

			[SerializeField]
			private bool _fillCenter;

			[SerializeField]
			private Image.FillMethod _fillMethod;

			[SerializeField]
			private float _fillAmout;

			[SerializeField]
			private bool _fillClockwise;

			[SerializeField]
			private int _fillOrigin;

			public Color color
			{
				get
				{
					return this._color;
				}
			}

			public Sprite sprite
			{
				get
				{
					return this._sprite;
				}
			}

			public Material materal
			{
				get
				{
					return this._materal;
				}
			}

			public Image.Type type
			{
				get
				{
					return this._type;
				}
			}

			public bool preserveAspect
			{
				get
				{
					return this._preserveAspect;
				}
			}

			public bool fillCenter
			{
				get
				{
					return this._fillCenter;
				}
			}

			public Image.FillMethod fillMethod
			{
				get
				{
					return this._fillMethod;
				}
			}

			public float fillAmout
			{
				get
				{
					return this._fillAmout;
				}
			}

			public bool fillClockwise
			{
				get
				{
					return this._fillClockwise;
				}
			}

			public int fillOrigin
			{
				get
				{
					return this._fillOrigin;
				}
			}

			public virtual void CopyTo(Image image)
			{
				if (image == null)
				{
					return;
				}
				image.color = this._color;
				image.sprite = this._sprite;
				image.material = this._materal;
				image.type = this._type;
				image.preserveAspect = this._preserveAspect;
				image.fillCenter = this._fillCenter;
				image.fillMethod = this._fillMethod;
				image.fillAmount = this._fillAmout;
				image.fillClockwise = this._fillClockwise;
				image.fillOrigin = this._fillOrigin;
			}
		}

		[Serializable]
		private struct CustomColorBlock
		{
			[SerializeField]
			private float m_ColorMultiplier;

			[SerializeField]
			private Color m_DisabledColor;

			[SerializeField]
			private float m_FadeDuration;

			[SerializeField]
			private Color m_HighlightedColor;

			[SerializeField]
			private Color m_NormalColor;

			[SerializeField]
			private Color m_PressedColor;

			[SerializeField]
			private Color m_DisabledHighlightedColor;

			public float colorMultiplier
			{
				get
				{
					return this.m_ColorMultiplier;
				}
				set
				{
					this.m_ColorMultiplier = value;
				}
			}

			public Color disabledColor
			{
				get
				{
					return this.m_DisabledColor;
				}
				set
				{
					this.m_DisabledColor = value;
				}
			}

			public float fadeDuration
			{
				get
				{
					return this.m_FadeDuration;
				}
				set
				{
					this.m_FadeDuration = value;
				}
			}

			public Color highlightedColor
			{
				get
				{
					return this.m_HighlightedColor;
				}
				set
				{
					this.m_HighlightedColor = value;
				}
			}

			public Color normalColor
			{
				get
				{
					return this.m_NormalColor;
				}
				set
				{
					this.m_NormalColor = value;
				}
			}

			public Color pressedColor
			{
				get
				{
					return this.m_PressedColor;
				}
				set
				{
					this.m_PressedColor = value;
				}
			}

			public Color disabledHighlightedColor
			{
				get
				{
					return this.m_DisabledHighlightedColor;
				}
				set
				{
					this.m_DisabledHighlightedColor = value;
				}
			}

			public static implicit operator ColorBlock(ThemeSettings.CustomColorBlock item)
			{
				return new ColorBlock
				{
					colorMultiplier = item.m_ColorMultiplier,
					disabledColor = item.m_DisabledColor,
					fadeDuration = item.m_FadeDuration,
					highlightedColor = item.m_HighlightedColor,
					normalColor = item.m_NormalColor,
					pressedColor = item.m_PressedColor
				};
			}
		}

		[Serializable]
		private struct CustomSpriteState
		{
			[SerializeField]
			private Sprite m_DisabledSprite;

			[SerializeField]
			private Sprite m_HighlightedSprite;

			[SerializeField]
			private Sprite m_PressedSprite;

			[SerializeField]
			private Sprite m_DisabledHighlightedSprite;

			public Sprite disabledSprite
			{
				get
				{
					return this.m_DisabledSprite;
				}
				set
				{
					this.m_DisabledSprite = value;
				}
			}

			public Sprite highlightedSprite
			{
				get
				{
					return this.m_HighlightedSprite;
				}
				set
				{
					this.m_HighlightedSprite = value;
				}
			}

			public Sprite pressedSprite
			{
				get
				{
					return this.m_PressedSprite;
				}
				set
				{
					this.m_PressedSprite = value;
				}
			}

			public Sprite disabledHighlightedSprite
			{
				get
				{
					return this.m_DisabledHighlightedSprite;
				}
				set
				{
					this.m_DisabledHighlightedSprite = value;
				}
			}

			public static implicit operator SpriteState(ThemeSettings.CustomSpriteState item)
			{
				return new SpriteState
				{
					disabledSprite = item.m_DisabledSprite,
					highlightedSprite = item.m_HighlightedSprite,
					pressedSprite = item.m_PressedSprite
				};
			}
		}

		[Serializable]
		private class CustomAnimationTriggers
		{
			[SerializeField]
			private string m_DisabledTrigger;

			[SerializeField]
			private string m_HighlightedTrigger;

			[SerializeField]
			private string m_NormalTrigger;

			[SerializeField]
			private string m_PressedTrigger;

			[SerializeField]
			private string m_DisabledHighlightedTrigger;

			public string disabledTrigger
			{
				get
				{
					return this.m_DisabledTrigger;
				}
				set
				{
					this.m_DisabledTrigger = value;
				}
			}

			public string highlightedTrigger
			{
				get
				{
					return this.m_HighlightedTrigger;
				}
				set
				{
					this.m_HighlightedTrigger = value;
				}
			}

			public string normalTrigger
			{
				get
				{
					return this.m_NormalTrigger;
				}
				set
				{
					this.m_NormalTrigger = value;
				}
			}

			public string pressedTrigger
			{
				get
				{
					return this.m_PressedTrigger;
				}
				set
				{
					this.m_PressedTrigger = value;
				}
			}

			public string disabledHighlightedTrigger
			{
				get
				{
					return this.m_DisabledHighlightedTrigger;
				}
				set
				{
					this.m_DisabledHighlightedTrigger = value;
				}
			}

			public CustomAnimationTriggers()
			{
				this.m_DisabledTrigger = string.Empty;
				this.m_HighlightedTrigger = string.Empty;
				this.m_NormalTrigger = string.Empty;
				this.m_PressedTrigger = string.Empty;
				this.m_DisabledHighlightedTrigger = string.Empty;
			}

			public static implicit operator AnimationTriggers(ThemeSettings.CustomAnimationTriggers item)
			{
				return new AnimationTriggers
				{
					disabledTrigger = item.m_DisabledTrigger,
					highlightedTrigger = item.m_HighlightedTrigger,
					normalTrigger = item.m_NormalTrigger,
					pressedTrigger = item.m_PressedTrigger
				};
			}
		}

		[Serializable]
		private class TextSettings
		{
			[SerializeField]
			private Color _color = Color.white;

			[SerializeField]
			private Font _font;

			public Color color
			{
				get
				{
					return this._color;
				}
			}

			public Font font
			{
				get
				{
					return this._font;
				}
			}
		}

		[SerializeField]
		private ThemeSettings.ImageSettings _mainWindowBackground;

		[SerializeField]
		private ThemeSettings.ImageSettings _popupWindowBackground;

		[SerializeField]
		private ThemeSettings.ImageSettings _areaBackground;

		[SerializeField]
		private ThemeSettings.SelectableSettings _selectableSettings;

		[SerializeField]
		private ThemeSettings.SelectableSettings _buttonSettings;

		[SerializeField]
		private ThemeSettings.SelectableSettings _inputGridFieldSettings;

		[SerializeField]
		private ThemeSettings.ScrollbarSettings _scrollbarSettings;

		[SerializeField]
		private ThemeSettings.SliderSettings _sliderSettings;

		[SerializeField]
		private ThemeSettings.ImageSettings _invertToggle;

		[SerializeField]
		private Color _invertToggleDisabledColor;

		[SerializeField]
		private ThemeSettings.ImageSettings _calibrationValueMarker;

		[SerializeField]
		private ThemeSettings.ImageSettings _calibrationRawValueMarker;

		[SerializeField]
		private ThemeSettings.TextSettings _textSettings;

		[SerializeField]
		private ThemeSettings.TextSettings _buttonTextSettings;

		[SerializeField]
		private ThemeSettings.TextSettings _inputGridFieldTextSettings;

		public void Apply(ThemedElement.ElementInfo[] elementInfo)
		{
			if (elementInfo == null)
			{
				return;
			}
			for (int i = 0; i < elementInfo.Length; i++)
			{
				if (elementInfo[i] != null)
				{
					this.Apply(elementInfo[i].themeClass, elementInfo[i].component);
				}
			}
		}

		private void Apply(string themeClass, Component component)
		{
			if (component as Selectable != null)
			{
				this.Apply(themeClass, (Selectable)component);
				return;
			}
			if (component as Image != null)
			{
				this.Apply(themeClass, (Image)component);
				return;
			}
			if (component as Text != null)
			{
				this.Apply(themeClass, (Text)component);
				return;
			}
			if (component as UIImageHelper != null)
			{
				this.Apply(themeClass, (UIImageHelper)component);
				return;
			}
		}

		private void Apply(string themeClass, Selectable item)
		{
			if (item == null)
			{
				return;
			}
			ThemeSettings.SelectableSettings_Base selectableSettings_Base;
			if (item as UnityEngine.UI.Button != null)
			{
				if (themeClass != null)
				{
					if (ThemeSettings.<>f__switch$map14 == null)
					{
						ThemeSettings.<>f__switch$map14 = new Dictionary<string, int>(1)
						{
							{
								"inputGridField",
								0
							}
						};
					}
					int num;
					if (ThemeSettings.<>f__switch$map14.TryGetValue(themeClass, out num))
					{
						if (num == 0)
						{
							selectableSettings_Base = this._inputGridFieldSettings;
							goto IL_7E;
						}
					}
				}
				selectableSettings_Base = this._buttonSettings;
				IL_7E:;
			}
			else if (item as Scrollbar != null)
			{
				selectableSettings_Base = this._scrollbarSettings;
			}
			else if (item as Slider != null)
			{
				selectableSettings_Base = this._sliderSettings;
			}
			else if (item as Toggle != null)
			{
				if (themeClass != null)
				{
					if (ThemeSettings.<>f__switch$map15 == null)
					{
						ThemeSettings.<>f__switch$map15 = new Dictionary<string, int>(1)
						{
							{
								"button",
								0
							}
						};
					}
					int num;
					if (ThemeSettings.<>f__switch$map15.TryGetValue(themeClass, out num))
					{
						if (num == 0)
						{
							selectableSettings_Base = this._buttonSettings;
							goto IL_12E;
						}
					}
				}
				selectableSettings_Base = this._selectableSettings;
				IL_12E:;
			}
			else
			{
				selectableSettings_Base = this._selectableSettings;
			}
			selectableSettings_Base.Apply(item);
		}

		private void Apply(string themeClass, Image item)
		{
			if (item == null)
			{
				return;
			}
			switch (themeClass)
			{
			case "area":
				this._areaBackground.CopyTo(item);
				break;
			case "popupWindow":
				this._popupWindowBackground.CopyTo(item);
				break;
			case "mainWindow":
				this._mainWindowBackground.CopyTo(item);
				break;
			case "calibrationValueMarker":
				this._calibrationValueMarker.CopyTo(item);
				break;
			case "calibrationRawValueMarker":
				this._calibrationRawValueMarker.CopyTo(item);
				break;
			case "invertToggle":
				this._invertToggle.CopyTo(item);
				break;
			case "invertToggleBackground":
				this._inputGridFieldSettings.imageSettings.CopyTo(item);
				break;
			case "invertToggleButtonBackground":
				this._buttonSettings.imageSettings.CopyTo(item);
				break;
			}
		}

		private void Apply(string themeClass, Text item)
		{
			if (item == null)
			{
				return;
			}
			ThemeSettings.TextSettings textSettings;
			if (themeClass != null)
			{
				if (ThemeSettings.<>f__switch$map17 == null)
				{
					ThemeSettings.<>f__switch$map17 = new Dictionary<string, int>(2)
					{
						{
							"button",
							0
						},
						{
							"inputGridField",
							1
						}
					};
				}
				int num;
				if (ThemeSettings.<>f__switch$map17.TryGetValue(themeClass, out num))
				{
					if (num == 0)
					{
						textSettings = this._buttonTextSettings;
						goto IL_8C;
					}
					if (num == 1)
					{
						textSettings = this._inputGridFieldTextSettings;
						goto IL_8C;
					}
				}
			}
			textSettings = this._textSettings;
			IL_8C:
			if (textSettings.font != null)
			{
				item.font = textSettings.font;
			}
			item.color = textSettings.color;
		}

		private void Apply(string themeClass, UIImageHelper item)
		{
			if (item == null)
			{
				return;
			}
			item.SetEnabledStateColor(this._invertToggle.color);
			item.SetDisabledStateColor(this._invertToggleDisabledColor);
			item.Refresh();
		}
	}
}
