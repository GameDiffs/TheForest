using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Gets the current State information on a specified layer")]
	public class GetAnimatorCurrentStateInfo : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
		public FsmInt layerIndex;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		[ActionSection("Results"), HutongGames.PlayMaker.Tooltip("The layer's name."), UIHint(UIHint.Variable)]
		public FsmString name;

		[HutongGames.PlayMaker.Tooltip("The layer's name Hash"), UIHint(UIHint.Variable)]
		public FsmInt nameHash;

		[HutongGames.PlayMaker.Tooltip("The layer's tag hash"), UIHint(UIHint.Variable)]
		public FsmInt tagHash;

		[HutongGames.PlayMaker.Tooltip("Is the state looping. All animations in the state must be looping"), UIHint(UIHint.Variable)]
		public FsmBool isStateLooping;

		[HutongGames.PlayMaker.Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree "), UIHint(UIHint.Variable)]
		public FsmFloat length;

		[HutongGames.PlayMaker.Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop"), UIHint(UIHint.Variable)]
		public FsmFloat normalizedTime;

		[HutongGames.PlayMaker.Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime"), UIHint(UIHint.Variable)]
		public FsmInt loopCount;

		[HutongGames.PlayMaker.Tooltip("The progress in the current loop. This is extracted from the normalizedTime"), UIHint(UIHint.Variable)]
		public FsmFloat currentLoopProgress;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameHash = null;
			this.tagHash = null;
			this.length = null;
			this.normalizedTime = null;
			this.isStateLooping = null;
			this.loopCount = null;
			this.currentLoopProgress = null;
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
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.GetLayerInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.GetLayerInfo();
			}
		}

		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.GetLayerInfo();
			}
		}

		private void GetLayerInfo()
		{
			if (this._animator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
				this.nameHash.Value = currentAnimatorStateInfo.nameHash;
				if (!this.name.IsNone)
				{
					this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
				}
				this.tagHash.Value = currentAnimatorStateInfo.tagHash;
				this.length.Value = currentAnimatorStateInfo.length;
				this.isStateLooping.Value = currentAnimatorStateInfo.loop;
				this.normalizedTime.Value = currentAnimatorStateInfo.normalizedTime;
				if (!this.loopCount.IsNone || !this.currentLoopProgress.IsNone)
				{
					this.loopCount.Value = (int)Math.Truncate((double)currentAnimatorStateInfo.normalizedTime);
					this.currentLoopProgress.Value = currentAnimatorStateInfo.normalizedTime - (float)this.loopCount.Value;
				}
			}
		}

		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}
	}
}
