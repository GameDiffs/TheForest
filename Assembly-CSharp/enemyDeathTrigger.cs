using HutongGames.PlayMaker;
using System;
using UnityEngine;

public class enemyDeathTrigger : MonoBehaviour
{
	private PlayMakerFSM pmControl;

	private FsmBool fsmDeathTrigger;

	private FsmGameObject fsmTargetGO;

	private GameObject player;

	private void OnDeserialized()
	{
		this.doStart();
	}

	private void Start()
	{
		this.doStart();
	}

	private void doStart()
	{
		this.player = GameObject.FindWithTag("Player");
		PlayMakerFSM[] componentsInChildren = this.player.transform.GetComponentsInChildren<PlayMakerFSM>();
		PlayMakerFSM[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			PlayMakerFSM playMakerFSM = array[i];
			if (playMakerFSM.FsmName == "controlFSM")
			{
				this.pmControl = playMakerFSM;
			}
		}
		this.fsmDeathTrigger = this.pmControl.FsmVariables.GetFsmBool("deathTrigger");
		this.fsmTargetGO = this.pmControl.FsmVariables.GetFsmGameObject("targetGO");
		base.gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.fsmDeathTrigger.Value = true;
			this.fsmTargetGO.Value = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.fsmDeathTrigger.Value = false;
		}
	}
}
