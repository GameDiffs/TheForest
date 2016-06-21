using System;
using TheForest.Utils;
using UnityEngine;

public class simpleIkLegs : MonoBehaviour
{
	public LayerMask groundMask;

	public bool IsActive = true;

	private Transform baseTr;

	public Transform hips;

	public Transform LeftIkTarget;

	public Transform RightIkTarget;

	public Transform leftFootTarget;

	public Transform RightFootTarget;

	public float heightOffset;

	public float leftFootOffset;

	public float rightFootOffset;

	private float ankleToGroundOffset;

	private RaycastHit hitPoint;

	private RaycastHit[] allHit;

	private RaycastHit hit;

	private void Start()
	{
		this.baseTr = base.transform;
		this.ankleToGroundOffset = 2.2f;
	}

	private void LateUpdate()
	{
		this.CalculateFootOffsets();
	}

	private void CalculateFootOffsets()
	{
		Vector3 position = this.LeftIkTarget.position;
		Vector3 origin = position;
		origin.y += 1.5f;
		float num;
		if (LocalPlayer.FpCharacter.Grounded)
		{
			if (Physics.Raycast(origin, Vector3.down, out this.hitPoint, 4f, this.groundMask))
			{
				num = this.hitPoint.point.y - this.baseTr.position.y;
				this.leftFootTarget.rotation = this.LeftIkTarget.rotation;
				this.leftFootTarget.rotation = Quaternion.LookRotation(Vector3.Cross(this.leftFootTarget.right, this.hitPoint.normal), this.hitPoint.normal);
			}
			else
			{
				num = 0f;
			}
		}
		else
		{
			this.leftFootTarget.rotation = this.LeftIkTarget.rotation;
			num = 0f;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		position.y += num;
		this.leftFootTarget.position = position;
		position = this.RightIkTarget.position;
		origin = position;
		origin.y += 1f;
		if (LocalPlayer.FpCharacter.Grounded)
		{
			if (Physics.Raycast(origin, Vector3.down, out this.hitPoint, 4f, this.groundMask))
			{
				num = this.hitPoint.point.y - this.baseTr.position.y;
				this.RightFootTarget.rotation = this.RightIkTarget.rotation;
				this.RightFootTarget.rotation = Quaternion.LookRotation(Vector3.Cross(this.RightFootTarget.right, -this.hitPoint.normal), -this.hitPoint.normal);
			}
			else
			{
				num = 0f;
			}
		}
		else
		{
			this.RightFootTarget.rotation = this.RightIkTarget.rotation;
			num = 0f;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		position.y += num;
		this.RightFootTarget.position = position;
		if (num < this.leftFootOffset)
		{
			this.leftFootOffset = num;
		}
	}

	private void CalculateFootRotation()
	{
	}

	private RaycastHit raycastContacts(Vector3 pos)
	{
		this.allHit = Physics.RaycastAll(pos, Vector3.down, 4f, this.groundMask);
		float num = float.PositiveInfinity;
		for (int i = 0; i < this.allHit.Length; i++)
		{
			if (!this.allHit[i].collider.isTrigger)
			{
				float distance = this.allHit[i].distance;
				if (distance < num)
				{
					num = distance;
					Collider collider = this.allHit[i].collider;
					this.hit = this.allHit[i];
				}
			}
		}
		return this.hit;
	}
}
