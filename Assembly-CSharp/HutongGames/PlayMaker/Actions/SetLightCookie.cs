using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights), HutongGames.PlayMaker.Tooltip("Sets the Texture projected by a Light.")]
	public class SetLightCookie : ComponentAction<Light>
	{
		[CheckForComponent(typeof(Light)), RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmTexture lightCookie;

		public override void Reset()
		{
			this.gameObject = null;
			this.lightCookie = null;
		}

		public override void OnEnter()
		{
			this.DoSetLightCookie();
			base.Finish();
		}

		private void DoSetLightCookie()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.light.cookie = this.lightCookie.Value;
			}
		}
	}
}
