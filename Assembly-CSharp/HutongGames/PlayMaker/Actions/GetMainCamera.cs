using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Gets the camera tagged MainCamera from the scene")]
	public class GetMainCamera : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmGameObject storeGameObject;

		public override void Reset()
		{
			this.storeGameObject = null;
		}

		public override void OnEnter()
		{
			this.storeGameObject.Value = ((!(Camera.main != null)) ? null : Camera.main.gameObject);
			base.Finish();
		}
	}
}
