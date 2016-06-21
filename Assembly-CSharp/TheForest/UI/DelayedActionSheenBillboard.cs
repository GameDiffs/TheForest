using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class DelayedActionSheenBillboard : MonoBehaviour
	{
		public SheenBillboard _icon;

		private string _actionName;

		private void Awake()
		{
			base.enabled = false;
		}

		private void Start()
		{
			if (string.IsNullOrEmpty(this._actionName))
			{
				this._actionName = this._icon._action.ToString();
			}
		}

		private void Update()
		{
			if (this._icon && this._icon.FillSprite && this._icon.gameObject.activeSelf)
			{
				if (!this._icon.FillSprite.gameObject.activeSelf)
				{
					this._icon.FillSprite.gameObject.SetActive(true);
				}
				bool flag = TheForest.Utils.Input.DelayedActionName.Equals(this._actionName);
				this._icon.FillSprite.fillAmount = ((!flag) ? 0f : TheForest.Utils.Input.DelayedActionAlpha);
			}
		}

		private void GrabEnter()
		{
			base.enabled = true;
			TheForest.Utils.Input.ResetDelayedAction();
			if (this._icon.FillSprite)
			{
				this._icon.FillSprite.gameObject.SetActive(true);
			}
		}

		private void GrabExit()
		{
			base.enabled = false;
			if (this._icon.FillSprite)
			{
				this._icon.FillSprite.gameObject.SetActive(false);
				this._icon.FillSprite.fillAmount = 0f;
			}
		}
	}
}
