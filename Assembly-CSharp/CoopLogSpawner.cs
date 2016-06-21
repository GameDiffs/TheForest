using System;
using UnityEngine;

public class CoopLogSpawner : MonoBehaviour
{
	private void Start()
	{
		if (PlayerPrefs.GetInt("LOGSPAWN_ENABLED", 1) == 1)
		{
			BoltNetwork.Instantiate(BoltPrefabs.Log, base.transform.position, base.transform.rotation);
		}
	}
}
