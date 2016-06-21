using System;
using UnityEngine;

public class CoopMutantClientHitPrediction : MonoBehaviour
{
	private CoopMutantTransformDelayer trDelayer;

	private coopPlayerBloodDamage playerDamage;

	private float segmentTime = 0.55f;

	private float endTime;

	private float blendAmount;

	public int HitLayer = 1;

	public Animator Animator;

	private Transform rotateTr;

	private AnimatorStateInfo currState0;

	private AnimatorStateInfo currState1;

	private int deathTag = Animator.StringToHash("death");

	private int hitStaggerTag = Animator.StringToHash("hitStagger");

	private bool doingPrediction;

	private bool restoreRotationCheck;

	private void Start()
	{
		this.playerDamage = base.transform.GetComponent<coopPlayerBloodDamage>();
		this.trDelayer = base.transform.GetComponent<CoopMutantTransformDelayer>();
		this.Animator.SetLayerWeight(this.HitLayer, 0f);
		this.rotateTr = this.Animator.transform;
	}

	private void Update()
	{
		if (!this.trDelayer)
		{
			return;
		}
		this.currState0 = this.Animator.GetCurrentAnimatorStateInfo(0);
		this.currState1 = this.Animator.GetCurrentAnimatorStateInfo(1);
		if (this.currState0.IsTag("inTrap") || this.currState0.IsTag("runTrap"))
		{
			this.Animator.SetLayerWeight(this.HitLayer, 0f);
			this.doingPrediction = false;
			if (!this.restoreRotationCheck)
			{
				this.restoreRotationCheck = true;
			}
			return;
		}
		if (this.currState0.tagHash == this.deathTag)
		{
			this.Animator.SetLayerWeight(this.HitLayer, this.Animator.GetLayerWeight(this.HitLayer) * (1f - Time.deltaTime * 8f));
			this.doingPrediction = false;
			if (!this.restoreRotationCheck)
			{
				this.restoreRotationCheck = true;
				return;
			}
		}
		else if (this.endTime > Time.time)
		{
			this.doingPrediction = true;
			this.Animator.SetLayerWeight(this.HitLayer, 1f);
			this.restoreRotationCheck = false;
		}
		else if ((double)this.Animator.GetLayerWeight(this.HitLayer) > 0.03)
		{
			this.blendAmount = this.Animator.GetLayerWeight(this.HitLayer) * (1f - Time.deltaTime * 4f);
			this.Animator.SetLayerWeight(this.HitLayer, this.blendAmount);
			if (!this.restoreRotationCheck)
			{
				this.restoreRotationCheck = true;
				this.doingPrediction = false;
			}
		}
		else
		{
			this.Animator.SetLayerWeight(this.HitLayer, 0f);
			this.blendAmount = 0f;
			this.doingPrediction = false;
			this.restoreRotationCheck = false;
		}
		if (this.doingPrediction)
		{
		}
	}

	public void getCombo(int getCombo)
	{
		this.Animator.SetInteger("ClientHitCombo", getCombo);
	}

	public void getClientHitDirection(int getDir)
	{
		this.Animator.SetInteger("ClientHitDirection", getDir);
	}

	public void StartPrediction()
	{
		if (this.currState1.tagHash == this.deathTag)
		{
			return;
		}
		if (this.playerDamage)
		{
			this.playerDamage.setSkinDamage();
		}
		this.endTime = Time.time + this.segmentTime;
		this.Animator.SetTrigger("ClientPredictionTrigger");
	}
}
