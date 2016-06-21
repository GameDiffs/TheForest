using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("GUI Label.")]
	public class GUILabel : GUIContentAction
	{
		public override void OnGUI()
		{
			base.OnGUI();
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUI.Label(this.rect, this.content);
			}
			else
			{
				GUI.Label(this.rect, this.content, this.style.Value);
			}
		}
	}
}
