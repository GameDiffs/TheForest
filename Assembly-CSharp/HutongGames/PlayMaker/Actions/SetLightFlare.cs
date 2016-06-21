using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights), HutongGames.PlayMaker.Tooltip("Sets the Flare effect used by a Light.")]
	public class SetLightFlare : ComponentAction<Light>
	{
		[CheckForComponent(typeof(Light)), RequiredField]
		public FsmOwnerDefault gameObject;

		public Flare lightFlare;

		public override void Reset()
		{
			this.gameObject = null;
			this.lightFlare = null;
		}

		public override void OnEnter()
		{
			this.DoSetLightRange();
			base.Finish();
		}

		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.flare = this.lightFlare;
			}
		}
	}
}
