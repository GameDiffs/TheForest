using System;
using UnityEngine;

public class simpleIkSolver : MonoBehaviour
{
	[Serializable]
	public class JointEntity
	{
		public Transform Joint;

		public simpleIkSolver.AngleRestriction AngleRestrictionRange;

		internal Quaternion _initialRotation;
	}

	[Serializable]
	public class AngleRestriction
	{
		public bool xAxis;

		public float xMin = -180f;

		public float xMax = 180f;

		public bool yAxis;

		public float yMin = -180f;

		public float yMax = 180f;

		public bool zAxis;

		public float zMin = 180f;

		public float zMax = 180f;
	}

	private const float IK_POS_THRESH = 0.0001f;

	private const int MAX_IK_TRIES = 20;

	public bool IsActive = true;

	public Transform Target;

	public simpleIkSolver.JointEntity[] JointEntities;

	public Transform leftFootTarget;

	public float heightOffset;

	public bool IsDamping;

	public float DampingMax = 0.5f;

	private void Start()
	{
		if (this.Target == null)
		{
			this.Target = base.transform;
		}
		simpleIkSolver.JointEntity[] jointEntities = this.JointEntities;
		for (int i = 0; i < jointEntities.Length; i++)
		{
			simpleIkSolver.JointEntity jointEntity = jointEntities[i];
			jointEntity._initialRotation = jointEntity.Joint.localRotation;
		}
	}

	private void LateUpdate()
	{
		this.Target.position = new Vector3(this.leftFootTarget.position.x, this.leftFootTarget.position.y + this.heightOffset, this.leftFootTarget.position.z);
		if (this.IsActive)
		{
			this.Solve();
		}
	}

	private void Solve()
	{
		Transform joint = this.JointEntities[this.JointEntities.Length - 1].Joint;
		Vector3 rhs = Vector3.zero;
		Vector3 lhs = Vector3.zero;
		Vector3 axis = Vector3.zero;
		int num = this.JointEntities.Length - 1;
		int num2 = 0;
		Vector3 position2;
		do
		{
			if (num < 0)
			{
				num = this.JointEntities.Length - 1;
			}
			Vector3 position = this.JointEntities[num].Joint.position;
			position2 = joint.position;
			lhs = position2 - position;
			rhs = this.Target.position - position;
			lhs.Normalize();
			rhs.Normalize();
			float num3 = Vector3.Dot(lhs, rhs);
			if (num3 < 0.99999f)
			{
				axis = Vector3.Cross(lhs, rhs);
				axis.Normalize();
				float num4 = Mathf.Acos(num3);
				if (this.IsDamping && num4 > this.DampingMax)
				{
					num4 = this.DampingMax;
				}
				num4 *= 57.29578f;
				this.JointEntities[num].Joint.rotation = Quaternion.AngleAxis(num4, axis) * this.JointEntities[num].Joint.rotation;
				this.CheckAngleRestrictions(this.JointEntities[num]);
			}
			num--;
		}
		while (num2++ < 20 && (position2 - this.Target.position).sqrMagnitude > 0.0001f);
	}

	private void CheckAngleRestrictions(simpleIkSolver.JointEntity jointEntity)
	{
		Vector3 eulerAngles = jointEntity.Joint.localRotation.eulerAngles;
		if (jointEntity.AngleRestrictionRange.xAxis)
		{
			if (eulerAngles.x > 180f)
			{
				eulerAngles.x -= 360f;
			}
			eulerAngles.x = Mathf.Clamp(eulerAngles.x, jointEntity.AngleRestrictionRange.xMin, jointEntity.AngleRestrictionRange.xMax);
		}
		if (jointEntity.AngleRestrictionRange.yAxis)
		{
			if (eulerAngles.y > 180f)
			{
				eulerAngles.y -= 360f;
			}
			eulerAngles.y = Mathf.Clamp(eulerAngles.y, jointEntity.AngleRestrictionRange.yMin, jointEntity.AngleRestrictionRange.yMax);
		}
		if (jointEntity.AngleRestrictionRange.zAxis)
		{
			if (eulerAngles.z > 180f)
			{
				eulerAngles.z -= 360f;
			}
			eulerAngles.z = Mathf.Clamp(eulerAngles.z, jointEntity.AngleRestrictionRange.zMin, jointEntity.AngleRestrictionRange.zMax);
		}
		jointEntity.Joint.localEulerAngles = eulerAngles;
	}

	public void ResetJoints()
	{
		simpleIkSolver.JointEntity[] jointEntities = this.JointEntities;
		for (int i = 0; i < jointEntities.Length; i++)
		{
			simpleIkSolver.JointEntity jointEntity = jointEntities[i];
			jointEntity.Joint.localRotation = jointEntity._initialRotation;
		}
	}
}
