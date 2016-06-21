using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Check the current State name on a specified layer, this is more than the layer name, it holds the current state as well.")]
	public class GetAnimatorCurrentStateInfoIsName : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
		public FsmInt layerIndex;

		[HutongGames.PlayMaker.Tooltip("The name to check the layer against.")]
		public FsmString name;

		public bool everyFrame;

		[ActionSection("Results")]
		public FsmBool nameMatch;

		public FsmEvent nameMatchEvent;

		public FsmEvent nameDoNotMatchEvent;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameMatch = null;
			this.nameMatchEvent = null;
			this.nameDoNotMatchEvent = null;
			this.everyFrame = false;
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
			this.IsName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.IsName();
		}

		private void IsName()
		{
			if (this._animator != null)
			{
				if (this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value).IsName(this.name.Value))
				{
					this.nameMatch.Value = true;
					base.Fsm.Event(this.nameMatchEvent);
				}
				else
				{
					this.nameMatch.Value = false;
					base.Fsm.Event(this.nameDoNotMatchEvent);
				}
			}
		}
	}
}
