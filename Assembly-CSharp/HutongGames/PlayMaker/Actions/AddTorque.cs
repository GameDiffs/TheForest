using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Adds torque (rotational force) to a Game Object.")]
	public class AddTorque : ComponentAction<Rigidbody>
	{
		[CheckForComponent(typeof(Rigidbody)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to add torque to.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("A Vector3 torque. Optionally override any axis with the X, Y, Z parameters."), UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		[HutongGames.PlayMaker.Tooltip("Torque around the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		[HutongGames.PlayMaker.Tooltip("Torque around the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		[HutongGames.PlayMaker.Tooltip("Torque around the Z axis. To leave unchanged, set to 'None'.")]
		public FsmFloat z;

		[HutongGames.PlayMaker.Tooltip("Apply the force in world or local space.")]
		public Space space;

		[HutongGames.PlayMaker.Tooltip("The type of force to apply. See Unity Physics docs.")]
		public ForceMode forceMode;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
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
			this.space = Space.World;
			this.forceMode = ForceMode.Force;
			this.everyFrame = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			this.DoAddTorque();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			this.DoAddTorque();
		}

		private void DoAddTorque()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 torque = (!this.vector.IsNone) ? this.vector.Value : new Vector3(this.x.Value, this.y.Value, this.z.Value);
			if (!this.x.IsNone)
			{
				torque.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				torque.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				torque.z = this.z.Value;
			}
			if (this.space == Space.World)
			{
				base.rigidbody.AddTorque(torque, this.forceMode);
			}
			else
			{
				base.rigidbody.AddRelativeTorque(torque, this.forceMode);
			}
		}
	}
}
