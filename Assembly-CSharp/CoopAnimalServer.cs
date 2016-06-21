using Bolt;
using System;
using UnityEngine;

public class CoopAnimalServer : CoopBase<IAnimalState>
{
	public GameObject NetworkContainerPrefab;

	public bool NonPooled;

	private void Start()
	{
		if (BoltNetwork.isRunning && !base.GetComponent<BoltEntity>())
		{
			SpawnBunny spawnBunny = SpawnBunny.Create(GlobalTargets.OnlyServer);
			spawnBunny.Pos = base.transform.position;
			spawnBunny.Rot = base.transform.rotation;
			spawnBunny.Send();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public override void Attached()
	{
		CoopAnimal componentInChildren = base.GetComponentInChildren<CoopAnimal>();
		if (componentInChildren.rotationTransform)
		{
			base.state.TransformPosition.SetTransforms(base.transform);
			base.state.TransformRotation.SetTransforms(componentInChildren.rotationTransform);
		}
		else
		{
			base.state.TransformFull.SetTransforms(base.transform);
		}
	}
}
