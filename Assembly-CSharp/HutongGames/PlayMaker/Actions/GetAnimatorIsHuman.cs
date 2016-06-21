using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Returns true if the current rig is humanoid, false if it is generic. Can also sends events")]
	public class GetAnimatorIsHuman : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Results"), HutongGames.PlayMaker.Tooltip("True if the current rig is humanoid, False if it is generic"), UIHint(UIHint.Variable)]
		public FsmBool isHuman;

		[HutongGames.PlayMaker.Tooltip("Event send if rig is humanoid")]
		public FsmEvent isHumanEvent;

		[HutongGames.PlayMaker.Tooltip("Event send if rig is generic")]
		public FsmEvent isGenericEvent;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.isHuman = null;
			this.isHumanEvent = null;
			this.isGenericEvent = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this._animator = ownerDefaultTarget.GetComponent<Animator>();
			if (this._animator == null)
			{
				base.Finish();
				return;
			}
			this.DoCheckIsHuman();
			base.Finish();
		}

		private void DoCheckIsHuman()
		{
			if (this._animator == null)
			{
				return;
			}
			bool flag = this._animator.isHuman;
			this.isHuman.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.isHumanEvent);
			}
			else
			{
				base.Fsm.Event(this.isGenericEvent);
			}
		}
	}
}
