using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Store the current send rate for all NetworkViews")]
	public class NetworkGetSendRate : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the current send rate for NetworkViews"), UIHint(UIHint.Variable)]
		public FsmFloat sendRate;

		public override void Reset()
		{
			this.sendRate = null;
		}

		public override void OnEnter()
		{
			this.DoGetSendRate();
			base.Finish();
		}

		private void DoGetSendRate()
		{
			this.sendRate.Value = Network.sendRate;
		}
	}
}
