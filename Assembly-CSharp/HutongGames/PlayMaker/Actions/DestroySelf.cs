using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Destroys the Owner of the Fsm! Useful for spawned Prefabs that need to kill themselves, e.g., a projectile that explodes on impact.")]
	public class DestroySelf : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Detach children before destroying the Owner.")]
		public FsmBool detachChildren;

		public override void Reset()
		{
			this.detachChildren = false;
		}

		public override void OnEnter()
		{
			if (base.Owner != null)
			{
				if (this.detachChildren.Value)
				{
					base.Owner.transform.DetachChildren();
				}
				UnityEngine.Object.Destroy(base.Owner);
			}
			base.Finish();
		}
	}
}
