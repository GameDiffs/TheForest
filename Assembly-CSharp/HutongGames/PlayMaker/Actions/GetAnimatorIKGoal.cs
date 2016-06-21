using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Gets the position, rotation and weights of an IK goal. A GameObject can be set to use for the position and rotation")]
	public class GetAnimatorIKGoal : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The IK goal")]
		public AvatarIKGoal iKGoal;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		[ActionSection("Results"), HutongGames.PlayMaker.Tooltip("The gameObject to apply ik goal position and rotation to")]
		public FsmGameObject goal;

		[HutongGames.PlayMaker.Tooltip("Gets The position of the ik goal. If Goal GameObject define, position is used as an offset from Goal"), UIHint(UIHint.Variable)]
		public FsmVector3 position;

		[HutongGames.PlayMaker.Tooltip("Gets The rotation of the ik goal.If Goal GameObject define, rotation is used as an offset from Goal"), UIHint(UIHint.Variable)]
		public FsmQuaternion rotation;

		[HutongGames.PlayMaker.Tooltip("Gets The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)"), UIHint(UIHint.Variable)]
		public FsmFloat positionWeight;

		[HutongGames.PlayMaker.Tooltip("Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)"), UIHint(UIHint.Variable)]
		public FsmFloat rotationWeight;

		private Animator _animator;

		private Transform _transform;

		public override void Reset()
		{
			this.gameObject = null;
			this.goal = null;
			this.position = null;
			this.rotation = null;
			this.positionWeight = null;
			this.rotationWeight = null;
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
			GameObject value = this.goal.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this.DoGetIKGoal();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetIKGoal();
		}

		private void DoGetIKGoal()
		{
			if (this._animator == null)
			{
				return;
			}
			if (this._transform != null)
			{
				this._transform.position = this._animator.GetIKPosition(this.iKGoal);
				this._transform.rotation = this._animator.GetIKRotation(this.iKGoal);
			}
			if (!this.position.IsNone)
			{
				this.position.Value = this._animator.GetIKPosition(this.iKGoal);
			}
			if (!this.rotation.IsNone)
			{
				this.rotation.Value = this._animator.GetIKRotation(this.iKGoal);
			}
			if (!this.positionWeight.IsNone)
			{
				this.positionWeight.Value = this._animator.GetIKPositionWeight(this.iKGoal);
			}
			if (!this.rotationWeight.IsNone)
			{
				this.rotationWeight.Value = this._animator.GetIKRotationWeight(this.iKGoal);
			}
		}
	}
}
