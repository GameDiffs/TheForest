using System;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class BuildLog : MonoBehaviour
	{
		[ItemIdPicker]
		public int _itemId;

		public int _needed;

		private int lastPlayerAmount;

		private int lastNeeded;

		private UILabel Label;

		private TweenScale Tween;

		private void Awake()
		{
			this.Label = base.gameObject.GetComponent<UILabel>();
			this.Tween = base.gameObject.GetComponent<TweenScale>();
		}

		private void Update()
		{
			if (this._needed > 0)
			{
				int num = LocalPlayer.Inventory.AmountOf(this._itemId, true);
				if (num > this._needed)
				{
					num = this._needed;
				}
				if (this.lastPlayerAmount != num || this._needed != this.lastNeeded)
				{
					this.Label.text = num + "/" + this._needed.ToString();
					if (num >= this._needed)
					{
						this.Tween.enabled = true;
					}
					else
					{
						this.Tween.enabled = false;
					}
					this.lastPlayerAmount = num;
					this.lastNeeded = this._needed;
				}
			}
		}
	}
}
