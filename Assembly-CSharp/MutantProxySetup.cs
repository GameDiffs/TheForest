using System;
using UnityEngine;

public class MutantProxySetup : MonoBehaviour
{
	public GameObject[] ObjectsToDestroy;

	public MonoBehaviour[] ScriptsToDestroy;

	public GameObject Base;

	private void Start()
	{
		UnityEngine.Object.Destroy(this);
	}

	private void SendPosition()
	{
	}
}
