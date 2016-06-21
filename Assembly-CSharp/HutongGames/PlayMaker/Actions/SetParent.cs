using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Sets the Parent of a Game Object.")]
	public class SetParent : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The Game Object to parent.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The new parent for the Game Object.")]
		public FsmGameObject parent;

		[HutongGames.PlayMaker.Tooltip("Set the local position to 0,0,0 after parenting.")]
		public FsmBool resetLocalPosition;

		[HutongGames.PlayMaker.Tooltip("Set the local rotation to 0,0,0 after parenting.")]
		public FsmBool resetLocalRotation;

		public override void Reset()
		{
			this.gameObject = null;
			this.parent = null;
			this.resetLocalPosition = null;
			this.resetLocalRotation = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				ownerDefaultTarget.transform.parent = ((!(this.parent.Value == null)) ? this.parent.Value.transform : null);
				if (this.resetLocalPosition.Value)
				{
					ownerDefaultTarget.transform.localPosition = Vector3.zero;
				}
				if (this.resetLocalRotation.Value)
				{
					ownerDefaultTarget.transform.localRotation = Quaternion.identity;
				}
			}
			base.Finish();
		}
	}
}
