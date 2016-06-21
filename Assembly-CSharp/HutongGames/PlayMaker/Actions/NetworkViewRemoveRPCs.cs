using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Remove the RPC function calls accociated with a Game Object.\n\nNOTE: The Game Object must have a NetworkView component attached.")]
	public class NetworkViewRemoveRPCs : ComponentAction<NetworkView>
	{
		[CheckForComponent(typeof(NetworkView)), RequiredField, HutongGames.PlayMaker.Tooltip("Remove the RPC function calls accociated with this Game Object.\n\nNOTE: The GameObject must have a NetworkView component attached.")]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			this.gameObject = null;
		}

		public override void OnEnter()
		{
			this.DoRemoveRPCsFromViewID();
			base.Finish();
		}

		private void DoRemoveRPCsFromViewID()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				Network.RemoveRPCs(base.networkView.viewID);
			}
		}
	}
}
