using System;
using TheForest.Utils;
using UnityEngine;

public class sendEnemyRunAway : MonoBehaviour
{
	public float startDelay;

	public float repeatDelay;

	public float maxDistance;

	private void Start()
	{
		this.startDelay += Time.time;
	}

	private void Update()
	{
		if (Time.time > this.startDelay && Time.time > this.repeatDelay)
		{
			this.sendRunaway();
			this.repeatDelay = Time.time + this.repeatDelay;
		}
	}

	private void sendRunaway()
	{
		for (int i = 0; i < Scene.MutantControler.activeCannibals.Count; i++)
		{
			if (Scene.MutantControler.activeCannibals[i] && Vector3.Distance(Scene.MutantControler.activeCannibals[i].transform.position, base.transform.position) < this.maxDistance)
			{
				Scene.MutantControler.activeCannibals[i].SendMessage("switchToRunAway", base.gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		for (int j = 0; j < Scene.MutantControler.activeInstantSpawnedCannibals.Count; j++)
		{
			if (Scene.MutantControler.activeInstantSpawnedCannibals[j] && Vector3.Distance(Scene.MutantControler.activeInstantSpawnedCannibals[j].transform.position, base.transform.position) < this.maxDistance)
			{
				Scene.MutantControler.activeInstantSpawnedCannibals[j].SendMessage("switchToRunAway", base.gameObject, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
