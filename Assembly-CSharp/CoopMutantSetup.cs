using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class CoopMutantSetup : EntityEventListener<IMutantState>
{
	public bool creepy;

	public GameObject BloodPos;

	public GameObject BloodSplat;

	public Transform nooseTrapPivot;

	public GameObject nooseTrapGo;

	public clientNooseTrapFixer nooseFixer;

	public Transform rootTr;

	private mutantRagdollSetup mrs;

	public GameObject storePrefab;

	public int storedRagDollName;

	public Transform[] storedCreepyJoints;

	private void Start()
	{
		this.mrs = base.transform.GetComponentInChildren<mutantRagdollSetup>();
	}

	public override void Detached()
	{
		if (this.creepy && BoltNetwork.isClient)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.storePrefab, base.transform.position, base.transform.rotation) as GameObject;
			storeLocalMutantInfo2 component = gameObject.GetComponent<storeLocalMutantInfo2>();
			Scene.SceneTracker.storedRagDollPrefabs.Add(gameObject);
			component.identifier = this.storedRagDollName;
			component.rootRotation = this.rootTr.rotation;
			component.rootPosition = this.rootTr.position;
			for (int i = 0; i < this.storedCreepyJoints.Length; i++)
			{
				component.jointAngles.Add(this.storedCreepyJoints[i].localRotation);
			}
		}
	}

	public void getRagDollName(int name)
	{
		this.storedRagDollName = name;
	}

	public override void OnEvent(FxEnemeyBlood evnt)
	{
		this.BloodActual();
	}

	public override void Attached()
	{
		if (BoltNetwork.isClient)
		{
			storeRagDollName storeRagDollName = storeRagDollName.Create(GlobalTargets.Everyone);
			storeRagDollName.Target = base.transform.GetComponent<BoltEntity>();
			storeRagDollName.name = (int)(UnityEngine.Random.value * 1E+07f);
			storeRagDollName.Send();
		}
	}

	private void enableNetRagDoll()
	{
		this.mrs.setupRagDollParts(true);
	}

	private void setClientNoosePivot(Transform tr)
	{
		this.nooseTrapPivot = tr;
	}

	private void setClientTrigger(GameObject go)
	{
		this.nooseTrapGo = go;
		this.nooseFixer = go.GetComponent<clientNooseTrapFixer>();
	}

	private void BloodActual()
	{
		if (this.BloodSplat && this.BloodPos)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.BloodSplat, this.BloodPos.transform.position, this.BloodPos.transform.rotation) as GameObject;
			gameObject.transform.parent = this.BloodPos.transform;
			UnityEngine.Object.Destroy(gameObject, 0.5f);
		}
	}
}
