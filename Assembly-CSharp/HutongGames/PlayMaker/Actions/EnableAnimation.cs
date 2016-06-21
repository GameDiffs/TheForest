using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Enables/Disables an Animation on a GameObject.\nAnimation time is paused while disabled. Animation must also have a non zero weight to play.")]
	public class EnableAnimation : FsmStateAction
	{
		[CheckForComponent(typeof(Animation)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject playing the animation.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The name of the animation to enable/disable."), UIHint(UIHint.Animation)]
		public FsmString animName;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enable;

		[HutongGames.PlayMaker.Tooltip("Reset the initial enabled state when exiting the state.")]
		public FsmBool resetOnExit;

		private AnimationState anim;

		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.enable = true;
			this.resetOnExit = false;
		}

		public override void OnEnter()
		{
			this.DoEnableAnimation(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		private void DoEnableAnimation(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			Animation component = go.GetComponent<Animation>();
			if (component == null)
			{
				this.LogError("Missing animation component!");
				return;
			}
			this.anim = component[this.animName.Value];
			if (this.anim != null)
			{
				this.anim.enabled = this.enable.Value;
			}
		}

		public override void OnExit()
		{
			if (this.resetOnExit.Value && this.anim != null)
			{
				this.anim.enabled = !this.enable.Value;
			}
		}
	}
}
