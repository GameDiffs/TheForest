using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Sets a Game Object's Name.")]
	public class SetName : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmString name;

		public override void Reset()
		{
			this.gameObject = null;
			this.name = null;
		}

		public override void OnEnter()
		{
			this.DoSetLayer();
			base.Finish();
		}

		private void DoSetLayer()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			ownerDefaultTarget.name = this.name.Value;
		}
	}
}
