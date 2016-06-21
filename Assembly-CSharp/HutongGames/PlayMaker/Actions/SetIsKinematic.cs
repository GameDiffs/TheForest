using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Controls whether physics affects the Game Object.")]
	public class SetIsKinematic : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmBool isKinematic;

		public override void Reset()
		{
			this.gameObject = null;
			this.isKinematic = false;
		}

		public override void OnEnter()
		{
			this.DoSetIsKinematic();
			base.Finish();
		}

		private void DoSetIsKinematic()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.isKinematic = this.isKinematic.Value;
			}
		}
	}
}
