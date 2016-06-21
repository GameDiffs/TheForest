using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application), HutongGames.PlayMaker.Tooltip("Gets the Width of the Screen in pixels.")]
	public class GetScreenWidth : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat storeScreenWidth;

		public override void Reset()
		{
			this.storeScreenWidth = null;
		}

		public override void OnEnter()
		{
			this.storeScreenWidth.Value = (float)Screen.width;
			base.Finish();
		}
	}
}
