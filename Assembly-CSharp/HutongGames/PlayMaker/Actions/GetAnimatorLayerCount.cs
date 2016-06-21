using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1051"), HutongGames.PlayMaker.Tooltip("Returns the Animator controller layer count")]
	public class GetAnimatorLayerCount : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Results"), RequiredField, HutongGames.PlayMaker.Tooltip("The Animator controller layer count"), UIHint(UIHint.Variable)]
		public FsmInt layerCount;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.layerCount = null;
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
			this.DoGetLayerCount();
			base.Finish();
		}

		private void DoGetLayerCount()
		{
			if (this._animator == null)
			{
				return;
			}
			this.layerCount.Value = this._animator.layerCount;
		}
	}
}
