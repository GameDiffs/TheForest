using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
	public enum Action
	{
		PressAndClick,
		Select,
		All
	}

	public enum Modifier
	{
		Any,
		Shift,
		Control,
		Alt,
		None
	}

	private static List<UIKeyBinding> mList = new List<UIKeyBinding>();

	public KeyCode keyCode;

	public UIKeyBinding.Modifier modifier;

	public UIKeyBinding.Action action;

	[NonSerialized]
	private bool mIgnoreUp;

	[NonSerialized]
	private bool mIsInput;

	[NonSerialized]
	private bool mPress;

	public static bool IsBound(KeyCode key)
	{
		int i = 0;
		int count = UIKeyBinding.mList.Count;
		while (i < count)
		{
			UIKeyBinding uIKeyBinding = UIKeyBinding.mList[i];
			if (uIKeyBinding != null && uIKeyBinding.keyCode == key)
			{
				return true;
			}
			i++;
		}
		return false;
	}

	protected virtual void OnEnable()
	{
		UIKeyBinding.mList.Add(this);
	}

	protected virtual void OnDisable()
	{
		UIKeyBinding.mList.Remove(this);
	}

	protected virtual void Start()
	{
		UIInput component = base.GetComponent<UIInput>();
		this.mIsInput = (component != null);
		if (component != null)
		{
			EventDelegate.Add(component.onSubmit, new EventDelegate.Callback(this.OnSubmit));
		}
	}

	protected virtual void OnSubmit()
	{
		if (UICamera.currentKey == this.keyCode && this.IsModifierActive())
		{
			this.mIgnoreUp = true;
		}
	}

	protected virtual bool IsModifierActive()
	{
		if (this.modifier == UIKeyBinding.Modifier.Any)
		{
			return true;
		}
		if (this.modifier == UIKeyBinding.Modifier.Alt)
		{
			if (UICamera.GetKey(KeyCode.LeftAlt) || UICamera.GetKey(KeyCode.RightAlt))
			{
				return true;
			}
		}
		else if (this.modifier == UIKeyBinding.Modifier.Control)
		{
			if (UICamera.GetKey(KeyCode.LeftControl) || UICamera.GetKey(KeyCode.RightControl))
			{
				return true;
			}
		}
		else if (this.modifier == UIKeyBinding.Modifier.Shift)
		{
			if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
			{
				return true;
			}
		}
		else if (this.modifier == UIKeyBinding.Modifier.None)
		{
			return !UICamera.GetKey(KeyCode.LeftAlt) && !UICamera.GetKey(KeyCode.RightAlt) && !UICamera.GetKey(KeyCode.LeftControl) && !UICamera.GetKey(KeyCode.RightControl) && !UICamera.GetKey(KeyCode.LeftShift) && !UICamera.GetKey(KeyCode.RightShift);
		}
		return false;
	}

	protected virtual void Update()
	{
		if (UICamera.inputHasFocus)
		{
			return;
		}
		if (this.keyCode == KeyCode.None || !this.IsModifierActive())
		{
			return;
		}
		bool flag = UICamera.GetKeyDown(this.keyCode);
		bool flag2 = UICamera.GetKeyUp(this.keyCode);
		if (flag)
		{
			this.mPress = true;
		}
		if (this.action == UIKeyBinding.Action.PressAndClick || this.action == UIKeyBinding.Action.All)
		{
			if (flag)
			{
				UICamera.currentKey = this.keyCode;
				this.OnBindingPress(true);
			}
			if (this.mPress && flag2)
			{
				UICamera.currentKey = this.keyCode;
				this.OnBindingPress(false);
				this.OnBindingClick();
			}
		}
		if ((this.action == UIKeyBinding.Action.Select || this.action == UIKeyBinding.Action.All) && flag2)
		{
			if (this.mIsInput)
			{
				if (!this.mIgnoreUp && !UICamera.inputHasFocus && this.mPress)
				{
					UICamera.selectedObject = base.gameObject;
				}
				this.mIgnoreUp = false;
			}
			else if (this.mPress)
			{
				UICamera.hoveredObject = base.gameObject;
			}
		}
		if (flag2)
		{
			this.mPress = false;
		}
	}

	protected virtual void OnBindingPress(bool pressed)
	{
		UICamera.Notify(base.gameObject, "OnPress", pressed);
	}

	protected virtual void OnBindingClick()
	{
		UICamera.Notify(base.gameObject, "OnClick", null);
	}
}
