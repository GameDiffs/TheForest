using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Casts a Ray against all Colliders in the scene. Use either a Game Object or Vector3 world position as the origin of the ray. Use GetRaycastInfo to get more detailed info.")]
	public class Raycast : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		[HutongGames.PlayMaker.Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
		public FsmVector3 fromPosition;

		[HutongGames.PlayMaker.Tooltip("A vector3 direction vector")]
		public FsmVector3 direction;

		[HutongGames.PlayMaker.Tooltip("Cast the ray in world or local space. Note if no Game Object is specfied, the direction is in world space.")]
		public Space space;

		[HutongGames.PlayMaker.Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		[ActionSection("Result"), HutongGames.PlayMaker.Tooltip("Event to send if the ray hits an object."), UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		[HutongGames.PlayMaker.Tooltip("Set a bool variable to true if hit something, otherwise false."), UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		[HutongGames.PlayMaker.Tooltip("Store the game object hit in a variable."), UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;

		[HutongGames.PlayMaker.Tooltip("Get the world position of the ray hit point and store it in a variable."), UIHint(UIHint.Variable)]
		public FsmVector3 storeHitPoint;

		[HutongGames.PlayMaker.Tooltip("Get the normal at the hit point and store it in a variable."), UIHint(UIHint.Variable)]
		public FsmVector3 storeHitNormal;

		[HutongGames.PlayMaker.Tooltip("Get the distance along the ray to the hit point and store it in a variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeHitDistance;

		[ActionSection("Filter"), HutongGames.PlayMaker.Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		[HutongGames.PlayMaker.Tooltip("Pick only from these layers."), UIHint(UIHint.Layer)]
		public FsmInt[] layerMask;

		[HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[ActionSection("Debug"), HutongGames.PlayMaker.Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		[HutongGames.PlayMaker.Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		private int repeat;

		public override void Reset()
		{
			this.fromGameObject = null;
			this.fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.direction = new FsmVector3
			{
				UseVariable = true
			};
			this.space = Space.Self;
			this.distance = 100f;
			this.hitEvent = null;
			this.storeDidHit = null;
			this.storeHitObject = null;
			this.storeHitPoint = null;
			this.storeHitNormal = null;
			this.storeHitDistance = null;
			this.repeatInterval = 1;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.debugColor = Color.yellow;
			this.debug = false;
		}

		public override void OnEnter()
		{
			this.DoRaycast();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		private void DoRaycast()
		{
			this.repeat = this.repeatInterval.Value;
			if (this.distance.Value == 0f)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
			Vector3 vector = (!(ownerDefaultTarget != null)) ? this.fromPosition.Value : ownerDefaultTarget.transform.position;
			float num = float.PositiveInfinity;
			if (this.distance.Value > 0f)
			{
				num = this.distance.Value;
			}
			Vector3 a = this.direction.Value;
			if (ownerDefaultTarget != null && this.space == Space.Self)
			{
				a = ownerDefaultTarget.transform.TransformDirection(this.direction.Value);
			}
			RaycastHit raycastHitInfo;
			Physics.Raycast(vector, a, out raycastHitInfo, num, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			base.Fsm.RaycastHitInfo = raycastHitInfo;
			bool flag = raycastHitInfo.collider != null;
			this.storeDidHit.Value = flag;
			if (flag)
			{
				this.storeHitObject.Value = raycastHitInfo.collider.gameObject;
				this.storeHitPoint.Value = base.Fsm.RaycastHitInfo.point;
				this.storeHitNormal.Value = base.Fsm.RaycastHitInfo.normal;
				this.storeHitDistance.Value = base.Fsm.RaycastHitInfo.distance;
				base.Fsm.Event(this.hitEvent);
			}
			if (this.debug.Value)
			{
				float d = Mathf.Min(num, 1000f);
				Debug.DrawLine(vector, vector + a * d, this.debugColor.Value);
			}
		}
	}
}
