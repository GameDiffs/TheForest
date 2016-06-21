using HutongGames.PlayMaker;
using System;
using UnityEngine;

public class mutantMaleHashId : MonoBehaviour
{
	private PlayMakerFSM pm;

	private PlayMakerFSM pmCollide;

	private PlayMakerFSM pmTree;

	private PlayMakerFSM pmStalk;

	private mutantScriptSetup setup;

	public int attackTag;

	public int idleTag;

	public int runTag;

	public int deathTag;

	public int onRockTag;

	public int noMoveTag;

	private FsmInt attackTagFSM;

	private FsmInt idleTagFSM;

	private FsmInt runTagFSM;

	private FsmInt fearIdleFSM;

	private FsmInt onRockFSM;

	private void Start()
	{
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.attackTagFSM = this.setup.pmCombat.FsmVariables.GetFsmInt("HashAttack");
		this.idleTagFSM = this.setup.pmCombat.FsmVariables.GetFsmInt("HashIdle");
		this.runTagFSM = this.setup.pmCombat.FsmVariables.GetFsmInt("HashRun");
		this.fearIdleFSM = this.setup.pmCombat.FsmVariables.GetFsmInt("HashFearIdle");
		this.onRockFSM = this.setup.pmCombat.FsmVariables.GetFsmInt("HashOnRock");
		this.attackTagFSM.Value = Animator.StringToHash("attacking");
		this.idleTagFSM.Value = Animator.StringToHash("idle");
		this.runTagFSM.Value = Animator.StringToHash("running");
		this.fearIdleFSM.Value = Animator.StringToHash("fearIdle");
		this.onRockFSM.Value = Animator.StringToHash("onRock");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashStagger").Value = Animator.StringToHash("stagger");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashDamaged").Value = Animator.StringToHash("damaged");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashDeath").Value = Animator.StringToHash("death");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashLanding").Value = Animator.StringToHash("landing");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashInTree").Value = Animator.StringToHash("inTree");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashJumpFall").Value = Animator.StringToHash("jumpFall");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashInTreeMir").Value = Animator.StringToHash("inTreeMir");
		if (this.setup.pmSearch)
		{
			this.setup.pmSearch.FsmVariables.GetFsmInt("HashOnRock").Value = Animator.StringToHash("onRock");
			this.setup.pmSearch.FsmVariables.GetFsmInt("HashAttack").Value = Animator.StringToHash("attacking");
		}
		if (this.setup.pmEncounter)
		{
			this.setup.pmEncounter.FsmVariables.GetFsmInt("hashRunTrap").Value = Animator.StringToHash("runTrap");
			this.setup.pmEncounter.FsmVariables.GetFsmInt("hashInTrap").Value = Animator.StringToHash("inTrap");
		}
		this.attackTag = this.attackTagFSM.Value;
		this.idleTag = this.idleTagFSM.Value;
		this.runTag = this.runTagFSM.Value;
		this.deathTag = Animator.StringToHash("death");
		this.onRockTag = this.onRockFSM.Value;
		this.noMoveTag = Animator.StringToHash("noMove");
	}
}
