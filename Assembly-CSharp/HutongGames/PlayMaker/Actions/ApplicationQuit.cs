using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application), HutongGames.PlayMaker.Tooltip("Quits the player application.")]
	public class ApplicationQuit : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			Application.Quit();
			base.Finish();
		}
	}
}
