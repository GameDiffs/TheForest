using System;
using System.Collections;
using UnityEngine;

public class HeadLookController : MonoBehaviour
{
	public Transform rootNode;

	public BendingSegment[] segments;

	public NonAffectedJoints[] nonAffectedJoints;

	public Vector3 headLookVector = Vector3.forward;

	public Vector3 headUpVector = Vector3.up;

	public Vector3 target = Vector3.zero;

	public float effect = 1f;

	public bool overrideAnimation;

	private void Start()
	{
		if (this.rootNode == null)
		{
			this.rootNode = base.transform;
		}
		BendingSegment[] array = this.segments;
		for (int i = 0; i < array.Length; i++)
		{
			BendingSegment bendingSegment = array[i];
			Quaternion rotation = bendingSegment.firstTransform.parent.rotation;
			Quaternion lhs = Quaternion.Inverse(rotation);
			bendingSegment.referenceLookDir = lhs * this.rootNode.rotation * this.headLookVector.normalized;
			bendingSegment.referenceUpDir = lhs * this.rootNode.rotation * this.headUpVector.normalized;
			bendingSegment.angleH = 0f;
			bendingSegment.angleV = 0f;
			bendingSegment.dirUp = bendingSegment.referenceUpDir;
			bendingSegment.chainLength = 1;
			Transform transform = bendingSegment.lastTransform;
			while (transform != bendingSegment.firstTransform && transform != transform.root)
			{
				bendingSegment.chainLength++;
				transform = transform.parent;
			}
			bendingSegment.origRotations = new Quaternion[bendingSegment.chainLength];
			transform = bendingSegment.lastTransform;
			for (int j = bendingSegment.chainLength - 1; j >= 0; j--)
			{
				bendingSegment.origRotations[j] = transform.localRotation;
				transform = transform.parent;
			}
		}
	}

	private void LateUpdate()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		Vector3[] array = new Vector3[this.nonAffectedJoints.Length];
		for (int i = 0; i < this.nonAffectedJoints.Length; i++)
		{
			IEnumerator enumerator = this.nonAffectedJoints[i].joint.GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					array[i] = transform.position - this.nonAffectedJoints[i].joint.position;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		BendingSegment[] array2 = this.segments;
		for (int j = 0; j < array2.Length; j++)
		{
			BendingSegment bendingSegment = array2[j];
			Transform transform2 = bendingSegment.lastTransform;
			if (this.overrideAnimation)
			{
				for (int k = bendingSegment.chainLength - 1; k >= 0; k--)
				{
					transform2.localRotation = bendingSegment.origRotations[k];
					transform2 = transform2.parent;
				}
			}
			Quaternion rotation = bendingSegment.firstTransform.parent.rotation;
			Quaternion rotation2 = Quaternion.Inverse(rotation);
			Vector3 normalized = (this.target - bendingSegment.lastTransform.position).normalized;
			Vector3 vector = rotation2 * normalized;
			float num = HeadLookController.AngleAroundAxis(bendingSegment.referenceLookDir, vector, bendingSegment.referenceUpDir);
			Vector3 axis = Vector3.Cross(bendingSegment.referenceUpDir, vector);
			Vector3 dirA = vector - Vector3.Project(vector, bendingSegment.referenceUpDir);
			float num2 = HeadLookController.AngleAroundAxis(dirA, vector, axis);
			float f = Mathf.Max(0f, Mathf.Abs(num) - bendingSegment.thresholdAngleDifference) * Mathf.Sign(num);
			float f2 = Mathf.Max(0f, Mathf.Abs(num2) - bendingSegment.thresholdAngleDifference) * Mathf.Sign(num2);
			num = Mathf.Max(Mathf.Abs(f) * Mathf.Abs(bendingSegment.bendingMultiplier), Mathf.Abs(num) - bendingSegment.maxAngleDifference) * Mathf.Sign(num) * Mathf.Sign(bendingSegment.bendingMultiplier);
			num2 = Mathf.Max(Mathf.Abs(f2) * Mathf.Abs(bendingSegment.bendingMultiplier), Mathf.Abs(num2) - bendingSegment.maxAngleDifference) * Mathf.Sign(num2) * Mathf.Sign(bendingSegment.bendingMultiplier);
			num = Mathf.Clamp(num, -bendingSegment.maxBendingAngle, bendingSegment.maxBendingAngle);
			num2 = Mathf.Clamp(num2, -bendingSegment.maxBendingAngle, bendingSegment.maxBendingAngle);
			Vector3 axis2 = Vector3.Cross(bendingSegment.referenceUpDir, bendingSegment.referenceLookDir);
			bendingSegment.angleH = Mathf.Lerp(bendingSegment.angleH, num, Time.deltaTime * bendingSegment.responsiveness);
			bendingSegment.angleV = Mathf.Lerp(bendingSegment.angleV, num2, Time.deltaTime * bendingSegment.responsiveness);
			vector = Quaternion.AngleAxis(bendingSegment.angleH, bendingSegment.referenceUpDir) * Quaternion.AngleAxis(bendingSegment.angleV, axis2) * bendingSegment.referenceLookDir;
			Vector3 referenceUpDir = bendingSegment.referenceUpDir;
			Vector3.OrthoNormalize(ref vector, ref referenceUpDir);
			Vector3 forward = vector;
			bendingSegment.dirUp = Vector3.Slerp(bendingSegment.dirUp, referenceUpDir, Time.deltaTime * 5f);
			Vector3.OrthoNormalize(ref forward, ref bendingSegment.dirUp);
			Quaternion to = rotation * Quaternion.LookRotation(forward, bendingSegment.dirUp) * Quaternion.Inverse(rotation * Quaternion.LookRotation(bendingSegment.referenceLookDir, bendingSegment.referenceUpDir));
			Quaternion lhs = Quaternion.Slerp(Quaternion.identity, to, this.effect / (float)bendingSegment.chainLength);
			transform2 = bendingSegment.lastTransform;
			for (int l = 0; l < bendingSegment.chainLength; l++)
			{
				transform2.rotation = lhs * transform2.rotation;
				transform2 = transform2.parent;
			}
		}
		for (int m = 0; m < this.nonAffectedJoints.Length; m++)
		{
			Vector3 vector2 = Vector3.zero;
			IEnumerator enumerator2 = this.nonAffectedJoints[m].joint.GetEnumerator();
			try
			{
				if (enumerator2.MoveNext())
				{
					Transform transform3 = (Transform)enumerator2.Current;
					vector2 = transform3.position - this.nonAffectedJoints[m].joint.position;
				}
			}
			finally
			{
				IDisposable disposable2 = enumerator2 as IDisposable;
				if (disposable2 != null)
				{
					disposable2.Dispose();
				}
			}
			Vector3 toDirection = Vector3.Slerp(array[m], vector2, this.nonAffectedJoints[m].effect);
			this.nonAffectedJoints[m].joint.rotation = Quaternion.FromToRotation(vector2, toDirection) * this.nonAffectedJoints[m].joint.rotation;
		}
	}

	public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		float num = Vector3.Angle(dirA, dirB);
		return num * (float)((Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) >= 0f) ? 1 : -1);
	}
}
