using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Sets the current Time of an Animation, Normalize time means 0 (start) to 1 (end); useful if you don't care about the exact time. Check Every Frame to update the time continuosly.")]
	public class SetAnimationTime : ComponentAction<Animation>
	{
		[CheckForComponent(typeof(Animation)), RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField, UIHint(UIHint.Animation)]
		public FsmString animName;

		public FsmFloat time;

		public bool normalized;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.time = null;
			this.normalized = false;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetAnimationTime((this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetAnimationTime((this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner);
		}

		private void DoSetAnimationTime(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			base.animation.Play(this.animName.Value);
			AnimationState animationState = base.animation[this.animName.Value];
			if (animationState == null)
			{
				this.LogWarning("Missing animation: " + this.animName.Value);
				return;
			}
			if (this.normalized)
			{
				animationState.normalizedTime = this.time.Value;
			}
			else
			{
				animationState.time = this.time.Value;
			}
			if (this.everyFrame)
			{
				animationState.speed = 0f;
			}
		}
	}
}
