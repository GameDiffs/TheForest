using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Level), HutongGames.PlayMaker.Tooltip("Makes the Game Object not be destroyed automatically when loading a new scene.")]
	public class DontDestroyOnLoad : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("GameObject to mark as DontDestroyOnLoad.")]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			this.gameObject = null;
		}

		public override void OnEnter()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.Owner.transform.root.gameObject);
			base.Finish();
		}
	}
}
