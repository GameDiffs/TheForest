using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights), HutongGames.PlayMaker.Tooltip("Sets the Range of a Light.")]
	public class SetLightRange : ComponentAction<Light>
	{
		[CheckForComponent(typeof(Light)), RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmFloat lightRange;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.lightRange = 20f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetLightRange();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetLightRange();
		}

		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.range = this.lightRange.Value;
			}
		}
	}
}
