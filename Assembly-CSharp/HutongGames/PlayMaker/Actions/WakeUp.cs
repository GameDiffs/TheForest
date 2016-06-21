using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body to wake up.")]
	public class WakeUp : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			this.gameObject = null;
		}

		public override void OnEnter()
		{
			this.DoWakeUp();
			base.Finish();
		}

		private void DoWakeUp()
		{
			GameObject go = (this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner;
			if (base.UpdateCache(go))
			{
				base.rigidbody.WakeUp();
			}
		}
	}
}
