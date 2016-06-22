using System;
using UnityEngine;

[AddComponentMenu("")]
public class LookAtBaseClass : ConstraintBaseClass
{
	public Vector3 pointAxis = -Vector3.back;

	public Vector3 upAxis = Vector3.up;

	protected override Transform internalTarget
	{
		get
		{
			if (this._internalTarget != null)
			{
				return this._internalTarget;
			}
			Transform internalTarget = base.internalTarget;
			internalTarget.position = this.xform.rotation * this.pointAxis + this.xform.position;
			return this._internalTarget;
		}
	}

	protected override void NoTargetDefault()
	{
		this.xform.rotation = Quaternion.identity;
	}

	protected Quaternion GetUserLookRotation(Quaternion lookRot)
	{
		Quaternion rotation = Quaternion.LookRotation(this.pointAxis, this.upAxis);
		return lookRot * Quaternion.Inverse(rotation);
	}
}
