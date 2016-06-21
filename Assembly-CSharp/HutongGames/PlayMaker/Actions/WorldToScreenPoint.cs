using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Transforms position from world space into screen space. NOTE: Uses the MainCamera!")]
	public class WorldToScreenPoint : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("World position to transform into screen coordinates."), UIHint(UIHint.Variable)]
		public FsmVector3 worldPosition;

		[HutongGames.PlayMaker.Tooltip("World X position.")]
		public FsmFloat worldX;

		[HutongGames.PlayMaker.Tooltip("World Y position.")]
		public FsmFloat worldY;

		[HutongGames.PlayMaker.Tooltip("World Z position.")]
		public FsmFloat worldZ;

		[HutongGames.PlayMaker.Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero."), UIHint(UIHint.Variable)]
		public FsmVector3 storeScreenPoint;

		[HutongGames.PlayMaker.Tooltip("Store the screen X position in a Float Variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeScreenX;

		[HutongGames.PlayMaker.Tooltip("Store the screen Y position in a Float Variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeScreenY;

		[HutongGames.PlayMaker.Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public FsmBool normalize;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			this.worldPosition = null;
			this.worldX = new FsmFloat
			{
				UseVariable = true
			};
			this.worldY = new FsmFloat
			{
				UseVariable = true
			};
			this.worldZ = new FsmFloat
			{
				UseVariable = true
			};
			this.storeScreenPoint = null;
			this.storeScreenX = null;
			this.storeScreenY = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoWorldToScreenPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoWorldToScreenPoint();
		}

		private void DoWorldToScreenPoint()
		{
			if (Camera.main == null)
			{
				this.LogError("No MainCamera defined!");
				base.Finish();
				return;
			}
			Vector3 vector = Vector3.zero;
			if (!this.worldPosition.IsNone)
			{
				vector = this.worldPosition.Value;
			}
			if (!this.worldX.IsNone)
			{
				vector.x = this.worldX.Value;
			}
			if (!this.worldY.IsNone)
			{
				vector.y = this.worldY.Value;
			}
			if (!this.worldZ.IsNone)
			{
				vector.z = this.worldZ.Value;
			}
			vector = Camera.main.WorldToScreenPoint(vector);
			if (this.normalize.Value)
			{
				vector.x /= (float)Screen.width;
				vector.y /= (float)Screen.height;
			}
			this.storeScreenPoint.Value = vector;
			this.storeScreenX.Value = vector.x;
			this.storeScreenY.Value = vector.y;
		}
	}
}
