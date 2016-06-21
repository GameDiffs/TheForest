using System;
using UnityEngine;

public class trapReceiver : MonoBehaviour
{
	private trapTrigger trigger;

	private void Start()
	{
		this.trigger = base.GetComponentInChildren<trapTrigger>();
	}

	private void enableTrapReset()
	{
		this.trigger.enableTrapReset();
	}

	private void setTrapDummy(GameObject go)
	{
		this.trigger.dummyGo = go;
	}

	private void releaseNooseTrap()
	{
		this.trigger.releaseNooseTrap();
	}

	private void releaseNooseTrapMP()
	{
		this.trigger.releaseNooseTrapMP();
	}
}
