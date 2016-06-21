using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Perform a raycast into the scene using screen coordinates and stores the results. Use Ray Distance to set how close the camera must be to pick the object. NOTE: Uses the MainCamera!")]
	public class ScreenPick : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("A Vector3 screen position. Commonly stored by other actions.")]
		public FsmVector3 screenVector;

		[HutongGames.PlayMaker.Tooltip("X position on screen.")]
		public FsmFloat screenX;

		[HutongGames.PlayMaker.Tooltip("Y position on screen.")]
		public FsmFloat screenY;

		[HutongGames.PlayMaker.Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
		public FsmBool normalized;

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
			this.screenVector = new FsmVector3
			{
				UseVariable = true
			};
			this.screenX = new FsmFloat
			{
				UseVariable = true
			};
			this.screenY = new FsmFloat
			{
				UseVariable = true
			};
			this.normalized = false;
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
			this.DoScreenPick();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoScreenPick();
		}

		private void DoScreenPick()
		{
			if (Camera.main == null)
			{
				this.LogError("No MainCamera defined!");
				base.Finish();
				return;
			}
			Vector3 position = Vector3.zero;
			if (!this.screenVector.IsNone)
			{
				position = this.screenVector.Value;
			}
			if (!this.screenX.IsNone)
			{
				position.x = this.screenX.Value;
			}
			if (!this.screenY.IsNone)
			{
				position.y = this.screenY.Value;
			}
			if (this.normalized.Value)
			{
				position.x *= (float)Screen.width;
				position.y *= (float)Screen.height;
			}
			Ray ray = Camera.main.ScreenPointToRay(position);
			RaycastHit raycastHit;
			Physics.Raycast(ray, out raycastHit, this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
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
				this.storeDistance = float.PositiveInfinity;
				this.storePoint.Value = Vector3.zero;
				this.storeNormal.Value = Vector3.zero;
			}
		}
	}
}
