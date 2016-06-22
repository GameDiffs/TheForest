using System;
using UnityEngine;

[AddComponentMenu("Path-o-logical/UnityConstraints/Purchase Unity Constraints for More! (Or Install it after TargetPRO)")]
public class TurntableConstraint : ConstraintFrameworkBaseClass
{
	public float speed = 1f;

	public bool randomStart;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (Application.isPlaying && this.randomStart)
		{
			this.xform.Rotate(0f, UnityEngine.Random.value * 360f, 0f);
		}
	}

	protected override void OnConstraintUpdate()
	{
		this.xform.Rotate(0f, this.speed, 0f);
	}
}
