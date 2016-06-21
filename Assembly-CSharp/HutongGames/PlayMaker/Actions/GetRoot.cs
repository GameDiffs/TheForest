using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets the top most parent of the Game Object.\nIf the game object has no parent, returns itself.")]
	public class GetRoot : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmGameObject storeRoot;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeRoot = null;
		}

		public override void OnEnter()
		{
			this.DoGetRoot();
			base.Finish();
		}

		private void DoGetRoot()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeRoot.Value = ownerDefaultTarget.transform.root.gameObject;
		}
	}
}
