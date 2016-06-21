using System;
using UnityEngine;

public class yachtAnimSetup : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("animSetup", 2f);
	}

	private void OnEnable()
	{
		this.animSetup();
	}

	private void animSetup()
	{
		base.transform.GetComponent<Animation>()["yachtWobble"].wrapMode = WrapMode.Loop;
		base.transform.GetComponent<Animation>()["yachtWobble"].speed = 1f;
		base.transform.GetComponent<Animation>().Play("yachtWobble", PlayMode.StopAll);
	}
}
