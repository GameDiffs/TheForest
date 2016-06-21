using System;
using UnityEngine;

public class setupPlayerCrawl : MonoBehaviour
{
	private void startCrawl()
	{
		base.transform.GetComponent<Animator>().SetBool("begin", true);
	}
}
