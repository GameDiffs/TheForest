using System;
using UnityEngine;

public class wallTriggerSetup : MonoBehaviour
{
	public Animator animator;

	private bool atWall;

	private float val;

	private float smoothVal;

	public float speedVal = 0.09f;

	private void Start()
	{
		this.animator = base.transform.root.GetComponentInChildren<Animator>();
		this.speedVal = 0.09f;
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.isTrigger && !other.gameObject.CompareTag("enemyBlocker") && !other.gameObject.CompareTag("Player"))
		{
			this.atWall = true;
		}
	}

	private void FixedUpdate()
	{
		if (this.atWall)
		{
			this.val = 1f;
		}
		else
		{
			this.val = 0f;
		}
		float num = 0f;
		this.smoothVal = Mathf.SmoothDamp(this.smoothVal, this.val, ref num, this.speedVal);
		this.animator.SetFloatReflected("weaponClipBlend", this.smoothVal);
		this.atWall = false;
	}
}
