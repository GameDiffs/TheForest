using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Sets a Game Object's Tag.")]
	public class SetTag : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Tag)]
		public FsmString tag;

		public override void Reset()
		{
			this.gameObject = null;
			this.tag = "Untagged";
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				ownerDefaultTarget.tag = this.tag.Value;
			}
			base.Finish();
		}
	}
}
