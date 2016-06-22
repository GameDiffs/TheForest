using System;
using UnityEngine;

[Serializable]
public class BuildChecker : MonoBehaviour
{
	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Con"))
		{
			otherObject.SendMessage("LockIn");
		}
	}

	public override void OnTriggerExit(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Con"))
		{
			otherObject.SendMessage("UnLock");
		}
	}

	public override void Main()
	{
	}
}
