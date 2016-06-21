using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Sets the Background Color used by the Camera.")]
	public class SetBackgroundColor : ComponentAction<Camera>
	{
		[CheckForComponent(typeof(Camera)), RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmColor backgroundColor;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.backgroundColor = Color.black;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetBackgroundColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetBackgroundColor();
		}

		private void DoSetBackgroundColor()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.camera.backgroundColor = this.backgroundColor.Value;
			}
		}
	}
}
