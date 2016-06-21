using System;
using UnityEngine;

public class spawnEncounter : MonoBehaviour
{
	public float delay;

	public bool hideMeshes;

	public bool parentToGo;

	public GameObject EncounterGo;

	public Transform orientTr;

	private GameObject spawnedEncounter;

	private enableRandomGo setMeshes;

	private void Start()
	{
		if (this.delay > 0f)
		{
			base.Invoke("spawnNewEncounter", this.delay);
		}
	}

	private void OnEnable()
	{
		if (this.delay > 0f)
		{
			base.Invoke("spawnNewEncounter", this.delay);
		}
	}

	private void spawnNewEncounter()
	{
		if (!this.spawnedEncounter)
		{
			this.spawnedEncounter = (UnityEngine.Object.Instantiate(this.EncounterGo, this.orientTr.position, base.transform.rotation) as GameObject);
			if (this.hideMeshes)
			{
				this.spawnedEncounter.SendMessage("removeDummyGeo", SendMessageOptions.DontRequireReceiver);
			}
			if (this.parentToGo)
			{
				this.spawnedEncounter.transform.parent = base.transform;
			}
		}
	}

	private void OnDestroy()
	{
		if (this.spawnedEncounter)
		{
			UnityEngine.Object.Destroy(this.spawnedEncounter);
		}
	}

	private void OnDisable()
	{
		if (this.spawnedEncounter)
		{
			UnityEngine.Object.Destroy(this.spawnedEncounter);
		}
	}
}
