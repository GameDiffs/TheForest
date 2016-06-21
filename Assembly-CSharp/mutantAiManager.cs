using HutongGames.PlayMaker;
using System;
using TheForest.Utils;
using UnityEngine;

public class mutantAiManager : MonoBehaviour
{
	private mutantAI ai;

	private mutantScriptSetup setup;

	private mutantSearchFunctions searchFunctions;

	private Animator animator;

	private int currMaxAttackers;

	public bool flee;

	private bool timerBlock;

	public bool checkCloseSearch;

	public bool inTopHalfMap;

	private Vector3 mapPos;

	private FsmFloat fsmRunAwayAfterAttack;

	private FsmFloat fsmfollowUpAfterAttack;

	private FsmFloat fsmClimbTreeAfterAttack;

	private FsmFloat fsmRunHideAfterAttack;

	private FsmFloat fsmToStalkAfterAttack;

	private FsmFloat fsmRunTowardsFlank;

	private FsmFloat fsmRunTowardsScream;

	private FsmFloat fsmRunTowardsAttack;

	private FsmFloat fsmScreamRunTowards;

	private FsmFloat fsmLeaderCallFollowers;

	private FsmFloat fsmAttack;

	private FsmFloat fsmStepLeft;

	private FsmFloat fsmStepRight;

	private FsmFloat fsmScream;

	private FsmFloat fsmTreeAttack;

	private FsmFloat fsmTreeClimb;

	private FsmFloat fsmBackAway;

	private FsmFloat fsmDisengage;

	private FsmFloat fsmAwayReturn;

	private FsmFloat fsmAwayScream;

	private FsmFloat fsmAwayFlank;

	private FsmFloat fsmAwayToRock;

	private FsmFloat fsmAwayToBurn;

	private FsmFloat fsmToStructure;

	private FsmFloat fsmRunAwayDist;

	private FsmFloat fsmRunForwardStopDist;

	private FsmFloat fsmRunTowardPlayerDist;

	private FsmFloat fsmAttackChance;

	private FsmFloat fsmRunAwayChance;

	private FsmFloat fsmSneakForward;

	private FsmFloat fsmSneakBack;

	private FsmFloat fsmRunBack;

	private FsmFloat fsmRunForwardToTree;

	private FsmFloat fsmRunForward;

	private FsmFloat fsmStalkToTree;

	private FsmFloat fsmStalkToFlank;

	private FsmFloat fsmStalkRunTowards;

	private FsmFloat fsmStalkLeaveArea;

	private FsmFloat fsmStalkToSneakForward;

	private FsmFloat fsmStalkToRock;

	private FsmFloat fsmStalkToAmbush;

	public FsmFloat fsmFindPointNearPlayer;

	private FsmFloat fsmFindRandomPoint;

	private FsmFloat fsmRandomSearchRange;

	private FsmFloat fsmFindWayPoint;

	private FsmFloat fsmFindStructure;

	private FsmFloat fsmFindPlaneCrash;

	private FsmFloat fsmFindPlayerRangedPoint;

	private FsmInt fsmDaysToSearchPlane;

	private FsmFloat fsmSearchToWalk;

	private FsmFloat fsmSearchToRun;

	private FsmFloat FsmSearchWaitMin;

	private FsmFloat FsmSearchWaitMax;

	private FsmFloat fsmSearchRangedValue;

	private FsmFloat fsmSearchToEncounter;

	private FsmFloat fsmSearchToCall;

	private FsmFloat fsmSearchToPlaceArt;

	private FsmFloat fsmSearchToSearch;

	private FsmFloat fsmSearchToSleep;

	private FsmFloat fsmSearchToStructure;

	private float searchCautiousReset;

	private float searchCloseReset;

	private FsmFloat fsmWayPointArriveDist;

	private void Start()
	{
		this.setup = base.transform.GetComponent<mutantScriptSetup>();
		this.ai = base.GetComponent<mutantAI>();
		this.searchFunctions = base.GetComponent<mutantSearchFunctions>();
		this.animator = base.transform.GetComponent<Animator>();
		this.fsmRunAwayAfterAttack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunAwayAfterAttack");
		this.fsmfollowUpAfterAttack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmFollowUpAfterAttack");
		this.fsmClimbTreeAfterAttack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmClimbTreeAfterAttack");
		this.fsmRunHideAfterAttack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunHideAfterAttack");
		this.fsmToStalkAfterAttack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmToStalkAfterAttack");
		this.fsmRunTowardsFlank = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunTowardsFlank");
		this.fsmRunTowardsScream = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunTowardsScream");
		this.fsmRunTowardsAttack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunTowardsAttack");
		this.fsmScreamRunTowards = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmScreamRunTowards");
		this.fsmLeaderCallFollowers = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmLeaderCallFollowers");
		this.fsmAttack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmAttack");
		this.fsmStepLeft = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStepLeft");
		this.fsmStepRight = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStepRight");
		this.fsmScream = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmScream");
		this.fsmTreeAttack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmTreeAttack");
		this.fsmTreeClimb = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmTreeClimb");
		this.fsmBackAway = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmBackAway");
		this.fsmDisengage = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmDisengage");
		this.fsmAwayReturn = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmAwayReturn");
		this.fsmAwayScream = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmAwayScream");
		this.fsmAwayFlank = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmAwayFlank");
		this.fsmAwayToRock = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmAwayToRock");
		this.fsmAwayToBurn = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmAwayToBurn");
		this.fsmToStructure = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmToStructure");
		this.fsmRunAwayDist = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunAwayDist");
		this.fsmRunForwardStopDist = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunForwardStopDist");
		this.fsmRunTowardPlayerDist = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunTowardPlayerDist");
		this.fsmAttackChance = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmAttackChance");
		this.fsmRunAwayChance = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunAwayChance");
		this.fsmSneakForward = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmSneakForward");
		this.fsmSneakBack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmSneakBack");
		this.fsmRunBack = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunBack");
		this.fsmRunForwardToTree = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunForwardToTree");
		this.fsmRunForward = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRunForward");
		this.fsmStalkToTree = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStalkToTree");
		this.fsmStalkToFlank = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStalkToFlank");
		this.fsmStalkRunTowards = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStalkRunTowards");
		this.fsmStalkLeaveArea = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStalkLeaveArea");
		this.fsmStalkToSneakForward = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStalkToSneakForward");
		this.fsmStalkToRock = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStalkToRock");
		this.fsmStalkToAmbush = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmStalkToAmbush");
		this.fsmFindPointNearPlayer = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmFindPointNearPlayer");
		this.fsmFindRandomPoint = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmFindRandomPoint");
		this.fsmRandomSearchRange = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmRandomSearchRange");
		this.fsmFindWayPoint = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmFindWayPoint");
		this.fsmFindStructure = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmFindStructure");
		this.fsmFindPlaneCrash = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmFindPlaneCrash");
		this.fsmDaysToSearchPlane = this.setup.pmSearch.FsmVariables.GetFsmInt("fsmDaysToSearchPlane");
		this.fsmFindPlayerRangedPoint = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmFindPlayerRangedPoint");
		this.fsmSearchToWalk = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchToWalk");
		this.fsmSearchToRun = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchToRun");
		this.FsmSearchWaitMin = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchWaitMin");
		this.FsmSearchWaitMax = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchWaitMax");
		this.fsmSearchRangedValue = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchRangedValue");
		this.fsmSearchToEncounter = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchToEncounter");
		this.fsmSearchToCall = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchToCall");
		this.fsmSearchToPlaceArt = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchToPlaceArt");
		this.fsmSearchToSearch = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchToSearch");
		this.fsmSearchToSleep = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchToSleep");
		this.fsmSearchToStructure = this.setup.pmSearch.FsmVariables.GetFsmFloat("fsmSearchToStructure");
		this.fsmWayPointArriveDist = this.setup.pmSearch.FsmVariables.GetFsmFloat("waypointArriveDist");
		this.searchFunctions.cautiousReset = 24f;
		this.searchFunctions.searchReset = 30f;
		this.searchFunctions.playerAwareReset = 30f;
		this.setDefaultCombat();
		this.setDefaultStalking();
		this.setDefaultSearching();
	}

	private void OnSpawned()
	{
		base.Invoke("setupLists", 2f);
		if (!base.IsInvoking("setPlaneCrashBehaviour"))
		{
			base.InvokeRepeating("setPlaneCrashBehaviour", 3f, 3f);
		}
		if (!base.IsInvoking("setupPlayerSearchValues"))
		{
			base.InvokeRepeating("setupPlayerSearchValues", 10f, UnityEngine.Random.Range(5f, 10f));
		}
	}

	private void OnDespawned()
	{
		if (Scene.SceneTracker.aiManagers.Contains(this))
		{
			Scene.SceneTracker.aiManagers.Remove(this);
		}
		base.CancelInvoke("setupPlayerSearchValues");
		base.CancelInvoke("setPlaneCrashBehaviour");
		this.fsmFindPointNearPlayer.Value = 0f;
	}

	private void setupLists()
	{
		if (base.transform.root.gameObject.activeSelf && this.ai.leader && !Scene.SceneTracker.aiManagers.Contains(this))
		{
			Scene.SceneTracker.aiManagers.Add(this);
		}
	}

	private void setupPlayerSearchValues()
	{
		if (!Clock.InCave && this.ai.leader && !this.animator.GetBool("sleepBOOL"))
		{
			foreach (mutantAiManager current in Scene.SceneTracker.aiManagers)
			{
				if (current.fsmFindPointNearPlayer.Value > 0f)
				{
					this.checkCloseSearch = true;
				}
			}
			if (!this.checkCloseSearch || this.fsmFindPointNearPlayer.Value > 0f)
			{
				if (Clock.Dark)
				{
					if (this.ai.maleSkinny || this.ai.femaleSkinny)
					{
						this.fsmFindPointNearPlayer.Value = 0f;
					}
					else if (BoltNetwork.isRunning && Clock.Day > 6)
					{
						this.fsmFindPointNearPlayer.Value = 0f;
					}
					else
					{
						this.fsmFindPointNearPlayer.Value = 0f;
					}
					this.checkCloseSearch = false;
				}
				else if (this.ai.maleSkinny || this.ai.femaleSkinny)
				{
					if (BoltNetwork.isRunning && Clock.Day > 6)
					{
						this.fsmFindPointNearPlayer.Value = 0f;
					}
					else
					{
						this.fsmFindPointNearPlayer.Value = 0f;
					}
				}
				else if (BoltNetwork.isRunning && Clock.Day > 6)
				{
					this.fsmFindPointNearPlayer.Value = 0f;
				}
				else
				{
					this.fsmFindPointNearPlayer.Value = 0f;
				}
			}
			this.checkCloseSearch = false;
		}
		else
		{
			this.fsmFindPointNearPlayer.Value = 0f;
		}
	}

	private void setPlaneCrashBehaviour()
	{
		if (this.setup.disableAiForDebug)
		{
			return;
		}
		if (LocalPlayer.AnimControl.planeDist < 85f && this.setup.ai.playerDist < 85f)
		{
			if (this.ai.maleSkinny || this.ai.femaleSkinny)
			{
				this.setSkinnyStalking();
			}
			else
			{
				this.setPlaneCrashStalking();
				this.setPlaneCrashCombat();
			}
			if (!this.timerBlock)
			{
				base.Invoke("setFleeOverride", 75f);
				this.timerBlock = true;
			}
			if (Clock.InCave)
			{
				this.setCaveSearching();
				this.setCaveCombat();
			}
			if (!this.flee || !Clock.InCave)
			{
			}
		}
		else
		{
			base.CancelInvoke("setFleeOverride");
			this.flee = false;
			this.timerBlock = false;
			if (!Clock.InCave)
			{
				if (this.ai.maleSkinny || this.ai.femaleSkinny)
				{
					if (!Clock.Dark)
					{
						this.setSkinnyStalking();
					}
					else
					{
						this.setSkinnyNightStalking();
						this.setSkinnyAggressiveCombat();
					}
					if (this.ai.pale)
					{
						this.setCaveCombat();
					}
				}
				else
				{
					if (Clock.Dark)
					{
						this.setDefaultStalking();
					}
					else
					{
						this.setDayStalking();
					}
					if (this.ai.pale)
					{
						this.setCaveCombat();
					}
					else if (this.ai.fireman)
					{
						this.setFiremanCombat();
					}
					else
					{
						this.setDefaultCombat();
					}
				}
				if (Scene.MutantControler.hordeModeActive)
				{
					this.setAggressiveCombat();
				}
			}
			else
			{
				this.setCaveSearching();
				this.setCaveCombat();
			}
		}
	}

	private void setFleeOverride()
	{
		this.flee = true;
	}

	public void setDefaultCombat()
	{
		this.fsmRunAwayAfterAttack.Value = 0.4f;
		this.fsmfollowUpAfterAttack.Value = 1f;
		this.fsmClimbTreeAfterAttack.Value = 0.15f;
		this.fsmRunHideAfterAttack.Value = 0.15f;
		this.fsmToStalkAfterAttack.Value = 0.15f;
		this.fsmRunTowardsFlank.Value = 0.3f;
		this.fsmRunTowardsScream.Value = 0.1f;
		this.fsmRunTowardsAttack.Value = 1f;
		this.fsmScreamRunTowards.Value = 0.2f;
		this.fsmAttack.Value = 1f;
		this.fsmStepLeft.Value = 0.25f;
		this.fsmStepRight.Value = 0.25f;
		this.fsmScream.Value = 0.3f;
		if (this.ai.female)
		{
			this.fsmTreeAttack.Value = 0.45f;
		}
		else
		{
			this.fsmTreeAttack.Value = 0f;
		}
		this.fsmTreeClimb.Value = 0.15f;
		this.fsmBackAway.Value = 0.3f;
		if (this.flee)
		{
			this.fsmDisengage.Value = 0.6f;
		}
		else if (this.setup.typeSetup.spawner)
		{
			if (this.setup.typeSetup.spawner.allMembers.Count < 3)
			{
				this.fsmDisengage.Value = 0.6f;
			}
			else
			{
				this.fsmDisengage.Value = 0.1f;
			}
		}
		this.fsmAwayReturn.Value = 1f;
		this.fsmAwayScream.Value = 0.3f;
		this.fsmAwayFlank.Value = 0.6f;
		this.fsmAwayToRock.Value = 0.25f;
		this.fsmToStructure.Value = 0.25f;
		this.setup.dayCycle.updateFsmVariables();
	}

	public void setFiremanCombat()
	{
		this.fsmRunAwayAfterAttack.Value = 1f;
		this.fsmfollowUpAfterAttack.Value = 0.1f;
		this.fsmClimbTreeAfterAttack.Value = 0f;
		this.fsmRunHideAfterAttack.Value = 0f;
		this.fsmToStalkAfterAttack.Value = 0f;
		this.fsmRunTowardsFlank.Value = 0.1f;
		this.fsmRunTowardsScream.Value = 0f;
		this.fsmRunTowardsAttack.Value = 1f;
		this.fsmScreamRunTowards.Value = 0.1f;
		this.fsmAttack.Value = 1f;
		this.fsmStepLeft.Value = 0f;
		this.fsmStepRight.Value = 0f;
		this.fsmScream.Value = 0.5f;
		this.fsmTreeAttack.Value = 0f;
		this.fsmTreeClimb.Value = 0f;
		this.fsmBackAway.Value = 1f;
		if (this.setup.typeSetup.spawner)
		{
			if (this.setup.typeSetup.spawner.allMembers.Count < 3)
			{
				this.fsmDisengage.Value = 0.6f;
			}
			else
			{
				this.fsmDisengage.Value = 0.1f;
			}
		}
		this.fsmAwayReturn.Value = 0.5f;
		this.fsmAwayScream.Value = 0.45f;
		this.fsmAwayFlank.Value = 0.1f;
		this.fsmAwayToRock.Value = 0.1f;
		this.fsmAwayToBurn.Value = 1f;
		this.fsmToStructure.Value = 0.25f;
	}

	public void setAggressiveCombat()
	{
		if (Scene.MutantControler.hordeModeActive)
		{
			this.setup.dayCycle.aggression = 10;
		}
		this.fsmRunAwayAfterAttack.Value = 0.05f;
		this.fsmfollowUpAfterAttack.Value = 1f;
		this.fsmClimbTreeAfterAttack.Value = 0.1f;
		this.fsmRunHideAfterAttack.Value = 0f;
		this.fsmToStalkAfterAttack.Value = 0f;
		this.fsmRunTowardsFlank.Value = 0.1f;
		this.fsmRunTowardsScream.Value = 0f;
		this.fsmRunTowardsAttack.Value = 1f;
		this.fsmScreamRunTowards.Value = 0.25f;
		this.fsmAttack.Value = 1f;
		this.fsmStepLeft.Value = 0f;
		this.fsmStepRight.Value = 0f;
		this.fsmScream.Value = 0.5f;
		this.fsmTreeAttack.Value = 0.05f;
		this.fsmTreeClimb.Value = 0.1f;
		this.fsmBackAway.Value = 0f;
		if (this.setup.typeSetup.spawner)
		{
			if (this.setup.typeSetup.spawner.allMembers.Count < 3)
			{
				this.fsmDisengage.Value = 0.6f;
			}
			else
			{
				this.fsmDisengage.Value = 0f;
			}
		}
		this.fsmAwayReturn.Value = 1f;
		this.fsmAwayScream.Value = 0.3f;
		this.fsmAwayFlank.Value = 0.6f;
		this.fsmAwayToRock.Value = 0.25f;
		if (this.ai.insideBase)
		{
			this.fsmToStructure.Value = 1.35f;
		}
		else
		{
			this.fsmToStructure.Value = 0.25f;
		}
	}

	public void setCaveCombat()
	{
		this.fsmRunAwayAfterAttack.Value = 0.1f;
		this.fsmfollowUpAfterAttack.Value = 0.9f;
		this.fsmClimbTreeAfterAttack.Value = 0f;
		this.fsmRunHideAfterAttack.Value = 0f;
		this.fsmToStalkAfterAttack.Value = 0f;
		this.fsmRunTowardsFlank.Value = 0f;
		this.fsmRunTowardsScream.Value = 0f;
		this.fsmRunTowardsAttack.Value = 1f;
		this.fsmScreamRunTowards.Value = 0.25f;
		this.fsmAttack.Value = 1f;
		this.fsmStepLeft.Value = 0f;
		this.fsmStepRight.Value = 0f;
		this.fsmScream.Value = 0.35f;
		this.fsmTreeAttack.Value = 0f;
		this.fsmTreeClimb.Value = 0f;
		this.fsmBackAway.Value = 0f;
		this.fsmDisengage.Value = 0f;
		this.fsmAwayReturn.Value = 1f;
		this.fsmAwayScream.Value = 0.2f;
		this.fsmAwayFlank.Value = 0f;
		this.fsmAwayToRock.Value = 0f;
		this.fsmAwayToBurn.Value = 0f;
		this.fsmToStructure.Value = 0f;
		this.setup.dayCycle.aggression = 20;
		this.setup.dayCycle.updateFsmVariables();
	}

	public void setPlaneCrashCombat()
	{
		this.fsmRunAwayAfterAttack.Value = 0.4f;
		this.fsmfollowUpAfterAttack.Value = 0.7f;
		this.fsmClimbTreeAfterAttack.Value = 0.15f;
		this.fsmRunHideAfterAttack.Value = 0.1f;
		this.fsmToStalkAfterAttack.Value = 0.3f;
		this.fsmRunTowardsFlank.Value = 0.3f;
		this.fsmRunTowardsScream.Value = 0.2f;
		this.fsmRunTowardsAttack.Value = 1f;
		this.fsmScreamRunTowards.Value = 0.2f;
		this.fsmAttack.Value = 1f;
		this.fsmStepLeft.Value = 0.1f;
		this.fsmStepRight.Value = 0.1f;
		this.fsmScream.Value = 0.3f;
		if (this.ai.female)
		{
			this.fsmTreeAttack.Value = 0.45f;
		}
		else
		{
			this.fsmTreeAttack.Value = 0f;
		}
		this.fsmTreeClimb.Value = 0.15f;
		this.fsmBackAway.Value = 0.3f;
		if (this.flee)
		{
			this.fsmDisengage.Value = 0.6f;
		}
		else if (this.setup.typeSetup.spawner)
		{
			if (this.setup.typeSetup.spawner.allMembers.Count < 3)
			{
				this.fsmDisengage.Value = 0.5f;
			}
			else
			{
				this.fsmDisengage.Value = 0.1f;
			}
		}
		this.fsmAwayReturn.Value = 1f;
		this.fsmAwayScream.Value = 0.3f;
		this.fsmAwayFlank.Value = 0.6f;
		this.fsmAwayToRock.Value = 0.25f;
		this.fsmToStructure.Value = 0.25f;
		this.setup.dayCycle.updateFsmVariables();
	}

	public void setDefensiveCombat()
	{
		this.fsmRunAwayAfterAttack.Value = 1f;
		this.fsmfollowUpAfterAttack.Value = 0.3f;
		this.fsmClimbTreeAfterAttack.Value = 0.5f;
		this.fsmRunHideAfterAttack.Value = 1f;
		this.fsmRunTowardsFlank.Value = 1f;
		this.fsmRunTowardsScream.Value = 0.5f;
		this.fsmRunTowardsAttack.Value = 1f;
		this.fsmAttack.Value = 0.5f;
		this.fsmStepLeft.Value = 0.5f;
		this.fsmStepRight.Value = 0.5f;
		this.fsmScream.Value = 0.5f;
		this.fsmTreeAttack.Value = 0.5f;
		this.fsmTreeClimb.Value = 0.5f;
		this.fsmBackAway.Value = 1f;
		if (this.flee)
		{
			this.fsmDisengage.Value = 0.6f;
		}
		else if (this.setup.typeSetup.spawner)
		{
			if (this.setup.typeSetup.spawner.allMembers.Count < 3)
			{
				this.fsmDisengage.Value = 0.6f;
			}
			else
			{
				this.fsmDisengage.Value = 0.1f;
			}
		}
		this.fsmAwayReturn.Value = 1f;
		this.fsmAwayScream.Value = 0.3f;
		this.fsmAwayFlank.Value = 0.6f;
		this.fsmAwayToRock.Value = 0.25f;
		this.fsmToStructure.Value = 0.2f;
	}

	public void setSkinnyCombat()
	{
		this.fsmRunAwayAfterAttack.Value = 1f;
		this.fsmfollowUpAfterAttack.Value = 0.1f;
		this.fsmClimbTreeAfterAttack.Value = 0.15f;
		this.fsmRunHideAfterAttack.Value = 1f;
		this.fsmRunTowardsFlank.Value = 1f;
		this.fsmRunTowardsScream.Value = 0.5f;
		this.fsmRunTowardsAttack.Value = 1f;
		this.fsmAttack.Value = 0.5f;
		this.fsmStepLeft.Value = 0.5f;
		this.fsmStepRight.Value = 0.5f;
		this.fsmScream.Value = 0.5f;
		this.fsmTreeAttack.Value = 0.5f;
		this.fsmTreeClimb.Value = 0.2f;
		this.fsmBackAway.Value = 1f;
		if (this.flee)
		{
			this.fsmDisengage.Value = 1f;
		}
		else if (this.setup.typeSetup.spawner)
		{
			if (this.setup.typeSetup.spawner.allMembers.Count < 3)
			{
				this.fsmDisengage.Value = 0.6f;
			}
			else
			{
				this.fsmDisengage.Value = 0.1f;
			}
		}
		this.fsmAwayToBurn.Value = 0f;
		this.fsmToStructure.Value = 0.25f;
		this.setup.dayCycle.updateFsmVariables();
	}

	public void setSkinnyAggressiveCombat()
	{
		this.fsmRunAwayAfterAttack.Value = 0f;
		this.fsmfollowUpAfterAttack.Value = 1f;
		this.fsmClimbTreeAfterAttack.Value = 0.1f;
		this.fsmRunHideAfterAttack.Value = 0.1f;
		this.fsmToStalkAfterAttack.Value = 0.05f;
		this.fsmRunTowardsFlank.Value = 0.24f;
		this.fsmRunTowardsScream.Value = 0.1f;
		this.fsmRunTowardsAttack.Value = 1f;
		this.fsmScreamRunTowards.Value = 0.2f;
		this.fsmAttack.Value = 1f;
		this.fsmStepLeft.Value = 0.1f;
		this.fsmStepRight.Value = 0.1f;
		this.fsmScream.Value = 0.3f;
		this.fsmTreeAttack.Value = 0.25f;
		this.fsmTreeClimb.Value = 0.1f;
		this.fsmBackAway.Value = 0f;
		if (this.flee)
		{
			this.fsmDisengage.Value = 0.5f;
		}
		this.fsmDisengage.Value = 0.1f;
		this.fsmAwayReturn.Value = 1f;
		this.fsmAwayScream.Value = 0.25f;
		this.fsmAwayFlank.Value = 0.2f;
		this.fsmAwayToRock.Value = 0.2f;
		this.fsmAwayToBurn.Value = 0f;
		this.fsmToStructure.Value = 0.05f;
		this.setup.dayCycle.aggression = 10;
		this.setup.dayCycle.updateFsmVariables();
	}

	public void setDayStalking()
	{
		this.fsmRunAwayDist.Value = 24f;
		this.fsmRunForwardStopDist.Value = 38f;
		this.fsmRunTowardPlayerDist.Value = 53f;
		this.fsmAttackChance.Value = 0.1f;
		this.fsmRunAwayChance.Value = 1f;
		this.fsmSneakForward.Value = 1f;
		this.fsmRunForward.Value = 0.3f;
		this.fsmRunForwardToTree.Value = 0.2f;
		this.fsmSneakBack.Value = 0.5f;
		this.fsmRunBack.Value = 1f;
		this.fsmStalkToTree.Value = 0.1f;
		this.fsmStalkToFlank.Value = 1f;
		this.fsmStalkRunTowards.Value = 0.2f;
		this.fsmStalkLeaveArea.Value = 0.6f;
		this.fsmStalkToSneakForward.Value = 0.5f;
		this.fsmStalkToRock.Value = 0.5f;
	}

	public void setDefaultStalking()
	{
		this.fsmRunAwayDist.Value = 24f;
		this.fsmRunForwardStopDist.Value = 38f;
		this.fsmRunTowardPlayerDist.Value = 53f;
		this.fsmAttackChance.Value = 0.5f;
		this.fsmRunAwayChance.Value = 1f;
		this.fsmSneakForward.Value = 1f;
		this.fsmRunForward.Value = 0.3f;
		this.fsmRunForwardToTree.Value = 0.3f;
		this.fsmSneakBack.Value = 0.7f;
		this.fsmRunBack.Value = 1f;
		this.fsmStalkToTree.Value = 0.15f;
		this.fsmStalkToFlank.Value = 1f;
		this.fsmStalkRunTowards.Value = 0.2f;
		if (this.flee)
		{
			this.fsmStalkLeaveArea.Value = 0.75f;
		}
		else
		{
			this.fsmStalkLeaveArea.Value = 0.05f;
		}
		this.fsmStalkToSneakForward.Value = 0.5f;
		this.fsmStalkToRock.Value = 0.5f;
		this.fsmStalkToAmbush.Value = 0.1f;
	}

	public void setSkinnyStalking()
	{
		this.fsmRunAwayDist.Value = 24f;
		this.fsmRunForwardStopDist.Value = 38f;
		this.fsmRunTowardPlayerDist.Value = 53f;
		this.fsmAttackChance.Value = 0.2f;
		this.fsmRunAwayChance.Value = 1f;
		this.fsmSneakForward.Value = 1f;
		this.fsmRunForward.Value = 0.1f;
		this.fsmRunForwardToTree.Value = 0.1f;
		this.fsmSneakBack.Value = 0.25f;
		this.fsmRunBack.Value = 1f;
		this.fsmStalkToTree.Value = 0.35f;
		this.fsmStalkToFlank.Value = 1f;
		this.fsmStalkRunTowards.Value = 0.2f;
		this.fsmStalkLeaveArea.Value = 0.7f;
		this.fsmStalkToSneakForward.Value = 0.5f;
		this.fsmStalkToRock.Value = 0.5f;
	}

	public void setSkinnyNightStalking()
	{
		this.fsmRunAwayDist.Value = 24f;
		this.fsmRunForwardStopDist.Value = 38f;
		this.fsmRunTowardPlayerDist.Value = 53f;
		this.fsmAttackChance.Value = 0.85f;
		this.fsmRunAwayChance.Value = 1f;
		this.fsmSneakForward.Value = 8f;
		this.fsmRunForward.Value = 0.4f;
		this.fsmRunForwardToTree.Value = 0.1f;
		this.fsmSneakBack.Value = 0.25f;
		this.fsmRunBack.Value = 1f;
		this.fsmStalkToTree.Value = 0.1f;
		this.fsmStalkToFlank.Value = 1f;
		this.fsmStalkRunTowards.Value = 0.2f;
		this.fsmStalkLeaveArea.Value = 0.05f;
		this.fsmStalkToSneakForward.Value = 0.5f;
		this.fsmStalkToRock.Value = 0.5f;
	}

	public void setPlaneCrashStalking()
	{
		this.fsmRunAwayDist.Value = 30f;
		this.fsmRunForwardStopDist.Value = 45f;
		this.fsmRunTowardPlayerDist.Value = 58f;
		this.fsmAttackChance.Value = 0.35f;
		this.fsmRunAwayChance.Value = 1f;
		this.fsmSneakForward.Value = 1f;
		this.fsmRunForward.Value = 0.3f;
		this.fsmRunForwardToTree.Value = 0.5f;
		this.fsmSneakBack.Value = 0.7f;
		this.fsmRunBack.Value = 1f;
		this.fsmStalkToTree.Value = 0.15f;
		this.fsmStalkToFlank.Value = 1f;
		this.fsmStalkRunTowards.Value = 0.05f;
		if (this.flee)
		{
			this.fsmStalkLeaveArea.Value = 0.75f;
		}
		else
		{
			this.fsmStalkLeaveArea.Value = 0.6f;
		}
		this.fsmStalkToSneakForward.Value = 0.4f;
		this.fsmStalkToRock.Value = 0.3f;
	}

	public void setTestStalking()
	{
		this.fsmRunAwayDist.Value = 30f;
		this.fsmRunForwardStopDist.Value = 45f;
		this.fsmRunTowardPlayerDist.Value = 60f;
		this.fsmAttackChance.Value = 1f;
		this.fsmRunAwayChance.Value = 0.2f;
		this.fsmSneakForward.Value = 1f;
		this.fsmRunForward.Value = 0.3f;
		this.fsmRunForwardToTree.Value = 0.7f;
		this.fsmSneakBack.Value = 0.7f;
		this.fsmRunBack.Value = 1f;
		this.fsmStalkToTree.Value = 0f;
		this.fsmStalkToFlank.Value = 1f;
		this.fsmStalkRunTowards.Value = 0.2f;
		this.fsmStalkLeaveArea.Value = 0f;
		this.fsmStalkToSneakForward.Value = 0.4f;
		this.fsmStalkToRock.Value = 0.3f;
	}

	public void setDefaultSearching()
	{
		if (this.inTopHalfMap)
		{
			this.fsmFindRandomPoint.Value = 0.6f;
		}
		else
		{
			this.fsmFindRandomPoint.Value = 0.7f;
		}
		this.fsmRandomSearchRange.Value = 150f;
		this.fsmFindWayPoint.Value = 0.35f;
		this.fsmFindStructure.Value = 0.9f;
		this.fsmFindPlaneCrash.Value = 0f;
		if (this.inTopHalfMap)
		{
			this.fsmFindPlayerRangedPoint.Value = 0.7f;
		}
		else
		{
			this.fsmFindPlayerRangedPoint.Value = 0.6f;
		}
		this.fsmDaysToSearchPlane.Value = 2;
		this.fsmSearchToWalk.Value = 0.75f;
		this.fsmSearchToRun.Value = 0.4f;
		this.FsmSearchWaitMin.Value = 5f;
		this.FsmSearchWaitMax.Value = 10f;
		this.fsmWayPointArriveDist.Value = 15f;
		if (this.inTopHalfMap)
		{
			this.fsmSearchRangedValue.Value = 150f;
		}
		else
		{
			this.fsmSearchRangedValue.Value = 250f;
		}
		this.fsmSearchToEncounter.Value = 1f;
		this.fsmSearchToCall.Value = 1f;
		this.fsmSearchToPlaceArt.Value = 0.15f;
		this.fsmSearchToSearch.Value = 0.7f;
		this.fsmSearchToSleep.Value = 0f;
		this.fsmSearchToStructure.Value = 0.25f;
		this.searchFunctions.cautiousReset = 20f;
		this.searchFunctions.searchReset = 10f;
		this.searchFunctions.playerAwareReset = 30f;
	}

	public void setCaveSearching()
	{
		this.fsmFindRandomPoint.Value = 1f;
		this.fsmRandomSearchRange.Value = 10f;
		this.fsmFindWayPoint.Value = 0.2f;
		this.fsmFindStructure.Value = 0f;
		this.fsmFindPlaneCrash.Value = 0f;
		this.fsmFindPlayerRangedPoint.Value = 0f;
		this.fsmDaysToSearchPlane.Value = 2;
		this.fsmSearchToWalk.Value = 1f;
		this.fsmSearchToRun.Value = 0f;
		this.FsmSearchWaitMin.Value = 5f;
		this.FsmSearchWaitMax.Value = 10f;
		this.fsmWayPointArriveDist.Value = 5f;
		this.fsmSearchToEncounter.Value = 0f;
		this.fsmSearchToCall.Value = 0.25f;
		this.fsmSearchToPlaceArt.Value = 0f;
		this.fsmSearchToSearch.Value = 1f;
		this.fsmSearchToSleep.Value = 0f;
		this.fsmSearchToStructure.Value = 0f;
		this.searchFunctions.cautiousReset = 20f;
		this.searchFunctions.searchReset = 10f;
		this.searchFunctions.playerAwareReset = 30f;
	}

	public void setDaySearching()
	{
		if (this.inTopHalfMap)
		{
			this.fsmFindRandomPoint.Value = 0.6f;
		}
		else
		{
			this.fsmFindRandomPoint.Value = 0.8f;
		}
		this.fsmRandomSearchRange.Value = 100f;
		this.fsmFindWayPoint.Value = 0.2f;
		this.fsmFindStructure.Value = 0.2f;
		this.fsmFindPlaneCrash.Value = 0f;
		if (this.inTopHalfMap)
		{
			this.fsmFindPlayerRangedPoint.Value = 0.75f;
		}
		else
		{
			this.fsmFindPlayerRangedPoint.Value = 0.5f;
		}
		this.fsmDaysToSearchPlane.Value = 2;
		this.fsmSearchToWalk.Value = 1f;
		this.fsmSearchToRun.Value = 0.35f;
		this.FsmSearchWaitMin.Value = 5f;
		this.FsmSearchWaitMax.Value = 10f;
		this.fsmWayPointArriveDist.Value = 15f;
		if (this.inTopHalfMap)
		{
			this.fsmSearchRangedValue.Value = 180f;
		}
		else
		{
			this.fsmSearchRangedValue.Value = 250f;
		}
		this.fsmSearchToEncounter.Value = 1f;
		this.fsmSearchToCall.Value = 0.5f;
		this.fsmSearchToPlaceArt.Value = 0.15f;
		this.fsmSearchToSearch.Value = 1f;
		this.fsmSearchToSleep.Value = 0f;
		this.fsmSearchToStructure.Value = 0.25f;
		this.searchFunctions.cautiousReset = 20f;
		this.searchFunctions.searchReset = 10f;
		this.searchFunctions.playerAwareReset = 30f;
	}

	public void setSkinnyDaySearching()
	{
		if (this.inTopHalfMap)
		{
			this.fsmFindRandomPoint.Value = 0.4f;
		}
		else
		{
			this.fsmFindRandomPoint.Value = 0.75f;
		}
		this.fsmRandomSearchRange.Value = 140f;
		this.fsmFindWayPoint.Value = 0.2f;
		this.fsmFindStructure.Value = 0.2f;
		this.fsmFindPlaneCrash.Value = 0f;
		if (this.inTopHalfMap)
		{
			this.fsmFindPlayerRangedPoint.Value = 0.85f;
		}
		else
		{
			this.fsmFindPlayerRangedPoint.Value = 0.5f;
		}
		this.fsmDaysToSearchPlane.Value = 2;
		this.fsmSearchToWalk.Value = 0.5f;
		this.fsmSearchToRun.Value = 0.5f;
		this.FsmSearchWaitMin.Value = 5f;
		this.FsmSearchWaitMax.Value = 15f;
		this.fsmWayPointArriveDist.Value = 15f;
		if (this.inTopHalfMap)
		{
			this.fsmSearchRangedValue.Value = 180f;
		}
		else
		{
			this.fsmSearchRangedValue.Value = 400f;
		}
		this.fsmSearchToEncounter.Value = 0.3f;
		this.fsmSearchToCall.Value = 0.15f;
		this.fsmSearchToPlaceArt.Value = 0f;
		this.fsmSearchToSearch.Value = 1f;
		this.fsmSearchToSleep.Value = 0f;
		this.fsmSearchToStructure.Value = 0.25f;
		this.searchFunctions.cautiousReset = 20f;
		this.searchFunctions.searchReset = 10f;
		this.searchFunctions.playerAwareReset = 30f;
	}

	public void setSkinnyNightSearching()
	{
		if (this.inTopHalfMap)
		{
			this.fsmFindRandomPoint.Value = 0.3f;
		}
		else
		{
			this.fsmFindRandomPoint.Value = 0.5f;
		}
		this.fsmFindRandomPoint.Value = 0.3f;
		this.fsmRandomSearchRange.Value = 110f;
		this.fsmFindWayPoint.Value = 0.1f;
		this.fsmFindStructure.Value = 0.8f;
		this.fsmFindPlayerRangedPoint.Value = 0.6f;
		this.fsmFindPlaneCrash.Value = 0.05f;
		this.fsmDaysToSearchPlane.Value = 2;
		this.fsmFindPlayerRangedPoint.Value = 0.75f;
		if (this.inTopHalfMap)
		{
			this.fsmFindPlayerRangedPoint.Value = 1f;
		}
		else
		{
			this.fsmFindPlayerRangedPoint.Value = 1f;
		}
		this.fsmSearchToWalk.Value = 0.25f;
		this.fsmSearchToRun.Value = 0.75f;
		this.FsmSearchWaitMin.Value = 5f;
		this.FsmSearchWaitMax.Value = 15f;
		this.fsmWayPointArriveDist.Value = 15f;
		if (this.inTopHalfMap)
		{
			this.fsmSearchRangedValue.Value = 140f;
		}
		else
		{
			this.fsmSearchRangedValue.Value = 250f;
		}
		this.fsmSearchToEncounter.Value = 0.25f;
		this.fsmSearchToCall.Value = 0.2f;
		this.fsmSearchToPlaceArt.Value = 0f;
		this.fsmSearchToSearch.Value = 1f;
		this.fsmSearchToSleep.Value = 0f;
		this.fsmSearchToStructure.Value = 0.25f;
		this.searchFunctions.cautiousReset = 20f;
		this.searchFunctions.searchReset = 10f;
		this.searchFunctions.playerAwareReset = 30f;
	}

	public void setTestSearching()
	{
		this.fsmFindRandomPoint.Value = 0.1f;
		this.fsmRandomSearchRange.Value = 50f;
		this.fsmFindWayPoint.Value = 0f;
		this.fsmFindStructure.Value = 0f;
		this.fsmFindPlaneCrash.Value = 0f;
		this.fsmDaysToSearchPlane.Value = 2;
		this.fsmSearchToWalk.Value = 0.1f;
		this.fsmSearchToRun.Value = 1f;
		this.FsmSearchWaitMin.Value = 5f;
		this.FsmSearchWaitMax.Value = 8f;
		this.fsmSearchToEncounter.Value = 0f;
		this.fsmSearchToCall.Value = 0.1f;
		this.fsmSearchToPlaceArt.Value = 0f;
		this.fsmSearchToSearch.Value = 0.5f;
		this.fsmSearchToSleep.Value = 0f;
		this.searchFunctions.cautiousReset = 20f;
		this.searchFunctions.searchReset = 10f;
		this.searchFunctions.playerAwareReset = 30f;
	}

	private void sendDeathReactions()
	{
		if (this.ai.female)
		{
			if (!BoltNetwork.isRunning)
			{
				this.currMaxAttackers = LocalPlayer.TargetFunctions.maxAttackers;
				LocalPlayer.TargetFunctions.maxAttackers = 5;
			}
			this.setup.familyFunctions.sendAggressiveCombat(45f);
			base.Invoke("resetMaxAttackers", 45f);
		}
		else if (!Clock.InCave && !Clock.Dark && UnityEngine.Random.value > 0.4f)
		{
			this.setup.familyFunctions.sendFleeArea();
		}
		else if (!Clock.InCave && Clock.Dark && UnityEngine.Random.value > 0.7f)
		{
			this.setup.familyFunctions.sendFleeArea();
		}
	}

	private void resetMaxAttackers()
	{
		if (!BoltNetwork.isRunning)
		{
			LocalPlayer.TargetFunctions.maxAttackers = this.currMaxAttackers;
		}
	}

	private void debugSearchParams()
	{
	}
}
