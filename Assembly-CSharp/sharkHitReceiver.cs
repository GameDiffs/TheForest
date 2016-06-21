using System;
using UnityEngine;

public class sharkHitReceiver : MonoBehaviour
{
	private Fish fishScript;

	private void Start()
	{
		this.fishScript = base.transform.root.GetComponent<Fish>();
	}

	private void Hit(int d)
	{
		this.fishScript.Hit(d);
	}
}
