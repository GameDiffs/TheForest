using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class mutantRagdollSetup : MonoBehaviour
{
	private CoopMutantSetup cms;

	public Transform[] jointsToSync;

	public GameObject[] legParts;

	public GameObject[] armParts;

	public GameObject[] headParts;

	public float enableLegsTime;

	public float enableArmsTime;

	public float enableHeadTime;

	public GameObject storePrefab;

	private bool doneGenerateList;

	private void Start()
	{
		this.cms = base.transform.parent.GetComponent<CoopMutantSetup>();
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.enableRagDollParts(this.legParts, this.enableLegsTime, false));
		base.StartCoroutine(this.enableRagDollParts(this.armParts, this.enableArmsTime, false));
		base.StartCoroutine(this.enableRagDollParts(this.headParts, this.enableHeadTime, false));
		this.doneGenerateList = false;
	}

	public void setupRagDollParts(bool onoff)
	{
		base.StartCoroutine(this.enableRagDollParts(this.legParts, this.enableLegsTime, onoff));
		base.StartCoroutine(this.enableRagDollParts(this.armParts, this.enableArmsTime, onoff));
		base.StartCoroutine(this.enableRagDollParts(this.headParts, this.enableHeadTime, onoff));
	}

	[DebuggerHidden]
	public IEnumerator generateStoredJointList()
	{
		mutantRagdollSetup.<generateStoredJointList>c__Iterator81 <generateStoredJointList>c__Iterator = new mutantRagdollSetup.<generateStoredJointList>c__Iterator81();
		<generateStoredJointList>c__Iterator.<>f__this = this;
		return <generateStoredJointList>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator enableRagDollParts(GameObject[] parts, float enableDelay, bool onoff)
	{
		mutantRagdollSetup.<enableRagDollParts>c__Iterator82 <enableRagDollParts>c__Iterator = new mutantRagdollSetup.<enableRagDollParts>c__Iterator82();
		<enableRagDollParts>c__Iterator.onoff = onoff;
		<enableRagDollParts>c__Iterator.enableDelay = enableDelay;
		<enableRagDollParts>c__Iterator.parts = parts;
		<enableRagDollParts>c__Iterator.<$>onoff = onoff;
		<enableRagDollParts>c__Iterator.<$>enableDelay = enableDelay;
		<enableRagDollParts>c__Iterator.<$>parts = parts;
		return <enableRagDollParts>c__Iterator;
	}
}
