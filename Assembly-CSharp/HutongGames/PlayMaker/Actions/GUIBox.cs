using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("GUI Box.")]
	public class GUIBox : GUIContentAction
	{
		public override void OnGUI()
		{
			base.OnGUI();
			if (string.IsNullOrEmpty(this.style.Value))
			{
				GUI.Box(this.rect, this.content);
			}
			else
			{
				GUI.Box(this.rect, this.content, this.style.Value);
			}
		}
	}
}
