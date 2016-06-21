using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUIElement), HutongGames.PlayMaker.Tooltip("Sets the Color of the GUITexture attached to a Game Object.")]
	public class SetGUITextureColor : ComponentAction<GUITexture>
	{
		[CheckForComponent(typeof(GUITexture)), RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmColor color;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.color = Color.white;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetGUITextureColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetGUITextureColor();
		}

		private void DoSetGUITextureColor()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.guiTexture.color = this.color.Value;
			}
		}
	}
}
