using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class SleepTimerDisplay : MonoBehaviour
	{
		public UITexture _fillSprite;

		private void Update()
		{
			if (LocalPlayer.Stats)
			{
				this._fillSprite.fillAmount = 1f - Mathf.Abs(LocalPlayer.Stats.DaySurvived - LocalPlayer.Stats.NextSleepTime);
			}
		}
	}
}
