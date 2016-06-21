using System;
using UnityEngine;

public class setupMutantNetBlood : MonoBehaviour
{
	private Animator animator;

	private mutantAI_net ai;

	private mutantNetHealthSync netHealth;

	public SkinnedMeshRenderer MySkin;

	private AnimatorStateInfo currState0;

	private AnimatorStateInfo nextState0;

	private bool damageCoolDown;

	private int damageHash = Animator.StringToHash("damaged");

	private int deathHash = Animator.StringToHash("death");

	private MaterialPropertyBlock bloodPropertyBlock;

	private void Awake()
	{
		this.bloodPropertyBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		this.resetSkinDamage();
		this.animator = base.transform.root.GetComponentInChildren<Animator>();
		this.ai = base.transform.root.GetComponentInChildren<mutantAI_net>();
		this.netHealth = base.transform.root.GetComponentInChildren<mutantNetHealthSync>();
	}

	private void OnEnable()
	{
		this.damageCoolDown = false;
		base.Invoke("resetSkinDamage", 0.5f);
	}

	private void getSkinHitPosition(Transform hitTr)
	{
		this.setSkinDamage(2);
	}

	private void Update()
	{
		if (!this.animator)
		{
			return;
		}
		if (!this.animator.enabled)
		{
			return;
		}
		this.currState0 = this.animator.GetCurrentAnimatorStateInfo(0);
		this.nextState0 = this.animator.GetNextAnimatorStateInfo(0);
		if (this.nextState0.tagHash == this.damageHash && this.currState0.tagHash != this.damageHash && !this.damageCoolDown)
		{
			this.setSkinDamage(UnityEngine.Random.Range(0, 3));
			this.damageCoolDown = true;
			base.Invoke("resetCoolDown", 0.5f);
		}
		if (this.currState0.tagHash == this.deathHash)
		{
			this.setSkinDamage(1);
		}
	}

	public void setSkinDamage(int d)
	{
		if (!this.animator)
		{
			return;
		}
		if (!this.animator.enabled)
		{
			return;
		}
		if (!this.damageCoolDown)
		{
			this.MySkin.GetPropertyBlock(this.bloodPropertyBlock);
			float @float = this.animator.GetFloat("skinDamage1");
			this.bloodPropertyBlock.SetFloat("_Damage1", @float);
			@float = this.animator.GetFloat("skinDamage3");
			this.bloodPropertyBlock.SetFloat("_Damage3", @float);
			@float = this.animator.GetFloat("skinDamage2");
			this.bloodPropertyBlock.SetFloat("_Damage2", @float);
			@float = this.animator.GetFloat("skinDamage4");
			if (this.ai.creepy || this.ai.creepy_fat || this.ai.creepy_male || this.ai.creepy_baby)
			{
				this.bloodPropertyBlock.SetFloat("_Damage4", @float);
			}
			else
			{
				this.bloodPropertyBlock.SetFloat("_Damage4", @float);
			}
			this.MySkin.SetPropertyBlock(this.bloodPropertyBlock);
			this.damageCoolDown = true;
			base.Invoke("resetCoolDown", 0.5f);
		}
	}

	private void resetSkinDamage()
	{
		this.MySkin.GetPropertyBlock(this.bloodPropertyBlock);
		this.bloodPropertyBlock.SetFloat("_Damage1", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage2", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage3", 0f);
		this.bloodPropertyBlock.SetFloat("_Damage4", 0f);
		this.MySkin.SetPropertyBlock(this.bloodPropertyBlock);
	}

	private void resetCoolDown()
	{
		this.damageCoolDown = false;
	}
}
