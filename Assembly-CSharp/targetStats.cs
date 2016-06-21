using System;
using UnityEngine;

public class targetStats : MonoBehaviour
{
	public bool setPlayerType;

	private Animator animator;

	private AnimatorStateInfo state;

	private int deathTag = Animator.StringToHash("death");

	public bool targetDown;

	public bool inNooseTrap;

	private void Start()
	{
		if (this.setPlayerType)
		{
			this.animator = base.transform.GetComponentInChildren<Animator>();
		}
		this.targetDown = false;
		this.inNooseTrap = false;
	}

	private void Update()
	{
		if (this.setPlayerType)
		{
			this.state = this.animator.GetCurrentAnimatorStateInfo(2);
			if (this.state.tagHash == this.deathTag)
			{
				this.targetDown = true;
			}
			else
			{
				this.targetDown = false;
			}
		}
	}

	public void setTargetDown()
	{
		this.targetDown = true;
	}

	public void setTargetUp()
	{
		this.targetDown = false;
	}
}
