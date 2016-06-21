using System;
using UnityEngine;

public class mutantWaterDetect : MonoBehaviour
{
	public bool creepy;

	private Animator animator;

	private enemyAnimEvents events;

	private mutantScriptSetup setup;

	private creepyAnimatorControl animControl;

	private int fallHash = Animator.StringToHash("jumpFall");

	private bool splashCoolDown;

	private bool waterCheck;

	public bool inWater;

	private float eventInterval;

	private float eventMaxTime;

	private bool doWaterEvent;

	private void Start()
	{
		this.animControl = base.transform.GetComponentInChildren<creepyAnimatorControl>();
		this.setup = base.transform.GetComponentInChildren<mutantScriptSetup>();
		this.animator = base.transform.GetComponentInChildren<Animator>();
		this.events = base.transform.GetComponentInChildren<enemyAnimEvents>();
	}

	private void OnEnable()
	{
		this.splashCoolDown = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Water"))
		{
			if (!this.setup.search.fsmInCave.Value && this.setup.pmCombat)
			{
				this.setup.pmCombat.FsmVariables.GetFsmBool("exitWaterBool").Value = true;
			}
			if (!this.creepy)
			{
				this.waterCheck = true;
				if (this.animator.GetCurrentAnimatorStateInfo(0).tagHash == this.fallHash && !this.splashCoolDown)
				{
					this.events.doWaterSplash(other);
					this.splashCoolDown = true;
					base.Invoke("resetSplashCoolDown", 1f);
				}
			}
		}
	}

	private void resetSplashCoolDown()
	{
		this.splashCoolDown = false;
	}
}
