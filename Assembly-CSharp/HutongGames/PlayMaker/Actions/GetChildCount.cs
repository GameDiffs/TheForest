using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets the number of children that a GameObject has.")]
	public class GetChildCount : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the number of children in an int variable."), UIHint(UIHint.Variable)]
		public FsmInt storeResult;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			this.DoGetChildCount();
			base.Finish();
		}

		private void DoGetChildCount()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.childCount;
		}
	}
}
