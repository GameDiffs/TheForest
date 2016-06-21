using System;
using TheForest.Utils;
using UnityEngine;

public class mutantHitReactions : MonoBehaviour
{
	private Transform tr;

	private mutantScriptSetup setup;

	private void Start()
	{
		this.setup = base.transform.GetComponentInChildren<mutantScriptSetup>();
		this.tr = this.setup.transform;
	}

	private void lookAtExplosion(Vector3 pos)
	{
		Vector3 worldPosition = pos;
		worldPosition.y = this.tr.position.y;
		this.tr.LookAt(worldPosition, this.tr.up);
	}

	private void staggerFromHit()
	{
		this.setup.animator.SetTriggerReflected("hitTrigger");
	}

	private void Douse()
	{
	}

	private void setPlayerAttacking()
	{
		float num = 8f;
		if (LocalPlayer.Animator.GetBool("bowHeld") || LocalPlayer.Animator.GetBool("spearHeld") || LocalPlayer.Animator.GetBool("molotovHeld") || LocalPlayer.Animator.GetBool("ballHeld"))
		{
			num = 30f;
		}
		if ((this.setup.ai.maleSkinny || this.setup.ai.femaleSkinny || this.setup.ai.male || this.setup.ai.female) && this.setup.ai.mainPlayerDist < num && this.setup.ai.mainPlayerAngle < 40f && this.setup.ai.mainPlayerAngle > -40f && UnityEngine.Random.value > 0.2f && this.setup.search.playerAware)
		{
			if (UnityEngine.Random.value < 0.5f)
			{
				this.setup.animator.SetIntegerReflected("randInt1", 3);
			}
			else
			{
				this.setup.animator.SetIntegerReflected("randInt1", 4);
			}
			this.setup.animator.SetBoolReflected("dodgePlayer", true);
			base.Invoke("resetPlayerAttacking", 1f);
		}
	}

	private void resetPlayerAttacking()
	{
		this.setup.animator.SetBoolReflected("dodgePlayer", false);
	}
}
