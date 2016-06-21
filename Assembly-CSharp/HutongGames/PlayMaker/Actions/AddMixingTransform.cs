using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation), HutongGames.PlayMaker.Tooltip("Play an animation on a subset of the hierarchy. E.g., A waving animation on the upper body.")]
	public class AddMixingTransform : FsmStateAction
	{
		[CheckForComponent(typeof(Animation)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject playing the animation.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The name of the animation to mix. NOTE: The animation should already be added to the Animation Component on the GameObject.")]
		public FsmString animationName;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The mixing transform. E.g., root/upper_body/left_shoulder")]
		public FsmString transform;

		[HutongGames.PlayMaker.Tooltip("If recursive is true all children of the mix transform will also be animated.")]
		public FsmBool recursive;

		public override void Reset()
		{
			this.gameObject = null;
			this.animationName = string.Empty;
			this.transform = string.Empty;
			this.recursive = true;
		}

		public override void OnEnter()
		{
			this.DoAddMixingTransform();
			base.Finish();
		}

		private void DoAddMixingTransform()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Animation component = ownerDefaultTarget.GetComponent<Animation>();
			if (component == null)
			{
				return;
			}
			AnimationState animationState = component[this.animationName.Value];
			if (animationState == null)
			{
				return;
			}
			Transform mix = ownerDefaultTarget.transform.Find(this.transform.Value);
			animationState.AddMixingTransform(mix, this.recursive.Value);
		}
	}
}
