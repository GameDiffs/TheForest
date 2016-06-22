using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantDayCycle : MonoBehaviour
{
	private mutantScriptSetup setup;

	private mutantPropManager props;

	private mutantAiManager aiManager;

	private mutantAI ai;

	private spawnMutants spawn;

	public bool creepy;

	public int aggression;

	public int fear;

	public bool sleep;

	private int countBlock;

	public int day;

	public bool dayBlocker;

	public bool sleepBlocker;

	public bool dark;

	private bool previousDark;

	private bool mutantInCave;

	private int sleepCount;

	private FsmInt fsmAggresion;

	private FsmInt fsmFear;

	private FsmBool fsmSleep;

	private FsmInt fsmCurrentDay;

	private FsmBool fsmDark;

	private FsmFloat fsmRoamRange;

	private FsmFloat fsmRangedSearchDist;

	private FsmFloat fsmSearchRange;

	private FsmBool fsmInCave;

	private void Awake()
	{
		this.props = base.GetComponent<mutantPropManager>();
		this.aiManager = base.GetComponent<mutantAiManager>();
		this.ai = base.GetComponent<mutantAI>();
	}

	private void Start()
	{
		this.previousDark = Clock.Dark;
		this.setup = base.GetComponent<mutantScriptSetup>();
		if (this.setup.ai.creepy || this.setup.ai.creepy_male || this.setup.ai.creepy_baby || this.setup.ai.creepy_fat)
		{
			this.fsmRoamRange = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmRoamRange");
			this.fsmSearchRange = this.setup.pmCombat.FsmVariables.GetFsmFloat("fsmSearchRange");
			this.fsmInCave = this.setup.pmCombat.FsmVariables.GetFsmBool("inCaveBool");
		}
		else
		{
			this.fsmAggresion = this.setup.pmBrain.FsmVariables.GetFsmInt("aggression");
			this.fsmFear = this.setup.pmBrain.FsmVariables.GetFsmInt("fear");
			this.fsmSleep = this.setup.pmBrain.FsmVariables.GetFsmBool("sleepBool");
			this.fsmCurrentDay = this.setup.pmBrain.FsmVariables.GetFsmInt("currentDay");
			this.fsmDark = this.setup.pmBrain.FsmVariables.GetFsmBool("dark");
			this.fsmInCave = this.setup.pmSleep.FsmVariables.GetFsmBool("inCaveBool");
		}
		base.InvokeRepeating("setDayConditions", 1f, 5f);
		base.Invoke("initWakeUp", 5f);
	}

	private void OnEnable()
	{
		if (Time.time > 10f && this.setup)
		{
			base.Invoke("initWakeUp", 5f);
		}
	}

	public void initWakeUp()
	{
		if (this.setup.spawnGo)
		{
			this.spawn = this.setup.spawnGo.GetComponent<spawnMutants>();
			if (!this.setup.ai.creepy && !this.setup.ai.creepy_male && !this.setup.ai.creepy_baby && !this.setup.ai.creepy_fat)
			{
				if (this.spawn.sleepingSpawn)
				{
					this.fsmSleep.Value = true;
					this.setup.pmBrain.SendEvent("toActivateFSM");
					return;
				}
				if (Clock.Day > 0)
				{
					this.fsmSleep.Value = false;
				}
				if (!Clock.InCave && !this.setup.pmSleep.FsmVariables.GetFsmBool("inCaveBool").Value)
				{
					if (Clock.Day == 0 && !this.spawn.instantSpawn && !Scene.MutantControler.hordeModeActive && !Clock.Dark)
					{
						base.StartCoroutine(this.sendWakeUp(250f));
					}
					else
					{
						base.StartCoroutine(this.sendWakeUp(0f));
					}
				}
				if (this.sleepBlocker)
				{
					base.StartCoroutine(this.sendWakeUp(0f));
				}
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator sendWakeUp(float delay)
	{
		mutantDayCycle.<sendWakeUp>c__Iterator7B <sendWakeUp>c__Iterator7B = new mutantDayCycle.<sendWakeUp>c__Iterator7B();
		<sendWakeUp>c__Iterator7B.delay = delay;
		<sendWakeUp>c__Iterator7B.<$>delay = delay;
		<sendWakeUp>c__Iterator7B.<>f__this = this;
		return <sendWakeUp>c__Iterator7B;
	}

	private void setRandomWakeUp()
	{
		base.Invoke("initWakeUp", (float)UnityEngine.Random.Range(90, 250));
	}

	private void getDark()
	{
		this.fsmDark.Value = Clock.Dark;
	}

	private void checkDayChanged()
	{
		if (this.previousDark != Clock.Dark)
		{
			if (this.setup.pmSearch)
			{
				this.setup.pmSearch.FsmVariables.GetFsmInt("sleepCounter").Value = 0;
			}
			this.previousDark = Clock.Dark;
		}
	}

	private void enableSleepOverride()
	{
		this.dayBlocker = true;
	}

	private void disableSleepOverride()
	{
		this.dayBlocker = false;
	}

	private void setDayMutant()
	{
		if (this.fsmInCave.Value)
		{
			return;
		}
		if (this.ai.femaleSkinny || this.ai.maleSkinny)
		{
			this.aiManager.setSkinnyDaySearching();
		}
		else
		{
			this.aiManager.setDaySearching();
		}
		this.props.disableLights();
	}

	private void setNightMutant()
	{
		if (this.fsmInCave.Value)
		{
			return;
		}
		if (this.ai.maleSkinny || this.ai.femaleSkinny)
		{
			this.aiManager.setSkinnyNightSearching();
		}
		else
		{
			this.aiManager.setDefaultSearching();
		}
		if (!this.setup.ai.pale)
		{
			if (this.setup.ai.leader)
			{
				this.props.enableLights();
			}
			else
			{
				this.props.disableLights();
			}
		}
	}

	public void armsyInCave()
	{
		this.mutantInCave = true;
	}

	private void setDayConditions()
	{
		this.day = Clock.Day;
		this.dark = Clock.Dark;
		this.checkDayChanged();
		if (!this.creepy)
		{
			if (Clock.InCave && !this.dayBlocker && !this.sleepBlocker)
			{
				this.sleep = true;
				this.fsmSleep.Value = this.sleep;
				this.getDark();
				this.setup.pmSleep.enabled = true;
			}
			else if (!Clock.Dark && !this.dayBlocker)
			{
				this.getDark();
				this.setDayMutant();
			}
			else if (this.dayBlocker || Clock.Dark)
			{
				this.getDark();
				if (Clock.Dark)
				{
					this.setNightMutant();
				}
				else
				{
					this.setDayMutant();
				}
			}
		}
		if (this.day == 0 && this.countBlock < 1)
		{
			this.countBlock++;
			if (Clock.Dark)
			{
				this.sleep = false;
			}
			else
			{
				this.sleep = false;
			}
			if (this.setup.ai.creepy || this.setup.ai.creepy_male || this.setup.ai.creepy_fat)
			{
				if (this.fsmInCave.Value)
				{
					this.fsmRoamRange.Value = 15f;
					this.fsmSearchRange.Value = 10f;
				}
				else
				{
					this.fsmRoamRange.Value = 20f;
					this.fsmSearchRange.Value = 50f;
				}
			}
			else
			{
				if (!BoltNetwork.isRunning)
				{
					LocalPlayer.TargetFunctions.maxAttackers = 3;
				}
				this.aggression = 1;
				this.fear = 4;
				this.updateFsmVariables();
			}
		}
		else if (this.day == 1 && this.countBlock < 2)
		{
			this.countBlock++;
			if (Clock.Dark)
			{
				this.sleep = false;
			}
			else
			{
				this.sleep = false;
			}
			if (this.creepy)
			{
				if (this.mutantInCave)
				{
					this.fsmRoamRange.Value = 15f;
					this.fsmSearchRange.Value = 10f;
				}
				else
				{
					this.fsmRoamRange.Value = 60f;
					this.fsmSearchRange.Value = 50f;
				}
			}
			else
			{
				if (!BoltNetwork.isRunning)
				{
					LocalPlayer.TargetFunctions.maxAttackers = 3;
				}
				this.aggression = 1;
				this.fear = 4;
				this.updateFsmVariables();
			}
		}
		else if (this.day == 2 && this.countBlock < 3)
		{
			this.countBlock++;
			if (Clock.Dark)
			{
				this.sleep = false;
			}
			else
			{
				this.sleep = false;
			}
			if (this.creepy)
			{
				if (this.mutantInCave)
				{
					this.fsmRoamRange.Value = 15f;
					this.fsmSearchRange.Value = 10f;
				}
				else
				{
					this.fsmRoamRange.Value = 100f;
					this.fsmSearchRange.Value = 50f;
				}
			}
			else
			{
				if (!BoltNetwork.isRunning)
				{
					LocalPlayer.TargetFunctions.maxAttackers = 3;
				}
				this.aggression = 2;
				this.fear = 4;
				this.updateFsmVariables();
			}
		}
		else if (this.day >= 3 && this.countBlock < 4)
		{
			this.countBlock++;
			if (Clock.Dark)
			{
				this.sleep = false;
			}
			else
			{
				this.sleep = false;
			}
			if (this.creepy)
			{
				if (this.mutantInCave)
				{
					this.fsmRoamRange.Value = 15f;
					this.fsmSearchRange.Value = 10f;
				}
				else
				{
					this.fsmRoamRange.Value = 200f;
					this.fsmSearchRange.Value = 50f;
				}
			}
			else
			{
				this.aggression = 3;
				this.fear = 2;
				this.updateFsmVariables();
			}
		}
	}

	public void updateFsmVariables()
	{
		this.fsmCurrentDay.Value = this.day;
		if (Scene.MutantControler.hordeModeActive)
		{
			this.fsmAggresion.Value = 10;
		}
		else
		{
			this.fsmAggresion.Value = this.aggression;
		}
		this.fsmFear.Value = this.fear;
	}
}
