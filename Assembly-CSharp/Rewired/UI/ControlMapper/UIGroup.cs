using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu("")]
	public class UIGroup : MonoBehaviour
	{
		[SerializeField]
		private Text _label;

		[SerializeField]
		private Transform _content;

		public string labelText
		{
			get
			{
				return (!(this._label != null)) ? string.Empty : this._label.text;
			}
			set
			{
				if (this._label == null)
				{
					return;
				}
				this._label.text = value;
			}
		}

		public Transform content
		{
			get
			{
				return this._content;
			}
		}

		public void SetLabelActive(bool state)
		{
			if (this._label == null)
			{
				return;
			}
			this._label.gameObject.SetActive(state);
		}
	}
}
