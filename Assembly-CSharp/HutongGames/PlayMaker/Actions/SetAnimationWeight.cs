using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Sets the Blend Weight of an Animation. Check Every Frame to update the weight continuosly, e.g., if you're manipulating a variable that controls the weight.")]
	public class SetAnimationWeight : ComponentAction<Animation>
	{
		[CheckForComponent(typeof(Animation)), RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField, UIHint(UIHint.Animation)]
		public FsmString animName;

		public FsmFloat weight = 1f;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.weight = 1f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetAnimationWeight((this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetAnimationWeight((this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner);
		}

		private void DoSetAnimationWeight(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			AnimationState animationState = base.animation[this.animName.Value];
			if (animationState == null)
			{
				this.LogWarning("Missing animation: " + this.animName.Value);
				return;
			}
			animationState.weight = this.weight.Value;
		}
	}
}
