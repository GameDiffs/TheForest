using System;
using UnityEngine;

public class CaveBlockingMask : MonoBehaviour
{
	private void Update()
	{
		if (Clock.InCave)
		{
			base.gameObject.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			base.gameObject.GetComponent<Renderer>().enabled = false;
		}
	}
}
