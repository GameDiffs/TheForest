using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class mutantFollowerFunctions : MonoBehaviour
{
	public List<GameObject> followersList = new List<GameObject>();

	private mutantScriptSetup setup;

	private mutantWorldSearchFunctions worldFunctions;

	private Animator animator;

	private FsmBool fsmDeathBool;

	private void Start()
	{
		this.animator = base.GetComponentInChildren<Animator>();
		this.setup = base.GetComponentInChildren<mutantScriptSetup>();
		this.worldFunctions = base.GetComponentInChildren<mutantWorldSearchFunctions>();
		this.fsmDeathBool = this.setup.pmCombat.FsmVariables.GetFsmBool("deathBool");
	}

	private void sendFollowerEvent(string e)
	{
		for (int i = 0; i < this.followersList.Count; i++)
		{
			if (this.followersList[i])
			{
				this.followersList[i].SendMessage(e, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void sendToLeaderEvent()
	{
		this.sendFollowerEvent("switchToGoToLeader");
	}

	private void sendArtEvent()
	{
		this.sendFollowerEvent("switchToArt");
	}

	private void sendSearchEvent()
	{
		this.sendFollowerEvent("switchToSearch");
	}

	private void sendCloseSearchEvent()
	{
		this.sendFollowerEvent("switchToCloseSearch");
	}

	private void sendStalkEvent()
	{
		this.sendFollowerEvent("switchToStalk");
	}

	private void sendEncounterEvent()
	{
		for (int i = 0; i < this.followersList.Count; i++)
		{
			GameObject encounterGo = this.worldFunctions.encounterGo;
			if (this.followersList[i])
			{
				this.followersList[i].SendMessage("switchToEncounter", encounterGo, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void sendResetEvent()
	{
		this.sendFollowerEvent("resetEncounter");
	}

	private void sendAttackEvent()
	{
		this.sendFollowerEvent("switchToAttack");
	}

	private void sendTreeStalkEvent()
	{
		this.sendFollowerEvent("switchToTreeStalk");
	}

	private void sendTreeDownEvent()
	{
		this.sendFollowerEvent("switchToTreeDown");
	}

	public void sendAlertEvent()
	{
		this.sendFollowerEvent("switchToAlerted");
	}

	public void sendSleepEvent()
	{
		this.sendFollowerEvent("switchToSleep");
	}

	private void switchToGoToLeader()
	{
		this.setup.pmCombat.FsmVariables.GetFsmBool("doGoToLeader").Value = true;
		base.Invoke("resetGoToLeader", 10f);
	}

	private void resetGoToLeader()
	{
		this.setup.pmCombat.FsmVariables.GetFsmBool("doGoToLeader").Value = false;
	}

	private void switchToArt()
	{
		if (this.setup.pmSearch.enabled)
		{
			this.setup.pmSearch.SendEvent("toPlaceArt");
		}
	}

	private void switchToSearch()
	{
		if (!this.animator.enabled)
		{
			this.setup.pmSearch.enabled = true;
			this.setup.pmSearch.SendEvent("toActivateFSM");
			return;
		}
		if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			this.setup.pmSearch.enabled = true;
			this.setup.pmSearch.SendEvent("toActivateFSM");
			this.animator.SetBoolReflected("treeBOOL", false);
			this.animator.SetBoolReflected("onRockBOOL", false);
			this.animator.SetIntegerReflected("randInt1", 0);
			this.animator.SetBoolReflected("screamBOOL", false);
		}
	}

	private void switchToCloseSearch()
	{
		if (!this.animator.enabled)
		{
			if (!this.setup.pmCombat.enabled && !this.setup.pmSleep.enabled)
			{
				this.setup.pmSearch.enabled = true;
				this.setup.pmSearch.SendEvent("toActivateCloseSearch");
			}
			return;
		}
		if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire && !this.setup.pmCombat.enabled && !this.setup.pmSleep.enabled)
		{
			this.setup.pmSearch.enabled = true;
			this.setup.pmSearch.SendEvent("toActivateCloseSearch");
		}
	}

	private void switchToStalk()
	{
		if (!this.animator.enabled)
		{
			this.setup.pmCombat.enabled = true;
			this.setup.pmBrain.SendEvent("toSetPassive");
			this.setup.ai.forceTreeDown();
			return;
		}
		if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			this.setup.pmCombat.enabled = true;
			this.setup.pmBrain.SendEvent("toSetPassive");
			this.setup.ai.forceTreeDown();
			this.animator.SetBoolReflected("onRockBOOL", false);
			this.animator.SetBoolReflected("screamBOOL", false);
		}
	}

	private void switchToEncounter(GameObject encounter)
	{
		if (!this.setup.pmCombat.enabled)
		{
			this.worldFunctions.getEncounterComponent(encounter);
			this.worldFunctions.setEncounterType();
			this.worldFunctions.encounterGo = encounter;
			this.setup.pmEncounter.FsmVariables.GetFsmGameObject("encounterGo").Value = encounter;
			this.setup.ai.forceTreeDown();
		}
	}

	private void switchToAttack()
	{
		if (!this.animator.enabled)
		{
			this.setup.pmCombat.enabled = true;
			this.setup.pmBrain.SendEvent("toSetAggressive");
			this.setup.ai.forceTreeDown();
			return;
		}
		if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			this.setup.pmCombat.enabled = true;
			this.setup.pmBrain.SendEvent("toSetAggressive");
			this.setup.ai.forceTreeDown();
		}
	}

	private void switchToTreeStalk()
	{
		if (!this.animator.enabled)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("stalking").Value = true;
			this.setup.pmCombat.SendEvent("goToTree");
			return;
		}
		if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("stalking").Value = true;
			this.setup.pmCombat.SendEvent("goToTree");
		}
	}

	private void switchToTreeDown()
	{
		this.setup.ai.forceTreeDown();
	}

	private void switchToAlerted()
	{
		if (!LocalPlayer.PlayerBase)
		{
			return;
		}
		if (!this.animator.enabled)
		{
			return;
		}
		if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			this.setup.lastSighting.transform.position = LocalPlayer.PlayerBase.transform.position;
			if (!this.setup.pmCombat.enabled)
			{
				if (this.setup.pmSearch)
				{
					this.setup.pmSearch.FsmVariables.GetFsmBool("tempRun").Value = true;
					this.setup.pmSearch.SendEvent("toPlayerNoise");
				}
				if (this.setup.pmSleep)
				{
					this.setup.pmSleep.SendEvent("toNoise");
				}
				if (this.setup.pmEncounter)
				{
					this.setup.pmEncounter.SendEvent("toNoise");
				}
			}
		}
	}

	private void switchToSleep()
	{
		if (!this.animator.GetBool("trapBool") && this.setup.pmSleep)
		{
			this.setup.ai.forceTreeDown();
			this.setup.pmBrain.FsmVariables.GetFsmBool("fearOverrideBool").Value = true;
			if (this.setup.pmSearch)
			{
				this.setup.pmSearch.SendEvent("toDisableFSM");
			}
			if (this.setup.pmCombat)
			{
				this.setup.pmCombat.SendEvent("toDisableFSM");
			}
			this.setup.pmBrain.SendEvent("toSetSleep");
		}
	}

	private void resetEncounter()
	{
		this.setup.pmEncounter.SendEvent("toResetEncounter");
	}
}
