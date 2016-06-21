using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class UIControlSet : MonoBehaviour
	{
		[SerializeField]
		private Text title;

		private Dictionary<int, UIControl> _controls;

		private Dictionary<int, UIControl> controls
		{
			get
			{
				Dictionary<int, UIControl> arg_1B_0;
				if ((arg_1B_0 = this._controls) == null)
				{
					arg_1B_0 = (this._controls = new Dictionary<int, UIControl>());
				}
				return arg_1B_0;
			}
		}

		public void SetTitle(string text)
		{
			if (this.title == null)
			{
				return;
			}
			this.title.text = text;
		}

		public T GetControl<T>(int uniqueId) where T : UIControl
		{
			UIControl uIControl;
			this.controls.TryGetValue(uniqueId, out uIControl);
			return uIControl as T;
		}

		public UISliderControl CreateSlider(GameObject prefab, Sprite icon, float minValue, float maxValue, Action<int, float> valueChangedCallback, Action<int> cancelCallback)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			UISliderControl control = gameObject.GetComponent<UISliderControl>();
			if (control == null)
			{
				UnityEngine.Object.Destroy(gameObject);
				Debug.LogError("Prefab missing UISliderControl component!");
				return null;
			}
			gameObject.transform.SetParent(base.transform, false);
			if (control.iconImage != null)
			{
				control.iconImage.sprite = icon;
			}
			if (control.slider != null)
			{
				control.slider.minValue = minValue;
				control.slider.maxValue = maxValue;
				if (valueChangedCallback != null)
				{
					control.slider.onValueChanged.AddListener(delegate(float value)
					{
						valueChangedCallback(control.id, value);
					});
				}
				if (cancelCallback != null)
				{
					control.SetCancelCallback(delegate
					{
						cancelCallback(control.id);
					});
				}
			}
			this.controls.Add(control.id, control);
			return control;
		}
	}
}
