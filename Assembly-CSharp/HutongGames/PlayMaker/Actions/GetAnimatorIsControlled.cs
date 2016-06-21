using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Returns true if a transform is controlled by the Animator. Can also send events")]
	public class GetAnimatorIsControlled : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject transform to check for control.")]
		public FsmGameObject transform;

		[ActionSection("Results"), HutongGames.PlayMaker.Tooltip("True if automatic matching is active"), UIHint(UIHint.Variable)]
		public FsmBool isControlled;

		[HutongGames.PlayMaker.Tooltip("Event send if the transform is controlled")]
		public FsmEvent isControlledEvent;

		[HutongGames.PlayMaker.Tooltip("Event send if the transform is not controlled")]
		public FsmEvent isNotControlledEvent;

		private Animator _animator;

		private Transform _transform;

		public override void Reset()
		{
			this.gameObject = null;
			this.transform = null;
			this.isControlled = null;
			this.isControlledEvent = null;
			this.isNotControlledEvent = null;
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
			GameObject value = this.transform.Value;
			if (value == null)
			{
				base.Finish();
				return;
			}
			this._transform = value.transform;
			this.DoCheckIsControlled();
			base.Finish();
		}

		private void DoCheckIsControlled()
		{
			if (this._animator == null)
			{
				return;
			}
		}
	}
}
