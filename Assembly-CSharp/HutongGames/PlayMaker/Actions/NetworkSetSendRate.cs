using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Set the send rate for all networkViews. Default is 15")]
	public class NetworkSetSendRate : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The send rate for all networkViews")]
		public FsmFloat sendRate;

		public override void Reset()
		{
			this.sendRate = 15f;
		}

		public override void OnEnter()
		{
			this.DoSetSendRate();
			base.Finish();
		}

		private void DoSetSendRate()
		{
			Network.sendRate = this.sendRate.Value;
		}
	}
}
