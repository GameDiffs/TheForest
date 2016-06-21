using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Get the current network time (seconds).")]
	public class NetworkGetTime : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("The network time."), UIHint(UIHint.Variable)]
		public FsmFloat time;

		public override void Reset()
		{
			this.time = null;
		}

		public override void OnEnter()
		{
			this.time.Value = (float)Network.time;
			base.Finish();
		}
	}
}
