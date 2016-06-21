using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Get the number of connected players.\n\nOn a client this returns 1 (the server).")]
	public class NetworkGetConnectionsCount : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Number of connected players."), UIHint(UIHint.Variable)]
		public FsmInt connectionsCount;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.connectionsCount = null;
			this.everyFrame = true;
		}

		public override void OnEnter()
		{
			this.connectionsCount.Value = Network.connections.Length;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.connectionsCount.Value = Network.connections.Length;
		}
	}
}
