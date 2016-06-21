using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class UISliderControl : UIControl
	{
		public Image iconImage;

		public Slider slider;

		private bool _showIcon;

		private bool _showSlider;

		public bool showIcon
		{
			get
			{
				return this._showIcon;
			}
			set
			{
				if (this.iconImage == null)
				{
					return;
				}
				this.iconImage.gameObject.SetActive(value);
				this._showIcon = value;
			}
		}

		public bool showSlider
		{
			get
			{
				return this._showSlider;
			}
			set
			{
				if (this.slider == null)
				{
					return;
				}
				this.slider.gameObject.SetActive(value);
				this._showSlider = value;
			}
		}

		public override void SetCancelCallback(Action cancelCallback)
		{
			base.SetCancelCallback(cancelCallback);
			if (cancelCallback == null || this.slider == null)
			{
				return;
			}
			if (this.slider is ICustomSelectable)
			{
				(this.slider as ICustomSelectable).CancelEvent += delegate
				{
					cancelCallback();
				};
			}
			else
			{
				EventTrigger eventTrigger = this.slider.GetComponent<EventTrigger>();
				if (eventTrigger == null)
				{
					eventTrigger = this.slider.gameObject.AddComponent<EventTrigger>();
				}
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.callback = new EventTrigger.TriggerEvent();
				entry.eventID = EventTriggerType.Cancel;
				entry.callback.AddListener(delegate(BaseEventData data)
				{
					cancelCallback();
				});
				if (eventTrigger.triggers == null)
				{
					eventTrigger.triggers = new List<EventTrigger.Entry>();
				}
				eventTrigger.triggers.Add(entry);
			}
		}
	}
}
