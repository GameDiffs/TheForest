using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class PlaneCrashController : MonoBehaviour
{
	public GameObject savedHullPrefab;

	[HideInInspector]
	public Transform savePos;

	[SerializeThis]
	public bool Crashed;

	public bool ShowCrash;

	public bool fakePlaneActive;

	public bool doneHidePlayer;

	private void Start()
	{
		Scene.PlaneCrash = this;
	}

	[DebuggerHidden]
	public IEnumerator InitPlaneCrashSequence()
	{
		PlaneCrashController.<InitPlaneCrashSequence>c__Iterator183 <InitPlaneCrashSequence>c__Iterator = new PlaneCrashController.<InitPlaneCrashSequence>c__Iterator183();
		<InitPlaneCrashSequence>c__Iterator.<>f__this = this;
		return <InitPlaneCrashSequence>c__Iterator;
	}

	private void OnDeserialized()
	{
		base.Invoke("setupCrashedPlane", 0.3f);
	}

	private void setupCrashedPlane()
	{
		this.setupCrashedPlane(GameObject.FindGameObjectWithTag("savePlanePos").transform);
	}

	public void SetupCrashedPlane_MP()
	{
		Transform transform = new GameObject("MPHostPlanePos").transform;
		transform.position = CoopServerInfo.Instance.state.PlanePosition;
		transform.rotation = CoopServerInfo.Instance.state.PlaneRotation;
		this.setupCrashedPlane(transform);
	}

	private void setupCrashedPlane(Transform t)
	{
		if (Scene.PlaneCrashAnimGO)
		{
			UnityEngine.Object.Destroy(Scene.PlaneCrashAnimGO);
		}
		this.savePos = t;
		if (this.savePos)
		{
			base.Invoke("loadCrashPlane", 0.15f);
			UnityEngine.Debug.Log("fake plane loaded");
		}
	}

	private void loadCrashPlane()
	{
		UnityEngine.Object.Instantiate(this.savedHullPrefab, this.savePos.position, this.savePos.rotation);
		if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.PlaneCrash)
		{
			LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.Loading;
		}
		this.fakePlaneActive = true;
		Scene.PlaneGreebles.transform.position = this.savePos.position;
		Scene.PlaneGreebles.transform.rotation = this.savePos.rotation;
		Scene.PlaneGreebles.SetActive(true);
		if (!BoltNetwork.isRunning || BoltNetwork.isClient)
		{
		}
	}
}
