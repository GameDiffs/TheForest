using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Perform a Mouse Pick on the scene and stores the results. Use Ray Distance to set how close the camera must be to pick the object.")]
	public class MousePick : FsmStateAction
	{
		[RequiredField]
		public FsmFloat rayDistance = 100f;

		[UIHint(UIHint.Variable)]
		public FsmBool storeDidPickObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeGameObject;

		[UIHint(UIHint.Variable)]
		public FsmVector3 storePoint;

		[UIHint(UIHint.Variable)]
		public FsmVector3 storeNormal;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeDistance;

		[HutongGames.PlayMaker.Tooltip("Pick only from these layers."), UIHint(UIHint.Layer)]
		public FsmInt[] layerMask;

		[HutongGames.PlayMaker.Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		public bool everyFrame;

		public override void Reset()
		{
			this.rayDistance = 100f;
			this.storeDidPickObject = null;
			this.storeGameObject = null;
			this.storePoint = null;
			this.storeNormal = null;
			this.storeDistance = null;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoMousePick();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoMousePick();
		}

		private void DoMousePick()
		{
			RaycastHit raycastHit = ActionHelpers.MousePick(this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			bool flag = raycastHit.collider != null;
			this.storeDidPickObject.Value = flag;
			if (flag)
			{
				this.storeGameObject.Value = raycastHit.collider.gameObject;
				this.storeDistance.Value = raycastHit.distance;
				this.storePoint.Value = raycastHit.point;
				this.storeNormal.Value = raycastHit.normal;
			}
			else
			{
				this.storeGameObject.Value = null;
				this.storeDistance.Value = float.PositiveInfinity;
				this.storePoint.Value = Vector3.zero;
				this.storeNormal.Value = Vector3.zero;
			}
		}
	}
}
