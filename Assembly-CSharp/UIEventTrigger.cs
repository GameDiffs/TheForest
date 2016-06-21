using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Event Trigger")]
public class UIEventTrigger : MonoBehaviour
{
	public static UIEventTrigger current;

	public List<EventDelegate> onHoverOver = new List<EventDelegate>();

	public List<EventDelegate> onHoverOut = new List<EventDelegate>();

	public List<EventDelegate> onPress = new List<EventDelegate>();

	public List<EventDelegate> onRelease = new List<EventDelegate>();

	public List<EventDelegate> onSelect = new List<EventDelegate>();

	public List<EventDelegate> onDeselect = new List<EventDelegate>();

	public List<EventDelegate> onClick = new List<EventDelegate>();

	public List<EventDelegate> onDoubleClick = new List<EventDelegate>();

	public List<EventDelegate> onDragStart = new List<EventDelegate>();

	public List<EventDelegate> onDragEnd = new List<EventDelegate>();

	public List<EventDelegate> onDragOver = new List<EventDelegate>();

	public List<EventDelegate> onDragOut = new List<EventDelegate>();

	public List<EventDelegate> onDrag = new List<EventDelegate>();

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

	private void OnHover(bool isOver)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (isOver)
		{
			EventDelegate.Execute(this.onHoverOver);
		}
		else
		{
			EventDelegate.Execute(this.onHoverOut);
		}
		UIEventTrigger.current = null;
	}

	private void OnPress(bool pressed)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (pressed)
		{
			EventDelegate.Execute(this.onPress);
		}
		else
		{
			EventDelegate.Execute(this.onRelease);
		}
		UIEventTrigger.current = null;
	}

	private void OnSelect(bool selected)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		if (selected)
		{
			EventDelegate.Execute(this.onSelect);
		}
		else
		{
			EventDelegate.Execute(this.onDeselect);
		}
		UIEventTrigger.current = null;
	}

	private void OnClick()
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onClick);
		UIEventTrigger.current = null;
	}

	private void OnDoubleClick()
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDoubleClick);
		UIEventTrigger.current = null;
	}

	private void OnDragStart()
	{
		if (UIEventTrigger.current != null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragStart);
		UIEventTrigger.current = null;
	}

	private void OnDragEnd()
	{
		if (UIEventTrigger.current != null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragEnd);
		UIEventTrigger.current = null;
	}

	private void OnDragOver(GameObject go)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragOver);
		UIEventTrigger.current = null;
	}

	private void OnDragOut(GameObject go)
	{
		if (UIEventTrigger.current != null || !this.isColliderEnabled)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDragOut);
		UIEventTrigger.current = null;
	}

	private void OnDrag(Vector2 delta)
	{
		if (UIEventTrigger.current != null)
		{
			return;
		}
		UIEventTrigger.current = this;
		EventDelegate.Execute(this.onDrag);
		UIEventTrigger.current = null;
	}
}
