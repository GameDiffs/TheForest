using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Transforms position from screen space into world space. NOTE: Uses the MainCamera!")]
	public class ScreenToWorldPoint : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Screen position as a vector."), UIHint(UIHint.Variable)]
		public FsmVector3 screenVector;

		[HutongGames.PlayMaker.Tooltip("Screen X position in pixels or normalized. See Normalized.")]
		public FsmFloat screenX;

		[HutongGames.PlayMaker.Tooltip("Screen X position in pixels or normalized. See Normalized.")]
		public FsmFloat screenY;

		[HutongGames.PlayMaker.Tooltip("Distance into the screen in world units.")]
		public FsmFloat screenZ;

		[HutongGames.PlayMaker.Tooltip("If true, X/Y coordinates are considered normalized (0-1), otherwise they are expected to be in pixels")]
		public FsmBool normalized;

		[HutongGames.PlayMaker.Tooltip("Store the world position in a vector3 variable."), UIHint(UIHint.Variable)]
		public FsmVector3 storeWorldVector;

		[HutongGames.PlayMaker.Tooltip("Store the world X position in a float variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeWorldX;

		[HutongGames.PlayMaker.Tooltip("Store the world Y position in a float variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeWorldY;

		[HutongGames.PlayMaker.Tooltip("Store the world Z position in a float variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeWorldZ;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			this.screenVector = null;
			this.screenX = new FsmFloat
			{
				UseVariable = true
			};
			this.screenY = new FsmFloat
			{
				UseVariable = true
			};
			this.screenZ = 1f;
			this.normalized = false;
			this.storeWorldVector = null;
			this.storeWorldX = null;
			this.storeWorldY = null;
			this.storeWorldZ = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoScreenToWorldPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoScreenToWorldPoint();
		}

		private void DoScreenToWorldPoint()
		{
			if (Camera.main == null)
			{
				this.LogError("No MainCamera defined!");
				base.Finish();
				return;
			}
			Vector3 vector = Vector3.zero;
			if (!this.screenVector.IsNone)
			{
				vector = this.screenVector.Value;
			}
			if (!this.screenX.IsNone)
			{
				vector.x = this.screenX.Value;
			}
			if (!this.screenY.IsNone)
			{
				vector.y = this.screenY.Value;
			}
			if (!this.screenZ.IsNone)
			{
				vector.z = this.screenZ.Value;
			}
			if (this.normalized.Value)
			{
				vector.x *= (float)Screen.width;
				vector.y *= (float)Screen.height;
			}
			vector = Camera.main.ScreenToWorldPoint(vector);
			this.storeWorldVector.Value = vector;
			this.storeWorldX.Value = vector.x;
			this.storeWorldY.Value = vector.y;
			this.storeWorldZ.Value = vector.z;
		}
	}
}
