using HutongGames.PlayMaker;
using System;
using UnityEngine;

public class enemyAvoidance : MonoBehaviour
{
	private mutantScriptSetup setup;

	private mutantAnimatorControl animControl;

	private Transform rootTr;

	private Animator animator;

	private bool playerContactCheck;

	private FsmBool treeBool;

	private bool startUp;

	private void Start()
	{
		this.setup = base.transform.root.GetComponentInChildren<mutantScriptSetup>();
		this.animControl = base.transform.root.GetComponentInChildren<mutantAnimatorControl>();
		this.animator = this.animControl.transform.GetComponent<Animator>();
		this.rootTr = base.transform.root;
		this.treeBool = this.setup.pmCombat.FsmVariables.GetFsmBool("inTreeBool");
		base.Invoke("setStartUp", 1f);
	}

	private void setStartUp()
	{
		this.startUp = true;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.startUp)
		{
			return;
		}
		if (this.animControl.fullBodyState.tagHash == this.setup.hashs.deathTag || this.animator.GetBool("trapBool"))
		{
			return;
		}
		if (other.gameObject.CompareTag("enemyBlocker") || other.gameObject.CompareTag("PlayerNet"))
		{
			Vector3 position = other.transform.position;
			position.y = this.rootTr.position.y;
			float num = Vector3.Distance(this.rootTr.position, position);
			if (num < 2.2f)
			{
				this.animControl.blockerCollider = other;
				this.animControl.inBlockerTrigger = true;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!this.startUp)
		{
			return;
		}
		if (this.animControl.fullBodyState.tagHash == this.setup.hashs.deathTag || this.animator.GetBool("trapBool"))
		{
			return;
		}
		if (other.gameObject.CompareTag("enemyBlocker"))
		{
			Vector3 position = other.transform.position;
			position.y = this.rootTr.position.y;
			float num = Vector3.Distance(this.rootTr.position, position);
			if (num < 2.3f)
			{
				this.animControl.blockerCollider = other;
				this.animControl.inBlockerTrigger = true;
			}
		}
		if ((other.gameObject.CompareTag("Tree") || other.gameObject.CompareTag("jumpObject")) && !this.animator.GetBool("jumpBlockBool") && !this.animator.GetBool("jumpBOOL") && !this.treeBool.Value)
		{
			this.animControl.blockerCollider = other;
			this.animControl.inTreeTrigger = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!this.startUp)
		{
			return;
		}
		if (other.gameObject.CompareTag("enemyBlocker"))
		{
			this.animControl.inBlockerTrigger = false;
			this.animControl.blockerCollider = null;
		}
	}
}
