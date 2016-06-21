using System;
using UnityEngine;

public class quickBuild : MonoBehaviour
{
	public GameObject buildThis;

	private void Start()
	{
		base.Invoke("doFastBuild", 0f);
	}

	private void doFastBuild()
	{
		if (BoltNetwork.isRunning)
		{
			BoltNetwork.Instantiate(this.buildThis, base.transform.position, base.transform.rotation);
		}
		else
		{
			UnityEngine.Object.Instantiate(this.buildThis, base.transform.position, base.transform.rotation);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
