using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Slider")]
public class UISlider360 : UIWidgetContainer
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	public static UISlider360 current;

	public Transform foreground;

	public Transform thumb;

	public UISlider360.Direction direction;

	public int numberOfSteps;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[HideInInspector, SerializeField]
	private float rawValue = 1f;

	[HideInInspector, SerializeField]
	private GameObject eventReceiver;

	[HideInInspector, SerializeField]
	private string functionName = "OnSliderChange";

	private BoxCollider mCol;

	private Transform mTrans;

	private Transform mFGTrans;

	private UIWidget mFGWidget;

	private UISprite mFGFilled;

	private bool mInitDone;

	private Vector2 mSize = Vector2.zero;

	private Vector2 mCenter = Vector3.zero;

	public float value
	{
		get
		{
			float num = this.rawValue;
			if (this.numberOfSteps > 1)
			{
				num = Mathf.Round(num * (float)(this.numberOfSteps - 1)) / (float)(this.numberOfSteps - 1);
			}
			return num;
		}
		set
		{
			this.Set(value, false);
		}
	}

	[Obsolete("Use 'value' instead")]
	public float sliderValue
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

	public Vector2 fullSize
	{
		get
		{
			return this.mSize;
		}
		set
		{
			if (this.mSize != value)
			{
				this.mSize = value;
				this.ForceUpdate();
			}
		}
	}

	private void Init()
	{
		this.mInitDone = true;
		if (this.foreground != null)
		{
			this.mFGWidget = this.foreground.GetComponent<UIWidget>();
			this.mFGFilled = ((!(this.mFGWidget != null)) ? null : (this.mFGWidget as UISprite));
			this.mFGTrans = this.foreground.transform;
			if (this.mSize == Vector2.zero)
			{
				UIWidget component = this.foreground.GetComponent<UIWidget>();
				this.mSize = ((!(component != null)) ? this.foreground.localScale : new Vector2((float)component.width, (float)component.height));
			}
			if (this.mCenter == Vector2.zero)
			{
				UIWidget component2 = this.foreground.GetComponent<UIWidget>();
				if (component2 != null)
				{
					Vector3[] localCorners = component2.localCorners;
					this.mCenter = Vector3.Lerp(localCorners[0], localCorners[2], 0.5f);
				}
				else
				{
					this.mCenter = this.foreground.localPosition + this.foreground.localScale * 0.5f;
				}
			}
		}
		else if (this.mCol != null)
		{
			if (this.mSize == Vector2.zero)
			{
				this.mSize = this.mCol.size;
			}
			if (this.mCenter == Vector2.zero)
			{
				this.mCenter = this.mCol.center;
			}
		}
		else
		{
			Debug.LogWarning("UISlider expected to find a foreground object or a box collider to work with", this);
		}
	}

	private void Awake()
	{
		this.mTrans = base.transform;
		this.mCol = (base.GetComponent<Collider>() as BoxCollider);
	}

	private void Start()
	{
		this.Init();
		if (Application.isPlaying && this.thumb != null && this.thumb.GetComponent<Collider>() != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(this.thumb.gameObject);
			UIEventListener expr_49 = uIEventListener;
			expr_49.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(expr_49.onPress, new UIEventListener.BoolDelegate(this.OnPressThumb));
			UIEventListener expr_6B = uIEventListener;
			expr_6B.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(expr_6B.onDrag, new UIEventListener.VectorDelegate(this.OnDragThumb));
		}
		this.Set(this.rawValue, true);
	}

	private void OnPress(bool pressed)
	{
		if (base.enabled && pressed && UICamera.currentTouchID != -100)
		{
			this.UpdateDrag();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (base.enabled)
		{
			this.UpdateDrag();
		}
	}

	private void OnPressThumb(GameObject go, bool pressed)
	{
		if (base.enabled && pressed)
		{
			this.UpdateDrag();
		}
	}

	private void OnDragThumb(GameObject go, Vector2 delta)
	{
		if (base.enabled)
		{
			this.UpdateDrag();
		}
	}

	private void OnKey(KeyCode key)
	{
		if (base.enabled)
		{
			float num = ((float)this.numberOfSteps <= 1f) ? 0.125f : (1f / (float)(this.numberOfSteps - 1));
			if (this.direction == UISlider360.Direction.Horizontal)
			{
				if (key == KeyCode.LeftArrow)
				{
					this.Set(this.rawValue - num, false);
				}
				else if (key == KeyCode.RightArrow)
				{
					this.Set(this.rawValue + num, false);
				}
			}
			else if (key == KeyCode.DownArrow)
			{
				this.Set(this.rawValue - num, false);
			}
			else if (key == KeyCode.UpArrow)
			{
				this.Set(this.rawValue + num, false);
			}
		}
	}

	private void UpdateDrag()
	{
		if (this.mCol == null || UICamera.currentCamera == null || UICamera.currentTouch == null)
		{
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		Plane plane = new Plane(this.mTrans.rotation * Vector3.back, this.mTrans.position);
		float distance;
		if (!plane.Raycast(ray, out distance))
		{
			return;
		}
		Vector3 b = this.mTrans.localPosition + (this.mCenter - this.mSize * 0.5f);
		Vector3 b2 = this.mTrans.localPosition - b;
		Vector3 a = this.mTrans.InverseTransformPoint(ray.GetPoint(distance));
		Vector3 vector = a + b2;
		this.Set((this.direction != UISlider360.Direction.Horizontal) ? (vector.y / this.mSize.y) : (vector.x / this.mSize.x), false);
	}

	private void Set(float input, bool force)
	{
		if (!this.mInitDone)
		{
			this.Init();
		}
		float num = Mathf.Clamp01(input);
		if (num < 0.001f)
		{
			num = 0f;
		}
		float value = this.value;
		this.rawValue = num;
		float value2 = this.value;
		if (force || value != value2)
		{
			Vector3 localScale = this.mSize;
			if (this.direction == UISlider360.Direction.Horizontal)
			{
				localScale.x *= value2;
			}
			else
			{
				localScale.y *= value2;
			}
			if (this.mFGFilled != null && this.mFGFilled.type == UIBasicSprite.Type.Filled)
			{
				this.mFGFilled.fillAmount = value2;
			}
			else if (this.mFGWidget != null)
			{
				if (value2 > 0.001f)
				{
					this.mFGWidget.width = Mathf.RoundToInt(localScale.x);
					this.mFGWidget.height = Mathf.RoundToInt(localScale.y);
					this.mFGWidget.enabled = true;
				}
				else
				{
					this.mFGWidget.enabled = false;
				}
			}
			else if (this.foreground != null)
			{
				this.mFGTrans.localScale = localScale;
			}
			if (this.thumb != null)
			{
				Vector3 localPosition = this.thumb.localPosition;
				if (this.mFGFilled != null && this.mFGFilled.type == UIBasicSprite.Type.Filled)
				{
					if (this.mFGFilled.fillDirection == UIBasicSprite.FillDirection.Horizontal)
					{
						localPosition.x = ((!this.mFGFilled.invert) ? localScale.x : (this.mSize.x - localScale.x));
					}
					else if (this.mFGFilled.fillDirection == UIBasicSprite.FillDirection.Vertical)
					{
						localPosition.y = ((!this.mFGFilled.invert) ? localScale.y : (this.mSize.y - localScale.y));
					}
					else
					{
						Debug.LogWarning("Slider thumb is only supported with Horizontal or Vertical fill direction", this);
					}
				}
				else if (this.direction == UISlider360.Direction.Horizontal)
				{
					localPosition.x = localScale.x;
				}
				else
				{
					localPosition.y = localScale.y;
				}
				this.thumb.localPosition = localPosition;
			}
			UISlider360.current = this;
			if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
			{
				this.eventReceiver.SendMessage(this.functionName, value2, SendMessageOptions.DontRequireReceiver);
			}
			UISlider360.current = null;
		}
	}

	public void ForceUpdate()
	{
		this.Set(this.rawValue, true);
	}
}
