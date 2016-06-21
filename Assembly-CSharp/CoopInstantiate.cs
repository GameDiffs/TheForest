using System;
using UnityEngine;

public class CoopInstantiate : MonoBehaviour
{
	public bool InMP;

	public bool InSP;

	public bool NotOnServer;

	public bool NotOnClient = true;

	public GameObject Prefab;

	private void Start()
	{
		if (BoltNetwork.isRunning)
		{
			if (this.InMP)
			{
				if (BoltNetwork.isServer && !this.NotOnServer)
				{
					BoltNetwork.Instantiate(this.Prefab, base.transform.position, base.transform.rotation);
				}
				if (BoltNetwork.isClient && !this.NotOnClient)
				{
					BoltNetwork.Instantiate(this.Prefab, base.transform.position, base.transform.rotation);
				}
			}
		}
		else if (this.InSP)
		{
			UnityEngine.Object.Instantiate(this.Prefab, base.transform.position, base.transform.rotation);
		}
	}
}
