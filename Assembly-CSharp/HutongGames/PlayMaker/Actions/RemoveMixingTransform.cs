using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Removes a mixing transform previously added with Add Mixing Transform. If transform has been added as recursive, then it will be removed as recursive. Once you remove all mixing transforms added to animation state all curves become animated again.")]
	public class RemoveMixingTransform : ComponentAction<Animation>
	{
		[CheckForComponent(typeof(Animation)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject playing the animation.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The name of the animation.")]
		public FsmString animationName;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The mixing transform to remove. E.g., root/upper_body/left_shoulder")]
		public FsmString transfrom;

		public override void Reset()
		{
			this.gameObject = null;
			this.animationName = string.Empty;
		}

		public override void OnEnter()
		{
			this.DoRemoveMixingTransform();
			base.Finish();
		}

		private void DoRemoveMixingTransform()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			AnimationState animationState = base.animation[this.animationName.Value];
			if (animationState == null)
			{
				return;
			}
			Transform mix = ownerDefaultTarget.transform.Find(this.transfrom.Value);
			animationState.AddMixingTransform(mix);
		}
	}
}
