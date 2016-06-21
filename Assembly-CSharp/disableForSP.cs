using System;
using UnityEngine;

public class disableForSP : MonoBehaviour
{
	private void Start()
	{
		if (!BoltNetwork.isRunning)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		if (!BoltNetwork.isRunning)
		{
			base.gameObject.SetActive(false);
		}
	}
}
