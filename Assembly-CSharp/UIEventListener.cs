using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, bool state);

	public delegate void FloatDelegate(GameObject go, float delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void ObjectDelegate(GameObject go, GameObject obj);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

	public object parameter;

	public UIEventListener.VoidDelegate onSubmit;

	public UIEventListener.VoidDelegate onClick;

	public UIEventListener.VoidDelegate onDoubleClick;

	public UIEventListener.BoolDelegate onHover;

	public UIEventListener.BoolDelegate onPress;

	public UIEventListener.BoolDelegate onSelect;

	public UIEventListener.FloatDelegate onScroll;

	public UIEventListener.VoidDelegate onDragStart;

	public UIEventListener.VectorDelegate onDrag;

	public UIEventListener.VoidDelegate onDragOver;

	public UIEventListener.VoidDelegate onDragOut;

	public UIEventListener.VoidDelegate onDragEnd;

	public UIEventListener.ObjectDelegate onDrop;

	public UIEventListener.KeyCodeDelegate onKey;

	public UIEventListener.BoolDelegate onTooltip;

	private bool isColliderEnabled
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

	private void OnSubmit()
	{
		if (this.isColliderEnabled && this.onSubmit != null)
		{
			this.onSubmit(base.gameObject);
		}
	}

	private void OnClick()
	{
		if (this.isColliderEnabled && this.onClick != null)
		{
			this.onClick(base.gameObject);
		}
	}

	private void OnDoubleClick()
	{
		if (this.isColliderEnabled && this.onDoubleClick != null)
		{
			this.onDoubleClick(base.gameObject);
		}
	}

	private void OnHover(bool isOver)
	{
		if (this.isColliderEnabled && this.onHover != null)
		{
			this.onHover(base.gameObject, isOver);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (this.isColliderEnabled && this.onPress != null)
		{
			this.onPress(base.gameObject, isPressed);
		}
	}

	private void OnSelect(bool selected)
	{
		if (this.isColliderEnabled && this.onSelect != null)
		{
			this.onSelect(base.gameObject, selected);
		}
	}

	private void OnScroll(float delta)
	{
		if (this.isColliderEnabled && this.onScroll != null)
		{
			this.onScroll(base.gameObject, delta);
		}
	}

	private void OnDragStart()
	{
		if (this.onDragStart != null)
		{
			this.onDragStart(base.gameObject);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (this.onDrag != null)
		{
			this.onDrag(base.gameObject, delta);
		}
	}

	private void OnDragOver()
	{
		if (this.isColliderEnabled && this.onDragOver != null)
		{
			this.onDragOver(base.gameObject);
		}
	}

	private void OnDragOut()
	{
		if (this.isColliderEnabled && this.onDragOut != null)
		{
			this.onDragOut(base.gameObject);
		}
	}

	private void OnDragEnd()
	{
		if (this.onDragEnd != null)
		{
			this.onDragEnd(base.gameObject);
		}
	}

	private void OnDrop(GameObject go)
	{
		if (this.isColliderEnabled && this.onDrop != null)
		{
			this.onDrop(base.gameObject, go);
		}
	}

	private void OnKey(KeyCode key)
	{
		if (this.isColliderEnabled && this.onKey != null)
		{
			this.onKey(base.gameObject, key);
		}
	}

	private void OnTooltip(bool show)
	{
		if (this.isColliderEnabled && this.onTooltip != null)
		{
			this.onTooltip(base.gameObject, show);
		}
	}

	public static UIEventListener Get(GameObject go)
	{
		UIEventListener uIEventListener = go.GetComponent<UIEventListener>();
		if (uIEventListener == null)
		{
			uIEventListener = go.AddComponent<UIEventListener>();
		}
		return uIEventListener;
	}
}
