using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Clear the host list which was received by MasterServer Request Host List")]
	public class MasterServerClearHostList : FsmStateAction
	{
		public override void OnEnter()
		{
			MasterServer.ClearHostList();
			base.Finish();
		}
	}
}
