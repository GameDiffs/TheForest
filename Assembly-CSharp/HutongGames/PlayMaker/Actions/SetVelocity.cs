using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Sets the Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody.")]
	public class SetVelocity : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		public FsmFloat x;

		public FsmFloat y;

		public FsmFloat z;

		public Space space;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.Self;
			this.everyFrame = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		private void DoSetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space != Space.World) ? ownerDefaultTarget.transform.InverseTransformDirection(base.rigidbody.velocity) : base.rigidbody.velocity);
			}
			else
			{
				vector = this.vector.Value;
			}
			if (!this.x.IsNone)
			{
				vector.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				vector.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				vector.z = this.z.Value;
			}
			base.rigidbody.velocity = ((this.space != Space.World) ? ownerDefaultTarget.transform.TransformDirection(vector) : vector);
		}
	}
}
