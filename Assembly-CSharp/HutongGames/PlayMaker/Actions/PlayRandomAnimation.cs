using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Plays a Random Animation on a Game Object. You can set the relative weight of each animation to control how often they are selected.")]
	public class PlayRandomAnimation : ComponentAction<Animation>
	{
		[CheckForComponent(typeof(Animation)), RequiredField, HutongGames.PlayMaker.Tooltip("Game Object to play the animation on.")]
		public FsmOwnerDefault gameObject;

		[CompoundArray("Animations", "Animation", "Weight"), UIHint(UIHint.Animation)]
		public FsmString[] animations;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

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
			this.animations = new FsmString[0];
			this.weights = new FsmFloat[0];
			this.playMode = PlayMode.StopAll;
			this.blendTime = 0.3f;
			this.finishEvent = null;
			this.loopEvent = null;
			this.stopOnExit = false;
		}

		public override void OnEnter()
		{
			this.DoPlayRandomAnimation();
		}

		private void DoPlayRandomAnimation()
		{
			if (this.animations.Length > 0)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				if (randomWeightedIndex != -1)
				{
					this.DoPlayAnimation(this.animations[randomWeightedIndex].Value);
				}
			}
		}

		private void DoPlayAnimation(string animName)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				base.Finish();
				return;
			}
			if (string.IsNullOrEmpty(animName))
			{
				this.LogWarning("Missing animName!");
				base.Finish();
				return;
			}
			this.anim = base.animation[animName];
			if (this.anim == null)
			{
				this.LogWarning("Missing animation: " + animName);
				base.Finish();
				return;
			}
			float value = this.blendTime.Value;
			if (value < 0.001f)
			{
				base.animation.Play(animName, this.playMode);
			}
			else
			{
				base.animation.CrossFade(animName, value, this.playMode);
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
				base.animation.Stop(this.anim.name);
			}
		}
	}
}
