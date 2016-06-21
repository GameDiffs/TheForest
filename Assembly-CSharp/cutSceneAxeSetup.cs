using System;
using TheForest.Utils;
using UnityEngine;

public class cutSceneAxeSetup : MonoBehaviour
{
	public GameObject skull;

	private void OnDisable()
	{
		LocalPlayer.Stats.equipCutSceneAxe();
		this.skull.GetComponent<Rigidbody>().isKinematic = false;
		this.skull.GetComponent<Rigidbody>().useGravity = true;
	}
}
