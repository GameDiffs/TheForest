using System;
using UnityEngine;

public class mutantStaminaManager : MonoBehaviour
{
	private mutantScriptSetup setup;

	private EnemyHealth mutantHealth;

	private void Start()
	{
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.mutantHealth = base.GetComponent<EnemyHealth>();
	}

	public void checkStaminaState()
	{
		if (this.mutantHealth.Health > 70)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathRecoverBool").Value = true;
		}
		else
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathRecoverBool").Value = false;
		}
		if (this.mutantHealth.Health >= 1)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("deathFinal").Value = false;
		}
	}
}
