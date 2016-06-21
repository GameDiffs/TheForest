using System;
using UnityEngine;

public class animalTakeDamage : MonoBehaviour
{
	private Animator animator;

	private PlayMakerFSM playMaker;

	private animalHealth health;

	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
		PlayMakerFSM[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			if (playMakerFSM.FsmName == "aiBaseFSM")
			{
				this.playMaker = playMakerFSM;
			}
		}
		this.health = base.gameObject.GetComponent<animalHealth>();
	}

	private void getAttackDirection(int hitDir)
	{
		this.animator.SetIntegerReflected("hitDirection", hitDir);
	}

	private void getCombo(int combo)
	{
		this.animator.SetIntegerReflected("hitCombo", combo);
	}

	private void takeDamage(int direction)
	{
		if (this.health.Health > 1)
		{
			if (direction == 1)
			{
				this.animator.SetBoolReflected("damageBehindBOOL", true);
				this.playMaker.SendEvent("gotHit");
			}
			else if (direction == 0)
			{
				this.animator.SetBoolReflected("damageBOOL", true);
				this.playMaker.SendEvent("gotHit");
			}
		}
		else
		{
			this.playMaker.SendEvent("toDeath");
		}
	}
}
