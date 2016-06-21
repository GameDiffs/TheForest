using System;
using UnityEngine;

public class mutantIkLegs : MonoBehaviour
{
	private mutantAnimatorControl animControl;

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

	public Transform leftUpLeg;

	public Transform rightUpLeg;

	public float hipToFootDist;

	private Vector3 hipSmoothPos;

	private float hipSmoothPosY;

	private float leftHipOffset;

	private float rightHipOffset;

	private float ankleToGroundOffset;

	private float ankleHeightBlend;

	private float ankleAngleBlend;

	private float leftFootSmoothY;

	private float rightFootSmoothY;

	private int deathTag = Animator.StringToHash("death");

	private RaycastHit hitPoint;

	public float smoothSpeed = 0.35f;

	private RaycastHit[] allHit;

	private RaycastHit hit;

	private void Awake()
	{
		this.hipToFootDist = Vector3.Distance(this.leftUpLeg.position, this.LeftIkTarget.position);
		this.hipToFootDist = 2.3f;
	}

	private void Start()
	{
		this.animControl = base.transform.GetComponent<mutantAnimatorControl>();
		this.baseTr = base.transform;
		this.ankleToGroundOffset = this.leftFootTarget.position.y - this.baseTr.position.y;
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
		this.ankleHeightBlend = this.LeftIkTarget.position.y - this.baseTr.position.y - 0.25f;
		this.ankleHeightBlend = 1f - Mathf.Clamp(this.ankleHeightBlend, 0f, 1f) * 1.25f;
		this.ankleAngleBlend = Vector3.Angle(this.LeftIkTarget.up, Vector3.up) / 30f - 5f;
		this.ankleAngleBlend = 1f - Mathf.Clamp(this.ankleAngleBlend, 0f, 1f);
		float num;
		if (this.animControl.fullBodyState.tagHash != this.deathTag)
		{
			if (Physics.Raycast(origin, Vector3.down, out this.hitPoint, 3f, this.groundMask))
			{
				num = this.hitPoint.point.y - this.baseTr.position.y;
				Quaternion to = this.LeftIkTarget.rotation;
				to = Quaternion.LookRotation(Vector3.Cross(this.leftFootTarget.right, this.hitPoint.normal), this.hitPoint.normal);
				this.leftFootTarget.rotation = Quaternion.Lerp(this.LeftIkTarget.rotation, to, this.ankleHeightBlend * this.ankleAngleBlend);
			}
			else
			{
				this.leftFootTarget.rotation = this.LeftIkTarget.rotation;
				num = 0f;
			}
		}
		else
		{
			this.leftFootTarget.rotation = this.LeftIkTarget.rotation;
			num = 0f;
		}
		num = Mathf.Clamp(num, -1f, 3f);
		if (num < 0f)
		{
			num = 0f;
		}
		else
		{
			position.y += num;
		}
		this.leftFootTarget.position = position;
		Vector3 position2 = this.hips.position;
		if (num < 0f)
		{
			this.leftHipOffset = Vector3.Distance(this.leftUpLeg.position, this.leftFootTarget.position);
			if (this.leftHipOffset > this.hipToFootDist)
			{
				position2.y += this.hipToFootDist - this.leftHipOffset;
			}
		}
		position = this.RightIkTarget.position;
		origin = position;
		origin.y += 1f;
		this.ankleHeightBlend = this.RightIkTarget.position.y - this.baseTr.position.y - 0.25f;
		this.ankleHeightBlend = 1f - Mathf.Clamp(this.ankleHeightBlend, 0f, 1f);
		this.ankleAngleBlend = Vector3.Angle(this.RightIkTarget.up, Vector3.up) / 30f - 5f;
		this.ankleAngleBlend = 1f - Mathf.Clamp(this.ankleAngleBlend, 0f, 1f);
		if (this.animControl.fullBodyState.tagHash != this.deathTag)
		{
			if (Physics.Raycast(origin, Vector3.down, out this.hitPoint, 3f, this.groundMask))
			{
				num = this.hitPoint.point.y - this.baseTr.position.y;
				Quaternion to2 = this.RightIkTarget.rotation;
				to2 = Quaternion.LookRotation(Vector3.Cross(this.RightFootTarget.right, -this.hitPoint.normal), -this.hitPoint.normal);
				this.RightFootTarget.rotation = Quaternion.Lerp(this.RightIkTarget.rotation, to2, this.ankleHeightBlend * this.ankleAngleBlend);
			}
			else
			{
				num = 0f;
				this.RightFootTarget.rotation = this.RightIkTarget.rotation;
			}
		}
		else
		{
			this.RightFootTarget.rotation = this.RightIkTarget.rotation;
			num = 0f;
		}
		num = Mathf.Clamp(num, -2f, 3f);
		if (num < 0f)
		{
			num = 0f;
		}
		else
		{
			position.y += num;
		}
		this.RightFootTarget.position = position;
		this.rightHipOffset = Vector3.Distance(this.rightUpLeg.position, this.RightFootTarget.position);
		if (num < 0f && this.rightHipOffset > this.leftHipOffset && this.rightHipOffset > this.hipToFootDist)
		{
			position2.y += this.hipToFootDist - this.rightHipOffset;
		}
		float num2 = 0f;
		this.hipSmoothPosY = Mathf.SmoothDamp(this.hipSmoothPosY, position2.y, ref num2, this.smoothSpeed);
		position2.y = this.hipSmoothPosY;
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
