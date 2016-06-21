using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Adds a named Animation Clip to a Game Object. Optionally trims the Animation.")]
	public class AddAnimationClip : FsmStateAction
	{
		[CheckForComponent(typeof(Animation)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to add the Animation Clip to.")]
		public FsmOwnerDefault gameObject;

		[ObjectType(typeof(AnimationClip)), RequiredField, HutongGames.PlayMaker.Tooltip("The animation clip to add. NOTE: Make sure the clip is compatible with the object's hierarchy.")]
		public FsmObject animationClip;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Name the animation. Used by other actions to reference this animation.")]
		public FsmString animationName;

		[HutongGames.PlayMaker.Tooltip("Optionally trim the animation by specifying a first and last frame.")]
		public FsmInt firstFrame;

		[HutongGames.PlayMaker.Tooltip("Optionally trim the animation by specifying a first and last frame.")]
		public FsmInt lastFrame;

		[HutongGames.PlayMaker.Tooltip("Add an extra looping frame that matches the first frame.")]
		public FsmBool addLoopFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.animationClip = null;
			this.animationName = string.Empty;
			this.firstFrame = 0;
			this.lastFrame = 0;
			this.addLoopFrame = false;
		}

		public override void OnEnter()
		{
			this.DoAddAnimationClip();
			base.Finish();
		}

		private void DoAddAnimationClip()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AnimationClip animationClip = this.animationClip.Value as AnimationClip;
			if (animationClip == null)
			{
				return;
			}
			Animation component = ownerDefaultTarget.GetComponent<Animation>();
			if (this.firstFrame.Value == 0 && this.lastFrame.Value == 0)
			{
				component.AddClip(animationClip, this.animationName.Value);
			}
			else
			{
				component.AddClip(animationClip, this.animationName.Value, this.firstFrame.Value, this.lastFrame.Value, this.addLoopFrame.Value);
			}
		}
	}
}
