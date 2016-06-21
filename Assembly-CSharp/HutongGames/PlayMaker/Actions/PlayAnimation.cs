using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Plays an Animation on a Game Object. You can add named animation clips to the object in the Unity editor, or with the Add Animation Clip action.")]
	public class PlayAnimation : ComponentAction<Animation>
	{
		[CheckForComponent(typeof(Animation)), RequiredField, HutongGames.PlayMaker.Tooltip("Game Object to play the animation on.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The name of the animation to play."), UIHint(UIHint.Animation)]
		public FsmString animName;

		[HutongGames.PlayMaker.Tooltip("How to treat previously playing animations.")]
		public PlayMode playMode;

		[HasFloatSlider(0f, 5f), HutongGames.PlayMaker.Tooltip("Time taken to blend to this animation.")]
		public FsmFloat blendTime;

		[HutongGames.PlayMaker.Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
		public FsmEvent finishEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
		public FsmEvent loopEvent;

		[HutongGames.PlayMaker.Tooltip("Stop playing the animation when this state is exited.")]
		public bool stopOnExit;

		private AnimationState anim;

		private float prevAnimtTime;

		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.playMode = PlayMode.StopAll;
			this.blendTime = 0.3f;
			this.finishEvent = null;
			this.loopEvent = null;
			this.stopOnExit = false;
		}

		public override void OnEnter()
		{
			this.DoPlayAnimation();
		}

		private void DoPlayAnimation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				base.Finish();
				return;
			}
			if (string.IsNullOrEmpty(this.animName.Value))
			{
				this.LogWarning("Missing animName!");
				base.Finish();
				return;
			}
			this.anim = base.animation[this.animName.Value];
			if (this.anim == null)
			{
				this.LogWarning("Missing animation: " + this.animName.Value);
				base.Finish();
				return;
			}
			float value = this.blendTime.Value;
			if (value < 0.001f)
			{
				base.animation.Play(this.animName.Value, this.playMode);
			}
			else
			{
				base.animation.CrossFade(this.animName.Value, value, this.playMode);
			}
			this.prevAnimtTime = this.anim.time;
		}

		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || this.anim == null)
			{
				return;
			}
			if (!this.anim.enabled || (this.anim.wrapMode == WrapMode.ClampForever && this.anim.time > this.anim.length))
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
			if (this.anim.wrapMode != WrapMode.ClampForever && this.anim.time > this.anim.length && this.prevAnimtTime < this.anim.length)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		public override void OnExit()
		{
			if (this.stopOnExit)
			{
				this.StopAnimation();
			}
		}

		private void StopAnimation()
		{
			if (base.animation != null)
			{
				base.animation.Stop(this.animName.Value);
			}
		}
	}
}
