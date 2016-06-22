using System;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Transform (Postion, Rotation, Scale)")]
	public class TransformConstraint : ConstraintBaseClass
	{
		public bool constrainPosition = true;

		public bool outputPosX = true;

		public bool outputPosY = true;

		public bool outputPosZ = true;

		public bool constrainRotation;

		public UnityConstraints.OUTPUT_ROT_OPTIONS output;

		public bool constrainScale;

		internal Transform parXform;

		internal static Transform scaleCalculator;

		protected override void Awake()
		{
			base.Awake();
			this.parXform = this.xform.parent;
		}

		protected override void OnConstraintUpdate()
		{
			if (this.constrainScale)
			{
				this.SetWorldScale(base.target);
			}
			if (this.constrainRotation)
			{
				this.xform.rotation = base.target.rotation;
				UnityConstraints.MaskOutputRotations(this.xform, this.output);
			}
			if (this.constrainPosition)
			{
				this.pos = this.xform.position;
				if (this.outputPosX)
				{
					this.pos.x = base.target.position.x;
				}
				if (this.outputPosY)
				{
					this.pos.y = base.target.position.y;
				}
				if (this.outputPosZ)
				{
					this.pos.z = base.target.position.z;
				}
				this.xform.position = this.pos;
			}
		}

		protected override void NoTargetDefault()
		{
			if (this.constrainScale)
			{
				this.xform.localScale = Vector3.one;
			}
			if (this.constrainRotation)
			{
				this.xform.rotation = Quaternion.identity;
			}
			if (this.constrainPosition)
			{
				this.xform.position = Vector3.zero;
			}
		}

		public virtual void SetWorldScale(Transform sourceXform)
		{
			this.xform.localScale = this.GetTargetLocalScale(sourceXform);
		}

		internal Vector3 GetTargetLocalScale(Transform sourceXform)
		{
			if (TransformConstraint.scaleCalculator == null)
			{
				string name = "TransformConstraint_spaceCalculator";
				GameObject gameObject = GameObject.Find(name);
				if (gameObject != null)
				{
					TransformConstraint.scaleCalculator = gameObject.transform;
				}
				else
				{
					TransformConstraint.scaleCalculator = new GameObject(name)
					{
						gameObject = 
						{
							hideFlags = HideFlags.HideAndDontSave
						}
					}.transform;
				}
			}
			Transform transform = TransformConstraint.scaleCalculator;
			transform.parent = sourceXform;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			transform.parent = this.parXform;
			return transform.localScale;
		}
	}
}
