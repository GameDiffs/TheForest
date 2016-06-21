using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights), HutongGames.PlayMaker.Tooltip("Sets the Intensity of a Light.")]
	public class SetLightIntensity : ComponentAction<Light>
	{
		[CheckForComponent(typeof(Light)), RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmFloat lightIntensity;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.lightIntensity = 1f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetLightIntensity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetLightIntensity();
		}

		private void DoSetLightIntensity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.intensity = this.lightIntensity.Value;
			}
		}
	}
}
