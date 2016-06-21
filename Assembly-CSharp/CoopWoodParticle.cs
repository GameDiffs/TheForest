using Bolt;
using System;
using UnityEngine;

public class CoopWoodParticle : MonoBehaviour
{
	private void Start()
	{
		if (BoltNetwork.isRunning)
		{
			BoltEntity component = GameObject.FindGameObjectWithTag("Player").GetComponent<BoltEntity>();
			SpawnWoodParticle spawnWoodParticle = SpawnWoodParticle.Raise(GlobalTargets.Others, ReliabilityModes.Unreliable);
			spawnWoodParticle.Position = base.transform.position;
			spawnWoodParticle.Send();
		}
	}
}
