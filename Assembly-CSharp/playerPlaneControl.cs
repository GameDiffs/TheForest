using System;
using TheForest.Utils;
using UnityEngine;

public class playerPlaneControl : MonoBehaviour
{
	private Animator planeAnimator;

	public Animator timmyAnimator;

	private Rigidbody controller;

	private Transform playerCam;

	public Transform neckJnt;

	public GameObject book;

	public float torsoFollowSpeed;

	private Transform tr;

	private float normCamX;

	private float normCamY;

	private float smoothCamX;

	private float smoothCamY;

	private float mouseCurrentPosx;

	private float mouseDeltax;

	private void Awake()
	{
		if (LevelSerializer.IsDeserializing || CoopPeerStarter.DedicatedHost)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		this.playerCam = LocalPlayer.MainCamTr;
		this.planeAnimator = base.GetComponent<Animator>();
		this.tr = base.transform;
		this.smoothCamX = 0f;
	}

	private void Update()
	{
		if (!this.playerCam)
		{
			this.playerCam = LocalPlayer.MainCamTr;
		}
		else
		{
			float x = this.playerCam.eulerAngles.x;
			Vector3 forward = this.tr.forward;
			forward.y = 0f;
			Vector3 forward2 = this.playerCam.forward;
			forward2.y = 0f;
			float num = Vector3.Angle(forward, forward2);
			Vector3 lhs = Vector3.Cross(forward, forward2);
			float num2 = Vector3.Dot(lhs, this.tr.up);
			if (num2 < 0f)
			{
				num *= -1f;
			}
			if (x > 180f)
			{
				this.normCamX = x - 360f;
			}
			else
			{
				this.normCamX = x;
			}
			this.normCamX /= 90f;
			this.normCamX = (Mathf.Clamp(this.normCamX, -1f, 1f) - 0.1f) * 10f;
			this.normCamY = num / 90f;
			this.normCamY = Mathf.Clamp(this.normCamY, -1f, 1f) * 10f;
			this.smoothCamY = this.normCamY;
			if (this.planeAnimator)
			{
				this.planeAnimator.SetFloatReflected("normCamX", this.smoothCamX);
				this.planeAnimator.SetFloatReflected("normCamY", this.smoothCamY);
			}
			if (this.timmyAnimator)
			{
				this.timmyAnimator.SetFloatReflected("normCamX", this.smoothCamX);
				this.timmyAnimator.SetFloatReflected("normCamY", this.smoothCamY);
			}
		}
	}

	private void LateUpdate()
	{
		Quaternion quaternion = this.neckJnt.rotation;
		Vector3 vector = this.tr.InverseTransformDirection(LocalPlayer.MainCamTr.forward);
		quaternion = Quaternion.AngleAxis(this.normCamX * 10f, base.transform.right) * quaternion;
		quaternion = Quaternion.AngleAxis(this.normCamY * -10f, base.transform.forward) * quaternion;
		this.neckJnt.rotation = quaternion;
	}
}
