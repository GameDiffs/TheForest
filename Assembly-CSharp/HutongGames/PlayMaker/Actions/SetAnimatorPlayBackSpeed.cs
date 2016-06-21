using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1072"), HutongGames.PlayMaker.Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
	public class SetAnimatorPlayBackSpeed : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The playBack speed")]
		public FsmFloat playBackSpeed;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.playBackSpeed = null;
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
			this.DoPlayBackSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoPlayBackSpeed();
		}

		private void DoPlayBackSpeed()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.speed = this.playBackSpeed.Value;
		}
	}
}
