using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body to Sleep at least one frame.")]
	public class Sleep : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			this.gameObject = null;
		}

		public override void OnEnter()
		{
			this.DoSleep();
			base.Finish();
		}

		private void DoSleep()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.Sleep();
			}
		}
	}
}
