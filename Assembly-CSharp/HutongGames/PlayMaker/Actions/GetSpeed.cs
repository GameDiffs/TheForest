using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Gets the Speed of a Game Object and stores it in a Float Variable. NOTE: The Game Object must have a rigid body.")]
	public class GetSpeed : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the speed in a float variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetSpeed();
		}

		private void DoGetSpeed()
		{
			if (this.storeResult == null)
			{
				return;
			}
			GameObject go = (this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner;
			if (base.UpdateCache(go))
			{
				Vector3 velocity = base.rigidbody.velocity;
				this.storeResult.Value = velocity.magnitude;
			}
		}
	}
}
