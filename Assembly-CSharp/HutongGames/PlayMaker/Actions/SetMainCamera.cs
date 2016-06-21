using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Sets the main camera.")]
	public class SetMainCamera : FsmStateAction
	{
		[CheckForComponent(typeof(Camera)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to set as the main camera (should have a Camera component).")]
		public FsmGameObject gameObject;

		public override void Reset()
		{
			this.gameObject = null;
		}

		public override void OnEnter()
		{
			if (this.gameObject.Value != null)
			{
				if (Camera.main != null)
				{
					Camera.main.gameObject.tag = "Untagged";
				}
				this.gameObject.Value.tag = "MainCamera";
			}
			base.Finish();
		}
	}
}
