using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights), HutongGames.PlayMaker.Tooltip("Set Spot, Directional, or Point Light type.")]
	public class SetLightType : ComponentAction<Light>
	{
		[CheckForComponent(typeof(Light)), RequiredField]
		public FsmOwnerDefault gameObject;

		public LightType lightType;

		public override void Reset()
		{
			this.gameObject = null;
			this.lightType = LightType.Point;
		}

		public override void OnEnter()
		{
			this.DoSetLightType();
			base.Finish();
		}

		private void DoSetLightType()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.type = this.lightType;
			}
		}
	}
}
