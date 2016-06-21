using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights), HutongGames.PlayMaker.Tooltip("Sets the Color of a Light.")]
	public class SetLightColor : ComponentAction<Light>
	{
		[CheckForComponent(typeof(Light)), RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmColor lightColor;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.lightColor = Color.white;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetLightColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetLightColor();
		}

		private void DoSetLightColor()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.color = this.lightColor.Value;
			}
		}
	}
}
