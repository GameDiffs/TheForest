using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class getAnimatorParams : MonoBehaviour
{
	public class DummyParams
	{
		public Quaternion Angle;

		public bool DiedLayingDown;
	}

	private CoopMutantSetup cms;

	private mutantRagdollSetup mrs;

	private Animator animator;

	private EnemyHealth health;

	private mutantScriptSetup setup;

	private setupBodyVariation bodyVar;

	public GameObject dummyMutant;

	public Transform hips;

	public Transform dummyHips;

	public Transform mutantBase;

	private MaterialPropertyBlock bloodPropertyBlock;

	private void Awake()
	{
		this.bloodPropertyBlock = new MaterialPropertyBlock();
		this.cms = base.transform.GetComponent<CoopMutantSetup>();
		this.mrs = base.transform.GetComponentInChildren<mutantRagdollSetup>();
		this.animator = base.transform.GetComponentInChildren<Animator>();
		this.health = base.transform.GetComponentInChildren<EnemyHealth>();
		this.setup = base.transform.GetComponentInChildren<mutantScriptSetup>();
		this.bodyVar = base.transform.GetComponentInChildren<setupBodyVariation>();
	}

	[DebuggerHidden]
	public IEnumerator spawnDummy(getAnimatorParams.DummyParams p)
	{
		getAnimatorParams.<spawnDummy>c__Iterator1CB <spawnDummy>c__Iterator1CB = new getAnimatorParams.<spawnDummy>c__Iterator1CB();
		<spawnDummy>c__Iterator1CB.p = p;
		<spawnDummy>c__Iterator1CB.<$>p = p;
		<spawnDummy>c__Iterator1CB.<>f__this = this;
		return <spawnDummy>c__Iterator1CB;
	}

	[DebuggerHidden]
	private IEnumerator fixHipPosition(Transform d)
	{
		getAnimatorParams.<fixHipPosition>c__Iterator1CC <fixHipPosition>c__Iterator1CC = new getAnimatorParams.<fixHipPosition>c__Iterator1CC();
		<fixHipPosition>c__Iterator1CC.d = d;
		<fixHipPosition>c__Iterator1CC.<$>d = d;
		<fixHipPosition>c__Iterator1CC.<>f__this = this;
		return <fixHipPosition>c__Iterator1CC;
	}
}
