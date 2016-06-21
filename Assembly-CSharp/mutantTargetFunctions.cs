using HutongGames.PlayMaker;
using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantTargetFunctions : MonoBehaviour
{
	private Animator animator;

	private mutantScriptSetup setup;

	private Transform thisTr;

	private FsmBool fsmFearBool;

	private FsmBool fsmDeadBool;

	private FsmBool fsmDownBool;

	private FsmBool fsmLookingForTarget;

	public float threatRemoveDist = 20f;

	public int defaultVisionRange = 50;

	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.thisTr = base.transform;
		if (this.setup.pmCombat)
		{
			this.fsmFearBool = this.setup.pmCombat.FsmVariables.GetFsmBool("fearBOOL");
		}
		if (this.setup.pmBrain)
		{
			this.fsmDeadBool = this.setup.pmBrain.FsmVariables.GetFsmBool("deadBool");
		}
		if (this.setup.pmCombat)
		{
			this.fsmDownBool = this.setup.pmCombat.FsmVariables.GetFsmBool("deathBool");
		}
		if (this.setup.pmVision)
		{
			this.fsmLookingForTarget = this.setup.pmVision.FsmVariables.GetFsmBool("lookingForTarget");
		}
		base.InvokeRepeating("sendRemoveAttacker", 1f, 4f);
		base.InvokeRepeating("sendAddVisibleTarget", 1f, 3f);
	}

	private void OnDespawned()
	{
		this.forceRemoveAttacker();
	}

	private void sendAddVisibleTarget()
	{
		if (this.setup.disableAiForDebug)
		{
			return;
		}
		if (!this.setup.ai.creepy && !this.setup.ai.creepy_male && !this.setup.ai.creepy_fat && this.setup.search.currentTarget)
		{
			if (this.setup.search.currentTarget.CompareTag("Player") && this.setup.ai.playerDist < 90f && !this.fsmLookingForTarget.Value)
			{
				this.setup.sceneInfo.addToVisible(base.transform.parent.gameObject);
			}
			else if (!this.setup.search.currentTarget.CompareTag("Player") || this.fsmLookingForTarget.Value)
			{
				this.setup.sceneInfo.removeFromVisible(base.transform.parent.gameObject);
			}
		}
	}

	private void sendAddAttacker()
	{
		if (this.setup.search.currentTarget && (this.setup.search.currentTarget.CompareTag("Player") || this.setup.search.currentTarget.CompareTag("PlayerNet") || this.setup.search.currentTarget.CompareTag("PlayerRemote")) && !this.setup.ai.creepy && !this.setup.ai.creepy_male)
		{
			LocalPlayer.TargetFunctions.addAttacker(this.setup.hashName);
		}
	}

	private void sendRemoveAttacker()
	{
		if (!this.setup.ai.creepy && !this.setup.ai.creepy_male && !this.setup.ai.creepy_fat && (this.setup.ai.playerDist > this.threatRemoveDist || this.fsmDeadBool.Value || this.fsmDownBool.Value))
		{
			LocalPlayer.TargetFunctions.removeAttacker(this.setup.hashName);
		}
	}

	public void forceRemoveAttacker()
	{
		if (!this.setup.ai.creepy && !this.setup.ai.creepy_male && !this.setup.ai.creepy_fat)
		{
			LocalPlayer.TargetFunctions.removeAttacker(this.setup.hashName);
		}
	}

	private void EnemyInLight(TargetTracker source)
	{
		if (source.gameObject.name == "Light")
		{
			this.animator.SetBoolReflected("fearBOOL", true);
			this.fsmFearBool.Value = true;
		}
	}

	private void EnemyOutOfLight(TargetTracker source)
	{
		if (source.gameObject.name == "Light")
		{
			this.fsmFearBool.Value = false;
			this.animator.SetBoolReflected("fearBOOL", false);
		}
	}

	private void PlayerNoiseDetected(TargetTracker source)
	{
		if (source.gameObject.CompareTag("playerBase"))
		{
			base.StartCoroutine("sendPlayerNoise");
			this.setup.lastSighting.transform.position = source.transform.position;
		}
	}

	private void PlayerNoiseStop(TargetTracker source)
	{
		if (source.gameObject.CompareTag("playerBase"))
		{
			base.StopCoroutine("sendPlayerNoise");
		}
	}

	[DebuggerHidden]
	private IEnumerator sendPlayerNoise()
	{
		mutantTargetFunctions.<sendPlayerNoise>c__IteratorA7 <sendPlayerNoise>c__IteratorA = new mutantTargetFunctions.<sendPlayerNoise>c__IteratorA7();
		<sendPlayerNoise>c__IteratorA.<>f__this = this;
		return <sendPlayerNoise>c__IteratorA;
	}

	[DebuggerHidden]
	private IEnumerator returnObjectAngle(Vector3 pos)
	{
		mutantTargetFunctions.<returnObjectAngle>c__IteratorA8 <returnObjectAngle>c__IteratorA = new mutantTargetFunctions.<returnObjectAngle>c__IteratorA8();
		<returnObjectAngle>c__IteratorA.pos = pos;
		<returnObjectAngle>c__IteratorA.<$>pos = pos;
		<returnObjectAngle>c__IteratorA.<>f__this = this;
		return <returnObjectAngle>c__IteratorA;
	}

	[DebuggerHidden]
	private IEnumerator returnTargetObjectAngle(GameObject go)
	{
		mutantTargetFunctions.<returnTargetObjectAngle>c__IteratorA9 <returnTargetObjectAngle>c__IteratorA = new mutantTargetFunctions.<returnTargetObjectAngle>c__IteratorA9();
		<returnTargetObjectAngle>c__IteratorA.go = go;
		<returnTargetObjectAngle>c__IteratorA.<$>go = go;
		<returnTargetObjectAngle>c__IteratorA.<>f__this = this;
		return <returnTargetObjectAngle>c__IteratorA;
	}

	[DebuggerHidden]
	private IEnumerator getTargetRunningAway()
	{
		mutantTargetFunctions.<getTargetRunningAway>c__IteratorAA <getTargetRunningAway>c__IteratorAA = new mutantTargetFunctions.<getTargetRunningAway>c__IteratorAA();
		<getTargetRunningAway>c__IteratorAA.<>f__this = this;
		return <getTargetRunningAway>c__IteratorAA;
	}
}
