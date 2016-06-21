using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantFamilyFunctions : MonoBehaviour
{
	public mutantScriptSetup setup;

	public mutantFamilyFunctions targetFamilyFunctions;

	private mutantTypeSetup spawnSetup;

	private mutantAI ai;

	private mutantSearchFunctions search;

	private Animator animator;

	private mutantController mutantControl;

	private EnemyHealth health;

	public GameObject currentMemberTarget;

	public GameObject currentHelper;

	public GameObject dragPoint;

	public Vector3 dragPointInit;

	public bool occupied;

	public bool busy;

	public bool dying;

	private bool timeout1;

	private bool freakOverride;

	private FsmBool fsmDeathBool;

	private void Start()
	{
		this.setup = base.GetComponentInChildren<mutantScriptSetup>();
		this.spawnSetup = base.transform.root.GetComponent<mutantTypeSetup>();
		this.ai = base.GetComponentInChildren<mutantAI>();
		this.search = base.GetComponentInChildren<mutantSearchFunctions>();
		this.animator = base.GetComponentInChildren<Animator>();
		if (this.dragPoint)
		{
			this.dragPointInit = this.dragPoint.transform.localPosition;
		}
		this.mutantControl = Scene.MutantControler;
		this.health = base.GetComponentInChildren<EnemyHealth>();
		this.fsmDeathBool = this.setup.pmCombat.FsmVariables.GetFsmBool("deathBool");
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	private void startEatMeEvent()
	{
		if (base.gameObject.activeSelf)
		{
			base.StartCoroutine("sendEatMeEvent");
		}
	}

	private void startRescueEvent()
	{
		if (base.gameObject.activeSelf)
		{
			base.StartCoroutine("sendRescueEvent");
		}
	}

	public void cancelRescueEvent()
	{
		base.CancelInvoke("startRescueEvent");
		base.StopCoroutine("sendRescueEvent");
		this.dying = false;
		if (this.spawnSetup.spawner)
		{
			foreach (GameObject current in this.spawnSetup.spawner.allMembers)
			{
				if (current)
				{
					current.SendMessage("resetRescueEvent", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void cancelEatMeEvent()
	{
		base.CancelInvoke("startEatMeEvent");
		base.StopCoroutine("sendEatMeEvent");
		foreach (GameObject current in this.mutantControl.activeSkinnyCannibals)
		{
			if (current)
			{
				current.SendMessage("cancelEatMe", base.gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void resetRescueEvent()
	{
		this.setup.pmCombat.FsmVariables.GetFsmBool("rescueBool").Value = false;
	}

	[DebuggerHidden]
	public IEnumerator sendAmbushEvent()
	{
		mutantFamilyFunctions.<sendAmbushEvent>c__Iterator7B <sendAmbushEvent>c__Iterator7B = new mutantFamilyFunctions.<sendAmbushEvent>c__Iterator7B();
		<sendAmbushEvent>c__Iterator7B.<>f__this = this;
		return <sendAmbushEvent>c__Iterator7B;
	}

	[DebuggerHidden]
	public IEnumerator sendRescueEvent()
	{
		mutantFamilyFunctions.<sendRescueEvent>c__Iterator7C <sendRescueEvent>c__Iterator7C = new mutantFamilyFunctions.<sendRescueEvent>c__Iterator7C();
		<sendRescueEvent>c__Iterator7C.<>f__this = this;
		return <sendRescueEvent>c__Iterator7C;
	}

	[DebuggerHidden]
	public IEnumerator sendEatMeEvent()
	{
		mutantFamilyFunctions.<sendEatMeEvent>c__Iterator7D <sendEatMeEvent>c__Iterator7D = new mutantFamilyFunctions.<sendEatMeEvent>c__Iterator7D();
		<sendEatMeEvent>c__Iterator7D.<>f__this = this;
		return <sendEatMeEvent>c__Iterator7D;
	}

	public void sendTargetSpotted()
	{
		if (!this.spawnSetup)
		{
			return;
		}
		if (!this.ai.maleSkinny && !this.ai.femaleSkinny && this.spawnSetup.spawner)
		{
			foreach (GameObject current in this.spawnSetup.spawner.allMembers)
			{
				if (current && current.name != base.gameObject.name)
				{
					current.SendMessage("switchToTargetSpotted", this.ai.target, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void sendAggressive()
	{
		if (!this.spawnSetup)
		{
			return;
		}
		if (!this.ai.maleSkinny && !this.ai.femaleSkinny && this.spawnSetup.spawner)
		{
			foreach (GameObject current in this.spawnSetup.spawner.allMembers)
			{
				if (current)
				{
					current.SendMessage("switchToCombat", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void sendAggressiveCombat(float timeout)
	{
		if (!this.spawnSetup)
		{
			return;
		}
		if (!this.ai.maleSkinny && !this.ai.femaleSkinny && this.spawnSetup.spawner)
		{
			foreach (GameObject current in this.spawnSetup.spawner.allMembers)
			{
				if (current)
				{
					current.SendMessage("switchToAggressiveCombat", timeout, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void sendFleeArea()
	{
		if (!this.spawnSetup)
		{
			return;
		}
		if (this.setup.search.fsmInCave.Value)
		{
			return;
		}
		if (this.spawnSetup.spawner)
		{
			foreach (GameObject current in this.spawnSetup.spawner.allMembers)
			{
				if (current && current.name != base.gameObject.name)
				{
					current.SendMessage("switchToFleeArea", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void sendAllFleeArea()
	{
		if (!this.spawnSetup)
		{
			return;
		}
		if (this.setup.search.fsmInCave.Value)
		{
			return;
		}
		if (UnityEngine.Random.value > 0.4f)
		{
			return;
		}
		foreach (GameObject current in Scene.MutantControler.activeWorldCannibals)
		{
			if (current && current.name != base.gameObject.name)
			{
				current.SendMessage("switchToFleeArea", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void sendWakeUp()
	{
		if (!this.spawnSetup)
		{
			return;
		}
		if (this.spawnSetup.spawner)
		{
			foreach (GameObject current in this.spawnSetup.spawner.allMembers)
			{
				if (current)
				{
					current.SendMessage("switchToWakeUp", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	private void switchToAmbush()
	{
		if (!this.animator.enabled)
		{
			return;
		}
		if (!this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.animator.GetBool("trapBool") && !this.setup.health.onFire && !this.setup.pmCombat.FsmVariables.GetFsmBool("doingAmbush").Value)
		{
			this.setup.pmCombat.SendEvent("goToAmbush");
		}
	}

	[DebuggerHidden]
	private IEnumerator switchToRescueFriend(GameObject go)
	{
		mutantFamilyFunctions.<switchToRescueFriend>c__Iterator7E <switchToRescueFriend>c__Iterator7E = new mutantFamilyFunctions.<switchToRescueFriend>c__Iterator7E();
		<switchToRescueFriend>c__Iterator7E.go = go;
		<switchToRescueFriend>c__Iterator7E.<$>go = go;
		<switchToRescueFriend>c__Iterator7E.<>f__this = this;
		return <switchToRescueFriend>c__Iterator7E;
	}

	private void switchToGuardFriend(GameObject go)
	{
		if (!this.animator.enabled)
		{
			return;
		}
		if (!this.ai.maleSkinny && !this.ai.femaleSkinny && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			if (!this.busy)
			{
				this.targetFamilyFunctions = go.GetComponent<mutantFamilyFunctions>();
			}
			if (!this.targetFamilyFunctions)
			{
				return;
			}
			this.currentMemberTarget = go;
			this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentMemberGo").Value = go.transform.GetChild(0).gameObject;
			if (this.targetFamilyFunctions.dying)
			{
				this.setup.pmCombat.SendEvent("toGuard1");
			}
		}
	}

	private void switchToFreakout(GameObject go)
	{
		if (!this.animator.enabled)
		{
			return;
		}
		if (!this.busy && this.setup.ai.female && !this.freakOverride && !this.ai.maleSkinny && !this.ai.femaleSkinny && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			this.targetFamilyFunctions = go.GetComponent<mutantFamilyFunctions>();
			if (!this.targetFamilyFunctions)
			{
				return;
			}
			this.currentMemberTarget = go;
			this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentMemberGo").Value = go.transform.GetChild(0).gameObject;
			this.setup.pmCombat.FsmVariables.GetFsmBool("freakoutBool").Value = true;
			this.freakOverride = true;
			base.Invoke("resetFreakOverride", 30f);
		}
	}

	private void switchToEatMe(GameObject go)
	{
		if (!this.busy && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			if (this.animator.enabled && this.animator.GetBool("deathBOOL"))
			{
				return;
			}
			if (this.setup.ai.femaleSkinny || this.setup.ai.maleSkinny)
			{
				this.currentMemberTarget = go;
				this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentMemberGo").Value = go;
				this.setup.pmSearch.FsmVariables.GetFsmGameObject("currentMemberGo").Value = go;
				this.setup.pmBrain.FsmVariables.GetFsmBool("eatBodyBool").Value = true;
				this.setup.pmCombat.FsmVariables.GetFsmBool("eatBodyBool").Value = true;
				this.setup.pmSearch.FsmVariables.GetFsmBool("eatBodyBool").Value = true;
			}
		}
	}

	private void cancelEatMe(GameObject go)
	{
		if ((this.setup.ai.femaleSkinny || this.setup.ai.maleSkinny) && this.currentMemberTarget == go)
		{
			this.setup.pmBrain.FsmVariables.GetFsmBool("eatBodyBool").Value = false;
			this.setup.pmCombat.FsmVariables.GetFsmBool("eatBodyBool").Value = false;
			this.setup.pmSearch.FsmVariables.GetFsmBool("eatBodyBool").Value = false;
			this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentMemberGo").Value = null;
			this.setup.pmSearch.FsmVariables.GetFsmGameObject("currentMemberGo").Value = null;
			this.currentMemberTarget = null;
		}
	}

	private void switchToTargetSpotted(Transform target)
	{
		if (!this.animator.enabled)
		{
			return;
		}
		if (this.setup.pmSearch.enabled && !this.animator.GetBool("trapBool"))
		{
			this.setup.pmSearch.SendEvent("toTargetSpotted");
			this.search.updateCurrentWaypoint(target.position);
		}
	}

	private void switchToCombat()
	{
		if (!this.animator.enabled)
		{
			return;
		}
		if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			this.setup.pmBrain.SendEvent("toSetAggressive");
			if (!this.timeout1)
			{
				this.setup.pmBrain.FsmVariables.GetFsmBool("fearOverrideBool").Value = true;
				this.timeout1 = true;
				base.Invoke("resetFearOverride", 3f);
			}
		}
	}

	private void switchToAggressiveCombat(float timeout)
	{
		this.setup.aiManager.setAggressiveCombat();
		this.setup.aiManager.Invoke("setDefaultCombat", timeout);
	}

	private void switchToFleeArea()
	{
		if (!this.animator.enabled)
		{
			return;
		}
		if (this.setup.search.fsmInCave.Value)
		{
			return;
		}
		if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			this.setup.ai.forceTreeDown();
			this.setup.pmCombat.FsmVariables.GetFsmBool("skipActionBool").Value = true;
			this.setup.pmCombat.SendEvent("toTimeout");
			this.setup.pmSearch.SendEvent("toLeaveArea");
		}
	}

	private void switchToRunAway(GameObject target)
	{
		this.setup.pmBrain.SendEvent("toSetRunAway");
		this.setup.pmBrain.FsmVariables.GetFsmGameObject("fearTargetGo").Value = target;
	}

	private void switchToWakeUp()
	{
		if (!this.animator.enabled)
		{
			return;
		}
		if (!this.animator.GetBool("trapBool"))
		{
			this.setup.pmSleep.SendEvent("toWakeUp");
		}
	}

	private void resetFreakOverride()
	{
		this.freakOverride = false;
	}

	private void resetEncounter()
	{
		this.setup.pmEncounter.SendEvent("toResetEncounter");
	}

	private void setOccupied()
	{
		if (this.targetFamilyFunctions)
		{
			this.targetFamilyFunctions.occupied = true;
		}
		this.busy = true;
	}

	private void setThisOccupied()
	{
		this.occupied = true;
		this.busy = true;
	}

	private void checkTargetOccupied()
	{
		if (!this.targetFamilyFunctions.occupied)
		{
			this.setup.pmCombat.SendEvent("cancelEvent");
		}
	}

	private void checkIfOccupied()
	{
		if (this.targetFamilyFunctions.occupied)
		{
			this.setup.pmCombat.SendEvent("cancelEvent");
		}
	}

	public void resetFamilyParams()
	{
		this.busy = false;
		this.occupied = false;
		this.dying = false;
		this.currentMemberTarget = null;
		this.dragPoint.transform.localPosition = this.dragPointInit;
		this.animator.SetBoolReflected("rescueBool1", false);
	}

	public void sendCancelEvent()
	{
		if (this.targetFamilyFunctions)
		{
			this.targetFamilyFunctions.setup.pmCombat.SendEvent("cancelEvent");
		}
	}

	private void resetFearOverride()
	{
		this.setup.pmBrain.FsmVariables.GetFsmBool("fearOverrideBool").Value = false;
		this.timeout1 = false;
	}

	private void setupDragParams()
	{
		this.setup.pmCombat.FsmVariables.GetFsmGameObject("matchPosGo").Value = this.targetFamilyFunctions.dragPoint;
		this.targetFamilyFunctions.currentHelper = base.gameObject;
		this.dragPoint.transform.localPosition = new Vector3(0f, 0f, 1f);
		this.targetFamilyFunctions.setup.pmCombat.FsmVariables.GetFsmGameObject("matchPosGo").Value = this.dragPoint;
		this.targetFamilyFunctions.setup.pmCombat.FsmVariables.GetFsmGameObject("lookatGo").Value = this.setup.thisGo;
		this.setup.animator.speed = 1f;
		this.targetFamilyFunctions.setup.animator.speed = 1f;
	}

	private void explodeMutant()
	{
		this.health.Explosion(5f);
	}

	private void resetParent()
	{
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
		}
	}
}
