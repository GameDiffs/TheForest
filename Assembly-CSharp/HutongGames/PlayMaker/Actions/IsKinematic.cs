using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Tests if a Game Object's Rigid Body is Kinematic.")]
	public class IsKinematic : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool store;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.store = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoIsKinematic();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoIsKinematic();
		}

		private void DoIsKinematic()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				bool isKinematic = base.rigidbody.isKinematic;
				this.store.Value = isKinematic;
				base.Fsm.Event((!isKinematic) ? this.falseEvent : this.trueEvent);
			}
		}
	}
}
