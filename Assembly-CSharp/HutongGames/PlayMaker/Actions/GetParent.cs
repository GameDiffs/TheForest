using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets the Parent of a Game Object.")]
	public class GetParent : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this.storeResult.Value = ((!(ownerDefaultTarget.transform.parent == null)) ? ownerDefaultTarget.transform.parent.gameObject : null);
			}
			else
			{
				this.storeResult.Value = null;
			}
			base.Finish();
		}
	}
}
