using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Color"), ExecuteInEditMode]
public class UIButtonColor : UIWidgetContainer
{
	public enum State
	{
		Normal,
		Hover,
		Pressed,
		Disabled
	}

	public GameObject tweenTarget;

	public Color hover = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	public Color pressed = new Color(0.7176471f, 0.6392157f, 0.482352942f, 1f);

	public Color disabledColor = Color.grey;

	public float duration = 0.2f;

	[NonSerialized]
	protected Color mStartingColor;

	[NonSerialized]
	protected Color mDefaultColor;

	[NonSerialized]
	protected bool mInitDone;

	[NonSerialized]
	protected UIWidget mWidget;

	[NonSerialized]
	protected UIButtonColor.State mState;

	public UIButtonColor.State state
	{
		get
		{
			return this.mState;
		}
		set
		{
			this.SetState(value, false);
		}
	}

	public Color defaultColor
	{
		get
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			return this.mDefaultColor;
		}
		set
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			this.mDefaultColor = value;
			UIButtonColor.State state = this.mState;
			this.mState = UIButtonColor.State.Disabled;
			this.SetState(state, false);
		}
	}

	public virtual bool isEnabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	public void ResetDefaultColor()
	{
		this.defaultColor = this.mStartingColor;
	}

	public void CacheDefaultColor()
	{
		if (!this.mInitDone)
		{
			this.OnInit();
		}
	}

	private void Start()
	{
		if (!this.mInitDone)
		{
			this.OnInit();
		}
		if (!this.isEnabled)
		{
			this.SetState(UIButtonColor.State.Disabled, true);
		}
	}

	protected virtual void OnInit()
	{
		this.mInitDone = true;
		if (this.tweenTarget == null)
		{
			this.tweenTarget = base.gameObject;
		}
		if (this.tweenTarget != null)
		{
			this.mWidget = this.tweenTarget.GetComponent<UIWidget>();
		}
		if (this.mWidget != null)
		{
			this.mDefaultColor = this.mWidget.color;
			this.mStartingColor = this.mDefaultColor;
		}
		else if (this.tweenTarget != null)
		{
			Renderer component = this.tweenTarget.GetComponent<Renderer>();
			if (component != null)
			{
				this.mDefaultColor = ((!Application.isPlaying) ? component.sharedMaterial.color : component.material.color);
				this.mStartingColor = this.mDefaultColor;
			}
			else
			{
				Light component2 = this.tweenTarget.GetComponent<Light>();
				if (component2 != null)
				{
					this.mDefaultColor = component2.color;
					this.mStartingColor = this.mDefaultColor;
				}
				else
				{
					this.tweenTarget = null;
					this.mInitDone = false;
				}
			}
		}
	}

	protected virtual void OnEnable()
	{
		if (this.mInitDone)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
		if (UICamera.currentTouch != null)
		{
			if (UICamera.currentTouch.pressed == base.gameObject)
			{
				this.OnPress(true);
			}
			else if (UICamera.currentTouch.current == base.gameObject)
			{
				this.OnHover(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		if (this.mInitDone && this.tweenTarget != null)
		{
			this.SetState(UIButtonColor.State.Normal, true);
			TweenColor component = this.tweenTarget.GetComponent<TweenColor>();
			if (component != null)
			{
				component.value = this.mDefaultColor;
				component.enabled = false;
			}
		}
	}

	protected virtual void OnHover(bool isOver)
	{
		if (this.isEnabled)
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.tweenTarget != null)
			{
				this.SetState((!isOver) ? UIButtonColor.State.Normal : UIButtonColor.State.Hover, false);
			}
		}
	}

	protected virtual void OnPress(bool isPressed)
	{
		if (this.isEnabled && UICamera.currentTouch != null)
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.tweenTarget != null)
			{
				if (isPressed)
				{
					this.SetState(UIButtonColor.State.Pressed, false);
				}
				else if (UICamera.currentTouch.current == base.gameObject)
				{
					if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
					{
						this.SetState(UIButtonColor.State.Hover, false);
					}
					else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse && UICamera.hoveredObject == base.gameObject)
					{
						this.SetState(UIButtonColor.State.Hover, false);
					}
					else
					{
						this.SetState(UIButtonColor.State.Normal, false);
					}
				}
				else
				{
					this.SetState(UIButtonColor.State.Normal, false);
				}
			}
		}
	}

	protected virtual void OnDragOver()
	{
		if (this.isEnabled)
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.tweenTarget != null)
			{
				this.SetState(UIButtonColor.State.Pressed, false);
			}
		}
	}

	protected virtual void OnDragOut()
	{
		if (this.isEnabled)
		{
			if (!this.mInitDone)
			{
				this.OnInit();
			}
			if (this.tweenTarget != null)
			{
				this.SetState(UIButtonColor.State.Normal, false);
			}
		}
	}

	public virtual void SetState(UIButtonColor.State state, bool instant)
	{
		if (!this.mInitDone)
		{
			this.mInitDone = true;
			this.OnInit();
		}
		if (this.mState != state)
		{
			this.mState = state;
			this.UpdateColor(instant);
		}
	}

	public void UpdateColor(bool instant)
	{
		if (this.tweenTarget != null)
		{
			TweenColor tweenColor;
			switch (this.mState)
			{
			case UIButtonColor.State.Hover:
				tweenColor = TweenColor.Begin(this.tweenTarget, this.duration, this.hover);
				break;
			case UIButtonColor.State.Pressed:
				tweenColor = TweenColor.Begin(this.tweenTarget, this.duration, this.pressed);
				break;
			case UIButtonColor.State.Disabled:
				tweenColor = TweenColor.Begin(this.tweenTarget, this.duration, this.disabledColor);
				break;
			default:
				tweenColor = TweenColor.Begin(this.tweenTarget, this.duration, this.mDefaultColor);
				break;
			}
			if (instant && tweenColor != null)
			{
				tweenColor.value = tweenColor.to;
				tweenColor.enabled = false;
			}
		}
	}
}
