using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantWorldSearchFunctions : MonoBehaviour
{
	private sceneTracker sceneInfo;

	private mutantScriptSetup setup;

	private mutantSearchFunctions searchFunctions;

	public groupEncounterSetup ge;

	private mutantAI ai;

	private Transform tr;

	public GameObject encounterGo;

	public GameObject encounterPosGo;

	private RaycastHit hit;

	private bool search;

	private Vector3 pos;

	private Vector2 randomPoint;

	private void Awake()
	{
		this.sceneInfo = Scene.SceneTracker;
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.ai = base.GetComponent<mutantAI>();
		this.searchFunctions = base.GetComponent<mutantSearchFunctions>();
		this.tr = base.transform;
	}

	private void OnEnable()
	{
		this.resetEncounterCoolDown();
	}

	[DebuggerHidden]
	private IEnumerator findClosestEntrance()
	{
		mutantWorldSearchFunctions.<findClosestEntrance>c__IteratorBC <findClosestEntrance>c__IteratorBC = new mutantWorldSearchFunctions.<findClosestEntrance>c__IteratorBC();
		<findClosestEntrance>c__IteratorBC.<>f__this = this;
		return <findClosestEntrance>c__IteratorBC;
	}

	[DebuggerHidden]
	private IEnumerator findClosestFeederToPlayer()
	{
		mutantWorldSearchFunctions.<findClosestFeederToPlayer>c__IteratorBD <findClosestFeederToPlayer>c__IteratorBD = new mutantWorldSearchFunctions.<findClosestFeederToPlayer>c__IteratorBD();
		<findClosestFeederToPlayer>c__IteratorBD.<>f__this = this;
		return <findClosestFeederToPlayer>c__IteratorBD;
	}

	private void findCloseGroupEncounter()
	{
		if (this.sceneInfo.encounters.Count < 1)
		{
			this.setup.pmSearch.SendEvent("noValidTarget");
			return;
		}
		GameObject gameObject = this.sceneInfo.encounters[UnityEngine.Random.Range(0, this.sceneInfo.encounters.Count)];
		if (Vector3.Distance(this.tr.position, gameObject.transform.position) > 350f)
		{
			this.setup.pmSearch.SendEvent("noValidTarget");
			return;
		}
		if (!gameObject.activeSelf)
		{
			this.setup.pmSearch.SendEvent("noValidTarget");
			return;
		}
		this.getEncounterComponent(gameObject);
		if (!this.ge.occupied && !this.ge.encounterCoolDown)
		{
			this.encounterGo = gameObject;
			this.setup.pmEncounter.FsmVariables.GetFsmGameObject("encounterGo").Value = gameObject;
			this.setEncounterType();
			this.ge.occupied = true;
			this.ge.enableEncounterCoolDown();
			this.setup.search.currentTarget = gameObject;
			this.setup.pmSearch.SendEvent("toEncounter");
		}
	}

	[DebuggerHidden]
	private IEnumerator findPointAroundEncounter(float dist)
	{
		mutantWorldSearchFunctions.<findPointAroundEncounter>c__IteratorBE <findPointAroundEncounter>c__IteratorBE = new mutantWorldSearchFunctions.<findPointAroundEncounter>c__IteratorBE();
		<findPointAroundEncounter>c__IteratorBE.dist = dist;
		<findPointAroundEncounter>c__IteratorBE.<$>dist = dist;
		<findPointAroundEncounter>c__IteratorBE.<>f__this = this;
		return <findPointAroundEncounter>c__IteratorBE;
	}

	public void getEncounterComponent(GameObject go)
	{
		this.ge = go.GetComponent<groupEncounterSetup>();
	}

	public void setEncounterType()
	{
		this.setup.pmEncounter.enabled = true;
		if (this.ge.typeRitual1)
		{
			this.setup.pmEncounter.FsmVariables.GetFsmBool("boolRitual1").Value = true;
		}
		if (this.ge.typeRitual2)
		{
			this.setup.pmEncounter.FsmVariables.GetFsmBool("boolRitual2").Value = true;
		}
		if (this.ge.typeFeeding1)
		{
			this.setup.pmEncounter.FsmVariables.GetFsmBool("boolFeeding1").Value = true;
		}
		if (this.ge.typeMourning1)
		{
			this.setup.pmEncounter.FsmVariables.GetFsmBool("boolMourning1").Value = true;
			this.ge.forceEnableAnimator();
		}
		this.ge.enableEncounterCoolDown();
		this.setup.pmSearch.FsmVariables.GetFsmBool("encounterCoolDown").Value = true;
		base.Invoke("resetEncounterCoolDown", 300f);
		this.setup.pmEncounter.SendEvent("toActivateFSM");
	}

	private void resetEncounterCoolDown()
	{
		if (!this.setup)
		{
			return;
		}
		if (this.setup.pmSearch)
		{
			this.setup.pmSearch.FsmVariables.GetFsmBool("encounterCoolDown").Value = false;
		}
	}

	private void getLookatGo()
	{
		this.setup.pmEncounter.FsmVariables.GetFsmGameObject("lookatGo").Value = this.ge.lookatGo;
	}

	[DebuggerHidden]
	private IEnumerator getLeaderPos()
	{
		mutantWorldSearchFunctions.<getLeaderPos>c__IteratorBF <getLeaderPos>c__IteratorBF = new mutantWorldSearchFunctions.<getLeaderPos>c__IteratorBF();
		<getLeaderPos>c__IteratorBF.<>f__this = this;
		return <getLeaderPos>c__IteratorBF;
	}

	[DebuggerHidden]
	private IEnumerator getFollowerPos()
	{
		mutantWorldSearchFunctions.<getFollowerPos>c__IteratorC0 <getFollowerPos>c__IteratorC = new mutantWorldSearchFunctions.<getFollowerPos>c__IteratorC0();
		<getFollowerPos>c__IteratorC.<>f__this = this;
		return <getFollowerPos>c__IteratorC;
	}

	private void removeEncounterPos()
	{
		if (this.ge)
		{
			this.ge.leaderPosFull = false;
			this.ge.followerPosFull1 = false;
			this.ge.followerPosFull2 = false;
			this.ge.followerPosFull3 = false;
			this.ge.followerPosFull4 = false;
			this.ge.occupied = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator gibCurrentFeeder(GameObject go)
	{
		mutantWorldSearchFunctions.<gibCurrentFeeder>c__IteratorC1 <gibCurrentFeeder>c__IteratorC = new mutantWorldSearchFunctions.<gibCurrentFeeder>c__IteratorC1();
		<gibCurrentFeeder>c__IteratorC.go = go;
		<gibCurrentFeeder>c__IteratorC.<$>go = go;
		return <gibCurrentFeeder>c__IteratorC;
	}

	[DebuggerHidden]
	private IEnumerator gibCurrentMutant(GameObject go)
	{
		mutantWorldSearchFunctions.<gibCurrentMutant>c__IteratorC2 <gibCurrentMutant>c__IteratorC = new mutantWorldSearchFunctions.<gibCurrentMutant>c__IteratorC2();
		<gibCurrentMutant>c__IteratorC.go = go;
		<gibCurrentMutant>c__IteratorC.<$>go = go;
		return <gibCurrentMutant>c__IteratorC;
	}

	[DebuggerHidden]
	private IEnumerator sendFeedingEffect(GameObject go)
	{
		mutantWorldSearchFunctions.<sendFeedingEffect>c__IteratorC3 <sendFeedingEffect>c__IteratorC = new mutantWorldSearchFunctions.<sendFeedingEffect>c__IteratorC3();
		<sendFeedingEffect>c__IteratorC.go = go;
		<sendFeedingEffect>c__IteratorC.<$>go = go;
		return <sendFeedingEffect>c__IteratorC;
	}

	[DebuggerHidden]
	public IEnumerator stopFeedingEffect(GameObject go)
	{
		mutantWorldSearchFunctions.<stopFeedingEffect>c__IteratorC4 <stopFeedingEffect>c__IteratorC = new mutantWorldSearchFunctions.<stopFeedingEffect>c__IteratorC4();
		<stopFeedingEffect>c__IteratorC.go = go;
		<stopFeedingEffect>c__IteratorC.<$>go = go;
		return <stopFeedingEffect>c__IteratorC;
	}

	public Vector2 Circle2(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	[DebuggerHidden]
	private IEnumerator moveToEncounter()
	{
		mutantWorldSearchFunctions.<moveToEncounter>c__IteratorC5 <moveToEncounter>c__IteratorC = new mutantWorldSearchFunctions.<moveToEncounter>c__IteratorC5();
		<moveToEncounter>c__IteratorC.<>f__this = this;
		return <moveToEncounter>c__IteratorC;
	}

	[DebuggerHidden]
	private IEnumerator resetMoveToEncounter()
	{
		mutantWorldSearchFunctions.<resetMoveToEncounter>c__IteratorC6 <resetMoveToEncounter>c__IteratorC = new mutantWorldSearchFunctions.<resetMoveToEncounter>c__IteratorC6();
		<resetMoveToEncounter>c__IteratorC.<>f__this = this;
		return <resetMoveToEncounter>c__IteratorC;
	}

	private void resetMourningAnimator()
	{
		if (!this.setup)
		{
			return;
		}
		if (this.setup.ai.leader && this.ge && this.ge.lookatGo)
		{
			Animator component = this.ge.lookatGo.GetComponent<Animator>();
			if (component)
			{
				component.SetBoolReflected("encounterBOOL", false);
			}
		}
		this.setup.animControl.forceIkBool = false;
	}

	private void setIkToEncounter()
	{
		if (this.encounterGo)
		{
			this.setup.search.currentTarget = this.encounterGo;
		}
		this.setup.animControl.forceIkBool = true;
	}
}
