using Bolt;
using System;
using UnityEngine;

public class coopDeadSharkCutHead : MonoBehaviour
{
	public BoltEntity ent;

	private void Start()
	{
		if (this.ent && BoltNetwork.isClient)
		{
			this.ent.transform.SendMessage("generateStorePrefab", SendMessageOptions.DontRequireReceiver);
			deadSharkCutHead deadSharkCutHead = deadSharkCutHead.Create(GlobalTargets.OnlyServer);
			deadSharkCutHead.target = this.ent;
			deadSharkCutHead.Send();
		}
	}

	public void syncRagDollForServer(GameObject go)
	{
		if (!BoltNetwork.isClient)
		{
			this.ent.transform.SendMessage("sendSyncRagDoll", go, SendMessageOptions.DontRequireReceiver);
		}
	}
}
