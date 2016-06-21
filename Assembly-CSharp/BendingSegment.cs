using System;
using UnityEngine;

[Serializable]
public class BendingSegment
{
	public Transform firstTransform;

	public Transform lastTransform;

	public float thresholdAngleDifference;

	public float bendingMultiplier = 0.6f;

	public float maxAngleDifference = 30f;

	public float maxBendingAngle = 80f;

	public float responsiveness = 5f;

	internal float angleH;

	internal float angleV;

	internal Vector3 dirUp;

	internal Vector3 referenceLookDir;

	internal Vector3 referenceUpDir;

	internal int chainLength;

	internal Quaternion[] origRotations;
}
