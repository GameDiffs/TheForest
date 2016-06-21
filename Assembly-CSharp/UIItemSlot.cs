using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIItemSlot : MonoBehaviour
{
	public UISprite icon;

	public UIWidget background;

	public UILabel label;

	public AudioClip grabSound;

	public AudioClip placeSound;

	public AudioClip errorSound;

	private InvGameItem mItem;

	private string mText = string.Empty;

	private static InvGameItem mDraggedItem;

	protected abstract InvGameItem observedItem
	{
		get;
	}

	protected abstract InvGameItem Replace(InvGameItem item);

	private void OnTooltip(bool show)
	{
		InvGameItem invGameItem = (!show) ? null : this.mItem;
		if (invGameItem != null)
		{
			InvBaseItem baseItem = invGameItem.baseItem;
			if (baseItem != null)
			{
				string text = string.Concat(new string[]
				{
					"[",
					NGUIText.EncodeColor(invGameItem.color),
					"]",
					invGameItem.name,
					"[-]\n"
				});
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"[AFAFAF]Level ",
					invGameItem.itemLevel,
					" ",
					baseItem.slot
				});
				List<InvStat> list = invGameItem.CalculateStats();
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					InvStat invStat = list[i];
					if (invStat.amount != 0)
					{
						if (invStat.amount < 0)
						{
							text = text + "\n[FF0000]" + invStat.amount;
						}
						else
						{
							text = text + "\n[00FF00]+" + invStat.amount;
						}
						if (invStat.modifier == InvStat.Modifier.Percent)
						{
							text += "%";
						}
						text = text + " " + invStat.id;
						text += "[-]";
					}
					i++;
				}
				if (!string.IsNullOrEmpty(baseItem.description))
				{
					text = text + "\n[FF9900]" + baseItem.description;
				}
				UITooltip.Show(text);
				return;
			}
		}
		UITooltip.Hide();
	}

	private void OnClick()
	{
		if (UIItemSlot.mDraggedItem != null)
		{
			this.OnDrop(null);
		}
		else if (this.mItem != null)
		{
			UIItemSlot.mDraggedItem = this.Replace(null);
			if (UIItemSlot.mDraggedItem != null)
			{
				NGUITools.PlaySound(this.grabSound);
			}
			this.UpdateCursor();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (UIItemSlot.mDraggedItem == null && this.mItem != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			UIItemSlot.mDraggedItem = this.Replace(null);
			NGUITools.PlaySound(this.grabSound);
			this.UpdateCursor();
		}
	}

	private void OnDrop(GameObject go)
	{
		InvGameItem invGameItem = this.Replace(UIItemSlot.mDraggedItem);
		if (UIItemSlot.mDraggedItem == invGameItem)
		{
			NGUITools.PlaySound(this.errorSound);
		}
		else if (invGameItem != null)
		{
			NGUITools.PlaySound(this.grabSound);
		}
		else
		{
			NGUITools.PlaySound(this.placeSound);
		}
		UIItemSlot.mDraggedItem = invGameItem;
		this.UpdateCursor();
	}

	private void UpdateCursor()
	{
		if (UIItemSlot.mDraggedItem != null && UIItemSlot.mDraggedItem.baseItem != null)
		{
			UICursor.Set(UIItemSlot.mDraggedItem.baseItem.iconAtlas, UIItemSlot.mDraggedItem.baseItem.iconName);
		}
		else
		{
			UICursor.Clear();
		}
	}

	private void Update()
	{
		InvGameItem observedItem = this.observedItem;
		if (this.mItem != observedItem)
		{
			this.mItem = observedItem;
			InvBaseItem invBaseItem = (observedItem == null) ? null : observedItem.baseItem;
			if (this.label != null)
			{
				string text = (observedItem == null) ? null : observedItem.name;
				if (string.IsNullOrEmpty(this.mText))
				{
					this.mText = this.label.text;
				}
				this.label.text = ((text == null) ? this.mText : text);
			}
			if (this.icon != null)
			{
				if (invBaseItem == null || invBaseItem.iconAtlas == null)
				{
					this.icon.enabled = false;
				}
				else
				{
					this.icon.atlas = invBaseItem.iconAtlas;
					this.icon.spriteName = invBaseItem.iconName;
					this.icon.enabled = true;
					this.icon.MakePixelPerfect();
				}
			}
			if (this.background != null)
			{
				this.background.color = ((observedItem == null) ? Color.white : observedItem.color);
			}
		}
	}
}
