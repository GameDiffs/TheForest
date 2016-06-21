using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Destroy the object across the network.\n\nThe object is destroyed locally and remotely.\n\nOptionally remove any RPCs accociated with the object.")]
	public class NetworkDestroy : ComponentAction<NetworkView>
	{
		[CheckForComponent(typeof(NetworkView)), RequiredField, HutongGames.PlayMaker.Tooltip("The Game Object to destroy.\nNOTE: The Game Object must have a NetworkView attached.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Remove all RPC calls associated with the Game Object.")]
		public FsmBool removeRPCs;

		public override void Reset()
		{
			this.gameObject = null;
			this.removeRPCs = true;
		}

		public override void OnEnter()
		{
			this.DoDestroy();
			base.Finish();
		}

		private void DoDestroy()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (this.removeRPCs.Value)
			{
				Network.RemoveRPCs(base.networkView.owner);
			}
			Network.DestroyPlayerObjects(base.networkView.owner);
		}
	}
}
