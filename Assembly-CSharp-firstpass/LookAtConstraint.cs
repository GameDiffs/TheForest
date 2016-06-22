using System;
using UnityEngine;

[AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Look At")]
public class LookAtConstraint : LookAtBaseClass
{
	public Transform upTarget;

	protected virtual Vector3 lookVect
	{
		get
		{
			return base.target.position - this.xform.position;
		}
	}

	protected Vector3 upVect
	{
		get
		{
			Vector3 result;
			if (this.upTarget == null)
			{
				result = Vector3.up;
			}
			else
			{
				result = this.upTarget.position - this.xform.position;
			}
			return result;
		}
	}

	protected override void OnConstraintUpdate()
	{
		Quaternion lookRot = Quaternion.LookRotation(this.lookVect, this.upVect);
		this.xform.rotation = base.GetUserLookRotation(lookRot);
	}
}
