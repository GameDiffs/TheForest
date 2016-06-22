using System;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Transform - Smooth")]
	public class SmoothTransformConstraint : TransformConstraint
	{
		public enum INTERP_OPTIONS_POS
		{
			Linear,
			Damp,
			DampLimited
		}

		public float positionSpeed = 0.1f;

		public float rotationSpeed = 1f;

		public float scaleSpeed = 0.1f;

		public UnityConstraints.INTERP_OPTIONS interpolation = UnityConstraints.INTERP_OPTIONS.Spherical;

		public float positionMaxSpeed = 0.1f;

		public SmoothTransformConstraint.INTERP_OPTIONS_POS position_interpolation;

		private Vector3 curDampVelocity = Vector3.zero;

		protected override void OnConstraintUpdate()
		{
			if (this.constrainScale)
			{
				this.SetWorldScale(base.target);
			}
			this.OutputRotationTowards(base.target.rotation);
			this.OutputPositionTowards(base.target.position);
		}

		protected override void NoTargetDefault()
		{
			if (this.constrainScale)
			{
				this.xform.localScale = Vector3.one;
			}
			this.OutputRotationTowards(Quaternion.identity);
			this.OutputPositionTowards(base.target.position);
		}

		private void OutputPositionTowards(Vector3 destPos)
		{
			if (!this.constrainPosition)
			{
				return;
			}
			switch (this.position_interpolation)
			{
			case SmoothTransformConstraint.INTERP_OPTIONS_POS.Linear:
				this.pos = Vector3.Lerp(this.xform.position, destPos, this.positionSpeed);
				break;
			case SmoothTransformConstraint.INTERP_OPTIONS_POS.Damp:
				this.pos = Vector3.SmoothDamp(this.xform.position, destPos, ref this.curDampVelocity, this.positionSpeed);
				break;
			case SmoothTransformConstraint.INTERP_OPTIONS_POS.DampLimited:
				this.pos = Vector3.SmoothDamp(this.xform.position, destPos, ref this.curDampVelocity, this.positionSpeed, this.positionMaxSpeed);
				break;
			}
			if (!this.outputPosX)
			{
				this.pos.x = this.xform.position.x;
			}
			if (!this.outputPosY)
			{
				this.pos.y = this.xform.position.y;
			}
			if (!this.outputPosZ)
			{
				this.pos.z = this.xform.position.z;
			}
			this.xform.position = this.pos;
		}

		private void OutputRotationTowards(Quaternion destRot)
		{
			if (!this.constrainRotation)
			{
				return;
			}
			UnityConstraints.InterpolateRotationTo(this.xform, destRot, this.interpolation, this.rotationSpeed);
			UnityConstraints.MaskOutputRotations(this.xform, this.output);
		}

		public override void SetWorldScale(Transform sourceXform)
		{
			this.xform.localScale = Vector3.Lerp(this.xform.localScale, base.GetTargetLocalScale(sourceXform), this.scaleSpeed);
		}
	}
}
