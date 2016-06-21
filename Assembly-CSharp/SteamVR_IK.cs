using System;
using UnityEngine;

public class SteamVR_IK : MonoBehaviour
{
	public Transform target;

	public Transform start;

	public Transform joint;

	public Transform end;

	public Transform poleVector;

	public Transform upVector;

	public float blendPct = 1f;

	[HideInInspector]
	public Transform startXform;

	[HideInInspector]
	public Transform jointXform;

	[HideInInspector]
	public Transform endXform;

	private void LateUpdate()
	{
		if (this.blendPct < 0.001f)
		{
			return;
		}
		Vector3 worldUp = (!this.upVector) ? Vector3.Cross(this.end.position - this.start.position, this.joint.position - this.start.position).normalized : this.upVector.up;
		Vector3 position = this.target.position;
		Quaternion rotation = this.target.rotation;
		Vector3 position2 = this.joint.position;
		Vector3 vector;
		Vector3 vector2;
		SteamVR_IK.Solve(this.start.position, position, this.poleVector.position, (this.joint.position - this.start.position).magnitude, (this.end.position - this.joint.position).magnitude, ref position2, out vector, out vector2);
		if (vector2 == Vector3.zero)
		{
			return;
		}
		Vector3 position3 = this.start.position;
		Vector3 position4 = this.joint.position;
		Vector3 position5 = this.end.position;
		Quaternion localRotation = this.start.localRotation;
		Quaternion localRotation2 = this.joint.localRotation;
		Quaternion localRotation3 = this.end.localRotation;
		Transform parent = this.start.parent;
		Transform parent2 = this.joint.parent;
		Transform parent3 = this.end.parent;
		Vector3 localScale = this.start.localScale;
		Vector3 localScale2 = this.joint.localScale;
		Vector3 localScale3 = this.end.localScale;
		if (this.startXform == null)
		{
			this.startXform = new GameObject("startXform").transform;
			this.startXform.parent = base.transform;
		}
		this.startXform.position = position3;
		this.startXform.LookAt(this.joint, worldUp);
		this.start.parent = this.startXform;
		if (this.jointXform == null)
		{
			this.jointXform = new GameObject("jointXform").transform;
			this.jointXform.parent = this.startXform;
		}
		this.jointXform.position = position4;
		this.jointXform.LookAt(this.end, worldUp);
		this.joint.parent = this.jointXform;
		if (this.endXform == null)
		{
			this.endXform = new GameObject("endXform").transform;
			this.endXform.parent = this.jointXform;
		}
		this.endXform.position = position5;
		this.end.parent = this.endXform;
		this.startXform.LookAt(position2, vector2);
		this.jointXform.LookAt(position, vector2);
		this.endXform.rotation = rotation;
		this.start.parent = parent;
		this.joint.parent = parent2;
		this.end.parent = parent3;
		this.end.rotation = rotation;
		if (this.blendPct < 1f)
		{
			this.start.localRotation = Quaternion.Slerp(localRotation, this.start.localRotation, this.blendPct);
			this.joint.localRotation = Quaternion.Slerp(localRotation2, this.joint.localRotation, this.blendPct);
			this.end.localRotation = Quaternion.Slerp(localRotation3, this.end.localRotation, this.blendPct);
		}
		this.start.localScale = localScale;
		this.joint.localScale = localScale2;
		this.end.localScale = localScale3;
	}

	public static bool Solve(Vector3 start, Vector3 end, Vector3 poleVector, float jointDist, float targetDist, ref Vector3 result, out Vector3 forward, out Vector3 up)
	{
		float num = jointDist + targetDist;
		Vector3 a = end - start;
		Vector3 normalized = (poleVector - start).normalized;
		float magnitude = a.magnitude;
		result = start;
		if (magnitude < 0.001f)
		{
			result += normalized * jointDist;
			forward = Vector3.Cross(normalized, Vector3.up);
			up = Vector3.Cross(forward, normalized).normalized;
		}
		else
		{
			forward = a * (1f / magnitude);
			up = Vector3.Cross(forward, normalized).normalized;
			if (magnitude + 0.001f < num)
			{
				float num2 = (num + magnitude) * 0.5f;
				if (num2 > jointDist + 0.001f && num2 > targetDist + 0.001f)
				{
					float num3 = Mathf.Sqrt(num2 * (num2 - jointDist) * (num2 - targetDist) * (num2 - magnitude));
					float num4 = 2f * num3 / magnitude;
					float d = Mathf.Sqrt(jointDist * jointDist - num4 * num4);
					Vector3 a2 = Vector3.Cross(up, forward);
					result += forward * d + a2 * num4;
					return true;
				}
				result += normalized * jointDist;
			}
			else
			{
				result += forward * jointDist;
			}
		}
		return false;
	}
}
