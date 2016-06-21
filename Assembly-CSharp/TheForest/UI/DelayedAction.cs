using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class DelayedAction : MonoBehaviour
	{
		public UISprite _fillIcon;

		private void Update()
		{
			this._fillIcon.fillAmount = TheForest.Utils.Input.DelayedActionAlpha;
		}
	}
}
