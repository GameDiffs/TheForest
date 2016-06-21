using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Sets whether a Game Object's Rigidy Body is affected by Gravity.")]
	public class UseGravity : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmBool useGravity;

		public override void Reset()
		{
			this.gameObject = null;
			this.useGravity = true;
		}

		public override void OnEnter()
		{
			this.DoUseGravity();
			base.Finish();
		}

		private void DoUseGravity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.useGravity = this.useGravity.Value;
			}
		}
	}
}
