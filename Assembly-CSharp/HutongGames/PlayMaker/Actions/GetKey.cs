using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Gets the pressed state of a Key.")]
	public class GetKey : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The key to test.")]
		public KeyCode key;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store if the key is down (True) or up (False)."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you're waiting for a key press/release.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.key = KeyCode.None;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetKey();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetKey();
		}

		private void DoGetKey()
		{
			this.storeResult.Value = Input.GetKey(this.key);
		}
	}
}
