using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Popup List"), ExecuteInEditMode]
public class UIPopupList : UIWidgetContainer
{
	public enum Position
	{
		Auto,
		Above,
		Below
	}

	public enum OpenOn
	{
		ClickOrTap,
		RightClick,
		DoubleClick,
		Manual
	}

	public delegate void LegacyEvent(string val);

	private const float animSpeed = 0.15f;

	public static UIPopupList current;

	private static GameObject mChild;

	private static float mFadeOutComplete;

	public UIAtlas atlas;

	public UIFont bitmapFont;

	public Font trueTypeFont;

	public int fontSize = 16;

	public FontStyle fontStyle;

	public string backgroundSprite;

	public string highlightSprite;

	public UIPopupList.Position position;

	public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	public List<string> items = new List<string>();

	public List<object> itemData = new List<object>();

	public Vector2 padding = new Vector3(4f, 4f);

	public Color textColor = Color.white;

	public Color backgroundColor = Color.white;

	public Color highlightColor = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	public bool isAnimated = true;

	public bool isLocalized;

	public UIPopupList.OpenOn openOn;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[HideInInspector, SerializeField]
	private string mSelectedItem;

	[HideInInspector, SerializeField]
	private UIPanel mPanel;

	[HideInInspector, SerializeField]
	private UISprite mBackground;

	[HideInInspector, SerializeField]
	private UISprite mHighlight;

	[HideInInspector, SerializeField]
	private UILabel mHighlightedLabel;

	[HideInInspector, SerializeField]
	private List<UILabel> mLabelList = new List<UILabel>();

	[HideInInspector, SerializeField]
	private float mBgBorder;

	[NonSerialized]
	private GameObject mSelection;

	[NonSerialized]
	private int mOpenFrame;

	[HideInInspector, SerializeField]
	private GameObject eventReceiver;

	[HideInInspector, SerializeField]
	private string functionName = "OnSelectionChange";

	[HideInInspector, SerializeField]
	private float textScale;

	[HideInInspector, SerializeField]
	private UIFont font;

	[HideInInspector, SerializeField]
	private UILabel textLabel;

	private UIPopupList.LegacyEvent mLegacyEvent;

	[NonSerialized]
	private bool mExecuting;

	private bool mUseDynamicFont;

	private bool mTweening;

	public GameObject source;

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			if (this.trueTypeFont != null)
			{
				return this.trueTypeFont;
			}
			if (this.bitmapFont != null)
			{
				return this.bitmapFont;
			}
			return this.font;
		}
		set
		{
			if (value is Font)
			{
				this.trueTypeFont = (value as Font);
				this.bitmapFont = null;
				this.font = null;
			}
			else if (value is UIFont)
			{
				this.bitmapFont = (value as UIFont);
				this.trueTypeFont = null;
				this.font = null;
			}
		}
	}

	[Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupList.current.value to determine the state")]
	public UIPopupList.LegacyEvent onSelectionChange
	{
		get
		{
			return this.mLegacyEvent;
		}
		set
		{
			this.mLegacyEvent = value;
		}
	}

	public static bool isOpen
	{
		get
		{
			return UIPopupList.current != null && (UIPopupList.mChild != null || UIPopupList.mFadeOutComplete > Time.unscaledTime);
		}
	}

	public string value
	{
		get
		{
			return this.mSelectedItem;
		}
		set
		{
			this.mSelectedItem = value;
			if (this.mSelectedItem == null)
			{
				return;
			}
			if (this.mSelectedItem != null)
			{
				this.TriggerCallbacks();
			}
		}
	}

	public object data
	{
		get
		{
			int num = this.items.IndexOf(this.mSelectedItem);
			return (num <= -1 || num >= this.itemData.Count) ? null : this.itemData[num];
		}
	}

	public bool isColliderEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	[Obsolete("Use 'value' instead")]
	public string selection
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	private bool isValid
	{
		get
		{
			return this.bitmapFont != null || this.trueTypeFont != null;
		}
	}

	private int activeFontSize
	{
		get
		{
			return (!(this.trueTypeFont != null) && !(this.bitmapFont == null)) ? this.bitmapFont.defaultSize : this.fontSize;
		}
	}

	private float activeFontScale
	{
		get
		{
			return (!(this.trueTypeFont != null) && !(this.bitmapFont == null)) ? ((float)this.fontSize / (float)this.bitmapFont.defaultSize) : 1f;
		}
	}

	public void Clear()
	{
		this.items.Clear();
		this.itemData.Clear();
	}

	public void AddItem(string text)
	{
		this.items.Add(text);
		this.itemData.Add(null);
	}

	public void AddItem(string text, object data)
	{
		this.items.Add(text);
		this.itemData.Add(data);
	}

	public void RemoveItem(string text)
	{
		int num = this.items.IndexOf(text);
		if (num != -1)
		{
			this.items.RemoveAt(num);
			this.itemData.RemoveAt(num);
		}
	}

	public void RemoveItemByData(object data)
	{
		int num = this.itemData.IndexOf(data);
		if (num != -1)
		{
			this.items.RemoveAt(num);
			this.itemData.RemoveAt(num);
		}
	}

	protected void TriggerCallbacks()
	{
		if (!this.mExecuting)
		{
			this.mExecuting = true;
			UIPopupList uIPopupList = UIPopupList.current;
			UIPopupList.current = this;
			if (this.mLegacyEvent != null)
			{
				this.mLegacyEvent(this.mSelectedItem);
			}
			if (EventDelegate.IsValid(this.onChange))
			{
				EventDelegate.Execute(this.onChange);
			}
			else if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
			{
				this.eventReceiver.SendMessage(this.functionName, this.mSelectedItem, SendMessageOptions.DontRequireReceiver);
			}
			UIPopupList.current = uIPopupList;
			this.mExecuting = false;
		}
	}

	private void OnEnable()
	{
		if (EventDelegate.IsValid(this.onChange))
		{
			this.eventReceiver = null;
			this.functionName = null;
		}
		if (this.font != null)
		{
			if (this.font.isDynamic)
			{
				this.trueTypeFont = this.font.dynamicFont;
				this.fontStyle = this.font.dynamicFontStyle;
				this.mUseDynamicFont = true;
			}
			else if (this.bitmapFont == null)
			{
				this.bitmapFont = this.font;
				this.mUseDynamicFont = false;
			}
			this.font = null;
		}
		if (this.textScale != 0f)
		{
			this.fontSize = ((!(this.bitmapFont != null)) ? 16 : Mathf.RoundToInt((float)this.bitmapFont.defaultSize * this.textScale));
			this.textScale = 0f;
		}
		if (this.trueTypeFont == null && this.bitmapFont != null && this.bitmapFont.isDynamic)
		{
			this.trueTypeFont = this.bitmapFont.dynamicFont;
			this.bitmapFont = null;
		}
	}

	private void OnValidate()
	{
		Font x = this.trueTypeFont;
		UIFont uIFont = this.bitmapFont;
		this.bitmapFont = null;
		this.trueTypeFont = null;
		if (x != null && (uIFont == null || !this.mUseDynamicFont))
		{
			this.bitmapFont = null;
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
		else if (uIFont != null)
		{
			if (uIFont.isDynamic)
			{
				this.trueTypeFont = uIFont.dynamicFont;
				this.fontStyle = uIFont.dynamicFontStyle;
				this.fontSize = uIFont.defaultSize;
				this.mUseDynamicFont = true;
			}
			else
			{
				this.bitmapFont = uIFont;
				this.mUseDynamicFont = false;
			}
		}
		else
		{
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
	}

	private void Start()
	{
		if (this.textLabel != null)
		{
			EventDelegate.Add(this.onChange, new EventDelegate.Callback(this.textLabel.SetCurrentSelection));
			this.textLabel = null;
		}
		if (Application.isPlaying)
		{
			if (string.IsNullOrEmpty(this.mSelectedItem) && this.items.Count > 0)
			{
				this.mSelectedItem = this.items[0];
			}
			if (!string.IsNullOrEmpty(this.mSelectedItem))
			{
				this.TriggerCallbacks();
			}
		}
	}

	private void OnLocalize()
	{
		if (this.isLocalized)
		{
			this.TriggerCallbacks();
		}
	}

	private void Highlight(UILabel lbl, bool instant)
	{
		if (this.mHighlight != null)
		{
			this.mHighlightedLabel = lbl;
			if (this.mHighlight.GetAtlasSprite() == null)
			{
				return;
			}
			Vector3 highlightPosition = this.GetHighlightPosition();
			if (!instant && this.isAnimated)
			{
				TweenPosition.Begin(this.mHighlight.gameObject, 0.1f, highlightPosition).method = UITweener.Method.EaseOut;
				if (!this.mTweening)
				{
					this.mTweening = true;
					base.StartCoroutine("UpdateTweenPosition");
				}
			}
			else
			{
				this.mHighlight.cachedTransform.localPosition = highlightPosition;
			}
		}
	}

	private Vector3 GetHighlightPosition()
	{
		if (this.mHighlightedLabel == null || this.mHighlight == null)
		{
			return Vector3.zero;
		}
		UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
		if (atlasSprite == null)
		{
			return Vector3.zero;
		}
		float pixelSize = this.atlas.pixelSize;
		float num = (float)atlasSprite.borderLeft * pixelSize;
		float y = (float)atlasSprite.borderTop * pixelSize;
		return this.mHighlightedLabel.cachedTransform.localPosition + new Vector3(-num, y, 1f);
	}

	[DebuggerHidden]
	private IEnumerator UpdateTweenPosition()
	{
		UIPopupList.<UpdateTweenPosition>c__Iterator10D <UpdateTweenPosition>c__Iterator10D = new UIPopupList.<UpdateTweenPosition>c__Iterator10D();
		<UpdateTweenPosition>c__Iterator10D.<>f__this = this;
		return <UpdateTweenPosition>c__Iterator10D;
	}

	private void OnItemHover(GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel component = go.GetComponent<UILabel>();
			this.Highlight(component, false);
		}
	}

	private void OnItemPress(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			this.Select(go.GetComponent<UILabel>(), true);
			UIEventListener component = go.GetComponent<UIEventListener>();
			this.value = (component.parameter as string);
			UIPlaySound[] components = base.GetComponents<UIPlaySound>();
			int i = 0;
			int num = components.Length;
			while (i < num)
			{
				UIPlaySound uIPlaySound = components[i];
				if (uIPlaySound.trigger == UIPlaySound.Trigger.OnClick)
				{
					NGUITools.PlaySound(uIPlaySound.audioClip, uIPlaySound.volume, 1f);
				}
				i++;
			}
			this.CloseSelf();
		}
	}

	private void Select(UILabel lbl, bool instant)
	{
		this.Highlight(lbl, instant);
	}

	private void OnNavigate(KeyCode key)
	{
		if (base.enabled && UIPopupList.current == this)
		{
			int num = this.mLabelList.IndexOf(this.mHighlightedLabel);
			if (num == -1)
			{
				num = 0;
			}
			if (key == KeyCode.UpArrow)
			{
				if (num > 0)
				{
					this.Select(this.mLabelList[num - 1], false);
				}
			}
			else if (key == KeyCode.DownArrow && num + 1 < this.mLabelList.Count)
			{
				this.Select(this.mLabelList[num + 1], false);
			}
		}
	}

	private void OnKey(KeyCode key)
	{
		if (base.enabled && UIPopupList.current == this && (key == UICamera.current.cancelKey0 || key == UICamera.current.cancelKey1))
		{
			this.OnSelect(false);
		}
	}

	private void OnDisable()
	{
		this.CloseSelf();
	}

	private void OnSelect(bool isSelected)
	{
		if (!isSelected)
		{
			this.CloseSelf();
		}
	}

	public static void Close()
	{
		if (UIPopupList.current != null)
		{
			UIPopupList.current.CloseSelf();
			UIPopupList.current = null;
		}
	}

	public void CloseSelf()
	{
		if (UIPopupList.mChild != null && UIPopupList.current == this)
		{
			base.StopCoroutine("CloseIfUnselected");
			this.mSelection = null;
			this.mLabelList.Clear();
			if (this.isAnimated)
			{
				UIWidget[] componentsInChildren = UIPopupList.mChild.GetComponentsInChildren<UIWidget>();
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					UIWidget uIWidget = componentsInChildren[i];
					Color color = uIWidget.color;
					color.a = 0f;
					TweenColor.Begin(uIWidget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
					i++;
				}
				Collider[] componentsInChildren2 = UIPopupList.mChild.GetComponentsInChildren<Collider>();
				int j = 0;
				int num2 = componentsInChildren2.Length;
				while (j < num2)
				{
					componentsInChildren2[j].enabled = false;
					j++;
				}
				UnityEngine.Object.Destroy(UIPopupList.mChild, 0.15f);
				UIPopupList.mFadeOutComplete = Time.unscaledTime + Mathf.Max(0.1f, 0.15f);
			}
			else
			{
				UnityEngine.Object.Destroy(UIPopupList.mChild);
				UIPopupList.mFadeOutComplete = Time.unscaledTime + 0.1f;
			}
			this.mBackground = null;
			this.mHighlight = null;
			UIPopupList.mChild = null;
			UIPopupList.current = null;
		}
	}

	private void AnimateColor(UIWidget widget)
	{
		Color color = widget.color;
		widget.color = new Color(color.r, color.g, color.b, 0f);
		TweenColor.Begin(widget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
	}

	private void AnimatePosition(UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 localPosition = widget.cachedTransform.localPosition;
		Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
		widget.cachedTransform.localPosition = localPosition2;
		GameObject gameObject = widget.gameObject;
		TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
	}

	private void AnimateScale(UIWidget widget, bool placeAbove, float bottom)
	{
		GameObject gameObject = widget.gameObject;
		Transform cachedTransform = widget.cachedTransform;
		float num = (float)this.activeFontSize * this.activeFontScale + this.mBgBorder * 2f;
		cachedTransform.localScale = new Vector3(1f, num / (float)widget.height, 1f);
		TweenScale.Begin(gameObject, 0.15f, Vector3.one).method = UITweener.Method.EaseOut;
		if (placeAbove)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y - (float)widget.height + num, localPosition.z);
			TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
		}
	}

	private void Animate(UIWidget widget, bool placeAbove, float bottom)
	{
		this.AnimateColor(widget);
		this.AnimatePosition(widget, placeAbove, bottom);
	}

	private void OnClick()
	{
		if (this.mOpenFrame == Time.frameCount)
		{
			return;
		}
		if (UIPopupList.mChild == null)
		{
			if (this.openOn == UIPopupList.OpenOn.DoubleClick || this.openOn == UIPopupList.OpenOn.Manual)
			{
				return;
			}
			if (this.openOn == UIPopupList.OpenOn.RightClick && UICamera.currentTouchID != -2)
			{
				return;
			}
			this.Show();
		}
		else if (this.mHighlightedLabel != null)
		{
			this.OnItemPress(this.mHighlightedLabel.gameObject, true);
		}
	}

	private void OnDoubleClick()
	{
		if (this.openOn == UIPopupList.OpenOn.DoubleClick)
		{
			this.Show();
		}
	}

	[DebuggerHidden]
	private IEnumerator CloseIfUnselected()
	{
		UIPopupList.<CloseIfUnselected>c__Iterator10E <CloseIfUnselected>c__Iterator10E = new UIPopupList.<CloseIfUnselected>c__Iterator10E();
		<CloseIfUnselected>c__Iterator10E.<>f__this = this;
		return <CloseIfUnselected>c__Iterator10E;
	}

	public void Show()
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && UIPopupList.mChild == null && this.atlas != null && this.isValid && this.items.Count > 0)
		{
			this.mLabelList.Clear();
			base.StopCoroutine("CloseIfUnselected");
			UICamera.selectedObject = (UICamera.hoveredObject ?? base.gameObject);
			this.mSelection = UICamera.selectedObject;
			this.source = UICamera.selectedObject;
			if (this.source == null)
			{
				UnityEngine.Debug.LogError("Popup list needs a source object...");
				return;
			}
			this.mOpenFrame = Time.frameCount;
			if (this.mPanel == null)
			{
				this.mPanel = UIPanel.Find(base.transform);
				if (this.mPanel == null)
				{
					return;
				}
			}
			UIPopupList.mChild = new GameObject("Drop-down List");
			UIPopupList.mChild.layer = base.gameObject.layer;
			UIPopupList.current = this;
			Transform transform = UIPopupList.mChild.transform;
			transform.parent = this.mPanel.cachedTransform;
			Vector3 localPosition;
			Vector3 vector;
			Vector3 v;
			if (this.openOn == UIPopupList.OpenOn.Manual && this.mSelection != base.gameObject)
			{
				localPosition = UICamera.lastEventPosition;
				vector = this.mPanel.cachedTransform.InverseTransformPoint(this.mPanel.anchorCamera.ScreenToWorldPoint(localPosition));
				v = vector;
				transform.localPosition = vector;
				localPosition = transform.position;
			}
			else
			{
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, base.transform, false, false);
				vector = bounds.min;
				v = bounds.max;
				transform.localPosition = vector;
				localPosition = transform.position;
			}
			base.StartCoroutine("CloseIfUnselected");
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			this.mBackground = NGUITools.AddSprite(UIPopupList.mChild, this.atlas, this.backgroundSprite);
			this.mBackground.pivot = UIWidget.Pivot.TopLeft;
			this.mBackground.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
			this.mBackground.color = this.backgroundColor;
			Vector4 border = this.mBackground.border;
			this.mBgBorder = border.y;
			this.mBackground.cachedTransform.localPosition = new Vector3(0f, border.y, 0f);
			this.mHighlight = NGUITools.AddSprite(UIPopupList.mChild, this.atlas, this.highlightSprite);
			this.mHighlight.pivot = UIWidget.Pivot.TopLeft;
			this.mHighlight.color = this.highlightColor;
			UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
			if (atlasSprite == null)
			{
				return;
			}
			float num = (float)atlasSprite.borderTop;
			float num2 = (float)this.activeFontSize;
			float activeFontScale = this.activeFontScale;
			float num3 = num2 * activeFontScale;
			float num4 = 0f;
			float num5 = -this.padding.y;
			List<UILabel> list = new List<UILabel>();
			if (!this.items.Contains(this.mSelectedItem))
			{
				this.mSelectedItem = null;
			}
			int i = 0;
			int count = this.items.Count;
			while (i < count)
			{
				string text = this.items[i];
				UILabel uILabel = NGUITools.AddWidget<UILabel>(UIPopupList.mChild);
				uILabel.name = i.ToString();
				uILabel.pivot = UIWidget.Pivot.TopLeft;
				uILabel.bitmapFont = this.bitmapFont;
				uILabel.trueTypeFont = this.trueTypeFont;
				uILabel.fontSize = this.fontSize;
				uILabel.fontStyle = this.fontStyle;
				uILabel.text = ((!this.isLocalized) ? text : Localization.Get(text));
				uILabel.color = this.textColor;
				uILabel.cachedTransform.localPosition = new Vector3(border.x + this.padding.x - uILabel.pivotOffset.x, num5, -1f);
				uILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
				uILabel.alignment = this.alignment;
				list.Add(uILabel);
				num5 -= num3;
				num5 -= this.padding.y;
				num4 = Mathf.Max(num4, uILabel.printedSize.x);
				UIEventListener uIEventListener = UIEventListener.Get(uILabel.gameObject);
				uIEventListener.onHover = new UIEventListener.BoolDelegate(this.OnItemHover);
				uIEventListener.onPress = new UIEventListener.BoolDelegate(this.OnItemPress);
				uIEventListener.parameter = text;
				if (this.mSelectedItem == text || (i == 0 && string.IsNullOrEmpty(this.mSelectedItem)))
				{
					this.Highlight(uILabel, true);
				}
				this.mLabelList.Add(uILabel);
				i++;
			}
			num4 = Mathf.Max(num4, v.x - vector.x - (border.x + this.padding.x) * 2f);
			float num6 = num4;
			Vector3 vector2 = new Vector3(num6 * 0.5f, -num3 * 0.5f, 0f);
			Vector3 vector3 = new Vector3(num6, num3 + this.padding.y, 1f);
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				UILabel uILabel2 = list[j];
				NGUITools.AddWidgetCollider(uILabel2.gameObject);
				uILabel2.autoResizeBoxCollider = false;
				BoxCollider component = uILabel2.GetComponent<BoxCollider>();
				if (component != null)
				{
					vector2.z = component.center.z;
					component.center = vector2;
					component.size = vector3;
				}
				else
				{
					BoxCollider2D component2 = uILabel2.GetComponent<BoxCollider2D>();
					component2.offset = vector2;
					component2.size = vector3;
				}
				j++;
			}
			int width = Mathf.RoundToInt(num4);
			num4 += (border.x + this.padding.x) * 2f;
			num5 -= border.y;
			this.mBackground.width = Mathf.RoundToInt(num4);
			this.mBackground.height = Mathf.RoundToInt(-num5 + border.y);
			int k = 0;
			int count3 = list.Count;
			while (k < count3)
			{
				UILabel uILabel3 = list[k];
				uILabel3.overflowMethod = UILabel.Overflow.ShrinkContent;
				uILabel3.width = width;
				k++;
			}
			float num7 = 2f * this.atlas.pixelSize;
			float f = num4 - (border.x + this.padding.x) * 2f + (float)atlasSprite.borderLeft * num7;
			float f2 = num3 + num * num7;
			this.mHighlight.width = Mathf.RoundToInt(f);
			this.mHighlight.height = Mathf.RoundToInt(f2);
			bool flag = this.position == UIPopupList.Position.Above;
			if (this.position == UIPopupList.Position.Auto)
			{
				UICamera uICamera = UICamera.FindCameraForLayer(this.mSelection.layer);
				if (uICamera != null)
				{
					flag = (uICamera.cachedCamera.WorldToViewportPoint(localPosition).y < 0.5f);
				}
			}
			if (this.isAnimated)
			{
				this.AnimateColor(this.mBackground);
				if (Time.timeScale == 0f || Time.timeScale >= 0.1f)
				{
					float bottom = num5 + num3;
					this.Animate(this.mHighlight, flag, bottom);
					int l = 0;
					int count4 = list.Count;
					while (l < count4)
					{
						this.Animate(list[l], flag, bottom);
						l++;
					}
					this.AnimateScale(this.mBackground, flag, bottom);
				}
			}
			if (flag)
			{
				vector.y = v.y - border.y;
				v.y = vector.y + (float)this.mBackground.height;
				v.x = vector.x + (float)this.mBackground.width;
				transform.localPosition = new Vector3(vector.x, v.y - border.y, vector.z);
			}
			else
			{
				v.y = vector.y + border.y;
				vector.y = v.y - (float)this.mBackground.height;
				v.x = vector.x + (float)this.mBackground.width;
			}
			Transform parent = this.mPanel.cachedTransform.parent;
			if (parent != null)
			{
				vector = this.mPanel.cachedTransform.TransformPoint(vector);
				v = this.mPanel.cachedTransform.TransformPoint(v);
				vector = parent.InverseTransformPoint(vector);
				v = parent.InverseTransformPoint(v);
			}
			Vector3 b = (!this.mPanel.hasClipping) ? this.mPanel.CalculateConstrainOffset(vector, v) : Vector3.zero;
			localPosition = transform.localPosition + b;
			localPosition.x = Mathf.Round(localPosition.x);
			localPosition.y = Mathf.Round(localPosition.y);
			transform.localPosition = localPosition;
		}
		else
		{
			this.OnSelect(false);
		}
	}
}
