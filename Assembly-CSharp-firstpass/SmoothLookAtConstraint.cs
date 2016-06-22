using System;
using UnityEngine;

[AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Look At - Smooth")]
public class SmoothLookAtConstraint : LookAtConstraint
{
	public UnityConstraints.INTERP_OPTIONS interpolation = UnityConstraints.INTERP_OPTIONS.Spherical;

	public float speed = 1f;

	public UnityConstraints.OUTPUT_ROT_OPTIONS output;

	private Quaternion lookRot;

	private Quaternion usrLookRot;

	private Quaternion curRot;

	private Vector3 angles;

	private Vector3 lookVectCache;

	protected override void OnConstraintUpdate()
	{
		this.lookVectCache = Vector3.zero;
		this.lookVectCache = this.lookVect;
		if (this.lookVectCache == Vector3.zero)
		{
			return;
		}
		this.lookRot = Quaternion.LookRotation(this.lookVectCache, base.upVect);
		this.usrLookRot = base.GetUserLookRotation(this.lookRot);
		this.OutputTowards(this.usrLookRot);
	}

	protected override void NoTargetDefault()
	{
		this.OutputTowards(Quaternion.identity);
	}

	private void OutputTowards(Quaternion destRot)
	{
		UnityConstraints.InterpolateRotationTo(this.xform, destRot, this.interpolation, this.speed);
		UnityConstraints.MaskOutputRotations(this.xform, this.output);
	}
}
