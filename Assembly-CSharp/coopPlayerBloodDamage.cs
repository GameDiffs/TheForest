using System;
using UnityEngine;

public class coopPlayerBloodDamage : MonoBehaviour
{
	private MaterialPropertyBlock bloodPropertyBlock;

	public Renderer MyBody;

	private Animator animator;

	private void Start()
	{
		this.bloodPropertyBlock = new MaterialPropertyBlock();
		this.animator = base.transform.GetComponentInChildren<Animator>();
		this.resetSkinDamage();
		base.InvokeRepeating("setSkinDamage", 1f, 0.3f);
	}

	public void setSkinDamage()
	{
		this.MyBody.GetPropertyBlock(this.bloodPropertyBlock);
		float num = this.bloodPropertyBlock.GetFloat("_Damage1");
		if (num < 1f)
		{
			num += 0.2f;
			this.bloodPropertyBlock.SetFloat("_Damage1", this.animator.GetFloat("skinDamage1"));
			this.bloodPropertyBlock.SetFloat("_Damage2", this.animator.GetFloat("skinDamage2"));
			this.bloodPropertyBlock.SetFloat("_Damage3", this.animator.GetFloat("skinDamage3"));
			this.bloodPropertyBlock.SetFloat("_Damage4", this.animator.GetFloat("skinDamage4"));
			this.MyBody.SetPropertyBlock(this.bloodPropertyBlock);
		}
	}

	public void resetSkinDamage()
	{
		this.MyBody.GetPropertyBlock(this.bloodPropertyBlock);
		this.bloodPropertyBlock.SetFloat("_Damage1", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage2", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage3", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage4", 0f);
		this.MyBody.SetPropertyBlock(this.bloodPropertyBlock);
	}
}
