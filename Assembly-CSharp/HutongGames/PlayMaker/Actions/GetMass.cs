using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Gets the Mass of a Game Object's Rigid Body.")]
	public class GetMass : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the Rigidbody")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the mass in a float variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			this.DoGetMass();
			base.Finish();
		}

		private void DoGetMass()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.storeResult.Value = base.rigidbody.mass;
			}
		}
	}
}
