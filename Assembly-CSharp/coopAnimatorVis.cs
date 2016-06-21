using System;
using TheForest.Utils;
using UnityEngine;

public class coopAnimatorVis : MonoBehaviour
{
	private Animator avatar;

	private Transform thisTr;

	private Transform animatorTr;

	private Transform playerTr;

	public bool checkForTree;

	public bool checkForTree2;

	private bool onTree;

	public float playerDist;

	public float animalAngle;

	private Vector3 playerTarget;

	private Vector3 animalTarget;

	public SkinnedMeshRenderer skinRenderer;

	private void Start()
	{
		this.avatar = base.GetComponent<Animator>();
		this.animatorTr = this.avatar.transform;
		this.thisTr = base.transform;
	}

	private void Update()
	{
		if (!this.avatar)
		{
			return;
		}
		this.playerTr = LocalPlayer.Transform;
		this.playerDist = Vector3.Distance(this.thisTr.position, this.playerTr.position);
		this.playerTarget = this.animatorTr.InverseTransformPoint(this.playerTr.position);
		this.animalTarget = LocalPlayer.Transform.InverseTransformPoint(this.animatorTr.position);
		this.animalAngle = Mathf.Atan2(this.animalTarget.x, this.animalTarget.z) * 57.29578f;
		if (!this.skinRenderer.isVisible && (this.animalAngle < -90f || this.animalAngle > 90f) && this.playerDist > 10f && !this.onTree)
		{
			if (this.avatar.enabled)
			{
				this.avatar.enabled = false;
			}
		}
		else if (!this.avatar.enabled)
		{
			this.avatar.enabled = true;
		}
	}
}
